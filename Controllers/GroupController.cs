using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml.Serialization;
using Generic.ViewModel;

namespace Generic.Controllers
{
    public class GroupController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Group/

        public ActionResult Index()
        {
            //var groups = db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID);
            //return View(groups.ToList());
            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            Session["groupsearch"] = arguments;
            return RedirectToAction("FindGroupResult");


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
        public ActionResult ExportExcel()
        {


            string arguments = Session["groupsearch"].ToString() + "active=1;";
            Session["group"] = db.Database.SqlQuery<view_GroupData>("EXEC pr_dynamicFiltersGroup  'view_GroupData' , '" + arguments + "'").ToList();
            List<view_GroupData> abc = (List<view_GroupData>)Session["partner"];
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<view_GroupData>));

            List<ExcelEventNotification> objEvents = new List<ExcelEventNotification>();

            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, abc);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "GroupList.xls");


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
            //if (ModelState.IsValid)
            if (group.name != "" && group.description != "")
            {
                group.description = string.IsNullOrEmpty(group.description) ? group.name : group.description;
                group.active = 1;
                group.sortOrder = 1;
                group.dateCreated = DateTime.Now;
                group.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;

                group.author = SessionSingleton.LoggedInUserId;
                group.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                db.group.Add(group);
                db.SaveChanges();
                ViewBag.ID = group.id;
                ViewBag.GroupName = group.description;
                //   db.pr_addGroup(group.enterprise, 0, group.author, 0, group.name, group.description, "", group.dateCreated, group.sortOrder, group.active);
                //return RedirectToAction("Create", "Group");
                ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name");
                return View(group);
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", group.enterprise);
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name", group.groupCollection);
            return View(group);
        }

        public ActionResult Archive(int id)
        {
            db.pr_archiveGroup(id);

            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public ActionResult ArchiveGroups(string[] id)
        {
            var result = false;
            try
            {
                foreach (var strId in id)
                    db.pr_archiveGroup(int.Parse(strId));
                result = true;
            }
            catch { }

            return Json(new { success = result }, JsonRequestBehavior.AllowGet);
            //return result;
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

        public ActionResult GetGroupByTouchpointid(int touchpointId)
        {
            if (touchpointId == 0)
            {

                var group = (from x in db.view_GroupData
                             select new { x.Enterprise, x.name, x.description, x.Owner }).ToList();
                return Json(new { data = group }, JsonRequestBehavior.AllowGet);

            }
            else
            {
                var group = (from x in db.view_GroupData
                             select new { x.description }).ToList();

                return Json(new { Data = group }, JsonRequestBehavior.AllowGet);
            }
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

        public ActionResult GetGroupsByTouchpoint(int touchpointId)
        {
            if (touchpointId == 0)
            {
                var group = db.pr_getGroupByTouchpoint(touchpointId).ToList();
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
        public ActionResult FindGroup(string searchType)
        {
            //ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");
            //return View();
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");

            ViewBag.group = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnerStatus = new SelectList(db.pr_getPartnerStatusAll(), "id", "description");

            ViewBag.searchType = searchType;

            return View();

        }


        [HttpPost]
        public ActionResult FindGroup(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode, string searchType)
        {
            //dbo.pr_dynamicFilters 'partner', ' Campaign=1009; Group=20;Country=2; Type=4'
            //var objPartners = db.pr_dynamicFiltersPartner("view_PartnerData", "name=well;enterprise=3");

            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            if (touchpoint != null)
                arguments += "touchpointID=" + touchpoint + ";";
            if (group != null)
                arguments += "groupID=" + group + ";";
            if (country != null)
                arguments += "countryID=" + country + ";";
            if (partnertype != null)
                arguments += "partnertypeID=" + partnertype + ";";

            if (partnerStatus != null)
                arguments += "StatusID=" + partnerStatus + ";";


            //if (txtInternalIdFind != "")
            //    arguments += "InternalId=" + txtInternalIdFind + ";";


            ////string , string , string , string , string , string )
            if (txtDunsNumberFind != "")
                arguments += "DunsNumber=" + txtDunsNumberFind + ";";
            if (txtNameFind != "")
                arguments += "Name=" + txtNameFind + ";";
            if (txtFederalIdFind != "")
                arguments += "FederalId=" + txtFederalIdFind + ";";

            if (accesscode != "")
                arguments += "accesscode=" + accesscode + ";";

            if (txtContactEmailFind != "")
                arguments += "ContactEmail=" + txtContactEmailFind + ";";
            if (txtHROEmailFind != "")
                arguments += "HROEmail=" + txtHROEmailFind + ";";
            if (txtScoreFromFind != "")
                arguments += "ScoreFrom=" + txtScoreFromFind + ";";
            if (txtScoreToFind != "")
                arguments += "ScoreTo=" + txtScoreToFind + ";";
            if (txtAddedFromFind != "")
                arguments += "AddedFrom=" + txtAddedFromFind + ";";
            if (txtAddedToFind != "")
                arguments += "AddedTo=" + txtAddedToFind + ";";
            if (txtFullTextSearch != "")
                arguments += "FullTextSearch=" + txtFullTextSearch + ";";
            //var objPartners2 =   db.Database.ExecuteSqlCommand("Yourprocedure @param, @param1", param1, param2);

            //var objPartners = db.Database.SqlQuery<view_GroupData>("EXEC pr_dynamicFiltersGroup  'view_GroupData' , '" + arguments + "'").ToList();

            //Session["group"] = objPartners;
            //TempData["group"] = objPartners;
            //return RedirectToAction("FindGroupResult", objPartners);



            Session["groupsearch"] = arguments;
            if (searchType == "Remove")
            {
                return RedirectToAction("RemoveGroup");
            }
            else if (searchType == "Archive")
            {
                return RedirectToAction("ArchiveGroup");
            }
            else if (searchType == "Restore")
            {
                return RedirectToAction("RestoreGroup");
            }
            else
            {
                return RedirectToAction("FindGroupResult");
            }
        }
        public ActionResult ArchiveGroup()
        {
            string arguments = Session["groupsearch"].ToString() + "active=1;";
            List<view_GroupData> objGroupDataList = db.Database.SqlQuery<view_GroupData>("EXEC pr_dynamicFiltersGroup  'view_GroupData' , '" + arguments + "'").ToList();
            List<GroupViewModel> objGroupViewModelList = ConvertToGroupViewModel(objGroupDataList);
            ViewBag.searchType = "Archive";
            return View("RemoveGroup", objGroupViewModelList);
        }

        public ActionResult RemoveGroup()
        {
            string arguments = Session["groupsearch"].ToString() + "active=1;";
            List<view_GroupData> objGroupDataList = db.Database.SqlQuery<view_GroupData>("EXEC pr_dynamicFiltersGroup  'view_GroupData' , '" + arguments + "'").ToList();
            List<GroupViewModel> objGroupViewModelList = ConvertToGroupViewModel(objGroupDataList);
            ViewBag.searchType = "Remove";
            return View("RemoveGroup", objGroupViewModelList);

        }

        [HttpPost]
        public ActionResult RemoveGroup(string searchType, List<int> chkSelect)
        {
            if (searchType == "Remove")
            {
                foreach (int groupID in chkSelect)
                {
                    db.pr_removeGroup(groupID);
                }

                ViewBag.searchType = "Remove";
                return RedirectToAction("RemoveGroup");
            }
            else if (searchType == "Archive")
            {
                foreach (int groupID in chkSelect)
                {
                    db.pr_archiveGroup(groupID);
                }
                ViewBag.searchType = "Archive";
                return RedirectToAction("ArchiveGroup");
            }
            else if (searchType == "Restore")
            {
                foreach (int groupID in chkSelect)
                {
                    db.pr_unArchiveGroup(groupID);
                }
                ViewBag.searchType = "Restore";
                return RedirectToAction("RestoreGroup");
            }
            else
            {
                return RedirectToAction("FindGroup");
            }
        }



        public ActionResult RestoreGroup()
        {
            string arguments = Session["groupsearch"].ToString() + "active=0;";

            List<view_GroupData> objGroupDataList = db.Database.SqlQuery<view_GroupData>("EXEC pr_dynamicFiltersGroup  'view_GroupData' , '" + arguments + "'").ToList();
            List<GroupViewModel> objGroupViewModelList = ConvertToGroupViewModel(objGroupDataList);
            ViewBag.searchType = "Restore";
            return View("RemoveGroup", objGroupViewModelList);

        }

        private List<GroupViewModel> ConvertToGroupViewModel(List<view_GroupData> iview_GroupDataList)
        {
            List<GroupViewModel> objGroupViewModelList = new List<GroupViewModel>();

            foreach (var iview_GroupDataData in iview_GroupDataList)
            {
                GroupViewModel objGroupViewModel = new GroupViewModel();
                objGroupViewModel.Enterprise = iview_GroupDataData.Enterprise;
                objGroupViewModel.id = iview_GroupDataData.id;
                objGroupViewModel.name = iview_GroupDataData.name;
                objGroupViewModel.CampaignCount = iview_GroupDataData.CampaignCount;
                objGroupViewModel.description = iview_GroupDataData.description;

                objGroupViewModel.Owner = iview_GroupDataData.Owner;
                objGroupViewModel.PartnerCount = iview_GroupDataData.PartnerCount;
                objGroupViewModel.ShadowCount = iview_GroupDataData.ShadowCount;
                objGroupViewModel.UserCount = iview_GroupDataData.UserCount;

                objGroupViewModel.IsSelected = false;



                objGroupViewModelList.Add(objGroupViewModel);
            }
            return objGroupViewModelList;

        }


        public ActionResult FindGroupResult()
        {
            try
            {
                //List<view_GroupData> abc = (List<view_GroupData>)Session["group"];
                //return View(abc);
                string arguments = Session["groupsearch"].ToString() + "active=1;";
                Session["group"] = db.Database.SqlQuery<view_GroupData>("EXEC pr_dynamicFiltersGroup  'view_GroupData' , '" + arguments + "'").ToList();
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