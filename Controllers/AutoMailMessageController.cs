using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.ViewModel;
using LinqToExcel;

namespace Generic.Controllers
{
    public class AutoMailMessageController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /AutoMailMessage/

        public ActionResult UploadAutoMailMessage()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            return View();
        }

        [HttpPost]
        public ActionResult UploadAutoMailMessage(int protocol, int partnertype, int touchpoint, HttpPostedFileBase uploadAutoMailMessage)
        {

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
            }

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles/AutoMailMessage"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/AutoMailMessage"));
            }

            // The Name of the Upload component is "attachments" 
            var file = uploadAutoMailMessage;

            // Some browsers send file names with full path. This needs to be stripped.
            var fileName = Path.GetFileName(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/AutoMailMessage"), fileName);

            // The files are not actually saved in this demo
            file.SaveAs(physicalPath);

            int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;

            string sheetname = "mailMessage";
            var excelRead = new ExcelQueryFactory(physicalPath.ToString());
            excelRead.AddMapping<ExcelAutoMailMessage>(x => x.SendDateCalcFactor, "Send Date Calc Factor");
            var autoMailMessageinExcel = from a in excelRead.Worksheet<ExcelAutoMailMessage>(sheetname) select a;

            partnerTypeTouchpointQuestionnaire objptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnerType(partnertype).ToList().Where(x => x.touchpoint == touchpoint).FirstOrDefault();

            int i = 1;
            foreach (var autoMailMessages in autoMailMessageinExcel)
            {
                autoMailMessage objautoMailMessage = new autoMailMessage();
                objautoMailMessage.footer1 = autoMailMessages.Footer;
                objautoMailMessage.subject = autoMailMessages.Subject;
                objautoMailMessage.footer2 = autoMailMessages.Signature;
                objautoMailMessage.text = autoMailMessages.Text;
                objautoMailMessage.partnerTypeTouchpointQuestionnaire = objptq.id;
                objautoMailMessage.sendDateCalcFactor = autoMailMessages.SendDateCalcFactor;

                objautoMailMessage.mailType = i;
                if (i != 4)
                {
                    i++;
                }

                db.autoMailMessage.Add(objautoMailMessage);
                db.SaveChanges();
            }
            ViewBag.Message = "1";
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            return View();
        }

        public ActionResult SkipAutoMailMessage()
        {
            //Session["QuestionnaireId"] = questionnaireId;
            //Session["protocolId"] = protocol;
            //Session["touchpointId"] = touchpoint;
            //Session["partnertypeId"] = partnertype;
            //Session["level"] = level;
            partnerTypeTouchpointQuestionnaire objptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnerType((int)Session["partnertypeId"]).ToList().Where(x => x.touchpoint == (int)Session["touchpointId"]).FirstOrDefault();

            db.pr_bootstrapAutomailMessage(objptq.id, "Not Defined Yet", "Not Defined Yet", "Not Defined Yet", "Not Defined Yet");

            ErrorView objerrorView = new ErrorView();
            objerrorView.errorMessage = "Upload done";
            return PartialView("_Error", objerrorView);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
