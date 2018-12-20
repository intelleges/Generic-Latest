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

        [Required]
        public string firstName { get; set; }

        [Required]
        public string lastName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string email { get; set; }

        [Required]
        public string title { get; set; }

        [Required]
        public string phone { get; set; }

        [Required]
        public string fax { get; set; }
    }

    public class UpdateCompanyInfoViewModel
    {
        [Required]
        public string accessCode { get; set; }

        [Required]
        public string name { get; set; }

        [Required]
        public string address1 { get; set; }

        [Required]
        public string address2 { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string zipcode { get; set; }
    }
}