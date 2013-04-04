using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;

namespace WebTraceMonitor.Classes
{
    public class Connection : PersistentConnection
    {
       
        protected override Task OnConnected(IRequest request, string connectionId)
        {
            return Groups.Add(connectionId, "traceListenerGroup");
        }

        protected override Task OnDisconnected(IRequest request, string connectionId)
        {
            return Groups.Remove(connectionId, "traceListenerGroup");
        }

        protected override IList<string> OnRejoiningGroups(IRequest request, IList<string> groups, string connectionId)
        {
            return groups;
        }

        protected override Task OnReceived(IRequest request, string connectionId, string data)
        {
            return Connection.Broadcast(data);
        }
    }
}