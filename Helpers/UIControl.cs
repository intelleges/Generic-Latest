using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Generic.Helpers
{
    public class UIControl
    {
        public class MyRadioButtonList :
System.Web.UI.WebControls.RadioButtonList
        {
          public  string tableNewAttributes { get; set; }
            protected override void Render(HtmlTextWriter writer)
            {
                var attr = this.Attributes;

//                <td class="brownbg" align="right" style="width:15%;"><input id="question_504_1206_0" type="radio" name="question_504_1206" value="74" data-val="true" data-val-required="Required" class="input-validation-error"><label for="question_504_1206_0" data-val="true" data-val-required="Required">Yes</label><br>

//            <input id="question_504_1206_1" type="radio" name="question_504_1206" value="75" data-val="true" data-val-required="Required" class="input-validation-error"><label for="question_504_1206_1" data-val="true" data-val-required="Required">No</label>

//        <br></td>


//<td class="brownbg" align="right" style="width:15%;"><table id="question_501_1203" onclick="showdivnew(this);removevalidation(this.id) ">
//            <tbody><tr>
//                <td><input id="question_501_1203_0" type="radio" name="question_501_1203" value="74" data-val="true" data-val-required="Required"><label for="question_501_1203_0">Yes</label></td><td><input id="question_501_1203_1" type="radio" name="question_501_1203" value="75" data-val="true" data-val-required="Required"><label for="question_501_1203_1">No</label></td>
//            </tr>
//        </tbody></table></td>


                //RenderChildren(writer);
                
                int i = 0;
                foreach (ListItem listItem in Items)
                {
                    if (i == 0)
                    {
                        writer.WriteBeginTag("table");
                        writer.WriteAttribute("ID", this.UniqueID);
                        try
                        {
                            writer.WriteAttribute("onclick", attr["onclick"]);
                        }
                        catch { }
                        try
                        {
                            writer.WriteAttribute("checked", attr["checked"]);

                        }
                        catch { }
                        writer.Write('>');
                        writer.WriteBeginTag("tbody"); writer.Write('>');
                        writer.WriteBeginTag("tr"); writer.Write('>');
                      
                    }
                   
                    writer.WriteBeginTag("td"); writer.Write('>');
                    writer.WriteBeginTag("input");
                    writer.WriteAttribute("ID", this.UniqueID + "_" + i);
                    writer.WriteAttribute("type", "radio");
                    writer.WriteAttribute("name", this.UniqueID);
                    writer.WriteAttribute("value", listItem.Value, true);
                    listItem.Attributes.Render(writer);
                    writer.Write('>');
                    writer.WriteEndTag("input");
                    writer.WriteBeginTag("label");
                    writer.WriteAttribute("for", this.UniqueID + "_" + i);
                    listItem.Attributes.Render(writer);
                    writer.Write('>');
                    HttpUtility.HtmlEncode(listItem.Text, writer);
                    writer.WriteEndTag("label");
                    if (i < Items.Count - 1)
                    {
                        writer.RenderBeginTag(HtmlTextWriterTag.Br);
                        writer.WriteEndTag("td");
                    }
                    writer.WriteLine();
                    i++;
                }
                writer.WriteEndTag("tr");
                writer.WriteEndTag("tbody");
                writer.WriteEndTag("table");
               // writer.WriteEndTag("table");
            }
        }
    }
}
