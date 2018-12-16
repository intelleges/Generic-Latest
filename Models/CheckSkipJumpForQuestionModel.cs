using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

    public class LoginViewModel
    {
        [DataType(DataType.EmailAddress)]
        [Required]
        public string email { get; set; }
        [Required]
        public string password { get; set; }       
    }
}