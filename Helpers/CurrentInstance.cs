
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public static class CurrentInstance
    {
        public static int EnterpriseID
        {
            get
            {
                if (System.Web.HttpContext.Current.Session["EnterpriseId"] == null)
                {
                     System.Web.HttpContext.Current.Response.Redirect("~/Admin/Index");
                }


                return Convert.ToInt16(System.Web.HttpContext.Current.Session["EnterpriseId"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["EnterpriseId"] = value;
            }
        }


        public static int MultiTenantProjectType = 1;

        public static int IsGeneric = 0;

    }
}