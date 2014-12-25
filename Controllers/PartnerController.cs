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
namespace Generic.Controllers
{
    [Authorize]
    public class PartnerController : Controller
    {
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
        public bool RemoveStockNumbers(string[] values)
        {
            var result = false;
            try
            {
                if (values != null)
                {
                    foreach (var value in values)
                        db.pr_archivePartnumber(int.Parse(value));
                    result = true;
                }
            }
            catch
            {
            }
            return result;
        }
        [HttpPost]
        public bool RemoveStockNumber(int value)
        {
            var result = false;
            try
            {

                db.pr_archivePartnumber(value);
                result = true;

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
            if (pptq!=null)
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
            //ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            //ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1");

            ViewBag.state = new SelectList(db.state, "id", "name");
            ViewBag.country = new SelectList(db.country, "id", "name");
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
           
            ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId), "id", "name");
            //ViewBag.owner = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstname");
            ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstname");
            ViewBag.owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();

            return View();
        }



        //
        // POST: /Partner/Create

        [HttpPost]
        public ActionResult Create(partner partner, int? protocol, int? partnertype, int? touchpoint, int? group, DateTime? DueDate)
        {
            List<Tuple<int, string>> uploadedpartners = new List<Tuple<int, string>>();


            string loadGroup = db.pr_getAccesscode().FirstOrDefault();
            partner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;

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
                ViewBag.state = new SelectList(db.state, "id", "name");
                ViewBag.country = new SelectList(db.country, "id", "name");
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
                ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
                ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId), "id", "name");
                ViewBag.owner = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstname");
                ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstname");
                return View();
            }
            catch
            {
                ViewBag.Message = "error";

                //  alertify-ok"
                ViewBag.state = new SelectList(db.state, "id", "name", partner.state);
                ViewBag.country = new SelectList(db.country, "id", "name", partner.country);
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name", protocol);
                ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name", partnertype);
                ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId), "id", "name", group);
                ViewBag.owner = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstname", partner.owner);
                ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstname", partner.author);
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
                var questionar = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partner.id).ToList();
                foreach (var quest in questionar)
                {
                    try
                    {

                        Session["accessCode"] = quest.accesscode;
                        var id = Areas.RegistrationArea.Controllers.HomeController.FillPdfHtml(ViewBag, db, Session, Server);
                        string htmltext = this.RenderActionResultToString(this.View("~/Areas/RegistrationArea/Views/Home/CustomizedQuestionnaireSurveyPdfDownload.cshtml"));
                        var bytes = (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(htmltext);
                        db.pr_addPPTQpdf(id, bytes);
                    }
                    catch (Exception ex)
                    {
                    }
                }
                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            //if(partnerDB)
                      
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

        public ActionResult UploadPartner()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.group = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            //ViewBag.title = db.touchpoint.First().title;

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


        public string Invite(int partnerId)
        {


            int ptq = 30;// db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;
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

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
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

            SqlConnection conn = new SqlConnection(db.Database.Connection.ConnectionString);
            conn.Open();
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

                    if (pdf == null)
                    {
                        //if pdf was deleted from db but questinnarie was completed then we created customized pdf again
                        if (pptq.status == 8)
                            Response.Redirect("~/Registration/Home/CustomizedPDFConfirmation");
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
            var result = "Reminder sent to " + pptq.partner1.name + " with access code " + accessCode ;
            if (pptq != null)
            {
                if (new int[] { 6, 7 }.Contains(pptq.status))
                    SchedulerServiceHelper.SendFirstReminderByPptq(pptq.id);
                else
                {
                    result = "The status for "+pptq.partner1.name+" with "+accessCode+" access code does not permit reminders at this time. Please contact your system adminitrator.";
                }

            }
            return result;
        }
    }
}