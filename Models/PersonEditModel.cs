using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class PersonEditModel
    {

        public int id { get; set; }
        public Nullable<int> enterprise { get; set; }
        public Nullable<int> manager { get; set; }
        [Display(Name = "person status", ShortName = "")]
        public Nullable<int> personStatus { get; set; }
        public Nullable<int> riskType { get; set; }
        public Nullable<int> loadHistory { get; set; }
        public Nullable<int> campaign { get; set; }
        public string internalId { get; set; }
        public string nmNumber { get; set; }
        public string socialSecurity { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string title { get; set; }
        public string suffix { get; set; }
        public string nickName { get; set; }
        public string passWord { get; set; }
        public string email { get; set; }
        public string address1 { get; set; }
        [Display(Name ="Logged In From", ShortName ="")]
        public string address2 { get; set; }
        public string city { get; set; }
        public Nullable<int> state { get; set; }
        public string zipcode { get; set; }
        public Nullable<int> country { get; set; }
        public string phone { get; set; }
        public string fax { get; set; }
        public Nullable<int> active { get; set; }
        public Nullable<int> ismanager { get; set; }
        public Nullable<int> partnerPerPage { get; set; }
        [Display(Name = "Current Login Date", ShortName = "")]
        public Nullable<System.DateTime> resetDate { get; set; }
        public Nullable<bool> IsArchived { get; set; }
        [Display(Name = "Previous Login Date", ShortName = "")]
        public Nullable<System.DateTime> archivedDate { get; set; }
    }
}