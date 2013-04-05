using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTraceMonitor.Classes
{
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
    }
}