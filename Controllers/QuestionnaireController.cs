using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Generic.ViewModel;
using LinqToExcel;
using Generic.SessionClass;
using Generic.Helpers.Questionnaire;

namespace Generic.Controllers
{
    public class QuestionnaireController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();

        //
        // GET: /Questionnaire/

        public ActionResult Index()
        {
            var questionnaire = db.pr_getQuestionnaireAll(SessionSingleton.MyEnterPriseId);
            return View(questionnaire.ToList());
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

            string sheetname = "questionnaireCMS";
            var excelRead = new ExcelQueryFactory(physicalPath.ToString());
            // ITEM TEXT LINK

            var cmsinExcel = from a in excelRead.Worksheet<ExcelQuestionnaireCMS>(sheetname) select a;

            var objQuestionnareCMS = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(questionnaireId).ToList();

            foreach (var cms in cmsinExcel)
            {
                var questionnaireCMSID = objQuestionnareCMS.Where(x => x.text == cms.ITEM).FirstOrDefault();
                if (questionnaireCMSID != null)
                {
                    using (var context = new EntitiesDBContext())
                    {
                        if (cms.ITEM == CMS.QUESTIONNAIRE_PDF)
                        {
                            if (uploadCMSFilePDF == null)
                            {
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.questionnaireCMS, cms.TEXT, cms.LINK, null);
                            }
                            else
                            {
                                byte[] uploadedFile = new byte[uploadCMSFilePDF.InputStream.Length];
                                uploadCMSFilePDF.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.questionnaireCMS, cms.TEXT, cms.LINK, uploadedFile);
                            }
                        }
                        else if (cms.ITEM == CMS.QUESTIONNAIRE_FAQ)
                        {
                            if (uploadCMSFileFAQ == null)
                            {
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.questionnaireCMS, cms.TEXT, cms.LINK, null);
                            }
                            else
                            {
                                byte[] uploadedFile = new byte[uploadCMSFileFAQ.InputStream.Length];
                                uploadCMSFileFAQ.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.questionnaireCMS, cms.TEXT, cms.LINK, uploadedFile);
                            }
                        }
                        else if (cms.ITEM == CMS.QUESTIONNAIRE_DOC_OTHER)
                        {
                            if (uploadCMSFileOther == null)
                            {
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.questionnaireCMS, cms.TEXT, cms.LINK, null);
                            }
                            else
                            {
                                byte[] uploadedFile = new byte[uploadCMSFileOther.InputStream.Length];
                                uploadCMSFileOther.InputStream.Read(uploadedFile, 0, uploadedFile.Length);
                                context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.questionnaireCMS, cms.TEXT, cms.LINK, uploadedFile);
                            }
                        }
                        else
                        {
                            context.pr_addQuestionnaireQuestionnaireCMS(questionnaireId, questionnaireCMSID.questionnaireCMS, cms.TEXT, cms.LINK, null);
                        }
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


        public ActionResult UploadQuestionnaire()
        {
            ViewBag.protocol = new SelectList(db.pr_getProtocolAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAll(), "id", "description");
            ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name");
            ViewBag.level = new SelectList(db.pr_getQuestionnaireLevelTypeByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
            return View();
        }

        [HttpPost]
        public ActionResult UploadQuestionnaire(int protocol, string protocolName
            , int touchpoint, string touchpointName
            , int partnertype, string partnertypeName, int level
            )
        {

            int EnterpriseID = Generic.Helpers.CurrentInstance.EnterpriseID;
            if (ModelState.IsValid)
            {

                try
                {

                    person objPerson = db.pr_getPersonByEmail(EnterpriseID, User.Identity.Name).FirstOrDefault();

                    string protocolTitle = protocolName;
                    string touchpointTitle = touchpointName;
                    string providerTypTitle = partnertypeName;
                    string questionnaireTitle = protocolTitle + " " + touchpointTitle + " " + providerTypTitle;


                    var fileName = TempData["fileName"];
                    var physicalPath = TempData["physicalPath"];
                    string sheetname = "surveyQuestion";
                    var excelRead = new ExcelQueryFactory(physicalPath.ToString());

                    //  excelRead.AddMapping<ExcelPartner>(x => x.internalID, "Internal ID");

                    var questionnaireinExcel = from a in excelRead.Worksheet<ExcelQuestionnaire>(sheetname) select a;

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
                    db.Entry(objQuestionnaire).State = System.Data.EntityState.Added;
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
                    db.partnerTypeTouchpointQuestionnaire.Add(objPartnertypeTouchpointQuestionnaire);
                    db.SaveChanges();

                    string previousPage = string.Empty, previousSurveySet = string.Empty, previousSurvey = string.Empty,
                        responses = string.Empty, responseType = string.Empty, jumpToQIDstr = string.Empty;

                    int pageId = 0, surveySetId = 0, surveyId = 0, responseTypeId = 0, isRequired = 0,
                        isRequiredComment = 0, jumpToQID = 0, hasSkipLogicQuestionId = 0, isSkipLogicAnwerYes = 0,
                        k = 0, responseId = 0, questionValue = 0;



                    foreach (var excelQuestionnaire in questionnaireinExcel)
                    {
                        responses = null; responseType = string.Empty;
                        if (excelQuestionnaire.Page < 1 || excelQuestionnaire.Surveyset.Length < 1 || excelQuestionnaire.Survey.Length < 1 || excelQuestionnaire.Question.Length < 1 || excelQuestionnaire.Response.Length < 1 || excelQuestionnaire.Title.Length < 1 || excelQuestionnaire.Required.Length < 1 || excelQuestionnaire.Comment.Length < 1)
                        {
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
                                db.pr_addQuestionnairePage(questionnaireId, pageId);
                                previousPage = objPage.description;

                                addSurvetSet(ref previousSurveySet, ref previousSurvey, pageId, ref surveySetId, ref surveyId, excelQuestionnaire);
                            }

                            //add a new SurveySet if the previous suveySet and current surveySet are not the same
                            if (previousSurveySet != excelQuestionnaire.Surveyset && previousPage == "Page " + excelQuestionnaire.Page.ToString())
                            {
                                addSurvetSet(ref previousSurveySet, ref previousSurvey, pageId, ref surveySetId, ref surveyId, excelQuestionnaire);
                            }

                            //add a new survey if the previous survey and current survey are not the same
                            if (previousSurvey != excelQuestionnaire.Survey && previousPage == "Page " + excelQuestionnaire.Page.ToString() && previousSurveySet == excelQuestionnaire.Surveyset)
                            {
                                addSurvey(ref previousSurvey, surveySetId, ref surveyId, excelQuestionnaire);
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
                                case "comyment":
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
                                default:
                                    if (responseType.ToLower().Substring(0, 4) == "list")
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
                            if (excelQuestionnaire.Comment.ToLower() == "y" || excelQuestionnaire.Comment.ToLower() == "yes" || excelQuestionnaire.Comment.ToLower() == "1")
                            {
                                isRequiredComment = 1;
                            }
                            else
                            {
                                isRequiredComment = 0;
                            }

                            question objQuestion = new question();

                            objQuestion.question1 = excelQuestionnaire.Question;
                            objQuestion.name = excelQuestionnaire.Question;
                            objQuestion.title = excelQuestionnaire.Title;
                            objQuestion.tag = string.Empty;
                            objQuestion.responseType = responseTypeId;
                            objQuestion.required = isRequired;

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


                            //objQuestion.skipLogicJump Pending
                            objQuestion.accessLevel = 1;
                            objQuestion.commentRequired = isRequiredComment;
                            objQuestion.commentBoxTxt = excelQuestionnaire.CommentBoxMessageText;
                            //objQuestion.commentUploadTxt

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
                                db.question.Add(objQuestion);
                                db.SaveChanges();
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
                                sArray = responses.Split(splitter);

                                for (int i = 0; i < sArray.Length; i++)
                                {
                                    //add Response
                                    try
                                    {
                                        response objResponse = new response();
                                        objResponse.active = true;
                                        objResponse.description = sArray[i];
                                        objResponse.enterprise = EnterpriseID;

                                        objResponse.sortOrder = 1;
                                        db.response.Add(objResponse);
                                        db.SaveChanges();
                                        int responsesId = objResponse.id;
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
                            //check if this question is a skipLogicJump question
                            //update skipLogicJump and skipLogicAnswer
                            //if (!string.IsNullOrEmpty(excelQuestionnaire.skipLogic) && !string.IsNullOrEmpty(excelQuestionnaire.skipLogicAnswer) && !string.IsNullOrEmpty(excelQuestionnaire.skipLogicJump))
                            //{
                            //    //try getting skipLogicJump Qid
                            //    jumpToQID = getskipLogicJumpQuestionId(questionId, excelQuestionnaire.QID, excelQuestionnaire.skipLogic, excelQuestionnaire.skipLogicAnswer, excelQuestionnaire.skipLogicJump);

                            //    if (jumpToQID > 0)
                            //    {
                            //        hasSkipLogicQuestionId = questionId;

                            //        if (excelQuestionnaire.skipLogicAnswer.ToLower() == "y" || excelQuestionnaire.skipLogicAnswer.ToLower() == "yes")
                            //        {
                            //            isSkipLogicAnwerYes = 1;
                            //        }
                            //        else
                            //        {
                            //            isSkipLogicAnwerYes = 0;
                            //        }

                            //        db.pr_modifyQuestionSkipLogicJump(questionId, isSkipLogicAnwerYes, jumpToQID);
                            //    }
                            //    else
                            //    {
                            //        hasSkipLogicQuestionId = 0;
                            //    }
                            //}
                            //else 
                            if (!string.IsNullOrEmpty(excelQuestionnaire.skipLogic) && !string.IsNullOrEmpty(excelQuestionnaire.skipLogicJump))
                            {
                                //try getting skipLogicJump Qid
                                jumpToQIDstr = getskipLogicJumpQuestionIdLogic(questionId, excelQuestionnaire.QID, excelQuestionnaire.skipLogic, excelQuestionnaire.skipLogicJump);

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
                                else if (excelQuestionnaire.Response.ToLower() == "y/n/na")
                                {
                                    k = 76;
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

                    return RedirectToAction("UploadCMS");
                }
                catch (Exception ex)
                {

                    ViewBag.Error = ex.InnerException.Message;
                    ViewBag.protocol = new SelectList(db.pr_getProtocolAll(EnterpriseID), "id", "name");
                    ViewBag.touchpoint = new SelectList(db.pr_getTouchpointAll(), "id", "description");
                    ViewBag.partnertype = new SelectList(db.pr_getPartnerTypeAll(EnterpriseID), "id", "name");
                    ViewBag.level = new SelectList(db.pr_getQuestionnaireLevelTypeByEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "description");
                    return View();
                }
                //var fileFolderName = ViewData["FileFolderName"];
            }
            return View(); // this will never executed
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


        private string getskipLogicJumpQuestionIdLogic(int questionId, int QID, string skipLogic, string skipLogicJump)
        {
            //1=Y&3=Y:15;1=Y&3=N:16;
            //QID is 3
            string retString = "";
            string[] arrySkipLogicJump = skipLogicJump.Split(';');

            for (int i = 0; i < arrySkipLogicJump.Length - 1; i++)
            {
                string[] subSrting = arrySkipLogicJump[i].Split('&');
                for (int j = 0; j < subSrting.Length; j++)
                {
                    string[] questionStr = subSrting[j].Split('=');
                    int firstQid = Convert.ToInt32(questionStr[0].ToString());
                    int diff = QID - firstQid;
                    int ActualFristQid = questionId - diff;
                    int yesnoValue = 0;
                    string[] tempStr = questionStr[1].Split(':');
                    if (tempStr.Length > 0)
                    {
                        questionStr[1] = tempStr[0];
                        yesnoValue = int.Parse(tempStr[0]);
                    }
                    if (questionStr[1].ToLower() == "y" || questionStr[1].ToLower() == "yes")
                    {
                        yesnoValue = 1;
                    }
                    if (j != subSrting.Length - 1)
                    {
                        retString += ActualFristQid + "=" + yesnoValue + "&";
                    }
                    else
                    {
                        retString += ActualFristQid + "=" + yesnoValue;
                    }
                }
                string[] columnSp = arrySkipLogicJump[i].Split(':');
                if (columnSp.Length > 0)
                {
                    int gotoQID = int.Parse(columnSp[1]);

                    int number = questionId + (gotoQID - QID);
                    retString += ":" + number + ";";
                }
            }
            return retString;
        }



        private void addSurvetSet(ref string previousSurveySet, ref string previousSurvey, int pageId, ref int surveySetId, ref int surveyId, ExcelQuestionnaire excelQuestionnaire)
        {
            surveyset objsurveyset = new surveyset();
            objsurveyset.description = excelQuestionnaire.Surveyset;
            objsurveyset.active = true;
            objsurveyset.sortOrder = 1;
            objsurveyset.accessLevel = 1;
            db.surveyset.Add(objsurveyset);

            db.SaveChanges();

            surveySetId = objsurveyset.id;
            db.pr_addPageSurveyset(pageId, surveySetId);
            db.SaveChanges();

            previousSurveySet = excelQuestionnaire.Surveyset;

            addSurvey(ref previousSurvey, surveySetId, ref surveyId, excelQuestionnaire);
        }

        private void addSurvey(ref string previousSurvey, int surveySetId, ref int surveyId, ExcelQuestionnaire excelQuestionnaire)
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
