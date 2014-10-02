using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    [Serializable]
    public class ExcelQuestionnaireQuestionnireCMS 
    {
        public int questionnaire { get; set; }
        public int questionnaireCMS { get; set; }
        public string description { get; set; }
        public string text { get; set; }
        public string link { get; set; }
        public byte[] doc { get; set; }
       
    }
}