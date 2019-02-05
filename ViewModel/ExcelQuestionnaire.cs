using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ExcelQuestionnaire
    {
        public int QID { get; set; }
        public string Question { get; set; }
        public string Title { get; set; }
        public string Required { get; set; }
        public string skipLogicJump { get; set; }
        public string skipLogicAnswer { get; set; }
        public string Comment { get; set; }
        public string CommentBoxMessageText { get; set; }
        public string CalendarMessageText { get; set; }
        public string SubCheckBoxChoice { get; set; }
        public string UploadMessageText { get; set; }
        public string CommentType { get; set; }
        public string snipOffQuestionnaire { get; set; }
        public int spinoffid { get; set; }
        public string emailalert { get; set; }
        public string emailalertlist { get; set; }

        public string skipLogic { get; set; }
        public int Length { get; set; }
        public int titleLength { get; set; }
        public int yValue { get; set; }
        public int nValue { get; set; }
        public int naValue { get; set; }
        public int otherValue { get; set; }
        public int qWeight { get; set; }

        public int Page { get; set; }
        public string Surveyset { get; set; }
        public string Survey { get; set; }
        public string Response { get; set; }
		public string NarrativeHint { get; set; }
        public int accessLevel { get; set; }

    }

    public class ExcelQuestionnaireCMS
    {
        public string ITEM { get; set; }
        public string TEXT { get; set; }
        public string LINK { get; set; }
    }

}