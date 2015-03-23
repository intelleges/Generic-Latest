using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Twilio.TwiML;
using Twilio.TwiML.Mvc;

namespace Generic.Controllers
{
    [AllowAnonymous]
    public class IvrController : TwilioController
    {

        public ActionResult Index()
        {
            UrlHelper helper = new UrlHelper(HttpContext.Request.RequestContext);

            var twiml = new TwilioResponse();
           // twiml.Play(helper.ContentAbsolute("~/Content/Audio/greeting.mp3"));
            return TwiML(twiml);
        }

        public ActionResult IncomingCall(int digits)
        {
            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Dial timeout=\"10\" record=\"true\">" + number + "</Dial></Response>", "text/xml");
        }

        [HttpGet]
        public ActionResult VoiceXml(string number)
        {

            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Dial timeout=\"10\" record=\"true\">"+number+"</Dial></Response>", "text/xml");
        }
    }
}