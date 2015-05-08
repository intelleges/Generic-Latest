using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace Generic.Helpers
{    
    public static class ResponsesExtensions
    {
        /// <summary>
        /// Returns text response for a question
        /// </summary>
        /// <param name="list"></param>
        /// <param name="questionObj"></param>
        /// <returns></returns>
        //public static string GetTextResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj)
        //{
        //    return list.FirstOrDefault(o => o.question == questionObj.id).comment;
        //}

        //public static int? GetTextResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj)
        //{
        //    return int.Parse(list.FirstOrDefault(o => o.question == questionObj.id).comment);
        //}
        //public static DateTime? GetTextResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj)
        //{
        //    return DateTime.Parse(list.FirstOrDefault(o => o.question == questionObj.id).comment);
        //}

        public static T GetTextResponse<T>(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj)
        {
            if (typeof(T).Name == typeof(int).Name)
            {
                try
                {
                    return (T)Convert.ChangeType(int.Parse(list.FirstOrDefault(o => o.question == questionObj.id).comment), typeof(T));
                }
                catch
                {
                    return  (T)Convert.ChangeType(0,typeof(T));
                }
            }
            if (typeof(T).Name == typeof(DateTime).Name)
            {
                try
                {
                    return (T)Convert.ChangeType(DateTime.Parse(list.FirstOrDefault(o => o.question == questionObj.id).comment), typeof(T));
                }
                catch
                {
                    return (T)Convert.ChangeType(DateTime.Now, typeof(T));
                }
            }
            return (T)Convert.ChangeType(list.FirstOrDefault(o => o.question == questionObj.id).comment, typeof(T));
        }
        /// <summary>
        /// Returns drop down reponse for a question
        /// </summary>
        /// <param name="list"></param>
        /// <param name="questionObj"></param>
        /// <returns></returns>
        public  static string GetDropDownResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj)
        {
            var value = list.FirstOrDefault(o => o.question == questionObj.id).response1.description;
            Regex exp = new Regex("\\([A-Z][A-Z]\\)");
            var match = exp.Match(value);
            if(match.Success)
                value = value.Replace(match.Value, "").Trim();
            return value;
        }

        public static string GetDropDownZCodeResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj)
        {
           return list.FirstOrDefault(o => o.question == questionObj.id).response1.zcode;            
        }

        public static int GetDropDownProductTypeResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj, EntitiesDBContext context)
        {
            var description = list.GetDropDownResponse(questionObj);            
            
            return context.pr_getProductAll().FirstOrDefault(o => o.description == description).id;
        }
        public static int GetDropDownSubscriptionTypeResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj, EntitiesDBContext context)
        {
            var description = list.GetDropDownResponse(questionObj);
            return context.subscriptionType.FirstOrDefault(o => o.description == description).id;
        }
        public static int GetDropDownProjectTypeResponse(this List<partnerPartnertypeTouchpointQuestionnaireQuestionResponse> list, question questionObj, EntitiesDBContext context)
        {
            var description = list.GetDropDownResponse(questionObj);
            return context.multiTenantProjectType.FirstOrDefault(o => o.description == description).id;
        }
    }
}