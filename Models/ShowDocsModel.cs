using System;
using System.Collections.Generic;

using System.Linq;
using System.Web;

namespace Generic.Models
{
    public enum PartnerDocType
    {
        
        PDF=1,
        EXCEL,
        PowerPoint,
        WORD,
        IMAGE,
        TEXT,
        Undefined
    }
    public class ShowDocsModel
    {
        public int Id { get; set; }
        public string title { get; set; }
        public string doctype { get; set; }
        public DateTime? uploadDateTime { get; set; }
        public string uploadedBy { get; set; }

        public ShowDocsModel(pptqDoc doc)
        {
            Id = doc.id;
            title = doc.title;
            doctype = ((PartnerDocType)doc.doctype).ToString();
            uploadDateTime = doc.uploadDateTime;
            using (var db = new EntitiesDBContext())
                uploadedBy = db.pr_getPerson(doc.uploadedBy).FirstOrDefault().email;
        }
        public ShowDocsModel()
        {

        }
    }
}