using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{section}/{controller}/{action}/{id}",
                defaults: new { section = "Home", controller = "Index", action = "Index", id = UrlParameter.Optional }
            );

            //routes.MapRoute(
            //    name: "Home",
            //    url: "",
            //    defaults: new { section = "Home", controller = "Index", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new string[] { "MVC.Controllers.Home" }
            //);

            //routes.MapRoute(
            //    name: "Admin",
            //    url: "Admin/{controller}/{action}/{id}",
            //    defaults: new { section = "Admin", controller = "Index", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new string[] { "MVC.Controllers.Admin" }
            //);

            //routes.MapRoute(
            //    name: "Dashboards",
            //    url: "Dashboards/{controller}/{action}/{id}",
            //    defaults: new { section = "Dashboards", controller = "Index", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new string[] { "MVC.Controllers.Dashboards" }
            //);

            //routes.MapRoute(
            //    name: "Reports",
            //    url: "Reports/{controller}/{action}/{id}",
            //    defaults: new { section = "Reports", controller = "Index", action = "Index", id = UrlParameter.Optional },
            //    namespaces: new string[] { "MVC.Controllers.Reports" }
            //);
        }
    }
}
