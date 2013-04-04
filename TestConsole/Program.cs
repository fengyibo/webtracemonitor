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
            int index = 0;
            for (;;)
            {
                Thread.Sleep(100);
                Trace.TraceInformation(++index + "Just a test");
                Trace.WriteLine(++index + "Verbose test");
            }
        }
    }
}
