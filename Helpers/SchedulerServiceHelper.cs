using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using Generic.Helpers.Utility;
using Generic.Models;
using SendGridMail;
using SendGridMail.Transport;
using System.Diagnostics;
using System.IO;
using SendGrid.Helpers.Mail;

namespace Generic.Helpers
{
    public static class SchedulerServiceHelper
    {
        public static string sSource;
        static string sLog;
        static string sEvent;


        public static bool init(int manualOrAutomatic)
        {

            sSource = "mvcSheduler";
            sLog = "Application";


            try
            {
                var pingTimeStamp = DateTime.Now;
                EntitiesDBContext db = new EntitiesDBContext();


                var reminderList = db.pr_getReminderListAll();

                Dictionary<int?, int> countPtq = new Dictionary<int?, int>();
                int pingRecordsProcessed = 0;
                foreach (var item in reminderList)
                {
                    var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(item.nextToSendPPTQ).FirstOrDefault();


                    var objpartner = db.pr_getPartner(pptq.partner).FirstOrDefault();
                    var person = db.pr_getPerson(objpartner.owner).FirstOrDefault();


                    var amm = db.pr_getAutomailMessage(item.nextToSendAutomailmessage).FirstOrDefault();
                    if (amm != null)
                    {
                        amm.text.Replace("[partner Access Code]", pptq.accesscode);
                    }



                    var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(item.nextToSendPTQ).FirstOrDefault();

                    var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                    Email email = new Email(amm);

                    EmailFormat emailFormat = new EmailFormat();
                    email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq.id);

                    email.emailTo = objpartner.email;
                    email.category = SendGridCategory.ReminderList;
                    email.reminderSource = (int)Reminders.Automaed;
                    email.accesscode = pptq.accesscode;
                    email.protocolTouchpoint = objtouchpoint.description;
                    email.automailMessage = amm.id.ToString();
                    email.protocolTouchpoint = objtouchpoint.description;

                    try
                    {
                        sendEmails(email, (int)ptq.partnerType1.enterprise, null, new EmailFormatSettings()
                        {
                            partner = objpartner,
                            sender = person,
                            ptq = ptq.id,
                            touchpoint = objtouchpoint
                        }, db);
                        db.pr_addPPTQautoMailMessageLog(item.nextToSendPPTQ, item.nextToSendAutomailmessage);

                        pingRecordsProcessed = pingRecordsProcessed + 1;
                        if (!countPtq.ContainsKey(item.nextToSendPTQ))
                        {
                            countPtq.Add(item.nextToSendPTQ, 1);
                        }
                        else
                        {
                            countPtq[item.nextToSendPTQ] += 1;
                        }
                    }
                    catch (Exception ex)
                    {
                        if (!EventLog.SourceExists(sSource))
                        {
                            EventLog.CreateEventSource(sSource, sLog);
                        }

                        sEvent = ex.StackTrace;
                        EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 1);
                        string text = "\r\n \r\n Error in First foreach loop : " + ex.StackTrace;
                        System.IO.File.AppendAllText(System.IO.Path.Combine(@"C:\reminder_Logs", "Logs.txt"), text);

                    }

                }


                foreach (var o in countPtq)
                {
                    db.pr_addPTQSendDateReminderCount(o.Key, o.Value, pingTimeStamp);
                }
                db.pr_addReminderScheduledTaskHeartBeat(pingTimeStamp, pingRecordsProcessed, manualOrAutomatic);
                return true;
            }
            catch (Exception exep)
            {
                if (!EventLog.SourceExists(sSource))
                {
                    EventLog.CreateEventSource(sSource, sLog);
                }

                sEvent = exep.StackTrace;
                EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 2);

                var t = exep.Message;
                return false;

            }

        }

        public static string sendEmails(Email email, int enterpriseId, MailAddress sendFrom, EmailFormatSettings settings, EntitiesDBContext db)
        {
            string returnValue = "";
            string receiver = "";

            key objSendGridPassword = db.pr_getKeyAll(enterpriseId).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();
            var msg = new SendGridMessage();
            List<SendGrid.Helpers.Mail.Attachment> attachments2 = new List<SendGrid.Helpers.Mail.Attachment>();
            Dictionary<string, string> additionalArguments = new Dictionary<string, string>();
            var client = new SendGrid.SendGridClient(objSendGridPassword.api);
            string htmlFooter = "";

            additionalArguments.Add("ApplicationName", "MVCMT - R");
            additionalArguments.Add("enterprise", enterpriseId.ToString());
            additionalArguments.Add("loadgroup", email.loadgroup);
            additionalArguments.Add("accesscode", email.accesscode);
            additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);
            additionalArguments.Add("email", email.emailTo);

            additionalArguments.Add("url", email.url);
            additionalArguments.Add("category", ((int)email.category).ToString());
            additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.Value.ToString());
            additionalArguments.Add("automailMessage", email.automailMessage);

            if (email.type == "user")
            {

            }
            else if (email.type == "provider")
            {

            }
            else if (email.type == "providerowner")
            {

            }
            else if (email.type == "owner")
            {

            }

            else if (email.type == "providerLogin")
            {

            }
            else
            {

                msg.AddTo(email.emailTo);
                receiver = email.emailTo;

            }

            enterpriseSystemInfo objEnterpriseSystemInfo = db.pr_getEnterpriseSystemInfoAll().Where(o => o.enterprise == enterpriseId).FirstOrDefault();
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
            }

            if (attachments2.Count > 0)
                msg.AddAttachments(attachments2);
            additionalArguments.Add("tags", tags);

            try
            {
                string html = email.body.Replace("\n", "<br />").Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");
                if (sendFrom == null)
                    msg.SetFrom(objEnterpriseSystemInfo.coordinatorEmail, objEnterpriseSystemInfo.contractCoordinator);
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

                var task = client.SendEmailAsync(msg);
                task.Wait();
                var response = task.Result;
                if (response.StatusCode != HttpStatusCode.Accepted)
                    throw new Exception("Not Send");

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

                    returnValue = receiver;
                }
            }

            catch (Exception ex)
            {
                if (!EventLog.SourceExists(sSource))
                {
                    EventLog.CreateEventSource(sSource, sLog);
                }

                sEvent = ex.StackTrace;
                EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 3);

                if (email.type == "provider")
                {

                }
                else if (email.type == "providerLogin")
                {

                }
                else
                {
                    returnValue = receiver + ": " + ex.Message.ToString() + ":Email not sent";

                }
            }
            return returnValue;
        }

        public static void sendEmail(Email email, MailAddress sendFrom, bool ccSender, EmailFormatSettings settings)
        {
            sendEmail(email, sendFrom, ccSender, null, settings);
        }
        public static void sendEmail(Email email, MailAddress sendFrom, bool ccSender, HttpFileCollectionBase attachments, EmailFormatSettings settings, iterateEmailText iterateEmailText = null, int? senderEnterpriseid = null)
        {
            int enterpriseid = senderEnterpriseid.HasValue ? senderEnterpriseid.Value : Generic.Helpers.CurrentInstance.EnterpriseID;
            key objSendGridPassword = null;
            using (var db = new EntitiesDBContext())
            {
                if (sendFrom.Address.ToLower().Contains("@battelle.org"))
                {
                    MailMessage message = new MailMessage();
                    SmtpClient smtp = new SmtpClient();
                    message.From = new MailAddress("battelle.admin@intelleges.comm");
                    message.To.Add(new MailAddress(email.emailTo));
                    message.Subject = email.subject;
                    message.IsBodyHtml = true; //to make message body as html  
                    message.Body = email.body;
                    smtp.Port = 587;
                    smtp.Host = "smtp.gmail.com"; //for gmail host  
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential("battelle.admin@intelleges.com", "iqokmnoahqnspfzf");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.Send(message);
                }
                else
                {

                    objSendGridPassword = db.pr_getKeyAll(enterpriseid).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();

                    var msg = new SendGridMessage();
                    List<SendGrid.Helpers.Mail.Attachment> attachments2 = new List<SendGrid.Helpers.Mail.Attachment>();
                    Dictionary<string, string> additionalArguments = new Dictionary<string, string>();
                    var client = new SendGrid.SendGridClient(objSendGridPassword.api);
                    string htmlFooter = "";

                    additionalArguments.Add("ApplicationName", "MVCMT");
                    additionalArguments.Add("enterprise", enterpriseid.ToString());
                    additionalArguments.Add("loadgroup", email.loadgroup);
                    additionalArguments.Add("accesscode", email.accesscode);
                    additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);
                    additionalArguments.Add("email", email.emailTo);

                    additionalArguments.Add("url", email.url);
                    additionalArguments.Add("category", ((int)email.category).ToString());
                    additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.Value.ToString());
                    additionalArguments.Add("automailMessage", email.automailMessage);

                    if (attachments != null)
                    {
                        foreach (string file in attachments)
                        {
                            string key = "c_" + DateTime.Now.Ticks;
                            using (MemoryStream ms = new MemoryStream())
                            {
                                attachments[file].InputStream.CopyTo(ms);
                                attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                                {
                                    Content = Convert.ToBase64String(ms.ToArray()),
                                    ContentId = key,
                                    Filename = attachments[file].FileName
                                });
                            }
                        }
                    }

                    if (iterateEmailText != null)
                    {
                        string key = "c_" + DateTime.Now.Ticks;
                        if (!string.IsNullOrEmpty(iterateEmailText.attachmentOneName))
                            attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                            {
                                Content = Convert.ToBase64String(iterateEmailText.attachmentOne),
                                ContentId = key,
                                Filename = iterateEmailText.attachmentOneName
                            });

                        key = "c_" + DateTime.Now.Ticks;
                        if (!string.IsNullOrEmpty(iterateEmailText.attachmentTwoName))
                            attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                            {
                                Content = Convert.ToBase64String(iterateEmailText.attachmentTwo),
                                ContentId = key,
                                Filename = iterateEmailText.attachmentTwoName
                            });
                    }

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
                    }
                    if (attachments2.Count > 0)
                        msg.AddAttachments(attachments2);
                    additionalArguments.Add("tags", tags);

                    string html = email.body.Replace("\n", "<br />").Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");

                    if (ccSender)
                        msg.AddCc(sendFrom.Address);
                    msg.AddTo(email.emailTo);
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


                    var task = client.SendEmailAsync(msg);
                    task.Wait();
                    var response = task.Result;
                    if (response.StatusCode != HttpStatusCode.Accepted)
                    {
                        var task2 = response.Body.ReadAsStringAsync();
                        task2.Wait();
                        var str = task2.Result;
                        throw new Exception("Not Send");
                    }
                }
            }
        }

        public static void sendEmail(Email email, string filepath, EmailFormatSettings settings, person cc = null)
        {
            key objSendGridPassword = null;
            using (var db = new EntitiesDBContext())
            {
                objSendGridPassword = db.pr_getKeyAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();
                var person = db.pr_getPersonByEmail2(email.emailTo).FirstOrDefault();
                var master = db.pr_getSystemMaster(person.enterprise.Value).FirstOrDefault();
                var msg = new SendGridMessage();
                List<SendGrid.Helpers.Mail.Attachment> attachments2 = new List<SendGrid.Helpers.Mail.Attachment>();
                Dictionary<string, string> additionalArguments = new Dictionary<string, string>();
                var client = new SendGrid.SendGridClient(objSendGridPassword.api);
                string htmlFooter = "";
                string htmlHeader = "";

                additionalArguments.Add("ApplicationName", "MVCMT");
                additionalArguments.Add("enterprise", person.enterprise.Value.ToString());
                additionalArguments.Add("loadgroup", email.loadgroup);
                additionalArguments.Add("accesscode", email.accesscode);
                additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);
                additionalArguments.Add("email", email.emailTo);
                additionalArguments.Add("reason", email.reason);
                additionalArguments.Add("url", email.url);
                additionalArguments.Add("category", ((int)email.category).ToString());
                additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.Value.ToString());
                additionalArguments.Add("automailMessage", email.automailMessage);

                try
                {
                    if (filepath != "")
                    {
                        string key = "c_" + DateTime.Now.Ticks;
                        attachments2.Add(new SendGrid.Helpers.Mail.Attachment()
                        {
                            Content = Convert.ToBase64String(File.ReadAllBytes(filepath)),
                            ContentId = key,
                            Filename = new FileInfo(filepath).Name
                        });
                    }

                    int amid = -1;
                    string tags = "";
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
                    }

                    if (attachments2.Count > 0)
                        msg.AddAttachments(attachments2);
                    additionalArguments.Add("tags", tags);

                    msg.AddTo(email.emailTo);
                    //  mail.AddTo("john@intelleges.com");
                    //   mail.AddTo("goldykhurmi@gmail.com");
                    if (cc != null)
                        msg.AddCc(cc.email, cc.FullName);

                    string html = email.body.Replace("\n", "<br />").Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");

                    msg.SetFrom(master.email, "Intelleges System Master");
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


                    var task = client.SendEmailAsync(msg);
                    task.Wait();
                    var response = task.Result;
                    
                    var body = response.Body.ReadAsStringAsync().Result;
                    if (response.StatusCode != HttpStatusCode.Accepted)
                        throw new Exception("Not Send");
                }

                catch (Exception ex)
                {
                    if (!EventLog.SourceExists(sSource))
                    {
                        EventLog.CreateEventSource(sSource, sLog);
                    }

                    sEvent = ex.StackTrace;
                    EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 4);
                }
            }
        }


        public static bool CreateExcelFile(DataTable dt, string filename)
        {
            try
            {

                string sTableStart = @"<HTML><BODY><TABLE Border=1>";
                string sTableEnd = @"</TABLE></BODY></HTML>";
                string sTHead = "<TR>";
                StringBuilder sTableData = new StringBuilder();
                foreach (DataColumn col in dt.Columns)
                {
                    sTHead += @"<TH>" + col.ColumnName + @"</TH>";
                }
                sTHead += @"</TR>";
                foreach (DataRow row in dt.Rows)
                {
                    sTableData.Append(@"<TR>");
                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        sTableData.Append(@"<TD>" + row[i].ToString() + @"</TD>");
                    }
                    sTableData.Append(@"</TR>");
                }
                string sTable = sTableStart + sTHead + sTableData.ToString() + sTableEnd;
                System.IO.StreamWriter oExcelWriter = System.IO.File.CreateText(filename);
                oExcelWriter.WriteLine(sTable);
                oExcelWriter.Close();
                return true;
            }
            catch (Exception ex)
            {
                if (!EventLog.SourceExists(sSource))
                {
                    EventLog.CreateEventSource(sSource, sLog);
                }

                sEvent = ex.StackTrace;
                EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 5);
                return false;
            }
        }

        public static bool SendFirstReminderByPptq(int pptqId, string accesscode, string url, MailAddress sendFrom)
        {
            bool result = false;
            using (EntitiesDBContext db = new EntitiesDBContext())
            {

                var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(pptqId).FirstOrDefault();

                var person = db.pr_getPersonByEmail(2, "john@intelleges.com").FirstOrDefault();
                var objpartner = db.pr_getPartner(pptq.partner).FirstOrDefault();

                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(pptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var message = db.pr_getAutoMailmessageByMailtypeandPTQ(4, ptq.id).FirstOrDefault();
                //objpartner.enterprise
                var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                Email email = new Email(message);

                email.accesscode = accesscode;
                if (objtouchpoint != null)
                    email.protocolTouchpoint = objtouchpoint.description;
                if (message != null)
                    email.automailMessage = message.id.ToString();
                email.url = url;
                email.category = SendGridCategory.SendFirstReminderByPptq;

                EmailFormat emailFormat = new EmailFormat();
                email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq.id);
                email.subject = emailFormat.sGetEmailBody(email.subject, person, objpartner, objtouchpoint, ptq.id);
                //email.
                email.emailTo = objpartner.email;
                email.reminderSource = (int)Reminders.PartnerFind;

                try
                {
                    sendEmails(email, (int)objpartner.enterprise, sendFrom, new EmailFormatSettings()
                    {
                        sender = person,
                        ptq = ptq.id,
                        partner = objpartner,
                        touchpoint = objtouchpoint
                    }, db);
                    db.pr_addPPTQautoMailMessageLog(pptqId, message.id);
                }
                catch (Exception ex)
                {
                    if (!EventLog.SourceExists(sSource))
                    {
                        EventLog.CreateEventSource(sSource, sLog);
                    }

                    sEvent = ex.StackTrace;
                    EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 6);

                    string text = "\r\n \r\n Error in First foreach loop : " + ex.StackTrace;
                    System.IO.File.AppendAllText(System.IO.Path.Combine(@"C:\reminder_Logs", "Logs.txt"), text);
                }
                result = true;
            }
            return result;
        }

        public static void SendPassword(person person, string emailTo)
        {
            Generic.Helpers.CurrentInstance.EnterpriseID = person.enterprise.Value;
            using (EntitiesDBContext db = new EntitiesDBContext())
            {
                var pr = db.pr_getPerson(person.id).First();
                string accesscode = db.pr_getAccesscode().First();
                db.pr_modifyPerson(pr.id, pr.enterprise, pr.manager, 5, pr.riskType, pr.loadHistory, pr.campaign, pr.internalId, pr.nmNumber, pr.socialSecurity, pr.firstName, pr.lastName, pr.title, pr.suffix, pr.nickName, accesscode, pr.email, pr.address1, pr.address2, pr.city, pr.state, pr.zipcode, pr.country, pr.phone, pr.fax, pr.active, pr.ismanager, pr.partnerPerPage, pr.resetDate, pr.IsArchived, pr.archivedDate);

                var htmlBody = string.Format(@"<br/><br/>
Hello {0} {1},<br/><br/>
Your current password is: {2}<br/><br/>
To set a new password for your intelleges.com account 
login with your existing password and select Change 
Password.<br/><br/>
To protect your privacy, we only send this information to 
the email address on file for this account. <br/><br/>
If you have any questions, please contact your Account 
Administrator.<br/><br/>
Thank you.<br/>
Intelleges Team
", person.firstName, person.lastName, accesscode);

                var master = db.pr_getSystemMaster(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
                var touchpoint = db.pr_getTouchpoint(person.campaign.Value).First();
                sendEmail(new Email()
                {
                    subject = "Intelleges Account Request",
                    emailTo = emailTo,
                    body = htmlBody,
                    category = SendGridCategory.SendPassword,
                    protocolTouchpoint = touchpoint.title,
                    url = "intelleges.com",
                    loadgroup="password reset",
                    reason="password reset"
                }, "", null);
                
                htmlBody = string.Format(@"<br/><br/>
System Master:,<br/><br/>
Please be advised that {0} addressed reset their password.<br/><br/>
Please contact Intelleges team if you have any questions.<br/><br/>
Thank you.<br/>
Intelleges Team
", emailTo);
                if (master != null)
                    sendEmail(new Email()
                    {
                        subject = "Intelleges: alert person.emailAddress Reset Password",
                        emailTo = master.email,
                        body = htmlBody,
                        category = SendGridCategory.SendEmailAlert,
                        accesscode = accesscode,
                    }, "", null);
            }
        }

        public static void SendPasswordChangedNotification(person person, string emailTo)
        {
            var htmlBody = string.Format(@"<br/><br/>
Hello {0} {1},<br/><br/>
Your current password has changed.<br/><br/>
To set a new password for your intelleges.com account 
login with your existing password and select Change 
Password.<br/><br/>
To protect your privacy, we only send this information to 
the email address on file for this account. <br/><br/>
If you have any questions, please contact your Account 
Administrator.<br/><br/>
Thank you.<br/>
Intelleges Team
", person.firstName, person.lastName, person.passWord);


            using (EntitiesDBContext db = new EntitiesDBContext())
            {
                var pr = db.pr_getPerson(person.id);
                sendEmail(new Email()
                {
                    subject = "Intelleges Account Request",
                    emailTo = emailTo,
                    body = htmlBody,
                    category = SendGridCategory.SendPasswordChangedNotification,
                    accesscode = pr.First().partnerPartnertypeTouchpointQuestionnaire.Last().accesscode
                }, "", null);
            }
        }
    }
}