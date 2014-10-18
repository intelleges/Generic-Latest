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

namespace Generic.DataLayer
{

    /// <summary>
    /// Summary description for surveyForm
    /// </summary>
    public class surveyForm
    {
        EntitiesDBContext db = new EntitiesDBContext();

        static int divShowHideFlag = 3;
        public surveyForm()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public surveyForm(protocol protocol, touchpoint touchpoint, partner partner, questionnaire questionnaire)
        {

            this.protocol = protocol;
            this.touchpoint = touchpoint;
            this.partner = partner;
            this.questionnaire = questionnaire;
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


        public Table tGetsurveyForm(questionnaire questionnaire, int pageNumber, int pageId, int jumpToquestion)
        {
            Table table = new Table();
            table.Width = Unit.Percentage(100);

            if (pageNumber == 0)
            {
                pageNumber = 1;
            }
            showPageCollectionByquestionnaire(questionnaire, pageNumber, pageId, jumpToquestion, table);

            return table;
        }
        public Table tGetsurveyForm(questionnaire questionnaire)
        {
            Table table = new Table();
            this.showContentOnly = true;
            showquestionCollectionByquestionnaire(questionnaire, table);
            return table;
        }
         
        private void showPageCollectionByquestionnaire(questionnaire questionnaire, int pageNumber, int pageId, int jumpToquestion, Table table)
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
                        showsurveysetCollectionByPage(page, jumpToquestion, table);
                    }
                }
            }
        }

        private void showsurveysetCollectionByPage(page page, int jumpToquestion, Table table)
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
                    showsurveyCollectionBysurveyset(surveyset, jumpToquestion, table);
                }
            }
        }

        private void showsurveyCollectionBysurveyset(surveyset surveyset, int jumpToquestion, Table table)
        {
            List<survey> surveyCollection = db.pr_getSurveyBySurveyset(surveyset.id).ToList();
            survey survey = null;

            if (surveyCollection.Count > 0)
            {
                for (int i = 0; i < surveyCollection.Count; i++)
                {
                    survey = surveyCollection[i];
                    showquestionCollectionBysurvey(surveyset, survey, ref jumpToquestion, table);
                }
            }
        }

        //    private void showquestionCollectionBysurvey(survey survey, int jumpToquestion, Table table)
        private void showquestionCollectionBysurvey(surveyset surveyset, survey survey, ref int jumpToquestion, Table table)
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
                    showquestion(surveyset, survey, question, this.questionIndex, table);
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
        private void showquestion(surveyset surveyset, survey survey, question question, int index, Table table)
        {
            TableRow tableRow = new TableRow();
            TableCell tableCell = new TableCell();
            TableRow tableRowsurvey = new TableRow();
            TableCell tableCellsurvey = new TableCell();
            partnerPartnertypeTouchpointQuestionnaire objpptq = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByAccessCode(HttpContext.Current.Session["accessCode"].ToString()).FirstOrDefault();

            Label label = new Label();
            Label labelsurvey = new Label();
            int divflag = 0;
            //question.question = convertLanguageApi(question.question);


            //abs21022012
            string strQuestion = question.question1;
            if (strQuestion != null)
            {
                if (strQuestion.Contains("[partnumber]"))
                {


                    if (HttpContext.Current.Session["partnumber"] != null && HttpContext.Current.Session["partnumber"] != "0" && HttpContext.Current.Session["partnumber"] != "")
                    {
                        int partid = Convert.ToInt32(HttpContext.Current.Session["partnumber"].ToString());
                        if (partid != 0)
                        {
                            string partName = db.pr_getPartnumber(partid).FirstOrDefault().description;
                            question.question1 = strQuestion.Replace("[partnumber]", partName);
                        }
                    }
                }
                string strQuestionAgain = question.question1;
                if (strQuestionAgain.Contains("[next partnumber]"))
                {
                    if (HttpContext.Current.Session["NextPartnumber"] != null)
                    {
                        int partid = Convert.ToInt32(HttpContext.Current.Session["NextPartnumber"].ToString());
                        if (partid != 0)
                        {
                            string partName = db.pr_getPartnumber(partid).FirstOrDefault().description;
                            question.question1 = strQuestionAgain.Replace("[next partnumber]", partName);
                        }
                    }
                }
            }


            if (this.showContentOnly)
            {
                label.Text = question.question1 + " " + getRuleByquestion(question.id);
            }
            else
            {
                label.Text = question.question1;

                //add error message to the end of the question
                if (question.id == this.errorquestion)
                {
                    label.Text += this.errorMessage;
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
            currentsurvey = surveyset.description;
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
                tableCell.Controls.Add(showButton(question.id.ToString(), question.question1, "lookUp"));
                tableCell.Controls.Add(showButton(question.id.ToString(), question.question1, "validation"));
            }

            //show question 
            tableCell = new TableCell();
            tableCell.CssClass = cssClass;
            tableCell.HorizontalAlign = HorizontalAlign.Left;
            tableCell.VerticalAlign = VerticalAlign.Top;
            tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(80);
            tableCell.Controls.Add(label);
            tableRow.Controls.Add(tableCell);

            string controlId = "";

            string responseTypeDescription = db.pr_getResponseType(question.responseType).FirstOrDefault().description;

            var objresponseByQuestion = db.pr_getResponseByQuestion(question.id).ToList();
            //Add required validation control
            if (question.required == 1 && this.showContentOnly == false)
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
            else if (responseTypeDescription.Contains("text"))
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
            response response = new response();

            partnerPartnertypeTouchpointQuestionnaireQuestionResponse pptqResponse = new partnerPartnertypeTouchpointQuestionnaireQuestionResponse();

            if ((int)HttpContext.Current.Session["leveltype"] == Generic.Helpers.Questionnaire.LevelType.PARTNUMBER_LEVEL)
            {
                var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ((int)HttpContext.Current.Session["partnumber"], (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault(); ;
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
            if (question.commentBoxTxt == "" || question.commentBoxTxt == null)
            {
                incldComment = convertLanguageApi("<span style='font-size:13px'> Include comments here: </span>");
            }
            else
            {
                incldComment = convertLanguageApi("<span style='font-size:13px'>" + question.commentBoxTxt + "</span>");
            }
            if (question.commentUploadTxt == "" || question.commentUploadTxt == null)
            {
                incldFileUpload = convertLanguageApi("<span style='font-size:13px'> Upload written procedures here: </span>");
            }
            else
            {
                incldFileUpload = convertLanguageApi("<span style='font-size:13px'>" + question.commentUploadTxt + "</span>");
            }
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

                        innerdivUpload.InnerHtml = "<br/> " + incldFileUpload + "<font color='blue'> " + convertLanguageApi("Uploaded file ") + Path.GetFileName(uploadedFile) + "</font>";

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
                        validator.ErrorMessage = "Required";//" Required";
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
                        validator.ErrorMessage = "Required";//" Required";
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
                    txtbox = new TextBox();
                    txtbox.Width = 600;
                    //fileupload = new FileUpload();
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    divn.InnerHtml = incldComment + " "; //"Include comments here: ";
                    divn.Controls.Add(txtbox);
                    if (question.commentType != CommentType.YN_WARNING_N && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                        question.commentType != CommentType.YN_WARNING_Y && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y)
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
                        validator.ErrorMessage = "Required";//" Required";
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
                        validator.ErrorMessage = "Required";//" Required";
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
                    txtbox = new TextBox();
                    txtbox.Width = 600;
                    txtbox.Text = convertLanguageApi(pptqResponse.comment.ToString());
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                    divn.Controls.Add(txtbox);
                    if (question.commentType != CommentType.YN_WARNING_N && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                       question.commentType != CommentType.YN_WARNING_Y && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y)
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
                        validator.ErrorMessage = "Required";//" Required";
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
                        validator.ErrorMessage = "Required";//" Required";
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
                    txtbox = new TextBox();
                    txtbox.Width = 600;
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";
                    divn.InnerHtml = incldComment + " ";  //"Include comments here: ";
                    divn.Controls.Add(txtbox);
                    if (question.commentType != CommentType.YN_WARNING_N && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                        question.commentType != CommentType.YN_WARNING_Y && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y)
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
                        validator.ErrorMessage = "Required";//" Required";
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
                        validator.ErrorMessage = "Required";//" Required";
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

                    txtbox = new TextBox();
                    //fileupload = new FileUpload();
                    txtbox.Width = 600;
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";

                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                    divn.Controls.Add(txtbox);
                    //divn.Controls.Add(fileupload);
                    if (question.commentType != CommentType.YN_WARNING_N && question.commentType != CommentType.YN_COMMENT_N && question.commentType != CommentType.YN_UPLOAD_N &&
                    question.commentType != CommentType.YN_WARNING_Y && question.commentType != CommentType.YN_COMMENT_Y && question.commentType != CommentType.YN_UPLOAD_Y)
                        tableCell.Controls.AddAt(1, divn);

                    //tableCell.Text = "<div Id='yDiv_" + question.id + "' style='display:none' runat='server' >Test Text <br/><br/>Test Text One</div><div Id='nDiv_" + question.id + "' style='display:none'>Test Two <br/><br/>Test Text One</div>";
                }

                else
                {
                    tableCell.Text = "&nbsp";
                }
            }
            else if (question.commentRequired == CommentType.YN_COMMENT_REQUIRED_N)
            {

                if (question.commentType == CommentType.YN_WARNING_Y)
                {
                    HtmlGenericControl div = new HtmlGenericControl();
                    div.ID = "yDiv_" + question.id.ToString();
                    //div.Visible = false;
                    div.Style.Add("display", "none");
                    div.InnerHtml = incldComment + " ";//"Include comments here: ";
                    tableCell.Controls.AddAt(0, div);
                }
                else if (question.commentType == CommentType.YN_WARNING_N)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    divn.Style.Add("display", "none");
                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                    tableCell.Controls.AddAt(0, divn);

                }
                else if (question.commentType == CommentType.YN_COMMENT_N)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "nDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    divn.Style.Add("display", "none");
                    txtbox = new TextBox();

                    txtbox.Width = 600;
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_onlyTextComment";

                    divn.InnerHtml = incldComment + " ";//"Include comments here: ";
                    divn.Controls.Add(txtbox);

                    tableCell.Controls.AddAt(0, divn);

                }
                else if (question.commentType == CommentType.YN_UPLOAD_Y)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "yDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    divn.Style.Add("display", "none");
                    fileupload = new FileUpload();
                    fileupload.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                    txtbox.Width = 600;
                    HtmlGenericControl innerdiv = new HtmlGenericControl();
                    innerdiv.InnerHtml = incldComment + " "; //"Include comments here: ";
                    innerdiv.Controls.Add(txtbox);

                    HtmlGenericControl innerdivUpload = new HtmlGenericControl();
                    innerdivUpload.InnerHtml = "<br/>" + incldFileUpload + " "; //Upload written procedures here: ";

                    ////add validators to _fileUploadComment
                    controlId = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_fileUploadComment";
                    addControlValidatorToFileUpload(controlId, "regularexpressionValidator", innerdiv);
                    addControlValidatorToFileUpload(controlId, "customValidator", innerdiv);
                    divn.Controls.AddAt(0, innerdiv);
                    divn.Controls.AddAt(1, innerdivUpload);
                    innerdivUpload.Controls.Add(fileupload);
                    RequiredFieldValidator validator = new RequiredFieldValidator();
                    validator.ID = question.id + "_R";
                    validator.ControlToValidate = controlId;
                    validator.ErrorMessage = "Required";//" Required";
                    validator.Display = ValidatorDisplay.Dynamic;
                    innerdivUpload.Controls.Add(validator);
                    tableCell.Controls.AddAt(0, divn);

                }
                else if (question.commentType == CommentType.YN_COMMENT_Y)
                {
                    HtmlGenericControl divn = new HtmlGenericControl();
                    divn.ID = "yDiv_" + question.id.ToString();
                    //divn.Visible = false;
                    divn.Style.Add("display", "none");
                    txtbox.ID = "question_" + question.id.ToString() + "_" + survey.id.ToString() + "_Commenttext";
                    txtbox.Width = 600;
                    divn.InnerHtml = incldComment + " "; //"Include comments here: ";
                    divn.Controls.Add(txtbox);

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
            FileUpload fileUpload = new FileUpload();
            CheckBox checkBox = new CheckBox();
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
                    var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ((int)HttpContext.Current.Session["partnumber"], (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault(); ;
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

            switch (responseType)
            {
                case "textComment":
                    textBox = new TextBox();
                    textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                    textBox.Width = 600;

                    if (pptqResponse != null)
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
                case "textInteger":
                    textBox = new TextBox();
                    textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                    textBox.Width = 600;

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
                    textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                    textBox.Width = 600;

                    if (pptqResponse.comment != null && pptqResponse.comment.Length > 0)
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
                    textBox = new TextBox();

                    textBox.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString() + "_text";
                    textBox.Width = 600;
                    textBox.TextMode = TextBoxMode.MultiLine;
                    textBox.Rows = 3;

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
                        textBox.Text = convertLanguageApi(pptqResponse.comment);
                    }

                    tableCell = new TableCell();
                    tableCell.Controls.Add(textBox);
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
                        dropDownList.Attributes.Add("data-val-required", "Required");
                    }

                    dropDownList.Width = 250;
                    tableCell = new TableCell();
                    tableCell.HorizontalAlign = HorizontalAlign.Left;
                    string selectval = convertLanguageApi("Please select one");
                    dropDownList.Items.Add(new ListItem(selectval, ""));

                    for (int i = 0; i < responseCollection.Count; i++)
                    {
                        //dropDownList.Items.Add(new ListItem(responseCollection[i].description, responseCollection[i].id.ToString()));
                        dropDownList.Items.Add(new ListItem(convertLanguageApi(responseCollection[i].description), responseCollection[i].id.ToString()));
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

                    tableCell = new TableCell();
                    radioButtonList = new  RadioButtonList();//new Generic.Helpers.UIControl.MyRadioButtonList();
                    radioButtonList.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString();
                    radioButtonList.Font.Size = 10;
                    //radioButtonList.Attributes.Add("onchange", "javascript:showdiv();");
                    radioButtonList.Attributes.Add("onClick", "showdivRadioList(this);removevalidation(this.id) ");

                    

                    radioButtonList.RepeatDirection = RepeatDirection.Vertical;
                    tableCell = new TableCell();
                    tableCell.HorizontalAlign = HorizontalAlign.Left;
                    for (int i = 0; i < responseCollection.Count; i++)
                    {
                        //radioButtonList.Items.Add(new ListItem(responseCollection[i].description, responseCollection[i].id.ToString()));
                        radioButtonList.Items.Add(new ListItem(convertLanguageApi(responseCollection[i].description), responseCollection[i].id.ToString()));
                        if (question.required == 1)
                        {
                            radioButtonList.Items[i].Attributes.Add("data-val", "true");
                            radioButtonList.Items[i].Attributes.Add("data-val-required", "Required");
                        }
                    
                        if (pptqResponse != null && responseCollection[i].id == pptqResponse.response)
                        {
                            radioButtonList.ClearSelection();
                            radioButtonList.Items[i].Selected = true;
                        }
                        tableCell.Controls.Add(radioButtonList);
                    }

                    

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
                            tableCell.Text = surveyfrm.convertLanguageApi("File uploaded") + ": " + Path.GetFileName(uploadedFile);
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
                                var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ((int)HttpContext.Current.Session["partnumber"], (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault(); ;

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
                            }catch{}
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
                        checkBox.Text = convertLanguageApi(responseCollection[i].description);

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
                    literal.Text = "<p>" + surveyfrm.convertLanguageApi("Please upload the file here") + "</p>";
                    if (showContentOnly == false)
                    {
                        string uploadedFile = "";// question.getUploadedFile(partner, protocol, touchpoint, questionnaire, survey);


                        if (!string.IsNullOrEmpty(uploadedFile))
                        {

                            literal.Text = "<p>" + surveyfrm.convertLanguageApi("File uploaded") + ": " + Path.GetFileName(uploadedFile) + "</p>";
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
                    var PartNumberSiteZcodepptq = db.pr_getPartnumberSiteZcodePPTQByPartnumberSiteAndPPTQ((int)HttpContext.Current.Session["partnumber"], (int)HttpContext.Current.Session["site"], objpptq.id).FirstOrDefault(); ;
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

            switch (responseType)
            {
                case "radioButton":
                    radioButtonList = new Generic.Helpers.UIControl.MyRadioButtonList();
                    radioButtonList.ID = "question_" + questionId.ToString() + "_" + surveyId.ToString();
                    radioButtonList.Attributes.Add("onClick", "showdivnew(this);removevalidation(this.id) ");
                   
                    radioButtonList.RepeatDirection = RepeatDirection.Horizontal;
                    tableCell = new TableCell();
                    
                    tableCell.HorizontalAlign = HorizontalAlign.Right;
                    tableCell.CssClass = cssClass;
                    tableCell.Width = System.Web.UI.WebControls.Unit.Percentage(15);
                    for (int i = 0; i < responseCollection.Count; i++)
                    {
                        radioButtonList.Items.Add(new ListItem(convertLanguageApi(responseCollection[i].description), responseCollection[i].id.ToString()));
                        if (question.required == 1)
                        {
                            radioButtonList.Items[i].Attributes.Add("data-val", "true");
                            radioButtonList.Items[i].Attributes.Add("data-val-required", "Required");
                        }
                   
                        if (pptqResponse != null && responseCollection[i].id == pptqResponse.response)
                        {
                            if (pptqResponse.response == 74)
                            {
                                divShowHideFlag = 1;
                            }
                            else if (pptqResponse.response == 75)
                            {
                                divShowHideFlag = 0;
                            }
                            else
                            {
                                divShowHideFlag = -1;
                            }
                            radioButtonList.Items[i].Selected = true;
                            radioButtonList.Items[i].Attributes.Add("checked", "true");
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