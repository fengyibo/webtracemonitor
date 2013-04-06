using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Thread.Sleep(10000);
            int index = 0;
            for (;;)
            {
                Thread.Sleep(1000);
                Trace.TraceInformation(++index + "Just a test");
                Trace.WriteLine(++index + "Verbose test");
                TraceSource source = new TraceSource("test");
                source.Listeners.AddRange(Trace.Listeners);
                source.Switch = new SourceSwitch("SourceSwitch", "Verbose");
                source.TraceEvent(TraceEventType.Verbose, 276, "Verbose Info with Event Id");
            }
        }
    }
}
