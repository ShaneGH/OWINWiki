using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(OwinWiki.Examples.OwinHost.Startup))]

namespace OwinWiki.Examples.OwinHost
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Run(ctxt => ctxt.Response.WriteAsync("Hello world"));
        }
    }
}
