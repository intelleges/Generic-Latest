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
                //CurrentInstance.EnterpriseID = 2;
               
                //System.Web.HttpContext.Current.Session["EnterpriseId"] = 2;
                // pr_getReminderListByCountryAll
                // pr_getReminderListIncompleteByCountryAll

                var reminderList = db.pr_getReminderListByCountryAll(true).ToList();
                var reminderIncompleteList = db.pr_getReminderListIncompleteByCountryAll().ToList();
                Dictionary<int?, int> countPtq = new Dictionary<int?, int>();
                int pingRecordsProcessed = 0;
                foreach (var item in reminderList)
                {
                    //  if (item.name == "Sukhbir Singh")
                    //if (item.enterprise == 1) srdjan
                    //{ srdjan
                        //Console.WriteLine(item.name);



                        var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(item.pptq).FirstOrDefault();
                        // pptq.invitedDate = DateTime.Now;
                        var person = db.pr_getPersonByEmail(2, "john@intelleges.com").FirstOrDefault();
                        //pptq.invitedBy = person.id;
                        //pptq.status = (int)PartnerStatus.Invited_NoResponse;
                        //db.Entry(pptq).State = EntityState.Modified;
                        //db.SaveChanges();

                        var objpartner = db.pr_getPartner(pptq.partner).FirstOrDefault();
                        //objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                        //db.Entry(objpartner).State = EntityState.Modified;
                        //db.SaveChanges();

                        var amm = db.pr_getAutomailMessage(item.automailmessage).FirstOrDefault();
                    if(amm != null)
                    {
                        amm.text.Replace("[partner Access Code]", pptq.accesscode);
                    }

                  // srdjan     var objpartnerByAccessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireDueDateByAccessCode(pptq.accesscode, pptq.loadGroup).FirstOrDefault();

                        //if (objpartnerByAccessCode != null)
                        //{
                        //    amm.text = amm.text.Replace("[Due Date]", objpartnerByAccessCode.Value.ToString("MMM, dd, yyyy"));
                        //}

                        var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(item.ptq).FirstOrDefault();

                        var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                        Email email = new Email(amm);


                        email.accesscode = pptq.accesscode;
                        email.protocolTouchpoint = objtouchpoint.description;

                        EmailFormat emailFormat = new EmailFormat();
                        email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq.id);

                        email.emailTo = objpartner.email;

                       
                        try
                        {
                          sendEmails(email, (int)item.enterprise, db);
                          db.pr_addPPTQautoMailMessageLog(item.pptq, item.automailmessage);
                          pingRecordsProcessed = pingRecordsProcessed + 1;
                          if(!countPtq.ContainsKey(item.ptq))
                          {
                              countPtq.Add(item.ptq, 1);
                          }
                          else
                          {
                              countPtq[item.ptq] += 1;
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
                #region Ignored
                //   foreach (var item in reminderIncompleteList)
             //   {
             //       //  if (item.name == "Sukhbir Singh")
             //////srdjan       if (item.enterprise == 1)
             //   ////srdjan    {
             //           //Console.WriteLine(item.name);



             //           var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaire(item.pptq).FirstOrDefault();
             //           // pptq.invitedDate = DateTime.Now;
             //           var person = db.pr_getPersonByEmail(2, "john@intelleges.com").FirstOrDefault();
             //           //pptq.invitedBy = person.id;
             //           //pptq.status = (int)PartnerStatus.Invited_NoResponse;
             //           //db.Entry(pptq).State = EntityState.Modified;
             //           //db.SaveChanges();

             //           var objpartner = db.pr_getPartner(pptq.partner).FirstOrDefault();
             //           //objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
             //           //db.Entry(objpartner).State = EntityState.Modified;
             //           //db.SaveChanges();

             //           var amm = db.pr_getAutomailMessage(item.automailmessage).FirstOrDefault();
             //       if(amm != null)
             //       {
             //           amm.text.Replace("[partner Access Code]", pptq.accesscode);
             //       }

             //      // srdjan    var objpartnerByAccessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireDueDateByAccessCode(pptq.accesscode, pptq.loadGroup).FirstOrDefault();

             //           //if (objpartnerByAccessCode != null)
             //           //{
             //           //    amm.text = amm.text.Replace("[Due Date]", objpartnerByAccessCode.Value.ToString("MMM, dd, yyyy"));
             //           //}

             //           var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(item.ptq).FirstOrDefault();

             //           var objtouchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
             //           Email email = new Email(amm);


             //           email.accesscode = pptq.accesscode;
             //           email.protocolTouchpoint = objtouchpoint.description;

             //           EmailFormat emailFormat = new EmailFormat();
             //           email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq.id);
             //           email.emailTo = objpartner.email;

             //           // email.emailTo = "goldykhurmi@gmail.com";

             //           // SendEmail objSendEmail = new SendEmail();
             //           //objSendEmail.sendEmail(email);
             //           try
             //           {
             //            sendEmails(email, (int)item.enterprise, db);
             //            db.pr_addPPTQautoMailMessageLog(item.pptq, item.automailmessage);
             //            pingRecordsProcessed = pingRecordsProcessed + 1;
             //           }
             //           catch (Exception ex)
             //           {

             //               string text = "\r\n \r\n Error in SECOND foreach loop : " + ex.StackTrace;
             //               System.IO.File.AppendAllText(System.IO.Path.Combine(@"C:\reminder_Logs", "Logs.txt"), text);

             //           }
             //           // sendEmail(item.subject, "", "", "john@intelleges.com");
             //   ////srdjan    }

                //   }
                #endregion

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

           
            //DataTable dt = DataAccessScheduler.getDailyReport();

            ////if the directory already exists
            ////create a new directory name

            //if (!Directory.Exists("c:/notRespondedlist/"))
            //{
            //    Directory.CreateDirectory("c:/notRespondedlist/");
            //}
            ////create the new directory and save the file in it

            //string fileName = "notRespondedPartnersList.xls";
            //string filePath = "c:/notRespondedlist/" + fileName;

            //CreateExcelFile(dt, filePath);

            //sendEmail("Not Responded partner List", "Here is the list of Partners who Not Responded yet.Please find the attachment for the same.", filePath, "john@intelleges.com");


        }

        public static string sendEmails(Email email, int enterpriseId, EntitiesDBContext db)
        {
           // EntitiesDBContext db = new EntitiesDBContext();
            string returnValue = "";
            string receiver = "";

            key objSendGridPassword = db.pr_getKeyAll(enterpriseId).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();

            var mail = SendGrid.GetInstance();
            var credentials = new NetworkCredential(objSendGridPassword.username, objSendGridPassword.password);
            

            Dictionary<string, string> additionalArguments = new Dictionary<string, string>();
            additionalArguments.Add("ApplicationName", "MVCMT - R");
            additionalArguments.Add("enterprise", enterpriseId.ToString());
            additionalArguments.Add("loadgroup", email.loadgroup);
            additionalArguments.Add("accesscode", email.accesscode);
            additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);

            mail.AddUniqueIdentifiers(additionalArguments);


            //mail.StreamedAttachments.Add(
            //mail.CreateMimeMessage().AlternateViews.Add;



          //  var transportSMTP = SMTP.GetInstance(credentials);
            var transportSMTP = SMTP.GetInstance(credentials, "smtp.sendgrid.net", 587);


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
            else
            {
               // email.emailTo = "office@softwarexpert.net";
                mail.AddTo(email.emailTo);
                receiver = email.emailTo;

            }
            //  string extension = mail.AddFileAttachment(email.attachment);

            enterpriseSystemInfo objEnterpriseSystemInfo = db.pr_getEnterpriseSystemInfoAll(enterpriseId).FirstOrDefault();

            mail.From = new MailAddress(objEnterpriseSystemInfo.coordinatorEmail, objEnterpriseSystemInfo.contractCoordinator);

            //mail.From = email.sender.email;
            //mail.FromName = email.sender.firstName + " " + email.sender.lastName;
            //mail.ReplyTo = email.sender.email;
            mail.Subject = email.subject;

            //set body format
            if (email.body.Contains("\n"))
                email.body = email.body.Replace("\n", "<br />");
            if (email.body.Contains("\t"))
                email.body = email.body.Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            mail.Html = email.body;
            try
            {
                transportSMTP.Deliver(mail);
             //   SendEmail objSendEmail = new SendEmail();
               // objSendEmail.sendEmail(email);
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
                if (!EventLog.SourceExists(sSource))
                {
                    EventLog.CreateEventSource(sSource, sLog);
                }

                sEvent = ex.StackTrace;
                EventLog.WriteEntry(sSource, sEvent, EventLogEntryType.Error, 3);

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
        public static void sendEmail(string subject, string body, string sendto, MailAddress sendFrom, bool ccSender)
        {
            var mail = SendGrid.GetInstance();
            key objSendGridPassword=null;
            using (var db = new EntitiesDBContext())
            {
                objSendGridPassword = db.pr_getKeyAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();
            }

            var credentials = new NetworkCredential(objSendGridPassword.username, objSendGridPassword.password);
            Dictionary<string, string> additionalArguments = new Dictionary<string, string>();


            additionalArguments.Add("ApplicationName", "MVCMT");
            mail.AddUniqueIdentifiers(additionalArguments);
            // Create an SMTP transport for sending email.
            var transportSMTP = SMTP.GetInstance(credentials);

            mail.AddTo(sendto);
            if (ccSender)
                mail.AddCc(sendFrom.Address);
            mail.From = sendFrom;

            mail.Subject = subject;

            body = body.Replace("\n", "<br />");
            body = body.Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");
            mail.Html = body;
            transportSMTP.Deliver(mail);

        }
        public static void sendEmail(string subject, string body, string sendto, MailAddress sendFrom, bool ccSender, HttpFileCollectionBase attachments)
        {
            var mail = SendGrid.GetInstance();

            key objSendGridPassword = null;
            using (var db = new EntitiesDBContext())
            {
                objSendGridPassword = db.pr_getKeyAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();
            }

            var credentials = new NetworkCredential(objSendGridPassword.username, objSendGridPassword.password);
            Dictionary<string, string> additionalArguments = new Dictionary<string, string>();


            additionalArguments.Add("ApplicationName", "MVCMT");
            mail.AddUniqueIdentifiers(additionalArguments);
            // Create an SMTP transport for sending email.
            var transportSMTP = SMTP.GetInstance(credentials);

            mail.AddTo(sendto);
            if (ccSender)
                mail.AddCc(sendFrom.Address);
            mail.From = sendFrom;

            mail.Subject = subject;
            foreach (string file in attachments)
            {
                mail.AddAttachment(attachments[file].InputStream, attachments[file].FileName);
            }

            body = body.Replace("\n", "<br />");
            body = body.Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");
            mail.Html = body;
            transportSMTP.Deliver(mail);

        }
        public static void sendEmail(string subject, string body, string filepath, string sendto)
        {


            var mail = SendGrid.GetInstance();

            key objSendGridPassword = null;
            using (var db = new EntitiesDBContext())
            {
                objSendGridPassword = db.pr_getKeyAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList().Where(x => x.@object == "sendgrid").FirstOrDefault();
            }

            var credentials = new NetworkCredential(objSendGridPassword.username, objSendGridPassword.password);
            Dictionary<string, string> additionalArguments = new Dictionary<string, string>();


            additionalArguments.Add("ApplicationName", "MVCMT");
            mail.AddUniqueIdentifiers(additionalArguments);


            // Create an SMTP transport for sending email.
            var transportSMTP = SMTP.GetInstance(credentials);

            try
            {

                mail.AddTo(sendto);

                //  mail.AddTo("john@intelleges.com");
                //   mail.AddTo("goldykhurmi@gmail.com");
                if (filepath != "")
                {
                    mail.AddAttachment(filepath);
                }

                mail.From = new MailAddress("hs3admin2@intelleges.com", "Honeywell Supply Chain Security");

                mail.Subject = subject;

                body = body.Replace("\n", "<br />");
                body = body.Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");
                mail.Html = body;




                transportSMTP.Deliver(mail);





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

        public static bool SendFirstReminderByPptq(int pptqId)
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

                email.accesscode = pptq.accesscode;
                email.protocolTouchpoint = objtouchpoint.description;

                EmailFormat emailFormat = new EmailFormat();
                email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq.id);

                email.emailTo = objpartner.email;
                try
                {
                  sendEmails(email, (int)objpartner.enterprise, db);
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

        public static void SendPassword(pr_getPasswordByEmail_Result person, string emailTo)
        {
            var htmlBody=string.Format(@"<br/><br/>
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
",person.firstName,person.lastName,person.passWord);
        sendEmail("Intelleges Account Request", htmlBody, "", emailTo);
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
         sendEmail("Intelleges Account Request", htmlBody, "", emailTo);
        }
    }
}