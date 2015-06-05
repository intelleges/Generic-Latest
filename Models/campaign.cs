using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic
{
    [MetadataType(typeof(campaignMetadata))]
    public partial class campaign
    {

        public sealed class campaignMetadata
        {
            [Required(ErrorMessage = " ")]
            public string description { get; set; }            
        }
    }
}