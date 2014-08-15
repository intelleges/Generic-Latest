using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Models;
using Generic.Helpers.Utility;
using Generic.Helpers;
using System.IO;
using LinqToExcel;
using Generic.ViewModel;

namespace Generic.Controllers
{
    [Authorize]
    public class PersonController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Person/

        public ActionResult Index()
        {

            //var personGroup =db.p
            //var person = db.person.Include(p => p.enterprise1).Include(p => p.personStatu).Include(p => p.role1);
            var person = db.pr_getPersonAll(SessionSingleton.MyEnterPriseId);
            return View(person.ToList());
        }

        //
        // GET: /Person/Details/5

        public ActionResult Details(int id = 0)
        {
            person person = db.pr_getPerson(id).FirstOrDefault();
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        //
        // GET: /Person/Create

        public ActionResult UploadPerson()
        {
            return View();
        }
        [HttpPost]
        public ActionResult UploadPerson(HttpPostedFileBase uploadPerson)
        {

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
            }

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles/Person"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/Person"));
            }

            // The Name of the Upload component is "attachments" 
            var file = uploadPerson;

            // Some browsers send file names with full path. This needs to be stripped.
            var fileName = Path.GetFileName(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/Person"), fileName);

            // The files are not actually saved in this demo 
            file.SaveAs(physicalPath);

            int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;

            string sheetname = "addUser";
            var excelRead = new ExcelQueryFactory(physicalPath.ToString());



            excelRead.AddMapping<ExcelPerson>(x => x.internalId, "internalID");
            excelRead.AddMapping<ExcelPerson>(x => x.state, "StateName");
            excelRead.AddMapping<ExcelPerson>(x => x.country, "CountryName");



            //   var columnnames = excelRead.GetColumnNames(sheetname);
            var personinExcel = from a in excelRead.Worksheet<ExcelPerson>(sheetname) select a;


            List<Tuple<int, string>> uploadedperson = new List<Tuple<int, string>>();
            person objInvitingUser = db.pr_getPersonByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
            int countpartNumbers = personinExcel.Count();
            int recordNumber = 1;
            foreach (var personItem in personinExcel.ToList())
            {
                if (personItem.internalId != null)
                {
                    if (personItem.phone == null)
                    {
                        ErrorView objerrorView = new ErrorView();
                        objerrorView.errorMessage = "Record " + recordNumber.ToString() + " of " + countpartNumbers + " has invalid values.";
                        return PartialView("_Error", objerrorView);
                    }
                }
                recordNumber++;
            }


            //  string loadGroup = db.pr_getAccesscode().FirstOrDefault();
            foreach (var objPerson in personinExcel.ToList())
            {
                if (objPerson.internalId != null)
                {

                    var objstateSpreadSheet = db.pr_getStateByStateCode(objPerson.StateName).FirstOrDefault();
                    int? stateIdSpreadSheet = null;
                    if (objstateSpreadSheet != null)
                    {
                        stateIdSpreadSheet = objstateSpreadSheet.id;
                    }

                    var objCountrySpreadSheet = db.pr_getCountryByName(objPerson.CountryName).FirstOrDefault();
                    int? countryIdSpreadsheet = null;
                    if (objCountrySpreadSheet != null)
                    {
                        countryIdSpreadsheet = objCountrySpreadSheet.id;
                    }

                    using (var context = new EntitiesDBContext())
                    {
                        person objaddPerson = new person();

                        objaddPerson.campaign = objInvitingUser.campaign;

                        objaddPerson.internalId = objPerson.internalId;
                        objaddPerson.firstName = objPerson.firstName;
                        objaddPerson.lastName = objPerson.lastName;
                        objaddPerson.title = objPerson.title;
                        objaddPerson.suffix = objPerson.suffix;
                        objaddPerson.address1 = objPerson.address1;
                        objaddPerson.address2 = objPerson.address2;
                        objaddPerson.city = objPerson.city;
                        objaddPerson.zipcode = objPerson.zipcode;
                        objaddPerson.phone = objPerson.phone;
                        objaddPerson.email = objPerson.email;


                        objaddPerson.personStatus = (int)PersonHelper.PersonStatus.Loaded;
                        objaddPerson.active = 1;
                        objaddPerson.ismanager = 0;
                        objaddPerson.partnerPerPage = 500;
                        objaddPerson.riskType = 0;
                        objaddPerson.loadHistory = 0;
                        objaddPerson.state = stateIdSpreadSheet;
                        objaddPerson.country = countryIdSpreadsheet;
                        objaddPerson.passWord = db.pr_getAccesscode().FirstOrDefault();

                        objaddPerson.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                        db.person.Add(objaddPerson);
                        db.SaveChanges();

                        int? PartnerId = objaddPerson.id;
                        uploadedperson.Add(new Tuple<int, string>(int.Parse(PartnerId.ToString()), ""));
                    }



                }
            }
            Session["uploadedPersonList"] = uploadedperson;

            //   Session["loadGroup"] = loadGroup;
            ViewBag.Message = "1";

            return View();
        }

        public ActionResult Create()
        {
            ViewBag.campaign = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            ViewBag.manager = new SelectList(db.pr_getPersonByEnterprise2(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");

            ViewBag.state = new SelectList(db.pr_getStateAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            return View();
        }

        //
        // POST: /Person/Create

        [HttpPost]
        public ActionResult Create(person person)
        {
            if (ModelState.IsValid)
            {

                person.personStatus = (int)PersonHelper.PersonStatus.Invited;
                person.active = 1;
                person.ismanager = 0;
                person.partnerPerPage = 500;
                person.riskType = 0;
                person.loadHistory = 0;
                person.passWord = db.pr_getAccesscode().FirstOrDefault();

                person.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                db.person.Add(person);
                db.SaveChanges();
                SessionSingleton.PersonId = person.id;



                person objdefaultSystemMaster = db.pr_getSystemMaster(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();


                int? PersonId = person.id;



                // Email Invite
                var objSystemMaster = db.pr_getPerson(SessionSingleton.PersonId).FirstOrDefault();

                enterprise objEnterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();


                autoMailMessage objamm = new autoMailMessage();

                objamm.subject = "Welcome to Intelleges for [Enterprise Name]: [Touchpoint Title]";
                //     objamm.text = "Dear " + objSystemMaster.firstName + "<br> please click on this <a href='https://www.intelleges.com/mvcmt/Generic'>hyperlink</a> and enter password " + objSystemMaster.passWord + " to login to the system.";
                objamm.text = @"Hi [User Firstname],<br>

You have a new account at Intelleges for [Enterprise Name] [Touchpoint Title].<br>

As you know the purpose of this system is to help us [Touchpoint Purpose].<br>

Your username is [User Email]. The administrator has set your password [Temporary Access Code].<br>

You can sign in to Intelleges services at:<br>

[Project Url]<br>

Note: you will need to change your password once you login.<br>

If you have any questions please contact me [User Inviting Email] or contact your system master [System Master Email]<br>

Thanks in advance.<br>

[User Inviting Firstname] [User Inviting Last Name]<br>
[Enterprise Name]";


                Email email = new Email(objamm);
                person objInvitingUser = db.pr_getPersonByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();

                touchpoint objCurrentTouchpoint = db.pr_getTouchpoint(objInvitingUser.campaign).FirstOrDefault();

                EmailFormat emailFormat = new EmailFormat();
                email.subject = emailFormat.sGetEmailBody(email.subject, objInvitingUser, objSystemMaster, objCurrentTouchpoint, objEnterprise, objdefaultSystemMaster);
                //   email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
                email.body = emailFormat.sGetEmailBody(email.body, objInvitingUser, objSystemMaster, objCurrentTouchpoint, objEnterprise, objdefaultSystemMaster);
                //  email.body = objamm.text;
                email.emailTo = objSystemMaster.email;
                SendEmail objSendEmail = new SendEmail();
                objSendEmail.sendEmail(email);




                return RedirectToAction("AssignGroup", "Person");
            }

            ViewBag.campaign = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            ViewBag.manager = new SelectList(db.pr_getPersonByEnterprise2(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");

            ViewBag.state = new SelectList(db.pr_getStateAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");


            return View(person);
        }

        public ActionResult CreatePerson()
        {
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description");
            ViewBag.role = new SelectList(db.role, "id", "description");

            ComboBoxModel objComboboxCountry = new ComboBoxModel();

            ViewBag.comboboxCountry = objComboboxCountry;
            if (SessionSingleton.EnterPriseId == null)
            {
                SessionSingleton.EnterPriseId = Generic.Helpers.CurrentInstance.EnterpriseID;
            }
            ViewBag.countries = db.pr_getCountryAll(SessionSingleton.EnterPriseId);
            return View();
        }

        //
        // POST: /Person/Create

        [HttpPost]
        public ActionResult CreatePerson(person person, string coordinator, string coordinatorEmail)
        {
            if (ModelState.IsValid)
            {
                person objdefaultSystemMaster = db.pr_getSystemMaster(1).FirstOrDefault();


                int? PersonId = db.pr_addSystemMasterToEnterprise(SessionSingleton.EnterPriseId, person.internalId, person.firstName, person.lastName, person.email, person.phone, person.zipcode, person.country).FirstOrDefault();

                SessionSingleton.PersonId = (int)PersonId;

                // Email Invite
                var objSystemMaster = db.pr_getPerson(SessionSingleton.PersonId).FirstOrDefault();

                enterprise objEnterprise = db.pr_getEnterprise(SessionSingleton.EnterPriseId).FirstOrDefault();

                db.pr_addEnterpriseSystemInfo(null, null, null, coordinator, null, coordinatorEmail, null, null, true, SessionSingleton.EnterPriseId);



                autoMailMessage objamm = new autoMailMessage();

                objamm.subject = "Welcome to Intelleges for [Enterprise Name]: [Touchpoint Title]";
                //     objamm.text = "Dear " + objSystemMaster.firstName + "<br> please click on this <a href='https://www.intelleges.com/mvcmt/Generic'>hyperlink</a> and enter password " + objSystemMaster.passWord + " to login to the system.";
                objamm.text = @"Hi [User Firstname],<br>

You have a new account at Intelleges for [Enterprise Name] [Touchpoint Title].<br>

As you know the purpose of this system is to help us [Touchpoint Purpose].<br>

Your username is [User Email]. The administrator has set your password [Temporary Access Code].<br>

You can sign in to Intelleges services at:<br>

[Project Url]<br>

Note: you will need to change your password once you login.<br>

If you have any questions please contact me [User Inviting Email] or contact your system master [System Master Email]<br>

Thanks in advance.<br>

[User Inviting Firstname] [User Inviting Last Name]<br>
[Enterprise Name]";


                Email email = new Email(objamm);
                person objInvitingUser = db.pr_getPersonByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();

                touchpoint objCurrentTouchpoint = db.pr_getTouchpoint(objInvitingUser.campaign).FirstOrDefault();

                EmailFormat emailFormat = new EmailFormat();
                email.subject = emailFormat.sGetEmailBody(email.subject, objInvitingUser, objSystemMaster, objCurrentTouchpoint, objEnterprise, objdefaultSystemMaster);
                //   email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
                email.body = emailFormat.sGetEmailBody(email.body, objInvitingUser, objSystemMaster, objCurrentTouchpoint, objEnterprise, objdefaultSystemMaster);
                //  email.body = objamm.text;
                email.emailTo = objSystemMaster.email;
                SendEmail objSendEmail = new SendEmail();
                objSendEmail.sendEmail(email);

                return RedirectToAction("AssignGroup", "Person");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", person.enterprise);
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description", person.personStatus);

            return View(person);
        }

        public ActionResult Edit(int id = 0)
        {

            person person = db.pr_getPerson(id).FirstOrDefault();
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", person.enterprise);
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description", person.personStatus);

            return View(person);
        }

        //
        // POST: /Person/Edit/5

        [HttpPost]
        public ActionResult Edit(person person)
        {
            if (ModelState.IsValid)
            {
                db.Entry(person).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", person.enterprise);
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description", person.personStatus);

            return View(person);
        }

        //
        // GET: /Person/Delete/5

        public ActionResult Delete(int id = 0)
        {
            person person = db.pr_getPerson(id).FirstOrDefault();
            if (person == null)
            {
                return HttpNotFound();
            }
            return View(person);
        }

        //
        // POST: /Person/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            person person = db.pr_getPerson(id).FirstOrDefault();
            db.person.Remove(person);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult ResetPassword()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ResetPassword(string password)
        {
            person objUser = db.pr_getPersonByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
            objUser.passWord = password;
            objUser.personStatus = (int)PersonHelper.PersonStatus.Registered;
            objUser.riskType = 1;
            objUser.loadHistory = 1;
            db.Entry(objUser).State = EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = 1;
            return View();
        }

        public ActionResult AssignGroup(int person = 0)
        {
            try
            {
                if (SessionSingleton.PersonId != 0)
                {
                    ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email", SessionSingleton.PersonId);
                    person = SessionSingleton.PersonId;
                }
                else
                {
                    ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
                }


            }
            catch
            {
            }

            var assignedGroups = db.pr_getGroupByPerson(person).ToList();

            var allGroups = db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            var unassignedGroups = allGroups.ToList().Where(x => !assignedGroups.ToList().Contains(x)).ToList();
            ViewData["grouplist"] = new MultiSelectList(unassignedGroups, "id", "name");
            ViewData["assignedGroups"] = new MultiSelectList(assignedGroups, "id", "name");
            return View();
        }


        [HttpPost]
        public ActionResult AssignGroup(int person, FormCollection collection)
        {
            try
            {
                string assignedGroups = collection["assignedGroups"];
                var assignedGroupList = assignedGroups.Split(',');


                var assignedGroupsOld = db.pr_getGroupByPerson(person).ToList();

                foreach (var items in assignedGroupsOld)
                {
                    db.pr_removePersonGroup(person, items.id);
                }


                foreach (var item in assignedGroupList)
                {
                    db.pr_addPersonGroup(int.Parse(item), person);
                }

                return RedirectToAction("AssignRole", "Person");
            }
            catch { }

            if (SessionSingleton.PersonId != 0)
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email", SessionSingleton.PersonId);
                person = SessionSingleton.PersonId;
            }
            else
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            }
            var assignedGroups2 = db.pr_getGroupByPerson(person).ToList();

            var allGroups = db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            var unassignedGroups = allGroups.ToList().Where(x => !assignedGroups2.ToList().Contains(x)).ToList();
            ViewData["grouplist"] = new MultiSelectList(unassignedGroups, "id", "name");
            ViewData["assignedGroups"] = new MultiSelectList(assignedGroups2, "id", "name");
            return View();
        }

        public ActionResult AssignGroupToPerson()
        {
            if (SessionSingleton.PersonId != 0)
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email", SessionSingleton.PersonId);
            }
            else
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            }

            ViewBag.groups = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            return View();
        }
        [HttpPost]
        public ActionResult AssignGroupToPerson(int person, int groups)
        {


            db.pr_addPersonGroup(groups, person);
            ViewBag.groups = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            return View();

        }


        public ActionResult AssignRoleToPerson()
        {
            if (SessionSingleton.PersonId != 0)
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email", SessionSingleton.PersonId);
            }
            else
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            }

            ViewBag.roles = new SelectList(db.pr_getRoleAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");

            return View();
        }
        [HttpPost]
        public ActionResult AssignRoleToPerson(int person, int roles)
        {


            db.pr_addPersonRole(person, roles);
            ViewBag.roles = new SelectList(db.pr_getRoleAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            return View();

        }
        public ActionResult AssignRole(int person = 0)
        {
            try
            {
                if (SessionSingleton.PersonId != 0)
                {
                    ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email", SessionSingleton.PersonId);
                    person = SessionSingleton.PersonId;
                }
                else
                {
                    ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
                }


            }
            catch
            {
            }

            var assignedRoles = db.pr_getRoleByPerson(person).ToList();

            var allRoles = db.pr_getRoleAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            var unassignedRoles = allRoles.ToList().Where(x => !assignedRoles.ToList().Contains(x)).ToList();
            ViewData["Rolelist"] = new MultiSelectList(unassignedRoles, "id", "description");
            ViewData["assignedRoles"] = new MultiSelectList(assignedRoles, "id", "description");
            return View();
        }


        [HttpPost]
        public ActionResult AssignRole(int person, FormCollection collection)
        {
            string assignedRoles = collection["assignedRoles"];
            var assignedRoleList = assignedRoles.Split(',');


            var assignedRolesOld = db.pr_getRoleByPerson(person).ToList();

            foreach (var items in assignedRolesOld)
            {
                db.pr_removePersonRole(person, items.id);
            }


            foreach (var item in assignedRoleList)
            {
                db.pr_addPersonRole(person, int.Parse(item));
            }

            if (SessionSingleton.PersonId != 0)
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email", SessionSingleton.PersonId);
                person = SessionSingleton.PersonId;
            }
            else
            {
                ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            }
            var assignedRoles2 = db.pr_getRoleByPerson(person).ToList();

            var allRoles = db.pr_getRoleAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            var unassignedRoles = allRoles.ToList().Where(x => !assignedRoles2.ToList().Contains(x)).ToList();
            ViewData["Rolelist"] = new MultiSelectList(unassignedRoles, "id", "description");
            ViewData["assignedRoles"] = new MultiSelectList(assignedRoles2, "id", "description");
            return View();
        }

        public person GetPerson(string username)
        {
            person objPerson = new person();
            try
            {
                objPerson = db.pr_getPersonByEmail(Generic.Helpers.CurrentInstance.EnterpriseID, username).FirstOrDefault();
            }
            catch { }
            return objPerson;

        }

        public ActionResult ArchivePerson()
        {
            string arguments = Session["personsearch"].ToString() + "active=1;";
            List<view_PersonData> objPersonDataList = db.Database.SqlQuery<view_PersonData>("EXEC pr_dynamicFiltersPerson  'view_PersonData' , '" + arguments + "'").ToList();
            List<PersonViewModel> objPersonViewModelList = ConvertToPersonViewModel(objPersonDataList);
            ViewBag.searchType = "Archive";
            return View("RemovePerson", objPersonViewModelList);
        }

        public ActionResult RemovePerson()
        {
            string arguments = Session["personsearch"].ToString() + "active=1;";
            List<view_PersonData> objPersonDataList = db.Database.SqlQuery<view_PersonData>("EXEC pr_dynamicFiltersPerson  'view_PersonData' , '" + arguments + "'").ToList();
            List<PersonViewModel> objPersonViewModelList = ConvertToPersonViewModel(objPersonDataList);
            ViewBag.searchType = "Remove";
            return View("RemovePerson", objPersonViewModelList);

        }

        [HttpPost]
        public ActionResult RemovePerson(string searchType, List<int> chkSelect)
        {
            if (searchType == "Remove")
            {
                foreach (int personID in chkSelect)
                {
                    db.pr_removePerson(personID);
                }

                ViewBag.searchType = "Remove";
                return RedirectToAction("RemovePerson");
            }
            else if (searchType == "Archive")
            {
                foreach (int personID in chkSelect)
                {
                    db.pr_archivePerson(personID);
                }
                ViewBag.searchType = "Archive";
                return RedirectToAction("ArchivePerson");
            }
            else if (searchType == "Restore")
            {
                foreach (int personID in chkSelect)
                {
                    db.pr_unArchivePerson(personID);
                }
                ViewBag.searchType = "Restore";
                return RedirectToAction("RestorePerson");
            }
            else
            {
                return RedirectToAction("FindPerson");
            }
        }

        public ActionResult RestorePerson()
        {
            string arguments = Session["personsearch"].ToString() + "active=0;";

            List<view_PersonData> objPersonDataList = db.Database.SqlQuery<view_PersonData>("EXEC pr_dynamicFiltersPerson  'view_PersonData' , '" + arguments + "'").ToList();
            List<PersonViewModel> objPersonViewModelList = ConvertToPersonViewModel(objPersonDataList);
            ViewBag.searchType = "Restore";
            return View("RemovePerson", objPersonViewModelList);

        }

        public ActionResult FindPerson(string searchType)
        {
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");

            ViewBag.group = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnerStatus = new SelectList(db.pr_getPartnerStatusAll(), "id", "description");

            ViewBag.searchType = searchType;
            return View();
        }



        [HttpPost]
        public ActionResult FindPerson(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode, string searchType)
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
                arguments += "Name=" + txtNameFind + ";";
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
            //var objPartners2 =   db.Database.ExecuteSqlCommand("Yourprocedure @param, @param1", param1, param2);


            //   var objPartners = db.Database.SqlQuery<view_PersonData>("EXEC pr_dynamicFiltersPerson  'view_PersonData' , '" + arguments + "'").ToList();
            Session["personsearch"] = arguments;
            //Session["person"] = objPartners;
            //TempData["person"] = objPartners;

            //  return RedirectToAction("FindPersonResult", objPartners);
            if (searchType == "Remove")
            {
                return RedirectToAction("RemovePerson");
            }
            else if (searchType == "Archive")
            {
                return RedirectToAction("ArchivePerson");
            }
            else if (searchType == "Restore")
            {
                return RedirectToAction("RestorePerson");
            }
            else
            {
                return RedirectToAction("FindPersonResult");
            }
        }

        public ActionResult FindPersonResult()
        {
            try
            {
                string arguments = Session["personsearch"].ToString();// +"active=1;";
                Session["person"] = db.Database.SqlQuery<view_PersonData>("EXEC pr_dynamicFiltersPerson  'view_PersonData' , '" + arguments + "'").ToList();
                List<view_PersonData> abc = (List<view_PersonData>)Session["person"];
                // List<view_PersonData> objPartnerViewModelList = ConvertToPartnerViewModel(abc);

                return View(abc);
            }
            catch
            {
                return RedirectToAction("FindPerson");

            }
            //List<view_PartnerData> abc = (List<view_PartnerData>)TempData["partner"];
            //Session["partner"] 


        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private List<PersonViewModel> ConvertToPersonViewModel(List<view_PersonData> iview_PersonDataList)
        {
            List<PersonViewModel> objPersonViewModelList = new List<PersonViewModel>();

            foreach (var iview_PersonDataData in iview_PersonDataList)
            {
                PersonViewModel objPersonViewModel = new PersonViewModel();
                objPersonViewModel.enterprise = iview_PersonDataData.enterprise;
                objPersonViewModel.id = iview_PersonDataData.id;
                objPersonViewModel.Touchpoint = iview_PersonDataData.Touchpoint;
                objPersonViewModel.firstname = iview_PersonDataData.firstname;
                objPersonViewModel.title = iview_PersonDataData.title;
                objPersonViewModel.internalID = iview_PersonDataData.internalID;
                objPersonViewModel.email = iview_PersonDataData.email;
                objPersonViewModel.Country = iview_PersonDataData.Country;
                objPersonViewModel.State = iview_PersonDataData.State;
                objPersonViewModel.Default_Touchpoint = iview_PersonDataData.Default_Touchpoint;
                objPersonViewModel.personStatusID = iview_PersonDataData.personStatusID;
                objPersonViewModel.Person_Status = iview_PersonDataData.Person_Status;
                objPersonViewModel.Group_Count = iview_PersonDataData.Group_Count;
                objPersonViewModel.Role_Count = iview_PersonDataData.Role_Count;
                objPersonViewModel.Person_Status = iview_PersonDataData.Person_Status;
                objPersonViewModel.IsSelected = false;



                objPersonViewModelList.Add(objPersonViewModel);
            }
            return objPersonViewModelList;

        }
    }
}