using Generic.Models;
using Google.Apis.Customsearch.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;

namespace Generic.Controllers
{
    public class LinkedInController : Controller
    {
        // GET: LinkedIn
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateLinkedInSearchModel model)
        {
            var service = new Google.Apis.Customsearch.v1.CustomsearchService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleTranslateApiKey"]

            });

            var listRequest = service.Cse.List("site:linkedin.com/in OR site:linkedin.com/pub -pub.dir " + model.GetSearchString());
            listRequest.Cx = ConfigurationManager.AppSettings["GoogleCse"];
            List<Result> results = new List<Result>();
            IList<Result> paging = new List<Result>();
            var count = 0;
            try
            {
                while (paging != null)
                {
                    Console.WriteLine($"Page {count}");
                    listRequest.Start = count * 10 + 1;
                    var result = listRequest.Execute();
                    paging = result.Items;
                    if (paging != null)
                    {
                        results.AddRange(paging);
                    }
                    //foreach (var item in paging)
                    //    Console.WriteLine("Title : " + item.Title + Environment.NewLine + "Link : " + item.Link +
                    //                      Environment.NewLine + Environment.NewLine);
                    count++;
                }
            }
            catch
            {

            }


            Session["LinkedInResult"] = results;

            //listRequest.Num = 2200;
            //Google.Apis.Customsearch.v1.Data.Search search = listRequest.Execute();
            return View("CreateLinkedInSearchView", results);
        }

        public ActionResult ExportExcel()
        {
            List<Result> abc = Session["LinkedInResult"] as List<Result>;
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<LnResult>));

            serializer.Serialize(stream, abc.Select(o => new LnResult()
            {
                HtmlFormattedUrl = o.HtmlFormattedUrl,              
                Snippet = o.Snippet,
                Title = o.Title
            }).ToList());
            stream.Position = 0;
            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "LinkedIn.xls");
        }
    }
}