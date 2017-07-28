using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class AddPartnerSpreadsheetDataLoadModel
    {
        public string PartnerInternalId { get; set; }
        public string PartnerSupId { get; set; }
        public string PartnerDunsNumber { get; set; }
        public string PartnerName { get; set; }
        public string PartnerAddressOne{ get; set; }
        public string PartnerAddressTwo { get; set; }
        public string PartnerCity { get; set; }
        public string PartnerState { get; set; }
        public string PartnerZipCode { get; set; }
        public string PartnerCountry { get; set; }
        public string PartnerPocFirstName { get; set; }
        public string PartnerPocLastName { get; set; }
        public string PartnerPocTitle { get; set; }
        public string PartnerPocPhoneNumber { get; set; }
        public string PartnerPocEmailAddress { get; set; }
        public string RoFirstName { get; set; }
        public string RoLastName { get; set; }
        public string RoEmail { get; set; }
        public DateTime?  DateLoaded { get; set; }
        public int? Enterprise { get; set; }
        public int? PartnerType { get; set; }
        public int? Touchpoint { get; set; }
        public int? Person { get; set; }
        public int? PartnerSpreadSheetDataLoad { get; set; }
        public string LoadGroup { get; set; }
        public DateTime? DueDate { get; set; }

        public int? Group { get; set; }

    }
}