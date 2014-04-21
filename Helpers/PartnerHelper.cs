using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers.PartnerHelper
{
   
    public enum PartnerStatus
    {
        Loaded = 1,
        Spreadsheet_Cleansed = 2,
        Unconfirmed = 3,
        Hold = 4,
        Confirmed = 5,
        Invited_NoResponse = 6,
        Responded_Incomplete = 7,
        Responded_Complete = 8,
        No_Response = 9,
        Transferred = 10,
        Refusal = 11,
        Rejected_By_Manager = 12,
        Approved_By_Manager = 13
    }

}