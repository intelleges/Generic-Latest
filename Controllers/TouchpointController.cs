using Generic.Models;
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
    public class TouchpointController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Touchpoint/

        public ActionResult Index()
        {
            //var touchPoint = db.pr_getTouchpointAll();
            //return View(touchPoint.ToList());

            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            Session["touchpointsearch"] = arguments;
            return RedirectToAction("FindTouchpointResult");
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
                touchpoint.active = 1;
                db.touchpoint.Add(touchpoint);

                db.SaveChanges();
                if (touchpoint.id > 0)
                {
                    ViewBag.ID = touchpoint.id;
                    ViewBag.Touchpoint = touchpoint.title;
                    ViewBag.EndDate = touchpoint.endDate.Value.ToShortDateString();
                    if (SessionSingleton.ProtocolId != 0)
                    {
                        ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name", SessionSingleton.ProtocolId);
                    }
                    else
                    {
                        ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
                    }
                    ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
                    ViewBag.sponsor = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
                    ViewBag.admin = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
                    ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description");
                    return View(touchpoint);
                    //return RedirectToAction("Index");
                }
            }
            if (SessionSingleton.ProtocolId != 0)
            {
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name", SessionSingleton.ProtocolId);
            }
            else
            {
                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            }
            ViewBag.person = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.sponsor = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.admin = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "firstName");
            ViewBag.target = new SelectList(db.pr_getTouchpointTargetAll(), "id", "description");
            return View(touchpoint);
        }


        public ActionResult Archive(int id)
        {
            db.pr_archiveTouchpoint(id);
            //if (ModelState.IsValid)
            //{
            //    //db.Entry(partner).State = EntityState.Modified;
            //    //db.SaveChanges();
            //    db.pr_modifyPartner(partner.id, partner.enterprise, partner.internalID, partner.name, partner.address1, partner.address2, partner.city, partner.state, partner.province, partner.zipcode, partner.country, partner.phone, partner.fax, partner.firstName, partner.lastName, partner.title, partner.email, partner.dunsNumber, partner.federalID, partner.status, partner.loadHistory, partner.owner, partner.author, partner.dateApproved, partner.active, partner.lastModified);

            //    return Json(new { success = true });
            //    //return RedirectToAction("Index");
            //}
            //ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            //ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
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
                //return RedirectToAction("Index");
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

        public ActionResult ExportExcel()
        {


            string arguments = Session["touchpointsearch"].ToString() + "active=1;";
            Session["touchpoint"] = db.Database.SqlQuery<view_TouchpointData>("EXEC pr_dynamicFiltersTouchpoint  'view_TouchpointData' , '" + arguments + "'").ToList();
            List<view_TouchpointData> abc = (List<view_TouchpointData>)Session["touchpoint"];





            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<view_TouchpointData>));


            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, abc);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "TouchpointList.xls");


        }

        public ActionResult ArchiveTouchpoint()
        {
            string arguments = Session["touchpointsearch"].ToString() + "active=1";

            List<view_TouchpointData> objTouchpointDataList = db.Database.SqlQuery<view_TouchpointData>("EXEC pr_dynamicFiltersTouchpoint  'view_TouchpointData' , '" + arguments + "'").ToList();
            List<TouchPointViewModel> objTouchpointViewModelList = ConvertToTouchpointViewModel(objTouchpointDataList);
            ViewBag.searchType = "Archive";
            return View("RemoveTouchpoint", objTouchpointViewModelList);

        }


        public ActionResult RemoveTouchpoint()
        {
            string arguments = Session["Touchpointsearch"].ToString() + "active=1;";

            List<view_TouchpointData> objTouchpointDataList = db.Database.SqlQuery<view_TouchpointData>("EXEC pr_dynamicFiltersTouchpoint  'view_TouchpointData' , '" + arguments + "'").ToList();
            List<TouchPointViewModel> objTouchpointViewModelList = ConvertToTouchpointViewModel(objTouchpointDataList);
            ViewBag.searchType = "Remove";
            return View("RemoveTouchpoint", objTouchpointViewModelList);

        }
        [HttpPost]
        public ActionResult RemoveTouchpoint(string searchType, List<int> chkSelect)
        {
            if (searchType == "Remove")
            {
                foreach (int touchpointID in chkSelect)
                {
                    db.pr_removeTouchpoint(touchpointID);
                }

                ViewBag.searchType = "Remove";
                return RedirectToAction("RemoveTouchpoint");
            }
            else if (searchType == "Archive")
            {
                foreach (int touchpointID in chkSelect)
                {
                    db.pr_archivePartner(touchpointID);
                }
                ViewBag.searchType = "Archive";
                return RedirectToAction("ArchiveTouchpoint");
            }
            else if (searchType == "Restore")
            {
                foreach (int touchpointID in chkSelect)
                {
                    db.pr_unArchiveTouchpoint(touchpointID);
                }
                ViewBag.searchType = "Restore";
                return RedirectToAction("RestoreTouchpoint");
            }
            else
            {
                return RedirectToAction("FindTouchpoint");
            }
        }
        public ActionResult RestoreTouchpoint()
        {
            string arguments = Session["touchpointsearch"].ToString() + "active=0;";

            List<view_TouchpointData> objTouchpointDataList = db.Database.SqlQuery<view_TouchpointData>("EXEC pr_dynamicFiltersTouchpoint  'view_TouchpointData' , '" + arguments + "'").ToList();
            List<TouchPointViewModel> objTouchpointViewModelList = ConvertToTouchpointViewModel(objTouchpointDataList);
            ViewBag.searchType = "Restore";
            return View("RemoveTouchpoint", objTouchpointViewModelList);

        }

        public ActionResult GetTouchPointByprotocolId(int protocolId)
        {
            var touchpoint = db.pr_getTouchpointByProtocol(protocolId).Where(x => x.active == 1).Select(x => new { x.id, x.description }).ToList();
            return Json(new { Data = touchpoint }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult FindTouchPoint(string searchType)
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
        public ActionResult FindTouchPoint(int? touchpoint, int? group, int? country, int? partnertype, int? partnerStatus, string txtInternalIdFind, string txtDunsNumberFind, string txtNameFind, string txtFederalIdFind, string txtContactEmailFind, string txtHROEmailFind, string txtZipCodeFind, string txtScoreFromFind, string txtScoreToFind, string txtAddedFromFind, string txtAddedToFind, string txtFullTextSearch, string accesscode, string searchType)
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

            //var objPartners = db.Database.SqlQuery<view_TouchpointData>("EXEC pr_dynamicFiltersTouchpoint  'view_TouchpointData' , '" + arguments + "'").ToList();

            //Session["touchpoint"] = objPartners;
            //TempData["touchpoint"] = objPartners;
            //return RedirectToAction("FindTouchPointResult", objPartners);
            Session["touchpointsearch"] = arguments;
            if (searchType == "Remove")
            {
                return RedirectToAction("RemoveTouchpoint");
            }
            else if (searchType == "Archive")
            {
                return RedirectToAction("ArchiveTouchpoint");
            }
            else if (searchType == "Restore")
            {
                return RedirectToAction("RestoreTouchpoint");
            }
            else
            {
                return RedirectToAction("FindTouchpointResult");
            }
        }

        public ActionResult FindTouchPointResult()
        {
            try
            {
                string arguments = Session["touchpointsearch"].ToString() + "active=1;";
                Session["touchpoint"] = db.Database.SqlQuery<view_TouchpointData>("EXEC pr_dynamicFiltersTouchpoint  'view_TouchpointData' , '" + arguments + "'").ToList();


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

        private List<TouchPointViewModel> ConvertToTouchpointViewModel(List<view_TouchpointData> iview_TouchpointDataList)
        {
            List<TouchPointViewModel> objTouchpointViewModelList = new List<TouchPointViewModel>();

            foreach (var iview_TouchpointData in iview_TouchpointDataList)
            {
                TouchPointViewModel objTouchpointViewModel = new TouchPointViewModel();
                objTouchpointViewModel.id = iview_TouchpointData.id;
                objTouchpointViewModel.Name = iview_TouchpointData.Name;
                objTouchpointViewModel.enterprise = iview_TouchpointData.enterprise;
                objTouchpointViewModel.description = iview_TouchpointData.description;
                objTouchpointViewModel.End_Date = iview_TouchpointData.End_Date;
                objTouchpointViewModel.Partner_Count = iview_TouchpointData.Partner_Count;
                objTouchpointViewModel.PartnerType_Count = iview_TouchpointData.PartnerType_Count;
                objTouchpointViewModel.Questionnaire_Count = iview_TouchpointData.Questionnaire_Count;
                objTouchpointViewModel.Start_Date = iview_TouchpointData.Start_Date;
                objTouchpointViewModel.Touchpoint_Admin = iview_TouchpointData.Touchpoint_Admin;
                objTouchpointViewModel.Touchpoint_Sponsor = iview_TouchpointData.Touchpoint_Sponsor;
                objTouchpointViewModel.User_Count = iview_TouchpointData.User_Count;

                objTouchpointViewModel.IsSelected = false;

                objTouchpointViewModelList.Add(objTouchpointViewModel);
            }
            return objTouchpointViewModelList;

        }
        public ActionResult EventNotification(int id = 0)
        {
            //string arguments = Session["touchpointsearch"].ToString() + "touchpointid=" + id + ";";
            //List<question> questionnairedetail = db.pr_dynamicFiltersEventNotification("view_EventNotificationData", arguments).ToList();
            //return View(questionnairedetail);

            string arguments = Session["touchpointsearch"].ToString() + "touchpointid=" + id + ";";
            Session["touchpoint"] = db.Database.SqlQuery<view_EventNotificationData>("EXEC pr_dynamicFiltersEventNotification  'view_EventNotificationData' , '" + arguments + "'").ToList();


            List<view_EventNotificationData> abc = (List<view_EventNotificationData>)Session["touchpoint"];

            return View(abc);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}