using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class DomainController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Domain/

        public ActionResult Index()
        {
            return View(db.domain.ToList());
        }

        //
        // GET: /Domain/Details/5

        public ActionResult Details(int id = 0)
        {
            domain domain = db.domain.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        //
        // GET: /Domain/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Domain/Create

        [HttpPost]
        public ActionResult Create(domain domain)
        {
            if (ModelState.IsValid)
            {
                domain.active = true;
                db.domain.Add(domain);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(domain);
        }

        //
        // GET: /Domain/Edit/5

        public ActionResult Edit(int id = 0)
        {
            domain domain = db.domain.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        //
        // POST: /Domain/Edit/5

        [HttpPost]
        public ActionResult Edit(domain domain)
        {
            if (ModelState.IsValid)
            {
                db.Entry(domain).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(domain);
        }

        //
        // GET: /Domain/Delete/5

        public ActionResult Delete(int id = 0)
        {
            domain domain = db.domain.Find(id);
            if (domain == null)
            {
                return HttpNotFound();
            }
            return View(domain);
        }

        //
        // POST: /Domain/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            domain domain = db.domain.Find(id);
            db.domain.Remove(domain);
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