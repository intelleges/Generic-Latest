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

        public ActionResult IncomingCall(int? digits)
        {
            if (digits.HasValue)
            {
                using (var db = new EntitiesDBContext())
                {
                    var callList = db.pr_getPersonAll(1).ToList();
                    if (callList.Count < digits.Value || digits.Value == 0)
                        return VoiceXml("9178488088");
                    else
                    {
                        var callPerson = callList.Take(digits.Value).Last();
                        if (callPerson != null)
                        {
                            return VoiceXml(callPerson.phone.Replace("-", "").Replace(" ", ""));
                        }
                        else return VoiceXml("9178488088");
                    }                   
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
            var gatherElement = new XElement("Gather",new XAttribute("action","IncomingCall"),new XAttribute("timeout",10),new XAttribute("finishOnKey","*"),new XAttribute("numDigits",1) );
            gatherElement.Add(GetSay("Hello, Thanks for Calling Intelleges the communications platform that empowers you to accomplish your business objectives."));
            var defaultNumber = 1;
            using (var db = new EntitiesDBContext())
            {
                foreach (var person in db.pr_getPersonAll(1).ToList())
                {
                    var text = defaultNumber == 1 ? string.Format("If you are looking for {0} press 1", person.FullName) : string.Format("{0} press {1}", person.FullName, defaultNumber);
                    gatherElement.Add(GetSay(text));
                    defaultNumber++;
                }
            }
            //gatherElement.Add(GetSay("Or press 0 to repeat the message"));
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
            //return Content("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Response>   <Gather action=\"IncomingCall\" timeout=\"10\" finishOnKey=\"*\" numDigits=\"1\">       <Say voice=\"woman\">Hello, Thanks for Calling Intelleges the communications platform that empowers you to accomplish your business objectives. </Say>       <Say voice=\"woman\">  </Say><Say voice=\"woman\">John Betancourt press 2 </Say> <Say voice=\"woman\">Daryl Davis press 3 </Say><Say voice=\"woman\">Vishal Patel press 4 </Say><Say voice=\"woman\">Technical Support press 5 </Say><Say voice=\"woman\">Sales press 6 </Say><Say voice=\"woman\">Billing Issues press 7 </Say><Say voice=\"woman\">Otherwise press 0 for a live operator </Say>  </Gather></Response>", "text/xml");
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