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
        private const string LevelInformation = "Information";
        private const string LevelVerbose = "Verbose";
        private const string LevelWarning = "Warning";
        private const string LevelError = "Error";

        private static readonly DoSProtection DenialOfServiceProtector = new DoSProtection();

        // POST api/trace
        public HttpResponseMessage Post([FromBody]TraceMessage message)
        {
            if (message.Timestamp == DateTime.MinValue)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Timestamp or Timestamp is missing");
            }

            if ((message.Level != LevelError) &&
                (message.Level != LevelInformation) &&
                (message.Level != LevelVerbose) &&
                (message.Level != LevelWarning))
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest, "Invalid Level or Level is missing");
            }

            if (DenialOfServiceProtector.CheckForDoS(this.Request.Headers.Host))
            {
                ConnectionManager.Broadcast(message);
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
