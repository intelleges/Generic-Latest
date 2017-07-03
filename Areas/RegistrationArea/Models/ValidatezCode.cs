using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Generic.Models;
using Generic.Helpers.Utility;
using Generic.Helpers.PartnerHelper;

namespace Generic.Areas.RegistrationArea.Models
{
    public class ValidatezCode
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        public string  ValidatezCodeFn(partnerPartnertypeTouchpointQuestionnaire pptq, int sessionTouchPoint, string sessionCurrentEmail, string sessionAccessCode, string url)
        {

            var zCodeValidationResult = db.pr_checkForInvalidZcode(pptq != null ? pptq.id : 0, pptq != null ? pptq.zcode : "");



            // Obtain the zCode error code.
            var zCodeValidationErrorCode = zCodeValidationResult != null ? zCodeValidationResult.FirstOrDefault() : null;

           var incorrectZipCode = zCodeValidationErrorCode != null ? zCodeValidationErrorCode.nextstep : null;

            using (var dbConext = new EntitiesDBContext())
            {
                var count = dbConext.pr_checkPartnumberStatusCountByPPTQ(pptq != null ? pptq.id : 0).FirstOrDefault();

                if (count == 0 && !(zCodeValidationErrorCode != null && zCodeValidationErrorCode.newStatus != 0 && zCodeValidationErrorCode.newStatus != 6))
                {
                    dbConext.pr_modifyPPTQStatus(pptq != null ? pptq.partner : 0, pptq != null ? pptq.partnerTypeTouchpointQuestionnaire : 0, (int)PartnerStatus.Responded_Complete);
                }
                else
                {

                    dbConext.pr_modifyPPTQStatus(pptq != null ? pptq.partner : 0, pptq != null ? pptq.partnerTypeTouchpointQuestionnaire : 0, (int)PartnerStatus.Responded_Incomplete);


                    var pptq_emaildata = pptq != null ? pptq.automailMessagePPTQ : null;


                    // (By:Manpreet) Send Email to Partner--- START HERE--
                    if (zCodeValidationErrorCode != null && zCodeValidationErrorCode.newStatus == 2)
                    {

                        // var emailhtl = dbConext.pr_getAutoMailmessageByMailtypeandPTQ(2, pptq.automailMessagePPTQ);

                        // var emailhtl = dbConext.pr_getAutoMailmessageByMailtypeandPTQ(2, pptq.id);

                        var touchpoint = db.pr_getTouchpoint((int)sessionTouchPoint).FirstOrDefault();
                        //Send Alert to TouchAdmin
                        var _person = db.pr_getPerson(touchpoint != null ? touchpoint.admin : 0).FirstOrDefault();
                        Email email = new Email();
                        string zCode = pptq != null ? pptq.zcode : "";
                        string strEmailBody = sessionCurrentEmail + " with Invalid zCode " + zCode + " for access code " + sessionAccessCode + ". The status has been reset to incomplete for this partner.";
                        email.subject = "Intelleges: Email Alert for Invalid zCode";
                        email.body = strEmailBody;
						email.accesscode = sessionAccessCode;
						email.category = SendGridCategory.ValdatezCodeFn;
						email.url = url;
                        email.emailTo = _person != null ? _person.email : "";

                        SendEmail objSendEmail = new SendEmail();
                        objSendEmail.sendEmail(email);
						/*db.pr_addEventNotification(email.emailTo, DateTime.Now, null, null, email.url, ((int)email.category).ToString(), sessionAccessCode, email.protocolTouchpoint, "MVCMT", null, null, null, null);*/
                    }
                    //else
                    //{

                    //    // var emailhtl = dbConext.pr_getAutoMailmessageByMailtypeandPTQ(3, pptq.id);
                    //    var touchpoint = db.pr_getTouchpoint((int)sessionTouchPoint).FirstOrDefault();
                    //    //Send Alert to TouchAdmin
                    //    var _person = db.pr_getPerson(touchpoint != null ? touchpoint.admin : 0).FirstOrDefault();
                    //    Email email = new Email();
                    //    string zCode = pptq != null ? pptq.zcode : "";
                    //    string strEmailBody = sessionCurrentEmail + " with Invalid zCode " + zCode + " for access code " + sessionAccessCode + ". The status has been reset to incomplete for this partner.";
                    //    email.subject = "Intelleges: Email Alert for Invalid zCode";
                    //    email.body = strEmailBody;
                    //    email.emailTo = _person != null ? _person.email : "";
                    //    SendEmail objSendEmail = new SendEmail();
                    //    objSendEmail.sendEmail(email);
                    //}

                    //--- ENDS HERE----
                    //TempData["IncorrectZipCode"] = zCodeValidationErrorCode.nextstep;
                }
            }
            return incorrectZipCode;

        }
    }
}