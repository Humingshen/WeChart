using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wshare
{
    public class WeChatAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies["user"] == null)
            {

                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary {
                        { "action", "Index" },
                        { "controller", "Auth" },
                        { "url", filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri }
                    }
                );
                return;
            }
        }
    }
}