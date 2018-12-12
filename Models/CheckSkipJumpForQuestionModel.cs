using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class CheckSkipJumpForQuestionModel
    {
        public string accessCode { get; set; }
        public string skipLogicJump { get; set; }
        public int questionId { get; set; }
    }
}