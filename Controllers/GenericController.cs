using Generic.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Generic;

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
	}
}
