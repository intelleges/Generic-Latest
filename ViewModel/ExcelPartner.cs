using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ExcelPartner : partner
    {
        public string StateName { get; set; }
        public string CountryName { get; set; }
        //public int InternalId { get; set; }
        //public string  Name { get; set; }
        //public string  Address1 { get; set; }
        //public string  Address2 { get; set; }
        //public string  City { get; set; }
        //public string  State { get; set; }
        //public string  Province { get; set; }
        //public string  Zipcode { get; set; }
        // public string  Country { get; set; }
        //public string  POC { get; set; }
        //public string  Phone { get; set; }
        //public string  POC { get; set; }
        //public string  Fax { get; set; }
        //public string  POCEmail { get; set; }
        //public string  POCFirstName { get; set; }
        //	POC LastName	POC Title	POC DUNS	POC EID	RO FirstName	RO LastName	RO Phone	RO Email

    }

    public class ExcelPartnumber : ExcelPartner
    {
        public string INTERNAL_SITE_ID { get; set; }
        public string SAP_SITE { get; set; }
        public string SAP_PLANT_CODE { get; set; }
        public string SITE_NAME { get; set; }

        public string PART_NUMBER_SAP { get; set; }
        public string PART_NUMBER_INTERNAL { get; set; }
        public string SUB_COMMODITY_OWNER { get; set; }
        public string CENTER_OF_EXCELLENCE { get; set; }

        public string RO_FIRST_NAME { get; set; }
        public string RO_LAST_NAME { get; set; }
        public string RO_EMAIL { get; set; }

    }
}