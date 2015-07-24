using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Helpers.Utility;
using Generic.Helpers;
using Generic.Models;
using LinqToExcel;

namespace Generic.Controllers
{
    public class ContractController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        // GET: Contract
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            this.GetDataForLists();
            return View();
        }

         [HttpPost]
        public ActionResult Create(Generic.Models.ContractModel contract)
        {
            this.GetDataForLists();
            ViewBag.saved = "true";
            return View(contract);

        }

         public ActionResult ContractApproval(int ContractId, string contactType)
         {
             if (string.IsNullOrEmpty(contactType)) contactType = "Simple";
             ViewBag.contactType = contactType;
             ViewBag.contactId = ContractId;
             return View();
         }

         public ActionResult ContactData(int ContractId)
         {
             var model = new ContactDataModel();

             var excelRead = new ExcelQueryFactory(Server.MapPath("~/DummyData/dummydata.xlsx"));
             excelRead.AddMapping<ContactDataTouchpoint>(x=>x.AccessCode,"Access Code");             
             excelRead.AddMapping<ContactDataTouchpoint>(x => x.DueDate, "Due Date");
             excelRead.AddMapping<ContactDataZcodeModel>(x => x.AccessCode, "Access Code");
             excelRead.AddMapping<ContactDataZcodeModel>(x => x.WorkBreakdownStructure, "Work Breakdown Structure");
             excelRead.AddMapping<ContactDataZcodeModel>(x => x.CompletedDate, "Completed Date");
             excelRead.AddMapping<ContactDataZcodeModel>(x => x.AssignedBy, "Assigned By");
             excelRead.AddMapping<ContactDataZcodeModel>(x => x.CompletedBy, "Completed By");
             excelRead.AddMapping<ContactDataAlertModel>(x => x.AccessCode, "Access Code");
             excelRead.AddMapping<ContactDataAlertModel>(x => x.DueDate, "Due Date");
             model.Touchpoints = excelRead.Worksheet<ContactDataTouchpoint>("Data1").ToList();
             model.ZCodes = excelRead.Worksheet<ContactDataZcodeModel>("Data2").ToList();
             model.Alerts = excelRead.Worksheet<ContactDataAlertModel>("Data3").ToList();
             return View(model);
         }

        private void GetDataForLists()
         {
             ViewBag.subscriptionStatus = new SelectList(db.subscriptionStatus.ToList(), "id", "description");
             ViewBag.subscriptionType = new SelectList(db.subscriptionType.ToList(), "id", "description");
             ViewBag.multiTenantProjectType = new SelectList(db.multiTenantProjectType.ToList(), "id", "description");
             ViewBag.product = new SelectList(db.pr_getProductAll().ToList(), "id", "description");
             ViewBag.GroupId = new SelectList(db.pr_getGroupByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
             ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");
             ViewBag.GovtContractOfficerPOC = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
             ViewBag.GCPPOCEmail = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0}", v.email) }).ToList();
             ViewBag.contractManagerName = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
             ViewBag.customerAccountAdministrator = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
             ViewBag.contractType = new SelectList(db.subscriptionType.ToList(), "id", "description");
             ViewBag.touchpointContracts = new SelectList(db.subscriptionType.ToList(), "id", "description");
         }
    }
}