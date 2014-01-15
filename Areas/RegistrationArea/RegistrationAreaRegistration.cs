using System.Web.Mvc;

namespace Generic.Areas.RegistrationArea
{
    public class RegistrationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "RegistrationArea";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Registration_default",
                "Registration/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
