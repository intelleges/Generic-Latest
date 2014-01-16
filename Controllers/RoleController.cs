using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class RoleController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Role/

        public ActionResult Index()
        {
            var role = db.role.Include(r => r.enterprise1);
            return View(role.ToList());
        }

        //
        // GET: /Role/Details/5

        public ActionResult Details(int id = 0)
        {
            role role = db.role.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // GET: /Role/Create

        public ActionResult Create(long Id)
        {
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(role role)
        {
            if (ModelState.IsValid)
            {
                db.role.Add(role);
                db.SaveChanges();
                return RedirectToAction("Create", "Group", new {Id=role.id });
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", role.enterprise);
            return View(role);
        }

        //
        // GET: /Role/Edit/5

        public ActionResult Edit(int id = 0)
        {
            role role = db.role.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", role.enterprise);
            return View(role);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", role.enterprise);
            return View(role);
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(int id = 0)
        {
            role role = db.role.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            role role = db.role.Find(id);
            db.role.Remove(role);
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