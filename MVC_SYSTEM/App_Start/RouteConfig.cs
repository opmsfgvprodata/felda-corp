using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_SYSTEM
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
            name: "Language",
            url: "{lang}/{controller}/{action}/{id}",
            defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional },
            constraints: new { lang = @"ms|en|id" }
        );

            routes.MapRoute(
            name: "Files",
            url: "{lang}/Files/",
            defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional },
            constraints: new { lang = @"ms|en|id" }
        );

            routes.MapRoute(
            name: "SystemConfigs",
            url: "{lang}/Admin/SystemConfigs/{action}/{id}",
            defaults: new { controller = "SystemConfigs", action = "Index", id = UrlParameter.Optional },
            constraints: new { lang = @"ms|en|id" }
        );

            routes.MapRoute(
           name: "User",
           url: "{lang}/Admin/Users/{action}/{id}",
           defaults: new { controller = "User", action = "Index", id = UrlParameter.Optional },
           constraints: new { lang = @"ms|en|id" }
       );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Login", action = "Index", id = UrlParameter.Optional, lang = "ms" }
            );

        }
    }
}
