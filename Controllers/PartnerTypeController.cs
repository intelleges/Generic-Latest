using Generic.Models;
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

        public ActionResult Create(int? ptypeId)
        {
            if (ptypeId.HasValue)
            {
                var ptype = db.pr_getPartnerType(ptypeId).FirstOrDefault();
                ViewBag.name = ptype.name;
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            ViewBag.partnerClass = new SelectList(db.partnerClass, "id", "description");
            return View();
        }

        //
        // POST: /PartnerType/Create

        [HttpPost]
        public ActionResult Create(CreatePartnerTypeModel ptype)
        {
            if (ModelState.IsValid)
            {               
                var id = db.pr_addPartnerType(ptype.Name, ptype.Alias, ptype.Description, null, Generic.Helpers.CurrentInstance.EnterpriseID, 1, 1).FirstOrDefault();                              
                return RedirectToAction("Create", new { ptypeId = id });
            }
           
            return View(ptype);
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

        public ActionResult GetPartnerTypeByTouchpoint(int touchpointId)
        {
            if (touchpointId == 0)
            {
                var partnerType = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(x => new { x.id, x.description }).ToList();
                return Json(new { Data = partnerType }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var partnerType = db.pr_getPartnertypeByTouchpoint2(touchpointId).Select(x => new { x.id, x.description }).ToList();
                return Json(new { Data = partnerType }, JsonRequestBehavior.AllowGet);
            }
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}