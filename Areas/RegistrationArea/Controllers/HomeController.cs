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
using Generic.Areas.RegistrationArea.Models;
using Generic.Areas.RegistrationArea.Services;
namespace Generic.Areas.RegistrationArea.Controllers
{
	public class HomeController : Controller
	{
		private EntitiesDBContext db = new EntitiesDBContext();
		IGoogleTranslatorHelper _translator;
		const string PLEASE_ENTER_ACCESS_CODE = "Please enter your access code:";
		const string ACCESS_CODE_SIPLE_TEXT = "Access Code:";
		const string VERIFY_COMPANY_INFO = "Please verify that your company information is correct";
		const string VERIFY_Purchase_INFO = "Please verify that your purchase order information is correct";
		const string COMPANY_INFORMATION_TEXT = "Company Information";
		const string CONTACT_INFORMATION_TEXT = "Contact Information";
		const string BUYER_INFORMATION_TEXT = "Buyer Information";
		const string Purchase_Order_INFORMATION_TEXT = "Purchase Order Information";
		const string VERIFY_CONTACT_TEXT_INFORMATION = "Please verify that your contact information is correct";
		const string VERIFY_BUYER_TEXT_INFORMATION = "Please verify that your buyer information is correct";

		const string Company = "Company";
		const string Supplier_Number = "Supplier";
		const string Supplier_Name = "Supplier Name";
		const string Purchase_Order = "Purchase Order";
		const string Purchase_Order_Value = "Purchase Order Value";
		const string Change_Amount = "Change Amount";
		const string PO_Version = "PO Version";
		const string Part_Number = "Part Number";
		const string Part_Number_Description = "Part Number Description";

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

		public HomeController()
		{
			_translator = new GoogleTranslatorHelper(new DatabaseTranslationService());
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
			try
			{
				ViewBag.accesscode = Session["accessCode"];
				ViewBagModel objViewBag = new ViewBagModel();

				objViewBag.CMS_PAGE_TITLE = CMS.ACCESS_CODE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = CMS.ACCESS_CODE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = CMS.ACCESS_CODE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = CMS.ACCESS_CODE_PANEL_TWO;
				ViewBag.CMS_FOOTER_ONE = CMS.ACCESS_CODE_FOOTER_ONE;
				ViewBag.CMS_FOOTER_TWO = CMS.ACCESS_CODE_FOOTER_TWO;
				ViewBag.CMS_SUBMIT_TEXT = CMS.ACCESS_CODE_SUBMIT_TEXT.Substring(0, 10);
				ViewBag.RETRIEVE_ACCESS_CODE_TEXT = CMS.RETRIEVE_ACCESS_CODE_TEXT;
				objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT;
				objViewBag.SAVE_FOR_LATER_TEXT_NOTICE = CMS.SAVE_FOR_LATER_TEXT_NOTICE;
				objViewBag.CMS_PAGE_NEXT_TEXT = CMS.SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT;
				//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF;
				//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ;
				//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER;
				//objViewBag.QUESTIONNAIRE_DOC_OTHER_2 = CMS.QUESTIONNAIRE_DOC_OTHER_2;
				var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
				var ppptqCms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

				var cmsId = 0;

				if (ppptqCms != null)
				{
					var enterpriseInfo = ppptqCms.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1.enterpriseSystemInfo.FirstOrDefault();
					if (enterpriseInfo != null)
						ViewBag.ENTERPRISE_URL = enterpriseInfo.companyWebSite;
					_translator.PPTQ = ppptqCms;
					ViewBag.ACCESS_CODE_PLEASE_ENTER = _translator.Translate(PLEASE_ENTER_ACCESS_CODE, CurrentLanguage);
					ViewBag.ACCESS_CODE_SIPLE_TEXT = _translator.Translate(ACCESS_CODE_SIPLE_TEXT, CurrentLanguage);
					var enterpriseDb = db.pr_getPartner(ppptqCms != null ? ppptqCms.partner : 0).FirstOrDefault();
					if (enterpriseDb != null)
					{
						Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(enterpriseDb.enterprise.ToString());
					}

					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptqCms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					ViewBag.ENTERPRISE_ID = ptq != null ? ptq.questionnaire1.enterprise : -1;
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, ptq != null ? ptq.questionnaire : 0, objViewBag);
					//try
					//{
					var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "SaveForLaterConfirm");
					if (!string.IsNullOrEmpty(model.CMS_PAGE_TITLE))
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
					if (!string.IsNullOrEmpty(model.CMS_PAGE_PANEL_ONE))
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
					if (!string.IsNullOrEmpty(model.CMS_PAGE_PANEL_TWO))
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
					if (!string.IsNullOrEmpty(model.CMS_PAGE_PREVIOUS_TEXT))
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
					if (!string.IsNullOrEmpty(model.CMS_PAGE_NEXT_TEXT))
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;
					Session["QuestionnaireTitle"] = model.CMS_PAGE_TITLE;
					objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
					objViewBag.SAVE_FOR_LATER_TEXT_NOTICE = !string.IsNullOrEmpty(model.SAVE_FOR_LATER_TEXT_NOTICE) ? model.SAVE_FOR_LATER_TEXT_NOTICE
						: objViewBag.SAVE_FOR_LATER_TEXT_NOTICE;

					ViewBag.QUESTIONNAIRE_DOC_OTHER_2 = !string.IsNullOrEmpty(model.QUESTIONNAIRE_DOC_OTHER_2) ? model.QUESTIONNAIRE_DOC_OTHER_2
						: ViewBag.QUESTIONNAIRE_DOC_OTHER_2;

					//cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBMIT_TEXT).id;
					//var cms_SubmitText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
					//if (cms_SubmitText != null)
					//{
					//    ViewBag.CMS_SUBMIT_TEXT = cms_SubmitText;
					//}
					//cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.RETRIEVE_ACCESS_CODE_TEXT).id;
					//var ret_AccCode = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
					//if (ret_AccCode != null)
					//{
					//    ViewBag.RETRIEVE_ACCESS_CODE_TEXT = ret_AccCode;
					//}
					//cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_ONE).id;
					//var cms_FooterOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
					//if (cms_FooterOne != null)
					//{
					//    ViewBag.CMS_FOOTER_ONE = cms_FooterOne;
					//}
					//cmsId = questionnairCMSAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_TWO).id;
					//var cms_FooterTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cmsId);
					//if (cms_FooterTwo != null)
					//{
					//    ViewBag.CMS_FOOTER_TWO = cms_FooterTwo;
					//}



				}
				else
				{
					ViewBag.message = "wrongstatus";
				}
				ViewBag.FormData = objViewBag;
			}
			catch (Google.GoogleApiException ex)
			{
				ViewBag.googleError = "Sorry, translation limit has been reached on server.";
				CurrentLanguage = "en";
				return SaveForLaterConfirm();
			}
			catch (Exception exc)
			{
			}

			return View();
		}

		public virtual ActionResult Index(string id = "", string accessCode = null, bool? advanced = null)
		{
			try
			{
				ViewBagModel objViewBag = new ViewBagModel();
				ViewBag.accesscode = accessCode;

				objViewBag.CMS_PAGE_TITLE = CMS.ACCESS_CODE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = CMS.ACCESS_CODE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = CMS.ACCESS_CODE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = CMS.ACCESS_CODE_PANEL_TWO;
				objViewBag.CMS_FOOTER_ONE = CMS.ACCESS_CODE_FOOTER_ONE;
				objViewBag.CMS_FOOTER_TWO = CMS.ACCESS_CODE_FOOTER_TWO;
				objViewBag.CMS_SUBMIT_TEXT = CMS.ACCESS_CODE_SUBMIT_TEXT.Substring(0, 10);
				objViewBag.RETRIEVE_ACCESS_CODE_TEXT = CMS.RETRIEVE_ACCESS_CODE_TEXT;


				var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

				var cmsId = 0;
				if (ppptq_cms != null)
				{
					_translator.PPTQ = ppptq_cms;
					objViewBag.ACCESS_CODE_PLEASE_ENTER = _translator.Translate(PLEASE_ENTER_ACCESS_CODE, CurrentLanguage);
					objViewBag.ACCESS_CODE_SIPLE_TEXT = _translator.Translate(ACCESS_CODE_SIPLE_TEXT, CurrentLanguage);

					var enterPrise = db.pr_getPartner(ppptq_cms.partner).FirstOrDefault();
					if (enterPrise != null)
					{
						Generic.Helpers.CurrentInstance.EnterpriseID =
							Int32.Parse(enterPrise.enterprise.ToString());
					}
					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					objViewBag.ENTERPRISE_ID = ptq != null ? ptq.questionnaire1.enterprise : -1;
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : -1).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					//try
					//{
					var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "Index");

					objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
					objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
					objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
					objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
					Session["QuestionnaireTitle"] = model.CMS_PAGE_TITLE;
					if (!string.IsNullOrEmpty(model.CMS_FOOTER_ONE))
						objViewBag.CMS_FOOTER_ONE = model.CMS_FOOTER_ONE;
					if (!string.IsNullOrEmpty(model.CMS_FOOTER_TWO))
						objViewBag.CMS_FOOTER_TWO = model.CMS_FOOTER_TWO;
					if (!string.IsNullOrEmpty(model.CMS_SUBMIT_TEXT))
						objViewBag.CMS_SUBMIT_TEXT = model.CMS_SUBMIT_TEXT;
					if (!string.IsNullOrEmpty(model.RETRIEVE_ACCESS_CODE_TEXT))
						objViewBag.RETRIEVE_ACCESS_CODE_TEXT = model.RETRIEVE_ACCESS_CODE_TEXT;
					//}
					//catch (Exception exc)
					//{
					//}
				}
				else
				{
					ViewBag.message = "wrongstatus";
				}
				ViewBag.FormData = objViewBag;
				if (advanced.HasValue && advanced.Value)
					return GenerateIndex(accessCode, advanced);
			}
			catch (Google.GoogleApiException ex)
			{
				ViewBag.googleError = "Sorry, translation limit has been reached on server.";
				CurrentLanguage = "en";
				return Index(id, accessCode, advanced);
			}
			catch (Exception exc)
			{
			}
			return View();
		}

		protected virtual ActionResult GenerateIndex(string accessCode, bool? advanced = null)
		{

			ViewBagModel objViewBag = new ViewBagModel();
			ViewBag.accesscode = accessCode;

			objViewBag.CMS_PAGE_TITLE = CMS.ACCESS_CODE_TITLE;
			objViewBag.CMS_PAGE_SUBTITLE = CMS.ACCESS_CODE_SUBTITLE;
			objViewBag.CMS_PAGE_PANEL_ONE = CMS.ACCESS_CODE_PANEL_ONE;
			objViewBag.CMS_PAGE_PANEL_TWO = CMS.ACCESS_CODE_PANEL_TWO;
			objViewBag.CMS_FOOTER_ONE = CMS.ACCESS_CODE_FOOTER_ONE;
			objViewBag.CMS_FOOTER_TWO = CMS.ACCESS_CODE_FOOTER_TWO;
			objViewBag.RETRIEVE_ACCESS_CODE_TEXT = CMS.RETRIEVE_ACCESS_CODE_TEXT;
			objViewBag.CMS_SUBMIT_TEXT = "Login";

			int cmsId = 0;
			var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			if (ppptq != null)
			{
				_translator.PPTQ = ppptq;
				objViewBag.ACCESS_CODE_PLEASE_ENTER = _translator.Translate(PLEASE_ENTER_ACCESS_CODE, CurrentLanguage);
				objViewBag.ACCESS_CODE_SIPLE_TEXT = _translator.Translate(ACCESS_CODE_SIPLE_TEXT, CurrentLanguage);
				if (new int[] { 6, 7, 8 }.Contains(ppptq.status))
				{
					var objPartner = db.pr_getPartner(ppptq.partner).FirstOrDefault();
					if (objPartner != null)
						Generic.Helpers.CurrentInstance.EnterpriseID = Int32.Parse(objPartner.enterprise.ToString());


					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					int questionnaire = ptq != null ? ptq.questionnaire : -1;
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(questionnaire).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					try
					{
						var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "Index");

						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						Session["QuestionnaireTitle"] = model.CMS_PAGE_TITLE;
						if (!string.IsNullOrEmpty(model.CMS_FOOTER_ONE))
							objViewBag.CMS_FOOTER_ONE = model.CMS_FOOTER_ONE;
						if (!string.IsNullOrEmpty(model.CMS_FOOTER_TWO))
							objViewBag.CMS_FOOTER_TWO = model.CMS_FOOTER_TWO;
						if (!string.IsNullOrEmpty(model.CMS_SUBMIT_TEXT))
							objViewBag.CMS_SUBMIT_TEXT = model.CMS_SUBMIT_TEXT;
						if (!string.IsNullOrEmpty(model.RETRIEVE_ACCESS_CODE_TEXT))
							objViewBag.RETRIEVE_ACCESS_CODE_TEXT = model.RETRIEVE_ACCESS_CODE_TEXT;

					}
					catch { }

					var touchpoint = db.pr_getTouchpoint(ptq != null ? ptq.touchpoint : -1).FirstOrDefault();

					var responseTypesQuestionnaire = db.pr_getResponseTypeByQuestionnaire(questionnaire).ToList();
					var objQuestionnaire = db.pr_getQuestionnaire(questionnaire).FirstOrDefault();

					if (touchpoint != null && touchpoint.endDate >= DateTime.Now)
					{
						Session["accessCode"] = accessCode;
						Session["hs3Registration"] = 1;
						Session["languageused"] = "en";
						Session["accessAttemp"] = 0;
						Session["partner"] = ppptq.partner;
						Session["touchpoint"] = touchpoint.id;
						Session["partnerType"] = ptq != null ? ptq.partnerType : -1;
						Session["questionnaire"] = questionnaire;
						Session["leveltype"] = objQuestionnaire != null ? objQuestionnaire.levelType : -1;
						Session["protocol"] = touchpoint.protocol;
						Session["responseTypesQuestionnaire"] = responseTypesQuestionnaire;
						Session["currentEmail"] = objPartner != null ? objPartner.email : "";

						List<CustomizedLSMW> customizedLsmw = new List<CustomizedLSMW>();
						Session["CustomizedLSMW"] = customizedLsmw;

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
			try
			{
				return GenerateIndex(accessCode, false);
			}
			catch (Google.GoogleApiException ex)
			{
				ViewBag.googleError = "Sorry, translation limit has been reached on server.";
				CurrentLanguage = "en";
				return Index(accessCode);
			}
			catch (Exception exc)
			{
			}
			return View();
		}

		public ViewBagModel GetPageTitles(int cms_id, List<pr_getQuestionnaireCMSAll_Result> questionnairCmsAll, partnerTypeTouchpointQuestionnaire ptq, string pageName, partnerPartnertypeTouchpointQuestionnaire pptq = null, List<questionnaireQuestionnaireCMS> questionareCMS = null)
		{
			ViewBagModel objViewBagModel = new ViewBagModel();
			try
			{
				pr_getQuestionnaireCMSAll_Result cms = new pr_getQuestionnaireCMSAll_Result();
				string pageTitle = string.Empty;
				string pageSubTitle = string.Empty;
				string pagePanelOne = string.Empty;
				string pagePanelTwo = string.Empty;
				string pagePreviousText = string.Empty;
				string pageNextText = string.Empty;
				switch (pageName)
				{
					case "CompanyInformation":
						pageTitle = CMS.COMPANY_PAGE_TITLE;
						pageSubTitle = CMS.COMPANY_PAGE_SUBTITLE;
						pagePanelOne = CMS.COMPANY_PAGE_PANEL_ONE;
						pagePanelTwo = CMS.COMPANY_PAGE_PANEL_TWO;
						pagePreviousText = CMS.COMPANY_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.COMPANY_PAGE_NEXT_TEXT;
						break;

					case "ContactInformation":
						pageTitle = CMS.CONTACT_PAGE_TITLE;
						pageSubTitle = CMS.CONTACT_PAGE_SUBTITLE;
						pagePanelOne = CMS.CONTACT_PAGE_PANEL_ONE;
						pagePanelTwo = CMS.CONTACT_PAGE_PANEL_TWO;
						pagePreviousText = CMS.CONTACT_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.CONTACT_PAGE_NEXT_TEXT;
						break;
					case "EditCompanyInformation":
						pageTitle = CMS.COMPANY_EDIT_PAGE_TITLE;
						pageSubTitle = CMS.COMPANY_EDIT_PAGE_SUBTITLE;
						pagePanelOne = CMS.COMPANY_EDIT_PAGE_PANEL_ONE;
						pagePanelTwo = CMS.COMPANY_EDIT_PAGE_PANEL_TWO;
						pagePreviousText = CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.COMPANY_EDIT_PAGE_NEXT_TEXT;
						break;
					case "EditContactInformation":
						pageTitle = CMS.CONTACT_EDIT_PAGE_TITLE;
						pageSubTitle = CMS.CONTACT_EDIT_PAGE_SUBTITLE;
						pagePanelOne = CMS.CONTACT_EDIT_PAGE_PANEL_ONE;
						pagePanelTwo = CMS.CONTACT_EDIT_PAGE_PANEL_TWO;
						pagePreviousText = CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.CONTACT_EDIT_PAGE_NEXT_TEXT;
						break;
					case "ESignature":
						pageTitle = CMS.ESIGNATURE_PAGE_TITLE;
						pageSubTitle = CMS.ESIGNATURE_PAGE_SUBTITLE;
						pagePanelOne = CMS.ESIGNATURE_PAGE_PANEL_ONE;
						pagePanelTwo = CMS.ESIGNATURE_PAGE_PANEL_TWO;
						pagePreviousText = CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.ESIGNATURE_PAGE_NEXT_TEXT;
						break;
					case "Finish":
						pageTitle = CMS.CONFIRMATION_PAGE_TITLE;
						pageSubTitle = CMS.CONFIRMATION_PAGE_SUBTITLE;
						pagePanelOne = CMS.CONFIRMATION_PAGE_PANEL_ONE;
						pagePanelTwo = CMS.CONFIRMATION_PAGE_PANEL_TWO;
						pagePreviousText = CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.CONFIRMATION_PAGE_NEXT_TEXT;
						break;
					case "SaveForLaterConfirm":
						pageTitle = CMS.ACCESS_CODE_TITLE;
						pageSubTitle = CMS.ACCESS_CODE_SUBTITLE;
						pagePanelOne = CMS.ACCESS_CODE_PANEL_ONE;
						pagePanelTwo = CMS.ACCESS_CODE_PANEL_TWO;
						pagePreviousText = CMS.SAVE_FOR_LATER_TEXT_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.SAVE_FOR_LATER_TEXT_PAGE_NEXT_TEXT;
						break;
					case "Index":
						pageTitle = CMS.ACCESS_CODE_TITLE;
						pageSubTitle = CMS.ACCESS_CODE_SUBTITLE;
						pagePanelOne = CMS.ACCESS_CODE_PANEL_ONE;
						pagePanelTwo = CMS.ACCESS_CODE_PANEL_TWO;
						break;
					case "QuestionnaireResponse":
						pageTitle = CMS.QUESTIONNAIRE_PAGE_TITLE;
						pageSubTitle = CMS.QUESTIONNAIRE_PAGE_SUBTITLE;
						pagePanelOne = CMS.QUESTIONNAIRE_PAGE_PANEL_ONE;
						pagePanelTwo = CMS.QUESTIONNAIRE_PAGE_PANEL_TWO;
						pagePreviousText = CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT;
						pageNextText = CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT;
						break;
				}

				int qusetionnarie = ptq != null ? ptq.questionnaire : 0;
				cms = questionnairCmsAll.FirstOrDefault(q => q.description == pageTitle);
				cms_id = cms != null ? cms.id : 0;
				var cmsPageTitle = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage, cms_id);
				if (cmsPageTitle != null)
					objViewBagModel.CMS_PAGE_TITLE = cmsPageTitle;

				cms = questionnairCmsAll.FirstOrDefault(q => q.description == pageSubTitle);
				cms_id = cms != null ? cms.id : 0;
				var cmsPageSubtitle = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage, cms_id);
				if (cmsPageSubtitle != null)
					objViewBagModel.CMS_PAGE_SUBTITLE = cmsPageSubtitle;

				cms = questionnairCmsAll.FirstOrDefault(q => q.description == pagePanelOne);
				cms_id = cms != null ? cms.id : 0;
				var cmsPagePanelOne = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage, cms_id);
				if (cmsPagePanelOne != null)
					objViewBagModel.CMS_PAGE_PANEL_ONE = cmsPagePanelOne;

				cms = questionnairCmsAll.FirstOrDefault(q => q.description == pagePanelTwo);
				cms_id = cms != null ? cms.id : 0;
				var cmsPagePanelTwo = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage, cms_id);
				if (cmsPagePanelTwo != null)
					objViewBagModel.CMS_PAGE_PANEL_TWO = cmsPagePanelTwo;
				if (pageName != "Index")
				{
					cms = questionnairCmsAll.FirstOrDefault(q => q.description == pagePreviousText);
					cms_id = cms != null ? cms.id : 0;
					var cmsPagePreviousText = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage,
						cms_id);
					if (cmsPagePreviousText != null)
						objViewBagModel.CMS_PAGE_PREVIOUS_TEXT = cmsPagePreviousText;

					cms = questionnairCmsAll.FirstOrDefault(q => q.description == pageNextText);
					cms_id = cms != null ? cms.id : 0;
					var cmsPageNextText = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage,
						cms_id);
					if (cmsPageNextText != null)
						objViewBagModel.CMS_PAGE_NEXT_TEXT = cmsPageNextText;
				}
				if (pageName == "ESignature")
				{
					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.ESIGNATURE_PAGE_TEXT);
					cms_id = cms != null ? cms.id : 0;
					var floatText = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage, cms_id);
					objViewBagModel.ESIGNATURE_PAGE_TEXT = new EmailFormat().sGetEmailBody(floatText, null, pptq != null ? pptq.partner1 : null, null, ptq != null ? ptq.id : 0);


					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER_2);
					cms_id = cms != null ? cms.id : 0;
					var questionnaireDocOther2 = _translator.Translate(qusetionnarie, TranslationType.CMS, CurrentLanguage, cms_id);
					if (questionnaireDocOther2 != null)
					{
						objViewBagModel.QUESTIONNAIRE_DOC_OTHER_2 = questionnaireDocOther2;
					}
				}
				if (pageName == "Finish")
				{
					try
					{
						cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER_2);
						cms_id = cms != null ? cms.id : 0;
						objViewBagModel.QUESTIONNAIRE_DOC_OTHER_2 = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					}
					catch { }
					try
					{
						if (questionareCMS != null)
						{
							cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT);
							cms_id = cms != null ? cms.id : 0;
							objViewBagModel.CMS_PAGE_PREVIOUS_LINK = questionareCMS.FirstOrDefault(x => x.questionnaireCMS == cms_id).link;
						}
					}
					catch { }
					try
					{
						cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.CONFIRMATION_PAGE_NEXT_TEXT);
						cms_id = cms != null ? cms.id : 0;

						var cms_PageNextText = questionareCMS.FirstOrDefault(x => x.questionnaireCMS == cms_id);
						if (cms_PageNextText != null)
						{
							ViewBag.CMS_PAGE_NEXT_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);

							if (cms_PageNextText.link == null)
							{
								objViewBagModel.CMS_PAGE_NEXT_LINK = db.pr_getEnterpriseSystemInfo(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault().companyWebSite;

							}
							else
							{
								objViewBagModel.CMS_PAGE_NEXT_LINK = cms_PageNextText.link;
							}
						}
					}
					catch { }


					try
					{
						cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.CONFIRMATION_PAGE_SIGNOFF_STATEMENT);
						cms_id = cms != null ? cms.id : 0;
						objViewBagModel.CONFIRMATION_PAGE_SIGNOFF_STATEMENT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					}
					catch { }

					try
					{
						cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.CONFIRMATION_PAGE_HEADLINE);
						cms_id = cms != null ? cms.id : 0;
						objViewBagModel.CONFIRMATION_PAGE_HEADLINE = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					}
					catch { }
					try
					{
						cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT);
						cms_id = cms != null ? cms.id : 0;
						objViewBagModel.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					}
					catch { } try
					{
						cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.WARNING);
						cms_id = cms != null ? cms.id : 0;
						objViewBagModel.WARNING = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);

					}
					catch { }
				}
				if (pageName == "SaveForLaterConfirm")
				{
					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER_2);
					cms_id = cms != null ? cms.id : 0;
					var questionaireDoc2 = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					if (questionaireDoc2 != null)
						objViewBagModel.QUESTIONNAIRE_DOC_OTHER_2 = questionaireDoc2;

					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT_NOTICE);
					cms_id = cms != null ? cms.id : 0;
					var textNotice = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					if (textNotice != null)
						objViewBagModel.SAVE_FOR_LATER_TEXT_NOTICE = textNotice;
				}
				if (pageName == "Index")
				{
					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_ONE);
					cms_id = cms != null ? cms.id : 0;
					var cmsFooterOne = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					if (cmsFooterOne != null)
					{
						objViewBagModel.CMS_FOOTER_ONE = cmsFooterOne;
					}

					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_FOOTER_TWO);
					cms_id = cms != null ? cms.id : 0;
					var cmsFooterTwo = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					if (cmsFooterTwo != null)
					{
						objViewBagModel.CMS_FOOTER_TWO = cmsFooterTwo;
					}

					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.ACCESS_CODE_SUBMIT_TEXT);
					cms_id = cms != null ? cms.id : 0;
					var cmsSubmitText = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					if (cmsSubmitText != null)
					{
						objViewBagModel.CMS_SUBMIT_TEXT = cmsSubmitText;
					}

					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.RETRIEVE_ACCESS_CODE_TEXT);
					cms_id = cms != null ? cms.id : 0;
					var retAccCode = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
					if (retAccCode != null)
					{
						objViewBagModel.RETRIEVE_ACCESS_CODE_TEXT = retAccCode;
					}
				}
				if (pageName == "QuestionnaireResponse")
				{
					cms = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT);
					cms_id = cms != null ? cms.id : 0;
					var cmsSaveForLater = questionareCMS.FirstOrDefault(x => x.questionnaireCMS == cms_id);
					if (cmsSaveForLater != null)
						objViewBagModel.SAVE_FOR_LATER_TEXT = _translator.Translate(ptq.questionnaire, TranslationType.CMS, CurrentLanguage, cms_id);
				}
			}
			catch (Exception)
			{

			}
			return objViewBagModel;
		}


		public virtual ActionResult CompanyInformation()
		{
			try
			{
				if (Session["hs3Registration"] == null)
				{
					return RedirectToAction("Default");
				}
				ViewBagModel objViewBag = new ViewBagModel();

				partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
				try
				{
					var country = db.pr_getCountry(objPartner == null ? 0 : objPartner.country).FirstOrDefault();
					objViewBag.country = country != null ? country.name : "";

				}
				catch { }
				try
				{
					var state = db.pr_getState(objPartner != null ? objPartner.state : 0).FirstOrDefault();
					objViewBag.state = state != null ? state.stateCode : "";
				}
				catch { }


				objViewBag.CMS_PAGE_TITLE = CMS.COMPANY_PAGE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = CMS.COMPANY_PAGE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = CMS.COMPANY_PAGE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = CMS.COMPANY_PAGE_PANEL_TWO;
				objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.COMPANY_PAGE_PREVIOUS_TEXT;
				objViewBag.CMS_PAGE_NEXT_TEXT = CMS.COMPANY_PAGE_NEXT_TEXT;


				//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
				//objViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);
				var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
				var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
				int cms_id = 0;
				if (ppptq != null)
				{
					_translator.PPTQ = ppptq;
					objViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_COMPANY_INFO, CurrentLanguage);
					objViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(COMPANY_INFORMATION_TEXT, CurrentLanguage);
					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();

					try
					{
						var model = GetPageTitles(cms_id, questionnairCmsAll, ptq, "CompanyInformation");
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;
						objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, ptq != null ? ptq.questionnaire : 0, objViewBag);
					}
					catch { }

					//PODS //Purchase_Order_INFORMATION_TEXT
					if (question.footer == "7" || question.footer == "8" || question.footer == "9")
					{
						objViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_Purchase_INFO, CurrentLanguage);
						objViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(Purchase_Order_INFORMATION_TEXT, CurrentLanguage);

						ViewBag.FormData = objViewBag;
						return View("CompanyInformationPODS", objPartner);
					}
				}

				ViewBag.FormData = objViewBag;
				return View(objPartner);
			}
			catch (Google.GoogleApiException ex)
			{
				ViewBag.googleError = "Sorry, translation limit has been reached on server.";
				CurrentLanguage = "en";
				return CompanyInformation();
			}
			catch (Exception exc)
			{
			}
			return View();
		}

		public virtual ActionResult ContactInformation()
		{
			try
			{
				if (Session["hs3Registration"] == null)
				{
					return RedirectToAction("Default");
				}
				ViewBagModel objViewBag = new ViewBagModel();
				partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
				try
				{

					var country = db.pr_getCountry(objPartner == null ? 0 : objPartner.country).FirstOrDefault();
					objViewBag.country = country != null ? country.name : "";
				}
				catch { }

				objViewBag.CMS_PAGE_TITLE = CMS.CONTACT_PAGE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = CMS.CONTACT_PAGE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = CMS.CONTACT_PAGE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = CMS.CONTACT_PAGE_PANEL_TWO;
				objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONTACT_PAGE_PREVIOUS_TEXT;
				objViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONTACT_PAGE_NEXT_TEXT;

				//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
				//objViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);
				var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
				var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

				var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();


				var cmsId = 0;
				if (ppptq != null)
				{
					//PODS //Purchase_Order_INFORMATION_TEXT
					if (question.footer == "7" || question.footer == "8" || question.footer == "9")
					{
						_translator.PPTQ = ppptq;
						objViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(BUYER_INFORMATION_TEXT, CurrentLanguage);
						objViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_BUYER_TEXT_INFORMATION, CurrentLanguage);
						var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
						var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
						var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
						try
						{
							var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "ContactInformation");
							objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
							objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
							objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
							objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
							objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
							objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;
							objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, ptq != null ? ptq.questionnaire : 0, objViewBag);


						}
						catch { }
					}
					else
					{
						_translator.PPTQ = ppptq;
						objViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(CONTACT_INFORMATION_TEXT, CurrentLanguage);
						objViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_CONTACT_TEXT_INFORMATION, CurrentLanguage);
						var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
						var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
						var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
						try
						{
							var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "ContactInformation");
							objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
							objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
							objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
							objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
							objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
							objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;
							objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, ptq != null ? ptq.questionnaire : 0, objViewBag);


						}
						catch { }
					}
				}
				ViewBag.FormData = objViewBag;
				if (question.footer == "7" || question.footer == "8" || question.footer == "9")
					return View("ContactInformationPODS", objPartner);
				else
					return View(objPartner);
			}
			catch (Google.GoogleApiException ex)
			{
				ViewBag.googleError = "Sorry, translation limit has been reached on server.";
				CurrentLanguage = "en";
				return ContactInformation();
			}
			catch (Exception exc)
			{
			}
			return View();
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

			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var pptqid = pptq != null ? pptq.id : 0;
			if (objQuestionnaire != null)
			{
				if (objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
				{
					var statuses =
						db.pr_getPartnumberSiteZcodePPTQByPPTQ(pptqid)
							.ToList()
							.Select(x => x.status)
							.Distinct()
							.ToList();
					if (statuses.Any(o => o != Status.COMPLETED))
						return RedirectToAction("QuestionnaireResponse", "PartNumber");
					else return RedirectToAction("ESignature");
				}

				else if (objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.COMPANY_LEVEL ||
						 objQuestionnaire.levelType == Generic.Helpers.Questionnaire.LevelType.SUBSCRIPTION)
				{
					return RedirectToAction("QuestionnaireResponse");
				}
				else
				{
					return Json(new { Questionnaire = "Questionnaire Not Found" }, JsonRequestBehavior.AllowGet);
				}
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

			if (question != null && answer != null && question.commentType.HasValue && (question.commentType.Value == CommentType.YN_REFERENCE_Y || question.commentType.Value == CommentType.YN_REFERENCE_N) && qresponse != null && !string.IsNullOrEmpty(qresponse.comment))
			{

				if ((question.commentType.Value == CommentType.YN_REFERENCE_Y && answer.id == 74) || (question.commentType.Value == CommentType.YN_REFERENCE_N && answer.id == 75))
					ReferenceEmails.Add(qresponse.comment);
			
				//if (!(bool)Session["YN_REFERENCE"])
				//	Session["YN_REFERENCE"] = question.commentType.Value == CommentType.YN_REFERENCE_N && answer.id == 75;
				//Session["YN_REFERENCE_EMAIL"] = qresponse.comment;

			}
		}

		protected List<string> ReferenceEmails
		{
			get
			{
				if (Session["ReferenceEmails"] == null)
					Session["ReferenceEmails"] = new List<string>();
				return (List<string>)Session["ReferenceEmails"];
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
			try
			{
				ViewBagModel objViewBag = new ViewBagModel();
				if (Session["hs3Registration"] == null)
				{
					return RedirectToAction("Default");
				}
				#region CMS
				objViewBag.CMS_PAGE_TITLE = CMS.QUESTIONNAIRE_PAGE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = CMS.QUESTIONNAIRE_PAGE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = CMS.QUESTIONNAIRE_PAGE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = CMS.QUESTIONNAIRE_PAGE_PANEL_TWO;
				objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT;
				objViewBag.CMS_PAGE_NEXT_TEXT = CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT;
				objViewBag.SAVE_FOR_LATER_TEXT = CMS.SAVE_FOR_LATER_TEXT;

				//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF;
				//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ;
				//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER;
				//objViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO;
				//objViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL;

				var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
				var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
				int cmsId = 0;//pr_getPartnerQuestionResponseByAccessCode4
				if (ppptq_cms != null)
				{
					_translator.PPTQ = ppptq_cms;
					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					int questionnaire = ptq != null ? ptq.questionnaire : -1;
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(questionnaire).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					try
					{
						var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "QuestionnaireResponse", null, cms);
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;
						objViewBag.SAVE_FOR_LATER_TEXT = model.SAVE_FOR_LATER_TEXT;

						objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);

					}
					catch { }
				}
				#endregion
				ViewBag.FormData = objViewBag;

				int questionnaireId = (int)Session["questionnaire"];



				//var items = db.pr_getPartnerQuestionResponseByAccessCode5("270387ER").ToList();
				//items.Count(); 

				var questioncount = db.pr_getQuestionCountByQuestionnaire(questionnaireId);
				int totalCount = questioncount != null ? (int)questioncount.FirstOrDefault() : 0;
				var rows = db.pr_getQuestionRowIDByQuestionnaire(questionnaireId).ToList();

				var pages = (from p in rows
							 group p by p.page into g
							 select new { Page = g.Key, Items = g.ToList() }).ToList();


				if (ppptq_cms != null && ppptq_cms.status == 8)
				{
					List<PageDetails> ps = new List<PageDetails>();
					int i = 0;
					int qindex = 0;
					foreach (var p in pages)
					{
						string url = Url.Action("QuestionnaireResponse", "Home", new { pageNumber = i + 1, page = p.Page, jumpToQuestion = 0, questionIndex = qindex });
						ps.Add(new PageDetails
						{
							Url = url,
							QuestionsCount = p.Items.Count,
							Title = "Page " + (i + 1) + ":<br/>" + string.Join("<br/>",
							p.Items.Select(o => o.title).ToArray())
						});
						i++;
						qindex = p.Items.Count;
					}


					ViewBag.Pages = ps;
					ViewBag.IsUseNavigateProgressBar = true;
				}


				foreach (var row in rows)
				{
					if (row.page == page)
					{
						if (ppptq_cms.status == 8)
						{
							//used pages count for progress bar
							var p = pages.Where(o => o.Page == page).FirstOrDefault();
							totalCount = pages.Count();
							int currentIndex = 0;
							if (p != null)
							{
								currentIndex = pages.IndexOf(p);
							}

							var percentageprogressbar = currentIndex / (float)totalCount * 100;
							ViewBag.percentageProgressbar = percentageprogressbar;
						}
						else
						{
							var percentageprogressbar = ((row.row - 1) / (float)totalCount) * 100;
							ViewBag.percentageProgressbar = percentageprogressbar;
						}
						break;
					}
				}
				questionnaire objQuestionnaire = db.pr_getQuestionnaire(questionnaireId).FirstOrDefault();

				touchpoint objtouchpoint = new touchpoint();
				partner objpartner = new partner();
				protocol objprotocol = new protocol();
				if (ReferenceEmails.Count>0)
				{
					//var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
					var pptqObj = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
					var pr = db.pr_getPTQreferencePTQ(pptqObj.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					var currentPtq = db.pr_getPartnertypeTouchpointQuestionnaire(pr.ptqReference).FirstOrDefault();

					ViewBag.ErrorMessage = @"We are about to send reference request  for " + pptqObj.partner1.name + " to this email address: " + (ReferenceEmails.Count > 1 ? ReferenceEmails.Aggregate((o, p) => o += p + ", ") : ReferenceEmails[0]) + ". Is that OK?";
					
				}
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
			}
			catch (Google.GoogleApiException ex)
			{
				ViewBag.googleError = "Sorry, translation limit has been reached on server.";
				CurrentLanguage = "en";
				return QuestionnaireResponse(questionIndex, jumpToQuestion, page, errorQuestion, pageNumber, errorMessage);
			}
			catch (Exception exc)
			{
			}
			return View();
		}

		public void getZcodeByProviderProtocolCampaignQuestionnaire()
		{
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
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
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

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

			if (objQuestion != null && objQuestion.responseType == ResponseType.DROPDOWN)
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
			var response = responseTypesQuestionnaire.FirstOrDefault(r => r.id == questionId);
			if (response != null)
			{
				if (response.description == "text")
				{
					NewZcodePart2_CurrentQuestion = ZCode.XX_Comment_Only_Question;
				}
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
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
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
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var pptqObj = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
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
					else if (keyName.ToString().Contains("_TEXTNUMBER"))
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
							jumpToQuestion = NextPageCalculationService.GetJumpToQuestion(objQuestion, db, pptq);
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

		[ValidateInput(false)]
		public ActionResult SaveDataFromHint(int questionid, string text)
		{
			var question = db.questions.FirstOrDefault(o => o.id == questionid);
			if (question != null)
			{
				question.commentBoxTxt = text;
				db.Entry(question).State = EntityState.Modified;
				db.SaveChanges();
			}
			else
			{
				return Json("There is no a such question");
			}
			return Json(false);
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
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			_translator.PPTQ = ppptq_cms;
			ViewBagModel objViewBag = new ViewBagModel();
			objViewBag.CMS_PAGE_TITLE = CMS.COMPANY_EDIT_PAGE_TITLE;
			objViewBag.CMS_PAGE_SUBTITLE = CMS.COMPANY_EDIT_PAGE_SUBTITLE;
			objViewBag.CMS_PAGE_PANEL_ONE = CMS.COMPANY_EDIT_PAGE_PANEL_ONE;
			objViewBag.CMS_PAGE_PANEL_TWO = CMS.COMPANY_EDIT_PAGE_PANEL_TWO;
			objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT;
			objViewBag.CMS_PAGE_NEXT_TEXT = CMS.COMPANY_EDIT_PAGE_NEXT_TEXT;

			//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
			//objViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);


			partner partner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
			if (partner == null)
			{
				return HttpNotFound();
			}


			int cmsId = 0;
			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			//PODS //Purchase_Order_INFORMATION_TEXT
			if (question.footer == "7" || question.footer == "8" || question.footer == "9")
			{
				if (ppptq_cms != null)
				{
					objViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(Purchase_Order_INFORMATION_TEXT, CurrentLanguage);
					objViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_Purchase_INFO, CurrentLanguage);
					objViewBag.Company = _translator.Translate(Supplier_Number, CurrentLanguage) + "#";
					objViewBag.PHYSYCAL_ADDRESS = _translator.Translate(Supplier_Name, CurrentLanguage);
					objViewBag.ADDRESS_ONE = _translator.Translate(Purchase_Order, CurrentLanguage) + "#";

					objViewBag.ADDRESS_TWO = _translator.Translate(Purchase_Order_Value, CurrentLanguage);
					objViewBag.CITY = _translator.Translate(Change_Amount, CurrentLanguage);
					objViewBag.STATE_TEXT = _translator.Translate(PO_Version, CurrentLanguage) + "#";
					objViewBag.POSTAL_CODE = _translator.Translate(Part_Number, CurrentLanguage);
					objViewBag.PROVINCE = _translator.Translate(Part_Number_Description, CurrentLanguage);
					objViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);


					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					try
					{
						int questionnaire = ptq != null ? ptq.questionnaire : 0;
						var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "EditCompanyInformation");
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;

						objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);
					}
					catch { }
				}


				partner.province = partner.internalID.Replace(partner.address1 + " ", "");
				objViewBag.IsPODS = true;
				ViewBag.FormData = objViewBag;
				return View("EditCompanyInformationPODS", partner);
			}



			if (ppptq_cms != null)
			{
				objViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(COMPANY_INFORMATION_TEXT, CurrentLanguage);
				objViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_COMPANY_INFO, CurrentLanguage);
				objViewBag.Company = _translator.Translate(Company, CurrentLanguage);
				objViewBag.PHYSYCAL_ADDRESS = _translator.Translate(PHYSYCAL_ADDRESS, CurrentLanguage);
				objViewBag.ADDRESS_ONE = _translator.Translate(ADDRESS_ONE, CurrentLanguage);
				objViewBag.ADDRESS_TWO = _translator.Translate(ADDRESS_TWO, CurrentLanguage);
				objViewBag.CITY = _translator.Translate(CITY, CurrentLanguage);
				objViewBag.STATE_TEXT = _translator.Translate(STATE_TEXT, CurrentLanguage);
				objViewBag.POSTAL_CODE = _translator.Translate(POSTAL_CODE, CurrentLanguage);
				objViewBag.PROVINCE = _translator.Translate(PROVINCE, CurrentLanguage);
				objViewBag.COUNTRY_TEXT = _translator.Translate(COUNTRY_TEXT, CurrentLanguage);
				objViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);
				var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
				var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
				var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
				try
				{
					int questionnaire = ptq != null ? ptq.questionnaire : 0;
					var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "EditCompanyInformation");
					objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
					objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
					objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
					objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
					objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
					objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;

					objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);
				}
				catch { }
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
			ViewBag.FormData = objViewBag;
			return View(partner);
		}
		[HttpPost]
		public ActionResult EditCompanyInformation(partner partner, bool? ispods)
		{
			if (Session["hs3Registration"] == null)
			{
				return RedirectToAction("Default");
			}

			partner objpartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
			if (objpartner != null)
			{
				if (ispods == null)
				{
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
				}
				else
				{

					objpartner.dunsNumber = partner.dunsNumber;
					objpartner.name = partner.name;
					objpartner.address1 = partner.address1;
					objpartner.address2 = partner.address2;
					objpartner.city = partner.city;
					objpartner.zipcode = partner.zipcode;
					objpartner.province = partner.province;
					objpartner.internalID = partner.address1 + " " + partner.province;
					objpartner.title = partner.title;
					if (ModelState.IsValid)
					{
						db.Entry(objpartner).State = EntityState.Modified;
						db.SaveChanges();
						return RedirectToAction("CompanyInformation");
					}
				}
			}

			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			_translator.PPTQ = ppptq_cms;

			ViewBagModel objViewBag = new ViewBagModel();

			objViewBag.CMS_PAGE_TITLE = CMS.COMPANY_EDIT_PAGE_TITLE;
			objViewBag.CMS_PAGE_SUBTITLE = CMS.COMPANY_EDIT_PAGE_SUBTITLE;
			objViewBag.CMS_PAGE_PANEL_ONE = CMS.COMPANY_EDIT_PAGE_PANEL_ONE;
			objViewBag.CMS_PAGE_PANEL_TWO = CMS.COMPANY_EDIT_PAGE_PANEL_TWO;
			objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.COMPANY_EDIT_PAGE_PREVIOUS_TEXT;
			objViewBag.CMS_PAGE_NEXT_TEXT = CMS.COMPANY_EDIT_PAGE_NEXT_TEXT;

			//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
			//objViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

			int cmsId = 0;

			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			//PODS //Purchase_Order_INFORMATION_TEXT
			if (question.footer == "7" || question.footer == "8" || question.footer == "9")
			{
				if (ppptq_cms != null)
				{
					objViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(Purchase_Order_INFORMATION_TEXT, CurrentLanguage);
					objViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_Purchase_INFO, CurrentLanguage);
					objViewBag.Company = _translator.Translate(Supplier_Number, CurrentLanguage) + "#";
					objViewBag.PHYSYCAL_ADDRESS = _translator.Translate(Supplier_Name, CurrentLanguage);
					objViewBag.ADDRESS_ONE = _translator.Translate(Purchase_Order, CurrentLanguage) + "#";

					objViewBag.ADDRESS_TWO = _translator.Translate(Purchase_Order_Value, CurrentLanguage);
					objViewBag.CITY = _translator.Translate(Change_Amount, CurrentLanguage);
					objViewBag.STATE_TEXT = _translator.Translate(PO_Version, CurrentLanguage) + "#";
					objViewBag.POSTAL_CODE = _translator.Translate(Part_Number, CurrentLanguage);
					objViewBag.PROVINCE = _translator.Translate(Part_Number_Description, CurrentLanguage);
					objViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);


					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					try
					{
						int questionnaire = ptq != null ? ptq.questionnaire : 0;
						var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "EditCompanyInformation");
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;

						objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);
					}
					catch { }
				}

				partner.province = partner.internalID.Replace(partner.address1 + " ", "");
				objViewBag.IsPODS = true;
				ViewBag.FormData = objViewBag;
				return View("EditCompanyInformationPODS", partner);
			}


			if (ppptq_cms != null)
			{
				objViewBag.COMPANY_INFORMATION_TEXT = _translator.Translate(COMPANY_INFORMATION_TEXT, CurrentLanguage);
				objViewBag.VERIFY_COMPANY_INFO = _translator.Translate(VERIFY_COMPANY_INFO, CurrentLanguage);
				objViewBag.Company = _translator.Translate(Company, CurrentLanguage);
				objViewBag.PHYSYCAL_ADDRESS = _translator.Translate(PHYSYCAL_ADDRESS, CurrentLanguage);
				objViewBag.ADDRESS_ONE = _translator.Translate(ADDRESS_ONE, CurrentLanguage);
				objViewBag.ADDRESS_TWO = _translator.Translate(ADDRESS_TWO, CurrentLanguage);
				objViewBag.CITY = _translator.Translate(CITY, CurrentLanguage);
				objViewBag.STATE_TEXT = _translator.Translate(STATE_TEXT, CurrentLanguage);
				objViewBag.POSTAL_CODE = _translator.Translate(POSTAL_CODE, CurrentLanguage);
				objViewBag.PROVINCE = _translator.Translate(PROVINCE, CurrentLanguage);
				objViewBag.COUNTRY_TEXT = _translator.Translate(COUNTRY_TEXT, CurrentLanguage);
				objViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);
				var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
				var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
				var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
				try
				{
					int questionnaire = ptq != null ? ptq.questionnaire : 0;
					var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "EditCompanyInformation");
					objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
					objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
					objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
					objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
					objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
					objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;

					objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);

				}
				catch { }
			}
			ViewBag.FormData = objViewBag;
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
			ViewBagModel objViewBag = new ViewBagModel();
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			objViewBag.CMS_PAGE_TITLE = CMS.CONTACT_EDIT_PAGE_TITLE;
			objViewBag.CMS_PAGE_SUBTITLE = CMS.CONTACT_EDIT_PAGE_SUBTITLE;
			objViewBag.CMS_PAGE_PANEL_ONE = CMS.CONTACT_EDIT_PAGE_PANEL_ONE;
			objViewBag.CMS_PAGE_PANEL_TWO = CMS.CONTACT_EDIT_PAGE_PANEL_TWO;
			objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT;
			objViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONTACT_EDIT_PAGE_NEXT_TEXT;

			//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
			//objViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);



			var cmsId = 0;
			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			if (ppptq_cms != null)
			{

				_translator.PPTQ = ppptq_cms;

				objViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);

				//PODS //Purchase_Order_INFORMATION_TEXT
				if (question.footer == "7" || question.footer == "8" || question.footer == "9")
				{
					objViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(BUYER_INFORMATION_TEXT, CurrentLanguage);
					objViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_BUYER_TEXT_INFORMATION, CurrentLanguage);
					objViewBag.IsPODS = true;
				}
				else
				{
					objViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(CONTACT_INFORMATION_TEXT, CurrentLanguage);
					objViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_CONTACT_TEXT_INFORMATION, CurrentLanguage);

				}

				objViewBag.FIRST_NAME = _translator.Translate(FIRST_NAME, CurrentLanguage);
				objViewBag.LAST_NAME = _translator.Translate(LAST_NAME, CurrentLanguage);
				objViewBag.TITLE_TEXT = _translator.Translate(TITLE_TEXT, CurrentLanguage);
				objViewBag.EMAIL_TEXT = _translator.Translate(EMAIL_TEXT, CurrentLanguage);
				objViewBag.PHONE_TEXT = _translator.Translate(PHONE_TEXT, CurrentLanguage);
				objViewBag.FAX_TEXT = _translator.Translate(FAX_TEXT, CurrentLanguage);
				var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
				var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
				var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
				try
				{
					int questionnaire = ptq != null ? ptq.questionnaire : 0;
					var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "EditContactInformation");
					objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
					objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
					objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
					objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
					objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
					objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;


					objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);

				}
				catch { }
			}
			ViewBag.FormData = objViewBag;
			if (question.footer == "7" || question.footer == "8" || question.footer == "9")
				return View("EditContactInformationPODS", partner);
			else
				return View(partner);
		}

		[HttpPost]
		public ActionResult EditContactInformation(partner partner, bool? ispods)
		{
			if (Session["hs3Registration"] == null)
			{
				return RedirectToAction("Default");
			}
			ViewBagModel objViewBag = new ViewBagModel();
			partner objpartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
			if (objpartner != null)
			{
				objpartner.firstName = partner.firstName;
				objpartner.lastName = partner.lastName;
				objpartner.email = partner.email;
				if (ispods != true)
				{
					objpartner.title = partner.title;
					objpartner.phone = partner.phone;
					objpartner.fax = partner.fax;
				}
			}
			if (objpartner != null && Session["currentEmail"].ToString() == objpartner.email)
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
				Session["currentEmail"] = objpartner != null ? objpartner.email : "";
				partner.email = Session["currentEmail"].ToString();

				objViewBag.CMS_PAGE_TITLE = CMS.CONTACT_EDIT_PAGE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = CMS.CONTACT_EDIT_PAGE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = CMS.CONTACT_EDIT_PAGE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = CMS.CONTACT_EDIT_PAGE_PANEL_TWO;
				objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONTACT_EDIT_PAGE_PREVIOUS_TEXT;
				objViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONTACT_EDIT_PAGE_NEXT_TEXT;
				var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
				var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
				var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
				if (ppptq_cms != null)
				{
					_translator.PPTQ = ppptq_cms;
					objViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(CONTACT_INFORMATION_TEXT, CurrentLanguage);
					if (question.footer == "7" || question.footer == "8" || question.footer == "9")
					{
						objViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(BUYER_INFORMATION_TEXT, CurrentLanguage);
						objViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_BUYER_TEXT_INFORMATION, CurrentLanguage);
						objViewBag.IsPODS = true;
					}
					else
					{
						objViewBag.CONTACT_INFORMATION_TEXT = _translator.Translate(CONTACT_INFORMATION_TEXT, CurrentLanguage);
						objViewBag.VERIFY_CONTACT_TEXT_INFORMATION = _translator.Translate(VERIFY_CONTACT_TEXT_INFORMATION, CurrentLanguage);
					}


					objViewBag.FIRST_NAME = _translator.Translate(FIRST_NAME, CurrentLanguage);
					objViewBag.LAST_NAME = _translator.Translate(LAST_NAME, CurrentLanguage);
					objViewBag.TITLE_TEXT = _translator.Translate(TITLE_TEXT, CurrentLanguage);
					objViewBag.EMAIL_TEXT = _translator.Translate(EMAIL_TEXT, CurrentLanguage);
					objViewBag.PHONE_TEXT = _translator.Translate(PHONE_TEXT, CurrentLanguage);
					objViewBag.FAX_TEXT = _translator.Translate(FAX_TEXT, CurrentLanguage);
					int cmsId = 0;
					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
					var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
					try
					{

						int questionnaire = ptq != null ? ptq.questionnaire : 0;
						var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "EditContactInformation");
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;


						objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);
					}
					catch { }
				}

				ViewBag.FormData = objViewBag;

				if (question.footer == "7" || question.footer == "8" || question.footer == "9")
					return View("EditContactInformationPODS", partner);
				else
					return View(partner);
			}
			ViewBag.FormData = objViewBag;
			return View(partner);
		}

		public ActionResult ESignature()
		{
			if (Session["hs3Registration"] == null)
			{
				return RedirectToAction("Default");
			}
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			eSignature objeSignature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : 0).FirstOrDefault();

			var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

			ViewBagModel objViewBag = new ViewBagModel();

			//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_DOC_OTHER_2 = CMS.QUESTIONNAIRE_DOC_OTHER_2;
			//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
			objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT;
			objViewBag.CMS_PAGE_NEXT_TEXT = CMS.ESIGNATURE_PAGE_NEXT_TEXT;
			objViewBag.ESIGNATURE_PAGE_TEXT = CMS.ESIGNATURE_PAGE_TEXT;

			objViewBag.CMS_PAGE_TITLE = CMS.ESIGNATURE_PAGE_TITLE;
			objViewBag.CMS_PAGE_SUBTITLE = CMS.ESIGNATURE_PAGE_SUBTITLE;
			objViewBag.CMS_PAGE_PANEL_ONE = CMS.ESIGNATURE_PAGE_PANEL_ONE;
			objViewBag.CMS_PAGE_PANEL_TWO = CMS.ESIGNATURE_PAGE_PANEL_TWO;

			int cmsId = 0;
			if (ppptq_cms != null)
			{
				_translator.PPTQ = ppptq_cms;
				objViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);


				objViewBag.FIRST_NAME = _translator.Translate(FIRST_NAME, CurrentLanguage);
				objViewBag.LAST_NAME = _translator.Translate(LAST_NAME, CurrentLanguage);
				objViewBag.TITLE_TEXT = _translator.Translate(TITLE_TEXT, CurrentLanguage);
				objViewBag.EMAIL_TEXT = _translator.Translate(EMAIL_TEXT, CurrentLanguage);
				objViewBag.PHONE_TEXT = _translator.Translate(PHONE_TEXT, CurrentLanguage);
				var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
				var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
				var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
				try
				{
					if (questionnairCmsAll.Any())
					{

						int questionnaire = ptq != null ? ptq.questionnaire : 0;
						var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "ESignature", pptq);
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;
						if (model.ESIGNATURE_PAGE_TEXT != null)
							objViewBag.ESIGNATURE_PAGE_TEXT = model.ESIGNATURE_PAGE_TEXT;
						if (model.QUESTIONNAIRE_DOC_OTHER_2 != null)
							objViewBag.QUESTIONNAIRE_DOC_OTHER_2 = model.QUESTIONNAIRE_DOC_OTHER_2;

						objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);
					}
				}
				catch { }
			}

			ViewBag.FormData = objViewBag;
			return View(objeSignature);
		}

		[HttpPost]
		public ActionResult ESignature(eSignature objeSignatureNew)
		{
			if (Session["hs3Registration"] == null)
			{
				return RedirectToAction("Default");
			}
			int sessionTouchPoint = Session["touchpoint"] != null ? (int)Session["touchpoint"] : -1;
			string currentEmail = Session["currentEmail"].ToString();
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";

			partnerPartnertypeTouchpointQuestionnaire pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			if (ModelState.IsValid)
			{

				eSignature objeSignature =
					db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1)
						.FirstOrDefault();
				if (objeSignature == null)
				{
					db.pr_addEsignature(objeSignatureNew.firstName, objeSignatureNew.lastName, objeSignatureNew.title, objeSignatureNew.email, "Yes", objeSignatureNew.officer, objeSignatureNew.phone, DateTime.Now, pptq != null ? pptq.id : -1);
				}
				else
				{
					db.pr_modifyEsignature(objeSignatureNew.id, objeSignatureNew.firstName, objeSignatureNew.lastName, objeSignatureNew.title, objeSignatureNew.email, "Yes", objeSignatureNew.officer, objeSignatureNew.phone, DateTime.Now, pptq != null ? pptq.id : -1);
				}
				// Validate the zCode.
				ValidatezCode _objInvalidzcode = new ValidatezCode();

				TempData["IncorrectZipCode"] = _objInvalidzcode.ValidatezCodeFn(pptq, sessionTouchPoint, currentEmail, accessCode);

				return RedirectToAction("Finish");
			}

			ViewBagModel objViewBag = new ViewBagModel();

			//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_DOC_OTHER_2 = CMS.QUESTIONNAIRE_DOC_OTHER_2;
			//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
			//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
			objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.ESIGNATURE_PAGE_PREVIOUS_TEXT;
			objViewBag.CMS_PAGE_NEXT_TEXT = CMS.ESIGNATURE_PAGE_NEXT_TEXT;
			objViewBag.ESIGNATURE_PAGE_TEXT = CMS.ESIGNATURE_PAGE_TEXT;

			objViewBag.CMS_PAGE_TITLE = CMS.ESIGNATURE_PAGE_TITLE;
			objViewBag.CMS_PAGE_SUBTITLE = CMS.ESIGNATURE_PAGE_SUBTITLE;
			objViewBag.CMS_PAGE_PANEL_ONE = CMS.ESIGNATURE_PAGE_PANEL_ONE;
			objViewBag.CMS_PAGE_PANEL_TWO = CMS.ESIGNATURE_PAGE_PANEL_TWO;
			var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			int cmsId = 0;
			if (ppptq_cms != null)
			{
				_translator.PPTQ = ppptq_cms;
				objViewBag.REQUIRED_FIELDS = _translator.Translate(REQUIRED_FIELDS, CurrentLanguage);


				objViewBag.FIRST_NAME = _translator.Translate(FIRST_NAME, CurrentLanguage);
				objViewBag.LAST_NAME = _translator.Translate(LAST_NAME, CurrentLanguage);
				objViewBag.TITLE_TEXT = _translator.Translate(TITLE_TEXT, CurrentLanguage);
				objViewBag.EMAIL_TEXT = _translator.Translate(EMAIL_TEXT, CurrentLanguage);
				objViewBag.PHONE_TEXT = _translator.Translate(PHONE_TEXT, CurrentLanguage);
				var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
				var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq != null ? ptq.questionnaire : 0).ToList();
				var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
				try
				{
					if (questionnairCmsAll.Any())
					{

						int questionnaire = ptq != null ? ptq.questionnaire : 0;
						var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "ESignature", pptq);
						objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
						objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
						objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
						objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
						objViewBag.CMS_PAGE_PREVIOUS_TEXT = model.CMS_PAGE_PREVIOUS_TEXT;
						objViewBag.CMS_PAGE_NEXT_TEXT = model.CMS_PAGE_NEXT_TEXT;
						objViewBag.ESIGNATURE_PAGE_TEXT = model.ESIGNATURE_PAGE_TEXT;
						objViewBag.QUESTIONNAIRE_DOC_OTHER_2 = model.QUESTIONNAIRE_DOC_OTHER_2;

						objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, questionnaire, objViewBag);
					}
				}
				catch { }

			}
			ViewBag.FormData = objViewBag;
			return View(objeSignatureNew);

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
					db.pr_addQuestionnaireQuestionnaireCMS((int)questionnarie, cms.questionnaireCMS, cms.text, cms.link, cms.doc, cms.uploadedFileType);

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

		public ActionResult SendInventation()
		{
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			if (ppptq_cms != null)
			{
				var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
				var pr = db.pr_getPTQreferencePTQ(ptq.id).FirstOrDefault();
				var currentPtq = db.pr_getPartnertypeTouchpointQuestionnaire(pr.ptqReference).FirstOrDefault();
				foreach (var newEmail in ReferenceEmails)
				{
					
					//creates new partner
					var country = db.country.FirstOrDefault().id.ToString();
					var state = db.state.FirstOrDefault().id.ToString();
					
					var group = db.pr_getGroupByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault().id;
					var partnerId = (int)db.pr_addPartnerSpreadsheetDataLoad(newEmail, "", newEmail, newEmail, newEmail, newEmail, newEmail, state, "", country, newEmail, newEmail, "", newEmail, newEmail, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, currentPtq.partnerType, currentPtq.touchpoint, ppptq_cms.person.id, (int)PartnerStatus.Invited_NoResponse, "", DateTime.Now.AddDays(5), group).FirstOrDefault();

					var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartnertypeTouchpointQuestionnaire(pr.ptqReference).ToList().FirstOrDefault(o => o.partner1.email == newEmail);
					db.pr_modifyPartnerInternalIDtoAccessCode(pptq.partner1.id, pptq.accesscode).FirstOrDefault();
					partner objPartner = pptq.partner1;					
					var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, pr.ptqReference).FirstOrDefault();
					if (amm != null)
					{

						var objtouchpoint = currentPtq.touchpoint1;
						Email email = new Email(amm);

						if (Session["loadgroup"] != null)
						{
							email.loadgroup = Session["loadgroup"].ToString();
						}
						email.accesscode = pptq.accesscode;
						email.protocolTouchpoint = objtouchpoint.description;

						EmailFormat emailFormat = new EmailFormat();
						email.subject = emailFormat.sGetEmailBody(email.subject, ppptq_cms.person, objPartner, currentPtq.partnerType1.enterprise1, objtouchpoint, pr.ptqReference);
						email.body = emailFormat.sGetEmailBody(email.body, ppptq_cms.person, objPartner, currentPtq.partnerType1.enterprise1, objtouchpoint, pr.ptqReference);
						email.emailTo = newEmail;
						SendEmail objSendEmail = new SendEmail();
						objSendEmail.sendEmail(email);
					}
					
				}
				ReferenceEmails.Clear();
			}
			return Json(true, JsonRequestBehavior.AllowGet);
		}


		public ActionResult Finish()
		{
			int _mailType = 3;
			if (Session["hs3Registration"] == null)
			{
				return RedirectToAction("Default");
			}
			ViewBagModel objViewBag = new ViewBagModel();
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

			if (ppptq_cms != null)
			{
				_translator.PPTQ = ppptq_cms;
				var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
				var leftSites = db.pr_getPartnumberSiteZcodePPTQByPPTQ_ToDo_ByPPTQ(ppptq_cms.id);
				var isCompletedSurvey = !(leftSites != null && leftSites.ToList().Count() > 0);
				objViewBag.isCompletedSurvey = !isCompletedSurvey;


				var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
				partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
				enterprise _enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

				var result1 = db.pr_checkForInvalidZcode(ppptq_cms.id, ppptq_cms.zcode);

				var zCodeActionType = result1.FirstOrDefault();

				//if (isCompletedSurvey && TempData["IncorrectZipCode"] == null)
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

							db.pr_addEnterpriseSystemInfo(DateTime.Now.AddYears(1), 20, responses.GetTextResponse<string>(companyName), string.Empty, responses.GetTextResponse<string>(website), string.Empty, 1, string.Empty, false, (int)result.Value, 0, 0, true).FirstOrDefault();
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
								context.pr_modifyEnterpriseSystemInfo(sysinfo.id, sysinfo.systemExpiry, sysinfo.licenseLimit, sysinfo.companyName, responses.GetTextResponse<string>(firstname) + " " + responses.GetTextResponse<string>(lasname), sysinfo.companyWebSite, responses.GetTextResponse<string>(email), sysinfo.isCurrentDataBase, sysinfo.logoImage, sysinfo.configured, sysinfo.enterprise, sysinfo.credit, sysinfo.sortOrder, sysinfo.active);
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


						if (zCodeActionType.newStatus == 2)
							_mailType = autoMailTypes.Incomplete;
						else
							_mailType = autoMailTypes.Complete_Confirmation;

						//  var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Complete_Confirmation, ptq.id).FirstOrDefault();
						var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(_mailType, ptq.id).FirstOrDefault();

						if (amm != null)
						{
							var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
							Email email = new Email(amm);
							EmailFormat emailFormat = new EmailFormat();
							email.subject = emailFormat.sGetEmailBody(email.subject, person, objPartner, _enterprise, objtouchpoint, ptq.id);
							email.body = emailFormat.sGetEmailBody(email.body, person, objPartner, _enterprise, objtouchpoint, ptq.id);
							email.emailTo = objPartner.email;
							SendEmail objSendEmail = new SendEmail();
							try
							{
								objSendEmail.sendEmail(email);
							}
							catch (FormatException ex)
							{
								ViewData["ErrorMessage"] = "Invalide Email Address - " + objPartner.email + ". Please correct email address and Retry";
								//RedirectToAction("eSignature");
							}
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
				
				objViewBag.CMS_PAGE_TITLE = CMS.CONFIRMATION_PAGE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = CMS.CONFIRMATION_PAGE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = CMS.CONFIRMATION_PAGE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = CMS.CONFIRMATION_PAGE_PANEL_TWO;
				objViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT;
				objViewBag.CMS_PAGE_NEXT_TEXT = CMS.CONFIRMATION_PAGE_NEXT_TEXT;

				objViewBag.CONFIRMATION_PAGE_SIGNOFF_STATEMENT = CMS.CONFIRMATION_PAGE_SIGNOFF_STATEMENT;
				ViewBag.CONFIRMATION_PAGE_EXIT_LINK = "http://www.intelleges.com";
				ViewBag.CONFIRMATION_PAGE_HEADLINE = CMS.CONFIRMATION_PAGE_HEADLINE;

				objViewBag.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT = CMS.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT;
				ViewBag.WARNING = CMS.WARNING;


				//objViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
				//objViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);

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
				var questionnairCmsAll = db.pr_getQuestionnaireCMSAll().ToList();
				objViewBag = QuestionnaireMenuLinks(cms, questionnairCmsAll, ptq.questionnaire, objViewBag);
				try
				{
					ViewBag.ACCESS_CODE = Session["accessCode"];
				}
				catch { }
				int cmsId = 0;
				var model = GetPageTitles(cmsId, questionnairCmsAll, ptq, "Finish", null, cms);
				objViewBag.CMS_PAGE_TITLE = model.CMS_PAGE_TITLE;
				objViewBag.CMS_PAGE_SUBTITLE = model.CMS_PAGE_SUBTITLE;
				objViewBag.CMS_PAGE_PANEL_ONE = model.CMS_PAGE_PANEL_ONE;
				objViewBag.CMS_PAGE_PANEL_TWO = model.CMS_PAGE_PANEL_TWO;
				ViewBag.CMS_PAGE_PREVIOUS_LINK = !string.IsNullOrEmpty(model.CMS_PAGE_PREVIOUS_LINK) ? model.CMS_PAGE_PREVIOUS_LINK : "";
				ViewBag.CONFIRMATION_PAGE_HEADLINE = !string.IsNullOrEmpty(model.CONFIRMATION_PAGE_HEADLINE) ? model.CONFIRMATION_PAGE_HEADLINE : "";
				ViewBag.WARNING = !string.IsNullOrEmpty(model.WARNING) ? model.WARNING : "";
				objViewBag.CMS_PAGE_NEXT_TEXT = !string.IsNullOrEmpty(model.CMS_PAGE_NEXT_TEXT) ? model.CMS_PAGE_NEXT_TEXT : objViewBag.CMS_PAGE_NEXT_TEXT;
				objViewBag.QUESTIONNAIRE_DOC_OTHER_2 = !string.IsNullOrEmpty(model.QUESTIONNAIRE_DOC_OTHER_2) ? model.QUESTIONNAIRE_DOC_OTHER_2 : "";
				objViewBag.CMS_PAGE_NEXT_LINK = !string.IsNullOrEmpty(model.CMS_PAGE_NEXT_LINK) ? model.CMS_PAGE_NEXT_LINK : "";

				objViewBag.CMS_PAGE_PREVIOUS_TEXT = !string.IsNullOrEmpty(model.CMS_PAGE_PREVIOUS_TEXT) ? model.CMS_PAGE_PREVIOUS_TEXT : objViewBag.CMS_PAGE_PREVIOUS_TEXT;

				objViewBag.CONFIRMATION_PAGE_SIGNOFF_STATEMENT = !string.IsNullOrEmpty(model.CONFIRMATION_PAGE_SIGNOFF_STATEMENT) ? model.CONFIRMATION_PAGE_SIGNOFF_STATEMENT : objViewBag.CONFIRMATION_PAGE_SIGNOFF_STATEMENT;

				objViewBag.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT = !string.IsNullOrEmpty(model.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT) ? model.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT : objViewBag.CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT;

			}
			ViewBag.FormData = objViewBag;
			return View();
		}

		private ViewBagModel QuestionnaireMenuLinks(List<questionnaireQuestionnaireCMS> cms, List<pr_getQuestionnaireCMSAll_Result> questionnairCmsAll, int ptqId, ViewBagModel objViewBagModel)
		{
			try
			{
				var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
				var ppptqCms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
				_translator.PPTQ = ppptqCms;
				int cmdId = 0;
				var cmd = new pr_getQuestionnaireCMSAll_Result();
				////QUESTIONNAIRE_PDF
				cmd = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PDF);
				cmdId = cmd != null ? cmd.id : 0;
				var questionnairePdf = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
				if (!string.IsNullOrEmpty(questionnairePdf))
					objViewBagModel.QUESTIONNAIRE_PDF = questionnairePdf;
				////QUESTIONNAIRE_FAQ
				cmd = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_FAQ);
				cmdId = cmd != null ? cmd.id : 0;
				var questionnaireFaq = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
				if (!string.IsNullOrEmpty(questionnaireFaq))
					objViewBagModel.QUESTIONNAIRE_FAQ = questionnaireFaq;
				////QUESTIONNAIRE_DOC_OTHER
				cmd = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER);
				cmdId = cmd != null ? cmd.id : 0;
				var questionnaireDocOther = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
				if (!string.IsNullOrEmpty(questionnaireDocOther))
					objViewBagModel.QUESTIONNAIRE_DOC_OTHER = questionnaireDocOther;
				////Questionnaire_doc_other2
				cmd = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER_2);
				cmdId = cmd != null ? cmd.id : 0;
				var doc2 = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
				if (doc2 != null)
					objViewBagModel.QUESTIONNAIRE_DOC_OTHER_2 = doc2;

				////QUESTIONNAIRE_CONTACT_US_EMAIL_LINK
				cmd = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL);
				cmdId = cmd != null ? cmd.id : 0;
				var cmsContact = cms.FirstOrDefault(x => x.questionnaireCMS == cmdId);
				var contactLink = cmsContact != null ? cmsContact.link : "";
				var contactEmailLink = contactLink != null ? replaceBlank(contactLink) : "";
				if (!string.IsNullOrEmpty(contactEmailLink))
				{
					objViewBagModel.QUESTIONNAIRE_CONTACT_US_EMAIL_LINK = contactEmailLink;
					var contactUsEmail = replaceBlank(_translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId));
					if (!string.IsNullOrEmpty(contactUsEmail))
						objViewBagModel.CONTACT_US_EMAIL = contactUsEmail;
					//else
					//    objViewBagModel.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);
				}
				////QUESTIONNAIRE_VIDEO_LINK
				cmd = questionnairCmsAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_VIDEO);
				cmdId = cmd != null ? cmd.id : 0;
				var video = cms.FirstOrDefault(x => x.questionnaireCMS == cmdId);
				var videoLink = video != null ? video.link : "";
				if (!string.IsNullOrEmpty(videoLink))
				{
					objViewBagModel.QUESTIONNAIRE_VIDEO_LINK = videoLink;
					var questionnaireVideo = _translator.Translate(ptqId, TranslationType.CMS, CurrentLanguage, cmdId);
					if (!string.IsNullOrEmpty(questionnaireVideo))
						objViewBagModel.QUESTIONNAIRE_VIDEO = questionnaireVideo;
					//else
					//    objViewBagModel.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
				}
			}
			catch
			{

			}
			return objViewBagModel;

		}
		public FileContentResult FileDownloadCMS(string CMSName)
		{
			try
			{
				//declare byte array to get file content from database and string to store file name
				byte[] fileData;
				byte[] fileDataBinary = null;
				string fileName = "application/pdf";


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
				string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
				var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

				if (ppptq_cms != null)
				{
					var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
					var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
					var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();

					if (CMSName == CMS.QUESTIONNAIRE_PDF)
					{
						fileDataBinary = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PDF).FirstOrDefault().id).FirstOrDefault().doc;
						fileName = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PDF).FirstOrDefault().id).FirstOrDefault().uploadedFileType;
					}
					else if (CMSName == CMS.QUESTIONNAIRE_FAQ)
					{
						fileDataBinary = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_FAQ).FirstOrDefault().id).FirstOrDefault().doc;
						fileName = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_FAQ).FirstOrDefault().id).FirstOrDefault().uploadedFileType;
					}
					else if (CMSName == CMS.QUESTIONNAIRE_DOC_OTHER)
					{
						//QUESTIONNAIRE_DOC_OTHER
						fileDataBinary = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).FirstOrDefault().id).FirstOrDefault().doc;
						fileName = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).FirstOrDefault().id).FirstOrDefault().uploadedFileType;


					}
					else
					{
						fileDataBinary = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER_2).FirstOrDefault().id).FirstOrDefault().doc;
						fileName = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER_2).FirstOrDefault().id).FirstOrDefault().uploadedFileType;
					}
				}

				fileData = fileDataBinary.ToArray();
				// fileName = CMSName;

				//return file and provide byte file content and file name --application/pdf
				return File(fileData, fileName ?? "file.pdf");
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
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			List<pr_getPartnerQuestionResponseByAccessCode_Result> result =
				db.pr_getPartnerQuestionResponseByAccessCode(accessCode).ToList();

			var find = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.reslt2 = find;
			var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq =
				db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode)
					.FirstOrDefault();
			if (pptq != null)
			{
				var _partnerId = pptq.partner;
				var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
				ViewBag.partner = _partner;

				if (_partner != null)
				{
					var _country = db.pr_getCountry(_partner.country).FirstOrDefault();
					ViewBag.country = _country != null ? _country.name : string.Empty;
				}

				if (_partner != null)
				{
					var _state = db.pr_getState(_partner.state).FirstOrDefault();
					ViewBag.state = _state != null ? _state.stateCode : string.Empty;
				}
			}


			var firstOrDefault = enterprise.FirstOrDefault();
			if (firstOrDefault != null)
			{
				var logo = firstOrDefault.logo;
				string dirname = @"C:\https\MVCMT\logo\"; //@"C:\https\MVCMT\Generic\uploadedFiles\EnterpriseLogo\";
				if (Directory.Exists(dirname))
				{
					var fileName = dirname + firstOrDefault.id + "Logo.png";
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
			if (pptq != null) return ViewPdf(result, pptq.id);
			else throw new Exception("Cannot find pptq");
		}

		public ActionResult OrdersInHTML()
		{
			QuestionnaireModel modl = new QuestionnaireModel();
			var accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			List<pr_getPartnerQuestionResponseByAccessCode_Result> result = db.pr_getPartnerQuestionResponseByAccessCode(accessCode).ToList();

			var find = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.reslt2 = find;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var partnerType = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var _partnerId = partnerType != null ? partnerType.partner : -1;
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
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var _partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();
			var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
			ViewBag.partner = _partner;

			//_signature
			ViewBag.signature = _signature;
			ViewBag.personTitle = _partner != null ? _partner.title : "";
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

			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
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
							ViewBag.Checkbox61 = _chacked;
						}
						else if (item.rid == _responseNO)
						{
							ViewBag.Checkbox62 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split(item.response, _responseSplitter);
							if (comments.Length > 1)
								ViewBag.Input14 = comments[1];
						}
						break;
					case 5885:
						if (item.rid == _responseYES)
						{
							ViewBag.Checkbox61 = _chacked;
						}
						else if (item.rid == _responseNO)
						{
							ViewBag.Checkbox62 = _chacked;
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
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			ViewBag.partner = _partner;

			//_signature
			ViewBag.signature = _signature;
			ViewBag.personTitle = _partner != null ? _partner.title : "";
			if (pptq != null)
				ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
			var _country = db.pr_getCountry(_partner != null ? _partner.country : -1).FirstOrDefault();
			if (_country != null)
				ViewBag.country = _country.name;
			else
				ViewBag.country = string.Empty;

			var _state = db.pr_getState(_partner != null ? _partner.state : -1).FirstOrDefault();
			if (_state != null)
				ViewBag.state = _state.stateCode;
			else
				ViewBag.state = string.Empty;
			if (question.footer == "4")
			{
				ViewBag.logoSrc = "https://www.intelleges.com/mvcmt/Generic/Contents/images/MOOG_Logo.png";
			}
			else
				if (enterprise != null && enterprise.Any())
				{
					var enterpriseLogo = enterprise.FirstOrDefault();
					byte[] logoBytes = new byte[0];
					var logo = enterpriseLogo != null ? enterpriseLogo.logo : logoBytes;//https://www.intelleges.com/mvcmt/Generic/uploadedFiles/EnterpriseLogo/
					string dirname = "~/uploadedFiles/EnterpriseLogo/";

					if (Directory.Exists(Server.MapPath(dirname)))
					{
						var fileName = enterpriseLogo != null ? enterpriseLogo.id + "Logo.png" : "Logo.png";
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

			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;

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
							// ViewBag.Checkbox77 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox78 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//if (comments.Length =1)
							//    ViewBag.Input0 = comments[1];

							//  ViewBag.Input39 = (comments.Length > 1 ? comments[1] : comments[0]);
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
							// if (comments.Length > 1)
							ViewBag.Input38 = (comments.Length > 1 ? comments[1] : comments[0]);// comments[1];
						}
						break;


					case 18981:
						if (item.response == _responseYES)
						{
							//  ViewBag.Checkbox60 = _chacked;

							// ViewBag.Checkbox59 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox60 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							// if (comments.Length > 1)
							ViewBag.Input34 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							// if (comments.Length > 1)
							ViewBag.Input35 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
								// if (comments.Length > 1)
								ViewBag.Input36 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
								//if (comments.Length > 1)
								ViewBag.Input37 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							//ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;
					case 18990:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input5 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];


						break;

					case 18991:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						if (item.response == _responseYES)
						{
							//  ViewBag.Checkbox60 = _chacked;
							//if (comments.Length > 1)
							ViewBag.Input6 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];

						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							// if (comments.Length > 1)
							ViewBag.Input6 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 19003:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
							ViewBag.Checkbox53 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							ViewBag.Checkbox54 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//   if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						switch (item.response)
						{
							//case 37537:
							// ViewBag.Checkbox54 = _chacked;
							//  ViewBag.Checkbox53 = _chacked;
							//    ViewBag.Case19003 = 37537;
							//      break;
							case 37538:
								//ViewBag.Checkbox54 = _chacked;
								ViewBag.Checkbox53 = _chacked;
								break;
						}
						break;


					case 19004:
						if (item.response == _responseYES)
						{
							//ViewBag.Checkbox60 = _chacked;
							// ViewBag.Checkbox53 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							ViewBag.Checkbox54 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							///if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						switch (item.response)
						{
							case 37539:
								ViewBag.Checkbox54 = _chacked;
								// ViewBag.Checkbox53 = string.Empty;
								break;
							/* case 37577:
							 //    ViewBag.Checkbox53 = _chacked;
							  //   break;
							 case 37578:
								 ViewBag.Checkbox54 = _chacked;
								 break;*/
						}
						break;
					case 19005:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input15 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19006:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input16 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19007:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input17 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;

					case 19008:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 19009:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input18 = (comments.Length > 1 ? comments[1] : comments[0]); // comments[0];
						break;
					case 19010:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input19 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19011:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input20 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19012:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 19013:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input21 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19014:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input22 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19015:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input23 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19016:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 19017:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input24 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19018:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input25 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19019:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input26 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19020:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 19021:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input27 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19022:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input28 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19023:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input29 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 19024:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
							ViewBag.Checkbox55 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//  ViewBag.Checkbox61 = _chacked;
							// ViewBag.Checkbox56 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 19025:
						if (item.response == _responseYES)
						{
							//ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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

						switch (item.response)
						{
							case 37541:
								ViewBag.Checkbox11 = _chacked;
								break;

							case 37542:
								ViewBag.Checkbox11 = _chacked;
								ViewBag.Checkbox11_comment = _chacked;
								break;
							case 37543:
								ViewBag.Checkbox11 = string.Empty;
								ViewBag.Checkbox11_comment = string.Empty;
								break;

						}

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
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 18993:
						ViewBag.Checkbox15 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 18994:
						ViewBag.Checkbox16 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 18995:
						ViewBag.Checkbox17 = item.response == _responseYES ? _chacked : string.Empty;
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
							// if (comments.Length > 1)
							ViewBag.Input1 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
								// if (comments.Length >= 1)
								ViewBag.Input2 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
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
							//if (comments.Length >= 1)
							ViewBag.Input4 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						}
						break;

					#endregion

					#region 3 Question
					case 18989:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						if (item.response == _responseYES)
						{
							//  if (comments.Length >= 1)
							ViewBag.Input5 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						}
						else if (item.response == _responseNO)
						{
							// if (comments.Length >= 1)
							ViewBag.Input6 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
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
								//   if (comments.Length > 1)
								ViewBag.Input7 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
								// if (comments.Length > 1)
								ViewBag.Input8 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							// if (comments.Length > 1)
							ViewBag.Input9 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							//  if (comments.Length > 1)
							ViewBag.Input10 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							// if (comments.Length > 1)
							ViewBag.Input11 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					#endregion

					#region 13 Question
					case 19046:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox47 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input12 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							//   if (comments.Length > 1)
							ViewBag.Input13 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
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
							//   if (comments.Length > 1)
							ViewBag.Input14 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox52 = _chacked;
						}
						break;

					#endregion

					#region 16 Question
					//When 19049 =75 then 19050 is skipped and should have no checkmarks at all

					case 19049:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox57 = _chacked;
							// ViewBag.Checkbox58 = string.Empty;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox58 = _chacked;
							ViewBag.case19049 = 75;
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
							ViewBag.Checkbox77 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox78 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

							ViewBag.Input39 = (comments.Length > 1 ? comments[1] : comments[0]);


						}
						break;

					#endregion
				}
			}
			return pptqID;
		}


		public static int FillCustomPdfHtml10(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
		{
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			ViewBag.partner = _partner;

			//_signature
			ViewBag.signature = _signature;
			ViewBag.personTitle = _partner != null ? _partner.title : "";
			if (pptq != null)
				ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
			var _country = db.pr_getCountry(_partner != null ? _partner.country : -1).FirstOrDefault();
			if (_country != null)
				ViewBag.country = _country.name;
			else
				ViewBag.country = string.Empty;

			var _state = db.pr_getState(_partner != null ? _partner.state : -1).FirstOrDefault();
			if (_state != null)
				ViewBag.state = _state.stateCode;
			else
				ViewBag.state = string.Empty;
			if (question.footer == "4")
			{
				ViewBag.logoSrc = "https://www.intelleges.com/mvcmt/Generic/Contents/images/MOOG_Logo.png";
			}
			else
				if (enterprise != null && enterprise.Any())
				{
					var enterpriseLogo = enterprise.FirstOrDefault();
					byte[] logoBytes = new byte[0];
					var logo = enterpriseLogo != null ? enterpriseLogo.logo : logoBytes;//https://www.intelleges.com/mvcmt/Generic/uploadedFiles/EnterpriseLogo/
					string dirname = "~/uploadedFiles/EnterpriseLogo/";

					if (Directory.Exists(Server.MapPath(dirname)))
					{
						var fileName = enterpriseLogo != null ? enterpriseLogo.id + "Logo.png" : "Logo.png";
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

			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;

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
					case 26520:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox71 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox72 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input38 = (comments.Length > 1 ? comments[1] : comments[0]);// comments[1];
						}
						break;


					case 26525:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox61 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox62 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input34 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 26526:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox63 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox64 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input35 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 26527:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox65 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox66 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							ViewBag.Input36 = (comments.Length > 1 ? comments[1] : comments[0]);
						}

						break;


					case 26528:
						switch (item.response)
						{
							case 48707:
								ViewBag.Checkbox67 = _chacked;
								break;
							case 48708:
								ViewBag.Checkbox68 = _chacked;
								comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
								//if (comments.Length > 1)
								ViewBag.Input37 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
								break;
							case 48709:
								ViewBag.Checkbox76 = _chacked;
								break;
						}
						break;

					case 26529:

						switch (item.response)
						{
							case 48710:
								ViewBag.Checkbox69 = _chacked;
								break;
							case 48711:
								ViewBag.Checkbox70 = _chacked;
								break;
						}
						break;


					case 26531:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input5 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];


						break;

					case 26532:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						if (item.response == _responseYES)
						{
							//  ViewBag.Checkbox60 = _chacked;
							//if (comments.Length > 1)
							ViewBag.Input6 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];

						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							// if (comments.Length > 1)
							ViewBag.Input6 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 26542:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox53 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							ViewBag.Checkbox54 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//   if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						switch (item.response)
						{
							case 48722:
								ViewBag.Checkbox53 = _chacked;
								break;
						}
						break;


					case 26543:
						if (item.response == _responseYES)
						{
							//ViewBag.Checkbox60 = _chacked;
							// ViewBag.Checkbox53 = _chacked;
						}
						else if (item.response == _responseNO)
						{

							ViewBag.Checkbox54 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							///if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						switch (item.response)
						{
							case 48723:
								ViewBag.Checkbox54 = _chacked;
								break;
						}
						break;
					case 26544:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input15 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26545:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						ViewBag.Input16 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26546:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						ViewBag.Input17 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;

					case 26547:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26548:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input18 = (comments.Length > 1 ? comments[1] : comments[0]); // comments[0];
						break;
					case 26549:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input19 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26550:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input20 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26551:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26552:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input21 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26553:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input22 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26554:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input23 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26555:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26556:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input24 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26557:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input25 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26558:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input26 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26559:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26560:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input27 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26561:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input28 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26562:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input29 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26563:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
							ViewBag.Checkbox55 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//  ViewBag.Checkbox61 = _chacked;
							// ViewBag.Checkbox56 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 26564:
						if (item.response == _responseYES)
						{
							//ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;
					case 26565:
						ViewBag.Input30 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;
					case 26566:
						ViewBag.Input31 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;
					case 26567:
						ViewBag.Input32 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;
					case 26568:
						ViewBag.Input33 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;

					#endregion
					#region 1 Question
					case 26577:
						ViewBag.CheckboxSmall = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 26569:
						ViewBag.CheckboxLarge = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26582:
						if (ViewBag.CheckboxLarge == _chacked)
						{
							ViewBag.Checkbox1 = item.response == _responseYES ? _chacked : string.Empty;
						}
						else if (ViewBag.CheckboxSmall == _chacked)
						{
							ViewBag.Checkbox3 = item.response == _responseYES ? _chacked : string.Empty;
						}
						break;

					case 26584:
						if (ViewBag.CheckboxLarge == _chacked)
						{
							ViewBag.Checkbox2 = item.response == _responseYES ? _chacked : string.Empty;
						}
						else if (ViewBag.CheckboxSmall == _chacked)
						{
							ViewBag.Checkbox4 = item.response == _responseYES ? _chacked : string.Empty;
						}
						break;
					case 26579:
						ViewBag.Checkbox5 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26580:
						ViewBag.Checkbox6 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26581:
						ViewBag.Checkbox7 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26572:
						ViewBag.Checkbox8 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26573:
						ViewBag.Checkbox9 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26574:
						ViewBag.Checkbox10 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26575:
						ViewBag.Checkbox11 = item.response == _responseYES ? _chacked : string.Empty;
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						if (comments.Length > 1 && comments[1].Contains("Yes"))
							ViewBag.Checkbox11_comment = _responseYES;

						switch (item.response)
						{
							case 48725:
								ViewBag.Checkbox11 = _chacked;
								break;
							case 48726:
								ViewBag.Checkbox11 = _chacked;
								ViewBag.Checkbox11_comment = _chacked;
								break;
							default:
								ViewBag.Checkbox11 = string.Empty;
								ViewBag.Checkbox11_comment = string.Empty;
								break;

						}
						break;

					case 26571:
						ViewBag.Checkbox12 = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 26570:
						ViewBag.Checkbox13 = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 26576:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox14 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;
					case 26534:
						ViewBag.Checkbox15 = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 26535:
						ViewBag.Checkbox16 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26536:
						ViewBag.Checkbox17 = item.response == _responseYES ? _chacked : string.Empty;
						break;



					case 26578:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox18 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox19 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input1 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26583:
						switch (item.response)
						{
							case 48728:
								ViewBag.Checkbox20 = _chacked;
								break;
							case 48729:
								ViewBag.Checkbox21 = _chacked;
								break;
							case 48730:
								ViewBag.Checkbox22 = _chacked;
								break;
							case 48731:
								ViewBag.Checkbox23 = _chacked;
								break;
							case 48732:
								ViewBag.Checkbox24 = _chacked;
								break;
							case 48733:
								ViewBag.Checkbox25 = _chacked;
								comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
								// if (comments.Length >= 1)
								ViewBag.Input2 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
								break;
						}
						break;

					#endregion

					#region 2 Question
					case 26519:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox26 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox27 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//if (comments.Length >= 1)
							ViewBag.Input4 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						}
						break;

					#endregion

					#region 4 Question
					case 26537:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox28 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox29 = _chacked;
						}
						break;
					case 26538:
						switch (item.response)
						{
							case 48712:
								ViewBag.Checkbox28_1 = _chacked;
								break;
							case 48713:
								ViewBag.Checkbox29_1 = _chacked;
								ViewBag.Input7_1 = item.comment;
								break;
							case 48714:
								ViewBag.Checkbox29_2 = _chacked;
								break;
						}
						break;

					#endregion

					#region 5 Question

					case 26539:
						switch (item.response)
						{
							case 48715:
								ViewBag.Checkbox30_1 = _chacked;
								break;
							case 48716:
								ViewBag.Checkbox31_1 = _chacked;
								break;
							case 48717:
								ViewBag.Checkbox32_1 = _chacked;
								break;

						}
						break;

					#endregion

					#region 6 Question

					case 26540:
						switch (item.response)
						{
							case 48718:
								ViewBag.Checkbox30 = _chacked;
								break;
							case 48719:
								ViewBag.Checkbox31 = _chacked;
								comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
								ViewBag.Input7 = (comments.Length > 1 ? comments[1] : comments[0]);
								break;
							case 48720:
								ViewBag.Checkbox73 = _chacked;
								break;
						}
						break;
					#endregion

					#region 7 Question
					case 26522:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox35 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox36 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input8 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					#endregion

					#region 8 Question
					case 26523:
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

					#region 9 Question
					case 26524:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox43 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox44 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input10 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					#endregion

					#region 10 Question
					case 26541:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox45 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox46 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input11 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					#endregion

					#region 11 Question
					case 26585:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox47 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input12 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox48 = _chacked;

						}
						break;

					case 26586:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox49 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//   if (comments.Length > 1)
							ViewBag.Input13 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox50 = _chacked;

						}
						break;

					case 26587:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox51 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//   if (comments.Length > 1)
							ViewBag.Input14 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox52 = _chacked;
						}
						break;

					#endregion

					#region 14 Question
					//When 19049 =75 then 19050 is skipped and should have no checkmarks at all

					case 26588:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox57 = _chacked;
							// ViewBag.Checkbox58 = string.Empty;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox58 = _chacked;
							ViewBag.case19049 = 75;
						}
						break;

					case 26589:
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

					#region ADDITIONAL SUPPLIER COMMENTS
					case 26590:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						ViewBag.Input391 = (comments.Length > 1 ? comments[1] : comments[0]);
						break;

					#endregion


					#region CERTIFICATION
					case 26592:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox77 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox78 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

							ViewBag.Input39 = (comments.Length > 1 ? comments[1] : comments[0]);


						}
						break;

					#endregion
				}
			}
			return pptqID;
		}


		public static int FillCustomPdfHtml11(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
		{
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			ViewBag.partner = _partner;

			//_signature
			ViewBag.signature = _signature;
			ViewBag.personTitle = _partner != null ? _partner.title : "";
			if (pptq != null)
				ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
			var _country = db.pr_getCountry(_partner != null ? _partner.country : -1).FirstOrDefault();
			if (_country != null)
				ViewBag.country = _country.name;
			else
				ViewBag.country = string.Empty;

			var _state = db.pr_getState(_partner != null ? _partner.state : -1).FirstOrDefault();
			if (_state != null)
				ViewBag.state = _state.stateCode;
			else
				ViewBag.state = string.Empty;
			if (question.footer == "4")
			{
				ViewBag.logoSrc = "https://www.intelleges.com/mvcmt/Generic/Contents/images/MOOG_Logo.png";
			}
			else
				if (enterprise != null && enterprise.Any())
				{
					var enterpriseLogo = enterprise.FirstOrDefault();
					byte[] logoBytes = new byte[0];
					var logo = enterpriseLogo != null ? enterpriseLogo.logo : logoBytes;//https://www.intelleges.com/mvcmt/Generic/uploadedFiles/EnterpriseLogo/
					string dirname = "~/uploadedFiles/EnterpriseLogo/";

					if (Directory.Exists(Server.MapPath(dirname)))
					{
						var fileName = enterpriseLogo != null ? enterpriseLogo.id + "Logo.png" : "Logo.png";
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

			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;

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
					case 26595:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox71 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox72 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input38 = (comments.Length > 1 ? comments[1] : comments[0]);// comments[1];
						}
						break;


					case 26600:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox61 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox62 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input34 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 26601:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox63 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox64 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

							ViewBag.Input35 = item.comment;
						}
						break;


					case 26602:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox65 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox66 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							ViewBag.Input36 = (comments.Length > 1 ? comments[1] : comments[0]);
						}

						break;


					case 26603:
						switch (item.response)
						{
							case 48734:
								ViewBag.Checkbox67 = _chacked;
								break;
							case 48735:
								ViewBag.Checkbox68 = _chacked;
								comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
								//if (comments.Length > 1)
								ViewBag.Input37 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
								break;
							case 48736:
								ViewBag.Checkbox76 = _chacked;
								break;
						}
						break;

					case 26604:

						switch (item.response)
						{
							case 48737:
								ViewBag.Checkbox69 = _chacked;
								break;
							case 48738:
								ViewBag.Checkbox70 = _chacked;
								break;
						}
						break;


					case 26606:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input5 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];


						break;

					case 26607:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						if (item.response == _responseYES)
						{
							//  ViewBag.Checkbox60 = _chacked;
							//if (comments.Length > 1)
							ViewBag.Input6 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];

						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							// if (comments.Length > 1)
							ViewBag.Input6 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 26617:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox53 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							ViewBag.Checkbox54 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//   if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						switch (item.response)
						{
							case 48749:
								ViewBag.Checkbox53 = _chacked;
								break;
						}
						break;


					case 26618:
						if (item.response == _responseYES)
						{
							//ViewBag.Checkbox60 = _chacked;
							// ViewBag.Checkbox53 = _chacked;
						}
						else if (item.response == _responseNO)
						{

							ViewBag.Checkbox54 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							///if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						switch (item.response)
						{
							case 48750:
								ViewBag.Checkbox54 = _chacked;
								break;
						}
						break;
					case 26619:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input15 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26620:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						ViewBag.Input16 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26621:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						ViewBag.Input17 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;

					case 26622:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26623:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input18 = (comments.Length > 1 ? comments[1] : comments[0]); // comments[0];
						break;
					case 26624:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input19 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26625:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input20 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26626:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26627:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input21 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26628:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input22 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26629:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input23 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26630:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26631:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input24 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26632:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input25 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26633:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input26 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26634:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26635:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input27 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26636:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input28 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26637:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

						ViewBag.Input29 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						break;
					case 26638:
						if (item.response == _responseYES)
						{
							// ViewBag.Checkbox60 = _chacked;
							ViewBag.Checkbox55 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							//  ViewBag.Checkbox61 = _chacked;
							// ViewBag.Checkbox56 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;


					case 26564:
						if (item.response == _responseYES)
						{
							//ViewBag.Checkbox60 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							// ViewBag.Checkbox61 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;
					case 26640:
						ViewBag.Input30 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;
					case 26641:
						ViewBag.Input31 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;
					case 26642:
						ViewBag.Input32 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;
					case 26643:
						ViewBag.Input33 = String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment;
						break;

					#endregion
					#region 1 Question
					case 26652:
						ViewBag.CheckboxSmall = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 26644:
						ViewBag.CheckboxLarge = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26657:
						if (ViewBag.CheckboxLarge == _chacked)
						{
							ViewBag.Checkbox1 = item.response == _responseYES ? _chacked : string.Empty;
						}
						else if (ViewBag.CheckboxSmall == _chacked)
						{
							ViewBag.Checkbox3 = item.response == _responseYES ? _chacked : string.Empty;
						}
						break;

					case 26659:
						if (ViewBag.CheckboxLarge == _chacked)
						{
							ViewBag.Checkbox2 = item.response == _responseYES ? _chacked : string.Empty;
						}
						else if (ViewBag.CheckboxSmall == _chacked)
						{
							ViewBag.Checkbox4 = item.response == _responseYES ? _chacked : string.Empty;
						}
						break;
					case 26654:
						ViewBag.Checkbox5 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26655:
						ViewBag.Checkbox6 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26656:
						ViewBag.Checkbox7 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26647:
						ViewBag.Checkbox8 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26648:
						ViewBag.Checkbox9 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26649:
						ViewBag.Checkbox10 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26650:
						ViewBag.Checkbox11 = item.response == _responseYES ? _chacked : string.Empty;
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						if (comments.Length > 1 && comments[1].Contains("Yes"))
							ViewBag.Checkbox11_comment = _responseYES;

						switch (item.response)
						{
							case 48752:
								ViewBag.Checkbox11 = _chacked;
								break;
							case 48753:
								ViewBag.Checkbox11 = _chacked;
								ViewBag.Checkbox11_comment = _chacked;
								break;
							default:
								ViewBag.Checkbox11 = string.Empty;
								ViewBag.Checkbox11_comment = string.Empty;
								break;

						}
						break;
					case 26646:
						ViewBag.Checkbox12 = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 26645:
						ViewBag.Checkbox13 = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 26651:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox14 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input0 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;
					case 26609:
						ViewBag.Checkbox15 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26610:
						ViewBag.Checkbox16 = item.response == _responseYES ? _chacked : string.Empty;
						break;

					case 26611:
						ViewBag.Checkbox17 = item.response == _responseYES ? _chacked : string.Empty;
						break;



					case 26653:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox18 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox19 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input1 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					case 26658:
						switch (item.response)
						{
							case 48755:
								ViewBag.Checkbox20 = _chacked;
								break;
							case 48756:
								ViewBag.Checkbox21 = _chacked;
								break;
							case 48757:
								ViewBag.Checkbox22 = _chacked;
								break;
							case 48758:
								ViewBag.Checkbox23 = _chacked;
								break;
							case 48759:
								ViewBag.Checkbox24 = _chacked;
								break;
							case 48760:
								ViewBag.Checkbox25 = _chacked;
								comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
								// if (comments.Length >= 1)
								ViewBag.Input2 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
								break;
						}
						break;

					#endregion

					#region 2 Question
					case 26594:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox26 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox27 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//if (comments.Length >= 1)
							ViewBag.Input4 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[0];
						}
						break;

					#endregion

					#region 4 Question
					case 26612:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox28 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox29 = _chacked;
						}
						break;
					case 26613:
						switch (item.response)
						{
							case 48739:
								ViewBag.Checkbox28_1 = _chacked;
								break;
							case 48740:
								ViewBag.Checkbox29_1 = _chacked;
								ViewBag.Input7_1 = item.comment;
								break;
							case 48741:
								ViewBag.Checkbox29_2 = _chacked;
								break;
						}
						break;

					#endregion

					#region 5 Question

					case 26614:
						switch (item.response)
						{
							case 48742:
								ViewBag.Checkbox30_1 = _chacked;
								break;
							case 48743:
								ViewBag.Checkbox31_1 = _chacked;
								break;
							case 48744:
								ViewBag.Checkbox32_1 = _chacked;
								break;

						}
						break;

					#endregion

					#region 6 Question

					case 26615:
						switch (item.response)
						{
							case 48745:
								ViewBag.Checkbox30 = _chacked;
								break;
							case 48746:
								ViewBag.Checkbox31 = _chacked;
								comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
								ViewBag.Input7 = (comments.Length > 1 ? comments[1] : comments[0]);
								break;
							case 48747:
								ViewBag.Checkbox73 = _chacked;
								break;
						}
						break;
					#endregion

					#region 7 Question
					case 26597:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox35 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox36 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input8 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					#endregion

					#region 8 Question
					case 26598:
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

					#region 9 Question
					case 26599:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox43 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox44 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//  if (comments.Length > 1)
							ViewBag.Input10 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					#endregion

					#region 10 Question
					case 26616:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox45 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox46 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input11 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						break;

					#endregion

					#region 11 Question
					case 26660:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox47 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							// if (comments.Length > 1)
							ViewBag.Input12 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox48 = _chacked;

						}
						break;

					case 26661:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox49 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//   if (comments.Length > 1)
							ViewBag.Input13 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox50 = _chacked;

						}
						break;

					case 26662:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox51 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
							//   if (comments.Length > 1)
							ViewBag.Input14 = (comments.Length > 1 ? comments[1] : comments[0]); //comments[1];
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox52 = _chacked;
						}
						break;

					#endregion

					#region 14 Question
					//When 19049 =75 then 19050 is skipped and should have no checkmarks at all

					case 26663:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox57 = _chacked;
							// ViewBag.Checkbox58 = string.Empty;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox58 = _chacked;
							ViewBag.case19049 = 75;
						}
						break;

					case 26664:
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

					#region ADDITIONAL SUPPLIER COMMENTS
					case 26665:
						comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);
						ViewBag.Input391 = (comments.Length > 1 ? comments[1] : comments[0]);
						break;

					#endregion


					#region CERTIFICATION
					case 26667:
						if (item.response == _responseYES)
						{
							ViewBag.Checkbox77 = _chacked;
						}
						else if (item.response == _responseNO)
						{
							ViewBag.Checkbox78 = _chacked;
							comments = System.Text.RegularExpressions.Regex.Split((String.IsNullOrEmpty(item.comment) ? string.Empty : item.comment), _responseSplitter);

							ViewBag.Input39 = (comments.Length > 1 ? comments[1] : comments[0]);


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
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			if (question != null && (question.footer == "3"))
			{
				pptqID = FillCustomPdfHtml(ViewBag, db, Session, Server);
				ViewName = "CustomQuestionnaireSurveyPdfDownload";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && (question.footer == "2"))
			{
				pptqID = FillPdfHtml(ViewBag, db, Session, Server);
				ViewName = "CustomizedQuestionnaireSurveyPdfDownload";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && (question.footer == "4"))
			{

			}
			else if (question != null && (question.footer == "5"))
			{
				pptqID = FillMOOGPdfHtml(ViewBag, db, Session, Server);
				ViewName = "MoogCustomizedQuestionnaireSurveyPdfDownload";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && (question.footer == "6"))
			{
				pptqID = FillMOOGPdfHtml6(ViewBag, db, Session, Server);
				ViewName = "MoogCustomizedQuestionnaireSurveyPdfDownload6";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && question.footer == "7")
			{
				pptqID = FillPODPdfHtml(ViewBag, db, Session, Server);
				ViewName = "PODQuestionnaireSurveyPdfDownload";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && question.footer == "8")
			{
				pptqID = FillPODPdfHtml8(ViewBag, db, Session, Server);
				ViewName = "PODQuestionnaireSurveyPdfDownload8";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && question.footer == "9")
			{
				pptqID = FillPODPdfHtml9(ViewBag, db, Session, Server);
				ViewName = "PODQuestionnaireSurveyPdfDownload9";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && (question.footer == "10"))
			{
				pptqID = FillCustomPdfHtml10(ViewBag, db, Session, Server);
				ViewName = "CustomQuestionnaireSurveyPdfDownload10";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			else if (question != null && (question.footer == "11"))
			{
				pptqID = FillCustomPdfHtml11(ViewBag, db, Session, Server);
				ViewName = "CustomQuestionnaireSurveyPdfDownload10";
				return ViewCustomizedPdf(pptqID, ViewName);
			}
			// else return PDFConfirmation();
			pptqID = FillCustomPdfHtml(ViewBag, db, Session, Server);
			return ViewCustomPdf(pptqID);
		}
		private static decimal GetNumber(string value)
		{
			/*Regex decimalDeterminator = new Regex("\\d+(\\.\\d{1,2})?");
			var match = decimalDeterminator.Match(value);
			if (match.Success)
				return decimal.Parse(match.Value);
			else return 0;*/

			decimal val = -1;

			if (decimal.TryParse(value, out val))
				return val;

			try
			{
				return decimal.Parse(Regex.Match(value, @"-?\d{1,3}(,\d{3})*(\.\d+)?").Value);
			}
			catch
			{
				return 0;
			}
		}

		public static int FillPODPdfHtml(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
		{
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			ViewBag.partner = _partner;
			ViewBag.Supplyer = _partner.name;
			ViewBag.PurchaseOrderNumber = _partner.address1;
			ViewBag.PurchaseOrderValue = "$" + new Regex("/\\B(?=(\\d{3})+(?!\\d))/g").Replace(_partner.address2.Replace(",", ""), ",");
			ViewBag.PO_REVISION_NUMBER = _partner.phone;
			ViewBag.PartNumber = _partner.zipcode;
			ViewBag.PnDescription = _partner.dunsNumber;
			ViewBag.ChangeAmount = _partner.city;
			ViewBag.BuyerName = _partner.firstName + " " + _partner.lastName;
			ViewBag.ComplienceAnalist = _partner.title;
			ViewBag.GlobalSourcing = _partner.fax;
			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;
			//_signature
			ViewBag.signature = _signature;
			ViewBag.personTitle = _partner != null ? _partner.title : "";
			ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
			//  var _PPTQQuestionResponse = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID).ToList();

			var _PPTQQuestionResponse = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseByPPTQ(pptqID).ToList();
			var _responseYES = 74;
			var _responseNO = 75;
			var _chacked = "checked";
			decimal sumofFirst = 0;
			decimal sumOfSecond = 0;
			decimal sumOfThird = 0;
			Regex codeRegex = new Regex("\\([A-Z][A-Z]\\)");

			foreach (var item in _PPTQQuestionResponse)
			{
				switch (item.question)
				{
					#region Question1
					case 23675:
						ViewBag.Q23675_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23675_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23570:
						switch (item.response)
						{
							case 46414:
								ViewBag.Q24812_FirmFixedPrice = "checked";
								break;
							case 46415:
								ViewBag.Q24812_CostReimbursement = "checked";
								break;
							case 46416:
								ViewBag.Q24812_Other = "checked";
								break;
							default: break;

						}
						ViewBag.Q24812_Comment = item.comment;
						break;
					case 23571:
						switch (item.response)
						{
							case 46417:
								ViewBag.Q23571_Yes = "checked";
								break;
							case 46418:
								ViewBag.Q23571_No = "checked";
								break;
							case 46419:
								ViewBag.Q23571_NA = "checked";
								break;
							default: break;

						}

						break;
					case 23572:
						switch (item.response)
						{
							case 46420:
								ViewBag.Q23572_46420 = _chacked;
								break;
							case 46421:
								ViewBag.Q23572_46421 = _chacked;
								break;
							case 46422:
								ViewBag.Q23572_46422 = _chacked;
								break;
							case 46423:
								ViewBag.Q23572_46423 = _chacked;
								break;
							default: break;

						}
						ViewBag.Q23572_Comment = item.comment;
						break;
					case 23573:
						ViewBag.Q23573_Yes = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23574:
						ViewBag.Q23574_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23574_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23575:
						ViewBag.Q23574_Comment = item.comment;
						break;
					case 23576:
						ViewBag.Q23576_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23576_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23577:
						ViewBag.Q23576_Comment = item.comment;
						break;
					case 23578:
						ViewBag.Q23578_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23578_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23579:
						ViewBag.Q23578_Comment = item.comment;
						break;
					case 23580:
						switch (item.response)
						{
							case 46424:
								ViewBag.Q23580_46424 = _chacked;
								break;
							case 46425:
								ViewBag.Q23580_46425 = _chacked;
								break;
							case 46426:
								ViewBag.Q23580_46426 = _chacked;
								break;
							default: break;

						}
						break;

					case 23581:
						ViewBag.Q24816_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24816_DropDownValues_Response = item.response;
						break;

					case 23582:
						ViewBag.Q24817_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24817_DropDownValues_Response = item.response;
						ViewBag.Q24817_Comment = item.comment;
						break;
					case 23583:
						switch (item.response)
						{
							case 46442:
								ViewBag.Q24818_46072 = _chacked;
								break;
							case 46443:
								ViewBag.Q24818_46073 = _chacked;
								break;
							case 46444:
								ViewBag.Q24818_46074 = _chacked;
								break;
							default: break;
						}
						break;
					#endregion
					#region Section 2-3
					case 23584:
						ViewBag.Q24819_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23585:
						ViewBag.Q24820_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23586:
						ViewBag.Q24821_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23587:
						ViewBag.Q24822_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23589:
						ViewBag.Q24824_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24824_Value = item.comment;
						break;
					case 23590:
						ViewBag.Q24825_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23591:
						ViewBag.Q24826_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23592:
						ViewBag.Q24827_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23593:
						ViewBag.Q24828_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23594:
						ViewBag.Q24829_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23595:
						ViewBag.Q24830_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23588:
						ViewBag.Q24823_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24823_Comment = item.comment;
						break;
					case 23596:
						ViewBag.Q24831_Value = item.comment;
						break;


					case 23597:
						ViewBag.Q24832_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23598:
						ViewBag.Q24833_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23599:
						ViewBag.Q24834_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23600:
						ViewBag.Q24835_Value = item.comment;
						break;
					case 23601:
						ViewBag.Q24836_Value = item.comment;
						break;
					case 23602:
						ViewBag.Q24837_Value = item.comment;
						break;
					case 23603:
						ViewBag.Q24838_Value = item.comment;
						break;
					case 23604:
						ViewBag.Q24839_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24839_Comment = item.comment;
						break;
					case 23605:
						ViewBag.Q24840_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23606:
						ViewBag.Q24841_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23607:
						ViewBag.Q24842_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23608:
						ViewBag.Q24843_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23609:
						ViewBag.Q24844_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24844_Comment = item.comment;
						break;
					case 23610:
						ViewBag.Q24845_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 23611:
						ViewBag.Q24846_Comment = item.comment;
						break;
					#endregion



					#region Section 4
					case 23612:
						ViewBag.Q24847_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24847_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23613:
						ViewBag.Q24848_Value = item.comment;
						break;
					case 23614:
						ViewBag.Q24849_Value = item.comment;
						break;
					case 23615:
						ViewBag.Q24850_Value = item.comment;
						break;
					case 23616:
						ViewBag.Q24851_Value = item.comment;
						break;
					case 23617:
						ViewBag.Q24852_Value = item.comment;
						break;
					case 23618:
						ViewBag.Q24853_Value = item.comment;
						break;
					case 23619:
						ViewBag.Q24854_Value = item.comment;
						break;
					#endregion

					#region Section 5
					case 23627:
						ViewBag.Q24862_Value = item.comment;
						break;
					case 23643:
						ViewBag.Q24878_Value = item.comment;
						break;
					case 23659:
						ViewBag.Q23659_Value = item.comment;
						break;
					case 23628:
						ViewBag.Q24863_Value = item.comment;
						break;
					case 23644:
						ViewBag.Q24879_Value = item.comment;
						break;
					case 23660:
						ViewBag.Q24895_Value = item.comment;
						break;
					//Sum of 23629+23632+23635+23638+23640+23641
					case 23629:
						ViewBag.Q23629_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 23632:
						ViewBag.Q24867_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 23635:
						ViewBag.Q24870_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 23638:
						ViewBag.Q24873_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 23640:
						ViewBag.Q24875_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 23641:
						ViewBag.Q2424876_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					//Sum of 23645+23648+23651+23654+23656+23657
					case 23645:
						ViewBag.Q24880_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 23648:
						ViewBag.Q24883_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 23651:
						ViewBag.Q24886_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 23654:
						ViewBag.Q24889_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 23656:
						ViewBag.Q24891_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 23657:
						ViewBag.Q2424892_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					//Sum of 23661+23664+23667+23670+23672+23673
					case 23661:
						ViewBag.Q24896_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 23664:
						ViewBag.Q24899_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 23667:
						ViewBag.Q24902_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 23670:
						ViewBag.Q24905_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 23672:
						ViewBag.Q24907_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 23673:
						ViewBag.Q2424908_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					//
					case 23630:
						ViewBag.Q24865_Value = item.comment;
						break;
					case 23620:
						ViewBag.Q24855_Value = item.comment;
						break;
					case 23646:
						ViewBag.Q24881_Value = item.comment;
						break;
					case 23662:
						ViewBag.Q24897_Value = item.comment;
						break;
					case 23622:
						ViewBag.Q24857_Value = item.comment;
						break;
					case 23649:
						ViewBag.Q24884_Value = item.comment;
						break;
					case 23665:
						ViewBag.Q24900_Value = item.comment;
						break;


					case 23624:
						ViewBag.Q24859_Value = item.comment;
						break;
					case 23636:
						ViewBag.Q24871_Value = item.comment;
						break;
					case 23652:
						ViewBag.Q24887_Value = item.comment;
						break;
					case 23668:
						ViewBag.Q24903_Value = item.comment;
						break;
					case 23626:
						ViewBag.Q24861_Value = item.comment;
						break;
					case 23639:
						ViewBag.Q24874_Value = item.comment;
						break;
					case 23655:
						ViewBag.Q24890_Value = item.comment;
						break;
					case 23671:
						ViewBag.Q24906_Value = item.comment;
						break;
					case 23633:
						ViewBag.Q24868_Value = item.comment;
						break;
					case 23674:
						ViewBag.Q24909_Value = item.comment;
						break;



					case 24910:
						ViewBag.Q24910_Value = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24911:
						ViewBag.Q24911_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24911_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24912:
						ViewBag.Q24912_Value = item.comment;
						break;
					case 24913:
						ViewBag.Q24913_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24913_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24914:
						ViewBag.Q24914_Value = item.comment;
						break;
					case 24915:

						ViewBag.Q24915_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24915_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24916:

						ViewBag.Q24916_Yes = item.response == 46075 ? _chacked : string.Empty;
						ViewBag.Q24916_No = item.response == 46076 ? _chacked : string.Empty;
						ViewBag.Q24916_NA = item.response == 46077 ? _chacked : string.Empty;
						break;
					case 23703:
						ViewBag.Q24917_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24917_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23704:
						ViewBag.Q24918_Value = item.comment;// == _responseYES ? _chacked : string.Empty;
						//ViewBag.Q24917_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23705:

						ViewBag.Q24919_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24919_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23706:
						ViewBag.Q24920_Value = item.comment;// == _responseYES ? _chacked : string.Empty;
						//ViewBag.Q24917_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23707:

						ViewBag.Q24921_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24921_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23708:
						ViewBag.Q24922_Value = item.comment;// == _responseYES ? _chacked : string.Empty;
						//ViewBag.Q24917_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23688:

						ViewBag.Q24923_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24923_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24924:

						ViewBag.Q24924_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24924_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23690:

						ViewBag.Q24925_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24925_No = item.response == _responseNO ? _chacked : string.Empty;
						break;

					case 23691:
						ViewBag.Q24926_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24926_DropDownValues_Response = item.response;
						break;
					case 23692:

						ViewBag.Q24927_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24927_No = item.response == _responseNO ? _chacked : string.Empty;
						break;

					case 23693:
						ViewBag.Q24928_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24928_DropDownValues_Response = item.response;
						break;
					case 23694:

						ViewBag.Q24929_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24929_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23695:
						ViewBag.Q24930_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24930_DropDownValues_Response = item.response;
						break;
					case 23696:

						ViewBag.Q24931_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24931_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23697:
						ViewBag.Q24932_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24932_DropDownValues_Response = item.response;
						break;
					case 23698:

						ViewBag.Q24933_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24933_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23699:
						ViewBag.Q24934_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24934_DropDownValues_Response = item.response;
						break;
					case 23700:

						ViewBag.Q24935_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24935_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 23701:
						ViewBag.Q24936_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24936_DropDownValues_Response = item.response;
						break;
					case 23702:
						ViewBag.Q24937_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24937_DropDownValues_Response = item.response;
						ViewBag.Q24937_Value = item.comment;
						break;
					case 23687:

						ViewBag.Q24938_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24938_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24939_Value = item.comment;
						break;
					#endregion
					#region 77
					case 23680:
						switch (item.response)
						{
							case 46457:
								ViewBag.Q24940_46123 = _chacked;
								break;
							case 46458:
								ViewBag.Q24940_46124 = _chacked;
								break;
							case 46459:
								ViewBag.Q24940_46125 = _chacked; break;
							//case 46054:
							//	ViewBag.Q24940_46054 = _chacked; 
							//	break;
							default: break;

						}
						//ViewBag.Q24940_Comment = item.comment;
						break;
					case 23681:
						ViewBag.Q23681_Comment = item.comment;
						break;
					case 23682:
						ViewBag.Q24941_46126 = item.response == 46460 ? _chacked : string.Empty;
						ViewBag.Q24941_46127 = item.response == 46461 ? _chacked : string.Empty;
						ViewBag.Q24941_comment = item.comment;
						break;
					case 23683:
						ViewBag.Q24942_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24942_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24942_Comment = item.comment;
						break;
					case 23684:
						ViewBag.Q24943_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24943_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24943_Comment = item.comment;
						break;
					case 23685:
						switch (item.response)
						{
							case 46462:
								ViewBag.Q24944_46128 = _chacked;
								break;
							case 46463:
								ViewBag.Q24944_46129 = _chacked;
								break;
							case 46464:
								ViewBag.Q24944_46130 = _chacked; break;
							//case 46054:
							//	ViewBag.Q24940_46054 = _chacked; 
							//	break;
							default: break;

						}
						ViewBag.Q24944_Comment = item.comment;
						break;
					case 23686:
						ViewBag.Q24945_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24945_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24945_Comment = item.comment;
						break;
					case 23678:
						ViewBag.Q24946_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24946_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24946_Comment = item.comment;
						break;
					case 23679:
						ViewBag.Q24947_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24947_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24947_Comment = item.comment;
						break;
					case 23709:
						ViewBag.Q23709_Yes = item.response == _responseYES ? _chacked : string.Empty;
						break;
					#endregion
					default: break;
				}
			}
			ViewBag.sumOfFirst = sumofFirst;
			ViewBag.sumOfSecond = sumOfSecond;
			ViewBag.sumOfThird = sumOfThird;
			return pptqID;
		}


		public static int FillPODPdfHtml9(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
		{
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			ViewBag.partner = _partner;
			ViewBag.Supplyer = _partner.name;
			ViewBag.PurchaseOrderNumber = _partner.address1;


			decimal v1 = 0;
			if (decimal.TryParse((_partner.address2 ?? "").Trim().Replace("$", ""), out v1))
				ViewBag.PurchaseOrderValue = string.Format("{0:C}", v1);
			else
				ViewBag.PurchaseOrderValue = _partner.address2;  /*"$" + new Regex("/\\B(?=(\\d{3})+(?!\\d))/g").Replace(_partner.address2.Replace(",", ""), ",");*/

			ViewBag.PO_REVISION_NUMBER = _partner.internalID.Replace(_partner.address1 + " ", "");
			ViewBag.PartNumber = _partner.zipcode;
			ViewBag.PnDescription = _partner.title;
			ViewBag.ChangeAmount = _partner.city;
			ViewBag.BuyerName = _partner.firstName + " " + _partner.lastName;
			ViewBag.ComplienceAnalist = _partner.firstName + " " + _partner.lastName;
			ViewBag.GlobalSourcing = _partner.fax;

			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;
			//_signature
			ViewBag.signature = _signature;
			ViewBag.personTitle = _partner != null ? _partner.title : "";
			ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
			//  var _PPTQQuestionResponse = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID).ToList();

			var _PPTQQuestionResponse = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseByPPTQ(pptqID).ToList();
			var _responseYES = 74;
			var _responseNO = 75;
			var _chacked = "checked";
			decimal sumofFirst = 0;
			decimal sumOfSecond = 0;
			decimal sumOfThird = 0;
			int qty1 = 0;
			int qty2 = 0;
			int qty3 = 0;
			int qty4 = 0;
			Regex codeRegex = new Regex("\\([A-Z][A-Z]\\)");

			foreach (var item in _PPTQQuestionResponse)
			{
				switch (item.question)
				{
					#region Question1
					case 25068:
						ViewBag.Q23675_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23675_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24981:
						switch (item.response)
						{
							case 47698:
								ViewBag.Q24812_FirmFixedPrice = "checked";
								break;
							case 47699:
								ViewBag.Q24812_CostReimbursement = "checked";
								break;
							case 47700:
								ViewBag.Q24812_Other = "checked";
								break;
							default: break;

						}
						ViewBag.Q24812_Comment = item.comment;
						break;
					case 24982:
						switch (item.response)
						{
							case 47701:
								ViewBag.Q23571_Yes = "checked";
								break;
							case 47702:
								ViewBag.Q23571_No = "checked";
								break;
							case 47703:
								ViewBag.Q23571_NA = "checked";
								break;
							default: break;
						}
						break;
					case 24983:
						switch (item.response)
						{
							case 47704:
								ViewBag.Q23572_46420 = _chacked;
								break;
							case 47705:
								ViewBag.Q23572_46421 = _chacked;
								break;
							case 47706:
								ViewBag.Q23572_46422 = _chacked;
								break;
							case 47707:
								ViewBag.Q23572_46423 = _chacked;
								break;
							default: break;

						}
						ViewBag.Q23572_Comment = item.comment;
						break;
					case 24984:
						ViewBag.Q23573_Yes = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24985:
						ViewBag.Q23574_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23574_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24986:
						ViewBag.Q23574_Comment = item.comment;
						break;
					case 24987:
						ViewBag.Q23576_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23576_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24988:
						ViewBag.Q23576_Comment = item.comment;
						break;
					case 24989:
						ViewBag.Q23578_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23578_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24990:
						ViewBag.Q23578_Comment = item.comment;
						break;
					case 24991:
						ViewBag.Q24815_Comment = item.comment;
						switch (item.response)
						{
							case 47708:
								ViewBag.Q23580_46424 = _chacked;
								break;
							case 47709:
								ViewBag.Q23580_46425 = _chacked;
								break;
							case 47710:
								ViewBag.Q23580_46426 = _chacked;
								break;
							default: break;

						}
						break;

					case 24992:
						ViewBag.Q24816_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24816_DropDownValues_Response = item.response;
						ViewBag.Q24816_Comment = item.comment;
						break;

					case 24993:
						ViewBag.Q24817_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24817_DropDownValues_Response = item.response;
						ViewBag.Q24817_Comment = item.comment;
						break;
					case 24994:
						switch (item.response)
						{
							case 47726:
								ViewBag.Q24818_46072 = _chacked;
								break;
							case 47727:
								ViewBag.Q24818_46073 = _chacked;
								break;
							case 47728:
								ViewBag.Q24818_46074 = _chacked;
								break;
							default: break;
						}
						break;
					#endregion
					#region Section 2-3

					case 24995:
						ViewBag.Q24819_Checked = string.Empty;
						ViewBag.Q24820_Checked = string.Empty;
						ViewBag.Q24824_Checked = string.Empty;
						ViewBag.Q24823_Checked = string.Empty;

						switch (item.response)
						{
							case 47729:
								ViewBag.Q24819_Checked = _chacked;
								break;
							case 47730:
								ViewBag.Q24820_Checked = _chacked;
								break;
							case 47731:
								ViewBag.Q24824_Checked = _chacked;
								ViewBag.Q24824_Value = item.comment;
								break;
							case 47732:
								ViewBag.Q24823_Checked = _chacked;
								ViewBag.Q24823_Comment = item.comment;
								break;
							default: break;
						}
						break;
					case 24996:
						ViewBag.Q24821_Checked = string.Empty;
						ViewBag.Q24822_Checked = string.Empty;

						switch (item.response)
						{
							case 47733:
								ViewBag.Q24821_Checked = _chacked;
								break;
							case 47734:
								ViewBag.Q24822_Checked = _chacked;
								break;

							default: break;
						}
						break;

					case 24997:
						ViewBag.Q24825_Checked = string.Empty;
						ViewBag.Q24826_Checked = string.Empty;
						ViewBag.Q24827_Checked = string.Empty;
						ViewBag.Q24828_Checked = string.Empty;
						ViewBag.Q24829_Checked = string.Empty;
						ViewBag.Q24830_Checked = string.Empty;
						ViewBag.Q24824_Value = item.comment;
						switch (item.response)
						{
							case 47735:
								ViewBag.Q24825_Checked = _chacked;
								break;
							case 47736:
								ViewBag.Q24826_Checked = _chacked;
								break;
							case 47737:
								ViewBag.Q24827_Checked = _chacked;
								break;
							case 47738:
								ViewBag.Q24828_Checked = _chacked;
								break;
							case 47739:
								ViewBag.Q24829_Checked = _chacked;
								break;
							case 47740:
								ViewBag.Q24830_Checked = _chacked;
								break;
							default: break;
						}
						break;


					case 25044:
						ViewBag.Q24831_Value = item.comment;
						break;


					// Section 3
					case 25045:
						ViewBag.Q24832_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 25046:
						ViewBag.Q24833_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 25047:
						ViewBag.Q24834_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 25048:
						ViewBag.Q24835_Value = item.comment;
						break;
					case 25049:
						ViewBag.Q24836_Value = item.comment;
						break;
					case 25050:
						ViewBag.Q24837_Value = item.comment;
						break;
					case 25051:
						ViewBag.Q24838_Value = item.comment;
						break;
					case 25052:
						ViewBag.Q24839_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24839_Comment = item.comment;
						break;
					case 25053:
						ViewBag.Q24840_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 25054:
						ViewBag.Q24841_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 25055:
						ViewBag.Q24842_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 25056:
						ViewBag.Q24843_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 25057:
						ViewBag.Q24844_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24844_Comment = item.comment;
						break;
					case 25058:
						ViewBag.Q24845_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24845_Comment = item.comment;
						break;
					case 25059:
						ViewBag.Q24846_Comment = item.comment;
						break;
					#endregion



					#region Section 4
					case 25060:
						ViewBag.Q24847_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24847_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25061:
						//Start Date
						DateTime dttm = DateTime.MinValue;
						if (DateTime.TryParse(item.comment, out dttm))
							ViewBag.Q24848_Value = dttm.ToString("MM/dd/yyyy");
						else ViewBag.Q24848_Value = item.comment;
						break;
					case 25062:
						//End Date
						DateTime dttm1 = DateTime.MinValue;
						if (DateTime.TryParse(item.comment, out dttm1))
							ViewBag.Q24849_Value = dttm1.ToString("MM/dd/yyyy");
						else ViewBag.Q24849_Value = item.comment;
						break;
					case 25063:
						ViewBag.Q24850_Value = item.comment;
						break;
					case 25064:
						ViewBag.Q24851_Value = item.comment;
						break;
					case 25065:
						ViewBag.Q24852_Value = item.comment;
						break;
					case 25066:
						ViewBag.Q24853_Value = item.comment;
						break;
					case 25067:
						ViewBag.Q24854_Value = item.comment;
						break;
					#endregion

					#region Section 5
					case 25002:
						ViewBag.Q24862_Value = item.comment;
						break;
					case 25016:
						ViewBag.Q24878_Value = item.comment;
						break;
					case 25030:
						ViewBag.Q23659_Value = item.comment;
						break;
					case 25003:
						ViewBag.Q24863_Value = item.comment;
						break;
					case 25017:
						ViewBag.Q24879_Value = item.comment;
						break;
					case 25031:
						ViewBag.Q24895_Value = item.comment;
						break;
					//Sum of  25004+25006+25008+25010+25013+25014
					case 25004:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q23629_Value = string.Format("{0:C}", v);
							else ViewBag.Q23629_Value = item.comment;
							sumofFirst += v * qty1;
						}
						break;
					case 25006:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24867_Value = string.Format("{0:C}", v);
							else ViewBag.Q24867_Value = item.comment;
							sumofFirst += v * qty2;
						}
						break;
					case 25008:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24870_Value = string.Format("{0:C}", v);
							else ViewBag.Q24870_Value = item.comment;
							sumofFirst += v * qty3;
						}
						break;
					case 25010:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24873_Value = string.Format("{0:C}", v);
							else ViewBag.Q24873_Value = item.comment;
							sumofFirst += v * qty4;
						}
						break;
					case 25013:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24875_Value = string.Format("{0:C}", v);
							else ViewBag.Q24875_Value = item.comment;
							sumofFirst += v;
						}
						break;
					case 25014:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q2424876_Value = string.Format("{0:C}", v);
							else ViewBag.Q2424876_Value = item.comment;
							sumofFirst += v;
						}
						break;
					//Sum of 25018+25020+25022+25024+25027+25028
					case 25018:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24880_Value = string.Format("{0:C}", v);
							else ViewBag.Q24880_Value = item.comment;
							sumOfSecond += v * qty1;
						}
						break;
					case 25020:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24883_Value = string.Format("{0:C}", v);
							else ViewBag.Q24883_Value = item.comment;
							sumOfSecond += v * qty2;
						}
						break;
					case 25022:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24886_Value = string.Format("{0:C}", v);
							else ViewBag.Q24886_Value = item.comment;
							sumOfSecond += v * qty3;
						}

						break;
					case 25024:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24889_Value = string.Format("{0:C}", v);
							else ViewBag.Q24889_Value = item.comment;
							sumOfSecond += v * qty4;
						}

						break;
					case 25027:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24891_Value = string.Format("{0:C}", v);
							else ViewBag.Q24891_Value = item.comment;
							sumOfSecond += v;
						}

						break;
					case 25028:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q2424892_Value = string.Format("{0:C}", v);
							else ViewBag.Q2424892_Value = item.comment;
							sumOfSecond += v;
						}

						break;
					//Sum of 25032+25034+25036+25038+25041+25042
					case 25032:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24896_Value = string.Format("{0:C}", v);
							else ViewBag.Q24896_Value = item.comment;
							sumOfThird += v * qty1;
						}

						break;
					case 25034:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24899_Value = string.Format("{0:C}", v);
							else ViewBag.Q24899_Value = item.comment;
							sumOfThird += v * qty2;
						}

						break;
					case 25036:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24902_Value = string.Format("{0:C}", v);
							else ViewBag.Q24902_Value = item.comment;
							sumOfThird += v * qty3;
						}
						break;
					case 25038:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24905_Value = string.Format("{0:C}", v);
							else ViewBag.Q24905_Value = item.comment;
							sumOfThird += v * qty4;
						}

						break;
					case 25041:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q24907_Value = string.Format("{0:C}", v);
							else ViewBag.Q24907_Value = item.comment;
							sumOfThird += v;
						}
						break;
					case 25042:
						if (!string.IsNullOrEmpty(item.comment))
						{
							decimal v = GetNumber(item.comment);
							if (v != 0) ViewBag.Q2424908_Value = string.Format("{0:C}", v);
							else ViewBag.Q2424908_Value = item.comment;
							sumOfThird += v;
						}

						break;
					//0.1

					case 24998:
						if (int.TryParse((item.comment ?? "").Trim(), out qty1))
						{
							ViewBag.Q24855_Value = item.comment;
						}
						break;
					case 25005:
						ViewBag.Q24865_Value = item.comment;
						break;
					case 25019:
						ViewBag.Q24881_Value = item.comment;
						break;
					case 25033:
						ViewBag.Q24897_Value = item.comment;
						break;

					//0.2
					case 24999:
						if (int.TryParse((item.comment ?? "").Trim(), out qty2))
						{
							ViewBag.Q24857_Value = item.comment;
						}
						break;
					case 25007:
						ViewBag.Q24868_Value = item.comment;
						break;
					case 25021:
						ViewBag.Q24884_Value = item.comment;
						break;
					case 25035:
						ViewBag.Q24900_Value = item.comment;
						break;

					//0.3
					case 25000:
						if (int.TryParse((item.comment ?? "").Trim(), out qty3))
						{
							ViewBag.Q24859_Value = item.comment;
						}
						break;
					case 25009:
						ViewBag.Q24871_Value = item.comment;
						break;
					case 25023:
						ViewBag.Q24887_Value = item.comment;
						break;
					case 25037:
						ViewBag.Q24903_Value = item.comment;
						break;

					//0.4
					case 25001:
						if (int.TryParse((item.comment ?? "").Trim(), out qty4))
						{
							ViewBag.Q24861_Value = item.comment;
						}
						break;
					case 25011:
						ViewBag.Q24874_Value = item.comment;
						break;
					case 25025:
						ViewBag.Q24890_Value = item.comment;
						break;
					case 25039:
						ViewBag.Q24906_Value = item.comment;
						break;

					//Memo
					case 25043:
						ViewBag.Q24909_Value = item.comment;
						break;


					//section 6
					case 25095:
						ViewBag.Q24917_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24917_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25096:
						ViewBag.Q24918_Value = item.comment;
						break;
					case 25097:
						ViewBag.Q24919_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24919_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25098:
						ViewBag.Q24920_Value = item.comment;
						break;
					case 25099:
						ViewBag.Q24921_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24921_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25100:
						ViewBag.Q24922_Value = item.comment;
						break;
					case 25080:
						ViewBag.Q24923_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24923_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25082:
						ViewBag.Q24925_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24925_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25083:
						ViewBag.Q24926_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24926_DropDownValues_Response = item.response;
						break;
					case 25084:
						ViewBag.Q24927_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24927_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25085:
						ViewBag.Q24928_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24928_DropDownValues_Response = item.response;
						break;
					case 25086:
						ViewBag.Q24929_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24929_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25087:
						ViewBag.Q24930_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24930_DropDownValues_Response = item.response;
						break;
					case 25088:
						ViewBag.Q24931_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24931_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25089:
						ViewBag.Q24932_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24932_DropDownValues_Response = item.response;
						break;
					case 25090:
						ViewBag.Q24933_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24933_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25091:
						ViewBag.Q24934_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24934_DropDownValues_Response = item.response;
						break;
					case 25092:
						ViewBag.Q24935_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24935_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 25093:
						ViewBag.Q24936_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24936_DropDownValues_Response = item.response;
						break;
					case 25094:
						ViewBag.Q24937_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24937_DropDownValues_Response = item.response;
						ViewBag.Q24937_Value = item.comment;
						break;
					case 25079:
						ViewBag.Q24938_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24938_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24939_Value = item.comment;
						break;
					#endregion
					#region 77
					case 25072:
						switch (item.response)
						{
							case 47747:
								ViewBag.Q24940_46123 = _chacked;
								break;
							case 47748:
								ViewBag.Q24940_46124 = _chacked;
								break;
							case 47749:
								ViewBag.Q24940_46125 = _chacked; break;
							default: break;

						}
						break;
					case 25073:
						ViewBag.Q23681_Comment = item.comment;
						break;
					case 25074:
						ViewBag.Q24941_46126 = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24941_46127 = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24941_comment = item.comment;
						break;
					case 25075:
						ViewBag.Q24942_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24942_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24942_Comment = item.comment;
						break;
					case 25076:
						ViewBag.Q24943_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24943_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24943_Comment = item.comment;
						break;
					case 25077:
						switch (item.response)
						{
							case 47750:
								ViewBag.Q24944_46128 = _chacked;
								break;
							case 47751:
								ViewBag.Q24944_46129 = _chacked;
								break;
							case 47752:
								ViewBag.Q24944_46130 = _chacked; break;
							default: break;

						}
						ViewBag.Q24944_Comment = item.comment;
						break;
					case 25078:
						ViewBag.Q24945_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24945_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24945_Comment = item.comment;
						break;
					case 25070:
						ViewBag.Q24946_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24946_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24946_Comment = item.comment;
						break;
					case 25071:
						ViewBag.Q24947_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24947_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24947_Comment = item.comment;
						break;
					case 25101:
						ViewBag.Q23709_Yes = item.response == _responseYES ? _chacked : string.Empty;
						break;
					#endregion
					default: break;
				}
			}
			ViewBag.sumOfFirst = string.Format("{0:C}", sumofFirst);
			ViewBag.sumOfSecond = string.Format("{0:C}", sumOfSecond);
			ViewBag.sumOfThird = string.Format("{0:C}", sumOfThird);
			return pptqID;
		}


		public static int FillPODPdfHtml8(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
		{
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			ViewBag.partner = _partner;
			ViewBag.Supplyer = _partner.name;
			ViewBag.PurchaseOrderNumber = _partner.address1;
			ViewBag.PurchaseOrderValue = "$" + new Regex("/\\B(?=(\\d{3})+(?!\\d))/g").Replace(_partner.address2.Replace(",", ""), ",");
			ViewBag.PO_REVISION_NUMBER = _partner.phone;
			ViewBag.PartNumber = _partner.zipcode;
			ViewBag.PnDescription = _partner.dunsNumber;
			ViewBag.ChangeAmount = _partner.city;
			ViewBag.BuyerName = _partner.firstName + " " + _partner.lastName;
			ViewBag.ComplienceAnalist = _partner.title;
			ViewBag.GlobalSourcing = _partner.fax;
			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;
			//_signature
			ViewBag.signature = _signature;
			ViewBag.personTitle = _partner != null ? _partner.title : "";
			ViewBag.completeDate = pptq.completedDate != null ? pptq.completedDate.Value.ToString("MM/dd/yyyy") : "";
			//  var _PPTQQuestionResponse = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID).ToList();

			var _PPTQQuestionResponse = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseByPPTQ(pptqID).ToList();
			var _responseYES = 74;
			var _responseNO = 75;
			var _chacked = "checked";
			decimal sumofFirst = 0;
			decimal sumOfSecond = 0;
			decimal sumOfThird = 0;
			Regex codeRegex = new Regex("\\([A-Z][A-Z]\\)");

			foreach (var item in _PPTQQuestionResponse)
			{
				switch (item.question)
				{
					#region Question1
					case 24213:
						ViewBag.Q23675_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23675_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24123:
						switch (item.response)
						{
							case 46888:
								ViewBag.Q24812_FirmFixedPrice = "checked";
								break;
							case 46889:
								ViewBag.Q24812_CostReimbursement = "checked";
								break;
							case 46890:
								ViewBag.Q24812_Other = "checked";
								break;
							default: break;

						}
						ViewBag.Q24812_Comment = item.comment;
						break;
					case 24124:
						switch (item.response)
						{
							case 46891:
								ViewBag.Q23571_Yes = "checked";
								break;
							case 46892:
								ViewBag.Q23571_No = "checked";
								break;
							case 46893:
								ViewBag.Q23571_NA = "checked";
								break;
							default: break;
						}
						break;
					case 24125:
						switch (item.response)
						{
							case 46894:
								ViewBag.Q23572_46420 = _chacked;
								break;
							case 46895:
								ViewBag.Q23572_46421 = _chacked;
								break;
							case 46896:
								ViewBag.Q23572_46422 = _chacked;
								break;
							case 46897:
								ViewBag.Q23572_46423 = _chacked;
								break;
							default: break;

						}
						ViewBag.Q23572_Comment = item.comment;
						break;
					case 24126:
						ViewBag.Q23573_Yes = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24127:
						ViewBag.Q23574_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23574_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24128:
						ViewBag.Q23574_Comment = item.comment;
						break;
					case 24129:
						ViewBag.Q23576_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23576_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24130:
						ViewBag.Q23576_Comment = item.comment;
						break;
					case 24131:
						ViewBag.Q23578_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q23578_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24132:
						ViewBag.Q23578_Comment = item.comment;
						break;
					case 24133:
						ViewBag.Q24815_Comment = item.comment;
						switch (item.response)
						{
							case 46898:
								ViewBag.Q23580_46424 = _chacked;
								break;
							case 46899:
								ViewBag.Q23580_46425 = _chacked;
								break;
							case 46900:
								ViewBag.Q23580_46426 = _chacked;
								break;
							default: break;

						}
						break;

					case 24134:
						ViewBag.Q24816_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24816_DropDownValues_Response = item.response;
						break;

					case 24135:
						ViewBag.Q24817_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24817_DropDownValues_Response = item.response;
						ViewBag.Q24817_Comment = item.comment;
						break;
					case 24136:
						switch (item.response)
						{
							case 46916:
								ViewBag.Q24818_46072 = _chacked;
								break;
							case 46917:
								ViewBag.Q24818_46073 = _chacked;
								break;
							case 46918:
								ViewBag.Q24818_46074 = _chacked;
								break;
							default: break;
						}
						break;
					#endregion
					#region Section 2-3
					case 24142:
						ViewBag.Q24819_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24137:
						ViewBag.Q24820_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24138:
						ViewBag.Q24821_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24139:
						ViewBag.Q24822_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;


					case 24141:
						ViewBag.Q24824_Value = item.comment;
						switch (item.response)
						{
							case 46919:
								ViewBag.Q24825_Checked = _chacked;
								ViewBag.Q24824_Checked = _chacked;
								break;
							case 46920:
								ViewBag.Q24826_Checked = _chacked;
								ViewBag.Q24824_Checked = _chacked;
								break;
							case 46921:
								ViewBag.Q24827_Checked = _chacked;
								ViewBag.Q24824_Checked = _chacked;
								break;
							case 46922:
								ViewBag.Q24828_Checked = _chacked;
								ViewBag.Q24824_Checked = _chacked;
								break;
							case 46923:
								ViewBag.Q24829_Checked = _chacked;
								ViewBag.Q24824_Checked = _chacked;
								break;
							case 46924:
								ViewBag.Q24830_Checked = _chacked;
								ViewBag.Q24824_Checked = _chacked;
								break;
							default: break;
						}
						break;
					case 24140:
						ViewBag.Q24823_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24823_Comment = item.comment;
						break;
					case 24189:
						ViewBag.Q24831_Value = item.comment;
						break;

					case 24190:
						ViewBag.Q24832_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24191:
						ViewBag.Q24833_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24192:
						ViewBag.Q24834_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24193:
						ViewBag.Q24835_Value = item.comment;
						break;
					case 24194:
						ViewBag.Q24836_Value = item.comment;
						break;
					case 24195:
						ViewBag.Q24837_Value = item.comment;
						break;
					case 24196:
						ViewBag.Q24838_Value = item.comment;
						break;
					case 24197:
						ViewBag.Q24839_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24839_Comment = item.comment;
						break;
					case 24198:
						ViewBag.Q24840_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24199:
						ViewBag.Q24841_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24200:
						ViewBag.Q24842_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24201:
						ViewBag.Q24843_Checked = item.response == _responseYES ? _chacked : string.Empty;
						break;
					case 24202:
						ViewBag.Q24844_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24844_Comment = item.comment;
						break;
					case 24203:
						ViewBag.Q24845_Checked = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24845_Comment = item.comment;
						break;
					case 24204:
						ViewBag.Q24846_Comment = item.comment;
						break;
					#endregion



					#region Section 4
					case 24205:
						ViewBag.Q24847_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24847_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24206:
						ViewBag.Q24848_Value = item.comment;
						break;
					case 24207:
						ViewBag.Q24849_Value = item.comment;
						break;
					case 24208:
						ViewBag.Q24850_Value = item.comment;
						break;
					case 24209:
						ViewBag.Q24851_Value = item.comment;
						break;
					case 24210:
						ViewBag.Q24852_Value = item.comment;
						break;
					case 24211:
						ViewBag.Q24853_Value = item.comment;
						break;
					case 24212:
						ViewBag.Q24854_Value = item.comment;
						break;
					#endregion

					#region Section 5
					case 24147:
						ViewBag.Q24862_Value = item.comment;
						break;
					case 24161:
						ViewBag.Q24878_Value = item.comment;
						break;
					case 24175:
						ViewBag.Q23659_Value = item.comment;
						break;
					case 24148:
						ViewBag.Q24863_Value = item.comment;
						break;
					case 24162:
						ViewBag.Q24879_Value = item.comment;
						break;
					case 24176:
						ViewBag.Q24895_Value = item.comment;
						break;
					//Sum of 24149+24151+24153+24155+24158+24159
					case 24149:
						ViewBag.Q23629_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 24151:
						ViewBag.Q24867_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 24153:
						ViewBag.Q24870_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 24155:
						ViewBag.Q24873_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 24158:
						ViewBag.Q24875_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					case 24159:
						ViewBag.Q2424876_Value = item.comment;
						sumofFirst += GetNumber(item.comment);
						break;
					//Sum of 24163+24165+24167+24169+24172+24173
					case 24163:
						ViewBag.Q24880_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 24165:
						ViewBag.Q24883_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 24167:
						ViewBag.Q24886_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 24169:
						ViewBag.Q24889_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 24172:
						ViewBag.Q24891_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					case 24173:
						ViewBag.Q2424892_Value = item.comment;
						sumOfSecond += GetNumber(item.comment);
						break;
					//Sum of 24177+24179+24181+24183+24186+24187
					case 24177:
						ViewBag.Q24896_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 24179:
						ViewBag.Q24899_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 24181:
						ViewBag.Q24902_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 24183:
						ViewBag.Q24905_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 24186:
						ViewBag.Q24907_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					case 24187:
						ViewBag.Q2424908_Value = item.comment;
						sumOfThird += GetNumber(item.comment);
						break;
					//0.1
					case 24150:
						ViewBag.Q24865_Value = item.comment;
						break;
					case 24143:
						ViewBag.Q24855_Value = item.comment;
						break;
					case 24164:
						ViewBag.Q24881_Value = item.comment;
						break;
					case 24178:
						ViewBag.Q24897_Value = item.comment;
						break;

					//0.2
					case 24144:
						ViewBag.Q24857_Value = item.comment;
						break;
					case 24152:
						ViewBag.Q24868_Value = item.comment;
						break;
					case 24166:
						ViewBag.Q24884_Value = item.comment;
						break;
					case 24180:
						ViewBag.Q24900_Value = item.comment;
						break;

					//0.3
					case 24145:
						ViewBag.Q24859_Value = item.comment;
						break;
					case 24154:
						ViewBag.Q24871_Value = item.comment;
						break;
					case 24168:
						ViewBag.Q24887_Value = item.comment;
						break;
					case 24182:
						ViewBag.Q24903_Value = item.comment;
						break;

					//0.4
					case 24146:
						ViewBag.Q24861_Value = item.comment;
						break;
					case 24156:
						ViewBag.Q24874_Value = item.comment;
						break;
					case 24170:
						ViewBag.Q24890_Value = item.comment;
						break;
					case 24184:
						ViewBag.Q24906_Value = item.comment;
						break;


					case 24188:
						ViewBag.Q24909_Value = item.comment;
						break;


					//section 6
					case 24240:
						ViewBag.Q24917_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24917_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24241:
						ViewBag.Q24918_Value = item.comment;
						break;
					case 24242:
						ViewBag.Q24919_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24919_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24243:
						ViewBag.Q24920_Value = item.comment;
						break;
					case 24244:
						ViewBag.Q24921_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24921_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24245:
						ViewBag.Q24922_Value = item.comment;
						break;
					case 24225:
						ViewBag.Q24923_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24923_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24227:
						ViewBag.Q24925_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24925_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24228:
						ViewBag.Q24926_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24926_DropDownValues_Response = item.response;
						break;
					case 24229:
						ViewBag.Q24927_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24927_No = item.response == _responseNO ? _chacked : string.Empty;
						break;

					case 24230:
						ViewBag.Q24928_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24928_DropDownValues_Response = item.response;
						break;
					case 24231:
						ViewBag.Q24929_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24929_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24232:
						ViewBag.Q24930_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24930_DropDownValues_Response = item.response;
						break;
					case 24233:
						ViewBag.Q24931_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24931_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24234:
						ViewBag.Q24932_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24932_DropDownValues_Response = item.response;
						break;
					case 24235:
						ViewBag.Q24933_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24933_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24236:
						ViewBag.Q24934_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24934_DropDownValues_Response = item.response;
						break;
					case 24237:
						ViewBag.Q24935_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24935_No = item.response == _responseNO ? _chacked : string.Empty;
						break;
					case 24238:
						ViewBag.Q24936_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24936_DropDownValues_Response = item.response;
						break;
					case 24239:
						ViewBag.Q24937_DropDownValues = new SelectList(db.pr_getResponseByQuestion(item.question).ToList().Select(o => new { description = codeRegex.Replace(o.description, ""), id = o.id }), "id", "description", item.response);
						ViewBag.Q24937_DropDownValues_Response = item.response;
						ViewBag.Q24937_Value = item.comment;
						break;
					case 24224:
						ViewBag.Q24938_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24938_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24939_Value = item.comment;
						break;
					#endregion
					#region 77
					case 24217:
						switch (item.response)
						{
							case 46931:
								ViewBag.Q24940_46123 = _chacked;
								break;
							case 46932:
								ViewBag.Q24940_46124 = _chacked;
								break;
							case 46933:
								ViewBag.Q24940_46125 = _chacked; break;
							default: break;

						}
						break;
					case 24218:
						ViewBag.Q23681_Comment = item.comment;
						break;
					case 24219:
						ViewBag.Q24941_46126 = item.response == 46934 ? _chacked : string.Empty;
						ViewBag.Q24941_46127 = item.response == 46935 ? _chacked : string.Empty;
						ViewBag.Q24941_comment = item.comment;
						break;
					case 24220:
						ViewBag.Q24942_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24942_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24942_Comment = item.comment;
						break;
					case 24221:
						ViewBag.Q24943_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24943_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24943_Comment = item.comment;
						break;
					case 24222:
						switch (item.response)
						{
							case 46936:
								ViewBag.Q24944_46128 = _chacked;
								break;
							case 46937:
								ViewBag.Q24944_46129 = _chacked;
								break;
							case 46938:
								ViewBag.Q24944_46130 = _chacked; break;
							default: break;

						}
						ViewBag.Q24944_Comment = item.comment;
						break;
					case 24223:
						ViewBag.Q24945_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24945_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24945_Comment = item.comment;
						break;
					case 24215:
						ViewBag.Q24946_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24946_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24946_Comment = item.comment;
						break;
					case 24216:
						ViewBag.Q24947_Yes = item.response == _responseYES ? _chacked : string.Empty;
						ViewBag.Q24947_No = item.response == _responseNO ? _chacked : string.Empty;
						ViewBag.Q24947_Comment = item.comment;
						break;
					case 24246:
						ViewBag.Q23709_Yes = item.response == _responseYES ? _chacked : string.Empty;
						break;
					#endregion
					default: break;
				}
			}
			ViewBag.sumOfFirst = sumofFirst;
			ViewBag.sumOfSecond = sumOfSecond;
			ViewBag.sumOfThird = sumOfThird;
			return pptqID;
		}

		public static int FillMOOGPdfHtml(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
		{
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.accessCode = accessCode;
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;
			eSignature objeSignature =
					db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptqID).FirstOrDefault();
			var state = db.pr_getState(_partner.state ?? -1).FirstOrDefault();
			ViewBag.Offerer = _partner.FullName;
			ViewBag.Date = pptq.completedDate;
			ViewBag.Address1 = _partner.address1 + " " + _partner.address2 + ",";
			ViewBag.Address2 = _partner.city + ", " + (state != null ? state.name : "") + ", " + _partner.zipcode;
			if (objeSignature != null)
			{
				ViewBag.eSig_Email = objeSignature.email;
				ViewBag.eSig_Date = objeSignature.completeDate;
				ViewBag.eSig_Name = _partner.firstName + " " + _partner.lastName;
				ViewBag.eSig_Title = _partner.title;
				ViewBag.eSig_Company = _partner.name;
				ViewBag.eSig_phone = _partner.phone;

			}
			//  var _PPTQQuestionResponse = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID).ToList();

			var _PPTQQuestionResponse = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseByPPTQ(pptqID).ToList();
			var _responseYES = 74;
			var _responseNO = 75;
			var _chacked = "checked";
			Regex codeRegex = new Regex("\\([A-Z][A-Z]\\)");

			foreach (var item in _PPTQQuestionResponse)
			{
				switch (item.question)
				{
					case 22111:
						ViewBag.Value_22111 = item.response == _responseYES ? _chacked : "";
						if (!string.IsNullOrEmpty(ViewBag.Value_22111))
							ViewBag.Value_22111_comment = item.comment;
						break;
					case 22112:
						ViewBag.Value_22112 = item.response == _responseYES ? _chacked : "";
						if (!string.IsNullOrEmpty(ViewBag.Value_22112))
							ViewBag.Value_22112_comment = item.comment;
						break;
					case 22113:
						ViewBag.Value_22113 = item.response == _responseYES ? _chacked : "";
						break;
					case 22114:
						//var wasChecked = false;
						if (string.IsNullOrEmpty(ViewBag.Value_22112) && string.IsNullOrEmpty(ViewBag.Value_22113))
						{
							switch (item.response)
							{
								case 45286:
									ViewBag.Value_22114_45286 = _chacked;
									//wasChecked = true;
									break;
								case 45287:
									ViewBag.Value_22114_45287 = _chacked;
									//wasChecked = true;
									break;
								case 45288:
									ViewBag.Value_22114_45288 = _chacked;
									//wasChecked = true;
									break;
								case 45289:
									ViewBag.Value_22114_45289 = _chacked;
									//wasChecked = true;
									break;
								default:

									break;
							}

							ViewBag.Value_22114 = _chacked;
						}
						//ViewBag.Value_22114_Comment = item.comment;
						break;
					case 22115:
						switch (item.response)
						{
							case 45290:
								ViewBag.Value_22115_45290 = _chacked;
								break;
							case 45291:
								ViewBag.Value_22115_45291 = _chacked;
								break;
							case 45292:
								ViewBag.Value_22115_45292 = _chacked;
								break;
							case 45293:
								ViewBag.Value_22115_45293 = _chacked;
								break;
							case 45294:
								ViewBag.Value_22115_45294 = _chacked;
								break;
							case 45295:
								ViewBag.Value_22115_45295 = _chacked;
								break;
							case 45296:
								ViewBag.Value_22115_45296 = _chacked;
								break;
							//case 4529:
							//	ViewBag.Value_22114_45296 = _chacked;
							//	break;
						}
						ViewBag.Value_22115 = item.comment;
						break;
					case 22116:
						ViewBag.Value_22116 = item.response == _responseNO ? _chacked : "";
						ViewBag.Value_22116_Name = item.response == _responseYES ? _chacked : "";
						break;
					case 22117:
						ViewBag.Value_22117 = item.comment;
						break;
					case 22118:
						ViewBag.Value_22118 = item.comment;
						break;
					case 22119:
						ViewBag.Value_22119 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22119_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22120:
						ViewBag.Value_22120 = item.comment;
						//ViewBag.Value_22119_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22121:

						var responseObj = db.pr_getResponse(item.response ?? -1).FirstOrDefault();
						if (responseObj != null)
						{
							ViewBag.Value_22121 = codeRegex.Replace(responseObj.description, "");
						}
						//ViewBag.Value_22119_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22122:
						ViewBag.Value_22122 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22122_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22128:
						ViewBag.Value_22128 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22128_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22123:
						ViewBag.Value_22123 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22123_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22124:
						ViewBag.Value_22124 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22124_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22125:
						ViewBag.Value_22125 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22125_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22126:
						ViewBag.Value_22126 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22126_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22127:
						ViewBag.Value_22127 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22127_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22129:
						ViewBag.Value_22129 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22129_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22130:
						ViewBag.Value_22130 = item.response == _responseYES ? _chacked : "";

						break;
					case 22131:
						ViewBag.Value_22131 = item.response == _responseYES ? _chacked : "";

						break;
					case 22132:
						ViewBag.Value_22132 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22132_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22133:
						ViewBag.Value_22133 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22133_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22134:
						ViewBag.Value_22134 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22134_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22135:
						ViewBag.Value_22135 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22135_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22137:
						ViewBag.Value_22137 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22137_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22138:
						ViewBag.Value_22138 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22138_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22139:
						ViewBag.Value_22139 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22139_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22140:
						ViewBag.Value_22140 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22140_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22141:
						ViewBag.Value_22141 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22141_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22143:
						ViewBag.Value_22143 = item.comment;
						//ViewBag.Value_22141_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22145:
						switch (item.response)
						{
							case 45310:
								ViewBag.Value_22145_45310 = _chacked;
								break;
							case 45311: ViewBag.Value_22145_45311 = _chacked; break;
							case 45312: ViewBag.Value_22145_45312 = _chacked; break;
							case 45313: ViewBag.Value_22145_45313 = _chacked; break;
							case 45314: ViewBag.Value_22145_45314 = _chacked; break;
							case 45315: ViewBag.Value_22145_45315 = _chacked; break;
						}
						break;
				}
			}
			return pptqID;
		}


		public static int FillMOOGPdfHtml6(dynamic ViewBag, EntitiesDBContext db, HttpSessionStateBase Session, HttpServerUtilityBase Server)
		{
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var _partnerHeader = db.pr_getPartnerHeaderByAccessCode(accessCode).ToList();
			ViewBag.accessCode = accessCode;
			ViewBag.partnerHeader = _partnerHeader;
			List<enterprise> enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
			var partnerId = pptq != null ? pptq.partner : -1;
			eSignature _signature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq != null ? pptq.id : -1).FirstOrDefault();
			var _partner = db.pr_getPartner(partnerId).FirstOrDefault();
			var _questionnaire = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			var partnerTouchPoint = _partner != null ? _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault() : null;
			var pptqID = partnerTouchPoint != null ? partnerTouchPoint.id : -1;
			eSignature objeSignature =
					db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptqID).FirstOrDefault();
			var state = db.pr_getState(_partner.state ?? -1).FirstOrDefault();
			ViewBag.Offerer = _partner.FullName;
			ViewBag.Date = pptq.completedDate;
			ViewBag.Address1 = _partner.address1 + " " + _partner.address2 + ",";
			ViewBag.Address2 = _partner.city + ", " + (state != null ? state.name : "") + ", " + _partner.zipcode;
			if (objeSignature != null)
			{
				ViewBag.eSig_Email = objeSignature.email;
				ViewBag.eSig_Date = objeSignature.completeDate;
				ViewBag.eSig_Name = _partner.firstName + " " + _partner.lastName;
				ViewBag.eSig_Title = _partner.title;
				ViewBag.eSig_Company = _partner.name;
				ViewBag.eSig_phone = _partner.phone;

			}
			//  var _PPTQQuestionResponse = db.pr_getPPTQQuestionResponseByQuestionnaire(pptqID).ToList();

			var _PPTQQuestionResponse = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseByPPTQ(pptqID).ToList();
			var _responseYES = 74;
			var _responseNO = 75;
			var _chacked = "checked";
			Regex codeRegex = new Regex("\\([A-Z][A-Z]\\)");

			foreach (var item in _PPTQQuestionResponse)
			{
				switch (item.question)
				{
					case 22826:
						ViewBag.Value_22111 = item.response == _responseYES ? _chacked : "";
						if (!string.IsNullOrEmpty(ViewBag.Value_22111))
							ViewBag.Value_22111_comment = item.comment;
						break;
					case 22827:
						ViewBag.Value_22112 = item.response == _responseYES ? _chacked : "";
						if (!string.IsNullOrEmpty(ViewBag.Value_22112))
							ViewBag.Value_22112_comment = item.comment;
						break;
					case 22828:
						ViewBag.Value_22113 = item.response == _responseYES ? _chacked : "";
						break;
					case 22829:
						//var wasChecked = false;
						if (string.IsNullOrEmpty(ViewBag.Value_22112) && string.IsNullOrEmpty(ViewBag.Value_22113))
						{
							switch (item.response)
							{
								case 45815:
									ViewBag.Value_22114_45286 = _chacked;
									//wasChecked = true;
									break;
								case 45816:
									ViewBag.Value_22114_45287 = _chacked;
									//wasChecked = true;
									break;
								case 45817:
									ViewBag.Value_22114_45288 = _chacked;
									//wasChecked = true;
									break;
								case 45818:
									ViewBag.Value_22114_45289 = _chacked;
									//wasChecked = true;
									break;
								default:

									break;
							}

							ViewBag.Value_22114 = _chacked;
						}
						//ViewBag.Value_22114_Comment = item.comment;
						break;
					case 22830:
						switch (item.response)
						{
							case 45819:
								ViewBag.Value_22115_45290 = _chacked;
								break;
							case 45820:
								ViewBag.Value_22115_45291 = _chacked;
								break;
							case 45821:
								ViewBag.Value_22115_45292 = _chacked;
								break;
							case 45822:
								ViewBag.Value_22115_45293 = _chacked;
								break;
							case 45823:
								ViewBag.Value_22115_45294 = _chacked;
								break;
							case 45824:
								ViewBag.Value_22115_45295 = _chacked;
								break;
							case 45825:
								ViewBag.Value_22115_45296 = _chacked;
								break;
							//case 4529:
							//	ViewBag.Value_22114_45296 = _chacked;
							//	break;
						}
						ViewBag.Value_22115 = item.comment;
						break;
					case 22831:
						ViewBag.Value_22116 = item.response == _responseNO ? _chacked : "";
						ViewBag.Value_22116_Name = item.response == _responseYES ? _chacked : "";
						break;
					case 22117:
						ViewBag.Value_22117 = item.comment;
						break;
					case 22118:
						ViewBag.Value_22118 = item.comment;
						break;
					case 22834:
						ViewBag.Value_22119 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22119_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22120:
						ViewBag.Value_22120 = item.comment;
						//ViewBag.Value_22119_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22836:

						var responseObj = db.pr_getResponse(item.response ?? -1).FirstOrDefault();
						if (responseObj != null)
						{
							ViewBag.Value_22121 = codeRegex.Replace(responseObj.description, "");
						}
						//ViewBag.Value_22119_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22837:
						ViewBag.Value_22122 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22122_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22843:
						ViewBag.Value_22128 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22128_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22838:
						ViewBag.Value_22123 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22123_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22839:
						ViewBag.Value_22124 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22124_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22840:
						ViewBag.Value_22125 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22125_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22841:
						ViewBag.Value_22126 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22126_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22842:
						ViewBag.Value_22127 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22127_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22844:
						ViewBag.Value_22129 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22129_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22845:
						ViewBag.Value_22130 = item.response == _responseYES ? _chacked : "";

						break;
					case 22846:
						ViewBag.Value_22131 = item.response == _responseYES ? _chacked : "";

						break;
					case 22847:
						ViewBag.Value_22132 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22132_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22848:
						ViewBag.Value_22133 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22133_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22849:
						ViewBag.Value_22134 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22134_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22850:
						ViewBag.Value_22135 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22135_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22852:
						ViewBag.Value_22137 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22137_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22853:
						ViewBag.Value_22138 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22138_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22854:
						ViewBag.Value_22139 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22139_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22855:
						ViewBag.Value_22140 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22140_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22856:
						ViewBag.Value_22141 = item.response == _responseYES ? _chacked : "";
						ViewBag.Value_22141_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22858:
						ViewBag.Value_22143 = item.comment;
						//ViewBag.Value_22141_oposite = item.response == _responseNO ? _chacked : "";
						break;
					case 22860:
						switch (item.response)
						{
							case 45838:
								ViewBag.Value_22860_45838 = _chacked;
								break;
							case 45839:
								ViewBag.Value_22145_45310 = _chacked;
								break;
							case 45840: ViewBag.Value_22145_45311 = _chacked; break;
							case 45841: ViewBag.Value_22145_45312 = _chacked; break;
							case 45842: ViewBag.Value_22145_45313 = _chacked; break;
							case 45843: ViewBag.Value_22145_45314 = _chacked; break;
							case 45844: ViewBag.Value_22145_45315 = _chacked; break;
						}
						break;
					case 22861:
						ViewBag.Value_22861 = item.response == _responseYES ? _chacked : "";
						break;
					case 22862:
						ViewBag.Value_22862 = item.response == _responseYES ? _chacked : "";
						break;
					case 22863:
						ViewBag.Value_22863 = item.response == _responseYES ? _chacked : "";
						break;
					case 22864:
						ViewBag.Value_22864 = item.response == _responseYES ? _chacked : "";
						break;
					case 22865:
						ViewBag.Value_22865 = item.response == _responseYES ? _chacked : "";
						break;
					case 22866:
						ViewBag.Value_22866 = item.response == _responseYES ? _chacked : "";
						break;
					case 22867:
						ViewBag.Value_22867 = item.response == _responseYES ? _chacked : "";
						break;
					case 22868:
						ViewBag.Value_22868 = item.response == _responseYES ? _chacked : "";
						break;
				}
			}
			return pptqID;
		}
		public ActionResult TestPODPage()
		{
			var pptqID = FillPODPdfHtml9(ViewBag, db, Session, Server);
			return View("PODQuestionnaireSurveyPdfDownload9");
		}
		public ActionResult TestMOOGPage()
		{
			var pptqID = FillMOOGPdfHtml(ViewBag, db, Session, Server);
			return View("MoogCustomizedQuestionnaireSurveyPdfDownload6");
		}

		protected ActionResult ViewCustomizedPdf(int pptqID, string ViewName)
		{
			string htmltext = this.RenderActionResultToString(this.View(ViewName));  //name of the view...
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			var question = db.pr_getQuestionnaireByAccesscode(accessCode).FirstOrDefault();
			if (question != null && question.footer == "4")
			{
				htmltext = htmltext.Replace("Honeywell", "Moog").Replace("honeywell", "moog").Replace("HONEYWELL", "MOOG");
			}
			string PDF_FileName = "HON_" + accessCode.Substring(1, 4) + ".pdf";

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
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			string PDF_FileName = "HON_" + accessCode.Substring(1, 4) + ".pdf";

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
			string accessCode = Session["accessCode"] != null ? Session["accessCode"].ToString() : "";
			if (!String.IsNullOrEmpty(accessCode))
			{
				var _pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
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
				var prGetQuestionnaireByAccesscodeResult = db.pr_getQuestionnaireByAccesscode(accesscode).FirstOrDefault();
				if (prGetQuestionnaireByAccesscodeResult != null && prGetQuestionnaireByAccesscodeResult.footer != "1")
					Response.Redirect("~/Registration/Home/CustomizedPDFConfirmation");
				else Response.Redirect("~/Registration/Home/PDFConfirmation");
			}
			return RedirectToAction("~/Registration/Home");
		}

		public ActionResult GetSubjectsByQuestion(int question)
		{
			//270740JR
			return Json(db.pr_getQuestionResponseNarrativeSelectionListByQuestion(question).ToList(), JsonRequestBehavior.AllowGet);

		}
	}
}
