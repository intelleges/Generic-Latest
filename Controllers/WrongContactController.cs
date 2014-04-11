using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Helpers.Questionnaire;
using Generic.Helpers.Utility;
using Generic.Models;

namespace Generic.Controllers
{
    public class WrongContactController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /WrongContact/

        public ActionResult Index(string accessCode = null)
        {
            ViewBag.accesscode = accessCode;

            partner objPartner = new partner();

            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

            ViewBag.CMS_PAGE_TITLE = CMS.REDIRECT_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.REDIRECT_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.REDIRECT_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.REDIRECT_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_HEADER = CMS.REDIRECT_PAGE_HEADER;
            ViewBag.CMS_PAGE_HEADER_TEXT = CMS.REDIRECT_PAGE_HEADER_TEXT;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.REDIRECT_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.REDIRECT_PAGE_NEXT_TEXT;


            if (ppptq != null)
            {

              
                


                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var touchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();

                if (touchpoint.endDate >= DateTime.Now)
                {
                    Session["partner"] = ppptq.partner;
                    Session["accesscode"] = accessCode;
                    Session["touchpoint"] = touchpoint.id;
                    Session["partnerType"] = ptq.partnerType;
                    objPartner = db.pr_getPartner(ppptq.partner).FirstOrDefault();
                }

                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();

                try
                {
                    var cms_PageTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_TITLE).id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle.text;
                    var cms_PageSubtitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_SUBTITLE).id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle.text;
                    var cms_PagePanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_PANEL_ONE).id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne.text;
                    var cms_PagePanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_PANEL_TWO).id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo.text;

                    var cms_PageHeader = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_HEADER).id);
                    if (cms_PageHeader != null)
                        ViewBag.CMS_PAGE_HEADER = cms_PageHeader.text;
                    var cms_PageHeaderText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_HEADER_TEXT).id);
                    if (cms_PageHeaderText != null)
                        ViewBag.CMS_PAGE_HEADER_TEXT = cms_PageHeaderText.text;
                    var cms_PagePreviuosText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_PREVIOUS_TEXT).id);
                    if (cms_PagePreviuosText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviuosText.text;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText.text;


                }
                catch
                {
                }


            }

            return View(objPartner);
        }

        [HttpPost]
        public ActionResult Index(partner partner, string accessCode = null)
        {
            partner objpartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            objpartner.firstName = partner.firstName;
            objpartner.lastName = partner.lastName;
            objpartner.title = partner.title;
            objpartner.email = partner.email;
            objpartner.phone = partner.phone;
            objpartner.fax = partner.fax;
           
            if (ModelState.IsValid)
            {
                db.Entry(objpartner).State = EntityState.Modified;
                db.SaveChanges();

                int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint((int)Session["partnerType"], (int)Session["touchpoint"]).FirstOrDefault().id;
                var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(objpartner.id, ptq).FirstOrDefault();
              
              

                //var objpartner = db.pr_getPartner(partnerId).FirstOrDefault();
                //objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                //db.Entry(objpartner).State = EntityState.Modified;
                //db.SaveChanges();

                var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();

                var objtouchpoint = db.pr_getTouchpoint((int)Session["touchpoint"]).FirstOrDefault();
                Email email = new Email(amm);
                EmailFormat emailFormat = new EmailFormat();

                person objperson = new person();

                email.body = emailFormat.sGetEmailBody(email.body, objperson, objpartner, objtouchpoint, ptq);
                email.emailTo = objpartner.email;
                SendEmail objSendEmail = new SendEmail();
                objSendEmail.sendEmail(email);


                return RedirectToAction("RedirectConfirmation");
               
            }
            return View(partner);
        }

        public ActionResult RedirectConfirmation()
        {
            if (Session["accesscode"] == null)
            {
                return RedirectToAction("Index");
            }


            ViewBag.CMS_PAGE_TITLE = CMS.REDIRECT_CONFIRMATION_PAGE_TITLE.Substring(0, 20);
            ViewBag.CMS_PAGE_SUBTITLE = CMS.REDIRECT_CONFIRMATION_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.REDIRECT_CONFIRMATION_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.REDIRECT_CONFIRMATION_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_HEADER = CMS.REDIRECT_CONFIRMATION_PAGE_HEADER;
            ViewBag.CMS_PAGE_HEADER_TEXT = CMS.REDIRECT_CONFIRMATION_PAGE_HEADER_TEXT;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.REDIRECT_CONFIRMATION_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.REDIRECT_CONFIRMATION_PAGE_NEXT_TEXT;


            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accesscode"].ToString()).FirstOrDefault();

            if (ppptq != null)
            {

                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var touchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();

                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();

                try
                {
                    var cms_PageTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_TITLE).id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle.text.Substring(0, 20);
                    var cms_PageSubtitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_SUBTITLE).id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle.text;
                    var cms_PagePanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_PANEL_ONE).id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne.text;
                    var cms_PagePanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_PANEL_TWO).id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo.text;

                    var cms_PageHeader = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_HEADER).id);
                    if (cms_PageHeader != null)
                        ViewBag.CMS_PAGE_HEADER = cms_PageHeader.text;
                    var cms_PageHeaderText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_HEADER_TEXT).id);
                    if (cms_PageHeaderText != null)
                        ViewBag.CMS_PAGE_HEADER_TEXT = cms_PageHeaderText.text;
                    var cms_PagePreviuosText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_PREVIOUS_TEXT).id);
                    if (cms_PagePreviuosText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviuosText.text;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.REDIRECT_CONFIRMATION_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText.text;

                }
                catch
                {
                }
            }


            return View();
        }


    }
}
