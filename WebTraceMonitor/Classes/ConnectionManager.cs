using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Script.Serialization;
using Microsoft.AspNet.SignalR;
using System.Diagnostics;
using WebTraceMonitor.Core;

namespace WebTraceMonitor.Classes
{
    public static class ConnectionManager
    {
        public static void Broadcast(TraceMessage message)
        {
            if (message != null)
            {
                try
                {
                    var context = GlobalHost.ConnectionManager.GetConnectionContext<Connection>();
                    var serializer = new JavaScriptSerializer();
                    context.Connection.Broadcast(serializer.Serialize(new [] { message}));
                }
                catch (Exception ex)
                {
                    Trace.TraceError("Broadcast Error: " + ex.ToString() + ", " +ex.StackTrace);
                }
            }
        }
    }
}