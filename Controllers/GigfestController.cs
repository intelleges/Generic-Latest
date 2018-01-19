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
        private EntitiesDBContext db = new EntitiesDBContext();
        // GET: Gigfest
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create(int? PersonGridPage = null)
        {

            if (Request.Params.AllKeys.Any(o => o == "PersonGrid-page"))
                return View("GigfestResultView", SessionModel.Response);
            return View();
        }
        [HttpPost]
        public ActionResult Create(CreateGigfestModel model)
        {
            if (SessionModel != null && SessionModel.CreateModel != null && SessionModel.CreateModel.Equals(model))
            {
                return View("GigfestResultView", SessionModel.Response);
            }
            else
            {
                var client = new RestClient(ConfigurationManager.AppSettings["CompanyProfilerApiHost"]);
                var request = new RestRequest(ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequest"], Method.POST);
                request.AddQueryParameter("code", ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestCodeParam"]);
               // request.AddQueryParameter("code", ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestCodeParam"]);
                request.RequestFormat = DataFormat.Json;
                var requestBody = model.GetRequest(db.pr_getAccesscode().FirstOrDefault());
                db.pr_addToken(requestBody.token, DateTime.Now, null, null, true).FirstOrDefault();
                request.AddBody(requestBody);
                var response = client.Execute<List<GigfestResponse>>(request);
                SessionModel = new GigfestSessionModel()
                {
                    CreateModel = model,
                    Response = response.Data
                };
                return View("GigfestResultView", response.Data);
            }
        }

        public ActionResult CheckData(string token)
        {
            var client = new RestClient(ConfigurationManager.AppSettings["CompanyProfilerApiHost"]);
            var request = new RestRequest(ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequest"], Method.GET);
            request.AddQueryParameter("code", ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestCodeParam"]);
            request.AddQueryParameter("token", token);

            
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        private GigfestSessionModel SessionModel
        {
            get
            {
                return (GigfestSessionModel)Session["SessionModel"];
            }
            set
            {
                Session["SessionModel"] = value;
            }
        }
    }
}