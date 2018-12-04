using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class QuestionnaireByAccessCodeModel_question

    {
        public QuestionnaireByAccessCodeModel_question()
        {
            responses = new List<QuestionnaireByAccessCodeModel_response>();
        }
        public int id { get; set; }
        public string question { get; set; }
        public string tag { get; set; }
        public int? weight { get; set; }
        public int? required { get; set; }
        public int? skipLogicAnswer { get; set; }
        public string skipLogicJump { get; set; }
        public string subCheckBoxChoice { get; set; }
        public int? accessLevel { get; set; }
        public int? commentRequired { get; set; }
        public string commentBoxTxt { get; set; }
        public string commentUploadTxt { get; set; }
        public string calendarMessageTxt { get; set; }
        public string commentType { get; set; }
        public string spinOffQuestionnaire { get; set; }
        public int? spinOffQID { get; set; }
        public string emailAlert { get; set; }
        public string emailAlertList { get; set; }
        public string responseType { get; set; }
        //public string updated { get; set; }
        //public int? sortOrder { get; set; }
        //public bool active { get; set; }
        //public int? enterprise { get; set; }
        public List<QuestionnaireByAccessCodeModel_response> responses { get; set; }
        public string answer { get; set; }

    }
}