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
using System.Data.Entity;
using Pechkin;
using Generic.Helpers.PartNumberHelper;
using System.Configuration;
using System.Text.RegularExpressions;
namespace Generic.Areas.RegistrationArea.Controllers
{
    public class HomeController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        IGoogleTranslatorHelper _translator;
        const string PLEASE_ENTER_ACCESS_CODE = "Please enter your access code:";
        const string ACCESS_CODE_SIPLE_TEXT = "Access Code:";
        const string VERIFY_COMPANY_INFO = "Please verify that your company information is correct";
        const string COMPANY_INFORMATION_TEXT = "Company Information";
        const string CONTACT_INFORMATION_TEXT = "Contact Information";
        const string VERIFY_CONTACT_TEXT_INFORMATION = "Please verify that your contact information is correct";

        const string Company = "Company";
        const string PHYSYCAL_ADDRESS = "Physical Address";
        const string ADDRESS_ONE = "Address One";
        const string ADDRESS_TWO = "Address Two";
        const string CITY = "City";
        const string STATE_TEXT = "Country's State";
        const string POSTAL_CODE = "Postal Code";
        const string PROVINCE = "Province";
        const string COUNTRY_TEXT = "Country";
        const string REQUIRED_FIELDS = "Required Fields";


        const string FIRST_NAME = "First Name";
        const string LAST_NAME = "Last Name";
        const string TITLE_TEXT = "Job Title";
        const string EMAIL_TEXT = "Email";
        const string PHONE_TEXT = "Phone";
        const string FAX_TEXT = "Fax";
        //
        // GET: /RegistrationArea/Home/

        public virtual ActionResult Default()
        {
            return View();
        }

        public HomeController(IGoogleTranslatorHelper translator)
        {
            _translator = translator;
        }
        public ActionResult SetLanguage(string language)
        {
            CurrentLanguage = language;
            return Json(true);
        }

        public static string CurrentLanguage
        {
            get
            {

                if (System.Web.HttpContext.Current.Session["CurrentLanguage"] == null)
                {
                    System.Web.HttpContext.Current.Session["CurrentLanguage"] = "en";
                }
                return System.Web.HttpContext.Current.Session["CurrentLanguage"].ToString();
            }
            set
            {
                System.Web.HttpContext.Current.Session["CurrentLanguage"] = value;
            }
        }


        public virtual ActionResult SaveForLaterConfirm()
        {
            ViewBag.accesscode = Session["accessCode"];

            ViewBag.CMS_TITLE = CMS.ACCESS_CODE_TITLE;
            ViewBag.CMS_SUBTITLE = CMS.ACCESS_CODE_SUBTITLE;
            ViewBag.CMS_PANEL_ONE = CMS.ACCESS_CODE_PANEL_ONE;
            ViewBag.CMS_PANEL_TWO = CMS.ACCESS_CODE_PANEL_TWO;
            ViewBag.CMS_FOOTER_ONE = CMS.ACCESS_CODE_FOOTER_ONE;
            ViewBag.CMS_FOOTER_TWO = CMS.ACCESS_CODE_FOOTER_TWO;
            ViewBag.CMS_SUBMIT_TEXT = CMS.ACCESS_CODE_SUBMIT_TEXT.Substring(0, 10);
            ViewBag.RETRIEVE_ACCESS_CODE_TEXT = CMS.RETRIEVE_ACCESS_CODE_TEXT;
            ViewBag.SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT = CMS.SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT;
            ViewBag.SAVE_FOR_LATER_TEXT_NOTICE = CMS.SAVE_FOR_LATER_TEXT_NOTICE;
            ViewBag.QUESTIONNAIRE_DOC_OTHER_2 = CMS.QUESTIONNAIRE_DOC_OTHER_2;
            ViewBag.SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT = CMS.SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT;
            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF;
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ;
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER;
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO;
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL;
            ViewBag.QUESTIONNAIRE_DOC_OTHER_2 = CMS.QUESTIONNAIRE_DOC_OTHER_2;

            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            var cmsId = 0;
            if (ppptq_cms != null)
            {
                var enterpriseInfo = ppptq_cms.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1.enterpriseSystemInfo.FirstOrDefault();
                if (enterpriseInfo != null)
                    ViewBag.ENTERPRISE_URL = enterpriseInfo.companyWebSite;
                _translator.PPTQ = ppptq_cms;
                ViewBag.ACCESS_CODE_PLEASE_ENTER = _translator.Translate(PLEASE_ENTER_ACCESS_CODE, CurrentLanguage);
                ViewBag.ACCESS_CODE_SIPLE_TEXT = _translator.Translate(ACCESS_CODE_SIPLE_TEXT, CurrentLanguage);
                Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(db.pr_getPartner(ppptq_cms.partner).FirstOrDefault().enterprise.ToString());

                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                ViewBag.ENTERPRISE_ID = ptq.questionnaire1.enterprise;
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT).id;
                    var SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT != null)
                    {
                        ViewBag.SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT = SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER_2).id;
                    var QUESTIONNAIRE_DOC_OTHER_2 = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (QUESTIONNAIRE_DOC_OTHER_2 != null)
                    {
                        ViewBag.QUESTIONNAIRE_DOC_OTHER_2 = QUESTIONNAIRE_DOC_OTHER_2;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT_NOTICE).id;
                    var SAVE_FOR_LATER_TEXT_NOTICE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (SAVE_FOR_LATER_TEXT_NOTICE != null)
                    {
                        ViewBag.SAVE_FOR_LATER_TEXT_NOTICE = SAVE_FOR_LATER_TEXT_NOTICE;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT).id;
                    var SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT != null)
                    {
                        ViewBag.SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT = SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT;
                    }




                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_TITLE).id;
                    //var cms_Title = cms.FirstOrDefault(x => x.questionnaireCMS == cmsId);
                    var cms_Title = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);// cms.FirstOrDefault(x => );
                    if (cms_Title != null)
                    {
                        ViewBag.CMS_TITLE = cms_Title;

                        Session["QuestionnaireTitle"] = cms_Title;

                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBTITLE).id;
                    var cms_SubTitle = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_SubTitle != null)
                    {
                        ViewBag.CMS_SUBTITLE = cms_SubTitle;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_ONE).id;
                    var cms_PanelOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PanelOne != null)
                    {
                        ViewBag.CMS_PANEL_ONE = cms_PanelOne;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_TWO).id;
                    var cms_PanelTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PanelTwo != null)
                    {
                        ViewBag.CMS_PANEL_TWO = cms_PanelTwo;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_ONE).id;
                    var cms_FooterOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_FooterOne != null)
                    {
                        ViewBag.CMS_FOOTER_ONE = cms_FooterOne;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_TWO).id;
                    var cms_FooterTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_FooterTwo != null)
                    {
                        ViewBag.CMS_FOOTER_TWO = cms_FooterTwo;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBMIT_TEXT).id;
                    var cms_SubmitText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_SubmitText != null)
                    {
                        ViewBag.CMS_SUBMIT_TEXT = cms_SubmitText;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.RETRIEVE_ACCESS_CODE_TEXT).id;
                    var ret_AccCode = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (ret_AccCode != null)
                    {
                        ViewBag.RETRIEVE_ACCESS_CODE_TEXT = ret_AccCode;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PDF).id;
                    var cms_quiestionnare = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PDF).id);
                    if (cms_quiestionnare != null)
                        ViewBag.QUESTIONNAIRE_PDF = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_FAQ).id;
                    var cms_QuestionnareFAQ = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_FAQ).id);
                    if (cms_QuestionnareFAQ != null)
                        ViewBag.QUESTIONNAIRE_FAQ = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).id;
                    var cms_questionnare_doc = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).id);
                    if (cms_questionnare_doc != null)
                        ViewBag.QUESTIONNAIRE_DOC_OTHER = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_VIDEO).id;
                    var cms_Questionnare_video = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_VIDEO).id);
                    if (cms_Questionnare_video != null)
                        ViewBag.QUESTIONNAIRE_VIDEO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL).id;
                    var cms_ContactEmail = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL).id);
                    if (cms_ContactEmail != null)
                    {
                        ViewBag.CONTACT_US_EMAIL = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        ViewBag.QUESTIONNAIRE_CONTACT_US_EMAIL_LINK = cms_ContactEmail.link;
                    }
                }
                catch (Exception exc)
                {
                }
            }
            else
            {
                ViewBag.message = "wrongstatus";
            }
            return View();
        }

        public virtual ActionResult Index(string id = "", string accessCode = null, bool? advanced = null)
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

            var cmsId = 0;
            if (ppptq_cms != null)
            {
                _translator.PPTQ = ppptq_cms;
                ViewBag.ACCESS_CODE_PLEASE_ENTER = _translator.Translate(PLEASE_ENTER_ACCESS_CODE, CurrentLanguage);
                ViewBag.ACCESS_CODE_SIPLE_TEXT = _translator.Translate(ACCESS_CODE_SIPLE_TEXT, CurrentLanguage);
                Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(db.pr_getPartner(ppptq_cms.partner).FirstOrDefault().enterprise.ToString());

                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                ViewBag.ENTERPRISE_ID = ptq.questionnaire1.enterprise;
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_TITLE).id;
                    //var cms_Title = cms.FirstOrDefault(x => x.questionnaireCMS == cmsId);
                    var cms_Title = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);// cms.FirstOrDefault(x => );
                    if (cms_Title != null)
                    {
                        ViewBag.CMS_TITLE = cms_Title;

                        Session["QuestionnaireTitle"] = cms_Title;

                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBTITLE).id;
                    var cms_SubTitle = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_SubTitle != null)
                    {
                        ViewBag.CMS_SUBTITLE = cms_SubTitle;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_ONE).id;
                    var cms_PanelOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PanelOne != null)
                    {
                        ViewBag.CMS_PANEL_ONE = cms_PanelOne;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_TWO).id;
                    var cms_PanelTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PanelTwo != null)
                    {
                        ViewBag.CMS_PANEL_TWO = cms_PanelTwo;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_ONE).id;
                    var cms_FooterOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_FooterOne != null)
                    {
                        ViewBag.CMS_FOOTER_ONE = cms_FooterOne;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_TWO).id;
                    var cms_FooterTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_FooterTwo != null)
                    {
                        ViewBag.CMS_FOOTER_TWO = cms_FooterTwo;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBMIT_TEXT).id;
                    var cms_SubmitText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_SubmitText != null)
                    {
                        ViewBag.CMS_SUBMIT_TEXT = cms_SubmitText;
                    }
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.RETRIEVE_ACCESS_CODE_TEXT).id;
                    var ret_AccCode = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (ret_AccCode != null)
                    {
                        ViewBag.RETRIEVE_ACCESS_CODE_TEXT = ret_AccCode;
                    }
                }
                catch (Exception exc)
                {
                }
            }
            else
            {
                ViewBag.message = "wrongstatus";
            }
            if (advanced.HasValue && advanced.Value)
                return GenerateIndex(accessCode, advanced);
            return View();
        }

        protected virtual ActionResult GenerateIndex(string accessCode, bool? advanced = null)
        {
            ViewBag.CMS_TITLE = CMS.ACCESS_CODE_TITLE;
            ViewBag.CMS_SUBTITLE = CMS.ACCESS_CODE_SUBTITLE;
            ViewBag.CMS_PANEL_ONE = CMS.ACCESS_CODE_PANEL_ONE;
            ViewBag.CMS_PANEL_TWO = CMS.ACCESS_CODE_PANEL_TWO;
            ViewBag.CMS_FOOTER_ONE = CMS.ACCESS_CODE_FOOTER_ONE;
            ViewBag.CMS_FOOTER_TWO = CMS.ACCESS_CODE_FOOTER_TWO;

            ViewBag.CMS_SUBMIT_TEXT = "Login";

            int cmsId = 0;
            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
            if (ppptq != null)
            {
                _translator.PPTQ = ppptq;
                ViewBag.ACCESS_CODE_PLEASE_ENTER = _translator.Translate(PLEASE_ENTER_ACCESS_CODE, CurrentLanguage);
                ViewBag.ACCESS_CODE_SIPLE_TEXT = _translator.Translate(ACCESS_CODE_SIPLE_TEXT, CurrentLanguage);
                if (new int[] { 6, 7, 8 }.Contains(ppptq.status))
                {
                    var objPartner = db.pr_getPartner(ppptq.partner).FirstOrDefault();
                    Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(objPartner.enterprise.ToString());


                    var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();

                    var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                    var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                    try
                    {
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_TITLE).id;
                        //var cms_Title = cms.FirstOrDefault(x => x.questionnaireCMS == cmsId);
                        var cms_Title = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (cms_Title != null)
                        {
                            ViewBag.CMS_TITLE = cms_Title;

                            Session["QuestionnaireTitle"] = cms_Title;

                        }
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBTITLE).id;
                        var cms_SubTitle = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (cms_SubTitle != null)
                        {
                            ViewBag.CMS_SUBTITLE = cms_SubTitle;
                        }
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_ONE).id;
                        var cms_PanelOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (cms_PanelOne != null)
                        {
                            ViewBag.CMS_PANEL_ONE = cms_PanelOne;
                        }
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_PANEL_TWO).id;
                        var cms_PanelTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (cms_PanelTwo != null)
                        {
                            ViewBag.CMS_PANEL_TWO = cms_PanelTwo;
                        }
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_ONE).id;
                        var cms_FooterOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (cms_FooterOne != null)
                        {
                            ViewBag.CMS_FOOTER_ONE = cms_FooterOne;
                        }
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_TWO).id;
                        var cms_FooterTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (cms_FooterTwo != null)
                        {
                            ViewBag.CMS_FOOTER_TWO = cms_FooterTwo;
                        }
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBMIT_TEXT).id;
                        var cms_SubmitText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (cms_SubmitText != null)
                        {
                            ViewBag.CMS_SUBMIT_TEXT = cms_SubmitText;
                        }
                        cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.RETRIEVE_ACCESS_CODE_TEXT).id;
                        var ret_AccCode = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        if (ret_AccCode != null)
                        {
                            ViewBag.RETRIEVE_ACCESS_CODE_TEXT = ret_AccCode;
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


                        if (advanced.HasValue && advanced.Value)
                            return RedirectToAction("CorrectContactInformation");
                        return RedirectToAction("companyInformation");
                    }
                    else
                    {
                        ViewBag.message = "expired";
                    }
                }
                else
                {
                    ViewBag.message = "wrongstatus";
                    ViewBag.pptqaccesscode = ppptq.accesscode;
                }
            }
            else
            {
                ViewBag.message = "expired";
            }

            return View();
        }
        [HttpPost]
        public virtual ActionResult Index(string accessCode)
        {
            return GenerateIndex(accessCode, false);
        }


        public virtual ActionResult CompanyInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Default");
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
            int cms_id = 0;
            if (ppptq != null)
            {
                _translator.PPTQ = ppptq;
                ViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_COMPANY_INFO, CurrentLanguage);
                ViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(COMPANY_INFORMATION_TEXT, CurrentLanguage);
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();


                try
                {

                    cms_id = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_TITLE).id;
                    var cms_PageTitle = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle;
                    cms_id = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_SUBTITLE).id;
                    var cms_PageSubtitle = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle;
                    cms_id = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_PANEL_ONE).id;
                    var cms_PagePanelOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne;
                    cms_id = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_PANEL_TWO).id;
                    var cms_PagePanelTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo;

                    cms_id = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_PREVIOUS_TEXT).id;
                    var cms_PagePreviousText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
                    if (cms_PagePreviousText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviousText;

                    cms_id = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.COMPANY_PAGE_NEXT_TEXT).id;
                    var cms_PageNextText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText;

                    QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);
                }
                catch { }

            }

            return View(objPartner);
        }

        public virtual ActionResult ContactInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Default");
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
            var cmsId = 0;
            if (ppptq != null)
            {
                _translator.PPTQ = ppptq;
                ViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(CONTACT_INFORMATION_TEXT, CurrentLanguage);
                ViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_CONTACT_TEXT_INFORMATION, CurrentLanguage);
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_TITLE).id;
                    var cms_PageTitle = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle;

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_SUBTITLE).id;
                    var cms_PageSubtitle = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle;

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_PANEL_ONE).id;
                    var cms_PagePanelOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne;

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_PANEL_TWO).id;
                    var cms_PagePanelTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo;

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_PREVIOUS_TEXT).id;
                    var cms_PagePreviousText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PagePreviousText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviousText;

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_PAGE_NEXT_TEXT).id;
                    var cms_PageNextText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText;

                    QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);
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


            var pptqid = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accesscode"].ToString()).FirstOrDefault().id;
            if (objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
            {
                var statuses = db.pr_getPartnumberSiteZcodePPTQByPPTQ(pptqid).ToList().Select(x => x.status).Distinct().ToList();
                if (statuses.Any(o => o != Status.COMPLETED))
                    return RedirectToAction("QuestionnaireResponse", "PartNumber");
                else return RedirectToAction("ESignature");
            }
            else if (objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.COMPANY_LEVEL || objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.SUBSCRIPTION)
            {
                return RedirectToAction("QuestionnaireResponse");
            }
            else
            {
                return Json(new { Questionnaire = "Questionnaire Not Found" }, JsonRequestBehavior.AllowGet);
            }

        }

        private void ResolveAndSendEmailAlert(int questionId, int pptqId, int answerId = -1, string text = "")
        {
            var question = db.pr_getQuestion(questionId).FirstOrDefault();
            var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByQuestion(question.id).FirstOrDefault();

            var answer = db.pr_getResponse(answerId).FirstOrDefault();
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(pptqId).FirstOrDefault();
            var qresponse = pptq.partnerPartnertypeTouchpointQuestionnaireQuestionResponses.FirstOrDefault(o => o.question == questionId);
            if (question != null && !string.IsNullOrEmpty(question.emailAlertList) && question.emailAlertList.ToLower() != "none" && question.emailAlertList.ToUpper() != "N" && pptq != null && qresponse != null)
            {
                if (answer != null)
                {
                    var choices = question.emailAlertList.Split(new char[] { ',' });
                    foreach (var choiceStr in choices)
                    {
                        var keyPair = choiceStr.Split(new char[] { ':' });
                        if (keyPair.Length > 1 && answer.zcode != null && keyPair[0].ToLower() == answer.zcode.ToLower())
                        {
                            SendEmailAlert(pptq.partner1, answer.description, question.Question, pptq.accesscode, text, keyPair[1], ptq.questionnaire, question.id, answerId);

                        }
                    }
                }
                else
                {
                    SendEmailAlert(pptq.partner1, text, question.Question, pptq.accesscode, text, question.emailAlertList, ptq.questionnaire, question.id);
                }
            }

            if (question != null && !string.IsNullOrEmpty(question.tag) && answer != null)
            {
                var splitted = question.tag.Split(new char[] { ':' });
                if (splitted.Length > 1)
                {
                    switch (splitted[0])
                    {
                        case "autochangeptq":
                            var check = splitted[1].Split(new char[] { '-' });
                            if (int.Parse(check[0]) == answerId)
                                Session["autochangeptq"] = check[1];
                            break;
                        default: break;
                    }
                }
            }
        }
        private void SendEmailAlert(partner partnerName, string answer, string question, string accessCode, string comment, string emailTo, int ptqId, int questionId, int responseId = -1)
        {
            autoMailMessage objamm = new autoMailMessage();
            objamm.subject = "Intelleges: Email Alert";
            objamm.text = partnerName.name + "(" + partnerName.email + ") answered '" + answer + "' to '" + question + "' for access code " + accessCode;
            if (!string.IsNullOrEmpty(comment))
            {
                objamm.text += " with comment '" + comment + "'.";
                //should only happens when other is selected in dropdown: from John
                if (answer.ToLower().Contains("other"))
                {
                    var url = new Uri(new Uri(this.Request.Url.GetLeftPart(UriPartial.Authority)), Url.Action("QuestionnaireDetailView", "Questionnaire", new { id = ptqId, ModifyResponse = questionId, area = String.Empty, ptqId = ptqId, questionId = questionId, partnerId = partnerName.id, responseId = responseId })).ToString();
                    objamm.text += "<br><a href='" + url + "'>Add to dropdown</a><br><a href='" + url + "'>Assign to dropdown</a>";
                }
            }
            else objamm.text += ".";
            Email mail = new Email(objamm);
            mail.type = "emailAlert";
            mail.emailTo = emailTo;
            SendEmail objSendEmail = new SendEmail();
            objSendEmail.sendEmail(mail);
        }

        public virtual ActionResult QuestionnaireResponse(int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {

            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Default");
            }
            #region CMS
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
            int cmsId = 0;
            if (ppptq_cms != null)
            {
                _translator.PPTQ = ppptq_cms;
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_TITLE).id;

                    var cms_PageTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_TITLE).id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_SUBTITLE).id;
                    var cms_PageSubtitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_SUBTITLE).id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_ONE).id;
                    var cms_PagePanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_ONE).id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_TWO).id;
                    var cms_PagePanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_TWO).id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT).id;
                    var cms_PagePreviousText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT).id);
                    if (cms_PagePreviousText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT).id;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);


                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT).id;
                    var cms_SaveForLater = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT).id);
                    if (cms_SaveForLater != null)
                        ViewBag.SAVE_FOR_LATER_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PDF).id;
                    var cms_quiestionnare = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PDF).id);
                    if (cms_quiestionnare != null)
                        ViewBag.QUESTIONNAIRE_PDF = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_FAQ).id;
                    var cms_QuestionnareFAQ = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_FAQ).id);
                    if (cms_QuestionnareFAQ != null)
                        ViewBag.QUESTIONNAIRE_FAQ = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).id;
                    var cms_questionnare_doc = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).id);
                    if (cms_questionnare_doc != null)
                        ViewBag.QUESTIONNAIRE_DOC_OTHER = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_VIDEO).id;
                    var cms_Questionnare_video = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_VIDEO).id);
                    if (cms_Questionnare_video != null)
                        ViewBag.QUESTIONNAIRE_VIDEO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL).id;
                    var cms_ContactEmail = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL).id);
                    if (cms_ContactEmail != null)
                    {
                        ViewBag.CONTACT_US_EMAIL = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                        ViewBag.QUESTIONNAIRE_CONTACT_US_EMAIL_LINK = cms_ContactEmail.link;
                    }
                }
                catch { }
            }
            #endregion






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

            surveyForm objSurveyForm = new surveyForm(objprotocol, objtouchpoint, objpartner, objQuestionnaire, _translator, CurrentLanguage);
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
            if (TempData.ContainsKey("message"))
            {
                ViewBag.message = TempData["message"];
            }
            getZcodeByProviderProtocolCampaignQuestionnaire();
            return View();
        }

        public void getZcodeByProviderProtocolCampaignQuestionnaire()
        {
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            //var psz = new object[] { new { partnumber } };// db.pr_getPartnumberSiteZcodeByPPTQForUI(pptq.id).ToList().Where(x => x.status == "Completed");

            ////List<CustomizedLSMW> customizedLSMW = (List<CustomizedLSMW>)Session["CustomizedLSMW"];


            string labeltext = "";
            //if (psz.Count() > 0)
            //{
            labeltext += "<table cellpadding='2' cellspacing='0' border='1' style='width: 100%;'>";
            labeltext += "<tr><td>Access Code</td><td colspan=\"3\">Zcode</td><td>SCORE</td><td>PRIORITY</td><td>DUE DATE</td><td>COMPLETED DATE</td></tr>";
            //    //
            //    foreach (var dr in psz)
            //    {
            labeltext += "<tr>";
            //        try
            //        {
            labeltext += "<td>" + pptq.accesscode + "</td><td colspan=\"3\">" + pptq.zcode + "</td><td>" + pptq.score + "</td><td>" + pptq.priority + "</td><td>" + pptq.dueDate ?? "" + "</td><td>" + pptq.completedDate ?? "" + "</td>";
            //                //+ "<td>"+customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().LIFNR + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().MATNR + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().WERKS + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().ZPOST + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().ZCFLAG + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().COMPLETED_DATE.ToString("MM-dd-yyyy") + "</td>";
            //        }
            //        catch
            //        {
            //            labeltext += "<td>" + dr.site + "</td><td>" + dr.zcode + "</td><td></td><td></td><td></td><td></td><td></td><td></td>";
            //        }
            labeltext += "</tr>";
            //    }
            labeltext += "</table>";
            //}






            ViewBag.zcodeList = labeltext;


        }

        private string ZcodeModify(int questionnaireId, int questionId, int? responseId)
        {
            var result = "";
            var responseTypesQuestionnaire = (List<responseType>)Session["responseTypesQuestionnaire"];
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            var allQuestions = db.pr_getQuestionByQuestionnaire(questionnaireId).ToList();
            string zcode = pptq.zcode ?? allQuestions.Aggregate("", (r, t) => r += "ZZ");
            int questionNo = 0;
            foreach (var item in allQuestions)
            {
                questionNo++;

                if (questionId == item.id)
                {
                    break;
                }
            }
            string NewZcodePart1 = zcode.Substring(0, (questionNo * 2) - 2);

            //get zcode according to answer
            var responseZcode = db.pr_getResponse(responseId).FirstOrDefault();

            var objQuestion = db.pr_getQuestion(questionId).FirstOrDefault();

            if (objQuestion.responseType == ResponseType.DROPDOWN)
            {
                if (responseZcode != null && responseZcode.zcode != null)
                {
                    Session["CountryCode"] = responseZcode.zcode;
                }
            }

            string NewZcodePart2_CurrentQuestion = "--";
            if (responseZcode != null)
            {
                if (responseZcode.zcode != null)
                {
                    NewZcodePart2_CurrentQuestion = responseZcode.zcode;
                }
            }

            if (responseTypesQuestionnaire.Where(r => r.id == questionId).FirstOrDefault().description == "text")
            {
                NewZcodePart2_CurrentQuestion = ZCode.XX_Comment_Only_Question;
            }

            string NewZcodePart3 = zcode.Substring((questionNo * 2), zcode.Length - (questionNo * 2));
            //3    45
            //zzzz zz zz
            //0 4
            //6 L-6
            var zzcode = NewZcodePart1 + NewZcodePart2_CurrentQuestion + NewZcodePart3;
            var count = db.pr_checkPartnumberBadZcodeCountByZcode(zzcode).FirstOrDefault();
            if (count > 0)
            {
                result = "Please try again, if problem persists, please contact your system administrator by clicking on contact us button.<br>Thank you.";
                zzcode = "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ";
                pptq.zcode = zzcode;
                using (var contect = new EntitiesDBContext())
                {
                    var pnszCodepptq = contect.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptq.id);
                    pnszCodepptq.zcode = zzcode;
                    pnszCodepptq.status = Status.NOT_STARTED;
                    contect.Entry(pnszCodepptq).State = EntityState.Modified;
                    contect.SaveChanges();
                }
            }
            else
            {
                pptq.zcode = zzcode;


                using (var contect = new EntitiesDBContext())
                {
                    var pnszCodepptq = contect.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptq.id);
                    pnszCodepptq.zcode = zzcode;
                    contect.Entry(pnszCodepptq).State = EntityState.Modified;
                    contect.SaveChanges();
                }
            }
            return result;
        }

        private string ZcodeModifyForSkip(int questionnaireId, int questionId, int jumpToQuestion)
        {
            var result = "";
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            var allQuestions = db.pr_getQuestionByQuestionnaire(questionnaireId).ToList();
            string zcode = pptq.zcode ?? allQuestions.Aggregate("", (r, t) => r += "ZZ");
            int questionNo = 1;
            foreach (var item in allQuestions)
            {
                questionNo++;

                if (questionId == item.id)
                {
                    break;
                }
            }
            string NewZcodePart1 = zcode.Substring(0, (questionNo * 2) - 2);

            //get zcode according to answer

            string NewZcodePart2_CurrentQuestion = "";
            for (int i = 0; i < jumpToQuestion - questionId - 1; i++)
            {
                NewZcodePart2_CurrentQuestion += ZCode.YY_Skipped;

            }

            //2    6 -> 3 4 5

            string NewZcodePart3 = zcode.Substring((questionNo * 2) + (jumpToQuestion - questionId - 2) * 2, zcode.Length - ((questionNo * 2) + (jumpToQuestion - questionId - 2) * 2));
            //3    45
            //zzzz zzzzzz zzzzzz
            //0 4
            //6 L-6
            var zzcode = NewZcodePart1 + NewZcodePart2_CurrentQuestion + NewZcodePart3;
            var count = db.pr_checkPartnumberBadZcodeCountByZcode(zzcode).FirstOrDefault();
            if (count > 0)
            {
                result = "Please try again, if problem persists, please contact your system administrator by clicking on contact us button.<br>Thank you.";
                zzcode = "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ";
                pptq.zcode = zzcode;
                using (var contect = new EntitiesDBContext())
                {
                    var pnszCodepptq = contect.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptq.id);
                    pnszCodepptq.zcode = zzcode;
                    pnszCodepptq.status = Status.NOT_STARTED;
                    contect.Entry(pnszCodepptq).State = EntityState.Modified;
                    contect.SaveChanges();
                }
            }
            else
            {
                pptq.zcode = zzcode;


                using (var context = new EntitiesDBContext())
                {
                    var pnszcodepptq = context.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptq.id);
                    pnszcodepptq.zcode = zzcode;
                    context.Entry(pnszcodepptq).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return result;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult QuestionnaireResponse(FormCollection formCollection, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            #region Method Body
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Default");
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
            var cms = db.pr_getQuestionnaireQuestionnaireCMSByQuestionnaire(questionnaireId).FirstOrDefault();
            var pptqObj = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            int pptq = pptqObj.id;



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
                }
                if (keyName.ToString().Contains("btnSaveForLater"))
                {
                    saveForLaterButton = bool.Parse(formCollection["btnSaveForLater"]);
                }

                if (keyName.ToString().Contains("question_"))
                {
                    //    ++questionIndex;

                    #region text question
                    if (keyName.ToString().Contains("_text"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                        string responseComment = answer;
                        var checkpsz = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).ToList();
                        if (checkpsz.Count == 0)
                        {
                            db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, null, responseComment, null, null, null, null, null, pptq);
                        }
                        else
                        {
                            if (responseComment == "")
                            {
                                responseComment = checkpsz.FirstOrDefault().comment;
                            }

                            db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(checkpsz.First().id, questionId, null, responseComment, null, null, null, null, null, pptq);
                        }
                        ResolveAndSendEmailAlert(questionId, pptq, text: responseComment);
                        ZcodeModify(questionnaireId, questionId, null);
                    }
                    #endregion
                    #region checkbox question
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
                                db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, null, pptq);
                            }
                            else
                            {
                                if (responseComment == "")
                                {
                                    responseComment = checkpsz.FirstOrDefault().comment;
                                }
                                db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(checkpsz.First().id, questionId, responseId, responseComment, null, null, null, null, null, pptq);
                            }

                            ResolveAndSendEmailAlert(questionId, pptq, answerId: responseId.HasValue ? responseId.Value : -1, text: responseComment);
                            ZcodeModify(questionnaireId, questionId, responseId);
                        }
                    }
                    #endregion
                    else if (keyName.ToString().Contains("_Commenttext"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                    }
                    else if (keyName.ToString().Contains("_onlyTextComment"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                    }
                    else if (keyName.ToString().Contains("_duedateAlert"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                    }
                    else if (keyName.ToString().Contains("_duedate"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                    }
                    else if (keyName.ToString().Contains("_checkboxList"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                    }
                    #region other types
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
                            string strvl = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_onlyTextComment"];
                            if (!string.IsNullOrEmpty(strvl))
                            {
                                responseComment = strvl;
                            }
                            else responseComment = null;
                        }
                        var checkBoxes = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_checkboxList"];
                        if (checkBoxes != null)
                        {
                            var chechkedList = checkBoxes.Split(",".ToArray()).Select(o => int.Parse(o));
                            var question = db.pr_getQuestion(questionId).FirstOrDefault();
                            if (question != null)
                            {
                                var lenght = 0;
                                var resMap = question.subCheckBoxChoice.Split(":".ToArray());
                                if (resMap.Length > 1)
                                    lenght = resMap[1].Split(";".ToArray()).Length;
                                else lenght = resMap[0].Split(";".ToArray()).Length;
                                for (int i = 0; i < lenght; i++)
                                    if (chechkedList.Contains(i))
                                        responseComment += "1";
                                    else responseComment += "0";

                            }
                        }
                        DateTime? dueDate = null;
                        var strDueDate = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_duedate"];
                        if (!string.IsNullOrEmpty(strDueDate))
                        {
                            dueDate = !string.IsNullOrEmpty(strDueDate) ? DateTime.Parse(strDueDate) : (DateTime?)null;
                            string stralert = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_duedateAlert"];
                            if (!string.IsNullOrEmpty(stralert))
                            {
                                responseComment = stralert;
                            }
                            else responseComment = null;
                        }


                        var checkpsz = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault();
                        if (checkpsz == null)
                        {
                            db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, dueDate, null, null, pptq).FirstOrDefault();
                        }
                        else
                        {
                            if (responseComment == "")
                            {
                                responseComment = checkpsz.comment;
                            }
                            db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(checkpsz.id, questionId, responseId, responseComment, null, null, dueDate, null, null, pptq);
                        }
                        ResolveAndSendEmailAlert(questionId, pptq, answerId: responseId.HasValue ? responseId.Value : -1, text: responseComment);
                        ZcodeModify(questionnaireId, questionId, responseId);
                    }
                    #endregion
                    objQuestion = db.pr_getQuestion(questionId).FirstOrDefault();



                    //JB skip logic handling begins
                    //if (answer == "74" || answer == "75" || answer == "76" || answer != "")
                    //{
                    if (objQuestion.skipLogicJump != null)
                    {
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
                                                if (questionId == questionidLogic)
                                                {
                                                    foundFlage = true;
                                                }
                                            }
                                        }

                                        if (foundFlage)
                                        {
                                            question questionnew = db.pr_getQuestion(questionidLogic).FirstOrDefault();
                                            var currentQuestion = db.pr_getQuestion(questionId).FirstOrDefault();
                                            var context = new EntitiesDBContext();

                                            int? rID = context.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault().response;
                                            response responsenew = db.pr_getResponse(rID).FirstOrDefault();


                                            int check = 0;
                                            //if skip logic answer type is multiply then check by response.id
                                            if (responsenew != null)
                                                if (responsenew != null && currentQuestion.skipLogicAnswer == SkipLogicAnswer.D)
                                                {
                                                    check = responsenew.id == ansLogicStatus ? 1 : 0;
                                                }
                                                else
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
                    //}
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
            if (jumpToQuestion != 0)
            {
                ZcodeModifyForSkip(questionnaireId, questionId, jumpToQuestion);
            }
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
                // TempData["message"] = "Your answers have been saved";
                var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Incomplete, pptqObj.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                if (amm != null)
                {
                    Email email = new Email(amm);
                    var objtouchpoint = db.pr_getTouchpoint(touchpointId).FirstOrDefault();
                    email.accesscode = pptqObj.accesscode;
                    email.protocolTouchpoint = objtouchpoint.description;
                    EmailFormat emailFormat = new EmailFormat();
                    email.subject = emailFormat.sGetEmailBody(amm.subject, null, pptqObj.partner1, pptqObj.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, objtouchpoint, pptqObj.partnerTypeTouchpointQuestionnaire);
                    email.body = emailFormat.sGetEmailBody(email.body, null, pptqObj.partner1, pptqObj.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, objtouchpoint, pptqObj.partnerTypeTouchpointQuestionnaire);
                    email.emailTo = pptqObj.partner1.email;
                    SendEmail objSendEmail = new SendEmail();
                    objSendEmail.sendEmail(email);
                }
                return RedirectToAction("SaveForLaterConfirm");
                //#region 20130222 new code
                //SaveLater(questionnaire, question);
                //#endregion

                //saveForLater();
            }

            else
            {

                goToNextPage(surveyId, jumpToQuestion, questionIndex, objQuestion, skip, errorQuestion, errorMessage, page, pageNumber);
            }

            #endregion
            return Redirect("QuestionnaireResponse?questionIndex=" + questionIndex + "&jumpToQuestion=" + jumpToQuestion + "&page=" + page + "&pageNumber=" + pageNumber);
            // QuestionnaireResponse(questionIndex, jumpToQuestion, page, errorQuestion, pageNumber, errorMessage);
            // return View();
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

                        using (var context = new EntitiesDBContext())
                        {
                            var pptqq = context.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault();



                            byte[] uploadedFile = new byte[Request.Files[i].InputStream.Length];
                            Request.Files[i].InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                            // Binary linqBinary = new Binary(uploadedFile);
                            pptqq.uploadedFile = uploadedFile;
                            pptqq.uploadedFileType = Request.Files[i].ContentType;
                            context.Entry(pptqq).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                        // db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(pptqq.id, questionId, pptqq.response, pptqq.comment, uploadedFile, Request.Files[i].ContentType, pptqq.value, pptqq.score, pptq);

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
                return RedirectToAction("Default");
            }
            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            _translator.PPTQ = ppptq_cms;
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


            int cmsId = 0;
            if (ppptq_cms != null)
            {
                ViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(COMPANY_INFORMATION_TEXT, CurrentLanguage);
                ViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_COMPANY_INFO, CurrentLanguage);
                ViewBag.Company = _translator.Translate(Company, CurrentLanguage);
                ViewBag.PHYSYCAL_ADDRESS = _translator.Translate(PHYSYCAL_ADDRESS, CurrentLanguage);
                ViewBag.ADDRESS_ONE = _translator.Translate(ADDRESS_ONE, CurrentLanguage);
                ViewBag.ADDRESS_TWO = _translator.Translate(ADDRESS_TWO, CurrentLanguage);
                ViewBag.CITY = _translator.Translate(CITY, CurrentLanguage);
                ViewBag.STATE_TEXT = _translator.Translate(STATE_TEXT, CurrentLanguage);
                ViewBag.POSTAL_CODE = _translator.Translate(POSTAL_CODE, CurrentLanguage);
                ViewBag.PROVINCE = _translator.Translate(PROVINCE, CurrentLanguage);
                ViewBag.COUNTRY_TEXT = _translator.Translate(COUNTRY_TEXT, CurrentLanguage);
                ViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_TITLE).FirstOrDefault().id;

                    ViewBag.CMS_PAGE_TITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_SUBTITLE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_SUBTITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_ONE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_ONE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_TWO).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_TWO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_NEXT_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);
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


            IEnumerable<state> states = new List<state>();
            states = db.pr_getStateAll(1).ToList();
            ViewBag.states = states;
            ComboBoxModel objCombobox = new ComboBoxModel();
            if (partner.state != null)
            {
                objCombobox.ComboBoxAttributes.SelectedIndex = partner.state;
            }
            ViewBag.combobox = objCombobox;


            ComboBoxModel objComboboxCountry = new ComboBoxModel();
            if (partner.country != null)
            {
                objComboboxCountry.ComboBoxAttributes.SelectedIndex = partner.country;
            }
            ViewBag.comboboxCountry = objComboboxCountry;
            ViewBag.countries = db.pr_getCountryAll(1);

            return View(partner);
        }
        [HttpPost]
        public ActionResult EditCompanyInformation(partner partner)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Default");
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
            _translator.PPTQ = ppptq_cms;
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

            int cmsId = 0;
            if (ppptq_cms != null)
            {
                ViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(COMPANY_INFORMATION_TEXT, CurrentLanguage);
                ViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_COMPANY_INFO, CurrentLanguage);
                ViewBag.Company = _translator.Translate(Company, CurrentLanguage);
                ViewBag.PHYSYCAL_ADDRESS = _translator.Translate(PHYSYCAL_ADDRESS, CurrentLanguage);
                ViewBag.ADDRESS_ONE = _translator.Translate(ADDRESS_ONE, CurrentLanguage);
                ViewBag.ADDRESS_TWO = _translator.Translate(ADDRESS_TWO, CurrentLanguage);
                ViewBag.CITY = _translator.Translate(CITY, CurrentLanguage);
                ViewBag.STATE_TEXT = _translator.Translate(STATE_TEXT, CurrentLanguage);
                ViewBag.POSTAL_CODE = _translator.Translate(POSTAL_CODE, CurrentLanguage);
                ViewBag.PROVINCE = _translator.Translate(PROVINCE, CurrentLanguage);
                ViewBag.COUNTRY_TEXT = _translator.Translate(COUNTRY_TEXT, CurrentLanguage);
                ViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_TITLE).FirstOrDefault().id;

                    ViewBag.CMS_PAGE_TITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_SUBTITLE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_SUBTITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_ONE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_ONE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PANEL_TWO).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_TWO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.COMPANY_EDIT_PAGE_NEXT_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);

                }
                catch { }
            }

            return View(partner);
        }


        public ActionResult EditContactInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Default");
            }

            partner partner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            if (partner == null)
            {
                return HttpNotFound();
            }
            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
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



            var cmsId = 0;
            if (ppptq_cms != null)
            {
                _translator.PPTQ = ppptq_cms;
                ViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(CONTACT_INFORMATION_TEXT, CurrentLanguage);
                ViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_CONTACT_TEXT_INFORMATION, CurrentLanguage);
                ViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);


                ViewBag.FIRST_NAME = _translator.Translate(FIRST_NAME, CurrentLanguage);
                ViewBag.LAST_NAME = _translator.Translate(LAST_NAME, CurrentLanguage);
                ViewBag.TITLE_TEXT = _translator.Translate(TITLE_TEXT, CurrentLanguage);
                ViewBag.EMAIL_TEXT = _translator.Translate(EMAIL_TEXT, CurrentLanguage);
                ViewBag.PHONE_TEXT = _translator.Translate(PHONE_TEXT, CurrentLanguage);
                ViewBag.FAX_TEXT = _translator.Translate(FAX_TEXT, CurrentLanguage);
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_TITLE).FirstOrDefault().id;

                    ViewBag.CMS_PAGE_TITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_SUBTITLE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_SUBTITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PANEL_ONE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_ONE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PANEL_TWO).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_TWO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONTACT_EDIT_PAGE_NEXT_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);


                    QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);

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
                return RedirectToAction("Default");
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
                return RedirectToAction("Default");
            }
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            eSignature objeSignature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();

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

            int cmsId = 0;
            if (ppptq_cms != null)
            {
                _translator.PPTQ = ppptq_cms;
                ViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);


                ViewBag.FIRST_NAME = _translator.Translate(FIRST_NAME, CurrentLanguage);
                ViewBag.LAST_NAME = _translator.Translate(LAST_NAME, CurrentLanguage);
                ViewBag.TITLE_TEXT = _translator.Translate(TITLE_TEXT, CurrentLanguage);
                ViewBag.EMAIL_TEXT = _translator.Translate(EMAIL_TEXT, CurrentLanguage);
                ViewBag.PHONE_TEXT = _translator.Translate(PHONE_TEXT, CurrentLanguage);
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_TITLE).FirstOrDefault().id;

                    ViewBag.CMS_PAGE_TITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_SUBTITLE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_SUBTITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_ONE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_ONE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_TWO).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_TWO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_NEXT_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_TEXT).FirstOrDefault().id;
                    var floatText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    ViewBag.ESIGNATURE_PAGE_TEXT = new EmailFormat().sGetEmailBody(floatText, null, pptq.partner1, null, ptq.id);
                    QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);
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
                return RedirectToAction("Default");
            }
            partnerPartnertypeTouchpointQuestionnaire pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();


            // Validate the zCode.
            var zCodeValidationResult = db.pr_checkForInvalidZcode(pptq.id, pptq.zcode);

            // Obtain the zCode error code.
            var zCodeValidationErrorCode = zCodeValidationResult.First();

            //Check the zCode erro code. 
            if (zCodeValidationErrorCode.newStatus != 0 && zCodeValidationErrorCode.newStatus != 6)
            {
                //TODO:: provide a correct validation message;

                // Create error message.
                ViewBag.message = zCodeValidationErrorCode.nextstep;
                return View();
            }

            if (ModelState.IsValid)
            {

                eSignature objeSignature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();
                if (objeSignature == null)
                {
                    db.pr_addEsignature(objeSignatureNew.firstName, objeSignatureNew.lastName, objeSignatureNew.title, objeSignatureNew.email, "Yes", objeSignatureNew.officer, objeSignatureNew.phone, DateTime.Now, pptq.id);
                }
                else
                {
                    db.pr_modifyEsignature(objeSignatureNew.id, objeSignatureNew.firstName, objeSignatureNew.lastName, objeSignatureNew.title, objeSignatureNew.email, "Yes", objeSignatureNew.officer, objeSignatureNew.phone, DateTime.Now, pptq.id);
                }
                using (var dbConext = new EntitiesDBContext())
                {
                    var count = dbConext.pr_checkPartnumberStatusCountByPPTQ(pptq.id).FirstOrDefault();
                    // var statuses = dbConext.pr_getPartnumberSiteZcodePPTQByPPTQ(pptq.id).ToList().Select(x => x.status).Distinct().ToList();
                    //if (statuses.Count == 0 || (statuses.Count == 1 && statuses.FirstOrDefault() == Status.COMPLETED))
                    if (count == 0)
                    {
                        dbConext.pr_modifyPPTQStatus(pptq.partner, pptq.partnerTypeTouchpointQuestionnaire, (int)PartnerStatus.Responded_Complete);
                    }
                    else dbConext.pr_modifyPPTQStatus(pptq.partner, pptq.partnerTypeTouchpointQuestionnaire, (int)PartnerStatus.Responded_Incomplete);
                }
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
            int cmsId = 0;
            if (ppptq_cms != null)
            {
                _translator.PPTQ = ppptq_cms;
                ViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);
                ViewBag.FIRST_NAME = _translator.Translate(FIRST_NAME, CurrentLanguage);
                ViewBag.LAST_NAME = _translator.Translate(LAST_NAME, CurrentLanguage);
                ViewBag.TITLE_TEXT = _translator.Translate(TITLE_TEXT, CurrentLanguage);
                ViewBag.EMAIL_TEXT = _translator.Translate(EMAIL_TEXT, CurrentLanguage);
                ViewBag.PHONE_TEXT = _translator.Translate(PHONE_TEXT, CurrentLanguage);
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_TITLE).FirstOrDefault().id;

                    ViewBag.CMS_PAGE_TITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_SUBTITLE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_SUBTITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_ONE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_ONE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PANEL_TWO).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_TWO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_NEXT_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.ESIGNATURE_PAGE_TEXT).FirstOrDefault().id;
                    var floatText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                    ViewBag.ESIGNATURE_PAGE_TEXT = new EmailFormat().sGetEmailBody(floatText, null, pptq.partner1, null, ptq.id);
                    QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);
                }
                catch { }
            }
            return View();

        }

        public ActionResult Unsubscribe(int id)
        {
            var partner = db.pr_getPartner(id).FirstOrDefault();
            if (partner != null)
            {
                db.pr_unSubscribePartner(id);
                ViewBag.account = partner.email;
                return View();
            }
            return Redirect("https://www.intelleges.com");
        }

        public ActionResult CopyQuestionnarie(int enterpriseId, int personId, int ptqId)
        {
            try
            {
                BootstrapDefaultQuestionnarie(enterpriseId, personId, ptqId);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
            return Json("Completed", JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Some help methods for SUBSCRIPTION questionnarie responses
        /// </summary>

        private void BootstrapDefaultQuestionnarie(int enterpriseId, int personId, int ptqId)
        {
            // var defaultID = int.Parse(ConfigurationManager.AppSettings["DefaultPartnerTypeTouchpointQuestionnaireID"]);
            var defaultPtq = db.pr_getPartnertypeTouchpointQuestionnaire(ptqId).FirstOrDefault();
            if (defaultPtq != null)
            {
                //var defPartnerType = db.pr_addPartnerType(defaultPtq.partnerType1.name, defaultPtq.partnerType1.alias, defaultPtq.partnerType1.description, defaultPtq.partnerType1.partnerClass, enterpriseId, defaultPtq.partnerType1.sortOrder, 1).FirstOrDefault();
                var defPartnerType = db.partnerType.FirstOrDefault(o => o.enterprise == enterpriseId).id;
                var defaultProtocol = db.pr_getProtocolAll(enterpriseId).FirstOrDefault().id;
                var dtp = db.pr_addTouchpoint(defaultProtocol, personId, personId, personId, defaultPtq.touchpoint1.title, defaultPtq.touchpoint1.description, defaultPtq.touchpoint1.purpose, defaultPtq.touchpoint1.target, defaultPtq.touchpoint1.abbreviation, DateTime.Now, DateTime.Now.AddMonths(12), defaultPtq.touchpoint1.automaticReminder, defaultPtq.touchpoint1.sortOrder, 1).FirstOrDefault();
                //var dtp = db.touchpoint.FirstOrDefault(o => o.person == personId);

                var questionnarie = Convert.ToInt32(db.pr_addQuestionnaire(defaultPtq.questionnaire1.title, defaultPtq.questionnaire1.description, defaultPtq.questionnaire1.footer, defaultPtq.questionnaire1.locked, defaultPtq.questionnaire1.sortOrder, 1, defaultPtq.questionnaire1.multiLanguage, enterpriseId, personId, (int)defPartnerType, defaultPtq.questionnaire1.letter, defaultPtq.questionnaire1.levelType).FirstOrDefault());

                var newptq = Convert.ToInt32(db.pr_addPartnertypeTouchpointQuestionnaire((int)defPartnerType, (int)dtp, (int)questionnarie, 0, true).FirstOrDefault());
                //copy automailmessages
                foreach (var message in defaultPtq.autoMailMessage)
                    db.pr_addAutomailMessage(message.subject, message.text, message.footer1, message.footer2, message.sendDateCalcFactor, message.sendDateSet, message.mailType, (int)newptq).FirstOrDefault();

                //copy QuestionnaireCMS
                foreach (var cms in defaultPtq.questionnaire1.questionnaireQuestionnaireCMS)
                    db.pr_addQuestionnaireQuestionnaireCMS((int)questionnarie, cms.questionnaireCMS, cms.text, cms.link, cms.doc);

                var questions = db.pr_getQuestionByQuestionnaire((int)defaultPtq.questionnaire).ToList();


                var existSurvey = new Dictionary<int, int>();
                var existSurveySet = new Dictionary<int, int>();
                var existPage = new Dictionary<int, int>();
                var existSurveySetSurvey = new HashSet<string>();
                var existSurveyPage = new HashSet<string>();

                foreach (var question in questions)
                {

                    var qId = Convert.ToInt32(db.pr_addQuestion(question.Question, question.name, question.title, question.tag, question.responseType, question.required, question.weight, question.skipLogicAnswer, question.skipLogicJump, question.subCheckBoxChoice, question.accessLevel, question.commentRequired, question.commentBoxTxt, question.commentUploadTxt, question.calendarMessageTxt, question.commentType, question.spinOffQuestionnaire, question.spinOffQID, question.emailAlert, question.emailAlertList, question.updated, question.sortOrder, question.active, enterpriseId).FirstOrDefault()
                        );
                    if (!string.IsNullOrEmpty(question.skipLogicJump))
                    {
                        db.pr_modifyQuestionSkipLogicJumpLogic((int)qId, question.skipLogicJump.Replace(question.id.ToString(), ((int)qId).ToString()));
                    }
                    foreach (var qres in question.questionResponses)
                    {
                        db.pr_addQuestionResponse((int)qId, qres.response);
                    }
                    //foreach (var answer in question.resp)
                    foreach (var survey in question.surveys)
                    {
                        if (!existSurvey.ContainsKey(survey.id))
                        {
                            var ns = Convert.ToInt32(db.pr_addSurvey(survey.description, survey.name, survey.display, 1, true, DateTime.Now, DateTime.Now).FirstOrDefault());
                            existSurvey.Add(survey.id, (int)ns);
                        }
                        db.pr_addSurveyQuestion(existSurvey[survey.id], (int)qId);
                        foreach (var surveySet in survey.surveyset)
                        {
                            if (!existSurveySet.ContainsKey(surveySet.id))
                            {
                                var ss = Convert.ToInt32(db.pr_addSurveyset(surveySet.description, surveySet.sortOrder, true, 1).FirstOrDefault());
                                existSurveySet.Add(surveySet.id, (int)ss);
                            }
                            if (!existSurveySetSurvey.Contains(existSurveySet[surveySet.id].ToString() + "*" + existSurvey[survey.id].ToString()))
                            {
                                db.pr_addSurveysetSurvey(existSurveySet[surveySet.id], existSurvey[survey.id]);
                                existSurveySetSurvey.Add(existSurveySet[surveySet.id].ToString() + "*" + existSurvey[survey.id].ToString());
                            }
                            foreach (var page in surveySet.page)
                            {
                                if (!existPage.ContainsKey(page.id))
                                {
                                    var sp = Convert.ToInt32(db.pr_addPage(page.description, page.sortOrder, true).FirstOrDefault());
                                    existPage.Add(page.id, (int)sp);
                                    db.pr_addQuestionnairePage((int)questionnarie, (int)sp);
                                }
                                if (!existSurveyPage.Contains(existPage[page.id].ToString() + "*" + existSurveySet[surveySet.id].ToString()))
                                {
                                    db.pr_addPageSurveyset(existPage[page.id], existSurveySet[surveySet.id]);
                                    existSurveyPage.Add(existPage[page.id].ToString() + "*" + existSurveySet[surveySet.id].ToString());
                                }
                            }
                        }
                    }
                }
            }
        }


        public ActionResult Finish()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Default");
            }

            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();

            if (ppptq_cms != null)
            {
                _translator.PPTQ = ppptq_cms;
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var leftSites = db.pr_getPartnumberSiteZcodePPTQByPPTQ_ToDo_ByPPTQ(ppptq_cms.id).ToList();
                var isCompletedSurvey = !(leftSites != null && leftSites.Count() > 0);
                ViewBag.isCompletedSurvey = !isCompletedSurvey;

                var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
                partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
                enterprise _enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
                if (isCompletedSurvey)
                {
                    #region subscriptionType action
                    if (ppptq_cms.partnerTypeTouchpointQuestionnaire1.questionnaire1.levelType == Generic.Helpers.Questionnaire.LevelType.SUBSCRIPTION)
                    {
                        var questions = db.pr_getQuestionByQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire1.questionnaire).ToList();
                        var responses = ppptq_cms.partnerPartnertypeTouchpointQuestionnaireQuestionResponses.ToList();

                        var description = questions.FirstOrDefault(o => o.title == "description");
                        var website = questions.FirstOrDefault(o => o.title == "website");
                        var companyName = questions.FirstOrDefault(o => o.title == "companyName");
                        var department = questions.FirstOrDefault(o => o.title == "department");
                        var userCount = questions.FirstOrDefault(o => o.title == "userCount");
                        var respCount = questions.FirstOrDefault(o => o.title == "respCount");
                        var partNumber = questions.FirstOrDefault(o => o.title == "partNumber");
                        var productType = questions.FirstOrDefault(o => o.title == "productType");
                        var subscriptionType = questions.FirstOrDefault(o => o.title == "subscriptionType");
                        var projectType = questions.FirstOrDefault(o => o.title == "projectType");
                        var tStartDate = questions.FirstOrDefault(o => o.title == "tStartDate");
                        var lstartDate = questions.FirstOrDefault(o => o.title == "lstartDate");
                        var licenseType = questions.FirstOrDefault(o => o.title == "licenseType");
                        var firstname = questions.FirstOrDefault(o => o.title == "firstname");
                        var lasname = questions.FirstOrDefault(o => o.title == "lasname");
                        var internalID = questions.FirstOrDefault(o => o.title == "internalID");
                        var title = questions.FirstOrDefault(o => o.title == "title");
                        var email = questions.FirstOrDefault(o => o.title == "email");
                        var address1 = questions.FirstOrDefault(o => o.title == "address1");
                        var address2 = questions.FirstOrDefault(o => o.title == "address2");
                        var city = questions.FirstOrDefault(o => o.title == "city");
                        var state = questions.FirstOrDefault(o => o.title == "state");
                        var zipcode = questions.FirstOrDefault(o => o.title == "zipcode");
                        var country = questions.FirstOrDefault(o => o.title == "country");
                        var phone = questions.FirstOrDefault(o => o.title == "phone");


                        var result = db.pr_addEnterprise(responses.GetDropDownResponse(description), 1, true, null, null, responses.GetTextResponse<string>(companyName), responses.GetTextResponse<string>(department), responses.GetTextResponse<int>(userCount), responses.GetTextResponse<int>(respCount), responses.GetTextResponse<int>(partNumber), responses.GetDropDownProductTypeResponse(productType, db), responses.GetDropDownSubscriptionTypeResponse(subscriptionType, db), responses.GetTextResponse<DateTime>(tStartDate), responses.GetTextResponse<DateTime>(tStartDate).AddMonths(1), responses.GetTextResponse<DateTime>(lstartDate), responses.GetTextResponse<DateTime>(lstartDate).AddMonths(6), 0, 1, null, null, 1).FirstOrDefault();
                        if (result.HasValue)
                        {
                            //SessionSingleton.EnterPriseId = (int)result.Value;

                            db.pr_addEnterpriseSystemInfo(DateTime.Now.AddYears(1), 20, responses.GetTextResponse<string>(companyName), string.Empty, responses.GetTextResponse<string>(website), string.Empty, 1, string.Empty, false, (int)result.Value).FirstOrDefault();
                            using (var context = new EntitiesDBContext())
                            {
                                Session["pr_bootstrapAgencyId"] = context.pr_bootstrapAgency((int)result).FirstOrDefault();
                                var roleId = context.pr_bootstrapRole((int)result).FirstOrDefault();
                                var ptId = context.pr_bootstrapPartnertype((int)result).FirstOrDefault();
                                context.pr_bootstrapEnterprise((int)result);
                            }
                            ViewBag.saved = "true";
                            var manager = db.pr_getPersonByEnterprise2(1).FirstOrDefault();
                            var campaign = db.pr_getTouchpointAllByEnterprise(1).FirstOrDefault();
                            var password = db.pr_getAccesscode().FirstOrDefault();
                            //return RedirectToAction("CreatePerson", "Person");
                            var personId = db.pr_addPerson((int)result, manager.id, (int)PersonHelper.PersonStatus.Invited, 0, 0, campaign.id, responses.GetTextResponse<string>(internalID), string.Empty, string.Empty, responses.GetTextResponse<string>(firstname), responses.GetTextResponse<string>(lasname), responses.GetTextResponse<string>(title), string.Empty, responses.GetTextResponse<string>(email), password, responses.GetTextResponse<string>(email), responses.GetTextResponse<string>(address1), responses.GetTextResponse<string>(address2), responses.GetTextResponse<string>(city), 1, responses.GetTextResponse<string>(zipcode), 1, responses.GetTextResponse<string>(phone), string.Empty, 1, 1, 200, null, false, null).FirstOrDefault();

                            using (var context = new EntitiesDBContext())
                            {
                                context.pr_addPersonRole((int)personId, 1);
                                var menuCount = context.pr_bootstrapSystemMasterMenu(1).FirstOrDefault();
                                var protocol = context.pr_bootstrapProtocol((int)result, (int)personId, int.Parse(Session["pr_bootstrapAgencyId"].ToString())).FirstOrDefault();
                                var touchpoint = context.pr_bootstrapTouchpoint(int.Parse(protocol.ToString()), (int)personId).FirstOrDefault();
                                var group = context.pr_bootstrapGroup((int)result, (int)personId).FirstOrDefault();
                                context.pr_modifyPersonTouchpoint((int)personId, int.Parse(touchpoint.ToString()));
                                context.pr_addPersonGroup(int.Parse(group.ToString()), (int)personId);
                                var sysinfo = context.enterpriseSystemInfo.FirstOrDefault(o => o.enterprise == (int)result);
                                context.pr_modifyEnterpriseSystemInfo(sysinfo.id, sysinfo.systemExpiry, sysinfo.licenseLimit, sysinfo.companyName, responses.GetTextResponse<string>(firstname) + " " + responses.GetTextResponse<string>(lasname), sysinfo.companyWebSite, responses.GetTextResponse<string>(email), sysinfo.isCurrentDataBase, sysinfo.logoImage, sysinfo.configured, sysinfo.enterprise);
                            }
                            var defaultQuest = db.pr_getQuestionnaireByFooter(responses.GetDropDownZCodeResponse(description)).FirstOrDefault();
                            if (defaultQuest != null)
                            {
                                var ptqObj = db.pr_getPartnertypeTouchpointQuestionnaireByQuestionnaire(defaultQuest.id).FirstOrDefault();
                                BootstrapDefaultQuestionnarie((int)result.Value, (int)personId, ptqObj.id);
                            }

                            var objSystemMaster = db.pr_getPerson((int)personId).FirstOrDefault();

                            enterprise objEnterprise = db.pr_getEnterprise((int)result).FirstOrDefault();


                            autoMailMessage objamm = new autoMailMessage();

                            objamm.subject = "Intelleges Account Created";

                            //     objamm.text = "Dear " + objSystemMaster.firstName + "<br> please click on this <a href='https://www.intelleges.com/mvcmt/Generic'>hyperlink</a> and enter password " + objSystemMaster.passWord + " to login to the system.";

                            objamm.text = @"Hello <b>[User Email]</b>,<br><br><br>
Congratulations. We have created your Intelleges Account.<br><br>
Your user name is : [User Email]<br><br>
Your current password is: [Temporary Access Code]<br><br>
To access your intelleges.com account please click here <a href='[Project Url]'>[Project Url]</a>. <br><br>
To change your existing password select Change Password once you log in.<br><br>
To protect your privacy, we only send this information to the email address on file for this account. <br><br>
If you have any questions, please contact your Account Administrator admin@intelleges.com.<br><br>
Thank you.<br><br>
Intelleges Team";

                            var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Complete_Confirmation, ptq.id).FirstOrDefault();

                            if (amm != null)
                            {
                                objamm.text += "<br><br><br><br>" + amm.text;
                            }

                            Email mail = new Email(objamm);
                            ///person objInvitingUser = db.pr_getPersonByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();

                            touchpoint objCurrentTouchpoint = db.pr_getTouchpoint(objSystemMaster.campaign).FirstOrDefault();

                            EmailFormat emailFormat = new EmailFormat();
                            mail.subject = emailFormat.sGetEmailBody(mail.subject, objSystemMaster, objSystemMaster, objCurrentTouchpoint, objEnterprise, objSystemMaster);
                            //   email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
                            mail.body = emailFormat.sGetEmailBody(mail.body, objSystemMaster, objSystemMaster, ppptq_cms.partner1, objCurrentTouchpoint, objEnterprise, objSystemMaster, ptq.id);
                            //  email.body = objamm.text;
                            mail.emailTo = objSystemMaster.email;
                            SendEmail objSendEmail = new SendEmail();
                            objSendEmail.sendEmail(mail);
                        }
                    }
                    #endregion
                    else
                    {
                        var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Complete_Confirmation, ptq.id).FirstOrDefault();
                        if (amm != null)
                        {
                            var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                            Email email = new Email(amm);
                            EmailFormat emailFormat = new EmailFormat();
                            email.subject = emailFormat.sGetEmailBody(email.subject, person, objPartner, _enterprise, objtouchpoint, ptq.id);
                            email.body = emailFormat.sGetEmailBody(email.body, person, objPartner, _enterprise, objtouchpoint, ptq.id);
                            email.emailTo = objPartner.email;
                            SendEmail objSendEmail = new SendEmail();
                            objSendEmail.sendEmail(email);
                        }
                    }
                    //change ptq if required
                    if (Session["autochangeptq"] != null)
                    {
                        var nextPtq = int.Parse(Session["autochangeptq"].ToString());
                        db.pr_replacePTQforPPTQ(ppptq_cms.id, ppptq_cms.partnerTypeTouchpointQuestionnaire, nextPtq).FirstOrDefault();
                        Session["autochangeptq"] = null;
                    }
                }
                else
                {
                    var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Incomplete, ptq.id).FirstOrDefault();

                    var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                    Email email = new Email(amm);
                    EmailFormat emailFormat = new EmailFormat();
                    email.subject = emailFormat.sGetEmailBody(email.subject, person, objPartner, _enterprise, objtouchpoint, ptq.id);
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
                QuestionnaireMenuLinks(cms, questionnairCMSAll, ptq.questionnaire);
                int cmsId = 0;
                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_TITLE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_TITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                }
                catch { }
                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_SUBTITLE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_SUBTITLE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                }
                catch { }

                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PANEL_ONE).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_ONE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                }
                catch { } try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PANEL_TWO).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PANEL_TWO = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                }
                catch { } try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT).FirstOrDefault().id;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
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
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_NEXT_TEXT).FirstOrDefault().id;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONFIRMATION_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                    {
                        ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

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
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_SIGNOFF_STATEMENT).FirstOrDefault().id;
                    ViewBag.CONFIRMATION_PAGE_SIGNOFF_STATEMENT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                }
                catch { }
                //   ViewBag.CONFIRMATION_PAGE_EXIT_LINK = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_EXIT_LINK).FirstOrDefault().id).FirstOrDefault().text;


                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_HEADLINE).FirstOrDefault().id;
                    ViewBag.CONFIRMATION_PAGE_HEADLINE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                }
                catch { }
                try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT).FirstOrDefault().id;
                    ViewBag.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
                }
                catch { } try
                {
                    cmsId = questionnairCMSAll.Where(q => q.description == CMS.WARNING).FirstOrDefault().id;
                    ViewBag.WARNING = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);

                }
                catch { }

            }

            return View();
        }

        private void QuestionnaireMenuLinks(List<questionnaireQuestionnaireCMS> cms, List<pr_getQuestionnaireCMSAll_Result> questionnairCMSAll, int ptqId)
        {
            try
            {
                var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
                _translator.PPTQ = ppptq_cms;
                var cmdId = questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PDF).FirstOrDefault().id;
                ViewBag.QUESTIONNAIRE_PDF = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
                //"~/Registration/Home/FileDownloadCMS?CMSName=" + CMS.QUESTIONNAIRE_PDF
                //"~/Registration/Home/FileDownloadCMS?CMSName=" + CMS.QUESTIONNAIRE_DOC_OTHER
                //"~/Registration/Home/FileDownloadCMS?CMSName=" + CMS.QUESTIONNAIRE_FAQ
                cmdId = questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_FAQ).FirstOrDefault().id;
                ViewBag.QUESTIONNAIRE_FAQ = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
                cmdId = questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).FirstOrDefault().id;
                ViewBag.QUESTIONNAIRE_DOC_OTHER = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
                cmdId = questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_VIDEO).FirstOrDefault().id;
                ViewBag.QUESTIONNAIRE_VIDEO = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
                cmdId = questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id;
                ViewBag.CONTACT_US_EMAIL = replaceBlank(_translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId));

                ViewBag.QUESTIONNAIRE_CONTACT_US_EMAIL_LINK = replaceBlank(cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id).FirstOrDefault().link);


                ViewBag.QUESTIONNAIRE_VIDEO_LINK = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id).FirstOrDefault().link;
            }
            catch
            {

            }

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



        protected ActionResult ViewPdf(object model, int pptqID)
        {
            // Create the iTextSharp document.
            Document pdfDoc = new Document();

            MemoryStream memStream = new MemoryStream();
            PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memStream);
            writer.CloseStream = false;
            pdfDoc.Open();

            string htmltext = this.RenderActionResultToString(this.View("QuestionnaireResponsePdfDownload", model));
            EmailFormat formatter = new EmailFormat();
            var quest = db.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptqID);
            var partner = db.pr_getPartner(quest.partner).FirstOrDefault();
            htmltext = formatter.sGetEmailBody(htmltext, null, partner, quest.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, quest.partnerTypeTouchpointQuestionnaire1.touchpoint1, quest.partnerTypeTouchpointQuestionnaire);
            //name of the view...
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



            db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire(quest.id, quest.partner, quest.partnerTypeTouchpointQuestionnaire, quest.accesscode, quest.invitedBy, quest.invitedDate, quest.completedDate, quest.status, 100, quest.zcode, buf, quest.docFolderAddress, quest.score, quest.loadGroup);

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
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            var _partnerId = pptq.partner;
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
            return ViewPdf(result, pptq.id);
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

        public static int FillPdfHtml(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
        {
            var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(Session["accessCode"].ToString()).ToList();
            ViewBag.partnerHeader = _partnerHeader;
            List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            var _partnerId = pptq.partner;
            eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();
            var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
            ViewBag.partner = _partner;

            //_signature
            ViewBag.signature = _signature;
            ViewBag.personTitle = _partner.title;
            ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
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
                    case 5876:
                        ViewBag.Checkbox1 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5788:
                        ViewBag.Checkbox2 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5868:
                        ViewBag.Checkbox2 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5801:
                        ViewBag.Checkbox3 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5881:
                        ViewBag.Checkbox3 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5803:
                        ViewBag.Checkbox4 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5883:
                        ViewBag.Checkbox4 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5798:
                        ViewBag.Checkbox5 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5878:
                        ViewBag.Checkbox5 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5799:
                        ViewBag.Checkbox6 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5879:
                        ViewBag.Checkbox6 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5800:
                        ViewBag.Checkbox7 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5880:
                        ViewBag.Checkbox7 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5791:
                        ViewBag.Checkbox8 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5871:
                        ViewBag.Checkbox8 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5792:
                        ViewBag.Checkbox9 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5872:
                        ViewBag.Checkbox9 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5793:
                        ViewBag.Checkbox10 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5873:
                        ViewBag.Checkbox10 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5794:
                        ViewBag.Checkbox11 = item.rid == _responseYES ? _chacked : string.Empty;
                        comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                        if (comments.Length > 1 && comments[1].Contains("Yes"))
                            ViewBag.Checkbox11_comment = _responseYES;

                        break;
                    case 5874:
                        ViewBag.Checkbox11 = item.rid == _responseYES ? _chacked : string.Empty;
                        comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                        if (comments.Length > 1 && comments[1].Contains("Yes"))
                            ViewBag.Checkbox11_comment = _responseYES;

                        break;
                    case 5790:
                        ViewBag.Checkbox12 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5870:
                        ViewBag.Checkbox12 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5789:
                        ViewBag.Checkbox13 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5869:
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
                    case 5875:
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
                    case 5852:
                        ViewBag.Checkbox15 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5773:
                        ViewBag.Checkbox16 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5853:
                        ViewBag.Checkbox16 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5771:
                        ViewBag.Checkbox17 = item.rid == _responseYES ? _chacked : string.Empty;
                        break;
                    case 5851:
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
                    case 5877:
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
                        switch (item.rid)
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
                    case 5882:
                        if (item.response.Contains("Black American"))
                            ViewBag.Checkbox20 = _chacked;
                        else if (item.response.Contains("Hispanic"))
                            ViewBag.Checkbox21 = _chacked;
                        else if (item.response.Contains("Eskimos"))
                            ViewBag.Checkbox22 = _chacked;
                        else if (item.response.Contains("Thailand"))
                            ViewBag.Checkbox23 = _chacked;
                        else if (item.response.Contains("Pakistan"))
                            ViewBag.Checkbox24 = _chacked;

                        else if (item.response.Contains("Individual"))
                        {
                            ViewBag.Checkbox25 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input2 = comments[1];
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
                    case 5846:
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
                    case 5849:
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
                    case 5854:
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
                    case 5855:
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
                    case 5856:
                        if (item.response.Contains("HAS NOT"))
                            ViewBag.Checkbox33 = _chacked;
                        else
                            ViewBag.Checkbox32 = _chacked;
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
                    case 5857:
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
                    case 5847:
                        if (item.response.Contains("ARE NOT"))
                            ViewBag.Checkbox37 = _chacked;
                        else ViewBag.Checkbox36 = _chacked;

                        break;
                    #endregion

                    #region 9 Question
                    case 5768:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox0 = _chacked;
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox39 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input7 = comments[1];
                        }
                        break;
                    case 5848:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox0 = _chacked;
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
                    case 5858:
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
                    case 5859:
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
                    case 5860:
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
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input10 = comments[1];
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox47 = _chacked;

                        }
                        break;
                    case 5865:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox46 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input10 = comments[1];
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox47 = _chacked;

                        }
                        break;
                    case 5786:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox48 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input11 = comments[1];
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox49 = _chacked;

                        }
                        break;
                    case 5866:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox48 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input11 = comments[1];
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox49 = _chacked;

                        }
                        break;
                    case 5787:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox50 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input12 = comments[1];
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox51 = _chacked;
                        }
                        break;
                    case 5867:
                        if (item.rid == _responseYES)
                        {
                            ViewBag.Checkbox50 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input12 = comments[1];
                        }
                        else if (item.rid == _responseNO)
                        {
                            ViewBag.Checkbox51 = _chacked;
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
                    case 5861:
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
                    case 5862:
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
                    case 5863:
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
                    case 5864:
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
                    case 5885:
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
            return pptqID;
        }

        public static int FillCustomPdfHtml(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
        {
            var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(Session["accessCode"].ToString()).ToList();
            ViewBag.partnerHeader = _partnerHeader;
            List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            var _partnerId = pptq.partner;
            eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();
            var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
            ViewBag.partner = _partner;

            //_signature
            ViewBag.signature = _signature;
            ViewBag.personTitle = _partner.title;
            ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
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

            //  var _PPTQQuestionResponse = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID).ToList();

            var _PPTQQuestionResponse = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseByPPTQ(pptqID).ToList();


            var _responseYES = 74;
            var _responseNO = 75;
            var _chacked = "checked";
            var _responseSplitter = "--";

            //Generic.pr_getPPTQQuestionResponseByQuestionnaire_Result[] lstItem = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID).ToList().ToArray();


            foreach (var item in _PPTQQuestionResponse)
            {
                var comments = new string[10];
                switch (item.question)
                {
                    #region NEW ITEMS
                    case 18978:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;
                    case 18980:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox71 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox72 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input38 = comments[1];
                        }
                        break;


                    case 18981:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;


                    case 18984:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox61 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox62 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input34 = comments[1];
                        }
                        break;


                    case 18985:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox63 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox64 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input35 = comments[1];
                        }
                        break;


                    case 18986:
                        //if (item.response == _responseYES)
                        //{
                        //    ViewBag.Checkbox65 = _chacked;
                        //}
                        //else if (item.response == _responseNO)
                        //{
                        //    ViewBag.Checkbox66 = _chacked;
                        //    comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty:item.comment), _responseSplitter);
                        //    if (comments.Length > 1)
                        //        ViewBag.Input36 = comments[1];
                        //}
                        switch (item.response)
                        {
                            case 37520:
                                ViewBag.Checkbox65 = _chacked;
                                break;
                            case 37521:
                                ViewBag.Checkbox66 = _chacked;
                                comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                                if (comments.Length > 1)
                                    ViewBag.Input36 = comments[1];
                                break;
                            case 37522:
                                ViewBag.Checkbox75 = _chacked;
                                break;
                        }
                        break;


                    case 18987:
                        switch (item.response)
                        {
                            case 37523:
                                ViewBag.Checkbox67 = _chacked;
                                break;
                            case 37524:
                                ViewBag.Checkbox68 = _chacked;
                                comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                                if (comments.Length > 1)
                                    ViewBag.Input37 = comments[1];
                                break;
                            case 37525:
                                ViewBag.Checkbox76 = _chacked;
                                break;
                        }
                        break;

                    case 18988:
                        //if (item.response == _responseYES)
                        //{
                        //    ViewBag.Checkbox60 = _chacked;
                        //}
                        //else if (item.response == _responseNO)
                        //{
                        //    ViewBag.Checkbox61 = _chacked;
                        //    comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty:item.comment), _responseSplitter);
                        //    if (comments.Length > 1)
                        //        ViewBag.Input0 = comments[1];
                        //}
                        //if (item.response == _responseYES)
                        //{
                        //    ViewBag.Checkbox69 = _chacked;
                        //}
                        //else if (item.response == _responseNO)
                        //{
                        //    ViewBag.Checkbox70 = _chacked;                        
                        //}
                        switch (item.response)
                        {
                            case 37526:
                                ViewBag.Checkbox69 = _chacked;
                                break;
                            case 37527:
                                ViewBag.Checkbox70 = _chacked;
                                break;
                        }
                        break;

                    case 18992:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;

                    case 18991:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                            if (comments.Length > 1)
                                ViewBag.Input5 = comments[1];

                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            if (comments.Length > 1)
                                ViewBag.Input6 = comments[1];
                        }
                        break;


                    case 19003:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                            ViewBag.Checkbox53 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            ViewBag.Checkbox54 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        switch (item.response)
                        {
                            case 37537:
                                ViewBag.Checkbox53 = _chacked;
                                break;
                            case 37538:
                                ViewBag.Checkbox54 = _chacked;
                                break;
                        }
                        break;


                    case 19004:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                            ViewBag.Checkbox53 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            ViewBag.Checkbox54 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        switch (item.response)
                        {
                            case 37577:
                                ViewBag.Checkbox53 = _chacked;
                                break;
                            case 37578:
                                ViewBag.Checkbox54 = _chacked;
                                break;
                        }
                        break;
                    case 19005:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input15 = comments[0];
                        break;
                    case 19006:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input16 = comments[0];
                        break;
                    case 19007:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input17 = comments[0];
                        break;

                    case 19008:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;

                    case 19009:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input18 = comments[0];
                        break;
                    case 19010:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input19 = comments[0];
                        break;
                    case 19011:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input20 = comments[0];
                        break;
                    case 19012:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;

                    case 19013:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input21 = comments[0];
                        break;
                    case 19014:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input22 = comments[0];
                        break;
                    case 19015:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input23 = comments[0];
                        break;
                    case 19016:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;

                    case 19017:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input24 = comments[0];
                        break;
                    case 19018:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input25 = comments[0];
                        break;
                    case 19019:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input26 = comments[0];
                        break;
                    case 19020:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;

                    case 19021:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input27 = comments[0];
                        break;
                    case 19022:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input28 = comments[0];
                        break;
                    case 19023:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

                        ViewBag.Input29 = comments[0];
                        break;
                    case 19024:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                            ViewBag.Checkbox55 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            ViewBag.Checkbox56 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;


                    case 19025:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;
                    case 19026:
                        ViewBag.Input30 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
                        break;
                    case 19027:
                        ViewBag.Input31 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
                        break;
                    case 19028:
                        ViewBag.Input32 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
                        break;
                    case 19029:
                        ViewBag.Input33 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
                        break;

                    #endregion
                    #region 1 Question
                    case 19038:
                        // ViewBag.Checkbox1 = item.response == _responseYES ? _chacked : string.Empty;
                        ViewBag.CheckboxSmall = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19030:
                        //var isBigBusiness = _PPTQQuestionResponse.Where(x=>x.qid == 18995).FirstOrDefault();
                        //if (isBigBusiness != null && isBigBusiness.rid == _responseYES)
                        //{
                        //    ViewBag.CheckboxLarge = _chacked;
                        //    ViewBag.Checkbox2 = item.response == _responseYES ? _chacked : string.Empty;
                        //}
                        //else
                        //{
                        //    ViewBag.CheckboxLarge = string.Empty;
                        //}
                        ViewBag.CheckboxLarge = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19043:
                        if (ViewBag.CheckboxLarge == _chacked)
                        {
                            ViewBag.Checkbox1 = item.response == _responseYES ? _chacked : string.Empty;
                        }
                        else if (ViewBag.CheckboxSmall == _chacked)
                        {
                            ViewBag.Checkbox3 = item.response == _responseYES ? _chacked : string.Empty;
                        }
                        //ViewBag.Checkbox3 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19045:
                        if (ViewBag.CheckboxLarge == _chacked)
                        {
                            ViewBag.Checkbox2 = item.response == _responseYES ? _chacked : string.Empty;
                        }
                        else if (ViewBag.CheckboxSmall == _chacked)
                        {
                            ViewBag.Checkbox4 = item.response == _responseYES ? _chacked : string.Empty;
                        }
                        //var isSmallBusiness = _PPTQQuestionResponse.Where(x=>x.qid == 18994).FirstOrDefault();
                        //if (isSmallBusiness != null && isSmallBusiness.rid == _responseYES)
                        //{
                        //    ViewBag.CheckboxSmall = _chacked;
                        //    ViewBag.Checkbox4 = item.response == _responseYES ? _chacked : string.Empty;
                        //}
                        //else
                        //{
                        //    ViewBag.CheckboxSmall = string.Empty;
                        //}
                        break;

                    case 19040:
                        ViewBag.Checkbox5 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19041:
                        ViewBag.Checkbox6 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19042:
                        ViewBag.Checkbox7 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19033:
                        ViewBag.Checkbox8 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19034:
                        ViewBag.Checkbox9 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19035:
                        ViewBag.Checkbox10 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19036:
                        ViewBag.Checkbox11 = item.response == _responseYES ? _chacked : string.Empty;
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                        if (comments.Length > 1 && comments[1].Contains("Yes"))
                            ViewBag.Checkbox11_comment = _responseYES;

                        break;

                    case 19032:
                        ViewBag.Checkbox12 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19031:
                        ViewBag.Checkbox13 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19037:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox14 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input0 = comments[1];
                        }
                        break;

                    case 18994:
                        ViewBag.Checkbox15 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 18995:
                        ViewBag.Checkbox16 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 18993:
                        ViewBag.Checkbox15 = item.response == _responseYES ? _chacked : string.Empty;
                        break;

                    case 19039:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox18 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox19 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input1 = comments[1];
                        }
                        break;

                    case 19044:
                        switch (item.response)
                        {
                            case 37544:
                                ViewBag.Checkbox20 = _chacked;
                                break;
                            case 37545:
                                ViewBag.Checkbox21 = _chacked;
                                break;
                            case 37546:
                                ViewBag.Checkbox22 = _chacked;
                                break;
                            case 37547:
                                ViewBag.Checkbox23 = _chacked;
                                break;
                            case 37548:
                                ViewBag.Checkbox24 = _chacked;
                                break;
                            case 37549:
                                ViewBag.Checkbox25 = _chacked;
                                comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                                if (comments.Length > 1)
                                    ViewBag.Input2 = comments[1];
                                break;
                        }
                        break;

                    #endregion

                    #region 2 Question
                    case 18979:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox26 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox27 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input4 = comments[1];
                        }
                        break;

                    #endregion

                    #region 3 Question
                    case 18989:
                        comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                        if (item.response == _responseYES)
                        {
                            if (comments.Length > 1)
                                ViewBag.Input5 = comments[1];
                        }
                        else if (item.response == _responseNO)
                        {
                            if (comments.Length > 1)
                                ViewBag.Input6 = comments[1];
                        }
                        break;

                    #endregion

                    #region 4 Question
                    case 18996:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox28 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox29 = _chacked;
                        }
                        break;

                    #endregion

                    #region 5 Question
                    case 18997:
                        //if (item.response == _responseYES)
                        //{
                        //    ViewBag.Checkbox30 = _chacked;
                        //}
                        //else if (item.response == _responseNO)
                        //{
                        //    ViewBag.Checkbox31 = _chacked;
                        //    comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty:item.comment), _responseSplitter);
                        //    if (comments.Length > 1)
                        //        ViewBag.Input5 = comments[1];
                        //}
                        switch (item.response)
                        {
                            case 37528:
                                ViewBag.Checkbox30 = _chacked;
                                break;
                            case 37529:
                                ViewBag.Checkbox31 = _chacked;
                                comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                                if (comments.Length > 1)
                                    ViewBag.Input5 = comments[1];
                                break;
                            case 37530:
                                ViewBag.Checkbox73 = _chacked;
                                break;

                        }
                        break;

                    #endregion

                    #region 6 Question
                    case 18998:
                        switch (item.response)
                        {
                            case 37531:
                                ViewBag.Checkbox32 = _chacked;
                                break;
                            case 37532:
                                ViewBag.Checkbox33 = _chacked;
                                break;
                            case 37533:
                                ViewBag.Checkbox34 = _chacked;
                                break;
                        }
                        break;

                    #endregion

                    #region 7 Question
                    case 18999:
                        //if (item.response == _responseYES)
                        //{
                        //    ViewBag.Checkbox34 = _chacked;
                        //}
                        //else if (item.response == _responseNO)
                        //{
                        //    ViewBag.Checkbox35 = _chacked;
                        //    comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty:item.comment), _responseSplitter);
                        //    if (comments.Length > 1)
                        //        ViewBag.Input6 = comments[1];
                        //}
                        switch (item.response)
                        {
                            case 37534:
                                ViewBag.Checkbox35 = _chacked;
                                break;
                            case 37535:
                                ViewBag.Checkbox36 = _chacked;
                                comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                                if (comments.Length > 1)
                                    ViewBag.Input8 = comments[1];
                                break;
                            case 37536:
                                ViewBag.Checkbox74 = _chacked;
                                break;
                        }
                        break;

                    #endregion

                    #region 8 Question
                    case 18982:
                        switch (item.response)
                        {
                            case 37518:
                                ViewBag.Checkbox37 = _chacked;
                                break;
                            case 37519:
                                ViewBag.Checkbox38 = _chacked;
                                break;
                        }
                        break;

                    #endregion

                    #region 9 Question
                    case 18983:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox39 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox40 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input9 = comments[1];
                        }
                        break;

                    #endregion

                    #region 10 Question
                    case 19000:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox41 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox42 = _chacked;
                        }
                        break;

                    #endregion

                    #region 11 Question
                    case 19001:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox43 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox44 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input10 = comments[1];
                        }
                        break;

                    #endregion

                    #region 12 Question
                    case 19002:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox45 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox46 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input11 = comments[1];
                        }
                        break;

                    #endregion

                    #region 13 Question
                    case 19046:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox47 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input12 = comments[1];
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox48 = _chacked;

                        }
                        break;

                    case 19047:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox49 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input13 = comments[1];
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox50 = _chacked;

                        }
                        break;

                    case 19048:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox51 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                                ViewBag.Input14 = comments[1];
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox52 = _chacked;
                        }
                        break;

                    #endregion

                    #region 16 Question
                    case 19049:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox57 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox58 = _chacked;
                        }
                        break;

                    case 19050:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox59 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox60 = _chacked;
                        }
                        break;

                    #endregion

                    #region CERTIFICATION
                    case 19051:
                        if (item.response == _responseYES)
                        {
                            ViewBag.Checkbox60 = _chacked;
                            ViewBag.Checkbox77 = _chacked;
                        }
                        else if (item.response == _responseNO)
                        {
                            ViewBag.Checkbox61 = _chacked;
                            ViewBag.Checkbox78 = _chacked;
                            comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
                            if (comments.Length > 1)
                            {
                                ViewBag.Input14 = comments[1];
                                ViewBag.Input39 = comments[1];
                            }
                        }
                        break;

                    #endregion
                }
            }
            return pptqID;
        }


        public ActionResult CustomizedPDFConfirmation()
        {
            string ViewName = string.Empty;
            int pptqID = 0;
            var question = db.pr_getQuestionnaireByAccesscode(Session["accesscode"].ToString()).FirstOrDefault();
            if (question.footer == "3")
            {
                pptqID = FillCustomPdfHtml(ViewBag, db, Session, Server);
                ViewName = "CustomQuestionnaireSurveyPdfDownload";
                return ViewCustomizedPdf(pptqID, ViewName);

            }
            else if (question.footer == "2")
            {
                pptqID = FillPdfHtml(ViewBag, db, Session, Server);
                ViewName = "CustomizedQuestionnaireSurveyPdfDownload";
                return ViewCustomizedPdf(pptqID, ViewName);
            }
            // else return PDFConfirmation();
            pptqID = FillCustomPdfHtml(ViewBag, db, Session, Server);
            return ViewCustomPdf(pptqID);
        }

        protected ActionResult ViewCustomizedPdf(int pptqID, string ViewName)
        {

            string htmltext = this.RenderActionResultToString(this.View(ViewName));  //name of the view...

            string PDF_FileName = "HON_" + Session["accessCode"].ToString().Substring(1, 4) + ".pdf";

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
                    catch { }
                }

                //FileStream file = new FileStream(fileName, FileMode.Create, System.IO.FileAccess.Write);
                // ConvertApi.Web2Pdf convertApi;
                // convertApi = new ConvertApi.Web2Pdf(400127803);// new ConvertApi.Web2Pdf(3989087);
                byte[] bytes = null;
                bytes = (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(htmltext);
                //using (var convertor = new SimplePechkin(new GlobalConfig()))
                //{
                //    bytes = convertor.Convert(htmltext);
                //}
                // convertApi.ConvertHtml(htmltext, file);


                // file.Close();
                // bytes = System.IO.File.ReadAllBytes(fileName);
                var quest = db.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptqID);
                db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire(quest.id, quest.partner, quest.partnerTypeTouchpointQuestionnaire, quest.accesscode, quest.invitedBy, quest.invitedDate, quest.completedDate, quest.status, 100, quest.zcode, bytes, quest.docFolderAddress, quest.score, quest.loadGroup);

                // Alexander Changed to check invalid zcode
                var result = db.pr_checkForInvalidZcode(pptqID, quest.zcode);
                //db.pr_addPPTQpdf(pptqID, bytes);
                //using (var context = new EntitiesDBContext())
                //{
                //    var pptq = context.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptqID);
                //    pptq.progress = 100;
                //    context.Entry(pptq).State = EntityState.Modified;
                //    context.SaveChanges();
                //}
                //  System.IO.File.Delete(fileName);
                // Send the binary data to the browser.
                return new BinaryContentResult(bytes, "application/pdf");
            }
            return new BinaryContentResult(null, "application/pdf");
        }

        protected ActionResult ViewCustomPdf(int pptqID)
        {
            string htmltext = this.RenderActionResultToString(this.View("CustomQuestionnaireSurveyPdfDownload"));  //name of the view...

            string PDF_FileName = "HON_" + Session["accessCode"].ToString().Substring(1, 4) + ".pdf";

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
                    catch { }
                }

                byte[] bytes = null;
                bytes = (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(htmltext);

                var quest = db.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault(o => o.id == pptqID);
                db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire(quest.id, quest.partner, quest.partnerTypeTouchpointQuestionnaire, quest.accesscode, quest.invitedBy, quest.invitedDate, quest.completedDate, quest.status, 100, quest.zcode, bytes, quest.docFolderAddress, quest.score, quest.loadGroup);
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

        public ActionResult PrintCustomizedPDFConfirmation(string accesscode)
        {
            if (!string.IsNullOrEmpty(accesscode))
            {
                Session["accessCode"] = accesscode;
                if (db.pr_getQuestionnaireByAccesscode(Session["accesscode"].ToString()).FirstOrDefault().footer != "1")
                    Response.Redirect("~/Registration/Home/CustomizedPDFConfirmation");
                else Response.Redirect("~/Registration/Home/PDFConfirmation");
            }
            return RedirectToAction("~/Registration/Home");
        }
    }
}
