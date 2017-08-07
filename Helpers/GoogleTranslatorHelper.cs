using Generic.Helpers.Utility;
using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Generic.Helpers
{
    public class GoogleTranslatorHelper : IGoogleTranslatorHelper
    {
        Google.Apis.Translate.v2.TranslateService _gService;
        private EntitiesDBContext _db = new EntitiesDBContext();
        IDatabaseTranslationService _dbservice;
        EmailFormat _emailFormat;
        public partnerPartnertypeTouchpointQuestionnaire PPTQ { get; set; }
        public GoogleTranslatorHelper(IDatabaseTranslationService dbservice)
        {
            _dbservice = dbservice;
            _gService = new Google.Apis.Translate.v2.TranslateService(new BaseClientService.Initializer()
            {
                ApiKey = ConfigurationManager.AppSettings["GoogleTranslateApiKey"]
            });
            _emailFormat = new EmailFormat();
        }

        public string Translate(int id, TranslationType type, string lang, int cmsId = 0)
        {
            if (PPTQ == null) throw new Exception("Please setup PPTQ to use translation services");
            switch (type)
            {
                case TranslationType.Question:
                    return TranslateQuestion(id, lang); 
                    break;
                case TranslationType.Response:
                    return TranslateResponse(id, lang);
                    break;
                case TranslationType.CMS:
                    return TranslateCMS(id, lang, cmsId);
                    break;
            }
            return null;
        }
        private string TranslateQuestion(int id, string lang)
        {
            string result = "";
            if (_dbservice.HasTranslation(id, TranslationType.Question, lang))
                result = _dbservice.GetQuestionTranslation(id, lang);
            else
            {
                var question = _db.pr_getQuestion(id).FirstOrDefault();
                if (question != null)
                {
                    var text = _emailFormat.sGetEmailBody(question.Question, null, PPTQ.partner1, PPTQ.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, PPTQ.partnerTypeTouchpointQuestionnaire1.touchpoint1);
                    if (lang == "en" || string.IsNullOrEmpty(question.Question))
                    {
                        result = text;
                    }
                    else
                    {
                        result = GoogleTranslate(text, lang);
                        _dbservice.SetQuestionTranslation(id, result, lang);
                    }
                }
            }
            return result;
        }
        private string TranslateCMS(int id, string lang, int cmsId)
        {
            string result = null;
            if (_dbservice.HasTranslation(id, TranslationType.CMS, lang))
                result = _dbservice.GetCmsItemTranslation(id, lang, cmsId);
            else
            {
                var cmsItem = _db.pr_getQuestionnaireQuestionnaireCMS(id, cmsId).FirstOrDefault();
                
                if (cmsItem != null)
                {
                    if (cmsItem.text == "") return cmsItem.text;
                    var text = _emailFormat.sGetEmailBody(cmsItem.text, null, PPTQ.partner1, PPTQ.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, PPTQ.partnerTypeTouchpointQuestionnaire1.touchpoint1, PPTQ.partnerTypeTouchpointQuestionnaire);
                    if (lang == "en" || string.IsNullOrEmpty(text))
                    {
                        result = text;
                    }
                    else
                    {
                        result = GoogleTranslate(text, lang);
                        _dbservice.SetCmsItemTranslation(id, result, lang, cmsId);
                    }
                }
            }
            return result;
        }
        private string TranslateResponse(int id, string lang)
        {
            string result = "";
            if (_dbservice.HasTranslation(id, TranslationType.Response, lang))
                result = _dbservice.GetResponseTranslation(id, lang);
            else
            {
                var response = _db.pr_getResponse(id).FirstOrDefault();
                
                if (response != null)
                {
                    var text = _emailFormat.sGetEmailBody(response.description, null, PPTQ.partner1, PPTQ.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, PPTQ.partnerTypeTouchpointQuestionnaire1.touchpoint1);
                    if (lang == "en" || string.IsNullOrEmpty(response.description))
                    {
                        result = text;
                    }
                    else
                    {
                        result = GoogleTranslate(text, lang);
                        _dbservice.SetResponseTranslation(id, result, lang);
                    }
                }
            }
            return result;
        }

        private string GoogleTranslate(string text, string lang)
        {
            Regex reg = new Regex("\\([A-Z][A-Z]\\)");
            var match = reg.Match(text);
            if (match.Success)
            {
                text = text.Replace(match.Value, "<span class=\"notranslate\">"+match.Value+"</span>");
            }
            var response = _gService.Translations.List(text, lang).Execute();
            return response.Translations.FirstOrDefault().TranslatedText;
        }


        public void Dispose()
        {
            if (_gService != null)
                _gService.Dispose();
            if (_db != null)
                _db.Dispose();
            if (_dbservice != null)
                _dbservice.Dispose();
        }


        public string Translate(string text, string lang)
        {
            
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(lang))
            {
                if(PPTQ!=null)
                {
                    text = _emailFormat.sGetEmailBody(text, null, PPTQ.partner1, PPTQ.partnerTypeTouchpointQuestionnaire1.partnerType1.enterprise1, PPTQ.partnerTypeTouchpointQuestionnaire1.touchpoint1);
                }
                if (lang.ToLower() == "en")
                    return text;
                return GoogleTranslate(text, lang);
            }
            return "";
        }
    }
}