using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class zCodeCheckController : Controller
    {
        EntitiesDBContext db = new EntitiesDBContext();
        protected void LoadDropDowns()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll
              (Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");

            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            ViewBag.partnerType = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");

            ViewBag.zCodeCheckActionTypes = new SelectList(db.pr_getZcodeCheckActionTypeAll().ToList(), "id", "description");
        }
        // GET: ZCodeCheck
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Create()
        {
            LoadDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Generic.invalidZcode model)
        {
            try
            {
                ViewBag.error = "false";
                var error = "";
                var current = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(model.partnerTypeCurrent, model.touchpointCurrent).FirstOrDefault();
                if (current == null)
                {
                    if (!string.IsNullOrEmpty(error)) error += "<br>";
                    error += "[Current] Partnertype Touchpoint Questionnaire does not exists";
                }
                if (string.IsNullOrEmpty(error))
                {
                    var partnerTypeCurrent = db.pr_getPartnerType(model.partnerTypeCurrent).FirstOrDefault();
                    var touchpointCurrent = db.pr_getTouchpoint(model.touchpointCurrent).FirstOrDefault();
                    var zCheckActionType = db.pr_getZcodeCheckActionType(model.zCodeActionType).FirstOrDefault();

                    db.pr_addInvalidZcode(current.id, model.zCode, model.zCodeActionType, zCheckActionType.sortOrder, zCheckActionType.active);
                    ViewBag.message = string.Format("Congratulations for {0} you just added this invalid {1}", touchpointCurrent.title, model.zCode);

                }
                else
                {
                    ViewBag.message = error;
                    ViewBag.error = "true";
                }
            }
            catch
            {
                ViewBag.message = "Unhandled error";
                ViewBag.error = "true";
            }
            LoadDropDowns();
            return View();
        }
    }
}