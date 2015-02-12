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

        public ActionResult Index()
        {
            return RedirectToAction("PageNotFound");
        }

        public ActionResult PageNotFound()
        {
            return View();
        }

        public ActionResult InternalError()
        {
            return View();
        }

    }
}
