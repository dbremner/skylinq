using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using SkyLinq.Web.Http;

namespace SkyLinq.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Formatters.Insert(0, new CsvFormatter());

            // Web API routes
            config.MapHttpAttributeRoutes();

            //config.Routes.MapHttpRoute(
            //    name: "DefaultApi",
            //    routeTemplate: "api/{controller}/{id}",
            //    defaults: new { id = RouteParameter.Optional }
            //);
            config.Routes.MapHttpRoute(
                name: "LogReportApi",
                routeTemplate: "api/{controller}/{report}"
            );
        }
    }
}
