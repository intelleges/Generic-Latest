using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.ViewModel;
using Generic.SessionClass;
using Generic.Helpers.Questionnaire;
using System.Xml.Serialization;
using System.Web.Routing;
using Generic.Helpers.Utility;
using Generic.Models;
using Generic.Helpers.PartnerHelper;
using System.Data;
using Generic.Helpers;
using System.Data.Entity;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Generic.Controllers
{
    [Authorize]
    public class QuestionnaireController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Questionnaire/

        public ActionResult Index()
        {
            //var questionnaire = db.pr_getQuestionnaireAll(SessionSingleton.MyEnterPriseId);
            //return View(questionnaire.ToList());

            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            Session["questionnairesearch"] = arguments;
            return RedirectToAction("FindQuestionnaireResult");
        }

        public ActionResult UploadCMS()
        {

            if (Session["QuestionnaireId"] == null)
            {

                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

                ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            }
            else
            {
                ViewBag.NoDropdowns = "1";

                ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

                ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

                //db.pr_getPartnertypeTouchpointQuestionnaireByQuestionnaire((int)Session["QuestionnaireId"]).ToList();
            }

            //

            return View();
        }

        [HttpPost]
        public ActionResult UploadCMS(HttpPostedFileBase uploadCMSFile, HttpPostedFileBase uploadCMSFilePDF, HttpPostedFileBase uploadCMSFileFAQ, HttpPostedFileBase uploadCMSFileOther, int touchpoint, int partnertype)
        {
            int questionnaireId = 0;
            if (Session["QuestionnaireId"] != null)
            {
                questionnaireId = (int)Session["QuestionnaireId"];
            }
            else
            {
                var objptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(partnertype, touchpoint).FirstOrDefault();
                if (objptq != null)
                {
                    questionnaireId = objptq.questionnaire;
                }
            }


            if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
            }

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles/uploadCMS"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/uploadCMS"));
            }

            // The Name of the Upload component is "attachments" 
            var file = uploadCMSFile;

            // Some browsers send file names with full path. This needs to be stripped.
            var fileName = Path.GetFileName(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/uploadCMS"), fileName);

            // The files are not actually saved in this demo
            file.SaveAs(physicalPath);

            int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;

            string sheetname = "sheet1";
            //var excelRead = new ExcelQueryFactory(physicalPath.ToString());
            // ITEM TEXT LINK

            var cmsinExcel = ExcelMapper.GetRows<ExcelQuestionnaireCMS>(Convert.ToString(physicalPath), sheetname).ToList();

            //var objQuestionnareCMS = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(questionnaireId).ToList();
            var objQuestionnareCMS = db.pr_getQuestionnaireCMSAll().ToList();

            foreach (var cms in cmsinExcel)
            {
                var questionnaireCMSID = objQuestionnareCMS.Where(x => x.description == cms.ITEM).FirstOrDefault();
                if (questionnaireCMSID != null)
                {
                    using (var context = new EntitiesDBContext())
                    {
                        if (cms.ITEM == CMS.QUESTIONNAIRE_PDF)
                        {
                            if (uploadCMSFilePDF == null)
                            {
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.id, cms.TEXT, cms.LINK, null, null);
                            }
                            else
                            {
                                byte[] uploadedFile = new byte[uploadCMSFilePDF.InputStream.Length];
                                uploadCMSFilePDF.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.id, cms.TEXT, cms.LINK, uploadedFile, MimeMapping.GetMimeMapping(uploadCMSFilePDF.FileName));
                            }
                        }
                        else if (cms.ITEM == CMS.QUESTIONNAIRE_FAQ)
                        {
                            if (uploadCMSFileFAQ == null)
                            {
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.id, cms.TEXT, cms.LINK, null, null);
                            }
                            else
                            {
                                byte[] uploadedFile = new byte[uploadCMSFileFAQ.InputStream.Length];
                                uploadCMSFileFAQ.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.id, cms.TEXT, cms.LINK, uploadedFile, MimeMapping.GetMimeMapping(uploadCMSFileFAQ.FileName));
                            }
                        }
                        else if (cms.ITEM == CMS.QUESTIONNAIRE_DOC_OTHER)
                        {
                            if (uploadCMSFileOther == null)
                            {
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.id, cms.TEXT, cms.LINK, null, null);
                            }
                            else
                            {
                                byte[] uploadedFile = new byte[uploadCMSFileOther.InputStream.Length];
                                uploadCMSFileOther.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.id, cms.TEXT, cms.LINK, uploadedFile, MimeMapping.GetMimeMapping(uploadCMSFileOther.FileName));
                            }
                        }
                        else
                        {
                            context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.id, cms.TEXT, cms.LINK, null, null);
                        }
                        context.pr_addQuestionnaireCMSLoad(questionnaireCMSID.id, cms.ITEM, cms.TEXT, cms.LINK, questionnaireId).FirstOrDefault();
                    }
                }
            }

            //
            //Session["protocolId"] = protocolId;
            //Session["touchpointId"] = touchpointId;
            //Session["partnertypeId"] = partnertypeId;
            //Session["level"] = level;
            ViewBag.message = "1";

            return RedirectToAction("UploadAutoMailMessage", "AutoMailMessage");
        }

        public ActionResult SkipCMS()
        {
            //Session["QuestionnaireId"] = questionnaireId;
            //Session["protocolId"] = protocol;
            //Session["touchpointId"] = touchpoint;
            //Session["partnertypeId"] = partnertype;
            //Session["level"] = level;

            db.pr_bootstrapQuestionnaireCMS((int)Session["QuestionnaireId"], "Not Defined Yet", "Not Defined Yet", "Not Defined Yet", "Not Defined Yet");

            return RedirectToAction("UploadAutoMailMessage", "AutoMailMessage");
        }



        [Authorize]
        public ActionResult QuestionnaireDetailView(int id, int? ptqId, int? questionId, int? partnerId, int? responseId)
        {
            ViewBag.questionnaireId = id;
            List<question> questionnairedetail = db.pr_getQuestionByQuestionnaire(id).ToList();
            if (questionId.HasValue && partnerId.HasValue && responseId.HasValue)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByQuestionnaire(id).FirstOrDefault();
                var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerId, ptq.id).FirstOrDefault();
                var pptqResponse = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq.id).FirstOrDefault();
                var question = db.pr_getQuestion(questionId).FirstOrDefault();
                var response = db.pr_getResponse(responseId).FirstOrDefault();
                ViewBag.ResponseDialogText = pptq.partner1.name + "(" + pptq.partner1.email + ") answered '" + response.description + "' to '" + question.title + "' for access code '" + pptq.accesscode + "' with comment '" + pptqResponse.comment + "'";
            }
            return View(questionnairedetail);
        }
        [HttpPost]
        public ActionResult GetQuestionnaireResponsesWithSelectedQuestion(int question, int questionnaire)
        {
            try
            {
                var questionnarieResponses = db.pr_getResponseByQuestionnaire(questionnaire).ToList().Distinct(new ResponseComparer());
                var questionResponses = db.pr_getResponseByQuestion(question).Select(o => o.id).ToList();
                return Json(questionnarieResponses.Select(o => new { value = o.id, text = o.description, selected = questionResponses.Contains(o.id) }));
            }
            catch
            {
                return Json(new { error = "" });
            }

        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetHtmlText(string question)
        {

            return Json(EmailFormat.GetHintResult(question));
            //EmailFormat
        }
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult SaveQuestion(int question, string text)
        {
            var questionObj = db.pr_getQuestion(question).FirstOrDefault();
            if (questionObj != null)
            {
                questionObj.Question = questionObj.name = text;
                db.Entry(questionObj).State = EntityState.Modified;
                db.SaveChanges();
                return Json("done");
            }
            return Json("");
        }
        [HttpPost]
        public ActionResult AddNewResponse(string text, string code, int question)
        {
            var nresp = new ResponseItem() { id = Guid.NewGuid(), text = text, code = code };
            //var id = db.pr_addResponse(text, code, 1, true, Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            AddedResponses.Add(nresp);
            return Json(nresp);
        }
        class ResponseItem
        {
            public Guid id
            {
                get;
                set;
            }
            public string text { get; set; }
            public string code { get; set; }
        }

        List<ResponseItem> AddedResponses
        {
            get
            {
                if (Session["AddedResponses"] == null)
                {
                    Session["AddedResponses"] = new List<ResponseItem>();
                }
                return (List<ResponseItem>)Session["AddedResponses"];
            }
        }


        private int BootstrapDefaultQuestionnarie(string touchpointName, int ptqId)
        {
            var enterpriseId = Helpers.CurrentInstance.EnterpriseID;
            var personId = SessionSingleton.LoggedInUserId;
            // var defaultID = int.Parse(ConfigurationManager.AppSettings["DefaultPartnerTypeTouchpointQuestionnaireID"]);
            var defaultPtq = db.pr_getPartnertypeTouchpointQuestionnaire(ptqId).FirstOrDefault();

            if (defaultPtq != null)
            {
                //var defPartnerType = db.pr_addPartnerType(defaultPtq.partnerType1.name, defaultPtq.partnerType1.alias, defaultPtq.partnerType1.description, defaultPtq.partnerType1.partnerClass, enterpriseId, defaultPtq.partnerType1.sortOrder, 1).FirstOrDefault();
                var defPartnerType = db.partnerType.FirstOrDefault(o => o.enterprise == enterpriseId).id;
                var defaultProtocol = db.pr_getProtocolAll(enterpriseId).FirstOrDefault().id;
                var dtp = db.pr_addTouchpoint(defaultProtocol, personId, personId, personId, touchpointName, touchpointName, defaultPtq.touchpoint1.purpose, defaultPtq.touchpoint1.target, defaultPtq.touchpoint1.abbreviation, DateTime.Now, DateTime.Now.AddMonths(12), defaultPtq.touchpoint1.automaticReminder, defaultPtq.touchpoint1.sortOrder, 1).FirstOrDefault();
                //create new Campaign for this issue
                db.pr_addCampaign(touchpointName + " Campaign", DateTime.Now, null, personId, personId, 1, true).FirstOrDefault();
                //var dtp = db.touchpoint.FirstOrDefault(o => o.person == personId);

                var questionnarie = Convert.ToInt32(db.pr_addQuestionnaire(defaultPtq.questionnaire1.title + touchpointName, defaultPtq.questionnaire1.description, defaultPtq.questionnaire1.footer, defaultPtq.questionnaire1.locked, defaultPtq.questionnaire1.sortOrder, 1, defaultPtq.questionnaire1.multiLanguage, enterpriseId, personId, (int)defPartnerType, defaultPtq.questionnaire1.letter, defaultPtq.questionnaire1.levelType).FirstOrDefault());

                var newptq = Convert.ToInt32(db.pr_addPartnertypeTouchpointQuestionnaire((int)defPartnerType, (int)dtp, (int)questionnarie, 0, true).FirstOrDefault());
                //copy automailmessages
                foreach (var message in defaultPtq.autoMailMessage)
                    db.pr_addAutomailMessage(message.subject, message.text, message.footer1, message.footer2, message.sendDateCalcFactor, message.sendDateSet, message.mailType, (int)newptq).FirstOrDefault();

                //copy QuestionnaireCMS
                foreach (var cms in defaultPtq.questionnaire1.questionnaireQuestionnaireCMS)
                    db.pr_addQuestionnaireQuestionnaireCMS((int)questionnarie, cms.questionnaireCMS, cms.text, cms.link, cms.doc, cms.uploadedFileType);

                var questions = db.pr_getQuestionByQuestionnaire((int)defaultPtq.questionnaire).ToList();


                var existSurvey = new Dictionary<int, int>();
                var existSurveySet = new Dictionary<int, int>();
                var existPage = new Dictionary<int, int>();
                var existSurveySetSurvey = new HashSet<string>();
                var existSurveyPage = new HashSet<string>();

                foreach (var question in questions)
                {

                    var qId = Convert.ToInt32(db.pr_addQuestion(question.Question, question.name, question.title, question.tag, question.responseType, question.required, question.weight, question.skipLogicAnswer, question.skipLogicJump, question.subCheckBoxChoice, question.accessLevel, question.commentRequired, question.commentBoxTxt, question.commentUploadTxt, question.calendarMessageTxt, question.commentType, question.spinOffQuestionnaire, question.spinOffQID, question.emailAlert, question.emailAlertList, question.updated, question.sortOrder, question.active, enterpriseId).FirstOrDefault()
                        );
                    if (!string.IsNullOrEmpty(question.skipLogicJump))
                    {
                        db.pr_modifyQuestionSkipLogicJumpLogic((int)qId, question.skipLogicJump.Replace(question.id.ToString(), ((int)qId).ToString()));
                    }
                    foreach (var qres in question.questionResponses)
                    {
                        db.pr_addQuestionResponse((int)qId, qres.response);
                    }
                    //foreach (var answer in question.resp)
                    foreach (var survey in question.surveys)
                    {
                        if (!existSurvey.ContainsKey(survey.id))
                        {
                            var ns = Convert.ToInt32(db.pr_addSurvey(survey.description, survey.name, survey.display, 1, true, DateTime.Now, DateTime.Now).FirstOrDefault());
                            existSurvey.Add(survey.id, (int)ns);
                        }
                        db.pr_addSurveyQuestion(existSurvey[survey.id], (int)qId);
                        foreach (var surveySet in survey.surveyset)
                        {
                            if (!existSurveySet.ContainsKey(surveySet.id))
                            {
                                var ss = Convert.ToInt32(db.pr_addSurveyset(surveySet.description, surveySet.sortOrder, true, 1).FirstOrDefault());
                                existSurveySet.Add(surveySet.id, (int)ss);
                            }
                            if (!existSurveySetSurvey.Contains(existSurveySet[surveySet.id].ToString() + "*" + existSurvey[survey.id].ToString()))
                            {
                                db.pr_addSurveysetSurvey(existSurveySet[surveySet.id], existSurvey[survey.id]);
                                existSurveySetSurvey.Add(existSurveySet[surveySet.id].ToString() + "*" + existSurvey[survey.id].ToString());
                            }
                            foreach (var page in surveySet.page)
                            {
                                if (!existPage.ContainsKey(page.id))
                                {
                                    var sp = Convert.ToInt32(db.pr_addPage(page.description, page.sortOrder, true).FirstOrDefault());
                                    existPage.Add(page.id, (int)sp);
                                    db.pr_addQuestionnairePage((int)questionnarie, (int)sp);
                                }
                                if (!existSurveyPage.Contains(existPage[page.id].ToString() + "*" + existSurveySet[surveySet.id].ToString()))
                                {
                                    db.pr_addPageSurveyset(existPage[page.id], existSurveySet[surveySet.id]);
                                    existSurveyPage.Add(existPage[page.id].ToString() + "*" + existSurveySet[surveySet.id].ToString());
                                }
                            }
                        }
                    }
                }
                return newptq;
            }
            return -1;
        }

        public ActionResult SaveQuestionResponses(string[] currentValues, int question)
        {
            try
            {
                int val = 0;
                Guid nVal;
                var cv = currentValues.Where(o => int.TryParse(o, out val)).Select(o => int.Parse(o)).ToArray();
                var questionResponses = db.pr_getResponseByQuestion(question).Select(o => o.id).ToList();
                var responseToDetach = questionResponses.Where(o => !cv.Contains(o)).ToList();
                var responseToAttach = cv.Where(o => !questionResponses.Contains(o)).ToList();
                foreach (var response in responseToDetach)
                    db.pr_removeQuestionResponse(question, response);
                foreach (var response in responseToAttach)
                {
                    db.pr_addQuestionResponse(question, response);
                }
                var cvNew = currentValues.Where(o => Guid.TryParse(o, out nVal)).Select(o => Guid.Parse(o)).ToArray();
                foreach (var responseId in cvNew)
                {
                    var response = AddedResponses.FirstOrDefault(o => o.id == responseId);
                    if (response != null)
                    {
                        var id = (int?)db.pr_addResponse(response.text, response.code, 1, true, Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
                        db.pr_addQuestionResponse(question, id);
                        var currentPtq = db.pr_getPartnertypeTouchpointQuestionnaireByQuestion(question).FirstOrDefault();
                        //question tag for change ptq who selects this respnse


                        var newptq = BootstrapDefaultQuestionnarie(response.text, currentPtq.id);
                        if (newptq != -1)
                        {
                            var questionObj = db.pr_getQuestion(question).FirstOrDefault();
                            questionObj.tag = "autochangeptq:" + id + "-" + newptq;
                            db.Entry(questionObj).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                    }
                }
                AddedResponses.Clear();
                return Json(new { error = "Campaign, Touchpoint and Questionnaire were successefully created for new responses" });
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.InnerException != null ? ex.InnerException.Message : ex.Message });
            }
        }

        class ResponseComparer : IEqualityComparer<pr_getResponseByQuestionnaire_Result>
        {

            public bool Equals(pr_getResponseByQuestionnaire_Result x, pr_getResponseByQuestionnaire_Result y)
            {
                return x.id == y.id;
            }

            public int GetHashCode(pr_getResponseByQuestionnaire_Result obj)
            {
                return obj.id.GetHashCode();
            }
        }

        public ActionResult QuestionnaireQuestionnaireCMS(int id = 0)
        {
            //  List<ExcelQuestionnaireQuestionnireCMS> questionnairedetail = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(id).ToList();
            List<view_QuestionnaireCMS> questionnairedetail = db.pr_getViewQuestionnaireCMSByQuestionnaire(id).ToList();

            ViewBag.questionnaire = id;

            return View(questionnairedetail);
        }
        public ActionResult QuestionnaireQuestionnaireAutoMail(int id = 0)
        {
            List<autoMailMessage> questionnaireAutoMailDetail =
                db.Database.SqlQuery<autoMailMessage>("EXEC pr_getAutomailMessageByQuestionnaire '" + id + "'").ToList();
            List<QuestionnaireAutoMailViewModel> lstquestionaireAutoMailDetails = ConvertToQuestionnaireAutoMailViewModel(questionnaireAutoMailDetail);

            ViewBag.Types = db.pr_getAutoMailAttachmentTypeAll().ToList();
            return View(lstquestionaireAutoMailDetails);
        }

        public async Task<ActionResult> UploadQuestionnaireAutomailDocument(HttpPostedFileBase noteTitle, string note, int id, string tags, int document)
        {
            return await Task.Run(() =>
            {
                if (noteTitle != null && noteTitle.ContentLength != 0)
                {
                    using (var stream = new MemoryStream())
                    {
                        noteTitle.InputStream.CopyTo(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                        db.pr_addAutoMailAttachment(id, stream.ToArray(), tags, note, document, 0, true);
                    }

                }

                return Json(true);
            });
        }

        public ActionResult QuestionnaireQuestionnaireTestAutomailAll(int id = 0)
        {
            var _enterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;
            var _currentPerson = db.pr_getPersonByEmail(_enterpriseID, User.Identity.Name).FirstOrDefault();
            var _internalID = db.pr_getAccesscode().FirstOrDefault();
            var _loadGroup = db.pr_getAccesscode().FirstOrDefault();
            var _partnertypeTouchpoint = db.pr_getPartnertypeTouchpointQuestionnaireByQuestionnaire(id).FirstOrDefault();
            var _ptq = db.pr_getPartnertypeTouchpointQuestionnaireByQuestionnaire(id).FirstOrDefault();
            var _group = db.pr_getGroupByPTQ(_ptq.id).FirstOrDefault();
            var _partnerSpreadsheetDataLoadStatus = 1;
            var _dueDate = DateTime.Now.AddDays(28);

            //var _partnerId = db.pr_addPartnerSpreadsheetDataLoad(_internalID, _internalID, _currentPerson.firstName + "_" + _currentPerson.lastName, 
            //        _currentPerson.address1, _currentPerson.address2, _currentPerson.city, _currentPerson.state.ToString(), _currentPerson.zipcode, 
            //        _currentPerson.country.ToString(), _currentPerson.firstName, _currentPerson.lastName, _currentPerson.title, _currentPerson.phone,
            //        _currentPerson.email, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, _partnertypeTouchpoint.partnerType, 
            //        _partnertypeTouchpoint.touchpoint, _currentPerson.id, _partnerSpreadsheetDataLoadStatus, _loadGroup, _dueDate).ToList().FirstOrDefault();

            var _partnerId = db.pr_addPartnerSpreadsheetDataLoad(_internalID, _internalID, "", _currentPerson.firstName + "_" + _currentPerson.lastName, _currentPerson.address1, _currentPerson.address2, _currentPerson.city, _currentPerson.state.ToString(), _currentPerson.zipcode,
                    _currentPerson.country.ToString(), _currentPerson.firstName, _currentPerson.lastName, _currentPerson.title, _currentPerson.phone,
                    _currentPerson.email, "", "", "", DateTime.Now, Generic.Helpers.CurrentInstance.EnterpriseID, _partnertypeTouchpoint.partnerType,
                    _partnertypeTouchpoint.touchpoint, _currentPerson.id, _partnerSpreadsheetDataLoadStatus, _loadGroup, _dueDate, _group.id).ToList().FirstOrDefault();

            var _partners = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByLoadGroup(_loadGroup).ToList();
            var ptq = db.pr_getPartnertypeTouchpointQuestionnaireByPartnertypeAndTouchpoint(_partnertypeTouchpoint.partnerType, _partnertypeTouchpoint.touchpoint).LastOrDefault().id;

            foreach (var partnerItem in _partners)
            {
                var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partnerItem.partner, ptq).FirstOrDefault();
                pptq.invitedDate = DateTime.Now;

                var person = db.pr_getPersonByEmail(CurrentInstance.EnterpriseID, User.Identity.Name).FirstOrDefault();
                pptq.invitedBy = person.id;
                pptq.status = (int)PartnerStatus.Invited_NoResponse;
                db.Entry(pptq).State = EntityState.Modified;
                db.SaveChanges();

                var _partner = db.pr_getPartner(partnerItem.partner).FirstOrDefault();
                _partner.status = partnerStatusTypes.PARTNER_INVITED_NO_RESPONSE;
                db.Entry(_partner).State = EntityState.Modified;
                db.SaveChanges();

                var _touchpoint = db.pr_getTouchpoint(_partnertypeTouchpoint.touchpoint).FirstOrDefault();
                var _enterprise = db.pr_getEnterprise(_partner.enterprise).FirstOrDefault();

                for (var autoMailTypeCounter = Generic.Helpers.Utility.autoMailTypes.Invitation; autoMailTypeCounter <= Generic.Helpers.Utility.autoMailTypes.Alert; autoMailTypeCounter++)
                {
                    var amm = db.pr_getAutoMailmessageByMailtypeandPTQ(autoMailTypeCounter, _ptq.id);

                    foreach (var ammItem in amm)
                    {
                        if (ammItem != null && !string.IsNullOrEmpty(ammItem.text))
                        {
                            Email email = new Email(ammItem);
                            EmailFormat emailFormat = new EmailFormat();
                            email.body = emailFormat.sGetEmailBody(email.body, _currentPerson, _partner, _enterprise, _touchpoint, _ptq.id);
                            email.emailTo = _partner.email;
                            email.automailMessage = ammItem.id.ToString();
                            email.accesscode = email.accesscode;
                            email.protocolTouchpoint = _touchpoint.description;
                            email.url = Request.Url.ToString();
                            email.category = SendGridCategory.QuestionnaireQuestionnaireTestAutomailAll;

                            SendEmail objSendEmail = new SendEmail();
                            objSendEmail.sendEmail(email, new EmailFormatSettings() {
                                 enterprise = _enterprise, partner = _partner, ptq = _ptq.id, touchpoint = _touchpoint,
                                  sender = _currentPerson
                            });
                        }
                    }
                }
            }
            return RedirectToAction("FindQuestionnaireResult");
        }

        private List<QuestionnaireAutoMailViewModel> ConvertToQuestionnaireAutoMailViewModel(List<autoMailMessage> questionnaireAutoMailDataList)
        {
            List<QuestionnaireAutoMailViewModel> objQuestionnaireAutoMailViewModelList = new List<QuestionnaireAutoMailViewModel>();

            foreach (var AutoMailData in questionnaireAutoMailDataList)
            {
                QuestionnaireAutoMailViewModel objAutoMailViewModel = new QuestionnaireAutoMailViewModel();
                objAutoMailViewModel.id = AutoMailData.id;
                objAutoMailViewModel.subject = AutoMailData.subject;
                objAutoMailViewModel.text = AutoMailData.text;
                objAutoMailViewModel.footer1 = AutoMailData.footer1;
                objAutoMailViewModel.footer2 = AutoMailData.footer2;
                objAutoMailViewModel.sendDateCalcFactor = AutoMailData.sendDateCalcFactor;
                objAutoMailViewModel.sendDateSet = AutoMailData.sendDateSet;
                objAutoMailViewModel.mailType = (AutoMailTypes)AutoMailData.mailType;
                objAutoMailViewModel.partnerTypeTouchpointQuestionnaire = AutoMailData.partnerTypeTouchpointQuestionnaire;


                objQuestionnaireAutoMailViewModelList.Add(objAutoMailViewModel);
            }
            return objQuestionnaireAutoMailViewModelList;
        }
        public ActionResult Edit(int id = 0)
        {
            questionnaire questionnaire = db.pr_getQuestionnaire(id).FirstOrDefault();
            if (questionnaire == null)
            {
                return HttpNotFound();
            }
            //   ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", partner.enterprise);
            ViewBag.CMS = new SelectList(db.questionnaireCMS, "description", "id", questionnaire.id);
            //ViewBag.state = new SelectList(db.questionn, "id", "name", partner.state);
            //ViewBag.country = new SelectList(db.country, "id", "name", partner.country);
            ////ViewBag.status = db.pr_getPartnerStatusAll().Where(x => x.id == partner.status).FirstOrDefault().description;
            //ViewBag.status = ((List<view_QuestionnaireData>)Session["partner"]).Where(x => x.id == partner.id).FirstOrDefault().status;
            return View(questionnaire);
        }

        [HttpPost]
        public ActionResult Edit(questionnaire questionnaire)
        {
            if (ModelState.IsValid)
            {
                //db.Entry(partner).State = EntityState.Modified;
                //db.SaveChanges();
                db.pr_modifyQuestionnaire(questionnaire.id, questionnaire.title, questionnaire.description, questionnaire.footer, questionnaire.locked, questionnaire.sortOrder, questionnaire.active, questionnaire.multiLanguage, questionnaire.enterprise, questionnaire.person, questionnaire.partnerType, questionnaire.letter, questionnaire.levelType);

                return Json(new { success = true });
                //return RedirectToAction("Index");
            }
            ViewBag.enterprise = new SelectList(db.enterprise, "id", "description", questionnaire.enterprise);
            //ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);
            return View(questionnaire);
        }

        public ActionResult EditQuestionDetail(int? id)
        {
            var question = db.pr_getQuestion(id).FirstOrDefault();
            if (question == null)
                return HttpNotFound();
            LoadResponseTypes(question.responseType);
            return View(question);
        }

        public ActionResult QuestionDetailView(int? id)
        {
            var question = db.pr_getQuestion(id).FirstOrDefault();
            return Json(new { text = question.name }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditQuestionDetail(question question)
        {
            //var question = db.pr_getQuestion(id).FirstOrDefault();
            //if (question == null)
            // return HttpNotFound();
            //LoadResponseTypes(question.responseType);
            return View(question);
        }

        private void LoadResponseTypes(int? responseType)
        {

            var items = typeof(ResponseType).GetFields().Where(o => o.IsLiteral).Select(o => new { Name = o.Name, Id = (int)o.GetRawConstantValue() }).ToList();
            ViewBag.ResponseTypes = new SelectList(items, "Id", "Name", responseType);
        }

        //[AllowAnonymous]
        public ActionResult DownloadCMSTemplate()
        {

            //List<ExcelQuestionnaireCMS> objReport = new List<ExcelQuestionnaireCMS>();

            // var test = db.pr_getQuestionnaireCMSAll().ToList().Select(x => new ExcelQuestionnaireCMS { ITEM = x.description, TEXT = "", LINK = "" }).ToList();
            // objReport = test;

            //var stream = new MemoryStream();
            //var serializer = new XmlSerializer(typeof(List<ExcelQuestionnaireCMS>));


            //We turn it into an XML and save it in the memory
            //serializer.Serialize(stream, objReport);
            //stream.Position = 0;

            //We return the XML from the memory as a .xls file

            return File(Server.MapPath("~/UploadTemplates/addQuestionnaireCMSTemplate.xlsx"), "application/vnd.ms-excel", "CMSTemplate.xls");
        }

        public ActionResult UploadQuestionnaire()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAll(), "id", "description");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.level = new SelectList(db.pr_getQuestionnaireLevelTypeByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            return View();
        }

        public ActionResult GetPartnerTypesByTouchpoint(int touchpoint)
        {
            return Json(db.pr_getPartnerTypeForAssignment(Generic.Helpers.CurrentInstance.EnterpriseID, touchpoint).ToList(), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Download(int questionnaireId)
        {
            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<ExcelQuestionnaire>));
            var result = new List<ExcelQuestionnaire>();

            var questionList = db.pr_getQuestionnaireLoad(questionnaireId).ToList();
            if (questionList.Any())
            {
                foreach (var question in questionList)
                {
                    var qObj = new ExcelQuestionnaire()
                    {
                        Comment = question.comment,
                        CommentBoxMessageText = question.commentBoxMessageText,
                        CommentType = question.commentType,
                        emailalert = question.emailAlert,
                        emailalertlist = question.emailAlertList,
                        Length = question.length,
                        naValue = question.otherValue,
                        nValue = question.nValue,
                        otherValue = question.otherValue,
                        Page = question.page,
                        QID = question.qid,
                        Question = question.question,
                        qWeight = question.qWeight,
                        Required = question.required,
                        Response = question.response,
                        skipLogic = question.skipLogic,
                        skipLogicAnswer = question.skipLogicAnswer,
                        skipLogicJump = question.skipLogicJump,
                        snipOffQuestionnaire = question.spinOffQuestionnaire,
                        spinoffid = question.spinOffID,
                        Survey = question.survey,
                        Surveyset = question.surveySet,
                        Title = question.title,
                        titleLength = question.titleLength,
                        UploadMessageText = question.uploadMessageText,
                        yValue = question.yValue
                    };
                    result.Add(qObj);
                }
            }
            else
            {
                var questions = db.pr_getQuestionByQuestionnaire(questionnaireId).ToList();
                foreach (var question in questions)
                {
                    var qObj = new ExcelQuestionnaire()
                    {
                        Comment = question.commentRequired.HasValue && question.commentRequired.Value > 0 ? "Y" : "N",
                        CommentBoxMessageText = question.commentBoxTxt,
                        Required = question.required.HasValue && question.required.Value == 1 ? "Y" : "N",
                        QID = question.id,
                        spinoffid = question.spinOffQID.HasValue ? question.spinOffQID.Value : 0,
                        snipOffQuestionnaire = question.spinOffQuestionnaire,
                        Title = question.title,
                        Question = question.Question,
                        titleLength = question.title.Length,
                        Length = question.Question.Length,
                        skipLogic = question.skipLogicAnswer.HasValue && !string.IsNullOrEmpty(question.skipLogicJump) ? "Y" : "N",
                        otherValue = -1,
                        yValue = 1,
                        nValue = 0,
                        naValue = -1,
                        emailalert = question.emailAlert,
                        emailalertlist = question.emailAlertList,
                        skipLogicJump = question.skipLogicJump
                    };
                    switch (question.commentType)
                    {
                        case (int)CommentType.YN_COMMENT_N:
                            qObj.CommentType = "YN_COMMENT_N";
                            break;
                        case (int)CommentType.YN_COMMENT_Y: qObj.CommentType = "YN_COMMENT_Y"; break;
                        case (int)CommentType.YN_NO_COMMENT: qObj.CommentType = "YN_NO_COMMENT"; break;
                        case (int)CommentType.YN_UPLOAD_N: qObj.CommentType = "YN_UPLOAD_N"; break;
                        case (int)CommentType.YN_UPLOAD_Y: qObj.CommentType = "YN_UPLOAD_Y"; break;
                        case (int)CommentType.YN_WARNING_N: qObj.CommentType = "YN_WARNING_N"; break;
                        case (int)CommentType.YN_WARNING_Y: qObj.CommentType = "YN_WARNING_Y"; break;
                        default:
                            qObj.CommentType = "0";
                            break;
                    }
                    switch (question.responseType)
                    {
                        case 3:
                            if (question.questionResponses.Count == 2)
                            {
                                qObj.Response = "Y/N";
                            }
                            else if (question.questionResponses.Any(o => o.response1.id == 77))
                            {
                                qObj.Response = "Y/N/COTS";
                            }
                            else
                                qObj.Response = "Y/N/NA";
                            break;
                        case 4:
                            qObj.Response = "TEXT";
                            break;
                        case 6: qObj.Response = "NUMBER"; break;
                        case 7: qObj.Response = "COMMENT"; break;
                        case 10:
                            qObj.Response = "DROPDOWN";
                            if (question.questionResponses.Count > 0)
                            {
                                qObj.Response += ":";
                                foreach (var response in question.questionResponses)
                                {
                                    qObj.Response += response.response1.description + ";";
                                }
                            }
                            break;
                        case 11:
                            qObj.Response = "LIST";
                            if (question.questionResponses.Count > 0)
                            {
                                qObj.Response += ":";
                                foreach (var response in question.questionResponses)
                                {
                                    qObj.Response += response.response1.description + ";";
                                }
                            }
                            break;
                        case 12:
                            qObj.Response = "CHECKBOX";
                            if (question.questionResponses.Count > 0)
                            {
                                qObj.Response += ":";
                                foreach (var response in question.questionResponses)
                                {
                                    qObj.Response += response.response1.description + ";";
                                }
                            }
                            break;
                        case 13: qObj.Response = "UPLOAD"; break;
                        case 14: qObj.Response = "TEXT/UPLOAD"; break;
                        default: break;
                    }
                    switch (question.skipLogicAnswer)
                    {
                        case SkipLogicAnswer.N: qObj.skipLogicAnswer = "N"; break;
                        case SkipLogicAnswer.Y: qObj.skipLogicAnswer = "Y"; break;
                        case SkipLogicAnswer.M: qObj.skipLogicAnswer = "M"; break;
                        case SkipLogicAnswer.A: qObj.skipLogicAnswer = "A"; break;
                        case SkipLogicAnswer.D: qObj.skipLogicAnswer = "D"; break;
                        case SkipLogicAnswer.B: qObj.skipLogicAnswer = "B"; break;
                        default: qObj.skipLogicAnswer = "N"; break;
                    }
                    if (!string.IsNullOrEmpty(question.skipLogicJump) && question.questionResponses.Count > 0 && question.skipLogicAnswer == SkipLogicAnswer.D)
                    {
                        foreach (var response in question.questionResponses)
                            question.skipLogicJump = question.skipLogicJump.Replace(response.response.ToString(), response.response1.zcode);
                    }

                    var questionSurvey = question.surveys.FirstOrDefault();
                    qObj.Survey = questionSurvey.description;
                    var questionSurveySet = questionSurvey.surveyset.FirstOrDefault();
                    qObj.Surveyset = questionSurveySet.description;
                    var page = questionSurveySet.page.FirstOrDefault();
                    var pageIndexArr = page.description.Split(new char[] { ' ' });
                    qObj.Page = int.Parse(pageIndexArr[1]);
                    result.Add(qObj);
                }
            }
            serializer.Serialize(stream, result);
            stream.Position = 0;
            return File(stream, "application/vnd.ms-excel", "Questionnaire.xls");

        }

        private int GetPage(ExcelQuestionnaire previosRow, ExcelQuestionnaire currentRow, List<int> landingPages)
        {
            return previosRow != null ? (previosRow.Surveyset != currentRow.Surveyset || previosRow.skipLogic.ToLower() == "y" || landingPages.Any(o => o == currentRow.QID) ? previosRow.Page + 1 : previosRow.Page) : 1;
        }
        [HttpPost]
        public ActionResult UploadQuestionnaire(int protocol, string protocolName
            , int touchpoint, string touchpointName
            , int partnertype, string partnertypeName, int level
            )
        {
            int rowNumber = 0;
            int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;
            if (ModelState.IsValid)
            {
                using (var scope = db.Database.BeginTransaction())
                {
                    try
                    {
                        #region part1
                        person objPerson = db.pr_getPersonByEmail(EnterpriseID, User.Identity.Name).FirstOrDefault();

                        string protocolTitle = protocolName;
                        string touchpointTitle = touchpointName;
                        string providerTypTitle = partnertypeName;
                        string questionnaireTitle = protocolTitle + " " + touchpointTitle + " " + providerTypTitle;


                        var fileName = TempData["fileName"];
                        var physicalPath = TempData["physicalPath"];
                        string sheetname = "surveyQuestion";
                        //var excelRead = new ExcelQueryFactory(Convert.ToString(physicalPath));
                        //excelRead.TrimSpaces = LinqToExcel.Query.TrimSpacesType.Both;
                        //  excelRead.AddMapping<ExcelPartner>(x => x.internalID, "Internal ID");
                        List<ExcelQuestionnaire> questionnaireinExcel = null;
                        try
                        {
                            questionnaireinExcel = ExcelMapper.GetRows<ExcelQuestionnaire>(Convert.ToString(physicalPath), sheetname).Where(o => !string.IsNullOrEmpty(o.Response) && !string.IsNullOrEmpty(o.Question)).ToList();
                            //questionnaireinExcel = (from a in excelRead.Worksheet<ExcelQuestionnaire>(sheetname) select a).ToList().Where(o=>!string.IsNullOrEmpty(o.Response)&&!string.IsNullOrEmpty(o.Question)).ToList();
                        }
                        catch (NullReferenceException ex)
                        {
                            throw new Exception("Please select all blank rows, right click, and select delete and try again.");
                        }
                        //validateQuestionnaire

                        questionnaire objQuestionnaire = new questionnaire();
                        objQuestionnaire.description = questionnaireTitle;
                        objQuestionnaire.enterprise = EnterpriseID;
                        objQuestionnaire.letter = 0;
                        objQuestionnaire.title = questionnaireTitle;
                        objQuestionnaire.person = objPerson.id;
                        objQuestionnaire.partnerType = partnertype;
                        objQuestionnaire.multiLanguage = 0;
                        objQuestionnaire.active = 1;
                        objQuestionnaire.levelType = level;
                        db.Entry(objQuestionnaire).State = EntityState.Added;
                        db.SaveChanges();
                        int questionnaireId = objQuestionnaire.id;


                        Session["QuestionnaireId"] = questionnaireId;
                        Session["protocolId"] = protocol;
                        Session["touchpointId"] = touchpoint;
                        Session["partnertypeId"] = partnertype;
                        Session["level"] = level;

                        partnerTypeTouchpointQuestionnaire objPartnertypeTouchpointQuestionnaire = new partnerTypeTouchpointQuestionnaire();
                        objPartnertypeTouchpointQuestionnaire.partnerType = partnertype;
                        objPartnertypeTouchpointQuestionnaire.touchpoint = touchpoint;
                        objPartnertypeTouchpointQuestionnaire.questionnaire = questionnaireId;
                        objPartnertypeTouchpointQuestionnaire.active = true;
                        db.partnerTypeTouchpointQuestionnaire.Add(objPartnertypeTouchpointQuestionnaire);
                        db.SaveChanges();

                        string previousPage = string.Empty, previousSurveySet = string.Empty, previousSurvey = string.Empty,
                            responses = string.Empty, responseType = string.Empty, jumpToQIDstr = string.Empty;

                        int pageId = 0, surveySetId = 0, surveyId = 0, responseTypeId = 0, isRequired = 0,
                            isRequiredComment = 0, jumpToQID = 0, hasSkipLogicQuestionId = 0, isSkipLogicAnwerYes = 0,
                            k = 0, responseId = 0, questionValue = 0;
                        ExcelQuestionnaire previosExcelRow = null;
                        List<int> landingPages = new List<int>();
                        Dictionary<int, question> questions = new Dictionary<int, question>();
                        Dictionary<int, string> narrativeHints = new Dictionary<int, string>();
                        var questionnaireLoads = new HashSet<int>();
                        var questionSet = new HashSet<int>();
                        var responseSet = new HashSet<int>();
                        var surveySet = new HashSet<int>();
                        var surveySetSets = new HashSet<int>();
                        var pagesSet = new HashSet<int>();


                        foreach (var excelQuestionnaire in questionnaireinExcel)
                        {
                            rowNumber++;
                            excelQuestionnaire.Page = GetPage(previosExcelRow, excelQuestionnaire, landingPages);
                            previosExcelRow = excelQuestionnaire;
                            responses = null; responseType = string.Empty;
                            //if (string.IsNullOrEmpty(excelQuestionnaire.Response))
                            //    throw new Exception("Response value for question with ID " + excelQuestionnaire.QID + " is empty. Please edit spreadsheet and reupload it");

                            if (string.IsNullOrEmpty(excelQuestionnaire.Surveyset) ||
                                string.IsNullOrEmpty(excelQuestionnaire.Survey) || string.IsNullOrEmpty(excelQuestionnaire.Question) ||
                                string.IsNullOrEmpty(excelQuestionnaire.Response) || string.IsNullOrEmpty(excelQuestionnaire.Title) ||
                                string.IsNullOrEmpty(excelQuestionnaire.Required))
                            {
                                string errorMessage = "";
                                if (string.IsNullOrEmpty(excelQuestionnaire.Surveyset))
                                {
                                    errorMessage = "Value for QId = " + excelQuestionnaire.QID + " and Column = Surveyset is empty , ";
                                }
                                if (string.IsNullOrEmpty(excelQuestionnaire.Survey))
                                {
                                    errorMessage += "Value for QId = " + excelQuestionnaire.QID + " and Column = Survey is empty , ";
                                }
                                if (string.IsNullOrEmpty(excelQuestionnaire.Question))
                                {
                                    errorMessage += " Value for QId = " + excelQuestionnaire.QID + " and Column = Question is empty , ";
                                }
                                if (string.IsNullOrEmpty(excelQuestionnaire.Response))
                                {
                                    errorMessage += " Value for QId = " + excelQuestionnaire.QID + " and Column = Response is empty , ";
                                }
                                if (string.IsNullOrEmpty(excelQuestionnaire.Title))
                                {
                                    errorMessage += "Value for QId = " + excelQuestionnaire.QID + " and Column = Title is empty , ";
                                }
                                if (string.IsNullOrEmpty(excelQuestionnaire.Required))
                                {
                                    errorMessage += "Value for QId = " + excelQuestionnaire.QID + " and Column = Required is empty , ";
                                }
                                if (errorMessage != "")
                                {
                                    errorMessage = errorMessage.TrimEnd(',');
                                    errorMessage += "Please edit spreadsheet and reupload it";
                                    throw new Exception(errorMessage);
                                }

                            }

                            if (excelQuestionnaire.Page < 1 || excelQuestionnaire.Surveyset.Length < 1 || excelQuestionnaire.Survey.Length < 1 || excelQuestionnaire.Question.Length < 1 || excelQuestionnaire.Response.Length < 1 || excelQuestionnaire.Title.Length < 1 || excelQuestionnaire.Required.Length < 1)
                            {
                                //  || excelQuestionnaire.Comment.Length < 1

                                //skip this row.
                                //no useful anymore
                            }
                            else
                            {
                                if (previousPage != "Page " + excelQuestionnaire.Page.ToString())
                                {
                                    page objPage = new page();
                                    objPage.description = "Page " + excelQuestionnaire.Page.ToString();
                                    objPage.active = true;
                                    objPage.sortOrder = 1;
                                    db.page.Add(objPage);

                                    db.SaveChanges();

                                    pageId = objPage.id;
                                    if (!pagesSet.Contains(pageId))
                                        pagesSet.Add(pageId);
                                    db.pr_addQuestionnairePage(questionnaireId, pageId);
                                    previousPage = objPage.description;

                                    addSurvetSet(ref previousSurveySet, ref previousSurvey, pageId, ref surveySetId, ref surveyId, excelQuestionnaire, surveySetSets, surveySet);
                                }

                                //add a new SurveySet if the previous suveySet and current surveySet are not the same
                                if (previousSurveySet != excelQuestionnaire.Surveyset && previousPage == "Page " + excelQuestionnaire.Page.ToString())
                                {
                                    addSurvetSet(ref previousSurveySet, ref previousSurvey, pageId, ref surveySetId, ref surveyId, excelQuestionnaire, surveySetSets, surveySet);
                                }

                                //add a new survey if the previous survey and current survey are not the same
                                if (previousSurvey != excelQuestionnaire.Survey && previousPage == "Page " + excelQuestionnaire.Page.ToString() && previousSurveySet == excelQuestionnaire.Surveyset)
                                {
                                    addSurvey(ref previousSurvey, surveySetId, ref surveyId, excelQuestionnaire, surveySet);
                                }

                                if (excelQuestionnaire.Response.ToLower().Contains("list:"))
                                {
                                    responses = excelQuestionnaire.Response.Substring(5, excelQuestionnaire.Response.Length - 5).Trim();
                                    responseType = "List";
                                }
                                else if (excelQuestionnaire.Response.ToLower().Contains("dropdown:"))
                                {
                                    responses = excelQuestionnaire.Response.Substring(9, excelQuestionnaire.Response.Length - 9).Trim();
                                    responseType = "DropDown";
                                }
                                else if (excelQuestionnaire.Response.ToLower().Contains("checkbox:"))
                                {
                                    responses = excelQuestionnaire.Response.Substring(9, excelQuestionnaire.Response.Length - 9).Trim();
                                    responseType = "CheckBox";
                                }
                                else if (excelQuestionnaire.Response.ToLower().Contains("text_"))
                                {
                                    responseType = excelQuestionnaire.Response.ToLower();
                                }

                                //get response type
                                switch (excelQuestionnaire.Response.ToLower())
                                {
                                    case "y/n/na":
                                        responseTypeId = 3;
                                        break;
                                    case "y/n":
                                        responseTypeId = 3;
                                        break;
                                    case "y/n/cots":
                                        responseTypeId = 3;
                                        break;
                                    case "text":
                                        responseTypeId = 4;
                                        break;
                                    case "number":
                                        responseTypeId = 6;
                                        break;
                                    case "comment":
                                        responseTypeId = 7;
                                        break;
                                    case "dropdown":
                                        responseTypeId = 10;
                                        break;
                                    case "list":
                                        responseTypeId = 11;
                                        break;
                                    case "checkbox":
                                        responseTypeId = 12;
                                        break;
                                    case "upload":
                                        responseTypeId = 13;
                                        break;
                                    case "text/upload":
                                        responseTypeId = 14;
                                        break;
                                    case "text_number":
                                        responseTypeId = 6;
                                        break;
                                    case "text_email":
                                        responseTypeId = 16;
                                        break;
                                    default:
                                        if (responseType.ToLower().Contains("text_number_"))
                                        {
                                            responseTypeId = 17;
                                        }
                                        else if (responseType.ToLower().Contains("text_"))
                                        {
                                            responseTypeId = 18;
                                        }
                                        else if (responseType.ToLower().Substring(0, 4) == "list")
                                        {
                                            responseTypeId = 11;
                                        }
                                        else if (responseType.ToLower().Substring(0, 8) == "dropdown")
                                        {
                                            responseTypeId = 10;
                                        }
                                        else if (responseType.ToLower().Substring(0, 8) == "checkbox")
                                        {
                                            responseTypeId = 12;
                                        }



                                        break;
                                }
                                if (excelQuestionnaire.Required.ToLower() == "y" || excelQuestionnaire.Required.ToLower() == "yes" || excelQuestionnaire.Required.ToLower() == "1")
                                {
                                    isRequired = 1;
                                }
                                else
                                {
                                    isRequired = 0;
                                }
                                if (excelQuestionnaire.Comment == null)
                                {
                                    excelQuestionnaire.Comment = "N";
                                }
                                String tagValue = "";
                                switch (excelQuestionnaire.Comment.ToLower())
                                {
                                    case "y": isRequiredComment = 1; break;
                                    case "1": isRequiredComment = 1; break;
                                    case "0": isRequiredComment = 0; break;
                                    case "n": isRequiredComment = 0; break;
                                    default:
                                        tagValue = excelQuestionnaire.Comment;
                                        excelQuestionnaire.Comment = "N";
                                        //TODO: skiplogic with comment field
                                        break;
                                }


                                question objQuestion = new question();

                                objQuestion.Question = excelQuestionnaire.Question;
                                objQuestion.name = excelQuestionnaire.Question;
                                objQuestion.title = excelQuestionnaire.Title;
                                objQuestion.tag = excelQuestionnaire.spinoffid.ToString();

                                objQuestion.responseType = responseTypeId;
                                objQuestion.required = responseTypeId == 17 || responseTypeId == 18 ? int.Parse(responseType.Split("_".ToArray(), StringSplitOptions.RemoveEmptyEntries)[responseTypeId == 17 ? 2 : 1]) : isRequired;
                                objQuestion.calendarMessageTxt = excelQuestionnaire.CalendarMessageText;

                                if (excelQuestionnaire.skipLogicAnswer == "N")
                                {
                                    objQuestion.skipLogicAnswer = SkipLogicAnswer.N;
                                }
                                else if (excelQuestionnaire.skipLogicAnswer == "Y")
                                {
                                    objQuestion.skipLogicAnswer = SkipLogicAnswer.Y;
                                }
                                else if (excelQuestionnaire.skipLogicAnswer == "M")
                                {
                                    objQuestion.skipLogicAnswer = SkipLogicAnswer.M;
                                }
                                else if (excelQuestionnaire.skipLogicAnswer == "A")
                                {
                                    objQuestion.skipLogicAnswer = SkipLogicAnswer.A;
                                }
                                else if (excelQuestionnaire.skipLogicAnswer == "D")
                                {
                                    objQuestion.skipLogicAnswer = SkipLogicAnswer.D;
                                }
                                else if ((excelQuestionnaire.skipLogicAnswer??"").ToUpper() == "B")
                                {
                                    objQuestion.skipLogicAnswer = SkipLogicAnswer.B;
                                }
                                else if (!string.IsNullOrEmpty(excelQuestionnaire.skipLogicAnswer) && excelQuestionnaire.skipLogicAnswer != "NULL")
                                {
                                    throw new Exception("Question with QID:" + excelQuestionnaire.QID + " has wrong skipLogicAnswer value. The possible values are : N, Y, M, A, D, NULL or empty value.");
                                }

                                //objQuestion.skipLogicJump Pending
                                objQuestion.accessLevel = 1;
                                objQuestion.commentRequired = isRequiredComment;
                                objQuestion.commentBoxTxt = excelQuestionnaire.CommentBoxMessageText;
                                objQuestion.commentUploadTxt = excelQuestionnaire.UploadMessageText;
                                objQuestion.subCheckBoxChoice = excelQuestionnaire.SubCheckBoxChoice;
                                if (!string.IsNullOrEmpty(excelQuestionnaire.CommentType))
                                    excelQuestionnaire.CommentType = excelQuestionnaire.CommentType.ToUpper();
                                if (excelQuestionnaire.CommentType == "YN_WARNING_N")
                                {
                                    objQuestion.commentType = CommentType.YN_WARNING_N;
                                }
                                else if (excelQuestionnaire.CommentType == "YN_WARNING_Y")
                                {
                                    objQuestion.commentType = CommentType.YN_WARNING_Y;
                                }
                                else if (excelQuestionnaire.CommentType == "YN_COMMENT_Y")
                                {
                                    objQuestion.commentType = CommentType.YN_COMMENT_Y;
                                }
                                else if (excelQuestionnaire.CommentType == "YN_COMMENT_N")
                                {
                                    objQuestion.commentType = CommentType.YN_COMMENT_N;
                                }
                                else if (excelQuestionnaire.CommentType == "YN_UPLOAD_Y")
                                {
                                    objQuestion.commentType = CommentType.YN_UPLOAD_Y;
                                }
                                else if (excelQuestionnaire.CommentType == "YN_UPLOAD_N")
                                {
                                    objQuestion.commentType = CommentType.YN_UPLOAD_N;
                                }
                                else if (excelQuestionnaire.CommentType == "YN_NO_COMMENT")
                                {
                                    objQuestion.commentType = CommentType.YN_NO_COMMENT;
                                }
                                else if (excelQuestionnaire.CommentType == "DROPDOWN_COMMENT")
                                    objQuestion.commentType = CommentType.DROPDOWN_COMMENT;
                                else if (excelQuestionnaire.CommentType == "YN_DUEDATE_Y")
                                    objQuestion.commentType = CommentType.YN_DUEDATE_Y;
                                else if (excelQuestionnaire.CommentType == "YN_DUEDATE_N")
                                    objQuestion.commentType = CommentType.YN_DUEDATE_N;
                                else if (excelQuestionnaire.CommentType == "YN_ALERT_Y")
                                    objQuestion.commentType = CommentType.YN_ALERT_Y;
                                else if (excelQuestionnaire.CommentType == "YN_ALERT_N")
                                    objQuestion.commentType = CommentType.YN_ALERT_N;
                                else if (excelQuestionnaire.CommentType == "YN_CHECKBOX_Y")
                                    objQuestion.commentType = CommentType.YN_CHECKBOX_Y;
                                else if (excelQuestionnaire.CommentType == "YN_CHECKBOX_N")
                                    objQuestion.commentType = CommentType.YN_CHECKBOX_N;
                                else if (excelQuestionnaire.CommentType == "XX_CHECKBOX_X")
                                    objQuestion.commentType = CommentType.XX_CHECKBOX_X;
                                else if (excelQuestionnaire.CommentType == "YN_UPLOADEXPIRY_Y")
                                    objQuestion.commentType = CommentType.YN_UPLOADEXPIRY_Y;
                                else if (excelQuestionnaire.CommentType == "YN_UPLOADEXPIRY_N")
                                    objQuestion.commentType = CommentType.YN_UPLOADEXPIRY_N;
                                else if (excelQuestionnaire.CommentType == "XX_COMMENT_ALL")
                                    objQuestion.commentType = CommentType.XX_COMMENT_ALL;
                                else if (excelQuestionnaire.CommentType == "YN_REFERENCE_Y")
                                    objQuestion.commentType = CommentType.YN_REFERENCE_Y;
                                else if (excelQuestionnaire.CommentType == "YN_REFERENCE_N")
                                    objQuestion.commentType = CommentType.YN_REFERENCE_N;
                                else if (excelQuestionnaire.CommentType == "YN_COLORPICKER_N")
                                    objQuestion.commentType = CommentType.YN_COLORPICKER_N;
                                else if (excelQuestionnaire.CommentType == "YN_COLORPICKER_Y")
                                    objQuestion.commentType = CommentType.YN_COLORPICKER_Y;
                                else if (!string.IsNullOrEmpty(excelQuestionnaire.CommentType) && excelQuestionnaire.CommentType.StartsWith("TEXT_NUMBER_N_"))
                                {
                                    try
                                    {
                                        objQuestion.commentType = int.Parse(CommentType.TEXT_NUMBER_N_X.ToString() + excelQuestionnaire.CommentType.Substring("TEXT_NUMBER_N_".Length));
                                    }
                                    catch
                                    {
                                        throw new Exception("Wrong TEXT_NUMBER_N_ comment type value");
                                    }
                                }
                                else if (!string.IsNullOrEmpty(excelQuestionnaire.CommentType) && excelQuestionnaire.CommentType.StartsWith("TEXT_NUMBER_Y_"))
                                {
                                    try
                                    {
                                        objQuestion.commentType = int.Parse(CommentType.TEXT_NUMBER_Y_X.ToString() + excelQuestionnaire.CommentType.Substring("TEXT_NUMBER_Y_".Length));
                                    }
                                    catch
                                    {
                                        throw new Exception("Wrong TEXT_NUMBER_Y_ comment type value");
                                    }
                                }
                                else
                                {
                                    objQuestion.commentType = 0;
                                }

                                try
                                {
                                    objQuestion.spinOffQuestionnaire = excelQuestionnaire.snipOffQuestionnaire.Substring(0, 1);
                                }
                                catch { }
                                objQuestion.spinOffQID = excelQuestionnaire.spinoffid;
                                try
                                {
                                    objQuestion.emailAlert = excelQuestionnaire.emailalert.Substring(0, 1);
                                }
                                catch { }
                                try
                                {
                                    objQuestion.emailAlertList = excelQuestionnaire.emailalertlist;
                                    objQuestion.sortOrder = 1;
                                    objQuestion.active = true;
                                    objQuestion.enterprise = EnterpriseID;
                                    db.questions.Add(objQuestion);
                                    db.SaveChanges();
                                    questionSet.Add(objQuestion.id);
                                    excelQuestionnaire.CommentBoxMessageText = excelQuestionnaire.CommentBoxMessageText ?? "";
                                    excelQuestionnaire.UploadMessageText = excelQuestionnaire.UploadMessageText ?? "";
                                    excelQuestionnaire.CommentType = excelQuestionnaire.CommentType ?? "";
                                    db.pr_addQuestionnaireLoad(excelQuestionnaire.QID, excelQuestionnaire.Page, excelQuestionnaire.Surveyset, excelQuestionnaire.Survey, excelQuestionnaire.Question, excelQuestionnaire.Response, excelQuestionnaire.Comment, excelQuestionnaire.Title, excelQuestionnaire.Required, excelQuestionnaire.Length, excelQuestionnaire.titleLength, excelQuestionnaire.yValue, excelQuestionnaire.nValue, excelQuestionnaire.otherValue, excelQuestionnaire.qWeight, excelQuestionnaire.skipLogic ?? "", excelQuestionnaire.skipLogicAnswer ?? "", excelQuestionnaire.SubCheckBoxChoice ?? "", excelQuestionnaire.CalendarMessageText, excelQuestionnaire.skipLogicJump ?? "", excelQuestionnaire.CommentBoxMessageText ?? "", excelQuestionnaire.UploadMessageText ?? "", excelQuestionnaire.CommentType ?? "", "", excelQuestionnaire.spinoffid, excelQuestionnaire.emailalert ?? "", excelQuestionnaire.emailalertlist ?? "", questionnaireId).FirstOrDefault();
                                }
                                catch (Exception ex)
                                {


                                }
                                int questionId = objQuestion.id;
                                if (responseType.ToLower() == "list" || responseType.ToLower() == "dropdown"
                                        || responseType.ToLower() == "checkbox")
                                {


                                    //add responses

                                    string[] sArray = new string[200];
                                    char[] splitter = { ';' };
                                    int id = 0;
                                    sArray = responses.Split(splitter, StringSplitOptions.RemoveEmptyEntries);

                                    for (int i = 0; i < sArray.Length; i++)
                                    {
                                        //add Response
                                        try
                                        {
                                            response objResponse = new response();
                                            objResponse.active = true;
                                            objResponse.description = sArray[i];
                                            objResponse.enterprise = EnterpriseID;
                                            if (responseType.ToLower() == "dropdown" || responseType.ToLower() == "list")
                                            {
                                                Regex reg = new Regex("\\([A-Z][A-Z]\\)");
                                                var match = reg.Match(objResponse.description);
                                                if (match.Success)
                                                {
                                                    objResponse.zcode = match.Value.Replace("(", "").Replace(")", "");
                                                }
                                            }
                                            objResponse.sortOrder = 1;
                                            db.response.Add(objResponse);
                                            db.SaveChanges();
                                            int responsesId = objResponse.id;
                                            responseSet.Add(responsesId);
                                            //addQuestionResponse
                                            db.pr_addQuestionResponse(questionId, responsesId);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }

                                db.pr_addSurveyQuestion(surveyId, questionId);

                                if (excelQuestionnaire.skipLogicAnswer == "NULL")
                                {
                                    excelQuestionnaire.skipLogicAnswer = null;
                                }

                                if (excelQuestionnaire.skipLogic == "NULL")
                                {
                                    excelQuestionnaire.skipLogic = null;
                                }

                                if (excelQuestionnaire.skipLogicJump == "NULL")
                                {
                                    excelQuestionnaire.skipLogicJump = null;
                                }
                                questions.Add(excelQuestionnaire.QID, db.pr_getQuestion(objQuestion.id).FirstOrDefault());
                                #endregion

                                if (!string.IsNullOrEmpty(excelQuestionnaire.NarrativeHint))
                                    narrativeHints.Add(objQuestion.id, excelQuestionnaire.NarrativeHint);

                                if (!string.IsNullOrEmpty(excelQuestionnaire.skipLogic) && !string.IsNullOrEmpty(excelQuestionnaire.skipLogicJump))
                                {
                                    var allAnswers = excelQuestionnaire.skipLogicJump.Split(";".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                                    foreach (var answer in allAnswers)
                                    {
                                        var split = answer.Split(":");
                                        if (split.Length > 2)
                                        {
                                            landingPages.Add(Convert.ToInt32(split[1]));
                                            landingPages.Add(Convert.ToInt32(split[2]));
                                        }
                                        else if (split.Length > 1)
                                        {
                                            landingPages.Add(Convert.ToInt32(split[1]));
                                        }
                                    }
                                    //landingPages.AddRange(allAnswers.Select(o => o.Split(":".ToCharArray()).Length > 1 ? Convert.ToInt32(o.Split(":".ToCharArray())[1]) : 0).ToList().Distinct());

                                    //try getting skipLogicJump Qid                                
                                    //if (excelQuestionnaire.skipLogicAnswer == "D")
                                    //{
                                    //    //lets use answer's codes mapping for multiply answers skip logic
                                    //    jumpToQIDstr = getskipLogicJumpQuestionIdLogic(questionId, excelQuestionnaire.QID, excelQuestionnaire.skipLogic, excelQuestionnaire.skipLogicJump, GetCodeMapping(db.pr_getResponseByQuestion(questionId).ToList(), responses));
                                    //}
                                    //else 

                                    try
                                    {
                                        jumpToQIDstr = getskipLogicJumpQuestionIdLogic(questions, excelQuestionnaire.skipLogicJump);
                                    }
                                    catch (Exception exp) {
                                        throw new Exception(exp.Message + " " + excelQuestionnaire.skipLogicJump);
                                    }

                                    if (jumpToQIDstr.Length > 0)
                                    {
                                        hasSkipLogicQuestionId = questionId;

                                        db.pr_modifyQuestionSkipLogicJumpLogic(questionId, jumpToQIDstr);
                                    }
                                    else
                                    {
                                        hasSkipLogicQuestionId = 0;
                                    }
                                }
                                if (!string.IsNullOrEmpty(tagValue))
                                {
                                    string tagResultValue = "";
                                    var stringsToParse = tagValue.Split("!");
                                    foreach (var stringtoParse in stringsToParse)
                                    {
                                        var stringsToSplit = stringtoParse.Split("@");
                                        if (stringsToSplit.Length == 2)
                                        {
                                            tagResultValue += stringsToSplit[0] + "@" + getskipLogicJumpQuestionIdLogic(questions, stringsToSplit[1], isCalculation:true);
                                        }
                                    }
                                    db.pr_modifyQuestionTag(questionId, tagResultValue);
                                }
                                if (excelQuestionnaire.Response.ToLower() == "y/n/cots")
                                {
                                    db.pr_addQuestionResponse(questionId, 74);
                                    db.pr_addQuestionResponse(questionId, 75);
                                    db.pr_addQuestionResponse(questionId, 77);
                                }
                                //if the responseType is "Y/N" then add question weight and responseValue for the question
                                if (excelQuestionnaire.Response.ToLower() == "y/n" || excelQuestionnaire.Response.ToLower() == "y/n/na")
                                {
                                    //sp_addProtocolCampaignQuestionWeight
                                    //   db.pr_addtouchpointQuestionWeight(touchpointId, questionId, excelQuestionnaire.qWeight);

                                    //add questionResponse
                                    if (excelQuestionnaire.Response.ToLower() == "y/n")
                                    {
                                        k = 75;
                                    }
                                    else
                                    {
                                        k = 76;
                                    }


                                    for (int j = 74; j <= k; ++j)
                                    {
                                        //create new response instant
                                        responseId = j;

                                        //add record to questionResponse table

                                        db.pr_addQuestionResponse(questionId, responseId);

                                        switch (responseId)
                                        {
                                            case 74:
                                                questionValue = excelQuestionnaire.yValue;
                                                break;
                                            case 75:
                                                questionValue = excelQuestionnaire.nValue;
                                                break;
                                            case 76:
                                                questionValue = excelQuestionnaire.naValue;
                                                break;
                                            default:
                                                break;
                                        }

                                        //add questionResponseValues
                                        //  db.pr_addTouchpointQuestionResponseValue(touchpointId, questionId, responseId, questionValue);

                                    }
                                }
                            }
                        }
                        foreach (var hint in narrativeHints)
                        {
                            CreateNarrativeHintsForQuestion(hint.Key, hint.Value, questions);
                        }
                        scope.Commit();
                        return RedirectToAction("UploadCMS");
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            ViewBag.Error = ex.InnerException.Message;
                        }
                        catch
                        {
                            ViewBag.Error = ex.Message;
                        }
                        ViewBag.protocol = new SelectList(db.pr_getProtocolAll(EnterpriseID), "id", "name");
                        ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAll(), "id", "description");
                        ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(EnterpriseID), "id", "name");
                        ViewBag.level = new SelectList(db.pr_getQuestionnaireLevelTypeByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
                        return Json(new { error = (ex.InnerException != null ? ex.Message + "; " + ex.InnerException.Message : ex.Message) + " On row:" + rowNumber });
                    }

                }
                //var fileFolderName = ViewData["FileFolderName"];
            }
            return View(); // this will never executed
        }

        private void CreateNarrativeHintsForQuestion(int questionId, string fullHintsText, Dictionary<int, question> questions)
        {
            const string FirstDelimeter = "=";
            const string SecondDelimiter = ":";
            const string ThirdDelimiter = ";";
            const string FourthDelimiter = "#";
            const string FifthDelimiter = "&";
            int reference_qid = 0, sortOrder = 0;
            var firstLoop = fullHintsText.Replace("<HINT>", "").Replace("</HINT>", "").Split(FirstDelimeter.ToCharArray());



            if (firstLoop.Length > 1)
            {
                var qidReference = firstLoop[0].Split(SecondDelimiter.ToCharArray());
                if (qidReference.Length != 2)
                    throw new Exception("Narrative hint field text for QID " + questionId + " doesn't contain correct syntax {QID}:{REF_QID}, there is only one identifier");
                var innerQuestionId = qidReference[0];
                if (questions.ContainsKey(int.Parse(qidReference[1])))
                    reference_qid = questions[int.Parse(qidReference[1])].id;
                else
                {
                    var difference = int.Parse(qidReference[1]) - int.Parse(qidReference[0]);
                    reference_qid = questionId + difference;
                }
                var responsesCodes = db.pr_getResponseByQuestion(reference_qid).ToList();
                var responses = firstLoop[1].Split(FifthDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                foreach (var response in responses)
                {
                    var responseItems = response.Split(SecondDelimiter.ToCharArray());
                    if (responseItems.Length != 2)
                        throw new Exception("Narrative hint field text for QID " + innerQuestionId + " doesn't contain correct syntax in response defenition: " + response);
                    var responseCode = responseItems[0];
                    var responseObj = responsesCodes.FirstOrDefault(o => o.description.Contains("(" + responseCode + ")"));
                    if (responseObj == null)
                        throw new Exception("Narrative hint field text for QID " + innerQuestionId + " .Can't find respnse object for response code: " + responseCode);
                    var subjects = responseItems[1].Split(ThirdDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (subjects.Length == 0)
                        throw new Exception("Narrative hint field text for QID: " + innerQuestionId + " doesn't contain correct syntax near response code: " + responseCode + ". There should be at least one subject#text pair for current response code");
                    foreach (var subject in subjects)
                    {
                        var pair = subject.Split(FourthDelimiter.ToCharArray());
                        if (pair.Length != 2)
                            throw new Exception("Narrative hint field text for QID: " + innerQuestionId + " doesn't contain correct syntax near response code: " + responseCode + " in subject pair text:'" + subject + "'");
                        db.pr_addQuestionResponseNarrativeSelectionList(questionId, reference_qid, responseObj != null ? responseObj.id : (int?)null
                            , pair[0], pair[1], sortOrder++, true).FirstOrDefault();
                    }
                }
            }
            else throw new Exception("Narrative hint field text for QID " + questionId + " doesn't contain correct syntax {QID}:{REF_QID}=[{RESPONSES}]");
            //throw new NotImplementedException();
        }

        public ActionResult ExportExcelQuestionnaireDetail(string id)
        {

            Session["questionnaireexport"] = db.Database.SqlQuery<view_QuestionnaireCMS>("EXEC pr_getViewQuestionnaireCMSByQuestionnaire '" + id + "'").ToList();

            List<view_QuestionnaireCMS> abc = (List<view_QuestionnaireCMS>)Session["questionnaireexport"];

            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<view_QuestionnaireCMS>));

            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, abc);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "QuestionnaireList.xls");
        }

        public ActionResult ExportExcelAutoMailDetail(string id)
        {

            Session["AutoMailexport"] = db.Database.SqlQuery<QuestionnaireAutoMailViewModel>("EXEC pr_getAutomailMessageByQuestionnaire '" + id + "'").ToList();

            List<AutoMailExportModel> lstAutoMail = ((List<QuestionnaireAutoMailViewModel>)Session["AutoMailexport"]).Select(o => new AutoMailExportModel()
            {
                RID = o.id,
                Footer = o.footer1,
                Send_Date_Calc_Factor = o.sendDateCalcFactor,
                Signature = o.footer2,
                Subject = o.subject,
                Text = o.text,
                Type = (int)o.mailType
            }).ToList();

            var stream = new MemoryStream();
            var serializer = new XmlSerializer(typeof(List<AutoMailExportModel>));

            //We turn it into an XML and save it in the memory
            serializer.Serialize(stream, lstAutoMail);
            stream.Position = 0;

            //We return the XML from the memory as a .xls file
            return File(stream, "application/vnd.ms-excel", "AutoMailQuestionnaireList.xls");
        }



        private int getskipLogicJumpQuestionId(int questionId, int QID, string skipLogic, string skipLogicAnswer, string skipLogicJump)
        {
            int number = 0;

            if (string.IsNullOrEmpty(skipLogic) || string.IsNullOrEmpty(skipLogicJump))
            {
                return 0;
            }
            else if (int.TryParse(skipLogicJump, out number))
            {
                if (QID > number)
                {
                    return 0;
                }
                else
                {
                    number = questionId + (number - QID);
                    return number;
                }
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Maps countries codes to reponse id
        /// </summary>
        /// <param name="responses">list of response objects</param>
        /// <param name="reponseString">response flat string</param>
        /// <returns>Mapped collection</returns>
        private Dictionary<string, int> GetCodeMapping(IEnumerable<response> responses, string reponseString)
        {
            Dictionary<string, int> result = new Dictionary<string, int>();
            var responsesStringArray = reponseString.Split(";");
            //looking for (UA), (US), (RU) matches
            Regex reg = new Regex("\\([A-Z][A-Z]\\)");
            foreach (var resString in responsesStringArray)
            {
                var match = reg.Match(resString);
                if (match.Success)
                {
                    var reponse = responses.FirstOrDefault(o => o.description == resString);
                    if (reponse != null)
                    {
                        result.Add(match.Value.Replace("(", "").Replace(")", ""), reponse.id);
                    }
                }
            }
            return result;
        }

        private string getskipLogicJumpQuestionIdLogic(Dictionary<int, question> questionsIds, string skipLogicJump, bool isCalculation = false)
        {
            //1=Y&3=Y:15;1=Y&3=N:16;
            //QID is 3
            string retString = "";
            string[] arrySkipLogicJump = skipLogicJump.Split(';');

            for (int i = 0; i < arrySkipLogicJump.Length - 1; i++)
            {
                string[] subSrting = arrySkipLogicJump[i].Split("&|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                int QID = 0, questionId = 0;

                for (int j = 0; j < subSrting.Length; j++)
                {
                    bool isEquality = false;
                    string[] questionStr = null;
                    if (subSrting[j].Contains("!="))
                        questionStr = subSrting[j].Split("!=");
                    else if (subSrting[j].Contains('='))
                    {
                        questionStr = subSrting[j].Split('=');
                        isEquality = true;
                    }
                    if (questionStr != null)
                    {
                        int firstQid = QID = Convert.ToInt32(questionStr[0].ToString());
                        var question = questionsIds[firstQid];
                        int ActualFristQid = questionId = question.id;

                        var value = "";
                        var intValue = 0;
                        string[] tempStr = questionStr[1].Split(':');

                        if (tempStr.Length > 0)
                        {
                            questionStr[1] = tempStr[0];
                            value = tempStr[0];
                        }
                        if (int.TryParse(value, out intValue))
                        {

                        }
                        else
                        {
                            var responses = db.pr_getResponseByQuestion(question.id).ToList();
                            //there can be an error in the future, I've commented next rows because there was  an error for questions with 2 answers
                            //if (responses.Count > 2)
                            //{
                            var response = responses.FirstOrDefault(o => o.description.Contains("(" + value + ")"));
                            if (response != null)
                                value = response.id.ToString();
                            //}
                        }
                        if (j != subSrting.Length - 1)
                        {
                            retString += ActualFristQid + (isEquality ? "=" : "!=") + value + arrySkipLogicJump[i].Substring(arrySkipLogicJump[i].IndexOf(subSrting[j]) + subSrting[j].Length, 1);
                        }
                        else
                        {
                            retString += ActualFristQid + (isEquality ? "=" : "!=") + value;
                        }
                    }
                }
                string[] columnSp = arrySkipLogicJump[i].Split(':');
                if (columnSp.Length > 2)
                {
                    string number = "";
                    int gotoQID;
                    if (!isCalculation)
                    {
                        gotoQID = int.Parse(columnSp[1]);

                        number = (questionId + (gotoQID - QID)).ToString();
                    }
                    else number = columnSp[1];
                    retString += ":" + number;
                    if (!isCalculation)
                    {
                        gotoQID = int.Parse(columnSp[2]);
                        number = (questionId + (gotoQID - QID)).ToString();
                    }
                    else number = columnSp[2];
                    retString += ":" + number + ";";
                }
                else
                    if (columnSp.Length > 1)
                {
                    string number = "";
                    if (!isCalculation)
                    {
                        int gotoQID = int.Parse(columnSp[1]);
                        number = (questionId + (gotoQID - QID)).ToString();
                    }
                    else number = columnSp[1];
                    retString += ":" + number + ";";
                }
            }
            return retString;
        }



        private void addSurvetSet(ref string previousSurveySet, ref string previousSurvey, int pageId, ref int surveySetId, ref int surveyId, ExcelQuestionnaire excelQuestionnaire, HashSet<int> surveySet, HashSet<int> surveySetSurvey)
        {
            surveyset objsurveyset = new surveyset();
            objsurveyset.description = excelQuestionnaire.Surveyset;
            objsurveyset.active = true;
            objsurveyset.sortOrder = 1;
            objsurveyset.accessLevel = 1;
            db.surveyset.Add(objsurveyset);
            surveySet.Add(objsurveyset.id);
            db.SaveChanges();

            surveySetId = objsurveyset.id;
            db.pr_addPageSurveyset(pageId, surveySetId);
            db.SaveChanges();

            previousSurveySet = excelQuestionnaire.Surveyset;

            addSurvey(ref previousSurvey, surveySetId, ref surveyId, excelQuestionnaire, surveySetSurvey);
        }

        private void addSurvey(ref string previousSurvey, int surveySetId, ref int surveyId, ExcelQuestionnaire excelQuestionnaire, HashSet<int> surveySetSurvey)
        {
            survey objSurvey = new survey();
            objSurvey.description = excelQuestionnaire.Survey;
            objSurvey.active = true;
            objSurvey.sortOrder = 1;
            objSurvey.name = excelQuestionnaire.Survey;
            objSurvey.display = 1;
            objSurvey.creationDate = DateTime.Now;
            objSurvey.lastModifiedDate = DateTime.Now;
            db.survey.Add(objSurvey);
            surveySetSurvey.Add(objSurvey.id);
            db.SaveChanges();
            surveyId = objSurvey.id;
            db.pr_addSurveysetSurvey(surveySetId, surveyId);
            previousSurvey = excelQuestionnaire.Survey;
        }

        public ActionResult Save(HttpPostedFileBase attachments)
        {

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
            }

            if (!Directory.Exists((Server.MapPath("~/uploadedFiles/Questionnaire"))))
            {
                Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/Questionnaire"));
            }

            // The Name of the Upload component is "attachments" 
            var file = attachments;

            // Some browsers send file names with full path. This needs to be stripped.
            var fileName = Path.GetFileName(file.FileName);
            var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/Questionnaire"), fileName);

            TempData["fileName"] = fileName;
            TempData["physicalPath"] = physicalPath;
            // The files are not actually saved in this demo
            file.SaveAs(physicalPath);

            // Return an empty string to signify success
            return Content("");
        }
        public ActionResult Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"
            foreach (var fullName in fileNames)
            {
                var fileName = Path.GetFileName(fullName);
                var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/Questionnaire"), fileName);

                // TODO: Verify user permissions
                if (System.IO.File.Exists(physicalPath))
                {
                    // The files are not actually removed in this demo
                    System.IO.File.Delete(physicalPath);
                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult FindQuestionnaire()
        {
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAllByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "title");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");

            var authors = db.pr_getPersonAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList()
                            .Select(s => new
                            {
                                id = s.id,
                                name = string.Format("{0} {1}", s.firstName, s.lastName)
                            })
                             .ToList();

            ViewBag.author = new SelectList(authors, "id", "name");

            return View();
        }

        [HttpPost]
        public ActionResult FindQuestionnaire(int? touchpoint, int? protocol, int? partnertype, int? author, string name, string description)
        {
            string arguments = "enterprise=" + Generic.Helpers.CurrentInstance.EnterpriseID + ";";

            if (touchpoint != null)
                arguments += "touchpointID=" + touchpoint + ";";
            if (protocol != null)
                arguments += "protocolID=" + protocol + ";";
            if (partnertype != null)
                arguments += "partnertypeID=" + partnertype + ";";
            if (author != null)
                arguments += "authorID=" + author + ";";
            if (name != null)
                arguments += "name=" + name + ";";
            if (description != null)
                arguments += "description=" + description + ";";


            var objPartners = db.Database.SqlQuery<view_QuestionnaireData>("EXEC pr_dynamicFiltersQuestionnaire  'view_QuestionnaireData' , '" + arguments + "'").ToList();

            Session["questionnaire"] = objPartners;

            return RedirectToAction("FindQuestionnaireResult");
        }

        public ActionResult FindQuestionnaireResult()
        {
            try
            {
                //List<view_QuestionnaireData> abc = (List<view_QuestionnaireData>)Session["questionnaire"];
                string arguments = Session["questionnairesearch"].ToString() + "active=1;";
                Session["questionnaire"] = db.Database.SqlQuery<view_QuestionnaireData>("EXEC pr_dynamicFiltersQuestionnaire  'view_QuestionnaireData' , '" + arguments + "'").ToList();
                List<view_QuestionnaireData> abc = (List<view_QuestionnaireData>)Session["questionnaire"];

                return View(abc);
            }
            catch
            {
                return RedirectToAction("FindQuestionnaire");

            }
        }

        [HttpPost]
        public void UploadExcelData(string id)
        {
            string questionnaireid = id;

            HttpPostedFileBase excelFile = Request.Files["file"];

            if (excelFile != null)
            {
                if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
                }

                if (!Directory.Exists((Server.MapPath("~/uploadedFiles/Questionnaire"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/Questionnaire"));
                }

                // The Name of the Upload component is "attachments" 
                var file = excelFile;
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/Questionnaire"), fileName);

                // The files are not actually saved in this demo
                file.SaveAs(physicalPath);
                string sheetname = "Sheet1";
                //var excelRead = new ExcelQueryFactory(physicalPath.ToString());

                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.questionnaire, questionnaireid);
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.questionnaireCMS, "questionnaireCMS");
                //Need to ignore now
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.section, "SECTION");
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.text, "text");
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.link, "link");
                //excelRead.AddMapping<ExcelQuestionnaireQuestionnireCMS>(x => x.doc, "doc");
                var questionnaireCMSinExcel = ExcelMapper.GetRows<ExcelQuestionnaireQuestionnireCMS>(physicalPath, sheetname);//from a in excelRead.Worksheet<ExcelQuestionnaireQuestionnireCMS>(sheetname) select a;
                List<Tuple<int, string>> uploadedquestionnaireCMS = new List<Tuple<int, string>>();

                foreach (var questionnaireCMSitem in questionnaireCMSinExcel.ToList())
                {
                    using (var context = new EntitiesDBContext())
                    {
                        if (!string.IsNullOrEmpty(questionnaireid))
                        {
                            //context.questionnaireQuestionnaireCMS.Attach(questionnaireCMSitem);

                            int modifiedQuestionnaire = context.pr_modifyQuestionnaireQuestionnaireCMS(Convert.ToInt32(questionnaireid),
                                questionnaireCMSitem.questionnaireCMS,
                                string.IsNullOrEmpty(questionnaireCMSitem.text) ? "" : questionnaireCMSitem.text,
                                questionnaireCMSitem.link, questionnaireCMSitem.doc, questionnaireCMSitem.docType);
                        }
                    }
                }
            }


            Response.Redirect(Url.Action("QuestionnaireQuestionnaireCMS", "Questionnaire", new { id = id }));
            // return RedirectToAction("QuestionnaireQuestionnaireCMS", new { id = int.Parse(id) });
            // return QuestionnaireQuestionnaireCMS(int.Parse(id));
            //  return RedirectToAction("QuestionnaireQuestionnaireCMS");
            //        return RedirectToAction("QuestionnaireQuestionnaireCMS", new RouteValueDictionary(
            //new { controller = "Questionnaire", action = "QuestionnaireQuestionnaireCMS", id = int.Parse(id) }));

        }

        [HttpPost]
        public ActionResult UploadAutoMailExcelData(string id)
        {
            string autoMailid = id;

            HttpPostedFileBase excelFile = Request.Files["file"];

            if (excelFile != null)
            {
                if (!Directory.Exists((Server.MapPath("~/uploadedFiles"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles"));
                }

                if (!Directory.Exists((Server.MapPath("~/uploadedFiles/AutoMailQuestionnaire"))))
                {
                    Directory.CreateDirectory(Server.MapPath("~/uploadedFiles/AutoMailQuestionnaire"));
                }

                // The Name of the Upload component is "attachments" 
                var file = excelFile;
                // Some browsers send file names with full path. This needs to be stripped.
                var fileName = Path.GetFileName(file.FileName);
                var physicalPath = Path.Combine(Server.MapPath("~/uploadedFiles/AutoMailQuestionnaire"), fileName);

                // The files are not actually saved in this demo
                file.SaveAs(physicalPath);
                string sheetname = "mailMessage";
                //var excelRead = new ExcelQueryFactory(physicalPath.ToString());
                //excelRead.AddMapping<ExcelAutoMailMessage>(x => x.SendDateCalcFactor, "Send Date Calc Factor");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.partnerTypeTouchpointQuestionnaire1, autoMailid);
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.id, "RID");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.subject, "Subject");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.text, "Text");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.footer1, "Footer");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.footer2, "Signature");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.sendDateCalcFactor, "Send Date Calc Factor");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.sendDateSet, "sendDateSet");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.mailType, "Type");
                //excelRead.AddMapping<QuestionnaireAutoMailViewModel>(x => x.partnerTypeTouchpointQuestionnaire, "partnerTypeTouchpointQuestionnaire");


                var map = new Dictionary<string, string>();
                map.Add("Send Date Calc Factor", "SendDateCalcFactor");
                var questionnaireCMSinExcel = ExcelMapper.GetRows<ExcelAutoMailMessage>(physicalPath, sheetname, map);
                List<Tuple<int, string>> uploadedquestionnaireCMS = new List<Tuple<int, string>>();

                var al = db.pr_getAutomailMessageByQuestionnaire(Convert.ToInt32(autoMailid)).ToList();
                foreach (var a in al)
                {
                    var items = db.pr_getAutoMailAttachmentByAutoMail(a.id).ToList();
                    foreach (var item in items)
                    {
                        db.pr_removeAutoMailAttachment(item.id);
                    }
                }

                db.pr_removeAutomailMessageByQuestionnaire(Convert.ToInt32(autoMailid));


                foreach (var questionnaireCMSitem in questionnaireCMSinExcel.ToList())
                {
                    using (var context = new EntitiesDBContext())
                    {
                        if (!string.IsNullOrEmpty(autoMailid))
                        {
                            // context.questionnaireQuestionnaireCMS.Attach(questionnaireCMSitem);
                            context.pr_addAutomailMessageByQuestionnaire(Convert.ToInt32(autoMailid), questionnaireCMSitem.Subject, string.IsNullOrEmpty(questionnaireCMSitem.Text) ? "" : questionnaireCMSitem.Text,
                                 questionnaireCMSitem.Footer, questionnaireCMSitem.Signature, questionnaireCMSitem.SendDateCalcFactor, null, int.Parse(questionnaireCMSitem.Type));
                            context.pr_addQuestionnaireAutomailMessageLoad(Convert.ToInt32(questionnaireCMSitem.RID), int.Parse(questionnaireCMSitem.Type), questionnaireCMSitem.Subject, questionnaireCMSitem.Text, questionnaireCMSitem.Footer, questionnaireCMSitem.Signature, questionnaireCMSitem.SendDateCalcFactor, Convert.ToInt32(autoMailid)).FirstOrDefault();
                            //int modifiedAutoMail = context.pr_modifyAutomailMessageByQuestionnaire(Convert.ToInt32(autoMailid), questionnaireCMSitem.id,
                            //    questionnaireCMSitem.subject, string.IsNullOrEmpty(questionnaireCMSitem.text) ? "" : questionnaireCMSitem.text,
                            //    questionnaireCMSitem.footer1, questionnaireCMSitem.footer2, questionnaireCMSitem.sendDateCalcFactor, questionnaireCMSitem.sendDateSet,
                            //    questionnaireCMSitem.mailType);
                        }
                    }
                }
            }

            return RedirectToAction("QuestionnaireQuestionnaireAutoMail", "Questionnaire", new { id = id });
            //return Redirect("~/Questionnaire/QuestionnaireQuestionnaireAutoMail/" + id);
            //return RedirectToAction("/QuestionnaireQuestionnaireAutoMail/" + id);

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        public virtual ActionResult UploadQDoc(int selectedQ, int cmsId, HttpPostedFileBase file)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    file.InputStream.CopyTo(stream);
                    var questObj = db.pr_getQuestionnaireQuestionnaireCMS(selectedQ, cmsId).FirstOrDefault();

                    db.pr_modifyQuestionnaireQuestionnaireCMS(selectedQ, cmsId, questObj.text, questObj.link, stream.ToArray(), MimeMapping.GetMimeMapping(file.FileName));
                }
                return Json("Document successfully uploaded");
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        public virtual ActionResult DownloadQDoc(int selectedQ, int cmsId)
        {
            var questObj = db.pr_getQuestionnaireQuestionnaireCMS(selectedQ, cmsId).FirstOrDefault();
            if (questObj != null && questObj.doc != null)
            {
                return File(questObj.doc, questObj.uploadedFileType);
            }
            return Json("No uploaded document", JsonRequestBehavior.AllowGet);
        }
    }
}
