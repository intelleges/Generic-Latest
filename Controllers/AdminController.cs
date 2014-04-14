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


            var enterprises = db.pr_getEnterprise(1);


            ViewBag.Project = "Generic";
            return View(enterprises.FirstOrDefault());
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
                    Generic.Helpers.CurrentInstance.EnterpriseID = int.Parse(person.enterprise.ToString());
                  
                    return RedirectToAction("Home", "Admin");
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }



            var enterprises = db.pr_getEnterprise(1);

            // If we got this far, something failed, redisplay form
            return View(enterprises.FirstOrDefault());

        }

        [Authorize]
        public virtual ActionResult Home()
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();

            //db.pr_getrol
            if (enterprise != null) { 
            ViewBag.enterpriseName = enterprise.description;
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

            return PartialView("_InstanceLogoPartial", enterprise);
        }
        public virtual ActionResult IntellegesLogo()
        {
            var enterprise = db.pr_getEnterprise(1).FirstOrDefault();

            return PartialView("_InstanceLogoPartial", enterprise);
        }
        public ActionResult TouchpointCombobox(TouchpointComboModel model)
        {
            model.AutoCompleteAttributes.Width = model.AutoCompleteAttributes.Width ?? 200;
            model.AutoCompleteAttributes.HighlightFirst = model.AutoCompleteAttributes.HighlightFirst ?? true;
            model.AutoCompleteAttributes.AutoFill = model.AutoCompleteAttributes.AutoFill ?? false;
            model.AutoCompleteAttributes.AllowMultipleValues = model.AutoCompleteAttributes.AllowMultipleValues ?? true;
            model.AutoCompleteAttributes.MultipleSeparator = model.AutoCompleteAttributes.MultipleSeparator ?? ", ";
            model.ComboBoxAttributes.Width = model.ComboBoxAttributes.Width ?? 200;
            model.ComboBoxAttributes.SelectedIndex = model.ComboBoxAttributes.SelectedIndex ?? 0;
            model.ComboBoxAttributes.HighlightFirst = model.ComboBoxAttributes.HighlightFirst ?? true;
            model.ComboBoxAttributes.AutoFill = model.ComboBoxAttributes.AutoFill ?? true;
            model.ComboBoxAttributes.OpenOnFocus = model.ComboBoxAttributes.OpenOnFocus ?? false;
            model.DropDownListAttributes.Width = model.DropDownListAttributes.Width ?? 200;
            model.DropDownListAttributes.SelectedIndex = model.DropDownListAttributes.SelectedIndex ?? 0;

            model.Touchpoints = db.pr_getTouchpointAll().ToList();

            return PartialView("_TouchpointPartial", model);

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
