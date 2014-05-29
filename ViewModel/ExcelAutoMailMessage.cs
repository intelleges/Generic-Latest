using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ExcelAutoMailMessage
    {
        public int RID { get; set; }
        public string Type { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string Footer { get; set; }
        public string Signature { get; set; }
        public int SendDateCalcFactor { get; set; }
    }
}