using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class ContractController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        // GET: Contract
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            ViewBag.subscriptionStatus = new SelectList(db.subscriptionStatus.ToList(), "id", "description");
            ViewBag.subscriptionType = new SelectList(db.subscriptionType.ToList(), "id", "description");
            return View();
        }
    }
}