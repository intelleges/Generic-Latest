using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Helpers;

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
            ViewBag.agency = new SelectList(db.pr_getAgencyAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            ViewBag.domain = new SelectList(db.pr_getDomainAll(), "id", "description");
            return View();
        }

        //
        // POST: /Protocol/Create

        [HttpPost]
        public ActionResult Create(protocol protocol)
        {
            if (ModelState.IsValid)
            {

                protocol.active = 1;

                protocol.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                protocol.sponsor = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id;
                protocol.admin = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id;
                db.protocol.Add(protocol);
                db.SaveChanges();
                SessionSingleton.ProtocolId = protocol.id;
                return RedirectToAction("Create","Touchpoint");
            }

            ViewBag.agency = new SelectList(db.pr_getAgencyAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description",protocol.agency);
            ViewBag.domain = new SelectList(db.pr_getDomainAll(), "id", "description",protocol.domain);
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

        public ActionResult FindProtocol()
        {
            ViewBag.Test = "Hi";
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}