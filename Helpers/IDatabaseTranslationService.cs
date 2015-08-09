using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Helpers
{
    public enum TranslationType
    {
        Question,
        CMS,
        Response
    }
    public interface IDatabaseTranslationService:IDisposable
    {
        string GetQuestionTranslation(int qid, string lang);
        void SetQuestionTranslation(int qid, string text, string lang);
        string GetCmsItemTranslation(int questionnaireId, string lang, int cmsId);
        void SetCmsItemTranslation(int questionnaireId, string text, string lang, int cmsId);
        string GetResponseTranslation(int responseId, string lang);
        void SetResponseTranslation(int responseId, string text, string lang);
        bool HasTranslation(int id, TranslationType type, string lang, int cmsId=0);
    }
}
