using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Microsoft.Web.WebPages.OAuth;

namespace Generic
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            //OAuthWebSecurity.RegisterLinkedInClient(
            //    ConfigurationManager.AppSettings["apiKey"],
            //    ConfigurationManager.AppSettings["apiSecret"]
            //    );
            OAuthWebSecurity.RegisterLinkedInClient("7747cjm5yf3gbp", "SzdxJQqxWWonlMz5");
        }
    }
}