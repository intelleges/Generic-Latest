using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ExcelEventNotification
    {
        public string Email { get; set; }
        public string AccessCode { get; set; }
        public string LoadGroup { get; set; }
        public string ProtocolTouchpoint { get; set; }
        public string Reason { get; set; }
        public DateTime? Date { get; set; }
        public string Event { get; set; }
    }
}