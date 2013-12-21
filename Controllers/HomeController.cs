using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Generic.Helpers;

namespace Generic.Controllers
{
    public class HomeController : Controller
    {
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
        public virtual ActionResult Index()
        {
            return View();
        }

    }
}
