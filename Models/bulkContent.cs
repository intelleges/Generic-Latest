using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic
{
    [MetadataType(typeof(bulkContentMetadata))]
    public partial class bulkContent
    {
        public int Industry { get; set; }
        public int Focus { get; set; }
        public int PartnerTypeList { get; set; }
        public int TouchPointList { get; set; }
        public class bulkContentMetadata
        {
            [Required(ErrorMessage = " ")]
            public int Industry { get; set; }
            [Required(ErrorMessage = " ")]
            public int Focus { get; set; }
            [Required(ErrorMessage = " ")]
            public int PartnerTypeList { get; set; }
            [Required(ErrorMessage = " ")]
            public int TouchPointList { get; set; }
        }
    }
}