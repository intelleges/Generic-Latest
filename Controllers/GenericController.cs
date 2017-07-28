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
			return Ok(db.pr_getGroupAll(enterpriseId).Select(o=>new  { o.active, o.author, o.dateCreated, o.description, o.email, o.enterprise, o.groupCollection, o.groupType, o.id,o.name,o.sortOrder, o.state }).ToList());
		}
        [Route("GetPartnerTypeAll")]
        [HttpGet]
        public IHttpActionResult GetPartnerTypeAll(int enterpriseId)
        {
            return Ok(db.pr_getPartnerTypeAll(enterpriseId).ToList());
        }
        [Route("GetTouchpointAllByEnterprise")]
        [HttpGet]
        public IHttpActionResult GetTouchpointAllByEnterprise(int enterpriseId)
        {
            return Ok(db.pr_getTouchpointAllByEnterprise(enterpriseId).ToList());
        }
        [Route("GetPersonAll")]
        [HttpGet]
        public IHttpActionResult GetPersonAll(int enterpriseId)
        {
            return Ok(db.pr_getPersonAll(enterpriseId).ToList());
        }
        [Route("GetCompanyProfileDataLoadForPartnerSpreadsheetDataLoad")]
        [HttpGet]
        public IHttpActionResult GetCompanyProfileDataLoadForPartnerSpreadsheetDataLoad()
        {
            return Ok(db.pr_getCompanyProfileDataLoadForPartnerSpreadsheetDataLoad().ToList());
        }
        [Route("AddCompanyProfileDataLoad")]
        [HttpPost]
        public IHttpActionResult AddCompanyProfileDataLoad(AddCompanyProfileDataLoadModel model)
        {
            return Ok(db.pr_addCompanyProfileDataLoad(model.ExternalId, model.CompanyName, model.JobAddress, model.JobCity, model.JobState, model.JobZipCode, model.JobCountry,model.AddDate, model.JobSource, model.PocSource, model.JobSnippet, model.JobOriginalSnippet, model.CompanyMainNumber, model.CompanyURL, model.SearchTerm, model.PocPhoneNumber, model.PocFirstName, model.PocLastName, model.PocTitle,model.PocEmailAddress, model.CompanyRevenue, model.CompanyEmployeeCount, model.IndustrySector, model.RelationshipOwner, model.PPTQ, model.SortOrder, model.Active).ToList());
        }
        [Route("AddPartnerSpreadsheetDataLoad")]
        [HttpPost]
        public IHttpActionResult AddPartnerSpreadsheetDataLoad(AddPartnerSpreadsheetDataLoadModel model)
        {
            return Ok(db.pr_addPartnerSpreadsheetDataLoad(model.PartnerInternalId, model.PartnerSupId, model.PartnerDunsNumber, model.PartnerName, model.PartnerAddressOne, model.PartnerAddressTwo, model.PartnerCity, model.PartnerState, model.PartnerZipCode, model.PartnerCountry, model.PartnerPocFirstName, model.PartnerPocLastName, model.PartnerPocTitle, model.PartnerPocPhoneNumber, model.PartnerPocEmailAddress, model.RoFirstName, model.RoLastName, model.RoEmail, model.DateLoaded, model.Enterprise, model.PartnerType, model.Touchpoint, model.Person, model.PartnerSpreadSheetDataLoad, model.LoadGroup, model.DueDate, model.Group).ToList());
        }
    }
}
