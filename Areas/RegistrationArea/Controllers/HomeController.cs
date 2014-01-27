using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Generic.DataLayer;
using Generic.Models;
namespace Generic.Areas.RegistrationArea.Controllers
{
    public class HomeController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /RegistrationArea/Home/

        public virtual ActionResult Index(string id = "", string accessCode = null)
        {
            ViewBag.accesscode = accessCode;
            return View();
        }
        [HttpPost]
        public virtual ActionResult Index(string accessCode)
        {
            var ppptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(accessCode).FirstOrDefault();
            if (ppptq.accesscode != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var touchpoint = db.pr_getTouchpoint(ptq.touchpoint).FirstOrDefault();
                if (touchpoint.endDate >= DateTime.Now)
                {
                    Session["accessCode"] = accessCode;
                    Session["hs3Registration"] = 1;
                    Session["languageused"] = "en";
                    Session["accessAttemp"] = 0;
                    Session["partner"] = ppptq.partner;
                    Session["touchpoint"] = touchpoint.id;
                    Session["partnerType"] = ptq.partnerType;
                    Session["questionnaire"] = ptq.questionnaire;
                    Session["protocol"] = touchpoint.protocol;

                    return RedirectToAction("companyInformation");
                }
            }

            return View();
        }


        public virtual ActionResult CompanyInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            ViewBag.country = db.pr_getCountry(objPartner.country).FirstOrDefault().name;
            try
            {
                ViewBag.state = db.pr_getState(objPartner.state).FirstOrDefault().stateCode;
            }
            catch { }
            return View(objPartner);
        }

        public virtual ActionResult ContactInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner objPartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            ViewBag.country = db.pr_getCountry(objPartner.country).FirstOrDefault().name;
            return View(objPartner);
        }
        public virtual ActionResult CorrectCompanyInformation()
        {
            return RedirectToAction("ContactInformation");
        }
        public virtual ActionResult CorrectContactInformation()
        {
            return RedirectToAction("QuestionnaireResponse");
        }


        public virtual ActionResult QuestionnaireResponse(int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            int questionnaireId = (int)Session["questionnaire"];

            questionnaire objQuestionnaire = db.pr_getQuestionnaire(questionnaireId).FirstOrDefault();

            touchpoint objtouchpoint = new touchpoint();
            partner objpartner = new partner();
            protocol objprotocol = new protocol();

            surveyForm objSurveyForm = new surveyForm(objprotocol, objtouchpoint, objpartner, objQuestionnaire);
            objSurveyForm.questionIndex = questionIndex;
            objSurveyForm.questionClass = "brownbg  brownbgarrow";
            objSurveyForm.answerClass = "brownbg";
            objSurveyForm.alternativeAnswerClass = "bluebg";
            objSurveyForm.alternativequestionClass = "bluebg bluebgarrow";

            objSurveyForm.errorquestion = errorQuestion;
            objSurveyForm.errorMessage = errorMessage;
            Table table = objSurveyForm.tGetsurveyForm(objQuestionnaire, pageNumber, page, jumpToQuestion);
            // Table table = objSurveyForm.tGetSurveyForm(objQuestionnaire, pageNumber, page, jumpToQuestion);
            // panel.Controls.Add(table);

            StringWriter objhtml = new StringWriter();
            using (var htmlWriter = new HtmlTextWriter(objhtml))
            {
                table.RenderControl(htmlWriter);
            }

            ViewBag.questions = objhtml.ToString();

            return View();
        }




        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult QuestionnaireResponse(FormCollection formCollection, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            //[2] = "question_454_1171_fileUploadComment"
            //

            int questionnaireId = 0;
            int partnerId = 0;
            int touchpointId = 0;
            int protocolId = 0;
            // int jumpToQuestion = 0;
            //  int questionIndex = 0;

            questionnaireId = (int)Session["questionnaire"];
            partnerId = (int)Session["partner"];
            touchpointId = (int)Session["touchpoint"];
            protocolId = (int)Session["protocol"];

            int count = 0;
            int questionId = 0;
            int surveyId = 0;
            string key = "";
            string[] array = new string[5];
            char[] splitter = { '_' };
            string answer = "";
            question objQuestion = new question();
            Boolean saveForLaterButton = false;
            string skip = "";
            string noSkip = "";
            // int errorQuestion = 0;
            //  string errorMessage = "";
            string goEsignature = "";

            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;


            jumpToQuestion = 0;


            foreach (var keyName in formCollection.Keys)
            {
                answer = formCollection[keyName.ToString()];
                if (!keyName.ToString().Contains("uploadText") && keyName.ToString().Contains("question_"))
                {
                    ++questionIndex;
                }

                if (keyName.ToString().Contains("questionHiddenField_"))
                {

                    array = keyName.ToString().Split(splitter);

                    questionId = int.Parse(array[1]);
                    surveyId = int.Parse(array[2]);


                    //question = new Question(new Id(questionId));
                    //response = new Response(int.Parse(answer));
                    //survey = new Survey(new Id(surveyId));
                    //question.response = response;
                    //survey.question = question;

                    ////to be confirmed
                    //provider.removeProviderProtocolCampaignQuestionnaireSurveyQuestion(protocol, campaign,
                    //    questionnaire, survey, question);
                }
                //if (Request.Form.Keys[i].Contains("_languageBtn"))
                //{
                //    string str = Request.Form.Keys[i].ToString();
                //    int indexdol = str.IndexOf('$');
                //    int indexunder = str.IndexOf('_');
                //    indexunder = indexunder - indexdol;
                //    indexdol += 1;
                //    indexunder = indexunder - 1;
                //    string langName = str.Substring(indexdol, indexunder);
                //    Session["languageused"] = langName;
                //    Response.Redirect(Request.Url.ToString());
                //}




                if (keyName.ToString().Contains("btnSaveForLater"))
                {
                    saveForLaterButton = true;
                }

                if (keyName.ToString().Contains("question_"))
                {
                    //    ++questionIndex;

                    if (keyName.ToString().Contains("_text"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);

                        int? responseId = null;
                        string responseComment = string.Empty;
                        try
                        {
                            responseId = int.Parse(answer);
                        }
                        catch
                        {
                            responseComment = answer;
                        }


                        db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);
                    }
                    else if (keyName.ToString().Contains("_checkBox"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                        answer = array[3];




                        if (formCollection[keyName.ToString()].ToLower() == "on")
                        {
                            int? responseId = null;
                            string responseComment = string.Empty;
                            try
                            {
                                responseId = int.Parse(answer);
                            }
                            catch
                            {
                                responseComment = answer;
                            }


                            db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);

                        }
                    }
                    else if (keyName.ToString().Contains("_Commenttext"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);

                        //provider.addProviderProtocolCampaignQuestionnaireSurveyQuestionResponse(
                        //    protocol, campaign, questionnaire, survey, question, response);
                    }
                    else if (keyName.ToString().Contains("_onlyTextComment"))
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);

                        //provider.addProviderProtocolCampaignQuestionnaireSurveyQuestionResponse(
                        //protocol, campaign, questionnaire, survey, question, response);
                    }
                    else
                    {
                        array = keyName.ToString().Split(splitter);
                        questionId = int.Parse(array[1]);
                        surveyId = int.Parse(array[2]);
                        int? responseId = null;
                        string responseComment = string.Empty;
                        try { responseId = int.Parse(answer); }
                        catch { }
                        if (answer == "74")
                        {

                            string strvl = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_Commenttext"];
                            if (strvl != null)
                            {
                                responseComment = strvl;
                                //response.description = surveyfrm.convertLanguageToEnglish(strvl);
                            }

                        }
                        else if (answer == "75")
                        {


                            string strvl = formCollection["question_" + questionId.ToString() + "_" + surveyId.ToString() + "_onlyTextComment"];
                            if (strvl != null)
                            {
                                responseComment = strvl;
                            }
                        }
                        else
                        {
                            responseComment = null;
                        }

                        db.pr_addPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(questionId, responseId, responseComment, null, null, null, null, pptq);

                    }

                    objQuestion = db.pr_getQuestion(questionId).FirstOrDefault();





                    if (answer == "74" || answer == "75" || answer == "76" || answer != "")
                    {
                        if (objQuestion.skipLogicJump != null)
                        {
                            if (objQuestion.skipLogicAnswer == true)
                            {
                                if (answer == "74")
                                {


                                    if (objQuestion.skipLogicJump.Contains("&"))
                                    {
                                    }
                                    else
                                    {
                                        jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                    }
                                }
                                else if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer == true) && answer != "" && answer != "75")

                                    jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                else
                                {
                                    jumpToQuestion = 0;
                                }
                            }
                            else
                            {
                                if (answer == "74" && (objQuestion.commentType == 5 || objQuestion.commentType == 3))
                                {
                                    if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer == true))

                                        jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                    else
                                        jumpToQuestion = 0;

                                }
                                else if (objQuestion.skipLogicJump.Contains("&"))
                                {
                                    string[] strQuestionLogic = objQuestion.skipLogicJump.Split(';');
                                    for (int k = 0; k < strQuestionLogic.Length - 1; k++)
                                    {
                                        string[] subStrQuestionlogic = strQuestionLogic[k].Split('&');
                                        Boolean logicOneStatus = false;
                                        Boolean logicTwoStatus = false;
                                        int gotoQuestionId = 0;
                                        for (int j = 0; j < subStrQuestionlogic.Length; j++)
                                        {
                                            string[] strquestionid = subStrQuestionlogic[j].Split('=');
                                            int questionidLogic = Convert.ToInt32(strquestionid[0]);
                                            string[] strNewQuestionAns = strquestionid[1].Split(':');
                                            int ansLogicStatus = 0;
                                            if (strNewQuestionAns.Length > 0)
                                            {
                                                ansLogicStatus = Convert.ToInt32(strNewQuestionAns[0]);
                                            }
                                            if (strNewQuestionAns.Length > 1)
                                            {
                                                gotoQuestionId = Convert.ToInt32(strNewQuestionAns[1]);
                                            }
                                            string answerStatus = "";
                                            if (ansLogicStatus == 1)
                                            {
                                                answerStatus = "74";
                                            }
                                            else
                                            {
                                                answerStatus = "1";
                                            }
                                            Boolean foundFlage = false;

                                            for (int l = 0; l < count; ++l)
                                            {
                                                key = formCollection.Keys[l];
                                                if (key.Contains("question_"))
                                                {
                                                    array = keyName.ToString().Split(splitter);
                                                    questionId = int.Parse(array[1]);
                                                    surveyId = int.Parse(array[2]);
                                                    answer = formCollection[l];

                                                    if (questionId == gotoQuestionId)
                                                    {
                                                            Response.Redirect("eSignature");
                                                    }
                                                    if (questionId == questionidLogic)
                                                    {
                                                        if (answer == "74" || answer == "75" || answer == "76")
                                                        {
                                                            foundFlage = true;
                                                            if (answer == answerStatus)
                                                            {
                                                                if (j == 0)
                                                                {
                                                                    logicOneStatus = true;
                                                                }
                                                                else if (j == 1)
                                                                {
                                                                    logicTwoStatus = true;
                                                                }
                                                            }
                                                            else if (answerStatus == "1")
                                                            {
                                                                if (answer == "75" || answer == "76")
                                                                {
                                                                    if (j == 0)
                                                                    {
                                                                        logicOneStatus = true;
                                                                    }
                                                                    else if (j == 1)
                                                                    {
                                                                        logicTwoStatus = true;
                                                                    }
                                                                }
                                                            }
                                                            break;
                                                        }

                                                    }
                                                }
                                            }

                                            if (!foundFlage)
                                            {
                                                question questionnew = db.pr_getQuestion(questionidLogic).FirstOrDefault();
                                                response responsenew = db.pr_getResponseByQuestion(questionidLogic).FirstOrDefault();

                                                if (responsenew.description.ToLower() == "yes" || responsenew.description.ToLower() == "n/a" || responsenew.description.ToLower() == "no")
                                                {
                                                    foundFlage = true;
                                                    string tempstr = "";
                                                    if (answerStatus == "74")
                                                    {
                                                        tempstr = "yes";
                                                    }
                                                    else if (answerStatus == "75")
                                                    {
                                                        tempstr = "no";
                                                    }
                                                    else if (answerStatus == "76")
                                                    {
                                                        tempstr = "n/a";
                                                    }
                                                    if (responsenew.description.ToLower() == tempstr)
                                                    {
                                                        if (j == 0)
                                                        {
                                                            logicOneStatus = true;
                                                        }
                                                        else if (j == 1)
                                                        {
                                                            logicTwoStatus = true;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (answerStatus == "1")
                                                        {
                                                            if (responsenew.description.ToLower() == "n/a" || responsenew.description.ToLower() == "no")
                                                            {
                                                                if (j == 0)
                                                                {
                                                                    logicOneStatus = true;
                                                                }
                                                                else if (j == 1)
                                                                {
                                                                    logicTwoStatus = true;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            //int ansLogicStatus =Convert.ToInt32(
                                        }
                                        if (logicOneStatus == true && logicTwoStatus == true)
                                        {
                                            objQuestion.skipLogicJump = gotoQuestionId.ToString();
                                            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                            break;
                                        }
                                    }
                                }
                                else if (answer != "74")
                                {


                                    if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer == true))
                                    {
                                        if (answer != "75")
                                            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                        else
                                            jumpToQuestion = 0;
                                    }
                                    else if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump != null && objQuestion.skipLogicAnswer == false))
                                    {
                                        if (answer == "75")
                                            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                        else
                                            jumpToQuestion = 0;
                                    }
                                    else if (objQuestion.commentType == 5 && (objQuestion.skipLogicJump == null))
                                    {
                                        jumpToQuestion = 0;
                                    }

                                    else
                                    {
                                        jumpToQuestion = int.Parse(objQuestion.skipLogicJump);

                                        if (Request.QueryString["skip"] != null)
                                        {

                                            string strJumToQuestion = db.pr_getQuestion(objQuestion.id + 1).FirstOrDefault().skipLogicJump;
                                            if (strJumToQuestion.Contains("&"))
                                            {
                                            }
                                            else
                                            {
                                                jumpToQuestion = int.Parse(strJumToQuestion);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    jumpToQuestion = 0;
                                }
                            }
                        }
                    }
                }
            }

            // save uploaded files
            jumpToQuestion = saveUploadedFile(protocolId, touchpointId, partnerId, questionnaireId, pptq);


            //for (int i = 0; i < Request.Form.Keys.Count; i++)
            //{


            //    if (Request.Form.Keys[i].Contains("_languageBtn"))
            //    {

            //        string str = Request.Form.Keys[i].ToString();
            //        int indexdol = str.IndexOf('$');
            //        int indexunder = str.IndexOf('_');

            //        indexunder = indexunder - indexdol;
            //        indexdol += 1;
            //        indexunder = indexunder - 1;
            //        string langName = str.Substring(indexdol, indexunder);
            //        Session["languageused"] = langName;
            //        Response.Redirect(Request.Url.ToString());
            //    }
            //}


            //foreach (var keyName in formCollection.Keys)
            //{
            //    var value = formCollection[keyName.ToString()];
            //}


            if (goEsignature == "true")
            {
                //if (menuGroup.id.id == 2)
                //{
                //    Response.Redirect("spendCategoryForm.aspx");
                //}
                //else
                //{
                //    //int providerId = 0;
                //    int quetionnaireId = 0;
                //    partnerId = (int)Session["partnerId"];
                //    quetionnaireId = (int)Session["questionnaire"];

                Response.Redirect("eSignature.aspx");
                // }
            }

            if (saveForLaterButton == true)
            {


                //#region 20130222 new code
                //SaveLater(questionnaire, question);
                //#endregion

                //saveForLater();
            }

            else
            {

                goToNextPage(surveyId, jumpToQuestion, questionIndex, objQuestion, skip, errorQuestion, errorMessage);
            }


            return View();
        }


        private void goToNextPage(int surveyId, int jumpToQuestion, int questionIndex, question question, string skip, int errorQuestion, string errorMessage)
        {
            int pageId = 0;
            int pageNumber = 0;
            int pgno = 0;
            int questionnaireId = (int)Session["questionnaire"];
            string errorQueryString = "";

            if (errorQuestion > 0)
            {
                errorQueryString = "&errorQuestion=" + errorQuestion.ToString() + "&errorMessage=" + errorMessage;
            }
            surveyForm surveyForm = new surveyForm();
            int partnerId = (int)Session["partner"];
            partner provider1 = new partner();
            questionnaire questionnaire = new questionnaire();
            string zode = surveyForm.generateZCode(provider1, questionnaire);
            page page = null;
            // menuGroup = (MenuGroup)Session["menuGroup"];

            if (Request.QueryString["pageNumber"] != null)
            {
                pageNumber = int.Parse(Request.QueryString["pageNumber"].ToString());
                pageNumber = pageNumber + 1;

            }
            else
            {
                pageNumber = 2;
            }

            if (Request.QueryString["page"] != null)
            {
                pageId = int.Parse(Request.QueryString["page"].ToString());

                page = db.pr_getNextPageByQuestionnaire(questionnaireId, pageId, jumpToQuestion).FirstOrDefault();
            }
            else
            {
                page = db.pr_getNextPageByQuestionnaire(questionnaireId, 0, 0).FirstOrDefault();
                page = db.pr_getNextPageByQuestionnaire(questionnaireId, page.id, jumpToQuestion).FirstOrDefault();
            }
            // update progressbare logic

            int providerId = 0;
            int quetionnaireId = 0;
            //   providerId = (int)Session["provider"];
            quetionnaireId = (int)Session["questionnaire"];
            //    Provider provider = new Provider(new Id(providerId));
            //  Campaign campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
            //int totalpage = questionnaire.getPageQuestionnaireCount(questionnaire);
            //pgno = pageNumber - 1;
            //for (int i = 0; i < totalpage; i++)
            //{
            //    if (pgno == i)
            //    {
            //        int totalperage = 0;
            //        int totper = 80 / totalpage;
            //        if (pgno == 0)
            //        {
            //            pgno = 1;
            //            totalperage = (totper * 1) + 10;
            //        }
            //        else
            //        {
            //            totalperage = (totper * i) + 10;
            //        }
            //        provider.updateSurveyProgressBar(provider, campaign, totalperage);
            //        break;
            //    }
            //}
            //
            if (Request.QueryString["questionIndex"] != null)
            {
                questionIndex += int.Parse(Request.QueryString["questionIndex"].ToString());
            }

            if (Request.QueryString["skip"] != null)
            {
                skip = "&skip=true";
            }

            if (jumpToQuestion == 0)
            {
                if (page != null)
                {
                    if (string.IsNullOrEmpty(errorQueryString))
                    {
                        Response.Redirect("QuestionnaireResponse?pageNumber=" + pageNumber.ToString() +
                                "&page=" + page.id.ToString() + "&jumpToQuestion=" + jumpToQuestion.ToString()
                                + "&questionIndex=" + questionIndex.ToString() + skip);
                    }
                    else if (Request.QueryString["errorQuestion"] == null)
                    {
                        Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString() + errorQueryString);
                    }
                    else
                    {
                        Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString());
                    }
                }
                else
                {
                    //if (menuGroup.id.id == 2)
                    //{
                    //    Response.Redirect("spendCategoryForm.aspx");
                    //}
                    //else
                    //{
                    //int providerId = 0;
                    //int quetionnaireId = 0;
                    //providerId = (int)Session["providerId"];
                    //quetionnaireId = (int)Session["questionnaire"];
                    //Provider provider = new Provider(new Id(providerId));
                    //Campaign campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
                    //provider.updateSurveyProgressBar(provider, campaign, 80);
                    Response.Redirect("eSignature.aspx");
                    // }
                }
            }
            else
            {
                if (page != null)
                {
                    if (string.IsNullOrEmpty(errorQueryString))
                    {
                        Response.Redirect("responseQuestionnaire.aspx?pageNumber=" + pageNumber.ToString() +
                           "&page=" + page.id.ToString() + "&jumpToQuestion=" + jumpToQuestion.ToString()
                           + "&questionIndex=" + questionIndex.ToString() + skip);
                    }
                    else if (Request.QueryString["errorQuestion"] == null)
                    {
                        Response.Redirect("responseQuestionnaire.aspx?" + Request.QueryString.ToString() + errorQueryString);
                    }
                    else
                    {
                        Response.Redirect("responseQuestionnaire.aspx?" + Request.QueryString.ToString());
                    }
                }
                else
                {
                    //if (menuGroup.id.id == 2)
                    //{
                    //    Response.Redirect("spendCategoryForm.aspx");
                    //}
                    //else
                    //{

                    Response.Redirect("eSignature.aspx");
                    //}
                }
            }
        }

        private int saveUploadedFile(int protocolId, int touchpointId, int partnerId, int questionnaireId, int pptq)
        {
            Random random = new Random();
            int number = random.Next(1000000, 9999999);
            string fileName = "";
            int questionId = 0;
            int surveyId = 0;
            string key = "";
            string[] array = new string[4];
            char[] splitter = { '_' };
            question question = null;
            survey survey = null;
            int jumpToQuestion = 0;
            string errorMessage = "";

            for (int i = 0; i < Request.Files.Keys.Count; i++)
            {

                key = Request.Files.Keys[i];

                //get question and survey id
                array = key.Split(splitter);
                questionId = int.Parse(array[1]);
                surveyId = int.Parse(array[2]);

                //question = new question(new Id(questionId)).getQuestionDetail();
                //survey = new survey(new Id(surveyId));
                //provider = provider.getProviderById();

                //get jump to question
                //if (question.skipLogicAnswer == true)
                //{
                //    if (question.skipLogicJump.Contains("&"))
                //    {
                //    }
                //    else
                //    {
                //        jumpToQuestion = int.Parse(question.skipLogicJump);
                //    }
                //}

                if (Request.Files[i].FileName.Length > 0)
                {
                    string Extension = Request.Files[i].FileName.Substring(Request.Files[i].FileName.LastIndexOf('.') + 1).ToLower();
                    //    if (Request.Files[i].FileName.Substring()
                    //    {


                    if (Request.Files[i].ContentLength <= 4194304)
                    {
                        //if the directory already exists
                        //create a new directory name
                        //string dirname = "/Hs3/Honeywell/CertsAndReps/2013/UploadFiles";
                        //if (!Directory.Exists(Server.MapPath(dirname)))
                        //{
                        //    Directory.CreateDirectory(Server.MapPath(dirname));
                        //}
                        //if (Directory.Exists(Server.MapPath("uploadedFile/" + number.ToString())))
                        //{
                        //    number = random.Next(1000000, 9999999);
                        //}

                        //create the new directory and save the file in it
                        //Directory.CreateDirectory(Server.MapPath("uploadedFile/" + number.ToString()));
                        //if (question.id.id == 5132)
                        //if (question.question == "SAM")
                        //    fileName = dirname + "/" + provider.accessCode.accessCode + "_SAM." + Extension;
                        //// else if (question.id.id == 5171)
                        //else if (question.question == "HUB")

                        //    fileName = dirname + "/" + provider.accessCode.accessCode + "_Hubzone." + Extension;
                        /*if (File.Exists(fileName))
                                //	File.Delete(fileName );
                        //FileStream file = new FileStream(Server.MapPath(fileName) , FileMode.Create, System.IO.FileAccess.Write);

                                //Request.Files[i].SaveAs(Server.MapPath(fileName));

                        file.Close(); //for File saving update Nitoo 1Nov*/
                        //save the file path in the database
                        //provider.addProviderProtocolCampaignQuestionnaireSurveyQuestionUploadedFile(
                        //    protocol, campaign, questionnaire, survey, question, fileName);


                        var pptqq = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault();



                        byte[] uploadedFile = new byte[Request.Files[i].InputStream.Length];
                        Request.Files[i].InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                        // Binary linqBinary = new Binary(uploadedFile);

                        db.pr_modifyPartnerPartnertypeTouchpointQuestionnaireQuestionResponse(pptqq.id, questionId, pptqq.response, pptqq.comment, uploadedFile, Request.Files[i].ContentType, pptqq.value, pptqq.score, pptq);

                        //int length = uploadFile.ContentLength;
                        //byte[] tempImage = new byte[length];
                        //myDBObject.ContentType = uploadFile.ContentType;

                        //uploadFile.InputStream.Read(tempImage, 0, length);
                        //myDBObject.ActualImage = tempImage;

                        ////create byte array of size equal to file input stream
                        //byte[] fileData = new byte[Request.Files[upload].InputStream.Length];
                        ////add file input stream into byte array
                        //Request.Files[upload].InputStream.Read(fileData, 0, Convert.ToInt32(Request.Files[upload].InputStream.Length));
                        ////create system.data.linq object using byte array
                        //System.Data.Linq.Binary binaryFile = new System.Data.Linq.Binary(fileData);
                        ////initialise object of FileDump LINQ to sql class passing values to be inserted
                        //FileDump record = new FileDump { FileData = binaryFile, FileName = System.IO.Path.GetFileName(Request.Files[upload].FileName) };
                        ////call InsertOnsubmit method to pass new object to entity
                        //dataContext.FileDumps.InsertOnSubmit(record);
                        ////call submitChanges method to execute implement changes into database
                        //dataContext.SubmitChanges();

                    }
                    else
                    {


                    }

                }

                //if (question.title == "Upload Certification letter if applicable.")
                //{
                //    jumpToQuestion = 1;
                //}
            }


            return jumpToQuestion;
        }

        public FileContentResult FileDownload()
        {
            //declare byte array to get file content from database and string to store file name
            byte[] fileData;
            string fileName;

            //using LINQ expression to get record from database for given id value
            var record = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(454, 3).FirstOrDefault();
            //var record = from p in dataContext.FileDumps
            //             where p.ID == id
            //             select p;
            //only one record will be returned from database as expression uses condtion on primary field
            //so get first record from returned values and retrive file content (binary) and filename 
            fileData = (byte[])record.uploadedFile.ToArray();
            fileName = "abc";
            //return file and provide byte file content and file name --application/pdf
            return File(fileData, "application/pdf", fileName);
        }


        public ActionResult EditCompanyInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner partner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            if (partner == null)
            {
                return HttpNotFound();
            }

            ViewBag.state = new SelectList(db.pr_getStateAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList().AsEnumerable(), "id", "stateCode", partner.state);
            ViewBag.country = new SelectList(db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID), "id", "name", partner.country);
            //ViewBag.id = new SelectList(db.partnerRemitAddress, "partner", "remitAddress1", partner.id);

            ComboBoxModel objCombobox = new ComboBoxModel();

            objCombobox.ComboBoxAttributes.SelectedIndex = partner.state;
            ViewBag.combobox = objCombobox;
            IEnumerable<state> states = new List<state>();
            states = db.pr_getStateAll(Generic.Helpers.CurrentInstance.EnterpriseID).ToList();
            ViewBag.states = states;



            ComboBoxModel objComboboxCountry = new ComboBoxModel();
            objComboboxCountry.ComboBoxAttributes.SelectedIndex = partner.country;
            ViewBag.comboboxCountry = objComboboxCountry;
            ViewBag.countries = db.pr_getCountryAll(Generic.Helpers.CurrentInstance.EnterpriseID);

            return View(partner);
        }
        [HttpPost]
        public ActionResult EditCompanyInformation(partner partner)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner objpartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            objpartner.name = partner.name;
            objpartner.address1 = partner.address1;
            objpartner.address2 = partner.address2;
            objpartner.city = partner.city;
            objpartner.state = partner.state;
            objpartner.zipcode = partner.zipcode;
            objpartner.province = partner.province;
            objpartner.country = partner.country;
            if (ModelState.IsValid)
            {
                db.Entry(objpartner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CompanyInformation");
            }

            return View(partner);
        }


        public ActionResult EditContactInformation()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner partner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            if (partner == null)
            {
                return HttpNotFound();
            }

            return View(partner);
        }

        [HttpPost]
        public ActionResult EditContactInformation(partner partner)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            partner objpartner = db.pr_getPartner((int)Session["partner"]).FirstOrDefault();
            objpartner.firstName = partner.firstName;
            objpartner.lastName = partner.lastName;
            objpartner.title = partner.title;
            objpartner.email = partner.email;
            objpartner.phone = partner.phone;
            objpartner.fax = partner.fax;

            if (ModelState.IsValid)
            {
                db.Entry(objpartner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("ContactInformation");
            }

            return View(partner);
        }

        public ActionResult ESignature()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            eSignature objeSignature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id).FirstOrDefault();
            return View(objeSignature);
        }

        [HttpPost]
        public ActionResult ESignature(eSignature objeSignatureNew)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            partnerPartnertypeTouchpointQuestionnaire pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            eSignature objeSignature = db.pr_getEsignatureByPartnerPartnerTypeTouchpointQuestionnaire(pptq.id).FirstOrDefault();
            if (objeSignature == null)
            {

                objeSignatureNew.affirmation = "Yes";
                objeSignatureNew.partnerPartnerTypeTouchpointQuestionnaire = pptq.id;
                db.Entry(objeSignatureNew).State = EntityState.Added;
                db.SaveChanges();
            }
            else
            {
                objeSignature.firstName = objeSignatureNew.firstName;
                objeSignature.lastName = objeSignatureNew.lastName;
                objeSignature.email = objeSignatureNew.email;
                objeSignature.affirmation = "Yes";
                db.Entry(objeSignature).State = EntityState.Modified;
                db.SaveChanges();
            }

            pptq.completedDate = DateTime.Now;
            db.Entry(pptq).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Finish");
        }

        public ActionResult Finish()
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }
            return View();
        }


        /// <summary> 
        ///This is the method for trialling overrides 
        /// </summary> 
        /// <returns></returns> 
        public ActionResult Submit()
        {
            string response = BreakHereIfYoureInTheArea();
            return RedirectToAction("Index", "Home", new { Area = "RegistrationArea" });
        }


        /// <summary> 
        ///Note - this is virtual, as we are going to override it in the client
        ///project. 
        /// </summary> 
        /// <returns></returns> 
        public virtual string BreakHereIfYoureInTheArea()
        {
            string test = "break here - i am generic.";
            return test;
        }
    }
}
