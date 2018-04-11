using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class QuestionnaireByAccessCodeModel
    {
        public string accessCode { get; set; }
        public string status { get; set; }
        public QuestionnaireByAccessCodeModel_Questionnaire questionnaire { get; set; }
    }
}