using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace AplicationNortwind.Filters
{
    public class SecurityCoockie : ActionFilterAttribute
    {
        public const string cookie = "AuthID";
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            var consentCookie = filterContext.HttpContext.Request.Cookies[cookie];
            var IDsesion = filterContext.HttpContext.Session.Contents[cookie];

            if (consentCookie != null && IDsesion != null)
            {
                if (consentCookie.Value.ToString() != IDsesion.ToString())
                {
                    var routeValues = new RouteValueDictionary
                    {
                        ["controller"] = "Home",
                        ["action"] = "Login"
                    };
                    filterContext.Result = new RedirectToRouteResult(routeValues);
                }
            }
            else
            {
                var routeValues = new RouteValueDictionary
                {
                    ["controller"] = "Home",
                    ["action"] = "Login"
                };
                filterContext.Result = new RedirectToRouteResult(routeValues);
            }

        }
    }
}