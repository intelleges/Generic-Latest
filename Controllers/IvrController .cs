using System;
using System.Collections.Generic;
using System.IO;
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

        public ActionResult IncomingCall(string digits)
        {
            if (!string.IsNullOrEmpty(digits))
            {
                int digit = 0;
                if (int.TryParse(digits, out digit))
                {
                    using (var db = new EntitiesDBContext())
                    {
                        var callList = db.pr_getPersonAll(1).ToList();
                        if (callList.Count < digit || digit == 0 || digit == 9)
                            return VoiceXml("9178488088");
                        else
                        {
                            var callPerson = callList.Take(digit).Last();
                            if (callPerson != null)
                            {
                                return VoiceXml(callPerson.phone.Replace("-", "").Replace(" ", ""));
                            }
                            else return VoiceXml("9178488088");
                        }
                    }
                }
                else
                {
                    return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Redirect method=\"POST\">https://www.intelleges.com/mvcmt/Generic/Ivr/IncomingCallXml</Redirect></Response>", "text/xml");
                }
            }
            else
                return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Redirect method=\"POST\">https://www.intelleges.com/mvcmt/Generic/Ivr/IncomingCallXml</Redirect></Response>", "text/xml");
            
        }

        [HttpPost]        
        public ActionResult IncomingCallXml()
        {
            XDocument doc = new XDocument();
            XElement rootElement = new XElement(XName.Get("Response"));
            var gatherElement = new XElement("Gather",new XAttribute("action","IncomingCall"),new XAttribute("timeout",20),new XAttribute("finishOnKey","#"),new XAttribute("numDigits",1) );
            gatherElement.Add(GetSay("Hello, Thanks for Calling Intelleges the communications platform that empowers you to accomplish your business objectives."));
            gatherElement.Add(GetSay("If you need an operator dial 0"));
            var defaultNumber = 1;
            using (var db = new EntitiesDBContext())
            {
                foreach (var person in db.pr_getPersonAll(1).ToList().Take(7))
                {
                    var text = defaultNumber == 1 ? string.Format("If you are looking for {0} press 1", person.FullName) : string.Format("{0} press {1}", person.FullName, defaultNumber);
                    gatherElement.Add(GetSay(text));
                    defaultNumber++;
                }
            }
            gatherElement.Add(GetSay("If you need tech support dial 9"));
            gatherElement.Add(GetSay("If you would like to hear this message again press an asterisk"));
            gatherElement.Add(GetSay("If you would like to finish this call press #"));
            rootElement.Add(gatherElement);
            doc.Add(rootElement);
            string result = "";
            using (var sw = new MemoryStream())
            {
                using (var strw = new StreamWriter(sw, System.Text.UTF8Encoding.UTF8))
                {
                    doc.Save(strw);
                    result = System.Text.UTF8Encoding.UTF8.GetString(sw.ToArray());
                }
            }
            return Content(result, "text/xml");
        }

        private XElement GetSay(string text)
        {
            const string voice = "woman";
            return new XElement("Say",text,new XAttribute("voice",voice));
        }

        [HttpGet]
        public ActionResult VoiceXml(string number)
        {

            return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response><Dial timeout=\"30\" record=\"true\">"+number+"</Dial></Response>", "text/xml");
        }
    }
}