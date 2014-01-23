using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Telerik.Web.Mvc;

namespace Generic.Models
{
    public class TouchpointComboModel
    {
        public TouchpointComboModel()
        {
            Touchpoints = new List<touchpoint>();
            ComboBoxAttributes = new ComboBoxAttributes();
            DropDownListAttributes = new DropDownListAttributes();
            AutoCompleteAttributes = new AutoCompleteAttributes();
        }
        public IEnumerable<touchpoint> Touchpoints
        {
            get;
            set;
        }
        public AutoCompleteAttributes AutoCompleteAttributes
        {
            get;
            set;
        }
        public ComboBoxAttributes ComboBoxAttributes
        {
            get;
            set;
        }
        public DropDownListAttributes DropDownListAttributes
        {
            get;
            set;
        }
    }
}