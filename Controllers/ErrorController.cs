using Generic.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        private readonly ILoggingService _logger;

        public ErrorController(ILoggingService logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public ActionResult Index()
        {
            _logger.LogWarning("Generic error page displayed");
            return View("Index");
        }

        public ActionResult NotFound()
        {
            _logger.LogWarning("404 error page displayed for URL: {Url}", Request.RawUrl);
            Response.StatusCode = 404;
            return View("NotFound");
        }

        public ActionResult ServerError()
        {
            _logger.LogWarning("500 error page displayed");
            Response.StatusCode = 500;
            return View("ServerError");
        }
    }
}
