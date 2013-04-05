Web Trace Monitor
=================


Introduction
------------

Web Trace Monitor is an open-source web application that can be installed on Windows Azure Websites, Windows Azure WebRoles or any other Windows Server running IIS and ASP.NET 4.5. 

It provides an unsecured, public REST endpoint, where trace messages can be posted to. For .NET developers a System.Diagnostics compatible trace listener exists for convenience. 

Web Trace Monitor uses [SignalR] to forward incoming trace messages to all connected browsers in near realtime.

Data is displayed with the means of the underlying [SlickGrid] and kept entirely in memory of the web browser until cleared. This enables quick filtering by machine or trace level. The usage of virtual Scrolling ensures, that web trace monitor can even deal with hundreds of thousands of trace messages.

The tool is targeted to gain insight into the process flow of complex distributed systems, like a Windows Azure application consisting of multiple roles, during development and testing. 


[SignalR]:http://signalr.net/
[SlickGrid]:https://github.com/mleibman/SlickGrid/
