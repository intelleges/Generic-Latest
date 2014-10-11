using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.IO;
using Generic.Helpers;
using System.Web.Security;
using System.Web.Routing;
using Generic.Models;
//using Generic.Helpers;
using Generic.Helpers.Utility;
using System.Net;
using Generic.SessionClass;
using System.Reflection;
using Generic.ViewModel;
using System.Data;
using WebMatrix.WebData;


namespace Generic.Controllers
{
    public class AdminController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
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
        public virtual ActionResult Index()
        {

            try
            {
                var enterprises = db.pr_getEnterprise(1);

                ViewBag.Project = "Generic";
                return View(enterprises.FirstOrDefault());
            }
            catch (Exception ex)
            {
                //Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                return View();
            }
        }

        //
        // POST: /Account/ExternalLogin

        [HttpPost]
        
        public void ExternalLogin(string provider, string returnUrl)
        {
            var credentials = new Hammock.Authentication.OAuth.OAuthCredentials
            {
                CallbackUrl = "https://intelleges.com/mvcmt/Generic/Admin/ExternalLoginCallback",
                //CallbackUrl = "http://localhost:51090/Admin/ExternalLoginCallback",
                ConsumerKey = "7747cjm5yf3gbp",
                ConsumerSecret = "SzdxJQqxWWonlMz5",
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

            //SessionSingleton.AccessToken = token;
            //SessionSingleton.AccessSecretToken = authToken;
            Session["AccessToken"] = token;
            Session["AccessSecretToken"] = authToken;

            Response.Redirect("https://www.linkedin.com/uas/oauth/authorize?oauth_token=" + token, false);
        }

        //
        // GET: /Account/ExternalLoginCallback
        
        public ActionResult ExternalLoginCallback(string returnUrl)
        {
            String verifier = string.Empty;
            try
            {
                verifier = Request.QueryString["oauth_verifier"].ToString();
            }
            catch
            {
                return RedirectToAction("Index");
            }

            var credentials = new Hammock.Authentication.OAuth.OAuthCredentials();
            try
            {
                credentials = new Hammock.Authentication.OAuth.OAuthCredentials
                {
                    ConsumerKey = "7747cjm5yf3gbp",
                    ConsumerSecret = "SzdxJQqxWWonlMz5",
                    //Token = SessionSingleton.AccessToken,
                    //TokenSecret = SessionSingleton.AccessSecretToken,
                    //Token = Server.HtmlEncode(System.Web.HttpContext.Current.Request.Cookies["LinkedinCookie"]["AccessToken"]).ToString(),
                    //TokenSecret = Server.HtmlEncode(System.Web.HttpContext.Current.Request.Cookies["LinkedinCookie"]["AccessSecretToken"]).ToString(),
                    Token = Session["AccessToken"].ToString(),
                    TokenSecret = Session["AccessSecretToken"].ToString(),
                    Verifier = verifier,
                    Type = Hammock.Authentication.OAuth.OAuthType.AccessToken,
                    ParameterHandling = Hammock.Authentication.OAuth.OAuthParameterHandling.HttpAuthorizationHeader,
                    SignatureMethod = Hammock.Authentication.OAuth.OAuthSignatureMethod.HmacSha1,
                    Version = "1.0"
                };
            }
            catch 
            {
                var returnError = "NullReferenceException! ";
                if (Session["AccessToken"] == null) returnError += " AccessToken is null ";
                if (Session["AccessSecretToken"] == null) returnError += " AccessSecretToken is null";
                //if (System.Web.HttpContext.Current.Request.Cookies["LinkedinCookie"] != null)
                //{
                //    if (System.Web.HttpContext.Current.Request.Cookies["AccessToken"] != null)
                //        returnError += " AccessToken - " + Server.HtmlEncode(System.Web.HttpContext.Current.Request.Cookies["LinkedinCookie"]["AccessToken"]);
                //    if (System.Web.HttpContext.Current.Request.Cookies["AccessSecretToken"] != null)
                //        returnError += " AccessSecretToken - " + Server.HtmlEncode(System.Web.HttpContext.Current.Request.Cookies["LinkedinCookie"]["AccessSecretToken"]);
                //}
                throw new Exception(returnError);
            }

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

            string token = strResponseAttributes[0].Substring(strResponseAttributes[0].LastIndexOf('=') + 1);
            string authToken = strResponseAttributes[1].Substring(strResponseAttributes[1].LastIndexOf('=') + 1);

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
                Token = token,
                TokenSecret = authToken,
                Verifier = verifier
            };

            var hammockClient = new Hammock.RestClient()
            {
                Authority = "https://api.linkedin.com/v1/people/",
                Credentials = hammockCredentials,
                Method = Hammock.Web.WebMethod.Get
            };

            var MyInfo = hammockClient.Request(hammockRequest);
            String content = MyInfo.Content.ToString();

            var person = from c in System.Xml.Linq.XElement.Parse(content).Elements() select c;

            string email = person.SingleOrDefault(e => e.Name == "email-address").Value;

            if (!string.IsNullOrEmpty(email))
            {
                SessionSingleton.EmailFromLinkedin = email;

                List<Generic.enterprise> result = db.pr_getEnterpriseByEmail(email).ToList();

                if (result.Count() == 0)
                {
                    var message = "Thank you for your interest in Intelleges. You currently do not have an Intelleges account, and right now LinkedIn members are by INVITATION ONLY. We are happy to add you to our WAITING LIST and send you an invite if this policy changes in the future. Please click Yes if you would like to be added. If you don &apos;t want to be added, click No. Thank you.";
                    TempData["LinkedInMessage"] = new KeyValuePair<string, string>("promptYesNo", message);
                }
                else if (result.Count() > 1)
                {
                    TempData["LinkedInMessage"] = new KeyValuePair<string, string>("promptDropDown", "Please select Enterprise");
                    TempData["Enterprises"] = result;
                }
                else if (result.Count() == 1)
                {
                    FormsAuthentication.SetAuthCookie(email, false);
                    person resultPerson = db.pr_getPersonByEmail(result[0].id, email).FirstOrDefault();
                    SessionSingleton.LoggedInUserId = resultPerson.id;
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

            return RedirectToAction("Index");
        }

        [HttpPost]
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
        public ActionResult LoginByEnterprise(string enterpriseid)
        {
            if (!string.IsNullOrEmpty(enterpriseid))
            {
                var email = SessionSingleton.EmailFromLinkedin;
                FormsAuthentication.SetAuthCookie(email, false);
                person resultPerson = db.pr_getPersonByEmail(Convert.ToInt32(enterpriseid), email).FirstOrDefault();
                SessionSingleton.LoggedInUserId = resultPerson.id;
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

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Index(string userName, string password)
        {
            if (ModelState.IsValid)
            {
                //  CustomMembershipProvider MembershipService = new CustomMembershipProvider();
                if (MembershipService.ValidateUser(userName, password))
                {
                    FormsAuthentication.SetAuthCookie(userName, false);
                    //if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    //    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    //else
                    //{
                    person person = db.pr_doLogin(userName, password).FirstOrDefault();
                    SessionSingleton.LoggedInUserId = person.id;
                    SessionSingleton.MyEnterPriseId = person.enterprise;
                    SessionSingleton.Touchpoint = (int)person.campaign;

                    try
                    {
                        SessionSingleton.EnterpriseURL = db.pr_getEnterpriseSystemInfo(person.enterprise).FirstOrDefault().companyWebSite;
                    }
                    catch {
                        SessionSingleton.EnterpriseURL = "#";
                    }
                    Generic.Helpers.CurrentInstance.EnterpriseID = int.Parse(person.enterprise.ToString());

                    if (person.personStatus == (int)PersonHelper.PersonStatus.Invited)
                    {
                        return RedirectToAction("ResetPassword", "Person");
                    }
                    else
                    {
                        return RedirectToAction("Home", "Admin");
                    }
                    //}
                }
                else
                {
                    ModelState.AddModelError("LoginFailed", "The user name or password provided is incorrect.");
                }
            }

            var enterprises = db.pr_getEnterprise(1);

            // If we got this far, something failed, redisplay form
            return View(enterprises.FirstOrDefault());
        }

        public virtual ActionResult Logout()
        {

            //  CustomMembershipProvider MembershipService = new CustomMembershipProvider();
            //FormsAuthentication.RedirectToLoginPage();

            FormsAuthentication.SignOut();
            Session.Abandon();

            return RedirectToAction("Index");
            //}


        }

        [Authorize]
        public virtual ActionResult Home()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
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
        public virtual ActionResult Dashbord1()
        {

            Dashboard1 dashBoard = new Dashboard1();

            var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByTouchpoint(SessionSingleton.Touchpoint).ToList();

            foreach (var ptqItem in ptq)
            {

                var objDashboard = db.pr_getDashboardCountForReferenceByPTQ(ptqItem.id).ToList();


                //  db.pr_getPartnerByPTQGroupStatus(


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
                }
                else
                {
                    dashBoard.groups = dashBoard.groups.Union(groupDataList).ToList().Distinct().ToList();
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
            return View(dashBoard);
        }

        [Authorize]
        public virtual ActionResult DashboardPartners(int status, int group, int partnerType)
        {
           
            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";
            if (group != 0)
                arguments += "groupID=" + group + ";";
         
            if (partnerType != 0)
                arguments += "partnertypeID=" + partnerType + ";";

            if (status != 0)
                arguments += "StatusID=" + status + ";";
            Session["partnersearch"] = arguments;
            return RedirectToAction("FindPartnerResult","partner");
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
            if (SchedulerServiceHelper.init())
            {
                ViewData["schedulerMessage"] = "Scheduler Service has been successfully processed";
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



        public virtual ActionResult SendGridTest()
        {
            return View();
        }

        [HttpPost]
        public virtual ActionResult SendGridTest(Email email)
        {
            SendEmail objSendEmail = new SendEmail();
            objSendEmail.sendEmail(email);
            return View();
        }

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

        public JsonResult ChangeTouchpoint(int selectedTouchpoint)
        {
            try
            {
                db.pr_modifyPersonTouchpoint((int)Session["LoggedInUserId"], selectedTouchpoint);
                Session["touchpoint"] = selectedTouchpoint;
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
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


    }
}
