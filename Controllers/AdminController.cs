using Generic.Helpers;
//using Generic.Helpers;
using Generic.Helpers.Utility;
using Generic.Models;
using Generic.SessionClass;
using Generic.ViewModel;
using SendGrid;
using SendGrid.Helpers.Mail;
//using Sustainsys.Saml2.Mvc;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Telerik.Web.Mvc;

namespace Generic.Controllers
{

    public class AdminController : Controller
    {
        protected EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /Admin/


        public CustomMembershipProvider MembershipService { get; set; }
        public CustomRoleProvider AuthorizationService { get; set; }


        protected override void Initialize(RequestContext requestContext)
        {


            if (MembershipService == null)
                MembershipService = new CustomMembershipProvider();
            if (AuthorizationService == null)
                AuthorizationService = new CustomRoleProvider();

            base.Initialize(requestContext);
        }

        /// <summary>
        /// Login Form
        /// </summary>
        /// <returns></returns>
        public virtual ActionResult Index(int? contactUs = 1, string returnUrl = null)
        {
            try
            {
                if (Request.IsAuthenticated)
                {
                    string userName = "";
                    var claims = ((ClaimsIdentity)User.Identity).Claims.ToList();
                    userName = claims.Where(o => o.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").First().Value;

                    ((ClaimsIdentity)User.Identity).AddClaim(new Claim(ClaimTypes.NameIdentifier, userName));
                    ((ClaimsIdentity)User.Identity).AddClaim(new Claim(ClaimTypes.Name, userName));

                    FormsAuthentication.SetAuthCookie(userName, false);
                    person person = db.sp_getPersonforSAML(userName).FirstOrDefault();
                    if (person == null) return View();

                    //var ip = "71.225.253.65";// Request.UserHostAddress;
                    var ip = Request.UserHostAddress;
                    var computerName = "";//computer_name[0].ToString();
                                          //string[] computer_name = { ip };

                    var personLoginAudit = db.pr_getPersonLoginAuditAll()
                        .Where(x => x.person == person.id)
                        .OrderByDescending(x => x.timestamp)
                        .FirstOrDefault();

                    try
                    {
                        var ipData = GetLocationByIp(ip);
                        computerName = ipData?.country_name + "," + ipData?.region_name + "," + ipData?.city + "," + ipData?.zip + "," + ipData?.hostname;
                    }
                    catch (SocketException ex)
                    {
                        //if can't resolve remote host then set up IP address
                    }
                    string ecn = System.Environment.MachineName;
                    var res = db.pr_modifyPersonLastLoginDate(person.id, DateTime.Now, string.Format("{0}:{1}", ip, computerName));
                    SessionSingleton.LoggedInUserId = person.id;
                    SessionSingleton.LoggedInUserRole = db.pr_getPersonRoleByPerson(person.id).FirstOrDefault().role;
                    SessionSingleton.IsSystemMaster = db.pr_isSystemMaster(person.id).First() == 1 ? true : false;
                    SessionSingleton.MyEnterPriseId = person.enterprise;
                    SessionSingleton.Touchpoint = (int)person.campaign;


                    try
                    {
                        SessionSingleton.EnterpriseURL = db.pr_getEnterpriseSystemInfo(person.enterprise).FirstOrDefault().companyWebSite;
                    }
                    catch
                    {
                        SessionSingleton.EnterpriseURL = "#";
                    }
                    Generic.Helpers.CurrentInstance.EnterpriseID = int.Parse(person.enterprise.ToString());
                    return RedirectToAction("Home", "Admin");
                }

                ViewBag.useReCaptcha = !Request?.Url?.Host?.ToLower()?.StartsWith("localhost");
                ViewBag.returnUrl = returnUrl;
                var enterprises = db.pr_getEnterprise(contactUs);
                //var elpptq = db.pr_getEnterpriseLandingPagePTQ(contactUs, (int)LangingPage.Login).FirstOrDefault();
                //if (elpptq != null)
                //    ViewBag.ContactUsText = elpptq.text;
                ViewBag.Project = "Generic";
                ViewBag.LinkedInLoginUri = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ExternalLogin", "Admin");
                ViewBag.EnterpriseId = contactUs;

                if (null != Session["REDIRECT_BY_EMAIL"] && Convert.ToInt16(Session["REDIRECT_BY_EMAIL"]) == -1)
                {
                    ViewBag.REDIRECT_BY_EMAIL = 1;
                }
                else { ViewBag.REDIRECT_BY_EMAIL = 0; }
                return View(enterprises.FirstOrDefault());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View();
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual string GetForgotPassword(string email)
        {
            var resultString = "";
            var result = db.pr_getPersonByEmailForLoginAttempts(email).FirstOrDefault();
            if (result == null)
                resultString = email + " is invalid and no longer has access to that Intelleges account. Please contact your Account Administrator to activate an expired account or to register a new one.";
            else
            {
                SchedulerServiceHelper.SendPassword(result, email);
            }
            return resultString;
        }

        [HttpPost]
        public virtual string ValidatePassword(string password)
        {
            var resultString = "";
            var person = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();

            var validation = db.pr_validatePasswordByEmail(person.email, password).FirstOrDefault();
            if (!validation.HasValue || (validation.HasValue && validation.Value == 0))
                resultString = "Invalid password";
            return resultString;
        }

        [HttpPost]
        public virtual ActionResult ChangePassword(string currentPassword, string newPassword, string confirmPassword)
        {
            var resultString = "done";
            var person = db.pr_getPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
            var validation = db.pr_modifyPasswordByEmailAndPassword(person.email, currentPassword, newPassword).FirstOrDefault();
            if (!validation.HasValue || (validation.HasValue && validation.Value == 0))
                resultString = "Invalid password";
            else SchedulerServiceHelper.SendPasswordChangedNotification(person, person.email);
            ViewBag.message = resultString;
            return View();
        }

        [HttpGet]
        public virtual ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {

            var credentials = new Hammock.Authentication.OAuth.OAuthCredentials
            {
                CallbackUrl = "https://intelleges.com/mvcmt/Generic/Admin/ExternalLoginCallback",
                //CallbackUrl = "http://localhost:51090/Admin/ExternalLoginCallback",
                ConsumerKey = ConfigurationManager.AppSettings["LinkedInConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["LinkedInConsumerSecret"],
                Type = Hammock.Authentication.OAuth.OAuthType.RequestToken
            };

            var client = new Hammock.RestClient
            {
                Authority = "https://api.linkedin.com/uas/oauth",
                Credentials = credentials
            };

            var request = new Hammock.RestRequest
            {
                Path = "requestToken"
            };

            Hammock.RestResponse response = client.Request(request);

            String[] strResponseAttributes = response.Content.Split('&');

            string token = strResponseAttributes[0].Substring(strResponseAttributes[0].LastIndexOf('=') + 1);
            string authToken = strResponseAttributes[1].Substring(strResponseAttributes[1].LastIndexOf('=') + 1);

            // save tokens to db. initial, requests tokens are saved.
            db.pr_addPersonLinkedinAuthInfo(token, authToken, string.Empty, 3);

            Response.Redirect("https://www.linkedin.com/uas/oauth/authorize?oauth_token=" + token, false);
            return null;
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            String verifier = string.Empty,
                   tokenFronUrl = string.Empty;
            try
            {
                tokenFronUrl = Request.QueryString["oauth_token"].ToString();
                verifier = Request.QueryString["oauth_verifier"].ToString();
            }
            catch
            {
                try
                {
                    db.pr_removePersonLinkedinAuthInfo(db.pr_getPersonLinkedinAuthInfoByToken(tokenFronUrl).FirstOrDefault().id);
                }
                catch { }

                return RedirectToAction("Index");
            }

            // get information about auth info by token from linkedin
            var fullLinkedinAuthInfo = db.pr_getPersonLinkedinAuthInfoByToken(tokenFronUrl).FirstOrDefault();

            var credentials = new Hammock.Authentication.OAuth.OAuthCredentials
            {
                ConsumerKey = ConfigurationManager.AppSettings["LinkedInConsumerKey"],
                ConsumerSecret = ConfigurationManager.AppSettings["LinkedInConsumerSecret"],
                Token = fullLinkedinAuthInfo.token,
                TokenSecret = fullLinkedinAuthInfo.tokenSecret,
                Verifier = verifier,
                Type = Hammock.Authentication.OAuth.OAuthType.AccessToken,
                ParameterHandling = Hammock.Authentication.OAuth.OAuthParameterHandling.HttpAuthorizationHeader,
                SignatureMethod = Hammock.Authentication.OAuth.OAuthSignatureMethod.HmacSha1,
                Version = "1.0"
            };

            var client = new Hammock.RestClient
            {
                Authority = "https://api.linkedin.com/uas/oauth",
                Credentials = credentials,
                Method = Hammock.Web.WebMethod.Post
            };

            var request = new Hammock.RestRequest
            {
                Path = "accessToken"
            };

            Hammock.RestResponse response = client.Request(request);
            String[] strResponseAttributes = response.Content.Split('&');

            string accessToken = strResponseAttributes[0].Substring(strResponseAttributes[0].LastIndexOf('=') + 1);
            string accessTokenSecret = strResponseAttributes[1].Substring(strResponseAttributes[1].LastIndexOf('=') + 1);

            var hammockRequest = new Hammock.RestRequest
            {
                Path = "~:(id,first-name,last-name,headline,member-url-resources,picture-url,location,public-profile-url,email-address)"
            };

            var hammockCredentials = new Hammock.Authentication.OAuth.OAuthCredentials
            {
                Type = Hammock.Authentication.OAuth.OAuthType.AccessToken,
                SignatureMethod = Hammock.Authentication.OAuth.OAuthSignatureMethod.HmacSha1,
                ParameterHandling = Hammock.Authentication.OAuth.OAuthParameterHandling.HttpAuthorizationHeader,
                ConsumerKey = "7747cjm5yf3gbp",
                ConsumerSecret = "SzdxJQqxWWonlMz5",
                Token = accessToken,
                TokenSecret = accessTokenSecret,
                Verifier = verifier
            };

            var hammockClient = new Hammock.RestClient()
            {
                Authority = "https://api.linkedin.com/v1/people/",
                Credentials = hammockCredentials,
                Method = Hammock.Web.WebMethod.Get
            };

            var MyInfo = hammockClient.Request(hammockRequest);

            var person = from c in System.Xml.Linq.XElement.Parse(MyInfo.Content.ToString()).Elements() select c;

            string email = person.SingleOrDefault(e => e.Name == "email-address").Value;

            if (!string.IsNullOrEmpty(email))
            {
                List<Generic.enterprise> result = db.pr_getEnterpriseByEmail(email).ToList();

                if (!result.Any())
                {
                    var message = "Thank you for your interest in Intelleges. You currently do not have an Intelleges account, and right now LinkedIn members are by INVITATION ONLY. We are happy to add you to our WAITING LIST and send you an invite if this policy changes in the future. Please click Yes if you would like to be added. If you don &apos;t want to be added, click No. Thank you.";
                    TempData["LinkedInMessage"] = new KeyValuePair<string, string>("promptYesNo", message);
                }
                else if (result.Any())
                {
                    person resultPerson = db.pr_getPersonByEmail(result[0].id, email).FirstOrDefault();

                    if (resultPerson != null)
                    {
                        // check if we have already row it db for current user
                        // if yes, remove currently created row and update prev row with new info
                        var existingLinkedinInfo = db.pr_getPersonLinkedinAuthInfoByPerson(resultPerson.id).ToArray();

                        foreach (var linkedinAuthInfoItem in existingLinkedinInfo)
                        {
                            db.pr_removePersonLinkedinAuthInfo(linkedinAuthInfoItem.id);
                        }

                        if (resultPerson.id == 3)
                        {
                            // default id. on prev step removed all his rows from db. add new one
                            db.pr_addPersonLinkedinAuthInfo(accessToken, accessTokenSecret, verifier, resultPerson.id);
                        }
                        else
                        {
                            // standard scenario. modify current row.
                            db.pr_modifyPersonLinkedinAuthInfo(fullLinkedinAuthInfo.id, accessToken, accessTokenSecret, verifier, resultPerson.id);
                        }

                        if (result.Count() > 1)
                        {
                            TempData["LinkedInMessage"] = new KeyValuePair<string, string>("promptDropDown", "Please select Enterprise");
                            TempData["Enterprises"] = result;
                            TempData["Email"] = email;
                        }
                        else
                        {
                            FormsAuthentication.SetAuthCookie(email, false);

                            SessionSingleton.LoggedInUserId = resultPerson.id;
                            SessionSingleton.LoggedInUserRole = db.pr_getPersonRoleByPerson(resultPerson.id).FirstOrDefault().role;
                            SessionSingleton.IsSystemMaster = db.pr_isSystemMaster(resultPerson.id).First() == 1 ? true : false;
                            SessionSingleton.MyEnterPriseId = resultPerson.enterprise;
                            SessionSingleton.Touchpoint = (int)resultPerson.campaign;

                            try
                            {
                                SessionSingleton.EnterpriseURL = db.pr_getEnterpriseSystemInfo(resultPerson.enterprise).FirstOrDefault().companyWebSite;
                            }
                            catch
                            {
                                SessionSingleton.EnterpriseURL = "#";
                            }
                            Generic.Helpers.CurrentInstance.EnterpriseID = int.Parse(resultPerson.enterprise.ToString());

                            if (resultPerson.personStatus == (int)PersonHelper.PersonStatus.Invited)
                            {
                                return RedirectToAction("ResetPassword", "Person");
                            }
                            else
                            {
                                return RedirectToAction("Home", "Admin");
                            }
                        }
                    }
                    else
                    {
                        db.pr_removePersonLinkedinAuthInfo(fullLinkedinAuthInfo.id);

                        return RedirectToAction("Index");
                    }
                }
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public JsonResult AddEmailToWaitingList()
        {
            // get email from session and add user to queue

            var email = SessionSingleton.EmailFromLinkedin;

            if (!string.IsNullOrEmpty(email))
            {
                // add to queue
            }

            return Json(true, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult LoginByEnterprise(string enterpriseid, string email)
        {
            if (!string.IsNullOrEmpty(enterpriseid))
            {
                person resultPerson = db.pr_getPersonByEmail(Convert.ToInt32(enterpriseid), email).FirstOrDefault();

                FormsAuthentication.SetAuthCookie(email, false);

                SessionSingleton.LoggedInUserId = resultPerson.id;
                SessionSingleton.LoggedInUserRole = db.pr_getPersonRoleByPerson(resultPerson.id).FirstOrDefault().role;
                SessionSingleton.IsSystemMaster = db.pr_isSystemMaster(resultPerson.id).First() == 1 ? true : false;
                SessionSingleton.MyEnterPriseId = resultPerson.enterprise;
                SessionSingleton.Touchpoint = (int)resultPerson.campaign;

                try
                {
                    SessionSingleton.EnterpriseURL = db.pr_getEnterpriseSystemInfo(resultPerson.enterprise).FirstOrDefault().companyWebSite;
                }
                catch
                {
                    SessionSingleton.EnterpriseURL = "#";
                }
                Generic.Helpers.CurrentInstance.EnterpriseID = int.Parse(resultPerson.enterprise.ToString());

                if (resultPerson.personStatus == (int)PersonHelper.PersonStatus.Invited)
                {
                    return RedirectToAction("ResetPassword", "Person");
                }
                else
                {
                    return RedirectToAction("Home", "Admin");
                }
            }

            return RedirectToAction("Index", "Admin");
        }
        private class LocationModelByIp
        {
            public string city { get; set; }
            public string country_name { get; set; }
            public string country_code { get; set; }
            public string region_name { get; set; }
            public string region_code { get; set; }
            public string zip { get; set; }
            public string hostname { get; set; }
            public string continent_code { get; set; }
            public string continent_name { get; set; }
            public string type { get; set; }
            public dynamic security { get; set; }

        }

        private LocationModelByIp GetLocationByIp(string ip)
        {
            RestSharp.RestClient client = new RestSharp.RestClient("https://api.ipstack.com/");
            RestSharp.RestRequest restRequest = new RestSharp.RestRequest(ip, RestSharp.Method.GET);
            restRequest.AddQueryParameter("access_key", "8f578ab0f32617fe27ce82424db2ee3e");
            restRequest.AddQueryParameter("hostname", "1");
            restRequest.AddQueryParameter("fields", "city,country_name,country_code,region_name,region_code,zip,hostname,continent_code,continent_name,type,security.proxy_type");
            var response = client.Execute<LocationModelByIp>(restRequest);
            return response.Data;
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Index(string userName, string password, string returnUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(password))
                {
                    var person = db.sp_getPersonforSAML(userName).FirstOrDefault();
                    if (person != null)
                        return RedirectToAction("signin", "saml2");
                }

                if (ModelState.IsValid)
                {
                    if (null != Session["REDIRECT_BY_EMAIL"] && Convert.ToInt16(Session["REDIRECT_BY_EMAIL"]) == -1)
                    {
                        var result = db.pr_validatePerson(userName, password).FirstOrDefault();
                        Session["REDIRECT_BY_EMAIL"] = result == 1 ? 1 : -2;
                        Session["REDIRECT_BY_EMAIL_APPROVAL"] = userName;
                        dynamic queryStringData = Session["REDIRECT_BY_EMAIL_Query"];
                        return RedirectToAction("QuestionnaireDetailQuestion", "Questionnaire", new { id = queryStringData.id, pptqId = queryStringData.pptqId, questionId = queryStringData.questionId, partnerId = queryStringData.partnerId, responseId = queryStringData.responseId, email = queryStringData.email });
                    }
                    else
                    {
                        //  CustomMembershipProvider MembershipService = new CustomMembershipProvider();
                        if (MembershipService.ValidateUser(userName, password))
                        {
                            FormsAuthentication.SetAuthCookie(userName, false);

                            person person = db.pr_doLogin(userName, password).FirstOrDefault();

                            //var ip = "71.225.253.65";// Request.UserHostAddress;
                            var ip = Request.UserHostAddress;
                            var computerName = "";//computer_name[0].ToString();
                                                  //string[] computer_name = { ip };

                            var personLoginAudit = db.pr_getPersonLoginAuditAll()
                                .Where(x => x.person == person.id)
                                .OrderByDescending(x => x.timestamp)
                                .FirstOrDefault();

                            //if (personLoginAudit != null && personLoginAudit.auditLoginEvent == 1 && personLoginAudit.deviceIP != ip)
                            //{
                            //    // Show screen to Logout from previous session.
                            //    var message = @"You are already logged in at ip address: "+ personLoginAudit.deviceIP + ". Would you like to end that session?";
                            //    var tempModel = new TempModel
                            //    {
                            //        Message = message,
                            //        PersonId = person.id
                            //    };
                            //    SessionSingleton.TempModelValue = tempModel;

                            //    return RedirectToAction("DifferentIPLogout");
                            //}
                            try
                            {
                                var ipData = GetLocationByIp(ip);
                                computerName = ipData?.country_name + "," + ipData?.region_name + "," + ipData?.city + "," + ipData?.zip + "," + ipData?.hostname;//System.Net.Dns.GetHostEntry(Request.ServerVariables["remote_addr"]).HostName.Split(new Char[] { '.' });
                                                                                                                                                                  //var countryCode = db.pr_getCountryByName("United States").FirstOrDefault();
                                                                                                                                                                  //int countryId = 1;
                                                                                                                                                                  //if (ipData != null)
                                                                                                                                                                  //{
                                                                                                                                                                  //    if (ipData.country_code.Length == 2)
                                                                                                                                                                  //    {
                                                                                                                                                                  //        var countryCode = db.pr_getCountryByCode(ipData?.country_code).FirstOrDefault();
                                                                                                                                                                  //        countryId = countryCode != null ? (countryCode.id > 0 ? countryCode.id : 1) : 1;
                                                                                                                                                                  //    }
                                                                                                                                                                  //    else if (ipData.country_code.Length == 3)
                                                                                                                                                                  //    {
                                                                                                                                                                  //        var countryCode = db.pr_getCountryByGovernanceCode(ipData?.country_code).FirstOrDefault();
                                                                                                                                                                  //        countryId = countryCode != null ? (countryCode.id > 0 ? countryCode.id : 1) : 1;
                                                                                                                                                                  //    }
                                                                                                                                                                  //    else
                                                                                                                                                                  //    {
                                                                                                                                                                  //        var countryCode = db.pr_getCountryByName(ipData?.country_name).FirstOrDefault();
                                                                                                                                                                  //        countryId = countryCode != null ? (countryCode.id > 0 ? countryCode.id : 1) : 1;
                                                                                                                                                                  //    }
                                                                                                                                                                  //}

                                //var personEmail = db.pr_getPersonByEmail2(userName).FirstOrDefault();
                                //if (personEmail != null)
                                //{
                                //    db.pr_addPersonLoginLog(person.id, countryId, ipData?.hostname, ipData?.region_code, ipData?.city, ipData?.zip, ip, ((int)LoginStatus.Successful_Login).ToString(), DateTime.Now, 1, true);
                                //    db.SaveChanges();
                                //}
                                //var proxy_type = ipData.security?.proxy_type?.ToString() ?? "";
                                //db.pr_addPersonLoginLog(person.id, ip, ipData.type, ipData.continent_code, ipData.continent_name, ipData.country_code, ipData.region_code, ipData.region_name, ipData.city, ipData.zip, ipData.hostname,
                                //    proxy_type, DateTime.Now,1,true);

                                //#region Checking MFA
                                //// Checking host name validation
                                //var hostName = ipData.hostname;
                                //var companyName = db.pr_getEnterprise(person.enterprise)
                                //    .Select(x => x.companyName).FirstOrDefault();
                                //if (
                                //    string.IsNullOrWhiteSpace(hostName) || 
                                //    hostName.IndexOf(companyName, StringComparison.OrdinalIgnoreCase) == -1)
                                //{
                                //    var message = $"You are logging in from a location outside of {companyName}, please get the security code sent to your email address and enter it below.";
                                //    return SendAccessCode(person, ip,
                                //        computerName, returnUrl, userName, message);
                                //}

                                //// Validating country code blocked

                                //var countryBlockedList = db.pr_getEnterpriseCountryBlockAll(enterprise: person.enterprise)
                                //    .Select(x => x.country).ToList();

                                //foreach (var country in countryBlockedList)
                                //{
                                //    var isCountryCodeBlocked = db.pr_getCountry(country)
                                //        .Any(x => x.code == ipData.country_code);

                                //    if (isCountryCodeBlocked)
                                //    {
                                //        var message = @"You are logging in from blocked location, Please get the security code sent to your email address and enter it below.";
                                //        return SendAccessCode(person, ip,
                                //            computerName, returnUrl, userName, message);
                                //    }
                                //}
                                //#endregion



                            }
                            catch (SocketException ex)
                            {
                                //if can't resolve remote host then set up IP address
                            }
                            String ecn = System.Environment.MachineName;

                            var res = db.pr_modifyPersonLastLoginDate(person.id, DateTime.Now, string.Format("{0}:{1}", ip, computerName));

                            if (res == null)
                            {
                                //  ModelState.AddModelError("Update Error", "You failed to update login date & time");
                                ViewBag.Message = "You failed to update login date & time";
                            }

                            //var loginAudit = db.pr_addPersonLoginAudit(person.id, 1, DateTime.Now, ip, 1, true);

                            SessionSingleton.LoggedInUserId = person.id;
                            SessionSingleton.LoggedInUserRole = db.pr_getPersonRoleByPerson(person.id).FirstOrDefault().role;
                            SessionSingleton.IsSystemMaster = db.pr_isSystemMaster(person.id).First() == 1 ? true : false;
                            SessionSingleton.MyEnterPriseId = person.enterprise;
                            SessionSingleton.Touchpoint = (int)person.campaign;


                            try
                            {
                                SessionSingleton.EnterpriseURL = db.pr_getEnterpriseSystemInfo(person.enterprise).FirstOrDefault().companyWebSite;
                            }
                            catch
                            {
                                SessionSingleton.EnterpriseURL = "#";
                            }
                            Generic.Helpers.CurrentInstance.EnterpriseID = int.Parse(person.enterprise.ToString());
                            if (Url.IsLocalUrl(returnUrl))
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                if (person.personStatus == (int)PersonHelper.PersonStatus.Invited)
                                {
                                    return RedirectToAction("ResetPassword", "Person");
                                }
                                else
                                {
                                    return RedirectToAction("Home", "Admin");
                                }
                            }

                            //}
                        }
                        else
                        {
                            var ip = Request.UserHostAddress;
                            var ipData = GetLocationByIp(ip);
                            if ("::1" == ip) ipData = null;
                            var computerName = ipData?.country_name + "," + ipData?.region_name + "," + ipData?.city + "," + ipData?.zip + "," + ipData?.hostname;
                            //var countryCode = db.pr_getCountryByName("United States").FirstOrDefault();
                            int countryId = 1;
                            if (ipData != null)
                            {
                                if (ipData.country_code.Length == 2)
                                {
                                    var countryCode = db.pr_getCountryByCode(ipData?.country_code).FirstOrDefault();
                                    countryId = countryCode != null ? (countryCode.id > 0 ? countryCode.id : 1) : 1;
                                }
                                else if (ipData.country_code.Length == 3)
                                {
                                    var countryCode = db.pr_getCountryByGovernanceCode(ipData?.country_code).FirstOrDefault();
                                    countryId = countryCode != null ? (countryCode.id > 0 ? countryCode.id : 1) : 1;
                                }
                                else
                                {
                                    var countryCode = db.pr_getCountryByName(ipData?.country_name).FirstOrDefault();
                                    countryId = countryCode != null ? (countryCode.id > 0 ? countryCode.id : 1) : 1;
                                }
                            }

                            var person = db.pr_getPersonByEmail2(userName).FirstOrDefault();
                            if (person != null)
                            {
                                db.pr_addPersonLoginLog(person.id, countryId, ipData?.hostname, ipData?.region_code, ipData?.city, ipData?.zip, ip, ((int)LoginStatus.Failed_Password_Valid_Email).ToString(), DateTime.Now, 1, true);
                            }
                            else
                            {
                                db.pr_addPersonLoginLog(5, countryId, ipData?.hostname, ipData?.region_code, ipData?.city, ipData?.zip, ip, ((int)LoginStatus.Failed_Login_Invalid_Email).ToString(), DateTime.Now, 1, true);
                            }
                            ModelState.AddModelError("LoginFailed", "The user name or password provided is incorrect.");
                        }
                    }
                }
            }
            catch { }
            var enterprises = db.pr_getEnterprise(1);
            ViewBag.EnterpriseId = 1;
            // If we got this far, something failed, redisplay form
            return View(enterprises.FirstOrDefault());
        }

        private static personLoginAudit GetLastActiveLoginActivity(string sessionId)
        {
            return null;
        }

        public ActionResult SendAccessCode(person person, string ip,
            string computerName, string returnUrl,
            string userName, string message)
        {
            var master = db.pr_getSystemMaster(person.enterprise).FirstOrDefault();
            var accessCode = db.pr_getAccesscode().FirstOrDefault();

            var result = SendAccessCode(accessCode,
                "Intelleges Login Access Code : " + accessCode,
                master: master, person: person);

            var model = new AccessCodeModel
            {
                Ip = ip,
                Person = person,
                ComputerName = computerName,
                ReturnUrl = returnUrl,
                AccessCode = accessCode,
                UserName = userName,
                Message = message
            };
            SessionSingleton.AccessCodeModelValue = model;
            //return AccessCodeValidate();
            return RedirectToAction("AccessCodeValidate");

        }

        public ActionResult AccessCodeValidate()
        {
            var accessCodeModel = SessionSingleton.AccessCodeModelValue;

            ViewBag.Message = accessCodeModel.Message;

            return View();
        }


        public ActionResult DifferentIPLogout()
        {
            var tempModel = SessionSingleton.TempModelValue;
            ViewBag.Message = tempModel.Message;
            ViewBag.PersonId = tempModel.PersonId;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult DifferentIPLogout(int? personId = null)
        {
            return null;
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult AccessCodeValidate(string securitycode)
        {
            var accessCodeModel = SessionSingleton.AccessCodeModelValue;
            if (accessCodeModel.AccessCode.Equals(securitycode))
            {
                FormsAuthentication.SetAuthCookie(accessCodeModel.UserName, false);

                var ip = accessCodeModel.Ip;
                var person = accessCodeModel.Person;
                var computerName = accessCodeModel.ComputerName;
                var returnUrl = accessCodeModel.ReturnUrl;

                String ecn = System.Environment.MachineName;

                var res = db.pr_modifyPersonLastLoginDate(person.id, DateTime.Now, string.Format("{0}:{1}", ip, computerName));

                if (res == null)
                {
                    //  ModelState.AddModelError("Update Error", "You failed to update login date & time");
                    ViewBag.Message = "You failed to update login date & time";
                }

                //var loginAudit = db.pr_addPersonLoginAudit(person.id, 1, DateTime.Now, ip, 1, true);

                SessionSingleton.LoggedInUserId = person.id;
                SessionSingleton.LoggedInUserRole = db.pr_getPersonRoleByPerson(person.id).FirstOrDefault().role;
                SessionSingleton.IsSystemMaster = db.pr_isSystemMaster(person.id).First() == 1 ? true : false;
                SessionSingleton.MyEnterPriseId = person.enterprise;
                SessionSingleton.Touchpoint = (int)person.campaign;

                try
                {
                    SessionSingleton.EnterpriseURL = db.pr_getEnterpriseSystemInfo(person.enterprise).FirstOrDefault().companyWebSite;
                }
                catch
                {
                    SessionSingleton.EnterpriseURL = "#";
                }
                Generic.Helpers.CurrentInstance.EnterpriseID = int.Parse(person.enterprise.ToString());
                if (Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    try
                    {
                        if (person.personStatus == (int)PersonHelper.PersonStatus.Invited)
                        {
                            return RedirectToAction("ResetPassword", "Person");
                        }
                        else
                        {
                            return RedirectToAction("Home", "Admin");
                        }
                    }
                    catch (Exception ex)
                    {

                        throw;
                    }

                }
            }
            else
            {
                ModelState.AddModelError("AccessDenied", "Access Code Invalid or Expired.");
            }
            //SetLoginAccess(accessCodeModel);
            return View();
        }

        [HttpPost]
        public bool SendAccessCode(string text, string subject, person master, person person)
        {
            var objSendEmail = new SendEmail();
            if (person != null)
            {
                var email = new Email
                {
                    subject = subject,
                    body = text,
                    category = SendGridCategory.EmailSend,
                    url = Request.Url.ToString(),
                    emailTo = person.email
                };
                objSendEmail.sendEmail(email, new EmailFormatSettings()
                {
                    enterprise = new enterprise()
                    {
                        id = person.enterprise.Value
                    }
                }, new System.Net.Mail.MailAddress(master.email, master.firstName + " " + master.lastName));
            }
            else
            {
                return false;
            }

            return true;
        }

        public virtual ActionResult Logout()
        {

            var claims = ((ClaimsIdentity)User.Identity).Claims.ToList();
            var claim = claims.Where(o => o.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").FirstOrDefault();

            //  CustomMembershipProvider MembershipService = new CustomMembershipProvider();
            //FormsAuthentication.RedirectToLoginPage();
            db.pr_modifyPersonLastLogoutDate(SessionSingleton.LoggedInUserId, DateTime.Now);
            //var loginAudit = db.pr_addPersonLoginAudit(SessionSingleton.LoggedInUserId, 2, DateTime.Now, null, 1, true);
            FormsAuthentication.SignOut();
            Session.Abandon();

            if (claim != null)
                return RedirectToAction("logout", "saml2");
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpPost]
        public ActionResult SetEventGroup(string group)
        {
            Session["EventGroup"] = group;
            return Json(true);
        }

        [Authorize]
        [GridAction]
        public ActionResult AjaxIteratePartners(int? touchpoint)
        {
            try
            {
                if (Session["EventGroup"] == null) throw new Exception("EventGroupNotFound");
                var result = db.pr_getEventNotificationByTouchpointAndEvent(CurrentInstance.EnterpriseID, touchpoint, Session["EventGroup"].ToString()).ToList();
                //if (SessionSingleton.AddIteratePartnerId.HasValue)
                //{
                //    var topItem = result.FirstOrDefault(o => o.id == SessionSingleton.AddIteratePartnerId.Value);
                //    result.Remove(topItem);
                //    result.Insert(0, topItem);
                //    SessionSingleton.AddIteratePartnerId = null;
                //}
                return Json(new GridModel(result), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new GridModel(), JsonRequestBehavior.AllowGet);
            }
        }

        [Authorize]
        public virtual ActionResult Home()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            ViewBag.TouchPoints = db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).Select(o => new SelectListItem()
            {
                Text = o.title,
                Value = o.id.ToString(),
                Selected = o.id == SessionSingleton.Touchpoint
            }).ToList();
            var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByTouchpoint(SessionSingleton.Touchpoint).ToList();
            var PTQ = "";
            //DataTable dt;
            try
            {
                //PTQ = (db.pr_getGroupByPTQ2(Convert.ToInt32(Session["PTQ"]))).ToString();
                //dt = db.pr_getGroupByPTQ2(2);
                ViewBag.abc = db.pr_getGroupByPTQ2(2);
            }
            catch (Exception ex)
            {

            }
            //db.pr_getrol
            if (enterprise != null)
            {
                var master = db.pr_getSystemMaster(enterprise.id).FirstOrDefault();
                if (master != null)
                {
                    ViewBag.masterFirstLastNames = master.firstName + " " + master.lastName;
                    ViewBag.masterPhone = master.phone;
                }
                ViewBag.enterpriseName = enterprise.description;
                try
                {
                    //pr_getStatusCountForReferenceByPTQ
                    //  List<pr_getStatusCountForReferenceByPTQ_Result> objCount = db.pr_getStatusCountForReferenceByPTQ(ptq.FirstOrDefault().id).ToList();
                    List<pr_getPartnerStatusCountByTouchpoint_Result> objCount = db.pr_getPartnerStatusCountByTouchpoint(SessionSingleton.Touchpoint).ToList();

                    //pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                    string pieChartData = "['Status','Count'],";
                    foreach (var data in objCount)
                    {
                        pieChartData += "['" + data.status + "'," + data.total + "],";
                    }
                    //  pieChartData += "['Total'," + objCount.total + "]";
                    //pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                    ViewBag.pieChartData = pieChartData;
                    var dataAll =
                        db.pr_getEventNotificationByTouchpointCount(Generic.Helpers.CurrentInstance.EnterpriseID,
                            SessionSingleton.Touchpoint);
                    var dataToday =
                        db.pr_getEventNotificationByTouchpointCountToday(Generic.Helpers.CurrentInstance.EnterpriseID,
                            SessionSingleton.Touchpoint);
                    //var y = db.pr_getTouchpointByPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
                    //var dataAll = db.pr_getReminderSentCountAll("MVCMT - R", y.description, Generic.Helpers.CurrentInstance.EnterpriseID);
                    //var dataToday = db.pr_getReminderSentCountDaily(DateTime.Now, "MVCMT - R", y.description, Generic.Helpers.CurrentInstance.EnterpriseID);

                    Dictionary<string, int?> dataAllDictionary = new Dictionary<string, int?>();
                    dataAllDictionary.Add("bounced", 0);
                    dataAllDictionary.Add("deferred", 0);
                    dataAllDictionary.Add("clicked", 0);
                    dataAllDictionary.Add("delivered", 0);
                    dataAllDictionary.Add("dropped", 0);
                    dataAllDictionary.Add("opened", 0);
                    dataAllDictionary.Add("processed", 0);
      
                    foreach (var o in dataAll)
                    {
                        if (o.@event.Trim().ToLower() == "bounce")
                        {
                            dataAllDictionary["bounced"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "deferred")
                        {
                            dataAllDictionary["deferred"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "click")
                        {
                            dataAllDictionary["clicked"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "delivered")
                        {
                            dataAllDictionary["delivered"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "dropped")
                        {
                            dataAllDictionary["dropped"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "open")
                        {
                            dataAllDictionary["opened"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "processed")
                        {
                            dataAllDictionary["processed"] = o.total;
                        }
                    }

                    Dictionary<string, int?> dataTodayDictionary = new Dictionary<string, int?>();
                    dataTodayDictionary.Add("bounced", 0);
                    dataTodayDictionary.Add("deferred", 0);
                    dataTodayDictionary.Add("clicked", 0);
                    dataTodayDictionary.Add("delivered", 0);
                    dataTodayDictionary.Add("dropped", 0);
                    dataTodayDictionary.Add("opened", 0);
                    dataTodayDictionary.Add("processed", 0);

                    foreach (var t in dataToday)
                    {
                        if (t.@event.Trim().ToLower() == "bounce")
                        {
                            dataTodayDictionary["bounced"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "deferred")
                        {
                            dataTodayDictionary["deferred"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "click")
                        {
                            dataTodayDictionary["clicked"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "delivered")
                        {
                            dataTodayDictionary["delivered"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "dropped")
                        {
                            dataTodayDictionary["dropped"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "open")
                        {
                            dataTodayDictionary["opened"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "processed")
                        {
                            dataTodayDictionary["processed"] = t.total;
                        }


                    }

                    ViewBag.dataAll = dataAllDictionary;
                    ViewBag.dataToday = dataTodayDictionary;
                }
                catch { }

            }
            if (PTQ != null)
                ViewBag.PTQ = PTQ;
            return View();
        }

        [Authorize]
        [HttpPost]
        public virtual ActionResult Home(int? touchpoint)
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            ViewBag.TouchPoints = db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).Select(o => new SelectListItem()
            {
                Text = o.title,
                Value = o.id.ToString(),
                Selected = o.id == touchpoint
            }).ToList();
            var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByTouchpoint(SessionSingleton.Touchpoint).ToList();
            var PTQ = "";
            //DataTable dt;
            try
            {
                //PTQ = (db.pr_getGroupByPTQ2(Convert.ToInt32(Session["PTQ"]))).ToString();
                //dt = db.pr_getGroupByPTQ2(2);
                ViewBag.abc = db.pr_getGroupByPTQ2(2);
            }
            catch (Exception ex)
            {

            }
            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;
                try
                {
                    //pr_getStatusCountForReferenceByPTQ
                    //  List<pr_getStatusCountForReferenceByPTQ_Result> objCount = db.pr_getStatusCountForReferenceByPTQ(ptq.FirstOrDefault().id).ToList();
                    List<pr_getPartnerStatusCountByTouchpoint_Result> objCount = db.pr_getPartnerStatusCountByTouchpoint(touchpoint).ToList();

                    //pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                    string pieChartData = "['Status','Count'],";
                    foreach (var data in objCount)
                    {
                        pieChartData += "['" + data.status + "'," + data.total + "],";
                    }
                    //  pieChartData += "['Total'," + objCount.total + "]";
                    //pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                    ViewBag.pieChartData = pieChartData;
                    var dataAll =
                        db.pr_getEventNotificationByTouchpointCount(Generic.Helpers.CurrentInstance.EnterpriseID,
                            touchpoint);
                    var dataToday =
                        db.pr_getEventNotificationByTouchpointCountToday(Generic.Helpers.CurrentInstance.EnterpriseID,
                            touchpoint);
                    //var y = db.pr_getTouchpointByPerson(SessionSingleton.LoggedInUserId).FirstOrDefault();
                    //var dataAll = db.pr_getReminderSentCountAll("MVCMT - R", y.description, Generic.Helpers.CurrentInstance.EnterpriseID);
                    //var dataToday = db.pr_getReminderSentCountDaily(DateTime.Now, "MVCMT - R", y.description, Generic.Helpers.CurrentInstance.EnterpriseID);

                    Dictionary<string, int?> dataAllDictionary = new Dictionary<string, int?>();
                    dataAllDictionary.Add("bounce", 0);
                    dataAllDictionary.Add("deferred", 0);
                    dataAllDictionary.Add("click", 0);
                    dataAllDictionary.Add("delivered", 0);
                    dataAllDictionary.Add("dropped", 0);
                    dataAllDictionary.Add("open", 0);
                    dataAllDictionary.Add("processed", 0);

                    foreach (var o in dataAll)
                    {
                        if (o.@event.Trim().ToLower() == "bounce")
                        {
                            dataAllDictionary["bounce"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "deferred")
                        {
                            dataAllDictionary["deferred"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "click")
                        {
                            dataAllDictionary["click"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "delivered")
                        {
                            dataAllDictionary["delivered"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "dropped")
                        {
                            dataAllDictionary["dropped"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "open")
                        {
                            dataAllDictionary["open"] = o.total;
                        }
                        else if (o.@event.Trim().ToLower() == "processed")
                        {
                            dataAllDictionary["processed"] = o.total;
                        }
                    }

                    Dictionary<string, int?> dataTodayDictionary = new Dictionary<string, int?>();
                    dataTodayDictionary.Add("bounce", 0);
                    dataTodayDictionary.Add("deferred", 0);
                    dataTodayDictionary.Add("click", 0);
                    dataTodayDictionary.Add("delivered", 0);
                    dataTodayDictionary.Add("dropped", 0);
                    dataTodayDictionary.Add("open", 0);
                    dataTodayDictionary.Add("processed", 0);

                    foreach (var t in dataToday)
                    {
                        if (t.@event.Trim().ToLower() == "bounce")
                        {
                            dataTodayDictionary["bounce"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "deferred")
                        {
                            dataTodayDictionary["deferred"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "click")
                        {
                            dataTodayDictionary["click"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "delivered")
                        {
                            dataTodayDictionary["delivered"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "dropped")
                        {
                            dataTodayDictionary["dropped"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "open")
                        {
                            dataTodayDictionary["open"] = t.total;
                        }
                        else if (t.@event.Trim().ToLower() == "processed")
                        {
                            dataTodayDictionary["processed"] = t.total;
                        }


                    }

                    ViewBag.dataAll = dataAllDictionary;
                    ViewBag.dataToday = dataTodayDictionary;
                }
                catch { }

            }
            if (PTQ != null)
                ViewBag.PTQ = PTQ;
            return View();
        }

        [Authorize]
        public virtual ActionResult View1()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;


                pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                string pieChartData = "['Status','Count'],";
                pieChartData += "['Completed'," + objCount.Completed + "],";
                pieChartData += "['Incomplete'," + objCount.Incomplete + "],";
                pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                ViewBag.pieChartData = pieChartData;


            }
            return View();
        }

        [Authorize]
        public virtual ActionResult View2()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;


                pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                string pieChartData = "['Status','Count'],";
                pieChartData += "['Completed'," + objCount.Completed + "],";
                pieChartData += "['Incomplete'," + objCount.Incomplete + "],";
                pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                ViewBag.pieChartData = pieChartData;


            }
            return View();
        }

        [Authorize]
        public virtual ActionResult View3()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;


                pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                string pieChartData = "['Status','Count'],";
                pieChartData += "['Completed'," + objCount.Completed + "],";
                pieChartData += "['Incomplete'," + objCount.Incomplete + "],";
                pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                ViewBag.pieChartData = pieChartData;


            }
            return View();
        }

        [Authorize]
        public virtual ActionResult Dashbord1(int? id, int? groupid)
        {

            Dashboard1 dashBoard = new Dashboard1();
            int _touchpoint = 0;
            var groupList = new List<group>();


            if (id == null || id == 0)
                _touchpoint = SessionSingleton.Touchpoint;
            else
                _touchpoint = (int)id;

            var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByTouchpoint(_touchpoint).ToList();
            var isSystemMaster = db.pr_isSystemMaster(SessionSingleton.LoggedInUserId).FirstOrDefault();
            ViewBag.IsSystemMaster = isSystemMaster.HasValue && isSystemMaster.Value == 1;
            ViewBag.TouchPointId = _touchpoint;
            foreach (var ptqItem in ptq)
            {

                var objDashboard = db.pr_getDashboardCountForReferenceByPTQ(ptqItem.id).ToList();


                //  db.pr_getPartnerByPTQGroupStatus(

                // pr_getDashboardCountForReferenceByPTQAndGroup

                if (groupid == null || groupid == 0)
                {
                    var groupDataList = db.pr_getGroupByPTQ(ptqItem.id).ToList();
                    var ptqgroupDataList = db.pr_getPTQGroupByPTQ(ptqItem.id).ToList();

                    PartnerTypeDataList datalist = new PartnerTypeDataList();
                    datalist.partnerType = new List<PartnerTypeData>();

                    foreach (var item in objDashboard.Select(x => x.partnertype).Distinct())
                    {
                        PartnerTypeData data = new PartnerTypeData();
                        data.ID = item;

                        data.Description = db.pr_getPartnerType(item).FirstOrDefault().description;

                        datalist.partnerType.Add(data);

                    }

                    if (dashBoard.partnerType == null)
                    {
                        dashBoard.partnerType = datalist.partnerType;
                    }
                    else
                    {
                        dashBoard.partnerType = dashBoard.partnerType.Union(datalist.partnerType).ToList().Distinct().ToList();
                    }


                    if (dashBoard.groups == null)
                    {
                        dashBoard.groups = groupDataList;
                        groupList = dashBoard.groups;
                    }
                    else
                    {
                        dashBoard.groups = dashBoard.groups.Union(groupDataList).ToList().Distinct().ToList();
                        groupList = dashBoard.groups;
                    }
                    if (dashBoard.ptqGroups == null)
                    {
                        dashBoard.ptqGroups = ptqgroupDataList;
                    }
                    else
                    {
                        dashBoard.ptqGroups = dashBoard.ptqGroups.Union(ptqgroupDataList).ToList().Distinct().ToList();
                    }
                    if (dashBoard.ptqDashboard == null)
                    {
                        dashBoard.ptqDashboard = objDashboard;
                    }
                    else
                    {
                        dashBoard.ptqDashboard = dashBoard.ptqDashboard.Union(objDashboard).ToList().Distinct().ToList();
                    }

                }
                else
                {
                    var grpList = db.pr_getGroupByPTQ(ptqItem.id).ToList();
                    var groupDataList = db.pr_getGroupByPTQ(ptqItem.id).ToList().Where(gro => gro.id == (int)groupid).ToList();
                    var ptqgroupDataList = db.pr_getPTQGroupByPTQ(ptqItem.id).ToList().Where(gro => gro.group == (int)groupid).ToList();


                    PartnerTypeDataList datalist = new PartnerTypeDataList();
                    datalist.partnerType = new List<PartnerTypeData>();

                    foreach (var item in objDashboard.Select(x => x.partnertype).Distinct())
                    {
                        PartnerTypeData data = new PartnerTypeData();
                        data.ID = item;

                        data.Description = db.pr_getPartnerType(item).FirstOrDefault().description;

                        datalist.partnerType.Add(data);

                    }

                    if (dashBoard.partnerType == null)
                    {
                        dashBoard.partnerType = datalist.partnerType;
                    }
                    else
                    {
                        dashBoard.partnerType = dashBoard.partnerType.Union(datalist.partnerType).ToList().Distinct().ToList();
                    }


                    if (dashBoard.groups == null)
                    {
                        dashBoard.groups = groupDataList;
                        groupList = grpList;
                    }
                    else
                    {
                        dashBoard.groups = dashBoard.groups.Union(groupDataList).ToList().Distinct().ToList();
                        groupList = groupList.Union(grpList).ToList().Distinct().ToList();
                    }
                    if (dashBoard.ptqGroups == null)
                    {
                        dashBoard.ptqGroups = ptqgroupDataList;
                    }
                    else
                    {
                        dashBoard.ptqGroups = dashBoard.ptqGroups.Union(ptqgroupDataList).ToList().Distinct().ToList();
                    }
                    if (dashBoard.ptqDashboard == null)
                    {
                        dashBoard.ptqDashboard = objDashboard;
                    }
                    else
                    {
                        dashBoard.ptqDashboard = dashBoard.ptqDashboard.Union(objDashboard).ToList().Distinct().ToList();
                    }

                }
            }

            if (groupList != null && groupList.Count > 0)
            {
                var groups = new SelectList(groupList, "id", "description", groupid);
                ViewBag.Groups = groups;
            }
            else
            {
                ViewBag.Groups = null;
            }


            ViewBag.TouchPoints = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title", _touchpoint); ;
            return View(dashBoard);
        }

        [Authorize]
        public virtual ActionResult Dashboard2()
        {
            Dashboard2 vm = new Dashboard2();
            var partnertype = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            ViewBag.partnertype = partnertype.Select(o => new SelectListItem()
            {
                Text = o.description,
                Value = o.id.ToString()
            }).ToList();

            vm.partnerType = partnertype.First().id;
            var projNames = db.pr_getPartnerByPartnerType(vm.partnerType).ToList();
            ViewBag.projectName = projNames.Select(o => new SelectListItem()
            {
                Text = o.name,
                Value = o.id.ToString()
            }).ToList();

            var accessCodes = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(projNames.First().id).ToList();
            ViewBag.accessCode = accessCodes.Select(o => new SelectListItem()
            {
                Text = o.accesscode,
                Value = o.person.partnerPartnertypeTouchpointQuestionnaire.First().id.ToString()
            }).ToList();

            if (accessCodes.Count > 0)
                vm.accessCode = accessCodes.First().id;

            return View(vm);
        }

        [Authorize]
        public virtual ActionResult Dashboard2_1(int? id, int? groupid)
        {
            Dashboard21 dashBoard = new Dashboard21();
            dashBoard.Objs = new List<pr_getDashboardCountForEventByPTQ2_Result>();
            dashBoard.PartnerTypes = new List<partnerType>();
            int _touchpoint = 0;
            var groupList = new List<group>();


            if (id == null || id == 0)
                _touchpoint = SessionSingleton.Touchpoint;
            else
                _touchpoint = (int)id;

            var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByTouchpoint(_touchpoint).ToList();
            var isSystemMaster = db.pr_isSystemMaster(SessionSingleton.LoggedInUserId).FirstOrDefault();
            ViewBag.IsSystemMaster = isSystemMaster.HasValue && isSystemMaster.Value == 1;

            List<pr_getDashboardCountForEventByPTQ2_Result> objs = new List<pr_getDashboardCountForEventByPTQ2_Result>();
            Dictionary<int, string> grptq = new Dictionary<int, string>();
            foreach (var ptqItem in ptq)
            {
                var items = db.pr_getDashboardCountForEventByPTQ2(ptqItem.id).ToList();
                objs.AddRange(items);
                dashBoard.Objs.AddRange(items);
                var gIds = items.Select(o => o.group).Distinct().ToList();
                foreach (var item in gIds)
                {
                    if (grptq.ContainsKey(item))
                    {
                        grptq[item] = grptq[item] + ptqItem.id.ToString() + ",";
                    }
                    else
                    {
                        grptq.Add(item, ptqItem.id.ToString() + ",");
                    }
                }
            }

            var grsIds = objs.Select(o => o.group).Distinct().ToList();
            var partnertypesIds = objs.Select(o => o.partnertype).Distinct().ToList();
            var groupsdb = db.pr_getGroupAll(Generic.Helpers.CurrentInstance.EnterpriseID).Where(gro => grsIds.Contains(gro.id)).ToList();
            var partnerTypes = db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).Where(gro => partnertypesIds.Contains(gro.id)).ToList();
            dashBoard.PartnerTypes = partnerTypes;

            dashBoard.Groups = new List<Dashboard21Group>();

            if (groupid.HasValue && groupid != 0)
            {
                foreach (var item in groupsdb.Where(o => o.id == groupid.Value))
                {
                    Dashboard21Group dg = new Dashboard21Group();
                    dg.Description = item.description;
                    dg.Id = item.id;
                    if (grptq.ContainsKey(item.id))
                    {
                        dg.PtqId = grptq[item.id].Substring(0, grptq[item.id].Length - 1);
                    }

                    dg.PartnerTypes = new List<Dashboard21PartnerType>();
                    var objs1 = objs.Where(o => o.group == item.id).ToList();
                    var ptIds = objs1.Select(o => o.partnertype).Distinct().ToList();
                    foreach (var pid in ptIds)
                    {
                        var p = partnerTypes.Where(o => o.id == pid).FirstOrDefault();
                        if (p == null) continue;

                        Dashboard21PartnerType vmPt = new Dashboard21PartnerType();
                        vmPt.Data = objs1.Where(o => o.partnertype == pid).ToList();
                        vmPt.Description = p.description;
                        vmPt.Id = p.id;
                        dg.PartnerTypes.Add(vmPt);
                    }

                    dashBoard.Groups.Add(dg);
                }
            }
            else
            {
                foreach (var item in groupsdb)
                {
                    Dashboard21Group dg = new Dashboard21Group();
                    dg.Description = item.description;
                    dg.Id = item.id;
                    if (grptq.ContainsKey(item.id))
                    {
                        dg.PtqId = grptq[item.id].Substring(0, grptq[item.id].Length - 1);
                    }

                    dg.PartnerTypes = new List<Dashboard21PartnerType>();
                    var objs1 = objs.Where(o => o.group == item.id).ToList();
                    var ptIds = objs1.Select(o => o.partnertype).Distinct().ToList();
                    foreach (var pid in ptIds)
                    {
                        var p = partnerTypes.Where(o => o.id == pid).FirstOrDefault();
                        if (p == null) continue;

                        Dashboard21PartnerType vmPt = new Dashboard21PartnerType();
                        vmPt.Data = objs1.Where(o => o.partnertype == pid).ToList();
                        vmPt.Description = p.description;
                        vmPt.Id = p.id;
                        dg.PartnerTypes.Add(vmPt);
                    }

                    dashBoard.Groups.Add(dg);
                }
            }


            if (groupsdb != null && groupsdb.Count > 0)
            {
                var groups = new SelectList(groupsdb, "id", "description", groupid);
                ViewBag.Groups = groups;
            }
            else
            {
                ViewBag.Groups = null;
            }

            ViewBag.TouchPoints = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title", _touchpoint); ;
            return View(dashBoard);
        }

        [Authorize]
        public virtual ActionResult Notifications(string ptq, int group, int partnerType, string ev)
        {
            Session["notificationsearch"] = new NotificationsSearchModel()
            {
                ev = ev,
                group = group,
                partnerType = partnerType,
                ptq = ptq

            };
            return RedirectToAction("NotificationsResult");
        }

        [Authorize]
        public ActionResult NotificationsResult()
        {
            NotificationsSearchModel model = Session["notificationsearch"] as NotificationsSearchModel;
            List<NotificationItem> items = new List<NotificationItem>();
            if (model != null)
            {
                string[] arr = model.ptq.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var gr = db.pr_getGroup(model.group).First().description;
                var pr = db.pr_getParnterType(model.partnerType).First().description;

                foreach (var item in arr)
                {
                    items.AddRange(db.pr_getEventNotificationByPTQPartnerTypeGroupAndEvent(Convert.ToInt32(item), model.partnerType, model.group, model.ev).Select(o => new NotificationItem()
                    {
                        Title = o.protocolTouchpoint,
                        AccessCode = o.accesscode,
                        Event = o.@event,
                        Email = o.email,
                        Reason = o.reason,
                        TimeStamp = o.timestamp,
                        Group = gr,
                        PartnerType = pr
                    }).ToList());
                }
            }
            return View(items);
        }

        [Authorize]
        public ActionResult GetDashboard2Result(int pptq, int partner, int partnerType)
        {
            ViewBag.PartnerPartnertypeTouchpointQuestionnaireByPartner = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partner).ToList();
            ViewBag.PPTQDocAll = db.pr_getPPTQDocAll(pptq).ToList();
            ViewBag.PersonPPTQClauseAll = db.pr_getPersonPPTQClauseAll(pptq).ToList();
            ViewBag.PersonPPTQClauseAllDesc = db.pr_getCFDBClauseAll().ToList();
            return View(db.pr_getPPTQTeamRacixByPPTQ_Grid_2(138965).ToList());
        }

        [Authorize]
        public ActionResult GetProjects(int partnerType)
        {
            var projNames = db.pr_getPartnerByPartnerType(partnerType).ToList();
            return Json(projNames.Select(o => new { o.id, o.name, title = o.name + " " + " " + " " }).ToList(), JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult GetAccessCodes(int project)
        {
            var accessCodes = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(project).Select(o => new { o.id, o.accesscode }).ToList();
            return Json(accessCodes, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public virtual ActionResult DashboardPartners(int status, int group, int partnerType, int touchpoint)
        {

            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";
            if (group != 0)
                arguments += "groupID=" + group + ";";

            if (touchpoint != 0)
                arguments += "touchpointID=" + touchpoint + ";";

            if (partnerType != 0)
                arguments += "partnertypeID=" + partnerType + ";";

            if (status != 0)
                arguments += "StatusID=" + status + ";";
            Session["partnersearch"] = arguments;
            Session["Partner_Find_Touchpoint"] = touchpoint;
            Session["groupID"] = group;
            Session["partnertypeID"] = partnerType;
            Session["statusID"] = status;
            return RedirectToAction("FindPartnerResult", "partner");
        }


        [Authorize]
        public virtual ActionResult View4()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            List<View4> objView4 = new List<View4>();
            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;

                List<pr_getGroupStatusCountForReferenceByPTQ_Result> objCount = db.pr_getGroupStatusCountForReferenceByPTQ(2073).ToList();
                //  pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();



                foreach (var objGroup in objCount.Select(x => x.group).Distinct())
                {
                    View4 objView4item = new View4();

                    string pieChartData = "['Status','Count'],";
                    foreach (var item in objCount.Where(x => x.group == objGroup))
                    {
                        pieChartData += "['" + item.status + "'," + item.total + "],";
                    }
                    objView4item.groupDescription = db.pr_getGroup(objGroup).FirstOrDefault().description;
                    objView4item.groupID = objGroup;
                    objView4item.pieChart = pieChartData;

                    objView4.Add(objView4item);
                }

                //pieChartData += "['Completed'," + objCount.Completed + "],";
                //pieChartData += "['Incomplete'," + objCount.Incomplete + "],";
                //pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                ViewBag.pieChartGroupData = objView4;


            }
            return View();
        }

        [Authorize]
        public virtual ActionResult Dashboard()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;


                pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                string pieChartData = "['Status','Count'],";
                pieChartData += "['Completed'," + objCount.Completed + "],";
                pieChartData += "['Incomplete'," + objCount.Incomplete + "],";
                pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                ViewBag.pieChartData = pieChartData;


            }
            return View();
        }


        [Authorize]
        public virtual ActionResult Icons()
        {

            return View();
        }

        [Authorize]
        public virtual ActionResult PickAProtocol()
        {
            var model = db.pr_getIndustryFocusDetailAll().ToList();
            return View(model);
        }

        [Authorize]
        public virtual ActionResult ManualRemind()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;
                pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                string pieChartData = "['Status','Count'],";
                pieChartData += "['Completed'," + objCount.Completed + "],";
                pieChartData += "['Incomplete'," + objCount.Incomplete + "],";
                pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                ViewBag.pieChartData = pieChartData;
            }

            if (SessionSingleton.LoggedInUserId == 3)
            {
                if (SchedulerServiceHelper.init(2))
                {
                    ViewData["schedulerMessage"] = "Scheduler Service has been successfully processed";
                }
            }
            return View();
        }


        [Authorize]
        public virtual ActionResult Welcome()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

            //db.pr_getrol
            if (enterprise != null)
            {
                ViewBag.enterpriseName = enterprise.description;


                pr_getCountFromPPTQByStatus_Result objCount = db.pr_getCountFromPPTQByStatus(1).FirstOrDefault();
                string pieChartData = "['Status','Count'],";
                pieChartData += "['Completed'," + objCount.Completed + "],";
                pieChartData += "['Incomplete'," + objCount.Incomplete + "],";
                pieChartData += "['Not Started'," + objCount.Not_Started + "]";

                ViewBag.pieChartData = pieChartData;


            }
            return View();
        }

        [Authorize]
        public virtual ActionResult HomePage()
        {
            ViewBag.Project = "Generic";
            return View();
        }

        //public virtual ActionResult SendGridTest()
        //{
        //	return View();
        //}

        //[HttpPost]
        //public virtual ActionResult SendGridTest(Email email)
        //{
        //	SendEmail objSendEmail = new SendEmail();
        //	objSendEmail.sendEmail(email);
        //	db.pr_addEventNotification(email.emailTo, DateTime.Now, "SendGridTest", null, null, null, null, email.protocolTouchpoint, "MVCMT", null, null);
        //	return View();
        //}

        public virtual ActionResult Menu(string animation, bool? enableOpacityAnimation, int? openDuration, int? closeDuration)
        {
            //List<Generic.menu> menu = db.pr_getMenuAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            //return PartialView("_MenuPartial", menu);

            ViewData["animation"] = animation ?? "slide";
            ViewData["enableOpacityAnimation"] = enableOpacityAnimation ?? true;
            ViewData["openDuration"] = openDuration ?? 200;
            ViewData["closeDuration"] = openDuration ?? 200;
            Generic.DataLayer.MenuOperation menuOperation = new DataLayer.MenuOperation();
            IEnumerable<MenuModel> menuModel = menuOperation.GetAllParentMenu();
            return PartialView("_MenuPartial", menuModel);
        }
        public virtual ActionResult UploadLogo(HttpPostedFileBase uploadLogo)
        {
            var fileFullPath = "";
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            if (uploadLogo != null)
            {
                if (enterprise != null)
                {
                    byte[] uploadedFile = new byte[uploadLogo.InputStream.Length];
                    uploadLogo.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    enterprise.logo = uploadedFile;


                    if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
                    }

                    if (!Directory.Exists((Server.MapPath("~/uploadedFiles/EnterpriseLogo"))))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/EnterpriseLogo"));
                    }

                    // db.pr_addEnterpriseSystemInfo(
                    var file = uploadLogo;

                    // Some browsers send file names with full path. This needs to be stripped.
                    var fileName = Path.GetFileName(file.FileName);
                    fileFullPath = Path.Combine(Server.MapPath("~/uploadedFiles/EnterpriseLogo"), fileName);


                    file.SaveAs(fileFullPath);

                    enterprise.applicationPath = fileFullPath;
                    db.Entry(enterprise).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
            }
            if (enterprise.logo == null)
            {
                var enterpriseIntelleges = db.pr_getEnterprise(1).FirstOrDefault();
                return PartialView("_InstanceLogoPartial", enterpriseIntelleges);
            }
            else
            {
                return PartialView("_InstanceLogoPartial", enterprise);
            }
        }
        protected string RenderActionResultToString(ActionResult result)
        {
            // Create memory writer.
            var sb = new StringBuilder();
            var memWriter = new StringWriter(sb);

            // Create fake http context to render the view.
            var fakeResponse = new HttpResponse(memWriter);
            var fakeContext = new HttpContext(System.Web.HttpContext.Current.Request,
                fakeResponse);
            var fakeControllerContext = new ControllerContext(
                new HttpContextWrapper(fakeContext),
                this.ControllerContext.RouteData,
                this.ControllerContext.Controller);
            var oldContext = System.Web.HttpContext.Current;
            System.Web.HttpContext.Current = fakeContext;

            // Render the view.
            result.ExecuteResult(fakeControllerContext);

            // Restore old context.
            System.Web.HttpContext.Current = oldContext;

            // Flush memory and return output.
            memWriter.Flush();
            return sb.ToString();
        }

        [AllowAnonymous]
        public virtual ActionResult InstanceLogo()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            if (enterprise.logo == null)
            {
                var enterpriseIntelleges = db.pr_getEnterprise(1).FirstOrDefault();
                return PartialView("_InstanceLogoPartial", enterpriseIntelleges);
            }
            else
            {
                return PartialView("_InstanceLogoPartial", enterprise);
            }
        }
        public virtual ActionResult IntellegesLogo()
        {
            var enterprise = db.pr_getEnterprise(1).FirstOrDefault();

            return PartialView("_InstanceLogoPartial", enterprise);
        }


        public virtual ActionResult DownloadTemplate()
        {
            ViewBag.templates = new SelectList(db.pr_getTemplateAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "url", "description");
            return PartialView("_templatesPartial");
        }

        [Authorize]
        public ActionResult TouchpointCombobox(TouchpointComboModel model)
        {
            model.AutoCompleteAttributes.Width = model.AutoCompleteAttributes.Width ?? 200;
            model.AutoCompleteAttributes.HighlightFirst = model.AutoCompleteAttributes.HighlightFirst ?? true;
            model.AutoCompleteAttributes.AutoFill = model.AutoCompleteAttributes.AutoFill ?? false;
            model.AutoCompleteAttributes.AllowMultipleValues = model.AutoCompleteAttributes.AllowMultipleValues ?? true;
            model.AutoCompleteAttributes.MultipleSeparator = model.AutoCompleteAttributes.MultipleSeparator ?? ", ";
            model.ComboBoxAttributes.Width = model.ComboBoxAttributes.Width ?? 400;
            model.ComboBoxAttributes.SelectedIndex = model.ComboBoxAttributes.SelectedIndex ?? (int)Session["touchpoint"];
            model.ComboBoxAttributes.HighlightFirst = model.ComboBoxAttributes.HighlightFirst ?? true;
            model.ComboBoxAttributes.AutoFill = model.ComboBoxAttributes.AutoFill ?? true;
            model.ComboBoxAttributes.OpenOnFocus = model.ComboBoxAttributes.OpenOnFocus ?? false;
            model.DropDownListAttributes.Width = model.DropDownListAttributes.Width ?? 200;
            model.DropDownListAttributes.SelectedIndex = model.DropDownListAttributes.SelectedIndex ?? 0;
            model.Touchpoints = db.pr_getTouchpointAllByEnterprise((int)Session["MyEnterPriseId"]).ToList();
            return PartialView("_TouchpointPartial", model);
        }

        public JsonResult ChangeTouchpoint(int touchPointId)
        {
            try
            {
                db.pr_modifyPersonTouchpoint((int)Session["LoggedInUserId"], touchPointId);
                Session["touchpoint"] = touchPointId;
                var touchpoint = db.pr_getTouchpoint(touchPointId).FirstOrDefault();
                return Json(new { touchpoint.description }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
        }


        [HttpPost]
        public ActionResult SendRequest(string text)
        {
            SendEmail objSendEmail = new SendEmail();
            var person = db.pr_getPerson(SessionSingleton.LoggedInUserId).First();
            var master = db.pr_getSystemMaster(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            if (person != null)
            {
                Email email = new Email();

                email.subject = "Intelleges Message";
                email.body = text;
                email.category = SendGridCategory.EmailSend;
                email.url = Request.Url.ToString();
                email.emailTo = master != null && !string.IsNullOrEmpty(master.email) ? master.email : "g0v6y5c6p3u5b1e0@startcritical.slack.com";

                objSendEmail.sendEmail(email, new EmailFormatSettings()
                {
                    enterprise = new enterprise()
                    {
                        id = Generic.Helpers.CurrentInstance.EnterpriseID
                    }
                }, new System.Net.Mail.MailAddress(person.email, person.firstName + " " + person.lastName));
            }
            else
            {
                return Json(new { success = false, msg = text });
            }

            return Json(new { success = true, msg = text });
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }

        public string GetAllMethods()
        {
            string detail = "";


            //MethodInfo[] methodInfos = typeof(HomeController).GetMethods(BindingFlags.Public | BindingFlags.Static); 
            //// sort methods by name 
            //Array.Sort(methodInfos, delegate(MethodInfo methodInfo1, MethodInfo methodInfo2) 
            //{ return methodInfo1.Name.CompareTo(methodInfo2.Name); });
            //// write method names 
            //foreach (MethodInfo methodInfo in methodInfos) 
            //{
            //    detail += methodInfo.Name+"<br>";

            //}

            foreach (var method in typeof(Generic.Areas.RegistrationArea.Controllers.HomeController).GetMethods())
            {
                var parameters = method.GetParameters();
                var parameterDescriptions = string.Join
                    (", ", method.GetParameters()
                                 .Select(x => x.ParameterType + " " + x.Name)
                                 .ToArray());

                detail += method.ReturnType +
                     " " + method.Name +
                        " " + parameterDescriptions + "<br>";
            }


            return detail;
        }


        public IEnumerable<SelectListItem> GetAllTouchPoints()
        {

            return new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");
        }
        public virtual ActionResult RemiderChartData()
        {
            var result = new List<object[]>();
            result.Add(new object[] { "Group", "Bounce", "Click", "Deferred", "Delivered", "Dropped", "Open", "Processed" });
            var data = db.pr_getEventNotificationByProtocolTouchpointCategoryCount("xxx").ToList();
            var grouped = data.GroupBy(o => o.description, p => p);
            if (!grouped.Any(o => o.Key == "Invite"))
            {
                result.Add(new object[] { "Invite", 300, 500, 250, 666, 168, 278, 400 });
            }
            if (!grouped.Any(o => o.Key == "Iterate"))
            {
                result.Add(new object[] { "Iterate", 300, 500, 250, 666, 168, 278, 400 });
            }
            if (!grouped.Any(o => o.Key == "Reminder"))
            {
                result.Add(new object[] { "Reminder", 300, 500, 250, 666, 168, 278, 400 });
            }
            foreach (var group in grouped)
            {
                var values = new List<object>();
                values.Add(group.Key);
                values.AddRange(group.OrderBy(p => p.@event).Select(o => (object)o.total).ToArray());
                result.Add(values.ToArray());
            }
            return Json(result.ToArray());
        }

        public virtual ActionResult CheckScheduleStatus()
        {
            return Json(db.pr_getReminderScheduledTaskHeartBeat().FirstOrDefault() > 0);
        }

        public JsonResult GetCurrentTouchPoint(int touchPointId)
        {
            var touchpoint = db.pr_getTouchpoint(touchPointId).FirstOrDefault();
            return Json(new { touchpoint.description }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult FreeTrial()
        {
            FreeTrialViewModel objmodel = new FreeTrialViewModel();
            var products = db.pr_getProductDetailAll().ToList();
            objmodel.products = new List<SelectListItem>();
            objmodel.products.Add(new SelectListItem
            {
                Value = "0",
                Text = "--Select--"
            });
            if (products != null)
            {
                foreach (var item in products)
                {
                    objmodel.products.Add(new SelectListItem
                    {
                        Value = item.id.ToString(),
                        Text = item.description
                    });
                }
            }
            objmodel.productId = 0;
            var industry = db.pr_getIndustryAll().ToList();
            objmodel.industries = new List<SelectListItem>();
            objmodel.industries.Add(new SelectListItem
            {
                Value = "0",
                Text = "--Select--"
            });
            if (industry != null)
            {
                foreach (var item in industry)
                {
                    objmodel.industries.Add(new SelectListItem
                    {
                        Value = item.id.ToString(),
                        Text = item.description
                    });
                }
            }
            objmodel.industryId = 0;
            ViewBag.useReCaptcha = !Request?.Url?.Host?.ToLower()?.StartsWith("localhost");
            return View(objmodel);
        }

        [HttpPost]
        public ActionResult FreeTrial(FreeTrialViewModel objmodel)
        {
           // System.Net.Mail.MailAddress addr = new System.Net.Mail.MailAddress(objmodel.emailAddress);
            autoMailMessage objamm = new autoMailMessage();
            objamm.subject = "Intelleges: Free Trial Form Details";
            objamm.text = "Dear Dana Klein, <br/> Here are teh details of Free Trial Form: <br/><br/>";
            objamm.text += "Name: " + objmodel.FirstName + " " + objmodel.LastName + "<br/>";
            objamm.text += "Email Address: "+objmodel.emailAddress + "<br/>";
            objamm.text += "Phone: "+objmodel.PhoneNumber+"<br/>";
            objamm.text += "Company: "+objmodel.Company+"<br/>";
            objamm.text += "Industry: "+ db.pr_getIndustry(objmodel.industryId).FirstOrDefault().description + "<br/>";
            objamm.text += "Area of Interest: "+ db.pr_getProductDetail(objmodel.productId).FirstOrDefault().description + "<br/>";
            //objamm.subject = "Invitation to Experience Supply Chain Compliance/Resilience Free Trial";
            //objamm.text = "Dear " + addr.User + ",<br/>";
            //objamm.text += "We are excited to invite you to experience the power and capabilities of Intelleges, a global supply chain compliance";
            //objamm.text += " and risk management solution. As a valued vistor to our website, we would like to offer you a 15-day free trial of our cloud-based product.";
            //objamm.text += "<br/> Here's how you get started: <br/>";
            //var url = "https://www.intelleges.com/mvcmt/Generic/Admin/FreeTrialForm?email=" + objmodel.emailAddress + "&product=" + objmodel.productId;
            //objamm.text += "Simply click on this <a href='" + url + "'>link</a> to access our user-friendly free trial form. <br/>";
            //objamm.text += "In the free trial form, you will be prompted to provide additional information such as your name, company name, and phone number, which we require for enterprise purposes. <br/>";
            //objamm.text += "Once you have completed and submitted the free trial form, our system will verify the information provided and create a limited access account tailored specifically for you.  <br/>";
            //objamm.text += "You can then log in to the system using your email address and the temporary password provided. <br/>";
            //objamm.text += "Congratulations! You will now enjoy limited access to our robust system for the duration of the free trial period, as specified in the license agreement. <br/>";
            //objamm.text += "Throughout the trial period, you will have the opportunity to explore the extensive features and functionalities of Intelleges, gaining invaluable insights into global supply chain compliance and risk management. We are confident that our product will help streamline your operations and mitigate potential risks effectively. <br/>";
            //objamm.text += "In the meanwhile, our dedicated support team will contact you to complete the setup. <br/>";
            //objamm.text += "Feel free to reach out to us at help@intelleges.combefore then if you have any questions. <br/>";
            //objamm.text += "At the conclusion of the free trial period, you will have the option to choose a suitable license to continue harnessing the full potential of Intelleges, or your access to the system will be automatically revoked. <br/>";
            //objamm.text += "Please note that Intelleges is an ISO 27001 certified trusted service provider, additionally, we have implemented stringent security measures to safeguard your data and ensure a smooth user experience. <br/>";
            //objamm.text += "Thank you for considering Intelleges as your partner in global supply chain compliance and risk management. We look forward to providing you with an exceptional trial experience and assisting you in making informed decisions for your organization's success. <br/>";
            //objamm.text += "Best regards, <br/> Dana Klein";
            Email mail = new Email(objamm);
            mail.type = "emailAlert";
            mail.emailTo = "dana.klein@intelleges.com";
            mail.url = Request.Url.ToString();
            mail.category = SendGridCategory.SendEmailAlert;
            SendEmail objSendEmail = new SendEmail();
            var enterprise = db.pr_getEnterpriseAll().FirstOrDefault();
            objSendEmail.sendEmail(mail, new EmailFormatSettings()
            {
                sender = null,
                touchpoint = null,
                enterprise= enterprise
            });
            return RedirectToAction("Index", "Admin");
        }
    }


}
