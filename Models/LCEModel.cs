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
        [DisplayName("Program Name")]
        public string ProgramName { get; set; }
        [Required]
        [DisplayName("Designation")]
        public string Designation { get; set; }
        [Required]
        [DisplayName("Owner")]
        public int Owner { get; set; }

        [Required]
        [DisplayName("Is China Involved?")]
        public int IsChinaInvolved { get; set; }


        [DisplayName("Activity Type")]
        [Required]
        public int partnertype { get; set; }

        [DisplayName("Region")]
        [Required]
        public int Region { get; set; }

        [DisplayName("From")]
        [Required]
        public int From { get; set; }

        [DisplayName("To")]
        [Required]
        public int To { get; set; }

        [DisplayName("CFDB Upload")]
        [Required]
        public HttpPostedFileBase File { get; set; }

        [DisplayName("LC&E Scope Upload")]
        [Required]
        public HttpPostedFileBase FileScope { get; set; }

        [Required]
        [DisplayName("Due date")]
        public DateTime? Duedate { get; set; }

    }
}