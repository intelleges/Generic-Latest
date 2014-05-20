using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public class PersonHelper
    {
        public enum PersonStatus
        {
            Loaded = 1,
            Spreadsheet_Cleansed = 2,
            Unconfirmed = 3,         
            Confirmed = 4,
            Invited = 5,
            Registered=6
        }
    }
}