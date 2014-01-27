using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class AutoMailMessagesController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /AutoMailMessages/

        public ActionResult Index()
        {
            var automailmessage = db.autoMailMessage.Include(a => a.autoMailType).Include(a => a.partnerTypeTouchpointQuestionnaire1);
            return View(automailmessage.ToList());
        }

        //
        // GET: /AutoMailMessages/Details/5

        public ActionResult Details(int id = 0)
        {
            autoMailMessage automailmessage = db.autoMailMessage.Find(id);
            if (automailmessage == null)
            {
                return HttpNotFound();
            }
            return View(automailmessage);
        }

        //
        // GET: /AutoMailMessages/Create

        public ActionResult Create()
        {
            ViewBag.mailType = new SelectList(db.autoMailType, "id", "description");
            ViewBag.partnerTypeTouchpointQuestionnaire = new SelectList(db.partnerTypeTouchpointQuestionnaire, "id", "id");
            return View();
        }

        //
        // POST: /AutoMailMessages/Create

        [HttpPost]
        public ActionResult Create(autoMailMessage automailmessage)
        {
            if (ModelState.IsValid)
            {
                db.autoMailMessage.Add(automailmessage);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.mailType = new SelectList(db.autoMailType, "id", "description", automailmessage.mailType);
            ViewBag.partnerTypeTouchpointQuestionnaire = new SelectList(db.partnerTypeTouchpointQuestionnaire, "id", "id", automailmessage.partnerTypeTouchpointQuestionnaire);
            return View(automailmessage);
        }

        //
        // GET: /AutoMailMessages/Edit/5

        public ActionResult Edit(int id = 0)
        {
            autoMailMessage automailmessage = db.autoMailMessage.Find(id);
            if (automailmessage == null)
            {
                return HttpNotFound();
            }
            ViewBag.mailType = new SelectList(db.autoMailType, "id", "description", automailmessage.mailType);
            ViewBag.partnerTypeTouchpointQuestionnaire = new SelectList(db.partnerTypeTouchpointQuestionnaire, "id", "id", automailmessage.partnerTypeTouchpointQuestionnaire);
            return View(automailmessage);
        }

        //
        // POST: /AutoMailMessages/Edit/5

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit(autoMailMessage automailmessage)
        {
            if (ModelState.IsValid)
            {
                db.Entry(automailmessage).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.mailType = new SelectList(db.autoMailType, "id", "description", automailmessage.mailType);
            ViewBag.partnerTypeTouchpointQuestionnaire = new SelectList(db.partnerTypeTouchpointQuestionnaire, "id", "id", automailmessage.partnerTypeTouchpointQuestionnaire);
            return View(automailmessage);
        }

        //
        // GET: /AutoMailMessages/Delete/5

        public ActionResult Delete(int id = 0)
        {
            autoMailMessage automailmessage = db.autoMailMessage.Find(id);
            if (automailmessage == null)
            {
                return HttpNotFound();
            }
            return View(automailmessage);
        }

        //
        // POST: /AutoMailMessages/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            autoMailMessage automailmessage = db.autoMailMessage.Find(id);
            db.autoMailMessage.Remove(automailmessage);
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