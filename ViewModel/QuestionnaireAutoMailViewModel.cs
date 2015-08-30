using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.ViewModel
{
    public enum AutoMailTypes
    {

        Invitation = 1,
        Incomplete = 2,
        Complete_Confirmation = 3,
        Reminder = 4,
        Alert = 5
    };
    [Serializable]
    public class QuestionnaireAutoMailViewModel //:autoMailMessage
    {
        const int textToShow = 150;
        public int id { get; set; }
        public string subject { get; set; }
        public string text { get; set; }
        public string footer1 { get; set; }
        public string footer2 { get; set; }
        public AutoMailTypes mailType { get; set; }
        public int partnerTypeTouchpointQuestionnaire { get; set; }
        public int sendDateCalcFactor { get; set; }
        public Nullable<System.DateTime> sendDateSet { get; set; }
        public string ShortText
        {
            get
            {
                if (text.Length > textToShow)
                    return text.Substring(0, textToShow) + "...";
                else return text;
            }
        }
    }
}