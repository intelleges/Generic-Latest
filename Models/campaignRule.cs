using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic
{
    [MetadataType(typeof(campaignRuleMetadata))]
    public partial class campaignRule
    {
        public int currentProtocol { get; set; }
        public int nextProtocol { get; set; }

        public int touchpointCurrent { get; set; }
        public int touchpointNext { get; set; }

        public int partnerTypeNext { get; set; }       
        public int partnerTypeCurrent { get; set; }


        public bool initTestBool
        {
            get
            {
                return Convert.ToBoolean(initTest);
            }
            set
            {
                initTest = Convert.ToInt32(value);
            }
        }

        public bool initStraightLineBool
        {
            get
            {
                return Convert.ToBoolean(straightline);
            }
            set
            {
                straightline = Convert.ToBoolean(value);
            }

        }

        public sealed class campaignRuleMetadata
        {
            public int id { get; set; }
            [Required(ErrorMessage = " ")]
            public int campaign { get; set; }
            public int initTest { get; set; }

            public int ptqCurrent { get; set; }
            [Required(ErrorMessage = " ")]
            public int status { get; set; }
            [Required(ErrorMessage = " ")]
            public int statusLogic { get; set; }
            [Required(ErrorMessage = " ")]
            public int score { get; set; }
            [Required(ErrorMessage = " ")]
            public int scoreLogic { get; set; }
            [Required(ErrorMessage = " ")]
            public int responseInterval { get; set; }
            [Required(ErrorMessage = " ")]
            public int responseIntervalLogic { get; set; }
            [Required(ErrorMessage = " ")]
            public bool straightline { get; set; }
            [Required(ErrorMessage = " ")]
            public int delayInterval { get; set; }
            [Required(ErrorMessage = " ")]
            public int delayIntervalLogic { get; set; }
            public int ptqNext { get; set; }
            public bool active { get; set; }
            public DateTime hardEndDate  { get; set; }
            public DateTime switchOffDate { get; set; }

            [Required(ErrorMessage = " ")]
            public bool initTestBool { get; set; }
            [Required(ErrorMessage = " ")]
            public bool initStraightLineBool { get; set; }
            [Required(ErrorMessage = " ")]
            public int currentProtocol { get; set; }
            [Required(ErrorMessage = " ")]
            public int nextProtocol { get; set; }
            [Required(ErrorMessage = " ")]
            public int touchpointCurrent { get; set; }
            [Required(ErrorMessage = " ")]
            public int touchpointNext { get; set; }
            [Required(ErrorMessage = " ")]
            public int partnerTypeCurrent { get; set; }
            [Required(ErrorMessage = " ")]
            public int partnerTypeNext { get; set; }
        }
    }
}