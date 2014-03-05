using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class ProductTypeModel
    {
        public int userCount { get; set; }
        public int partnerCount { get; set; }
        public int partnumberCount { get; set; }
        public int SubscriptionType { get; set; }
        public int? CalculatedCost { get; set; }
        public LocalSignup Signup { get; set; }
    }
}