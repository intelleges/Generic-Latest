using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class PersonComboModel
    {
        public PersonComboModel()
        {
            Persons = new List<pr_getPersonByEnterprise1_Result>();
            ComboBoxAttributes = new ComboBoxAttributes();
            DropDownListAttributes = new DropDownListAttributes();
            AutoCompleteAttributes = new AutoCompleteAttributes();
        }

        public IEnumerable<pr_getPersonByEnterprise1_Result> Persons
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