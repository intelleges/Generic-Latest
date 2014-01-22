using Generic.Session;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class GroupController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Group/

        public ActionResult Index()
        {
            var groups = db.pr_getGroupAll(SessionSingleton.MyEnterPriseId);
            return View(groups.ToList());
        }

        //
        // GET: /Group/Details/5

        public ActionResult Details(int id = 0)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // GET: /Group/Create

        public ActionResult Create()
        {
            //ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name");
            return View();
        }

        //
        // POST: /Group/Create

        [HttpPost]
        public ActionResult Create(group group)
        {
            if (ModelState.IsValid)
            {
                group.author = SessionSingleton.LoggedInUserId;
                group.enterprise = SessionSingleton.EnterPriseId;
                db.group.Add(group);
                db.SaveChanges();
                return RedirectToAction("Create","Person");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", group.enterprise);
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name", group.groupCollection);
            return View(group);
        }

        //
        // GET: /Group/Edit/5

        public ActionResult Edit(int id = 0)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            if (group == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", group.enterprise);
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name", group.groupCollection);
            return View(group);
        }

        //
        // POST: /Group/Edit/5

        [HttpPost]
        public ActionResult Edit(group group)
        {
            if (ModelState.IsValid)
            {
                db.Entry(group).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", group.enterprise);
            ViewBag.groupCollection = new SelectList(db.groupCollection, "id", "name", group.groupCollection);
            return View(group);
        }

        //
        // GET: /Group/Delete/5

        public ActionResult Delete(int id = 0)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            if (group == null)
            {
                return HttpNotFound();
            }
            return View(group);
        }

        //
        // POST: /Group/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            group group = db.pr_getGroup(id).FirstOrDefault();
            db.group.Remove(group);
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