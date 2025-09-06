using Generic.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Generic;
using Generic.Models;
using System.Web.Http.Description;
using Swashbuckle.Swagger.Annotations;
using System.Web.Http.ModelBinding;
using Generic.Helpers.PartnerHelper;
using System.Data.Entity;
using Generic.Helpers.Utility;
using Generic.ViewModel;
using System.Text;
using Newtonsoft.Json;
using System.Web;
using System.Text.RegularExpressions;
using System.Data.Entity.Core.Objects;

namespace Generic.Controllers
{
    [RoutePrefix("api/hric/registrations")]
    [HricApiAuthorize]
    public class RegistrationController : ApiController
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        [Route("initiate")]
        [HttpPost]
        [SwaggerResponse(200, "OK", Type = typeof(HricRegistrationResponse))]
        [SwaggerResponse(400, "Bad Request", Type = typeof(ModelStateDictionary))]
        public IHttpActionResult initiate(HricRegistrationRequest model)
        {

            var result = new HricRegistrationResponse();
            try
            {
                var apiKey = HttpContext.Current.Items["HRIC_API_KEY"] as string;
                var requestId = HttpContext.Current.Items["HRIC_REQUEST_ID"] as string;
                var timestamp = HttpContext.Current.Items["HRIC_TIMESTAMP"] as string;
                // Generate unique registration ID
               
                if (ModelState.IsValid)
                {
                    var registrationId = GenerateRegistrationId();

                    // Create verification session in database
                    var verificationSession = CreateVerificationSession(
                        registrationId,
                        model.Pptq,
                        model.Tier,
                       model.Email,
                        model.Source,
                        apiKey,
                        requestId
                    );
                    var outId = new ObjectParameter("Outid", typeof(int));
                    var outLink = new ObjectParameter("OutLink", typeof(string));
                    var outStatus = new ObjectParameter("OutStatus", typeof(string));
                    var outExpires = new ObjectParameter("OutExpiresAtUtc", typeof(DateTime));
                    db.sp_HricRegistration_Initiate(model.Pptq, model.Email, model.Country, model.Tier, model.IdempotencyKey, model.Source,
                       model.RedirectBaseUrl, model.Locale, model.LinkTtlMinutes, outId, outLink, outStatus, outExpires);
                    var pptqdetail = db.partnerPartnertypeTouchpointQuestionnaire.Where(x => x.id == model.Pptq).FirstOrDefault();
                    var touchpoint = db.partnerTypeTouchpointQuestionnaire.Where(x => x.id == pptqdetail.partnerTypeTouchpointQuestionnaire).Select(x => x.touchpoint).FirstOrDefault();


                    var partnerType = db.partnerType.Where(x => x.name == "HRIC_STARTUP_BASIC_UNVERIFIED").FirstOrDefault();
                    var objptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnerType(partnerType.id)
                      .ToList().Where(x => x.touchpoint == touchpoint).FirstOrDefault();
                    var random = new Random();
                    string internalID = random.Next(0, 1000000).ToString();
                    //var dbRecord = db.pr_getPartnerByInternalIDAndPTQ(internalID, objptq.id).FirstOrDefault();
                    var objtouchpoint = db.pr_getTouchpoint(touchpoint).FirstOrDefault();

                    string loadGroup = db.pr_getAccesscode().FirstOrDefault();
                    int? enterprise = db.protocol.Where(x => x.id == objtouchpoint.protocol).Select(x => x.enterprise).FirstOrDefault();
                    int? group = db.pptqGroup.Where(x => x.pptq == model.Pptq).Select(x => x.group).FirstOrDefault();
                    string emial= "john@intelleges.com";
                    int? PartnerId = (int)db.pr_addPartnerSpreadsheetDataLoad(internalID, "", "12345", "FirstName LastName", "address 1 - test", "address 2 - test", "city -test", "1", "123456", "2", "FirstName -Test", "LastName -Test",
                        "Compliance manager", "1234567890", model.Email, "", "", "", DateTime.Now, enterprise, partnerType.id, touchpoint, 13224, (int)PartnerStatus.Loaded, loadGroup, DateTime.UtcNow, group).ToList().FirstOrDefault();
                    var objPartners = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).ToList();
                    int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnerType.id, touchpoint).LastOrDefault().id;
                    string accesscode = "";
                    foreach (var partnerItem in objPartners)
                    {
                        var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerItem.partner, ptq).FirstOrDefault();
                        accesscode = pptq.accesscode;
                        var pptqstatus = db.partnerPartnertypeTouchpointQuestionnaire.Where(x => x.accesscode == accesscode).FirstOrDefault();
                        if (pptqstatus != null)
                        {
                            pptqstatus.status = (int)PartnerStatus.Confirmed;
                            db.Entry(pptqstatus).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        PartnerId = partnerItem.partner;
                    }
                    int ptqdetail = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnerType.id, touchpoint).LastOrDefault().id;



                    var pptqdetail_updated = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accesscode).FirstOrDefault();

                    if (pptqdetail_updated != null)
                    {
                        pptqdetail_updated.invitedDate = DateTime.Now;
                        var person = db.pr_getPersonByEmail(enterprise, emial).FirstOrDefault();
                        pptqdetail_updated.invitedBy = person.id;
                        pptqdetail_updated.status = (int)PartnerStatus.Invited_NoResponse;
                        db.Entry(pptqdetail_updated).State = EntityState.Modified;
                        db.SaveChanges();

                        db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireDueDate(pptqdetail_updated.id, DateTime.UtcNow);

                        var objpartner = db.pr_getPartner(PartnerId).FirstOrDefault();
                        objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                        db.Entry(objpartner).State = EntityState.Modified;
                        db.SaveChanges();
                        var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptqdetail).FirstOrDefault();
                        var objpartnerByAccessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireDueDateByAccessCode(pptqdetail_updated.accesscode, loadGroup).FirstOrDefault();
                        var p = db.pr_getPerson(objpartner.owner).FirstOrDefault();

                        amm.text.Replace("[partner Access Code]", loadGroup);
                        amm.text = amm.text.Replace("[Due Date]", DateTime.UtcNow.ToString("MMM, dd, yyyy"));
                        amm.text = amm.text.Replace("[due date]", DateTime.UtcNow.ToString("MMM, dd, yyyy"));

                        Email email = new Email(amm);
                        email.loadgroup = loadGroup;
                        email.accesscode = pptqdetail_updated.accesscode;
                        email.protocolTouchpoint = objtouchpoint.description;
                        EmailFormat emailFormat = new EmailFormat();


                        email.subject = emailFormat.sGetEmailBody(amm.subject, p, objpartner, pptqdetail_updated.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, objtouchpoint, ptq);
                        email.body = emailFormat.sGetEmailBody(email.body, p, objpartner, pptqdetail_updated.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, objtouchpoint, ptq);

                        email.emailTo = model.Email;
                        email.url = Request.RequestUri.AbsolutePath;
                        email.automailMessage = amm.id.ToString();
                        email.category = SendGridCategory.InvitePartnes;
                        SendEmail objSendEmail = new SendEmail();
                        objSendEmail.sendEmail(email, new EmailFormatSettings() { sender = p, enterprise = pptqdetail_updated.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, ptq = ptq, partner = objpartner, touchpoint = objtouchpoint }, new System.Net.Mail.MailAddress(p.email, p.firstName + " " + p.lastName));
                    }
                    result.email = model.Email;
                    result.ExpiresAtUtc = Convert.ToDateTime(outExpires.Value);
                    result.product_tier = model.Tier;
                    result.questionnaire_link = Convert.ToString(outLink.Value);
                    result.registration_id = Convert.ToString(outId.Value);
                    result.status = "QUESTIONNAIRE_LINK_ISSUED";

                    return Ok(result);
                }
                else return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                // Log error
                LogError("HricInitiate", ex, HttpContext.Current.Items["HRIC_REQUEST_ID"] as string);
                return InternalServerError();
            }
        }
        private void LogError(string method, Exception ex, string requestId)
        {
            System.Diagnostics.Debug.WriteLine($"[HRIC Error] Method: {method}, Request ID: {requestId}, Error: {ex.Message}");
        }
        private string GenerateRegistrationId()
        {
            return $"hric_{DateTime.UtcNow:yyyyMMdd}_{Guid.NewGuid():N}";
        }
        private HricVerificationSession CreateVerificationSession(string registrationId, int questionnaireId, string productTier, string userEmail, string companyName, string apiKey, string requestId)
        {
            // Create session record in database
            var session = new HricVerificationSession
            {
                RegistrationId = registrationId,
                QuestionnaireId = questionnaireId,
                ProductTier = productTier,
                UserEmail = userEmail,
                CompanyName = companyName,
                Status = "INITIATED",
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                ApiKey = apiKey,
                RequestId = requestId
            };

            return session;
        }
        [Route("status")]
        [HttpGet]
        [SwaggerResponse(200, "OK", Type = typeof(HricRegistrationStatusResponse))]
        [SwaggerResponse(404, "Not found")]
        public IHttpActionResult statusByEmail(string email)
        {
            try
            {
                // Get authentication context
                var apiKey = HttpContext.Current.Items["HRIC_API_KEY"] as string;
                var requestId = HttpContext.Current.Items["HRIC_REQUEST_ID"] as string;

                var status = db.sp_HricRegistration_GetStatus(email).FirstOrDefault();
                if (status != null)
                {
                    var result = new HricRegistrationStatusResponse()
                    {
                        email = status.Email,
                        status = status.Status,
                        product_tier = status.ProductTier,
                        registration_id = Convert.ToString(status.id),
                        last_updated_utc = status.UpdatedAtUtc
                    };
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                LogError("statusByEmail", ex, HttpContext.Current.Items["HRIC_REQUEST_ID"] as string);
                return InternalServerError();
            }
        }

        [Route("registrations")]
        [HttpGet]
        [SwaggerResponse(200, "OK", Type = typeof(HricRegistrationStatusResponse))]
        [SwaggerResponse(404, "Not found")]
        public IHttpActionResult statusById(string id)
        {
            try
            {
                var status = db.sp_HricRegistration_GetStatus(id).FirstOrDefault();
                if (status != null)
                {
                    var result = new HricRegistrationStatusResponse()
                    {
                        email = status.Email,
                        status = status.Status,
                        product_tier = status.ProductTier,
                        registration_id = Convert.ToString(status.id),
                        last_updated_utc = status.UpdatedAtUtc
                    };
                    return Ok(result);
                }
                else
                {
                    return NotFound();
                }
            }
            catch(Exception ex)
            {
                LogError("statusById", ex, HttpContext.Current.Items["HRIC_REQUEST_ID"] as string);
                return InternalServerError();
            }
        }

        public class HricVerificationSession
        {
            public int Id { get; set; }
            public string RegistrationId { get; set; }
            public int QuestionnaireId { get; set; }
            public string ProductTier { get; set; }
            public string UserEmail { get; set; }
            public string CompanyName { get; set; }
            public string Status { get; set; }
            public DateTime CreatedAt { get; set; }
            public DateTime? CompletedAt { get; set; }
            public DateTime ExpiresAt { get; set; }
            public int? CompletionScore { get; set; }
            public string ApiKey { get; set; }
            public string RequestId { get; set; }
        }
    }
}
