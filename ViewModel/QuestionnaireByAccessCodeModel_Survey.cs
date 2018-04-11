using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class QuestionnaireByAccessCodeModel_Survey
    {
        public string description { get; set; }
        public List<QuestionnaireByAccessCodeModel_question> questions { get; set; }
        public QuestionnaireByAccessCodeModel_Survey()
        {
            questions = new List<QuestionnaireByAccessCodeModel_question>();
        }
    }
}