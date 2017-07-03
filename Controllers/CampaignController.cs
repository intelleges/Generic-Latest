using Generic.Helpers.PartnerHelper;
using Generic.Helpers.Utility;
using Generic.Models;
using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            //ViewBag.protocol = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            //ViewBag.status = new SelectList(db.pr_getCampaignStatusAll().ToList(), "id", "description");
            return View();
        }

        [HttpPost]
        public ActionResult Create(Generic.campaign model)
        {

            try
            {

                db.pr_addCampaign(model.description, model.startDate, model.endDate, SessionSingleton.LoggedInUserId, SessionSingleton.LoggedInUserId, 1, true).FirstOrDefault();
                // db.pr_addCampaign(model.description, model.year, model.sortOrder, true, model.protocol).FirstOrDefault();
                ViewBag.message = "Congratulations you have successfully added " + model.description;
            }
            catch
            {
                ViewBag.message = "Unhandled error";
            }
            //ViewBag.protocol = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            //ViewBag.status = new SelectList(db.pr_getCampaignStatusAll().ToList(), "id", "description");
            return View();
        }



        public ActionResult Index()
        {
            //var currentTouchpoints = db.pr_getTouchpointAllByEnterprise(Helpers.CurrentInstance.EnterpriseID).Select(o => o.id).ToList();
            //var campaignList = db.campaigns.ToList();
            var campaignList = db.pr_getCampaignAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            return View(campaignList);
        }

        public ActionResult Edit(int id)
        {
            ViewBag.protocol = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            ViewBag.statuses = new SelectList(db.pr_getCampaignStatusAll().ToList(), "id", "description");
            var model = db.pr_getCampaign(id).FirstOrDefault();
            return View(model);
        }
        [HttpPost]
        public ActionResult Edit(int id, campaign model)
        {
            ViewBag.protocol = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "description");
            ViewBag.statuses = new SelectList(db.pr_getCampaignStatusAll().ToList(), "id", "description");
            var prev = db.pr_getCampaign(id).FirstOrDefault();
            
            db.pr_modifyCampaign(id, model.description,  model.startDate, model.endDate, model.author, model.owner, prev.sortOrder, prev.active);           
            return View();
        }

        private int? CurrentCampaignID
        {
            get
            {
                if (Session["CurrentCampaignID"] == null)
                    Session["CurrentCampaignID"] = 0;
                return int.Parse(Session["CurrentCampaignID"].ToString());
            }
            set
            {
                Session["CurrentCampaignID"] = value;
            }
        }
        private int? CurrentPartnerType
        {
            get
            {
                if (Session["CurrentPartnerType"] == null)
                    Session["CurrentPartnerType"] = 0;
                return int.Parse(Session["CurrentPartnerType"].ToString());
            }
            set
            {
                Session["CurrentPartnerType"] = value;
            }
        }

        public ActionResult SearchPartner()
        {
            //string text = " ";
            //if (Request["filter[filters][0][value]"] != null)
            //    text = Request["filter[filters][0][value]"].ToString();
            var partners = db.partner.Where(o => o.enterprise == Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            return Json(partners.Select(o => new { id = o.id, FullName = o.FullName }), JsonRequestBehavior.AllowGet);
            //return new JsonResult { Data = new SelectList(partners, "id", "FullName") };
        }

        public ActionResult CurrentPtqList()
        {
            var ptqs = db.partnerTypeTouchpointQuestionnaire.Where(o => o.questionnaire1.enterprise == Generic.Helpers.CurrentInstance.EnterpriseID).Select(o => new {id=o.id,title= o.questionnaire1.title }).ToList();

            return new JsonResult { Data = new SelectList(ptqs, "id", "title") };
        }

        

        public ActionResult CreateEstimationQuestionnaire(int qid, string[] partners, int campaign)
        {
            try
            {
                var loadGroup = db.pr_getAccesscode().FirstOrDefault();
                var tiuhcpoint = 0;
                foreach (var partnerid in partners)
                {
                    var pid = int.Parse(partnerid);
                    var newCode = db.pr_getAccesscode().FirstOrDefault();
                    var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(pid).FirstOrDefault();
                    if (pptq == null)
                    db.pr_addPartnerPartnertypeTouchpointQuestionnaire(pid, qid, newCode, SessionSingleton.LoggedInUserId, DateTime.Now, DateTime.Now.AddDays(2), (int)PartnerStatus.Invited_NoResponse, 0, "", null, "", 0, loadGroup).FirstOrDefault();
                    else
                    {
                        
                        var changed = db.pr_replacePTQforPPTQ(pptq.id, pptq.partnerTypeTouchpointQuestionnaire, qid).FirstOrDefault();
                        changed.status = (int)PartnerStatus.Invited_NoResponse;
                        changed.invitedDate = DateTime.Now;
                        changed.completedDate = DateTime.Now.AddDays(2);
                        changed.loadGroup = loadGroup;
                        db.Entry(changed).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                var objPartners = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).ToList();
                foreach (var partnerItem in objPartners)
                {
                    var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerItem.partner, qid).FirstOrDefault();
                    pptq.invitedDate = DateTime.Now;
                    var person = db.pr_getPersonByEmail(Helpers.CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
                    pptq.invitedBy = person.id;
                    pptq.status = (int)PartnerStatus.Invited_NoResponse;
                    db.Entry(pptq).State = EntityState.Modified;
                    db.SaveChanges();

                    var objpartner = db.pr_getPartner(partnerItem.partner).FirstOrDefault();
                    objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                    db.Entry(objpartner).State = EntityState.Modified;
                    db.SaveChanges();

                    var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, qid).FirstOrDefault();
                    if (amm == null) return Json(new { error = "There is no automail invitation message for selected questionnaire. Please apply one and try again." });
                    amm.text.Replace("[partner Access Code]", partnerItem.accesscode);
                    var objpartnerByAccessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireDueDateByAccessCode(partnerItem.accesscode, loadGroup).FirstOrDefault();
                    var objtouchpoint = db.pr_getTouchpoint(pptq.partnerTypeTouchpointQuestionnaire1.touchpoint).FirstOrDefault();
                    Email email = new Email(amm);
                    email.loadgroup = loadGroup;
                    email.accesscode = partnerItem.accesscode;
                    email.protocolTouchpoint = objtouchpoint.description;
                    EmailFormat emailFormat = new EmailFormat();
                    email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, qid);
                    email.emailTo = objpartner.email;
					email.url = Request.Url.ToString();
					email.automailMessage = amm.id.ToString();
					email.category = SendGridCategory.CreaeEstimaionQuestionnaie;

                    SendEmail objSendEmail = new SendEmail();
                    objSendEmail.sendEmail(email);
					/*db.pr_addEventNotification(email.emailTo, DateTime.Now,null, null, email.url, ((int)email.category).ToString(), email.accesscode, email.protocolTouchpoint, "MVCMT", null, amm.id, Helpers.CurrentInstance.EnterpriseID, email.loadgroup);*/
                    var currentCompaign = db.pr_getCampaign(campaign).FirstOrDefault();
                    var cuurentTouchpoint = pptq.partnerTypeTouchpointQuestionnaire1.touchpoint1;
                    db.pr_addCampaignRule(campaign, cuurentTouchpoint.partnerTypeTouchpointQuestionnaire.FirstOrDefault().id, 0, 0, 0, 0, 0, 0, 0, true, 0, pptq.partnerTypeTouchpointQuestionnaire1.id, 1, DateTime.Now, DateTime.Now, 1, true).FirstOrDefault();                    
                }

                
            }
            catch (Exception ex)
            {
               return  Json(new { error = ex.InnerException == null ? ex.Message : ex.Message + " " + ex.InnerException.Message });
            }
            return Json(true);
        }

    }
}
