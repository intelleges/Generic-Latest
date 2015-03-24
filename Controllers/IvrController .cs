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

        public ActionResult IncomingCall(int? digits)
        {
            if (digits.HasValue)
            {
                switch (digits.Value)
                {
                    case 1:
                        return VoiceXml("2128515412");                        
                        break;
                    default:
                        return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Redirect method=\"POST\">https://www.intelleges.com/mvcmt/Generic/Ivr/IncomingCallXml</Redirect></Response>", "text/xml");
                        break;
                }
            }
            else
               return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Redirect method=\"POST\">https://www.intelleges.com/mvcmt/Generic/Ivr/IncomingCallXml</Redirect></Response>", "text/xml");
            
        }

        [HttpPost]        
        public ActionResult IncomingCallXml()
        {
            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response>   <Gather action=\"mvcmt/Generic/Ivr/IncomingCall\" timeout=\"10\" finishOnKey=\"*\" numDigits=\"1\">       <Say voice=\"woman\">Hello, Thanks for Calling Intelleges the communications platform that empowers you to accomplish your business objectives. </Say>       <Say voice=\"woman\"> If you are looking for Kyle Kononowitz press 1 </Say>   </Gather></Response>", "text/xml");
        }

        [HttpGet]
        public ActionResult VoiceXml(string number)
        {

            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Dial timeout=\"10\" record=\"true\">"+number+"</Dial></Response>", "text/xml");
        }
    }
}