using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class TouchPointViewModel:view_TouchpointData
    {
        public bool IsSelected { get; set; }
    }

    public class LastLoginCountDetail
    {
        public string MonthYear { get; set; }
        public int LastLoginCount { get; set; }
    }
}