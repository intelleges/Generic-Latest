using Generic.SessionClass;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Mvc;
using Google.Apis.Gmail.v1;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Session
{
    
    public class AppFlowMetadata:FlowMetadata
    {
       
        private static readonly IAuthorizationCodeFlow flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets
                {
#if GOOGLE_MAIL_TEST
                    ClientId = "120860466613-vqrcsq84le24s8q5en4g10eauke25qkl.apps.googleusercontent.com",
                    ClientSecret = "byJ8P88Bb1FeRFXVvBvka13I"
#else 
                    ClientId = "120860466613-n6euab0n23g3ommaqb6luu0r1405etrt.apps.googleusercontent.com",
                    ClientSecret = "wCnMK0SbVKHxpuv0fWmee4ug"
#endif
                },
                Scopes = new[] { GmailService.Scope.GmailReadonly },
                DataStore = new FileDataStore(System.Web.HttpContext.Current.Server.MapPath("/App_Data/MyGoogleStorage")) 
            });
        public override Google.Apis.Auth.OAuth2.Flows.IAuthorizationCodeFlow Flow
        {
            get { return flow; }
        }

        public override string GetUserId(System.Web.Mvc.Controller controller)
        {           
            return controller.Session["LoggedInUserId"].ToString();
        }
    }
}