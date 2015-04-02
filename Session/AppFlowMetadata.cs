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
                    ClientId = "120860466613-n6euab0n23g3ommaqb6luu0r1405etrt.apps.googleusercontent.com",
                    ClientSecret = "wCnMK0SbVKHxpuv0fWmee4ug"
                },
                Scopes = new[] { GmailService.Scope.GmailReadonly },
                DataStore = new FileDataStore("Drive.Api.Auth.Store") 
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