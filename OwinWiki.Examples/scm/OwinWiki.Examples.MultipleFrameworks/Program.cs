using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Nancy.Owin;
using Microsoft.AspNet.SignalR.Owin;

namespace OwinWiki.Examples.MultipleFrameworks
{
    public class Program
    {
        static void Main(string[] args)
        {
            using (WebApp.Start<Program>("http://localhost:9876"))
            {
                Console.WriteLine("Serving on http://localhost:9876.");
                Console.WriteLine("Press [space] to exit.");
                while (Console.ReadKey(true).KeyChar != ' ') ;
            }
        }

        public void Configuration(IAppBuilder katana)
        {
            // static files (html/js)
            katana.UseStaticFiles("/files");

            // To use embedded resources instead of files
            //katana.UseStaticFiles(new Microsoft.Owin.StaticFiles.StaticFileOptions()
            //{
            //    FileSystem = new Microsoft.Owin.FileSystems.EmbeddedResourceFileSystem("OwinWiki.Examples.MultipleFrameworks.files")
            //});

            // if the signalR app was hosted in a seperate website (Cross Site Origin Scripting)
            // Install-Package Microsoft.Owin.Cors.SignalR.Selfhost
            //katana.UseCors();

            // use signal r for chat path
            katana.MapSignalR("/Chat", new Microsoft.AspNet.SignalR.HubConfiguration());

            // use nancy for all others
            katana.UseNancy(options => 
            {
                options.PerformPassThrough = context =>
                    context.Response.StatusCode == Nancy.HttpStatusCode.NotFound;
            });

            // if nancy not found, use custom 404 generator
            katana.Run(pipeline => Task.Run(() => pipeline.Response.StatusCode = 404));
        }
    }
}
