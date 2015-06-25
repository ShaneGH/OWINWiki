using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinWiki.Examples.SelfHostedApp
{
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
}
