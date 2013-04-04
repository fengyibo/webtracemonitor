using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebTraceMonitor.Classes
{
    public class TestMessageGenerator
    {
        public static void Start()
        {
            new Thread(new ThreadStart(Notify)).Start();
        }

        public static void Notify()
        {
            Thread.Sleep((5000));
            for (; ; )
            {

                Thread.Sleep(3000);

                Trace.TraceInformation("You are seeing auto-generated test messages, which are generated every 3 seconds for demo purposes.");

                const string warning = @"Event code: 3008 
Event message: A configuration error has occurred. 
Event time: 03.04.2013 17:06:47 
Event time (UTC): 03.04.2013 15:06:47 
Event ID: f68af02fc191444482a16d6a8f9f97a6 
Event sequence: 1 
Event occurrence: 1 
Event detail code: 0 
 
Application information: 
    Application domain: /LM/W3SVC/10/ROOT-1-130094752052146393 
    Trust level: Full 
    Application Virtual Path: / 
    Application Path: D:\Source\My\AzureLiveTrace\LiveTraceWebApp\ 
    Machine name: 
 
Process information: 
    Process ID: 13504 
    Process name: iisexpress.exe 
    Account name: 
 
Exception information: 
    Exception type: ConfigurationErrorsException 
    Exception message: Cannot create/shadow copy 'Microsoft.Data.OData, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' when that file already exists.
   at System.Web.Configuration.CompilationSection.LoadAssemblyHelper(String assemblyName, Boolean starDirective)
   at System.Web.Configuration.CompilationSection.LoadAllAssembliesFromAppDomainBinDirectory()
   at System.Web.Configuration.CompilationSection.LoadAssembly(AssemblyInfo ai)
   at System.Web.Compilation.BuildManager.GetReferencedAssemblies(CompilationSection compConfig)
   at System.Web.Compilation.BuildManager.GetPreStartInitMethodsFromReferencedAssemblies()
   at System.Web.Compilation.BuildManager.CallPreStartInitMethods(String preStartInitListPath)
   at System.Web.Compilation.BuildManager.ExecutePreAppStart()
   at System.Web.Hosting.HostingEnvironment.Initialize(ApplicationManager appManager, IApplicationHost appHost, IConfigMapPathFactory configMapPathFactory, HostingEnvironmentParameters hostingParameters, PolicyLevel policyLevel, Exception appDomainCreationException)

Cannot create/shadow copy 'Microsoft.Data.OData, Version=5.2.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35' when that file already exists.
   at System.Reflection.RuntimeAssembly._nLoad(AssemblyName fileName, String codeBase, Evidence assemblySecurity, RuntimeAssembly locationHint, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean throwOnFileNotFound, Boolean forIntrospection, Boolean suppressSecurityChecks)
   at System.Reflection.RuntimeAssembly.nLoad(AssemblyName fileName, String codeBase, Evidence assemblySecurity, RuntimeAssembly locationHint, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean throwOnFileNotFound, Boolean forIntrospection, Boolean suppressSecurityChecks)
   at System.Reflection.RuntimeAssembly.InternalLoadAssemblyName(AssemblyName assemblyRef, Evidence assemblySecurity, RuntimeAssembly reqAssembly, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean throwOnFileNotFound, Boolean forIntrospection, Boolean suppressSecurityChecks)
   at System.Reflection.RuntimeAssembly.InternalLoad(String assemblyString, Evidence assemblySecurity, StackCrawlMark& stackMark, IntPtr pPrivHostBinder, Boolean forIntrospection)
   at System.Reflection.RuntimeAssembly.InternalLoad(String assemblyString, Evidence assemblySecurity, StackCrawlMark& stackMark, Boolean forIntrospection)
   at System.Reflection.Assembly.Load(String assemblyString)
   at System.Web.Configuration.CompilationSection.LoadAssemblyHelper(String assemblyName, Boolean starDirective)

Cannot create/shadow copy 'Microsoft.Data.OData' when that file already exists.

 
 
Request information: 
    Request URL: http://localhost:16553/ 
    Request path: / 
    User host address: ::1 
    User:  
    Is authenticated: False 
    Authentication Type:  
    Thread account name:  
 
Thread information: 
    Thread ID: 5 
    Thread account name: 
    Is impersonating: False 
    Stack trace:    at System.Web.Configuration.CompilationSection.LoadAssemblyHelper(String assemblyName, Boolean starDirective)
   at System.Web.Configuration.CompilationSection.LoadAllAssembliesFromAppDomainBinDirectory()
   at System.Web.Configuration.CompilationSection.LoadAssembly(AssemblyInfo ai)
   at System.Web.Compilation.BuildManager.GetReferencedAssemblies(CompilationSection compConfig)
   at System.Web.Compilation.BuildManager.GetPreStartInitMethodsFromReferencedAssemblies()
   at System.Web.Compilation.BuildManager.CallPreStartInitMethods(String preStartInitListPath)
   at System.Web.Compilation.BuildManager.ExecutePreAppStart()
   at System.Web.Hosting.HostingEnvironment.Initialize(ApplicationManager appManager, IApplicationHost appHost, IConfigMapPathFactory configMapPathFactory, HostingEnvironmentParameters hostingParameters, PolicyLevel policyLevel, Exception appDomainCreationException)
 
 
Custom event details: 
";
                Trace.TraceWarning(warning);

                string verbose = @"Fault bucket -835936736, type 5
Event Name: Office11ShipAssert
Response: Not available
Cab Id: 0

Problem signature:
P1: 2oyl
P2: 15.0.4454.0
P3: 
P4: 
P5: 
P6: 
P7: 
P8: 
P9: 
P10: 

Attached files:
C:\Users\dummy\AppData\Local\Microsoft\Office\ShipAsserts\outlook.exe.2oyl.dmp
C:\Users\dummy\AppData\Local\Microsoft\Office\ShipAsserts\outlook.exe.2oyl.cvr

These files may be available here:
C:\Users\dummy\AppData\Local\Microsoft\Windows\WER\ReportArchive\NonCritical_2oyl_25dcdb56ffd8ab645eec70d5bdb96ce3658fb288_451f44d1

Analysis symbol: 
Rechecking for solution: 0
Report Id: 3d6f7244-9c73-11e2-bede-e3079fbde490
Report Status: 0
Hashed bucket: 328b7c8dcc84b1f1685b7bda847fd531";
                Trace.WriteLine(verbose);
                const string error = @"An unhandled exception occurred and the process was terminated.

Application ID: /LM/W3SVC/10/ROOT

Process ID: 19948

Exception: System.InvalidOperationException

Message: role discovery data is unavailable

StackTrace:    at Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment.get_CurrentRoleInstance()
   at LiveTraceWebApp.Classes.DirectSignalRTraceListener.SendTrace(String source, TraceEventType eventType, String message, String category) in d:\Source\My\AzureLiveTrace\LiveTraceWebApp\Classes\DirectSignalRTraceListener.cs:line 19
   at WebTraceMonitor.SystemDiagnosticsTraceListener.WebMonitorTraceListener.TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, Int32 id, String message) in d:\Source\My\AzureLiveTrace\WebTraceMonitor.SystemDiagnosticsTraceListener\WebMonitorTraceListener.cs:line 256
   at System.Diagnostics.TraceInternal.TraceEvent(TraceEventType eventType, Int32 id, String format, Object[] args)
   at System.Diagnostics.Trace.TraceInformation(String message)
   at LiveTraceWebApp.Notifier.Notify() in d:\Source\My\AzureLiveTrace\LiveTraceWebApp\Global.asax.cs:line 49
   at System.Threading.ThreadHelper.ThreadStart_Context(Object state)
   at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state, Boolean preserveSyncCtx)
   at System.Threading.ExecutionContext.Run(ExecutionContext executionContext, ContextCallback callback, Object state)
   at System.Threading.ThreadHelper.ThreadStart()";
                Trace.TraceError(error);
            }
        }
    }
}