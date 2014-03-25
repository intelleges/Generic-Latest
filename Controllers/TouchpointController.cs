using Generic.Models;
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
    public class TouchpointController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Touchpoint/

        public ActionResult Index()
        {
            var touchPoint = db.pr_getTouchpointAll();
            return View(touchPoint.ToList());

          
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

        public ActionResult PersonCombobox(PersonComboModel model)
        {
            model.AutoCompleteAttributes.Width = model.AutoCompleteAttributes.Width ?? 200;
            model.AutoCompleteAttributes.HighlightFirst = model.AutoCompleteAttributes.HighlightFirst ?? true;
            model.AutoCompleteAttributes.AutoFill = model.AutoCompleteAttributes.AutoFill ?? false;
            model.AutoCompleteAttributes.AllowMultipleValues = model.AutoCompleteAttributes.AllowMultipleValues ?? true;
            model.AutoCompleteAttributes.MultipleSeparator = model.AutoCompleteAttributes.MultipleSeparator ?? ", ";
            model.ComboBoxAttributes.Width = model.ComboBoxAttributes.Width ?? 200;
            model.ComboBoxAttributes.SelectedIndex = model.ComboBoxAttributes.SelectedIndex ?? 0;
            model.ComboBoxAttributes.HighlightFirst = model.ComboBoxAttributes.HighlightFirst ?? true;
            model.ComboBoxAttributes.AutoFill = model.ComboBoxAttributes.AutoFill ?? true;
            model.ComboBoxAttributes.OpenOnFocus = model.ComboBoxAttributes.OpenOnFocus ?? false;
            model.DropDownListAttributes.Width = model.DropDownListAttributes.Width ?? 200;
            model.DropDownListAttributes.SelectedIndex = model.DropDownListAttributes.SelectedIndex ?? 0;

            model.Persons = db.pr_getPersonByEnterprise1(SessionSingleton.EnterPriseId).ToList();
            return PartialView("_PersonPartial", model);

        }

        //
        // GET: /Touchpoint/Create

        public ActionResult Create()
        {
            if (SessionSingleton.ProtocolId != 0)
            {
                ViewBag.protocol = new SelectList( db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description", SessionSingleton.ProtocolId);
            }
            else
            {
                ViewBag.protocol = new SelectList( db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            }

            ViewBag.person= new SelectList( db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.sponsor = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.admin = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");

            return View();
        }

        //
        // POST: /Touchpoint/Create

        [HttpPost]
        public ActionResult Create(touchpoint touchpoint)
        {
            if (ModelState.IsValid)
            {
                
            //    touchpoint.protocol = ;
                db.touchpoint.Add(touchpoint);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            if (SessionSingleton.ProtocolId != 0)
            {
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description", SessionSingleton.ProtocolId);
            }
            else
            {
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            }
            ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.sponsor = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.admin = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
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


        public ActionResult GetTouchPointByprotocolId(int protocolId)
        {
            var touchpoint = db.pr_getTouchpointByProtocol(protocolId).Where(x=>x.active==1).Select(x => new { x.id, x.description}).ToList();
            return Json(new { Data = touchpoint }, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}