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


namespace Generic.Controllers
{
    public class AdminController : Controller
    {
        private hs3MVCMTQa2Entities db = new hs3MVCMTQa2Entities();
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

           
            var enterprises = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID);
            

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
                    FormsAuthentication.SetAuthCookie(userName, false );
                    //if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                    //    && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    //{
                    //    return Redirect(returnUrl);
                    //}
                    //else
                    //{


                   // return Redirect("~/mvcmt/scs/admin/home");
                        return RedirectToAction("Home", "Admin");
                    //}
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }
            var enterprises = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID);
        
            // If we got this far, something failed, redisplay form
            return View(enterprises.FirstOrDefault());

        }

        [Authorize]
        public virtual ActionResult Home()
        {
            ViewBag.Project = "Generic";
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();

            base.Dispose(disposing);
        }

    }
}
