using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.SessionClass;

namespace Generic.Controllers
{
    public class RoleController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Role/

        public ActionResult Index()
        {
            var role = db.role.Include(r => r.enterprise1);
            return View(role.ToList());
        }

        //
        // GET: /Role/Details/5

        public ActionResult Details(int id = 0)
        {
            role role = db.role.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // GET: /Role/Create

        public ActionResult Create()
        {
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description");
            return View();
        }

        //
        // POST: /Role/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(role role)
        {
            role.enterprise = SessionSingleton.EnterPriseId;

            if (ModelState.IsValid)
            {
                db.role.Add(role);
                db.SaveChanges();
                return RedirectToAction("Create", "Group");
            }

            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", role.enterprise);
            return View(role);
        }

        //
        // GET: /Role/Edit/5

        public ActionResult Edit(int id = 0)
        {
            role role = db.role.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", role.enterprise);
            return View(role);
        }

        //
        // POST: /Role/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(role role)
        {
            if (ModelState.IsValid)
            {
                db.Entry(role).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", role.enterprise);
            return View(role);
        }

        //
        // GET: /Role/Delete/5

        public ActionResult Delete(int id = 0)
        {
            role role = db.role.Find(id);
            if (role == null)
            {
                return HttpNotFound();
            }
            return View(role);
        }

        //
        // POST: /Role/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            role role = db.role.Find(id);
            db.role.Remove(role);
            db.SaveChanges();
            return RedirectToAction("Index");
        }




        public ActionResult AssignMenu(int role = 0)
        {

            if (role == 0)
            {
                ViewBag.Role = new SelectList(db.pr_getRoleAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            }
            else
            {
                ViewBag.Role = new SelectList(db.pr_getRoleAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description", role);
            }




            var assignedMenus = db.pr_getMenuByRole(role).ToList()
                .Select(s => new
                {
                    ID = s.id,
                    Description = db.pr_getMenu(s.parentid).FirstOrDefault().description + " >> " + s.description
                }).ToList();

            var allMenus = db.pr_getMenuForRole(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            var unassignedMenus = allMenus.ToList().Where(x => !assignedMenus.Select(s => s.ID).ToList().Contains(x.MenuID)).ToList();
            ViewData["Menulist"] = new MultiSelectList(unassignedMenus, "MenuID", "Description");
            ViewData["assignedMenus"] = new MultiSelectList(assignedMenus, "ID", "Description");
            return View();
        }


        [HttpPost]
        public ActionResult AssignMenu(int role, FormCollection collection)
        {
            try
            {
                string assignedMenus = collection["assignedMenus"];
                var assignedMenuList = assignedMenus.Split(',');


                var assignedMenusOld = db.pr_getMenuByRole(role).ToList();

                foreach (var items in assignedMenusOld)
                {
                    db.pr_removeRoleMenu(role, items.id);
                }


                foreach (var item in assignedMenuList)
                {
                    db.pr_addRoleMenu(role, int.Parse(item), 1, true);
                }

                //  return RedirectToAction("AssignMenu", "Role");
            }
            catch { }


            ViewBag.Role = new SelectList(db.pr_getRoleAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "email");

            var assignedMenus2 = db.pr_getMenuByRole(role).ToList();

            var allMenus = db.pr_getMenuForRole(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();

            var unassignedMenus = allMenus.ToList().Where(x => !assignedMenus2.Select(s => s.id).ToList().Contains(x.MenuID)).ToList();
            ViewData["Menulist"] = new MultiSelectList(unassignedMenus, "MenuID", "Description");
            ViewData["assignedMenus"] = new MultiSelectList(assignedMenus2, "ID", "Description");
            return View();
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}