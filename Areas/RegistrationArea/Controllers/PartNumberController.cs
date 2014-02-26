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

        public virtual ActionResult QuestionnaireResponse(int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 0, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.CMS_PAGE_TITLE = CMS.QUESTIONNAIRE_PAGE_TITLE;
            ViewBag.CMS_PAGE_SUBTITLE = CMS.QUESTIONNAIRE_PAGE_SUBTITLE;
            ViewBag.CMS_PAGE_PANEL_ONE = CMS.QUESTIONNAIRE_PAGE_PANEL_ONE;
            ViewBag.CMS_PAGE_PANEL_TWO = CMS.QUESTIONNAIRE_PAGE_PANEL_TWO;
            ViewBag.CMS_PAGE_PREVIOUS_TEXT = CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT;
            ViewBag.CMS_PAGE_NEXT_TEXT = CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT;
            ViewBag.SAVE_FOR_LATER_TEXT = CMS.SAVE_FOR_LATER_TEXT;

            ViewBag.QUESTIONNAIRE_PDF = CMS.QUESTIONNAIRE_PDF.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_FAQ = CMS.QUESTIONNAIRE_FAQ.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_DOC_OTHER = CMS.QUESTIONNAIRE_DOC_OTHER.Substring(0, 15);
            ViewBag.QUESTIONNAIRE_VIDEO = CMS.QUESTIONNAIRE_VIDEO.Substring(0, 15);
            ViewBag.CONTACT_US_EMAIL = CMS.CONTACT_US_EMAIL.Substring(0, 15);

            var ppptq_cms = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault();
            if (ppptq_cms != null)
            {
                var ptq = db.pr_getPartnertypeTouchpointQuestionnaire(ppptq_cms.partnerTypeTouchpointQuestionnaire).FirstOrDefault();
                var cms = db.pr_getQuestionnaireQuestionnaireCMSAllByQuestionnaire(ptq.questionnaire).ToList();
                var questionnairCMSAll = db.pr_getQuestionnaireCMSAll().ToList();
                try
                {
                    ViewBag.CMS_PAGE_TITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PAGE_TITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_SUBTITLE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PAGE_SUBTITLE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_ONE = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_ONE).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PANEL_TWO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PAGE_PANEL_TWO).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_PREVIOUS_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PAGE_PREVIOUS_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.CMS_PAGE_NEXT_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PAGE_NEXT_TEXT).FirstOrDefault().id).FirstOrDefault().text;
                    ViewBag.SAVE_FOR_LATER_TEXT = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.SAVE_FOR_LATER_TEXT).FirstOrDefault().id).FirstOrDefault().text;

                    ViewBag.QUESTIONNAIRE_PDF = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_PDF).FirstOrDefault().id).FirstOrDefault().text.PadRight(15).Substring(0, 15);
                    ViewBag.QUESTIONNAIRE_FAQ = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_FAQ).FirstOrDefault().id).FirstOrDefault().text.PadRight(15).Substring(0, 15);
                    ViewBag.QUESTIONNAIRE_DOC_OTHER = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_DOC_OTHER).FirstOrDefault().id).FirstOrDefault().text.PadRight(15).Substring(0, 15);
                    ViewBag.QUESTIONNAIRE_VIDEO = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.QUESTIONNAIRE_VIDEO).FirstOrDefault().id).FirstOrDefault().text.PadRight(15).Substring(0, 15);
                    ViewBag.CONTACT_US_EMAIL = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id).FirstOrDefault().text.PadRight(15).Substring(0, 15);

                    ViewBag.QUESTIONNAIRE_VIDEO_LINK = cms.Where(x => x.questionnaireCMS == questionnairCMSAll.Where(q => q.description == CMS.CONTACT_US_EMAIL).FirstOrDefault().id).FirstOrDefault().link;
                }
                catch { }
            }

            Session["partnumber"] = partNumberSelectList;
            Session["site"] = siteSelectList;
            Session["partnumberstatus"] = partnumberStatusSelectList;
            int questionnaireId = (int)Session["questionnaire"];

            questionnaire objQuestionnaire = db.pr_getQuestionnaire(questionnaireId).FirstOrDefault();

            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

            if (Session["partnumber"] != null && Int32.Parse(Session["partnumber"].ToString()) != 0)
            {
                int partNumberID = Int32.Parse(Session["partnumber"].ToString());
                if (Session["site"] != null && Int32.Parse(Session["site"].ToString()) != 0)
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
                if (Session["partnumberstatus"] != null && Int32.Parse(Session["partnumberstatus"].ToString()) != 0)
                {

                    int siteId = Int32.Parse(Session["site"].ToString());

                    var partNumberList = db.pr_getPartnumberByPPTQandStatus(pptq, Int32.Parse(Session["partnumberstatus"].ToString())).ToList();
                    var items = new SelectList(partNumberList, "id", "description", partNumberList.First().id);
                    // Session["partnumber"] = partNumberList.First().id;
                    // items.Insert(0, (new SelectListItem { Text = "All", Value = "" }));
                    ViewBag.partNumberSelectList = items;
                }
                else
                {
                    getPartnumber(pptq);
                }
                
                //  Session["partnumber"] = partNumberSelectList;
            }
            if (Session["site"] != null && Int32.Parse(Session["site"].ToString()) != 0)
            {

                int siteId = Int32.Parse(Session["site"].ToString());
                getSitepoint(pptq, siteId);

            }
            else
            {
                getSitepoint(pptq);
                Session["site"] = siteSelectList;
            }
            if (Session["partnumberstatus"] != null && Int32.Parse(Session["partnumberstatus"].ToString()) != 0)
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
                    getPartnumberByPartnumberStatus(pptq, partnumberstatusID);
                }
                else if (partnumberstatusID == Generic.Helpers.PartNumberHelper.Status.INCOMPLETE)
                {
                    getPartnumberstatus(partnumberstatusID);
                    getPartnumberByPartnumberStatus(pptq, partnumberstatusID);
                }
                else if (partnumberstatusID == Generic.Helpers.PartNumberHelper.Status.NOT_STARTED)
                {
                    //if (ddlSitepoint.SelectedIndex > -1)
                    //{
                    int site = siteSelectList;
                    if (Session["partnumber"] != null && Int32.Parse(Session["partnumber"].ToString()) != 0)
                    {
                        int partindex = Convert.ToInt32(Session["partnumber"]);
                        if (partindex > 0)
                        {
                            fillPartnumberBySite(site, pptq, partindex);
                        }
                    }
                    else
                    {
                        if (Session["partnumberstatus"] != null && Int32.Parse(Session["partnumberstatus"].ToString()) != 0)
                        {

                            int siteId = Int32.Parse(Session["site"].ToString());

                            var partNumberList = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, siteId, Int32.Parse(Session["partnumberstatus"].ToString())).ToList();
                            var items = new SelectList(partNumberList, "id", "description", partNumberList.First().id);
                            Session["partnumber"] = partNumberList.First().id;
                            // items.Insert(0, (new SelectListItem { Text = "All", Value = "" }));
                            ViewBag.partNumberSelectList = items;
                        }
                        else
                        {
                            fillPartnumberBySite(site, pptq);
                        }
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
                Session["partnumberstatus"] = partnumberStatusSelectList;
            }









            //int id = (int)Session["protocol"];

            //id = (int)Session["campaign"];

            //id = (int)Session["providerId"];


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
        public virtual ActionResult QuestionnaireResponse(FormCollection formCollection, int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 0, int questionIndex = 0, int jumpToQuestion = 0, int page = 0, int errorQuestion = 0, int pageNumber = 1, string errorMessage = null)
        {
            if (Session["hs3Registration"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            Session["partnumber"] = partNumberSelectList;

            Session["partnumberstatus"] = partnumberStatusSelectList;

            Session["site"] = siteSelectList;


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

                        responseId = null;

                        responseComment = answer;

                        var context = new EntitiesDBContext();

                        var PartNumberSiteZcodepptq = context.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault(); ;

                        var checkpsz = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).ToList();
                        if (checkpsz.Count == 0)
                        {
                            db.pr_addPartnumberSiteZcodePPTQQuestionResponse(questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                        }
                        else
                        {
                            db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(checkpsz.FirstOrDefault().id, questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                        }
                        ZcodeModify(questionnaireId, questionId, responseId, context, PartNumberSiteZcodepptq);
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

                            var context = new EntitiesDBContext();

                            var PartNumberSiteZcodepptq = context.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault(); ;

                            var checkpsz = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).ToList();
                            if (checkpsz.Count == 0)
                            {
                                db.pr_addPartnumberSiteZcodePPTQQuestionResponse(questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                            }
                            else
                            {
                                db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(checkpsz.FirstOrDefault().id, questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                            }



                            ZcodeModify(questionnaireId, questionId, responseId, context, PartNumberSiteZcodepptq);


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

                        var context = new EntitiesDBContext();
                        var PartNumberSiteZcodepptq = context.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault(); ;

                        var checkpsz = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).ToList();
                        if (checkpsz.Count == 0)
                        {
                            db.pr_addPartnumberSiteZcodePPTQQuestionResponse(questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                        }
                        else
                        {
                            //db.pr_removePartnumberSiteZcodePPTQQuestionResponse(
                            db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(checkpsz.FirstOrDefault().id, questionId, responseId, responseComment, null, null, null, null, PartNumberSiteZcodepptq.id);
                        }


                        ZcodeModify(questionnaireId, questionId, responseId, context, PartNumberSiteZcodepptq);


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
                                                //   responsenew = db.pr_getResponseByQuestion(questionidLogic).FirstOrDefault();
                                                var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault(); ;
                                                var context = new EntitiesDBContext();
                                                int? rId = context.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(questionId, PartNumberSiteZcodepptq.id).FirstOrDefault().response;
                                                response responsenew = db.pr_getResponse(rId).FirstOrDefault();
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

            if (jumpToQuestion != 0)
            {
                // Skip ZCode update            
                var context = new EntitiesDBContext();
                var PartNumberSiteZcodepptq = context.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(partNumberSelectList, siteSelectList, pptq).FirstOrDefault(); ;

                ZcodeModifyForSkip(questionnaireId, questionId, jumpToQuestion, context, PartNumberSiteZcodepptq);

            }


            // save uploaded files
            //jumpToQuestion = 
            saveUploadedFile(protocolId, touchpointId, partnerId, questionnaireId, pptq);


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
                //Session["partnumber"] = null;
                //Session["site"] = null;
                //Session["partnumberstatus"] = null;
                Response.Redirect("../Home/eSignature");
                // return RedirectToAction("QuestionnaireResponse", "PartNumber");
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
                goToNextPage(surveyId, jumpToQuestion, questionIndex, objQuestion, skip, errorQuestion, errorMessage, partNumberSelectList, siteSelectList, partnumberStatusSelectList, page, pageNumber);
            }


            return View();
        }

        private void ZcodeModify(int questionnaireId, int questionId, int? responseId, EntitiesDBContext context, partNumberSiteZcodePPTQ PartNumberSiteZcodepptq)
        {
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
            PartNumberSiteZcodepptq.zcode = NewZcodePart1 + NewZcodePart2_CurrentQuestion + NewZcodePart3;
            context.Entry(PartNumberSiteZcodepptq).State = EntityState.Modified;
            context.SaveChanges();
        }



        private void ZcodeModifyForSkip(int questionnaireId, int questionId, int jumpToQuestion, EntitiesDBContext context, partNumberSiteZcodePPTQ PartNumberSiteZcodepptq)
        {
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
            PartNumberSiteZcodepptq.zcode = NewZcodePart1 + NewZcodePart2_CurrentQuestion + NewZcodePart3;
            context.Entry(PartNumberSiteZcodepptq).State = EntityState.Modified;
            context.SaveChanges();
        }


        private void goToNextPage(int surveyId, int jumpToQuestion, int questionIndex, question question, string skip, int errorQuestion, string errorMessage, int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 0, int pageQ = 0, int pageNumberQ = 0)
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
                    //int providerId = 0;
                    //int quetionnaireId = 0;
                    //providerId = (int)Session["providerId"];
                    //quetionnaireId = (int)Session["questionnaire"];
                    //Provider provider = new Provider(new Id(providerId));
                    //Campaign campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
                    //provider.updateSurveyProgressBar(provider, campaign, 80);
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
                            //-- question = new Question(new Id(questionId1));

                            answer1 = Request.Form[r];
                            if (answer1 == "74")
                            {
                                finalAnswer = true;
                            }
                        }
                    }
                    if (finalAnswer)
                    {
                        int flag = 0;
                        previouspartnumber = Convert.ToInt32(Session["partnumber"]);
                        int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

                        List<partnumber> objPartNumberList;

                        //if (Session["site"] != null && Int32.Parse(Session["site"].ToString()) != 0)
                        //{
                        int site = Convert.ToInt32(Session["site"]);
                        objPartNumberList = db.pr_getPartnumberByPPTQandSite(pptq, site).ToList();
                        //}
                        //else
                        //{
                        //    objPartNumberList = db.pr_getPartnumberByPPTQ(pptq).ToList();
                        //}



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


                        if (checkNext == 0)
                        {
                            var sites = db.pr_getSiteByPPTQ(pptq);
                            int checkNextSite = 0;
                            foreach (var item in sites)
                            {
                                if (checkNextSite == 1)
                                {
                                    Session["site"] = item.id;

                                    break;
                                }
                                if (item.id == site)
                                {
                                    checkNextSite = 1;
                                }
                            }
                            if (checkNextSite == 1)
                            {
                                Session["NextPartnumber"] = db.pr_getPartnumberByPPTQandSite(pptq, site).FirstOrDefault().id;


                            }
                            else
                            {
                                flag = 1;
                            }
                        }

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
                    //if (menuGroup.id.id == 2)
                    //{
                    //    Response.Redirect("spendCategoryForm.aspx");
                    //}
                    //else
                    //{

                    Response.Redirect("~/Registration/Home/eSignature");
                    //}
                }
            }



        }



        //       private void goToNextPage1(int surveyId, int jumpToQuestion, int questionIndex, question question, string skip, int errorQuestion, string errorMessage, int partNumberSelectList = 0, int siteSelectList = 0, int partnumberStatusSelectList = 0, int pageQ = 0, int pageNumberQ = 0)
        //       {
        //           int pageId = 0;
        //           int pageNumber = 0;
        //           int pgno = 0;
        //           int questionnaireId = (int)Session["questionnaire"];
        //           string errorQueryString = "";

        //           if (errorQuestion > 0)
        //           {
        //               errorQueryString = "&errorQuestion=" + errorQuestion.ToString() + "&errorMessage=" + errorMessage;
        //           }
        //           surveyForm surveyForm = new surveyForm();
        //           int partnerId = (int)Session["partner"];
        //           partner provider1 = new partner();
        //           questionnaire questionnaire = new questionnaire();
        //           string zode = surveyForm.generateZCode(provider1, questionnaire);
        //           page page = null;
        //           // menuGroup = (MenuGroup)Session["menuGroup"];

        //           if (pageNumberQ != 0)
        //           {
        //               pageNumber = pageNumberQ;
        //               pageNumber = pageNumber + 1;

        //           }
        //           else
        //           {
        //               pageNumber = 2;
        //           }

        //           if (pageQ != 0)
        //           {
        //               pageId = pageQ;

        //               page = db.pr_getNextPageByQuestionnaire(questionnaireId, pageId, jumpToQuestion).FirstOrDefault();
        //           }
        //           else
        //           {
        //               page = db.pr_getNextPageByQuestionnaire(questionnaireId, 0, 0).FirstOrDefault();
        //               page = db.pr_getNextPageByQuestionnaire(questionnaireId, page.id, jumpToQuestion).FirstOrDefault();
        //           }
        //           // update progressbare logic

        //           int providerId = 0;
        //           int quetionnaireId = 0;
        //           //   providerId = (int)Session["provider"];
        //           quetionnaireId = (int)Session["questionnaire"];
        //           //    Provider provider = new Provider(new Id(providerId));
        //           //  Campaign campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
        //           //int totalpage = questionnaire.getPageQuestionnaireCount(questionnaire);
        //           //pgno = pageNumber - 1;
        //           //for (int i = 0; i < totalpage; i++)
        //           //{
        //           //    if (pgno == i)
        //           //    {
        //           //        int totalperage = 0;
        //           //        int totper = 80 / totalpage;
        //           //        if (pgno == 0)
        //           //        {
        //           //            pgno = 1;
        //           //            totalperage = (totper * 1) + 10;
        //           //        }
        //           //        else
        //           //        {
        //           //            totalperage = (totper * i) + 10;
        //           //        }
        //           //        provider.updateSurveyProgressBar(provider, campaign, totalperage);
        //           //        break;
        //           //    }
        //           //}
        //           //
        //           if (Request.QueryString["questionIndex"] != null)
        //           {
        //               questionIndex += int.Parse(Request.QueryString["questionIndex"].ToString());
        //           }

        //           if (Request.QueryString["skip"] != null)
        //           {
        //               skip = "&skip=true";
        //           }

        //           if (jumpToQuestion == 0)
        //           {
        //               if (page != null)
        //               {
        //                   if (string.IsNullOrEmpty(errorQueryString))
        //                   {
        //                       Response.Redirect("QuestionnaireResponse?partNumberSelectList=" + partNumberSelectList +
        //"&siteSelectList=" + siteSelectList + "&partnumberStatusSelectList=" + partnumberStatusSelectList + "&pageNumber=" + pageNumber.ToString() +
        //                               "&page=" + page.id.ToString() + "&jumpToQuestion=" + jumpToQuestion.ToString()
        //                               + "&questionIndex=" + questionIndex.ToString() + skip);
        //                   }
        //                   else if (Request.QueryString["errorQuestion"] == null)
        //                   {
        //                       Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString() + errorQueryString);
        //                   }
        //                   else
        //                   {
        //                       Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString());
        //                   }
        //               }
        //               else
        //               {
        //                   //if (menuGroup.id.id == 2)
        //                   //{
        //                   //    Response.Redirect("spendCategoryForm.aspx");
        //                   //}
        //                   //else
        //                   //{
        //                   //int providerId = 0;
        //                   //int quetionnaireId = 0;
        //                   //providerId = (int)Session["providerId"];
        //                   //quetionnaireId = (int)Session["questionnaire"];
        //                   //Provider provider = new Provider(new Id(providerId));
        //                   //Campaign campaign = new Campaign(new Id(Convert.ToInt32(Session["campaign"])));
        //                   //provider.updateSurveyProgressBar(provider, campaign, 80);
        //                   Response.Redirect("~/Registration/Home/eSignature");
        //                   // }
        //               }
        //           }
        //           else
        //           {
        //               if (page != null)
        //               {
        //                   if (string.IsNullOrEmpty(errorQueryString))
        //                   {
        //                       Response.Redirect("QuestionnaireResponse?pageNumber=" + pageNumber.ToString() +
        //                          "&page=" + page.id.ToString() + "&jumpToQuestion=" + jumpToQuestion.ToString()
        //                          + "&questionIndex=" + questionIndex.ToString() + skip);
        //                   }
        //                   else if (Request.QueryString["errorQuestion"] == null)
        //                   {
        //                       Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString() + errorQueryString);
        //                   }
        //                   else
        //                   {
        //                       Response.Redirect("QuestionnaireResponse?" + Request.QueryString.ToString());
        //                   }
        //               }
        //               else
        //               {
        //                   //if (menuGroup.id.id == 2)
        //                   //{
        //                   //    Response.Redirect("spendCategoryForm.aspx");
        //                   //}
        //                   //else
        //                   //{

        //                   Response.Redirect("~/Registration/Home/eSignature");
        //                   //}
        //               }
        //           }
        //       }

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


                        var pptqq = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPPTQ(questionId, pptq).FirstOrDefault();



                        byte[] uploadedFile = new byte[Request.Files[i].InputStream.Length];
                        Request.Files[i].InputStream.Read(uploadedFile, 0, uploadedFile.Length);

                        // Binary linqBinary = new Binary(uploadedFile);

                        db.pr_modifyPartnumberSiteZcodePPTQQuestionResponse(pptqq.id, questionId, pptqq.response, pptqq.comment, uploadedFile, Request.Files[i].ContentType, pptqq.value, pptqq.score, pptq);

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
            if (Session["partnumberstatus"] != null && Int32.Parse(Session["partnumberstatus"].ToString()) != 0)
            {
                ViewBag.partNumberSelectList = new SelectList(db.pr_getPartnumberByPPTQSiteAndStatus(pptqID, siteID, Int32.Parse(Session["partnumberstatus"].ToString())), "id", "description", partNumberID); 
                
            }
            else
            {
                ViewBag.partNumberSelectList = new SelectList(db.pr_getPartnumberByPPTQandSite(pptqID, siteID), "id", "description", partNumberID);
            }
            Session["partnumber"] = partNumberID;
        }
        public void fillPartnumberBySite(int siteID, int pptqID)
        {
            var partNumberList = db.pr_getPartnumberByPPTQandSite(pptqID, siteID).ToList();
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
            var items = new SelectList(db.pr_getPartnumberByPPTQ(pptqID), "id", "description", partNumberID).ToList();
            // items.Insert(0, (new SelectListItem { Text = "All", Value = "" }));
            ViewBag.partNumberSelectList = items;
        }
        public void getPartnumber(int pptqID)
        {
            var partNumberList = db.pr_getPartnumberByPPTQ(pptqID).ToList();
            if (partNumberList.Count > 0)
            {
                var items = new SelectList(partNumberList, "id", "description", partNumberList.First().id);
                Session["partnumber"] = partNumberList.First().id;
                // items.Insert(0, (new SelectListItem { Text = "All", Value = "" }));
                ViewBag.partNumberSelectList = items;
            }
            else
            {
                var items = new SelectList(partNumberList, "id", "description");

                // items.Insert(0, (new SelectListItem { Text = "All", Value = "" }));
                ViewBag.partNumberSelectList = items;
            }


        }
        public void nextpartnumber()
        {
            int previouspartnumber = Convert.ToInt32(Session["partnumber"]);
            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

            List<partnumber> objPartNumberList;

            if (Session["site"] != null && Int32.Parse(Session["site"].ToString()) != 0)
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
            //  items.Insert(0, (new SelectListItem { Text = "All", Value = "" }));
            ViewBag.siteSelectList = items;
        }
        public void getSitepoint(int pptqID)
        {
            var items = new SelectList(db.pr_getSiteByPPTQ(pptqID), "id", "description").ToList();
            //   items.Insert(0, (new SelectListItem { Text = "All", Value = "" }));
            ViewBag.siteSelectList = items;
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
            var partNumberList = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, site, Helpers.PartNumberHelper.Status.NOT_STARTED).ToList();
            ViewBag.partNumberSelectList = new SelectList(partNumberList, "id", "description");
            try
            {
                Session["partnumber"] = partNumberList.First().id;
            }
            catch { }
        }
        protected void getPartnumberByPartnumberStatus(int pptq,int status)
        {
            int site = 0;
            site = Convert.ToInt32(Session["site"]);
            var partNumberList = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, site, status).ToList();
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
        public ActionResult GetPartNumberByDropdown(int siteID, int partnumberStatusID)
        {
            int pptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(Session["accessCode"].ToString()).FirstOrDefault().id;

            foreach (var item in db.pr_getPartnumberSiteZcodePPTQByPPTQ(pptq).ToList())
            {
                if (item.zcode.Count(x => x == 'Z') == item.zcode.Length)
                {
                    item.status = Status.NOT_STARTED;

                }
                else if (item.zcode.Count(x => x == 'Z') == 0)
                {
                    item.status = Status.COMPLETED;
                }
                else
                {
                    item.status = Status.INCOMPLETE;
                }
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }

            List<partnumber> objpartnumber = new List<partnumber>();
            if (siteID == 0 && partnumberStatusID == 0)
            {
                //show all partnumbers
                objpartnumber = db.pr_getPartnumberByPPTQ(pptq).ToList();
            }
            else if (siteID != 0 && partnumberStatusID != 0)
            {
                // show partnumbers according to site and status
                objpartnumber = db.pr_getPartnumberByPPTQSiteAndStatus(pptq, siteID, partnumberStatusID).ToList();
            }
            else if (siteID == 0 && partnumberStatusID != 0)
            {
                // show partnumbers according to status only
                objpartnumber = db.pr_getPartnumberByPPTQandStatus(pptq, partnumberStatusID).ToList();
            }
            else
            {
                objpartnumber = db.pr_getPartnumberByPPTQandSite(pptq, siteID).ToList();
                // show partnumbers according to site only
            }
            Session["site"] = siteID;
            Session["partnumberstatus"] = partnumberStatusID;
            try
            {
                Session["partnumber"] = objpartnumber.FirstOrDefault().id;
            }
            catch { }
            //return Json(new { Data = objpartnumber.Select(x => new { x.id, x.description }) }, JsonRequestBehavior.AllowGet);
            Response.Redirect("QuestionnaireResponse?siteSelectList=" + siteID + "&partnumberStatusSelectList=" + partnumberStatusID);
            return View();
        }

    }
}
