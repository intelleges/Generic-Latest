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
       
        public int touchpointCurrent { get; set; }
       
        public int touchpointNext { get; set; }
       
        public int parnerTypeNext { get; set; }
        
        public int parnerTypeCurrent { get; set; }


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
        public sealed class campaignRuleMetadata
        {
            [Required(ErrorMessage = " ")]
            public bool initTestBool { get; set; }
            [Required(ErrorMessage = " ")]
            public int touchpointCurrent { get; set; }
            [Required(ErrorMessage = " ")]
            public int touchpointNext { get; set; }
            [Required(ErrorMessage = " ")]
            public int parnerTypeNext { get; set; }
            [Required(ErrorMessage = " ")]
            public int parnerTypeCurrent { get; set; }
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

            public bool straightline { get; set; }
             [Required(ErrorMessage = " ")]
            public int delayInterval { get; set; }
             [Required(ErrorMessage = " ")]
            public int delayIntervalLogic { get; set; }
            public int ptqNext { get; set; }
            public int sortOrder { get; set; }
            public bool active { get; set; }
        }
    }
}