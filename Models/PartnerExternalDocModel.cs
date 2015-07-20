using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class PartnerExternalDocModel
    {
        public int id { get; set; }
        public string title { get; set; }
        public string uploadedFileType { get; set; }
        public string updatedBy { get; set; }
        public PartnerExternalDocModel(pr_getPartnerDocs_Result model, string partnerEmail)
        {
            id = model.id;
            title = model.title;
            uploadedFileType = model.uploadedFileType;
            updatedBy = partnerEmail;
        }
        public PartnerExternalDocModel()
        {

        }
    }
}