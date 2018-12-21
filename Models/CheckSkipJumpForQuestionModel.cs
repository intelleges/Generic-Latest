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

    public class UpdateContactInfoViewModel {
        [Required]
        public string accessCode { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        public string title { get; set; }

        public string phone { get; set; }

        public string fax { get; set; }
    }

    public class UpdateCompanyInfoViewModel
    {
        [Required]
        public string accessCode { get; set; }

        public string name { get; set; }

        public string address1 { get; set; }

        public string address2 { get; set; }

        public string city { get; set; }

        public string zipcode { get; set; }
    }
}