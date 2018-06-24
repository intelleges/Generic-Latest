using Generic.Helpers.Questionnaire;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class DownloadController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        public ActionResult Index(string accesscode)
        {
            /*var t = db.xx_getAllPDFHyperlinks().Where(o => o.accesscode == accesscode).FirstOrDefault();
            if (t == null)
                return Content("");*/
            Session["accessCode"] = accesscode;
            var _pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            if (_pptq != null)
            {
                var _partnerId = _pptq.partner;
                var _partner = db.pr_getPartner(_partnerId).FirstOrDefault();
                partnerPartnertypeTouchpointQuestionnaire pptq = _partner.partnerPartnertypeTouchpointQuestionnaire.FirstOrDefault();

                var pdf = db.pr_getPPTQpdf(pptq.id).FirstOrDefault();
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(pptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                var questionnair = db.pr_getQuestionnaireByAccesscode(accesscode).FirstOrDefault();
                if (pdf == null || pptq.progress == null)
                {
                    //if pdf was deleted from db but questinnarie was completed then we created customized pdf again
                    if ((pptq.status == 8 && cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONFIRMATION_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().link != null)
                        || (questionnair != null && questionnair.footer != null && questionnair.footer != "1"))
                    {
                        return Redirect("~/Registration/Home/CustomizedPDFConfirmation");
                    }
                    else
                        //otherwise redirect to standart pdf
                        return Redirect("~/Registration/Home/PDFConfirmation");
                }
                else
                {
                    return Redirect("~/Registration/Home/PDFCustomizedConfirmation");
                }
            }
            return Content("");
        }
    }
}