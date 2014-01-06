using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class PartnerTypeController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /PartnerType/

        public ActionResult Index()
        {
            var partnertype = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID);
            return View(partnertype.ToList());
        }

        //
        // GET: /PartnerType/Details/5

        public ActionResult Details(int id = 0)
        {
            partnerType partnertype = db.pr_getPartnerType(id).FirstOrDefault();
            if (partnertype == null)
            {
                return HttpNotFound();
            }
            return View(partnertype);
        }

        //
        // GET: /PartnerType/Create

        public ActionResult Create()
        {
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            ViewBag.partnerClass = new SelectList(db.partnerClass, "id", "description");
            return View();
        }

        //
        // POST: /PartnerType/Create

        [HttpPost]
        public ActionResult Create(partnerType partnertype)
        {
            if (ModelState.IsValid)
            {
                db.partnerType.Add(partnertype);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partnertype.enterprise);
            ViewBag.partnerClass = new SelectList(db.partnerClass, "id", "description", partnertype.partnerClass);
            return View(partnertype);
        }

        //
        // GET: /PartnerType/Edit/5

        public ActionResult Edit(int id = 0)
        {
            partnerType partnertype = db.pr_getPartnerType(id).FirstOrDefault();
            if (partnertype == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partnertype.enterprise);
            ViewBag.partnerClass = new SelectList(db.partnerClass, "id", "description", partnertype.partnerClass);
            return View(partnertype);
        }

        //
        // POST: /PartnerType/Edit/5

        [HttpPost]
        public ActionResult Edit(partnerType partnertype)
        {
            if (ModelState.IsValid)
            {
                db.Entry(partnertype).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partnertype.enterprise);
            ViewBag.partnerClass = new SelectList(db.partnerClass, "id", "description", partnertype.partnerClass);
            return View(partnertype);
        }

        //
        // GET: /PartnerType/Delete/5

        public ActionResult Delete(int id = 0)
        {
            partnerType partnertype = db.pr_getPartnerType(id).FirstOrDefault();
            if (partnertype == null)
            {
                return HttpNotFound();
            }
            return View(partnertype);
        }

        //
        // POST: /PartnerType/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            partnerType partnertype = db.pr_getPartnerType(id).FirstOrDefault();
            db.partnerType.Remove(partnertype);
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