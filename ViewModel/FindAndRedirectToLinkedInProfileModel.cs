using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class FindAndRedirectToLinkedInProfileModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }

        public string GetSearchString()
        {
            return Name + " " + FirstName + " " + LastName;
        }
    }
}