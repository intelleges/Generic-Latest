using Generic.Helpers;
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

        public static int? AddIteratePartnerId
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["AddIteratePartnerId"] != null) ? (int?)System.Web.HttpContext.Current.Session["AddIteratePartnerId"] : null;
            }
            set
            {
                System.Web.HttpContext.Current.Session["AddIteratePartnerId"] = value;
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

		public static bool IsSystemMaster
		{
			get
			{
				return Convert.ToBoolean(System.Web.HttpContext.Current.Session["IsSystemMaster"]);
			}
			set
			{
				System.Web.HttpContext.Current.Session["IsSystemMaster"] = value;
			}
		}

        public static bool ShouldDisplayMenu
        {
            get
            {
                return !IsSystemMaster;
            }
        }

        public static List<string> GridLevelMenuItems
        {
            get
            {
                return (List<string>)System.Web.HttpContext.Current.Session["GridLevelMenuItems"] ?? new List<string>();
            }
            set
            {
                System.Web.HttpContext.Current.Session["GridLevelMenuItems"] = value;
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

        public static bool NeedGetEvernoteText
        {
            get
            {
                return (System.Web.HttpContext.Current.Session["NeedGetEvernoteText"] != null) ? (bool)System.Web.HttpContext.Current.Session["NeedGetEvernoteText"] : false;
            }
            set
            {
                System.Web.HttpContext.Current.Session["NeedGetEvernoteText"] = value;
            }
        }

        public static AccessCodeModel AccessCodeModelValue
        {
            get
            {
                return (AccessCodeModel)System.Web.HttpContext.Current.Session["AccessCodeModel"] ?? new AccessCodeModel();
            }
            set
            {
                System.Web.HttpContext.Current.Session["AccessCodeModel"] = value;
            }
        }

        public static TempModel TempModelValue
        {
            get
            {
                return (TempModel)System.Web.HttpContext.Current.Session["TempModel"] ?? new TempModel();
            }
            set
            {
                System.Web.HttpContext.Current.Session["TempModel"] = value;
            }
        }
    }
}