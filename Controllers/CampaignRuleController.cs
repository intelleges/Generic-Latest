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


        public ActionResult Archive(int id)
        {
            //db.pr_unArchiveCampaignRule
            db.pr_archiveCampaignRule(id);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /CampaignRule/Delete/5

        public ActionResult Delete(int id=0)
        {
            db.pr_removeCampaignRule(id);
            return RedirectToAction("Index");
        }



        //
        // POST: /CampaignRule/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            campaignRule campaignRule = db.pr_getCampaignRule(id).FirstOrDefault();
            if (campaignRule != null)
            {
                db.pr_removeCampaignRule(campaignRule.id);
                db.SaveChanges();
            }
            return RedirectToAction("Index");

        }
        //
        // GET: /Campaign/Edit/5

        public ActionResult Edit(int id = 0)
        {
            campaignRule campaignRule = db.pr_getCampaignRule(id).FirstOrDefault();
            if (campaignRule == null)
            {
                return HttpNotFound();
            }
            //   ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            ViewBag.id = campaignRule.id;
            //new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            ViewBag.ptqCurrent = campaignRule.ptqCurrent;
            ViewBag.ptqNext = campaignRule.ptqNext;
            touchpoint tpCurrent = db.pr_getTouchpointByPTQ(campaignRule.ptqCurrent).FirstOrDefault();
            touchpoint tpNext = db.pr_getTouchpointByPTQ(campaignRule.ptqNext).FirstOrDefault();

            ViewBag.touchpointCurrent = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description", tpCurrent != null ? tpCurrent.id : 0);
            ViewBag.touchpointNext = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description", tpNext != null ? tpNext.id : 0);

            protocol ptCurrent = db.pr_getProtocolByTouchpoint(tpCurrent.id).FirstOrDefault();
            protocol ptNext = db.pr_getProtocolByTouchpoint(tpNext.id).FirstOrDefault();

            ViewBag.currentProtocol = new SelectList(db.pr_getProtocolAll
              (Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description", ptCurrent != null ? ptCurrent.id : 0);
            ViewBag.nextProtocol = new SelectList(db.pr_getProtocolAll
              (Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description", ptNext != null ? ptNext.id : 0);

            partnerType prtCurrent = db.pr_getPartnerTypeByPTQ(campaignRule.ptqCurrent).FirstOrDefault();
            partnerType prtNext = db.pr_getPartnerTypeByPTQ(campaignRule.ptqNext).FirstOrDefault();

            ViewBag.partnerTypeCurrent = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name", prtCurrent != null ? prtCurrent.id : 0);
            ViewBag.partnerTypeNext = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name", prtNext != null ? prtNext.id : 0);

            ViewBag.campaign = new SelectList(db.pr_getCampaignAll
              (Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description", campaignRule.campaign);

            ViewBag.statusLogic = new SelectList(db.pr_getLogicAll().ToList(), "id", "description", campaignRule.statusLogic);
            ViewBag.scoreLogic = new SelectList(db.pr_getLogicAll().ToList(), "id", "description", campaignRule.scoreLogic);
            ViewBag.responseIntervalLogic = new SelectList(db.pr_getLogicAll().ToList(), "id", "description", campaignRule.responseIntervalLogic);
            ViewBag.delayIntervalLogic = new SelectList(db.pr_getLogicAll().ToList(), "id", "description", campaignRule.delayIntervalLogic);

            ViewBag.initTest = campaignRule.initTest;
            

            ViewBag.status = new SelectList(db.pr_getPartnerStatusAll().ToList(), "id", "description", campaignRule.status);
            ViewBag.score = campaignRule.score;
            ViewBag.responseInterval = campaignRule.responseInterval;
            ViewBag.straightline = campaignRule.straightline;
            ViewBag.delayInterval = campaignRule.delayInterval;
            ViewBag.active = campaignRule.active;
            ViewBag.active = campaignRule.hardEndDate;
            ViewBag.active = campaignRule.switchOffDate;
            
            return View(campaignRule);
        }

        //
        // POST: /Partner/Edit/5


        [HttpPost]
        public ActionResult Edit(Generic.campaignRule model)
        {
            if (ModelState.IsValid)
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


                    db.pr_modifyCampaignRule(model.id, model.campaign, model.initTest, current.id, model.status,
                        model.statusLogic, model.score, model.scoreLogic, model.responseInterval, model.responseIntervalLogic, model.straightline, model.delayInterval, model.delayIntervalLogic, next.id, model.hardEndDate, model.switchOffDate,
                        1, true);

                    string msg =  string.Format("Congratulations you have successfully updated campaign rule that sequences from (current) {0} for {1} to (next) {2} for {3}", partnerTypeCurrent.name, touchpointCurrent.description, partnerTypeNext.name, touchpointNext.description);

                    return Json(new { success = true, message = msg }, JsonRequestBehavior.AllowGet);

                }
                else
                {
                    ViewBag.message = error;
                    ViewBag.error = "true";
                }

                if (error != "")
                {
                    return Json(new { error = error });
                }
            }
            return View(model);
        }
    }
}
