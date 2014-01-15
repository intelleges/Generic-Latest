using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public class LayoutHelper
    {
        public static string GetLayout()
        {
            // Default to the generic layout 
            // Note the output will be in the tmp folder when published. 
            string layoutName ="~/tmp/Views/Shared/_GenericLayoutPage.cshtml";
            layoutName = "~/tmp/Views/Shared/_AdminLayout.cshtml";
            //if (1 == 2)// NOTE: Condition will be met. 
            //{
         //   layoutName = "~/Views/Shared/_GenericAdminLayout.cshtml";
            //}
            return layoutName;
        }
        public static string GetRegistrationLayout()
        {
            return "~/tmp/Views/Shared/_LayoutMasterPartner.cshtml";
        }
    }
}