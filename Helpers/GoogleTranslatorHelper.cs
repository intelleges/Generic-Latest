using Google.Apis.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public class GoogleTranslatorHelper : IGoogleTranslatorHelper
    {
        Google.Apis.Translate.v2.TranslateService _gService;
        private EntitiesDBContext _db = new EntitiesDBContext();
        IDatabaseTranslationService _dbservice;
        public GoogleTranslatorHelper(IDatabaseTranslationService dbservice, Google.Apis.Translate.v2.TranslateService gService)
        {
            _dbservice = dbservice;
            _gService = gService;
        }

        public string Translate(int id, TranslationType type, string lang, int cmsId=0)
        {
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
                    result = GoogleTranslate(question.Question, lang);
                    _dbservice.SetQuestionTranslation(id, result, lang);
                }
            }
            return result;
        }
        private string TranslateCMS(int id, string lang, int cmsId)
        {
            string result = "";
            if (_dbservice.HasTranslation(id, TranslationType.CMS, lang))
                result = _dbservice.GetCmsItemTranslation(id, lang, cmsId);
            else
            {
                var cmsItem = _db.pr_getQuestionnaireQuestionnaireCMS(id, cmsId).FirstOrDefault();
                if (cmsItem != null)
                {
                    result = GoogleTranslate(cmsItem.text, lang);
                    _dbservice.SetCmsItemTranslation(id, result, lang, cmsId);
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
                    result = GoogleTranslate(response.description, lang);
                    _dbservice.SetResponseTranslation(id, result, lang);
                }
            }
            return result;
        }

        private string GoogleTranslate(string text, string lang)
        {
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
    }
}