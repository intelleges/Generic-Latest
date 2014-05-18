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

                db.pr_bootstrapEnterprise(enterprise.id);

                return RedirectToAction("CreatePerson", "Person");
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

        public enterprise GetEnterprise(int enterpriseId)
        {
            enterprise objEnterprise = new enterprise();
            try
            {
                objEnterprise = db.pr_getEnterprise(enterpriseId).FirstOrDefault();
            }
            catch { }
            return objEnterprise;

        }

        public product GetProduct(int productId)
        {
            product objProduct = new product();
            try
            {
                objProduct = db.pr_getProduct(productId).FirstOrDefault();
            }
            catch { }
            return objProduct;

        }
        public subscriptionType GetSubscriptionType(int SubscriptionTypeId)
        {
            subscriptionType objSubscriptionType = new subscriptionType();
            try
            {
                objSubscriptionType = db.pr_getSubscriptiontype(SubscriptionTypeId).FirstOrDefault();
            }
            catch { }
            return objSubscriptionType;

        }

        public subscriptionStatus GetSubscriptionStatus(int SubscriptionStatusId)
        {
            subscriptionStatus objSubscriptionStatus = new subscriptionStatus();
            try
            {
                objSubscriptionStatus = db.pr_getSubscriptionStatus(SubscriptionStatusId).FirstOrDefault();
            }
            catch { }
            return objSubscriptionStatus;

        }

       

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}