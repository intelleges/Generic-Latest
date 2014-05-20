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

namespace Generic.Controllers
{
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

        public ActionResult Create()
        {
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description");
            ViewBag.role = new SelectList(db.role, "id", "description");
            return View();
        }

        //
        // POST: /Person/Create

        [HttpPost]
        public ActionResult Create(person person)
        {
            if (ModelState.IsValid)
            {
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

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", person.enterprise);
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description", person.personStatus);

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

                SessionSingleton.PersonId =(int)PersonId;

                // Email Invite
                var objSystemMaster= db.pr_getPerson(SessionSingleton.PersonId).FirstOrDefault();

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
                db.pr_addPersonRole(person,int.Parse(item));
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


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}