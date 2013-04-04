using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;

namespace WebTraceMonitor.Classes
{
    /// <summary>
    /// Simple mechanism to protect demo website from denial of service attacks. 
    /// 
    /// </summary>
    public class DoSProtection
    {
        private ConcurrentDictionary<string,int> stats = new ConcurrentDictionary<string, int>();

        private const int MaxAllowedMessagesPerHost = 100;

        public bool CheckForDoS(string host)
        {
            if (Config.DoSProtectionEnabled)
            {
                stats.AddOrUpdate(host, 0, (s, i) => i + 1);
                int count;
                stats.TryGetValue(host, out count);
                return count <= MaxAllowedMessagesPerHost;
            }
            else
            {
                return true;
            }
        }
    }
}