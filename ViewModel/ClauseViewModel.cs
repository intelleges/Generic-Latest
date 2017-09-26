using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ClauseViewModel
    {
        public List<ClauseViewModelItem> items { get; set; }

        public int pptq { get; set; }

        public int partnerType { get; set; }
    }

    public class ClauseViewModelItem
    {
        public int id { get; set; }

        public string text { get; set; }

        public int? sendDataTo { get; set; }

        public int? getApprovalFrom { get; set; }

        public DateTime approvalNeededBy { get; set; }
    }
}