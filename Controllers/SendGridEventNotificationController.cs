using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Helpers.Utility;

namespace Generic.Controllers
{
    public class SendGridEventNotificationController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /SendGridEventNotification/


		private string[] events = new string[]{"bounce","dropped","deferred","click","delivered","processed","open"};

        [HttpPost]
        public void Index(List<SendGridEvents> data)
        {
            foreach (var eventDetails in data)
            {
				if (!events.Contains(eventDetails.@event))
					continue;

                using (var context = new EntitiesDBContext())
                {

					int? enterprise = null;
					int? reminderSource = null;
					int? automailMessage = null;

					int res = 0;
					if (int.TryParse(eventDetails.enterprise, out res))
						enterprise = res;

					if (int.TryParse(eventDetails.reminderSource, out res))
						reminderSource = res;


					if (int.TryParse(eventDetails.automailMessage, out res))
						automailMessage = res;


					context.pr_addEventNotification(eventDetails.email, new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt32(eventDetails.timestamp)).AddHours(-4), eventDetails.@event, eventDetails.reason, eventDetails.url, eventDetails.category, eventDetails.accesscode, eventDetails.protocolTouchpoint, eventDetails.ApplicationName, reminderSource, automailMessage, enterprise, eventDetails.loadgroup);
							
					/* try
                    {
                        
                    }
					catch { context.pr_addEventNotification(eventDetails.email, new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt32(eventDetails.timestamp)).AddHours(-4), eventDetails.@event, eventDetails.reason, eventDetails.url, null, eventDetails.accesscode, eventDetails.protocolTouchpoint, eventDetails.ApplicationName, null, null, null, eventDetails.loadgroup); }*/
                }
            }

        }
        private void logerror(string message)
        {
            string LogFilePath = "~/log/";
            LogFilePath = Server.MapPath(LogFilePath);
            if (!Directory.Exists(LogFilePath))
            {
                Directory.CreateDirectory(LogFilePath);
            }
            LogFilePath = LogFilePath + "logs.txt";

            FileStream fs;
            if (!Directory.Exists(LogFilePath))
                fs = new FileStream(LogFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
            else
                fs = new FileStream(LogFilePath, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine(DateTime.Now.ToString("[ dd/MM/yyyy hh:mm:ss ]") + " : " + message);
            sw.WriteLine("_____________________________________________________________________");

            sw.Close();
            fs.Close();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
