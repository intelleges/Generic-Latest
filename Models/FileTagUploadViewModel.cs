using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class FileTagUploadViewModel
    {
        public HttpPostedFileBase TagFile { get; set; }

        public string TagName { get; set; }

        public int QuestionId { get; set; }
    }
}