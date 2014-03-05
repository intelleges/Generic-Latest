using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Generic.Models
{
    public class LocalSignup
    {
        public RegisterModel User { get; set; }

        public UserContactModel UserContact { get; set; }

        public ProductTypeModel ProductType { get; set; }

        public BillingAddressModel UserAddress { get; set; }

        public BillingPaymentModel UserPayment { get; set; }

        [Display(Name = "Coupon Code")]
        public string CouponCode { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserContactModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email")]
        public string EmailAddress { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
    }

    public class BillingAddressModel
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Zip { get; set; }
    }

    public class BillingPaymentModel
    {
        [Required]
        [Display(Name = "Exp. Month")]
        public int ExpirationMonth { get; set; }

        [Required]
        [Display(Name = "Exp. Year")]
        public int ExpirationYear { get; set; }

        [Required]
        [Display(Name = "Card Number")]
        public string CardNumber { get; set; }

        [Required]
        public string CVV { get; set; }
    }
}