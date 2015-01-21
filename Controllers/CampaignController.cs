using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class CampaignController : Controller
    {
        EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /Campaign/

        public ActionResult Create()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Generic.campaign model)
        {

            try
            {
                db.pr_addCampaign(model.description, model.year, model.sortOrder, true, model.protocol).FirstOrDefault();
                ViewBag.message = "Congratulations you have successfully added " + model.description;
            }
            catch
            {
                ViewBag.message = "Unhandled error";
            }
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            return View();
        }

    }
}
