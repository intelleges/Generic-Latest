using Generic.Helpers;
using Generic.Helpers.PartnerHelper;
using Generic.Models;
using Generic.SessionClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.Controllers
{
    public class PODController : Controller
    {
      
        private EntitiesDBContext db = new EntitiesDBContext(); 
        // GET: POD
		public ActionResult Create(string supplierName, string supplierNumber, string buyerFirstName, string buyerLastName, string buyerEmail, int? protocol)
        {
			partner model = new partner();
			model.internalID = supplierNumber;
			model.name = supplierName;
			model.firstName = buyerFirstName;
			model.lastName = buyerLastName;
			model.email = buyerEmail;
            GenerateCreateDropDownLists();
            return View(model);
        }


		public ActionResult FindPODS()
		{
			ViewBag.PT = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

			return View();
		}

		[HttpPost]

		public ActionResult FindPODS(FindPODSViewModel model)
		{
			ViewBag.PT = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

			return View();
		}

        protected void GenerateCreateDropDownLists()
        {
            ViewBag.state = new SelectList(db.state, "id", "name");
            ViewBag.country = new SelectList(db.country, "id", "name");
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name");
            ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).Take(1).ToList(), "id", "name");
            ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname");
            ViewBag.owner = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).Select(v => new SelectListItem { Value = v.id.ToString(), Text = string.Format("{0} {1}", v.firstName, v.lastName) }).ToList();
        }

        [HttpPost]
        public ActionResult Create(partner partner, int? protocol, int? partnertype, int? touchpoint, int? group, DateTime? DueDate)
        {
            List<Tuple<int, string>> uploadedpartners = new List<Tuple<int, string>>();

            string loadGroup = db.pr_getAccesscode().FirstOrDefault();
            partner.enterprise = Generic.Helpers.CurrentInstance.EnterpriseID;
            GenerateCreateDropDownLists();
            try
            {

                int? PartnerId = (int)db.pr_addPartnerSpreadsheetDataLoad(partner.internalID, partner.federalID, partner.dunsNumber, partner.name, partner.address1, partner.address2, partner.city,partner.province??"", partner.fax, "", partner.firstName, partner.lastName, partner.title??"", partner.phone, partner.email, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, partnertype, touchpoint, db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault().id, (int)PartnerStatus.Loaded, loadGroup, DueDate, group).ToList().FirstOrDefault();
                uploadedpartners.Add(new Tuple<int, string>(int.Parse(PartnerId.ToString()), ""));
                Session["uploadedpartnerList"] = uploadedpartners;
                Session["partnertype"] = partnertype;
                Session["touchpoint"] = touchpoint;
                Session["loadGroup"] = loadGroup;
                //    var Target = db.touchpoint.Where(x => x.id == touchpoint).Select(x => x.target).ToList();
                //   ViewBag.Message = Target[0].ToString();
                //ViewBag.Message = "1";

                var Target = db.touchpoint.Where(x => x.id == (touchpoint)).ToList();
                ViewBag.Message = Target[0].target.ToString();
                if (Target[0].target.ToString() == "2")
                {
                    ViewBag.MessageDetail = "Congratulations, you just added  " + partner.name + " to " + Target[0].title;
                }
                ModelState.Clear();
                return View();
            }
            catch (Exception ex)
            {

                ViewBag.Message = "error";
                ViewBag.MessageDetail = ex.ToString();
                //  alertify-ok"
                //ViewBag.state = new SelectList(db.state.ToList(), "id", "name", partner.state);
                //ViewBag.country = new SelectList(db.country.ToList(), "id", "name", partner.country);
                //ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name", protocol);
                //ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "name", partnertype);
                //ViewBag.group = new SelectList(db.pr_getGroupByPerson(SessionSingleton.LoggedInUserId).ToList(), "id", "name", group);
                //ViewBag.owner = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname", partner.owner);
                //ViewBag.author = new SelectList(db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList(), "id", "firstname", partner.author);
                return View(partner);
            }


        }

        public ActionResult GetPartnerTypes(int id)
        {
            return Json(db.pr_getPartnertypeByTouchpoint(id).ToList(), JsonRequestBehavior.AllowGet);
        }
    }
}