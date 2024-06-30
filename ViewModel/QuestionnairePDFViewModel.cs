using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class QuestionnairePDFViewModel
    {
        public int qId { get; set; }
        public int? rId { get; set; }
        public List<string>  comments { get; set; }
        public string  Question { get; set; }
        public string response { get; set; }
        public bool isCheckBox { get; set; }
    }
}