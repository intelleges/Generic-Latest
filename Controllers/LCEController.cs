using Generic.Models;
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
			GenerateOwner();
			return View();
		}
		private void GenerateOwner()
		{
			ViewBag.Owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
		}

		[HttpPost]
		public ActionResult Create(LCEModel model)
		{
			GenerateOwner();
			if (ModelState.IsValid)
			{
				db.pr_getLCE_Special_Data(model.Owner, model.Designation, model.ProgramName, model.Duedate).FirstOrDefault();
				return View();
			}
			return View(model);
		}

		public ActionResult Users()
		{
			return Json(db.pr_getPersonAll(1066).Select(o=>o.email+" "+o.firstName+" "+o.lastName));
		}
    }
}