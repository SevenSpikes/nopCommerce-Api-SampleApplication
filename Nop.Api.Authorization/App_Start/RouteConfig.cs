using System.Web.Mvc;
using System.Web.Routing;

namespace Nop.Api.Authorization
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Submit",
                url: "submit",
                defaults: new { controller = "Authorization", action = "Submit" }
            );

            routes.MapRoute(
               name: "GetAccessToken",
               url: "api/token",
               defaults: new { controller = "Authorization", action = "GetAccessToken" }
           );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Authorization", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
