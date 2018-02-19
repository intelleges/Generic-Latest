using Generic.Helpers;
using Generic.Helpers.Utility;
using Generic.Models;
using Generic.SessionClass;
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
    public class AMLMMController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        // GET: AMLMM
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
            //var partnertype = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID);
            //ViewBag.partnertype = partnertype.Select(o => new SelectListItem()
            //{
            //    Text = o.description,
            //    Value = o.id.ToString()
            //}).ToList();

            var owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();

            ViewBag.Owner = owner;
            ViewBag.CustomerEntityReviewer = owner;
            ViewBag.CustomerEntityRelationshipReviewer = owner;
            ViewBag.TransactionReviewer = owner;


            ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).ToList(), "id", "name");
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");


            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");
            ViewBag.state = new SelectList(db.state, "id", "name");
            ViewBag.country = new SelectList(db.country, "id", "name");


            //  ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname");


        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(AMLMMModel model)
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
                
                string loadGroup = db.pr_getAccesscode().FirstOrDefault();
                 

                var pptq = db.pr_getPartnerClassByTouchpoint(model.touchpoint).First();

                
                var pptqId = pptq.id;
                

                string sheetname = "transaction";
                var map = new Dictionary<string, string>();

                map.Add("Order", "orderNumber");
                map.Add("DateTime", "orderTime");
                map.Add("Type", "Type");
                map.Add("Size", "size");
                map.Add("Symbol", "symbol");
                map.Add("Price", "orderPrice");
                map.Add("Stop/Loss (S/L)", "stopLoss");
                map.Add("Take Profit (T/P)", "takeProfit");
                map.Add("Time", "tradeTime");
                map.Add("Trade Price", "tPrice");
                map.Add("Swap", "swap");
                map.Add("Profit", "profit");
                map.Add("Country", "countryName");
                map.Add("Entity", "entityName");
                map.Add("Bank", "bankName");
                map.Add("A/C", "ac");
               
                var countries = db.pr_getCountryAll(enterpriseID).ToList();


                int i = 0;
             
                if (model.FileFinancialInstitution != null)
                {
                    ViewBag.Check = "disabled";
                    var personinExcel = ExcelMapper.GetRows<ExcelAMLMM>(model.FileFinancialInstitution.InputStream, sheetname, map).ToList();
                    foreach (var item in personinExcel)
                    {


                        if (item.orderNumber != 0)
                        {
                            if (item.Type == "sell")
                            { item.orderType = 0; }
                            else { item.orderType = 1; }

                            if (item.ac == "A")
                            { item.bankRole = 0; }
                            else { item.bankRole = 1; }
                            var countryID = countries.Where(x => x.governanceCode == item.countryName || x.code == item.countryName).FirstOrDefault();

                            if(countryID!=null)
                            {
                                item.country = countryID.id;
                            }

                            db.pr_addTransactionRecord(pptqId, item.orderNumber, item.orderTime, item.orderType, item.size, item.symbol, item.orderPrice, item.stopLoss, item.takeProfit, item.tradeTime, decimal.Parse(item.tPrice), item.swap, item.profit, item.country, item.entityName, item.bankName, item.bankRole, i, true).FirstOrDefault();

                            i++;
                        }

                    }

                   
                }
              
                Session["AMLMMModel"] = model;
                return View(model);
            }

            return View(model);
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
                    db.pr_modifyPPTQDoc(cfdbFile.id, cfdbFile.pptq, cfdbFile.title, cfdbFile.description, memoryStream.ToArray(), cfdbFile.doctype, null, DateTime.Now, SessionSingleton.LoggedInUserId, cfdbFile.sortOrder, cfdbFile.active);
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
        public ActionResult GetPartnerClass(int touchpointid)
        {

            var partnerClass = db.pr_getPartnerClassByTouchpoint(touchpointid).ToList();

            return Json(new
            {
              all =  partnerClass
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