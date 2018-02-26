using Elmah;
using Generic.Helpers;
using Google.Apis.Services;
using LightInject;
using LightInject.Mvc;
using LightInject.Web;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Generic
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            var container = new ServiceContainer();
            container.RegisterControllers();
            //register other services
            container.Register<IDatabaseTranslationService, DatabaseTranslationService>(new PerScopeLifetime());
            container.Register<IGoogleTranslatorHelper, GoogleTranslatorHelper>(new PerScopeLifetime());            
            container.EnablePerWebRequestScope();
            container.EnableMvc();
			
            Helpers.CurrentInstance.IsGeneric = 1;
            AreaRegistration.RegisterAllAreas();

            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			GlobalConfiguration.Configuration.EnsureInitialized();
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("elmah.axd");
        }

        protected void ErrorLog_Logged(object sender, ErrorLoggedEventArgs args)
        {
            args.Entry.Error.Exception.Data.Add("IncidentId", args.Entry.Id);           
            HttpContext.Current.Cache.Insert("IncidentId", args.Entry.Id, null, DateTime.Now.AddMinutes(10d), Cache.NoSlidingExpiration);
        }
        protected void ErrorMail_Mailing(object sender, ErrorMailEventArgs e)
        {
            e.Mail.Subject = e.Error.Exception.Data["IncidentId"].ToString();
        }
    }
}