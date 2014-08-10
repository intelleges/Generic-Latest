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
    public class GroupController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Group/

        public ActionResult Index()
        {
            var groups = db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID);
            return View(groups.ToList());
        }

        //
        // GET: /Group/Details/5

        public ActionResult Details(int id = 0)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // GET: /Group/Create

        public ActionResult Create()
        {
            //ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name");
            return View();
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult Create(group group)
        {
            if (ModelState.IsValid)
            {
                group.author = SessionSingleton.LoggedInUserId;
                group.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                db.group.Add(group);
                db.SaveChanges();
                return RedirectToAction("Create","Person");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", group.enterprise);
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name", group.groupCollection);
            return View(group);
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id = 0)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", group.enterprise);
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name", group.groupCollection);
            return View(group);
        }

        //
        // POST: /Group/Edit/5

        [HttpPost]
        public ActionResult Edit(group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", group.enterprise);
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name", group.groupCollection);
            return View(group);
        }

        //
        // GET: /Group/Delete/5

        public ActionResult Delete(int id = 0)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // POST: /Group/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            db.group.Remove(group);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GetGroupByTouchpoint(int touchpointId)
        {
            if (touchpointId == 0)
            {
                var group = db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(x => new { x.id, x.description }).ToList();
                return Json(new { Data = group }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var group = db.pr_getGroupByTouchpoint(touchpointId).Select(x => new { x.id, x.description }).ToList();
                return Json(new { Data = group }, JsonRequestBehavior.AllowGet);
            }
        }
        //public ActionResult GetGroupByTouchpointName(string touchpointId)
        //{
        //    if (touchpointId == 0)
        //    {
        //        var group = db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(x => new { x.id, x.description }).ToList();
        //        return Json(new { Data = group }, JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        var group = db.pr_getGroupByTouchpoint(touchpointId).Select(x => new { x.id, x.description }).ToList();
        //        return Json(new { Data = group }, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult FindGroup()
        {
            ViewBag.Test = "Hi";
            return View();
        }


        [HttpPost]
        public ActionResult FindGroup(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode)
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

            var objPartners = db.Database.SqlQuery<view_GroupData>("EXEC pr_dynamicFiltersGroup  'view_GroupData' , '" + arguments + "'").ToList();

            Session["group"] = objPartners;
            TempData["group"] = objPartners;
            return RedirectToAction("FindGroupResult", objPartners);
        }

        public ActionResult FindGroupResult()
        {
            try
            {
                List<view_GroupData> abc = (List<view_GroupData>)Session["group"];
                return View(abc);
            }
            catch
            {
                return RedirectToAction("FindGroup");

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