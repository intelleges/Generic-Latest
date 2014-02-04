using Generic.Session;
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
using System.Data.Objects;

namespace Generic.Controllers
{
    [Authorize]
    public class PartnerController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Partner/

        public virtual ActionResult Index()
        {
            var partner = db.pr_getPartnerAll(SessionSingleton.MyEnterPriseId);
            return View(partner.ToList());
        }

        //
        // GET: /Partner/Details/5

        public virtual ActionResult Details(int id = 0)
        {
            partner partner = db.pr_getPartner(id).FirstOrDefault();
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
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1");
            return View();
        }

        //
        // POST: /Partner/Create

        [HttpPost]
        public ActionResult Create(partner partner)
        {
            if (ModelState.IsValid)
            {
                db.partner.Add(partner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            return View(partner);
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
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            return View(partner);
        }

        //
        // POST: /Partner/Edit/5

        [HttpPost]
        public ActionResult Edit(partner partner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            return View(partner);
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

            string sheetname = "toUpload";
            var excelRead = new ExcelQueryFactory(physicalPath.ToString());
            // 								 									RO LastName	RO Phone	RO Email

            excelRead.AddMapping<ExcelPartner>(x => x.internalID, "Internal ID");
            excelRead.AddMapping<ExcelPartner>(x => x.name, "Name");
            excelRead.AddMapping<ExcelPartner>(x => x.address1, "Address 1");
            excelRead.AddMapping<ExcelPartner>(x => x.address2, "Address 2");
            excelRead.AddMapping<ExcelPartner>(x => x.city, "City");
            excelRead.AddMapping<ExcelPartner>(x => x.StateName, "State");
            excelRead.AddMapping<ExcelPartner>(x => x.province, "Province");
            excelRead.AddMapping<ExcelPartner>(x => x.zipcode, "Zipcode");
            excelRead.AddMapping<ExcelPartner>(x => x.CountryName, "Country");
            excelRead.AddMapping<ExcelPartner>(x => x.phone, "POC Phone");
            excelRead.AddMapping<ExcelPartner>(x => x.fax, "POC Fax");
            excelRead.AddMapping<ExcelPartner>(x => x.email, "POC Email");
            excelRead.AddMapping<ExcelPartner>(x => x.firstName, "POC FirstName");
            excelRead.AddMapping<ExcelPartner>(x => x.lastName, "POC LastName");
            excelRead.AddMapping<ExcelPartner>(x => x.title, "POC Title");
            excelRead.AddMapping<ExcelPartner>(x => x.dunsNumber, "POC DUNS");
            excelRead.AddMapping<ExcelPartner>(x => x.federalID, "POC EID");




            //   var columnnames = excelRead.GetColumnNames(sheetname);
            var partnerinExcel = from a in excelRead.Worksheet<ExcelPartner>(sheetname) select a;

            //partnerTypeTouchpointQuestionnaire objptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnerType(partnertype).ToList().Where(x => x.touchpoint == touchpoint).FirstOrDefault();

            //int i = 1;

            List<int> uploadedpartners = new List<int>();

            foreach (var partners in partnerinExcel.ToList())
            {
                if (partners.internalID != null)
                {
                    partner checkPartner = new partner();
                    using (var context = new EntitiesDBContext())
                    {
                        checkPartner = db.pr_getPartnerByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, partners.email).FirstOrDefault();
                        partners.id = checkPartner.id;
                    }

                    if (checkPartner == null)
                    {
                        partner objPartner = new partner();
                        objPartner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                        objPartner.active = true;
                        objPartner.internalID = partners.internalID;
                        objPartner.name = partners.name;
                        objPartner.address1 = partners.address1;
                        objPartner.address2 = partners.address2;
                        objPartner.city = partners.city;
                        objPartner.state = db.pr_getStateByStateCode(partners.StateName).FirstOrDefault().id;
                        objPartner.province = partners.province;
                        objPartner.zipcode = partners.zipcode;
                        objPartner.country = db.pr_getCountryByName(partners.CountryName).FirstOrDefault().id;
                        objPartner.phone = partners.phone;
                        objPartner.fax = partners.fax;
                        objPartner.email = partners.email;
                        objPartner.firstName = partners.firstName;
                        objPartner.lastName = partners.lastName;
                        objPartner.title = partners.title;
                        objPartner.dunsNumber = partners.dunsNumber;
                        objPartner.federalID = partners.federalID;
                        db.partner.Add(objPartner);
                        db.SaveChanges();

                        uploadedpartners.Add(objPartner.id);
                    }
                    else
                    {
                        uploadedpartners.Add(partners.id);
                    }

                    string accessCode = db.pr_getAccesscode().FirstOrDefault();

                    int pqt = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;

                    using (var context = new EntitiesDBContext())
                    {
                        context.pr_addPartnerPartnertypeTouchpointQuestionnaire(partners.id, pqt, accessCode, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, DateTime.Now, null, null, null, null, null, null);
                    }
                }
            }
            Session["uploadedpartnerList"] = uploadedpartners;
            Session["partnertype"] = partnertype;
            Session["touchpoint"] = touchpoint;
            ViewBag.Message = "1";
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.group = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            return View();
        }

        [HttpPost]
        public ActionResult InvitePartners()
        {
            int partnertype = (int)Session["partnertype"];
            int touchpoint = (int)Session["touchpoint"];

            string message = string.Empty;
            if (Session["uploadedpartnerList"] != null)
            {
                List<int> uploadedpartnerList = (List<int>)Session["uploadedpartnerList"];

                int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;
                // db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartnertypeTouchpointQuestionnaire
                // db.pr_modifyPartnerPartnertypeTouchpointQuestionnaire()
                foreach (int partnerId in uploadedpartnerList)
                {


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

                    var objtouchpoint = db.pr_getTouchpoint(touchpoint).FirstOrDefault();
                    Email email = new Email(amm);
                    EmailFormat emailFormat = new EmailFormat();
                    email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint,ptq);
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
                ViewBag.Message = "2";
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
            email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint,ptq);
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

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            return View();
        }
        [HttpPost]
        public ActionResult UploadPartNumber(int protocol, int partnertype, int touchpoint, HttpPostedFileBase uploadPartNumber)
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

            List<int> uploadedpartners = new List<int>();

            foreach (var partnumbers in partnerinExcel.ToList())
            {
                if (partnumbers.internalID != null)
                {
                    partner checkPartner = new partner();
                    using (var context = new EntitiesDBContext())
                    {
                        checkPartner = db.pr_getPartnerByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, partnumbers.email).FirstOrDefault();

                    }
                    int partnerID = 0;
                    if (checkPartner == null)
                    {
                        partner objPartner = new partner();

                        objPartner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                        objPartner.active = true;
                        objPartner.internalID = partnumbers.internalID;
                        objPartner.name = partnumbers.name;
                        objPartner.address1 = partnumbers.address1;
                        objPartner.address2 = partnumbers.address2;
                        objPartner.city = partnumbers.city;
                        var objstate = db.pr_getStateByStateCode(partnumbers.StateName).FirstOrDefault();
                        if (objstate != null)
                        {
                            objPartner.state = objstate.id;
                        }

                        objPartner.province = partnumbers.province;
                        objPartner.zipcode = partnumbers.zipcode;

                        var objCountry = db.pr_getCountryByName(partnumbers.CountryName).FirstOrDefault();
                        if (objCountry != null)
                        {
                            objPartner.country = objCountry.id;
                        }


                        objPartner.phone = partnumbers.phone;
                        objPartner.fax = partnumbers.fax;
                        objPartner.email = partnumbers.email;
                        objPartner.firstName = partnumbers.firstName;
                        objPartner.lastName = partnumbers.lastName;
                        objPartner.title = partnumbers.title;
                        objPartner.dunsNumber = partnumbers.dunsNumber;
                        objPartner.federalID = partnumbers.federalID;
                        objPartner.dunsNumber = partnumbers.dunsNumber;
                        objPartner.active = true;
                        objPartner.enterprise = EnterpriseID;
                        db.partner.Add(objPartner);
                        db.SaveChanges();

                        uploadedpartners.Add(objPartner.id);
                        partnerID = objPartner.id;
                    }
                    else
                    {
                        partnerID = checkPartner.id;
                        uploadedpartners.Add(partnerID);
                    }

                    string accessCode = db.pr_getAccesscode().FirstOrDefault();

                    int pqt = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault().id;
                    int pptqID = 0;
                    using (var context = new EntitiesDBContext())
                    {
                        context.Configuration.ValidateOnSaveEnabled = true;
                        var pptq = new partnerPartnertypeTouchpointQuestionnaire();
                        pptq.partner = partnerID;
                        pptq.partnerTypeTouchpointQuestionnaire = pqt;
                        pptq.invitedBy = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id;
                        pptq.accesscode = accessCode;
                        pptq.invitedDate = DateTime.Now;
                        context.partnerPartnertypeTouchpointQuestionnaire.Add(pptq);
                        context.SaveChanges();
                        //try
                        //{
                        //    context.SaveChanges();
                        //}
                        //catch (OptimisticConcurrencyException)
                        //{
                        
                        //   // context.Refresh(RefreshMode.ClientWins, db.Articles);
                        //    context.SaveChanges();
                        //}
                        pptqID = pptq.id;
                       // context.pr_addPartnerPartnertypeTouchpointQuestionnaire(partnerID, pqt, accessCode, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, DateTime.Now, null, null, null, null, null, null);
                    }

                    var checkSite = db.pr_getSiteByInternalID(EnterpriseID, partnumbers.INTERNAL_SITE_ID).FirstOrDefault();
                    int siteID = 0;
                    if (checkSite == null)
                    {
                        using (var context = new EntitiesDBContext())
                        {
                            site objsite = new site();
                            objsite.description = partnumbers.SITE_NAME;
                            objsite.sapID = partnumbers.SAP_SITE;
                            objsite.internalID = partnumbers.INTERNAL_SITE_ID;
                            objsite.sortOrder = 1;
                            objsite.active = true;
                            objsite.enterprise = EnterpriseID;
                            context.site.Add(objsite);
                            context.SaveChanges();
                            siteID = objsite.id;
                        }
                        
                    }
                    else
                    {
                        siteID = checkSite.id;
                    }

                    int partNumberID = 0;

                    var checkPartNumber = db.pr_getPartnumberByInternalID(EnterpriseID, partnumbers.PART_NUMBER_INTERNAL).FirstOrDefault();

                    if (checkPartNumber == null)
                    {
                        partnumber objPartNumber = new partnumber();
                        objPartNumber.sapID = partnumbers.PART_NUMBER_SAP;
                        objPartNumber.description = partnumbers.PART_NUMBER_SAP;
                        objPartNumber.internalId = partnumbers.PART_NUMBER_INTERNAL;
                        objPartNumber.active = true;
                        objPartNumber.partner = partnerID;
                        db.partnumber.Add(objPartNumber);
                        db.SaveChanges();
                        partNumberID = objPartNumber.id;

                    }
                    else
                    {
                        partNumberID = checkPartNumber.id;
                    }
                    using (var context2 = new EntitiesDBContext())
                    {
                      //  context2.Configuration.ValidateOnSaveEnabled = true;
                        context2.pr_addPartnumberSiteZcodePPTQ(partNumberID, siteID, string.Empty, Helpers.PartNumberHelper.Status.NOT_STARTED , pptqID);
                    }






                }
            }
            Session["uploadedpartnerList"] = uploadedpartners;
            Session["partnertype"] = partnertype;
            Session["touchpoint"] = touchpoint;
            ViewBag.Message = "1";
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            return View();
        }





        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}