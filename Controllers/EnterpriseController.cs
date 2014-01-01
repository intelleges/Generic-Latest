using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class EnterpriseController : Controller
    {
        private hs3MVCMTQa2Entities db = new hs3MVCMTQa2Entities();

        //
        // GET: /Enterprise/

        public ActionResult Index()
        {
            var EnterpriceGrid = db.pr_getEnterpriseAll1().ToList();
            return View(EnterpriceGrid);
        }

        //
        // GET: /Enterprise/Details/5

        public ActionResult Details(int id = 0)
        {
            enterprise enterprise = db.pr_getEnterprise1(id).FirstOrDefault();
            if (enterprise == null)
            {
                return HttpNotFound();
            }
            return View(enterprise);
        }

        //
        // GET: /Enterprise/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Enterprise/Create

        [HttpPost]
        public ActionResult Create(enterprise enterprise)
        {
            if (ModelState.IsValid)
            {
                db.enterprise.Add(enterprise);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(enterprise);
        }

        //
        // GET: /Enterprise/Edit/5

        public ActionResult Edit(int id = 0)
        {
            enterprise enterprise = db.pr_getEnterprise1(id).FirstOrDefault();
            if (enterprise == null)
            {
                return HttpNotFound();
            }
            return View(enterprise);
        }

        //
        // POST: /Enterprise/Edit/5

        [HttpPost]
        public ActionResult Edit(enterprise enterprise)
        {
            if (ModelState.IsValid)
            {
                db.Entry(enterprise).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(enterprise);
        }

        //
        // GET: /Enterprise/Delete/5

        public ActionResult Delete(int id = 0)
        {
            enterprise enterprise = db.pr_getEnterprise1(id).FirstOrDefault();
            if (enterprise == null)
            {
                return HttpNotFound();
            }
            return View(enterprise);
        }

        //
        // POST: /Enterprise/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            enterprise enterprise = db.pr_getEnterprise1(id).FirstOrDefault();
            db.enterprise.Remove(enterprise);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}