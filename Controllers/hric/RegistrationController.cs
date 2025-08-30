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
    [BasicHttpAuthorize]
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
            if (ModelState.IsValid)
            {
                var outId = new ObjectParameter("Outid", typeof(int));
                var outLink = new ObjectParameter("OutLink", typeof(string));
                var outStatus = new ObjectParameter("OutStatus", typeof(string));
                var outExpires = new ObjectParameter("OutExpiresAtUtc", typeof(DateTime));
                 db.sp_HricRegistration_Initiate(model.Pptq, model.Email, model.Country, model.Tier, model.IdempotencyKey, model.Source,
                    model.RedirectBaseUrl, model.Locale, model.LinkTtlMinutes, outId,outLink,outStatus,outExpires);
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
                int? PartnerId = (int)db.pr_addPartnerSpreadsheetDataLoad(internalID, "", "12345", "FirstName LastName", "address 1 - test", "address 2 - test", "city -test", "Alaska", "123456", "United States", "FirstName -Test", "LastName -Test",
                    "Compliance manager", "1234567890", model.Email, "", "", "", DateTime.Now, enterprise, partnerType.id, touchpoint, db.pr_getPersonByEmail(enterprise, User.Identity.Name).FirstOrDefault().id, (int)PartnerStatus.Loaded, loadGroup, DateTime.UtcNow, group).ToList().FirstOrDefault();
                var objPartners = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(loadGroup).ToList();
                int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnerType.id, touchpoint).LastOrDefault().id;
                foreach (var partnerItem in objPartners)
                {
                    var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerItem.partner, ptq).FirstOrDefault();
                }
                int ptqdetail = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnerType.id, touchpoint).LastOrDefault().id;

                var pptqdetail_updated = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(loadGroup).FirstOrDefault();

                if (pptqdetail_updated != null)
                {
                    pptqdetail_updated.invitedDate = DateTime.Now;
                    var person = db.pr_getPersonByEmail(enterprise, User.Identity.Name).FirstOrDefault();
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
                result.ExpiresAtUtc =Convert.ToDateTime( outExpires);
                result.product_tier = model.Tier;
                result.questionnaire_link =Convert.ToString( outLink);
                result.registration_id =Convert.ToString( outId);
                result.status = "QUESTIONNAIRE_LINK_ISSUED";
                
                return Ok(result);
            }
            else return BadRequest(ModelState);
        }

        [Route("status")]
        [HttpGet]
        [SwaggerResponse(200, "OK", Type = typeof(HricRegistrationStatusResponse))]
        [SwaggerResponse(404, "Not found")]
        public IHttpActionResult statusByEmail(string email)
        {
            var status = db.sp_HricRegistration_GetStatus(email).FirstOrDefault();
            if (status != null)
            {
                var result = new HricRegistrationStatusResponse()
                {
                   email= status.Email,
                   status= status.Status,
                   product_tier= status.ProductTier,
                   registration_id=Convert.ToString( status.id),
                   last_updated_utc= status.UpdatedAtUtc
                };
                return Ok(result);
            }
            else
            {
                return NotFound();
            }
        }

        [Route("registrations")]
        [HttpGet]
        [SwaggerResponse(200, "OK", Type = typeof(HricRegistrationStatusResponse))]
        [SwaggerResponse(404, "Not found")]
        public IHttpActionResult statusById(string id)
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


    }
}
