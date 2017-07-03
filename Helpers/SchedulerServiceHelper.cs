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
						sendEmails(email, (int)ptq.partnerType1.enterprise, db);
						db.pr_addPPTQautoMailMessageLog(item.nextToSendPPTQ, item.nextToSendAutomailmessage);


						/*db.pr_addEventNotification(pptq.partner1.email, DateTime.Now,null, null, "", ((int)SendGridCategory.ReminderList).ToString(), email.accesscode, pptq.partnerTypeTouchpointQuestionnaire1.touchpoint1.description, "MVCMT", (int)Reminders.Automaed, amm.id, pptq.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1.id, null).FirstOrDefault();*/

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
			additionalArguments.Add("email", email.emailTo);

			additionalArguments.Add("url", email.url);
			additionalArguments.Add("category", ((int)email.category).ToString());
			additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.Value.ToString());
			additionalArguments.Add("automailMessage", email.automailMessage);

			mail.AddUniqueIdentifiers(additionalArguments);
			var transportSMTP = SMTP.GetInstance(credentials, "smtp.sendgrid.net", 587);


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

				mail.AddTo(email.emailTo);
				receiver = email.emailTo;

			}


			enterpriseSystemInfo objEnterpriseSystemInfo = db.pr_getEnterpriseSystemInfoAll().Where(o => o.enterprise == enterpriseId).FirstOrDefault();

			mail.From = new MailAddress(objEnterpriseSystemInfo.coordinatorEmail, objEnterpriseSystemInfo.contractCoordinator);


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
		public static void sendEmail(Email email, MailAddress sendFrom, bool ccSender)
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
			additionalArguments.Add("enterprise", Generic.Helpers.CurrentInstance.EnterpriseID.ToString());
			additionalArguments.Add("loadgroup", email.loadgroup);
			additionalArguments.Add("accesscode", email.accesscode);
			additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);
			additionalArguments.Add("email", email.emailTo);

			additionalArguments.Add("url", email.url);
			additionalArguments.Add("category", ((int)email.category).ToString());
			additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.Value.ToString());
			additionalArguments.Add("automailMessage", email.automailMessage);

			mail.AddUniqueIdentifiers(additionalArguments);
			// Create an SMTP transport for sending email.
			var transportSMTP = SMTP.GetInstance(credentials, "smtp.sendgrid.net", 587);

			mail.AddTo(email.emailTo);
			if (ccSender)
				mail.AddCc(sendFrom.Address);
			mail.From = sendFrom;

			mail.Subject = email.subject;
			mail.Html = email.body.Replace("\n", "<br />").Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");
			transportSMTP.Deliver(mail);

		}
		public static void sendEmail(Email email, MailAddress sendFrom, bool ccSender, HttpFileCollectionBase attachments, iterateEmailText iterateEmailText = null)
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
			additionalArguments.Add("enterprise", Generic.Helpers.CurrentInstance.EnterpriseID.ToString());
			additionalArguments.Add("loadgroup", email.loadgroup);
			additionalArguments.Add("accesscode", email.accesscode);
			additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);
			additionalArguments.Add("email", email.emailTo);

			additionalArguments.Add("url", email.url);
			additionalArguments.Add("category", ((int)email.category).ToString());
			additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.Value.ToString());
			additionalArguments.Add("automailMessage", email.automailMessage);

			mail.AddUniqueIdentifiers(additionalArguments);
			// Create an SMTP transport for sending email.
			var transportSMTP = SMTP.GetInstance(credentials, "smtp.sendgrid.net", 587);

			mail.AddTo(email.emailTo);
			if (ccSender)
				mail.AddCc(sendFrom.Address);
			mail.From = sendFrom;

			mail.Subject = email.subject;
			foreach (string file in attachments)
			{
				mail.AddAttachment(attachments[file].InputStream, attachments[file].FileName);
			}

			if (iterateEmailText != null)
			{
				if (!string.IsNullOrEmpty(iterateEmailText.attachmentOneName))
					mail.AddAttachment(new MemoryStream(iterateEmailText.attachmentOne), iterateEmailText.attachmentOneName);

				if (!string.IsNullOrEmpty(iterateEmailText.attachmentTwoName))
					mail.AddAttachment(new MemoryStream(iterateEmailText.attachmentTwo), iterateEmailText.attachmentTwoName);
			}


			mail.Html = email.body.Replace("\n", "<br />").Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");
			transportSMTP.Deliver(mail);
		}

		public static void sendEmail(Email email, string filepath)
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
			additionalArguments.Add("enterprise", Generic.Helpers.CurrentInstance.EnterpriseID.ToString());
			additionalArguments.Add("loadgroup", email.loadgroup);
			additionalArguments.Add("accesscode", email.accesscode);
			additionalArguments.Add("protocolTouchpoint", email.protocolTouchpoint);
			additionalArguments.Add("email", email.emailTo);

			additionalArguments.Add("url", email.url);
			additionalArguments.Add("category", ((int)email.category).ToString());
			additionalArguments.Add("reminderSource", email.reminderSource == null ? "" : email.reminderSource.Value.ToString());
			additionalArguments.Add("automailMessage", email.automailMessage);
			mail.AddUniqueIdentifiers(additionalArguments);


			// Create an SMTP transport for sending email.
			var transportSMTP = SMTP.GetInstance(credentials, "smtp.sendgrid.net", 587);

			try
			{

				mail.AddTo(email.emailTo);

				//  mail.AddTo("john@intelleges.com");
				//   mail.AddTo("goldykhurmi@gmail.com");
				if (filepath != "")
				{
					mail.AddAttachment(filepath);
				}

				mail.From = new MailAddress("hs3admin2@intelleges.com", "Honeywell Supply Chain Security");

				mail.Subject = email.subject;

				mail.Html = email.body.Replace("\n", "<br />").Replace("\t", "&nbsp&nbsp&nbsp&nbsp&nbsp");
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

		public static bool SendFirstReminderByPptq(int pptqId, string accesscode, string url)
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
", person.firstName, person.lastName, person.passWord);


			using (EntitiesDBContext db = new EntitiesDBContext())
			{
				var pr = db.pr_getPerson(person.id);
				sendEmail(new Email()
				{
					subject = "Intelleges Account Request",
					emailTo = emailTo,
					body = htmlBody,
					category = SendGridCategory.SendPassword,
					accesscode = pr.First().partnerPartnertypeTouchpointQuestionnaire.Last().accesscode
				}, "");
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
				}, "");
			}
		}
	}
}