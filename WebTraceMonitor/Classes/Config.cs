using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebTraceMonitor.Classes
{
    public static class Config
    {
        private const string ConfigKeyDosProtectionEnabled = "WebTraceMonitor.DoSProtection";
        private const string ConfigKeyTestDataGenerationEnabled = "WebTraceMonitor.TestDataEnabled";

        public static bool DoSProtectionEnabled
        {
            get { return GetBool(ConfigKeyDosProtectionEnabled, false); }
        }

        public static bool TestDataGenerationEnabled
        {
            get { return GetBool(ConfigKeyTestDataGenerationEnabled, false); }
        }

        public static string Version
        {
            get
            {
                FileVersionInfo version = FileVersionInfo.GetVersionInfo(typeof(Config).Assembly.Location);
                return version.ProductVersion.ToString();

            }
        }

        private static bool GetBool(string key, bool defaultValue)
        {
            bool result = defaultValue;
            try
            {
                if (RoleEnvironment.IsAvailable)
                {
                    if (!bool.TryParse(RoleEnvironment.GetConfigurationSettingValue(key), out result))
                    {
                        result = defaultValue;
                    }
                }
            }
            catch
            {
                result = defaultValue;
            }
            return result;
        }
    }
}