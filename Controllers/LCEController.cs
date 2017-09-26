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
    [Flags]
    public enum FilesUploaded
    {
        File,
        FileScope,
        FileCID,
        FileEntanglement
    }
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
            ModelState.Clear();
            ViewBag.Count = null;
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
                int enterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;
                var result = db.pr_getLCE_Special_Data(model.Owner, model.Designation, model.ProgramName, model.Duedate).FirstOrDefault();
                string loadGroup = db.pr_getAccesscode().FirstOrDefault();
                var partnerSpreadsheetDataLoadId = db.pr_addPartnerSpreadsheetDataLoad(result.partner_internal_id, result.partner_sap_id, result.partner_duns_number, model.ProgramName, model.Designation ?? "", model.BuyToBuyType ?? "", result.partner_city, null, "", null, model.From ?? "", model.To ?? "", result.partner_poc_title, result.partner_poc_phone_number, result.partner_poc_email_address, "", "", "", DateTime.Now, enterpriseID, model.partnertype, result.touchpoint, model.Owner, result.partnerSpreadsheetDataLoadStatus, loadGroup, result.dueDate, result.group).FirstOrDefault();
                var pptqId = db.pr_getPerson(result.person).First().partnerPartnertypeTouchpointQuestionnaire.First().id;

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
                ViewBag.Pptq = pptqId;
                ViewBag.PartnerSpreadsheetDataLoadId = partnerSpreadsheetDataLoadId;
                ViewBag.TeamAssigned = db.pr_getPersonTeamAssignedByTouchpoint(result.touchpoint).ToList();
                ViewBag.UnassignedByEnterprise = db.pr_getPersonTouchpointTeamUnassignedByEnterprise(CurrentInstance.EnterpriseID).ToList();

                ViewBag.PrName = model.ProgramName;
                ViewBag.AllClause = db.pr_getCFDBClauseAllForDisplay(model.partnertype).ToList();
                var selectedClauses = db.pr_getCFDBPartnertypeClauseByPartnertype(model.partnertype).ToList();
                var selectedClausesIds = selectedClauses.Select(o => o.clause).ToList();

                ViewBag.SelectedClauses = db.pr_getCFDBClauseAll().Where(o => selectedClausesIds.Contains(o.id)).ToList();
                Session["LceModel"] = model;
                return View(model);
            }

            return View(model);
        }


        [HttpPost]
        public ActionResult AddPPTQTeam(AddPPTQTeamViewModel model)
        {
            if (model.Ids != null)
            {

                foreach (var item in model.Ids)
                {
                    try
                    {
                        db.pr_addPPTQTeam(model.Id, item, 0, true);
                    }
                    catch { }
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddClause(AddPPTQTeamViewModel model)
        {
            if (model.Ids == null)
                model.Ids = new List<int>();

            var list = db.pr_getCFDBPartnertypeClauseByPartnertype(model.partnerType).ToList();
            List<dynamic> rtn = new List<dynamic>();
            foreach (var item in model.Ids)
            {
                var defApproval = list.Where(o => o.clause == item).FirstOrDefault();
                var defSendData = list.Where(o => o.clause == item).FirstOrDefault();

                rtn.Add(new
                {
                    id = item,
                    defApproval = defApproval == null ? null : (int?)defApproval.getApprovalFromPerson,
                    defSendData = defSendData == null ? null : (int?)defSendData.sendDataToPerson,
                });
            }

            return Json(new { success = true, list = rtn });
        }

        public ActionResult GetClause(int id)
        {
            var allClause = db.pr_getCFDBClauseAllForDisplay(id).ToList();
            var selectedClauses = db.pr_getCFDBPartnertypeClauseByPartnertype(id).ToList();
            var selectedClausesIds = selectedClauses.Select(o => o.clause).ToList();

            return Json(new
            {
                all = allClause.Where(o => !selectedClausesIds.Contains(o.id)).ToList(),
                selected = db.pr_getCFDBClauseAll().Where(o => selectedClausesIds.Contains(o.id)).ToList()
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SetClauses(ClauseViewModel model)
        {
            if (model.items != null)
            {
                LCEModel vm = Session["LceModel"] as LCEModel;
                SendEmail objSendEmail = new SendEmail();
                var owner = db.pr_getPerson(vm.Owner).First();
                var from = new System.Net.Mail.MailAddress(owner.email, owner.firstName + " " + owner.lastName);
                var activityType = db.pr_getParnterType(model.partnerType).First().description;


                byte[] b = new byte[0];
                foreach (var item in model.items)
                {
                    try
                    {
                        db.pr_addPersonPPTQClause(item.sendDataTo, model.pptq, item.id, 0, DateTime.Now, item.approvalNeededBy, null, b, vm.Comments ?? "", "", 0, true);
                    }
                    catch { }


                    var person = db.pr_getPerson(item.sendDataTo).FirstOrDefault();
                    var personApp = db.pr_getPerson(item.getApprovalFrom).FirstOrDefault();
                    if (person != null)
                    {
                        Email email = new Email();
                        string strEmailBody = "Hi " + person.firstName + ",<br/>" +
                            "Thanks in advance for helping us on this important transition.<br/>" +
                            "Honeywell Aerospace is performing the due diligence for " + vm.ProgramName + " a " + activityType + " transition from " + vm.From + " to " + vm.To + ".<br/>" +
                            "We need you to review the attached CFDB output and respond and/or approve no later than " + (personApp == null ? "" : (personApp.firstName + " " + personApp.lastName)) + ".<br/>" +
                            "If you have questions or mitigating actions, please reply to this email.  Mitigating actions will be worked outside of the LC&E application.<br/>" +
                            "Please use this link " + item.text + " to record your approval.<br/><br/>" +
                            "I appreciate your assistance,<br/>" +
                            "Owner " + owner.firstName + " " + owner.lastName + "<br/><br/>"+
                            "<p style='font-style: italic;'>[<br/>Legal Disclaimer:<br/>The plans and proposals described in this presentation or email are forward-looking business plans subject to modification based on many factors, including changing economic and business conditions.  Unless otherwise noted, the plans and proposals described here are not final and may be modified or even abandoned at any time.  No final decision will be taken with respect to such plans or proposals without prior satisfaction of any applicable requirements that the relevant company inform, consult or negotiate with employees or their representatives.<br/>Preliminary-not final-no decision will be taken without satisfaction of any applicable consultation or negotiation requirements.</p>"
                           ;
                        email.subject = "LC& E Checklist for Transition Approval Request";
                        email.body = strEmailBody;
                        email.category = SendGridCategory.EmailSend;
                        email.url = Request.Url.ToString();
                        email.emailTo = person.email;

                        objSendEmail.sendEmail(email, new EmailFormatSettings()
                        {
                            enterprise = new enterprise()
                            {
                                id = Generic.Helpers.CurrentInstance.EnterpriseID
                            }
                        }, from);
                    }
                }
            }

            return Json(new { success = true });
        }
    }
}