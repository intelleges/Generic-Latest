using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class LCEModel
    {
        [Required]
        [DisplayName("Project Name")]
        public string ProgramName { get; set; }
        [Required]
        [DisplayName("Designation")]
        public string Designation { get; set; }
        [Required]
        [DisplayName("Owner")]
        public int Owner { get; set; }

        [DisplayName("Activity Type")]
        [Required]
        public int partnertype { get; set; }

        [DisplayName("From")]
        [Required]
        public string From { get; set; }

        [DisplayName("Project Url")]
        [Required]
        public string ProjectUrl { get; set; }

        [DisplayName("Score (UAT ONLY)")]
        public int? Score { get; set; }

        [DisplayName("Priority (UAT ONLY)")]
        public int? Priority { get; set; }

        [DisplayName("To")]
        [Required]
        public string To { get; set; }

        [DisplayName("CFDB Upload")]
        // [Required]
        public HttpPostedFileBase File { get; set; }
        public string FileName { get; set; }

        [DisplayName("LC&E Scope Upload")]
        // [Required]
        public HttpPostedFileBase FileScope { get; set; }
        public string FileScopeName { get; set; }

        [DisplayName("CID Upload")]
        // [Required]
        public HttpPostedFileBase FileCID { get; set; }
        public string FileCIDName { get; set; }
        [DisplayName("Entanglement Upload")]
        // [Required]
        public HttpPostedFileBase FileEntanglement { get; set; }
        public string FileEntanglementName { get; set; }
        [DisplayName("Supplier Self-Assessment Upload")]
        // [Required]
        public HttpPostedFileBase SupplierSelfAssessmentUpload { get; set; }
        public string SupplierSelfAssessmentUploadName { get; set; }
        [DisplayName("BAA Diligence File Upload")]
        // [Required]
        public HttpPostedFileBase BAATransitionScopeUpload { get; set; }
        public string BAATransitionScopeUploadName { get; set; }
        [Required]
        [DisplayName("Due date")]
        public DateTime? Duedate { get; set; }

        public string Comments { get; set; }

        public string BuyToBuyType { get; set; }
    }
}