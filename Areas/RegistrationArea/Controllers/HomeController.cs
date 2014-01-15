using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Generic.DataLayer;
namespace Generic.Areas.RegistrationArea.Controllers
{
    public class HomeController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /RegistrationArea/Home/

        public virtual ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public virtual ActionResult Index(string accessCode)
        {
            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
            if (ppptq.accesscode != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var touchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                if (touchpoint.endDate >= DateTime.Now)
                {
                    Session["accessCode"] = accessCode;
                    Session["hs3Registration"] = 1;
                    Session["languageused"] = "en";
                    Session["accessAttemp"] = 0;
                    Session["partner"] = ppptq.partner;
                    Session["touchpoint"] = touchpoint.id;
                    Session["partnerType"] = ptq.partnerType;
                    Session["questionnaire"] = ptq.questionnaire;
                    Session["protocol"] = touchpoint.protocol;

                    return RedirectToAction("companyInformation");
                }
            }

            return View();
        }


        public virtual ActionResult CompanyInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            ViewBag.country = db.pr_getCountry(objPartner.country).FirstOrDefault().name;
            return View(objPartner);
        }

        public virtual ActionResult ContactInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            ViewBag.country = db.pr_getCountry(objPartner.country).FirstOrDefault().name;
            return View(objPartner);
        }
        public virtual ActionResult CorrectCompanyInformation()
        {
            return RedirectToAction("ContactInformation");
        }
        public virtual ActionResult CorrectContactInformation()
        {
            return RedirectToAction("QuestionnaireResponse");
        }


        public virtual ActionResult QuestionnaireResponse(int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            int questionnaireId = (int)Session["questionnaire"];

            questionnaire objQuestionnaire = db.pr_getQuestionnaire(questionnaireId).FirstOrDefault();
            
            touchpoint objtouchpoint = new touchpoint();
            partner objpartner = new partner();
            protocol objprotocol = new protocol();

            surveyForm objSurveyForm = new surveyForm(objprotocol, objtouchpoint, objpartner, objQuestionnaire);
            objSurveyForm.questionIndex = questionIndex;
            objSurveyForm.questionClass = "brownbg  brownbgarrow";
            objSurveyForm.answerClass = "brownbg";
            objSurveyForm.alternativeAnswerClass = "bluebg";
            objSurveyForm.alternativequestionClass = "bluebg bluebgarrow";
            
            objSurveyForm.errorquestion = errorQuestion;
            objSurveyForm.errorMessage = errorMessage;
            Table table = objSurveyForm.tGetsurveyForm(objQuestionnaire, pageNumber, page, jumpToQuestion);
           // Table table = objSurveyForm.tGetSurveyForm(objQuestionnaire, pageNumber, page, jumpToQuestion);
           // panel.Controls.Add(table);

            StringWriter objhtml = new StringWriter();
            using (var htmlWriter = new HtmlTextWriter(objhtml))
            {
                table.RenderControl(htmlWriter);
            }

            ViewBag.questions = objhtml.ToString();

            return View();
        }


        /// <summary> 
        ///This is the method for trialling overrides 
        /// </summary> 
        /// <returns></returns> 
        public ActionResult Submit()
        {
            string response = BreakHereIfYoureInTheArea();
            return RedirectToAction("Index", "Home", new { Area = "RegistrationArea" });
        }


        /// <summary> 
        ///Note - this is virtual, as we are going to override it in the client
        ///project. 
        /// </summary> 
        /// <returns></returns> 
        public virtual string BreakHereIfYoureInTheArea()
        {
            string test = "break here - i am generic.";
            return test;
        }
    }
}
