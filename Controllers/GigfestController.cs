using Generic.ViewModel;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Telerik.Web.Mvc;

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
            //if (SessionModel != null && SessionModel.CreateModel != null && SessionModel.CreateModel.Equals(model))
            //{
            //    return View("GigfestResultView", SessionModel.Response);
            //}
            //else
            //{
            var client = new RestClient(ConfigurationManager.AppSettings["CompanyProfilerApiHost"]);
            var request = new RestRequest(ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequest"], Method.POST);
            request.AddQueryParameter("code", ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestCodeParam"]);
            // request.AddQueryParameter("code", ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestCodeParam"]);
            request.RequestFormat = DataFormat.Json;
            var requestBody = model.GetRequest(db.pr_getAccesscode().FirstOrDefault());
            Session["token"] = requestBody.token;
            db.pr_addToken(requestBody.token, DateTime.Now, null, 0, true).FirstOrDefault();
            request.AddBody(requestBody);
            client.Execute(request);
            //SessionModel = new GigfestSessionModel()
            //{
            //    CreateModel = model,
            //    Response = response.Data
            //};
            return View("GigfestResultView", new List<GigfestResponse>());
            //}
        }

        [GridAction(EnableCustomBinding = true)]
        public ActionResult CheckData(GridCommand command)
        {
            var offset = ((command.Page - 1) * command.PageSize);
            var limit = command.PageSize;
            var client = new RestClient(ConfigurationManager.AppSettings["CompanyProfilerApiHost"]);
            var request = new RestRequest(ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestData"], Method.GET);
            request.AddQueryParameter("code", ConfigurationManager.AppSettings["CompanyProfilerApiGigfestRestRequestDataCodeParam"]);
            request.AddQueryParameter("token", Session["token"].ToString());
            request.AddQueryParameter("limit", limit.ToString());
            request.AddQueryParameter("offset", offset.ToString());
            var response = client.Execute<List<GigfestResponse>>(request);            
            return Json(new GridModel() { Data = response.Data, Total = (response.Data != null && response.Data.Count == limit ? offset + limit + 1 : offset + response.Data.Count) },  JsonRequestBehavior.AllowGet);
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