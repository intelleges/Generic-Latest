using Generic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
namespace Generic.Controllers
{
    public class LCEController : Controller
    {
		private EntitiesDBContext db = new EntitiesDBContext();
        // GET: LCE
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Create()
		{
			GenerateOwner();
			return View();
		}
		private void GenerateOwner()
		{
			ViewBag.Owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
		}

		[HttpPost]
		public ActionResult Create(LCEModel model)
		{
			GenerateOwner();
			if (ModelState.IsValid)
			{
				
				var result = db.pr_getLCE_Special_Data(model.Owner, model.Designation, model.ProgramName, model.Duedate).FirstOrDefault();
				string loadGroup = db.pr_getAccesscode().FirstOrDefault();
				Session["partnertype"] = result.partnertype;
				Session["touchpoint"] = result.touchpoint;
				Session["loadGroup"] = loadGroup;
				var f = result.partner_country;
				var partnerId = db.pr_addPartnerSpreadsheetDataLoad(result.partner_internal_id, result.partner_sap_id, result.partner_duns_number, result.partner_name, result.partner_address_one, result.partner_address_two, result.partner_city, result.partner_state.ToString(), result.partner_zipcode, result.partner_country.ToString(), result.partner_poc_first_name, result.partner_poc_last_name, result.partner_poc_title, result.partner_poc_phone_number, result.partner_poc_email_address, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, result.partnertype, result.touchpoint, result.person, result.partnerSpreadsheetDataLoadStatus, loadGroup, result.dueDate, result.group).FirstOrDefault();
				var Target = db.touchpoint.Where(x => x.id == result.touchpoint).ToList();
				var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).FirstOrDefault();
				ViewBag.Message = Target[0].target.ToString();
				if (Target[0].target.ToString() == "2")
				{
					ViewBag.MessageDetail = "Congratulations, you just added  " + result.partner_name + " to " + Target[0].title;
				}
				return RedirectToAction("CreateResult");
			}
			return View(model);
		}

		public ActionResult CreateResult()
		{
			var testId = 117226;
			if(Session["loadGroup"]!=null)
			{
				var pptqO = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(Session["loadGroup"].ToString()).FirstOrDefault();
				var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(pptqO.accesscode).FirstOrDefault();
				var data = db.pr_getPPTQTeamRacixByPPTQ_Grid(testId).ToList();
				return View(data.Select(o=>new pr_getPPTQTeamRacixByPPTQ_Grid_Result(){
					eXcluded = o.eXcluded,
					questionDesc = o.questionDesc,
					sectionDesc = o.sectionDesc,
					Responsible_Edit_ = o.Responsible_Edit_,
					Accountable_Approve_ = o.Accountable_Approve_,
					Consulted__Approval_Alerts_ = o.Consulted__Approval_Alerts_,
					Informed__All_Alerts_ = o.Informed__All_Alerts_
				}));
			}
			return View();
		}

		public ActionResult Users()
		{
			return Json(db.pr_getPersonAll(1066).Select(o=>o.email+" "+o.firstName+" "+o.lastName));
		}
    }
}