using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.SessionClass
{
    public class SessionSingleton
    {
        public SessionSingleton()
        {
        }
        public static SessionSingleton _instance = new SessionSingleton();
        public static SessionSingleton Instance
        {
            get { return _instance; }
        }
        public bool IsSetFromClient
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["IsSetFromClient"] !=null) ?(bool)System.Web.HttpContext.Current.Session["IsSetFromClient"] :false; 
            }
            set
            { 
                System.Web.HttpContext.Current.Session["IsSetFromClient"] =value; 
            }
        }

        public static bool NeedAddEverNote
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["NeedAddEverNote"] != null) ? (bool)System.Web.HttpContext.Current.Session["NeedAddEverNote"] : false;
            }
            set
            {
                System.Web.HttpContext.Current.Session["NeedAddEverNote"] = value;
            }
        }

        public static int? EnterPriseId
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["NewEnterpriseId"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["NewEnterpriseId"] = value;
            }
        }

        public static int? MyEnterPriseId
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["MyEnterPriseId"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["MyEnterPriseId"] = value;
            }
        }

        public static int ProtocolId
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["ProtocolId"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["ProtocolId"] = value;
            }
        }

        public static int PersonId
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["PersonId"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["PersonId"] = value;
            }
        }

        public static int? LoggedInUserId
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["LoggedInUserId"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["LoggedInUserId"] = value;
            }
        }

        public static int? LoggedInUserRole
        {
            get
            {
                return Convert.ToInt32(System.Web.HttpContext.Current.Session["LoggedInUserRole"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["LoggedInUserRole"] = value;
            }
        }

        public static bool ShouldDisplayMenu
        {
            get
            {
                return LoggedInUserRole == 350 || LoggedInUserRole == 351;
            }
        }
        public static bool ShouldDisplayMenuBAA
        {
            get
            {
                if (Generic.Helpers.CurrentInstance.MultiTenantProjectType == 2)
                {
                    return LoggedInUserRole == 1383;
                }
                else return false;
            }
        }
        public static bool ShouldDisplayMenuBAAFull
        {
            get
            {
                return Generic.Helpers.CurrentInstance.MultiTenantProjectType == 2 && LoggedInUserRole == 1383 && Generic.Helpers.CurrentInstance.EnterpriseID == 2;
            }
        }

        public static int PTQ
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["PTQ"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["PTQ"] = value;
            }
        }

        public static int Touchpoint
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["touchpoint"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["touchpoint"] = value;
            }
        }

        public static string EnterpriseURL
        {
            get
            {
                return System.Web.HttpContext.Current.Session["EnterpriseURL"].ToString();
            }
            set
            {
                System.Web.HttpContext.Current.Session["EnterpriseURL"] = value;
            }
        }

        public static string EmailFromLinkedin
        {
            get
            {
                return System.Web.HttpContext.Current.Session["EmailFromLinkedin"].ToString();
            }
            set
            {
                System.Web.HttpContext.Current.Session["EmailFromLinkedin"] = value;
            }
        }

        public static string AccessToken
        {
            get
            {
                return System.Web.HttpContext.Current.Session["AccessToken"].ToString();
            }
            set
            {
                System.Web.HttpContext.Current.Session["AccessToken"] = value;
            }
        }

        public static string AccessSecretToken
        {
            get
            {
                return System.Web.HttpContext.Current.Session["AccessSecretToken"].ToString();
            }
            set
            {
                System.Web.HttpContext.Current.Session["AccessSecretToken"] = value;
            }
        }
    }
}