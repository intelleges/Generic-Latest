
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Filters
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        private readonly Helpers.ILoggingService _logger;

        public CustomHandleErrorAttribute(Helpers.ILoggingService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException(nameof(filterContext));
            }

            // Log the exception
            _logger.LogError(
                filterContext.Exception,
                "Unhandled exception in {Controller}.{Action}: {Message}",
                filterContext.RouteData.Values["controller"],
                filterContext.RouteData.Values["action"],
                filterContext.Exception.Message);

            // Get the exception details
            var controllerName = (string)filterContext.RouteData.Values["controller"];
            var actionName = (string)filterContext.RouteData.Values["action"];
            var model = new HandleErrorInfo(filterContext.Exception, controllerName, actionName);

            // Determine if we should show detailed error information
            bool showDetailedErrors = filterContext.HttpContext.IsDebuggingEnabled;

            // Create the result based on the request type
            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                // For AJAX requests, return a JSON result
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        success = false,
                        message = showDetailedErrors ? filterContext.Exception.Message : "An error occurred while processing your request.",
                        stackTrace = showDetailedErrors ? filterContext.Exception.StackTrace : null
                    },
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }
            else
            {
                // For regular requests, show the error view
                filterContext.Result = new ViewResult
                {
                    ViewName = View,
                    MasterName = Master,
                    ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                    TempData = filterContext.Controller.TempData
                };
            }

            // Mark the exception as handled
            filterContext.ExceptionHandled = true;

            // Clear the error on the response
            filterContext.HttpContext.Response.Clear();
            filterContext.HttpContext.Response.StatusCode = 500;
            filterContext.HttpContext.Response.TrySkipIisCustomErrors = true;
        }
    }
}