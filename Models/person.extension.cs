using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic
{
    public partial class person
    {
        public string FullName
        {
            get
            {
                return firstName + " " + lastName;
            }
        }

        public int? RoleId { get; set; }
        public int? GroupId { get; set; }
       
    }
}