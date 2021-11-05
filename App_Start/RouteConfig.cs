using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Generic
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute("GenericNew", // Route name 
                "{controller}/{action}/{id}",
                // URL with parameters 
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional },// Parameter defaults 
                null,
                new string[] { "Generic.Controllers" }  //NOTE: namespace to check 
                );

            routes.MapRoute("GenericNew2", // Route name 
                "{controller}/{action}/{id}/{groupid}",
                // URL with parameters 
                new { controller = "Admin", action = "Index", id = UrlParameter.Optional, groupid = UrlParameter.Optional },// Parameter defaults 
                null,
                new string[] { "Generic.Controllers" }  //NOTE: namespace to check 
                );


            routes.MapRoute("GenericNew3", // Route name 
                "{controller}/{action}/{id}",
                // URL with parameters 
                new { controller = "Saml2", action = "Signin", id = UrlParameter.Optional},// Parameter defaults 
                null,
                new string[] { "Sustainsys.Saml2.Mvc" }  //NOTE: namespace to check 
                );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Admin", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}