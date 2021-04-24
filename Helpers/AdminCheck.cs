using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MMS.Helpers
{
    public class AdminCheck : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (HttpContext.Current.Session["EmpID"] == null)
            {
               
                filterContext.Result =
               new RedirectToRouteResult(new RouteValueDictionary
                 {{ "action", "Login" },{ "controller", "MyAccount" },{ "returnUrl", filterContext.HttpContext.Request.RawUrl}});

                return;
            }
            else
            {
                if (HttpContext.Current.Session["EmpID"].ToString() != "EMP0000001")
                {
                    filterContext.Result =
                     new RedirectToRouteResult(new RouteValueDictionary
                        {{ "action", "MyDashBoard" },{ "controller", "Home" },{ "returnUrl", filterContext.HttpContext.Request.RawUrl}});

                    return;
                }
            }
        }
    }
}