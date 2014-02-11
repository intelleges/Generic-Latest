using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers.Questionnaire
{
    public static class CommentType
    {
        public const int YN_WARNING_N = 1;
        public const int YN_WARNING_Y = 2;
        public const int YN_COMMENT_Y = 3;
        public const int YN_COMMENT_N = 4;
        public const int YN_UPLOAD_Y = 5;
        public const int YN_UPLOAD_N = 6;
        public const int YN_NO_COMMENT = 7;

        public const int YN_COMMENT_REQUIRED_Y = 1;
        public const int YN_COMMENT_REQUIRED_N = 0;

        // old system
        //        commentType 4 = File  Upload
        //commentType 1 = Warning
        //commentType 3 = Comment
        //commentType 0 = Nothing

    }

    public static class LevelType
    {
        public const int COMPANY_LEVEL = 1;
        public const int PARTNUMBER_LEVEL = 2;
        public const int PURCHASE_ORDER_LEVEL = 3;
        public const int FMF_COST_BREAKDOWN = 4;
    }

    public static class CMS
    {
        public const string ACCESS_CODE_SUBTITLE = "ACCESS_CODE_SUBTITLE";
        public const string ACCESS_CODE_PANEL_ONE = "ACCESS_CODE_PANEL_ONE";
        public const string ACCESS_CODE_PANEL_TWO = "ACCESS_CODE_PANEL_TWO";
        public const string ACCESS_CODE_SUBMIT_TEXT = "ACCESS_CODE_SUBMIT_TEXT";
        public const string ACCESS_CODE_FOOTER_ONE = "ACCESS_CODE_FOOTER_ONE";
        public const string ACCESS_CODE_FOOTER_TWO = "ACCESS_CODE_FOOTER_TWO";
        public const string COMPANY_PAGE_TITLE = "COMPANY_PAGE_TITLE";
        public const string COMPANY_PAGE_SUBTITLE = "COMPANY_PAGE_SUBTITLE";
        public const string COMPANY_PAGE_PANEL_ONE = "COMPANY_PAGE_PANEL_ONE";
        public const string COMPANY_PAGE_PANEL_TWO = "COMPANY_PAGE_PANEL_TWO";
        public const string COMPANY_PAGE_PREVIOUS_TEXT = "COMPANY_PAGE_PREVIOUS_TEXT";
        public const string COMPANY_PAGE_NEXT_TEXT = "COMPANY_PAGE_NEXT_TEXT";
        public const string COMPANY_EDIT_PAGE_TITLE = "COMPANY_EDIT_PAGE_TITLE";
        public const string COMPANY_EDIT_PAGE_SUBTITLE = "COMPANY_EDIT_PAGE_SUBTITLE";
        public const string COMPANY_EDIT_PAGE_PANEL_ONE = "COMPANY_EDIT_PAGE_PANEL_ONE";
        public const string COMPANY_EDIT_PAGE_PANEL_TWO = "COMPANY_EDIT_PAGE_PANEL_TWO";
        public const string COMPANY_EDIT_PAGE_PREVIOUS_TEXT = "COMPANY_EDIT_PAGE_PREVIOUS_TEXT";
        public const string COMPANY_EDIT_PAGE_NEXT_TEXT = "COMPANY_EDIT_PAGE_NEXT_TEXT";
        public const string CONTACT_PAGE_TITLE = "CONTACT_PAGE_TITLE";
        public const string CONTACT_PAGE_SUBTITLE = "CONTACT_PAGE_SUBTITLE";
        public const string CONTACT_PAGE_PANEL_ONE = "CONTACT_PAGE_PANEL_ONE";
        public const string CONTACT_PAGE_PANEL_TWO = "CONTACT_PAGE_PANEL_TWO";
        public const string CONTACT_PAGE_PREVIOUS_TEXT = "CONTACT_PAGE_PREVIOUS_TEXT";
        public const string CONTACT_PAGE_NEXT_TEXT = "CONTACT_PAGE_NEXT_TEXT";
        public const string CONTACT_EDIT_PAGE_TITLE = "CONTACT_EDIT_PAGE_TITLE";
        public const string CONTACT_EDIT_PAGE_SUBTITLE = "CONTACT_EDIT_PAGE_SUBTITLE";
        public const string CONTACT_EDIT_PAGE_PANEL_ONE = "CONTACT_EDIT_PAGE_PANEL_ONE";
        public const string CONTACT_EDIT_PAGE_PANEL_TWO = "CONTACT_EDIT_PAGE_PANEL_TWO";
        public const string CONTACT_EDIT_PAGE_PREVIOUS_TEXT = "CONTACT_EDIT_PAGE_PREVIOUS_TEXT";
        public const string CONTACT_EDIT_PAGE_NEXT_TEXT = "CONTACT_EDIT_PAGE_NEXT_TEXT";
        public const string QUESTIONNAIRE_PAGE_TITLE = "QUESTIONNAIRE_PAGE_TITLE";
        public const string QUESTIONNAIRE_PAGE_SUBTITLE = "QUESTIONNAIRE_PAGE_SUBTITLE";
        public const string QUESTIONNAIRE_PAGE_PANEL_ONE = "QUESTIONNAIRE_PAGE_PANEL_ONE";
        public const string QUESTIONNAIRE_PAGE_PANEL_TWO = "QUESTIONNAIRE_PAGE_PANEL_TWO";
        public const string QUESTIONNAIRE_PAGE_PREVIOUS_TEXT = "QUESTIONNAIRE_PAGE_PREVIOUS_TEXT";
        public const string QUESTIONNAIRE_PAGE_NEXT_TEXT = "QUESTIONNAIRE_PAGE_NEXT_TEXT";
        public const string ESIGNATURE_PAGE_TITLE = "ESIGNATURE_PAGE_TITLE";
        public const string ESIGNATURE_PAGE_SUBTITLE = "ESIGNATURE_PAGE_SUBTITLE";
        public const string ESIGNATURE_PAGE_PANEL_ONE = "ESIGNATURE_PAGE_PANEL_ONE";
        public const string ESIGNATURE_PAGE_PANEL_TWO = "ESIGNATURE_PAGE_PANEL_TWO";
        public const string ESIGNATURE_PAGE_PREVIOUS_TEXT = "ESIGNATURE_PAGE_PREVIOUS_TEXT";
        public const string ESIGNATURE_PAGE_NEXT_TEXT = "ESIGNATURE_PAGE_NEXT_TEXT";
        public const string CONFIRMATION_PAGE_TITLE = "CONFIRMATION_PAGE_TITLE";
        public const string CONFIRMATION_PAGE_SUBTITLE = "CONFIRMATION_PAGE_SUBTITLE";
        public const string CONFIRMATION_PAGE_PANEL_ONE = "CONFIRMATION_PAGE_PANEL_ONE";
        public const string CONFIRMATION_PAGE_PANEL_TWO = "CONFIRMATION_PAGE_PANEL_TWO";
        public const string CONFIRMATION_PAGE_PREVIOUS_TEXT = "CONFIRMATION_PAGE_PREVIOUS_TEXT";
        public const string CONFIRMATION_PAGE_NEXT_TEXT = "CONFIRMATION_PAGE_NEXT_TEXT";

        public const string CONFIRMATION_PAGE_EXIT_LINK = "CONFIRMATION_PAGE_EXIT_LINK";
        public const string ACCESS_CODE_TITLE = "ACCESS_CODE_TITLE";

        public const string ESIGNATURE_PAGE_TEXT = "ESIGNATURE_PAGE_TEXT";
        public const string CONFIRMATION_PAGE_SIGNOFF_STATEMENT = "CONFIRMATION_PAGE_SIGNOFF_STATEMENT";
        public const string CONFIRMATION_PAGE_HEADLINE = "CONFIRMATION_PAGE_HEADLINE";
        
        public const string CONTACT_US_EMAIL = "CONTACT_US_EMAIL";

        public const string RETRIEVE_ACCESS_CODE_TEXT = "RETRIEVE_ACCESS_CODE_TEXT";
        public const string SAVE_FOR_LATER_TEXT = "SAVE_FOR_LATER_TEXT";
        
    public const string     QUESTIONNAIRE_PDF ="QUESTIONNAIRE_PDF";
    public const string QUESTIONNAIRE_FAQ = "QUESTIONNAIRE_FAQ";
    public const string QUESTIONNAIRE_DOC_OTHER = "QUESTIONNAIRE_DOC_OTHER";
    public const string QUESTIONNAIRE_VIDEO = "QUESTIONNAIRE_VIDEO";
        
    }
}