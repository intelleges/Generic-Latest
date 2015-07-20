using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Helpers.Utility;
using Generic.Helpers;

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
            ViewBag.multiTenantProjectType = new SelectList(db.multiTenantProjectType.ToList(), "id", "description");
            ViewBag.product = new SelectList(db.pr_getProductAll().ToList(), "id", "description");
            ViewBag.GroupId = new SelectList(db.pr_getGroupByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");
            return View();
        }
    }
}