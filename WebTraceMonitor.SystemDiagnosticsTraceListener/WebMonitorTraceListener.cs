using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebTraceMonitor.SystemDiagnosticsTraceListener
{
    public class WebMonitorTraceListener : TraceListener
    {
        private string _machineName;
        private int _processId;
        private const string DefaultSource = "";
        private const string DefaultCategory = "";
        private const char Delimiter = (char) 0x7F;
        private const string LevelInformation = "Information";
        private const string LevelVerbose = "Verbose";
        private const string LevelError = "Error";
        private const string LevelWarning = "Warning";
        private const string Path = "api/v1/trace";
        private const int DefaultPort = 80;
        private string uri = string.Empty;
        private string azureHost;
        private int azurePort = 80;
        private bool enabled = true;
        private const string ConfigKeyAzureHost = "WebTraceMonitor.Host";
        private const string ConfigKeyAzurePort = "WebTraceMonitor.Port";
        private const string ConfigKeyAzureEnabled = "WebTraceMonitor.Enabled";
        private const string ContentType = "application/json; charset=utf-8";
        private const string Verb = "POST";
        private const int RequestTimeout = 5000;

        public class TraceMessage
        {
            public string Level { get; set; }
            public DateTime Timestamp { get; set; }
            public string Machine { get; set; }
            public string Category { get; set; }
            public string Source { get; set; }
            public string Message { get; set; }
            public int ProcessId { get; set; }
            public int ThreadId { get; set; }
            public int EventId { get; set; }
        }

        private class HttpWebRequestAsyncState
        {
            public byte[] RequestBytes { get; set; }
            public HttpWebRequest HttpWebRequest { get; set; }
            public HttpWebRequestAsyncState State { get; set; }
        }

        public string Uri
        {
            get
            {
                if (string.IsNullOrEmpty(uri))
                {
                    int port = DefaultPort;
                    string hostname = string.Empty;
                    if (string.IsNullOrEmpty(azureHost))
                    {
                        foreach (DictionaryEntry de in this.Attributes)
                        {
                            if (de.Key.ToString().ToLower() == "host")
                                hostname = de.Value.ToString();
                            if (de.Key.ToString().ToLower() == "port")
                                Int32.TryParse(de.Value.ToString(), out port);
                        }
                    }
                    else
                    {
                        hostname = azureHost;
                        port = azurePort;
                    }

                    UriBuilder builder = new UriBuilder("http", hostname, port, Path);
                    uri = builder.ToString();
                }
                return uri;
            }
        }

        public WebMonitorTraceListener()
            : base("WebMonitorTraceListener")
        {
            try
            {
                _machineName = Environment.MachineName;
            }
            catch
            {
            }

            try
            {
                _processId = Process.GetCurrentProcess().Id;
            }
            catch
            {
            }

            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    _machineName = RoleEnvironment.CurrentRoleInstance.Id;
                    ReadAzureConfiguration();
                    RoleEnvironment.Changed += RoleEnvironment_Changed;
                }
            }
            catch
            {
            }
        }

        private void ReadAzureConfiguration()
        {
            if (!bool.TryParse(RoleEnvironment.GetConfigurationSettingValue(ConfigKeyAzureEnabled), out enabled))
            {
                enabled = false; // disable WebTraceMon by default in Azure
            }
            azureHost = RoleEnvironment.GetConfigurationSettingValue(ConfigKeyAzureHost);
            int.TryParse(RoleEnvironment.GetConfigurationSettingValue(ConfigKeyAzurePort), out azurePort);
        }

        void RoleEnvironment_Changed(object sender, RoleEnvironmentChangedEventArgs e)
        {
            ReadAzureConfiguration();
            uri = string.Empty;
        }

        public override void Fail(string message, string detailMessage)
        {
            StringBuilder builder = new StringBuilder(message);
            if (detailMessage != null)
            {
                builder.Append(" ");
                builder.Append(detailMessage);
            }
            this.TraceEvent(null, null, TraceEventType.Error, 0, builder.ToString());
        }

        public sealed override void Flush()
        {
        }

        protected override string[] GetSupportedAttributes()
        {
            return new string[] { "host", "port"};
        }

        public sealed override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if (((base.Filter == null) ||
                 base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, null)))
            {
                StringBuilder builder = new StringBuilder(0x200);
                if (data != null)
                {
                    builder.Append(data.ToString());
                }
                else
                {
                    builder.Append(": null");
                }
                if ((eventCache != null) && ((base.TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None))
                {
                    builder.Append(" : Callstack:");
                    builder.Append(eventCache.Callstack);
                }
                SendTrace(source, eventType, builder.ToString(), DefaultCategory, id);
            }
        }

        public sealed override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, params object[] data)
        {
            if (((base.Filter == null) ||
                 base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, null)))
            {
                StringBuilder builder = new StringBuilder(0x200);

                if ((data != null) && (data.Length > 0))
                {
                    int index = 0;
                    while (index < (data.Length -1))
                    {
                        if (data[index] != null)
                        {
                            builder.Append(data[index].ToString());
                            builder.Append(Delimiter);
                        }
                        else
                        {
                            builder.Append("null,");
                        }
                        index++;
                    }
                    if (data[index] != null)
                    {
                        builder.Append(data[index].ToString());
                    }
                    else
                    {
                        builder.Append("null");
                    }
                }
                else
                {
                    builder.Append("null");
                }
                if ((eventCache != null) && ((base.TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None))
                {
                    builder.Append(" : Callstack:");
                    builder.Append(eventCache.Callstack);
                }
                SendTrace(source, eventType, builder.ToString(), DefaultCategory, id);
            }
        }

        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            if (((base.Filter == null) ||
                 base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, null)))
            {
                if ((eventCache != null) && ((base.TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None))
                {
                    SendTrace(source, eventType, " : CallStack:" + eventCache.Callstack, DefaultCategory, id);
                }
                else
                {
                    SendTrace(source, eventType, string.Empty, DefaultCategory, id);
                }
            }
        }

        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
            if (((base.Filter == null) ||
                 base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, null)))
            {
                StringBuilder builder = new StringBuilder(0x200);

                builder.Append(message);

                if ((eventCache != null) && ((base.TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None))
                {
                    builder.Append(" : CallStack");
                    builder.Append(eventCache.Callstack);
                }
                SendTrace(source, eventType, builder.ToString(), DefaultCategory, id);
            }
        }

        public sealed override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (((base.Filter == null) ||
             base.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, null)))
            {
                string message = string.Format(format, args);

                if ((eventCache != null) && ((base.TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None))
                {
                    SendTrace(source, eventType, message + " : Callstack:" + eventCache.Callstack, DefaultCategory);
                }
                else
                {
                    SendTrace(source, eventType, message, DefaultCategory, id);
                }
            }
        }

        public sealed override void TraceTransfer(TraceEventCache eventCache, string source, int id, string message, Guid relatedActivityId)
        {
            StringBuilder builder = new StringBuilder(0x200);
            object activityId = Trace.CorrelationManager.ActivityId;
            if (activityId != null)
            {
                Guid guid = (Guid) activityId;
                builder.AppendFormat("activityId={0}{1}", guid.ToString(), Delimiter);
            }
            builder.AppendFormat("relatedActivityId={0}{1}", relatedActivityId.ToString(), Delimiter + message);
            if ((eventCache != null) && ((base.TraceOutputOptions & TraceOptions.Callstack) != TraceOptions.None))
            {
                builder.Append(" : CallStack");
                builder.Append(eventCache.Callstack);
            }
            SendTrace(source, TraceEventType.Information, builder.ToString(), DefaultCategory, id);
        }

        public sealed override void Write(string message)
        {
            this.SendTrace(DefaultSource, TraceEventType.Verbose, message, DefaultCategory);
        }

        public override void WriteLine(string message)
        {
            this.Write(message);
        }

        public sealed override void Write(string message, string category)
        {
            base.Write(message, category);
        }

        public sealed override void WriteLine(string message, string category)
        {
            base.WriteLine(message, category);
        }

        protected virtual void SendTrace(string source, TraceEventType eventType, string message, string category, int id = 0)
        {
            if (enabled)
            {
                var msg = CreateTraceMsg(source, eventType, message, category, id);

                try
                {
                    var request = (HttpWebRequest) HttpWebRequest.Create(Uri);
                    request.ContentType = WebMonitorTraceListener.ContentType;
                    request.Method = WebMonitorTraceListener.Verb;

                    JavaScriptSerializer serializer = new JavaScriptSerializer();
                    serializer.Serialize(msg);
                    byte[] requestBytes = Encoding.UTF8.GetBytes(serializer.Serialize(msg));
                    request.ContentLength = requestBytes.Length;
                    request.Timeout = WebMonitorTraceListener.RequestTimeout;

                    var requestStream = request.BeginGetRequestStream(BeginGetRequestStreamCallback,
                                                                      new HttpWebRequestAsyncState()
                                                                          {
                                                                              RequestBytes = requestBytes,
                                                                              HttpWebRequest = request,
                                                                          });
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message + "" + ex.StackTrace);
                }
            }
        }

        protected TraceMessage CreateTraceMsg(string source, TraceEventType eventType, string message, string category, int id)
        {
            var msg = new TraceMessage()
                                   {
                                       Message = message,
                                       Level = MapEventTypeToString(eventType),
                                       Source = source,
                                       Category = category,
                                       ProcessId = _processId,
                                       ThreadId = Thread.CurrentThread.ManagedThreadId,
                                       Timestamp = DateTime.UtcNow,
                                       Machine = _machineName,
                                       EventId = id
                                   };
            return msg;
        }

        private void BeginGetRequestStreamCallback(IAsyncResult asyncResult)
        {
            Stream requestStream = null;
            HttpWebRequestAsyncState asyncState = null;
            try
            {
                asyncState = (HttpWebRequestAsyncState)asyncResult.AsyncState;
                requestStream = asyncState.HttpWebRequest.EndGetRequestStream(asyncResult);
                requestStream.Write(asyncState.RequestBytes, 0, asyncState.RequestBytes.Length);
                requestStream.Close();
                asyncState.HttpWebRequest.BeginGetResponse(BeginGetResponseCallback,
                  new HttpWebRequestAsyncState
                  {
                      HttpWebRequest = asyncState.HttpWebRequest,
                      State = asyncState.State
                  });
            }
            catch 
            {
                if (asyncState == null)
                    throw;
            }
            finally
            {
                if (requestStream != null)
                    requestStream.Close();
            }
        }

        private void BeginGetResponseCallback(IAsyncResult asyncResult)
        {
            WebResponse webResponse = null;
            Stream responseStream = null;
            HttpWebRequestAsyncState asyncState = null;
            try
            {
                asyncState = (HttpWebRequestAsyncState)asyncResult.AsyncState;
                webResponse = asyncState.HttpWebRequest.EndGetResponse(asyncResult);
                responseStream = webResponse.GetResponseStream();
                responseStream.Close();
                responseStream = null;
                webResponse.Close();
                webResponse = null;
            }
            catch
            {
                if (asyncState == null)
                    throw;
            }
            finally
            {
                if (responseStream != null)
                    responseStream.Close();
                if (webResponse != null)
                    webResponse.Close();
            }
        }


        protected string MapEventTypeToString(TraceEventType eventType)
        {
            switch (eventType)
            {
                case TraceEventType.Information:
                    return LevelInformation;
                case TraceEventType.Critical:
                case TraceEventType.Error:
                    return LevelError;
                case TraceEventType.Resume:
                case TraceEventType.Start:
                case TraceEventType.Stop:
                    case TraceEventType.Suspend:
                    case TraceEventType.Transfer:
                    case TraceEventType.Verbose:
                    return LevelVerbose;
                case TraceEventType.Warning:
                    return LevelWarning;
                default:
                    return LevelVerbose;
            }
        }
    }
}
