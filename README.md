# CobaltStrike-Redirector

A .NET Core API with reverse proxy middleware for redirecting C2 traffic through cloud-based services. 
Below is a sample usage with Azure websites: 

To deploy application, change the teamserver variable in ReverseProxy.cs to your listener and use the Azure cli to upload the project. 

![Deploy](/pictures/deploy.png)

Configure your listener so it points to your website. 

![Listener](/pictures/listener.png)

Your beacon will use azurewebsites.com for C2 connections. 

![Beacon](/pictures/beacon.png)
