using Nancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OwinWiki.Examples.MultipleFrameworks
{
    public class HomeModule : NancyModule
    {
        public HomeModule()
        {
            Get["/Nancy"] = _ => "Hello world";

            Get["/Nancy/{name}"] = _ => "Hello " + _.name + "!";
        }
    }
}
