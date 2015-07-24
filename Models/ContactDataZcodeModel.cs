using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class ContactDataZcodeModel
    {
        public string AccessCode { get; set; }
        public string WorkBreakdownStructure { get; set; }
        public DateTime CompletedDate { get; set; }
        public string Status { get; set; }
        public string zCode { get; set; }
        public string AssignedBy { get; set; }
        public string CompletedBy { get; set; }
    }
}