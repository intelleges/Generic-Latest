using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class AddCompanyProfileDataLoadModel
    {
        public string ExternalId { get; set; }
        public string CompanyName { get; set; }
        public string JobAddress { get; set; }
        public string JobCity { get; set; }
        public string JobState { get; set; }
        public string JobZipCode { get; set; }
        public string JobCountry { get; set; }
        public DateTime? AddDate { get; set; }
        public string JobSource { get; set; }
        public string PocSource { get; set; }
        public string JobSnippet { get; set; }
        public string JobOriginalSnippet { get; set; }
        public string CompanyMainNumber { get; set; }
        public string CompanyURL { get; set; }
        public string SearchTerm { get; set; }
        public long? PocPhoneNumber { get; set; }
        public string PocFirstName { get; set; }
        public string PocLastName { get; set; }
        public string PocTitle { get; set; }
        public string PocEmailAddress { get; set; }
        public string CompanyRevenue { get; set; }
        public string CompanyEmployeeCount{ get; set; }
        public string IndustrySector { get; set; }
        public int?  RelationshipOwner { get; set; }
        public int? PPTQ { get; set; }
        public int? SortOrder { get; set; }
        public bool? Active { get; set; }
    }
}