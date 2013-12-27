using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class TouchpointController : Controller
    {
        private hs3MVCMTQa2Entities db = new hs3MVCMTQa2Entities();

        //
        // GET: /Touchpoint/

        public ActionResult Index()
        {
            return View(db.pr_getTouchpointAll());
        }

        //
        // GET: /Touchpoint/Details/5

        public ActionResult Details(int id = 0)
        {
            touchpoint touchpoint = db.pr_getTouchpoint(id).FirstOrDefault();
            if (touchpoint == null)
            {
                return HttpNotFound();
            }
            return View(touchpoint);
        }

        //
        // GET: /Touchpoint/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Touchpoint/Create

        [HttpPost]
        public ActionResult Create(touchpoint touchpoint)
        {
            if (ModelState.IsValid)
            {
                db.touchpoint.Add(touchpoint);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(touchpoint);
        }

        //
        // GET: /Touchpoint/Edit/5

        public ActionResult Edit(int id = 0)
        {
            touchpoint touchpoint = db.pr_getTouchpoint(id).FirstOrDefault();
            if (touchpoint == null)
            {
                return HttpNotFound();
            }
            return View(touchpoint);
        }

        //
        // POST: /Touchpoint/Edit/5

        [HttpPost]
        public ActionResult Edit(touchpoint touchpoint)
        {
            if (ModelState.IsValid)
            {
                db.Entry(touchpoint).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(touchpoint);
        }

        //
        // GET: /Touchpoint/Delete/5

        public ActionResult Delete(int id = 0)
        {
            touchpoint touchpoint = db.pr_getTouchpoint(id).FirstOrDefault();
            if (touchpoint == null)
            {
                return HttpNotFound();
            }
            return View(touchpoint);
        }

        //
        // POST: /Touchpoint/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            touchpoint touchpoint = db.pr_getTouchpoint(id).FirstOrDefault();
            db.touchpoint.Remove(touchpoint);
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