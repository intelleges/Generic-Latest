using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generic.Helpers
{
    public interface IGoogleTranslatorHelper:IDisposable
    {
        partnerPartnertypeTouchpointQuestionnaire PPTQ { get; set; }
        string Translate(int id, TranslationType type, string lang, int cmsId = 0);
        string Translate(string text, string lang);
    }
}
