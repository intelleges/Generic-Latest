using Generic.Helpers.Questionnaire;
using Generic.Helpers.Utility;
using Generic.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Generic.Helpers
{
    public static class EmailHelper
    {
        public static void SendEmailAlertWhere(partner partner, partnerPartnertypeTouchpointQuestionnaire pptqObj, string accessCode, string emailTo, int ptqId, int questionId, string qnextId, string text, Uri baseUri,
            UrlHelper urlHelper,string requestUrl, string ccEmail = "")
        {
            var db = new EntitiesDBContext();
            autoMailMessage objamm = new autoMailMessage
            {
                subject = "Intelleges: Email Alert"
            };
            var pptq = db.pr_getPartnertypeTouchpointQuestionnaire(ptqId).FirstOrDefault();
            var person = db.pr_getPerson(pptqObj.invitedBy).FirstOrDefault();

            var yesUrl = QuestionnaireHelper.GetQuestionnaireUrl(baseUri, urlHelper, partner.id, emailTo, pptqObj.id, qnextId, 74);
            var noUrl = QuestionnaireHelper.GetQuestionnaireUrl(baseUri, urlHelper, partner.id, emailTo, pptqObj.id, qnextId, 75);
            var unlockUrl = QuestionnaireHelper.GetQuestionnaireUrl(baseUri, urlHelper, partner.id, emailTo, pptqObj.id, qnextId, -1);

            objamm.subject = "Supplier Responsibility Assessment for " + partner.name + " " + accessCode;

            string t = "";
            t = "Company Name: " + partner.name + "<br/>";
            t += "Company Internal ID: " + partner.internalID + "<br/>";
            t += "POC First Name: " + partner.firstName + "<br/>";
            t += "POC Last Name: " + partner.lastName + "<br/>";
            t += "POC Phone #: " + partner.phone + "<br/>";
            t += "POC Email: " + partner.email + "<br/><br/>";
            t += "Access Code Link: <a href='https://www.intelleges.com/mvcmt/Generic/Registration?Accesscode=" + accessCode + "'>" + accessCode + "</a><br/><br/>";
            t += "PDF Link: <a href='https://www.intelleges.com/mvcmt/Generic/Download?accesscode=" + accessCode + "'>" + accessCode + "</a><br/><br/>Clauses Subject to Review:<br/>";
            t += text + "<br/><br/>APPROVAL:  " + "<br/><a href='" + yesUrl + "'>Yes</a><br/><a href='" + noUrl + "'>No</a><br/><a href='" + unlockUrl + "'>Unlock</a>" +
                 "<br/><br/>Thanks.<br/><br/>" + StringHelper.UppercaseFirst(person.firstName) + " " + StringHelper.UppercaseFirst(person.lastName) + "<br>";

            objamm.text = t;

            string emailsTo = emailTo;

            if (ccEmail != "")
            {
                emailsTo += ";" + ccEmail;
            }

            Email mail = new Email(objamm)
            {
                type = "emailAlert",
                emailTo = emailsTo,
                url = requestUrl,
                accesscode = accessCode,
                category = SendGridCategory.SendEmailAlert
            };

            if (pptqObj != null && pptqObj.partnerTypeTouchpointQuestionnaire1 != null)
            {
                var tp = pptqObj.partnerTypeTouchpointQuestionnaire1.touchpoint1;
                if (tp != null)
                {
                    mail.protocolTouchpoint = tp.description;
                }
            }

            if (person != null)
            {
                SendEmail objSendEmail = new SendEmail();
                objSendEmail.sendEmail(mail, new EmailFormatSettings()
                {
                    sender = null,
                    enterprise = pptqObj.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1,
                    partner = pptqObj.partner1,
                    ptq = pptqObj.partnerTypeTouchpointQuestionnaire,
                    touchpoint = null
                }, sendFrom: new System.Net.Mail.MailAddress(person.email, StringHelper.UppercaseFirst(person.firstName) + " " + StringHelper.UppercaseFirst(person.lastName)));
            }
            else
            {
                SendEmail objSendEmail = new SendEmail();
                objSendEmail.sendEmail(mail, new EmailFormatSettings()
                {
                    sender = null,
                    enterprise = pptqObj.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1,
                    partner = pptqObj.partner1,
                    ptq = pptqObj.partnerTypeTouchpointQuestionnaire,
                    touchpoint = null
                });
            }
        }
    }
}