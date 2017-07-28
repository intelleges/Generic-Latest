
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class AddPartnerSpreadsheetDataLoadModel
    {
        [Required]
        public string PartnerInternalId { get; set; }
        [Required]
        public string PartnerSupId { get; set; }
        [Required]
        public string PartnerDunsNumber { get; set; }
        [Required]
        public string PartnerName { get; set; }
        [Required]
        public string PartnerAddressOne{ get; set; }
        [Required]
        public string PartnerAddressTwo { get; set; }
        [Required]
        public string PartnerCity { get; set; }
        [Required]
        public string PartnerState { get; set; }
        [Required]
        public string PartnerZipCode { get; set; }
        [Required]
        public string PartnerCountry { get; set; }
        [Required]
        public string PartnerPocFirstName { get; set; }
        [Required]
        public string PartnerPocLastName { get; set; }
        [Required]
        public string PartnerPocTitle { get; set; }
        [Required]
        public string PartnerPocPhoneNumber { get; set; }
        [Required]
        public string PartnerPocEmailAddress { get; set; }
        [Required]
        public string RoFirstName { get; set; }
        [Required]
        public string RoLastName { get; set; }
        [Required]
        public string RoEmail { get; set; }
        [Required]
        public DateTime?  DateLoaded { get; set; }
        [Required]
        public int? Enterprise { get; set; }
        [Required]
        public int? PartnerType { get; set; }
        [Required]
        public int? Touchpoint { get; set; }
        [Required]
        public int? Person { get; set; }
        [Required]
        public int? PartnerSpreadSheetDataLoad { get; set; }
        [Required]
        public string LoadGroup { get; set; }
        [Required]
        public DateTime? DueDate { get; set; }
        [Required]
        public int? Group { get; set; }

    }
}