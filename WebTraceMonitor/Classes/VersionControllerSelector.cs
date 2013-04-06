using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace WebTraceMonitor.Classes
{
    public class VersionControllerSelector : IHttpControllerSelector
    {
        private struct ControllerIdentification : IEquatable<ControllerIdentification>
        {
            private static readonly Lazy<IEqualityComparer<ControllerIdentification>> ComparerInstance = new Lazy<IEqualityComparer<ControllerIdentification>>(() => new ControllerNameComparer());


            public static IEqualityComparer<ControllerIdentification> Comparer
            {
                get { return ComparerInstance.Value; }
            }

            public string Name { get; set; }
            public int? Version { get; set; }
            
            public ControllerIdentification(string name, int? version)
                : this()
            {
                this.Name = name;
                this.Version = version;
            }

            public bool Equals(ControllerIdentification other)
            {
                return StringComparer.InvariantCultureIgnoreCase.Equals(other.Name, this.Name) &&
                       other.Version == this.Version;
            }


            public override bool Equals(object obj)
            {
                if (obj is ControllerIdentification)
                {
                    var cn = (ControllerIdentification)obj;
                    return this.Equals(cn);
                }


                return false;
            }


            public override int GetHashCode()
            {
                return this.ToString().ToUpperInvariant().GetHashCode();
            }


            public override string ToString()
            {
                if (this.Version == null)
                {
                    return this.Name;
                }


                return VersionControllerSelector.VersionPrefix + this.Version.Value.ToString(CultureInfo.InvariantCulture) + "." + this.Name;
            }


            private class ControllerNameComparer : IEqualityComparer<ControllerIdentification>
            {
                public bool Equals(ControllerIdentification x, ControllerIdentification y)
                {
                    return x.Equals(y);
                }


                public int GetHashCode(ControllerIdentification obj)
                {
                    return obj.GetHashCode();
                }
            }
        }

        private class HttpControllerTypeCache
        {
            private readonly Lazy<Dictionary<ControllerIdentification, ILookup<string, Type>>> _cache;
            private readonly HttpConfiguration _configuration;


            internal Dictionary<ControllerIdentification, ILookup<string, Type>> Cache
            {
                get { return this._cache.Value; }
            }


            public HttpControllerTypeCache(HttpConfiguration configuration)
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException("configuration");
                }


                this._configuration = configuration;
                this._cache = new Lazy<Dictionary<ControllerIdentification, ILookup<string, Type>>>(this.InitializeCache);
            }


            public ICollection<Type> GetControllerTypes(ControllerIdentification controllerName)
            {
                if (String.IsNullOrEmpty(controllerName.Name))
                {
                    throw new ArgumentNullException("controllerName");
                }


                var matchingTypes = new HashSet<Type>();


                ILookup<string, Type> namespaceLookup;
                if (this._cache.Value.TryGetValue(controllerName, out namespaceLookup))
                {
                    foreach (var namespaceGroup in namespaceLookup)
                    {
                        matchingTypes.UnionWith(namespaceGroup);
                    }
                }


                return matchingTypes;
            }


            private Dictionary<ControllerIdentification, ILookup<string, Type>> InitializeCache()
            {
                IAssembliesResolver assembliesResolver = this._configuration.Services.GetAssembliesResolver();
                IHttpControllerTypeResolver controllersResolver = this._configuration.Services.GetHttpControllerTypeResolver();


                ICollection<Type> controllerTypes = controllersResolver.GetControllerTypes(assembliesResolver);
                IEnumerable<IGrouping<ControllerIdentification, Type>> groupedByName = controllerTypes.GroupBy(
                                                                                                     GetControllerName,
                                                                                                     ControllerIdentification.Comparer);


                return groupedByName.ToDictionary(
                                                  g => g.Key,
                                                  g => g.ToLookup(t => t.Namespace ?? String.Empty, StringComparer.OrdinalIgnoreCase),
                                                  ControllerIdentification.Comparer);
            }


            private static ControllerIdentification GetControllerName(Type type)
            {
                string fullName = type.FullName;
                Debug.Assert(fullName != null);


                fullName = fullName.Substring(0, fullName.Length - DefaultHttpControllerSelector.ControllerSuffix.Length);

                string[] nameSplit = fullName.Split('.');


                string name = nameSplit[nameSplit.Length - 1]; // same as Type.Name
                int? version = null;


                for (int i = nameSplit.Length - 2; i >= 0; i--)
                {
                    string namePart = nameSplit[i];
                    if (!namePart.StartsWith(VersionControllerSelector.VersionPrefix, StringComparison.InvariantCultureIgnoreCase))
                    {
                        continue;
                    }


                    string versionNumberStr = namePart.Substring(VersionControllerSelector.VersionPrefix.Length);
                    int versionNumber;
                    if (Int32.TryParse(versionNumberStr, NumberStyles.None, CultureInfo.InvariantCulture, out versionNumber))
                    {
                        version = versionNumber;
                        break;
                    }
                }


                return new ControllerIdentification(name, version);
            }
        }

        private const string VersionKey = "version";
        private const string VersionPrefix = "Version";
        private const string ControllerKey = "controller";

        private readonly HttpConfiguration _configuration;
        private readonly Lazy<ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor>> _controllerInfoCache;
        private readonly HttpControllerTypeCache _controllerTypeCache;


        public VersionControllerSelector(HttpConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration");
            }


            this._controllerInfoCache =
                    new Lazy<ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor>>(this.InitializeControllerInfoCache);
            this._configuration = configuration;
            this._controllerTypeCache = new HttpControllerTypeCache(this._configuration);
        }




        #region IHttpControllerSelector Members
        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }


            ControllerIdentification controllerName = GetControllerIdentificationFromRequest(request);
            if (String.IsNullOrEmpty(controllerName.Name))
            {
                throw new HttpResponseException(request.CreateResponse(HttpStatusCode.NotFound));
            }


            HttpControllerDescriptor controllerDescriptor;
            if (this._controllerInfoCache.Value.TryGetValue(controllerName, out controllerDescriptor))
            {
                return controllerDescriptor;
            }


            ICollection<Type> matchingTypes = this._controllerTypeCache.GetControllerTypes(controllerName);


            // ControllerInfoCache is already initialized.
            Contract.Assert(matchingTypes.Count != 1);


            if (matchingTypes.Count == 0)
            {
                // no matching types
                throw new HttpResponseException(request.CreateResponse(
                                                                       HttpStatusCode.NotFound,
                                                                       "The API '" + controllerName + "' doesn't exist"));
            }


            // multiple matching types
            throw new HttpResponseException(request.CreateResponse(
                                                                   HttpStatusCode.InternalServerError,
                                                                   CreateAmbiguousControllerExceptionMessage(request.GetRouteData().Route,
                                                                                                             controllerName.Name,
                                                                                                             matchingTypes)));
        }


        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return this._controllerInfoCache.Value.ToDictionary(c => VersionPrefix + c.Key.Version + "." + c.Key.Name, c => c.Value, StringComparer.OrdinalIgnoreCase);
        }


        #endregion

        protected string GetControllerNameFromRequest(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }


            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                return default(String);
            }


            object controllerName;
            routeData.Values.TryGetValue(ControllerKey, out controllerName);


            return controllerName.ToString();
        }

        private static string CreateAmbiguousControllerExceptionMessage(IHttpRoute route, string controllerName,
                                                                         IEnumerable<Type> matchingTypes)
        {
            Contract.Assert(route != null);
            Contract.Assert(controllerName != null);
            Contract.Assert(matchingTypes != null);


            var typeList = new StringBuilder();
            foreach (Type matchedType in matchingTypes)
            {
                typeList.AppendLine();
                typeList.Append(matchedType.FullName);
            }


            return String.Format("Multiple possibilities for {0}, using route template {1}. The following types were selected: {2}.",
                                 controllerName,
                                 route.RouteTemplate,
                                 typeList);
        }


        private ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor> InitializeControllerInfoCache()
        {
            var result = new ConcurrentDictionary<ControllerIdentification, HttpControllerDescriptor>(ControllerIdentification.Comparer);
            var duplicateControllers = new HashSet<ControllerIdentification>();
            Dictionary<ControllerIdentification, ILookup<string, Type>> controllerTypeGroups = this._controllerTypeCache.Cache;


            foreach (KeyValuePair<ControllerIdentification, ILookup<string, Type>> controllerTypeGroup in controllerTypeGroups)
            {
                ControllerIdentification controllerName = controllerTypeGroup.Key;


                foreach (IGrouping<string, Type> controllerTypesGroupedByNs in controllerTypeGroup.Value)
                {
                    foreach (Type controllerType in controllerTypesGroupedByNs)
                    {
                        if (result.Keys.Contains(controllerName))
                        {
                            duplicateControllers.Add(controllerName);
                            break;
                        }
                        else
                        {
                            result.TryAdd(controllerName,
                                          new HttpControllerDescriptor(this._configuration, controllerName.Name, controllerType));
                        }
                    }
                }
            }


            foreach (ControllerIdentification duplicateController in duplicateControllers)
            {
                HttpControllerDescriptor descriptor;
                result.TryRemove(duplicateController, out descriptor);
            }


            return result;
        }

        private ControllerIdentification GetControllerIdentificationFromRequest(HttpRequestMessage request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("request");
            }


            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                return default(ControllerIdentification);
            }


            string controllerName = this.GetControllerNameFromRequest(request);

            object apiVersionObj;
            int? apiVersion = null;
            int version;
            if (routeData.Values.TryGetValue(VersionKey, out apiVersionObj) &&
                Int32.TryParse(apiVersionObj.ToString(), out version))
            {
                apiVersion = version;
            }


            return new ControllerIdentification(controllerName, apiVersion);
        }
    }
}
