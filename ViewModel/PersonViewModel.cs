using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.ViewModel
{
    public class PersonViewModel
    {
        public Nullable<int> enterprise { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string internalID { get; set; }
        public string title { get; set; }
        public string email { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public Nullable<int> Default_Touchpoint { get; set; }
        public Nullable<int> personStatusID { get; set; }
        public string Person_Status { get; set; }
        public int Group_Count { get; set; }
        public int Role_Count { get; set; }
        public bool IsSelected { get; set; }

        public int id { get; set; }
        public string Touchpoint { get; set; }
    }
}