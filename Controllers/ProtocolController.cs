using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Helpers;
using System.IO;
using System.Xml.Serialization;
using Generic.ViewModel;

namespace Generic.Controllers
{
    public class ProtocolController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Protocol/

        public ActionResult Index()
        {
            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            Session["protocolsearch"] = arguments;
            return RedirectToAction("FindProtocolResult");

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
            /*ViewBag.agency = new SelectList(db.pr_getAgencyAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");*/
            /*ViewBag.agency = new SelectList(db.pr_getAgencyAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");*/
            ViewBag.agency = new SelectList(db.pr_getAgencyAll(), "id", "description");
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
                protocol.description = string.IsNullOrEmpty(protocol.description) ? protocol.name : protocol.description;
                protocol.active = 1;

                protocol.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
                protocol.sponsor = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id;
                protocol.admin = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id;
                db.protocol.Add(protocol);
                db.SaveChanges();
                SessionSingleton.ProtocolId = protocol.id;
                if (protocol.id > 0)
                {
                    ViewBag.ID = protocol.id;
                    ViewBag.Name = protocol.name;
                    ViewBag.EndDate = protocol.endDate.HasValue ? protocol.endDate.Value.ToString("MM/dd/yyyy") : null;
                    /*ViewBag.agency = new SelectList(db.pr_getAgencyAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description", protocol.agency);*/
                    ViewBag.agency = new SelectList(db.pr_getAgencyAll(), "id", "description", protocol.agency);
                    ViewBag.domain = new SelectList(db.pr_getDomainAll(), "id", "description", protocol.domain);
                    return View(protocol);
                }
                //return RedirectToAction("Create", "Touchpoint");
            }

            ViewBag.agency = new SelectList(db.pr_getAgencyAll(), "id", "description", protocol.agency);
            ViewBag.domain = new SelectList(db.pr_getDomainAll(), "id", "description", protocol.domain);
            return View(protocol);
        }

        public ActionResult Archive(int id)
        {
            db.pr_archiveProtocol(id);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
        public ActionResult ExportExcel()
        {


            string arguments = Session["protocolsearch"].ToString() + "active=1;";
            Session["protocol"] = db.Database.SqlQuery<view_ProtocolData>("EXEC pr_dynamicFiltersProtocol  'view_ProtocolData' , '" + arguments + "'").ToList();
            List<view_ProtocolData> abc = (List<view_ProtocolData>)Session["protocol"];

            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<view_ProtocolData>));

            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, abc);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "ProtocolList.xls");


        }


        public ActionResult ArchiveProtocol()
        {
            string arguments = Session["protocolsearch"].ToString() + "active=1";

            List<view_ProtocolData> objProtocolDataList = db.Database.SqlQuery<view_ProtocolData>("EXEC pr_dynamicFiltersProtocol  'view_ProtocolData' , '" + arguments + "'").ToList();
            List<ProtocolViewModel> objProtocolViewModelList = ConvertToProtocolViewModel(objProtocolDataList);
            ViewBag.searchType = "Archive";
            return View("RemoveProtocol", objProtocolViewModelList);

        }

        public ActionResult RemoveProtocol()
        {
            string arguments = Session["protocolsearch"].ToString() + "active=1";

            List<view_ProtocolData> objProtocolDataList = db.Database.SqlQuery<view_ProtocolData>("EXEC pr_dynamicFiltersProtocol  'view_ProtocolData' , '" + arguments + "'").ToList();
            List<ProtocolViewModel> objProtocolViewModelList = ConvertToProtocolViewModel(objProtocolDataList);
            ViewBag.searchType = "Remove";
            return View("RemoveProtocol", objProtocolViewModelList);

        }
        [HttpPost]
        public ActionResult RemoveProtocol(string searchType, List<int> chkSelect)
        {
            if (searchType == "Remove")
            {
                foreach (int ID in chkSelect)
                {
                    db.pr_removeProtocol(ID);
                }

                ViewBag.searchType = "Remove";
                return RedirectToAction("RemoveProtocol");
            }
            else if (searchType == "Archive")
            {
                foreach (int ID in chkSelect)
                {
                    db.pr_archiveProtocol(ID);
                }
                ViewBag.searchType = "Archive";
                return RedirectToAction("ArchiveProtocol");
            }
            else if (searchType == "Restore")
            {
                foreach (int ID in chkSelect)
                {
                    db.pr_unArchiveProtocol(ID);
                }
                ViewBag.searchType = "Restore";
                return RedirectToAction("RestoreProtocol");
            }
            else
            {
                return RedirectToAction("FindProtocol");
            }
        }
        public ActionResult RestoreProtocol()
        {
            string arguments = Session["protocolsearch"].ToString() + "active=0";

            List<view_ProtocolData> objProtocolDataList = db.Database.SqlQuery<view_ProtocolData>("EXEC pr_dynamicFiltersProtocol  'view_ProtocolData' , '" + arguments + "'").ToList();
            List<ProtocolViewModel> objProtocolViewModelList = ConvertToProtocolViewModel(objProtocolDataList);
            ViewBag.searchType = "Restore";
            return View("RemoveProtocol", objProtocolViewModelList);

        }


        public ActionResult FindProtocol(string searchType)
        {
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");

            ViewBag.group = new SelectList(db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnerStatus = new SelectList(db.pr_getPartnerStatusAll(), "id", "description");

            ViewBag.searchType = searchType;

            return View();
        }
        [HttpPost]
        public ActionResult FindProtocol(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode, string searchType)
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

            Session["protocolsearch"] = arguments;
            if (searchType == "Remove")
            {
                return RedirectToAction("RemoveProtocol");
            }
            else if (searchType == "Archive")
            {
                return RedirectToAction("ArchiveProtocol");
            }
            else if (searchType == "Restore")
            {
                return RedirectToAction("RestoreProtocol");
            }
            else
            {
                return RedirectToAction("FindProtocolResult");
            }
        }

        public ActionResult FindProtocolResult()
        {
            try
            {
                string arguments = Session["protocolsearch"].ToString() + "active=1;";
                Session["protocol"] = db.Database.SqlQuery<view_ProtocolData>("EXEC pr_dynamicFiltersProtocol  'view_ProtocolData' , '" + arguments + "'").ToList();
                List<view_ProtocolData> abc = (List<view_ProtocolData>)Session["protocol"];

                return View(abc);
            }
            catch
            {
                return RedirectToAction("FindProtocol");

            }




        }

        private List<ProtocolViewModel> ConvertToProtocolViewModel(List<view_ProtocolData> iview_ProtocolDataList)
        {
            List<ProtocolViewModel> objProtocolViewModelList = new List<ProtocolViewModel>();

            foreach (var iview_ProtocolData in iview_ProtocolDataList)
            {
                ProtocolViewModel objProtocolViewModel = new ProtocolViewModel();
                objProtocolViewModel.id = iview_ProtocolData.id;

                objProtocolViewModel.enterprise = iview_ProtocolData.enterprise;

                objProtocolViewModel.active = iview_ProtocolData.active;
                objProtocolViewModel.Group_Count = iview_ProtocolData.Group_Count;
                objProtocolViewModel.name = iview_ProtocolData.name;
                objProtocolViewModel.Partner_Count = iview_ProtocolData.Partner_Count;
                objProtocolViewModel.Protocol_Admin = iview_ProtocolData.Protocol_Admin;
                objProtocolViewModel.Protocol_Sponsor = iview_ProtocolData.Protocol_Sponsor;
                objProtocolViewModel.User_Count = iview_ProtocolData.User_Count;

                objProtocolViewModel.IsSelected = false;

                objProtocolViewModelList.Add(objProtocolViewModel);
            }
            return objProtocolViewModelList;

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}