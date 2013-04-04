using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using WebTraceMonitor.Classes;

namespace WebTraceMonitor.Controllers
{
    public class TraceController : ApiController
    {
        private static readonly DoSProtection DenialOfServiceProtector = new DoSProtection();

        // POST api/trace
        public void Post([FromBody]TraceMessage message)
        {
            if (DenialOfServiceProtector.CheckForDoS(this.Request.Headers.Host))
            {
                ConnectionManager.Broadcast(message);
            }
        }
    }
}
