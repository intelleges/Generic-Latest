using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Helpers
{
    public static class ExtentionMethods
    {
        public static string[] Split(this string str, string splitter)
        {
            var list = new List<string>();
            list.Add(splitter);
            return str.Split(list.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }
    }
    
}