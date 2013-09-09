using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace Generic.Helpers
{
    public class ClientJavascriptHelper
    {
        /// <summary> 
        ///This helper method will search for Javascript in a loaded project and
        ///register the scripts. 
        ///This has not been implemented as an htmlHelper extension, as info on
        ///the web shows it will
        ///run a lot quicker.
        ///</summary> 
        /// <returns></returns> 
        public static HtmlString LoadClientJavascript()
        {
            StringBuilder clientScriptsBuilder = new StringBuilder();
            // All client specific files must be placed in a folder called
            // "Javascript" (we can change this if we
            //want) 
            DirectoryInfo directory = new DirectoryInfo(HttpContext.Current.Server.MapPath(@"~\Javascript"));
            // If there is no folder or no files in the folder, don’t do anything 
            if (directory == null || directory.GetFiles() == null)
                return new HtmlString(clientScriptsBuilder.ToString());
            // Get all the files in the folder 
            var files = directory.GetFiles().ToList();
            // Loop through the files in the folder. Register each one as a script
            // in the page. 
            foreach (var file in files)
            {
                string script = @"/Javascript/" + file;
                string jswrap = String.Format("<script type=\"text/javascript\" src=\"" + script + "\"></script>");
                clientScriptsBuilder.AppendLine(jswrap);
            }
            // return the code as HTML (otherwise the Html.Encode will just output
            // it as text in the UI) 
            return new HtmlString(clientScriptsBuilder.ToString());
        }
    }
}