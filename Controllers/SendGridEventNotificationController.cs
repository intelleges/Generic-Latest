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

        [HttpPost]
        public void Index(List<SendGridEvents> data)
        {
            foreach (var eventDetails in data)
            {
                using (var context = new EntitiesDBContext())
                {
                    try
                    {
                        context.pr_addEventNotification(eventDetails.email, new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt32(eventDetails.timestamp)).AddHours(-4), eventDetails.@event, eventDetails.reason, eventDetails.url, null, eventDetails.accesscode, eventDetails.protocolTouchpoint, eventDetails.ApplicationName, null,null, int.Parse(eventDetails.enterprise), eventDetails.loadgroup);
                    }
					catch { context.pr_addEventNotification(eventDetails.email, new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(Convert.ToInt32(eventDetails.timestamp)).AddHours(-4), eventDetails.@event, eventDetails.reason, eventDetails.url, null, eventDetails.accesscode, eventDetails.protocolTouchpoint, eventDetails.ApplicationName, null, null, null, eventDetails.loadgroup); }
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
