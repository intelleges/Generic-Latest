using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
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
            ViewBag.product = new SelectList(db.pr_getProductAll().ToList(), "id", "description");
            ViewBag.subscriptionType = new SelectList(db.subscriptionType.ToList(), "id", "description");
            ViewBag.subscriptionStatus = new SelectList(db.subscriptionStatus.ToList(), "id", "description");
            ViewBag.multiTenantProjectType = new SelectList(db.multiTenantProjectType.ToList(), "id", "description");
            return View();
        }

        //
        // POST: /Enterprise/Create

        [HttpPost]
        public ActionResult Create(enterprise enterprise, HttpPostedFileBase uploadLogo)
        {
            try
            {
                ViewBag.product = new SelectList(db.pr_getProductAll().ToList(), "id", "description", enterprise.product);
                ViewBag.subscriptionType = new SelectList(db.subscriptionType.ToList(), "id", "description", enterprise.subscriptionType);
                ViewBag.subscriptionStatus = new SelectList(db.subscriptionStatus.ToList(), "id", "description", enterprise.subscriptionStatus);
                ViewBag.multiTenantProjectType = new SelectList(db.multiTenantProjectType.ToList(), "id", "description", enterprise.multiTenantProjectType);
                if (ModelState.IsValid)
                {
                    var fileFullPath = "";
                    if (uploadLogo != null)
                    {
                        byte[] uploadedFile = new byte[uploadLogo.InputStream.Length];
                        uploadLogo.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                        enterprise.logo = uploadedFile;


                        if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
                        }

                        if (!Directory.Exists((Server.MapPath("~/uploadedFiles/EnterpriseLogo"))))
                        {
                            Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/EnterpriseLogo"));
                        }

                        // db.pr_addEnterpriseSystemInfo(
                        var file = uploadLogo;

                        // Some browsers send file names with full path. This needs to be stripped.
                        var fileName = Path.GetFileName(file.FileName);
                        fileFullPath = Path.Combine(Server.MapPath("~/uploadedFiles/EnterpriseLogo"), fileName);


                        file.SaveAs(fileFullPath);

                        enterprise.applicationPath = fileFullPath;

                    }

                    enterprise.active = true;
                    //enterprise.multiTenantProjectType = 1;
                    var result = db.pr_addEnterprise(enterprise.description, enterprise.sortOrder, enterprise.active, enterprise.logo, enterprise.applicationPath, enterprise.companyName, enterprise.instanceName, enterprise.userMax, enterprise.partnerMax, enterprise.partnumberMax, enterprise.product, enterprise.subscriptionType, enterprise.freeTrialStartDate, enterprise.freeTrialEndDate, enterprise.licenseStartDate, enterprise.licenseEndDate, enterprise.monthlyFee, enterprise.subscriptionStatus, null, null, enterprise.multiTenantProjectType).FirstOrDefault();
                    //db.enterprise.Add(enterprise);
                    //db.SaveChanges();
                    if (result.HasValue)
                    {
                        SessionSingleton.EnterPriseId = (int)result.Value;
                        enterprise.id = (int)result.Value;
						db.pr_addEnterpriseSystemInfo(enterprise.systemExpiry, enterprise.licenseLimit, enterprise.companyName, string.Empty, enterprise.companyWebSite, string.Empty, 1, string.Empty, false, SessionSingleton.EnterPriseId, 0, 0, true);
                        using (var context = new EntitiesDBContext())
                        {
                            Session["pr_bootstrapAgencyId"] = context.pr_bootstrapAgency(SessionSingleton.EnterPriseId).FirstOrDefault();
                            var roleId = context.pr_bootstrapRole(SessionSingleton.EnterPriseId).FirstOrDefault();
                            var ptId = context.pr_bootstrapPartnertype(SessionSingleton.EnterPriseId).FirstOrDefault();
                            context.pr_bootstrapEnterprise(SessionSingleton.EnterPriseId);
                        }
                        ViewBag.saved = "true";
                        //return RedirectToAction("CreatePerson", "Person");
                       // return View();
                    }
                    else
                        ViewBag.saved = "false";
                }
            }
            catch
            {
                ViewBag.saved = "false";
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

                    if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
                    }

                    if (!Directory.Exists((Server.MapPath("~/uploadedFiles/EnterpriseLogo"))))
                    {
                        Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/EnterpriseLogo"));
                    }


                    var file = uploadLogo;

                    // Some browsers send file names with full path. This needs to be stripped.
                    var fileName = Path.GetFileName(file.FileName);
                    var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/EnterpriseLogo"), fileName);


                    file.SaveAs(physicalPath);

                    enterprise.applicationPath = physicalPath.ToString();

                }
                else
                {
                    using (var context = new EntitiesDBContext())
                    {
                        var enterpriseExisting = context.pr_getEnterprise(enterprise.id).FirstOrDefault();
                        enterprise.logo = enterpriseExisting.logo;
                        enterprise.applicationPath = enterpriseExisting.applicationPath;
                    }
                }

                enterprise.active = true;
                enterprise.multiTenantProjectType = 1;


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


        public ActionResult FindEnterprise()
        {
            ViewBag.subscriptionType = new SelectList(db.pr_getSubscriptionTypeAll1(), "id", "description");
            ViewBag.product = new SelectList(db.pr_getProductAll(), "id", "description");
            ViewBag.subscriptionStatus = new SelectList(db.pr_getSubscriptionStatusAll(), "id", "description");
            ViewBag.multiTenantProjectType = new SelectList(db.pr_getMultiTenantProjectTypeAll(), "id", "description");           
                
                
            return View();
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}