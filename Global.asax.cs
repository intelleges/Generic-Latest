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
            container.RegisterInstance(typeof(Google.Apis.Translate.v2.TranslateService), new Google.Apis.Translate.v2.TranslateService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleTranslateApiKey"]
            }));
            container.EnableMvc();

            Helpers.CurrentInstance.IsGeneric = 1;
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("elmah.axd");
        }

         


      }
}