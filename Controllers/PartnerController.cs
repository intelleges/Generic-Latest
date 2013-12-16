using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class PartnerController : Controller
    {
        //
        // GET: /Partner/

        public virtual ActionResult Index()
        {
           // ViewBag.Project = "Generic";
            return View();
        }

        public virtual ActionResult AccessCode()
        {
            return View();
        }

        public virtual ActionResult CompanyInfomation()
        {
            return View();
        }

        public virtual ActionResult ContactInformation()
        {
            return View();
        }
        public virtual ActionResult EditCompanyInformation()
        {
            return View();
        }

        public virtual ActionResult EditContactInformation()
        {
            return View();
        }

        public virtual ActionResult ESignature()
        {
            return View();
        }

        public virtual ActionResult eSignatureEnd()
        {
            return View();
        }

        public virtual ActionResult Questionnaire()
        {
            return View();
        }
    }
}
