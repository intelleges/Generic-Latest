using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class GigfestSessionModel
    {
        public CreateGigfestModel CreateModel { get; set; }
        public IEnumerable<GigfestResponse> Response { get; set; }
    }
    public class CreateGigfestModel
    {
        public string SearchTerm { get; set; }
        public string Qualifier1 { get; set; }
        public string Qualifier2 { get; set; }
        public string Qualifier3 { get; set; }
        public string Qualifier4 { get; set; }
        public string Qualifier5 { get; set; }
        public string Qualifier6 { get; set; }
        public string Qualifier7 { get; set; }
        public string Qualifier8 { get; set; }
        public GigfestRequest GetRequest(string token)
        {
            var result = new GigfestRequest
            {
                searchTerm = SearchTerm,
                qualifiers = new List<string>()
            };
            if (!string.IsNullOrEmpty(Qualifier1))
            {
                result.qualifiers.Add(Qualifier1);
            }
            if (!string.IsNullOrEmpty(Qualifier2))
            {
                result.qualifiers.Add(Qualifier2);
            }
            if (!string.IsNullOrEmpty(Qualifier3))
            {
                result.qualifiers.Add(Qualifier3);
            }
            if (!string.IsNullOrEmpty(Qualifier4))
            {
                result.qualifiers.Add(Qualifier4);
            }
            if (!string.IsNullOrEmpty(Qualifier5))
            {
                result.qualifiers.Add(Qualifier5);
            }
            if (!string.IsNullOrEmpty(Qualifier6))
            {
                result.qualifiers.Add(Qualifier6);
            }
            if (!string.IsNullOrEmpty(Qualifier7))
            {
                result.qualifiers.Add(Qualifier7);
            }
            if (!string.IsNullOrEmpty(Qualifier8))
            {
                result.qualifiers.Add(Qualifier8);
            }
            result.token = token;
            return result;
        }
        
        public override bool Equals(object obj)
        {
            var casted = (CreateGigfestModel)obj;
            if (casted == null) return false;
            return casted.Qualifier1 == Qualifier1 && casted.SearchTerm == SearchTerm && Qualifier2 == casted.Qualifier2 && Qualifier3 == casted.Qualifier3 && Qualifier4 == casted.Qualifier4 && Qualifier5 == casted.Qualifier5 && Qualifier6 == casted.Qualifier6 && Qualifier7 == casted.Qualifier7 && Qualifier8 == casted.Qualifier8;
        }
    }

    public class GigfestRequest
    {
        public string searchTerm { get; set; }
        public List<string> qualifiers { get; set; }
        public string token { get; set; }
    }

    public class GigfestResponse
    {
        public string CompanyAddress { get; set; }
        public string CompanyCity { get; set; }
        public string CompanyCountry { get; set; }
        public string CompanyDuns { get; set; }
        public string CompanyId { get; set; }
        public string CompanyNaicsCode { get; set; }
        public string CompanyName { get; set; }
        public string CompanyPhoneMain { get; set; }
        public string CompanyState { get; set; }
        public string CompanyWebsite { get; set; }
        public string CompanyZipCode { get; set; }
        public DateTime? JobAdDate { get; set; }
        public string JobCity { get; set; }
        public string JobCountry { get; set; }
        public string JobExternalId { get; set; }
        public string JobInternalId { get; set; }
        public List<string> JobKeywords { get; set; }
        public string JobSnippet { get; set; }
        public string JobSource { get; set; }
        public string JobState { get; set; }
        public string JobTitle { get; set; }
        public string JobUrl { get; set; }
        public string SearchQualifier { get; set; }
        public string SearchTerm { get; set; }
    }
}