using Generic.Helpers;
using Generic.Helpers.PartnerHelper;
using Generic.Models;
using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class FRPJController : Controller
    {
		private EntitiesDBContext db = new EntitiesDBContext();
        // GET: FRPJ
		public ActionResult Create()
        {
			GenerateCreateDropDownLists();
            return View();
        }

		[HttpPost]
		public ActionResult Create(FRPJModel model)
		{
			GenerateCreateDropDownLists();
			string loadGroup = db.pr_getAccesscode().FirstOrDefault();
			var defaultPartnerType = 223;
			try
			{

				int? PartnerId = (int)db.pr_addPartnerSpreadsheetDataLoad(model.ContractNumber, "", model.DunsNumber, model.SuplierName, "", "", "", "", model.CMEmail, "", model.CMFirstName, model.CMLastName, "", "", model.AmendmentNumber, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, defaultPartnerType, model.Touchpoint, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, (int)PartnerStatus.Loaded, loadGroup, model.Date, model.Group).ToList().FirstOrDefault();
				var partner = db.pr_getPartnerByEmailAndInternalID(Generic.Helpers.CurrentInstance.EnterpriseID, model.AmendmentNumber, model.ContractNumber).FirstOrDefault();
				
				if (partner != null)
				{
					using (var innerDb = new EntitiesDBContext())
					{
						var pptq = innerDb.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partner.id).FirstOrDefault();
						var partnerFromDb = innerDb.partner.FirstOrDefault(o => o.id == partner.id);
						partnerFromDb.federalID = model.EstimatedUSDAnnualForcast;
						partnerFromDb.fax = model.EstimatedUSDTotalForcast;
						partnerFromDb.owner = model.Owner;
						partnerFromDb.dateApproved = model.ContractStartDate;
						pptq.invitedDate = model.ContractExpirationDate ?? DateTime.Now;
						innerDb.Entry(pptq).State = System.Data.Entity.EntityState.Modified;
						innerDb.Entry(partnerFromDb).State = System.Data.Entity.EntityState.Modified;
						innerDb.SaveChanges();
					}
				}
				
				ModelState.Clear();
				ViewBag.Message = "Success";
				ViewBag.MessageDetail = HttpUtility.JavaScriptStringEncode("Created successfully");
				return View();
			}
			
			catch (Exception ex)
			{
				ViewBag.Message = "error";
				ViewBag.MessageDetail = HttpUtility.JavaScriptStringEncode(ex.ToString());
				return View(model);
			}
			return View();
		}
		protected void GenerateCreateDropDownLists()
		{
			//ViewBag.state = new SelectList(db.state, "id", "name");
			//ViewBag.country = new SelectList(db.country, "id", "name");
			ViewBag.Protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).Where(o => o.id == 2157).ToList(), "id", "name");
			ViewBag.Group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).ToList(), "id", "name");
			ViewBag.Touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");
			ViewBag.Owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
		}
    }
}