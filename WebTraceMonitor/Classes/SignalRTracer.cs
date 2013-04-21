using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebTraceMonitor.Classes
{
    /// <summary>
    /// Writes Trace Messages directly to SignalR. This listener is only used internally by the the Web Trace Monitor web application.
    /// </summary>
    public static class SignalRTracer
    {
        public const string LevelInformation = "Information";
        public const string LevelVerbose = "Verbose";
        public const string LevelError = "Error";
        public const string LevelWarning = "Warning";
        private static string[] Machines =
    {
        "R2D2XYZABC",
        "TRON12345",
        "HAL00000"
    };

        private static int Seed = 1;
        
        public static void Send(string source, string eventType, string message, string category)
        {
            var msg = new WebTraceMonitor.Core.TraceMessage()
            {
                Message = message,
                Level = eventType,
                Source = source,
                Category = category,
                ProcessId = Process.GetCurrentProcess().Id,
                ThreadId = Thread.CurrentThread.ManagedThreadId,
                Timestamp = DateTime.UtcNow, 
                Machine = GetRandomMachine()
            };

            ConnectionManager.Broadcast(msg);
        }

        private static string GetRandomMachine()
        {
            Random random = new Random(Seed++);
            int randomIndex = random.Next(0, Machines.Length);
            return Machines[randomIndex];
        }
    }
}