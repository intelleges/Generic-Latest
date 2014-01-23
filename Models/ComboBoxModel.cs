using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class ComboBoxModel
    {

        public ComboBoxModel()
        {           
            ComboBoxAttributes = new ComboBoxAttributes();
            DropDownListAttributes = new DropDownListAttributes();
            AutoCompleteAttributes = new AutoCompleteAttributes();

            this.AutoCompleteAttributes.Width =  200;
            this.AutoCompleteAttributes.HighlightFirst =  true;
            this.AutoCompleteAttributes.AutoFill =  false;
            this.AutoCompleteAttributes.AllowMultipleValues =  true;
            this.AutoCompleteAttributes.MultipleSeparator =  ", ";
            this.ComboBoxAttributes.Width =  200;
            this.ComboBoxAttributes.SelectedIndex =  0;
            this.ComboBoxAttributes.HighlightFirst =  true;
            this.ComboBoxAttributes.AutoFill =  true;
            this.ComboBoxAttributes.OpenOnFocus =  false;
            this.DropDownListAttributes.Width =  200;
            this.DropDownListAttributes.SelectedIndex = 0; this.ComboBoxAttributes.FilterMode = 0;
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