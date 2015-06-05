using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Helpers;

namespace Generic.Controllers
{
    public class MerckController : Controller
    {
        protected EntitiesDBContext db = new EntitiesDBContext();
        // GET: Merck
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ContactUs(int? contactUs = 1122)
        {
            var enterprises = db.pr_getEnterprise(contactUs);
            var elpptq = db.pr_getEnterpriseLandingPagePTQ(contactUs, (int)LangingPage.Login).FirstOrDefault();
            if (elpptq != null)
                ViewBag.ContactUsText = elpptq.text;
            ViewBag.Project = "Generic";
            ViewBag.LinkedInLoginUri = Request.Url.GetLeftPart(UriPartial.Authority) + Url.Action("ExternalLogin", "Admin");
            ViewBag.EnterpriseId = contactUs;
            return View(enterprises.FirstOrDefault());
            //return View();
        }
    }
}