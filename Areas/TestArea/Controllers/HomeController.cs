using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Areas.TestArea.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /TestArea/Home/

        public virtual ActionResult Index()
        {
            return View();
        }

        /// <summary> 
        ///This is the method for trialling overrides 
        /// </summary> 
        /// <returns></returns> 
        public  ActionResult Submit()
        {
            string response = BreakHereIfYoureInTheArea();
            return RedirectToAction("Index","Home",new{ Area=""});}
        

        /// <summary> 
        ///Note - this is virtual, as we are going to override it in the client
        ///project. 
        /// </summary> 
        /// <returns></returns> 
        public virtual string BreakHereIfYoureInTheArea()
        {
            string test ="break here - i am generic.";
            return test;
        }
    }
}
