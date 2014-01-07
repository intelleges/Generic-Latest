using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class MenuModel
    {
        public int id { get; set; }
        public string description { get; set; }
        public string url { get; set; }
        public Nullable<int> parentid { get; set; }
        public Nullable<int> sortOrder { get; set; }
        public Nullable<bool> active { get; set; }
        public Nullable<int> accesslevel { get; set; }
        public Nullable<int> enterprise { get; set; }        
    }
}