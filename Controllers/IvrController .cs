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
                        return VoiceXml("9178488088");                        
                        break;
                    default:
                        return VoiceXml("9178488088");
                        break;
                }
            }
            else
               return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Redirect method=\"POST\">https://www.intelleges.com/mvcmt/Generic/Ivr/IncomingCallXml</Redirect></Response>", "text/xml");
            
        }

        [HttpPost]        
        public ActionResult IncomingCallXml()
        {
            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response>   <Gather action=\"IncomingCall\" timeout=\"10\" finishOnKey=\"*\" numDigits=\"1\">       <Say voice=\"woman\">Hello, Thanks for Calling Intelleges the communications platform that empowers you to accomplish your business objectives. </Say>       <Say voice=\"woman\"> If you are looking for Kyle Kononowitz press 1 </Say><Say voice=\"woman\">John Betancourt press 2 </Say> <Say voice=\"woman\">Daryl Davis press 3 </Say><Say voice=\"woman\">Vishal Patel press 4 </Say><Say voice=\"woman\">Technical Support press 5 </Say><Say voice=\"woman\">Sales press 6 </Say><Say voice=\"woman\">Billing Issues press 7 </Say><Say voice=\"woman\">Otherwise press 0 for a live operator </Say>  </Gather></Response>", "text/xml");
        }

        [HttpGet]
        public ActionResult VoiceXml(string number)
        {

            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Dial timeout=\"10\" record=\"true\">"+number+"</Dial></Response>", "text/xml");
        }
    }
}