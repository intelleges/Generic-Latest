using Generic.Filters;
using Generic.Helpers;
using System.Web;
using System.Web.Mvc;


namespace Generic
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            var logger = DependencyResolver.Current.GetService<ILoggingService>();
            filters.Add(new CustomHandleErrorAttribute(logger));

        }
    }
}