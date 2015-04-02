using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public class GmailHelper
    {
        Google.Apis.Gmail.v1.GmailService service = new Google.Apis.Gmail.v1.GmailService();

        public void GetMessages(DateTime dateFrom, DateTime dateTo, string from)
        {
            
        }
    }
}