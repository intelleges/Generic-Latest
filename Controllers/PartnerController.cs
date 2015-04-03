using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using LinqToExcel;
using Generic.ViewModel;
using Generic.Models;
using Generic.Helpers.Utility;
using Generic.Helpers;
using System.Data;
using System.Threading;
using System.Xml.Serialization;
using Generic.Helpers.PartnerHelper;
using System.Web.Routing;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Core.Objects;
using System.Text;
using Generic.Helpers.Questionnaire;
using DHTMLX.Scheduler;
using DHTMLX.Scheduler.Data;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Services;
using DHTMLX.Common;
using Telerik.Web.Mvc;
using AsyncOAuth.Evernote.Simple;
using System.Configuration;
using Thrift.Transport;
using Thrift.Protocol;
using Evernote.EDAM.UserStore;
using Evernote.EDAM.NoteStore;
using Evernote.EDAM.Type;
using System.Web.Http.Cors;
using Telerik.Web.Mvc.UI;
using jsTree3.Models;
using System.Xml.Linq;

#region Twilio References
using Twilio;
using Twilio.Mvc;
using Google.Apis.Auth.OAuth2.Mvc;
using Generic.Session;
using System.Threading.Tasks;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using HtmlAgilityPack;
#endregion

namespace Generic.Controllers
{
    [Authorize]
    [EnableCors(origins: "http://localhost:51090", headers: "*", methods: "*")]
    public class PartnerController : Controller
    {
        /// <summary>
        /// The Simple Authorizer
        /// </summary>
        readonly Generic.Helpers.EvernoteAuthorizer _evernoteAuthorizer = new Generic.Helpers.EvernoteAuthorizer(ConfigurationManager.AppSettings["Evernote.Url"], ConfigurationManager.AppSettings["Evernote.Key"], ConfigurationManager.AppSettings["Evernote.Secret"]);
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Partner/
        //1170	All	../Partner/index	1142	89	True	1	2
        public virtual ActionResult Index()
        {
            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";
            Session["partnersearch"] = arguments;
            return RedirectToAction("FindPartnerResult");

        }
        [HttpPost]
        public bool RemoveStockNumbers(string[] values, int partnerId)
        {
            var result = false;
            try
            {
                var partnerPPTQ = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partnerId).FirstOrDefault();

                if (values != null && partnerPPTQ != null)
                {
                    foreach (var value in values)
                        db.pr_removePartnumberSiteZcodePPTQQuestionResponseByPPTQPartnumber(partnerPPTQ.id, int.Parse(value));
                    //db.pr_archivePartnumber(int.Parse(value));
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }
        [HttpPost]
        public bool RemoveStockNumber(int value, int partnerId)
        {
            var result = false;
            try
            {
                var partnerPPTQ = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partnerId).FirstOrDefault();
                if (partnerPPTQ != null)
                {
                    db.pr_removePartnumberSiteZcodePPTQQuestionResponseByPPTQPartnumber(partnerPPTQ.id, value);
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }
        [HttpPost]
        public string ResetPartNumber(int partnerId, int partNumberId)
        {
            var result = "";
            try
            {
                var pptqId = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partnerId).FirstOrDefault().id;
                db.pr_resetPartnumberSiteZcodePPTQQuestionResponseByPPTQPartnumber(pptqId, partNumberId);
                var resulrValues = db.pr_getPartnumberSiteZcodePPTQByPPTQPartnumber(pptqId, partNumberId).FirstOrDefault();
                if (resulrValues != null)
                    result = string.Format("Congratulations you have reset partnumber: {0} to status: {1} for site: {2}", resulrValues.partnumber, resulrValues.status, resulrValues.site);

            }
            catch
            {
            }
            return result;
        }

        [HttpPost]
        public bool RemovePartnersQuestinnarie(string[] values)
        {
            var result = false;
            try
            {
                if (values != null)
                {
                    foreach (var value in values)
                        db.pr_removePartnerPartnertypeTouchpointQuestionnaire(int.Parse(value));
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }


        //
        // GET: /Partner/Details/5

        public virtual ActionResult Details(int id = 0)
        {
            partner partner = db.pr_getPartner(id).FirstOrDefault();
            // List<country> objCountries = db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            var pptq = partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault();
            if (pptq != null)
            {
                ViewBag.partNumbers = db.pr_getPartnumberByPPTQ(pptq.id).ToList();
            }

            try
            {
                ViewBag.CoutryName = db.pr_getCountry(partner.country).FirstOrDefault().name;
            }
            catch { }

            try
            {
                ViewBag.StateName = db.pr_getState(partner.state).FirstOrDefault().name;
            }
            catch { }

            try
            {
                var owner = db.pr_getPerson(partner.owner).FirstOrDefault();
                ViewBag.OwnerName = owner.firstName + " " + owner.lastName;
            }
            catch { }

            try
            {
                var author = db.pr_getPerson(partner.author).FirstOrDefault();
                ViewBag.AuthorName = author.firstName + " " + author.lastName;
            }
            catch { }
            try
            {
                PartnerStatus status = (PartnerStatus)partner.status;

                ViewBag.StatusName = status;
            }
            catch { }

            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";partnerID=" + partner.id + ";";
            var values = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partner.id).ToList();

            List<view_EventNotificationData> objevents = db.Database.SqlQuery<view_EventNotificationData>("EXEC pr_dynamicFiltersEventNotification  'view_EventNotificationData' , '" + arguments + "'").ToList();

            ViewBag.events = values;

            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        //
        // GET: /Partner/Create

        public ActionResult Create()
        {
            GenerateCreateDropDownLists();
            return View();
        }

        public ActionResult GetPartnerTypes(int id)
        {
            return Json(db.pr_getPartnertypeByTouchpoint(id).ToList(), JsonRequestBehavior.AllowGet);
        }


        protected void GenerateCreateDropDownLists()
        {
            ViewBag.state = new SelectList(db.state, "id", "name");
            ViewBag.country = new SelectList(db.country, "id", "name");
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");
            ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).ToList(), "id", "name");
            ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname");
            ViewBag.owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
        }
        //
        // POST: /Partner/Create

        [HttpPost]
        public ActionResult Create(partner partner, int? protocol, int? partnertype, int? touchpoint, int? group, DateTime? DueDate)
        {
            List<Tuple<int, string>> uploadedpartners = new List<Tuple<int, string>>();

            string loadGroup = db.pr_getAccesscode().FirstOrDefault();
            partner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
            GenerateCreateDropDownLists();
            try
            {
                int? PartnerId = db.pr_addPartnerSpreadsheetDataLoad(partner.internalID, partner.dunsNumber, partner.name, partner.address1, partner.address2, partner.city, partner.state.ToString(), partner.zipcode, partner.country.ToString(), partner.firstName, partner.lastName, partner.title, partner.phone, partner.email, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, partnertype, touchpoint, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, (int)PartnerStatus.Loaded, loadGroup, DueDate, group).ToList().FirstOrDefault();
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


        public ActionResult Archive(int id)
        {
            db.pr_archivePartner(id);
            //if (ModelState.IsValid)
            //{
            //    //db.Entry(partner).State = EntityState.Modified;
            //    //db.SaveChanges();
            //    db.pr_modifyPartner(partner.id, partner.enterprise, partner.internalID, partner.name, partner.address1, partner.address2, partner.city, partner.state, partner.province, partner.zipcode, partner.country, partner.phone, partner.fax, partner.firstName, partner.lastName, partner.title, partner.email, partner.dunsNumber, partner.federalID, partner.status, partner.loadHistory, partner.owner, partner.author, partner.dateApproved, partner.active, partner.lastModified);

            //    return Json(new { success = true });
            //    //return RedirectToAction("Index");
            //}
            //ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            //ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        //
        // GET: /Partner/Edit/5

        public ActionResult Edit(int id = 0)
        {
            partner partner = db.pr_getPartner(id).FirstOrDefault();
            if (partner == null)
            {
                return HttpNotFound();
            }
            //   ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            ViewBag.state = new SelectList(db.state, "id", "name", partner.state);
            ViewBag.country = new SelectList(db.country, "id", "name", partner.country);
            //ViewBag.status = db.pr_getPartnerStatusAll().Where(x => x.id == partner.status).FirstOrDefault().description;
            ViewBag.status = ((List<view_PartnerData>)Session["partner"]).Where(x => x.id == partner.id).FirstOrDefault().status;
            return View(partner);
        }

        //
        // POST: /Partner/Edit/5


        [HttpPost]
        public ActionResult Edit(partner partner)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(partner).State = EntityState.Modified;
                //db.SaveChanges();
                db.pr_modifyPartner(partner.id, Generic.Helpers.CurrentInstance.EnterpriseID, partner.internalID, partner.name, partner.address1, partner.address2, partner.city, partner.state, partner.province, partner.zipcode, partner.country, partner.phone, partner.fax, partner.firstName, partner.lastName, partner.title, partner.email, partner.dunsNumber, partner.federalID, partner.status, partner.loadHistory, partner.owner, partner.author, partner.dateApproved, partner.active, partner.lastModified);
                var quest = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partner.id).FirstOrDefault();

                try
                {
                    var id = (int)Areas.RegistrationArea.Controllers.HomeController.FillPdfHtml(ViewBag, db, Session, Server);
                    string htmltext = this.RenderActionResultToString(this.View("~/Areas/RegistrationArea/Views/Home/CustomizedQuestionnaireSurveyPdfDownload.cshtml"));
                    var bytes = (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(htmltext);
                    db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire(quest.id, quest.partner, quest.partnerTypeTouchpointQuestionnaire, quest.accesscode, quest.invitedBy, quest.invitedDate, quest.completedDate, quest.status, 100, quest.zcode, bytes, quest.docFolderAddress, quest.score, quest.loadGroup);

                }
                catch (Exception ex)
                {
                }
                // }
                return Json(new { success = true });

            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            return View(partner);
        }
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

        //
        // GET: /Partner/Delete/5

        public ActionResult Delete(int id = 0)
        {
            partner partner = db.pr_getPartner(id).FirstOrDefault();
            if (partner == null)
            {
                return HttpNotFound();
            }
            return View(partner);
        }

        //
        // POST: /Partner/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            partner partner = db.pr_getPartner(id).FirstOrDefault();
            db.partner.Remove(partner);
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        public ActionResult AddPartnerPartnertypeTouchpointQuestionnaire()
        {
            // partnerPartnertypeTouchpointQuestionnaire 

            //   db.pr_addPartnerPartnertypeTouchpointQuestionnaire(

            return View();
        }

        public ActionResult UploadPartner(bool defaultTouchpoint = false)
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.group = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            //ViewBag.title = db.touchpoint.First().title;
            if (defaultTouchpoint)
            {
                ViewBag.defaultTouchpoint = "true";
                ViewBag.defaultTouchpointValue = SessionSingleton.Touchpoint;
                ViewBag.defaultTouchpointName = db.pr_getTouchpoint(SessionSingleton.Touchpoint).FirstOrDefault().title;
            }

            return View();
        }

        [HttpPost]
        public ActionResult UploadPartner(int protocol, int partnertype, int touchpoint, int group, HttpPostedFileBase uploadPartner)
        {


            if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
            }

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles/Partner"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/Partner"));
            }

            // The Name of the Upload component is "attachments" 
            var file = uploadPartner;

            // Some browsers send file names with full path. This needs to be stripped.
            var fileName = Path.GetFileName(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/Partner"), fileName);

            // The files are not actually saved in this demo
            file.SaveAs(physicalPath);

            int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;

            string sheetname = "Sheet1";
            var excelRead = new ExcelQueryFactory(physicalPath.ToString());

            //excelRead.AddMapping<ExcelPartner>(x => x.internalID, "HON Internal ID");
            //excelRead.AddMapping<ExcelPartner>(x => x.name, "Provider Name");
            //excelRead.AddMapping<ExcelPartner>(x => x.address1, "Provider Address 1");

            //excelRead.AddMapping<ExcelPartner>(x => x.city, "Provider City");
            //excelRead.AddMapping<ExcelPartner>(x => x.StateName, "Provider State");

            //excelRead.AddMapping<ExcelPartner>(x => x.CountryName, "Provider Country");

            //excelRead.AddMapping<ExcelPartner>(x => x.email, "Provider Contact Email");
            //excelRead.AddMapping<ExcelPartner>(x => x.firstName, "Provider Contact First Name");
            //excelRead.AddMapping<ExcelPartner>(x => x.lastName, "Provider Contact Last Name");





            excelRead.AddMapping<ExcelPartner>(x => x.internalID, "PARTNER_INTERNAL_ID");
            excelRead.AddMapping<ExcelPartner>(x => x.name, "PARTNER_NAME");
            excelRead.AddMapping<ExcelPartner>(x => x.address1, "PARTNER_ADDRESS_ONE");
            excelRead.AddMapping<ExcelPartner>(x => x.address2, "PARTNER_ADDRESS_TWO");
            excelRead.AddMapping<ExcelPartner>(x => x.city, "PARTNER_CITY");
            excelRead.AddMapping<ExcelPartner>(x => x.StateName, "PARTNER_STATE");
            excelRead.AddMapping<ExcelPartner>(x => x.province, "PARTNER_PROVINCE");
            excelRead.AddMapping<ExcelPartner>(x => x.zipcode, "PARTNER_ZIPCODE");
            excelRead.AddMapping<ExcelPartner>(x => x.CountryName, "PARTNER_COUNTRY");
            excelRead.AddMapping<ExcelPartner>(x => x.phone, "PARTNER_POC_PHONE_NUMBER");
            excelRead.AddMapping<ExcelPartner>(x => x.fax, "PARTNER_CONTACT_FAX");
            excelRead.AddMapping<ExcelPartner>(x => x.email, "PARTNER_POC_EMAIL_ADDRESS");
            excelRead.AddMapping<ExcelPartner>(x => x.firstName, "PARTNER_POC_FIRST_NAME");
            excelRead.AddMapping<ExcelPartner>(x => x.lastName, "PARTNER_POC_LAST_NAME");
            excelRead.AddMapping<ExcelPartner>(x => x.title, "PARTNER_POC_TITLE");
            excelRead.AddMapping<ExcelPartner>(x => x.dunsNumber, "PARTNER_DUNS");
            excelRead.AddMapping<ExcelPartner>(x => x.federalID, "POC EID");
            excelRead.AddMapping<ExcelPartner>(x => x.PARTNER_SAP_ID, "PARTNER_SAP_ID");
            excelRead.AddMapping<ExcelPartner>(x => x.RO_FIRST_NAME, "RO_FIRST_NAME");
            excelRead.AddMapping<ExcelPartner>(x => x.RO_LAST_NAME, "RO_LAST_NAME");
            excelRead.AddMapping<ExcelPartner>(x => x.RO_EMAIL, "RO_EMAIL");


            //   var columnnames = excelRead.GetColumnNames(sheetname);
            var partnerinExcel = from a in excelRead.Worksheet<ExcelPartner>(sheetname) select a;

            //partnerTypeTouchpointQuestionnaire objptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnerType(partnertype).ToList().Where(x => x.touchpoint == touchpoint).FirstOrDefault();

            //int i = 1;

            //List<int> uploadedpartners = new List<int>();
            List<Tuple<int, string>> uploadedpartners = new List<Tuple<int, string>>();


            string loadGroup = db.pr_getAccesscode().FirstOrDefault();
            int countPartners = 0;

            foreach (var partners in partnerinExcel.ToList())
            {
                if (partners.internalID != null)
                {

                    var objstateSpreadSheet = db.pr_getStateByStateCode(partners.StateName).FirstOrDefault();
                    string stateIdSpreadSheet = null;
                    if (objstateSpreadSheet != null)
                    {
                        stateIdSpreadSheet = objstateSpreadSheet.id.ToString();
                    }

                    var objCountrySpreadSheet = db.pr_getCountryByName(partners.CountryName).FirstOrDefault();
                    string countryIdSpreadsheet = null;
                    if (objCountrySpreadSheet != null)
                    {
                        countryIdSpreadsheet = objCountrySpreadSheet.id.ToString();
                    }

                    using (var context = new EntitiesDBContext())
                    {
                        try
                        {
                            var _person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id;
                            var _enterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;
                            int? PartnerId = context.pr_addPartnerSpreadsheetDataLoad(
                                partners.internalID,
                                partners.PARTNER_SAP_ID,
                                partners.name,
                                partners.address1 != null ? partners.address1.Length > 20 ? partners.address1.Substring(0, 20) : partners.address1 : null,
                                partners.address2 != null ? partners.address2.Length > 20 ? partners.address2.Substring(0, 20) : partners.address2 : null,
                                partners.city,
                                stateIdSpreadSheet,
                                partners.zipcode != null ? partners.zipcode.Length > 20 ? partners.zipcode.Substring(0, 20) : partners.zipcode : null,
                                countryIdSpreadsheet,
                                partners.firstName,
                                partners.lastName,
                                partners.title,
                                partners.phone != null ? partners.phone.Length > 20 ? partners.phone.Substring(0, 20) : partners.phone : null,
                                partners.email,
                                partners.RO_FIRST_NAME,
                                partners.RO_LAST_NAME,
                                partners.RO_EMAIL,
                                DateTime.Now,
                                _enterpriseID,
                                partnertype,
                                touchpoint,
                                _person,
                                null,
                                loadGroup,
                                partners.DUE_DATE,
                                group).ToList().FirstOrDefault();
                            uploadedpartners.Add(new Tuple<int, string>(int.Parse(PartnerId.ToString()), ""));
                        }
                        catch
                        {
                        }

                    }


                    //partner checkPartner = new partner();
                    //using (var context = new EntitiesDBContext())
                    //{
                    //    checkPartner = db.pr_getPartnerByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, partners.email).FirstOrDefault();
                    //    partners.id = checkPartner.id;
                    //}

                    //if (checkPartner == null)
                    //{
                    //    partner objPartner = new partner();
                    //    objPartner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                    //    objPartner.active = true;
                    //    objPartner.internalID = partners.internalID;
                    //    objPartner.name = partners.name;
                    //    objPartner.address1 = partners.address1;
                    //    objPartner.address2 = partners.address2;
                    //    objPartner.city = partners.city;
                    //    objPartner.state = db.pr_getStateByStateCode(partners.StateName).FirstOrDefault().id;
                    //    objPartner.province = partners.province;
                    //    objPartner.zipcode = partners.zipcode;
                    //    objPartner.country = db.pr_getCountryByName(partners.CountryName).FirstOrDefault().id;
                    //    objPartner.phone = partners.phone;
                    //    objPartner.fax = partners.fax;
                    //    objPartner.email = partners.email;
                    //    objPartner.firstName = partners.firstName;
                    //    objPartner.lastName = partners.lastName;
                    //    objPartner.title = partners.title;
                    //    objPartner.dunsNumber = partners.dunsNumber;
                    //    objPartner.federalID = partners.federalID;

                    //    db.partner.Add(objPartner);
                    //    db.SaveChanges();

                    //    uploadedpartners.Add(objPartner.id);
                    //}
                    //else
                    //{
                    //    uploadedpartners.Add(partners.id);
                    //}

                    //string accessCode = db.pr_getAccesscode().FirstOrDefault();

                    //int pqt = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;

                    //using (var context = new EntitiesDBContext())
                    //{
                    //    context.pr_addPartnerPartnertypeTouchpointQuestionnaire(partners.id, pqt, accessCode, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, DateTime.Now, null, partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE, null, null, null, null, null);
                    //}
                }
            }
            Session["uploadedpartnerList"] = uploadedpartners;
            Session["partnertype"] = partnertype;
            Session["touchpoint"] = touchpoint;
            Session["loadGroup"] = loadGroup;
            //ViewBag.Message = "1";

            var Target = db.touchpoint.Where(x => x.id == (touchpoint)).ToList();
            ViewBag.Message = Target[0].target.ToString();
            if (Target[0].target.ToString() == "2")
            {
                ViewBag.MessageDetail = "Congratulations, you have uploaded " + uploadedpartners.Count + " to " + Target[0].title;
            }

            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.group = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.touchpointTitle = "You have successfully added " + uploadedpartners.Count + " Partners to the " + db.touchpoint.Single(p => p.id == touchpoint).title + " Touchpoint";
            return View();
        }



        [HttpPost]
        public ActionResult InvitePartners()
        {
            int partnertype = (int)Session["partnertype"];

            int touchpoint = (int)Session["touchpoint"];

            string loadGroup = (string)Session["loadGroup"];


            string message = string.Empty;
            if (Session["loadGroup"] != null)
            {
                //  List<Tuple<int, string>> uploadedpartnerList = (List<Tuple<int, string>>)Session["uploadedpartnerList"];
                //  List<int> uploadedpartnerList = (List<int>)Session["uploadedpartnerList"];

                var objPartners = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).ToList();

                int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).LastOrDefault().id;
                // db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartnertypeTouchpointQuestionnaire
                // db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire()
                foreach (var partnerItem in objPartners)
                {


                    var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerItem.partner, ptq).FirstOrDefault();
                    pptq.invitedDate = DateTime.Now;
                    var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
                    pptq.invitedBy = person.id;
                    pptq.status = (int)PartnerStatus.Invited_NoResponse;
                    db.Entry(pptq).State = EntityState.Modified;
                    db.SaveChanges();

                    var objpartner = db.pr_getPartner(partnerItem.partner).FirstOrDefault();
                    objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                    db.Entry(objpartner).State = EntityState.Modified;
                    db.SaveChanges();

                    var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();
                    amm.text.Replace("[partner Access Code]", partnerItem.accesscode);

                    var objpartnerByAccessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireDueDateByAccessCode(partnerItem.accesscode, loadGroup).FirstOrDefault();

                    //if (objpartnerByAccessCode != null)
                    //{

                    //    amm.text = amm.text.Replace("[Due Date]", objpartnerByAccessCode.Value.ToString("MMM, dd, yyyy"));
                    //}

                    var objtouchpoint = db.pr_getTouchpoint(touchpoint).FirstOrDefault();
                    Email email = new Email(amm);

                    if (Session["loadgroup"] != null)
                    {
                        email.loadgroup = Session["loadgroup"].ToString();
                    }
                    email.accesscode = partnerItem.accesscode;
                    email.protocolTouchpoint = objtouchpoint.description;

                    EmailFormat emailFormat = new EmailFormat();
                    email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
                    email.emailTo = objpartner.email;
                    SendEmail objSendEmail = new SendEmail();
                    objSendEmail.sendEmail(email);



                }

                message = "Invite Sent";
                ViewBag.Message = "2";
            }
            else
            {
                message = "No invite sent";
                ViewBag.Message = "1";
            }


            //Thread.Sleep(5000);

            List<pr_getEventNotificationByLoadGroup_Result> objEventNotification = new List<pr_getEventNotificationByLoadGroup_Result>();
            if (Session["loadgroup"] != null)
            {
                objEventNotification = db.pr_getEventNotificationByLoadGroup(Session["loadgroup"].ToString()).ToList();
            }

            return Json(new { Data = new { message = message }, EventNotification = objEventNotification }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InvitePartnersList()
        {
            string message = "";
            List<pr_getEventNotificationByLoadGroup_Result> objEventNotification = new List<pr_getEventNotificationByLoadGroup_Result>();
            //objEventNotification[0].timestamp
            // objEventNotification[0].loadgroup
            if (Session["loadgroup"] != null)
            {
                objEventNotification = db.pr_getEventNotificationByLoadGroup(Session["loadgroup"].ToString()).ToList();
            }

            return Json(new { Data = new { message = message }, EventNotification = objEventNotification.Select(x => new { x.@event, x.accesscode, x.email, x.loadgroup, x.protocolTouchpoint, x.reason, timestamp = x.timestamp.ToString() }) }, JsonRequestBehavior.AllowGet);

        }


        public ActionResult ExportExcel()
        {


            string arguments = Session["partnersearch"].ToString() + "active=1;";
            Session["partner"] = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
            List<view_PartnerData> abc = (List<view_PartnerData>)Session["partner"];





            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<view_PartnerData>));


            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, abc);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "PartnerList.xls");


        }
        public ActionResult InvitePartnersListDownload()
        {

            List<pr_getEventNotificationByLoadGroup_Result> objEventNotification = new List<pr_getEventNotificationByLoadGroup_Result>();
            //objEventNotification[0].timestamp
            // objEventNotification[0].loadgroup
            if (Session["loadgroup"] != null)
            {
                objEventNotification = db.pr_getEventNotificationByLoadGroup(Session["loadgroup"].ToString()).ToList();
            }

            //SimpleExcelExport.ExportToExcel objExport = new SimpleExcelExport.ExportToExcel();
            //var result = objExport.ListToExcel(objEventNotification);
            //return new FileContentResult(result, "application/vnd.ms-excel") { FileDownloadName = "InviteList.xls" };




            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<ExcelEventNotification>));

            List<ExcelEventNotification> objEvents = new List<ExcelEventNotification>();
            objEvents = objEventNotification.Select(x => new ExcelEventNotification { Email = x.email, AccessCode = x.accesscode, LoadGroup = x.loadgroup, ProtocolTouchpoint = x.protocolTouchpoint, Reason = x.reason, Date = x.timestamp, Event = x.@event }).ToList();

            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, objEvents);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "InviteList.xls");

            //  return Json(new { Data = new { message = message }, EventNotification = objEventNotification }, JsonRequestBehavior.AllowGet);

        }


        [HttpPost]
        public ActionResult InvitePartnersLater()
        {
            int partnertype = (int)Session["partnertype"];
            int touchpoint = (int)Session["touchpoint"];
            //partnerpartnertypeTouchpointQustionnaire 
            string message = string.Empty;
            if (Session["uploadedpartnerList"] != null)
            {
                List<int> uploadedpartnerList = (List<int>)Session["uploadedpartnerList"];

                int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;
                // db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartnertypeTouchpointQuestionnaire
                // db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire()
                foreach (int partnerId in uploadedpartnerList.Distinct())
                {


                    var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerId, ptq).FirstOrDefault();
                    //   pptq.invitedDate = DateTime.Now;
                    //  var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
                    //  pptq.invitedBy = person.id;

                    pptq.status = (int)Generic.Helpers.PartnerHelper.PartnerStatus.Hold;

                    db.Entry(pptq).State = EntityState.Modified;
                    db.SaveChanges();

                    //var objpartner = db.pr_getPartner(partnerId).FirstOrDefault();
                    //objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                    //db.Entry(objpartner).State = EntityState.Modified;
                    //db.SaveChanges();

                    //var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();

                    //var objtouchpoint = db.pr_getTouchpoint(touchpoint).FirstOrDefault();
                    //Email email = new Email(amm);
                    //EmailFormat emailFormat = new EmailFormat();
                    //email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
                    //email.emailTo = objpartner.email;
                    //SendEmail objSendEmail = new SendEmail();
                    //objSendEmail.sendEmail(email);



                }

                message = "Invite Not Sent";
                //ViewBag.Message = "2";
            }
            else
            {
                // message = "No invite sent";
                // ViewBag.Message = "1";
            }
            return Json(new { Data = new { message = message } }, JsonRequestBehavior.AllowGet);
        }


        public string Invite(int partnerId, int ptq)
        {


            //int ptq = 30; db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;
            // db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartnertypeTouchpointQuestionnaire
            // db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire()

            var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerId, ptq).FirstOrDefault();
            pptq.invitedDate = DateTime.Now;
            var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
            pptq.invitedBy = person.id;
            db.Entry(pptq).State = EntityState.Modified;
            db.SaveChanges();

            var objpartner = db.pr_getPartner(partnerId).FirstOrDefault();
            objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
            db.Entry(objpartner).State = EntityState.Modified;
            db.SaveChanges();

            var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();

            var objptq = db.pr_getPartnertypeTouchpointQuestionnaire(ptq).FirstOrDefault();

            var objtouchpoint = db.pr_getTouchpoint(objptq.touchpoint).FirstOrDefault();
            Email email = new Email(amm);
            EmailFormat emailFormat = new EmailFormat();
            email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
            email.emailTo = objpartner.email;
            SendEmail objSendEmail = new SendEmail();
            objSendEmail.sendEmail(email);


            string message = "Invite Sent";
            ViewBag.Message = "2";

            return "Sent";
        }

        public ActionResult UploadPartNumber()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.group = new SelectList(db.pr_getGroupByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            return View();
        }
        [HttpPost]
        public ActionResult UploadPartNumber(int protocol, int partnertype, int touchpoint, int group, HttpPostedFileBase uploadPartNumber)
        {

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
            }

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles/uploadPartNumber"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/uploadPartNumber"));
            }

            // The Name of the Upload component is "attachments" 
            var file = uploadPartNumber;

            // Some browsers send file names with full path. This needs to be stripped.
            var fileName = Path.GetFileName(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/uploadPartNumber"), fileName);

            // The files are not actually saved in this demo
            file.SaveAs(physicalPath);

            int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;

            string sheetname = "upload";
            var excelRead = new ExcelQueryFactory(physicalPath.ToString());

            excelRead.AddMapping<ExcelPartnumber>(x => x.internalID, "PARTNER_INTERNAL_ID");
            excelRead.AddMapping<ExcelPartnumber>(x => x.dunsNumber, "PARTNER_SAP_ID");
            excelRead.AddMapping<ExcelPartnumber>(x => x.name, "PARTNER_NAME");
            excelRead.AddMapping<ExcelPartnumber>(x => x.address1, "PARTNER_ADDRESS_ONE");
            excelRead.AddMapping<ExcelPartnumber>(x => x.address2, "PARTNER_ADDRESS_TWO");
            excelRead.AddMapping<ExcelPartnumber>(x => x.city, "PARTNER_CITY");
            excelRead.AddMapping<ExcelPartnumber>(x => x.StateName, "PARTNER_STATE");

            excelRead.AddMapping<ExcelPartnumber>(x => x.zipcode, "PARTNER_ZIPCODE");
            excelRead.AddMapping<ExcelPartnumber>(x => x.CountryName, "PARTNER_COUNTRY");
            excelRead.AddMapping<ExcelPartnumber>(x => x.phone, "PARTNER_POC_PHONE_NUMBER");

            excelRead.AddMapping<ExcelPartnumber>(x => x.email, "PARTNER_POC_EMAIL_ADDRESS");
            excelRead.AddMapping<ExcelPartnumber>(x => x.firstName, "PARTNER_POC_FIRST_NAME");
            excelRead.AddMapping<ExcelPartnumber>(x => x.lastName, "PARTNER_POC_LAST_NAME");
            excelRead.AddMapping<ExcelPartnumber>(x => x.title, "PARTNER_POC_TITLE");







            //  var columnnames = excelRead.GetColumnNames(sheetname);
            var partnerinExcel = from a in excelRead.Worksheet<ExcelPartnumber>(sheetname) select a;

            //partnerTypeTouchpointQuestionnaire objptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnerType(partnertype).ToList().Where(x => x.touchpoint == touchpoint).FirstOrDefault();

            //int i = 1;

            List<Tuple<int, string>> uploadedpartners = new List<Tuple<int, string>>();

            //uploadedpartners.Add(new Tuple<int, string>(1, "1"));
            //uploadedpartners.ToList().FirstOrDefault().Item1;

            string loadGroup = db.pr_getAccesscode().FirstOrDefault();
            int countpartNumbers = partnerinExcel.Count();
            int recordNumber = 1;
            foreach (var partnumbersItem in partnerinExcel.ToList())
            {
                if (partnumbersItem.internalID != null)
                {
                    if (partnumbersItem.PARTNER_SAP_ID == null || partnumbersItem.dunsNumber == null || partnumbersItem.PART_NUMBER_INTERNAL == null || partnumbersItem.PART_NUMBER_SAP == null)
                    {
                        ErrorView objerrorView = new ErrorView();
                        objerrorView.errorMessage = "Record " + recordNumber.ToString() + " of " + countpartNumbers + " has invalid values.";
                        return PartialView("_Error", objerrorView);
                    }
                }
                recordNumber++;
            }

            foreach (var partnumbers in partnerinExcel.ToList())
            {

                var objstateSpreadSheet = db.pr_getStateByStateCode(partnumbers.StateName).FirstOrDefault();
                string stateIdSpreadSheet = null;
                if (objstateSpreadSheet != null)
                {
                    stateIdSpreadSheet = objstateSpreadSheet.id.ToString();
                }

                var objCountrySpreadSheet = db.pr_getCountryByName(partnumbers.CountryName).FirstOrDefault();
                string countryIdSpreadsheet = null;
                if (objCountrySpreadSheet != null)
                {
                    countryIdSpreadsheet = objCountrySpreadSheet.id.ToString();
                }

                if (partnumbers.internalID != null)
                {
                    using (var context = new EntitiesDBContext())
                    {

                        var PartnerID = context.pr_addPartnumberSpreadsheetDataLoad(partnumbers.internalID, partnumbers.dunsNumber, partnumbers.name, partnumbers.address1, partnumbers.address2, partnumbers.city, stateIdSpreadSheet, partnumbers.zipcode, countryIdSpreadsheet, partnumbers.firstName, partnumbers.lastName, partnumbers.title, partnumbers.phone, partnumbers.email, partnumbers.INTERNAL_SITE_ID, partnumbers.SAP_SITE, partnumbers.SAP_PLANT_CODE, partnumbers.SITE_NAME, partnumbers.PART_NUMBER_SAP, partnumbers.PART_NUMBER_INTERNAL, partnumbers.SUB_COMMODITY_OWNER, partnumbers.CENTER_OF_EXCELLENCE, partnumbers.RO_FIRST_NAME, partnumbers.RO_LAST_NAME, partnumbers.RO_EMAIL, DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, partnertype, touchpoint, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, null, loadGroup, partnumbers.DUE_DATE, group).ToList().FirstOrDefault();
                        uploadedpartners.Add(new Tuple<int, string>(int.Parse(PartnerID.ToString()), partnumbers.PARTNER_SAP_ID));
                    }
                }

                //if (partnumbers.internalID != null)
                //{
                //    partner checkPartner = new partner();
                //    using (var context = new EntitiesDBContext())
                //    {
                //        checkPartner = db.pr_getPartnerByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, partnumbers.email).FirstOrDefault();

                //    }
                //    int partnerID = 0;
                //    if (checkPartner == null)
                //    {
                //        partner objPartner = new partner();i have 

                //        objPartner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                //        objPartner.active = true;
                //        objPartner.internalID = partnumbers.internalID;
                //        objPartner.name = partnumbers.name;
                //        objPartner.address1 = partnumbers.address1;
                //        objPartner.address2 = partnumbers.address2;
                //        objPartner.city = partnumbers.city;
                //        var objstate = db.pr_getStateByStateCode(partnumbers.StateName).FirstOrDefault();
                //        if (objstate != null)
                //        {
                //            objPartner.state = objstate.id;
                //        }

                //        objPartner.province = partnumbers.province;
                //        objPartner.zipcode = partnumbers.zipcode;

                //        var objCountry = db.pr_getCountryByName(partnumbers.CountryName).FirstOrDefault();
                //        if (objCountry != null)
                //        {
                //            objPartner.country = objCountry.id;
                //        }


                //        objPartner.phone = partnumbers.phone;
                //        objPartner.fax = partnumbers.fax;
                //        objPartner.email = partnumbers.email;
                //        objPartner.firstName = partnumbers.firstName;
                //        objPartner.lastName = partnumbers.lastName;
                //        objPartner.title = partnumbers.title;
                //        objPartner.dunsNumber = partnumbers.dunsNumber;
                //        objPartner.federalID = partnumbers.federalID;
                //        objPartner.dunsNumber = partnumbers.dunsNumber;
                //        objPartner.active = true;
                //        objPartner.enterprise = EnterpriseID;
                //        db.partner.Add(objPartner);
                //        db.SaveChanges();

                //        uploadedpartners.Add(objPartner.id);
                //        partnerID = objPartner.id;
                //    }
                //    else
                //    {
                //        partnerID = checkPartner.id;
                //        uploadedpartners.Add(partnerID);
                //    }

                //    string accessCode = db.pr_getAccesscode().FirstOrDefault();

                //    int pqt = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;
                //    int pptqID = 0;
                //    using (var context = new EntitiesDBContext())
                //    {
                //        context.Configuration.ValidateOnSaveEnabled = true;
                //        var pptq = new partnerPartnertypeTouchpointQuestionnaire();
                //        var objCheckpptq = context.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerID, pqt).FirstOrDefault();
                //        if (objCheckpptq == null)
                //        {

                //            pptq.partner = partnerID;
                //            pptq.partnerTypeTouchpointQuestionnaire = pqt;
                //            pptq.invitedBy = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id;
                //            pptq.accesscode = accessCode;
                //            pptq.invitedDate = DateTime.Now;
                //            pptq.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                //            context.partnerPartnertypeTouchpointQuestionnaire.Add(pptq);
                //            context.SaveChanges();
                //            pptqID = pptq.id;
                //        }
                //        else
                //        {
                //            pptqID = objCheckpptq.id;
                //        }
                //        //try
                //        //{
                //        //    context.SaveChanges();
                //        //}
                //        //catch (OptimisticConcurrencyException)
                //        //{

                //        //   // context.Refresh(RefreshMode.ClientWins, db.Articles);
                //        //    context.SaveChanges();
                //        //}

                //       // context.pr_addPartnerPartnertypeTouchpointQuestionnaire(partnerID, pqt, accessCode, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, DateTime.Now, null, null, null, null, null, null);
                //    }

                //    var checkSite = db.pr_getSiteByInternalID(EnterpriseID, partnumbers.INTERNAL_SITE_ID).FirstOrDefault();
                //    int siteID = 0;
                //    if (checkSite == null)
                //    {
                //        using (var context = new EntitiesDBContext())
                //        {
                //            site objsite = new site();
                //            objsite.description = partnumbers.SITE_NAME;
                //            objsite.sapID = partnumbers.SAP_SITE;
                //            objsite.internalID = partnumbers.INTERNAL_SITE_ID;
                //            objsite.sortOrder = 1;
                //            objsite.active = true;
                //            objsite.enterprise = EnterpriseID;
                //            context.site.Add(objsite);
                //            context.SaveChanges();
                //            siteID = objsite.id;
                //        }

                //    }
                //    else
                //    {
                //        siteID = checkSite.id;
                //    }

                //    int partNumberID = 0;

                //    var checkPartNumber = db.pr_getPartnumberByInternalID(EnterpriseID, partnumbers.PART_NUMBER_INTERNAL).FirstOrDefault();

                //    if (checkPartNumber == null)
                //    {
                //        partnumber objPartNumber = new partnumber();
                //        objPartNumber.sapID = partnumbers.PART_NUMBER_SAP;
                //        objPartNumber.description = partnumbers.PART_NUMBER_SAP;
                //        objPartNumber.internalId = partnumbers.PART_NUMBER_INTERNAL;
                //        objPartNumber.active = true;
                //        objPartNumber.partner = partnerID;
                //        db.partnumber.Add(objPartNumber);
                //        db.SaveChanges();
                //        partNumberID = objPartNumber.id;

                //    }
                //    else
                //    {
                //        partNumberID = checkPartNumber.id;
                //    }
                //    using (var context2 = new EntitiesDBContext())
                //    {
                //      //  context2.Configuration.ValidateOnSaveEnabled = true;
                //        context2.pr_addPartnumberSiteZcodePPTQ(partNumberID, siteID, string.Empty, Helpers.PartNumberHelper.Status.NOT_STARTED , pptqID);
                //    }






                //  }
            }

            //List<partner> objPartners = db.pr_getPartnerForPartnumberEmailSendByLoadGroup(loadGroup).ToList();
            //foreach (partner item in objPartners)
            //{
            //    uploadedpartners.Add(item.id);
            //}


            Session["uploadedpartnerList"] = uploadedpartners;
            Session["partnertype"] = partnertype;
            Session["touchpoint"] = touchpoint;
            Session["loadgroup"] = loadGroup;

            ViewBag.Message = "1";


            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.group = new SelectList(db.pr_getGroupByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");


            return View();
        }



        public ActionResult PartnumberSpreadsheetDataLoadReportDownload()
        {


            List<pr_getPartnumberSpreadsheetDataLoadReport_Result> objReport = new List<pr_getPartnumberSpreadsheetDataLoadReport_Result>();

            objReport = db.pr_getPartnumberSpreadsheetDataLoadReport(0, 0).ToList();


            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<pr_getPartnumberSpreadsheetDataLoadReport_Result>));


            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, objReport);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "DataLoadReport.xls");

        }
        public ActionResult EventNotificationBounceReportDownload()
        {


            List<pr_getEventNotificationBounce_Result1> objReport = new List<pr_getEventNotificationBounce_Result1>();

            objReport = db.pr_getEventNotificationBounce(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();


            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<pr_getEventNotificationBounce_Result1>));


            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, objReport);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "DataLoadReport.xls");

        }

        public void ResponsesByProtocolTouchpointGroupPartnertype2Download()
        {
            int qId = 0;
            try
            {
                qId = db.pr_getPartnertypeTouchpointQuestionnaireByTouchpoint(SessionSingleton.Touchpoint).FirstOrDefault().questionnaire;
            }
            catch { }
            int levelType = 1;
            try
            {
                levelType = int.Parse(db.pr_getQuestionnaire(qId).FirstOrDefault().levelType.ToString());
            }
            catch { }
            if (levelType == 1)
            {

                ExportToExcel(getResponsesByProtocolCampaignGroupProviderType2(db.pr_getProtocolByTouchpoint(SessionSingleton.Touchpoint).FirstOrDefault().id, SessionSingleton.Touchpoint));
            }
            else
            {
                ExportToExcel(getCustomizedLSMWReport());
            }

        }

        public void ExportToExcel(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                string filename = "DownloadExcel.xls";
                System.IO.StringWriter tw = new System.IO.StringWriter();
                System.Web.UI.HtmlTextWriter hw = new System.Web.UI.HtmlTextWriter(tw);
                DataGrid dgGrid = new DataGrid();
                dgGrid.DataSource = dt;
                dgGrid.DataBind();

                //Get the HTML for the control.
                dgGrid.RenderControl(hw);
                //Write the HTML back to the browser.
                //Response.ContentType = application/vnd.ms-excel;
                Response.ContentType = "application/vnd.ms-excel";
                Response.AppendHeader("Content-Disposition", "attachment; filename=" + filename + "");

                Response.Write(tw.ToString());
                Response.End();
            }
        }

        /// <summary>
        /// Return a campaign object
        /// </summary>
        /// <returns></returns>
        public DataTable getResponsesByProtocolCampaignGroupProviderType2(int protocol, int touchpoint)
        {
            DataTable dataTable = new DataTable();

            SqlConnection conn = new SqlConnection("data source=50.56.237.192;initial catalog=hs3MVCMTQa2;user id=dev;password=I>pkP8s|n1;");
            
            conn.Open();
            SqlCommand command = new SqlCommand("pr_getResponsesByProtocolTouchpointGroupPartnertype2", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@protocol", SqlDbType.VarChar).Value = protocol;
            command.Parameters.Add("@touchpoint", SqlDbType.VarChar).Value = touchpoint;
            command.Parameters.Add("@group", SqlDbType.VarChar).Value = DBNull.Value;
            command.Parameters.Add("@partnertype", SqlDbType.VarChar).Value = DBNull.Value;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            sqlDataAdapter.Fill(dataTable);
            conn.Close();
            return dataTable;
        }


        public DataTable getCustomizedLSMWReport()
        {
            DataTable dataTable = new DataTable();

            SqlConnection conn = new SqlConnection("data source=50.56.237.192;initial catalog=hs3MVCMTQa2;user id=dev;password=I>pkP8s|n1;");
            conn.Open();
            //db.pr_getcustomizedLSMWReportByPartnumberSiteZcodePPTQ
            SqlCommand command = new SqlCommand("[honeywellBAA].[pr_getcustomizedLSMWReport]", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@enterprise", SqlDbType.VarChar).Value = Generic.Helpers.CurrentInstance.EnterpriseID;
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            sqlDataAdapter.Fill(dataTable);
            conn.Close();
            return dataTable;
        }


        public ActionResult PartnumberSpreadsheetDataLoadReportDownloadForSecondReport()
        {

            List<pr_getPartnerStatusByEnterprise_Result> objReport = new List<pr_getPartnerStatusByEnterprise_Result>();

            var test = db.pr_getPartnerStatusByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            objReport = test;

            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<pr_getPartnerStatusByEnterprise_Result>));


            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, objReport);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "DataLoadReport.xls");
        }
        public ActionResult PartnumberSpreadsheetDataLoadReportDownloadForReport3()
        {

            List<pr_getPartnerStatusByEnterpriseAll_Result> objReport = new List<pr_getPartnerStatusByEnterpriseAll_Result>();

            var test = db.pr_getPartnerStatusByEnterpriseAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            objReport = test;

            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<pr_getPartnerStatusByEnterpriseAll_Result>));


            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, objReport);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "Report3.xls");
        }

        public ActionResult PartnumberSpreadsheetDataLoadReportDownloadForAgingReport()
        {
            XmlSerializer serializer = null;
            var stream = new MemoryStream();
            db.Database.CommandTimeout = 180;
            var levelType = db.pr_getQuestionnaireLevelTypeyEnterpriseAndDefaultTouchpoint(Generic.Helpers.CurrentInstance.EnterpriseID, SessionSingleton.Touchpoint).FirstOrDefault().Value;
            if (levelType == 1)
            {
                var forLevelOne = db.pr_getAgingReportByEnterpriseAndDefaultTouchpointLevelTypeOne(Generic.Helpers.CurrentInstance.EnterpriseID, SessionSingleton.Touchpoint).ToList();
                serializer = new XmlSerializer(typeof(List<pr_getAgingReportByEnterpriseAndDefaultTouchpointLevelTypeOne_Result>));
                //We turn it into an XML and save it in the memory
                serializer.Serialize(stream, forLevelOne);
            }
            else
            {
                var forLevelTwo = db.pr_getAgingReportByEnterpriseAndDefaultTouchpointLevelTypeTwo(Generic.Helpers.CurrentInstance.EnterpriseID, SessionSingleton.Touchpoint).ToList();
                serializer = new XmlSerializer(typeof(List<pr_getAgingReportByEnterpriseAndDefaultTouchpointLevelTypeTwo_Result>));
                //We turn it into an XML and save it in the memory
                serializer.Serialize(stream, forLevelTwo);
            }
            //set stream position to begining
            stream.Position = 0;
            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "AgingReport.xls");
        }

        public ActionResult PartnumberSpreadsheetDataLoadReportDownloadForMailDeliveredReport()
        {
            var values = db.pr_getEventNotificationDelivered(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            XmlSerializer serializer = new XmlSerializer(typeof(List<pr_getEventNotificationDelivered_Result>));
            var stream = new MemoryStream();
            serializer.Serialize(stream, values);
            //set stream position to begining
            stream.Position = 0;
            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "MailDelivered.xls");
        }


        public ActionResult ArchivePartner()
        {
            string arguments = Session["partnersearch"].ToString() + "active=1";
            List<view_PartnerData> objPartnerDateList = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
            List<PartnerViewModel> objPartnerViewModelList = ConvertToPartnerViewModel(objPartnerDateList);
            ViewBag.searchType = "Archive";
            return View("RemovePartner", objPartnerViewModelList);
        }

        public ActionResult InvitePartner()
        {
            string arguments = Session["partnersearch"].ToString() + "active=1;statusID=5";

            List<view_PartnerData> objPartnerDateList = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
            List<PartnerViewModel> objPartnerViewModelList = ConvertToPartnerViewModel(objPartnerDateList);
            ViewBag.searchType = "Invite";
            return View("RemovePartner", objPartnerViewModelList);

        }


        public ActionResult RemovePartner()
        {
            string arguments = Session["partnersearch"].ToString() + "active=1;";

            List<view_PartnerData> objPartnerDateList = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
            List<PartnerViewModel> objPartnerViewModelList = ConvertToPartnerViewModel(objPartnerDateList);
            ViewBag.searchType = "Remove";
            return View("RemovePartner", objPartnerViewModelList);

        }
        [HttpPost]
        public ActionResult RemovePartner(string searchType, List<int> chkSelect, List<int> partnertypeID, List<int> touchpoint, List<string> AccessCode, List<string> ContactEmail, DateTime? DueDate)
        {
            if (searchType == "Remove")
            {
                foreach (int partnerID in chkSelect)
                {
                    db.pr_removePartner(partnerID);
                }

                ViewBag.searchType = "Remove";
                return RedirectToAction("RemovePartner");
            }
            else if (searchType == "Archive")
            {
                foreach (int partnerID in chkSelect)
                {
                    db.pr_archivePartner(partnerID);
                }
                ViewBag.searchType = "Archive";
                return RedirectToAction("ArchivePartner");
            }
            else if (searchType == "Restore")
            {
                foreach (int partnerID in chkSelect)
                {
                    db.pr_unArchivePartner(partnerID);
                }
                ViewBag.searchType = "Restore";
                return RedirectToAction("RestorePartner");
            }
            else if (searchType == "Invite")
            {
                int index = 0;
                foreach (int partnerID in chkSelect)
                {
                    // Code to Invite selected partners

                    int partnertypeId = partnertypeID[index];
                    int touchpointId = touchpoint[index];
                    string accesscode = AccessCode[index];
                    string emailID = ContactEmail[index];

                    // var objPartners = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).ToList();

                    int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertypeId, touchpointId).LastOrDefault().id;

                    var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accesscode).FirstOrDefault();

                    //var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerID, ptq).FirstOrDefault();
                    pptq.invitedDate = DateTime.Now;
                    var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
                    pptq.invitedBy = person.id;
                    pptq.status = (int)PartnerStatus.Invited_NoResponse;
                    db.Entry(pptq).State = EntityState.Modified;
                    db.SaveChanges();

                    var objpartner = db.pr_getPartner(partnerID).FirstOrDefault();
                    objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                    db.Entry(objpartner).State = EntityState.Modified;
                    db.SaveChanges();

                    var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();
                    amm.text.Replace("[partner Access Code]", accesscode);

                    //  var objpartnerByAccessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireDueDateByAccessCode(accesscode, loadGroup).FirstOrDefault();

                    // if (objpartnerByAccessCode != null)
                    // {

                    // amm.text = amm.text.Replace("[Due Date]", DateTime.Parse(DueDate.ToString()).ToString("MMM, dd, yyyy"));
                    // }

                    var objtouchpoint = db.pr_getTouchpoint(touchpointId).FirstOrDefault();
                    Email email = new Email(amm);

                    if (Session["loadgroup"] != null)
                    {
                        email.loadgroup = "";
                    }
                    email.accesscode = accesscode;
                    email.protocolTouchpoint = objtouchpoint.description;

                    EmailFormat emailFormat = new EmailFormat();
                    email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
                    email.emailTo = objpartner.email;
                    SendEmail objSendEmail = new SendEmail();

                    int checkEventNotification = 0;

                    try
                    {
                        checkEventNotification = db.pr_eventNotificationCheck(accesscode).FirstOrDefault().Value;
                    }
                    catch { }

                    if (checkEventNotification == 0)
                    {
                        objSendEmail.sendEmail(email);
                    }


                    index++;
                }
                ViewBag.searchType = "Invite";
                return RedirectToAction("InvitePartner");
            }
            else
            {
                return RedirectToAction("FindPartner");
            }
        }
        public ActionResult RestorePartner()
        {
            string arguments = Session["partnersearch"].ToString() + "active=0;";

            List<view_PartnerData> objPartnerDateList = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
            List<PartnerViewModel> objPartnerViewModelList = ConvertToPartnerViewModel(objPartnerDateList);
            ViewBag.searchType = "Restore";
            return View("RemovePartner", objPartnerViewModelList);

        }

        public ActionResult UnArchievePartners(int[] items)
        {
            foreach (var item in items)
            {
                db.pr_unArchivePartner(item);
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindPartner(string searchType)
        {
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");

            ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId), "id", "name");

            ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnerStatus = new SelectList(db.pr_getPartnerStatusAll(), "id", "description");

            ViewBag.searchType = searchType;

            return View();
        }

        [HttpPost]
        public ActionResult FindPartner(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode, string searchType)
        {
            //dbo.pr_dynamicFilters 'partner', ' Campaign=1009; Group=20;Country=2; Type=4'
            //var objPartners = db.pr_dynamicFiltersPartner("view_PartnerData", "name=well;enterprise=3");

            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            if (touchpoint != null)
                arguments += "touchpointID=" + touchpoint + ";";
            if (group != null)
                arguments += "groupID=" + group + ";";
            if (country != null)
                arguments += "countryID=" + country + ";";
            if (partnertype != null)
                arguments += "partnertypeID=" + partnertype + ";";

            if (partnerStatus != null)
                arguments += "StatusID=" + partnerStatus + ";";


            if (txtInternalIdFind != "")
                arguments += "InternalId=" + txtInternalIdFind + ";";


            //string , string , string , string , string , string )
            if (txtDunsNumberFind != "")
                arguments += "DunsNumber=" + txtDunsNumberFind + ";";
            if (txtNameFind != "")
                arguments += "PartnerName=" + txtNameFind + ";";
            if (txtFederalIdFind != "")
                arguments += "FederalId=" + txtFederalIdFind + ";";

            if (accesscode != "")
                arguments += "accesscode=" + accesscode + ";";

            if (txtContactEmailFind != "")
                arguments += "ContactEmail=" + txtContactEmailFind + ";";
            if (txtHROEmailFind != "")
                arguments += "HROEmail=" + txtHROEmailFind + ";";
            if (txtScoreFromFind != "")
                arguments += "ScoreFrom=" + txtScoreFromFind + ";";
            if (txtScoreToFind != "")
                arguments += "ScoreTo=" + txtScoreToFind + ";";
            if (txtAddedFromFind != "")
                arguments += "AddedFrom=" + txtAddedFromFind + ";";
            if (txtAddedToFind != "")
                arguments += "AddedTo=" + txtAddedToFind + ";";
            if (txtFullTextSearch != "")
                arguments += "FullTextSearch=" + txtFullTextSearch + ";";


            Session["partnersearch"] = arguments;
            if (searchType == "Remove")
            {
                return RedirectToAction("RemovePartner");
            }
            else if (searchType == "Archive")
            {
                return RedirectToAction("ArchivePartner");
            }
            else if (searchType == "Restore")
            {
                return RedirectToAction("RestorePartner");
            }
            else if (searchType == "Invite")
            {
                return RedirectToAction("InvitePartner");
            }
            else
            {
                return RedirectToAction("FindPartnerResult");
            }
        }

        public ActionResult FindPartnerResult()
        {
            try
            {
                string arguments = Session["partnersearch"].ToString() + "active=1;";
                Session["partner"] = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
                List<view_PartnerData> abc = (List<view_PartnerData>)Session["partner"];
                // List<PartnerViewModel> objPartnerViewModelList = ConvertToPartnerViewModel(abc);

                return View(abc);
            }
            catch
            {
                return RedirectToAction("FindPartner");

            }

            //List<view_PartnerData> abc = (List<view_PartnerData>)TempData["partner"];
            //Session["partner"] 


        }
        public ActionResult PrintPDF(string accesscode)
        {
            if (!string.IsNullOrEmpty(accesscode))
            {
                Session["accessCode"] = accesscode;
                var _pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
                if (_pptq != null)
                {
                    var _partnerId = _pptq.partner;
                    var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
                    var pptq = _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault();

                    var pdf = db.pr_getPPTQpdf(pptq.id).FirstOrDefault();
                    var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(pptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                    var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                    var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                    if (pdf == null || pptq.progress == null)
                    {
                        //if pdf was deleted from db but questinnarie was completed then we created customized pdf again
                        if (pptq.status == 8 && cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().link != null)
                        {
                            Response.Redirect("~/Registration/Home/CustomizedPDFConfirmation");
                        }
                        else
                            //otherwise redirect to standart pdf
                            Response.Redirect("~/Registration/Home/PDFConfirmation");
                    }
                    else
                        Response.Redirect("~/Registration/Home/PDFCustomizedConfirmation");
                }
                else
                    Response.Redirect("~/Registration/Home/PDFConfirmation");

                ///Registration/Home/PDFConfirmation
                // return RedirectToAction("PDFConfirmation","Home",new  {area="Registration"});
                //Response.Redirect("~/Registration/Home/PDFConfirmation");
            }
            return RedirectToAction("FindPartnerResult");

        }
        public ActionResult PrintHTML(string accesscode)
        {
            if (!string.IsNullOrEmpty(accesscode))
            {
                Session["accessCode"] = accesscode;
                ///Registration/Home/PDFConfirmation
                // return RedirectToAction("PDFConfirmation","Home",new  {area="Registration"});
                Response.Redirect("~/Registration/Home/OrdersInHTML");
            }
            return RedirectToAction("FindPartnerResult");

        }




        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private List<PartnerViewModel> ConvertToPartnerViewModel(List<view_PartnerData> iview_PartnerDataList)
        {
            List<PartnerViewModel> objPartnerViewModelList = new List<PartnerViewModel>();

            foreach (var iview_PartnerData in iview_PartnerDataList)
            {
                PartnerViewModel objPartnerViewModel = new PartnerViewModel();
                objPartnerViewModel.id = iview_PartnerData.id;
                objPartnerViewModel.internalID = iview_PartnerData.internalID;
                objPartnerViewModel.enterprise = iview_PartnerData.enterprise;
                objPartnerViewModel.PartnerName = iview_PartnerData.PartnerName;
                objPartnerViewModel.Contact = iview_PartnerData.Contact;
                objPartnerViewModel.phone = iview_PartnerData.phone;
                objPartnerViewModel.ContactEmail = iview_PartnerData.ContactEmail;
                objPartnerViewModel.ContactTitle = iview_PartnerData.ContactTitle;
                objPartnerViewModel.owner = iview_PartnerData.owner;
                objPartnerViewModel.Country = iview_PartnerData.Country;
                objPartnerViewModel.city = iview_PartnerData.city;
                objPartnerViewModel.state = iview_PartnerData.state;
                objPartnerViewModel.address = iview_PartnerData.address;
                objPartnerViewModel.zipcode = iview_PartnerData.zipcode;
                objPartnerViewModel.Touchpoint = iview_PartnerData.Touchpoint;

                objPartnerViewModel.Partnertype = iview_PartnerData.Partnertype;
                objPartnerViewModel.Group = iview_PartnerData.Group;
                objPartnerViewModel.AccessCode = iview_PartnerData.AccessCode;
                objPartnerViewModel.status = iview_PartnerData.status;
                objPartnerViewModel.Expr1 = iview_PartnerData.Expr1;
                objPartnerViewModel.countryID = iview_PartnerData.countryID;
                objPartnerViewModel.touchpointID = iview_PartnerData.touchpointID;

                objPartnerViewModel.partnertypeID = iview_PartnerData.partnertypeID;
                objPartnerViewModel.groupID = iview_PartnerData.groupID;
                objPartnerViewModel.statusID = iview_PartnerData.statusID;
                objPartnerViewModel.campaign = iview_PartnerData.campaign;
                objPartnerViewModel.IsSelected = false;

                objPartnerViewModelList.Add(objPartnerViewModel);
            }
            return objPartnerViewModelList;

        }

        // To Confirm Partner
        public ActionResult ConfirmPartner()
        {

            //string arguments = "enterprise=" + (int)Generic.Helpers.CurrentInstance.EnterpriseID + ";touchpoint=" + Session["touchpoint"] + ";";
            int touchpointID = Convert.ToInt32(Session["touchpoint"].ToString().Trim());
            int enterpriseid = Generic.Helpers.CurrentInstance.EnterpriseID;
            //int enterpriseid = 1067;
            //int touchpointID = 1016;  
            List<ConfirmPartnerViewModel> objConfirmPartnerList = new List<ConfirmPartnerViewModel>();
            //   List<ConfirmPartnerViewModel> objConfirmPartnerList = db.Database.SqlQuery<ConfirmPartnerViewModel>("pr_getPartnerConfirmationData @enterprise,@touchpoint", new SqlParameter("enterprise", enterpriseid), new SqlParameter("touchpoint", touchpointID)).ToList();
            //   List<pr_getPartnerConfirmationData_Result> objConfirmPartnerList = db.pr_getPartnerConfirmationData((int)Generic.Helpers.CurrentInstance.EnterpriseID, (int)Session["touchpoint"]).ToList();
            //  List<ConfirmPartnerViewModel> objConfirmPartnerViewModelList = ConvertToConfirmPartnerViewModel(objConfirmPartnerList);
            return View("ConfirmPartner", objConfirmPartnerList);

        }
        public DataTable getConfirmPartnerSpreadsheet(int enterprise, int touchpoint)
        {
            DataTable dataTable = new DataTable();

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
            SqlCommand command = new SqlCommand("pr_getPartnerConfirmationData_Spreadsheet", conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 120;
            command.Parameters.Add("@enterprise", SqlDbType.VarChar).Value = enterprise;
            command.Parameters.Add("@touchpoint", SqlDbType.VarChar).Value = touchpoint;

            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);
            sqlDataAdapter.Fill(dataTable);
            conn.Close();
            return dataTable;
        }
        public void ExportExcelConfirmPartners()
        {

            int touchpointID = Convert.ToInt32(Session["touchpoint"].ToString().Trim());
            int enterpriseid = Generic.Helpers.CurrentInstance.EnterpriseID;
            ExportToExcel(getConfirmPartnerSpreadsheet(enterpriseid, touchpointID));
            //((IObjectContextAdapter)db).ObjectContext.CommandTimeout = 180;
            //List<ConfirmPartnerSpreadsheetViewModel> objConfirmPartnerList = db.Database.SqlQuery<ConfirmPartnerSpreadsheetViewModel>("pr_getPartnerConfirmationData_Spreadsheet @enterprise,@touchpoint", new SqlParameter("enterprise", enterpriseid), new SqlParameter("touchpoint", touchpointID)).ToList();
            //GridView gv = new GridView();
            //gv.DataSource = objConfirmPartnerList;
            //gv.DataBind();
            //Response.ClearContent();
            //Response.Buffer = true;
            //Response.AddHeader("content-disposition", "attachment; filename=ConfirmPartner.xls");
            //Response.ContentType = "application/ms-excel";
            //Response.Charset = "";
            //StringWriter sw = new StringWriter();
            //HtmlTextWriter htw = new HtmlTextWriter(sw);
            //gv.RenderControl(htw);
            //Response.Output.Write(sw.ToString());
            //Response.Flush();
            //Response.End();

            //var stream = new MemoryStream();
            //var serializer = new XmlSerializer(typeof(List<ConfirmPartnerSpreadsheetViewModel>));           

            ////We turn it into an XML and save it in the memory
            //serializer.Serialize(stream, objConfirmPartnerList);
            //stream.Position = 0;

            ////We return the XML from the memory as a .xls file
            //return File(stream, "application/vnd.ms-excel", "ConfirmPartner.xls");


        }

        private List<ConfirmPartnerViewModel> ConvertToConfirmPartnerViewModel(List<pr_getPartnerConfirmationData2_Result> iview_ConfirmPartnerDataList)
        {
            List<ConfirmPartnerViewModel> objConfirmPartnerViewModelList = new List<ConfirmPartnerViewModel>();

            foreach (var iview_ConfirmPartnerData in iview_ConfirmPartnerDataList)
            {
                ConfirmPartnerViewModel objConfirmPartnerViewModel = new ConfirmPartnerViewModel();
                // objConfirmPartnerViewModel.id = iview_ConfirmPartnerData.id;
                //   objConfirmPartnerViewModel.enterprise = iview_ConfirmPartnerData.enterprise;
                //    objConfirmPartnerViewModel.touchpoint = iview_ConfirmPartnerData.touchpoint;
                //    objConfirmPartnerViewModel.Partner_A = iview_ConfirmPartnerData.Partner_A;
                //     objConfirmPartnerViewModel.Partner_A_Name = iview_ConfirmPartnerData.Partner_A;

                objConfirmPartnerViewModel.Group1ID = iview_ConfirmPartnerData.group1;
                objConfirmPartnerViewModel.Group1 = iview_ConfirmPartnerData.group1_Name;
                objConfirmPartnerViewModel.StatusID_1 = iview_ConfirmPartnerData.status1;

                objConfirmPartnerViewModel.Status1 = iview_ConfirmPartnerData.status1_Name;
                // string strref1 = iview_ConfirmPartnerData.isReference1.ToString()=="True" ? "Yes" : "No";
                objConfirmPartnerViewModel.IsReference1 = iview_ConfirmPartnerData.isReference1.ToString() == "True" ? "Yes" : "No";

                objConfirmPartnerViewModel.Partner_B = iview_ConfirmPartnerData.partnerB;
                objConfirmPartnerViewModel.Partner_B_Name = iview_ConfirmPartnerData.partnerB_Name;
                objConfirmPartnerViewModel.Group2ID = iview_ConfirmPartnerData.group2;
                objConfirmPartnerViewModel.Group2 = iview_ConfirmPartnerData.group2_Name;
                objConfirmPartnerViewModel.StatusID_2 = iview_ConfirmPartnerData.status2;
                objConfirmPartnerViewModel.Status2 = iview_ConfirmPartnerData.status2_Name;

                //string strref2 = iview_ConfirmPartnerData.isReference2.ToString() == "True" ? "Yes" : "No";
                objConfirmPartnerViewModel.IsReference2 = iview_ConfirmPartnerData.isReference2.ToString() == "True" ? "Yes" : "No";
                //string emailmatch = iview_ConfirmPartnerData.emailMatch.ToString() == "True" ? "Yes" : "No";
                objConfirmPartnerViewModel.EmailMatch = iview_ConfirmPartnerData.emailMatch.ToString() == "True" ? "Yes" : "No";
                //string nameMatch = iview_ConfirmPartnerData.nameMatch.ToString() == "True" ? "Yes" : "No";
                objConfirmPartnerViewModel.NameMatch = iview_ConfirmPartnerData.nameMatch.ToString() == "True" ? "Yes" : "No";
                //string internalIDMatch = iview_ConfirmPartnerData.internalIDMatch.ToString() == "True" ? "Yes" : "No";
                objConfirmPartnerViewModel.InternalIDMatch = iview_ConfirmPartnerData.internalIDMatch.ToString() == "True" ? "Yes" : "No";
                //string federalIDMatch = iview_ConfirmPartnerData.federalIDMatch.ToString() == "True" ? "Yes" : "No";
                objConfirmPartnerViewModel.FederalIDMatch = iview_ConfirmPartnerData.federalIDMatch.ToString() == "True" ? "Yes" : "No";
                //string dunsMatch = iview_ConfirmPartnerData.dunsMatch.ToString() == "True" ? "Yes" : "No";
                objConfirmPartnerViewModel.DUNSMatch = iview_ConfirmPartnerData.dunsMatch.ToString() == "True" ? "Yes" : "No";

                objConfirmPartnerViewModel.IsSelected1 = false;
                objConfirmPartnerViewModel.IsSelected2 = false;
                objConfirmPartnerViewModel.IsCheckboxSelected = false;
                objConfirmPartnerViewModelList.Add(objConfirmPartnerViewModel);
            }
            return objConfirmPartnerViewModelList;
        }
        [HttpPost]
        public ActionResult UploadExcelData(string id)
        {
            string questionnaireid = id;

            HttpPostedFileBase excelFile = Request.Files["uploadfile"];
            int confirmPartnerCount = 0;
            if (excelFile != null)
            {
                if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
                }

                if (!Directory.Exists((Server.MapPath("~/uploadedFiles/Questionnaire"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/Questionnaire"));
                }

                // The Name of the Upload component is "attachments" 
                var file = excelFile;
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/Questionnaire"), fileName);

                // The files are not actually saved in this demo
                file.SaveAs(physicalPath);
                string sheetname = "Sheet1";
                var excelRead = new ExcelQueryFactory(physicalPath.ToString());

                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.questionnaire, questionnaireid);
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.questionnaireCMS, "questionnaireCMS");
                ////Need to ignore now
                ////excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.section, "SECTION");
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.text, "text");
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.link, "link");
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.doc, "doc");
                var confirmPartnerExcel = from a in excelRead.Worksheet<ConfirmPartnerSpreadsheetViewModelupload>(sheetname) select a;

                //  var confirmPartnerExcel = from a in excelRead.Worksheet(sheetname) select a;

                List<Tuple<int, string>> uploadConfirmPartner = new List<Tuple<int, string>>();

                foreach (var confirmPartneritem in confirmPartnerExcel.ToList())
                {
                    using (var context = new EntitiesDBContext())
                    {
                        if (!string.IsNullOrEmpty(confirmPartneritem.AccessCode_A))
                        {


                            int? modifiedQuestionnaire = context.pr_addPartnerConfirmationData_Spreadsheet(confirmPartneritem.pptqA_id,
                                confirmPartneritem.Partner_A, confirmPartneritem.AccessCode_A,
                                confirmPartneritem.PartnerA_Name, confirmPartneritem.PartnerA_Address,
                                confirmPartneritem.InternalID_A, confirmPartneritem.email_A, confirmPartneritem.GroupID_A,
                                confirmPartneritem.Group_A, confirmPartneritem.StatusID_A, confirmPartneritem.Status_A,
                                confirmPartneritem.pptqB_id, confirmPartneritem.Partner_B, confirmPartneritem.AccessCode_B,
                                confirmPartneritem.PartnerB_Name, confirmPartneritem.PartnerB_Address, confirmPartneritem.InternalID_B,
                                confirmPartneritem.email_B, confirmPartneritem.GroupID_B, confirmPartneritem.Group_B, confirmPartneritem.StatusID_B, confirmPartneritem.Status_B,
                                confirmPartneritem.IsReference2, confirmPartneritem.EM, confirmPartneritem.IM, confirmPartneritem.FM, confirmPartneritem.DM,
                                confirmPartneritem.NM, confirmPartneritem.action
                               ).FirstOrDefault();
                            confirmPartnerCount++;
                        }
                    }
                }
            }

            ViewBag.isMessage = 1;
            ViewBag.message = "Congratulations, you have uploaded " + confirmPartnerCount + " partner confirmation actions.";

            return View("ConfirmPartner");
            // return RedirectToAction("QuestionnaireQuestionnaireCMS", new { id = int.Parse(id) });
            // return QuestionnaireQuestionnaireCMS(int.Parse(id));
            //  return RedirectToAction("QuestionnaireQuestionnaireCMS");
            //        return RedirectToAction("QuestionnaireQuestionnaireCMS", new RouteValueDictionary(
            //new { controller = "Questionnaire", action = "QuestionnaireQuestionnaireCMS", id = int.Parse(id) }));

        }
        public ActionResult getActionTypes()
        {
            var data = db.pr_getConfirmPartnerActionTypeAll().FirstOrDefault();
            return Json(data.ToString(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult IgnoreDetails()
        {
            return View("ConfirmPartner");
        }
        public ActionResult RemoveDetails()
        {
            return View("ConfirmPartner");
        }
        public ActionResult ShadowDetails()
        {
            return View("ConfirmPartner");
        }

        public JsonResult ConfirmPartnerRecords(string inputList)
        {
            var inputParam = GetInputListParameterForPerformSelectedActions(inputList);

            try
            {
                db.pr_PerformSelectedActions(inputParam);
                return Json(inputParam, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(-1, JsonRequestBehavior.AllowGet);
            }
        }

        private string GetInputListParameterForPerformSelectedActions(string inputList)
        {
            var currentPersonId = (int)Session["LoggedInUserId"];

            var currentPersonIdStringAddition = string.Format(",{0};", currentPersonId);

            return inputList.Replace(";", currentPersonIdStringAddition);
        }

        [HttpPost]
        public string Remind(string accessCode)
        {

            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
            var result = "Congratulations, you have just sent a reminder to " + pptq.partner1.name + " with access code " + accessCode;
            if (pptq != null)
            {
                if (new int[] { 6, 7 }.Contains(pptq.status))
                    SchedulerServiceHelper.SendFirstReminderByPptq(pptq.id);
                else
                {
                    result = "The status for " + pptq.partner1.name + " with " + accessCode + " access code does not permit reminders at this time. Please contact your system adminitrator.";
                }

            }
            return result;
        }

        public ActionResult RailReport()
        {
            try
            {
                var currentPersonId = (int)Session["LoggedInUserId"];
                var person = db.pr_getPerson(currentPersonId).FirstOrDefault();
                if (person != null)
                {

                    var result = new RailModelView()
                    {
                        Rails = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseFromCustomer(person.email).ToList(),
                        Statuses = db.pr_getRailStatusAll().ToList()
                    };
                    return View(result);
                }
            }
            catch { }
            return RedirectToAction("Home", "Admin");
        }
        public ActionResult RailReportExel()
        {
            var currentPersonId = (int)Session["LoggedInUserId"];
            var person = db.pr_getPerson(currentPersonId).FirstOrDefault();
            var values = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseFromCustomer(person.email).ToList();
            XmlSerializer serializer = new XmlSerializer(typeof(List<pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseFromCustomer_Result>));
            var stream = new MemoryStream();
            serializer.Serialize(stream, values);
            //set stream position to begining
            stream.Position = 0;
            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "RailReport.xls");
        }
        [HttpPost]
        public ActionResult ChangeRailsStatus(int statusId, int railId)
        {
            var currentPersonId = (int)Session["LoggedInUserId"];
            var person = db.pr_getPerson(currentPersonId).FirstOrDefault();
            var reponse = db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseFromCustomer(person.email).FirstOrDefault(o => o.id == railId);

            var newStatus = db.pr_getRailStatus(statusId).FirstOrDefault();
            if (newStatus != null && reponse != null)
            {
                db.pr_modifypartnerPartnertypeTouchpointQuestionnaireQuestionResponseValue(railId, statusId);
                if (reponse.Sender.ToLower() != person.email.ToLower())
                {
                    SchedulerServiceHelper.sendEmail("Intelleges RAIL Update Notice", "Please be advised that the status for '" + reponse.Comment + "' has been updated to '" + newStatus.description + "'. If you have any additional questions about this please contact your System Admin.<br> Thank you. <br> Intelleges Rapid Response Team", null, reponse.Sender);
                    return Json(reponse);
                }
                return Json(newStatus);
            }
            return Json(null);
        }

        public virtual async Task<ActionResult> Iterate(bool? showNotes)
        {
            ViewBag.state = new SelectList(db.state.ToList(), "stateCode", "name");
            ViewBag.country = new SelectList(db.country.ToList(), "id", "name");
            // db.pr_getTouchpointAllByEnterprise(SessionSingleton.EnterPriseId).FirstOrDefault().
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "title");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");
            ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).ToList(), "id", "name");
            ViewBag.nextaction = new SelectList(db.pr_getIterateNextAction().ToList(), "id", "nextAction");
            ViewBag.partnerstatus = new SelectList(db.pr_getIteratePartnerStatusAll().ToList(), "id", "description");
            ViewBag.currentUserPartnerType = new SelectList(db.pr_getPartnertypeByTouchpoint(db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault().campaign).ToList(), "id", "name");

            //Scheduler Initializeer
            //var scheduler = new DHXScheduler(this) { LoadData = true, EnableDataprocessor = true };
            // ViewBag.Scheduler = scheduler.Render();
            ViewBag.showNotes = showNotes;
            return View();

        }

        /// <summary>
        /// Method import events from google calendar
        /// </summary>
        /// <returns></returns>
        //public ContentResult Data()
        //{
        //    var data = new SchedulerAjaxData();

        //    var iCal = db.pr_getPersonIcal(SessionSingleton.LoggedInUserId).FirstOrDefault();
        //    data.FromICal(iCal);
        //    return data;
        //}

        /// <summary>
        /// Get Person Ical url
        /// </summary>
        /// <returns></returns>
        //public ActionResult GetPersonIcal()
        //{
        //    var iCal = db.pr_getPersonIcal(SessionSingleton.LoggedInUserId).FirstOrDefault();
        //    return Json(new { Data = iCal }, JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// Method to update the iCal of perosn logged in
        /// </summary>
        /// <param name="iCal"></param>
        /// <returns></returns>
        public ActionResult ModifyPersonIcal(string iCal)
        {
            var moidfyIcal = db.pr_modifyPersonIcal(SessionSingleton.LoggedInUserId, iCal);
            return Json(new { Data = "success" }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// Method to insert/update/delete the events from the event table
        /// </summary>
        /// <param name="modelEvents"></param>
        /// <param name="formData"></param>
        /// <returns></returns>
        public ActionResult Save(FormCollection formData)
        {
            var credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                            new ClientSecrets
                            {
                                ClientId = "650556964933-70jaej6pdd2orhrs5u1fl1msvhg4eise.apps.googleusercontent.com",
                                ClientSecret = "Gxjf1qZ12YNaeiZui3p2hNZr",
                            },
                            new[] { CalendarService.Scope.Calendar },
                            "user",
                            CancellationToken.None).Result;

            if (credential != null)
            {

                // Create the service.
                var service = new CalendarService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Calendar API Sample",
                });


                //get start date from the form collection
                var startDate = formData["start_date"];
                //split string date by separator, here I'm using '/'
                var arrDate = startDate.Split('/');
                //now use array to get specific date object
                var month = Convert.ToInt32(arrDate[0]);
                var day = Convert.ToInt32(arrDate[1]);
                //split year and tiem by space
                var arrYearTime = arrDate[2].Split(' ');
                var year = Convert.ToInt32(arrYearTime[0]);
                //split hours and minutes by colon
                var arrHourMin = arrYearTime[1].Split(':');
                var hour = Convert.ToInt32(arrHourMin[0]);
                var min = Convert.ToInt32(arrHourMin[1]);

                //get end date from form collection and split
                var endDate = formData["end_date"];
                var arrEndDate = endDate.Split('/');
                var endMonth = Convert.ToInt32(arrEndDate[0]);
                var endDay = Convert.ToInt32(arrEndDate[1]);
                var arrEndYearTime = arrEndDate[2].Split(' ');
                var endYear = Convert.ToInt32(arrEndYearTime[0]);
                var arrEndHourMin = arrEndYearTime[1].Split(':');
                var endHour = Convert.ToInt32(arrEndHourMin[0]);
                var endMin = Convert.ToInt32(arrEndHourMin[1]);

                //get assignees comma seperated
                var attendees = formData["attendees"];
                var arrAttendees = attendees.Split(',');
                var listEventAttendees = arrAttendees.Select(arrAttendee => new EventAttendee { Email = arrAttendee.Trim() }).ToList();

                var myEvent = new Event
                {
                    Summary = formData["text"],
                    Location = formData["location"],
                    Start = new EventDateTime
                    {
                        DateTime = new DateTime(year, month, day, hour, min, 0),
                    },
                    End = new EventDateTime
                    {
                        DateTime = new DateTime(endYear, endMonth, endDay, endHour, endMin, 0),
                    },
                    Attendees = listEventAttendees,
                };

                var insertEvent = service.Events.Insert(myEvent, "primary");
                insertEvent.SendNotifications = true;
                insertEvent.Execute();
            }
            var action = new DataAction(formData);

            return (new AjaxSaveResponse(action));
        }


        [HttpPost]
        public ActionResult Notes(string partnerId, string partnerName)
        {
           
            return RedirectToAction("AuthorizeEverNote");
        }
        /// <summary>
        /// Authorize the user with evernote login
        /// </summary>
        /// <param name="reauth"></param>
        /// <returns></returns>
        //[HttpPost]
        public JsonResult AuthorizeEverNote(bool reauth = false)
        {
            // Allow for reauth
            if (reauth)
                SessionHelper.Clear();

            // First of all, check to see if the user is already registered, in which case tell them that
            if (SessionHelper.EvernoteCredentials != null)
                return Json(Url.Action("AlreadyAuthorized"));

            // Evernote will redirect the user to this URL once they have authorized your application
            var callBackUrl = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ObtainTokenCredentials");

            // Generate a request token - this needs to be persisted till the callback
            var requestToken = Generic.Helpers.EvernoteAuthorizer.GetRequestToken(callBackUrl);

            // Persist the token
            SessionHelper.RequestToken = requestToken;

            // Redirect the user to Evernote so they can authorize the app
            var callForwardUrl = Generic.Helpers.EvernoteAuthorizer.BuildAuthorizeUrl(requestToken);
            // return Content("testing");
            return Json(callForwardUrl, JsonRequestBehavior.AllowGet);
            //return Redirect(callForwardUrl);
        }

        /// <summary>
        /// This action is the callback that Evernote will redirect to after 
        /// the call to Authorize above
        /// </summary>
        /// <param name="oauth_verifier"></param>
        /// <returns></returns>
        public async Task<ActionResult> ObtainTokenCredentials(string oauth_verifier)
        {
            // Use the verifier to get all the user details we need and
            // store them in EvernoteCredentials
            var credentials = Generic.Helpers.EvernoteAuthorizer.ParseAccessToken(oauth_verifier, SessionHelper.RequestToken);
            if (credentials != null)
            {
                SessionHelper.EvernoteCredentials = credentials;
                if (SessionSingleton.NeedAddEverNote)
                    await AddNote(TempData["PartnerName"].ToString(), TempData["noteTitle"].ToString(), TempData["noteText"].ToString(), TempData["partnerId"].ToString());
                else if (SessionSingleton.NeedGetEvernoteText)
                    GetEvernoteText(int.Parse(TempData["partnerId"].ToString()));
                return Redirect(Url.Action("Iterate", new { showNotes = "True" }));
            }
            else
            {
                return Redirect(Url.Action("Unauthorized"));
            }
        }

        [HttpPost]
        public ActionResult UpdateNote(string title, string id, string newNoteBookId)
        {
            int iterateStatus = 0, partnerId = 0;
            string iterateStatusDesc = "";
            bool statusUpdated = false;
            var gId = Guid.Parse(id);
            if (SessionHelper.EvernoteCredentials != null)
            {
                var authToken = SessionHelper.EvernoteCredentials.AuthToken;
                var noteStore = GetNoteStore();
                var noteBook = noteStore.getNotebook(authToken, newNoteBookId);
                if (noteBook != null)
                {
                    var noteBookGuid = Guid.Parse(noteBook.Guid);
                    var statuses = db.pr_getIteratePartnerStatusAll().ToList();
                    var newStatus = statuses.FirstOrDefault(o => o.notebook == noteBookGuid);
                    if (newStatus != null)
                    {
                        db.pr_modifyIteratePartnerStatusByNote(gId, newStatus.id);
                        iterateStatus = newStatus.id;
                        iterateStatusDesc = newStatus.description;
                        var partner = db.iteratePartner.FirstOrDefault(o => o.note == gId);
                        if (partner != null)
                        {
                            partnerId = partner.id;
                            statusUpdated = true;
                        }
                    }
                }
                if(string.IsNullOrEmpty(title))
                    noteStore.updateNote(authToken, new Note() { Guid = id, NotebookGuid = newNoteBookId });
                else
                noteStore.updateNote(authToken, new Note() { Guid = id, Title = title, NotebookGuid = newNoteBookId });
                if (statusUpdated)
                    return Json(new { iteratestatusid = iterateStatus, partnerid = partnerId, iteratestatusdescription = iterateStatusDesc });
            }
            return Json(false);
        }

        /// <summary>
        /// Returns Current Evernote Notebooks Tree List
        /// </summary>
        /// <returns></returns>
        public ActionResult EvernoteNotebooksList()
        {
            
            //var result = new List<TreeViewItemModel>();
            if (SessionHelper.EvernoteCredentials != null)
            {
                var authToken = SessionHelper.EvernoteCredentials.AuthToken;
                var noteStore = GetNoteStore();

                var notebooks = noteStore.listNotebooks(authToken);
                //var notesCount = noteStore.findNoteCounts(authToken, new NoteFilter()
                //{
                //    Inactive = false
                //}, true);
               
                var allnotes = noteStore.findNotesMetadata(authToken, new NoteFilter()
                {
                    Inactive = false                    
                }, 0, 10000, new NotesMetadataResultSpec() { IncludeCreated=true,IncludeNotebookGuid=true,IncludeTitle=true,IncludeAttributes=true});
               
                
                //noteStore.
                var stacks = notebooks.GroupBy(o => o.Stack,p=>p,(key,values)=>new {Key=key,NoteBooks=values.ToList()});
                //var user = GetUserStore().getUser(authToken);

                var dictImage = new Dictionary<string, string>();
                dictImage.Add("style", "width:16px;height:16px");
                var result = stacks.Select(o => new JsTree3Node()
                {
                    text = o.Key ?? "Default",
                    type="root",
                    state=new State(true,false,false),
                     icon= "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAqklEQVR42mMoqGkLKaxtew7E/6H4W0Ft2xkQBrLfwsXr2pIZsAGg5EOQgoK6ttjQ0FXMDKQCqA0fQGyyXINsAFmuQTMAzi6ub9MCspciuQYbPoDTACB9FeiSaVn19Tx4LH8I0/QNiwEfYDZhcfF/dAP+U9OAb5Qa8H9ADHhFtgF4JMk24BW6AQV1rd7g5FzXHojXADTDPuBLhWA1NW2LcBtQ03YejwG3YeoA0MZlTRgfDacAAAAASUVORK5CYII=",
                    children = o.NoteBooks.Select(p => new JsTree3Node()
                    {
                        text = p.Name,
                        type = "notebook",
                        icon= "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAVUlEQVR42mNgGFSguL5Nq7C2bSkQ/ycCH8AwACh4tbCuPZAYy4BqH2IT/IBuE5o8XGzUgCFlQEFdq3dBbdsZUPogywBseHAYAPTWRmwGPCHCgM/IegAsqyv9CNAK4AAAAABJRU5ErkJggg==",
                        id = p.Guid,
                        state = new State(false, false, false),
                        children = allnotes.Notes.Where(c => c.NotebookGuid == p.Guid).Select(d => new JsTree3Node()
                        {
                            type="note",
                            text = d.Title,
                            icon="glyphicon glyphicon-file",
                           //"data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABoAAAAaCAYAAACpSkzOAAABaklEQVRIS+2WvUrEQBCAbxMUrNTGZzgLRUhnI9ecryL+1PZ29iL3FPbaiI1dQtBCfQYL7QSPGL+R22NddpON4bZyIUXmZ7+dyWxm1MBaO6w0Tc8Rj3iWbL35Xtf1K+9nRVFcNNmJTpkGM8g9spU2Rwt4CewYWe3z+wXKsuwawzEnvVNKHeR5/uJzxnaMTuz1mmB/6LO3QZ+zdG3i9NwUFdGvkeI3y8YLs0E/oVdVtV6W5Xtb+ojKlSonbBGgAamXb3ZkHnQhIAEAOzGrsS/ogT23XCmW0ge0oXW9QHI9KIpVE5QkyYiKvRIZBTXfvy/IFYyiSL46gXDwpsZBeCSCbZHragyOKBqo7R759J0jigYKTN382+iDdY4oGiha6qKBAlOnz/N/j8IbX58LG9zKGyBDdE88U/51y742ETycuEAUz5A+NKFN7KG/AbTvBP113HJAP5g7dpk7SidIhF0GSAdgiuwWyKkJEbtv8T5gKvBv2R8AAAAASUVORK5CYII=",
                            id = d.Guid//,                            li_attr = new { style = "width:16px;height:16px" }
                            //ImageHtmlAttributes = dictImage
                            , state = new State(false, false, false),
                        }).ToList()
                    }).ToList()
                }).ToList();
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                
               var result =new[]{new  { text = "Authorize to get notebooks",icon="glyphicon glyphicon-flash" }};
               return Json(result, JsonRequestBehavior.AllowGet);
            }
           
        }

        /// <summary>
        /// Returns current evernote notestore
        /// </summary>
        /// <returns></returns>
        private NoteStore.Client GetNoteStore()
        {
            var authToken = SessionHelper.EvernoteCredentials.AuthToken;
            var userStore = GetUserStore();
            var noteStoreUrl = userStore.getNoteStoreUrl(authToken);
            var noteStoreTransport = new THttpClient(new Uri(noteStoreUrl));
            var noteStoreProtocol = new TBinaryProtocol(noteStoreTransport);
            var noteStore = new NoteStore.Client(noteStoreProtocol);
            return noteStore;
        }
        /// <summary>
        /// Returns current evernote user store
        /// </summary>
        /// <returns></returns>
        private UserStore.Client GetUserStore()
        {
            var authToken = SessionHelper.EvernoteCredentials.AuthToken;
            var evernoteHost = "www.evernote.com";

            var userStoreUrl = new Uri("https://" + evernoteHost + "/edam/user");
            var userStoreTransport = new THttpClient(userStoreUrl);
            var userStoreProtocol = new TBinaryProtocol(userStoreTransport);
            return new UserStore.Client(userStoreProtocol);
        }

        private Notebook CreateNoteBook(string name)
        {
            var authToken = SessionHelper.EvernoteCredentials.AuthToken;
            var noteStore = GetNoteStore();
            var newNotebook = new Notebook();
            string stack = db.pr_getEvernoteStackByPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
            if (string.IsNullOrEmpty(stack))
            {
                //default stack name could be changed, but it shouldn't be current user id because stack name is displayable value
                stack = "Iterate partners";
                db.pr_addEvernoteStackByPerson(SessionSingleton.LoggedInUserId, stack);                
            }
            newNotebook.Stack = stack;
            newNotebook.Name = DefaultNoteBook;
            return noteStore.createNotebook(authToken, newNotebook);
        }

        private Note CreateNote(string title, string text, string noteBookId)
        {
            var authToken = SessionHelper.EvernoteCredentials.AuthToken;
            // To create a new note, simply create a new Note object and fill in 
            // attributes such as the note's title.
            var note = new Note();
            note.Title = title;
            note.NotebookGuid = noteBookId;
            // The content of an Evernote note is represented using Evernote Markup Language
            // (ENML). The full ENML specification can be found in the Evernote API Overview
            // at http://dev.evernote.com/documentation/cloud/chapters/ENML.php
            note.Content = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<!DOCTYPE en-note SYSTEM \"http://xml.evernote.com/pub/enml2.dtd\">" +
                "<en-note><div>"+text+"</div><br/>" +
                "</en-note>";
            // Finally, send the new note to Evernote using the createNote method
            // The new Note object that is returned will contain server-generated
            // attributes such as the new note's unique GUID.
             return GetNoteStore().createNote(authToken, note);
        }
        [HttpPost]
        public ActionResult GetEvernoteText(int partnerId)
        {
            if (SessionHelper.EvernoteCredentials != null)
            {
                var currentPerson = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
                var iPartner = db.iteratePartner.FirstOrDefault(o => o.id == partnerId);
                if (iPartner != null && iPartner.note.HasValue)
                {
                    var note = GetNoteStore().getNote(SessionHelper.EvernoteCredentials.AuthToken, iPartner.note.Value.ToString(), true, false, false, false);
                    if (note != null)
                    {
                        var doc = XDocument.Parse(note.Content.Replace("&nbsp;", "&#160;").Replace("&lrm;","&#8206;"));
                        return Json(new { text = doc.Element(XName.Get("en-note")).ToString().Replace("<en-note>", "").Replace("</en-note>", ""), email = currentPerson.email });
                    }
                    else return Json(new { text = "none", email = currentPerson.email });
                }
                else return Json(new { text = "none", email = currentPerson.email });
                SessionSingleton.NeedGetEvernoteText = false;
            }
            else
            {
                TempData["partnerId"] = partnerId;
                SessionSingleton.NeedGetEvernoteText = true;
                return AuthorizeEverNote();
            }
            return Json("");
        }

        [HttpPost]
        public ActionResult GetIteratePartnerByNote(Guid noteId)
        {
            var iParner = db.iteratePartner.FirstOrDefault(o => o.note == noteId);
            if(iParner!=null)
                return Json(new { partnerId = iParner.id, partnerName = iParner.name, finded = true});
            else
                return Json(new {  finded = false });
        }
        
        private Note AppendNote(string text, Guid noteId, bool rewrite=true)
        {
            try
            {
                var store = GetNoteStore();
                var note = store.getNote(SessionHelper.EvernoteCredentials.AuthToken, noteId.ToString(), true, false, false, false);
                //var doc = System.Xml.Linq.XDocument.Parse(note.Content);
                var indexNote = note.Content.IndexOf("<en-note>");
                // elem = doc.Element(XName.Get("en-note"));
                //elem.RemoveAll();
                //foreach (var node in XElement.Parse("<root>" + text.Replace("&nbsp;", "") + "</root>").Elements())
                //{
                //    elem.Add(node);
                //} 
                if (!rewrite)
                    note.Content = new string(note.Content.Take(indexNote + 9).ToArray()) + text + new string(note.Content.Skip(indexNote + 9).ToArray());
                else note.Content = new string(note.Content.Take(indexNote + 9).ToArray()) + text + "</en-note>";
                store.updateNote(SessionHelper.EvernoteCredentials.AuthToken, note);
                return note;
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// Email's to Evernote Notes Synchronisation
        /// </summary>
        /// <param name="gCreadentials">gmail credentials</param>
        /// <param name="iPartner">iterate partner </param>
        /// <param name="iPerson">iterate person</param>
        private void SyncEmailAndNotes(UserCredential gCreadentials, iteratePartner iPartner, iteratePerson iPerson)
        {
            List<Google.Apis.Gmail.v1.Data.Message> result = new List<Google.Apis.Gmail.v1.Data.Message>();
            var gService = new GmailService(new BaseClientService.Initializer { HttpClientInitializer = gCreadentials, ApplicationName = "Intelleges Inc." });
            
            var listRequest = gService.Users.Messages.List("me");
            if (!iPartner.emailLastUpdate.HasValue)
                listRequest.Q = string.Format("in:inbox from:{0} after:{1}", iPerson.email, DateTime.Now.AddMonths(-3).ToString("yyyy/MM/dd"));
            else
            {
                listRequest.Q = string.Format("in:inbox from:{0} after:{1}", iPerson.email, iPartner.emailLastUpdate.Value.ToString("yyyy/MM/dd"));
            }

            do
            {
                try
                {
                    ListMessagesResponse list = listRequest.Execute();
                    result.AddRange(list.Messages);
                    listRequest.PageToken = list.NextPageToken;
                }
                catch (Exception ex)
                {

                }
            } while (!String.IsNullOrEmpty(listRequest.PageToken));
            Stack<string> futureNotes = new Stack<string>();
            foreach (var message in result)
            {
                try
                {
                    var messageRequiest = gService.Users.Messages.Get("me", message.Id);
                    messageRequiest.Format = UsersResource.MessagesResource.GetRequest.FormatEnum.Raw;
                    var loadMessage = messageRequiest.Execute();
                    var bytes = Base64UrlDecode(loadMessage.Raw);
                    OpenPop.Mime.Message mes = new OpenPop.Mime.Message(bytes);
                    if (!iPartner.emailLastUpdate.HasValue)
                        iPartner.emailLastUpdate = mes.Headers.DateSent;
                    else
                    {
                        if (iPartner.emailLastUpdate.Value < mes.Headers.DateSent)
                            iPartner.emailLastUpdate = mes.Headers.DateSent;
                    }
                    OpenPop.Mime.MessagePart plainVersion = mes.FindFirstHtmlVersion();
                    if (plainVersion != null)
                    {
                        plainVersion = mes.FindFirstHtmlVersion();
                        futureNotes.Push("<hr/><div>" + mes.Headers.DateSent.ToShortDateString() + "@@" + mes.Headers.DateSent.ToLongTimeString() + "- email received - from " + iPerson.email + "</div>" + SafeMailText(plainVersion.GetBodyAsText(), true));
                    }
                    else
                    {
                        plainVersion = mes.FindFirstPlainTextVersion();
                        futureNotes.Push("<hr/><div>" + mes.Headers.DateSent.ToShortDateString() + "@@" + mes.Headers.DateSent.ToLongTimeString() + "- email received - from " + iPerson.email + "</div>" + SafeMailText(plainVersion.GetBodyAsText(), false) );
                    }
                    
                }
                catch (Exception ex)
                {

                }
            }
            foreach (var note in futureNotes)
                AppendNote(note, iPartner.note.Value, false);
            db.Entry(iPartner).State = EntityState.Modified;
            db.SaveChanges();
            //list.
            
        }
        /// <summary>
        /// Clears previous emails and removes invalid characters
        /// </summary>
        /// <param name="mailText"></param>
        /// <returns></returns>
        private string SafeMailText(string mailText, bool isHtml)
        {
            if (!isHtml)
            {
                StringBuilder builder = new StringBuilder();
                using (var strem = new StreamReader(new MemoryStream(Encoding.UTF8.GetBytes(mailText))))
                {
                    while (!strem.EndOfStream)
                    {
                        var mailLine = strem.ReadLine();
                        if (!mailLine.StartsWith(">") && !mailLine.StartsWith("<"))
                            builder.AppendLine("<div>" + mailLine.Replace("<", "&lt;").Replace("&", "&amp;").Replace(">", "&gt;").Replace("'", "&apos;").Replace("\"", "&quot;") + "</div>");
                    }
                }
                return builder.ToString();
            }
            else
            {
                HtmlDocument doc = new HtmlDocument();
                doc.OptionWriteEmptyNodes = true;
                doc.OptionUseIdAttribute = false;
                doc.OptionOutputAsXml = true;
                if (mailText.IndexOf("<body") == -1)
                {
                    mailText = "<body>" + mailText + "</body>";
                }
                if (mailText.IndexOf("<html>") == -1)
                {
                    mailText = "<html>" + mailText + "</html>";
                }
                doc.LoadHtml(mailText);
                //let's remove all prohibited elements and attributes for Evernote Markup Language
                RemoveNode(doc, "script");
                RemoveNode(doc, "applet");
                RemoveNode(doc, "base");
                RemoveNode(doc, "basefont");
                RemoveNode(doc, "bgsound");
                //body
                RemoveNode(doc, "blink");
                RemoveNode(doc, "button");
                RemoveNode(doc, "dir");
                RemoveNode(doc, "embed");
                RemoveNode(doc, "fieldset");
                RemoveNode(doc, "form");
                RemoveNode(doc, "frame");
                RemoveNode(doc, "frameset");
                RemoveNode(doc, "head");
                //html
                RemoveNode(doc, "iframe");
                RemoveNode(doc, "ilayer");
                RemoveNode(doc, "input");
                RemoveNode(doc, "isindex");
                RemoveNode(doc, "label");
                RemoveNode(doc, "layer");
                RemoveNode(doc, "legend");
                RemoveNode(doc, "link");
                RemoveNode(doc, "menu");
                RemoveNode(doc, "meta");
                RemoveNode(doc, "noframes");
                RemoveNode(doc, "noscript");
                RemoveNode(doc, "object");
                RemoveNode(doc, "optgroup");
                RemoveNode(doc, "option");
                RemoveNode(doc, "param");
                RemoveNode(doc, "plaintext");
                RemoveNode(doc, "marquee");
                RemoveNode(doc, "script");
                RemoveNode(doc, "select");
                RemoveNode(doc, "style");
                RemoveNode(doc, "textarea");
                RemoveNode(doc, "xml");
                RemoveAttributes(doc, "id");
                RemoveAttributes(doc, "class");
                RemoveAttributes(doc, "onclick");
                RemoveAttributes(doc, "ondblclick");
                RemoveAttributes(doc, "on");
                RemoveAttributes(doc, "accesskey");
                RemoveAttributes(doc, "data");
                RemoveAttributes(doc, "dynsrc");
                RemoveAttributes(doc, "tabindex");
                RemoveAttributes(doc, "data-externalstyle");
                RemoveAttributes(doc, "data-signatureblock");
                RemoveDataAttributesFromNodes(doc);
                StringBuilder result = new StringBuilder();
                StringWriter sw = new StringWriter(result);
                doc.Save(sw);
                sw.Flush();
                return sw.ToString().Replace("<html>", "").Replace("</html>", "").Replace("<body>", "").Replace("</body>", "").Replace("<?xml version=\"1.0\" encoding=\"iso-8859-1\"?>", "").Replace("<body dir=\"ltr\">", "").Replace("<?xml version=\"1.0\" encoding=\"UTF-8\"?>", "").Replace("<?xml version=\"1.0\" encoding=\"Windows-1252\"?>", "").Replace("<?xml version=\"1.0\" encoding=\"ASCII\"?>", "");
            }
        }

        /// <summary>
        /// Removes all 
        /// </summary>
        /// <param name="doc"></param>
        private void RemoveDataAttributesFromNodes(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes(@"//*[@*[starts-with(name(),'data-')]]");
            
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    foreach (var attr in node.Attributes.Cast<HtmlAttribute>().Where(o => o.Name.Contains("data-")))
                        node.Attributes.Remove(attr);
                }
            }
        }
        /// <summary>
        /// Removes all nodes from html
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="nodeName"></param>
        private void RemoveNode(HtmlDocument doc, string nodeName)
        {
            var nodes = doc.DocumentNode.SelectNodes(@"//" + nodeName);
            if (nodes != null)
            {
                var separateCollection = nodes.ToList();
                foreach (HtmlNode script in separateCollection)
                {

                    //if (script.Attributes["language"] != null)
                    //  script.Attributes.Remove("language");
                    doc.DocumentNode.SelectSingleNode(script.XPath).Remove();
                }
            }
        }
        /// <summary>
        /// Removes attribute from all nodes in html document
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="attrName"></param>
        private void RemoveAttributes(HtmlDocument doc, string attrName)
        {
            var nodes = doc.DocumentNode.SelectNodes(@"//*[@" + attrName+"]");
            if (nodes != null)
            {
                foreach (HtmlNode node in nodes)
                {
                    if (node.Attributes[attrName] != null)
                        node.Attributes.Remove(attrName);
                }
            }
        }
        /// <summary>
        /// Decodes string from Base64Url to byte array
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        private byte[] Base64UrlDecode(string arg)
        {
            string s = arg;
            s = s.Replace('-', '+'); // 62nd char of encoding
            s = s.Replace('_', '/'); // 63rd char of encoding
            switch (s.Length % 4) // Pad with trailing '='s
            {
                case 0: break; // No pad chars in this case
                case 2: s += "=="; break; // Two pad chars
                case 3: s += "="; break; // One pad char
                default: throw new System.Exception(
                "Illegal base64url string!");
            }
            return Convert.FromBase64String(s); // Standard base64 decoder
        }

        const string DefaultNoteBook = "zDefault";
        /// <summary>
        /// Show the user if they are authorized
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> AddNote(string partnerName, string noteTitle, string noteText, string partnerId)
        {

            if (SessionHelper.EvernoteCredentials != null)
            {
                var googleResult = await new IntellegesAuthorizationCodeMvcApp(this, new AppFlowMetadata(), Request.Url.GetLeftPart(UriPartial.Authority)+Url.Action("Iterate","Partner")).AuthorizeAsync(CancellationToken.None);
                if (googleResult.Credential != null)
                {

                    var authToken = SessionHelper.EvernoteCredentials.AuthToken;
                    //find existed notebook for partner
                    Notebook notebook = GetNoteStore().listNotebooks(authToken).FirstOrDefault(o => o.Name == DefaultNoteBook);
                    var pId = int.Parse(partnerId);
                    var iPartner = db.iteratePartner.FirstOrDefault(o => o.id == pId);
                    var iPerson = db.iteratePerson.FirstOrDefault(o => o.iteratePartner == pId);
                    if (notebook == null)
                    {
                        // create notebook for new user
                        notebook = CreateNoteBook(partnerName.ToString());
                    }
                    if (iPartner.note == null)
                    {
                        var note = CreateNote(noteTitle, noteText, notebook.Guid);
                        iPerson.notes = true;
                        iPartner.note = Guid.Parse(note.Guid);
                        iPartner.status = db.pr_getIteratePartnerStatusAll().FirstOrDefault(o => o.description == DefaultNoteBook).id;
                        db.Entry(iPartner).State = EntityState.Modified;
                        db.Entry(iPerson).State = EntityState.Modified;
                        db.SaveChanges();
                        SyncEmailAndNotes(googleResult.Credential, iPartner, iPerson);
                    }
                    else
                    {
                        AppendNote(noteText, iPartner.note.Value);
                        SyncEmailAndNotes(googleResult.Credential, iPartner, iPerson);
                    }
                    SessionSingleton.NeedAddEverNote = false;
                    
                    return Json("done");
                }
                else
                {
                    return Json(googleResult.RedirectUri);
                }
            }
            else
            {
                TempData["noteTitle"] = noteTitle;
                TempData["PartnerName"] = partnerName;
                TempData["noteText"] = noteText;
                TempData["partnerId"] = partnerId;
                SessionSingleton.NeedAddEverNote = true;
                return AuthorizeEverNote();
            }
        }

        public ActionResult Unauthorized()
        {
            return Content("You are unautorized");
        }

        [HttpPost]
        public virtual ActionResult IterateAllContacts()
        {
            var data = db.partner.Where(o => o.enterprise == Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            return Json(data);
        }

        [HttpPost]
        public virtual ActionResult IterateContacts(string searchText, int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode, string searchType)
        {
            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            if (touchpoint != null)
                arguments += "touchpointID=" + touchpoint + ";";
            if (group != null)
                arguments += "groupID=" + group + ";";
            if (country != null)
                arguments += "countryID=" + country + ";";
            if (partnertype != null)
                arguments += "partnertypeID=" + partnertype + ";";

            if (partnerStatus != null)
                arguments += "StatusID=" + partnerStatus + ";";


            if (txtInternalIdFind != "")
                arguments += "InternalId=" + txtInternalIdFind + ";";


            //string , string , string , string , string , string )
            if (txtDunsNumberFind != "")
                arguments += "DunsNumber=" + txtDunsNumberFind + ";";
            if (txtNameFind != "")
                arguments += "PartnerName=" + searchText + ";";
            if (txtFederalIdFind != "")
                arguments += "FederalId=" + txtFederalIdFind + ";";

            if (accesscode != "")
                arguments += "accesscode=" + accesscode + ";";

            if (txtContactEmailFind != "")
                arguments += "ContactEmail=" + txtContactEmailFind + ";";
            if (txtHROEmailFind != "")
                arguments += "HROEmail=" + txtHROEmailFind + ";";
            if (txtScoreFromFind != "")
                arguments += "ScoreFrom=" + txtScoreFromFind + ";";
            if (txtScoreToFind != "")
                arguments += "ScoreTo=" + txtScoreToFind + ";";
            if (txtAddedFromFind != "")
                arguments += "AddedFrom=" + txtAddedFromFind + ";";
            if (txtAddedToFind != "")
                arguments += "AddedTo=" + txtAddedToFind + ";";
            if (txtFullTextSearch != "")
                arguments += "FullTextSearch=" + txtFullTextSearch + ";";
            var data = db.Database.SqlQuery<view_PartnerData>("EXEC pr_dynamicFiltersPartner  'view_PartnerData' , '" + arguments + "'").ToList();
            return Json(data);
        }

       
        [HttpPost]
        public ActionResult UploadPartnerExcelData(HttpPostedFileBase excelFile)
        {
            var confirmPartnerCount = 0;
            if (excelFile != null)
            {
                if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
                }

                if (!Directory.Exists((Server.MapPath("~/uploadedFiles/Partner"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/Partner"));
                }

                // The Name of the Upload component is "attachments" 
                var file = excelFile;
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/Partner"), fileName);

                // The files are not actually saved in this demo
                file.SaveAs(physicalPath);
                var sheetname = "Sheet1";
                var excelRead = new ExcelQueryFactory(physicalPath.ToString());
                var newPartnerExcel = from a in excelRead.Worksheet<ExcelInteratePartner>(sheetname) select a;

                //dictionaries initialization
                var statuses = db.pr_getIteratePersonStatus().ToList().ToDictionary(o => o.description, p => p.id);
                var nextActions = db.pr_getIterateNextAction().ToList().ToDictionary(o => o.nextAction, p => p.id);
                var partnerStatuses = db.pr_getIteratePartnerStatusAll().ToList().ToDictionary(o => o.description, p => p.id);               

                foreach (var newPartnerItem in newPartnerExcel.ToList())
                {                    
                        if (!string.IsNullOrWhiteSpace(newPartnerItem.PARTNER_INTERNAL_ID))
                        {
                            try
                            {
                                var partnerStatus = !string.IsNullOrEmpty(newPartnerItem.CURRENT_STATUS) && partnerStatuses.ContainsKey(newPartnerItem.CURRENT_STATUS) ? partnerStatuses[newPartnerItem.CURRENT_STATUS] : 8;
                                int EMPLOYEE_COUNT=0, ANNUAL_REVENUE=0;
                                int.TryParse(newPartnerItem.EMPLOYEE_COUNT,out EMPLOYEE_COUNT);
                                int.TryParse(newPartnerItem.ANNUAL_REVENUE, out ANNUAL_REVENUE);
                                var savedPartners = db.pr_addIteratePartner(newPartnerItem.PARTNER_INTERNAL_ID,
                                    newPartnerItem.PARTNER_NAME, newPartnerItem.PARTNER_ADDRESS_ONE,
                                    newPartnerItem.PARTNER_ADDRESS_TWO, newPartnerItem.PARTNER_CITY,
                                    newPartnerItem.PARTNER_STATE, newPartnerItem.PARTNER_ZIPCODE, newPartnerItem.PARTNER_COUNTRY,
                                    newPartnerItem.PARTNER_DUNS, newPartnerItem.PARTNER_SAP_ID, EMPLOYEE_COUNT,
                                    ANNUAL_REVENUE, partnerStatus, SessionSingleton.LoggedInUserId,
                                    SessionSingleton.LoggedInUserId, DateTime.Now, true,
                                    DateTime.Now, DateTime.Now,null, SessionSingleton.LoggedInUserId,null
                                   ).FirstOrDefault();
                                var lastContact = !string.IsNullOrEmpty(newPartnerItem.LAST_CONTACT) && statuses.ContainsKey(newPartnerItem.LAST_CONTACT) ? statuses[newPartnerItem.LAST_CONTACT] : 1;
                                var previosContact = !string.IsNullOrEmpty(newPartnerItem.PREVIOUS_CONTACT) && statuses.ContainsKey(newPartnerItem.PREVIOUS_CONTACT) ? statuses[newPartnerItem.PREVIOUS_CONTACT] : 1;
                                var nextAction = !string.IsNullOrEmpty(newPartnerItem.NEXT_ACTION) && nextActions.ContainsKey(newPartnerItem.NEXT_ACTION) ? nextActions[newPartnerItem.NEXT_ACTION] : 1;
                                DateTime LAST_CONTACT_DATE, PREVIOUS_CONTACT_DATE, NEXT_ACTION_DATE;
                                var savedPersons = db.pr_addIteratePerson(newPartnerItem.PARTNER_POC_FIRST_NAME,
                                    newPartnerItem.PARTNER_POC_LAST_NAME, newPartnerItem.PARTNER_POC_TITLE,
                                    newPartnerItem.RO_EMAIL, newPartnerItem.PARTNER_POC_PHONE_NUMBER,
                                    newPartnerItem.PARTNER_CONTACT_FAX, true,
                                    DateTime.Now, DateTime.Now, (int)savedPartners, lastContact, DateTime.TryParse(newPartnerItem.LAST_CONTACT_DATE, out LAST_CONTACT_DATE) ? LAST_CONTACT_DATE : (DateTime?)null, previosContact, DateTime.TryParse(newPartnerItem.PREVIOUS_CONTACT_DATE, out PREVIOUS_CONTACT_DATE) ? PREVIOUS_CONTACT_DATE : (DateTime?)null, nextAction, DateTime.TryParse(newPartnerItem.NEXT_ACTION_DATE, out NEXT_ACTION_DATE) ? NEXT_ACTION_DATE : (DateTime?)null, newPartnerItem.NOTES == "Y" ? true : false).FirstOrDefault();
                            }
                            catch (Exception ex)
                            {
                                return Content(ex.Message);
                            }
                            confirmPartnerCount++;                       
                    }
                }
            }

            

            return Json(new { message = "Congratulations, you have uploaded " + confirmPartnerCount + " partner confirmation actions." }, "text/plain");
        }

        [GridAction]
        public ActionResult AjaxIteratePartners()
        {
            try
            {
                var result = db.pr_getIteratePartnerPerson3(SessionSingleton.LoggedInUserId).ToList();
               
                return Json(new GridModel(result), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new GridModel());
            }
        }

        [HttpPost]
        public ActionResult ClearIterateData()
        {
            bool result = false;
            try
            {
                db.pr_clearIterateAll(SessionSingleton.LoggedInUserId);
                result = true;
            }
            catch
            {
            }
            return Json(result);
        }

        public static IEnumerable<string> GetIteratePartnerStatusList()
        {
            return new string[] { "-Select new status-", "Busy", "Successful Call - Appointment", "Successful Call - Call Back", "Do Not Call", "Hung Up", "Left Message", "No Answer", "No Help", "Not In Service", "No Message Left (Call Back)", "Transferred", "Wrong Number", "Music Box", "Other" }.ToList();
            return new string[] { "-Select new status-", "Busy", "Successful Call - Appointment", "Successful Call - Call Back", "Do Not Call", "Hung Up", "Left Message", "No Answer", "No Help", "Not In Service", "No Message Left (Call Back)", "Transferred", "Wrong Number", "Music Box", "Other" }.ToList();
        }

        [HttpGet]
        public ActionResult ExportPartnerExcel()
        {
            var partnerStatuses = db.pr_getIteratePartnerStatusAll().ToList().ToDictionary(o => o.id, p => p.description); 
           // using (var context = new EntitiesDBContext())
         //   {
                //TODO : check SessionSingleton.PersonId is always 0
               // var abc = context.pr_getIteratePartnerAll(SessionSingleton.PersonId).ToList();

            var result = db.pr_getIteratePartnerPerson3(SessionSingleton.LoggedInUserId).Select(o => new ExcelInteratePartner()
            {
                PARTNER_NAME = o.name,
                ANNUAL_REVENUE = o.annualRevenue.ToString(),
                PARTNER_POC_TITLE = o.title,
                PARTNER_INTERNAL_ID = o.internalID,
                PARTNER_POC_FIRST_NAME = o.firstname,
                PARTNER_POC_LAST_NAME = o.lastname,
                PARTNER_POC_PHONE_NUMBER = o.phone,
                LAST_CONTACT = o.lastContact,
                PREVIOUS_CONTACT = o.previousContact,
                LAST_CONTACT_DATE = o.lastContactDate.ToString(),
                PREVIOUS_CONTACT_DATE = o.previousContactDate.ToString(),
                NEXT_ACTION = o.nextAction,
                NEXT_ACTION_DATE = o.nextActionDate.ToString(),
                EMPLOYEE_COUNT = o.numberOfEmployees.ToString(),
                NOTES = o.notes.HasValue ? (o.notes.Value ? "Y" : "N") : "N",
                PARTNER_CITY = o.city,
                PARTNER_ADDRESS_ONE = o.address1,
                PARTNER_ADDRESS_TWO = o.address2,
                PARTNER_POC_EMAIL_ADDRESS = o.email,
                PARTNER_COUNTRY = o.country,
                PARTNER_CONTACT_FAX = o.fax,
                PARTNER_DUNS = o.dunsnumber,
                PARTNER_SAP_ID = o.federalID,
                PARTNER_STATE = o.state,
                PARTNER_ZIPCODE = o.zipcode,
                RO_EMAIL = o.email,
                RO_FIRST_NAME = o.firstname,
                RO_LAST_NAME = o.lastname,
                CURRENT_STATUS = o.iteratePartnerStatus.HasValue ? partnerStatuses[o.iteratePartnerStatus.Value] : ""
            }).ToList();
                var stream = new MemoryStream();
                var serializer = new XmlSerializer(typeof(List<ExcelInteratePartner>));

                //We turn it into an XML and save it in the memory
                serializer.Serialize(stream, result);
                stream.Position = 0;

                //We return the XML from the memory as a .xls file
                return File(stream, "application/vnd.ms-excel", "PartnerList.xls");
           // }
        }

        //code done earlier b4 rewriting
        public ActionResult UpdateTwilioCallStatus(int TID)
        {
            //var touchpoint = db.pr_getTouchpointByProtocol(protocolId).Where(x => x.active == 1).Select(x => new { x.id, x.title }).ToList();
            var touchpoint = TID;
            iteratePartner _Obj = db.iteratePartner.Where(i => i.id == TID).FirstOrDefault();
            if (_Obj != null)
            {
                //_Obj.person = DateTime.Now; 
            }
            ViewBag.touchpoints = touchpoint;
            return Json(new { Data = touchpoint }, JsonRequestBehavior.AllowGet);
        }


        private string PreparePhoneString(string phone)
        {
            return phone.Replace("(", "").Replace(")", "").Replace(" ", "").Replace("-", "").Replace("_", "");
        }
        [HttpPost]
        public ActionResult CallPartnerNow(int id)
        {



            var iPerson = db.iteratePerson.FirstOrDefault(o => o.iteratePartner == id);
            
            var currentUser = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
            if (iPerson != null && currentUser != null)
            {
                string accountSid = ConfigurationManager.AppSettings["accountSidTwilio"].ToString(); //account sid
                string authToken = ConfigurationManager.AppSettings["authTokenTwilio"].ToString(); //auth token
                string StoreSid = "";
                string phoneNumberFrom = "+19178180225";// dialer's phone and pass according to needs.
                string phoneNumberTo = PreparePhoneString(currentUser.phone); //Recipient phone as phone.pass from above
                var client = new TwilioRestClient(accountSid, authToken);
                var options = new CallOptions();
                options.To = phoneNumberTo;
                options.From = phoneNumberFrom;
                options.Url = Request.Url.GetLeftPart(UriPartial.Authority) + ConfigurationManager.AppSettings["Twilio.URL"].ToString() + "?number=" + PreparePhoneString(iPerson.phone); //url for twilio
                options.Method = "GET";
                options.FallbackMethod = "GET";
                options.StatusCallbackMethod = "GET";
                options.Record = true;
                var call = client.InitiateOutboundCall(options);

                if (call.RestException == null)
                {
                    StoreSid = call.Sid;
                }
                else
                {
                    //Response.Write(string.Format("Error: {0}", call.RestException.Message));
                    return Json(call.RestException.Message);
                    StoreSid = "-";
                }
            }
            return Json(true);
        }


        [HttpPost]
        public ActionResult SaveIteratePartner(int partnerid,string name, string id, string address, string phone, string city, string state, string zipcode, string firstName, string lastName, string title,string email, string url, int? na, DateTime? nad, int? partnerStatus )
        {
            try
            {
                var iPartner = db.iteratePartner.FirstOrDefault(o => o.id == partnerid);
                if (iPartner != null)
                {
                    var iPerson = db.iteratePerson.FirstOrDefault(o => o.iteratePartner == partnerid);
                    if (iPerson != null)
                    {
                        iPartner.name = name;
                        iPartner.internalID = id;
                        iPartner.address1 = address;
                        iPerson.phone = phone;
                        iPartner.city = city;
                        iPartner.state = state;
                        iPartner.zipcode = zipcode;
                        iPerson.firstname = firstName;
                        iPerson.lastname = lastName;
                        iPerson.title = title;
                        iPerson.email = email;
                        iPartner.dunsnumber = url;
                        iPerson.nextAction = na;
                        iPerson.nextActionDate = nad;
                        iPartner.status = partnerStatus;
                        db.Entry(iPartner).State = EntityState.Modified;
                        db.Entry(iPerson).State = EntityState.Modified;
                        db.SaveChanges();
                        return Json(true);
                    }
                }
            }
            catch
            {
            }
            return Json(false);
        }

        [HttpPost]
        public ActionResult ChangeiteratePartnerStatus(int partnerId, int partnerStatusId)
        {
            var iPartner = db.iteratePartner.FirstOrDefault(o => o.id == partnerId);
            if (iPartner != null)
            {
                if (iPartner.note != null)
                    if (SessionHelper.EvernoteCredentials != null)
                    {

                        var newsStatus = db.pr_getIteratePartnerStatus(partnerStatusId).FirstOrDefault();
                        if (newsStatus != null)
                        {
                            var authToken = SessionHelper.EvernoteCredentials.AuthToken;
                            var noteStore = GetNoteStore();

                            var notebooks = noteStore.listNotebooks(authToken);
                            var existNoteBook = notebooks.FirstOrDefault(o => o.Name == newsStatus.description);
                            if (existNoteBook == null)
                                existNoteBook = CreateNoteBook(newsStatus.description);
                            var note = noteStore.getNote(authToken, iPartner.note.Value.ToString(), true, true, true, true);
                            note.NotebookGuid = existNoteBook.Guid;
                            noteStore.updateNote(authToken, note);
                        }
                    }
                    else return AuthorizeEverNote();
                
                iPartner.status = partnerStatusId;
                db.Entry(iPartner).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(true);
        }

        [HttpPost]
        public ActionResult ChangeIteratePartnerNextAction(int partnerId, int nextActionId, DateTime localDate)
        {
             var iPartner = db.iteratePartner.FirstOrDefault(o => o.id == partnerId);
             if (iPartner != null)
             {
                 var iPerson = iPartner.iteratePerson.FirstOrDefault();
                 if (iPerson != null)
                 {
                     iPerson.nextAction = nextActionId;
                     if (nextActionId == 1)
                     {
                         var date = localDate;
                         switch(date.DayOfWeek)
                         {
                             case DayOfWeek.Friday:
                                 date = date.AddDays(3);
                                 break;
                             case DayOfWeek.Saturday:
                                 date = date.AddDays(2);

                                 break;
                             default:
                                 date = date.AddDays(1);
                                 break;
                         }
                         iPerson.nextActionDate = date;
                     }
                     db.Entry(iPerson).State = EntityState.Modified;
                     db.SaveChanges();
                 }
             }
            return Json(true);
        }

        [HttpPost]
        public ActionResult ChangeIteratePartnerLastContact(int partnerId, int lastContactId)
        {
            var result = db.pr_getIteratePartnerStatusByLastContact(partnerId, lastContactId).FirstOrDefault();
            var iPartner = db.iteratePartner.FirstOrDefault(o => o.id == partnerId);
            if (iPartner != null)
            {
                var iPerson = iPartner.iteratePerson.FirstOrDefault();
                if (iPerson != null)
                {
                    iPerson.previousContact = iPerson.lastContact;
                    iPerson.lastContact = lastContactId;
                    iPerson.previousContactDate = iPerson.lastContactDate;
                    iPerson.lastContactDate = DateTime.Now;
                    db.Entry(iPerson).State = EntityState.Modified;
                    db.SaveChanges();
                    if (result!=null&&result.PartnerStatus != 0)
                    {
                        return ChangeiteratePartnerStatus(partnerId, result.PartnerStatus);
                    }
                    
                }
            }
            return Json(true);
        }

        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> SendIteratePartnerEmail(int partnerId, string subject, string text, DateTime userDate, bool ccSender)
        {
            try
            {
                var iPerson = db.iteratePerson.FirstOrDefault(o => o.iteratePartner == partnerId);
                
                var currentPerson = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
                if (iPerson != null && !string.IsNullOrEmpty(iPerson.email) && currentPerson != null)
                {
                    SchedulerServiceHelper.sendEmail(subject, text, iPerson.email, new System.Net.Mail.MailAddress(currentPerson.email, currentPerson.FullName), ccSender);

                    iPerson.nextAction = (int)InteratePartnerStatus.EmailSent;
                    iPerson.previousContact = iPerson.lastContact;
                    iPerson.lastContact = (int)InteratePartnerStatus.EmailSent;
                    iPerson.lastContactDate = userDate;
                    iPerson.previousContactDate = userDate;
                    iPerson.nextActionDate = userDate.AddDays(3);
                    db.Entry(iPerson).State = EntityState.Modified;
                    db.SaveChanges();
                    var addResult = await AddNote(iPerson.iteratePartner1.name, iPerson.iteratePartner1.name, "<div>" + userDate.ToShortDateString() + "@" + userDate.ToLongTimeString() + " email sent - by "+currentPerson.email+" </div>" + text, iPerson.iteratePartner1.id.ToString());
                    return addResult;
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
            return Json("done");
        }

        public ActionResult NurtureRemindIteratePartner(string internalId, string email, int partnerId, int personId)
        {            

            var currentUser = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
            if (currentUser != null && currentUser.campaign.HasValue)
            {
                var pptqId = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByTouchpointInternalIDAndEmail(currentUser.campaign, internalId, email).FirstOrDefault();
                if(pptqId!=null)
                {
                    return Json(Remind(pptqId.accesscode));
                }
                else
                {
                    return Json("NOT_RELATED");
                }
            }
            
            return Json("Current user should be related with any touchpoint");
        }


       

        public ActionResult CreateAndInviteIteratePartner(int partnerId, int personId, int partnertype, int group)
        {
            try
            {
                var currentUser = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
                if (currentUser != null && currentUser.campaign.HasValue)
                {
                    //then create new partner
                    string loadGroup = db.pr_getAccesscode().FirstOrDefault();
                    var iPartner = db.iteratePartner.FirstOrDefault(o => o.id == partnerId);
                    var iPerson = db.iteratePerson.FirstOrDefault(o => o.id == personId);
                    var partner = new partner();
                    partner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                    //TODO: STATE from string to INT, also country
                    db.pr_addPartnerSpreadsheetDataLoad(iPartner.internalID, iPartner.dunsnumber, iPartner.name, iPartner.address1, iPartner.address2, iPartner.city, "", iPartner.zipcode, "", iPerson.firstname, iPerson.lastname, iPerson.title, iPerson.phone, iPerson.email, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, partnertype, currentUser.campaign, currentUser.id, (int)PartnerStatus.Loaded, loadGroup, DateTime.Now.AddDays(4), group).ToList().FirstOrDefault();
                    var dbPartner = db.pr_getPartnerByEmailAndInternalID(CurrentInstance.EnterpriseID, iPerson.email, iPartner.internalID).FirstOrDefault();
                    var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, currentUser.campaign).FirstOrDefault();

                    Invite(dbPartner.id, ptq.id);
                    iPerson.previousContact = iPerson.lastContact;
                    iPerson.previousContactDate = iPerson.lastContactDate;
                    iPerson.lastContact = (int)InteratePartnerStatus.EmailSent;
                    iPerson.lastContactDate = DateTime.Now;
                    db.Entry(iPerson).State = EntityState.Modified;
                    db.SaveChanges();
                    var newStatus = db.pr_getIteratePartnerStatusAll().Where(o => o.description == "gDrip/Nurture").FirstOrDefault();
                    if (iPartner.note != null)
                    {
                        if (SessionHelper.EvernoteCredentials != null)
                        {
                            var store = GetNoteStore();
                            var note = store.getNote(SessionHelper.EvernoteCredentials.AuthToken, iPartner.note.ToString(), false, false, false, false);
                            UpdateNote(note.Title, note.Guid, newStatus.notebook.ToString());
                        }
                        else return Json("Please Authorize Notes Tree"); 
                    }
                    else
                    {
                        iPartner.status = newStatus.id;
                        db.Entry(iPartner).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                   
                    
                    
                    
                    return Json("Partner is invited");
                }
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
            return Json("Can't relate partner");
        }

        public async Task<ActionResult> IteratePartnerCheckoAuth()
        {
            if (SessionHelper.EvernoteCredentials == null) return AuthorizeEverNote();
            var googleResult = await new IntellegesAuthorizationCodeMvcApp(this, new AppFlowMetadata(), Request.Url.GetLeftPart(UriPartial.Authority) +Url.Action("Iterate", "Partner")).AuthorizeAsync(CancellationToken.None);
            if (googleResult.Credential == null)
                return Json(googleResult.RedirectUri);
            return Json(true);
        }

        [HttpPost]
        public ActionResult getIteratePartnerNoteIds()
        {
            var result = db.pr_getIteratePartnerPerson3(SessionSingleton.LoggedInUserId).ToList();
            return Json(result.Select(o => o.note).ToList().Distinct());
        }
    }
    
}