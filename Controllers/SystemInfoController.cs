using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.SessionClass;

namespace Generic.Controllers
{
    public class SystemInfoController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /SystemInfo/

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult FindSystemInfo()
        {
            ViewBag.Test = "Hi";
            return View();
        }



    }
}
