using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers.Exceptions
{
    public class WrongSkipLogicJumpColumn: Exception
    {
        public WrongSkipLogicJumpColumn(string code):base("Wrong skip logic jump column. There is no any response with code = '" + code + "'")
        {
           
        }
    }
}