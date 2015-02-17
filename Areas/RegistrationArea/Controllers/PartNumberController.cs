using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Linq;
using System.IO;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using Generic.DataLayer;
using Generic.Helpers.PartnerHelper;
using Generic.Helpers.PartNumberHelper;
using Generic.Helpers.Questionnaire;
using Generic.Models;

namespace Generic.Areas.RegistrationArea.Controllers
{
    public class PartNumberController : Controller
    {
        private EntitiesDBContext db = new EntitiesDBContext();
        //
        // GET: /RegistrationArea/PartNumber/
        
        public virtual ActionResult QuestionnaireResponse(int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 1, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            #region CMS Related
            //JB: this is content management
            ViewBag.CMS_PAGE_TITLE = CMS.QUESTIONNAIRE_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.QUESTIONNAIRE_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.QUESTIONNAIRE_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.QUESTIONNAIRE_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT;
            ViewBag.SAVE_FOR_LATER_TEXT = CMS.SAVE_FOR_LATER_TEXT;

            ViewBag.QUESTIONNAIRE_PAGE_PN = CMS.QUESTIONNAIRE_PAGE_PN;
            ViewBag.QUESTIONNAIRE_PAGE_SITE = CMS.QUESTIONNAIRE_PAGE_SITE;




            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Length >= 20 ? CMS.QUESTIONNAIRE_PDF.Substring(0, 20) : CMS.QUESTIONNAIRE_PDF;
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Length >= 20 ? CMS.QUESTIONNAIRE_FAQ.Substring(0, 20) : CMS.QUESTIONNAIRE_FAQ;
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Length >= 20 ? CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 20) : CMS.QUESTIONNAIRE_DOC_OTHER;
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Length >= 20 ? CMS.QUESTIONNAIRE_VIDEO.Substring(0, 20) : CMS.QUESTIONNAIRE_VIDEO;
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Length >= 20 ? CMS.CONTACT_US_EMAIL.Substring(0, 20) : CMS.CONTACT_US_EMAIL;


            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    var cms_PageTitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_TITLE).id);
                    if (cms_PageTitle != null)
                        ViewBag.CMS_PAGE_TITLE = cms_PageTitle.text;
                    var cms_PageSubtitle = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_SUBTITLE).id);
                    if (cms_PageSubtitle != null)
                        ViewBag.CMS_PAGE_SUBTITLE = cms_PageSubtitle.text;
                    var cms_PagePanelOne = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_ONE).id);
                    if (cms_PagePanelOne != null)
                        ViewBag.CMS_PAGE_PANEL_ONE = cms_PagePanelOne.text;
                    var cms_PagePanelTwo = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_TWO).id);
                    if (cms_PagePanelTwo != null)
                        ViewBag.CMS_PAGE_PANEL_TWO = cms_PagePanelTwo.text;
                    var cms_PagePreviousText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT).id);
                    if (cms_PagePreviousText != null)
                        ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms_PagePreviousText.text;
                    var cms_PageNextText = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT).id);
                    if (cms_PageNextText != null)
                        ViewBag.CMS_PAGE_NEXT_TEXT = cms_PageNextText.text;

                    var cms_SaveForLater = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.SAVE_FOR_LATER_TEXT).id);
                    if (cms_SaveForLater != null)
                        ViewBag.SAVE_FOR_LATER_TEXT = cms_SaveForLater.text;

                    var cms_quiestionnare = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PDF).id);
                    if (cms_quiestionnare != null)
                        ViewBag.QUESTIONNAIRE_PDF = cms_quiestionnare.text;

                    var cms_QuestionnareFAQ = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_FAQ).id);
                    if (cms_QuestionnareFAQ != null)
                        ViewBag.QUESTIONNAIRE_FAQ = cms_QuestionnareFAQ.text;
                    var cms_questionnare_doc = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).id);
                    if (cms_questionnare_doc != null)
                        ViewBag.QUESTIONNAIRE_DOC_OTHER = cms_questionnare_doc.text;
                    var cms_Questionnare_video = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_VIDEO).id);
                    if (cms_Questionnare_video != null)
                        ViewBag.QUESTIONNAIRE_VIDEO = cms_Questionnare_video.text;
                    var cms_ContactEmail = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL).id);
                    if (cms_ContactEmail != null)
                        ViewBag.CONTACT_US_EMAIL = cms_ContactEmail.text;
                    var cms_ContactEmailLink = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.CONTACT_US_EMAIL).id);
                    if (cms_ContactEmailLink != null)
                        ViewBag.CONTACT_US_EMAIL_LINK = cms_ContactEmailLink.link;

                    //
                    var cms_QUESTIONNAIRE_PAGE_PN = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_PN).id);
                    if (cms_QUESTIONNAIRE_PAGE_PN != null)
                        ViewBag.QUESTIONNAIRE_PAGE_PN = cms_QUESTIONNAIRE_PAGE_PN.text;

                    //
                    var cms_QUESTIONNAIRE_PAGE_SITE = cms.FirstOrDefault(x => x.questionnaireCMS == questionnairCMSAll.FirstOrDefault(q => q.description == CMS.QUESTIONNAIRE_PAGE_SITE).id);
                    if (cms_QUESTIONNAIRE_PAGE_SITE != null)
                        ViewBag.QUESTIONNAIRE_PAGE_SITE = cms_QUESTIONNAIRE_PAGE_SITE.text;



                }
                catch { }
            }
            //JB: this ends content management
            #endregion

            //JB: this begins process of setting dropdown bindings based on data in session
            //temp comment out
            if (partNumberSelectList == 0 && siteSelectList == 0)
            {
                var nextPartNumber = db.pr_getPartnumberSiteZcodePPTQByPPTQ_ToDo_ByPPTQ(ppptq_cms.id).FirstOrDefault();

                if (nextPartNumber != null)
                {
                    partNumberSelectList = nextPartNumber.partnumber;
                    siteSelectList = nextPartNumber.site;
                }

            }
            Session["partnumber"] = partNumberSelectList;
            Session["site"] = siteSelectList;
            Session["partnumberstatus"] = partnumberStatusSelectList;
            int questionnaireId = (int)Session["questionnaire"];
            questionnaire objQuestionnaire = db.pr_getQuestionnaire(questionnaireId).FirstOrDefault();

            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

            dropdownBindings(siteSelectList, partnumberStatusSelectList, pptq, partNumberSelectList);
            //temp comment out
            //JB: this ends process of setting dropdown bindings based on data in session


            //JB: creating the questionnaire part
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

            StringWriter objhtml = new StringWriter();
            using (var htmlWriter = new HtmlTextWriter(objhtml))
            {
                table.RenderControl(htmlWriter);
            }

            ViewBag.questions = objhtml.ToString();
            //JB: questionnaire creation part

            //JB: setting zcodes (for partnumbers that are completed)
            getZcodeByProviderProtocolCampaignQuestionnaire();
            //JB: end setting zcodes for partnumer


            return View();
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public virtual ActionResult QuestionnaireResponse(FormCollection formCollection, int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 0, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {

            var meesage = "";
                if (Session["hs3Registration"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                //JB: to set dropdown bindings begins
                Session["partnumber"] = partNumberSelectList;

                Session["partnumberstatus"] = partnumberStatusSelectList;

                Session["site"] = siteSelectList;

                int questionnaireId = 0;
                int partnerId = 0;
                int touchpointId = 0;
                int protocolId = 0;

                questionnaireId = (int)Session["questionnaire"];
                partnerId = (int)Session["partner"];
                touchpointId = (int)Session["touchpoint"];
                protocolId = (int)Session["protocol"];

                int questionId = 0;
                int surveyId = 0;
                string key = "";
                string[] array = new string[5];
                char[] splitter = { '_' };
                string answer = "";
                question objQuestion = new question();
                Boolean saveForLaterButton = false;
                string skip = "";
                string goEsignature = "";

                int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

                dropdownBindings(siteSelectList, partnumberStatusSelectList, pptq, partNumberSelectList);
                //JB: to set dropdown bindings ends
                //explain this -- start here

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



                    }





                    if (keyName.ToString().Contains("btnSaveForLater"))
                    {
                        saveForLaterButton = true;
                    }

                    if (keyName.ToString().Contains("question_"))
                    {

                        if (keyName.ToString().Contains("_text"))
                        {
                            array = keyName.ToString().Split(splitter);
                            questionId = int.Parse(array[1]);
                            surveyId = int.Parse(array[2]);

                            int? responseId = null;
                            string responseComment = string.Empty;

                            responseId = null;

                            responseComment = answer;
                            //explain this -- ends here


                            //JB: here he is actually setting responses to questions for the CURRENT PARTNUMBER
                            // var context = new EntitiesDBContext();

                            var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault();

                            var checkpsz = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).ToList();
                            if (checkpsz.Count == 0)
                            {
                                db.pr_addPartnumberSiteZcodePPTQQuestionResponse(questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                            }
                            else
                            {
                                var checkpszObj = checkpsz.FirstOrDefault();
                                if (checkpszObj != null)
                                {
                                    var checkpszId = checkpszObj.id;
                                    db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(checkpszId, questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                                }
                            }
                            meesage= ZcodeModify(questionnaireId, questionId, responseId, PartNumberSiteZcodepptq);
                            //JB: here ends setting responses to questions for the CURRENT PARTNUMBER
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

                                 //var context = new EntitiesDBContext();

                                var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault();

                                var checkpsz = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).ToList();
                                if (checkpsz.Count == 0)
                                {
                                    db.pr_addPartnumberSiteZcodePPTQQuestionResponse(questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                                }
                                else
                                {
                                    var checkpszObj = checkpsz.FirstOrDefault();
                                    if (checkpszObj != null)
                                    {
                                        var checkpszId = checkpszObj.id;
                                        db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(checkpszId, questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                                    }
                                }



                                meesage = ZcodeModify(questionnaireId, questionId, responseId, PartNumberSiteZcodepptq);


                            }
                        }
                        else if (keyName.ToString().Contains("_Commenttext"))
                        {
                            array = keyName.ToString().Split(splitter);
                            questionId = int.Parse(array[1]);
                            surveyId = int.Parse(array[2]);

                        }
                        else if (keyName.ToString().Contains("_onlyTextComment"))
                        {
                            array = keyName.ToString().Split(splitter);
                            questionId = int.Parse(array[1]);
                            surveyId = int.Parse(array[2]);

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

                             //var context = new EntitiesDBContext();
                            var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault();

                            var checkpsz = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).ToList();
                            if (checkpsz.Count == 0)
                            {
                                db.pr_addPartnumberSiteZcodePPTQQuestionResponse(questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                            }
                            else
                            {
                                var checkpszObj = checkpsz.FirstOrDefault();

                                if (checkpszObj != null && PartNumberSiteZcodepptq != null)
                                {
                                    var checkpszId = checkpszObj.id;
                                    try
                                    {
                                        db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(checkpszId, questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }


                            meesage = ZcodeModify(questionnaireId, questionId, responseId, PartNumberSiteZcodepptq);


                        }
                        //JB - just going through questions and setting question response values and modifying zcode values and updating splitter

                        //JB skip logic handling begins
                        objQuestion = db.pr_getQuestion(questionId).FirstOrDefault();

                        if (objQuestion.skipLogicJump != null)
                        {
                            if (objQuestion.skipLogicAnswer != null)
                            {
                                if (objQuestion.skipLogicJump.Contains("&"))
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
                                            Boolean foundFlage = false;
                                            for (int l = 0; l < formCollection.Keys.Count; ++l)
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
                                                        Response.Redirect("~/Registration/Home/eSignature");
                                                    }
                                                    if (questionId == questionidLogic)
                                                    {

                                                        foundFlage = true;

                                                    }

                                                    if (foundFlage)
                                                    {
                                                        question questionnew = db.pr_getQuestion(questionidLogic).FirstOrDefault();
                                                        var currentQuestion = db.pr_getQuestion(questionId).FirstOrDefault();
                                                        var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault();                                                        
                                                        int? rId = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).FirstOrDefault().response;
                                                        response responsenew = db.pr_getResponse(rId).FirstOrDefault();
                                                        int check = 0;
                                                        //if skip logic answer type is multiply then check by response.id
                                                        if (currentQuestion.skipLogicAnswer == SkipLogicAnswer.M)
                                                        {
                                                            check = responsenew.id == ansLogicStatus ? 1 : 0;
                                                        }
                                                        else
                                                            if (responsenew.description.ToLower() == "yes" || responsenew.description.ToLower() == "n/a" || responsenew.description.ToLower() == "no" || responsenew.description.ToLower() == "cots")
                                                            {
                                                                foundFlage = true;

                                                                if (ansLogicStatus == 1 && responsenew.description.ToLower() == "yes")
                                                                {
                                                                    check = 1;
                                                                }
                                                                else if (ansLogicStatus == 0 && responsenew.description.ToLower() == "no")
                                                                {
                                                                    check = 1;
                                                                }
                                                                else if (ansLogicStatus == -1 && responsenew.description.ToLower() == "n/a")
                                                                {
                                                                    check = 1;
                                                                }
                                                                else if (ansLogicStatus == 2 && responsenew.description.ToLower() == "cots")
                                                                {
                                                                    check = 1;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (ansLogicStatus == 3 && responsenew != null)
                                                                {
                                                                    check = 1;
                                                                }
                                                            }
                                                        if (check == 1)
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
                                        if (logicOneStatus == true && logicTwoStatus == true)
                                        {
                                            objQuestion.skipLogicJump = gotoQuestionId.ToString();
                                            jumpToQuestion = int.Parse(objQuestion.skipLogicJump);
                                            break;
                                        }
                                    }



                                }
                            }
                        }



                    }
                }

                if (jumpToQuestion != 0)
                {
                    // Skip ZCode update            
                    //var context = new EntitiesDBContext();
                    var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault(); ;

                    meesage= ZcodeModifyForSkip(questionnaireId, questionId, jumpToQuestion, PartNumberSiteZcodepptq);

                }


                if (meesage == "")
                {
                    // save uploaded files
                    saveUploadedFile(protocolId, touchpointId, partnerId, questionnaireId, pptq);




                    if (goEsignature == "true")
                    {

                        Response.Redirect("../Home/eSignature");

                    }

                    if (saveForLaterButton == true)
                    {

                    }

                    else
                    {
                        //if it is last question and all are completed for partnumberSelectList Status, then go to next site...reset siteSelectList --maybe the problem here
                        goToNextPage(surveyId, jumpToQuestion, questionIndex, objQuestion, skip, errorQuestion, errorMessage, partNumberSelectList, siteSelectList, partnumberStatusSelectList, page, pageNumber);
                    }
                }
                ViewBag.message = meesage;
            return View();
        }

        private string ZcodeModify(int questionnaireId, int questionId, int? responseId,  partNumberSiteZcodePPTQ PartNumberSiteZcodepptq)
        {
            var result = "";
            var responseTypesQuestionnaire = (List<responseType>)Session["responseTypesQuestionnaire"];

            string zcode = PartNumberSiteZcodepptq.zcode;
            var allQuestions = db.pr_getQuestionByQuestionnaire(questionnaireId).ToList();
            int questionNo = 0;
            foreach (var item in allQuestions)
            {
                questionNo++;

                if (questionId == item.id)
                {
                    break;
                }
            }
            string NewZcodePart1 = zcode.Substring(0, (questionNo * 2) - 2);

            //get zcode according to answer
            var responseZcode = db.pr_getResponse(responseId).FirstOrDefault();

            var objQuestion = db.pr_getQuestion(questionId).FirstOrDefault();

            if (objQuestion.responseType == ResponseType.DROPDOWN)
            {
                if (responseZcode.zcode != null)
                {
                    Session["CountryCode"] = responseZcode.zcode;
                }
            }

            string NewZcodePart2_CurrentQuestion = "--";
            if (responseZcode != null)
            {
                if (responseZcode.zcode != null)
                {
                    NewZcodePart2_CurrentQuestion = responseZcode.zcode;
                }
            }

            if (responseTypesQuestionnaire.Where(r => r.id == questionId).FirstOrDefault().description == "text")
            {
                NewZcodePart2_CurrentQuestion = ZCode.XX_Comment_Only_Question;
            }

            string NewZcodePart3 = zcode.Substring((questionNo * 2), zcode.Length - (questionNo * 2));
            //3    45
            //zzzz zz zz
            //0 4
            //6 L-6
            var zzcode = NewZcodePart1 + NewZcodePart2_CurrentQuestion + NewZcodePart3;
            var count = db.pr_checkPartnumberBadZcodeCountByZcode(zzcode).FirstOrDefault();
            if (count > 0)
            {
                result = "Please try again, if problem persists, please contact your system administrator by clicking on contact us button.<br>Thank you.";
                zzcode = "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ";
                PartNumberSiteZcodepptq.zcode = zzcode;
                using (var contect = new EntitiesDBContext())
                {
                    var pnszCodepptq = contect.partNumberSiteZcodePPTQSet.FirstOrDefault(o => o.id == PartNumberSiteZcodepptq.id);
                    pnszCodepptq.zcode = zzcode;
                    pnszCodepptq.status = Status.NOT_STARTED;
                    contect.Entry(pnszCodepptq).State = EntityState.Modified;
                    contect.SaveChanges();
                }
            }
            else
            {
                PartNumberSiteZcodepptq.zcode = zzcode;


                using (var contect = new EntitiesDBContext())
                {
                    var pnszCodepptq = contect.partNumberSiteZcodePPTQSet.FirstOrDefault(o => o.id == PartNumberSiteZcodepptq.id);
                    pnszCodepptq.zcode = zzcode;
                    contect.Entry(pnszCodepptq).State = EntityState.Modified;
                    contect.SaveChanges();
                }
            }
            return result;
        }

        private string ZcodeModifyForSkip(int questionnaireId, int questionId, int jumpToQuestion,  partNumberSiteZcodePPTQ PartNumberSiteZcodepptq)
        {
            var result = "";
            string zcode = PartNumberSiteZcodepptq.zcode;
            var allQuestions = db.pr_getQuestionByQuestionnaire(questionnaireId).ToList();
            int questionNo = 1;
            foreach (var item in allQuestions)
            {
                questionNo++;

                if (questionId == item.id)
                {
                    break;
                }
            }
            string NewZcodePart1 = zcode.Substring(0, (questionNo * 2) - 2);

            //get zcode according to answer

            string NewZcodePart2_CurrentQuestion = "";
            for (int i = 0; i < jumpToQuestion - questionId - 1; i++)
            {
                NewZcodePart2_CurrentQuestion += ZCode.YY_Skipped;

            }

            //2    6 -> 3 4 5

            string NewZcodePart3 = zcode.Substring((questionNo * 2) + (jumpToQuestion - questionId - 2) * 2, zcode.Length - ((questionNo * 2) + (jumpToQuestion - questionId - 2) * 2));
            //3    45
            //zzzz zzzzzz zzzzzz
            //0 4
            //6 L-6
            var zzcode = NewZcodePart1 + NewZcodePart2_CurrentQuestion + NewZcodePart3;
            var count = db.pr_checkPartnumberBadZcodeCountByZcode(zzcode).FirstOrDefault();
            if (count > 0)
            {
                result = "Please try again, if problem persists, please contact your system administrator by clicking on contact us button.<br>Thank you.";
                zzcode = "ZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZZ";
                PartNumberSiteZcodepptq.zcode = zzcode;
                using (var contect = new EntitiesDBContext())
                {
                    var pnszCodepptq = contect.partNumberSiteZcodePPTQSet.FirstOrDefault(o => o.id == PartNumberSiteZcodepptq.id);
                    pnszCodepptq.zcode = zzcode;
                    pnszCodepptq.status = Status.NOT_STARTED;
                    contect.Entry(pnszCodepptq).State = EntityState.Modified;
                    contect.SaveChanges();
                }
            }
            else
            {
                PartNumberSiteZcodepptq.zcode = zzcode;


                using (var context = new EntitiesDBContext())
                {
                    var pnszcodepptq = context.partNumberSiteZcodePPTQSet.FirstOrDefault(o => o.id == PartNumberSiteZcodepptq.id);
                    pnszcodepptq.zcode = zzcode;
                    context.Entry(pnszcodepptq).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            return result;
        }

        private void goToNextPage(int surveyId, int jumpToQuestion, int questionIndex, question question, string skip, int errorQuestion, string errorMessage, int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 0, int pageQ = 0, int pageNumberQ = 0)
        {
            int pageId = 0;
            int pageNumber = 0;
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


            if (pageNumberQ != 0)
            {
                pageNumber = pageNumberQ;
                pageNumber = pageNumber + 1;

            }
            else
            {
                pageNumber = 2;
            }

            if (pageQ != 0)
            {
                pageId = pageQ;

                page = db.pr_getNextPageByQuestionnaire(questionnaireId, pageId, jumpToQuestion).FirstOrDefault();
            }
            else
            {
                page = db.pr_getNextPageByQuestionnaire(questionnaireId, 0, 0).FirstOrDefault();
                page = db.pr_getNextPageByQuestionnaire(questionnaireId, page.id, jumpToQuestion).FirstOrDefault();
            }


            int providerId = 0;
            int quetionnaireId = 0;
            quetionnaireId = (int)Session["questionnaire"];

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
                        Response.Redirect("QuestionnaireResponse?partNumberSelectList=" + partNumberSelectList +
 "&siteSelectList=" + siteSelectList + "&partnumberStatusSelectList=" + partnumberStatusSelectList + "&pageNumber=" + pageNumber.ToString() +
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

                    int previouspartnumber = Convert.ToInt32(Session["partnumber"]);
                    int count1 = 0;
                    string key1 = "";
                    string[] array1 = new string[5];
                    char[] splitter1 = { '_' };
                    string answer1 = "";
                    bool finalAnswer = false;
                    int questionId1 = 0;
                    int surveyId1 = 0;
                    count1 = Request.Form.Count;
                    for (int r = 0; r < count1; r++)
                    {
                        key1 = Request.Form.Keys[r];
                        if (key1.Contains("question_"))
                        {
                            array1 = key1.Split(splitter1);
                            questionId1 = int.Parse(array1[1]);
                            surveyId1 = int.Parse(array1[2]);
                            answer1 = Request.Form[r];

                            answer1 = Request.Form[r];
                            if (answer1 == "74")
                            {
                                finalAnswer = true;
                            }
                        }
                    }

                    updateZcodesAll();

                    int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;
                    var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault(); ;


                    string countryCode = "";
                    if (Session["CountryCode"] != null)
                    {
                        countryCode = Session["CountryCode"].ToString();
                    }
                    CustomizedLSMW objCustomizedLSMW = db.pr_addCustomizedLSMWReport(PartNumberSiteZcodepptq.id, PartNumberSiteZcodepptq.zcode, countryCode).Select(x => new CustomizedLSMW
                    {
                        LIFNR = x.LIFNR,
                        MATNR = x.MATNR,
                        WERKS = x.WERKS,
                        ZCFLAG = x.ZCFLAG,
                        ZCODE = x.ZCODE,
                        ZPOST = x.ZPOST,
                        COMPLETED_DATE = x.COMPLETED_DATE,
                        PartnumberSiteZcode = PartNumberSiteZcodepptq.id
                    }).FirstOrDefault();
                    if (finalAnswer)
                    {
                        int flag = 0;
                        nextpartnumber();                        
                        List<CustomizedLSMW> customizedLSMW = (List<CustomizedLSMW>)Session["CustomizedLSMW"];
                        customizedLSMW.Add(objCustomizedLSMW);
                        Session["CustomizedLSMW"] = customizedLSMW;
                        Session["CountryCode"] = null;
                        if (Session["NextPartnumber"] == null)
                        {
                            flag = 1;
                        }
                        if (flag == 0)
                        {
                            Response.Redirect("QuestionnaireResponse?partNumberSelectList=" + Session["NextPartnumber"] +
"&siteSelectList=" + Session["site"] + "&partnumberStatusSelectList=" + Session["partnumberstatus"]);

                        }
                        else
                        {
                            //Reset all session of partnumber
                            Session["partnumber"] = null;
                            Session["site"] = null;
                            Session["partnumberstatus"] = null;
                            Response.Redirect("~/Registration/Home/eSignature");
                        }
                    }
                    else
                    {
                       
                        //Reset all session of partnumber
                        Session["partnumber"] = null;
                        Session["site"] = null;
                        Session["partnumberstatus"] = null;
                        Response.Redirect("~/Registration/Home/eSignature");
                    }
                }

            }
            else
            {
                if (page != null)
                {
                    if (string.IsNullOrEmpty(errorQueryString))
                    {
                        Response.Redirect("QuestionnaireResponse?partNumberSelectList=" + partNumberSelectList +
"&siteSelectList=" + siteSelectList + "&partnumberStatusSelectList=" + partnumberStatusSelectList + "&pageNumber=" + pageNumber.ToString() +
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


                    Response.Redirect("~/Registration/Home/eSignature");

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

                array = key.Split(splitter);
                questionId = int.Parse(array[1]);
                surveyId = int.Parse(array[2]);


                if (Request.Files[i].FileName.Length > 0)
                {
                    string Extension = Request.Files[i].FileName.Substring(Request.Files[i].FileName.LastIndexOf('.') + 1).ToLower();

                    if (Request.Files[i].ContentLength <= 4194304)
                    {

                        var pptqq = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault();



                        byte[] uploadedFile = new byte[Request.Files[i].InputStream.Length];
                        Request.Files[i].InputStream.Read(uploadedFile, 0, uploadedFile.Length);


                        db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(pptqq.id, questionId, pptqq.response, pptqq.comment, uploadedFile, Request.Files[i].ContentType, pptqq.value, pptqq.score, pptq);



                    }
                    else
                    {


                    }

                }


            }


            return jumpToQuestion;
        }

        public void getPartnumberBySite(int siteID, int pptqID, int partNumberID)
        {
            //if (Session["partnumberstatus"] != null && Int32.Parse(Session["partnumberstatus"].ToString()) != 0)
            //{
            //    ViewBag.partNumberSelectList = new SelectList(db.pr_getPartnumberByPPTQSiteAndStatus(pptqID, siteID, Int32.Parse(Session["partnumberstatus"].ToString())).Distinct(), "id", "description", partNumberID);

            //}
            //else
            //{
            ViewBag.partNumberSelectList = new SelectList(db.pr_getPartnumberSiteZcodePPTQByPPTQ_ToDo_ByPPTQ(pptqID).Where(p => p.site == siteID).ToList(), "partnumber", "description", partNumberID);
            //}
            Session["partnumber"] = partNumberID;
        }
        public void fillPartnumberBySite(int siteID, int pptqID)
        {
            var partNumberList = db.pr_getPartnumberByPPTQandSite(pptqID, siteID).Distinct().ToList();
            if (partNumberList.Count > 0)
            {
                Session["partnumber"] = partNumberList.First().id;
                ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description", partNumberList.First().id);
            }
            else
            {
                ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description");
            }
        }
        public void getPartnumber(int pptqID, int partNumberID)
        {
            var items = new SelectList(db.pr_getPartnumberByPPTQ(pptqID), "id", "description", partNumberID).Distinct().ToList();
            ViewBag.partNumberSelectList = items;
        }
        public void getPartnumber(int pptqID)
        {
            var partNumberList = db.pr_getPartnumberByPPTQ(pptqID).Distinct().ToList();
            if (partNumberList.Count > 0)
            {
                var items = new SelectList(partNumberList, "id", "description", partNumberList.First().id);
                Session["partnumber"] = partNumberList.First().id;
                ViewBag.partNumberSelectList = items;
            }
            else
            {
                var items = new SelectList(partNumberList, "id", "description");

                ViewBag.partNumberSelectList = items;
            }


        }
        public void nextpartnumber(int isForNext = 0)
        {
            int previouspartnumber = Convert.ToInt32(Session["partnumber"]);
            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;
            int site = Convert.ToInt32(Session["site"]);

            var nextSite = db.pr_getPartnumberSiteZcodePPTQByPPTQ_ToDo_ByPPTQ(pptq).Where(p => p.partnumber != previouspartnumber).FirstOrDefault();
            if (nextSite != null)
            {
                Session["site"] = nextSite.site;
                Session["NextPartnumber"] = nextSite.partnumber;
            }
        }


        public void previouspartnumber(int isForNext = 0)
        {
            int previouspartnumber = Convert.ToInt32(Session["partnumber"]);
            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;
            int partnumberstatus = Convert.ToInt32(Session["partnumberstatus"]);

            List<partnumber> objPartNumberList = new List<partnumber>();
            if (partnumberstatus == 0)
            {
                if (Session["site"] != null && Int32.Parse(Session["site"].ToString()) != 0)
                {
                    int site = Convert.ToInt32(Session["site"]);
                    objPartNumberList = db.pr_getPartnumberByPPTQandSite(pptq, site).Distinct().ToList();
                }
                else
                {
                    objPartNumberList = db.pr_getPartnumberByPPTQ(pptq).Distinct().ToList();
                }
            }
            else
            {
                if (Session["site"] != null && Int32.Parse(Session["site"].ToString()) != 0)
                {
                    int site = Convert.ToInt32(Session["site"]);
                    objPartNumberList = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, site, partnumberstatus).Distinct().ToList();
                }
                else
                {
                    objPartNumberList = db.pr_getPartnumberByPPTQandStatus(pptq, partnumberstatus).Distinct().ToList();
                }


            }


            Session["PreviousPartnumber"] = null;
            int checkNext = 0;
            int listitemNo = 1;
            objPartNumberList = objPartNumberList.OrderByDescending(x => x.id).ToList();
            if (objPartNumberList.Count > 1)
            {
                foreach (var item in objPartNumberList)
                {
                    if (checkNext == 1)
                    {
                        Session["PreviousPartnumber"] = item.id;
                        break;
                    }
                    if (item.id == previouspartnumber)
                    {
                        checkNext = 1;
                    }
                    listitemNo++;
                }
            }

            if (Session["PreviousPartnumber"] == null)
            {
                try
                {
                    Session["PreviousPartnumber"] = objPartNumberList.FirstOrDefault().id;
                }
                catch { }
            }

        }


        public void getSitepoint(int pptqID, int siteId)
        {
            var items = new SelectList(db.pr_getSiteByPPTQ(pptqID), "id", "description", siteId).ToList();
            ViewBag.siteSelectList = items;
        }
        public void getSitepoint(int pptqID)
        {
            var items = new SelectList(db.pr_getSiteByPPTQ(pptqID), "id", "description", Session["site"]).ToList();
            ViewBag.siteSelectList = items;
            try
            {
                //Session["site"] = int.Parse(items.FirstOrDefault().Value);
            }
            catch { }
        }

        public void getPartnumberstatus(int partNumberStatusId)
        {
            var items = new SelectList(db.pr_getPartnumberStatusAll(), "id", "description", partNumberStatusId).ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.partnumberStatusSelectList = items;
        }
        public void getPartnumberstatus()
        {
            var items = new SelectList(db.pr_getPartnumberStatusAll(), "id", "description").ToList();
            items.Insert(0, (new SelectListItem { Text = "All", Value = "0" }));
            ViewBag.partnumberStatusSelectList = items;
        }


        protected void getPartnumberByPartnumberStatusNotstarted(int pptq)
        {
            int site = 0;
            site = Convert.ToInt32(Session["site"]);
            var partNumberList = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, site, Helpers.PartNumberHelper.Status.NOT_STARTED).Distinct().ToList();
            ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description");
            try
            {
                Session["partnumber"] = partNumberList.First().id;
            }
            catch { }
        }
        protected void getPartnumberByPartnumberStatus(int pptq, int status)
        {
            int site = 0;
            site = Convert.ToInt32(Session["site"]);
            var partNumberList = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, site, status).Distinct().ToList();
            ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description");
            try
            {
                if (Session["partnumber"] != null && Int32.Parse(Session["partnumber"].ToString()) != 0)
                {

                }
                else
                {
                    Session["partnumber"] = partNumberList.First().id;
                }
            }
            catch { }
        }
        public void getZcodeByProviderProtocolCampaignQuestionnaire()
        {
            var pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            var psz = db.pr_getPartnumberSiteZcodeByPPTQForUI(pptq.id).ToList().Where(x => x.status == "Completed");

            List<CustomizedLSMW> customizedLSMW = (List<CustomizedLSMW>)Session["CustomizedLSMW"];


            string labeltext = "";
            if (psz.Count() > 0)
            {
                labeltext += "<table cellpadding='2' cellspacing='0' border='1' style='width: 100%;'>";
                labeltext += "<tr><td>Part Number</td><td>Site</td><td>Zcode</td><td>LIFNR</td><td>MATNR</td><td>WERKS</td><td>ZPOST</td><td>ZCFLAG</td><td>COMPLETED DATE</td></tr>";
                foreach (var dr in psz)
                {
                    labeltext += "<tr>";
                    try
                    {
                        labeltext += "<td>" + dr.partnumber + "</td><td>" + dr.site + "</td><td>" + dr.zcode + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().LIFNR + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().MATNR + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().WERKS + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().ZPOST + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().ZCFLAG + "</td><td>" + customizedLSMW.Where(x => x.PartnumberSiteZcode == dr.id).FirstOrDefault().COMPLETED_DATE.ToString("MM-dd-yyyy") + "</td>";
                    }
                    catch
                    {
                        labeltext += "<td>" + dr.partnumber + "</td><td>" + dr.site + "</td><td>" + dr.zcode + "</td><td></td><td></td><td></td><td></td><td></td><td></td>";
                    }
                    labeltext += "</tr>";
                }
                labeltext += "</table>";
            }






            ViewBag.zcodeList = labeltext;


        }

        public ActionResult GetQuestionnaire()
        {
            Response.Redirect("QuestionnaireResponse");
            return View();
        }
        [HttpPost]
        public ActionResult GetQuestionnaire(int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 0)
        {
            Response.Redirect("QuestionnaireResponse?partNumberSelectList=" + partNumberSelectList + "&siteSelectList=" + siteSelectList + "&partnumberStatusSelectList=" + partnumberStatusSelectList);
            return View();
        }


        public ActionResult SaveForLater()
        {
            updateZcodesAll();
            Response.Redirect("~/Registration/Home/eSignature");
            return View();
        }

        public ActionResult PreviousPartNumberUI()
        {
            previouspartnumber();
            Response.Redirect("QuestionnaireResponse?partNumberSelectList=" + Session["PreviousPartnumber"] +
"&siteSelectList=" + Session["site"] + "&partnumberStatusSelectList=" + Session["partnumberstatus"]);

            return View();
        }
        public ActionResult NextPartNumberUI()
        {
            Response.Redirect("QuestionnaireResponse?partNumberSelectList=" + Session["NextPartnumber"] +
"&siteSelectList=" + Session["site"] + "&partnumberStatusSelectList=" + Session["partnumberstatus"]);
            return View();
        }

        public ActionResult GetPartNumberByDropdown(int siteID, int partnumberStatusID)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index", "Home");
            }


            updateZcodesAll();


            Response.Redirect("QuestionnaireResponse?siteSelectList=" + siteID + "&partnumberStatusSelectList=" + partnumberStatusID);
            return View();
        }


        private void dropdownBindings(int siteSelectList, int partnumberStatusSelectList, int pptq, int partNumberSelectList)
        {
            //partnumberStatusSelectList
            if (partnumberStatusSelectList == 0)
            {
                getPartnumberstatus();
                Session["partnumberstatus"] = 0;
            }
            else
            {
                getPartnumberstatus(partnumberStatusSelectList);
                Session["partnumberstatus"] = partnumberStatusSelectList;
            }
            if (siteSelectList == 0)
            {
                getSitepoint(pptq);

            }
            else
            {
                getSitepoint(pptq, siteSelectList);
                Session["site"] = siteSelectList;
            }

            if (partNumberSelectList == 0)
            {
                if (partnumberStatusSelectList != 0)
                {
                    var partNumberList = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, Int32.Parse(Session["site"].ToString()), Int32.Parse(Session["partnumberstatus"].ToString())).Distinct().ToList();
                    try
                    {
                        ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description", partNumberList.First().id);
                        Session["partnumber"] = partNumberList.First().id;
                    }
                    catch
                    {
                        ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description");
                    }
                }
                else
                {
                    var partNumberList = db.pr_getPartnumberByPPTQandSite(pptq, Int32.Parse(Session["site"].ToString())).Distinct().ToList();
                    try
                    {
                        ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description", partNumberList.First().id);
                        Session["partnumber"] = partNumberList.First().id;
                    }
                    catch
                    {
                        ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description");
                    }
                }

            }
            else
            {
                getPartnumberBySite(int.Parse(Session["site"].ToString()), pptq, partNumberSelectList);
            }
            nextpartnumber();
        }

        private int updateZcodesAll()
        {
            int pptq = 0;
            using (var dbConext = new EntitiesDBContext())
            {
                pptq = dbConext.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;
                var mlist = dbConext.pr_getPartnumberSiteZcodePPTQByPPTQ(pptq).ToList();
                foreach (var item in mlist)
                {
                    if (item.zcode.Count(x => x == 'Z') == item.zcode.Length)
                    {
                        item.status = Status.NOT_STARTED;

                    }
                    else if (item.zcode.Count(x => x == 'Z') < 2)
                    {
                        item.status = Status.COMPLETED;
                    }
                    else
                    {
                        item.status = Status.INCOMPLETE;
                    }
                    dbConext.Entry(item).State = EntityState.Modified;
                    dbConext.SaveChanges();
                }
                
            }
            using (var dbConext = new EntitiesDBContext())
            {
                partnerPartnertypeTouchpointQuestionnaire objpptq = dbConext.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
                var statuses = dbConext.pr_getPartnumberSiteZcodePPTQByPPTQ(objpptq.id).ToList().Select(x => x.status).Distinct().ToList();
                if (statuses.Any(o => o == Status.COMPLETED || o == Status.INCOMPLETE))
                {                   
                    dbConext.pr_modifyPPTQStatus(objpptq.partner, objpptq.partnerTypeTouchpointQuestionnaire, (int)PartnerStatus.Responded_Incomplete);
                }
            }
            return pptq;
        }

    }
}
