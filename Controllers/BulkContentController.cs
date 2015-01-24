using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class BulkContentController : Controller
    {
        //
        // GET: /BulkContent/

        public ActionResult Create()
        {
            LoadDropDown();           
            return View();
        }

        protected void LoadDropDown()
        {
            using (var db = new EntitiesDBContext())
            {
                ViewBag.Industry = new SelectList(db.pr_getIndustryAll().ToList(), "id", "description");
                ViewBag.Focus = new SelectList(new List<pr_getFocusByIndustry_Result>());
                ViewBag.PartnerTypeList = new SelectList(db.partnerType.ToList().Distinct(new PartnerTypeComparer()), "id", "name");
                ViewBag.TouchPointList = new SelectList(db.pr_getTouchpointAll().ToList().Distinct(new TouchpointComparer()), "id", "description");
            }
        }
        [HttpPost]
        public ActionResult Create(Generic.bulkContent model, HttpPostedFileBase questionnaireFile, HttpPostedFileBase questionnaireCMSFile, HttpPostedFileBase automailMessageInviteFile, HttpPostedFileBase automailMessageCompleteConfirmation, HttpPostedFileBase automailMessageIncompleteReminder, HttpPostedFileBase automailMessageFirstReminder, HttpPostedFileBase automailMessageSecondReminder, HttpPostedFileBase automailMessageThirdReminder, HttpPostedFileBase automailMessageOverdueReminder)
        {
            try
            {
                LoadDropDown();
                using (var db = new EntitiesDBContext())
                {
                    var partnerType = db.pr_getPartnerType(model.PartnerTypeList).FirstOrDefault();
                    var TouchPointList = db.pr_getTouchpoint(model.TouchPointList).FirstOrDefault();
                    var Focus = db.focus.FirstOrDefault(o => o.id == model.Focus);
                    var Industry = db.industry.FirstOrDefault(o => o.id == model.Industry);
                    var questionnairestream = new MemoryStream();
                    var questionnaireCMSstream = new MemoryStream();
                    var automailMessageInvitestream = new MemoryStream();
                    var automailMessageCompleteConfirmationstream = new MemoryStream();
                    var automailMessageIncompleteReminderstream = new MemoryStream();
                    var automailMessageFirstReminderstream = new MemoryStream();
                    var automailMessageSecondReminderstream = new MemoryStream();
                    var automailMessageThirdReminderstream = new MemoryStream();
                    var automailMessageOverdueReminderstream = new MemoryStream();
                    questionnaireFile.InputStream.CopyTo(questionnairestream);
                    questionnaireCMSFile.InputStream.CopyTo(questionnaireCMSstream);
                    automailMessageInviteFile.InputStream.CopyTo(automailMessageInvitestream);
                    automailMessageCompleteConfirmation.InputStream.CopyTo(automailMessageCompleteConfirmationstream);
                    automailMessageIncompleteReminder.InputStream.CopyTo(automailMessageIncompleteReminderstream);
                    automailMessageFirstReminder.InputStream.CopyTo(automailMessageFirstReminderstream);
                    automailMessageSecondReminder.InputStream.CopyTo(automailMessageSecondReminderstream);
                    automailMessageThirdReminder.InputStream.CopyTo(automailMessageThirdReminderstream);
                    automailMessageOverdueReminder.InputStream.CopyTo(automailMessageOverdueReminderstream);
                    db.Database.CommandTimeout = 180;
                    var result = db.pr_addBulkContent(partnerType.name, TouchPointList.description, questionnairestream.ToArray(), questionnaireCMSstream.ToArray(), automailMessageInvitestream.ToArray(), automailMessageCompleteConfirmationstream.ToArray(), automailMessageIncompleteReminderstream.ToArray(), automailMessageFirstReminderstream.ToArray(), automailMessageSecondReminderstream.ToArray(), automailMessageThirdReminderstream.ToArray(), automailMessageOverdueReminderstream.ToArray(), 1, 1).FirstOrDefault();
                    ViewBag.message = "Congratulation. You have added new Bulk content with id = " + result + " , focus.id=" + Focus.id + " Industry.id=" + Industry.id;
                    ModelState.Clear();
                }
                
            }
            catch
            {
                ViewBag.message = "Unhandled error";
            }
            return View();
        }

        public ActionResult GetFocusList(int industry)
        {
            using (var db = new EntitiesDBContext())
            {
                return Json(db.pr_getFocusByIndustry(industry).ToList(), JsonRequestBehavior.AllowGet);
            }
        }

    }
}

public class PartnerTypeComparer:IEqualityComparer<Generic.partnerType>
{

    public bool Equals(Generic.partnerType x, Generic.partnerType y)
    {
        return x.name == y.name;
    }

    public int GetHashCode(Generic.partnerType obj)
    {
        return string.IsNullOrEmpty(obj.name) ? string.Empty.GetHashCode() : obj.name.GetHashCode();
    }
}
public class TouchpointComparer : IEqualityComparer<Generic.touchpoint>
{

    public bool Equals(Generic.touchpoint x, Generic.touchpoint y)
    {
        return x.description == y.description;
    }

    public int GetHashCode(Generic.touchpoint obj)
    {
        return string.IsNullOrEmpty(obj.description)?string.Empty.GetHashCode(): obj.description.GetHashCode();
    }
}

