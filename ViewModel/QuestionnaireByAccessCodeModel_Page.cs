using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class QuestionnaireByAccessCodeModel_Page
    {
        public string description { get; set; }
        public List<QuestionnaireByAccessCodeModel_SurveySet> surveySets { get; set; }
        public QuestionnaireByAccessCodeModel_Page()
        {
            surveySets = new List<QuestionnaireByAccessCodeModel_SurveySet>();
        }
    }
}