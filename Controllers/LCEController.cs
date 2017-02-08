using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;
namespace Generic.Controllers
{
    public class LCEController : Controller
    {
		private EntitiesDBContext db = new EntitiesDBContext();
        // GET: LCE
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult Create()
		{
			return View();
		}

		public ActionResult Users()
		{
			return Json(db.pr_getPersonAll(1066).Select(o=>o.email+" "+o.firstName+" "+o.lastName));
		}
    }
}