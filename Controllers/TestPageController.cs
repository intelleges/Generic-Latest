using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class TestPageController : Controller
    {
        //
        // GET: /TestPage/

        public ActionResult Index()
        {
            ViewBag.IsSetFromClient = Generic.Session.SessionSingleton.Instance.IsSetFromClient;
            return View();
        }
        /// <summary> 
        ///This is the method for trialling overrides 
        /// </summary> 
        /// <returns></returns> 
        public ActionResult Submit()
        {
            string response = BreakHereIfYouHitMe();
            return RedirectToAction("Index", "Home");
        }
        /// <summary> 
        ///NOTE: - this is virtual, as we are going to override it in the///client project. 
        /// </summary> 
        /// <returns></returns> 
        public virtual string BreakHereIfYouHitMe()
        {
            string test = "break here - i am generic.";
            return test;
        }
    }
}
