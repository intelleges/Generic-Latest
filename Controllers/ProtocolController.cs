using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class ProtocolController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Protocol/

        public ActionResult Index()
        {
            var protocols = db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID);
            return View(protocols.ToList());
            
        }

        //
        // GET: /Protocol/Details/5

        public ActionResult Details(int id = 0)
        {
            protocol protocol = db.pr_getProtocol(id).FirstOrDefault();
            if (protocol == null)
            {
                return HttpNotFound();
            }
            return View(protocol);
        }

        //
        // GET: /Protocol/Create

        public ActionResult Create()
        {
            //ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            //ViewBag.admin = new SelectList(db.person, "id", "internalId");
            //ViewBag.sponsor = new SelectList(db.person, "id", "internalId");
            return View();
        }

        //
        // POST: /Protocol/Create

        [HttpPost]
        public ActionResult Create(protocol protocol)
        {
            if (ModelState.IsValid)
            {
                protocol.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                protocol.sponsor = SessionSingleton.PersonId;
                protocol.admin = SessionSingleton.PersonId;
                db.protocol.Add(protocol);
                db.SaveChanges();
                SessionSingleton.ProtocolId = protocol.id;
                return RedirectToAction("Create","Touchpoint");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", protocol.enterprise);
            ViewBag.admin = new SelectList(db.person, "id", "internalId", protocol.admin);
            ViewBag.sponsor = new SelectList(db.person, "id", "internalId", protocol.sponsor);
            return View(protocol);
        }

        //
        // GET: /Protocol/Edit/5

        public ActionResult Edit(int id = 0)
        {
            protocol protocol = db.pr_getProtocol(id).FirstOrDefault();
            if (protocol == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", protocol.enterprise);
            ViewBag.admin = new SelectList(db.person, "id", "internalId", protocol.admin);
            ViewBag.sponsor = new SelectList(db.person, "id", "internalId", protocol.sponsor);
            return View(protocol);
        }

        //
        // POST: /Protocol/Edit/5

        [HttpPost]
        public ActionResult Edit(protocol protocol)
        {
            if (ModelState.IsValid)
            {
                db.Entry(protocol).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", protocol.enterprise);
            ViewBag.admin = new SelectList(db.person, "id", "internalId", protocol.admin);
            ViewBag.sponsor = new SelectList(db.person, "id", "internalId", protocol.sponsor);
            return View(protocol);
        }

        //
        // GET: /Protocol/Delete/5

        public ActionResult Delete(int id = 0)
        {
            protocol protocol = db.pr_getProtocol(id).FirstOrDefault();
            if (protocol == null)
            {
                return HttpNotFound();
            }
            return View(protocol);
        }

        //
        // POST: /Protocol/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            protocol protocol = db.pr_getProtocol(id).FirstOrDefault();
            db.protocol.Remove(protocol);
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