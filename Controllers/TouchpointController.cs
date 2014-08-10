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
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description", SessionSingleton.ProtocolId);
            }
            else
            {
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            }

            ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.sponsor = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.admin = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description");


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
            ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description");
            return View(touchpoint);
        }

        //
        // GET: /Touchpoint/Edit/5

        public ActionResult Edit(int id = 0)
        {
            touchpoint touchpoint = db.pr_getTouchpoint(id).FirstOrDefault();
            if (touchpoint.target != null)
            {
                ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description", touchpoint.target);
            }
            else
            {
                ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description");
            }

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

            if (touchpoint.target != null)
            {
                ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description", touchpoint.target);
            }
            else
            {
                ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description");
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
            var touchpoint = db.pr_getTouchpointByProtocol(protocolId).Where(x => x.active == 1).Select(x => new { x.id, x.description }).ToList();
            return Json(new { Data = touchpoint }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindTouchPoint()
        {
            ViewBag.Test = "Hi";
            return View();
        }

        [HttpPost]
        public ActionResult FindTouchPoint(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode)
        {
            //dbo.pr_dynamicFilters 'partner', ' Campaign=1009; Group=20;Country=2; Type=4'
            //var objPartners = db.pr_dynamicFiltersPartner("view_PartnerData", "name=well;enterprise=3");

            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            //if (touchpoint != null)
            //    arguments += "touchpointID=" + touchpoint + ";";
            //if (group != null)
            //    arguments += "groupID=" + group + ";";
            //if (country != null)
            //    arguments += "countryID=" + country + ";";
            //if (partnertype != null)
            //    arguments += "partnertypeID=" + partnertype + ";";

            //if (partnerStatus != null)
            //    arguments += "StatusID=" + partnerStatus + ";";


            //if (txtInternalIdFind != "")
            //    arguments += "InternalId=" + txtInternalIdFind + ";";


            ////string , string , string , string , string , string )
            //if (txtDunsNumberFind != "")
            //    arguments += "DunsNumber=" + txtDunsNumberFind + ";";
            //if (txtNameFind != "")
            //    arguments += "Name=" + txtNameFind + ";";
            //if (txtFederalIdFind != "")
            //    arguments += "FederalId=" + txtFederalIdFind + ";";

            //if (accesscode != "")
            //    arguments += "accesscode=" + accesscode + ";";

            //if (txtContactEmailFind != "")
            //    arguments += "ContactEmail=" + txtContactEmailFind + ";";
            //if (txtHROEmailFind != "")
            //    arguments += "HROEmail=" + txtHROEmailFind + ";";
            //if (txtScoreFromFind != "")
            //    arguments += "ScoreFrom=" + txtScoreFromFind + ";";
            //if (txtScoreToFind != "")
            //    arguments += "ScoreTo=" + txtScoreToFind + ";";
            //if (txtAddedFromFind != "")
            //    arguments += "AddedFrom=" + txtAddedFromFind + ";";
            //if (txtAddedToFind != "")
            //    arguments += "AddedTo=" + txtAddedToFind + ";";
            //if (txtFullTextSearch != "")
            //    arguments += "FullTextSearch=" + txtFullTextSearch + ";";
            //var objPartners2 =   db.Database.ExecuteSqlCommand("Yourprocedure @param, @param1", param1, param2);

            var objPartners = db.Database.SqlQuery<view_TouchpointData>("EXEC pr_dynamicFiltersTouchpoint  'view_TouchpointData' , '" + arguments + "'").ToList();

            Session["touchpoint"] = objPartners;
            TempData["touchpoint"] = objPartners;
            return RedirectToAction("FindTouchPointResult", objPartners);
        }

        public ActionResult FindTouchPointResult()
        {
            try
            {
                List<view_TouchpointData> abc = (List<view_TouchpointData>)Session["touchpoint"];
                return View(abc);
            }
            catch
            {
                return RedirectToAction("FindTouchPoint");

            }

            //List<view_PartnerData> abc = (List<view_PartnerData>)TempData["partner"];
            //Session["partner"] 


        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}