using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Areas.RegistrationArea.Models
{
    public class ViewBagModel
    {
        public string CONTACT_US_EMAIL { get; set; }
        public string QUESTIONNAIRE_PDF { get; set; }
        public string QUESTIONNAIRE_FAQ { get; set; }
        public string QUESTIONNAIRE_DOC_OTHER { get; set; }
        public string QUESTIONNAIRE_VIDEO { get; set; }
        public string QUESTIONNAIRE_DOC_OTHER_2 { get; set; }
        public string REQUIRED_FIELDS { get; set; }
        public string FIRST_NAME { get; set; }
        public string LAST_NAME { get; set; }
        public string TITLE_TEXT { get; set; }
        public string EMAIL_TEXT { get; set; }
        public string PHONE_TEXT { get; set; }
        public string CMS_PAGE_PREVIOUS_TEXT { get; set; }
        public string CMS_PAGE_NEXT_TEXT { get; set; }
        public string ESIGNATURE_PAGE_TEXT { get; set; }
        public string CMS_PAGE_TITLE { get; set; }
        public string CMS_PAGE_SUBTITLE { get; set; }
        public string CMS_PAGE_PANEL_ONE { get; set; }
        public string CMS_PAGE_PANEL_TWO { get; set; }
        //company information
        public string COMPANY_INFORMATION_TEXT { get; set; }
        public string VERIFY_COMPANY_INFO { get; set; }
        public string country { get; set; }
        public string state { get; set; }
        public string Company { get; set; }
        public string PHYSYCAL_ADDRESS { get; set; }
        public string ADDRESS_ONE { get; set; }
        public string ADDRESS_TWO { get; set; }
        public string CITY { get; set; }
        public string STATE_TEXT { get; set; }
        public string POSTAL_CODE { get; set; }
        public string PROVINCE { get; set; }
        public string COUNTRY_TEXT { get; set; }
        //Contact information
        public string CONTACT_INFORMATION_TEXT { get; set; }
        public string VERIFY_CONTACT_TEXT_INFORMATION { get; set; }
        public string FAX_TEXT { get; set; }
        //Finish 
        public string CONFIRMATION_PAGE_SIGNOFF_STATEMENT { get; set; }
        public string CONFIRMATION_PAGE_EXIT_LINK { get; set; }
        public string CONFIRMATION_PAGE_HEADLINE { get; set; }
        public string CONFIRMATION_PAGE_SIGNOFF_INCOMPLETE_STATEMENT { get; set; }
        public string WARNING { get; set; }
        public string CMS_PAGE_PREVIOUS_LINK { get; set; }
        public string CMS_PAGE_NEXT_LINK { get; set; }
        public string QUESTIONNAIRE_CONTACT_US_EMAIL_LINK { get; set; }
        public string QUESTIONNAIRE_VIDEO_LINK { get; set; }
        public bool isCompletedSurvey { get; set; }
        ////Save for later confirm
        public string SAVE_FOR_LATER_TEXT_NOTICE { get; set; }
        ////Index 
        public string RETRIEVE_ACCESS_CODE_TEXT { get; set; }
        public string CMS_FOOTER_ONE { get; set; }
        public string CMS_FOOTER_TWO { get; set; }
        public string CMS_SUBMIT_TEXT { get; set; }
        public string ACCESS_CODE_PLEASE_ENTER { get; set; }
        public string ACCESS_CODE_SIPLE_TEXT { get; set; }
        public Nullable<int> ENTERPRISE_ID { get;set; }
        ////
        public string SAVE_FOR_LATER_TEXT { get; set; }
    }

   
}