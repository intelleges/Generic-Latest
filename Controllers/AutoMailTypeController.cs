using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class AutoMailTypeController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /AutoMailType/

        public ActionResult Index()
        {
            return View(db.pr_getAutoMailTypeAll().ToList());
        }

        //
        // GET: /AutoMailType/Details/5

        public ActionResult Details(int id = 0)
        {
            autoMailType automailtype = db.pr_getAutoMailType(id).FirstOrDefault();
            if (automailtype == null)
            {
                return HttpNotFound();
            }
            return View(automailtype);
        }

        //
        // GET: /AutoMailType/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /AutoMailType/Create

        [HttpPost]
        public ActionResult Create(autoMailType automailtype)
        {
            if (ModelState.IsValid)
            {
                db.autoMailType.Add(automailtype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(automailtype);
        }

        //
        // GET: /AutoMailType/Edit/5

        public ActionResult Edit(int id = 0)
        {
            autoMailType automailtype = db.pr_getAutoMailType(id).FirstOrDefault();
            if (automailtype == null)
            {
                return HttpNotFound();
            }
            return View(automailtype);
        }

        //
        // POST: /AutoMailType/Edit/5

        [HttpPost]
        public ActionResult Edit(autoMailType automailtype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(automailtype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(automailtype);
        }

        //
        // GET: /AutoMailType/Delete/5

        public ActionResult Delete(int id = 0)
        {
            autoMailType automailtype = db.pr_getAutoMailType(id).FirstOrDefault();
            if (automailtype == null)
            {
                return HttpNotFound();
            }
            return View(automailtype);
        }

        //
        // POST: /AutoMailType/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            autoMailType automailtype = db.pr_getAutoMailType(id).FirstOrDefault();
            db.autoMailType.Remove(automailtype);
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