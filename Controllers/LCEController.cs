using Generic.Helpers;
using Generic.Helpers.PartnerHelper;
using Generic.Helpers.Utility;
using Generic.Models;
using Generic.SessionClass;
using Generic.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
namespace Generic.Controllers
{
    [Flags]
    public enum FilesUploaded
    {
        File = 1,
        FileScope = 2,
        FileCID = 4,
        FileEntanglement = 8,
        SupplierSelfAssessmentUpload = 16,
        BAATransitionScopeUpload = 32
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

        public ActionResult DownloadDashboard(int pptq)
        {
            var items = db.pr_getRequestApprovalDashboard1(pptq).ToList();
            byte[] barr = ExcelMapper.CreateExcel<pr_getRequestApprovalDashboard1_Result>(new MemoryStream(), "Dashboard", items);
            return File(barr, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Dashboard.xls");
        }

        public ActionResult EditPartial(string accessCode)
        {
            ModelState.Clear();
            ViewBag.Count = null;
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
            ViewBag.PptqId = pptq.id;
            ViewBag.accessCode = accessCode;
            var pptqDocs = db.pr_getPPTQDocByPPTQ(pptq.id).ToList();
            var file = pptqDocs.Where(o => o.sortOrder == (int)FilesUploaded.File).FirstOrDefault();
            var fileCID = pptqDocs.Where(o => o.sortOrder == (int)FilesUploaded.FileCID).FirstOrDefault();
            var fileEntanglement = pptqDocs.Where(o => o.sortOrder == (int)FilesUploaded.FileEntanglement).FirstOrDefault();
            var fileScope = pptqDocs.Where(o => o.sortOrder == (int)FilesUploaded.FileScope).FirstOrDefault();
            var supplierSelfAssessmentUpload = pptqDocs.Where(o => o.sortOrder == (int)FilesUploaded.SupplierSelfAssessmentUpload).FirstOrDefault();
            var BAATransitionScopeUpload = pptqDocs.Where(o => o.sortOrder == (int)FilesUploaded.BAATransitionScopeUpload).FirstOrDefault();

            ViewBag.Dashboard = db.pr_getRequestApprovalDashboard1(pptq.id).ToList();

            var model = new LCEModel()
            {
                ProgramName = pptq.partner1.name,
                Designation = pptq.partner1.address1,
                Comments = pptq.partner1.dunsNumber,
                BuyToBuyType = pptq.partner1.address2,
                From = pptq.partner1.firstName,
                To = pptq.partner1.lastName,
                ProjectUrl = pptq.partner1.title,
                Owner = pptq.person.id,
                Duedate = pptq.dueDate,
                Priority = pptq.priority,
                partnertype = pptq.partnerTypeTouchpointQuestionnaire1.partnerType,
                FileName = file != null ? file.title : null,
                FileCIDName = fileCID != null ? fileCID.title : null,
                FileEntanglementName = fileEntanglement != null ? fileEntanglement.title : null,
                FileScopeName = fileScope != null ? fileScope.title : null,
                SupplierSelfAssessmentUploadName = supplierSelfAssessmentUpload != null ? supplierSelfAssessmentUpload.title : null,
                BAATransitionScopeUploadName = BAATransitionScopeUpload != null ? BAATransitionScopeUpload.title : null
            };
            var files = db.pr_getPPTQDocByPPTQ(pptq.id).ToList();
            //foreach(var file in )
            GenerateViewBag(model.partnertype, pptq.person.id);
            return PartialView(model);
        }
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult EditPartial(LCEModel model, int Id)
        {
            if (ModelState.IsValid)
            {
                var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(Id).FirstOrDefault();
                if (pptq != null)
                {
                    var partner = pptq.partner1;
                    var person = pptq.person;
                    partner.name = model.ProgramName;
                    partner.address1 = model.Designation;
                    partner.dunsNumber = model.Comments;
                    partner.address2 = model.BuyToBuyType;
                    partner.firstName = model.From;
                    partner.lastName = model.To;
                    partner.title = model.ProjectUrl;
                    pptq.invitedBy = model.Owner;
                    pptq.dueDate = model.Duedate;
                    pptq.priority = model.Priority;
                    pptq.partnerTypeTouchpointQuestionnaire1.partnerType = model.partnertype;
                    db.Entry(partner).CurrentValues.SetValues(partner);
                    db.Entry(pptq.partnerTypeTouchpointQuestionnaire1).CurrentValues.SetValues(pptq.partnerTypeTouchpointQuestionnaire1);
                    db.Entry(pptq).CurrentValues.SetValues(pptq);
                    db.SaveChanges();

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
                    FilesUploaded? filesUploadedResult = (FilesUploaded?)null;
                    int channel = 2;
                    if (model.File != null)
                    {
                        ViewBag.Check = "disabled";
                        var personinExcel = ExcelMapper.GetRows<ExcelLCE>(model.File.InputStream, sheetname, map).ToList();
                        foreach (var item in personinExcel)
                        {
                            db.pr_addCFDB(pptq.id, item.SalesOffice, item.DistributionChannel, item.SalesOrderType, item.SalesOrderNumber, item.SalesLineItem, item.SalesOrderHeaderStatus, item.SalesOrderHdrStatusDesc, item.SalesOrderItemStatus, item.SalesOrderItemStatusDesc, item.SalesOrderDataDescription, item.CustomerGroup, item.SBU, item.CBT, item.CBT2, item.PONumber, item.CustomerID, item.CustomerName, item.CustomerCountry, item.SAPMasterContract ?? "", item.ContractLine, item.ContractStartDate, item.ContractEndDate, item.PartNumber, item.ContractAdminName, item.AribaID, item.HWContractManager, item.ContractingEntity, item.ProgramName, item.EndUse, item.EndUseDescription, item.PrimeContractNumber, item.ContractType, item.ContractTypeDescription, item.DPAS, item.FMS, item.ForeignInterestsText, item.GovtPropClauseApply, item.GovtPropertyClauses, item.SpecialToolingClause, item.GFP_CFP, item.NasaQualReqd, item.NasaQualText, item.PropertyType, item.PropertyTypeDesc, item.PropAgmtType, item.BuyAmericanClauseApply, item.BuyAmericanClauses, item.BuyAmericanClauseOther, item.BuyAmericanClauseApply, item.TradeAgreementsCls, item.FARPart12applies, item.FARPart15applies, item.TINA, item.CostActgClauseApply, item.CostActgClause, item.CostActgClauseDesc, item.CostActgClauseOthers, item.CommercialItemStatus, item.AllowableCostClauses, item.CostActgClauseXemptDesc, item.PropOnContractApply, item.PlaceOfPerformApply, item.PlaceofPerformClses, item.PlaceofPerformOthers, item.ChangeinLocation, item.CustomerApproval, item.Requalification, item.CitizenshipRestrictionApply, item.CitizenshipClauses, item.CitizenshipRestrOthers, item.SecurityReqsApply, item.SecurityReqsClses, item.SecurityDetailsText, item.SectkRepsCertsApply, item.SectkRepsCertsOthers, item.SectkRepsCertsClses, item.ConfigMgmtClass1, item.ConfigMgmtClass2, item.ConfigMgmtChangesText, item.QualityReqApply, item.QualityReqOthers, item.OtherShipPkgReq, item.RequiredTagsApply, item.RequiredTagsDesc, item.Mil129Apply, item.Mil130Apply, item.DomesticPrefRestApply, item.DomesticPrefRestOther, item.DomesticPrefRestClause, item.Outsourcerestrict, item.Outsourceclauses, item.ExportCusUniqReq, item.ExportReqClauses, item.ReportingDisclosureApply, item.RptgDisclosClses, item.ReportingDisclosureOther, item.p3rdPartyDisclosureRestrictions, item.WarrantyClausesApply, item.WarrantyClauses, item.WarrantyClausesOthers, item.SubsNotConsApply, item.SubsnotconsClauses, item.SubsNotConsOthers, item.SupplierApprovalApply, item.SupplierChgApply, item.AcctgSystemAdminstration252234, item.ContractorBusSystems252234, item.ContractorPropertyMgmtSystemAdmin252234, item.ContractorPurchasingSystemAdmin252234, item.EarnedValueMgmtSystems252234, item.MMASApply, item.MMASClauses, item.MMASOthers, item.CounterfeitPartsClausesApply, item.CounterfeitClauses, item.NationalStockNumber, item.TransPN, item.TransDesc, i, 1).FirstOrDefault();
                            i++;
                        }

                        var ch = db.pr_getCFDBChannelTwoCheck(pptq.id).FirstOrDefault();
                        if (ch != null)
                            channel = ch.Value;
                        try
                        {
                            //sometime generated error
                            AddModifyPptqDoc(model.File, pptq.id, "CFDB uploaded document", FilesUploaded.File);
                            //position 1
                            UpdateScore(pptq.id, 1);
                        }
                        catch { }
                    }
                    if (model.FileScope != null)
                    {
                        AddModifyPptqDoc(model.FileScope, pptq.id, "LC&E Scope Uploaded document", FilesUploaded.FileScope);
                        //position 6
                        UpdateScore(pptq.id, 6);
                    }
                    if (model.FileCID != null)
                    {
                        AddModifyPptqDoc(model.FileCID, pptq.id, "CID Uploaded document", FilesUploaded.FileCID);
                        //position 3
                        UpdateScore(pptq.id, 3);
                    }
                    if (model.FileEntanglement != null)
                    {
                        AddModifyPptqDoc(model.FileEntanglement, pptq.id, "Entanglement Uploaded document", FilesUploaded.FileEntanglement);
                        //position 2
                        UpdateScore(pptq.id, 2);
                    }
                    if (model.BAATransitionScopeUpload != null)
                    {
                        AddModifyPptqDoc(model.BAATransitionScopeUpload, pptq.id, "BAA Transition Scope Upload", FilesUploaded.BAATransitionScopeUpload);
                        //position 5
                        UpdateScore(pptq.id, 5);
                    }
                    if (model.SupplierSelfAssessmentUpload != null)
                    {
                        AddModifyPptqDoc(model.SupplierSelfAssessmentUpload, pptq.id, "Supplier Self-Assessment Upload", FilesUploaded.SupplierSelfAssessmentUpload);
                        //position 4
                        UpdateScore(pptq.id, 4);
                    }
                    //using (var dbEntityes = new EntitiesDBContext())
                    //{
                    //    var files = dbEntityes.pr_getPPTQDocByPPTQ(pptq.id).ToList();
                    //    var requesredPptq = dbEntityes.pr_getPartnerPartnertypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();
                    //    requesredPptq.score = files.Sum(o => o.sortOrder);
                    //    dbEntityes.Entry(requesredPptq).State = System.Data.Entity.EntityState.Modified;
                    //    dbEntityes.SaveChanges();
                    //}
                    return Json(true);
                }
            }
            return Json(false);
        }

        private void UpdateScore(int pptqId, int position)
        {
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(pptqId).FirstOrDefault();
            int score = Convert.ToInt32(pptq.score ?? 0);
            string binary = Convert.ToString(score, 2).PadLeft(6, '0');
            if (binary.Length < 6 || binary.Length > 6)
                binary = "000000";

            var aStringBuilder = new StringBuilder(binary);
            aStringBuilder.Remove(position - 1, 1);
            aStringBuilder.Insert(position - 1, "1");
            binary = aStringBuilder.ToString();

            int val = Convert.ToInt32(binary, 2);
            db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireScore(pptq.id, val);
        }

        private void GenerateViewBag(int? partnertypeId = null, int? ownerId = null)
        {
            var partnertype = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            ViewBag.partnertypeList = new SelectList(partnertype, "id", "description", partnertypeId);

            ViewBag.OwnerList = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "Id", "FullName", ownerId);
        }

        public ActionResult GetCountByPptqId(int pptqId)
        {
            var c = db.pr_getCFDBRecordCountByPPTQ(pptqId).FirstOrDefault().Value;
            return Json(new { count = c }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(LCEModel model)
        {
            GenerateViewBag();
            if (ModelState.IsValid)
            {
                if ((model.Comments ?? "").Length > 255)
                {
                    ModelState.AddModelError("", "Description max length 255!");
                    return View(model);
                }

                int enterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;
                var result = db.pr_getLCE_Special_Data(model.Owner, model.Designation, model.ProgramName, model.Duedate, model.partnertype).FirstOrDefault();
                string loadGroup = db.pr_getAccesscode().FirstOrDefault();

                int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnerType(model.partnertype).FirstOrDefault().id;

                var partnerId = (int)db.pr_addPartnerSpreadsheetDataLoad(result.partner_internal_id, result.partner_sap_id, model.Comments ?? "", model.ProgramName, model.Designation ?? "", model.BuyToBuyType ?? "", result.partner_city, null, "", null, model.From ?? "", model.To ?? "", model.ProjectUrl ?? "", result.partner_poc_phone_number, result.partner_poc_email_address, "", "", "", DateTime.Now, enterpriseID, model.partnertype, result.touchpoint, model.Owner, result.partnerSpreadsheetDataLoadStatus, loadGroup, result.dueDate, result.group).FirstOrDefault();

                string acc = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).FirstOrDefault().accesscode;
                int pptqId = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(acc).FirstOrDefault().id;

                db.pr_addPPTQDocShellForLCE(pptqId, SessionSingleton.LoggedInUserId).FirstOrDefault();
                try
                {
                    int val = Convert.ToInt32("000000", 2);
                    //Not clear why again there generated error
                    db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireScoreAndPriority(pptqId, val, model.Priority);
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }

                if (model.FileScope != null)
                {
                    AddModifyPptqDoc(model.FileScope, pptqId, "LC&E Scope Uploaded document", FilesUploaded.FileScope);
                }
                if (model.FileCID != null)
                {
                    AddModifyPptqDoc(model.FileCID, pptqId, "CID Uploaded document", FilesUploaded.FileCID);
                }
                if (model.FileEntanglement != null)
                {
                    AddModifyPptqDoc(model.FileEntanglement, pptqId, "Entanglement Uploaded document", FilesUploaded.FileEntanglement);
                }
                if (model.BAATransitionScopeUpload != null)
                {
                    AddModifyPptqDoc(model.BAATransitionScopeUpload, pptqId, "BAA Transition Scope Upload", FilesUploaded.BAATransitionScopeUpload);
                }
                if (model.SupplierSelfAssessmentUpload != null)
                {
                    AddModifyPptqDoc(model.SupplierSelfAssessmentUpload, pptqId, "Supplier Self-Assessment Upload", FilesUploaded.SupplierSelfAssessmentUpload);
                }

                //ViewBag.Count = i;
                ViewBag.Pptq = pptqId;
                ViewBag.PartnerSpreadsheetDataLoadId = partnerId;
                ViewBag.TeamAssigned = db.pr_getPersonTeamAssignedByTouchpoint(result.touchpoint).ToList();
                ViewBag.UnassignedByEnterprise = db.pr_getPersonTouchpointTeamUnassignedByEnterprise(CurrentInstance.EnterpriseID).ToList();

                int channel = 2;
                ViewBag.ChannelValue = channel;

                ViewBag.PrName = model.ProgramName;
                var allclbychannels = db.pr_getCFDBPartnertypeClauseAll().ToList().Where(o => o.channel == channel).Select(o => o.clause).ToList();
                ViewBag.AllClause = db.pr_getCFDBClauseAllForDisplay(model.partnertype).Where(b => allclbychannels.Contains(b.id)).ToList();
                var selectedClauses = db.pr_getCFDBPartnertypeClauseByPartnertype(model.partnertype).Where(o => o.channel == channel).ToList();
                var selectedClausesIds = selectedClauses.Select(o => o.clause).ToList();

                ViewBag.SelectedClauses = db.pr_getCFDBClauseAll().Where(o => selectedClausesIds.Contains(o.id)).ToList();
                Session["LceModel"] = model;

                ViewBag.Items = db.pr_getClausePersonPartnertypeSortOrderByPartnertype(model.partnertype).ToList();
                var partnertype = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
                ViewBag.ActiveType = partnertype.Where(o => o.id == model.partnertype).First().description;
                ViewBag.AccessCode = acc;
                return View(model);
            }

            return View(model);
        }

        public ActionResult UploadCfdb(IEnumerable<HttpPostedFileBase> files, int pptq)
        {
            // The Name of the Upload component is "files"
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);
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
                FilesUploaded? filesUploadedResult = (FilesUploaded?)null;
                int channel = 2;

                ViewBag.Check = "disabled";
                db.pr_removeCFDBByPPTQ(pptq);
                var personinExcel = ExcelMapper.GetRows<ExcelLCE>(file.InputStream, sheetname, map).ToList();
                foreach (var item in personinExcel)
                {
                    db.pr_addCFDB(pptq, item.SalesOffice, item.DistributionChannel, item.SalesOrderType, item.SalesOrderNumber, item.SalesLineItem, item.SalesOrderHeaderStatus, item.SalesOrderHdrStatusDesc, item.SalesOrderItemStatus, item.SalesOrderItemStatusDesc, item.SalesOrderDataDescription, item.CustomerGroup, item.SBU, item.CBT, item.CBT2, item.PONumber, item.CustomerID, item.CustomerName, item.CustomerCountry, item.SAPMasterContract ?? "", item.ContractLine, item.ContractStartDate, item.ContractEndDate, item.PartNumber, item.ContractAdminName, item.AribaID, item.HWContractManager, item.ContractingEntity, item.ProgramName, item.EndUse, item.EndUseDescription, item.PrimeContractNumber, item.ContractType, item.ContractTypeDescription, item.DPAS, item.FMS, item.ForeignInterestsText, item.GovtPropClauseApply, item.GovtPropertyClauses, item.SpecialToolingClause, item.GFP_CFP, item.NasaQualReqd, item.NasaQualText, item.PropertyType, item.PropertyTypeDesc, item.PropAgmtType, item.BuyAmericanClauseApply, item.BuyAmericanClauses, item.BuyAmericanClauseOther, item.BuyAmericanClauseApply, item.TradeAgreementsCls, item.FARPart12applies, item.FARPart15applies, item.TINA, item.CostActgClauseApply, item.CostActgClause, item.CostActgClauseDesc, item.CostActgClauseOthers, item.CommercialItemStatus, item.AllowableCostClauses, item.CostActgClauseXemptDesc, item.PropOnContractApply, item.PlaceOfPerformApply, item.PlaceofPerformClses, item.PlaceofPerformOthers, item.ChangeinLocation, item.CustomerApproval, item.Requalification, item.CitizenshipRestrictionApply, item.CitizenshipClauses, item.CitizenshipRestrOthers, item.SecurityReqsApply, item.SecurityReqsClses, item.SecurityDetailsText, item.SectkRepsCertsApply, item.SectkRepsCertsOthers, item.SectkRepsCertsClses, item.ConfigMgmtClass1, item.ConfigMgmtClass2, item.ConfigMgmtChangesText, item.QualityReqApply, item.QualityReqOthers, item.OtherShipPkgReq, item.RequiredTagsApply, item.RequiredTagsDesc, item.Mil129Apply, item.Mil130Apply, item.DomesticPrefRestApply, item.DomesticPrefRestOther, item.DomesticPrefRestClause, item.Outsourcerestrict, item.Outsourceclauses, item.ExportCusUniqReq, item.ExportReqClauses, item.ReportingDisclosureApply, item.RptgDisclosClses, item.ReportingDisclosureOther, item.p3rdPartyDisclosureRestrictions, item.WarrantyClausesApply, item.WarrantyClauses, item.WarrantyClausesOthers, item.SubsNotConsApply, item.SubsnotconsClauses, item.SubsNotConsOthers, item.SupplierApprovalApply, item.SupplierChgApply, item.AcctgSystemAdminstration252234, item.ContractorBusSystems252234, item.ContractorPropertyMgmtSystemAdmin252234, item.ContractorPurchasingSystemAdmin252234, item.EarnedValueMgmtSystems252234, item.MMASApply, item.MMASClauses, item.MMASOthers, item.CounterfeitPartsClausesApply, item.CounterfeitClauses, item.NationalStockNumber, item.TransPN, item.TransDesc, i, 1).FirstOrDefault();
                    i++;
                }

                var ch = db.pr_getCFDBChannelTwoCheck(pptq).FirstOrDefault();
                if (ch != null)
                    channel = ch.Value;
                try
                {
                    //sometime generated error
                    AddModifyPptqDoc(files.First(), pptq, "CFDB uploaded document", FilesUploaded.File);
                    UpdateScore(pptq, 1);
                }
                catch { }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult UploadFile(IEnumerable<HttpPostedFileBase> files, int pptq, int position)
        {
            // The Name of the Upload component is "files"
            if (Request.Files != null && Request.Files.Count > 0)
            {
                var file = Request.Files[0];
                var fileName = Path.GetFileName(file.FileName);

                if (position == 6)
                    AddModifyPptqDoc(file as HttpPostedFileBase, pptq, "LC&E Scope Uploaded document", FilesUploaded.FileScope);

                if (position == 3)
                {
                    AddModifyPptqDoc(file as HttpPostedFileBase, pptq, "CID Uploaded document", FilesUploaded.FileCID);
                    db.pr_removeCIDByPPTQ(pptq);
                    string sheetname = "cidFinal";
                    var map = new Dictionary<string, string>();
                    map.Add("FINALUSAGECode", "FINALUSAGECode");
                    var personinExcel = ExcelMapper.GetRows<ExcelCid>(file.InputStream, sheetname, map).ToList();
                    foreach (var item in personinExcel.Where(o => o.FINALUSAGECode != null))
                        db.pr_addCID(pptq, SessionSingleton.LoggedInUserId, item.FINALUSAGECode);
                }

                if (position == 2)
                    AddModifyPptqDoc(file as HttpPostedFileBase, pptq, "Entanglement Uploaded document", FilesUploaded.FileEntanglement);

                if (position == 5)
                    AddModifyPptqDoc(file as HttpPostedFileBase, pptq, "BAA Transition Scope Upload", FilesUploaded.BAATransitionScopeUpload);

                if (position == 4)
                    AddModifyPptqDoc(file as HttpPostedFileBase, pptq, "Supplier Self-Assessment Upload", FilesUploaded.SupplierSelfAssessmentUpload);

                UpdateScore(pptq, position);
            }

            return Content("");
        }

        private void AddModifyPptqDoc(HttpPostedFileBase fileStream, int pptqId, string fileDescription, FilesUploaded sortOrder)
        {
            using (var memoryStream = new MemoryStream())
            {
                var files = db.pr_getPPTQDocByPPTQ(pptqId).ToList();
                var cfdbFile = files.FirstOrDefault(o => o.uploadedBy == SessionSingleton.LoggedInUserId && o.sortOrder == (int)sortOrder);
                fileStream.InputStream.CopyTo(memoryStream);
                if (cfdbFile == null)
                {
                    db.pr_addPPTQDoc(pptqId, fileStream.FileName, fileDescription, memoryStream.ToArray(), (int)PartnerDocType.EXCEL, null, DateTime.Now, SessionSingleton.LoggedInUserId, (int)sortOrder, true).FirstOrDefault();
                }
                else
                    db.pr_modifyPPTQDoc(cfdbFile.id, cfdbFile.pptq, fileStream.FileName, cfdbFile.description, memoryStream.ToArray(), cfdbFile.doctype, null, DateTime.Now, SessionSingleton.LoggedInUserId, cfdbFile.sortOrder, cfdbFile.active);
            }
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
        public ActionResult SendData(SendDataViewModel model)
        {
            LCEModel vm = Session["LceModel"] as LCEModel;
            SendEmail objSendEmail = new SendEmail();
            var owner = db.pr_getPerson(vm.Owner).First();
            var from = new System.Net.Mail.MailAddress(owner.email, owner.firstName + " " + owner.lastName);
            var activityType = db.pr_getParnterType(vm.partnertype).First().description;
            var items101 = db.pr_getClausePersonPartnertypeSortOrderByPartnertype(vm.partnertype).ToList();

            string subject = "";
            string accessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(model.pptq).First().accesscode;

            foreach (var item in model.items)
            {
                subject = items101.Where(o => o.cfdbclause == item.id).First().clause;

                byte[] barr;
                List<AttachmentInfo> attachments = new List<AttachmentInfo>();
                switch (item.id)
                {
                    case 8:
                        var items = db.pr_getCFDBGovtPropertyReviewForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBGovtPropertyReviewForDataSend_Result>(new MemoryStream(), "CFDBGovtPropertyReviewForDataSend", items,
                            new List<string>() { "PartNumber", "TransPN", "CustomerName", "CustomerID", "SAPMasterContract", "GovtPropClauseApply", "GovtPropertyClauses" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBGovtPropertyReviewForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBGovtPropertyReviewForDataSend.xls"
                        });
                        break;
                    case 2:
                        var items2 = db.pr_getCFDBCitizenshipRestrictionForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBCitizenshipRestrictionForDataSend_Result>(new MemoryStream(), "CFDBCitizenshipRestrictionForDataSend", items2, new List<string>() { "SalesOrderNumber", "CitizenshipClauses", "CitizenshipRestrOthers", "CitizenshipRestrictionApply" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBCitizenshipRestrictionForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBCitizenshipRestrictionForDataSend.xls"
                        });
                        break;
                    case 19:
                        var items3 = db.pr_getCFDBSecurityReqsForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBSecurityReqsForDataSend_Result>(new MemoryStream(), "CFDBSecurityReqsForDataSend", items3, new List<string>() { "SecurityReqsApply", "SecurityReqsClses", "SecurityDetailsText" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBSecurityReqsForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBSecurityReqsForDataSend.xls"
                        });
                        break;
                    case 12:
                        var items4 = db.pr_getCFDBOutSourceRestrictionsForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBOutSourceRestrictionsForDataSend_Result>(new MemoryStream(), "CFDBOutSourceRestrictionsForDataSend",
                            items4, new List<string>() { "SalesOrderNumber", "Outsourceclauses", "OutsourceRestrictionsApply" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBOutSourceRestrictionsForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBOutSourceRestrictionsForDataSend.xls"
                        });
                        break;
                    case 21:
                        var items5 = db.pr_getCFDBSupplierApprovalForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBSupplierApprovalForDataSend_Result>(new MemoryStream(), "CFDBSupplierApprovalForDataSend", items5,
                            new List<string>() { "SalesOrderNumber", "SupplierApprovalApply" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBSupplierApprovalForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBSupplierApprovalForDataSend.xls"
                        });
                        break;
                    case 22:
                        var items6 = db.pr_getCFDBSupplierChgForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBSupplierChgForDataSend_Result>(new MemoryStream(), "CFDBSupplierChgForDataSend",
                            items6, new List<string>() { "SalesOrderNumber", "SupplierChgApply" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBSupplierChgForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBSupplierChgForDataSend.xls"
                        });
                        break;
                    case 49:
                        var items7 = db.pr_getCFDB3rdPartyDisclosureRestrictionsForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDB3rdPartyDisclosureRestrictionsForDataSend_Result>(new MemoryStream(), "CFDB3rdPartyDisclosureRestrictionsForDataSend", items7,
                            new List<string>() { "SalesOrderNumber", "C3rdPartyDisclosureRestrictions" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDB3rdPartyDisclosureRestrictionsForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDB3rdPartyDisclosureRestrictionsForDataSend.xls"
                        });
                        break;
                    case 20:
                        var items8 = db.pr_getCFDBSubNotConsForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBSubNotConsForDataSend_Result>(new MemoryStream(), "CFDBSubNotConsForDataSend",
                            items8, new List<string>() { "TransPN", "PartNumber", "CustomerName", "CustomerID", "SAPMasterContract",
                            "AribaID", "SubsNotConsApply", "SubsnotconsClauses", "SubsNotConsOthers"});
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBSubNotConsForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBSubNotConsForDataSend.xls"
                        });
                        break;
                    case 53:
                        var items9 = db.pr_getCFDBTINAForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBTINAForDataSend_Result>(new MemoryStream(), "CFDBTINAForDataSend", items9,
                            new List<string>() { "PartNumber", "TransPN", "CustomerName", "CustomerID", "SAPMasterContract", "TINA" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBTINAForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBTINAForDataSend.xls"
                        });
                        break;
                    case 3:
                        var items10 = db.pr_getCFDBCostActgForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBCostActgForDataSend_Result>(new MemoryStream(), "CFDBCostActgForDataSend", items10,
                            new List<string>() { "PartNumber", "TransPN", "CustomerName", "CostActgClauseapply",
                                "CostActgClause", "CostActgClauseOthers", "Allowablecostclauses", "CostActgClauseXemptDesc" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBCostActgForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBCostActgForDataSend.xls"
                        });
                        break;
                    case 52:
                        var items11 = db.pr_getCFDBAllowableCostForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBAllowableCostForDataSend_Result>(new MemoryStream(), "CFDBAllowableCostForDataSend", items11,
                            new List<string>() { "PartNumber", "TransPN", "CustomerName", "CostActgClauseapply", "CostActgClause", "CostActgClauseOthers", "Allowablecostclauses", "CostActgClauseXemptDesc" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBAllowableCostForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBAllowableCostForDataSend.xls"
                        });
                        break;
                    case 13:
                        var items12 = db.pr_getCFDBPlaceOfPerformanceForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBPlaceOfPerformanceForDataSend_Result>(new MemoryStream(), "CFDBPlaceOfPerformanceForDataSend", items12, new List<string>() {
                        "SalesOrderNumber","PlaceofPerformClses","PlaceofPerformOthers","PlaceOfPerformApply"});
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBPlaceOfPerformanceForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBPlaceOfPerformanceForDataSend.xls"
                        });
                        break;
                    case 50:
                        var items1 = db.pr_getCFDBSpecialToolingForDataSend(model.pptq).ToList();
                        barr = ExcelMapper.CreateExcel<pr_getCFDBSpecialToolingForDataSend_Result>(new MemoryStream(), "CFDBSpecialToolingForDataSend", items1,
                            new List<string>() { "PartNumber", "TransPN", "CustomerName", "CustomerID", "SAPMasterContract", "SpecialToolingclause" });
                        attachments.Add(new AttachmentInfo()
                        {
                            Content = Convert.ToBase64String(barr),
                            ContentId = "CFDBSpecialToolingForDataSend.xls",
                            Disposition = "inline",
                            Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            Filename = "CFDBSpecialToolingForDataSend.xls"
                        });
                        break;
                }

                var person = db.pr_getPerson(item.sendDataTo).FirstOrDefault();
                //var person = db.pr_getPerson(12427).FirstOrDefault();
                if (person != null)
                {
                    Email email = new Email();
                    string strEmailBody = "Hi " + person.firstName + ",<br/>";
                    // email.subject = "Clause Description";
                    email.subject = subject + " AccessCode: " + accessCode;
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
                    }, from, attachments);
                }
            }

            return Json(new { success = true });
        }

        [HttpPost]
        public ActionResult AddClause(AddPPTQTeamViewModel model)
        {
            if (model.Ids == null)
                model.Ids = new List<int>();

            var list = db.pr_getCFDBPartnertypeClauseByPartnertype(model.partnerType).Where(o => model.channel == 3 ? (o.channel == 1 || o.channel == 2) : o.channel == model.channel).ToList();
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

        public ActionResult GetClause(int id, int channel)
        {
            var allclbychannels = db.pr_getCFDBPartnertypeClauseAll().ToList().Where(o => o.channel == channel).Select(o => o.clause).ToList();
            var allClause = db.pr_getCFDBClauseAllForDisplay(id).Where(b => allclbychannels.Contains(b.id)).ToList();
            var selectedClauses = db.pr_getCFDBPartnertypeClauseByPartnertype(id).Where(o => channel == 3 ? (o.channel == 1 || o.channel == 2) : o.channel == channel).ToList();
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
                    byte[] barr;
                    List<AttachmentInfo> attachments = new List<AttachmentInfo>();
                    switch (item.id)
                    {
                        case 49:
                            var items = db.pr_getCFDB3rdPartyDisclosureRestrictions(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDB3rdPartyDisclosureRestrictions_Result>(new MemoryStream(), "3rdPartyDisclosureRestrictions", items);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "3rdPartyDisclosureRestrictions.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "3rdPartyDisclosureRestrictions.xls"
                            });
                            break;
                        case 1:
                            var items1 = db.pr_getCFDBBuyAmericanClause(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBBuyAmericanClause_Result>(new MemoryStream(), "BuyAmericanClause", items1);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "BuyAmericanClause.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "BuyAmericanClause.xls"
                            });
                            break;
                        case 2:
                            var items2 = db.pr_getCFDBCitizenshipRestriction(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBCitizenshipRestriction_Result>(new MemoryStream(), "CitizenshipRestriction", items2);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "CitizenshipRestriction.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "CitizenshipRestriction.xls"
                            });
                            break;
                        case 3:
                            var items3 = db.pr_getCFDBCostAccountClause(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBCostAccountClause_Result>(new MemoryStream(), "CostAccountClause", items3);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "CostAccountClause.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "CostAccountClause.xls"
                            });
                            break;
                        case 8:
                            var items4 = db.pr_getCFDBGovtPropClause(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBGovtPropClause_Result>(new MemoryStream(), "GovtPropClause", items4);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "GovtPropClause.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "GovtPropClause.xls"
                            });
                            break;
                        case 12:
                            var items5 = db.pr_getCFDBOutsourceRestrictions(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBOutsourceRestrictions_Result>(new MemoryStream(), "OutsourceRestrictions", items5);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "OutsourceRestrictions.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "OutsourceRestrictions.xls"
                            });
                            break;
                        case 13:
                            var items6 = db.pr_getCFDBPlaceOfPerformanceClause(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBPlaceOfPerformanceClause_Result>(new MemoryStream(), "PlaceOfPerformanceClause", items6);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "PlaceOfPerformanceClause.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "PlaceOfPerformanceClause.xls"
                            });
                            break;
                        case 19:
                            var items7 = db.pr_getCFDBSecurityReqs(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBSecurityReqs_Result>(new MemoryStream(), "SecurityReqs", items7);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "SecurityReqs.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "SecurityReqs.xls"
                            });
                            break;
                        /* case 57:
                             var items8 = db.pr_getCFDBStandardClauses(model.pptq).ToList();
                             break;*/
                        case 20:
                            var items9 = db.pr_getCFDBSubsNotCons(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBSubsNotCons_Result>(new MemoryStream(), "SubsNotCons", items9);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "SubsNotCons.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "SubsNotCons.xls"
                            });
                            break;
                        case 21:
                            var items10 = db.pr_getCFDBSupplierApproval(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBSupplierApproval_Result>(new MemoryStream(), "SupplierApproval", items10);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "SupplierApproval.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "SupplierApproval.xls"
                            });
                            break;
                        case 53:
                            var items11 = db.pr_getCFDBTina(model.pptq).ToList();
                            barr = ExcelMapper.CreateExcel<pr_getCFDBTina_Result>(new MemoryStream(), "Tina", items11);
                            attachments.Add(new AttachmentInfo()
                            {
                                Content = Convert.ToBase64String(barr),
                                ContentId = "Tina.xls",
                                Disposition = "inline",
                                Type = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                                Filename = "Tina.xls"
                            });
                            break;
                        default:
                            break;
                    }

                    var person = db.pr_getPerson(item.sendDataTo).FirstOrDefault();
                    // var personApp = db.pr_getPerson(item.getApprovalFrom).FirstOrDefault();
                    if (person != null)
                    {
                        Email email = new Email();
                        string strEmailBody = "Hi " + person.firstName + ",<br/>" +
                            "Thanks in advance for helping us on this important transition.<br/>" +
                            "Honeywell Aerospace is performing the due diligence for " + vm.ProgramName + " a " + activityType + " transition from " + vm.From + " to " + vm.To + ".<br/>" +
                            "We need you to review the attached CFDB output and respond and/or approve no later than " + (item.approvalNeededBy == null ? "" : item.approvalNeededBy.ToShortDateString()) + ".<br/>" +
                            "If you have questions or mitigating actions, please reply to this email.  Mitigating actions will be worked outside of the LC&E application.<br/>" +
                            "Please use this link " + item.text + " to record your approval.<br/><br/>" +
                            "I appreciate your assistance,<br/>" +
                            "Owner " + owner.firstName + " " + owner.lastName + "<br/><br/>" +
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
                        }, from, attachments);
                    }
                }
            }

            return Json(new { success = true, list = db.pr_getPPTQTeamRacixByPPTQ_Grid_2(138965).ToList() });
        }
    }
}