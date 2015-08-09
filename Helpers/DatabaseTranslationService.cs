using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public class DatabaseTranslationService : IDatabaseTranslationService
    {

        public string GetQuestionTranslation(int qid, string lang)
        {
            throw new NotImplementedException();
        }

        public void SetQuestionTranslation(int qid, string text, string lang)
        {
            
        }

        public string GetCmsItemTranslation(int questionnaireId, string lang, int cmsId)
        {
            throw new NotImplementedException();
        }

        public void SetCmsItemTranslation(int questionnaireId, string text, string lang, int cmsId)
        {
            
        }

        public string GetResponseTranslation(int responseId, string lang)
        {
            throw new NotImplementedException();
        }

        public void SetResponseTranslation(int responseId, string text, string lang)
        {
            
        }

        public bool HasTranslation(int id, TranslationType type, string lang, int cmsId = 0)
        {
            //temporary TODO: db check
            return false;
        }

        public void Dispose()
        {
            ///TODO: dispose DB context
        }
    }
}