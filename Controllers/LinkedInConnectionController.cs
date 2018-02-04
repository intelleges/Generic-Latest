using FileHelpers;
using Generic.SessionClass;
using Generic.ViewModel;
using Google.Apis.Customsearch.v1.Data;
//using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class LinkedInConnectionController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        // GET: LinkedInConnection
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(HttpPostedFileBase currentFile)
        {
            HttpContext.Server.ScriptTimeout = 500000000;
            Session.Timeout = 120;
            var fileData = new StreamReader(currentFile.InputStream).ReadToEnd();
            var engine = new FileHelperEngine<LinkedInCsvConnection>();
            var results = engine.ReadString(fileData);
            int count = 0;
            var service = new Google.Apis.Customsearch.v1.CustomsearchService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleTranslateApiKey"]

            });
            foreach (var result in results)
            {

                if (count >= 100)
                {
                    Thread.Sleep(1000 * 101);
                    count = 0;
                }
                DateTime connectedOn;
                var useDate = DateTime.TryParse(result.ConnectedOn.Replace(",", ""), out connectedOn);
                var listRequestForPerson = service.Cse.List("site:linkedin.com/in OR site:linkedin.com/pub -pub.dir " + result.GetCSEQueryForPerson());
                listRequestForPerson.Cx = ConfigurationManager.AppSettings["GoogleCse"];
                var listExecuteResultPerson = listRequestForPerson.Execute();
                var listRequestForCompant = service.Cse.List("site:linkedin.com/in OR site:linkedin.com/pub -pub.dir " + result.GetSCEQueryForCompany());
                listRequestForCompant.Cx = ConfigurationManager.AppSettings["GoogleCse"];
                var listExecuteResultCompany = listRequestForPerson.Execute();
                var listRequestForCompanyUrl = service.Cse.List(result.Company + " company");
                listRequestForCompanyUrl.Cx = ConfigurationManager.AppSettings["GoogleCse"];
                var listExecuteResultCompanyUrl = listRequestForCompanyUrl.Execute();
                if (listExecuteResultPerson.Items != null && listExecuteResultPerson.Items.Any() && listExecuteResultCompany.Items != null && listExecuteResultCompany.Items.Any())
                {
                    var dataCompany = listExecuteResultCompany.Items.FirstOrDefault();
                    Result dataCOmpanyUrl = null, dataContactUs = null; ;
                    if (listExecuteResultCompanyUrl.Items != null && listExecuteResultCompanyUrl.Items.Any())
                    {
                        dataCOmpanyUrl = listExecuteResultCompanyUrl.Items.FirstOrDefault();
                        var listRequestForCompanyContactUs = service.Cse.List(result.Company + " company");
                        listRequestForCompanyContactUs.Cx = ConfigurationManager.AppSettings["GoogleCse"];
                        var listExecuteResultCompanyContactUs = listRequestForCompanyUrl.Execute();
                        if (listExecuteResultCompanyContactUs.Items != null)
                            dataContactUs = listExecuteResultCompanyContactUs.Items.FirstOrDefault();
                    }
                    var dataPerson = listExecuteResultPerson.Items.FirstOrDefault();
                    db.pr_addLinkedInConnectionDataLoad(result.FirstName, result.LastName, result.Email, result.Company, result.Position, dataPerson.Title, dataPerson.Link, "", dataPerson.Snippet, dataCOmpanyUrl != null ? dataCOmpanyUrl.Link : "", dataContactUs != null ? dataContactUs.Link : "", "", "", 0, 0, dataCompany.Title, dataCompany.Link, "", dataCompany.Snippet, useDate ? connectedOn : (DateTime?)null, SessionSingleton.LoggedInUserId, 1, true);
                }
                count += 4;
            }
            return View();
        }

        public ActionResult Template()
        {
            return File(Server.MapPath("~/UploadTemplates/Connections.csv"), "application/vnd.ms-excel", "Connections.csv");
        }
    }
}