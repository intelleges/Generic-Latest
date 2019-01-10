using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Google.API.Translate;
using Generic;
using System.Text;
using Generic.Helpers.Questionnaire;
using Generic.Helpers.Utility;
using System.Text.RegularExpressions;
using Generic.Helpers;
using HtmlAgilityPack;

namespace Generic.DataLayer
{

    /// <summary>
    /// Summary description for surveyForm
    /// </summary>
    public class surveyForm
    {
        EntitiesDBContext db = new EntitiesDBContext();
        IGoogleTranslatorHelper _translator;
        static int divShowHideFlag = 3;
        string _currentLanguage;
        public surveyForm()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public surveyForm(protocol protocol, touchpoint touchpoint, partner partner, questionnaire questionnaire, IGoogleTranslatorHelper translator, string currentLanguage)
        {

            this.protocol = protocol;
            this.touchpoint = touchpoint;
            this.partner = partner;
            this.questionnaire = questionnaire;
            _translator = translator;
            _currentLanguage = currentLanguage;
        }

        private int questionCount;

        private string _questionClass;

        public string questionClass
        {
            get { return _questionClass; }
            set { _questionClass = value; }
        }

        private int _questionIndex;

        public int questionIndex
        {
            get { return _questionIndex; }
            set { _questionIndex = value; }
        }

        private string _alternativequestionClass;

        public string alternativequestionClass
        {
            get { return _alternativequestionClass; }
            set { _alternativequestionClass = value; }
        }

        private string _answerClass;

        public string answerClass
        {
            get { return _answerClass; }
            set { _answerClass = value; }
        }

        private string _alternativeAnswerClass;

        public string alternativeAnswerClass
        {
            get { return _alternativeAnswerClass; }
            set { _alternativeAnswerClass = value; }
        }

        private bool _showContentOnly;

        public bool showContentOnly
        {
            get { return _showContentOnly; }
            set { _showContentOnly = value; }
        }

        private protocol _protocol;

        public protocol protocol
        {
            get { return _protocol; }
            set { _protocol = value; }
        }

        private touchpoint _touchpoint;

        public touchpoint touchpoint
        {
            get { return _touchpoint; }
            set { _touchpoint = value; }
        }

        private partner _partner;

        public partner partner
        {
            get { return _partner; }
            set { _partner = value; }
        }

        private questionnaire _questionnaire;

        public questionnaire questionnaire
        {
            get { return _questionnaire; }
            set { _questionnaire = value; }
        }

        private Panel _panelModal;

        public Panel panelModal
        {
            get { return _panelModal; }
            set { _panelModal = value; }
        }

        private int _errorquestion;

        public int errorquestion
        {
            get { return _errorquestion; }
            set { _errorquestion = value; }
        }

        private string _errorMessage;

        public string errorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = "<span style='color: red'> " + value + "</span>"; }
        }


        public Table tGetsurveyForm(questionnaire questionnaire, int pageNumber, int pageId, int jumpToquestion, string urlQuestion)
        {
            Table table = new Table();
            table.Width = Unit.Percentage(100);

            if (pageNumber == 0)
            {
                pageNumber = 1;
            }
            showPageCollectionByquestionnaire(questionnaire, pageNumber, pageId, jumpToquestion, table, urlQuestion);

            return table;
        }
        public Table tGetsurveyForm(questionnaire questionnaire)
        {
            Table table = new Table();
            this.showContentOnly = true;
            showquestionCollectionByquestionnaire(questionnaire, table);
            return table;
        }

        private void showPageCollectionByquestionnaire(questionnaire questionnaire, int pageNumber, int pageId, int jumpToquestion, Table table, string urlQuestion)
        {
            List<page> pageCollection = db.pr_getPageByQuestionnaire(questionnaire.id).ToList();
            page page = null;

            if (pageCollection.Count > 0)
            {
                for (int i = 0; i < pageCollection.Count; i++)
                {
                    page = pageCollection[i];

                    //show only current page
                    //pageId is got from queryString
                    if (pageId == page.id || (pageId == 0 && i == 0))
                    {
                        //get surveysets by page
                        showsurveysetCollectionByPage(questionnaire, page, jumpToquestion, table, urlQuestion);
                    }
                }
            }
        }

        private void showsurveysetCollectionByPage(questionnaire questionnaire, page page, int jumpToquestion, Table table, string urlQuestion)
        {
            List<surveyset> surveysetCollection = db.pr_getSurveysetByPage(page.id).ToList();
            surveyset surveyset = null;
            this.questionIndex = 0;
            if (surveysetCollection.Count > 0)
            {
                for (int i = 0; i < surveysetCollection.Count; i++)
                {
                    surveyset = surveysetCollection[i];

                    //get surveys by surveSet
                    showsurveyCollectionBysurveyset(questionnaire, surveyset, jumpToquestion, table, urlQuestion);
                }
            }
        }

        private void showsurveyCollectionBysurveyset(questionnaire questionnaire, surveyset surveyset, int jumpToquestion, Table table, string urlQuestion)
        {
            List<survey> surveyCollection = db.pr_getSurveyBySurveyset(surveyset.id).ToList();
            survey survey = null;

            if (surveyCollection.Count > 0)
            {
                for (int i = 0; i < surveyCollection.Count; i++)
                {
                    survey = surveyCollection[i];
                    showquestionCollectionBysurvey(questionnaire, surveyset, survey, ref jumpToquestion, table, urlQuestion);
                }
            }
        }

        //    private void showquestionCollectionBysurvey(survey survey, int jumpToquestion, Table table)
        private void showquestionCollectionBysurvey(questionnaire questionnaire, surveyset surveyset, survey survey, ref int jumpToquestion, Table table, string urlQuestion)
        {

            List<question> questionCollection = db.pr_getQuestionBySurveySkipLogic(survey.id, jumpToquestion).ToList();
            question question = null;

            if (questionCollection.Count > 0)
            {
                this.questionCount = questionCollection.Count;

                for (int i = 0; i < questionCollection.Count; i++)
                {
                    question = questionCollection[i];
                    this.questionIndex += 1;
                    jumpToquestion++;
                    showquestion(questionnaire, surveyset, survey, question, this.questionIndex, table, urlQuestion);
                }
            }
        }

        private void showquestionCollectionByquestionnaire(questionnaire questionnaire, Table table)
        {
            //List<question> questionCollection = questionnaire.getquestionCollection();
            //string surveysetDesc = "";
            //surveysetDesc = questionCollection[0].surveyset.description;
            //surveyset surveyset = new surveyset(surveysetDesc);
            //question question = null;

            //if (questionCollection.Count > 0)
            //{
            //    for (int i = 0; i < questionCollection.Count; i++)
            //    {
            //        question = questionCollection[i].getquestionDetail();
            //        showquestion(surveyset, new survey(new Id(0)), question, i, table);
            //    }
            //}
        }
        public string convertLanguageApi(string srtto)
        {
            //string translated = "";
            //if (srtto != null)
            //{
            //    if (GlobalVariable.languageStr == "" || GlobalVariable.languageStr == null)
            //    {
            //        GlobalVariable.languageStr = "en";
            //    }
            //    if (srtto.Length > 0)
            //    {
            //        //TranslateClient client = new TranslateClient("http://www.intelleges.com");
            //        //string convertin = GlobalVariable.languageStr; 
            //        //translated = client.Translate(srtto, Google.API.Translate.Language.English, convertin);            
            //    }
            //    //return translated;
            //}
            return srtto;

        }
        public string convertLanguageToEnglish(string strfrom)
        {
            //        string translated = "";
            //        if (GlobalVariable.languageStr == "" || GlobalVariable.languageStr == null)
            //        {
            //            GlobalVariable.languageStr = "en";
            //        }
            //        if (strfrom.Length > 0)
            //        {
            //            //TranslateClient client = new TranslateClient("http://www.intelleges.com");
            //            //string convertin = GlobalVariable.languageStr; //Google.API.Translate.Language.Spanish;
            //            //translated = client.Translate(strfrom, convertin, Google.API.Translate.Language.English);          
            //            TranslateClient client = new TranslateClient("http://www.intelleges.com");
            //            string convertin = GlobalVariable.languageStr; //Google.API.Translate.Language.Spanish;
            //            translated = client.Translate(strfrom, convertin, Google.API.Translate.Language.English);          
            //        }
            //        return translated;     
            //        //return translated;     
            return strfrom;

        }
        string currentsurvey = "";
        string previoussurvey = "";
        //    private void showquestion(survey survey, question question, int index, Table table)
        private void showquestion(questionnaire questionnaire, surveyset surveyset, survey survey, question question, int index, Table table, string urlQuestion)
        {
            TableRow tableRow = new TableRow();
            TableCell tableCell = new TableCell();
            TableRow tableRowsurvey = new TableRow();
            TableCell tableCellsurvey = new TableCell();
            EmailFormat format = new EmailFormat();
            partnerPartnertypeTouchpointQuestionnaire objpptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(HttpContext.Current.Session["accessCode"].ToString()).FirstOrDefault();

            Label label = new Label();
            Label labelsurvey = new Label();
            int divflag = 0;
            //question.question = convertLanguageApi(question.question);

            //abs21022012
            string strQuestion = _translator.Translate(question.id, TranslationType.Question, _currentLanguage);
            var partnerID = (int)HttpContext.Current.Session["partner"];
            var _currentPartner = db.pr_getPartner(partnerID).FirstOrDefault();
            var _enterprise = db.pr_getEnterprise(Generic.Helpers.CurrentInstance.EnterpriseID).FirstOrDefault();
            strQuestion = format.sGetEmailBody(strQuestion, null, _currentPartner, _enterprise, objpptq.partnerTypeTouchpointQuestionnaire1.touchpoint1, objpptq.partnerTypeTouchpointQuestionnaire);
            question.Question = strQuestion;

            if (this.showContentOnly)
            {
                label.Text = question.Question + " " + getRuleByquestion(question.id);
            }
            else
            {
                label.Text = question.Question;

                //add error message to the end of the question
                if (question.id == this.errorquestion)
                {
                    label.Text += this.errorMessage;
                }
            }

            var tags = db.pr_getQuestionDocumentAll().Where(o => o.question == question.id).Select(o => new { o.id, o.description }).ToList();
            if (tags.Count > 0)
            {
                foreach (var item in tags)
                {
                    string url = urlQuestion + "/" + item.id;
                    var newNodeStr = "<a style='color:blue;cursor:pointer;' target='_blank' href=\"" + url + "\">" + item.description + "</a>";
                    Regex rgx = new Regex(@"<document.*?>" + item.description+"</document>");
                    string result = rgx.Replace(label.Text, newNodeStr);
                    label.Text = result;
   
                }
            }

            string cssClass = "";
            string answerCssClass = "";

            if (index % 2 > 0)
            {
                cssClass = this.questionClass;
                answerCssClass = this.answerClass;
            }
            else
            {
                cssClass = this.alternativequestionClass;
                answerCssClass = this.alternativeAnswerClass;
            }
            //        currentsurvey = survey.description;
            currentsurvey = _translator.Translate(surveyset.description, _currentLanguage);
            //show survey 
            tableCellsurvey = new TableCell();
            tableCellsurvey.CssClass = "brownbgtop";
            tableCellsurvey.HorizontalAlign = HorizontalAlign.Center;
            tableCellsurvey.VerticalAlign = VerticalAlign.Top;
            tableCellsurvey.ColumnSpan = 3;
            tableCellsurvey.Width = System.Web.UI.WebControls.Unit.Percentage(80);
            tableCellsurvey.Controls.Add(labelsurvey);
            tableRowsurvey.Controls.Add(tableCellsurvey);
            if (currentsurvey != previoussurvey)
            {
                previoussurvey = currentsurvey;
                labelsurvey.Text = "<b>" + previoussurvey + "</b>";
                table.Controls.Add(tableRowsurvey);
            }
            //show question number
            tableCell.Text = index.ToString();
            tableCell.HorizontalAlign = HorizontalAlign.Right;
            tableCell.VerticalAlign = VerticalAlign.Top;
            tableCell.CssClass = cssClass;
            tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
            tableCell.Style.Add("border-spacing", "0");
            tableCell.Style.Add("padding-right", "5px");
            tableRow.Controls.Add(tableCell);

            if (this.showContentOnly)
            {
                tableCell.Controls.Add(showButton(question.id.ToString(), question.Question, "lookUp"));
                tableCell.Controls.Add(showButton(question.id.ToString(), question.Question, "validation"));
            }

            //show question 
            string responseTypeDescription = db.pr_getResponseType(question.responseType).FirstOrDefault().description;
            //determine is there should be displayed HINT link
            var hits = db.pr_getQuestionResponseNarrativeSelectionListByQuestion(question.id).FirstOrDefault();
            if (hits != null)
            {
                tableCell = new TableCell();
                tableCell.CssClass = cssClass;
                tableCell.HorizontalAlign = HorizontalAlign.Left;
                tableCell.VerticalAlign = VerticalAlign.Top;
                tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(80);
                Table innerTable = new Table();
                innerTable.Width = System.Web.UI.WebControls.Unit.Percentage(100);
                TableRow innerRow = new TableRow();
                innerRow.Width = System.Web.UI.WebControls.Unit.Percentage(100);
                TableCell firstInnerCell = new TableCell();
                firstInnerCell.HorizontalAlign = HorizontalAlign.Left;
                firstInnerCell.VerticalAlign = VerticalAlign.Top;
                firstInnerCell.Width = System.Web.UI.WebControls.Unit.Percentage(80);
                firstInnerCell.Controls.Add(label);
                innerRow.Controls.Add(firstInnerCell);
                firstInnerCell = new TableCell();
                firstInnerCell.HorizontalAlign = HorizontalAlign.Left;
                firstInnerCell.VerticalAlign = VerticalAlign.Top;
                if (new string[] { "textarea", "textComment" }.Contains(responseTypeDescription))
                {
                    firstInnerCell.Width = System.Web.UI.WebControls.Unit.Percentage(10);
                }
                else
                {
                    firstInnerCell.Width = System.Web.UI.WebControls.Unit.Percentage(20);
                }
                //tableCell.Controls.Add(label);
                HyperLink link = new HyperLink();
                link.ID = "narrative-hint-" + question.id.ToString();
                link.CssClass = "btn btn-default narrative-hint";
                //link.NavigateUrl = "#";
                link.Text = "HINT";
                firstInnerCell.Controls.Add(link);
                innerRow.Controls.Add(firstInnerCell);
                if (new string[] { "textarea", "textComment" }.Contains(responseTypeDescription))
                {
                    firstInnerCell = new TableCell();
                    firstInnerCell.HorizontalAlign = HorizontalAlign.Left;
                    firstInnerCell.VerticalAlign = VerticalAlign.Top;
                    firstInnerCell.Width = System.Web.UI.WebControls.Unit.Percentage(10);
                    HtmlImage buttonImage = new HtmlImage();
                    buttonImage.Attributes.Add("class", "editor");
                    buttonImage.Attributes.Add("title", "Click to use editor");
                    buttonImage.Src = "/Contents/images/if_MB__note_81100.png";
                    buttonImage.Style.Add(HtmlTextWriterStyle.Width, "20px;");
                    buttonImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer;");
                    buttonImage.Style.Add(HtmlTextWriterStyle.Display, "none;");
                    firstInnerCell.Controls.Add(buttonImage);
                    innerRow.Controls.Add(firstInnerCell);
                }
                innerTable.Controls.Add(innerRow);
                tableCell.Controls.Add(innerTable);
            }
            else
            {
                if (new string[] { "textarea", "textComment" }.Contains(responseTypeDescription))
                {
                    tableCell = new TableCell();
                    tableCell.CssClass = cssClass;
                    tableCell.HorizontalAlign = HorizontalAlign.Left;
                    tableCell.VerticalAlign = VerticalAlign.Top;
                    tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(80);
                    Table innerTable = new Table();
                    innerTable.Width = System.Web.UI.WebControls.Unit.Percentage(100);
                    TableRow innerRow = new TableRow();
                    innerRow.Width = System.Web.UI.WebControls.Unit.Percentage(100);
                    TableCell firstInnerCell = new TableCell();
                    firstInnerCell.HorizontalAlign = HorizontalAlign.Left;
                    firstInnerCell.VerticalAlign = VerticalAlign.Top;
                    firstInnerCell.Width = System.Web.UI.WebControls.Unit.Percentage(80);
                    firstInnerCell.Controls.Add(label);
                    innerRow.Controls.Add(firstInnerCell);
                    firstInnerCell = new TableCell();
                    firstInnerCell.HorizontalAlign = HorizontalAlign.Right;
                    firstInnerCell.VerticalAlign = VerticalAlign.Top;
                    firstInnerCell.Width = System.Web.UI.WebControls.Unit.Percentage(20);
                    HtmlImage buttonImage = new HtmlImage();
                    buttonImage.Attributes.Add("class", "editor");
                    buttonImage.Attributes.Add("title", "Click to use editor");
                    buttonImage.Src = "/Contents/images/if_MB__note_81100.png";
                    buttonImage.Style.Add(HtmlTextWriterStyle.Width, "20px;");
                    buttonImage.Style.Add(HtmlTextWriterStyle.Cursor, "pointer;");
                    buttonImage.Style.Add(HtmlTextWriterStyle.Display, "none;");
                    //buttonImage.PostBackUrl = "#";
                    firstInnerCell.Controls.Add(buttonImage);
                    innerRow.Controls.Add(firstInnerCell);
                    innerTable.Controls.Add(innerRow);
                    tableCell.Controls.Add(innerTable);
                }
                else
                {
                    tableCell = new TableCell();
                    tableCell.CssClass = cssClass;
                    tableCell.HorizontalAlign = HorizontalAlign.Left;
                    tableCell.VerticalAlign = VerticalAlign.Top;
                    tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(80);
                    tableCell.Controls.Add(label);
                }
                //tableRow.Controls.Add(tableCell);
            }
            tableRow.Controls.Add(tableCell);
            string controlId = "";
            int tagNumber = 0;
            if (question.tag != null && int.TryParse(question.tag, out tagNumber))
            {

                var canBlock = true;
                if (tagNumber != 0)
                {
                    if (objpptq.score.HasValue)
                    {
                        var intFlag = (int)objpptq.score;
                        canBlock = (tagNumber & intFlag) != tagNumber;
                    }
                }
                if (canBlock)
                {
                    tableRow.CssClass = "disabledRow";
                    var pr_getQuestionnaireTagCommentresult = db.pr_getQuestionnaireTagCommentAll(questionnaire.id).FirstOrDefault(o => o.tag == tagNumber);
                    if (pr_getQuestionnaireTagCommentresult != null)
                        tableRow.ToolTip = pr_getQuestionnaireTagCommentresult.comment;
                }
            }

            if (tableRow.CssClass != "disabledRow")
            {
                var calling = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(question.id, objpptq.id).FirstOrDefault();
                if (calling != null && calling.score == 0)
                {
                    tableRow.CssClass = "disabledRow";
                }
            }
            var objresponseByQuestion = db.pr_getResponseByQuestion(question.id).ToList();
            //Add required validation control
            if (question.required > 0 && this.showContentOnly == false)
            {

                if (responseTypeDescription == "dropdown")
                {
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString();
                    addControlValidator(controlId, "rangeValidator", tableCell);
                }
                else if (responseTypeDescription.Contains("checkBox"))
                {
                    int responseCount = objresponseByQuestion.Count;

                    if (responseCount == 1)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_" + objresponseByQuestion[0].id.ToString() + "_checkBox";
                        addControlValidator(controlId, "requiredFieldValidatorForCheckBoxList", tableCell);
                    }
                }
                else if (responseTypeDescription == "textComment" ||
                         responseTypeDescription == "textInteger" ||
                         responseTypeDescription == "textNumber" ||
                         responseTypeDescription == "textarea")
                {
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_text";
                    addControlValidator(controlId, "requiredFieldValidator", tableCell);

                }
                else if (responseTypeDescription == "upload")
                {

                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_upload";

                    //addControlValidatorToUploadText(controlId, "requiredFieldValidator", tableCell);
                    addControlValidator(controlId, "regularexpressionValidator", tableCell);
                    addControlValidator(controlId, "customValidator", tableCell);


                }
                else if (responseTypeDescription == "text/upload")
                {

                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_uploadText";

                    //addControlValidatorToUploadText(controlId, "requiredFieldValidator", tableCell);
                    addControlValidatorToUploadText(controlId, "regularexpressionValidator", tableCell);
                    addControlValidatorToUploadText(controlId, "customValidator", tableCell);

                }
                else
                {
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString();
                    addControlValidator(controlId, "requiredFieldValidator", tableCell);
                }
            }
            //add custom validatoion to file upload control
            else if (question.required == 0 && this.showContentOnly == false)
            {
                if (responseTypeDescription == "upload")
                {

                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_upload";

                    addControlValidator(controlId, "regularexpressionValidator", tableCell);
                    addControlValidator(controlId, "customValidator", tableCell);
                }
                if (responseTypeDescription == "text/upload")
                {

                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_uploadText";

                    addControlValidatorToUploadText(controlId, "regularexpressionValidator", tableCell);
                    addControlValidatorToUploadText(controlId, "customValidator", tableCell);
                }
            }






            //add question answer control
            if (responseTypeDescription == "dropdown")
            {
                tableCell.ColumnSpan = 2;

                //add previous row
                table.Controls.Add(tableRow);

                tableRow = getAnswerRow(survey.id, question.id, responseTypeDescription, answerCssClass, table);
            }
            else if (responseTypeDescription.Contains("text") || question.responseType > 15)
            {
                tableCell.ColumnSpan = 2;

                //add previous row
                table.Controls.Add(tableRow);

                tableRow = getAnswerRow(survey.id, question.id, responseTypeDescription, answerCssClass, table);

                //    //add lookupAndValidation controls
                generateValidationAndLookup(survey.id, question.id, controlId, tableCell);
            }

            //else if (question.responseType.description.Contains("text") || question.responseType.description.Contains("upload"))
            //{
            //    tableCell.ColumnSpan = 2;

            //    //add previous row
            //    table.Controls.Add(tableRow);

            //    tableRow = getAnswerRow(survey.id, question.id, question.responseType.description, answerCssClass, table);

            //    //add lookupAndValidation controls
            //    generateValidationAndLookup(survey.id, question.id, controlId, tableCell);
            //}
            else if (responseTypeDescription == "upload" || responseTypeDescription == "text/upload")
            {
                tableCell.ColumnSpan = 2;

                //add previous row
                table.Controls.Add(tableRow);

                tableRow = getAnswerRow(survey.id, question.id, responseTypeDescription, answerCssClass, table);

                //add lookupAndValidation controls
                generateValidationAndLookup(survey.id, question.id, controlId, tableCell);
            }
            else if (responseTypeDescription == "verticalRadioButton" || responseTypeDescription == "checkBox")
            {
                tableCell.ColumnSpan = 2;

                //add previous row
                table.Controls.Add(tableRow);

                tableRow = getAnswerRow(survey.id, question.id, responseTypeDescription, answerCssClass, table);
            }
            else
            {
                tableCell = getAnswerCell(survey.id, question.id, responseTypeDescription, answerCssClass, table);
                divflag = 1;
                tableRow.Controls.Add(tableCell);
            }

            table.Controls.Add(tableRow);

            //add an empty space between questions
            tableCell = new TableCell();
            tableCell.ColumnSpan = 3;
            TextBox txtbox = new TextBox();
            FileUpload fileupload = new FileUpload();
            fileupload.Attributes.Add("required", "");
            fileupload.Attributes.Add("data-val-filesize", "Maximum file size is 2MB");
            fileupload.Attributes.Add("data-val-filesize-filesize", "2097152");
            fileupload.Attributes.Add("fileextensions", "");
            fileupload.Attributes.Add("data-val-fileextensions-fileextensions", "doc,docx,pdf,jpg,jpeg,gif,bmp,png,xls,xlsx,txt,ppt,pptx");
            fileupload.Attributes.Add("data-val-required", "Required");
            fileupload.Attributes.Add("data-val-fileextensions", "Invalid! File Type valid : doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx");
            fileupload.Attributes.Add("data-val", "true");
            response response = new response();

            partnerPartnertypeTouchpointQuestionnaireQuestionResponse pptqResponse = new partnerPartnertypeTouchpointQuestionnaireQuestionResponse();

            if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
            {
                var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(((int[])HttpContext.Current.Session["partnumber"])[0], (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault(); ;
                try
                {
                    pptqResponse = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(question.id, PartNumberSiteZcodepptq.id).ToList()
                        .Select(x => new partnerPartnertypeTouchpointQuestionnaireQuestionResponse()
                        {
                            id = x.id,
                            question = x.question,
                            response = x.response,
                            comment = x.comment,
                            value = x.value,
                            // score=int.Parse(x.score),
                            partnerPartnerTypeTouchpointQuestionnaire = x.partNumberSiteZcodePPTQ,
                            uploadedFile = x.uploadedFile,
                            uploadedFileType = x.uploadedFileType

                        }).FirstOrDefault();
               
                }
                catch { }
            }
            else if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.COMPANY_LEVEL)
            {
                pptqResponse = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(question.id, objpptq.id).FirstOrDefault();
            }







            surveyForm surveyfrm = new surveyForm();
            string incldComment = "";
            string incldFileUpload = "";
            string inclDueDate = "";
            if (question.commentBoxTxt == "" || question.commentBoxTxt == null)
            {
                incldComment = convertLanguageApi("<span style='font-size:13px'> " + _translator.Translate("Include comments here", _currentLanguage) + ": </span>");
            }
            else
            {
                incldComment = convertLanguageApi("<span style='font-size:13px'>" + _translator.Translate(question.commentBoxTxt, _currentLanguage) + "</span>");
            }
            if (question.calendarMessageTxt == "" || question.calendarMessageTxt == null)
            {
                inclDueDate = convertLanguageApi("<span style='font-size:13px'> " + _translator.Translate("Please enter due date:", _currentLanguage) + " </span>");
            }
            else
            {
                inclDueDate = convertLanguageApi("<span style='font-size:13px'>" + _translator.Translate(question.calendarMessageTxt, _currentLanguage) + "</span>");
            }
            if (question.commentUploadTxt == "" || question.commentUploadTxt == null)
            {
                incldFileUpload = convertLanguageApi("<span style='font-size:13px'>" + _translator.Translate("Upload written procedures here:", _currentLanguage) + " </span>");
            }
            else
            {
                incldFileUpload = convertLanguageApi("<span style='font-size:13px'>" + _translator.Translate(question.commentUploadTxt, _currentLanguage) + "</span>");
            }
            if (responseTypeDescription == "verticalRadioButton")
            {
                HtmlGenericControl divn = new HtmlGenericControl();
                divn.ID = "commentedDiv_" + question.id.ToString();
                //divn.Visible = false;
                divn.Style.Add("display", "none");
                var txtbox1 = new HtmlTextArea();

                //txtbox1.Width = 600;
                txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                divn.InnerHtml = incldComment + " ";
                if (pptqResponse != null && !string.IsNullOrEmpty(pptqResponse.comment) && pptqResponse.response1.description.Contains("????"))
                {
                    divn.Style.Clear();
                    txtbox1.InnerText = convertLanguageApi(pptqResponse.comment);
                    if (question.commentBoxTxt == "" || question.commentBoxTxt == null)
                        divn.InnerHtml = convertLanguageApi("<span style='font-size:13px'> " + _translator.Translate(pptqResponse.response1.id, TranslationType.Response, _currentLanguage).Replace("????", "") + ": </span>") + " ";

                }
                else divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                txtbox1.Attributes.Add("required", "");
                txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                txtbox1.Attributes.Add("data-val", "true");


                divn.Controls.Add(txtbox1);

                tableCell.Controls.AddAt(0, divn);
                addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);
            }
            #region comment required
            if (question.commentRequired == CommentType.YN_COMMENT_REQUIRED_Y)
            {
                if (divflag == 1 && divShowHideFlag == 1)
                {

                    HtmlGenericControl div = new HtmlGenericControl("div");
                    div.ID = "yDiv_" + question.id.ToString();
                    div.Style.Add("display", "block");
                    txtbox = new TextBox();
                    txtbox.Width = 600;
                    txtbox.Text = convertLanguageApi(pptqResponse.comment.ToString());
                    fileupload = new FileUpload(); // textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text"; for referance
                    string uploadedFile = "";// question.getUploadedFile(partner, protocol, touchpoint, questionnaire, survey);
                    fileupload.Attributes.Add("data-val-filesize", _translator.Translate("Maximum file size is 2MB", _currentLanguage));
                    fileupload.Attributes.Add("data-val-filesize-filesize", "2097152");
                    fileupload.Attributes.Add("required", "");
                    fileupload.Attributes.Add("fileextensions", "");
                    fileupload.Attributes.Add("data-val-fileextensions-fileextensions", "doc,docx,pdf,jpg,jpeg,gif,bmp,png,xls,xlsx,txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val-required", "Required");
                    fileupload.Attributes.Add("data-val-fileextensions", _translator.Translate("Invalid! Valid file types", _currentLanguage) + " : doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val", "true");
                    fileupload.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";




                    HtmlGenericControl innerdiv = new HtmlGenericControl();
                    innerdiv.InnerHtml = incldComment + " ";
                    innerdiv.Controls.Add(txtbox);

                    HtmlGenericControl innerdivUpload = new HtmlGenericControl();
                    if (!string.IsNullOrEmpty(uploadedFile))
                    {
                        ////add empty cell
                        //tableCell = new TableCell();
                        //tableCell.Text = "&nbsp;";
                        //tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        //tableCell.Style.Add("border-spacing", "0");
                        //tableCell.Style.Add("padding-right", "5px");
                        //tableRow.Controls.Add(tableCell);


                        //tableCell = new TableCell();
                        //tableCell.ColumnSpan = 2;
                        //tableCell.Text = surveyfrm.convertLanguageApi("File uploaded") + ": " + Path.GetFileName(uploadedFile);
                        //tableRow.Controls.Add(tableCell);
                        //table.Controls.Add(tableRow);

                        //tableRow = new TableRow();
                        //table.Controls.Add(tableRow);

                        innerdivUpload.InnerHtml = "<br/> " + incldFileUpload + "<font color='blue'> " + _translator.Translate("Uploaded file", _currentLanguage) + Path.GetFileName(uploadedFile) + "</font>";

                    }
                    else
                    {


                        innerdivUpload.InnerHtml = "<br/>" + incldFileUpload + " "; //("Upload written procedures here: ");
                    }
                    ////add validators to _fileUploadComment

                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";

                    addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);

                    div.Controls.AddAt(0, innerdiv);
                    div.Controls.AddAt(1, innerdivUpload);

                    innerdivUpload.Controls.Add(fileupload);
                    if (question.commentType == CommentType.YN_WARNING_Y || question.commentType == CommentType.YN_WARNING_N)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        div.Controls.AddAt(0, innerdiv);
                    }
                    else if (question.commentType == CommentType.YN_UPLOAD_Y || question.commentType == CommentType.YN_UPLOAD_N)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }
                    //else if (question.commentType == 2)
                    //{
                    //    div.Controls.AddAt(0, innerdivUpload);
                    //}

                    else
                    {
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }
                    tableCell.Controls.AddAt(0, div);

                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    divn.Style.Add("display", "none");
                    var txtbox1 = new HtmlTextArea();
                    //txtbox1.Width = 600;
                    //fileupload = new FileUpload();
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    divn.InnerHtml = incldComment + " "; //"Include comments here: ";
                    divn.Controls.Add(txtbox1);
                    if (question.commentType != CommentType.YN_WARNING_N 
                        && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                        question.commentType != CommentType.YN_WARNING_Y 
                        && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y 
                        && question.commentType != CommentType.YN_COMMENT_Y_PUBLIC
                        && question.commentType != CommentType.YN_COMMENT_N_PUBLIC)
                        tableCell.Controls.AddAt(1, divn);
                }
                else if (divflag == 1 && divShowHideFlag == 0)
                {
                    //txtbox = new TextBox();
                    //txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_text";
                    //tableCell.Controls.Add(txtbox);
                    //tableCell.Text = "<div Id='yDiv_" + question.id + "' style='display:none' runat='server'>Test Text <br/><br/>Test Text One</div><div Id='nDiv_" + question.id + "' style='display:block'>Test Two <br/><br/>Test Text One</div>";
                    HtmlGenericControl div = new HtmlGenericControl();
                    div.ID = "yDiv_" + question.id.ToString();
                    // div.Visible = false;
                    div.Style.Add("display", "none");
                    txtbox = new TextBox();
                    txtbox.Width = 600;
                    fileupload = new FileUpload();
                    fileupload.Attributes.Add("data-val-filesize", _translator.Translate("Maximum file size is 2MB", _currentLanguage));
                    fileupload.Attributes.Add("data-val-filesize-filesize", "2097152");
                    fileupload.Attributes.Add("required", "");
                    fileupload.Attributes.Add("fileextensions", "");
                    fileupload.Attributes.Add("data-val-fileextensions-fileextensions", "doc,docx,pdf,jpg,jpeg,gif,bmp,png,xls,xlsx,txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    fileupload.Attributes.Add("data-val-fileextensions", _translator.Translate("Invalid! Valid file types", _currentLanguage) + ": doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val", "true");
                    fileupload.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";



                    HtmlGenericControl innerdiv = new HtmlGenericControl();

                    innerdiv.InnerHtml = incldComment + " "; //"Include comments here: ";
                    innerdiv.Controls.Add(txtbox);

                    HtmlGenericControl innerdivUpload = new HtmlGenericControl();
                    innerdivUpload.InnerHtml = "<br/>" + incldFileUpload + " "; //Upload written procedures here: ";

                    ////add validators to _fileUploadComment
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";

                    addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);
                    div.Controls.AddAt(0, innerdiv);
                    div.Controls.AddAt(1, innerdivUpload);

                    innerdivUpload.Controls.Add(fileupload);

                    //controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    //addControlValidatorToFileUpload(controlId, "requiredFieldValidator", innerdiv);
                    //addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    //addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);
                    //div.Controls.AddAt(0, innerdiv);
                    //div.Controls.AddAt(1, innerdivUpload);

                    //div.Controls.AddAt(0, innerdiv);
                    //div.Controls.AddAt(1, innerdivUpload);
                    if (question.commentType == CommentType.YN_WARNING_N || question.commentType == CommentType.YN_WARNING_Y)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        div.Controls.AddAt(0, innerdiv);
                    }
                    else if (question.commentType == CommentType.YN_UPLOAD_N || question.commentType == CommentType.YN_UPLOAD_Y)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }
                    //else if (question.commentType == 2)
                    //{
                    //    div.Controls.AddAt(0, innerdivUpload);
                    //}
                    else
                    {
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }

                    tableCell.Controls.AddAt(0, div);

                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = true;
                    divn.Style.Add("display", "block");
                    var txtbox1 = new HtmlTextArea();
                    //txtbox.Width = 600;
                    txtbox1.InnerText = convertLanguageApi(pptqResponse.comment.ToString());
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                    divn.Controls.Add(txtbox1);
                    if (question.commentType != CommentType.YN_WARNING_N && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                       question.commentType != CommentType.YN_WARNING_Y && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y
                       && question.commentType != CommentType.YN_COMMENT_N_PUBLIC && question.commentType != CommentType.YN_COMMENT_Y_PUBLIC)
                        tableCell.Controls.AddAt(1, divn);
                }
                else if (divflag == 1 && divShowHideFlag == -1)
                {
                    //txtbox = new TextBox();
                    //txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_text";
                    //tableCell.Controls.Add(txtbox);
                    //tableCell.Text = "<div Id='yDiv_" + question.id + "' style='display:none' runat='server'>Test Text <br/><br/>Test Text One</div><div Id='nDiv_" + question.id + "' style='display:none'>Test Two <br/><br/>Test Text One</div>";

                    HtmlGenericControl div = new HtmlGenericControl();
                    div.ID = "yDiv_" + question.id.ToString();
                    //div.Visible = false;
                    div.Style.Add("display", "none");
                    txtbox = new TextBox();
                    txtbox.Width = 600;
                    fileupload = new FileUpload();

                    fileupload.Attributes.Add("data-val-filesize", _translator.Translate("Maximum file size is 2MB", _currentLanguage));
                    fileupload.Attributes.Add("data-val-filesize-filesize", "2097152");
                    fileupload.Attributes.Add("required", "");
                    fileupload.Attributes.Add("fileextensions", "");
                    fileupload.Attributes.Add("data-val-fileextensions-fileextensions", "doc,docx,pdf,jpg,jpeg,gif,bmp,png,xls,xlsx,txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    fileupload.Attributes.Add("data-val-fileextensions", _translator.Translate("Invalid! Valid file types", _currentLanguage) + ": doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val", "true");
                    fileupload.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";



                    HtmlGenericControl innerdiv = new HtmlGenericControl();
                    innerdiv.InnerHtml = incldComment + " ";//"Include comments here: ";
                    innerdiv.Controls.Add(txtbox);

                    HtmlGenericControl innerdivUpload = new HtmlGenericControl();
                    innerdivUpload.InnerHtml = "<br/>" + incldFileUpload + " "; //Upload written procedures here: ";

                    ////add validators to _fileUploadComment
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);
                    div.Controls.AddAt(0, innerdiv);
                    div.Controls.AddAt(1, innerdivUpload);

                    innerdivUpload.Controls.Add(fileupload);

                    //controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    //addControlValidatorToFileUpload(controlId, "requiredFieldValidator", innerdiv);
                    //addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    //addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);
                    //div.Controls.AddAt(0, innerdiv);
                    //div.Controls.AddAt(1, innerdivUpload);

                    //div.Controls.AddAt(0, innerdiv);
                    //div.Controls.AddAt(1, innerdivUpload);
                    if (question.commentType == CommentType.YN_WARNING_N || question.commentType == CommentType.YN_WARNING_Y)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        div.Controls.AddAt(0, innerdiv);
                    }
                    else if (question.commentType == CommentType.YN_UPLOAD_N || question.commentType == CommentType.YN_UPLOAD_Y)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }
                    //else if (question.commentType == 2)
                    //{
                    //    div.Controls.AddAt(0, innerdivUpload);
                    //}

                    else
                    {
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }
                    tableCell.Controls.AddAt(0, div);

                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = true;
                    divn.Style.Add("display", "none");
                    var txtbox1 = new HtmlTextArea();
                    //txtbox.Width = 600;
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    divn.InnerHtml = incldComment + " ";  //"Include comments here: ";
                    divn.Controls.Add(txtbox1);
                    if (question.commentType != CommentType.YN_WARNING_N && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                        question.commentType != CommentType.YN_WARNING_Y && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y
                        && question.commentType != CommentType.YN_COMMENT_Y_PUBLIC && question.commentType != CommentType.YN_COMMENT_N_PUBLIC)
                        tableCell.Controls.AddAt(1, divn);
                }
                else if (divflag == 1 && divShowHideFlag == 2)
                {
                    HtmlGenericControl div = new HtmlGenericControl();
                    div.ID = "yDiv_" + question.id.ToString();
                    //div.Visible = false;
                    div.Style.Add("display", "none");
                    txtbox = new TextBox();
                    txtbox.Width = 600;
                    fileupload = new FileUpload();
                    fileupload.Attributes.Add("data-val-filesize", _translator.Translate("Maximum file size is 2MB", _currentLanguage));
                    fileupload.Attributes.Add("data-val-filesize-filesize", "2097152");
                    fileupload.Attributes.Add("required", "");
                    fileupload.Attributes.Add("fileextensions", "");
                    fileupload.Attributes.Add("data-val-fileextensions-fileextensions", "doc,docx,pdf,jpg,jpeg,gif,bmp,png,xls,xlsx,txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    fileupload.Attributes.Add("data-val-fileextensions", _translator.Translate("Invalid! Valid file types", _currentLanguage) + ": doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx");

                    fileupload.Attributes.Add("data-val", "true");
                    fileupload.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                    //div.InnerHtml = "Comments  ";
                    // div.Controls.Add(txtbox);

                    HtmlGenericControl innerdiv = new HtmlGenericControl();
                    innerdiv.InnerHtml = incldComment + " "; //"Include comments here: ";
                    innerdiv.Controls.Add(txtbox);

                    HtmlGenericControl innerdivUpload = new HtmlGenericControl();
                    innerdivUpload.InnerHtml = "<br/>" + incldFileUpload + " "; //Upload written procedures here: ";

                    ////add validators to _fileUploadComment
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);
                    div.Controls.AddAt(0, innerdiv);
                    div.Controls.AddAt(1, innerdivUpload);
                    innerdivUpload.Controls.Add(fileupload);



                    //div.Controls.AddAt(0, innerdiv);
                    //div.Controls.AddAt(1, innerdivUpload);
                    if (question.commentType == CommentType.YN_WARNING_N || question.commentType == CommentType.YN_WARNING_Y)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        // addControlValidator(controlId, "requiredFieldValidator", tableCell);
                        div.Controls.AddAt(0, innerdiv);
                    }
                    else if (question.commentType == CommentType.YN_UPLOAD_N || question.commentType == CommentType.YN_UPLOAD_Y)
                    {
                        controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                        RequiredFieldValidator validator = new RequiredFieldValidator();
                        validator.ID = question.id + "_R";
                        validator.ControlToValidate = controlId;
                        validator.ErrorMessage = _translator.Translate("Required", _currentLanguage);//" Required";
                        validator.Display = ValidatorDisplay.Dynamic;
                        innerdiv.Controls.Add(validator);
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }
                    //else if (question.commentType == 2)
                    //{
                    //    div.Controls.AddAt(0, innerdivUpload);
                    //}

                    else
                    {
                        div.Controls.AddAt(0, innerdiv);
                        div.Controls.AddAt(1, innerdivUpload);
                    }
                    tableCell.Controls.AddAt(0, div);

                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    divn.Style.Add("display", "none");

                    var txtbox1 = new HtmlTextArea();
                    //fileupload = new FileUpload();
                    //txtbox.Width = 600;
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";

                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                    divn.Controls.Add(txtbox1);
                    //divn.Controls.Add(fileupload);
                    if (question.commentType != CommentType.YN_WARNING_N && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                    question.commentType != CommentType.YN_WARNING_Y && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y
                    && question.commentType != CommentType.YN_COMMENT_Y_PUBLIC && question.commentType != CommentType.YN_COMMENT_N_PUBLIC)
                        tableCell.Controls.AddAt(1, divn);

                    //tableCell.Text = "<div Id='yDiv_" + question.id + "' style='display:none' runat='server' >Test Text <br/><br/>Test Text One</div><div Id='nDiv_" + question.id + "' style='display:none'>Test Two <br/><br/>Test Text One</div>";
                }

                else
                {
                    tableCell.Text = "&nbsp";
                }
            }
            #endregion

            #region YN_COMMENT_REQUIRED_N
            else if (question.commentRequired == CommentType.YN_COMMENT_REQUIRED_N)
            {

                if (question.commentType == CommentType.YN_WARNING_Y)
                {
                    HtmlGenericControl div = new HtmlGenericControl();
                    div.ID = "yDiv_" + question.id.ToString();
                    //div.Visible = false;
                    if (pptqResponse == null || (pptqResponse != null && pptqResponse.response == 75))
                        div.Style.Add("display", "none");
                    div.InnerHtml = incldComment + " ";//"Include comments here: ";
                    tableCell.Controls.AddAt(0, div);
                }
                else if (question.commentType == CommentType.YN_WARNING_N)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    Regex checkCOde = new Regex("\\([A-Z][A-Z]\\)");
                    var responsesList = db.pr_getResponseByQuestion(question.id).ToList();
                    //divn.Visible = false;
                    if (checkCOde.IsMatch(incldComment))
                    {
                        foreach (Match match in checkCOde.Matches(incldComment))
                        {
                            if (responsesList.Any(o => o.zcode.ToLower() == match.Value.ToLower().Replace("(", "").Replace(")", "")))
                            {
                                divn.Attributes["data-code"] = match.Value;
                                incldComment = incldComment.Replace(match.Value, "");
                            }
                        }

                    }
                    if (string.IsNullOrEmpty(divn.Attributes["data-code"]) && db.pr_questionResponseWarningCheck(question.id).FirstOrDefault() != null)
                    {
                        var result = responsesList.FirstOrDefault(o => o.description.Contains("Yes"));
                        if (result != null)
                        {
                            divn.Attributes["data-code"] = result.zcode;
                        }
                    }
                    if (!(pptqResponse != null && pptqResponse.response == 75 || pptqResponse != null && pptqResponse.response1 != null && divn.Attributes["data-code"] != null && divn.Attributes["data-code"].Replace("(", "").Replace(")", "") == pptqResponse.response1.zcode))
                        divn.Style.Add("display", "none");
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                    tableCell.Controls.AddAt(0, divn);

                }
                else if (question.commentType == CommentType.YN_COMMENT_N)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    Regex checkCOde = new Regex("\\([A-Z][A-Z]\\)");
                    var responsesList = db.pr_getResponseByQuestion(question.id).ToList();

                    var txtbox1 = new HtmlTextArea();
                    foreach (Match match in checkCOde.Matches(incldComment))
                    {
                        if (responsesList.Any(o => o.zcode.ToLower() == match.Value.ToLower().Replace("(", "").Replace(")", "")))
                        {
                            divn.Attributes["data-code"] = match.Value;
                            incldComment = incldComment.Replace(match.Value, "");
                        }
                    }
                    if (string.IsNullOrEmpty(divn.Attributes["data-code"]) && db.pr_questionResponseWarningCheck(question.id).FirstOrDefault() != null)
                    {
                        var result = responsesList.FirstOrDefault(o => o.description.Contains("No"));
                        if (result != null)
                        {
                            divn.Attributes["data-code"] = result.zcode;
                        }
                    }
                    if (pptqResponse != null && pptqResponse.response == 75 || pptqResponse != null && pptqResponse.response1 != null && divn.Attributes["data-code"] != null && divn.Attributes["data-code"].Replace("(", "").Replace(")", "") == pptqResponse.response1.zcode)
                    {
                        txtbox1.InnerText = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    // txtbox.Width = 600;
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";

                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);

                }
                else if (question.commentType == CommentType.YN_COMMENT_N_PUBLIC)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    Regex checkCOde = new Regex("\\([A-Z][A-Z]\\)");
                    var responsesList = db.pr_getResponseByQuestion(question.id).ToList();

                    var txtbox1 = new HtmlTextArea();
                    foreach (Match match in checkCOde.Matches(incldComment))
                    {
                        if (responsesList.Any(o => o.zcode.ToLower() == match.Value.ToLower().Replace("(", "").Replace(")", "")))
                        {
                            divn.Attributes["data-code"] = match.Value;
                            incldComment = incldComment.Replace(match.Value, "");
                        }
                    }
                    if (string.IsNullOrEmpty(divn.Attributes["data-code"]) && db.pr_questionResponseWarningCheck(question.id).FirstOrDefault() != null)
                    {
                        var result = responsesList.FirstOrDefault(o => o.description.Contains("No"));
                        if (result != null)
                        {
                            divn.Attributes["data-code"] = result.zcode;
                        }
                    }
                    if (pptqResponse != null && pptqResponse.response == 75 || pptqResponse != null && pptqResponse.response1 != null && divn.Attributes["data-code"] != null && divn.Attributes["data-code"].Replace("(", "").Replace(")", "") == pptqResponse.response1.zcode)
                    {
                        txtbox1.InnerText = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    // txtbox.Width = 600;
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";

                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);

                }
                else if (question.commentType == CommentType.YN_REFERENCE_N)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    Regex checkCOde = new Regex("\\([A-Z][A-Z]\\)");


                    var txtbox1 = new HtmlInputGenericControl("email");
                    //txtbox1.Type = ;
                    if (checkCOde.IsMatch(incldComment))
                    {
                        divn.Attributes["data-code"] = checkCOde.Match(incldComment).Value;
                        incldComment = incldComment.Replace(checkCOde.Match(incldComment).Value, "");
                    }
                    if (pptqResponse != null && pptqResponse.response == 75 || pptqResponse != null && pptqResponse.response1 != null && divn.Attributes["data-code"] != null && divn.Attributes["data-code"].Replace("(", "").Replace(")", "") == pptqResponse.response1.zcode)
                    {
                        txtbox1.Value = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    // txtbox.Width = 600;
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";

                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);

                }
                else if (question.commentType == CommentType.YN_COLORPICKER_N)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    Regex checkCOde = new Regex("\\([A-Z][A-Z]\\)");


                    var txtbox1 = new HtmlInputGenericControl("color");
                    //txtbox1.Type = ;
                    if (checkCOde.IsMatch(incldComment))
                    {
                        divn.Attributes["data-code"] = checkCOde.Match(incldComment).Value;
                        incldComment = incldComment.Replace(checkCOde.Match(incldComment).Value, "");
                    }
                    if (pptqResponse != null && pptqResponse.response == 75 || pptqResponse != null && pptqResponse.response1 != null && divn.Attributes["data-code"] != null && divn.Attributes["data-code"].Replace("(", "").Replace(")", "") == pptqResponse.response1.zcode)
                    {
                        txtbox1.Value = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    // txtbox.Width = 600;
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";

                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);

                }
                else if (question.commentType == CommentType.YN_UPLOAD_Y)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "yDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    divn.Style.Add("display", "none");
                    fileupload = new FileUpload();
                    fileupload.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    fileupload.Attributes.Add("data-val-filesize", _translator.Translate("Maximum file size is 2MB", _currentLanguage));
                    fileupload.Attributes.Add("data-val-filesize-filesize", "2097152");
                    fileupload.Attributes.Add("required", "");
                    fileupload.Attributes.Add("fileextensions", "");
                    fileupload.Attributes.Add("data-val-fileextensions-fileextensions", "doc,docx,pdf,jpg,jpeg,gif,bmp,png,xls,xlsx,txt,ppt,pptx");
                    fileupload.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    fileupload.Attributes.Add("data-val-fileextensions", _translator.Translate("Invalid! Valid file types", _currentLanguage) + ": doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx");

                    fileupload.Attributes.Add("data-val", "true");
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                    txtbox.Width = 600;
                    HtmlGenericControl innerdiv = new HtmlGenericControl();
                    innerdiv.InnerHtml = incldComment + " "; //"Include comments here: ";
                    innerdiv.Controls.Add(txtbox);

                    HtmlGenericControl innerdivUpload = new HtmlGenericControl();
                    innerdivUpload.InnerHtml = "<br/>" + incldFileUpload + " "; //Upload written procedures here: ";

                    ////add validators to _fileUploadComment
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    // addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    //addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);
                    divn.Controls.AddAt(0, innerdiv);
                    divn.Controls.AddAt(1, innerdivUpload);
                    innerdivUpload.Controls.Add(fileupload);
                    HtmlGenericControl validationSpan = new HtmlGenericControl("span");
                    validationSpan.Attributes.Add("data-valmsg-for", controlId);
                    validationSpan.Attributes.Add("data-valmsg-replace", "true");
                    innerdivUpload.Controls.Add(validationSpan);
                    //RequiredFieldValidator validator = new RequiredFieldValidator();
                    // validator.ID = question.id + "_R";
                    //validator.ControlToValidate = controlId;
                    //validator.ErrorMessage = "Required";//" Required";
                    // validator.Display = ValidatorDisplay.Dynamic;
                    // innerdivUpload.Controls.Add(validator);
                    tableCell.Controls.AddAt(0, divn);

                }
                else if (question.commentType == CommentType.YN_COMMENT_Y)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "yDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    var txtbox1 = new HtmlTextArea();
                    if (pptqResponse != null && pptqResponse.response == 74)
                    {
                        txtbox1.InnerText = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";

                    divn.InnerHtml = incldComment + " "; //"Include comments here: ";                   
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);
                }
                else if (question.commentType == CommentType.YN_COMMENT_Y_PUBLIC)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "yDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    var txtbox1 = new HtmlTextArea();
                    if (pptqResponse != null && pptqResponse.response == 74)
                    {
                        txtbox1.InnerText = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";

                    divn.InnerHtml = incldComment + " "; //"Include comments here: ";                   
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);
                }
                else if (question.commentType == CommentType.YN_REFERENCE_Y)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "yDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    var txtbox1 = new HtmlInputGenericControl("email");
                    if (pptqResponse != null && pptqResponse.response == 74)
                    {
                        txtbox1.Value = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";

                    divn.InnerHtml = incldComment + " "; //"Include comments here: ";                   
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);
                }
                else if (question.commentType == CommentType.YN_COLORPICKER_Y)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "yDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    var txtbox1 = new HtmlInputGenericControl("color");
                    if (pptqResponse != null && pptqResponse.response == 74)
                    {
                        txtbox1.Value = pptqResponse.comment;

                    }
                    else
                        divn.Style.Add("display", "none");
                    txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";

                    divn.InnerHtml = incldComment + " "; //"Include comments here: ";                   
                    txtbox1.Attributes.Add("required", "");
                    txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    txtbox1.Attributes.Add("data-val", "true");
                    divn.Controls.Add(txtbox1);

                    tableCell.Controls.AddAt(0, divn);
                    addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);
                }
            }
            if (question.commentType == CommentType.XX_COMMENT_ALL)
            {
                HtmlGenericControl divn = new HtmlGenericControl();
                divn.ID = "AllDiv_" + question.id.ToString();
                var txtbox1 = new HtmlTextArea();
                //divn.Visible = false;
                if (pptqResponse != null)
                {
                    txtbox1.InnerText = pptqResponse.comment;
                }
                else
                    divn.Style.Add("display", "none");
                txtbox1.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                //txtbox.Width = 600;
                divn.InnerHtml = incldComment + " "; //"Include comments here: ";                   
                txtbox1.Attributes.Add("required", "");
                txtbox1.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                txtbox1.Attributes.Add("data-val", "true");
                divn.Controls.Add(txtbox1);

                tableCell.Controls.AddAt(0, divn);
                addControlValidator(txtbox1.ID, "requiredFieldValidator", tableCell);
            }
            #endregion
            if (question.commentType == CommentType.XX_CHECKBOX_X || question.commentType == CommentType.YN_CHECKBOX_N || question.commentType == CommentType.YN_CHECKBOX_Y)
            {
                Generic.Helpers.UIControl.ExtendedChechBoxList list = new Generic.Helpers.UIControl.ExtendedChechBoxList();
                Generic.Helpers.UIControl.MyRadioButtonList rList = new UIControl.MyRadioButtonList();
                bool useRadioList = false;
                list.UseValidation = true;
                list.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_checkboxList";
                rList.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_checkboxList";
                HtmlGenericControl divn = new HtmlGenericControl("div");
                divn.Attributes["class"] = "fullWidth";
                var indexChB = 0;
                var answersVsResponse = question.subCheckBoxChoice.Split(":".ToArray());
                if (answersVsResponse.Length > 2)
                {
                    if (question.commentType == CommentType.XX_CHECKBOX_X)
                    {
                        divn.ID = "nDiv_" + question.id.ToString();
                        divn.Attributes["data-code"] = answersVsResponse[0];
                    }
                    else
                    {
                        divn.ID = (question.commentType == CommentType.YN_CHECKBOX_N ? "n" : "y") + "Div_" + question.id.ToString();
                    }
                    var controlType = answersVsResponse[1].Split("=".ToArray(), StringSplitOptions.RemoveEmptyEntries);
                    if (controlType[0] == "checkbox")
                        foreach (var answer in answersVsResponse[2].Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            var listBoxItem = new ListItem(_translator.Translate(answer, _currentLanguage), indexChB.ToString());
                            if (controlType[1] == "0")
                            {
                                listBoxItem.Attributes.Add("required", "");
                                listBoxItem.Attributes.Add("data-val-required", _translator.Translate("Please select at least one option", _currentLanguage));
                            }
                            else
                            {
                                listBoxItem.Attributes.Add("data-val-selectall", _translator.Translate("Please select all options", _currentLanguage));
                            }
                            listBoxItem.Attributes.Add("data-val", "true");
                            list.Items.Add(listBoxItem);
                            indexChB++;
                        }
                    else
                    {
                        useRadioList = true;
                        foreach (var answer in answersVsResponse[2].Split(";".ToArray(), StringSplitOptions.RemoveEmptyEntries))
                        {
                            var listBoxItem = new ListItem(_translator.Translate(answer, _currentLanguage), indexChB.ToString());
                            if (controlType[1] == "1")
                            {
                                listBoxItem.Attributes.Add("required", "");
                                listBoxItem.Attributes.Add("data-val-required", _translator.Translate("Please select at least one option", _currentLanguage));
                                listBoxItem.Attributes.Add("data-val", "true");
                            }
                            rList.Items.Add(listBoxItem);
                            indexChB++;
                        }
                    }
                }
                else
                    if (answersVsResponse.Length > 1)
                {
                    divn.ID = "nDiv_" + question.id.ToString();
                    divn.Attributes["data-code"] = answersVsResponse[0];
                    foreach (var answer in answersVsResponse[1].Split(";".ToArray()))
                    {
                        var listBoxItem = new ListItem(_translator.Translate(answer, _currentLanguage), indexChB.ToString());
                        listBoxItem.Attributes.Add("required", "");
                        listBoxItem.Attributes.Add("data-val-required", _translator.Translate("Please select at least one option", _currentLanguage));
                        listBoxItem.Attributes.Add("data-val", "true");
                        list.Items.Add(listBoxItem);
                        indexChB++;
                    }
                }
                else
                {
                    divn.ID = (question.commentType == CommentType.YN_CHECKBOX_N ? "n" : "y") + "Div_" + question.id.ToString();
                    foreach (var answer in answersVsResponse[0].Split(";".ToArray()))
                    {
                        var listBoxItem = new ListItem(_translator.Translate(answer, _currentLanguage), indexChB.ToString());
                        listBoxItem.Attributes.Add("required", "");
                        listBoxItem.Attributes.Add("data-val-required", _translator.Translate("Please select at least one option", _currentLanguage));
                        listBoxItem.Attributes.Add("data-val", "true");
                        list.Items.Add(listBoxItem);
                        indexChB++;
                    }

                }
                divn.Style.Add("display", "none");
                divn.Style.Add("margin-left", "30px");
                divn.Controls.Add((useRadioList ? (Control)rList : (Control)list));
                //var cell = new TableCell();
                //cell.Attributes["style"] = "width:5%;border-spacing:0;padding-right:5px;font-size:medium;";
                //cell.Text = "&nbsp;";
                //tableCell.ColumnSpan = 2;
                //tableCell.HorizontalAlign = HorizontalAlign.Left;
                //tableRow.Controls.AddAt(0,cell);
                tableCell.Controls.Add(divn);
            }
            if (question.commentType == CommentType.YN_UPLOADEXPIRY_Y || question.commentType == CommentType.YN_UPLOADEXPIRY_N)
            {
                var divPrefix = question.commentType == CommentType.YN_UPLOADEXPIRY_N ? "n" : "y";
                HtmlGenericControl divn = new HtmlGenericControl();
                divn.ID = divPrefix + "Div_" + question.id.ToString();
                //divn.Visible = false;
                divn.Style.Add("display", "none");

                Table tb = new Table();
                tb.Attributes.Add("style", "width:100%");
                TableRow headerRow = new TableRow();

                var dueHeaderCell = new TableHeaderCell();
                dueHeaderCell.Text = "<span style='font-size:13px;'>" + _translator.Translate("Upload written procedures here", _currentLanguage) + "</span>";
                dueHeaderCell.Attributes.Add("style", "text-align:left;");
                headerRow.Cells.Add(dueHeaderCell);
                TableRow controlRow = new TableRow();
                var duedateCtrlCell = new TableCell();
                duedateCtrlCell.Attributes.Add("style", "vertical-align:middle");

                txtbox = new TextBox();
                txtbox.Width = 100;
                txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_duedate";
                txtbox.Attributes.Add("required", "");
                txtbox.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                txtbox.Attributes.Add("data-val-dpDate", _translator.Translate("Enter valid date value", _currentLanguage));
                txtbox.Attributes.Add("data-val", "true");
                txtbox.Attributes.Add("class", "duedate dpDate");
                txtbox.Attributes.Add("style", "vertical-align:top");
                //divn.InnerHtml = inclDueDate + " ";//"Include comments here: ";


                fileupload = new FileUpload();
                fileupload.Attributes.Add("data-val-filesize", _translator.Translate("Maximum file size is 2MB", _currentLanguage));
                fileupload.Attributes.Add("data-val-filesize-filesize", "2097152");
                fileupload.Attributes.Add("required", "");
                fileupload.Attributes.Add("fileextensions", "");
                fileupload.Attributes.Add("data-val-fileextensions-fileextensions", "doc,docx,pdf,jpg,jpeg,gif,bmp,png,xls,xlsx,txt,ppt,pptx");
                fileupload.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                fileupload.Attributes.Add("data-val-fileextensions", _translator.Translate("Invalid! Valid file types", _currentLanguage) + ": doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx");
                fileupload.Attributes.Add("data-val", "true");
                fileupload.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";

                var alertHeaderCell = new TableHeaderCell();
                alertHeaderCell.Text = inclDueDate;
                alertHeaderCell.Attributes.Add("style", "text-align:left;");
                headerRow.Cells.Add(alertHeaderCell);
                var alertCtrlCell = new TableHeaderCell();
                alertCtrlCell.Attributes.Add("style", "text-align:left;");

                duedateCtrlCell.Controls.Add(fileupload);
                controlRow.Cells.Add(duedateCtrlCell);
                alertCtrlCell.Controls.Add(txtbox);
                controlRow.Cells.Add(alertCtrlCell);

                tb.Rows.Add(headerRow);
                tb.Rows.Add(controlRow);
                divn.Controls.Add(tb);
                tableCell.Controls.AddAt(0, divn);
            }

            if (question.commentType == CommentType.YN_DUEDATE_N || question.commentType == CommentType.YN_DUEDATE_Y || question.commentType == CommentType.YN_ALERT_N || question.commentType == CommentType.YN_ALERT_Y)
            {
                var divPrefix = question.commentType == CommentType.YN_DUEDATE_N || question.commentType == CommentType.YN_ALERT_N || question.commentType == CommentType.YN_UPLOADEXPIRY_N ? "n" : "y";
                HtmlGenericControl divn = new HtmlGenericControl();
                divn.ID = divPrefix + "Div_" + question.id.ToString();
                //divn.Visible = false;
                divn.Style.Add("display", "none");

                Table tb = new Table();
                tb.Attributes.Add("style", "width:100%");
                TableRow headerRow = new TableRow();
                var dueHeaderCell = new TableHeaderCell();
                dueHeaderCell.Text = inclDueDate;
                dueHeaderCell.Attributes.Add("style", "text-align:left;");
                headerRow.Cells.Add(dueHeaderCell);
                TableRow controlRow = new TableRow();
                var duedateCtrlCell = new TableCell();
                duedateCtrlCell.Attributes.Add("style", "vertical-align:middle");

                txtbox = new TextBox();
                txtbox.Width = 100;
                txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_duedate";
                txtbox.Attributes.Add("required", "");
                txtbox.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                txtbox.Attributes.Add("data-val-dpDate", _translator.Translate("Enter valid date value", _currentLanguage));
                txtbox.Attributes.Add("data-val", "true");
                txtbox.Attributes.Add("class", "duedate dpDate");
                txtbox.Attributes.Add("style", "vertical-align:top");
                //divn.InnerHtml = inclDueDate + " ";//"Include comments here: ";
                duedateCtrlCell.Controls.Add(txtbox);
                controlRow.Cells.Add(duedateCtrlCell);
                //divn.Controls.Add(txtbox);
                if (question.commentType == CommentType.YN_ALERT_N || question.commentType == CommentType.YN_ALERT_Y)
                {
                    var textNew = new TextBox();
                    textNew.Width = 500;
                    textNew.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_duedateAlert";
                    textNew.Attributes.Add("required", "");
                    textNew.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    //textNew.Attributes.Add("data-val-date", "Enter valid date value");
                    textNew.Attributes.Add("data-val", "true");
                    textNew.Attributes.Add("style", "vertical-align:top");
                    var alertHeaderCell = new TableHeaderCell();
                    alertHeaderCell.Text = "<span style='font-size:13px;'>" + _translator.Translate("Please enter the alert text", _currentLanguage) + "</span>";
                    alertHeaderCell.Attributes.Add("style", "text-align:left;");
                    headerRow.Cells.Add(alertHeaderCell);
                    var alertCtrlCell = new TableHeaderCell();
                    alertCtrlCell.Attributes.Add("style", "text-align:left;");
                    alertCtrlCell.Controls.Add(textNew);
                    controlRow.Cells.Add(alertCtrlCell);
                    //divn.Controls.Add(textNew);
                }


                tb.Rows.Add(headerRow);
                tb.Rows.Add(controlRow);
                divn.Controls.Add(tb);
                tableCell.Controls.AddAt(0, divn);
            }
            if (question.commentType > 100)
            {
                var value = question.commentType.ToString().Substring(0, 2).ToString();
                var clearCommentType = int.Parse(value);
                var restrict = int.Parse(question.commentType.ToString().Substring(2).ToString());
                if (clearCommentType == CommentType.TEXT_NUMBER_N_X || clearCommentType == CommentType.TEXT_NUMBER_Y_X)
                {
                    var divPrefix = clearCommentType == CommentType.TEXT_NUMBER_N_X ? "n" : "y";
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = divPrefix + "Div_" + question.id.ToString();
                    //divn.Visible = false;
                    //divn.Style.Add("display", "none");

                    Table tb = new Table();
                    tb.Attributes.Add("style", "width:100%");
                    TableRow headerRow = new TableRow();
                    var dueHeaderCell = new TableHeaderCell();
                    //dueHeaderCell.Text = incldComment;

                    //<span class="field-validation-valid" data-valmsg-for="question_21946_14406" data-valmsg-replace="true"></span>
                    dueHeaderCell.Attributes.Add("style", "text-align:left;");
                    headerRow.Cells.Add(dueHeaderCell);
                    TableRow controlRow = new TableRow();
                    var duedateCtrlCell = new TableCell();
                    duedateCtrlCell.Attributes.Add("style", "vertical-align:middle");

                    txtbox = new TextBox();
                    txtbox.Width = 100;
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                    txtbox.Attributes.Add("data-val", "true");
                    txtbox.Attributes.Add("required", "");
                    txtbox.Attributes.Add("min", "0");
                    txtbox.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                    //textBox.Attributes["data-val-required"] = "Required";
                    txtbox.Attributes["data-val-number"] = "Required number";
                    txtbox.Attributes["data-val-length"] = "Required number where length equals {0}";
                    txtbox.Attributes["data-val-length-min"] = restrict.ToString();
                    txtbox.Attributes["data-val-length-max"] = restrict.ToString();
                    txtbox.TextMode = TextBoxMode.Number;
                    //txtbox.MaxLength = restrict;
                    if (clearCommentType == CommentType.TEXT_NUMBER_Y_X)
                    {
                        if (pptqResponse != null && pptqResponse.response == 74 || pptqResponse != null && pptqResponse.response1 != null)
                        {
                            txtbox.Text = pptqResponse.comment;

                        }
                        else
                            divn.Style.Add("display", "none");
                    }
                    else
                    {
                        if (pptqResponse != null && pptqResponse.response == 75 || pptqResponse != null && pptqResponse.response1 != null)
                        {
                            txtbox.Text = pptqResponse.comment;

                        }
                        else
                            divn.Style.Add("display", "none");
                    }
                    //txtbox.Attributes.Add("data-val-dpDate", _translator.Translate("Enter valid date value", _currentLanguage));
                    txtbox.Attributes.Add("data-val", "true");
                    //txtbox.Attributes.Add("class", "duedate dpDate");
                    txtbox.Attributes.Add("style", "vertical-align:top");
                    divn.InnerHtml = incldComment + " <span class='field-validation-valid' data-valmsg-for='" + txtbox.ID + "' data-valmsg-replace='true'></span>";//"Include comments here: ";
                    duedateCtrlCell.Controls.Add(txtbox);
                    controlRow.Cells.Add(duedateCtrlCell);
                    tb.Rows.Add(headerRow);
                    tb.Rows.Add(controlRow);
                    divn.Controls.Add(tb);
                    tableCell.Controls.AddAt(0, divn);
                }
            }



            //}
            divShowHideFlag = 3;
            tableRow = new TableRow();

            tableRow.Controls.Add(tableCell);
            table.Controls.Add(tableRow);
        }

        public TableRow getAnswerRow(int surveyId, int questionId, string responseType, string cssClass, Table table)
        {


            partnerPartnertypeTouchpointQuestionnaire objpptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(HttpContext.Current.Session["accessCode"].ToString()).FirstOrDefault();

            question question = new question();

            question = db.pr_getQuestion(questionId).FirstOrDefault();

            List<response> responseCollection = new List<response>();
            TextBox textBox = new TextBox();
            RadioButton radioButton = new RadioButton();
            RadioButtonList radioButtonList = new RadioButtonList();
            DropDownList dropDownList = new DropDownList();
            ListBox listBox = new ListBox();
            FileUpload fileUpload = new FileUpload();
            CheckBox checkBox = new CheckBox();
            HtmlTextArea textBox1 = new HtmlTextArea();
            TableRow tableRow = new TableRow();
            TableCell tableCell = new TableCell();
            survey survey = new survey();
            response response = new response();
            partnerPartnertypeTouchpointQuestionnaireQuestionResponse pptqResponse = new partnerPartnertypeTouchpointQuestionnaireQuestionResponse();

            surveyForm surveyfrm = new surveyForm();

            if (this.showContentOnly)
            {
                response = new response();
            }
            else
            {
                if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
                {
                    var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(((int[])HttpContext.Current.Session["partnumber"])[0], (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault();
                    try
                    {
                        pptqResponse = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(question.id, PartNumberSiteZcodepptq.id).ToList()
                            .Select(x => new partnerPartnertypeTouchpointQuestionnaireQuestionResponse()
                            {
                                id = x.id,
                                question = x.question,
                                response = x.response,
                                comment = x.comment,
                                value = x.value,
                                // score=int.Parse(x.score),
                                partnerPartnerTypeTouchpointQuestionnaire = x.partNumberSiteZcodePPTQ,
                                uploadedFile = x.uploadedFile,
                                uploadedFileType = x.uploadedFileType

                            }).FirstOrDefault();
                    }
                    catch { }
                }
                else if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.COMPANY_LEVEL)
                {
                    pptqResponse = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(question.id, objpptq.id).FirstOrDefault();
                }
            }

            responseCollection = db.pr_getResponseByQuestion(questionId).ToList();
            if (question.responseType > 15 && question.responseType < 19)
            {
                switch (question.responseType)
                {
                    case 16:
                        textBox = new TextBox();
                        textBox.TextMode = TextBoxMode.Email;
                        textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        textBox.Width = 600;
                        textBox.Attributes["data-val-email"] = "Email required";

                        if (question.required > 0)
                        {
                            textBox.Attributes["required"] = textBox.Attributes["data-val"] = "true";
                            textBox.Attributes["data-val-required"] = "Required";

                        }
                        if (pptqResponse != null && pptqResponse.comment != null && pptqResponse.comment.Length > 0)
                        {

                            textBox.Text = convertLanguageApi(pptqResponse.comment);
                        }

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(textBox);
                        tableCell.Controls.Add(new System.Web.UI.WebControls.Literal() { Text = "<br>" });
                        addControlValidator(textBox.ID, "requiredFieldValidator", tableCell);
                        tableRow.Controls.Add(tableCell);
                        break;
                    case 17:
                        textBox = new TextBox();
                        textBox.TextMode = TextBoxMode.Number;
                        textBox.Attributes["min"] = "0";
                        textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        textBox.Width = 600;
                        if (question.required > 0)
                        {
                            textBox.Attributes["required"] = textBox.Attributes["data-val"] = "true";
                            textBox.Attributes["data-val-required"] = "Required";
                            textBox.Attributes["data-val-number"] = "Required number";
                            textBox.Attributes["data-val-length"] = "Required number where length equals {0}";
                            textBox.Attributes["data-val-length-min"] = question.required.ToString();
                            textBox.Attributes["data-val-length-max"] = question.required.ToString();
                        }
                        if (pptqResponse != null && pptqResponse.comment != null && pptqResponse.comment.Length > 0)
                        {

                            textBox.Text = convertLanguageApi(pptqResponse.comment);
                        }

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(textBox);
                        tableRow.Controls.Add(tableCell);
                        break;
                    case 18:
                        textBox = new TextBox();
                        //textBox.TextMode = TextBoxMode.Number;
                        textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        textBox.Width = 600;
                        if (question.required > 0)
                        {
                            textBox.Attributes["required"] = textBox.Attributes["data-val"] = "true";
                            textBox.Attributes["data-val-required"] = "Required";
                            textBox.Attributes["data-val-length"] = "Required text where length equals {0}";
                            textBox.Attributes["data-val-length-min"] = question.required.ToString();
                            textBox.Attributes["data-val-length-max"] = question.required.ToString();
                        }
                        if (pptqResponse != null && pptqResponse.comment != null && pptqResponse.comment.Length > 0)
                        {

                            textBox.Text = convertLanguageApi(pptqResponse.comment);
                        }

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(textBox);
                        tableRow.Controls.Add(tableCell);
                        break;
                }
            }
            else



                switch (responseType)
                {
                    case "textComment":
                        textBox1 = new HtmlTextArea();
                        textBox1.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        // textBox.Width = 600;
                        if (question.required > 0)
                        {
                            textBox1.Attributes["required"] = textBox.Attributes["data-val"] = "true";
                            textBox1.Attributes["data-val-required"] = "Required";
                        }
                        if (pptqResponse != null)
                        {
                            //textBox.Text = response.description;
                            textBox1.InnerText = convertLanguageApi(pptqResponse.comment);
                        }

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(textBox1);
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "textInteger":
                        textBox = new TextBox();
                        textBox.TextMode = TextBoxMode.Number;
                        textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        textBox.Width = 600;
                        if (question.required > 0)
                        {
                            textBox.Attributes["required"] = textBox.Attributes["data-val"] = "true";
                            textBox.Attributes["data-val-number"] = "Required number";
                            textBox.Attributes["data-val-required"] = "Required";
                        }
                        if (pptqResponse.comment != null && pptqResponse.comment.Length > 0)
                        {
                            //textBox.Text = response.description;
                            textBox.Text = convertLanguageApi(pptqResponse.comment);
                        }

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(textBox);
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "textNumber":
                        textBox = new TextBox();
                        textBox.TextMode = TextBoxMode.Number;
                        textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        textBox.Width = 600;
                        if (question.required > 0)
                        {
                            textBox.Attributes["required"] = textBox.Attributes["data-val"] = "true";
                            textBox.Attributes["data-val-number"] = "Required number";
                            textBox.Attributes["data-val-required"] = "Required";
                        }
                        if (pptqResponse != null && pptqResponse.comment != null && pptqResponse.comment.Length > 0)
                        {

                            textBox.Text = convertLanguageApi(pptqResponse.comment);
                        }

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(textBox);
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "textarea":
                        textBox1 = new HtmlTextArea();

                        textBox1.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        //textBox1.Width = 600;
                        //textBox1.TextMode = TextBoxMode.MultiLine;
                        textBox1.Rows = 3;

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        if (pptqResponse.comment != null && pptqResponse.comment.Length > 0)
                        {
                            //textBox.Text = response.description;
                            textBox1.InnerText = convertLanguageApi(pptqResponse.comment);
                        }

                        tableCell = new TableCell();
                        tableCell.Controls.Add(textBox1);
                        tableCell.ColumnSpan = 2;
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "dropdown":
                        tableRow = new TableRow();
                        table.Controls.Add(tableRow);

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        dropDownList = new DropDownList();
                        dropDownList.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString();
                        if (question.required == 1)
                        {
                            dropDownList.Attributes.Add("data-val", "true");
                            dropDownList.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                        }

                        dropDownList.Width = 300;
                        tableCell = new TableCell();
                        tableCell.HorizontalAlign = HorizontalAlign.Left;
                        string selectval = _translator.Translate("Please select one", _currentLanguage);
                        dropDownList.Items.Add(new ListItem(selectval, ""));
                        dropDownList.Attributes.Add("onChange", "showdropdowndiv(this);");
                        Regex checkCOde = new Regex("\\([A-Z][A-Z]\\)");
                        for (int i = 0; i < responseCollection.Count; i++)
                        {
                            //dropDownList.Items.Add(new ListItem(responseCollection[i].description, responseCollection[i].id.ToString()));
                            var item = new ListItem(_translator.Translate(responseCollection[i].id, TranslationType.Response, _currentLanguage), responseCollection[i].id.ToString());

                            if (checkCOde.IsMatch(item.Text))
                            {
                                item.Attributes["data-code"] = checkCOde.Match(item.Text).Value;
                                item.Text = item.Text.Replace(checkCOde.Match(item.Text).Value, "");
                            }
                            if (!string.IsNullOrEmpty(question.skipLogicJump))
                            {
                                item.Attributes["skipLogicJump"] = "true";
                                item.Attributes["questionId"] = question.id.ToString();
                                item.Attributes["jumpTo"] = question.skipLogicJump;
                                if (pptqResponse != null && pptqResponse.response != null)
                                    item.Attributes["responseCurrent"] = pptqResponse.response.Value.ToString();
                                else item.Attributes["responseCurrent"] = "";
                            }

                            dropDownList.Items.Add(item);
                            if (pptqResponse != null && responseCollection[i].id == pptqResponse.response)
                            {
                                dropDownList.ClearSelection();
                                dropDownList.Items[i + 1].Selected = true;

                            }
                            tableCell.Controls.Add(dropDownList);
                        }

                        tableCell.Controls.Add(dropDownList);
                        tableCell.ColumnSpan = 2;
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "list2list":
                        tableRow = new TableRow();
                        table.Controls.Add(tableRow);

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        listBox = new ListBox();
                        listBox.SelectionMode = ListSelectionMode.Multiple;
                        listBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_list2list";
                        listBox.CssClass = "list2listComponent";
                        if (question.required == 1)
                        {
                            listBox.Attributes.Add("data-val", "true");
                            listBox.Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                        }
                        //listBox.Attributes.Add("multiple", "multiple");
                        //dropDownList.Width = 300;
                        tableCell = new TableCell();
                        tableCell.HorizontalAlign = HorizontalAlign.Left;
                        //string selectval = _translator.Translate("Please select one", _currentLanguage);
                        //dropDownList.Items.Add(new ListItem(selectval, ""));
                        //dropDownList.Attributes.Add("onChange", "showdropdowndiv(this);");
                        //Regex checkCOde = new Regex("\\([A-Z][A-Z]\\)");
                        for (int i = 0; i < responseCollection.Count; i++)
                        {

                            //dropDownList.Items.Add(new ListItem(responseCollection[i].description, responseCollection[i].id.ToString()));
                            //_translator.Translate(responseCollection[i].id, TranslationType.Response, _currentLanguage), responseCollection[i].id.ToString()
                            var splittedDescription = responseCollection[i].description.Split("|".ToCharArray());
                            var item = new ListItem(splittedDescription[0], splittedDescription[0]);
                            item.Attributes.Add("title", splittedDescription[1]);
                            //if (checkCOde.IsMatch(item.Text))
                            //{
                            //    item.Attributes["data-code"] = checkCOde.Match(item.Text).Value;
                            //    item.Text = item.Text.Replace(checkCOde.Match(item.Text).Value, "");
                            //}
                            if (pptqResponse != null && (pptqResponse.comment ?? "").Contains(splittedDescription[0]))
                            {
                                //listBox.ClearSelection();
                                item.Selected = true;

                            }
                            listBox.Items.Add(item);

                            //tableCell.Controls.Add(dropDownList);
                        }

                        tableCell.Controls.Add(listBox);
                        tableCell.ColumnSpan = 2;
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "verticalRadioButton":
                        tableRow = new TableRow();
                        table.Controls.Add(tableRow);

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableCell.Style.Add("font-size", "medium");
                        tableRow.Controls.Add(tableCell);

                        //  tableCell = new TableCell();
                        // var tableRadio = new Table();
                        // tableRadio.Font.Size = 10;
                        // tableRadio.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString();
                        radioButtonList = new Generic.Helpers.UIControl.MyRadioButtonList();//new Generic.Helpers.UIControl.MyRadioButtonList();
                        radioButtonList.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString();
                        (radioButtonList as Generic.Helpers.UIControl.MyRadioButtonList).UseValidation = true;
                        radioButtonList.Font.Size = 10;
                        //radioButtonList.Attributes.Add("onchange", "javascript:showdiv();");
                        radioButtonList.Attributes.Add("onClick", "showdivRadioList(this);removevalidation(this.id);showDivByCode(this) ");
                        radioButtonList.Attributes.Add("onChange", "showdropdowndiv(this,radioListShowIfNeeded)");

                        radioButtonList.RepeatDirection = RepeatDirection.Vertical;
                        tableCell = new TableCell();
                        tableCell.HorizontalAlign = HorizontalAlign.Left;
                        for (int i = 0; i < responseCollection.Count; i++)
                        {
                            //var row = new TableRow();

                            //tableRadio.Rows.Add(new 
                            //radioButtonList.Items.Add(new ListItem(responseCollection[i].description, responseCollection[i].id.ToString()));
                            var optionText = _translator.Translate(responseCollection[i].id, TranslationType.Response, _currentLanguage);
                            var hasAdditionalCommentBox = optionText.Contains("????");
                            optionText = optionText.Replace("????", "");
                            Regex reg = new Regex("\\([A-Z][A-Z]\\)");
                            var match = reg.Match(optionText);
                            if (match.Success)
                            {
                                optionText = optionText.Replace(match.Value, "");

                            }
                            radioButtonList.Items.Add(new ListItem(optionText, responseCollection[i].id.ToString()));
                            if (match.Success)
                                radioButtonList.Items[i].Attributes.Add("data-code", match.Value);
                            radioButtonList.Items[i].Attributes["data-commented"] = hasAdditionalCommentBox.ToString();
                            if (question.required == 1)
                            {
                                radioButtonList.Items[i].Attributes["data-val"] = "true";
                                radioButtonList.Items[i].Attributes["data-val-required"] = _translator.Translate("Required", _currentLanguage);
                                radioButtonList.Items[i].Attributes["required"] = "";

                            } 
                            if (pptqResponse != null && responseCollection[i].id == pptqResponse.response)
                            {
                                radioButtonList.ClearSelection();
                                radioButtonList.Items[i].Selected = true;
                                radioButtonList.Items[i].Attributes["checked"] = "";
                            }
                            if (!string.IsNullOrEmpty(question.skipLogicJump))
                            {
                                radioButtonList.Items[i].Attributes["skipLogicJump"] = "true";
                                radioButtonList.Items[i].Attributes["questionId"] = question.id.ToString();
                                radioButtonList.Items[i].Attributes["jumpTo"] = question.skipLogicJump;
                                if (pptqResponse != null && pptqResponse.response != null)
                                    radioButtonList.Items[i].Attributes["responseCurrent"] = pptqResponse.response.Value.ToString();
                                else radioButtonList.Items[i].Attributes["responseCurrent"] = "";
                            }
                        }
                        //tableCell.Controls.Add(radioButtonList);


                        tableCell.Controls.Add(radioButtonList);
                        tableCell.ColumnSpan = 2;
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "upload":
                        if (showContentOnly == false)
                        {

                            //db.pr_getPartnerPartnertypeTouchpointQuestionnaireQuestionResponseByPPTQ(1);
                            string uploadedFile = "";// question.getUploadedFile(partner, protocol, touchpoint, questionnaire, survey);

                            if (!string.IsNullOrEmpty(uploadedFile))
                            {
                                //add empty cell
                                tableCell = new TableCell();
                                tableCell.Text = "&nbsp;";
                                tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                                tableCell.Style.Add("border-spacing", "0");
                                tableCell.Style.Add("padding-right", "5px");
                                tableRow.Controls.Add(tableCell);


                                tableCell = new TableCell();
                                tableCell.ColumnSpan = 2;
                                tableCell.Text = _translator.Translate("File uploaded", _currentLanguage) + ": " + Path.GetFileName(uploadedFile);
                                tableRow.Controls.Add(tableCell);
                                table.Controls.Add(tableRow);

                                tableRow = new TableRow();
                                table.Controls.Add(tableRow);
                            }
                        }

                        fileUpload = new FileUpload();
                        fileUpload.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_upload";

                        fileUpload.Width = 650;
                        fileUpload.Attributes.Add("size", "40");

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(fileUpload);
                        //tableCell = new TableCell();
                        Label labelErrormsg = new Label();
                        //labelErrormsg.Text = surveyfrm.convertLanguageApi("File size exceeded than limit 4 MB, Please upload smaller file.");
                        labelErrormsg.Style.Add("color", "red");
                        labelErrormsg.Style.Add("font-size", "12px");
                        tableCell.Controls.Add(labelErrormsg);
                        tableRow.Controls.Add(tableCell);


                        //labelErrormsg.ForeColor = System.Drawing.Color.Red;
                        // tableCell = new TableCell();

                        // tableRow.Controls.Add(tableCell);
                        break;
                    case "checkBox":
                        tableRow = new TableRow();
                        table.Controls.Add(tableRow);

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.HorizontalAlign = HorizontalAlign.Left;



                        List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> pptqResponses = null;

                        //  List<response> responses = null;
                        if (showContentOnly == false)
                        {
                            if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
                            {
                                try
                                {
                                    var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(((int[])HttpContext.Current.Session["partnumber"])[0], (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault(); ;

                                    pptqResponses = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(question.id, PartNumberSiteZcodepptq.id).ToList()
                                        .Select(x => new partnerPartnertypeTouchpointQuestionnaireQuestionResponse()
                                        {
                                            id = x.id,
                                            question = x.question,
                                            response = x.response,
                                            comment = x.comment,
                                            value = x.value,
                                            // score=int.Parse(x.score),
                                            partnerPartnerTypeTouchpointQuestionnaire = x.partNumberSiteZcodePPTQ,
                                            uploadedFile = x.uploadedFile,
                                            uploadedFileType = x.uploadedFileType

                                        }).ToList();
                                }
                                catch { }
                            }
                            else if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.COMPANY_LEVEL)
                            {
                                pptqResponses = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(question.id, objpptq.id).ToList();
                            }
                        }
                        //hidden field
                        HiddenField hiddenField = new HiddenField();

                        for (int i = 0; i < responseCollection.Count; i++)
                        {
                            checkBox = new CheckBox();
                            checkBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_" + responseCollection[i].id.ToString() + "_checkBox";
                            checkBox.Text = _translator.Translate(responseCollection[i].id, TranslationType.Response, _currentLanguage);// convertLanguageApi(responseCollection[i].description);

                            if (showContentOnly == false)
                            {
                                foreach (var item in pptqResponses)
                                {
                                    if (item.response == responseCollection[i].id)
                                    {
                                        checkBox.Checked = true;
                                    }
                                }
                            }

                            tableCell.Controls.Add(checkBox);


                        }
                        //render a new hiddenField
                        hiddenField = new HiddenField();
                        hiddenField.ID = "questionHiddenField_" + questionId.ToString() + "_" + surveyId.ToString();
                        hiddenField.Value = questionId.ToString();
                        tableCell.Controls.Add(hiddenField);

                        tableCell.ColumnSpan = 2;
                        tableRow.Controls.Add(tableCell);
                        break;
                    case "text/upload":
                        textBox = new TextBox();
                        textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                        textBox.Width = 600;

                        if (response.description != null && response.description.Length > 0)
                        {
                            textBox.Text = convertLanguageApi(response.description);
                        }

                        fileUpload = new FileUpload();
                        fileUpload.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_uploadText";
                        fileUpload.Width = 650;
                        fileUpload.Attributes.Add("size", "40");

                        //add empty cell
                        tableCell = new TableCell();
                        tableCell.Text = "&nbsp;";
                        tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(5);
                        tableCell.Style.Add("border-spacing", "0");
                        tableCell.Style.Add("padding-right", "5px");
                        tableRow.Controls.Add(tableCell);

                        tableCell = new TableCell();
                        tableCell.ColumnSpan = 2;
                        tableCell.Controls.Add(textBox);

                        Literal literal = new Literal();
                        literal.Text = "<p>" + _translator.Translate("Please upload the file here", _currentLanguage) + "</p>";
                        if (showContentOnly == false)
                        {
                            string uploadedFile = "";// question.getUploadedFile(partner, protocol, touchpoint, questionnaire, survey);


                            if (!string.IsNullOrEmpty(uploadedFile))
                            {

                                literal.Text = "<p>" + _translator.Translate("File uploaded", _currentLanguage) + ": " + Path.GetFileName(uploadedFile) + "</p>";
                            }


                        }
                        tableCell.Controls.Add(literal);
                        tableCell.Controls.Add(fileUpload);
                        tableRow.Controls.Add(tableCell);
                        break;
                    default:
                        break;
                }

            return tableRow;
        }

        private TableCell getAnswerCell(int surveyId, int questionId, string responseType, string cssClass, Table table)
        {
            question question = new question();
            question = db.pr_getQuestion(questionId).FirstOrDefault();
            List<response> responseCollection = new List<response>();
            TextBox textBox = new TextBox();
            RadioButton radioButton = new RadioButton();
            RadioButtonList radioButtonList = new RadioButtonList();
            DropDownList dropDownList = new DropDownList();
            TableCell tableCell = new TableCell();

            response response = new response();
            survey survey = new survey();
            partnerPartnertypeTouchpointQuestionnaire objpptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(HttpContext.Current.Session["accessCode"].ToString()).FirstOrDefault();
            partnerPartnertypeTouchpointQuestionnaireQuestionResponse pptqResponse = new partnerPartnertypeTouchpointQuestionnaireQuestionResponse();

            if (this.showContentOnly)
            {
                response = new response();
            }
            else
            {

                if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
                {
                    var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ(((int[])HttpContext.Current.Session["partnumber"])[0],
                        (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault();
                    try
                    {
                        pptqResponse = db.pr_getPartnumberSiteZcodePPTQQuestionResponseByQuestionAndPartnumberSite(question.id, PartNumberSiteZcodepptq.id).ToList()
                            .Select(x => new partnerPartnertypeTouchpointQuestionnaireQuestionResponse()
                            {
                                id = x.id,
                                question = x.question,
                                response = x.response,
                                comment = x.comment,
                                value = x.value,
                                // score=int.Parse(x.score),
                                partnerPartnerTypeTouchpointQuestionnaire = x.partNumberSiteZcodePPTQ,
                                uploadedFile = x.uploadedFile,
                                uploadedFileType = x.uploadedFileType

                            }).FirstOrDefault();
                    }
                    catch { }
                }
                else if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.COMPANY_LEVEL)
                {
                    pptqResponse = db.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(question.id, objpptq.id).FirstOrDefault();
                }
            }
            responseCollection = db.pr_getResponseByQuestion(questionId).ToList();
            pr_getQuestionBlockedResponseByPPTQ_Result blockedResponse = null;


            if (question.tag != null && question.tag.ToLower() == "0")
            {
                blockedResponse = db.pr_getQuestionBlockedResponseByPPTQ(objpptq.id).FirstOrDefault(o => o.question == question.id);
            }
            switch (responseType)
            {
                case "radioButton":
                    radioButtonList = new Generic.Helpers.UIControl.MyRadioButtonList();
                    radioButtonList.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString();
                    radioButtonList.Attributes.Add("onClick", "showdivnew(this);removevalidation(this.id) ");
                   

                    radioButtonList.RepeatDirection = RepeatDirection.Horizontal;
                    tableCell = new TableCell();

                    tableCell.HorizontalAlign = HorizontalAlign.Right;
                    tableCell.CssClass = cssClass + " yes-no-answers";
                    tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(15);
                    for (int i = 0; i < responseCollection.Count; i++)
                    {
                        radioButtonList.Items.Add(new ListItem(_translator.Translate(responseCollection[i].id, TranslationType.Response, _currentLanguage), responseCollection[i].id.ToString()));
                        if (question.required == 1)
                        {
                            radioButtonList.Items[i].Attributes.Add("data-val", "true");
                            radioButtonList.Items[i].Attributes.Add("data-val-required", _translator.Translate("Required", _currentLanguage));
                            radioButtonList.Items[i].Attributes.Add("required", "");
                        }

                        if ((pptqResponse != null && responseCollection[i].id == pptqResponse.response) || (blockedResponse != null && blockedResponse.response == responseCollection[i].id))
                        {
                            if (pptqResponse != null)
                                divShowHideFlag = pptqResponse.response == 74 ? 1 : 0;
                            if (blockedResponse != null)
                                divShowHideFlag = blockedResponse.response == 74 ? 1 : 0;
                            radioButtonList.Items[i].Selected = true;
                            radioButtonList.Items[i].Attributes.Add("checked", "true");
                        }
                        if (!string.IsNullOrEmpty(question.skipLogicJump))
                        {
                            radioButtonList.Items[i].Attributes["skipLogicJump"] = "true";
                            radioButtonList.Items[i].Attributes["questionId"] = question.id.ToString();
                            radioButtonList.Items[i].Attributes["jumpTo"] = question.skipLogicJump;
                            if (pptqResponse != null && pptqResponse.response != null)
                                radioButtonList.Items[i].Attributes["responseCurrent"] = pptqResponse.response.Value.ToString();
                            else radioButtonList.Items[i].Attributes["responseCurrent"] = "";
                        }
                        tableCell.Controls.Add(radioButtonList);
                    }

                  

                    if (divShowHideFlag == 3)
                    {
                        divShowHideFlag = 2;
                    }
                    break;
                default:
                    break;
            }

            return tableCell;
        }

        private void addControlValidator(string controlId, string validatorType, TableCell tableCell)
        {
            string requiredtext = " <span class=\"field-validation-valid\" data-valmsg-for=\"" + controlId + "\" data-valmsg-replace=\"true\"></span>";
            //  string requiredtext = " " + convertLanguageApi("Required");
            if (validatorType == "requiredFieldValidator")
            {
                RequiredFieldValidator validator = new RequiredFieldValidator();
                validator.ControlToValidate = controlId;
                validator.ErrorMessage = requiredtext;//" Required";
                validator.Display = ValidatorDisplay.Dynamic;
                tableCell.Controls.Add(validator);
                //        tableCell.FindControl("controlId")
            }
            else if (validatorType == "rangeValidator")
            {
                RangeValidator validator = new RangeValidator();
                validator.ControlToValidate = controlId;
                validator.ErrorMessage = requiredtext;// " Required";
                validator.MinimumValue = "1";
                validator.MaximumValue = "9999";
                validator.Display = ValidatorDisplay.Dynamic;
                tableCell.Controls.Add(validator);
            }
            else if (validatorType == "requiredFieldValidatorForCheckBoxList")
            {
                //RequiredFieldValidatorForCheckBox validator = new RequiredFieldValidatorForCheckBox();
                //validator.ID = "required" + controlId;
                //validator.ControlToValidate = controlId;
                //validator.ErrorMessage = requiredtext;// " Required";
                //validator.Display = ValidatorDisplay.Dynamic;
                //tableCell.Controls.Add(validator);
            }
            else if (validatorType == "customValidator")
            {
                CustomValidator customvalidator = new CustomValidator();
                customvalidator.ID = controlId + "_customvalidator";
                customvalidator.ControlToValidate = controlId;
                customvalidator.ErrorMessage = " File size exceeded than limit 4 mb, please upload smaller file.";
                customvalidator.Display = ValidatorDisplay.Dynamic;
                customvalidator.ClientValidationFunction = "ValidateFileForUpload";
                // customvalidator.ClientValidationFunction = ServerVal(controlId, Request.Files[1].ContentLength);
                // customvalidator.ServerValidate += new ServerValidateEventHandler(customvalidator_ServerValidate);
                tableCell.Controls.Add(customvalidator);
            }

            else if (validatorType == "regularexpressionValidator")
            {
                RegularExpressionValidator validator = new RegularExpressionValidator();
                validator.ID = controlId + "_regExp";
                validator.ControlToValidate = controlId;
                validator.ValidationExpression = "^.+(.doc|.DOC|.docx|.DOCX|.pdf|.PDF|.jpg|.JPG|.gif|.GIF|.jpeg|.JPEG|.png|.PNG|.bmp|.BMP|.xls|.XLS|.xlsx|.XSLX|.txt|.ppt|pptx)$";
                validator.ErrorMessage = " Invalid! File Type valid : doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx ";
                validator.Display = ValidatorDisplay.Dynamic;
                tableCell.Controls.Add(validator);
            }

        }

        private void addControlValidatorToFileUpload(string controlId, string validatorType, HtmlGenericControl div)
        {
            //string requiredtext = " " + convertLanguageApi("Required");
            //if (validatorType == "requiredFieldValidator")
            //{
            //    RequiredFieldValidator validator = new RequiredFieldValidator();
            //    validator.ID = controlId + "_requiredFileUpload";
            //    validator.ControlToValidate = controlId;
            //    validator.ErrorMessage = requiredtext;//" Required";
            //    validator.Display = ValidatorDisplay.Dynamic;
            //    div.Controls.Add(validator);
            //}


            if (validatorType == "customValidator")
            {
                CustomValidator customvalidator = new CustomValidator();
                customvalidator.ID = controlId + "_customvalidatorFileUpload";
                customvalidator.ControlToValidate = controlId;
                customvalidator.ErrorMessage = "File size exceeded than limit 4 mb, please upload smaller file.";
                customvalidator.Display = ValidatorDisplay.Dynamic;
                customvalidator.ClientValidationFunction = "ValidateFileForFileUploadComment";
                // customvalidator.ClientValidationFunction = ServerVal(controlId, Request.Files[1].ContentLength);
                // customvalidator.ServerValidate += new ServerValidateEventHandler(customvalidator_ServerValidate);
                div.Controls.Add(customvalidator);
            }

            else if (validatorType == "regularexpressionValidator")
            {
                RegularExpressionValidator validator = new RegularExpressionValidator();
                validator.ID = controlId + "_regExpFileUpload";
                validator.ControlToValidate = controlId;
                validator.ValidationExpression = "^.+(.doc|.DOC|.docx|.DOCX|.pdf|.PDF|.jpg|.JPG|.gif|.GIF|.jpeg|.JPEG|.png|.PNG|.bmp|.BMP|.xls|.XLS|.xlsx|.XSLX|.txt|.ppt|pptx)$";
                validator.ErrorMessage = " Invalid! File Type valid : doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx ";
                validator.Display = ValidatorDisplay.Dynamic;
                div.Controls.Add(validator);
            }

        }


        private void addControlValidatorToUploadText(string controlId, string validatorType, TableCell tableCell)
        {
            //string requiredtext = " " + convertLanguageApi("Required");
            //if (validatorType == "requiredFieldValidator")
            //{
            //    RequiredFieldValidator validator = new RequiredFieldValidator();
            //    validator.ID = controlId + "_requiredUploadText";
            //    validator.ControlToValidate = controlId;
            //    validator.ErrorMessage = requiredtext;//" Required";
            //    validator.Display = ValidatorDisplay.Dynamic;
            //    tableCell.Controls.Add(validator);
            //}


            if (validatorType == "customValidator")
            {
                CustomValidator customvalidator = new CustomValidator();
                customvalidator.ID = controlId + "_customvalidatorUploadText";
                customvalidator.ControlToValidate = controlId;
                customvalidator.ErrorMessage = " File size exceeded than limit 4 mb, please upload smaller file.";
                customvalidator.Display = ValidatorDisplay.Dynamic;
                customvalidator.ClientValidationFunction = "ValidateFileForUploadText";
                // customvalidator.ClientValidationFunction = ServerVal(controlId, Request.Files[1].ContentLength);
                // customvalidator.ServerValidate += new ServerValidateEventHandler(customvalidator_ServerValidate);
                tableCell.Controls.Add(customvalidator);
            }

            else if (validatorType == "regularexpressionValidator")
            {
                RegularExpressionValidator validator = new RegularExpressionValidator();
                validator.ID = controlId + "_regExpUploadText";
                validator.ControlToValidate = controlId;
                validator.ValidationExpression = "^.+(.doc|.DOC|.docx|.DOCX|.pdf|.PDF|.jpg|.JPG|.gif|.GIF|.jpeg|.JPEG|.png|.PNG|.bmp|.BMP|.xls|.XLS|.xlsx|.XSLX|.txt|.ppt|pptx)$";
                validator.ErrorMessage = " Invalid! File Type valid : doc,docx, pdf, jpg, jpeg, gif, bmp, png, xls, xlsx, txt,ppt,pptx ";
                validator.Display = ValidatorDisplay.Dynamic;
                tableCell.Controls.Add(validator);
            }

        }


        void validator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;
        }

        private void generateValidationAndLookup(int surveyId, int questionId, string targetControlId, TableCell cell)
        {
            //question question = new question(new Id(questionId));
            //List<questionRule> ruleCollection = question.getquestionRuleCollection();
            //LinkButton linkButton = null;
            //RegularExpressionValidator regValidator = null;
            //ValidateAndLookUpControl validationControl = new ValidateAndLookUpControl();
            //string controlId = "";

            //foreach (questionRule rule in ruleCollection)
            //{
            //    controlId = rule.description + questionId.ToString();
            //    switch (rule.description)
            //    {
            //        case "lookUpZipCode":
            //            validationControl = new ValidateAndLookUpControl(controlId, rule.description);
            //            linkButton = validationControl.lookUp(question);
            //            linkButton.ID = rule.description + questionId.ToString();
            //            cell.Controls.Add(linkButton);
            //            break;
            //        case "lookUpNaicsCode":
            //            validationControl = new ValidateAndLookUpControl(controlId, rule.description);
            //            linkButton = validationControl.lookUp(question);
            //            linkButton.ID = rule.description + questionId.ToString();
            //            linkButton.PostBackUrl += "&survey=" + surveyId.ToString();
            //            cell.Controls.Add(linkButton);
            //            break;
            //        case "lookUpDunsNumber":
            //            validationControl = new ValidateAndLookUpControl(controlId, rule.description);
            //            linkButton = validationControl.lookUp(question);
            //            linkButton.ID = rule.description + questionId.ToString();
            //            cell.Controls.Add(linkButton);
            //            break;
            //        case "validateFederalId":
            //            validationControl = new ValidateAndLookUpControl(controlId, targetControlId, rule.description);
            //            regValidator = validationControl.validateData();
            //            regValidator.ID = rule.description + questionId.ToString();
            //            cell.Controls.Add(regValidator);
            //            break;
            //        case "validateDunsNumber":
            //            validationControl = new ValidateAndLookUpControl(controlId, targetControlId, rule.description);
            //            regValidator = validationControl.validateData();
            //            regValidator.ID = rule.description + questionId.ToString();
            //            cell.Controls.Add(regValidator);
            //            break;
            //        case "validateHubZone":
            //            break;
            //        case "validateSDB":
            //            break;
            //        case "validateEmail":
            //            validationControl = new ValidateAndLookUpControl(controlId, targetControlId, rule.description);
            //            regValidator = validationControl.validateData();
            //            regValidator.ID = rule.description + questionId.ToString();
            //            cell.Controls.Add(regValidator);
            //            break;
            //        case "validateZipCode":
            //            validationControl = new ValidateAndLookUpControl(controlId, targetControlId, rule.description);
            //            regValidator = validationControl.validateData();
            //            regValidator.ID = rule.description + questionId.ToString();
            //            cell.Controls.Add(regValidator);
            //            break;
            //        case "validateNaicsCode":
            //            validationControl = new ValidateAndLookUpControl(controlId, targetControlId, rule.description);
            //            regValidator = validationControl.validateData();
            //            regValidator.ID = rule.description + questionId.ToString();
            //            cell.Controls.Add(regValidator);
            //            break;
            //        default:
            //            break;
            //    }
            //}
        }

        private LinkButton showButton(string questionId, string description, string type)
        {
            LinkButton button = new LinkButton();
            button.ID = "button" + type + "_" + questionId;
            button.Text = type + " ";

            generateModal(button.ID, questionId, description, type);

            return button;
        }

        private void generateModal(string targetControlId, string questionId, string description, string type)
        {
            Panel panel = new Panel();
            panel.ID = "panelModal" + type + "_" + questionId;
            panel.Attributes.Add("display", "none");
            panel.CssClass = "modalPopupquestion";
            this.panelModal.Controls.Add(panel);

            AjaxControlToolkit.ModalPopupExtender modal = new AjaxControlToolkit.ModalPopupExtender();
            modal.ID = "modal" + type + "_" + questionId;
            modal.TargetControlID = targetControlId;
            modal.PopupControlID = panel.ID;
            modal.BackgroundCssClass = "modalBackground";
            this.panelModal.Controls.Add(modal);

            generatePanel(questionId, description, panel, type, modal);
        }

        private void generatePanel(string questionId, string description, Panel modalPanel, string type, AjaxControlToolkit.ModalPopupExtender modal)
        {
            Table table = new Table();
            TableRow row = new TableRow();
            TableCell cell = new TableCell();
            ListBox listBox = null;
            ImageButton button = null;

            table.Width = Unit.Percentage(100);
            modalPanel.Controls.Add(table);

            //add question description
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "<p style='padding-left: 45px; padding-right: 45px; font-size: 12px;'>" + description + "</p>";
            row.Controls.Add(cell);
            table.Controls.Add(row);

            //add available rules listBox
            row = new TableRow();
            cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Center;
            listBox = generateListBoxAvailable(questionId, type);
            cell.Controls.Add(listBox);
            row.Controls.Add(cell);
            table.Controls.Add(row);

            //empty row
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "<p></p>";
            row.Controls.Add(cell);
            table.Controls.Add(row);

            //add buttons
            row = new TableRow();
            cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Center;

            //add addButton
            button = generateAddButton(questionId, type);
            cell.Controls.Add(button);

            //add removeButton
            button = new ImageButton();
            button = generateRemoveButton(questionId, type);
            cell.Controls.Add(button);

            //add cancelButton
            button = new ImageButton();
            button = generateCancelButton(questionId, type);

            //set cancelButton as an cancel button on the modal popup
            modal.CancelControlID = button.ID;
            cell.Controls.Add(button);

            row.Controls.Add(cell);
            table.Controls.Add(row);

            //empty row
            row = new TableRow();
            cell = new TableCell();
            cell.Text = "<p></p>";
            row.Controls.Add(cell);
            table.Controls.Add(row);

            //add assigned rules listBox
            row = new TableRow();
            cell = new TableCell();
            cell.HorizontalAlign = HorizontalAlign.Center;
            listBox = new ListBox();
            listBox = generateListBoxAssigned(questionId, type);
            cell.Controls.Add(listBox);
            row.Controls.Add(cell);
            table.Controls.Add(row);
        }

        private ListBox generateListBoxAvailable(string questionId, string type)
        {
            ListBox listBox = new ListBox();
            listBox.ID = "lbxAvailable" + type + "_" + questionId;
            listBox.Width = 400;
            listBox.Rows = 15;
            listBox.SelectionMode = ListSelectionMode.Multiple;

            //List<Rule> ruleCollection = null;

            generateListBox(listBox, type, int.Parse(questionId), "available");
            return listBox;
        }

        private ListBox generateListBoxAssigned(string questionId, string type)
        {
            ListBox listBox = new ListBox();
            listBox.ID = "lbxAssigned" + type + "_" + questionId;
            listBox.Width = 400;
            listBox.Rows = 15;
            listBox.SelectionMode = ListSelectionMode.Multiple;

            //List<Rule> ruleCollection = null;

            generateListBox(listBox, type, int.Parse(questionId), "assigned");
            return listBox;
        }

        private void generateListBox(ListBox listBox, string type, int questionId, string action)
        {
            //int id = 0;
            //string description = "";

            //List<questionRule> questionRuleCollection = new List<questionRule>();
            //question question = new question(new Id(questionId));
            //questionRuleType ruleType = null;

            //if (type == "lookUp")
            //{
            //    ruleType = new questionRuleType(new Id(1));
            //}
            //else
            //{
            //    ruleType = new questionRuleType(new Id(2));
            //}

            //if (action == "available")
            //{
            //    questionRuleCollection = question.getquestionRuleAvailableCollection(ruleType);
            //}
            //else if (action == "assigned")
            //{
            //    questionRuleCollection = question.getquestionRuleAssignedCollection(ruleType);
            //}

            //if (questionRuleCollection.Count > 0)
            //{
            //    foreach (questionRule rule in questionRuleCollection)
            //    {
            //        id = rule.id;
            //        description = rule.description;

            //        listBox.Items.Add(new ListItem(description, id.ToString()));
            //    }
            //}
        }

        private ImageButton generateAddButton(string questionId, string type)
        {
            ImageButton button = new ImageButton();
            button.ID = "btnAdd" + type + "_" + questionId;
            button.ImageUrl = "images/addSmall_button.gif";

            return button;
        }

        private ImageButton generateCancelButton(string questionId, string type)
        {
            ImageButton button = new ImageButton();
            button.ID = "btnCancel" + type + "_" + questionId;
            button.ImageUrl = "images/cancel_button.gif";

            return button;
        }

        private ImageButton generateRemoveButton(string questionId, string type)
        {
            ImageButton button = new ImageButton();
            button.ID = "btnRemove" + type + "_" + questionId;
            button.ImageUrl = "images/removeSmall_button.gif";

            return button;
        }

        private string getRuleByquestion(int questionId)
        {
            string ruleTypes = "";

            rule rule = null;
            List<rule> ruleCollection = db.pr_getRuleByQuestion(questionId).ToList();

            for (int i = 0; i < ruleCollection.Count; i++)
            {
                rule = ruleCollection[i];

                if (i == 0)
                {
                    ruleTypes += "<span style='color: blue'> Rules: " + rule.description + "</span>";
                }
                else
                {
                    ruleTypes += " , <span style='color: blue'>" + rule.description + "</span>";
                }
            }

            return ruleTypes;

        }

        public string generateZCode(partner partner, questionnaire questionnaire)
        {

            //DataTable dataTable = new DataTable();
            //DataColumn column = new DataColumn();
            //dataTable = partner.getsurveyResponse(questionnaire);
            //string columnName = "";
            //string ZCode = string.Empty;
            //int count = dataTable.Columns.Count;
            //int questionId = 0;
            //question question = new question();
            //string[] array = new string[2];
            //char[] splitter = { ')' };
            //if (dataTable.Rows.Count > 0)
            //{
            //    for (int i = 0; i < count; ++i)
            //    {
            //        columnName = dataTable.Columns[i].ColumnName;


            //        //questionnaire
            //        if (columnName.Contains(")"))
            //        {
            //            array = columnName.Split(splitter);
            //            questionId = int.Parse(array[1]);
            //            string[] array2 = array[0].Split('(');
            //            int questionNo = int.Parse(array2[1]);
            //            question = new question(new Id(questionId));
            //            question = question.getquestionDetail();

            //            if (questionNo == 1 || questionNo == 2 || questionNo == 3 || questionNo == 4 || questionNo == 5 || questionNo == 7 || questionNo == 9 || questionNo == 15 ||
            //                questionNo == 16 || questionNo == 18 || questionNo == 20 || questionNo == 22 || questionNo == 23 || questionNo == 24 || questionNo == 25 || questionNo == 26
            //                || questionNo == 27 || questionNo == 32 || questionNo == 33 || questionNo == 34 || questionNo == 35 || questionNo == 37 || questionNo == 38 || questionNo == 39
            //                || questionNo == 40 || questionNo == 41 || questionNo == 42 || questionNo == 43 || questionNo == 44 || questionNo == 45 || questionNo == 46 || questionNo == 47 ||
            //                questionNo == 48 || questionNo == 49 || questionNo == 50 || questionNo == 51 || questionNo == 53 || questionNo == 54)
            //                if (dataTable.Rows[0][columnName].ToString() == "Yes")
            //                    ZCode += "1";
            //                else if (dataTable.Rows[0][columnName].ToString() == "No")
            //                    ZCode += "0";
            //                else
            //                    ZCode += "9";

            //            if (questionNo == 6)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "422")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "769")
            //                    ZCode += "B";
            //                else
            //                    ZCode += "9";
            //            }

            //            if (questionNo == 8)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "422")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "769")
            //                    ZCode += "B";
            //                else
            //                    ZCode += "9";
            //            }

            //            if (questionNo == 10)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "378")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "379")
            //                    ZCode += "B";
            //                else if (dataTable.Rows[0][columnName].ToString() == "768")
            //                    ZCode += "C";
            //                else
            //                    ZCode += "9";
            //            }
            //            if (questionNo == 11)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "426")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "427")
            //                    ZCode += "B";
            //                else if (dataTable.Rows[0][columnName].ToString() == "768")
            //                    ZCode += "C";
            //                else
            //                    ZCode += "9";
            //            }

            //            if (questionNo == 12)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "424")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "428")
            //                    ZCode += "B";
            //                else if (dataTable.Rows[0][columnName].ToString() == "768")
            //                    ZCode += "C";
            //                else
            //                    ZCode += "9";
            //            }

            //            if (questionNo == 13)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "429")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "430")
            //                    ZCode += "B";
            //                else if (dataTable.Rows[0][columnName].ToString() == "768")
            //                    ZCode += "C";
            //                else
            //                    ZCode += "9";
            //            }


            //            if (questionNo == 14)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "389")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "420")
            //                    ZCode += "B";
            //                else
            //                    ZCode += "9";
            //            }

            //            if (questionNo == 17 || questionNo == 19 || questionNo == 21 || questionNo == 28 || questionNo == 29 || questionNo == 52)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() != "")
            //                    ZCode += "1";
            //                else
            //                    ZCode += "9";
            //            }



            //            if (questionNo == 30)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "Under $10 million")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "$10 - $50 million")
            //                    ZCode += "B";
            //                else if (dataTable.Rows[0][columnName].ToString() == "$50 - $100 million")
            //                    ZCode += "C";
            //                else if (dataTable.Rows[0][columnName].ToString() == "Over $100 million")
            //                    ZCode += "D";
            //                else if (dataTable.Rows[0][columnName].ToString() == "Not Provided")
            //                    ZCode += "Z";
            //                else
            //                    ZCode += "9";
            //            }

            //            if (questionNo == 31)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "50 and under")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "51 - 500")
            //                    ZCode += "B";
            //                else if (dataTable.Rows[0][columnName].ToString() == "Over 500")
            //                    ZCode += "C";
            //                else if (dataTable.Rows[0][columnName].ToString() == "Not Provided")
            //                    ZCode += "Z";
            //                else
            //                    ZCode += "9";
            //            }



            //            if (questionNo == 36)
            //            {
            //                if (dataTable.Rows[0][columnName].ToString() == "391")
            //                    ZCode += "A";
            //                else if (dataTable.Rows[0][columnName].ToString() == "392")
            //                    ZCode += "B";
            //                else if (dataTable.Rows[0][columnName].ToString() == "393")
            //                    ZCode += "C";
            //                else if (dataTable.Rows[0][columnName].ToString() == "394")
            //                    ZCode += "D";
            //                else if (dataTable.Rows[0][columnName].ToString() == "395")
            //                    ZCode += "E";
            //                else if (dataTable.Rows[0][columnName].ToString() == "396")
            //                    ZCode += "F";
            //                else
            //                    ZCode += "9";
            //            }


            //        }
            //    }
            //    partner.zCode = ZCode;
            //    partner.modifypartnerZCode();
            //}
            //return ZCode;
            return "";
        }

    }


}