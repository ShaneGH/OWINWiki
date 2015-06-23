# OWINWiki
Documentation and source examples on Microsoft OWIN

# Index

* [Introduction](#introduction)
* [Owin Pipeline](#owin-pipeline)

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
The host is also in control of the middleware (TODO: is it?), and 3 key components of the middleware are [App Delegate](#app-delegate) and the [Envirnonment Dictionary](#envirnonment-dictionary).

## App Delegate

The app delegate is in essence: 
```C#
Func<IDictionary<string, object>, Task>
```

It is the entry point to middleware components where a middleware component receives a dictionary (the [Envirnonment Dictionary](#envirnonment-dictionary)) and returns an asynchronus task to run.

## Environment Dictionary
The environment dictionary is a collection of objects relevent to the OWIN request. There are certain items populated by the web server which are mandatory in the environment dictionary



[back to top ^](#owinwiki)






























