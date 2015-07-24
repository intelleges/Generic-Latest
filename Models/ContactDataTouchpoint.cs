using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class ContactDataTouchpoint
    {
        public string AccessCode { get; set; }
        public string Email { get; set; }
        public DateTime Assigned { get; set; }
        public DateTime DueDate { get; set; }
        public string Touchpoint { get; set; }
        public string Status { get; set; }
        public string Loadgroup { get; set; }
    }
}