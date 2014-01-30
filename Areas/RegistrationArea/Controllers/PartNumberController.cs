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
    public class PartNumberController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /RegistrationArea/PartNumber/

        public virtual ActionResult QuestionnaireResponse(int drpPartNumber = 0, int drpSite = 0,int drpPartNumberStatus=0, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index");
            }

            int questionnaireId = (int)Session["questionnaire"];

            questionnaire objQuestionnaire = db.pr_getQuestionnaire(questionnaireId).FirstOrDefault();

            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

            if (Session["partnumber"] != null)
            {
                int partNumberID = Int32.Parse(Session["partnumber"].ToString());
                if (Session["site"] != null && Session["site"] != "0")
                {
                    int site = Convert.ToInt32(Session["site"]);
                    fillPartnumberBySite(site, pptq, partNumberID);
                }
                else
                {
                    getPartnumber(pptq, partNumberID);
                }
                nextpartnumber();
            }
            else
            {
                getPartnumber(pptq);
                Session["partnumber"] = drpPartNumber;
            }
            if (Session["site"] != null && Session["site"] != "0")
            {

                int siteId = Int32.Parse(Session["site"].ToString());
                getSitepoint(pptq, siteId);

            }
            else
            {
                getSitepoint(pptq);
                Session["site"] = drpSite;
            }
            if (Session["partnumberstatus"] != null)
            {
                int partnumberstatusID = Convert.ToInt32(Session["partnumberstatus"]);
                //if (index == 6)
                //{
                //    getPartnumberstatus(index);
                //    getPartnumberByPartnumberStatus(index);
                //}
                //else 
                if (partnumberstatusID == Generic.Helpers.PartNumberHelper.Status.COMPLETED)
                {
                    getPartnumberstatus(partnumberstatusID);
                    getPartnumberByPartnumberStatusNotstarted(pptq);
                }
                else if (partnumberstatusID == Generic.Helpers.PartNumberHelper.Status.INCOMPLETE)
                {
                    getPartnumberstatus(partnumberstatusID);
                    getPartnumberByPartnumberStatusNotstarted(pptq);
                }
                else if (partnumberstatusID == Generic.Helpers.PartNumberHelper.Status.NOT_STARTED)
                {
                    //if (ddlSitepoint.SelectedIndex > -1)
                    //{
                        int site = drpSite;
                        if (Session["partnumber"] != null)
                        {
                            int partindex = Convert.ToInt32(Session["partnumber"]);
                            if (partindex > 0)
                            {
                                fillPartnumberBySite(site,pptq, partindex);
                            }
                        }
                        else
                        {
                            fillPartnumberBySite(site,pptq);
                        }
                        getPartnumberstatus(partnumberstatusID);
                   // }
                    // else
                    //{
                    //  getPartnumberstatus(index);
                    //}
                }
                //else
                //{
                //  getPartnumberstatus();
                //}
            }
            else
            {
                getPartnumberstatus();
                Session["partnumberstatus"] = drpPartNumberStatus;
            }





           
            
           

            int id = (int)Session["protocol"];
            
            id = (int)Session["campaign"];
            
            id = (int)Session["providerId"];
          
       
            //SaveForLater save = new SaveForLater();
            //var list = save.GetSaveForLater(provider);
            //int partid = 0;
            //int siteid = 0;
            //int partstatusid = 0;
            //int saveid = 0;
            //if (list.Count > 0)
            //{
            //    if (list[0].id != 0)
            //    {
            //        saveid = list[0].id;
            //    }
            //    if (list[0].part.id != 0)
            //    {
            //        int index = list[0].part.id;
            //        if (list[0].site.id != 0)
            //        {
            //            int site = list[0].site.id;
            //            fillPartnumberBySite(site, index);
            //        }
            //        else
            //        {
            //            getPartnumber(index);
            //        }
            //        Session["partnumber"] = index;
            //        nextpartnumber();
            //    }
            //    if (list[0].site.id != 0)
            //    {
            //        int index = list[0].site.id;
            //        Session["site"] = index;
            //        getSitepoint(index);
            //    }
            //    if (list[0].partnumberstatus.id != 0)
            //    {
            //        int index = list[0].partnumberstatus.id;
            //        Session["partnumberstatus"] = index;
            //        getPartnumberstatus(index);
            //    }
            //    if (list[0].questionindex > 0)
            //    {
            //        questionIndex = list[0].questionindex;
            //    }
            //    if (list[0].pagenumber > 0)
            //    {
            //        pageNumber = list[0].pagenumber;
            //    }
            //    else
            //    {
            //        pageNumber = 1;
            //    }
            //    if (list[0].pageid > 0)
            //    {
            //        page = list[0].pageid;
            //    }
            //    if (list[0].jumptoquestion > 0)
            //    {
            //        jumpToQuestion = list[0].jumptoquestion;
            //    }
            //    Session["saveforlater"] = saveid;
            //}
            //else
            //{
            //    if (Request.QueryString["questionIndex"] != null)
            //    {
            //        questionIndex = int.Parse(Request.QueryString["questionIndex"].ToString());
            //    }

            //    if (Request.QueryString["pageNumber"] != null)
            //    {
            //        pageNumber = int.Parse(Request.QueryString["pageNumber"].ToString());
            //    }
            //    else
            //    {
            //        pageNumber = 1;
            //    }

            //    if (Request.QueryString["page"] != null)
            //    {
            //        page = int.Parse(Request.QueryString["page"].ToString());
            //    }

            //    if (Request.QueryString["jumpToQuestion"] != null)
            //    {
            //        jumpToQuestion = int.Parse(Request.QueryString["jumpToQuestion"].ToString());
            //    }
            //}

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
           
            





            
                //User user = new User(new Id(userId)).getUserDetail();
                //Provider provider = new Provider();
                //Campaign campaign = new Campaign();
                ////Campaign campaign = user.defaultCampaign;
                //provider = new Provider(new Id(Convert.ToInt32(Session["providerId"])));
                //campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
                //int number = provider.updateSurveyProgressBar(provider, campaign, 0);
                getZcodeByProviderProtocolCampaignQuestionnaire();
            

            //this.btnBack.Attributes.Add("onClick", "javascript: history.go(-1); return false;");









           

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


        public void fillPartnumberBySite(int siteID, int pptqID, int partNumberID)
        {
            ViewBag.partNumberSelectList = new SelectList(db.pr_getPartnumberByPPTQandSite(pptqID, siteID), "id", "description", partNumberID);
        }
        public void fillPartnumberBySite(int siteID, int pptqID)
        {
            ViewBag.partNumberSelectList = new SelectList(db.pr_getPartnumberByPPTQandSite(pptqID, siteID), "id", "description");
        }
        public void getPartnumber(int pptqID, int partNumberID)
        {
            var items = new SelectList(db.pr_getPartnumberByPPTQ(pptqID), "id", "description", partNumberID).ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.partNumberSelectList = items;
        }
        public void getPartnumber(int pptqID)
        {
            var items = new SelectList(db.pr_getPartnumberByPPTQ(pptqID), "id", "description").ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.partNumberSelectList = items;
        }
        public void nextpartnumber()
        {
            int previouspartnumber = Convert.ToInt32(Session["partnumber"]);
            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

            List<partnumber> objPartNumberList;

            if (Session["site"] != null && Session["site"] != "0")
            {
                int site = Convert.ToInt32(Session["site"]);
                objPartNumberList = db.pr_getPartnumberByPPTQandSite(pptq, site).ToList();
            }
            else
            {
                objPartNumberList = db.pr_getPartnumberByPPTQ(pptq).ToList();
            }


            Session["NextPartnumber"] = null;
            int checkNext = 0;
            foreach (var item in objPartNumberList)
            {
                if (checkNext == 1)
                {
                    Session["NextPartnumber"] = item.id;
                    break;
                }
                if (item.id == previouspartnumber)
                {
                    checkNext = 1;
                }
            }
        }

        public void getSitepoint(int pptqID, int siteId)
        {
            var items = new SelectList(db.pr_getSiteByPPTQ(pptqID), "id", "description", siteId).ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.siteSelectList = items;
        }
        public void getSitepoint(int pptqID)
        {
            var items = new SelectList(db.pr_getSiteByPPTQ(pptqID), "id", "description").ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.siteSelectList = items;
        }

        public void getPartnumberstatus(int partNumberStatusId)
        {
            var items = new SelectList(db.pr_getPartnumberStatusAll(), "id", "description", partNumberStatusId).ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.siteSelectList = items;
        }
        public void getPartnumberstatus()
        {
            var items = new SelectList(db.pr_getPartnumberStatusAll(), "id", "description").ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.siteSelectList = items;
        }


        protected void getPartnumberByPartnumberStatusNotstarted(int pptq)
        {
            int site = 0;
            site = Convert.ToInt32(Session["site"]);
            ViewBag.partNumberSelectList = new SelectList(db.pr_getPartnumberByPPTQSiteAndStatus(pptq,site, Helpers.PartNumberHelper.Status.NOT_STARTED).ToList(), "id", "description");
            
        }

        public void getZcodeByProviderProtocolCampaignQuestionnaire()
        {
            //int site = Convert.ToInt32(ddlSitepoint.SelectedValue.ToString());

            //int id = (int)Session["questionnaire"];
           
            //id = (int)Session["protocol"];
           
            //id = (int)Session["campaign"];
            
            //id = (int)Session["providerId"];
           
           // //db.pr_getPartnumberSiteZcodeByPPTQ
            //partnumber part = new partnumber();

            //DataTable datatable = new DataTable();
            //datatable = part.getZcodeByProviderProtocolCampaignQuestionnaire(provider, protocol, campaign, questionnaire);
            //string labeltext = "";
            //if (datatable.Rows.Count > 0)
            //{
            //    labeltext += "<table cellpadding='2' cellspacing='0' border='1'>";
            //    labeltext += "<tr><td>Part Number</td><td>Site</td><td>Zcode</td></tr>";
            //    foreach (DataRow dr in datatable.Rows)
            //    {
            //        labeltext += "<tr>";
            //        labeltext += "<td>" + (dr["partname"].ToString() + "</td><td>" + dr["sitename"].ToString() + "</td><td>" + dr["zcode"].ToString()) + "</td>";
            //        labeltext += "</tr>";
            //    }
            //    labeltext += "</table>";
            //}

            //lblzcode.Visible = true;
            //lblzcode.Text = labeltext;


        }

       
    }
}
