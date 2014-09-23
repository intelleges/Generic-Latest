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
    public class ConfirmPartnerSpreadsheetViewModel
    {
        public Nullable<int> pptqA_id { get; set; }
        public int Partner_A { get; set; }
        public string AccessCode_A { get; set; }
        public string PartnerA_Name { get; set; }
        public string PartnerA_Address { get; set; }
        public string InternalID_A { get; set; }
        public string email_A { get; set; }
        public int GroupID_A { get; set; }
        public string Group_A { get; set; }
        public int StatusID_A { get; set; }
        public string Status_A { get; set; }
        public int pptqB_id { get; set; }
        public int Partner_B { get; set; }
        public string AccessCode_B { get; set; }
        public string PartnerB_Name { get; set; }
        public string PartnerB_Address { get; set; }
        public string InternalID_B { get; set; }
        public string email_B { get; set; }
        public int GroupID_B { get; set; }
        public string Group_B { get; set; }
        public int StatusID_B { get; set; }
        public string Status_B { get; set; }
        public string IsReference2 { get; set; }
        public string EM { get; set; }
        public string IM { get; set; }
        public string FM { get; set; }
        public string DM { get; set; }
        public string NM { get; set; }
        public string action { get; set; }



    }
    public class ConfirmPartnerSpreadsheetViewModelupload
    {
        public Nullable<int> pptqA_id { get; set; }
        public int Partner_A { get; set; }
        public string AccessCode_A { get; set; }
        public string PartnerA_Name { get; set; }
        public string PartnerA_Address { get; set; }
        public string InternalID_A { get; set; }
        public string email_A { get; set; }
        public int GroupID_A { get; set; }
        public string Group_A { get; set; }
        public int StatusID_A { get; set; }
        public string Status_A { get; set; }
        public int pptqB_id { get; set; }
        public int Partner_B { get; set; }
        public string AccessCode_B { get; set; }
        public string PartnerB_Name { get; set; }
        public string PartnerB_Address { get; set; }
        public string InternalID_B { get; set; }
        public string email_B { get; set; }
        public int GroupID_B { get; set; }
        public string Group_B { get; set; }
        public string StatusID_B { get; set; }
        public string Status_B { get; set; }
        public string IsReference2 { get; set; }
        public string EM { get; set; }
        public string IM { get; set; }
        public string FM { get; set; }
        public string DM { get; set; }
        public string NM { get; set; }
        public string action { get; set; }


    }
}