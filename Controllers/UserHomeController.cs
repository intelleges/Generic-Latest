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
namespace BAA.Controllers
{
    public class UserHomeController : Controller
    {
        //
        // GET: /UserHome/
        private EntitiesDBContext db = new EntitiesDBContext();
        public ActionResult Index(string query, ProductTypeModel model, int? SubscriptionType)
        {
            
            if (query == "cost")
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
                }
                else if (SubscriptionType == 2)
                {
                    var product = Chargify.LoadProduct("advanced");
                    ViewBag.ProductName = product.Name ?? string.Empty;
                }
                else
                {
                    var product = Chargify.LoadProduct("enterprise");
                    ViewBag.ProductName = product.Name ?? string.Empty;
                }

                ViewBag.userCount = model.userCount;
                ViewBag.partner = model.partnerCount;
                ViewBag.part = model.partnumberCount;
                ViewBag.Query = "AccountInfo";
                ViewBag.CalculatedCost = db.pr_getCalculatedCostForEnterpriseSubscription(model.userCount, model.partnerCount, model.partnumberCount, SubscriptionType).FirstOrDefault();
        
            
            }
            
            return View();
        }

        
        [HttpPost]
        public ActionResult Signup(LocalSignup model, string id)
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

                    Session["subscriptionProduct"] = subscriptionProduct;
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

        public ActionResult Confirmation()
        { 
          return View();
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
                else {
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
