using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class QuestionnaireByAccessCodeModel_SurveySet
    {
        public string description { get; set; }
        public List<QuestionnaireByAccessCodeModel_Survey> surveys { get; set; }
        public QuestionnaireByAccessCodeModel_SurveySet()
        {
            surveys = new List<QuestionnaireByAccessCodeModel_Survey>();
        }
    }
}