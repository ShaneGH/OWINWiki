﻿using Microsoft.Owin;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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