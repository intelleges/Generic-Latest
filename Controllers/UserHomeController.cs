using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic;
using Generic.Models;

using System.Configuration;

using System.Web.Routing;
using System.Web.Security;
using Generic.Models;
using ChargifyNET;
using ChargifyNET.Configuration;
using WebMatrix.WebData;
using Generic.Helpers.Utility;
using System.Data.Entity.Core.Objects;


namespace BAA.Controllers
{
    public class UserHomeController : Controller
    {
        //
        // GET: /UserHome/
        private EntitiesDBContext db = new EntitiesDBContext();
        public ActionResult Index(ProductTypeModel model, int? SubscriptionType)
        {
            ViewBag.Query = "";
            if (model.partnerCount >0)
            {
                List<SelectListItem> expYears = new List<SelectListItem>();
                for (int i = 0; i <= 10; i++)
                {
                    string year = (DateTime.Today.Year + i).ToString();
                    expYears.Add(new SelectListItem { Text = year, Value = year });
                }
                ViewBag.ExpYears = new SelectList(expYears, "Value", "Text");

                IEnumerable<SelectListItem> expMonths = DateTimeFormatInfo.InvariantInfo.MonthNames.Where(m => !String.IsNullOrEmpty(m)).Select((monthName, index) => new SelectListItem
                {
                    Value = (index + 1).ToString(),
                    Text = (index + 1).ToString("00")
                });

                ViewBag.ExpMonths = new SelectList(expMonths, "Value", "Text");
                Session["subscriptiontype"] = SubscriptionType;

                if (SubscriptionType == 1)
                {
                    var product = Chargify.LoadProduct("standard");
                    ViewBag.ProductName = product.Name ?? string.Empty;
                    ViewBag.Product = "1";
                }
                else if (SubscriptionType == 2)
                {
                    var product = Chargify.LoadProduct("advanced");
                    ViewBag.ProductName = product.Name ?? string.Empty;
                    ViewBag.Product = "2";
                }
                else
                {
                    var product = Chargify.LoadProduct("enterprise");
                    ViewBag.ProductName = product.Name ?? string.Empty;
                    ViewBag.Product = "3";
                }
                ViewBag.CalculatedCost = db.pr_getCalculatedCostForEnterpriseSubscription(model.userCount, model.partnerCount, model.partnumberCount, SubscriptionType).FirstOrDefault();
                ViewBag.User = model.userCount;
                ViewBag.Partner = model.partnerCount;
                ViewBag.Query = "AccountInfo";
            }
           
            return View();
        }

        public ActionResult SelectProduct()
        {
            return View();

        }

        [HttpPost]
        public ActionResult AddEnterprise(FormCollection frmCollection)
        {
            try
            {
                string email = frmCollection["email"].ToString();
                //string Password = frmCollection["Password"].ToString();
                int product = int.Parse(frmCollection["product-select"].ToString());
                int userCount = int.Parse(frmCollection["user-count"].ToString());
                int partnerCount = int.Parse(frmCollection["partner-count"].ToString());
                decimal monthly = decimal.Parse(frmCollection["monthly"]);

                if (ModelState.IsValid)
                {
                    ObjectResult<Decimal?> addEnterpriseResult = db.pr_addEnterprise("", 0, true, null, null, "NewCompany", "newCompany", userCount, partnerCount, partnerCount, product, 1, DateTime.Now, DateTime.Now.AddDays(30), DateTime.Now, DateTime.Now.AddDays(30), monthly, 1, "1", null,1);
                 
                    int enterpriseID= (int)addEnterpriseResult.FirstOrDefault().Value;
                    ObjectResult<Decimal?> addpersonResult = db.pr_addPerson(enterpriseID, 1, 1, 1, 1, 1, "1", "1", "1", "New first name", "New lastname", "Mr", "", "", "", email, "", "", "", 1, "1", 1, "", "", 1, 1, null, null, null, null);
                    ObjectResult<Decimal?> addPartnerResult = db.pr_addPartner(enterpriseID, "1", "New Person", "", "", "", 1, "", "", 1, "", "", "F", "L", "", email, "", "", 1, 1, 1, 1, DateTime.Now, true, null);
                   int partnerID = (int)addPartnerResult.FirstOrDefault().Value;
                    string accessCode = db.pr_getAccesscode().FirstOrDefault();

                    int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(1, 1).FirstOrDefault().id;
                    Guid userId = Guid.NewGuid();
                    //db.pr_addPartnerPartnertypeTouchpointQuestionnaire(partnerID, ptq, accessCode, 1, DateTime.Now, null, partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE, null, null, null, null, null);
                    //db.pr_addPartnerPartnertypeTouchpointQuestionnaire(partnerID, ptq, accessCode, 1, DateTime.Now,DateTime.Now, partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE, 0, null, null, null, 0);
                    //var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();

                    //var objtouchpoint = db.pr_getTouchpoint(1).FirstOrDefault();
                    //var objperson = db.pr_getPerson((int)addpersonResult.FirstOrDefault().Value).FirstOrDefault();
                    //var objpartner = db.pr_getPartner(partnerID).FirstOrDefault();

                    //Email objemail = new Email(amm);
                    //EmailFormat emailFormat = new EmailFormat();
                    //objemail.body = emailFormat.sGetEmailBody(objemail.body, objperson, objpartner, objtouchpoint, ptq);
                    //objemail.emailTo = email;
                    //SendEmail objSendEmail = new SendEmail();
                    //objSendEmail.sendEmail(objemail);

                    var customerInfo = new CustomerAttributes()
                    {
                        FirstName = "First",
                        LastName = "Last",
                        Email = email,
                        SystemID = userId.ToString()
                    };

                    var paymentAttributes = new CreditCardAttributes()
                    {
                        FullNumber = "1",
                        CVV = "123",
                        ExpirationMonth = 12,
                        ExpirationYear =2018,
                        BillingAddress = "Address",
                        BillingCity ="City",
                        BillingZip = "1000100",
                        BillingState = "State",
                        BillingCountry = "USA"
                    };
                    string subscriptionProduct = "";
                    int SubscriptionType = Convert.ToInt32(Session["subscriptiontype"]);
                    if (SubscriptionType == 1)
                    {
                        subscriptionProduct = "standard";
                    }
                    else if (SubscriptionType == 2)
                    {
                        subscriptionProduct = "advanced";
                        // ViewBag.ProductName = product.Name ?? string.Empty;
                    }
                    else
                    {
                        subscriptionProduct = "enterprise";
                        // ViewBag.ProductName = product.Name ?? string.Empty;
                    }


                    try
                    {
                        var newSubscription = Chargify.CreateSubscription(subscriptionProduct, customerInfo, paymentAttributes);

                        //WebSecurity.Login(model.User.UserName, model.User.Password, false);
                        return RedirectToAction("Confirmation");
                    }
                    catch (ChargifyException ex)
                    {
                        if (ex.ErrorMessages.Count > 0)
                        {
                            ModelState.AddModelError("", ex.ErrorMessages.FirstOrDefault().Message);
                        }
                        else
                        {
                            ModelState.AddModelError("", ex.ToString());
                        }
                    }


                }
            }
            catch
            {
            }
            return RedirectToAction("Confirmation");
        }

        public ActionResult Confirmation()
        {
            int Stype = Convert.ToInt32(Session["subscriptiontype"]);
            if (Stype == 1)
            {
                ViewBag.Type = "STANDARD";
            }
            if (Stype == 2)
            {
                ViewBag.Type = "ADVANCED";
            }

            return View();
        }

       
        [HttpPost]
        public ActionResult Local(LocalSignup model, string id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Attempt to register the user
                    Guid userId = Guid.NewGuid();
                    // WebSecurity.CreateUserAndAccount(model.User.UserName, model.User.UserName);
                    //var newUser = WebSecurity.CreateUserAndAccount(model.User.UserName, model.User.UserName, new { Email = model.UserContact.EmailAddress, UserId = userId });
                    // MembershipCreateStatus createStatus;
                    //Membership.CreateUser(model.User.UserName, model.User.Password, model.UserContact.EmailAddress, null, null, true, null, out createStatus);
                    //if (createStatus == MembershipCreateStatus.Success)
                    //{
                    //if (!Roles.RoleExists("user"))
                    //{
                    //    Roles.CreateRole("user");
                    //}
                    //Roles.AddUsersToRoles(new string[] { model.User.UserName }, new string[] { "User" });

                    // Now that the user is created, attempt to create the corresponding subscription
                    var customerInfo = new CustomerAttributes()
                    {
                        FirstName = model.UserContact.FirstName,
                        LastName = model.UserContact.LastName,
                        Email = model.UserContact.EmailAddress,
                        SystemID = userId.ToString()
                    };

                    var paymentAttributes = new CreditCardAttributes()
                    {
                        FullNumber = model.UserPayment.CardNumber.Trim(),
                        CVV = model.UserPayment.CVV,
                        ExpirationMonth = model.UserPayment.ExpirationMonth,
                        ExpirationYear = model.UserPayment.ExpirationYear,
                        BillingAddress = model.UserAddress.Address,
                        BillingCity = model.UserAddress.City,
                        BillingZip = model.UserAddress.Zip,
                        BillingState = model.UserAddress.State,
                        BillingCountry = model.UserAddress.Country
                    };
                    string subscriptionProduct = "";
                    int SubscriptionType = Convert.ToInt32(Session["subscriptiontype"]);
                    if (SubscriptionType == 1)
                    {
                        subscriptionProduct = "standard";
                    }
                    else if (SubscriptionType == 2)
                    {
                        subscriptionProduct = "advanced";
                        // ViewBag.ProductName = product.Name ?? string.Empty;
                    }
                    else
                    {
                        subscriptionProduct = "enterprise";
                        // ViewBag.ProductName = product.Name ?? string.Empty;
                    }


                    try
                    {
                        var newSubscription = Chargify.CreateSubscription(subscriptionProduct, customerInfo, paymentAttributes);

                        //WebSecurity.Login(model.User.UserName, model.User.Password, false);
                        return RedirectToAction("Index", "UserHome");
                    }
                    catch (ChargifyException ex)
                    {
                        if (ex.ErrorMessages.Count > 0)
                        {
                            ModelState.AddModelError("", ex.ErrorMessages.FirstOrDefault().Message);
                        }
                        else
                        {
                            ModelState.AddModelError("", ex.ToString());
                        }
                    }
                    //}
                    //else
                    //{
                    //    //ModelState.AddModelError("", AccountValidation.ErrorCodeToString(createStatus));
                    //}
                }

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }


        #region Helpers
        private ChargifyConnect Chargify
        {
            get
            {
                ChargifyConnect chargify = new ChargifyConnect();
                chargify.apiKey = "Eo2SlTxx7usULMVRRKc";
                chargify.Password = "x";
                chargify.SharedKey = " 4qdVBYOxaBquGqHK9tDw";
                chargify.UseJSON = false;
                int sType = Convert.ToInt32(Session["subscriptiontype"]);
                if (sType == 1)
                {
                    if (HttpContext.Cache["Chargify"] == null)
                    {
                        //ChargifyAccountRetrieverSection config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                        //ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();                       
                        chargify.URL = "https://intelleges-2.chargify.com/";
                        HttpContext.Cache.Add("Chargify", chargify, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                    }
                }
                else if (sType == 2)
                {
                    if (HttpContext.Cache["Chargify"] == null)
                    {
                        //ChargifyAccountRetrieverSection config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                        //ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                        chargify.URL = "https://intelleges-2.chargify.com";
                        HttpContext.Cache.Add("Chargify", chargify, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                    }
                }
                else
                {
                    if (HttpContext.Cache["Chargify"] == null)
                    {
                        //ChargifyAccountRetrieverSection config = ConfigurationManager.GetSection("chargify") as ChargifyAccountRetrieverSection;
                        //ChargifyAccountElement accountInfo = config.GetDefaultOrFirst();
                        chargify.URL = "https://intelleges-2.chargify.com/";

                        HttpContext.Cache.Add("Chargify", chargify, null, System.Web.Caching.Cache.NoAbsoluteExpiration, System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.High, null);
                    }
                }

                return HttpContext.Cache["Chargify"] as ChargifyConnect;
            }
        }
        #endregion

    }
}
