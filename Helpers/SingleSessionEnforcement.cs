using Generic;
using System;
using System.Web;
using System.Web.Security;
using System.Linq;


namespace IntellegesWebsite
{
    /// <summary>
    /// Enforces a single login session
    /// Needs an entry in Web.Config, exactly where depends on the version of IIS, but you
    /// can safely put it in both places.
    /// 1:
    ///  <system.web>
    ///     <httpModules>
    ///      <add name="SingleSessionEnforcement" type="SingleSessionEnforcement" />
    ///    </httpModules>
    ///  </system.web>
    /// 2:
    ///  <system.webServer>
    ///    <modules runAllManagedModulesForAllRequests="true">
    ///      <add name="SingleSessionEnforcement" type="SingleSessionEnforcement" />
    ///    </modules>
    ///  </system.webServer>
    /// Also, slidingExpiration for the forms must be set to false, also set a
    /// suitable timeout period (in minutes)
    ///  <authentication mode="Forms">
    ///   <forms protection="All" slidingExpiration="false" loginUrl="login.aspx" timeout="600" />
    ///  </authentication>
    /// </summary>
    public class SingleSessionEnforcement : IHttpModule
    {

        public SingleSessionEnforcement()
        {
            // No construction needed
        }

        private void OnPostAuthenticate(Object sender, EventArgs e)
        {
            EntitiesDBContext db = new EntitiesDBContext();

            HttpApplication httpApplication = (HttpApplication)sender;
            HttpContext httpContext = httpApplication.Context;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                var ip = httpContext.Request.UserHostAddress;
                var person = db.pr_getPersonByEmail2(httpContext.User.Identity.Name);
                string last_ip = "";
                if (person != null && person.Count() > 0)
                {
                    last_ip = person.First().address2;
                }

                if (ip != last_ip)
                {
                    FormsAuthentication.SignOut();
                    FormsAuthentication.RedirectToLoginPage();
                }
            }
        }

        public void Dispose()
        {
            // Nothing to dispose
        }

        public void Init(HttpApplication context)
        {
            context.PostAuthenticateRequest += new EventHandler(OnPostAuthenticate);
        }
    }
}