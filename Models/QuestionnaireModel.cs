using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class QuestionnaireModel
    {

       
        public List<QuestionnaireModel> lstQuesAns;
        public List<questionResponse> quesResponse = new List<questionResponse>();

        public string description { get; set; }
        public string question { get; set; }
       
    }

    
}