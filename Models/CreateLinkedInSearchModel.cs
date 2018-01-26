using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class CreateLinkedInSearchModel
    {
        public string SearchTerm { get; set; }
        public string Qualifier1 { get; set; }
        public string Qualifier2 { get; set; }
        public string Qualifier3 { get; set; }
        public string Excluded1 { get; set; }
        public string Excluded2 { get; set; }
        public string Excluded3 { get; set; }
        public string GetSearchString()
        {
            var result = "";
            if(!string.IsNullOrEmpty(SearchTerm))
            {
                result += SearchTerm;
            }
            if (!string.IsNullOrEmpty(Qualifier1))
            {
                result += (!string.IsNullOrEmpty(result) ? " " : "") + Qualifier1;
            }
            if (!string.IsNullOrEmpty(Qualifier2))
            {
                result += (!string.IsNullOrEmpty(result) ? " " : "") + Qualifier2;
            }
            if (!string.IsNullOrEmpty(Qualifier3))
            {
                result += (!string.IsNullOrEmpty(result) ? " " : "") + Qualifier3;
            }
            if (!string.IsNullOrEmpty(Excluded1))
            {
                result += (!string.IsNullOrEmpty(result) ? " " : "") + "-"+ Excluded1;
            }
            if (!string.IsNullOrEmpty(Excluded2))
            {
                result += (!string.IsNullOrEmpty(result) ? " " : "") + "-" + Excluded2;
            }
            if (!string.IsNullOrEmpty(Excluded3))
            {
                result += (!string.IsNullOrEmpty(result) ? " " : "") + "-" + Excluded3;
            }
            return result;
        }
    }
}