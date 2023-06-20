using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.ViewModel
{
    public class QuestionnaireViewModel
    {
        public int ProtocolId { get; set; }
        public SelectList ProtocolList { get; set; }


    }
    public class MappedQuestionnaireList
    {
        public int QuestId { get; set; }
        public string QuestTitle { get; set; }
    }
    public class HardCodedModel
    {
        public int PreviousQuestion { get; set; }
        public int PreviousResponse { get; set; }
        public int CurrentQuestion { get; set; }
        public int CurrentResponse { get; set; }
    }
}