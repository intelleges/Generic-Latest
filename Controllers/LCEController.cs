using Generic.Helpers;
using Generic.Helpers.Utility;
using Generic.Models;
using Generic.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
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
            GenerateViewBag();
            return View();
        }

        private void GenerateViewBag()
        {
            var partnertype = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID);
            ViewBag.partnertype = partnertype.Select(o => new SelectListItem()
            {
                Text = o.description,
                Value = o.id.ToString()
            }).ToList();

            ViewBag.Owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LCEModel model)
        {
            GenerateViewBag();
            if (ModelState.IsValid)
            {
                var result = db.pr_getLCE_Special_Data(model.Owner, model.Designation, model.ProgramName, model.Duedate).FirstOrDefault();
                string loadGroup = db.pr_getAccesscode().FirstOrDefault();
                var partnerId = db.pr_addPartnerSpreadsheetDataLoad(result.partner_internal_id, result.partner_sap_id, result.partner_duns_number, model.ProgramName, model.Designation ?? "", result.partner_address_two, result.partner_city, null, "",null, model.From ?? "", model.To ?? "", result.partner_poc_title, result.partner_poc_phone_number, result.partner_poc_email_address, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, model.partnertype, result.touchpoint, model.Owner, result.partnerSpreadsheetDataLoadStatus, loadGroup, result.dueDate, result.group).FirstOrDefault();
                var Target = db.touchpoint.Where(x => x.id == result.touchpoint).ToList();
                var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).FirstOrDefault();

                var pptqId = db.pr_getPerson(result.person).First().partnerPartnertypeTouchpointQuestionnaire.First().id;
                int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;
                string sheetname = "CFDB";
                var map = new Dictionary<string, string>();

                map.Add("SalesOffice", "SalesOffice");
                map.Add("DistributionChannel", "DistributionChannel");
                map.Add("SalesOrderType", "SalesOrderType");
                map.Add("SalesOrderNumber", "SalesOrderNumber");
                map.Add("ContractTypeDescription", "ContractTypeDescription");
                map.Add("DPAS", "DPAS");
                map.Add("FMS", "FMS");
                map.Add("ForeignInterestsText", "ForeignInterestsText");
                map.Add("SecurityReqsApply", "SecurityReqsApply");
                map.Add("SecurityReqsClses", "SecurityReqsClses");
                map.Add("SecurityDetailsText", "SecurityDetailsText");
                map.Add("SectkRepsCertsApply", "SectkRepsCertsApply");
                map.Add("ContractorPurchasingSystemAdmin252234-7001", "ContractorPurchasingSystemAdmin252234");
                map.Add("EarnedValueMgmtSystems252234-7002", "EarnedValueMgmtSystems252234");
                map.Add("MMASApply", "MMASApply");
                map.Add("MMASClauses", "MMASClauses");
                map.Add("SalesOrderItemStatusDesc", "SalesOrderItemStatusDesc");
                map.Add("SalesOrderDataDescription", "SalesOrderDataDescription");
                map.Add("CustomerGroup", "CustomerGroup");
                map.Add("SBU", "SBU");
                map.Add("AribaID", "AribaID");
                map.Add("HWContractManager", "HWContractManager");
                map.Add("ContractingEntity", "ContractingEntity");
                map.Add("ProgramName", "ProgramName");
                map.Add("PlaceofPerformClses", "PlaceofPerformClses");
                map.Add("PlaceofPerformOthers", "PlaceofPerformOthers");
                map.Add("ChangeinLocation", "ChangeinLocation");
                map.Add("CustomerApproval", "CustomerApproval");
                map.Add("SubsNotConsApply", "SubsNotConsApply");
                map.Add("SubsnotconsClauses", "SubsnotconsClauses");
                map.Add("SubsNotConsOthers", "SubsNotConsOthers");
                map.Add("SupplierApprovalApply", "SupplierApprovalApply");
                map.Add("TradeAgreeActApply", "TradeAgreeActApply");
                map.Add("TradeAgreementsCls", "TradeAgreementsCls");
                map.Add("DomesticPrefRestApply", "DomesticPrefRestApply");
                map.Add("DomesticPrefRestOther", "DomesticPrefRestOther");
                map.Add("DomesticPrefRestClause", "DomesticPrefRestClause");
                map.Add("Outsourcerestrict's", "Outsourcerestricts");
                map.Add("ID", "ID");
                map.Add("SalesLineItem", "SalesLineItem");
                map.Add("SalesOrderHeaderStatus", "SalesOrderHeaderStatus");
                map.Add("SalesOrderHdrStatusDesc", "SalesOrderHdrStatusDesc");
                map.Add("SalesOrderItemStatus", "SalesOrderItemStatus");
                map.Add("CBT", "CBT");
                map.Add("CBT2", "CBT2");
                map.Add("PONumber", "PONumber");
                map.Add("CustomerID", "CustomerID");
                map.Add("FARPart12applies?", "FARPart12applies");
                map.Add("FARPart15applies?", "FARPart15applies");
                map.Add("TINA", "TINA");
                map.Add("CostActgClauseApply", "CostActgClauseApply");
                map.Add("ContractStartDate", "ContractStartDate");
                map.Add("ContractEndDate", "ContractEndDate");
                map.Add("PartNumber", "PartNumber");
                map.Add("ContractAdminName", "ContractAdminName");
                map.Add("AllowableCostClauses", "AllowableCostClauses");
                map.Add("CostActgClauseXemptDesc", "CostActgClauseXemptDesc");
                map.Add("PropOnContractApply", "PropOnContractApply");
                map.Add("PlaceOfPerformApply", "PlaceOfPerformApply");
                map.Add("p3rdPartyDisclosureRestrictions", "p3rdPartyDisclosureRestrictions");
                map.Add("WarrantyClausesApply", "WarrantyClausesApply");
                map.Add("WarrantyClauses", "WarrantyClauses");
                map.Add("WarrantyClausesOthers", "WarrantyClausesOthers");
                map.Add("CustomerName", "CustomerName");
                map.Add("CustomerCountry", "CustomerCountry");
                map.Add("SAPMasterContract", "SAPMasterContract");
                map.Add("ContractLine", "ContractLine");
                map.Add("CostActgClause", "CostActgClause");
                map.Add("CostActgClauseDesc", "CostActgClauseDesc");
                map.Add("CostActgClauseOthers", "CostActgClauseOthers");
                map.Add("CommercialItemStatus", "CommercialItemStatus");
                map.Add("ExportReqClauses", "ExportReqClauses");
                map.Add("ReportingDisclosureApply", "ReportingDisclosureApply");
                map.Add("RptgDisclosClses", "RptgDisclosClses");
                map.Add("ReportingDisclosureOther", "ReportingDisclosureOther");
                map.Add("Outsourceclauses", "Outsourceclauses");
                map.Add("ExportCusUniqReq", "ExportCusUniqReq");
                map.Add("PropAgmtType", "PropAgmtType");
                map.Add("BuyAmericanClauseApply", "BuyAmericanClauseApply");
                map.Add("BuyAmericanClauses", "BuyAmericanClauses");
                map.Add("BuyAmericanClauseOther", "BuyAmericanClauseOther");
                map.Add("RequiredTagsApply", "RequiredTagsApply");
                map.Add("RequiredTagsDesc", "RequiredTagsDesc");
                map.Add("Mil129Apply", "Mil129Apply");
                map.Add("Mil130Apply", "Mil130Apply");
                map.Add("EndUse", "EndUse");
                map.Add("EndUseDescription", "EndUseDescription");
                map.Add("PrimeContractNumber", "PrimeContractNumber");
                map.Add("ContractType", "ContractType");
                map.Add("GovtPropClauseApply", "GovtPropClauseApply");
                map.Add("GovtPropertyClauses", "GovtPropertyClauses");
                map.Add("SpecialToolingClause", "SpecialToolingClause");
                map.Add("GFP/CFP", "GFP_CFP");
                map.Add("Requalification", "Requalification");
                map.Add("CitizenshipRestrictionApply", "CitizenshipRestrictionApply");
                map.Add("CitizenshipClauses", "CitizenshipClauses");
                map.Add("CitizenshipRestrOthers", "CitizenshipRestrOthers");
                map.Add("SectkRepsCertsOthers", "SectkRepsCertsOthers");
                map.Add("SectkRepsCertsClses", "SectkRepsCertsClses");
                map.Add("ConfigMgmtClass1", "ConfigMgmtClass1");
                map.Add("ConfigMgmtClass2", "ConfigMgmtClass2");
                map.Add("SupplierChgApply", "SupplierChgApply");
                map.Add("AcctgSystemAdminstration252234-7006", "AcctgSystemAdminstration252234");
                map.Add("ContractorBusSystems252234-7005", "ContractorBusSystems252234");
                map.Add("ContractorPropertyMgmtSystemAdmin252234-7003", "ContractorPropertyMgmtSystemAdmin252234");
                map.Add("MMASOthers", "MMASOthers");
                map.Add("CounterfeitPartsClausesApply", "CounterfeitPartsClausesApply");
                map.Add("CounterfeitClauses", "CounterfeitClauses");
                map.Add("NationalStockNumber", "NationalStockNumber");
                map.Add("NasaQualReqd", "NasaQualReqd");
                map.Add("NasaQualText", "NasaQualText");
                map.Add("PropertyType", "PropertyType");
                map.Add("PropertyTypeDesc", "PropertyTypeDesc");
                map.Add("ConfigMgmtChangesText", "ConfigMgmtChangesText");
                map.Add("QualityReqApply", "QualityReqApply");
                map.Add("QualityReqOthers", "QualityReqOthers");
                map.Add("OtherShipPkgReq", "OtherShipPkgReq");
                map.Add("TransPN", "TransPN");
                map.Add("TransDesc", "TransDesc");

                int i = 0;
                if (model.File != null)
                {
                    var personinExcel = ExcelMapper.GetRows<ExcelLCE>(model.File.InputStream, sheetname, map).ToList();
                    foreach (var item in personinExcel)
                    {
                        db.pr_addCFDB(pptqId, item.SalesOffice, item.DistributionChannel, item.SalesOrderType, item.SalesOrderNumber, item.SalesLineItem, item.SalesOrderHeaderStatus, item.SalesOrderHdrStatusDesc, item.SalesOrderItemStatus, item.SalesOrderItemStatusDesc, item.SalesOrderDataDescription, item.CustomerGroup, item.SBU, item.CBT, item.CBT2, item.PONumber, item.CustomerID, item.CustomerName, item.CustomerCountry, item.SAPMasterContract ?? "", item.ContractLine, item.ContractStartDate, item.ContractEndDate, item.PartNumber, item.ContractAdminName, item.AribaID, item.HWContractManager, item.ContractingEntity, item.ProgramName, item.EndUse, item.EndUseDescription, item.PrimeContractNumber, item.ContractType, item.ContractTypeDescription, item.DPAS, item.FMS, item.ForeignInterestsText, item.GovtPropClauseApply, item.GovtPropertyClauses, item.SpecialToolingClause, item.GFP_CFP, item.NasaQualReqd, item.NasaQualText, item.PropertyType, item.PropertyTypeDesc, item.PropAgmtType, item.BuyAmericanClauseApply, item.BuyAmericanClauses, item.BuyAmericanClauseOther, item.BuyAmericanClauseApply, item.TradeAgreementsCls, item.FARPart12applies, item.FARPart15applies, item.TINA, item.CostActgClauseApply, item.CostActgClause, item.CostActgClauseDesc, item.CostActgClauseOthers, item.CommercialItemStatus, item.AllowableCostClauses, item.CostActgClauseXemptDesc, item.PropOnContractApply, item.PlaceOfPerformApply, item.PlaceofPerformClses, item.PlaceofPerformOthers, item.ChangeinLocation, item.CustomerApproval, item.Requalification, item.CitizenshipRestrictionApply, item.CitizenshipClauses, item.CitizenshipRestrOthers, item.SecurityReqsApply, item.SecurityReqsClses, item.SecurityDetailsText, item.SectkRepsCertsApply, item.SectkRepsCertsOthers, item.SectkRepsCertsClses, item.ConfigMgmtClass1, item.ConfigMgmtClass2, item.ConfigMgmtChangesText, item.QualityReqApply, item.QualityReqOthers, item.OtherShipPkgReq, item.RequiredTagsApply, item.RequiredTagsDesc, item.Mil129Apply, item.Mil130Apply, item.DomesticPrefRestApply, item.DomesticPrefRestOther, item.DomesticPrefRestClause, item.Outsourcerestrict, item.Outsourceclauses, item.ExportCusUniqReq, item.ExportReqClauses, item.ReportingDisclosureApply, item.RptgDisclosClses, item.ReportingDisclosureOther, item.p3rdPartyDisclosureRestrictions, item.WarrantyClausesApply, item.WarrantyClauses, item.WarrantyClausesOthers, item.SubsNotConsApply, item.SubsnotconsClauses, item.SubsNotConsOthers, item.SupplierApprovalApply, item.SupplierChgApply, item.AcctgSystemAdminstration252234, item.ContractorBusSystems252234, item.ContractorPropertyMgmtSystemAdmin252234, item.ContractorPurchasingSystemAdmin252234, item.EarnedValueMgmtSystems252234, item.MMASApply, item.MMASClauses, item.MMASOthers, item.CounterfeitPartsClausesApply, item.CounterfeitClauses, item.NationalStockNumber, item.TransPN, item.TransDesc, i, 1);
                        i++;
                    }
                }

                ViewBag.Count = i;
                ViewBag.Accesscode = pptq.accesscode;
                
                ViewBag.Pptq = pptqId;
                ViewBag.TeamAssigned = db.pr_getPersonTeamAssignedByTouchpoint(result.touchpoint).ToList();
                ViewBag.UnassignedByEnterprise = db.pr_getPersonTouchpointTeamUnassignedByEnterprise(CurrentInstance.EnterpriseID).ToList();

                ViewBag.PrName = model.ProgramName;
                ViewBag.AllClause = db.pr_getCFDBClauseAllForDisplay(model.partnertype).ToList();
                ViewBag.SelectedClauses = db.pr_getCFDBPartnertypeClauseByPartnertype(model.partnertype).ToList();
                Session["LceModel"] = model;
                return View();
            }

            return View(model);
        }

        public ActionResult CreateResult()
        {
            var testId = 117226;
            if (Session["loadGroup"] != null)
            {
                var pptqO = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(Session["loadGroup"].ToString()).FirstOrDefault();
                var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(pptqO.accesscode).FirstOrDefault();
                var data = db.pr_getPPTQTeamRacixByPPTQ_Grid(testId).ToList();
                return View(data.Select(o => new pr_getPPTQTeamRacixByPPTQ_Grid_Result()
                {
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
            return Json(db.pr_getPersonAll(1066).Select(o => o.email + " " + o.firstName + " " + o.lastName));
        }

        public ActionResult GetRegions(int id)
        {
            return Json(
                db.pr_getRegionByPartnerType(id).ToList(), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddPPTQTeam(AddPPTQTeamViewModel model)
        {
            LCEModel vm = Session["LceModel"] as LCEModel;
            SendEmail objSendEmail = new SendEmail();
            var owner = db.pr_getPerson(vm.Owner).First();
            var from = new System.Net.Mail.MailAddress(owner.email, owner.firstName + " " + owner.lastName);
            var activityType = db.pr_getParnterType(vm.partnertype).First().description;

            foreach (var item in model.Ids)
            {
                try
                {
                    db.pr_addPPTQTeam(model.Id, item, 0, true);
                }
                catch { }
                  
                var person = db.pr_getPerson(item).FirstOrDefault();
                if (person != null)
                {
                    Email email = new Email();
                    string strEmailBody = "Hi " + person.firstName + ",<br/>" + "I have an action as part of the new REV 20 LCE checklist for transition "
                        + vm.ProgramName + " " + vm.Designation + ".<br/>" + activityType + " - " + vm.From + " to " + vm.To;
                    email.subject = "This is a test";
                    email.body = strEmailBody;
                    email.category = SendGridCategory.EmailSend;
                    email.url = Request.Url.ToString();
                    email.emailTo = person.email;
                 
                    objSendEmail.sendEmail(email, new EmailFormatSettings() {
                         enterprise = new enterprise() {
                              id = Generic.Helpers.CurrentInstance.EnterpriseID
                         }
                    }, from);
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddClause(AddPPTQTeamViewModel model)
        {
            foreach (var item in model.Ids)
            {
                //db.pr_amodel.Id, item, 0, true);
            }

            return Json(new { success = true });
        }
    }
}