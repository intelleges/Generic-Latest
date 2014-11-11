using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Generic.DataLayer;
using Generic.Models;
using Generic.Helpers.Questionnaire;
using Generic.Helpers.Utility;
using Generic.Helpers;
using Generic.Helpers.PartnerHelper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Xml;
using iTextSharp.text.html.simpleparser;
using System.Text;
namespace Generic.Areas.RegistrationArea.Controllers
{
    public class HomeController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /RegistrationArea/Home/

        public virtual ActionResult Index(string id = "", string accessCode = null)
        {



            ViewBag.accesscode = accessCode;

            ViewBag.CMS_TITLE = CMS.ACCESS_CODE_TITLE;
            ViewBag.CMS_SUBTITLE = CMS.ACCESS_CODE_SUBTITLE;
            ViewBag.CMS_PANEL_ONE = CMS.ACCESS_CODE_PANEL_ONE;
            ViewBag.CMS_PANEL_TWO = CMS.ACCESS_CODE_PANEL_TWO;
            ViewBag.CMS_FOOTER_ONE = CMS.ACCESS_CODE_FOOTER_ONE;
            ViewBag.CMS_FOOTER_TWO = CMS.ACCESS_CODE_FOOTER_TWO;
            ViewBag.CMS_SUBMIT_TEXT = CMS.ACCESS_CODE_SUBMIT_TEXT.Substring(0, 10);
            ViewBag.RETRIEVE_ACCESS_CODE_TEXT = CMS.RETRIEVE_ACCESS_CODE_TEXT;


            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

            if (ppptq_cms != null)
            {
                Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(db.pr_getPartner(ppptq_cms.partner).FirstOrDefault().enterprise.ToString());

                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    var cms_Title = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_TITLE).id);
                    if (cms_Title != null)
                    {
                        ViewBag.CMS_TITLE = cms_Title.text;

                        Session["QuestionnaireTitle"] = cms_Title.text;

                    }
                    var cms_SubTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBTITLE).id);
                    if (cms_SubTitle != null)
                    {
                        ViewBag.CMS_SUBTITLE = cms_SubTitle.text;
                    }

                    var cms_PanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_ONE).id);
                    if (cms_PanelOne != null)
                    {
                        ViewBag.CMS_PANEL_ONE = cms_PanelOne.text;
                    }

                    var cms_PanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_TWO).id);
                    if (cms_PanelTwo != null)
                    {
                        ViewBag.CMS_PANEL_TWO = cms_PanelTwo.text;
                    }

                    var cms_FooterOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_ONE).id);
                    if (cms_FooterOne != null)
                    {
                        ViewBag.CMS_FOOTER_ONE = cms_FooterOne.text;
                    }

                    var cms_FooterTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_TWO).id);
                    if (cms_FooterTwo != null)
                    {
                        ViewBag.CMS_FOOTER_TWO = cms_FooterTwo.text;
                    }

                    var cms_SubmitText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBMIT_TEXT).id);
                    if (cms_SubmitText != null)
                    {
                        ViewBag.CMS_SUBMIT_TEXT = cms_SubmitText.text;
                    }

                    var ret_AccCode = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.RETRIEVE_ACCESS_CODE_TEXT).id);
                    if (ret_AccCode != null)
                    {
                        ViewBag.RETRIEVE_ACCESS_CODE_TEXT = ret_AccCode.text;
                    }
                }
                catch (Exception exc)
                {
                }
            }

            return View();
        }
        [HttpPost]
        public virtual ActionResult Index(string accessCode)
        {
            ViewBag.CMS_TITLE = CMS.ACCESS_CODE_TITLE;
            ViewBag.CMS_SUBTITLE = CMS.ACCESS_CODE_SUBTITLE;
            ViewBag.CMS_PANEL_ONE = CMS.ACCESS_CODE_PANEL_ONE;
            ViewBag.CMS_PANEL_TWO = CMS.ACCESS_CODE_PANEL_TWO;
            ViewBag.CMS_FOOTER_ONE = CMS.ACCESS_CODE_FOOTER_ONE;
            ViewBag.CMS_FOOTER_TWO = CMS.ACCESS_CODE_FOOTER_TWO;
            ViewBag.CMS_SUBMIT_TEXT = "Login";

            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
            if (ppptq != null)
            {
                var objPartner = db.pr_getPartner(ppptq.partner).FirstOrDefault();
                Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(objPartner.enterprise.ToString());


                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();

                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    var cms_Title = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_TITLE).id);
                    if (cms_Title != null)
                    {
                        ViewBag.CMS_TITLE = cms_Title.text;

                        Session["QuestionnaireTitle"] = cms_Title.text;

                    }
                    var cms_SubTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBTITLE).id);
                    if (cms_SubTitle != null)
                    {
                        ViewBag.CMS_SUBTITLE = cms_SubTitle.text;
                    }

                    var cms_PanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_ONE).id);
                    if (cms_PanelOne != null)
                    {
                        ViewBag.CMS_PANEL_ONE = cms_PanelOne.text;
                    }

                    var cms_PanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_TWO).id);
                    if (cms_PanelTwo != null)
                    {
                        ViewBag.CMS_PANEL_TWO = cms_PanelTwo.text;
                    }

                    var cms_FooterOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_ONE).id);
                    if (cms_FooterOne != null)
                    {
                        ViewBag.CMS_FOOTER_ONE = cms_FooterOne.text;
                    }

                    var cms_FooterTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_TWO).id);
                    if (cms_FooterTwo != null)
                    {
                        ViewBag.CMS_FOOTER_TWO = cms_FooterTwo.text;
                    }

                    var cms_SubmitText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBMIT_TEXT).id);
                    if (cms_SubmitText != null)
                    {
                        ViewBag.CMS_SUBMIT_TEXT = cms_SubmitText.text;
                    }

                    var ret_AccCode = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.RETRIEVE_ACCESS_CODE_TEXT).id);
                    if (ret_AccCode != null)
                    {
                        ViewBag.RETRIEVE_ACCESS_CODE_TEXT = ret_AccCode.text;
                    }

                }
                catch { }

                var touchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();

                var responseTypesQuestionnaire = db.pr_getResponseTypeByQuestionnaire(ptq.questionnaire).ToList();
                var objQuestionnaire = db.pr_getQuestionnaire(ptq.questionnaire).FirstOrDefault();

                if (touchpoint.endDate >= DateTime.Now)
                {
                    Session["accessCode"] = accessCode;
                    Session["hs3Registration"] = 1;
                    Session["languageused"] = "en";
                    Session["accessAttemp"] = 0;
                    Session["partner"] = ppptq.partner;
                    Session["touchpoint"] = touchpoint.id;
                    Session["partnerType"] = ptq.partnerType;
                    Session["questionnaire"] = ptq.questionnaire;
                    Session["leveltype"] = objQuestionnaire.levelType;
                    Session["protocol"] = touchpoint.protocol;
                    Session["responseTypesQuestionnaire"] = responseTypesQuestionnaire;
                    Session["currentEmail"] = objPartner.email;
                    //  Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(db.pr_getPartner(ppptq.partner).FirstOrDefault().enterprise.ToString());
                    List<CustomizedLSMW> CustomizedLSMW = new List<CustomizedLSMW>();
                    Session["CustomizedLSMW"] = CustomizedLSMW;

                    if (ppptq.status < 7)
                    {
                        ppptq.status = (int)PartnerStatus.Responded_Incomplete;
                        db.Entry(ppptq).State = EntityState.Modified;
                        db.SaveChanges();

                    }



                    return RedirectToAction("companyInformation");
                }
                else
                {
                    ViewBag.message = "expired";
                }
            }
            else
            {
                ViewBag.message = "expired";
            }

            return View();
        }


        public virtual ActionResult CompanyInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            try
            {
                ViewBag.country = db.pr_getCountry(objPartner.country).FirstOrDefault().name;
            }
            catch { }
            try
            {
                ViewBag.state = db.pr_getState(objPartner.state).FirstOrDefault().stateCode;
            }
            catch { }


            ViewBag.CMS_PAGE_TITLE = CMS.COMPANY_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.COMPANY_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.COMPANY_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.COMPANY_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.COMPANY_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.COMPANY_PAGE_NEXT_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();


                try
                {
                    var cms_PageTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_TITLE).id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle.text;
                    var cms_PageSubtitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_SUBTITLE).id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle.text;
                    var cms_PagePanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_PANEL_ONE).id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne.text;
                    var cms_PagePanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_PANEL_TWO).id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo.text;
                    var cms_PagePreviousText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_PREVIOUS_TEXT).id);
                    if (cms_PagePreviousText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviousText.text;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText.text;

                    QuestionnaireMenuLinks(cms, questionnairCMSAll);
                }
                catch { }

            }

            return View(objPartner);
        }

        public virtual ActionResult ContactInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            try
            {

                ViewBag.country = db.pr_getCountry(objPartner.country).FirstOrDefault().name;
            }
            catch { }

            ViewBag.CMS_PAGE_TITLE = CMS.CONTACT_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.CONTACT_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.CONTACT_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.CONTACT_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONTACT_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONTACT_PAGE_NEXT_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    var cms_PageTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_TITLE).id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle.text;
                    var cms_PageSubtitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_SUBTITLE).id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle.text;
                    var cms_PagePanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_PANEL_ONE).id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne.text;
                    var cms_PagePanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_PANEL_TWO).id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo.text;
                    var cms_PagePreviousText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_PREVIOUS_TEXT).id);
                    if (cms_PagePreviousText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviousText.text;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText.text;

                    QuestionnaireMenuLinks(cms, questionnairCMSAll);
                }
                catch { }
            }



            return View(objPartner);
        }
        public virtual ActionResult CorrectCompanyInformation()
        {
            return RedirectToAction("ContactInformation");
        }
        public virtual ActionResult CorrectContactInformation()
        {
            Session["partnumber"] = null;
            Session["site"] = null;
            Session["partnumberstatus"] = null;

            int id = (int)Session["questionnaire"];

            var objQuestionnaire = db.pr_getQuestionnaire(id).FirstOrDefault();



            if (objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
            {
                return RedirectToAction("QuestionnaireResponse", "PartNumber");
            }
            else if (objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.COMPANY_LEVEL)
            {
                return RedirectToAction("QuestionnaireResponse");
            }
            else
            {
                return Json(new { Questionnaire = "Questionnaire Not Found" }, JsonRequestBehavior.AllowGet);
            }

        }


        public virtual ActionResult QuestionnaireResponse(int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {

            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.CMS_PAGE_TITLE = CMS.QUESTIONNAIRE_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.QUESTIONNAIRE_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.QUESTIONNAIRE_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.QUESTIONNAIRE_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT;
            ViewBag.SAVE_FOR_LATER_TEXT = CMS.SAVE_FOR_LATER_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF;
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ;
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER;
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO;
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL;


            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    var cms_PageTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_TITLE).id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle.text;
                    var cms_PageSubtitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_SUBTITLE).id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle.text;
                    var cms_PagePanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_ONE).id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne.text;
                    var cms_PagePanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_TWO).id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo.text;
                    var cms_PagePreviousText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT).id);
                    if (cms_PagePreviousText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviousText.text;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText.text;

                    var cms_SaveForLater = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT).id);
                    if (cms_SaveForLater != null)
                        ViewBag.SAVE_FOR_LATER_TEXT = cms_SaveForLater.text;

                    var cms_quiestionnare = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PDF).id);
                    if (cms_quiestionnare != null)
                        ViewBag.QUESTIONNAIRE_PDF = cms_quiestionnare.text;

                    var cms_QuestionnareFAQ = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_FAQ).id);
                    if (cms_QuestionnareFAQ != null)
                        ViewBag.QUESTIONNAIRE_FAQ = cms_QuestionnareFAQ.text;
                    var cms_questionnare_doc = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).id);
                    if (cms_questionnare_doc != null)
                        ViewBag.QUESTIONNAIRE_DOC_OTHER = cms_questionnare_doc.text;
                    var cms_Questionnare_video = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_VIDEO).id);
                    if (cms_Questionnare_video != null)
                        ViewBag.QUESTIONNAIRE_VIDEO = cms_Questionnare_video.text;
                    var cms_ContactEmail = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL).id);
                    if (cms_ContactEmail != null)
                    {
                        ViewBag.CONTACT_US_EMAIL = cms_ContactEmail.text;
                        ViewBag.QUESTIONNAIRE_CONTACT_US_EMAIL_LINK = cms_ContactEmail.link;
                    }
                }
                catch { }
            }







            int questionnaireId = (int)Session["questionnaire"];


            int totalCount = (int)db.pr_getQuestionCountByQuestionnaire(questionnaireId).FirstOrDefault();
            var rows = db.pr_getQuestionRowIDByQuestionnaire(questionnaireId).ToList();

            foreach (var row in rows)
            {
                if (row.page == page)
                {

                    var percentageprogressbar = ((row.row - 1) / (float)totalCount) * 100;

                    ViewBag.percentageProgressbar = percentageprogressbar;
                    break;
                }
            }
            questionnaire objQuestionnaire = db.pr_getQuestionnaire(questionnaireId).FirstOrDefault();

            touchpoint objtouchpoint = new touchpoint();
            partner objpartner = new partner();
            protocol objprotocol = new protocol();

            surveyForm objSurveyForm = new surveyForm(objprotocol, objtouchpoint, objpartner, objQuestionnaire);
            objSurveyForm.questionIndex = questionIndex;
            objSurveyForm.questionClass = "brownbg  brownbgarrow";
            objSurveyForm.answerClass = "brownbg";
            objSurveyForm.alternativeAnswerClass = "bluebg";
            objSurveyForm.alternativequestionClass = "bluebg bluebgarrow";

            objSurveyForm.errorquestion = errorQuestion;
            objSurveyForm.errorMessage = errorMessage;
            Table table = objSurveyForm.tGetsurveyForm(objQuestionnaire, pageNumber, page, jumpToQuestion);
            // Table table = objSurveyForm.tGetSurveyForm(objQuestionnaire, pageNumber, page, jumpToQuestion);
            // panel.Controls.Add(table);

            StringWriter objhtml = new StringWriter();
            using (var htmlWriter = new HtmlTextWriter(objhtml))
            {
                table.RenderControl(htmlWriter);
            }

            ViewBag.questions = objhtml.ToString();

            return View();
        }




        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult QuestionnaireResponse(FormCollection formCollection, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            //[2] = "question_454_1171_fileUploadComment"
            //

            int questionnaireId = 0;
            int partnerId = 0;
            int touchpointId = 0;
            int protocolId = 0;
            // int jumpToQuestion = 0;
            //  int questionIndex = 0;

            questionnaireId = (int)Session["questionnaire"];
            partnerId = (int)Session["partner"];
            touchpointId = (int)Session["touchpoint"];
            protocolId = (int)Session["protocol"];

            int count = 0;
            int questionId = 0;
            int surveyId = 0;
            string key = "";
            string[] array = new string[5];
            char[] splitter = { '_' };
            string answer = "";
            question objQuestion = new question();
            Boolean saveForLaterButton = false;
            string skip = "";
            string noSkip = "";
            // int errorQuestion = 0;
            //  string errorMessage = "";
            string goEsignature = "";

            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;


            jumpToQuestion = 0;


            foreach (var keyName in formCollection.Keys)
            {
                answer = formCollection[keyName.ToString()];
                if (!keyName.ToString().Contains("uploadText") && keyName.ToString().Contains("question_"))
                {
                    ++questionIndex;
                }

                if (keyName.ToString().Contains("questionHiddenField_"))
                {

                    array = keyName.ToString().Split(splitter);

                    questionId = int.Parse(array[1]);
                    surveyId = int.Parse(array[2]);


                    //question = new Question(new Id(questionId));
                    //response = new Response(int.Parse(answer));
                    //survey = new Survey(new Id(surveyId));
                    //question.response = response;
                    //survey.question = question;

                    ////to be confirmed
                    //provider.removeProviderProtocolCampaignQuestionnaireSurveyQuestion(protocol, campaign,
                    //    questionnaire, survey, question);
                }
                //if (Request.Form.Keys[i].Contains("_languageBtn"))
                //{
                //    string str = Request.Form.Keys[i].ToString();
                //    int indexdol = str.IndexOf('$');
                //    int indexunder = str.IndexOf('_');
                //    indexunder = indexunder - indexdol;
                //    indexdol += 1;
                //    indexunder = indexunder - 1;
                //    string langName = str.Substring(indexdol, indexunder);
                //    Session["languageused"] = langName;
                //    Response.Redirect(Request.Url.ToString());
                //}




                if (keyName.ToString().Contains("btnSaveForLater"))
                {
                    saveForLaterButton = true;
                }

                if (keyName.ToString().Contains("question_"))
                {
                    //    ++questionIndex;

                    if (keyName.ToString().Contains("_text"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);

                        int? responseId = null;
                        string responseComment = string.Empty;
                        try
                        {
                            responseId = int.Parse(answer);
                        }
                        catch
                        {
                            responseComment = answer;
                        }

                        var checkpsz = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).ToList();
                        if (checkpsz.Count == 0)
                        {
                            db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);
                        }
                        else
                        {
                            db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(checkpsz.First().id, questionId, responseId, responseComment, null, null, null, null, pptq);
                        }

                        // db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);
                    }
                    else if (keyName.ToString().Contains("_checkBox"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                        answer = array[3];




                        if (formCollection[keyName.ToString()].ToLower() == "on")
                        {
                            int? responseId = null;
                            string responseComment = string.Empty;
                            try
                            {
                                responseId = int.Parse(answer);
                            }
                            catch
                            {
                                responseComment = answer;
                            }

                            var checkpsz = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).ToList();
                            if (checkpsz.Count == 0)
                            {
                                db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);
                            }
                            else
                            {
                                db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(checkpsz.First().id, questionId, responseId, responseComment, null, null, null, null, pptq);
                            }


                            //   db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);

                        }
                    }
                    else if (keyName.ToString().Contains("_Commenttext"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);

                        //provider.addProviderProtocolCampaignQuestionnaireSurveyQuestionResponse(
                        //    protocol, campaign, questionnaire, survey, question, response);
                    }
                    else if (keyName.ToString().Contains("_onlyTextComment"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);

                        //provider.addProviderProtocolCampaignQuestionnaireSurveyQuestionResponse(
                        //protocol, campaign, questionnaire, survey, question, response);
                    }
                    else
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                        int? responseId = null;
                        string responseComment = string.Empty;
                        try { responseId = int.Parse(answer); }
                        catch { }
                        if (answer == "74")
                        {

                            string strvl = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_Commenttext"];
                            if (strvl != null)
                            {
                                responseComment = strvl;
                                //response.description = surveyfrm.convertLanguageToEnglish(strvl);
                            }

                        }
                        else if (answer == "75")
                        {


                            string strvl = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_onlyTextComment"];
                            if (strvl != null)
                            {
                                responseComment = strvl;
                            }
                        }
                        else
                        {
                            responseComment = null;
                        }

                        //   db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);
                        var checkpsz = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).ToList();
                        if (checkpsz.Count == 0)
                        {
                            db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);
                        }
                        else
                        {
                            db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(checkpsz.First().id, questionId, responseId, responseComment, null, null, null, null, pptq);
                        }
                    }

                    objQuestion = db.pr_getQuestion(questionId).FirstOrDefault();



                    //JB skip logic handling begins

                    if (answer == "74" || answer == "75" || answer == "76" || answer != "")
                    {
                        if (objQuestion.skipLogicJump != null)
                        {
                            //if (objQuestion.skipLogicAnswer != null)
                            //{
                            //    if (answer == "74")
                            //    {


                            //        if (objQuestion.skipLogicJump.Contains("&"))
                            //        {
                            //        }
                            //        else
                            //        {
                            //            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                            //        }
                            //    }
                            //    else if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer != null) && answer != "" && answer != "75")

                            //        jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                            //    else
                            //    {
                            //        jumpToQuestion = 0;
                            //    }
                            //}
                            //else

                            //{
                            //    if (answer == "74" && (objQuestion.commentType == 5 || objQuestion.commentType == 3))
                            //    {
                            //        if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer != null))
                            //            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                            //        else
                            //            jumpToQuestion = 0;

                            //    }
                            if (objQuestion.skipLogicAnswer != null)
                            {

                                if (objQuestion.skipLogicJump.Contains("&"))
                                {
                                    string[] strQuestionLogic = objQuestion.skipLogicJump.Split(';');
                                    for (int k = 0; k < strQuestionLogic.Length - 1; k++)
                                    {
                                        string[] subStrQuestionlogic = strQuestionLogic[k].Split('&');
                                        Boolean logicOneStatus = false;
                                        Boolean logicTwoStatus = false;
                                        int gotoQuestionId = 0;
                                        for (int j = 0; j < subStrQuestionlogic.Length; j++)
                                        {
                                            string[] strquestionid = subStrQuestionlogic[j].Split('=');
                                            int questionidLogic = Convert.ToInt32(strquestionid[0]);
                                            string[] strNewQuestionAns = strquestionid[1].Split(':');
                                            int ansLogicStatus = 0;
                                            if (strNewQuestionAns.Length > 0)
                                            {
                                                ansLogicStatus = Convert.ToInt32(strNewQuestionAns[0]);
                                            }
                                            if (strNewQuestionAns.Length > 1)
                                            {
                                                gotoQuestionId = Convert.ToInt32(strNewQuestionAns[1]);
                                            }
                                            string answerStatus = "";
                                            //if (ansLogicStatus == 1)
                                            //{
                                            //    answerStatus = "74";
                                            //}
                                            //else
                                            //{
                                            //    answerStatus = "1";
                                            //}
                                            Boolean foundFlage = false;

                                            for (int l = 0; l < formCollection.Keys.Count; ++l)
                                            {
                                                key = formCollection.Keys[l];
                                                if (key.Contains("question_"))
                                                {
                                                    array = keyName.ToString().Split(splitter);
                                                    questionId = int.Parse(array[1]);
                                                    surveyId = int.Parse(array[2]);
                                                    answer = formCollection[l];

                                                    //if (questionId == gotoQuestionId)
                                                    //{
                                                    //    Response.Redirect("eSignature");
                                                    //}
                                                    if (questionId == questionidLogic)
                                                    {

                                                        foundFlage = true;

                                                        //if (answer == "74" || answer == "75" || answer == "76")
                                                        //{
                                                        //    foundFlage = true;
                                                        //    if (answer == answerStatus)
                                                        //    {
                                                        //        if (j == 0)
                                                        //        {
                                                        //            logicOneStatus = true;
                                                        //        }
                                                        //        else if (j == 1)
                                                        //        {
                                                        //            logicTwoStatus = true;
                                                        //        }
                                                        //    }
                                                        //    else if (answerStatus == "1")
                                                        //    {
                                                        //        if (answer == "75" || answer == "76")
                                                        //        {
                                                        //            if (j == 0)
                                                        //            {
                                                        //                logicOneStatus = true;
                                                        //            }
                                                        //            else if (j == 1)
                                                        //            {
                                                        //                logicTwoStatus = true;
                                                        //            }
                                                        //        }
                                                        //    }
                                                        //    break;
                                                        //}

                                                    }
                                                }
                                            }

                                            if (foundFlage)
                                            {
                                                question questionnew = db.pr_getQuestion(questionidLogic).FirstOrDefault();
                                                var context = new EntitiesDBContext();

                                                int? rID = context.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault().response;
                                                response responsenew = db.pr_getResponse(rID).FirstOrDefault();


                                                int check = 0;
                                                if (responsenew.description.ToLower() == "yes" || responsenew.description.ToLower() == "n/a" || responsenew.description.ToLower() == "no" || responsenew.description.ToLower() == "cots")
                                                {
                                                    foundFlage = true;

                                                    if (ansLogicStatus == 1 && responsenew.description.ToLower() == "yes")
                                                    {
                                                        check = 1;
                                                    }
                                                    else if (ansLogicStatus == 0 && responsenew.description.ToLower() == "no")
                                                    {
                                                        check = 1;
                                                    }
                                                    else if (ansLogicStatus == -1 && responsenew.description.ToLower() == "n/a")
                                                    {
                                                        check = 1;
                                                    }
                                                    else if (ansLogicStatus == 2 && responsenew.description.ToLower() == "cots")
                                                    {
                                                        check = 1;
                                                    }
                                                }
                                                else
                                                {
                                                    if (ansLogicStatus == 3 && responsenew != null)
                                                    {
                                                        check = 1;
                                                    }
                                                }
                                                if (check == 1)
                                                {
                                                    if (j == 0)
                                                    {
                                                        logicOneStatus = true;
                                                    }
                                                    else if (j == 1)
                                                    {
                                                        logicTwoStatus = true;
                                                    }
                                                }
                                            }


                                            //int ansLogicStatus =Convert.ToInt32(
                                        }
                                        if (logicOneStatus == true && logicTwoStatus == true)
                                        {
                                            objQuestion.skipLogicJump = gotoQuestionId.ToString();
                                            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                            break;
                                        }
                                    }
                                }
                                else if (answer != "74")
                                {


                                    if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer != null))
                                    {
                                        if (answer != "75")
                                            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                        else
                                            jumpToQuestion = 0;
                                    }
                                    else if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer == null))
                                    {
                                        if (answer == "75")
                                            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                        else
                                            jumpToQuestion = 0;
                                    }
                                    else if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump == null))
                                    {
                                        jumpToQuestion = 0;
                                    }

                                    else
                                    {
                                        jumpToQuestion = int.Parse(objQuestion.skipLogicJump);

                                        if (Request.QueryString["skip"] != null)
                                        {

                                            string strJumToQuestion = db.pr_getQuestion(objQuestion.id + 1).FirstOrDefault().skipLogicJump;
                                            if (strJumToQuestion.Contains("&"))
                                            {
                                            }
                                            else
                                            {
                                                jumpToQuestion = int.Parse(strJumToQuestion);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    jumpToQuestion = 0;
                                }
                            }
                        }
                    }
                }
            }

            // save uploaded files
            // jumpToQuestion = 
            saveUploadedFile(protocolId, touchpointId, partnerId, questionnaireId, pptq);


            //for (int i = 0; i < Request.Form.Keys.Count; i++)
            //{


            //    if (Request.Form.Keys[i].Contains("_languageBtn"))
            //    {

            //        string str = Request.Form.Keys[i].ToString();
            //        int indexdol = str.IndexOf('$');
            //        int indexunder = str.IndexOf('_');

            //        indexunder = indexunder - indexdol;
            //        indexdol += 1;
            //        indexunder = indexunder - 1;
            //        string langName = str.Substring(indexdol, indexunder);
            //        Session["languageused"] = langName;
            //        Response.Redirect(Request.Url.ToString());
            //    }
            //}


            //foreach (var keyName in formCollection.Keys)
            //{
            //    var value = formCollection[keyName.ToString()];
            //}

            if (questionId == jumpToQuestion)
            {
                goEsignature = "true";
            }


            if (goEsignature == "true")
            {
                //if (menuGroup.id.id == 2)
                //{
                //    Response.Redirect("spendCategoryForm.aspx");
                //}
                //else
                //{
                //    //int providerId = 0;
                //    int quetionnaireId = 0;
                //    partnerId = (int)Session["partnerId"];
                //    quetionnaireId = (int)Session["questionnaire"];

                Response.Redirect("eSignature");
                // }
            }

            if (saveForLaterButton == true)
            {


                //#region 20130222 new code
                //SaveLater(questionnaire, question);
                //#endregion

                //saveForLater();
            }

            else
            {

                goToNextPage(surveyId, jumpToQuestion, questionIndex, objQuestion, skip, errorQuestion, errorMessage, page, pageNumber);
            }


            return View();
        }


        private void goToNextPage(int surveyId, int jumpToQuestion, int questionIndex, question question, string skip, int errorQuestion, string errorMessage, int pageQ = 0, int pageNumberQ = 0)
        {
            int pageId = 0;
            int pageNumber = 0;
            int pgno = 0;
            int questionnaireId = (int)Session["questionnaire"];
            string errorQueryString = "";

            if (errorQuestion > 0)
            {
                errorQueryString = "&errorQuestion=" + errorQuestion.ToString() + "&errorMessage=" + errorMessage;
            }
            surveyForm surveyForm = new surveyForm();
            int partnerId = (int)Session["partner"];
            partner provider1 = new partner();
            questionnaire questionnaire = new questionnaire();
            string zode = surveyForm.generateZCode(provider1, questionnaire);
            page page = null;
            // menuGroup = (MenuGroup)Session["menuGroup"];

            if (pageNumberQ != 0)
            {
                pageNumber = pageNumberQ;
                pageNumber = pageNumber + 1;

            }
            else
            {
                pageNumber = 2;
            }

            if (pageQ != 0)
            {
                pageId = pageQ;

                page = db.pr_getNextPageByQuestionnaire(questionnaireId, pageId, jumpToQuestion).FirstOrDefault();
            }
            else
            {
                page = db.pr_getNextPageByQuestionnaire(questionnaireId, 0, 0).FirstOrDefault();
                page = db.pr_getNextPageByQuestionnaire(questionnaireId, page.id, jumpToQuestion).FirstOrDefault();
            }
            // update progressbare logic

            int providerId = 0;
            int quetionnaireId = 0;
            //   providerId = (int)Session["provider"];
            quetionnaireId = (int)Session["questionnaire"];
            //    Provider provider = new Provider(new Id(providerId));
            //  Campaign campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
            //int totalpage = questionnaire.getPageQuestionnaireCount(questionnaire);
            //pgno = pageNumber - 1;
            //for (int i = 0; i < totalpage; i++)
            //{
            //    if (pgno == i)
            //    {
            //        int totalperage = 0;
            //        int totper = 80 / totalpage;
            //        if (pgno == 0)
            //        {
            //            pgno = 1;
            //            totalperage = (totper * 1) + 10;
            //        }
            //        else
            //        {
            //            totalperage = (totper * i) + 10;
            //        }
            //        provider.updateSurveyProgressBar(provider, campaign, totalperage);
            //        break;
            //    }
            //}
            //
            if (Request.QueryString["questionIndex"] != null)
            {
                questionIndex += int.Parse(Request.QueryString["questionIndex"].ToString());
            }

            if (Request.QueryString["skip"] != null)
            {
                skip = "&skip=true";
            }

            if (jumpToQuestion == 0)
            {
                if (page != null)
                {
                    if (string.IsNullOrEmpty(errorQueryString))
                    {
                        Response.Redirect("QuestionnaireResponse?pageNumber=" + pageNumber.ToString() +
                                "&page=" + page.id.ToString() + "&jumpToQuestion=" + jumpToQuestion.ToString()
                                + "&questionIndex=" + questionIndex.ToString() + skip);
                    }
                    else if (Request.QueryString["errorQuestion"] == null)
                    {
                        Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString() + errorQueryString);
                    }
                    else
                    {
                        Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString());
                    }
                }
                else
                {
                    //if (menuGroup.id.id == 2)
                    //{
                    //    Response.Redirect("spendCategoryForm.aspx");
                    //}
                    //else
                    //{
                    //int providerId = 0;
                    //int quetionnaireId = 0;
                    //providerId = (int)Session["providerId"];
                    //quetionnaireId = (int)Session["questionnaire"];
                    //Provider provider = new Provider(new Id(providerId));
                    //Campaign campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
                    //provider.updateSurveyProgressBar(provider, campaign, 80);
                    Response.Redirect("~/Registration/Home/eSignature");
                    // }
                }
            }
            else
            {
                if (page != null)
                {
                    if (string.IsNullOrEmpty(errorQueryString))
                    {
                        Response.Redirect("QuestionnaireResponse?pageNumber=" + pageNumber.ToString() +
                           "&page=" + page.id.ToString() + "&jumpToQuestion=" + jumpToQuestion.ToString()
                           + "&questionIndex=" + questionIndex.ToString() + skip);
                    }
                    else if (Request.QueryString["errorQuestion"] == null)
                    {
                        Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString() + errorQueryString);
                    }
                    else
                    {
                        Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString());
                    }
                }
                else
                {
                    //if (menuGroup.id.id == 2)
                    //{
                    //    Response.Redirect("spendCategoryForm.aspx");
                    //}
                    //else
                    //{

                    Response.Redirect("~/Registration/Home/eSignature");
                    //}
                }
            }
        }

        private int saveUploadedFile(int protocolId, int touchpointId, int partnerId, int questionnaireId, int pptq)
        {
            Random random = new Random();
            int number = random.Next(1000000, 9999999);
            string fileName = "";
            int questionId = 0;
            int surveyId = 0;
            string key = "";
            string[] array = new string[4];
            char[] splitter = { '_' };
            question question = null;
            survey survey = null;
            int jumpToQuestion = 0;
            string errorMessage = "";

            for (int i = 0; i < Request.Files.Keys.Count; i++)
            {

                key = Request.Files.Keys[i];

                //get question and survey id
                array = key.Split(splitter);
                questionId = int.Parse(array[1]);
                surveyId = int.Parse(array[2]);

                //question = new question(new Id(questionId)).getQuestionDetail();
                //survey = new survey(new Id(surveyId));
                //provider = provider.getProviderById();

                //get jump to question
                //if (question.skipLogicAnswer == true)
                //{
                //    if (question.skipLogicJump.Contains("&"))
                //    {
                //    }
                //    else
                //    {
                //        jumpToQuestion = int.Parse(question.skipLogicJump);
                //    }
                //}

                if (Request.Files[i].FileName.Length > 0)
                {
                    string Extension = Request.Files[i].FileName.Substring(Request.Files[i].FileName.LastIndexOf('.') + 1).ToLower();
                    //    if (Request.Files[i].FileName.Substring()
                    //    {


                    if (Request.Files[i].ContentLength <= 4194304)
                    {
                        //if the directory already exists
                        //create a new directory name
                        //string dirname = "/Hs3/Honeywell/CertsAndReps/2013/UploadFiles";
                        //if (!Directory.Exists(Server.MapPath(dirname)))
                        //{
                        //    Directory.CreateDirectory(Server.MapPath(dirname));
                        //}
                        //if (Directory.Exists(Server.MapPath("uploadedFile/" + number.ToString())))
                        //{
                        //    number = random.Next(1000000, 9999999);
                        //}

                        //create the new directory and save the file in it
                        //Directory.CreateDirectory(Server.MapPath("uploadedFile/" + number.ToString()));
                        //if (question.id.id == 5132)
                        //if (question.question == "SAM")
                        //    fileName = dirname + "/" + provider.accessCode.accessCode + "_SAM." + Extension;
                        //// else if (question.id.id == 5171)
                        //else if (question.question == "HUB")

                        //    fileName = dirname + "/" + provider.accessCode.accessCode + "_Hubzone." + Extension;
                        /*if (File.Exists(fileName))
                                //	File.Delete(fileName );
                        //FileStream file = new FileStream(Server.MapPath(fileName) , FileMode.Create, System.IO.FileAccess.Write);

                                //Request.Files[i].SaveAs(Server.MapPath(fileName));

                        file.Close(); //for File saving update Nitoo 1Nov*/
                        //save the file path in the database
                        //provider.addProviderProtocolCampaignQuestionnaireSurveyQuestionUploadedFile(
                        //    protocol, campaign, questionnaire, survey, question, fileName);


                        var pptqq = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault();



                        byte[] uploadedFile = new byte[Request.Files[i].InputStream.Length];
                        Request.Files[i].InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                        // Binary linqBinary = new Binary(uploadedFile);

                        db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(pptqq.id, questionId, pptqq.response, pptqq.comment, uploadedFile, Request.Files[i].ContentType, pptqq.value, pptqq.score, pptq);

                        //int length = uploadFile.ContentLength;
                        //byte[] tempImage = new byte[length];
                        //myDBObject.ContentType = uploadFile.ContentType;

                        //uploadFile.InputStream.Read(tempImage, 0, length);
                        //myDBObject.ActualImage = tempImage;

                        ////create byte array of size equal to file input stream
                        //byte[] fileData = new byte[Request.Files[upload].InputStream.Length];
                        ////add file input stream into byte array
                        //Request.Files[upload].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files[upload].InputStream.Length));
                        ////create system.data.linq object using byte array
                        //System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                        ////initialise object of FileDump LINQ to sql class passing values to be inserted
                        //FileDump record = new FileDump { FileData = binaryFile, FileName = System.IO.Path.GetFileName(Request.Files[upload].FileName) };
                        ////call InsertOnsubmit method to pass new object to entity
                        //dataContext.FileDumps.InsertOnSubmit(record);
                        ////call submitChanges method to execute implement changes into database
                        //dataContext.SubmitChanges();

                    }
                    else
                    {


                    }

                }

                //if (question.title == "Upload Certification letter if applicable.")
                //{
                //    jumpToQuestion = 1;
                //}
            }


            return jumpToQuestion;
        }

        public FileContentResult FileDownload()
        {
            //declare byte array to get file content from database and string to store file name
            byte[] fileData;
            string fileName;

            //using LINQ expression to get record from database for given id value
            var record = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(454, 3).FirstOrDefault();
            //var record = from p in dataContext.FileDumps
            //             where p.ID == id
            //             select p;
            //only one record will be returned from database as expression uses condtion on primary field
            //so get first record from returned values and retrive file content (binary) and filename 
            fileData = (byte[])record.uploadedFile.ToArray();
            fileName = "abc";
            //return file and provide byte file content and file name --application/pdf
            return File(fileData, "application/pdf", fileName);
        }



        public ActionResult EditCompanyInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.CMS_PAGE_TITLE = CMS.COMPANY_EDIT_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.COMPANY_EDIT_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.COMPANY_EDIT_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.COMPANY_EDIT_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.COMPANY_EDIT_PAGE_NEXT_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;

                    QuestionnaireMenuLinks(cms, questionnairCMSAll);
                }
                catch { }
            }

            partner partner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            if (partner == null)
            {
                return HttpNotFound();
            }
            if (partner.state == null)
            {
                ViewBag.state = new SelectList(db.pr_getStateAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList().AsEnumerable(), "id", "stateCode");
            }
            else
            {
                ViewBag.state = new SelectList(db.pr_getStateAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList().AsEnumerable(), "id", "stateCode", partner.state);
            }

            if (partner.country == null)
            {
                ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            }
            else
            {

                ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name", partner.country);
            }
            //ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);

            ComboBoxModel objCombobox = new ComboBoxModel();
            if (partner.state != null)
            {
                objCombobox.ComboBoxAttributes.SelectedIndex = partner.state;
            }
            ViewBag.combobox = objCombobox;
            IEnumerable<state> states = new List<state>();
            states = db.pr_getStateAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            ViewBag.states = states;



            ComboBoxModel objComboboxCountry = new ComboBoxModel();
            if (partner.country != null)
            {
                objComboboxCountry.ComboBoxAttributes.SelectedIndex = partner.country;
            }
            ViewBag.comboboxCountry = objComboboxCountry;
            ViewBag.countries = db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID);

            return View(partner);
        }
        [HttpPost]
        public ActionResult EditCompanyInformation(partner partner)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner objpartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            objpartner.name = partner.name;
            objpartner.address1 = partner.address1;
            objpartner.address2 = partner.address2;
            objpartner.city = partner.city;
            objpartner.state = partner.state;
            objpartner.zipcode = partner.zipcode;
            objpartner.province = partner.province;
            objpartner.country = partner.country;
            if (ModelState.IsValid)
            {
                db.Entry(objpartner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CompanyInformation");
            }


            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            ViewBag.CMS_PAGE_TITLE = CMS.COMPANY_EDIT_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.COMPANY_EDIT_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.COMPANY_EDIT_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.COMPANY_EDIT_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.COMPANY_EDIT_PAGE_NEXT_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {

                    ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;

                    QuestionnaireMenuLinks(cms, questionnairCMSAll);

                }
                catch { }
            }

            return View(partner);
        }


        public ActionResult EditContactInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner partner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            if (partner == null)
            {
                return HttpNotFound();
            }

            ViewBag.CMS_PAGE_TITLE = CMS.CONTACT_EDIT_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.CONTACT_EDIT_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.CONTACT_EDIT_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.CONTACT_EDIT_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONTACT_EDIT_PAGE_NEXT_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {

                    ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;


                    QuestionnaireMenuLinks(cms, questionnairCMSAll);

                }
                catch { }
            }

            return View(partner);
        }

        [HttpPost]
        public ActionResult EditContactInformation(partner partner)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner objpartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            objpartner.firstName = partner.firstName;
            objpartner.lastName = partner.lastName;
            objpartner.title = partner.title;
            objpartner.email = partner.email;
            objpartner.phone = partner.phone;
            objpartner.fax = partner.fax;
            if (Session["currentEmail"].ToString() == objpartner.email)
            {
                if (ModelState.IsValid)
                {

                    db.Entry(objpartner).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("ContactInformation");
                }

            }
            else
            {
                ViewBag.isEmailChanged = "1";
                ViewBag.accessCode = Session["accessCode"];
                partner.email = Session["currentEmail"].ToString();

                ViewBag.CMS_PAGE_TITLE = CMS.CONTACT_EDIT_PAGE_TITLE;
                ViewBag.CMS_PAGE_SUBTITLE = CMS.CONTACT_EDIT_PAGE_SUBTITLE;
                ViewBag.CMS_PAGE_PANEL_ONE = CMS.CONTACT_EDIT_PAGE_PANEL_ONE;
                ViewBag.CMS_PAGE_PANEL_TWO = CMS.CONTACT_EDIT_PAGE_PANEL_TWO;
                ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT;
                ViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONTACT_EDIT_PAGE_NEXT_TEXT;
                var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

                if (ppptq_cms != null)
                {
                    var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                    var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                    var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                    try
                    {

                        ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                        ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                        ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                        ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    }
                    catch { }
                }
            }
            return View(partner);
        }

        public ActionResult ESignature()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            eSignature objeSignature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id).FirstOrDefault();

            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            ViewBag.CMS_PAGE_TITLE = CMS.ESIGNATURE_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.ESIGNATURE_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.ESIGNATURE_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.ESIGNATURE_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.ESIGNATURE_PAGE_NEXT_TEXT;
            ViewBag.ESIGNATURE_PAGE_TEXT = CMS.ESIGNATURE_PAGE_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {

                    ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.ESIGNATURE_PAGE_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_TEXT).FirstOrDefault().id).FirstOrDefault().text;

                    QuestionnaireMenuLinks(cms, questionnairCMSAll);
                }
                catch { }
            }


            return View(objeSignature);
        }

        [HttpPost]
        public ActionResult ESignature(eSignature objeSignatureNew)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            if (ModelState.IsValid)
            {
                partnerPartnertypeTouchpointQuestionnaire pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
                eSignature objeSignature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();
                if (objeSignature == null)
                {

                    objeSignatureNew.affirmation = "Yes";
                    objeSignatureNew.partnerPartnerTypeTouchpointQuestionnaire = pptq.id;
                    db.Entry(objeSignatureNew).State = EntityState.Added;
                    db.SaveChanges();
                }
                else
                {
                    objeSignature.firstName = objeSignatureNew.firstName;
                    objeSignature.lastName = objeSignatureNew.lastName;
                    objeSignature.email = objeSignatureNew.email;
                    objeSignature.affirmation = "Yes";
                    db.Entry(objeSignature).State = EntityState.Modified;
                    db.SaveChanges();
                }

                pptq.completedDate = DateTime.Now;
                pptq.status = (int)PartnerStatus.Responded_Complete;
                db.Entry(pptq).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Finish");
            }

            ViewBag.CMS_PAGE_TITLE = CMS.ESIGNATURE_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.ESIGNATURE_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.ESIGNATURE_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.ESIGNATURE_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.ESIGNATURE_PAGE_NEXT_TEXT;
            ViewBag.ESIGNATURE_PAGE_TEXT = CMS.ESIGNATURE_PAGE_TEXT;
            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {

                    ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    QuestionnaireMenuLinks(cms, questionnairCMSAll);
                }
                catch { }
            }
            return View();

        }

        public ActionResult Finish()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var leftSites = db.pr_getPartnumberSiteZcodePPTQByPPTQ_ToDo_ByPPTQ(ppptq_cms.id).ToList();
                var isCompletedSurvey = !(leftSites != null && leftSites.Count() > 0);
                ViewBag.isCompletedSurvey = !isCompletedSurvey;

                var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
                partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
                enterprise _enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
                if (isCompletedSurvey)
                {
                    var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Complete_Confirmation, ptq.id).FirstOrDefault();

                    var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                    Email email = new Email(amm);
                    EmailFormat emailFormat = new EmailFormat();
                    email.body = emailFormat.sGetEmailBody(email.body, person, objPartner, _enterprise, objtouchpoint, ptq.id);
                    email.emailTo = objPartner.email;
                    SendEmail objSendEmail = new SendEmail();
                    objSendEmail.sendEmail(email);
                }
                else
                {
                    var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Incomplete, ptq.id).FirstOrDefault();

                    var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                    Email email = new Email(amm);
                    EmailFormat emailFormat = new EmailFormat();
                    email.body = emailFormat.sGetEmailBody(email.body, person, objPartner, _enterprise, objtouchpoint, ptq.id);
                    email.emailTo = objPartner.email;
                    SendEmail objSendEmail = new SendEmail();
                    objSendEmail.sendEmail(email);
                }


                ViewBag.CMS_PAGE_TITLE = CMS.CONFIRMATION_PAGE_TITLE;
                ViewBag.CMS_PAGE_SUBTITLE = CMS.CONFIRMATION_PAGE_SUBTITLE;
                ViewBag.CMS_PAGE_PANEL_ONE = CMS.CONFIRMATION_PAGE_PANEL_ONE;
                ViewBag.CMS_PAGE_PANEL_TWO = CMS.CONFIRMATION_PAGE_PANEL_TWO;
                ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT;
                ViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONFIRMATION_PAGE_NEXT_TEXT;

                ViewBag.CONFIRMATION_PAGE_SIGNOFF_STATEMENT = CMS.CONFIRMATION_PAGE_SIGNOFF_STATEMENT;
                ViewBag.CONFIRMATION_PAGE_EXIT_LINK = "http://www.intelleges.com";
                ViewBag.CONFIRMATION_PAGE_HEADLINE = CMS.CONFIRMATION_PAGE_HEADLINE;

                ViewBag.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT = CMS.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT;
                ViewBag.WARNING = CMS.CONFIRMATION_PAGE_SIGNOFF_STATEMENT;


                ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
                ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
                ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
                ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
                ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

                #region Added by Suresh for the Logo

                List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
                if (enterprise == null)
                {
                    //var enterpriseIntelleges = db.pr_getEnterprise(1).FirstOrDefault();
                    //ViewBag.logoSrc = enterpriseIntelleges;
                    // return PartialView("_InstanceLogoPartial", enterpriseIntelleges);
                }
                else
                {
                    ViewBag.logoSrc = enterprise.FirstOrDefault().logo;
                    //  return PartialView("_InstanceLogoPartial", enterprise);
                }

                #endregion

                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                QuestionnaireMenuLinks(cms, questionnairCMSAll);
                try
                {
                    ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { }
                try
                {
                    ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { }

                try
                {
                    ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { } try
                {
                    ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { } try
                {
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { } try
                {
                    ViewBag.ACCESS_CODE = Session["accessCode"];
                }
                catch { } try
                {
                    ViewBag.CMS_PAGE_PREVIOUS_LINK = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().link;
                }
                catch { }

                // ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                try
                {
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONFIRMATION_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                    {
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText.text;

                        if (cms_PageNextText.link == null)
                        {
                            ViewBag.CMS_PAGE_NEXT_LINK = db.pr_getEnterpriseSystemInfo(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault().companyWebSite;

                        }
                        else
                        {
                            ViewBag.CMS_PAGE_NEXT_LINK = cms_PageNextText.link;
                        }
                    }
                }
                catch { }


                try
                {
                    ViewBag.CONFIRMATION_PAGE_SIGNOFF_STATEMENT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_SIGNOFF_STATEMENT).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { }
                //   ViewBag.CONFIRMATION_PAGE_EXIT_LINK = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_EXIT_LINK).FirstOrDefault().id).FirstOrDefault().text;


                try
                {
                    ViewBag.CONFIRMATION_PAGE_HEADLINE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_HEADLINE).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { }
                try
                {
                    ViewBag.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT).FirstOrDefault().id).FirstOrDefault().text;
                }
                catch { } try
                {
                    ViewBag.WARNING = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.WARNING).FirstOrDefault().id).FirstOrDefault().text;

                }
                catch { }

            }

            return View();
        }

        private void QuestionnaireMenuLinks(List<questionnaireQuestionnaireCMS> cms, List<pr_getQuestionnaireCMSAll_Result> questionnairCMSAll)
        {
            ViewBag.QUESTIONNAIRE_PDF = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PDF).FirstOrDefault().id).FirstOrDefault().text.PadRight(15).Substring(0, 15);
            //"~/Registration/Home/FileDownloadCMS?CMSName=" + CMS.QUESTIONNAIRE_PDF
            //"~/Registration/Home/FileDownloadCMS?CMSName=" + CMS.QUESTIONNAIRE_DOC_OTHER
            //"~/Registration/Home/FileDownloadCMS?CMSName=" + CMS.QUESTIONNAIRE_FAQ
            ViewBag.QUESTIONNAIRE_FAQ = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_FAQ).FirstOrDefault().id).FirstOrDefault().text.PadRight(15).Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).FirstOrDefault().id).FirstOrDefault().text.PadRight(15);
            ViewBag.QUESTIONNAIRE_VIDEO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_VIDEO).FirstOrDefault().id).FirstOrDefault().text.PadRight(15);
            ViewBag.CONTACT_US_EMAIL = replaceBlank(cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id).FirstOrDefault().text.PadRight(15));
            ViewBag.QUESTIONNAIRE_CONTACT_US_EMAIL_LINK = replaceBlank(cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id).FirstOrDefault().link);

            ViewBag.QUESTIONNAIRE_VIDEO_LINK = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id).FirstOrDefault().link;

        }
        public FileContentResult FileDownloadCMS(string CMSName)
        {
            try
            {
                //declare byte array to get file content from database and string to store file name
                byte[] fileData;
                byte[] fileDataBinary = null;
                string fileName;


                //if (CMSName == CMS.QUESTIONNAIRE_RESPONSE_PDF)
                //{
                //    var test_res_cm = db.pr_getPartnerQuestionResponseByAccessCode(Session["accessCode"].ToString()).ToList();// .pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

                //    if (test_res_cm != null)
                //    {


                //    }

                //    fileData = (byte[])fileDataBinary.ToArray();
                //    fileName = CMSName;
                //    return File(fileData, "application/pdf", fileName);
                //}
                // above added by Suresh for test result down as pdf  on 22nd July 2014.

                var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

                if (ppptq_cms != null)
                {
                    var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                    var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                    var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();

                    if (CMSName == CMS.QUESTIONNAIRE_PDF)
                    {
                        fileDataBinary = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PDF).FirstOrDefault().id).FirstOrDefault().doc;
                    }
                    else if (CMSName == CMS.QUESTIONNAIRE_FAQ)
                    {
                        fileDataBinary = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_FAQ).FirstOrDefault().id).FirstOrDefault().doc;
                    }
                    else
                    {
                        //QUESTIONNAIRE_DOC_OTHER
                        fileDataBinary = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).FirstOrDefault().id).FirstOrDefault().doc;


                    }
                }

                fileData = (byte[])fileDataBinary.ToArray();
                fileName = CMSName;
                //return file and provide byte file content and file name --application/pdf
                return File(fileData, "application/pdf", fileName);
            }
            catch
            {
                return null;
            }
        }

        protected ActionResult ViewHTML(object model)
        {

            return View("QuestionnaireResponsePdfDownload", model);  //name of the view...
        }





        //below code added by Suresn on 22nd July 2014. Reason: for Download pdf 



        protected ActionResult ViewPdf(object model)
        {
            // Create the iTextSharp document.
            Document pdfDoc = new Document();

            MemoryStream memStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memStream);
            writer.CloseStream = false;
            pdfDoc.Open();

            string htmltext = this.RenderActionResultToString(this.View("QuestionnaireResponsePdfDownload", model));  //name of the view...
            var parsedHtmlElements = HTMLWorker.ParseToList(new StringReader(htmltext), null);

            //Get each array values from parsed elements and add to the PDF document
            foreach (var htmlElement in parsedHtmlElements)
                pdfDoc.Add(htmlElement as IElement);

            //Close your PDF
            pdfDoc.Close();

            // Close and get the resulted binary data.
            pdfDoc.Close();
            byte[] buf = new byte[memStream.Position];
            memStream.Position = 0;
            memStream.Read(buf, 0, buf.Length);

            // Send the binary data to the browser.
            return new BinaryContentResult(buf, "application/pdf");
        }

        // for pdf download.
        protected string RenderActionResultToString(ActionResult result)
        {
            // Create memory writer.
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            // Create fake http context to render the view.
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request,
                fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                this.ControllerContext.RouteData,
                this.ControllerContext.Controller);
            var oldContext = System.Web.HttpContext.Current;
            System.Web.HttpContext.Current = fakeContext;

            // Render the view.
            result.ExecuteResult(fakeControllerContext);

            // Restore old context.
            System.Web.HttpContext.Current = oldContext;

            // Flush memory and return output.
            memWriter.Flush();
            return sb.ToString();
        }

        public class BinaryContentResult : ActionResult
        {
            private string ContentType;
            private byte[] ContentBytes;

            public BinaryContentResult(byte[] contentBytes, string contentType)
            {
                this.ContentBytes = contentBytes;
                this.ContentType = contentType;
            }

            public override void ExecuteResult(ControllerContext context)
            {
                var response = context.HttpContext.Response;
                response.Clear();
                response.Cache.SetCacheability(HttpCacheability.NoCache);
                response.ContentType = this.ContentType;

                var stream = new MemoryStream(this.ContentBytes);
                stream.WriteTo(response.OutputStream);
                stream.Dispose();
            }
        }

        public ActionResult PrintPDF(string accesscode)
        {
            if (!string.IsNullOrEmpty(accesscode))
            {
                Session["accessCode"] = accesscode;
                Response.Redirect("~/Registration/Home/PDFConfirmation");
            }
            return RedirectToAction("~/Registration/Home");
        }

        public ActionResult PDFConfirmation()
        {
            QuestionnaireModel modl = new QuestionnaireModel();
            List<pr_getPartnerQuestionResponseByAccessCode_Result> result = db.pr_getPartnerQuestionResponseByAccessCode(Session["accessCode"].ToString()).ToList();

            var find = db.pr_getPartnerHeaderByAccessCode(Session["accessCode"].ToString()).ToList();
            ViewBag.reslt2 = find;
            List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            var _partnerId = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().partner;
            var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
            ViewBag.partner = _partner;

            var _country = db.pr_getCountry(_partner.country).FirstOrDefault();
            if (_country != null)
                ViewBag.country = _country.name;
            else
                ViewBag.country = string.Empty;

            var _state = db.pr_getState(_partner.state).FirstOrDefault();
            if (_state != null)
                ViewBag.state = _state.stateCode;
            else
                ViewBag.state = string.Empty;

            if (enterprise == null)
            {
            }
            else
            {
                var logo = enterprise.FirstOrDefault().logo;
                string dirname = @"C:\https\MVCMT\logo\"; //@"C:\https\MVCMT\Generic\uploadedFiles\EnterpriseLogo\";
                if (Directory.Exists(dirname))
                {
                    var fileName = dirname + enterprise.FirstOrDefault().id + "Logo.png";
                    if (!System.IO.File.Exists(fileName))
                    {
                        var fs = new BinaryWriter(new FileStream(fileName, FileMode.Append, FileAccess.Write));
                        fs.Write(logo);
                        fs.Close();
                    }
                    ViewBag.logoSrc = fileName;
                }
            }

            ViewBag.QuestionnaireTitle = Session["QuestionnaireTitle"];
            return ViewPdf(result);
        }

        public ActionResult OrdersInHTML()
        {
            QuestionnaireModel modl = new QuestionnaireModel();
            List<pr_getPartnerQuestionResponseByAccessCode_Result> result = db.pr_getPartnerQuestionResponseByAccessCode(Session["accessCode"].ToString()).ToList();

            var find = db.pr_getPartnerHeaderByAccessCode(Session["accessCode"].ToString()).ToList();
            ViewBag.reslt2 = find;
            List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            var _partnerId = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().partner;
            var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
            ViewBag.partner = _partner;

            var _country = db.pr_getCountry(_partner.country).FirstOrDefault();
            if (_country != null)
                ViewBag.country = _country.name;
            else
                ViewBag.country = string.Empty;

            var _state = db.pr_getState(_partner.state).FirstOrDefault();
            if (_state != null)
                ViewBag.state = _state.stateCode;
            else
                ViewBag.state = string.Empty;

            if (enterprise == null)
            {
            }
            else
            {
                byte[] temp = enterprise.FirstOrDefault().logo;
                if (temp != null)
                {
                    string imageBase64 = Convert.ToBase64String(temp);
                    string imageSrc = string.Format("data:image/gif;base64,{0}", imageBase64);
                    ViewBag.logoSrc = imageSrc;
                }
            }

            ViewBag.QuestionnaireTitle = Session["QuestionnaireTitle"];

            return ViewHTML(result);
        }

        public FileContentResult ExportPng()
        {
            byte[] contents = (byte[])Session["logoPrint"];
            return File(contents, "image/png", "test" + ".png");
        }

        public string testS()
        {
            return Session["testS"].ToString();
        }
        public string replaceBlank(string toReplace)
        {
            return toReplace.Replace("[BLANK]", "");
        }

        public ActionResult CustomizedPDFConfirmation()
        {
            List<pr_getPartnerQuestionResponseByAccessCode_Result> reslt = db.pr_getPartnerQuestionResponseByAccessCode(Session["accessCode"].ToString()).ToList();

            var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(Session["accessCode"].ToString()).ToList();
            ViewBag.partnerHeader = _partnerHeader;
            List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            var _partnerId = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().partner;
            var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
            ViewBag.partner = _partner ;

            var _country = db.pr_getCountry(_partner.country).FirstOrDefault();
            if (_country != null)
                ViewBag.country = _country.name;
            else
                ViewBag.country = string.Empty;

            var _state = db.pr_getState(_partner.state).FirstOrDefault();
            if (_state != null)
                ViewBag.state = _state.stateCode;
            else
                ViewBag.state = string.Empty;

            if (enterprise == null)
            {
            }
            else
            {
                var logo = enterprise.FirstOrDefault().logo;//https://www.intelleges.com/mvcmt/Generic/uploadedFiles/EnterpriseLogo/
                string dirname = "~/uploadedFiles/EnterpriseLogo/";

                if (Directory.Exists(Server.MapPath(dirname)))
                {
                    var fileName = enterprise.FirstOrDefault().id + "Logo.png";
                    var physicalPath = Path.Combine(Server.MapPath(dirname), fileName);
                    if (!System.IO.File.Exists(physicalPath))
                    {
                        var fs = new BinaryWriter(new FileStream(physicalPath, FileMode.Append, FileAccess.Write));
                        fs.Write(logo);
                        fs.Close();
                    }
                    ViewBag.logoSrc = "https://www.intelleges.com/mvcmt/Generic/uploadedFiles/EnterpriseLogo/" + fileName;
                }
            }

            ViewBag.QuestionnaireTitle = Session["QuestionnaireTitle"];

            var _questionnaire = db.pr_getQuestionnaireByAccesscode(Session["accessCode"].ToString()).FirstOrDefault();
            var pptqID = _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault().id;
            var _PPTQQuestionResponse = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID);
            

            var _responseYES = 74;
            var _responseNO = 75;
            var _chacked = "checked";
            var _responseSplitter = "--";

            foreach (var item in _PPTQQuestionResponse)
            {
                var comments = new string[10];
                switch (item.qid)
                {
                    #region 1 Question
                    case 5796:
                        ViewBag.Checkbox1 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5788:
                        ViewBag.Checkbox2 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5801:
                        ViewBag.Checkbox3 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5803:
                        ViewBag.Checkbox4 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5798:
                        ViewBag.Checkbox5 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5799:
                        ViewBag.Checkbox6 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5800:
                        ViewBag.Checkbox7 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5791:
                        ViewBag.Checkbox8 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5792:
                        ViewBag.Checkbox9 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5793:
                        ViewBag.Checkbox10 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5794:
                         ViewBag.Checkbox11 = item.rid == _responseYES ? _chacked : string.Empty;
                         comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                         if (comments.Length > 1 && comments[1].Contains("Yes"))
                             ViewBag.Checkbox11_comment = _responseYES;

                        break;
                    case 5790:
                        ViewBag.Checkbox12 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5789:
                        ViewBag.Checkbox13 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5795:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox14 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;
                    case 5772:
                        ViewBag.Checkbox15 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5773:
                        ViewBag.Checkbox16 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5771:
                        ViewBag.Checkbox17 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5797:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox18 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox19 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input1 = comments[1]; 
                        }
                        break;
                    case 5802:
                        switch(item.rid)
                        {
                            case 13685:
                                ViewBag.Checkbox20 = _chacked;
                                break;
                            case 13686:
                                ViewBag.Checkbox21 = _chacked;
                                break;
                            case 13687:
                                ViewBag.Checkbox22 = _chacked;
                                break;
                            case 13688:
                                ViewBag.Checkbox23 = _chacked;
                                break;
                            case 13689:
                                ViewBag.Checkbox24 = _chacked;
                                break;
                            case 13690:
                                ViewBag.Checkbox25 = _chacked;
                                comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                                if (comments.Length > 1)
                                    ViewBag.Input2 = comments[1]; 
                                break;
                        }
                        break;
                    #endregion

                    #region 2 Question
                    case 5766:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox26 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox27 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input3 = comments[1];
                        }
                        break;
                    #endregion

                    #region 3 Question
                    case 5769:
                        if (item.rid == _responseYES)
                        {
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input4 = comments[1];
                        }
                        break;
                    #endregion

                    #region 4 Question
                    case 5774:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox28 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox29 = _chacked;
                        }
                        break;
                    #endregion

                    #region 5 Question
                    case 5775:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox30 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox31 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input5 = comments[1];
                        }
                        break;
                    #endregion

                    #region 6 Question
                    case 5776:
                        switch (item.rid)
                        {
                            case 13683:
                                ViewBag.Checkbox32 = _chacked;
                                break;
                            case 13684:
                                ViewBag.Checkbox33 = _chacked;
                                break;
                        }
                        break;
                    #endregion

                    #region 7 Question
                    case 5777:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox34 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox35 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input6 = comments[1];
                        }
                        break;
                    #endregion

                    #region 8 Question
                    case 5767:
                        switch (item.rid)
                        {
                            case 13681:
                                ViewBag.Checkbox36 = _chacked;
                                break;
                            case 13682:
                                ViewBag.Checkbox37 = _chacked;
                                break;
                        }
                        break;
                    #endregion

                    #region 9 Question
                    case 5768:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox38 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox39 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input7 = comments[1];
                        }
                        break;
                    #endregion

                    #region 10 Question
                    case 5778:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox40 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox41 = _chacked;
                        }
                        break;
                    #endregion

                    #region 11 Question
                    case 5779:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox42 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox43 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input8 = comments[1];
                        }
                        break;
                    #endregion

                    #region 12 Question
                    case 5780:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox44 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox45 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input9 = comments[1];
                        }
                        break;
                    #endregion

                    #region 13 Question
                    case 5785:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox46 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox47 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input10 = comments[1];
                        }
                        break;
                    case 5786:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox48 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox49 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input11 = comments[1];
                        }
                        break;
                    case 5787:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox50 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox51 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input12 = comments[1];
                        }
                        break;
                    #endregion

                    #region 14 Question
                    case 5781:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox52 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox53 = _chacked;
                        }
                        break;
                    #endregion

                    #region 15 Question
                    case 5782:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox54 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox55 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input13 = comments[1];
                        }
                        break;
                    #endregion

                    #region 16 Question
                    case 5783:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox56 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox57 = _chacked;
                        }
                        break;
                    case 5784:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox58 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox59 = _chacked;
                        }
                        break;
                    #endregion

                    #region CERTIFICATION
                    case 5805:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input14 = comments[1];
                        }
                        break;
                    #endregion
                }
            }

            return ViewCustomizedPdf(reslt, pptqID);
        }

        protected ActionResult ViewCustomizedPdf(object model, int pptqID)
        {
            string htmltext = this.RenderActionResultToString(this.View("CustomizedQuestionnaireSurveyPdfDownload", model));  //name of the view...

            string PDF_FileName = "HON_" + Session["accessCode"].ToString().Substring(1, 4) +".pdf";

            string dirname = "~/uploadedFiles/";
            if (Directory.Exists(Server.MapPath(dirname)))
            {
                var fileName = Server.MapPath(dirname) + PDF_FileName;
                if (System.IO.File.Exists(fileName))
                {
                    try
                    {
                        System.IO.File.Delete(fileName);
                    }
                    catch{}
                }

                FileStream file = new FileStream(fileName, FileMode.Create, System.IO.FileAccess.Write);
                ConvertApi.Web2Pdf convertApi;
                convertApi = new ConvertApi.Web2Pdf(400127803);

                convertApi.ConvertHtml(htmltext, file);
                file.Close();
                byte[] bytes = System.IO.File.ReadAllBytes(fileName);
                db.pr_addPPTQpdf(pptqID, bytes);
                System.IO.File.Delete(fileName);
                // Send the binary data to the browser.
                return new BinaryContentResult(bytes, "application/pdf");
            }
            return new BinaryContentResult(null, "application/pdf");
        }

        public ActionResult PDFCustomizedConfirmation()
        {
            if (!String.IsNullOrEmpty(Session["accessCode"].ToString()))
            {
                 var _pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
                 if (_pptq != null)
                 {
                     var _partnerId = _pptq.partner;
                     var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
                     var pptqID = _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault().id;
                     var pdf = db.pr_getPPTQpdf(pptqID).FirstOrDefault();
                     return new BinaryContentResult(pdf, "application/pdf");
                 }
            }
            return RedirectToAction("~/Registration/Home");
        }

        public ActionResult CustomizedPDFConfirmation(string accesscode)
        {
            if (!string.IsNullOrEmpty(accesscode))
            {
                Session["accessCode"] = accesscode;
                Response.Redirect("~/Registration/Home/CustomizedPDFConfirmation");
            }
            return RedirectToAction("~/Registration/Home");
        }
    }
}
