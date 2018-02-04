using FileHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    [IgnoreFirst]
    [DelimitedRecord(";")]
    public class LinkedInCsvConnection
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string Position { get; set; }
        //[FieldConverter(ConverterKind.Date, "MM/dd/yyyy, hh:mm tt")]
        public string ConnectedOn { get; set; }
        // public string ConnectedOnLast { get; set; }
        [FieldOptional]
        public string Tags { get; set; }
        //public DateTime? ConnectedOn
        //{
        //    get
        //    {
        //        DateTime result;
        //        if(DateTime.TryParse((ConnectedOnFirst+ ConnectedOnLast).Replace("\"",""),out result))
        //        {
        //            return result;
        //        } else
        //        {
        //            return null;
        //        }
        //    }
        //}
        public string GetCSEQueryForPerson()
        {
            return "\""+FirstName + " " + LastName + "\" \"" + Company+ "\"";
        }

        public string GetSCEQueryForCompany()
        {
            return "\"" + Company + "\" company";
        }
    }
}