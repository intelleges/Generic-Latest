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
}