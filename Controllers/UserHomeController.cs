using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic;
using Generic.Models;
namespace BAA.Controllers
{
    public class UserHomeController : Controller
    {
        //
        // GET: /UserHome/
        private EntitiesDBContext db = new EntitiesDBContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SelectProduct()
        {
            return View();
        
        }

        [HttpPost]
        public ActionResult SelectProduct(ProductTypeModel model)
        {
            return View();
        }

    }
}
