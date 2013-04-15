using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.WindowsAzure.Management.Websites.Utilities;
using WebTraceMonitor.Core;

namespace WebTraceMonitor.Receivers.AzureWebsiteLogfiles
{
    public class WebsiteLogReceiver : IWebTraceReceiver
    {
        public const int WaitInterval = 10000;

        private RemoteLogStreamManager RemoteLogStreamManager;
        private LogStreamWaitHandle LogStreamWaitHandle;
        private Predicate<string> EndStreaming;

        public string Path { get; set; }
        public string Message { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SiteName { get; set; }

        public WebsiteLogReceiver()
        {
            Path = "/Application";

        }
        private void LogStreaming()
        {
            ICredentials credentials = new NetworkCredential(
            @"LogTestSite\berndku",
            "free4Admin");
            Path = HttpUtility.UrlEncode(Path);
            Message = HttpUtility.UrlEncode(Message);


            RemoteLogStreamManager = RemoteLogStreamManager ?? new RemoteLogStreamManager(
                "logtestsite2",
                Path,
                Message,
                credentials);


            using (LogStreamWaitHandle = LogStreamWaitHandle ?? 
                new LogStreamWaitHandle(RemoteLogStreamManager.GetStream().Result))
            {
                bool doStreaming = true;
                
                while (doStreaming)
                {
                    string line = LogStreamWaitHandle.WaitNextLine(WaitInterval);


                    if (line != null)
                    {
                        ForwardTrace(line);
                    }


                    doStreaming = EndStreaming == null ? true : EndStreaming(line);
                }
            }
        }

        private void ForwardTrace(string line)
        {
            Regex expr = new Regex(@"^(?<timestamp>\w+)  PID\[(?<pid>\d+)\]  (?<level>\w+)  (?<message>\w+)$");
            Match match = expr.Match(line);
            if (match != null)
            {
                TraceMessage trace = new TraceMessage()
                                         {
                                             ProcessId = Int32.Parse(match.Groups["pid"].Value),
                                             Message = match.Groups["timestamp"].Value,
                                             Level = ConvertLevel(match.Groups["level"].Value),
                                             Timestamp = DateTime.ParseExact(match.Groups["timestamp"].Value, "yyyy-MM-ddTHH:mm:ss.ff", CultureInfo.InvariantCulture),
                                             Category = string.Empty,
                                             EventId = 0,
                                             ThreadId = 0,
                                             Machine = "Azure Website: " + SiteName,
                                             Source = "Azure Website: " + SiteName
                                         };

            }

        }

private string ConvertLevel(string p)
{
 	throw new NotImplementedException();
}

        public event EventReceivedHandler Received;
        public void Start()
        {
            LogStreaming();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
