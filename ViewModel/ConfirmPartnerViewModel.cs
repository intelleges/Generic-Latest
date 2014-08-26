using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ConfirmPartnerViewModel : view_PartnerConfirmationData
    {
        public bool IsSelected1 { get; set; }
        public bool IsSelected2 { get; set; }
        public bool IsCheckboxSelected { get; set; }
       
    }
}