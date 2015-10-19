using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class CampaignRuleController : Controller
    {
        EntitiesDBContext db = new EntitiesDBContext();

        
        //
        // GET: /CampaignRule/
        protected void LoadDropDowns()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll
              (Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");

            ViewBag.campaign = new SelectList(db.pr_getCampaignAll
              (Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            ViewBag.partnerType = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");
            ViewBag.logicList = new SelectList(db.pr_getLogicAll().ToList(), "id", "description");
            ViewBag.status = new SelectList(db.pr_getPartnerStatusAll().ToList(), "id", "description");
        }

        public ActionResult Index()
        {
            var campaignRuleList = db.pr_getCampaignRuleAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            //return Json(campaignRuleList, JsonRequestBehavior.AllowGet);
            return View(campaignRuleList);
        }


        public ActionResult Create()
        {
            LoadDropDowns();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Generic.campaignRule model)
        {
            try
            {
                ViewBag.error = "false";
                var error = "";
                var current = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(model.partnerTypeCurrent, model.touchpointCurrent).FirstOrDefault();
                var next = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(model.partnerTypeNext, model.touchpointNext).FirstOrDefault();
                if (next == null)
                {
                    error = "[Next] Partnertype Touchpoint Questionnaire does not exists";
                }
                if (current == null)
                {
                    if (!string.IsNullOrEmpty(error)) error += "<br>";
                    error += "[Current] Partnertype Touchpoint Questionnaire does not exists";
                }
                if (string.IsNullOrEmpty(error))
                {
                    var partnerTypeNext = db.pr_getPartnerType(model.partnerTypeNext).FirstOrDefault();
                    var partnerTypeCurrent = db.pr_getPartnerType(model.partnerTypeCurrent).FirstOrDefault();
                    var touchpointNext = db.pr_getTouchpoint(model.touchpointNext).FirstOrDefault();
                    var touchpointCurrent = db.pr_getTouchpoint(model.touchpointCurrent).FirstOrDefault();


                    db.pr_addCampaignRule(model.campaign, model.initTest, current.id, model.status,
                        model.statusLogic, model.score, model.scoreLogic, model.responseInterval, model.responseIntervalLogic, model.straightline, model.delayInterval, model.delayIntervalLogic, next.id, model.hardEndDate, model.switchOffDate, 
                        1, true);
                    ViewBag.message = string.Format("Congratulations you have successfully added campaign rule that sequences from (current) {0} for {1} to (next) {2} for {3}", partnerTypeCurrent.name, touchpointCurrent.description, partnerTypeNext.name, touchpointNext.description);
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
