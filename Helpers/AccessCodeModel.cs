using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public class AccessCodeModel
    {
        public string Ip { get; set; }
        public person Person { get; set; }
        public string ComputerName { get; set; }
        public string ReturnUrl { get; set; }
        public string AccessCode { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
    }
}