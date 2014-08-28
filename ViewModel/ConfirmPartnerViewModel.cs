using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ConfirmPartnerViewModel
    {
        public bool IsSelected1 { get; set; }
        public bool IsSelected2 { get; set; }
        public bool IsCheckboxSelected { get; set; }

        public int id { get; set; }
        public int enterprise { get; set; }
        public int touchpoint { get; set; }
        public int partnerA { get; set; }
        public string partnerA_Name { get; set; }
        public int group1 { get; set; }
        public string group1_Name { get; set; }
        public int status1 { get; set; }
        public string status1_Name { get; set; }
        public string isReference1 { get; set; }
        public int partnerB { get; set; }
        public string partnerB_Name { get; set; }
        public int group2 { get; set; }
        public string group2_Name { get; set; }
        public int status2 { get; set; }
        public string status2_Name { get; set; }
        public string isReference2 { get; set; }
        public string emailMatch { get; set; }
        public string nameMatch { get; set; }
        public string internalIDMatch { get; set; }
        public string federalIDMatch { get; set; }
        public string dunsMatch { get; set; }
       
    }
}