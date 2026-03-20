using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace A25082
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route for Admin controllers without Admin prefix (for existing URLs)
            routes.MapRoute(
                name: "AdminDirect",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "A25082.Admin.Controllers" },
                constraints: new { controller = @"^(HomeAdmin|AdminProduct|AdminVoucher|AdminPayMentOrder|AdminRole|AdminUser|AdminProfile|AdminSubCategories|ImageCategories|AdminImageSlider|AdminImageProduct|AdminProductSize|AdminProductColor)$" }
            );

            // Default route for regular controllers (including ImageSlider)
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "A25082.Controllers" }
            );
        }
    }
}
