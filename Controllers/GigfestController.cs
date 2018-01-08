using Generic.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class GigfestController : Controller
    {
        // GET: Gigfest
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateGigfestModel model)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["CompanyProfilerApiHost"]);
            var request = new RestRequest(ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequest"], Method.POST);
            request.AddQueryParameter("code", ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestCodeParam"]);
            request.RequestFormat = DataFormat.Json;
            request.AddBody(model.GetRequest());
            var response = client.Execute<List<GigfestResponse>>(request);
            return View("GigfestResultView", response.Data);
        }
    }
}