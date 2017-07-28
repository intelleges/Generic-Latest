using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class AddCompanyProfileDataLoadModel
    {
        [Required]
        public string ExternalId { get; set; }
        [Required]
        public string CompanyName { get; set; }
        [Required]
        public string JobAddress { get; set; }
        [Required]
        public string JobCity { get; set; }
        [Required]
        public string JobState { get; set; }
        [Required]
        public string JobZipCode { get; set; }
        [Required]
        public string JobCountry { get; set; }
        [Required]
        public DateTime? AddDate { get; set; }
        [Required]
        public string JobSource { get; set; }
        [Required]
        public string PocSource { get; set; }
        [Required]
        public string JobSnippet { get; set; }
        [Required]
        public string JobOriginalSnippet { get; set; }
        [Required]
        public string CompanyMainNumber { get; set; }
        [Required]
        public string CompanyURL { get; set; }
        [Required]
        public string SearchTerm { get; set; }
        [Required]
        public long? PocPhoneNumber { get; set; }
        [Required]
        public string PocFirstName { get; set; }
        [Required]
        public string PocLastName { get; set; }
        [Required]
        public string PocTitle { get; set; }
        [Required]
        public string PocEmailAddress { get; set; }
        [Required]
        public string CompanyRevenue { get; set; }
        [Required]
        public string CompanyEmployeeCount{ get; set; }
        [Required]
        public string IndustrySector { get; set; }
        [Required]
        public int?  RelationshipOwner { get; set; }
        [Required]
        public int? PPTQ { get; set; }
        [Required]
        public int? SortOrder { get; set; }
        [Required]
        public bool? Active { get; set; }
    }
}