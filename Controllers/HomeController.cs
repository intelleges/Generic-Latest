using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Generic.Helpers;
using static Generic.Areas.RegistrationArea.Controllers.HomeController;

namespace Generic.Controllers
{
    public class HomeController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        public CustomMembershipProvider MembershipService { get; set; }
        public CustomRoleProvider AuthorizationService { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (MembershipService == null)
                MembershipService = new CustomMembershipProvider();
            if (AuthorizationService == null)
                AuthorizationService = new CustomRoleProvider();

            base.Initialize(requestContext);
        }

        [Authorize]
        public ActionResult eula() {
            List<enterprise> enterprise = db.pr_getEnterprise(1).ToList();
            var enterpriseLogo = enterprise.FirstOrDefault();
            byte[] logoBytes = new byte[0];
            var logo = enterpriseLogo != null ? enterpriseLogo.logo : logoBytes;
            string dirname = "~/uploadedFiles/EnterpriseLogo/";
            string logoImg = "";
            if (Directory.Exists(Server.MapPath(dirname)))
            {
                var fileName = enterpriseLogo != null ? enterpriseLogo.id + "Logo.png" : "Logo.png";
                var physicalPath = Path.Combine(Server.MapPath(dirname), fileName);
                if (!System.IO.File.Exists(physicalPath))
                {
                    var fs = new BinaryWriter(new FileStream(physicalPath, FileMode.Append, FileAccess.Write));
                    fs.Write(logo);
                    fs.Close();
                }
                string s = "https://www.intelleges.com/mvcmt/Generic/uploadedFiles/EnterpriseLogo/" + fileName;
                logoImg = "<img src='"+s+"' runat='server' style='height: 50px; width: 300px; border: none' />";
            }

            var items = db.pr_getLetter("EULA");
            byte[] bytes = null;
            string html = "<html><head></head><body>" + logoImg + "<br/><br/>" + items.First().body + "</body></html>";
            bytes = (new NReco.PdfGenerator.HtmlToPdfConverter()).GeneratePdf(html);
            return new BinaryContentResult(bytes, "application/pdf", "eula.pdf");
        }

        [Authorize]
        public virtual ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Logo(string fname)
        {
            var enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            var enterpriseLogo = enterprise.FirstOrDefault();
            byte[] logoBytes = new byte[0];
            var logo = enterpriseLogo != null ? enterpriseLogo.logo : logoBytes;
            return File(logo, System.Net.Mime.MediaTypeNames.Image.Jpeg, fname);
        }

        [HttpPost]
        public ActionResult AddEnterprise(FormCollection frmCollection)
        {
            try
            {
                string emailID = frmCollection["email"].ToString();
                string companyName = frmCollection["CompanyName"].ToString();
                string companyURL = frmCollection["CompanyURL"].ToString();

                //string Password = frmCollection["Password"].ToString();
                int industry = int.Parse(frmCollection["industry"].ToString());
                int focus = int.Parse(frmCollection["focus"].ToString());
                bool agreedToTerms = Boolean.Parse(frmCollection["agreedToTerms"]);

                Generic.Helpers.CurrentInstance.EnterpriseID = 1;

                var val = db.pr_addEnterpriseFreeTrial(emailID, companyName, companyURL, industry, focus, agreedToTerms);
              

              //  return Json(val, JsonRequestBehavior.AllowGet);

                if (ModelState.IsValid)
                {

                    //int enterpriseID = (int)addEnterpriseResult.FirstOrDefault().Value;
                    //  System.Data.Objects.ObjectResult<Decimal?> addpersonResult = db.pr_addPerson(enterpriseID, 1, 1, 1, 1, 1, "1", "1", "1", "New first name", "New lastname", "Mr", "", "", "", email, "", "", "", 1, "1", 1, "", "", 1, 1, null, null, null, null);
                    //  System.Data.Objects.ObjectResult<Decimal?> addPartnerResult = db.pr_addPartner(enterpriseID, "1", "New Person", "", "", "", 1, "", "", 1, "", "", "F", "L", "", email, "", "", 1, 1, 1, 1, DateTime.Now, true, null);
                    //  int partnerID = (int)addPartnerResult.FirstOrDefault().Value;
                    //  string accessCode = db.pr_getAccesscode().FirstOrDefault();

                    //  int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(1, 1).FirstOrDefault().id;

                    //  db.pr_addPartnerPartnertypeTouchpointQuestionnaire(partnerID, ptq, accessCode, 1, DateTime.Now, null, partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE, null, null, null, null, null);
                    //  db.pr_addPartnerPartnertypeTouchpointQuestionnaire(partnerID, ptq, accessCode, 1, DateTime.Now,DateTime.Now, partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE, 0, null, null, null, 0);
                    //  var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();

                    //  var objtouchpoint = db.pr_getTouchpoint(1).FirstOrDefault();
                    //  var objperson = db.pr_getPerson((int)addpersonResult.FirstOrDefault().Value).FirstOrDefault();
                    //  var objpartner = db.pr_getPartner(partnerID).FirstOrDefault();

                    //  Email objemail = new Email(amm);
                    //  EmailFormat emailFormat = new EmailFormat();
                    //  objemail.body = emailFormat.sGetEmailBody(objemail.body, objperson, objpartner, objtouchpoint, ptq);
                    //  objemail.emailTo = email;
                    //  SendEmail objSendEmail = new SendEmail();
                    //  objSendEmail.sendEmail(objemail);




                    //CODE commented by vishal -- START
                    //                    person objdefaultSystemMaster = db.pr_getSystemMaster(1).FirstOrDefault();


                    //                    int? PersonId = db.pr_addSystemMasterToEnterprise(enterpriseID, "1", "New Person", "1", emailID, "1", "1", 1).FirstOrDefault();

                    //                    System.Data.Objects.ObjectResult<Decimal?> addProtocolResult = db.pr_addProtocol(1, enterpriseID, "Default", "Default", "", "", "", "", 1, (int)PersonId, (int)PersonId, 1, "", "", "", null, null, "", "", "", null, null, null, 1, 1);

                    //                    int protocolID = (int)addProtocolResult.FirstOrDefault().Value;

                    //                    System.Data.Objects.ObjectResult<Decimal?> addTouchpointResult = db.pr_addTouchpoint(protocolID, PersonId, 1, 1, "Default", "Default", "", null, "", null, null, null, 1, 1);

                    //                    var objPerson = db.pr_getPerson((int)PersonId).FirstOrDefault();
                    //                    objPerson.campaign = (int)addTouchpointResult.FirstOrDefault().Value;
                    //                    db.Entry(objPerson).State = EntityState.Modified;
                    //                    db.SaveChanges();


                    //                    // Email Invite
                    //                    var objSystemMaster = db.pr_getPerson((int)PersonId).FirstOrDefault();

                    //                    enterprise objEnterprise = db.pr_getEnterprise(enterpriseID).FirstOrDefault();

                    //                    db.pr_addEnterpriseSystemInfo(null, null, null, "Admin", null, emailID, null, null, true, enterpriseID);



                    //                    autoMailMessage objamm = new autoMailMessage();

                    //                    objamm.subject = "Welcome to Intelleges for [Enterprise Name]: [Touchpoint Title]";
                    //                    //     objamm.text = "Dear " + objSystemMaster.firstName + "<br> please click on this <a href='https://www.intelleges.com/mvcmt/Generic'>hyperlink</a> and enter password " + objSystemMaster.passWord + " to login to the system.";
                    //                    objamm.text = @"Hi [User Firstname],<br>
                    //
                    //You have a new account at Intelleges for [Enterprise Name] [Touchpoint Title].<br>
                    //
                    //As you know the purpose of this system is to help us [Touchpoint Purpose].<br>
                    //
                    //Your username is [User Email]. The administrator has set your password [Temporary Access Code].<br>
                    //
                    //You can sign in to Intelleges services at:<br>
                    //
                    //[Project Url]<br>
                    //
                    //Note: you will need to change your password once you login.<br>
                    //
                    //If you have any questions please contact me [User Inviting Email] or contact your system master [System Master Email]<br>
                    //
                    //Thanks in advance.<br>
                    //
                    //[User Inviting Firstname] [User Inviting Last Name]<br>
                    //[Enterprise Name]";


                    //                    Email email = new Email(objamm);
                    //                    person objInvitingUser = db.pr_getPersonByEmail(1, "john@intelleges.com").FirstOrDefault();

                    //                    touchpoint objCurrentTouchpoint = db.pr_getTouchpoint(objInvitingUser.campaign).FirstOrDefault();

                    //                    EmailFormat emailFormat = new EmailFormat();
                    //                    email.subject = emailFormat.sGetEmailBody(email.subject, objInvitingUser, objSystemMaster, objCurrentTouchpoint, objEnterprise, objdefaultSystemMaster);
                    //                    //   email.body = emailFormat.sGetEmailBody(email.body, person, objpartner, objtouchpoint, ptq);
                    //                    email.body = emailFormat.sGetEmailBody(email.body, objInvitingUser, objSystemMaster, objCurrentTouchpoint, objEnterprise, objdefaultSystemMaster);
                    //                    //  email.body = objamm.text;
                    //                    email.emailTo = objSystemMaster.email;
                    //                    SendEmail objSendEmail = new SendEmail();

                    //                    objSendEmail.sendEmail(email);





                    //Guid userId = Guid.NewGuid();


                    //var customerInfo = new CustomerAttributes()
                    //{
                    //    FirstName = "First",
                    //    LastName = "Last",
                    //    Email = emailID,
                    //    SystemID = userId.ToString()
                    //};

                    //var paymentAttributes = new CreditCardAttributes()
                    //{
                    //    FullNumber = "1",
                    //    CVV = "123",
                    //    ExpirationMonth = 12,
                    //    ExpirationYear = 2018,
                    //    BillingAddress = "Address",
                    //    BillingCity = "City",
                    //    BillingZip = "1000100",
                    //    BillingState = "State",
                    //    BillingCountry = "USA"
                    //};
                    //string subscriptionProduct = "";
                    //int SubscriptionType = Convert.ToInt32(Session["subscriptiontype"]);
                    //if (SubscriptionType == 1)
                    //{
                    //    subscriptionProduct = "standard";
                    //}
                    //else if (SubscriptionType == 2)
                    //{
                    //    subscriptionProduct = "advanced";
                    //    // ViewBag.ProductName = product.Name ?? string.Empty;
                    //}
                    //else
                    //{
                    //    subscriptionProduct = "enterprise";
                    //    // ViewBag.ProductName = product.Name ?? string.Empty;
                    //}


                    //try
                    //{
                    //    var newSubscription = Chargify.CreateSubscription(subscriptionProduct, customerInfo, paymentAttributes);

                    //    //WebSecurity.Login(model.User.UserName, model.User.Password, false);
                    //    return RedirectToAction("Confirmation");
                    //}
                    //catch (ChargifyException ex)
                    //{
                    //    if (ex.ErrorMessages.Count > 0)
                    //    {
                    //        ModelState.AddModelError("", ex.ErrorMessages.FirstOrDefault().Message);
                    //    }
                    //    else
                    //    {
                    //        ModelState.AddModelError("", ex.ToString());
                    //    }
                    //}
                    //CODE commented by vishal -- END

                }
            }
            catch
            {
                return RedirectToAction("Index");
            }
            return RedirectToAction("Confirmation");
        }


    }
}
