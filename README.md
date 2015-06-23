# OWINWiki
Documentation and source examples on Microsoft OWIN based on the excelent book https://www.syncfusion.com/resources/techportal/ebooks/owin

# Index

1. [Introduction](#introduction)
2. [Owin Pipeline](#owin-pipeline)
  * [App Delegate](#app-delegate)
  * [Environment Dictionary](#environment-dictionary)
3. [Katana](#katana)
4. [Katana Code Basics](#katana-code-basics)

# Introduction

Owin is a set of standards to define an interface between server and web app. It's goals are to decouple server and app and reduce dependency on **IIS** and **System.Web.dll*.

Owin has 4 layers, from bottom to top:

### Host
The host is responsible for hosting the server. It's primary action is to start and stop the server

###Server
The server is responsible for listening to sockets and sending/receiving packets. It then passes data to the middleware

###Middleware
The middleware is anything which stands between the server and the web app. It can be anything from a simple logger to a web framework such as WebAPI

###Application
Does not really interact with OWIN, but rather with the framework. Given that it is the primary entry point for the dev, is also used to add configuration to the OWIN pipeline.

[back to top ^](#owinwiki)

# Owin Pipeline

So, the host starts the server, the server parses a Http request, then what?
The host is also in control of the middleware (TODO: is it?), and 3 key components of the middleware are [App Delegate](#app-delegate) and the [Environment Dictionary](#environment-dictionary).

## App Delegate

The app delegate is in essence: 
```C#
Func<IDictionary<string, object>, Task>
```

It is the entry point to middleware components where a middleware component receives a dictionary (the [Environment Dictionary](#envirnonment-dictionary)) and returns an asynchronus task to run.

## Envirnonment Dictionary
The environment dictionary is a collection of objects relevent to the OWIN request. There are certain items populated by the web server which are mandatory in the environment dictionary

|Key Name|Description|
| --- | --- |
|owin.RequestBody|A Stream with the request body, if any. Stream.Null MAY be used as a placeholder if there is no request body.|
|owin.RequestHeaders|A IDictionary<string, string[]> of request headers.|
|owin.RequestMethod|A string containing the HTTP request method of the request (e.g., GET, POST).|
|owin.RequestPath|A string containing the request path. The path must be relative to the root of the application delegate.|
|owin.RequestPathBase|A string containing the portion of the request path corresponding to the root of the application delegate.|
|owin.RequestProtocol|A string containing the protocol name and version (e.g., HTTP/1.0 or HTTP/1.1).|
|owin.RequestQueryString|A string containing the query string component of the HTTP request URI, without the leading “?” (e.g., "foo=bar&baz=quux"). The value may be an empty string.|
|owin.RequestScheme|A string containing the URI scheme used for the request (e.g., http, https).|
|owin.ResponseBody|A Stream used to write out the response body, if any.|
|owin.ResponseHeaders|An IDictionary<string, string[]> of response headers.|
|owin.ResponseStatusCode|An optional int containing the HTTP response status code as defined in RFC 2616 section 6.1.1. The default is 200.|
|owin.ResponseReasonPhrase|An optional string containing the reason phrase associated with the given status code. If none is provided then the server should provide a default as described in RFC 2616 section 6.1.1.|
|owin.ResponseProtocol|An optional string containing the protocol name and version (e.g. HTTP/1.0 or HTTP/1.1). If none is provided, then the owin.RequestProtocol key’s value is the default|
|owin.CallCancelled|A CancellationToken indicating if the request has been cancelled or aborted.|
|owin.Version|The string 1.0 indicating OWIN version.|

[back to top ^](#owinwiki)

#Katana

![Katana](https://raw.githubusercontent.com/ShaneGH/OWINWiki/master/Content/Katana.png)

Katana is a microsoft project (set of dlls) which implements OWIN. Katana is the glue between OWIN and IIS (or any other mocrosoft server)

## Katana Host

Katana can be hosted in 3 ways:
1 In IIS. The OWIN pipeline is started via HttpModule and HttpHandler. If this option is used, IIS must also be used as the server
2 Via a custom host, console app, wpf app, whatever
3 Via OwinHost.exe. Launches the HttpListener server, finds a startup class by convention
  * Fully customisable with console args

## Katana Server

The katana server depends on which server the ost wants to start up. The server opens a socket, populates the environment dictionary and invokes the middleware.

1. System.Web (IIS)
2. HttpListener
3. WebListener (coming soon)

## Middleware

![Katana](https://raw.githubusercontent.com/ShaneGH/OWINWiki/master/Content/KatanaMiddleware.png)

[back to top ^](#owinwiki)

# Katana Code Basics

Katana is configured in the `Configuration(IAppBuilder app)` method of the `Startup` class by convention.
```C#
namespace RootAssemblyNamespace
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            // ...configure...
        }
    }
}
```

[back to top ^](#owinwiki)


























