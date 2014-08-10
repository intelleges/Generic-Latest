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
        [HttpPost]
        public ActionResult FindProtocol(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode)
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

            var objPartners = db.Database.SqlQuery<view_ProtocolData>("EXEC pr_dynamicFiltersProtocol  'view_ProtocolData' , '" + arguments + "'").ToList();

            Session["protocol"] = objPartners;
            TempData["protocol"] = objPartners;
            return RedirectToAction("FindProtocolResult", objPartners);
        }

        public ActionResult FindProtocolResult()
        {
            try
            {
                List<view_ProtocolData> abc = (List<view_ProtocolData>)Session["protocol"];
                return View(abc);
            }
            catch
            {
                return RedirectToAction("FindProtocol");

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