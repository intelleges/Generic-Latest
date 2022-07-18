using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic
{
    public partial class pr_getIteratePartnerPersonByPartnertypeGroupStatus_Result
    {
        public string NotesDescription
        {
            get
            {
                return Note1 > 0 ? "Y" : "N";
            }
        }
        public string NextActionDateDisplay
        {
            get
            {
                return nextActionDate.HasValue ? nextActionDate.Value.ToShortDateString() + " " + nextActionDate.Value.ToLongTimeString() : "";
            }
        }
        public string iteratePartnerStatus_Description
        {
            get;
            set;
        }
    }
}