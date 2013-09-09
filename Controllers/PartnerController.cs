using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class PartnerController : Controller
    {
        //
        // GET: /Partner/

        public virtual ActionResult Index()
        {
            ViewBag.Project = "Generic";
            return View();
        }

    }
}
