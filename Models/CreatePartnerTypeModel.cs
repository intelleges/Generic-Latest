using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class CreatePartnerTypeModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Alias { get; set; }
        [Required]
        public string Description { get; set; }
    }
}