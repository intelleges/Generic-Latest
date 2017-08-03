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

namespace Generic.Controllers
{
    [RoutePrefix("api/Generic")]
    [BasicHttpAuthorize]
    public class GenericController : ApiController
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        [Route("ZcodeByAccessCode")]
        [HttpGet]
        public IHttpActionResult ZcodeByAccessCode(string accessCode)
        {
            return Ok(db.pr_getZcodeByAccessCode(accessCode).FirstOrDefault());
            //throw new NotImplementedException();
            //return Ok(db.pr_getzc)
        }

        [Route("PDFByAccessCode")]
        [HttpGet]
        public IHttpActionResult PDFByAccessCode(string accessCode)
        {
            return Ok(db.pr_getPDFByAccessCode(accessCode).FirstOrDefault());
        }

        [Route("AttachmentsByAccessCode")]
        [HttpGet]
        public IHttpActionResult AttachmentsByAccessCode(string accessCode)
        {
            return Ok(db.pr_getAttachmentsByAccessCode(accessCode).ToList());
        }

        [Route("CommentByAccessCode")]
        [HttpGet]
        public IHttpActionResult CommentByAccessCode(string accessCode)
        {
            return Ok(db.pr_getCommentByAccessCode(accessCode).ToList());
        }

        [Route("ResponseByAccessCode")]
        [HttpGet]
        public IHttpActionResult ResponseByAccessCode(string accessCode)
        {
            return Ok(db.pr_getResponseByAccessCode(accessCode).ToList());
        }
        [Route("EmailInviteSlackAlertByAccessCode")]
        [HttpGet]
        public IHttpActionResult EmailInviteSlackAlertByAccessCode(string accessCode, string email)
        {
            return Ok(db.pr_EmailInviteSlackAlertByAccessCode(accessCode, email).ToList());
        }
        [Route("GetGroupAll")]
        [HttpGet]
        public IHttpActionResult GetGroupAll(int enterpriseId)
        {
            return Ok(db.pr_getGroupAll(enterpriseId).Select(o => new { o.active, o.author, o.dateCreated, o.description, o.email, o.enterprise, o.groupCollection, o.groupType, o.id, o.name, o.sortOrder, o.state }).ToList());
        }
        [Route("GetPartnerTypeAll")]
        [HttpGet]
        public IHttpActionResult GetPartnerTypeAll(int enterpriseId)
        {
            return Ok(db.pr_getPartnerTypeAll(enterpriseId).Select(o => new { o.active, o.alias, o.description, o.enterprise, o.id, o.name, o.partnerClass, o.sortOrder }).ToList());
        }
        [Route("GetTouchpointAllByEnterprise")]
        [HttpGet]
        public IHttpActionResult GetTouchpointAllByEnterprise(int enterpriseId)
        {
            return Ok(db.pr_getTouchpointAllByEnterprise(enterpriseId).Select(o => new { o.abbreviation, o.active, o.admin, o.automaticReminder, o.description, o.endDate, o.id, o.person, o.protocol, o.sortOrder, o.sponsor, o.startDate, o.title }).ToList());
        }
        [Route("GetPersonAll")]
        [HttpGet]
        public IHttpActionResult GetPersonAll(int enterpriseId)
        {
            return Ok(db.pr_getPersonAll(enterpriseId).Select(o => new { o.active, o.address1, o.address2, o.archivedDate, o.campaign, o.city, o.country, o.email, o.enterprise, o.fax, o.firstName, o.FullName, o.GroupId, o.id, o.internalId, o.IsArchived, o.ismanager, o.lastName, o.loadHistory, o.manager, o.nickName, o.nmNumber, o.partnerPerPage, o.passWord, o.personStatus, o.phone, o.resetDate, o.riskType, o.RoleId, o.socialSecurity, o.state, o.suffix, o.title, o.zipcode }).ToList());
        }
        [Route("GetCompanyProfileDataLoadForPartnerSpreadsheetDataLoad")]
        [HttpGet]
        public IHttpActionResult GetCompanyProfileDataLoadForPartnerSpreadsheetDataLoad()
        {
            return Ok(db.pr_getCompanyProfileDataLoadForPartnerSpreadsheetDataLoad().ToList());
        }
        [Route("AddCompanyProfileDataLoad")]
        [HttpPost]
        [SwaggerResponse(200, "OK", Type = typeof(int))]
        [SwaggerResponse(400, "Bad Request", Type = typeof(ModelStateDictionary))]
        public IHttpActionResult AddCompanyProfileDataLoad(AddCompanyProfileDataLoadModel model)
        {
            if (ModelState.IsValid)
            {
                return Ok(db.pr_addCompanyProfileDataLoad(model.ExternalId, model.CompanyName, model.JobAddress, model.JobCity, model.JobState, model.JobZipCode, model.JobCountry, model.AddDate, model.JobSource, model.PocSource, model.JobSnippet, model.JobOriginalSnippet, model.CompanyMainNumber, model.CompanyURL, model.SearchTerm, model.PocPhoneNumber, model.PocFirstName, model.PocLastName, model.PocTitle, model.PocEmailAddress, model.CompanyRevenue, model.CompanyEmployeeCount, model.IndustrySector, model.RelationshipOwner, model.PPTQ, model.SortOrder, model.Active).ToList());
            }
            else return BadRequest(ModelState);
        }
        [Route("AddPartnerSpreadsheetDataLoad")]
        [HttpPost]
        [SwaggerResponse(200,"OK", Type=typeof(List<string>))]
        [SwaggerResponse(400,"Bad Request", Type = typeof(ModelStateDictionary))]
        public IHttpActionResult AddPartnerSpreadsheetDataLoad(AddPartnerSpreadsheetDataLoadModel model)
        {
            if (ModelState.IsValid)
            {
                int? PartnerId = db.pr_addPartnerSpreadsheetDataLoad(model.PartnerInternalId, model.PartnerSupId, model.PartnerDunsNumber, model.PartnerName, model.PartnerAddressOne, model.PartnerAddressTwo, model.PartnerCity, model.PartnerState, model.PartnerZipCode, model.PartnerCountry, model.PartnerPocFirstName, model.PartnerPocLastName, model.PartnerPocTitle, model.PartnerPocPhoneNumber, model.PartnerPocEmailAddress, model.RoFirstName, model.RoLastName, model.RoEmail, model.DateLoaded, model.Enterprise, model.PartnerType, model.Touchpoint, model.Person, model.PartnerSpreadSheetDataLoad, model.LoadGroup, model.DueDate, model.Group).FirstOrDefault();
                int ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(model.PartnerType, model.Touchpoint).FirstOrDefault().id;
                var objPartners = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(model.LoadGroup).ToList();

                List<string> rtn = new List<string>();
                rtn.Add("pr_addPartnerSpreadsheetDataLoad back id = " + PartnerId);

                foreach (var partnerItem in objPartners)
                {
                    var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerItem.partner, ptq).FirstOrDefault();
                    pptq.invitedDate = DateTime.Now;
                    var person = db.pr_getPersonByEmail(model.Enterprise, User.Identity.Name).FirstOrDefault();
                    pptq.invitedBy = person.id;
                    pptq.status = (int)PartnerStatus.Invited_NoResponse;
                    db.Entry(pptq).State = EntityState.Modified;
                    db.SaveChanges();
                    var objpartner = db.pr_getPartner(partnerItem.partner).FirstOrDefault();
                    objpartner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                    db.Entry(objpartner).State = EntityState.Modified;
                    db.SaveChanges();

                    var status = db.pr_checkPartnerStatus(partnerItem.partner).FirstOrDefault();

                    if (status == true)
                    {
                        rtn.Add("partner = " + partnerItem.partner + " email sent -- partner is active");

                        var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypes.Invitation, ptq).FirstOrDefault();
                        if (amm != null)
                        {
                            amm.text.Replace("[partner Access Code]", partnerItem.accesscode);
                            var objpartnerByAccessCode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireDueDateByAccessCode(partnerItem.accesscode, model.LoadGroup).FirstOrDefault();
                            var objtouchpoint = db.pr_getTouchpoint(model.Touchpoint).FirstOrDefault();
                            Email email = new Email(amm);
                            email.loadgroup = model.LoadGroup;
                            email.accesscode = partnerItem.accesscode;
                            email.protocolTouchpoint = objtouchpoint.description;
                            EmailFormat emailFormat = new EmailFormat();

                            var p = db.pr_getPerson(objpartner.owner).FirstOrDefault();

                            email.subject = emailFormat.sGetEmailBody(amm.subject, p, objpartner, pptq.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, objtouchpoint, ptq);
                            email.body = emailFormat.sGetEmailBody(email.body, p, objpartner, pptq.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, objtouchpoint, ptq);

                            email.emailTo = objpartner.email;
                            email.url = Request.RequestUri.AbsolutePath;
                            email.automailMessage = amm.id.ToString();
                            email.category = SendGridCategory.InvitePartnes;
                            SendEmail objSendEmail = new SendEmail();
                            objSendEmail.sendEmail(email, new EmailFormatSettings() { sender = p, enterprise = pptq.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, ptq = ptq, partner = objpartner, touchpoint = objtouchpoint }, new System.Net.Mail.MailAddress(p.email, p.firstName + " " + p.lastName));
                        }
                    }
                    else {

                        rtn.Add("partner = " + partnerItem.partner + " partner is inactive");
                    }
                }


                return Ok(rtn);
            }
            else return BadRequest(ModelState);
        }
    }
}
