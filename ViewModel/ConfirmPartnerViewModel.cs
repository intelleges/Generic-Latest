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

        public Nullable<int> id { get; set; }
        public Nullable<int> enterprise { get; set; }
        public int touchpoint { get; set; }
        public int Partner_A { get; set; }
        public string Partner_A_Name { get; set; }
        public int Group1ID { get; set; }
        public string Group1 { get; set; }
        public int StatusID_1 { get; set; }
        public string Status1 { get; set; }
        public string IsReference1 { get; set; }
        public int Partner_B { get; set; }
        public string Partner_B_Name { get; set; }
        public int Group2ID { get; set; }
        public string Group2 { get; set; }
        public int StatusID_2 { get; set; }
        public string Status2 { get; set; }
        public string IsReference2 { get; set; }
        public string EmailMatch { get; set; }
        public string InternalIDMatch { get; set; }
        public string FederalIDMatch { get; set; }
        public string DUNSMatch { get; set; }
        public string NameMatch { get; set; }
       
    }
}