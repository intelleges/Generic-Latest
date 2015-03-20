using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic
{
    public partial class pr_getIteratePartnerPerson3_Result
    {
        public string NotesDescription
        {
            get
            {
                return notes.HasValue && notes.Value ? "Y" : "N";
            }
        }
    }
}