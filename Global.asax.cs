using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BusinessObjects.Entity;
using System.Data.Entity;
using System.Web.Http;

namespace MMS
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            

            //Code First Database Migration

            Database.SetInitializer<Model1>(null);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            var path=Server.MapPath("~/ErrorLogs/");

            //var sr= System.IO.File.CreateText(path + "file_" +Guid.NewGuid().ToString() + ".txt");
            //sr.WriteLine("Error Message:" + exception.Message + "; Source= " + exception.Source);
            //sr.Close();

            Server.ClearError();
            Response.Redirect("~/ErrorPage/404.html");
        }
    }
}
