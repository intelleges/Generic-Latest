using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            var person = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID);
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


                db.person.Add(person);
                db.SaveChanges();
                

                return RedirectToAction("Index");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", person.enterprise);
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description", person.personStatus);
            ViewBag.role = new SelectList(db.role, "id", "description", person.role);
            return View(person);
        }

        //
        // GET: /Person/Edit/5

        public ActionResult Edit(int id = 0)
        {
            
            person person = db.pr_getPerson(id).FirstOrDefault();
            if (person == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", person.enterprise);
            ViewBag.personStatus = new SelectList(db.personStatus, "id", "description", person.personStatus);
            ViewBag.role = new SelectList(db.role, "id", "description", person.role);
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
            ViewBag.role = new SelectList(db.role, "id", "description", person.role);
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

        public ActionResult AssignGroupToPerson()
        {
            
            ViewBag.groups = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            return View();
        }
        [HttpPost]
        public ActionResult AssignGroupToPerson(int person,int groups)
        {

            db.pr_addPersonGroup(groups, person);
            ViewBag.groups = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");
            return View();

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}