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

        public static int? EnterPriseId
        {
            get
            {
                return Convert.ToInt16(System.Web.HttpContext.Current.Session["EnterpriseId"]);
            }
            set
            {
                System.Web.HttpContext.Current.Session["EnterpriseId"] = value;
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

    }
}