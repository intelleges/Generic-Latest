using Generic.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    /// <summary>
    /// Base controller providing standardized error handling and logging support.
    /// All controllers should inherit from BaseController.
    /// </summary>
    public abstract class BaseController : Controller
    {
        protected readonly ILoggingService Logger;

        protected BaseController(ILoggingService logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Standardized method to handle exceptions across controllers.
        /// Logs the exception and returns a proper response based on request type (AJAX or normal).
        /// </summary>
        protected ActionResult HandleException(Exception ex, string errorMessage = null)
        {
            Logger.LogError(ex, errorMessage ?? "An unexpected error occurred while processing the request.");

            if (Request.IsAjaxRequest())
            {
                return JsonError(errorMessage ?? "An unexpected error occurred while processing your request.");
            }

            return RedirectToAction("Index", "Error");
        }

        /// <summary>
        /// Returns a standardized JSON error response.
        /// Useful for AJAX error handling.
        /// </summary>
        protected JsonResult JsonError(string message, object additionalData = null)
        {
            var response = new
            {
                success = false,
                message = message,
                data = additionalData
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns a standardized JSON success response.
        /// Useful for consistent successful AJAX operations.
        /// </summary>
        protected JsonResult JsonSuccess(string message = "Operation completed successfully.", object additionalData = null)
        {
            var response = new
            {
                success = true,
                message = message,
                data = additionalData
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Returns a HTTP 400 Bad Request result with optional error message.
        /// Useful for model validation or business validation failures.
        /// </summary>
        protected ActionResult BadRequest(string message)
        {
            Response.StatusCode = 400; // Bad Request
            return Content(message);
        }
    }

}