using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers.Utility
{
    public class EmailFormatSettings
    {
        public person sender { get; set; }
        public person receiver { get; set; }
        public person systemMaster { get; set; }
        public partner partner { get; set; }
        public touchpoint touchpoint { get; set; }
        public int ptq { get; set; }
        public enterprise enterprise { get; set; }
    }
}