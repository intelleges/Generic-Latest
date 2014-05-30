using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class AgencyController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Agency/

        public ActionResult Index()
        {
            var agency = db.agency.Include(a => a.enterprise1);
            return View(agency.ToList());
        }

        //
        // GET: /Agency/Details/5

        public ActionResult Details(int id = 0)
        {
            agency agency = db.agency.Find(id);
            if (agency == null)
            {
                return HttpNotFound();
            }
            return View(agency);
        }

        //
        // GET: /Agency/Create

        public ActionResult Create()
        {
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            return View();
        }

        //
        // POST: /Agency/Create

        [HttpPost]
        public ActionResult Create(agency agency)
        {
            if (ModelState.IsValid)
            {
                agency.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                agency.active = true;
                db.agency.Add(agency);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", agency.enterprise);
            return View(agency);
        }

        //
        // GET: /Agency/Edit/5

        public ActionResult Edit(int id = 0)
        {
            agency agency = db.agency.Find(id);
            if (agency == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", agency.enterprise);
            return View(agency);
        }

        //
        // POST: /Agency/Edit/5

        [HttpPost]
        public ActionResult Edit(agency agency)
        {
            if (ModelState.IsValid)
            {
                db.Entry(agency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", agency.enterprise);
            return View(agency);
        }

        //
        // GET: /Agency/Delete/5

        public ActionResult Delete(int id = 0)
        {
            agency agency = db.agency.Find(id);
            if (agency == null)
            {
                return HttpNotFound();
            }
            return View(agency);
        }

        //
        // POST: /Agency/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            agency agency = db.agency.Find(id);
            db.agency.Remove(agency);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}