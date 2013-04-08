using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebTraceMonitor.Core
{
    public delegate void EventReceivedHandler(TraceMessage msg);

    /// <summary>
    /// IWebTraceReceiver
    /// </summary>
    public interface IWebTraceReceiver
    {
        /// <summary>
        /// Raised whenever a new event arrives. WebTraceMonitor registers on this event.
        /// </summary>
        event EventReceivedHandler Received;

        /// <summary>
        /// Start receiving trace messages
        /// </summary>
        void Start();

        /// <summary>
        /// Stop receiving trace messages
        /// </summary>
        void Stop();
    }
}