using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Generic.Helpers.Questionnaire;
using Generic.Helpers.Utility;
using Generic.Helpers;

namespace Generic.Areas.RegistrationArea.Services
{
    public static class NextPageCalculationService
    {
        public static int GetJumpToQuestion(question objQuestion, EntitiesDBContext db, int pptq)
        {
            string[] strQuestionLogic = objQuestion.skipLogicJump.Split(';');
            for (int k = 0; k < strQuestionLogic.Length - 1; k++)
            {
                string[] subStrQuestionlogic = strQuestionLogic[k].Split("&|".ToCharArray());
                var resultString = "";
                int gotoQuestionId = 0, gotoELseQuestionId = 0;
                for (int j = 0; j < subStrQuestionlogic.Length; j++)
                {
                    var equalSign = "=";
                    if (subStrQuestionlogic[j].Contains("!="))
                    {
                        equalSign = "!=";
                    }
                    else
                    {
                        equalSign = "=";
                    }
                    string[] strquestionid = subStrQuestionlogic[j].Split(equalSign);
                    int questionidLogic = Convert.ToInt32(strquestionid[0]);
                    string[] strNewQuestionAns = strquestionid[1].Split(':');
                    int ansLogicStatus = 0;
                    if (strNewQuestionAns.Length > 0)
                    {
                        ansLogicStatus = Convert.ToInt32(strNewQuestionAns[0]);
                    }
                    if (strNewQuestionAns.Length > 1)
                    {
                        gotoQuestionId = Convert.ToInt32(strNewQuestionAns[1]);
                    }
                    if (strNewQuestionAns.Length > 2)
                    {
                        gotoELseQuestionId = Convert.ToInt32(strNewQuestionAns[2]);
                    }
                    string answerStatus = "";

                    question questionnew = db.pr_getQuestion(questionidLogic).FirstOrDefault();

                    var context = new EntitiesDBContext();

                    int? rID = context.pr_getPartnerPartnerTypeTouchPointQuestionnaireQuestionResponseByQuestionAndPPTQ(questionidLogic, pptq).FirstOrDefault().response;
                    response responsenew = db.pr_getResponse(rID).FirstOrDefault();
                    if (responsenew != null)
                    {
                        if (responsenew.id > 73 && responsenew.id < 78)
                        {
                            switch (responsenew.id)
                            {
                                case 74: responsenew.id = 1; break;
                                case 75: responsenew.id = 0; break;
                                case 76: responsenew.id = -1; break;
                                case 77: responsenew.id = 2; break;
                            }
                        }
                        //---don't know why, but this was required in past version
                        if (ansLogicStatus == 3)
                        {
                            responsenew.id = 3;
                        }
                        //---
                        var currentResult = (equalSign == "=" ? responsenew.id == ansLogicStatus : responsenew.id != ansLogicStatus);
                        resultString += (currentResult ? "1" : "0");

                        var beginString = "";
                        if (j != subStrQuestionlogic.Length - 1)
                        {
                            for (int n = 0; n <= j; n++)
                                beginString += subStrQuestionlogic[n];
                            var contactSymbol = strQuestionLogic[k].Substring(beginString.Length, 1);
                            resultString += contactSymbol;
                        }
                    }
                }
                bool result = CalculateStack(ReversePolish(resultString)) > 0;
                if (result)
                {
                    return gotoQuestionId;
                }
                else
                {
                    if (gotoELseQuestionId != 0)
                    {
                        return gotoELseQuestionId;
                    }
                }

            }
            return 0;
        }

        private static int CalculateStack(string input)
        {
            int result = 0;
            Stack<int> firstStack = new Stack<int>();
            foreach (var symbol in input)
            {
                int number = 0;

                if (int.TryParse(symbol.ToString(), out number))
                {
                    firstStack.Push(number);
                }
                else
                {
                    if (symbol == '&')
                    {
                        result = firstStack.Pop() * firstStack.Pop();
                    }
                    else
                    {
                        result = firstStack.Pop() + firstStack.Pop();
                    }
                    firstStack.Push(result);
                }
            }
            if (firstStack.Count != 1) throw new Exception("Invalid input");
            return firstStack.Pop();
        }
        private static string ReversePolish(string input)
        {
            string result = "";
            Stack<char> stack = new Stack<char>();
            foreach (var symbol in input.ToCharArray())
            {
                int number = 0;

                if (int.TryParse(symbol.ToString(), out number))
                {
                    result += symbol;
                }
                else
                {
                    while (stack.Count != 0 && GetPriority(stack.Peek()) >= GetPriority(symbol))
                        result += stack.Pop();
                    stack.Push(symbol);
                }
            }
            while (stack.Count != 0)
                result += stack.Pop();
            return result;
        }
        private static int GetPriority(char t)
        {
            if (t == '&') return 1;
            else return 0;
        }
    }
}