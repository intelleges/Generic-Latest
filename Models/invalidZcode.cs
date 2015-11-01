using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;


namespace Generic
{

    [MetadataType(typeof(invalideZcodeMetadata))]
    public partial class invalidZcode
    {
        public int currentProtocol { get; set; }
        public int touchpointCurrent { get; set; }
        public int partnerTypeCurrent { get; set; }

        //public string zCode { get; set; }
        //public int zCodeActionType { get; set; }


        public sealed class invalideZcodeMetadata
        {
         //   public int id { get; set; }
         //   public int ptq { get; set; }
            [Required(ErrorMessage = " ")]
            public int currentProtocol { get; set; }
            [Required(ErrorMessage = " ")]
            public int touchpointCurrent { get; set; }
            [Required(ErrorMessage = " ")]
            public int partnerTypeCurrent { get; set; }
            [Required(ErrorMessage = " ")]
            public string zCode { get; set; }
            [Required(ErrorMessage = " ")]
            public int zCodeActionType { get; set; }
        //    public int sortOrder { get; set; }
         //   public bool active { get; set; }


        }
    }
   
}