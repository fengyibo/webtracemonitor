# Web Trace Monitor

## Introduction

Web Trace Monitor is an open-source web application that can be installed on Windows Azure Websites, Windows Azure WebRoles or any other Windows Server running IIS and ASP.NET 4.5. 

It provides an unsecured, public REST endpoint, where trace messages can be posted to. For .NET developers a System.Diagnostics compatible trace listener exists for convenience. 

Web Trace Monitor uses [SignalR] to forward incoming trace messages to all connected browsers in near realtime.

Data is displayed with the means of the underlying [SlickGrid] and kept entirely in memory of the web browser until cleared. This enables quick filtering by machine or trace level. The usage of virtual Scrolling ensures, that web trace monitor can even deal with hundreds of thousands of trace messages.

The tool is targeted to gain insight into the process flow of complex distributed systems, like a Windows Azure application consisting of multiple roles, during development and testing. 

See a [Demo].  

## Installation

### Windows Azure Websites

The easiest way to deploy Web Trace Monitor is by deploying it directly from GitHub to a Windows Azure Website:

+ Create a Fork of the [master] branch on GitHub
+ Create a new WebSite with the Windows Azure Management Portal 
+ On the Dashboard of the Website in Azure click "Set up deployment from source control"
+ Choose GitHub and approve the authorization request.
+ Next you need to choose the Repository Name and branch. Choose your fork of the Web Trace Monitor.

The deployment is started automatically and after it has finished you should be able to access your instance of Web Trace Monitor.

### Windows Azure Web Role

+ Clone the repo
+ Open the solution with Visual Studio 2012
+ Publish the project WebTraceMonitor.Azure to your Windows Azure subscription

### Other IIS

+ Clone the repo
+ Open the solution with Visual Studio 2012
+ Publish the WebTraceMonitor web project e.g. with WebDeploy on  local IIS server.



[SignalR]:http://signalr.net/
[SlickGrid]:https://github.com/mleibman/SlickGrid/
[Demo]:http://webtracemonitordemo.cloudapp.net/

