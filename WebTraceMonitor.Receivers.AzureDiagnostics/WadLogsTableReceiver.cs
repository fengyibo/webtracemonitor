using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebTraceMonitor.Core;

namespace WebTraceMonitor.Receivers.AzureDiagnostics
{
    [Export(typeof(IWebTraceReceiver))]
    [ExportMetadata("StorageAccout", null)]
    [ExportMetadata("PrimaryAccessToken", null)]
    public class WadLogsTableReceiver : IWebTraceReceiver
    {
        public string StorageAccount { get; set; }
        public string PrimaryAccessToken { get; set; }

        public event EventReceivedHandler Received;

        public WadLogsTableReceiver()
        {
            
        }

        public WadLogsTableReceiver(string storageAccount, string primaryAccessToken)
        {
            StorageAccount = storageAccount;
            PrimaryAccessToken = primaryAccessToken;
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
