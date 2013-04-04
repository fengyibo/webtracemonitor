using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using Microsoft.WindowsAzure.ServiceRuntime;
using WebTraceMonitor.SystemDiagnosticsTraceListener;

namespace WebTraceMonitor.Classes
{
    /// <summary>
    /// Writes Trace Messages directly to SignalR. This listener is only used internally by the the Web Trace Monitor web application.
    /// </summary>
    public class DirectSignalRTraceListener : WebMonitorTraceListener
    {
        protected override void SendTrace(string source, System.Diagnostics.TraceEventType eventType, string message, string category)
        {
            var msg = new WebTraceMonitor.Classes.TraceMessage()
            {
                Message = message,
                Level = MapEventTypeToString(eventType),
                Source = source,
                Category = category,
                ProcessId = Process.GetCurrentProcess().Id,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Timestamp = DateTime.UtcNow.ToString("dd-MM-yyyy HH:mm:ss.fff"),
                Machine = Environment.MachineName
            };

            ConnectionManager.Broadcast(msg);
        }
    }
}