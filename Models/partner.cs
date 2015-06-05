using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic
{
    public partial class partner
    {
        public string FullName
        {
            get
            {
                return name + ":" + firstName + " " + lastName;
            }
        }
    }
}