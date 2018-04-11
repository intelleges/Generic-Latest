using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class QuestionnaireByAccessCodeModel_Questionnaire
    {
        public string footer { get; set; }
        public int? multiLanguage { get; set; }
        public List<QuestionnaireByAccessCodeModel_Page> pages { get; set; }
        public QuestionnaireByAccessCodeModel_Questionnaire()
        {
            pages = new List<QuestionnaireByAccessCodeModel_Page>();
        }
    }
}