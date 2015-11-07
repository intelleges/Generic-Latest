using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class InvalidZCodeViewModel
    {
        public int Id { get; set; }
        public string Touchpoint { get; set; }
        public string PartnerType { get; set; }
        public string ZCode { get; set; }
        public string ZCodeActionType { get; set; }

    }
}