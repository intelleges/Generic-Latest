using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.UI.WebControls;
using Generic.Models;
using SendGridMail;
using SendGridMail.Transport;
using System.IO;
using SendGrid.Helpers.Mail;

namespace Generic.Helpers.Utility
{
    /// <summary>
    /// Summary description for utility
    /// </summary>
    public class SendEmail
    {
        public SendEmail()
        {
            //
            // TODO: Add constructor logic here
            //
        }
       
        public string sendEmail(Email email, EmailFormatSettings settings, MailAddress sendFrom = null, List<AttachmentInfo> attachments = null, byte[] pdfBytes=null)
        {
            EntitiesDBContext db = new EntitiesDBContext();
            string returnValue = "";
            string receiver = ""; 
           
            key objSendGridPassword = db.pr_getKeyAll(settings.enterprise.id).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();
            var msg = new SendGridMessage();
            List<SendGrid.Helpers.Mail.Attachment> attachments2 = new List<SendGrid.Helpers.Mail.Attachment>();
            Dictionary<string, string> additionalArguments = new Dictionary<string, string>();
            var client = new SendGrid.SendGridClient(objSendGridPassword.api);
            string htmlFooter = "";

            additionalArguments.Add("ApplicationName", "MVCMT");
            additionalArguments.Add("enterprise", settings.enterprise.id.ToString());
            additionalArguments.Add("loadgroup", email.loadgroup);
            additionalArguments.Add("accesscode", email.accesscode);
            additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);
            additionalArguments.Add("email", email.emailTo);
            additionalArguments.Add("url", email.url);
            additionalArguments.Add("category", ((int)email.category).ToString());
            additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.ToString());
            additionalArguments.Add("automailMessage", email.automailMessage);

            if (email.type == "user")
            {
                // mail.AddTo(email.user.email);
                //   receiver = email.user.email;
            }
            else if (email.type == "provider")
            {
                //mail.AddTo(email.provider.contact.email);
                //receiver = email.provider.contact.email;
            }
            else if (email.type == "providerowner")
            {
                //mail.AddTo(email.provider.contact.email);
                //mail.AddCc(email.provider.owner.email);
                //receiver = email.provider.contact.email;
            }
            else if (email.type == "owner")
            {
                //mail.AddTo(email.provider.owner.email);
                //receiver = email.provider.owner.email;
            }

            else if (email.type == "providerLogin")
            {
                //mail.AddTo(email.providerLogin.email);
                //receiver = email.providerLogin.email;
            }
            if (email.type == "emailAlert")
            {
                var emails = email.emailTo.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                List<EmailAddress> emads = new List<EmailAddress>();
                foreach (var emailTo in emails)
                {
                    emads.Add(new EmailAddress(emailTo));
                }
                msg.AddTos(emads);
            }
            else
            {
                msg.AddTo(email.emailTo);
                receiver = email.emailTo;
            }

            enterpriseSystemInfo objEnterpriseSystemInfo = db.pr_getEnterpriseSystemInfoAll().Where(o => o.enterprise == settings.enterprise.id).FirstOrDefault();

            int amid = -1;
            string tags = "";
            string htmlHeader = "";
            if (!string.IsNullOrEmpty(email.automailMessage) && int.TryParse(email.automailMessage, out amid))
            {
                var attachments1 = db.pr_getAutoMailAttachmentAllByAutoMail(amid).ToList();
                EmailFormat ef = new EmailFormat();
                foreach (var item in attachments1)
                {
                    string key = "c_" + DateTime.Now.Ticks;
                    if (item.automailAttachmentType == 3)
                    {
                        string link = item.tags;
                        if (settings != null)
                        {
                            try
                            {
                                if (settings.systemMaster != null && settings.ptq == 0)
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.receiver, settings.touchpoint, settings.enterprise, settings.systemMaster);
                                }
                                else if (settings.systemMaster != null && settings.ptq != 0)
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.receiver, settings.partner, settings.touchpoint, settings.enterprise, settings.systemMaster, settings.ptq);
                                }
                                else if (settings.enterprise != null)
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.partner, settings.enterprise, settings.touchpoint, settings.ptq);
                                }
                                else
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.partner, settings.touchpoint, settings.ptq);
                                }
                            }
                            catch { }
                        }

                        htmlHeader += " <table width='100%'><tr><td align='center'><a href='" + link.Trim().Replace(" ", "-") + "'><img width='600' src='cid:" + key + "' /></a></td></tr></table><br/>";
                        attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(item.attachment),
                            ContentId = key,
                            Disposition = "inline",
                            Type = "image/png",
                            Filename = item.note
                        });
                    }

                    if (item.automailAttachmentType == 1)
                    {
                        string link = item.tags;
                        if (settings != null)
                        {
                            try
                            {
                                if (settings.systemMaster != null && settings.ptq == 0)
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.receiver, settings.touchpoint, settings.enterprise, settings.systemMaster);
                                }
                                else if (settings.systemMaster != null && settings.ptq != 0)
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.receiver, settings.partner, settings.touchpoint, settings.enterprise, settings.systemMaster, settings.ptq);
                                }
                                else if (settings.enterprise != null)
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.partner, settings.enterprise, settings.touchpoint, settings.ptq);
                                }
                                else
                                {
                                    link = ef.sGetEmailBody(link, settings.sender, settings.partner, settings.touchpoint, settings.ptq);
                                }
                            }
                            catch { }
                        }

                        htmlFooter += "<a href='" + link.Trim().Replace(" ", "-") + "'><img src='cid:" + key + "' /></a><br/>";
                        attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(item.attachment),
                            ContentId = key,
                            Disposition = "inline",
                            Type = "image/png",
                            Filename = item.note
                        });
                    }

                    if (item.automailAttachmentType == 2)
                    {
                        attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(item.attachment),
                            ContentId = key,
                            Filename = item.note
                        });

                        if (!string.IsNullOrEmpty(item.tags))
                            tags = item.tags + ";";
                    }
                }
                //int ac
                if (string.IsNullOrEmpty(htmlFooter) && settings != null && settings.sender != null)
                {

                    var autoMailObj = db.pr_getAutomailMessage(amid).FirstOrDefault();
                    if (autoMailObj != null && autoMailObj.footer2 == "[footer2]")
                    {
                        var personSignature = db.pr_getPersonSignature(settings.sender.id).FirstOrDefault();
                        if (personSignature != null)
                            htmlFooter = personSignature.footer2;
                    }
                }
            }

            if (attachments != null)
            {
                foreach (var item in attachments)
                {
                    attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                    {
                        Content = item.Content,
                        ContentId = item.ContentId,
                        Disposition = item.Disposition,
                        Type = item.Filename,
                        Filename = item.Filename
                    });
                }
            }
            if (pdfBytes != null)
            {
                string attachmentBase64 = Convert.ToBase64String(pdfBytes);
                attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                {
                    Content = attachmentBase64,
                    Disposition = "attachment",
                    Type = "application/pdf",
                    Filename = settings.touchpoint.title + "_" + email.accesscode + ".pdf"
                });
            }

            if (attachments2.Count > 0)
                msg.AddAttachments(attachments2);

            additionalArguments.Add("tags", tags);

            string html = email.body.Replace("\n", "<br />").Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");

            if (sendFrom == null)
            {
                if (settings != null && settings.touchpoint!= null)
                {
                    var r = db.pr_getTouchpointAdminPersonByTouchpoint(settings.touchpoint.id).First();
                    msg.SetFrom(r.email, r.firstName + " "+ r.lastName);
                }
                else {
                    msg.SetFrom(objEnterpriseSystemInfo.coordinatorEmail, objEnterpriseSystemInfo.contractCoordinator);
                } 
            } 
            else
                msg.SetFrom(sendFrom.Address, sendFrom.DisplayName);

            msg.AddContent("text/html", htmlHeader + html);
            msg.Subject = email.subject;
            msg.AddCustomArgs(additionalArguments);
            if (!string.IsNullOrEmpty(htmlFooter))
                msg.SetFooterSetting(true, html: htmlFooter);
            msg.SetSandBoxMode(false);
            msg.TrackingSettings = new TrackingSettings()
            {
                ClickTracking = new ClickTracking() { Enable = true, EnableText = true },
                Ganalytics = new Ganalytics() { Enable = true },
                OpenTracking = new OpenTracking() { Enable = true, SubstitutionTag = "%opentrack" },
            };

            try
            {
                var task = client.SendEmailAsync(msg);
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception("Not Send");
                //mail.SetHtmlBody(email.body);


                //                Boolean x=true;

                //  mailMan.SendEmail(mail);

                //returnValue = receiver;

                //if (!x)
                // {
                //                    string abg = "Message not sent to ";
                // }
                ////bye me
                //if (!mailMan.SendEmail(mail))
                //{
                //    mailMan.SaveLastError("ErrorLog.xml");
                //}

                ////end 
                if (email.type == "user")
                {
                    returnValue = receiver;
                }

                else if (email.type == "providerLogin")
                {
                    returnValue = receiver;
                }
                else
                {
                    //  Provider provider = new Provider(new Id(email.provider.id.id)).getProviderById();
                    //   provider.addProviderLogLastContact(email);
                    returnValue = receiver;
                }
            }

            catch (Exception ex)
            {
                if (email.type == "provider")
                {
                    //        returnValue = receiver + ": " + addErrorMessage(ex.Message.ToString(), email.provider.id.id);
                }
                else if (email.type == "providerLogin")
                {
                    //         returnValue = receiver + ": " + addErrorMessage(ex.Message.ToString(), email.providerLogin.Id.id);
                }
                else
                {
                    returnValue = receiver + ": " + ex.Message.ToString() + ":Email not sent";

                }
            }
            return returnValue;
        }

        public string addErrorMessage(string message, int id)
        {
            string errorMessage = "";
            int errorCode = 0;
            int codeStart = message.IndexOf(":");
            int codeEnd = message.IndexOf("sorry"); ;
            int messageStart = message.IndexOf("sorry");
            int messageEnd = message.IndexOf("(#");

            //get error code
            //get error code
            try
            {
                errorCode = int.Parse(message.Substring(codeStart + 2, codeEnd - codeStart - 2));


                //get error message
                errorMessage = message.Substring(messageStart, messageEnd - messageStart);
            }
            catch (Exception ex) { }

            //EmailBounceType emailBounceType = new EmailBounceType();
            //emailBounceType.code = errorCode;
            //emailBounceType.description = errorMessage;

            //Provider provider = new Provider();
            //provider.emailBounceType = emailBounceType;

            ////add errorCode and errorMessage to database
            //DataAccessProvider dataAccess = new DataAccessProvider();
            //dataAccess.modifyProviderEmailBounceType(emailBounceType, id);
            return errorMessage;
        }

    }

    public class AttachmentInfo
    {
        public string Content { get; set; }
        public string ContentId { get; set; }
        public string Disposition { get; set; }
        public string Type { get; set; }
        public string Filename { get; set; }
    }

    public class EmailBounce
    {
        public EmailBounce()
        {

        }

        private int _emailBounceType;

        public int emailBounceType
        {
            get { return _emailBounceType; }
            set { _emailBounceType = value; }
        }
    }
    public class FormatedPhone
    {
        public FormatedPhone(string phone)
        {
            this.phone = phone;
        }

        private string _phone;

        public string phone
        {
            get { return _phone; }
            set
            {
                if (value.Length > 9)
                {
                    value = value.Replace("-", "");
                    value = value.Replace("/", "");
                    value = value.Replace(".", "");
                    value = value.Replace("#", "");
                    value = value.Replace(" ", "");
                    value = value.Replace("ext", " ext ");
                    if (value.Substring(0, 1) == "1")
                    {
                        string start = "";
                        string mid = "";
                        string end = "";
                        start = value.Substring(1, 3);
                        mid = value.Substring(4, 3);
                        end = value.Substring(7, value.Length - 7);
                        value = "1" + " " + start + "-" + mid + "-" + end;
                    }
                    else
                    {
                        string start = "";
                        string mid = "";
                        string end = "";
                        start = value.Substring(0, 3);
                        mid = value.Substring(3, 3);
                        end = value.Substring(6, value.Length - 6);
                        value = start + "-" + mid + "-" + end;
                    }
                }
                _phone = value;
            }
        }
        public string getFormatedPhone()
        {
            return phone;
        }
    }

    public class CheckSession
    {
        public CheckSession()
        {

        }

        public CheckSession(string content)
        {
            this.content = content;
        }

        private string _content;

        public string content
        {
            get { return _content; }
            set { _content = value; }
        }

        private Boolean _isLogIn;

        public Boolean isLogIn
        {
            get { return _isLogIn; }
            set { _isLogIn = value; }
        }

        public Boolean IsSessionOn()
        {
            if (this.content.Length > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }


    public class ValidateAndLookUpControl
    {
        public ValidateAndLookUpControl()
        {

        }

        public ValidateAndLookUpControl(string controlId, string targetControlId, string controlType)
        {
            this.controlId = controlId;
            this.targetControlId = targetControlId;
            this.controlType = controlType;
        }

        public ValidateAndLookUpControl(string controlId, string controlType)
        {
            this.controlId = controlId;
            this.controlType = controlType;
        }

        public ValidateAndLookUpControl(Address address, string controlType)
        {
            this.address = address;
            this.controlType = controlType;
        }

        private string _controlId;
        private string _targetControlId;
        private string _controlType;
        private Address _address;

        public Address address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string controlType
        {
            get { return _controlType; }
            set { _controlType = value; }
        }

        public string targetControlId
        {
            get { return _targetControlId; }
            set { _targetControlId = value; }
        }

        public string controlId
        {
            get { return _controlId; }
            set { _controlId = value; }
        }

        //    public LinkButton lookUp(Question question)
        //    {
        //        LinkButton linkButton = new LinkButton();
        //        linkButton.ID = this.controlId;
        //        linkButton.CausesValidation = false;

        //        switch (this.controlType)
        //        {
        //            case "lookUpZipCode":
        //                linkButton.Text = " lookUpZipCode ";
        //                linkButton.PostBackUrl = "lookUpZipCode.aspx?question=" + question.id.id.ToString();
        //                break;
        //            case "lookUpNaicsCode":
        //                linkButton.Text = " lookUpNaicsCode ";
        //                linkButton.PostBackUrl = "lookUpNaicsCode.aspx?question=" + question.id.id.ToString();
        //                break;
        //            case "lookUpDunsNumber":
        //                linkButton.Text = " lookUpDunsNumber ";
        //                linkButton.PostBackUrl = "lookUpDunsNumber.aspx?question" + question.id.id.ToString();
        //                break;
        //            default:
        //                break;
        //        }

        //        return linkButton;
        //    }

        //    public RegularExpressionValidator validateData()
        //    {
        //        RegularExpressionValidator regValidator = new RegularExpressionValidator();
        //        regValidator.ID = this.controlId;

        //        switch (this.controlType)
        //        {
        //            case "validateFederalId":
        //                regValidator.ControlToValidate = targetControlId;
        //                regValidator.ValidationExpression = "\\d{9}";
        //                regValidator.ErrorMessage = "Must be 9 digits";
        //                break;
        //            case "validateDunsNumber":
        //                regValidator.ControlToValidate = targetControlId;
        //                regValidator.ValidationExpression = "\\d{9}";
        //                regValidator.ErrorMessage = "Must be 9 digits";
        //                break;
        //            case "validateNaicsCode":
        //                regValidator.ControlToValidate = targetControlId;
        //                regValidator.ValidationExpression = "\\d{6}";
        //                regValidator.ErrorMessage = "Must be 6 digits";
        //                break;
        //            case "validateZipCode":
        //                regValidator.ControlToValidate = targetControlId;
        //                regValidator.ValidationExpression = "\\d{5}(-\\d{4})?";
        //                regValidator.ErrorMessage = "Invalid Postal Code";
        //                break;
        //            case "validateBusinessDescription":
        //                regValidator.ControlToValidate = targetControlId;
        //                regValidator.ValidationExpression = "^.{0,1000}$";
        //                regValidator.ErrorMessage = "Must be less than 1000 characters";
        //                break;
        //            case "validateEmail":
        //                regValidator.ControlToValidate = targetControlId;
        //                regValidator.ValidationExpression = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
        //                regValidator.ErrorMessage = "Invalid Email Address";
        //                break;
        //            case "validateDate":
        //                regValidator.ControlToValidate = targetControlId;
        //                regValidator.ValidationExpression = "\\b(0?[1-9]|1[012])[- /.](0?[1-9]|[12][0-9]|3[01])[- /.](19|20)?[0-9]{2}(?:(?:\\s+)?(?:(?:(0?[0-9]|1[0-9]|2[0-3])[:.]([0-5][0-9])[:.]?([0-5][0-9])?)?)?)(\\s+([AaPp][Mm]))?\\b";
        //                regValidator.ErrorMessage = "Invalid Email Address";
        //                break;
        //            default:
        //                break;
        //        }

        //        return regValidator;
        //    }
    }

    public static class partnerStatusTypes
    {
        public const int PARTNER_INVITED_NO_RESPONSE = 1;
        public const int PARTNER_RESPONSE_INCOMPLETE = 2;
        public const int PARTNER_RESPONSE_COMPLETE = 3;
    }

    public static class autoMailTypes
    {

        public const int Invitation = 1;
        public const int Incomplete = 2;
        public const int Complete_Confirmation = 3;
        public const int Reminder = 4;
        public const int Alert = 5;
    }

    public static class GlobalVariable
    {
        static string _language;
        public static string languageStr
        {
            get { return _language; }
            set { _language = value; }
        }
    }
    //public class findControl
    //{
    //    public static void PrepareControlForExport(Control control)
    //    {

    //        for (int i = 0; i < control.Controls.Count; i++)
    //        {
    //            Control current = control.Controls[i];
    //            if (current is LinkButton)
    //            {
    //                control.Controls.Remove(current);
    //                control.Controls.AddAt(i, new LiteralControl((current as
    //                LinkButton).Text));
    //            }
    //            else if (current is ImageButton)
    //            {
    //                control.Controls.Remove(current);
    //                control.Controls.AddAt(i, new LiteralControl((current as
    //                ImageButton).AlternateText));
    //            }
    //            else if (current is HyperLink)
    //            {
    //                control.Controls.Remove(current);
    //                control.Controls.AddAt(i, new LiteralControl((current as
    //                HyperLink).Text));
    //            }
    //            else if (current is DropDownList)
    //            {
    //                control.Controls.Remove(current);
    //                control.Controls.AddAt(i, new LiteralControl((current as
    //                DropDownList).SelectedItem.Text));
    //            }
    //            else if (current is CheckBox)
    //            {
    //                control.Controls.Remove(current);
    //                control.Controls.AddAt(i, new LiteralControl((current as
    //                CheckBox).Checked ? "true" : "false"));
    //            }

    //            if (current.HasControls())
    //            {
    //                PrepareControlForExport(current);

    //            }
    //        }
    //    }
    //}

    //Ckeck Mobile Application
    public static class Utilities
    {
        public static bool isMobileBrowser()
        {
            //GETS THE CURRENT USER CONTEXT
            HttpContext context = HttpContext.Current;

            //FIRST TRY BUILT IN ASP.NT CHECK
            if (context.Request.Browser.IsMobileDevice)
            {
                return true;
            }
            //THEN TRY CHECKING FOR THE HTTP_X_WAP_PROFILE HEADER
            if (context.Request.ServerVariables["HTTP_X_WAP_PROFILE"] != null)
            {
                return true;
            }
            //THEN TRY CHECKING THAT HTTP_ACCEPT EXISTS AND CONTAINS WAP
            if (context.Request.ServerVariables["HTTP_ACCEPT"] != null &&
                context.Request.ServerVariables["HTTP_ACCEPT"].ToLower().Contains("wap"))
            {
                return true;
            }
            //AND FINALLY CHECK THE HTTP_USER_AGENT 
            //HEADER VARIABLE FOR ANY ONE OF THE FOLLOWING
            if (context.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                //Create a list of all mobile types
                string[] mobiles =
                    new[]
                {
                    "midp", "j2me", "avant", "docomo",
                    "novarra", "palmos", "palmsource",
                    "240x320", "opwv", "chtml",
                    "pda", "windows ce", "mmp/",
                    "blackberry", "mib/", "symbian",
                    "wireless", "nokia", "hand", "mobi",
                    "phone", "cdm", "up.b", "audio",
                    "SIE-", "SEC-", "samsung", "HTC",
                    "mot-", "mitsu", "sagem", "sony"
                    , "alcatel", "lg", "eric", "vx",
                    "NEC", "philips", "mmm", "xx",
                    "panasonic", "sharp", "wap", "sch",
                    "rover", "pocket", "benq", "java",
                    "pg", "vox", "amoi",
                    "bird", "compal", "kg", "voda",
                    "sany", "kdd", "dbt", "sendo",
                    "sgh", "gradi", "jb", "dddi",
                    "moto", "iphone"
                };

                //Loop through each item in the list created above 
                //and check if the header contains that text
                foreach (string s in mobiles)
                {
                    if (context.Request.ServerVariables["HTTP_USER_AGENT"].ToLower().Contains(s.ToLower()))
                    {
                        //context.Request.ServerVariables["HTTP_USER_AGENT"]
                        return true;
                    }
                }
            }

            return false;
        }
    }

}