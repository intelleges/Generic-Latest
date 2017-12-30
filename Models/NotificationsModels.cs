using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic
{
    public class NotificationsSearchModel
    {
        public string ptq { get; set; } public int group { get; set; } public int partnerType { get; set; } public string ev { get; set; }
    }

    public class NotificationItem {
        public string Title { get; set; }

        public string Group { get; set; }

        public string PartnerType { get; set; }

        public string Event { get; set; }

        public DateTime? TimeStamp { get; set; }

        public string Email { get; set; }

        public string AccessCode { get; set; }

        public string Reason { get; set; }
    }
}