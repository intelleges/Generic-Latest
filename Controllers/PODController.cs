using Generic.Helpers;
using Generic.Helpers.PartnerHelper;
using Generic.Helpers.Utility;
using Generic.Models;
using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
	public class PODController : Controller
	{

		private EntitiesDBContext db = new EntitiesDBContext();
		// GET: POD
		public ActionResult Create()
		{
			partner model = new partner();
			if (Request.Cookies["supplierName"] != null)
			{
				model.name = Request.Cookies["supplierName"].Value;
				var c = new HttpCookie("supplierName");
				c.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(c);
			}

			if (Request.Cookies["supplierNumber"] != null)
			{
				model.internalID = Request.Cookies["supplierNumber"].Value;
				var c = new HttpCookie("supplierNumber");
				c.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(c);
			}

			if (Request.Cookies["buyerFirstName"] != null)
			{
				model.firstName = Request.Cookies["buyerFirstName"].Value;
				var c = new HttpCookie("buyerFirstName");
				c.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(c);
			}

			if (Request.Cookies["buyerLastName"] != null)
			{
				model.lastName = Request.Cookies["buyerLastName"].Value;
				var c = new HttpCookie("buyerLastName");
				c.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(c);
			}

			if (Request.Cookies["buyerEmail"] != null)
			{
				model.email = Request.Cookies["buyerEmail"].Value;
				var c = new HttpCookie("buyerEmail");
				c.Expires = DateTime.Now.AddDays(-1);
				Response.Cookies.Add(c);
			}


			GenerateCreateDropDownLists();
			return View(model);
		}


		public ActionResult FindPODS()
		{
			ViewBag.partnerStatus = new SelectList(db.pr_getPartnerStatusAll(), "id", "description");
			return View();
		}

		[HttpPost]

		public ActionResult FindPODS(FindPODSViewModel model)
		{
			ViewBag.partnerStatus = new SelectList(db.pr_getPartnerStatusAll(), "id", "description");

			string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

			arguments += "touchpointID=4226;";
			arguments += "partnertypeID=250;";

			if (model.partnerStatus != null)
				arguments += "StatusID=" + model.partnerStatus + ";";
			if (model.SupplierNumber != "")
				arguments += "dunsNumber=" + model.SupplierNumber + ";";
			if (model.PoNumber != "")
				arguments += "address=" + model.PoNumber + ";";
			if (model.SupplierName != "")
				arguments += "PartnerName=" + model.SupplierName + ";";
			if (model.PoVersion != "")
				arguments += "phone=" + model.PoVersion + ";";
			if (model.AccessCode != "")
				arguments += "accesscode=" + model.AccessCode + ";";
			if (model.BuyerEmail != "")
				arguments += "ContactEmail=" + model.BuyerEmail + ";";
			if (model.PartNumber != "")
				arguments += "zipcode=" + model.PartNumber + ";";

			Session["podssearch"] = arguments;

			return RedirectToAction("FindPODSResult");
		}

		public ActionResult FindPODSResult()
		{
			try
			{
				string arguments = Session["podssearch"].ToString() + "active=1;";
				Session["partner"] = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
				List<view_PartnerData> abc = (List<view_PartnerData>)Session["partner"];
				return View(abc);
			}
			catch
			{
				return RedirectToAction("FindPODS");

			}
		}

		protected void GenerateCreateDropDownLists()
		{
			ViewBag.state = new SelectList(db.state, "id", "name");
			ViewBag.country = new SelectList(db.country, "id", "name");
			ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");
			ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).Take(1).ToList(), "id", "name");
			ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname");
			ViewBag.owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
		}

		public ActionResult NarrativeEmailPopup(int id, string accessCode)
		{
			ViewBag.Id = id;
			ViewBag.AccessCode = accessCode;
			ViewBag.DropDownSubjects = new SelectList(db.pr_getAutomailMessageAllByPPTQ(id).ToList(), "id", "subject").ToList();

			ViewBag.IsShowGetRemoveBtns = db.pr_getNarrativePPTQ(id).FirstOrDefault() == null ? false : true;
			return View();
		}

		public ActionResult GetAutomail(int id, int pptq)
		{
			return Json(db.pr_getAutomailMessageAllByPPTQ(pptq).ToList().FirstOrDefault(o => o.id == id), JsonRequestBehavior.AllowGet);
		}

		[HttpPost, ValidateInput(false)]
		public ActionResult EmailSend(string accessCode, string subject, string text, HttpPostedFileBase attachment, int? autoMailId, bool? ccSender)
		{
			var error = "";
			EmailFormat formatter = new EmailFormat();
			var currentPerson = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
			var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();

			var resultBody = formatter.sGetEmailBody(text, null, pptq.partner1, pptq.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, pptq.partnerTypeTouchpointQuestionnaire1.touchpoint1, pptq.partnerTypeTouchpointQuestionnaire1.id);

			var email = 
				new Email()
				{
					accesscode = accessCode,
					emailTo = pptq.partner1.email,
					subject = subject,
					url = Request.Url.ToString(),
					body = resultBody,
					protocolTouchpoint = pptq.partnerTypeTouchpointQuestionnaire1.touchpoint1.description,
					category  = SendGridCategory.EmailSend
				};

			if (autoMailId.HasValue)
				email.automailMessage = autoMailId.Value.ToString();

			SchedulerServiceHelper.sendEmail(email, new System.Net.Mail.MailAddress(currentPerson.email, currentPerson.FullName), false, Request.Files);

			/*db.pr_addEventNotification(pptq.partner1.email, DateTime.Now, null, null, email.url, ((int)email.category).ToString(), pptq.accesscode, pptq.partnerTypeTouchpointQuestionnaire1.touchpoint1.description, "MVCMT", null, autoMailId, pptq.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1.id, null).FirstOrDefault();*/
			return Json(error);
		}

		public ActionResult RemoveNarrative(int pptq)
		{
			db.pr_removeNarrativePPTQ(pptq);
			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult BaseNarrative(int pptq)
		{
			string content = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseCommentByPPTQ(pptq).FirstOrDefault();
			return Json(new { success = true, content = content }, JsonRequestBehavior.AllowGet);
		}

		public ActionResult GetNarrative(int pptq)
		{
			var content = db.pr_getNarrativePPTQ(pptq).FirstOrDefault();
			return Json(new { success = true, content = content != null ? content.narrative : "" }, JsonRequestBehavior.AllowGet);
		}

		[HttpPost]
		[ValidateInput(false)]
		public ActionResult SaveNarrative(int pptq, string text)
		{

			var content = db.pr_getNarrativePPTQ(pptq).FirstOrDefault();
			if (content == null)
				db.pr_addNarrativePPTQ(pptq, text);
			else db.pr_modifyNarrativePPTQ(pptq, text);


			return Json(new { success = true }, JsonRequestBehavior.AllowGet);
		}


		[HttpPost]
		public ActionResult Create(partner partner, int? protocol, int? partnertype, int? touchpoint, int? group, DateTime? DueDate)
		{
			List<Tuple<int, string>> uploadedpartners = new List<Tuple<int, string>>();

			string loadGroup = db.pr_getAccesscode().FirstOrDefault();
			partner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
			GenerateCreateDropDownLists();
			try
			{

				var o = db.pr_partnerAddDuplicateCheck(partner.address1 + " " + partner.phone, partner.email, partnertype, touchpoint).FirstOrDefault();

				if (o != null) {
					ViewBag.Message = "duplicates";
					ViewBag.MessageDuplicates = "This PO " + partner.address1 + " and Version # " + partner.phone + "  have already been entered. Please check this accesscode <a href='" + Url.Content("~/Registration?Accesscode=" + o) + "'>" + o + "</a>.";
					ModelState.Clear();
					return View();
				}


				int? PartnerId = (int)db.pr_addPartnerSpreadsheetDataLoad(partner.address1 + " " + partner.phone, partner.dunsNumber, partner.internalID, partner.name, partner.address1, partner.address2, partner.city, "", partner.fax ?? "", "", partner.firstName, partner.lastName, partner.dunsNumber, "", partner.email, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, partnertype, touchpoint, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, (int)PartnerStatus.Loaded, loadGroup, DueDate, group).ToList().FirstOrDefault();

				uploadedpartners.Add(new Tuple<int, string>(int.Parse(PartnerId.ToString()), ""));
				Session["uploadedpartnerList"] = uploadedpartners;
				Session["partnertype"] = partnertype;
				Session["touchpoint"] = touchpoint;
				Session["loadGroup"] = loadGroup;
				//    var Target = db.touchpoint.Where(x => x.id == touchpoint).Select(x => x.target).ToList();
				//   ViewBag.Message = Target[0].ToString();
				//ViewBag.Message = "1";

				var Target = db.touchpoint.Where(x => x.id == (touchpoint)).ToList();
				ViewBag.Message = Target[0].target.ToString();
				if (Target[0].target.ToString() == "2")
				{
					ViewBag.MessageDetail = "Congratulations, you just added  " + partner.name + " to " + Target[0].title;
				}
				ModelState.Clear();
				return View();
			}
			catch (Exception ex)
			{

				ViewBag.Message = "error";
				ViewBag.MessageDetail = ex.ToString();
				//  alertify-ok"
				//ViewBag.state = new SelectList(db.state.ToList(), "id", "name", partner.state);
				//ViewBag.country = new SelectList(db.country.ToList(), "id", "name", partner.country);
				//ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name", protocol);
				//ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name", partnertype);
				//ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).ToList(), "id", "name", group);
				//ViewBag.owner = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname", partner.owner);
				//ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname", partner.author);
				return View(partner);
			}


		}

		public ActionResult GetPartnerTypes(int id)
		{
			return Json(db.pr_getPartnertypeByTouchpoint(id).ToList(), JsonRequestBehavior.AllowGet);
		}
	}
}