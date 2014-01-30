using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.Session;

namespace Generic.Controllers
{
    public class EnterpriseController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Enterprise/

        public ActionResult Index()
        {
            var EnterpriceGrid = db.pr_getEnterpriseAll().ToList();
            return View(EnterpriceGrid);
        }

        //
        // GET: /Enterprise/Details/5

        public ActionResult Details(int id = 0)
        {
            enterprise enterprise = db.pr_getEnterprise(id).FirstOrDefault();
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
        public ActionResult Create(enterprise enterprise, HttpPostedFileBase uploadLogo)
        {
            if (ModelState.IsValid)
            {
                if (uploadLogo != null)
                {
                    byte[] uploadedFile = new byte[uploadLogo.InputStream.Length];
                    uploadLogo.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    enterprise.logo = uploadedFile;
                }    

                db.enterprise.Add(enterprise);
                db.SaveChanges();
                SessionSingleton.EnterPriseId = enterprise.id;
                return RedirectToAction("Create", "Role");
            }

            return View(enterprise);
        }

        //
        // GET: /Enterprise/Edit/5

        public ActionResult Edit(int id = 0)
        {
            enterprise enterprise = db.pr_getEnterprise(id).FirstOrDefault();
            if (enterprise == null)
            {
                return HttpNotFound();
            }
            return View(enterprise);
        }

        //
        // POST: /Enterprise/Edit/5

        [HttpPost]
        public ActionResult Edit(enterprise enterprise, HttpPostedFileBase uploadLogo)
        {
            if (ModelState.IsValid)
            {
                //if (enterprise.logo == null)
                //{

                //}

                if (uploadLogo != null)
                {
                    byte[] uploadedFile = new byte[uploadLogo.InputStream.Length];
                    uploadLogo.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                    enterprise.logo = uploadedFile;
                }
                else
                {
                    using (var context = new EntitiesDBContext())
                    {
                        var enterpriseExisting = context.pr_getEnterprise(enterprise.id).FirstOrDefault();
                        enterprise.logo = enterpriseExisting.logo;
                    }
                }

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
            enterprise enterprise = db.pr_getEnterprise(id).FirstOrDefault();
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
            enterprise enterprise = db.pr_getEnterprise(id).FirstOrDefault();
            db.enterprise.Remove(enterprise);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        public ActionResult AssignQuestionnaireLevel()
        {
       //  var a =   db.pr_getQuestionnaireLevelType();
            ViewBag.enterprise = new SelectList(db.pr_getEnterpriseAll().ToList(), "id", "description");
            ViewBag.questionnaireLevel = new SelectList(db.pr_getQuestionnaireLevelTypeAll(), "id", "description");
            return View();
        }
        [HttpPost]
        public ActionResult AssignQuestionnaireLevel(int enterprise, int questionnaireLevel)
        {
            db.pr_addQuestionnaireLevel(enterprise, questionnaireLevel);

            ViewBag.enterprise = new SelectList(db.pr_getEnterpriseAll().ToList(), "id", "description");
            ViewBag.questionnaireLevel = new SelectList(db.pr_getQuestionnaireLevelTypeAll(), "id", "description");

            return View();

        }





        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}