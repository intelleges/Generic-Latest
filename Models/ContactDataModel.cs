using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class ContactDataModel
    {
        public List<ContactDataTouchpoint> Touchpoints { get; set; }
        public List<ContactDataZcodeModel> ZCodes { get; set; }
        public List<ContactDataAlertModel> Alerts { get; set; }
    }
}