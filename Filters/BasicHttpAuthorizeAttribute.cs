using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Http;

namespace Generic.Filters
{
	public class BasicHttpAuthorizeAttribute : System.Web.Http.AuthorizeAttribute
	{

		bool requireSsl = false;
		public bool RequireSsl
		{
			get { return requireSsl; }
			set { requireSsl = value; }
		}


		bool requireAuthentication = true;

		public bool RequireAuthentication
		{
			get { return requireAuthentication; }
			set { requireAuthentication = value; }
		}


		public override void OnAuthorization(System.Web.Http.Controllers.HttpActionContext actionContext)
		{

			if (Authenticate(actionContext) || !RequireAuthentication)
			{
				return;
			}
			else
			{
				HandleUnauthorizedRequest(actionContext);
			}
		}

		protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
		{
			var challengeMessage = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
			challengeMessage.Headers.Add("WWW-Authenticate", "Basic");
			throw new HttpResponseException(challengeMessage);
		}


		private bool Authenticate(System.Web.Http.Controllers.HttpActionContext actionContext) //HttpRequestMessage input)
		{
			if (RequireSsl && !HttpContext.Current.Request.IsSecureConnection && !HttpContext.Current.Request.IsLocal)
			{
				return false;
			}

			if (!HttpContext.Current.Request.Headers.AllKeys.Contains("Authorization")) return false;

			string authHeader = HttpContext.Current.Request.Headers["Authorization"];

			IPrincipal principal;
			if (TryGetPrincipal(authHeader, out principal))
			{
				HttpContext.Current.User = principal;
				return true;
			}
			return false;
		}


		private bool TryGetPrincipal(string authHeader, out IPrincipal principal)
		{
			var creds = ParseAuthHeader(authHeader);
			if (creds != null)
			{
				if (TryGetPrincipal(creds[0], creds[1], out principal)) return true;
			}

			principal = null;
			return false;
		}


		private string[] ParseAuthHeader(string authHeader)
		{
			// Check this is a Basic Auth header 
			if (authHeader == null || authHeader.Length == 0 || !authHeader.StartsWith("Basic")) return null;

			// Pull out the Credentials with are seperated by ':' and Base64 encoded 
			string base64Credentials = authHeader.Substring(6);
			string[] credentials = Encoding.ASCII.GetString(Convert.FromBase64String(base64Credentials)).Split(new char[] { ':' });

			if (credentials.Length != 2 || string.IsNullOrEmpty(credentials[0]) || string.IsNullOrEmpty(credentials[0])) return null;

			// Okay this is the credentials 
			return credentials;
		}


		private bool TryGetPrincipal(string username, string password, out IPrincipal principal)
		{
			EntitiesDBContext db = new EntitiesDBContext();
			person user = db.pr_doLogin(username, password).FirstOrDefault();

			if (user != null)
			{	
				principal = new GenericPrincipal(new GenericIdentity(user.email), new string[] { user.email });
				return true;
			}
			else
			{
				principal = null;
				return false;
			}
		}
	}
}