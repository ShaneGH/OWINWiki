# OWINWiki
Documentation and source examples on Microsoft OWIN based on the excelent book https://www.syncfusion.com/resources/techportal/ebooks/owin

# Index

1. [Introduction](#introduction)
2. [Owin Pipeline](#owin-pipeline)
    * [App Delegate](#app-delegate)
    * [Environment Dictionary](#environment-dictionary)
3. [Katana](#katana)
4. [Katana Code Basics](#katana-code-basics)
  * [Hello World](#hello-world)
5. [Katana Example 1: IIS hosted app](#katana-example-1-iis-hosted-app)
6. [Katana Example 2: Self hosted app](#katana-example-2-self-hosted-app)
7. [Katana Example 3: OwinHost.exe](#katana-example-3-owinhostexe)
8. [Changing the Startup Class](#changing-the-startup-class)
9. [Using Multiple Frameworks (Nancy and SignalR)](#using-multiple-frameworks-nancy-and-signalr)
10. [Writing Custom Middleware](#writing-custom-middleware)

# Introduction

Owin is a set of standards to define an interface between server and web app. It's goals are to decouple server and app and reduce dependency on **IIS** and **System.Web.dll**.

Owin has 4 layers, from bottom to top:

### Host
The host is responsible for hosting the server. It's primary action is to start and stop the server

### Server
The server is responsible for listening to sockets and sending/receiving packets. It then passes data (owin context) to the middleware

### Middleware
The middleware is a stack of components. Each component has 2 objectives:

1. Execute it's functionality given the Owin context
2. Execute the next middleware component in the stack if necessary

Your application framework will be a piece of middleware

### Application
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

## Environment Dictionary
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

# Katana

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

## Hello world

A simple app to write hello world to the response stream

1. Using the interfaces defined in the OWIN spec
2. Using katana abstractions

### Pure  OWIN
```C#
using AppFunc = Func<IDictionary<string, object>, Task>;

public void Configuration(IAppBuilder app)
{
  app.Use(new Func<AppFunc, AppFunc>(next => 
    (env => { 
      string text = "Hello World!"; 
      var response = env["owin.ResponseBody"] as Stream; 
      var headers = env["owin.ResponseHeaders"] as IDictionary<string, string[]>;
      headers["Content-Type"] = new[] { "text/plain" };
      return response.WriteAsync( Encoding.UTF8.GetBytes(text), 0, text.Length); 
     })
    ));
}
```

### OWIN and Katana

```C#
public void Configuration(IAppBuilder app) 
{ 
  app.Run(context => 
  { 
    context.Response.ContentType = "text/plain"; 
    return context.Response.WriteAsync("Hello World!");
  });
}
```

[back to top ^](#owinwiki)

# Katana Example 1: IIS hosted app

See OwinWiki.Examples.IISHostedApp

## Key points

1. Create an empty ASP web app
2. Use package Microsoft.Owin.Host.SystemWeb
3. Use Startup by convention: the `Configuration` method of the `Startup` class.
4. The app template also adds an assembly attribute to specify the `Startup` class (overkill I guess)
  * `[assembly: OwinStartup(typeof(OwinWiki.Examples.IISHostedApp.Startup))]`

## Code

```C#
[assembly: OwinStartup(typeof(OwinWiki.Examples.IISHostedApp.Startup))]

namespace OwinWiki.Examples.IISHostedApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Run(context => 
            {
                context.Response.ContentType = "text/plain";

                // say hello to the last segment in the Url path
                return context.Response.WriteAsync("Hello " + (context.Request.Uri.Segments.Last() ?? "world"));
            });
        }
    }
}
```

[back to top ^](#owinwiki)

# Katana Example 2: Self hosted app

See OwinWiki.Examples.SelfHostedApp

## Key points

1. Use package Microsoft.Owin.SelfHost
2. Use Startup by convention: the `Configuration` method of the `Startup` class.
3. Uses the HttpListener server.
4. Add 3 middleware items
  1. A home page
  2. An error screen
  3. A web framework (hello world)
5. Microsoft.Owin.Diagnostics exposes the `WelcomPage` and `ErrorPage` extensions.
6. Check the exception case, it is very detailed, giving a dump of the environment dictionary

## Code

```C#
public class Startup
{
    static void Main(string[] args)
    {
        using (WebApp.Start<Startup>("http://localhost:9000"))
        {
            Console.WriteLine("Launched site on http://localhost:9000");
            Console.WriteLine("Press [space] to quit...");

            while (Console.ReadKey(true).KeyChar != ' ') ;
        }
    }

    public void Configuration(IAppBuilder app) 
    {
        // Middleware 1
        // configure katana to display the OWIN 
        // welcome page at url "/"
        app.UseWelcomePage("/");

        // Middleware 2
        // configure a yellow screen of death
        app.UseErrorPage(); 

        // Middleware 3
        // Our web framework
        app.Run(context => 
        { 
            Trace.WriteLine(context.Request.Uri); 
                
            //Line to show the ErrorPage 
            if (context.Request.Path.ToString().Equals("/throwexception")) 
                throw new Exception("You requested the wrong URL :)");
                
            context.Response.ContentType = "text/plain"; 
            return context.Response.WriteAsync("Hello, world."); 
        }); 
    }
}
```

[back to top ^](#owinwiki)

# Katana Example 3: OwinHost.exe

See OwinWiki.Examples.OwinHost

## Key points
1. Can be an ASP web app or a dll
2. This time, add the Startup class with the Add Item -> Owin Startup class option
  * This will automatically add NuGet references
3. Add the OwinHost NuGet package
  * It resides in the tools folder of the OwinHost package
  * Loads all of the assemblies in the "/bin" folder, not the "bin/debug" folder
    * (so chage the output path in the proj properties)
4. Launch OwinHost with a console window
  * "../../packages/OwinHost.3.0.1/tools/OwinHost.exe"
5. Usa the `-h` option to view args

## Using a web app instead of a dll

You can create an asp mvc application instead of a dll. The benefits of this are, in debugging.

Go to the web tab of the project settings and you can now specify the server you wish to use. You can then just press F5 and debug as normal.

![OwinServer](https://raw.githubusercontent.com/ShaneGH/OWINWiki/master/Content/OwinServer.png)

[back to top ^](#owinwiki)

# Changing the Startup Class

The `Startup` class can be specified in many different ways. Each item of the list overrides previous items.

1. By convention, using the `Configuration` method of the `Startup` class.
2. By an assembly attribute
  * `[assembly: OwinStartup(typeof(MyStartupClass), "MyStartupMethod")]`.
  * Or, `[assembly: OwinStartup("Default", typeof(MyStartupClass))]`. Using this method you can have multiple startup classes. See [the next section](#friendly-name).
3. In the config file: `<appSettings> <add key="owin:appStartup" value="MyApp.MyStartupClass" /> </appSettings>`
4. Custom host `WebApp.Start<MyStartupClass>("http://localhost:9000");`
5. Command line arg to OwinHost.exe
 
### Friendly Name

You can specify multiple startup classes via the `[assembly: OwinStartup]` attribute and choose which one to use via the config file or command line args. Just use the `friendlyName` arg of the `OwinStartup` attribute and specify it as
* The `value` to the `owinStartup` config file setting --or--
* The first command line arg for OwinHost.exe


[back to top ^](#owinwiki)

# Using Multiple Frameworks (Nancy and SignalR)

The following section documents an app wich uses both the Nancy framework and SignalR.

The code is in the OwinWiki.Examples.MultipleFrameworks project.

### Things to note
* Using a self hosted .exe
* Packages:
  * Microsoft.Owin.SelfHost
  * Nancy.Owin
  * Microsoft.AspNet.SignalR.SelfHost
  * Microsoft.Owin.StaticFiles
  * jQuery 1.6.4
  * Microsoft.AspNet.SignalR.JS
* 3 Middleware options:
  * SignalR
  * Nancy
  * Custom 404
* Nancy is configured in HomeModule.cs
  * It has a method which accesses the Owin environment dictionary
* SignalR is configured on the /Chat/ path only
* Got to Chat/signalr/negotiate or Chat/signalr/hubs to verify signalR
* All static files must be placed in the files/ folder
* Build copy to output dir for static files must be Copy Always

[back to top ^](#owinwiki)

# Writing Custom Middleware

Middleware components look like this:
```C#
using AppFunc = Func<IDictionary<string, object>, Task>;
using MiddlewareDelegate = Func<AppFunc, AppFunc>;
```
The middleware takes in the AppFunc for the -next- middleware component and returns it's own func. It then has the responsibility of executing the next component.

```C#
app.Use(new Func<AppFunc, AppFunc>(next => (async env =>
{
	// get output stream
	var response = env["owin.ResponseBody"] as Stream;
	var writer = new StreamWriter(response) { AutoFlush = true };

	// surround output of the next middleware component with flags
	await writer.WriteAsync("Before");
	await next.Invoke(env);
	await writer.WriteAsync("After");
})));
```

## Packaging middleware components

You can package a middleware component in a class which has a constructor which accepts the AppFunc of the next component and an Invoke method which accepts the EnvironmentDictionary.

```C#
public class TraceMiddleware
{
    public readonly Func<IDictionary<string, object>, Task> Next;

    public TraceMiddleware(Func<IDictionary<string, object>, Task> next)
    {
        Next = next;
    }

    public async Task Invoke(IDictionary<string, object> environment)
    {
	    // get output stream
	    var response = environment["owin.ResponseBody"] as Stream;
	    var writer = new StreamWriter(response) { AutoFlush = true };

	    // surround output of the next middleware component with flags
	    await writer.WriteAsync("Before");
        await Next.Invoke(environment);
	    await writer.WriteAsync("After");
    }
}
```
And you can invoke it like this
```C#
app.Use(typeof(TraceMiddleware));
```

### Helpers

You can use the helpers in `Microsoft.Owin.dll` to
* simplify the `Use` delegate and stream writing.
* package components by inheriting from `OwinMiddleware` and overriding the `Invoke` method.

[back to top ^](#owinwiki)

[back to top ^](#owinwiki)

