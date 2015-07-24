using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class ContactDataAlertModel
    {
        public string AccessCode { get; set; }
        public string zCode { get; set; }
        public DateTime Assigned { get; set; }
        public DateTime DueDate { get; set; }
        public string Alert { get; set; }
        public string Author { get; set; }
        public string Status { get; set; }
    }
}