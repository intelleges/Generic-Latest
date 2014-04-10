using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class WrongContactController : Controller
    {
        //
        // GET: /WrongContact/

        public ActionResult Index(string accessCode = null)
        {
            ViewBag.accesscode = accessCode;
            return View();
        }

    }
}
