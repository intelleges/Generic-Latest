using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.ViewModel
{
    [Serializable]
    public class QuestionnaireAutoMailViewModel //:autoMailMessage
    {
        public int id { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string footer1 { get; set; }
        public string footer2 { get; set; }
        public int mailType { get; set; }
        public int partnerTypeTouchpointQuestionnaire { get; set; }
        public int sendDateCalcFactor { get; set; }
        public Nullable<System.DateTime> sendDateSet { get; set; }
    }
}