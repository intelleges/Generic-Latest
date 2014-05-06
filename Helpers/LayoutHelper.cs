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
           
            if (Generic.Helpers.CurrentInstance.IsGeneric == 1)
            {
                layoutName = "~/Views/Shared/_adminLayout.cshtml";
            }
            else
            {
                layoutName = "~/tmp/Views/Shared/_AdminLayout.cshtml";
            }

            return layoutName;
        }
        public static string GetRegistrationLayout()
        {
            if (Generic.Helpers.CurrentInstance.IsGeneric == 1)
            {
                return "~/Views/Shared/_LayoutMasterPartner.cshtml";
            }
            else
            {
                return "~/tmp/Views/Shared/_LayoutMasterPartner.cshtml";
            }
        }

        public static string GetMasterLayout()
        {
            if (Generic.Helpers.CurrentInstance.IsGeneric == 1)
            {
                return "~/Views/Shared/_LayoutMaster.cshtml";
            }
            else
            {
                return "~/tmp/Views/Shared/_LayoutMaster.cshtml";
            }
        }

        public static string GetPopupLayout()
        {
            if (Generic.Helpers.CurrentInstance.IsGeneric == 1)
            {
                return "~/Views/Shared/_PopupLayout.cshtml";
            }
            else
            {
                return "~/tmp/Views/Shared/_PopupLayout.cshtml";
            }
        }


       


    }
}