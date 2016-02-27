using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Generic.Helpers.Utility
{
    public class EmailFormat
    {


        public EmailFormat()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        public string sGetEmailBody(string sEmailBody, person sender, partner partner, enterprise enterprise, touchpoint touchpoint)
        {
            // touchpoint = touchpoint.gettouchpoint();
            //enterprise = enterprise;
            return this.sGetResult(sEmailBody, sender, null, partner, enterprise, touchpoint);
        }

        public string sGetEmailBody(string sEmailBody, person sender, person receiver, enterprise enterprise)
        {
            // enterprise = enterprise;
            return this.sGetResult(sEmailBody, sender, receiver, null, enterprise, null);
        }

        public string sGetEmailBody(string sEmailBody, person Sender, person receiver)
        {
            return this.sGetResult(sEmailBody, Sender, receiver, null, null, null);
        }

        public string sGetEmailBody(string sEmailBody, person sender, partner partner)
        {
            return this.sGetResult(sEmailBody, sender, null, partner, null, null);
        }

        public string sGetEmailBody(string sEmailBody, person sender, partner partner, touchpoint touchpoint, int ptq)
        {
            return this.sGetResult(sEmailBody, sender, null, partner, null, touchpoint, ptq);
        }

        public string sGetEmailBody(string sEmailBody, person sender, partner partner, enterprise enterprise, touchpoint touchpoint, int ptq)
        {
            return this.sGetResult(sEmailBody, sender, null, partner, enterprise, touchpoint, ptq);
        }
        public string sGetEmailBody(string sEmailBody, person sender, person receiver, touchpoint touchpoint, enterprise enterprise, person systemmaster)
        {
            return this.sGetResult(sEmailBody, sender, receiver, null, enterprise, touchpoint, 0, systemmaster);
        }
        public string sGetEmailBody(string sEmailBody, person sender, person receiver, partner partner, touchpoint touchpoint, enterprise enterprise, person systemmaster, int ptq)
        {
            return this.sGetResult(sEmailBody, sender, receiver, partner, enterprise, touchpoint, ptq, systemmaster);
        }

        public string sGetEmailBody(string sEmailBody, iteratePartner partner, iteratePerson person, person currentPerson, enterprise currentEnterprise)
        {
            EntitiesDBContext db = new EntitiesDBContext();
            Regex regex = new Regex(@"\[(.*?)\]");
            //comment add sebody
            if (sEmailBody == "" || sEmailBody == null)
            {
                sEmailBody = "Survey has been completed";
            }
            MatchCollection collection = regex.Matches(sEmailBody);
            string sVariable = "";
            string sValue = "";
            foreach (Match match in collection)
            {
                sVariable = match.ToString();
                switch (sVariable.ToLower())
                {
                    case "[personfirstname]":
                        sValue = currentPerson.firstName;
                        break;
                    case "[personfullname]":
                        sValue = currentPerson.FullName;
                        break;
                    case "[persontitle]":
                        sValue = currentPerson.title;
                        break;
                    case "[enterpriseapplicationpath]":
                        sValue = currentEnterprise.applicationPath;
                        break;
                    case "[enterprisecompanyname]":
                        sValue = currentEnterprise.companyName;
                        break;
                    case "[linkedinlink]":
                        //sValue = currentEnterprise.companyName;
                        break;
                    case "[twitterlink]":
                        //sValue = currentEnterprise.companyName;
                        break;
                    case "[personphone]":
                        sValue = currentPerson.phone;
                        break;
                    case "[personemail]":
                        sValue = currentPerson.email;
                        break;
                    case "[iteratepersontitle]":
                        sValue = person.title;
                        break;
                    case "[iteratepartnername]":
                        sValue = partner.name;
                        break;                   
                    default:
                        if (sVariable.Contains("[forward to"))
                        {
                            var splitted = sVariable.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitted.Length > 1)
                            {

                                sValue = "";
                            }
                            else sValue = "";
                        }
                        else if (sVariable.Contains("[unsubscribe"))
                        {
                            var splitted = sVariable.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitted.Length > 1)
                            {

                                sValue = "";
                            }
                            else sValue = "";

                        } else 
                            sValue = sVariable;
                        break;
                }
                //replace emailBody's variable with real data
                sEmailBody = sEmailBody.Replace(sVariable, sValue);
            }
            return sEmailBody;
        }



        private string sGetResult(string sEmailBody, person sender, person receiver, partner partner, enterprise enterprise, touchpoint touchpoint, int ptq = 0, person systemmaster = null)
        {
            EntitiesDBContext db = new EntitiesDBContext();
            Regex regex = new Regex(@"\[(.*?)\]");
            //comment add sebody
            if (sEmailBody == "" || sEmailBody == null)
            {
                sEmailBody = "Survey has been completed";
            }
            MatchCollection collection = regex.Matches(sEmailBody);
            string sVariable = "";
            string sValue = "";
            var hintTagsRegex = new Regex(@"<[Hh][Ii][Nn][Tt]\b[^>]*>(.*?)</[Hh][Ii][Nn][Tt]>");
            var hints = hintTagsRegex.Matches(sEmailBody);
            foreach(var match in hints)
            {
                sVariable = match.ToString();
                sValue = sVariable;
                var attributesREgex = new Regex("(?<=text=[\"'])([\\s\\d\\w\\W\\S])+(?=['\"])");
                var text = attributesREgex.Match(sVariable);
                if(text.Success)
                {
                    var elementTextExpression = new Regex("(?<=>)([\\s\\d\\w\\W\\S])+(?=</)");
                    var innerText = elementTextExpression.Match(sVariable);
                    if(innerText.Success)
                    {
                        sValue = string.Format("<a href='#' data-toggle='popover' title='{1}' data-placement='top'>{0}</a>", innerText.Value, HttpUtility.HtmlEncode(text.Value));
                    }
                }



                sEmailBody = sEmailBody.Replace(sVariable, sValue);

            }
             sVariable = "";
             sValue = "";
            //iterate each variale found
            foreach (Match match in collection)
            {
                sVariable = match.ToString();

                switch (sVariable.ToLower())
                {
                    case "[receiver full name]":
                        sValue = this.sGetpersonFullName(receiver);
                        break;
                    case "[receiver title]":
                        sValue = this.sGetpersonTitle(receiver);
                        break;
                    case "[receiver first name]":
                        sValue = this.sGetpersonFirstName(receiver);
                        break;
                    case "[receiver last name]":
                        sValue = this.sGetpersonLastName(receiver);
                        break;
                    case "[receiver phone]":
                        sValue = this.sGetpersonPhone(receiver);
                        break;
                    case "[receiver email]":
                        sValue = this.sGetpersonEmail(receiver);
                        break;
                    case "[receiver role]":
                        sValue = this.sGetpersonRole(receiver);
                        break;
                    case "[receiver password]":
                        sValue = this.sGetpersonPassword(receiver);
                        break;
                    case "[sender full name]":
                        sValue = this.sGetpersonFullName(sender);
                        break;
                    case "[sender title]":
                        sValue = this.sGetpersonTitle(sender);
                        break;
                    case "[sender first name]":
                        sValue = this.sGetpersonFirstName(sender);
                        break;
                    case "[sender last name]":
                        sValue = this.sGetpersonLastName(sender);
                        break;
                    case "[sender phone]":
                        sValue = this.sGetpersonPhone(sender);
                        break;
                    case "[sender email]":
                        sValue = this.sGetpersonEmail(sender);
                        break;
                    case "[sender role]":
                        sValue = this.sGetpersonRole(sender);
                        break;
                    case "[partner encrypted Id]":
                        sValue = this.sGetEncryptedpartnerId(partner);
                        break;
                    case "[partner name]":
                        sValue = this.sGetpartnerName(partner);
                        break;
                    case "[partner address]":
                        sValue = this.sGetpartnerAddress(partner);
                        break;
                    case "[partner contact full name]":
                        sValue = this.sGetpartnerContactFullName(partner);
                        break;
                    case "[partner contact first name]":
                        sValue = this.sGetpartnerContactFirstName(partner);
                        break;
                    case "[partner contact last name]":
                        sValue = this.sGetpartnerContactLastName(partner);
                        break;
                    case "[partner contact email]":
                        sValue = this.sGetpartnerContactEmail(partner);
                        break;
                    case "[partner contact title]":
                        sValue = this.sGetpartnerContactTitle(partner);
                        break;
                    case "[partner contact phone]":
                        sValue = this.sGetpartnerContactPhone(partner);
                        break;
                    case "[partner contact fax]":
                        sValue = this.sGetpartnerContactFax(partner);
                        break;
                    case "[partner hro full name]":
                        sValue = this.sGetpartnerHROFullName(partner);
                        break;
                    case "[partner hro first name]":
                        sValue = this.sGetpartnerHROFirstName(partner);
                        break;
                    case "[partner hro last name]":
                        sValue = this.sGetpartnerHROLastName(partner);
                        break;
                    case "[partner hro email]":
                        sValue = this.sGetpartnerHROEmail(partner);
                        break;
                    case "[partner hro phone]":
                        sValue = this.sGetpartnerContactPhone(partner);
                        break;
                    case "[partner hro title]":
                        sValue = this.sGetpartnerHROTitle(partner);
                        break;
                    case "[partner hro fax]":
                        sValue = this.sGetpartnerHROFax(partner);
                        break;
                    case "[partner access code]":
                        sValue = this.sGetpartnerAccessCode(partner, touchpoint, ptq);
                        break;
                    case "[partner code]":
                        sValue = this.sGetpartnerAccessCode(partner, touchpoint, ptq);
                        sValue = sValue.Substring(1, 4);
                        break;
                    case "[partner Type]":
                        sValue = this.sGetpartnerType(partner);
                        break;
                    case "[3yearsfromcompleteddate]":
                        var pptqFromCompeleted = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partner.id, ptq).FirstOrDefault();
                       // if (!pptqFromCompeleted.completedDate.HasValue || pptqFromCompeleted.completedDate < DateTime.Parse("01.01.1920"))
                            //sValue = "NOT COMPLETED";
                        //else 
                        sValue = pptqFromCompeleted.completedDate.HasValue && pptqFromCompeleted.completedDate > DateTime.Parse("01.01.1920") ? pptqFromCompeleted.completedDate.Value.AddYears(3).ToShortDateString() : DateTime.Now.AddYears(3).ToShortDateString();
                        break;

                    case "[touchpoint]":
                        sValue = touchpoint.title;
                        break;
                    case "[enterprise]":
                        sValue = enterprise.description;
                        break;
                    case "[protocol]":
                        //sValue = touchpoint.protocol.getProtocol().description;
                        sValue = "";
                        break;
                    case "[group]":
                        break;
                    case "[due date]":
                        var pptq = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partner.id, ptq).FirstOrDefault();

                        //var t = pptq;
                        //var dueDate = db.pr_getDueDateByPPTQ(pptq.id).FirstOrDefault();
                        if (pptq.dueDate != null)
                            sValue = Convert.ToDateTime(pptq.dueDate).ToString("d");
                        else
                            sValue = pptq.invitedDate.AddDays(30).ToString("d");
                        //sValue = partner.getDueDateByInitialInvitation(partner, touchpoint).ToShortDateString();
                        break;
                    case "[start date]":
                        var _invitedDate = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partner.id, ptq).FirstOrDefault().invitedDate;
                        if (_invitedDate != null)
                            sValue = _invitedDate.ToString("d");
                        else
                            sValue = DateTime.Now.ToShortDateString();
                        break;
                    case "[partner owner full name]":
                        sValue = this.sGetpartnerOwnerFullName(partner);
                        break;
                    case "[partner owner email]":
                        sValue = this.sGetpartnerOwnerEmail(partner);
                        break;
                    case "[partner email and password]":
                        sValue = this.sGetpartnerEmailandPassword(partner);
                        break;

                    case "[enterprise name]":
                        sValue = enterprise.description;
                        break;

                    case "[touchpoint title]":
                        sValue = touchpoint.description;
                        break;
                    case "[user firstname]":
                        sValue = receiver.firstName;
                        break;
                    case "[touchpoint purpose]":
                        sValue = touchpoint.purpose;
                        break;
                    case "[user email]":
                        sValue = receiver.email;
                        break;
                    case "[temporary access code]":
                        sValue = receiver.passWord;
                        break;
                    case "[project url]":
                        sValue = this.sGetProjectUrl(enterprise);
                        break;
                    case "[user inviting email]":
                        sValue = sender.email;
                        break;
                    case "[user inviting firstname]":
                        sValue = sender.firstName;
                        break;
                    case "[user inviting last name]":
                        sValue = sender.lastName;
                        break;
                    case "[system master email]":
                        sValue = systemmaster.email;
                        break;
                    case "[system master fullname]":
                        sValue = systemmaster.FullName;
                        break;
                    case "[hon internal id]":
                        sValue = partner.name;
                        break;
                    case "[partnumber]":
                        if (HttpContext.Current.Session["partnumber"] != null && HttpContext.Current.Session["partnumber"] != "0" && HttpContext.Current.Session["partnumber"] != "")
                        {
                            int partid = Convert.ToInt32(HttpContext.Current.Session["partnumber"].ToString());
                            if (partid != 0)
                            {
                                var partnumber = db.pr_getPartnumber(partid).FirstOrDefault();
                                var details = db.pr_getPartnumberDetail(partid).FirstOrDefault();
                                if (details != null)
                                    sValue = string.Format("<a href='#' data-toggle='popover' title='{1}' data-placement='top'>{0}</a>", partnumber.description, HttpUtility.HtmlEncode(details.description));
                                else sValue = partnumber.description;
                              
                            }
                        }
                        break;
                    case "[next partnumber]":
                        if (HttpContext.Current.Session["NextPartnumber"] != null)
                        {
                            int partid = Convert.ToInt32(HttpContext.Current.Session["NextPartnumber"].ToString());
                            if (partid != 0)
                            {
                                sValue = db.pr_getPartnumber(partid).FirstOrDefault().description;
                            }
                        }
                        else
                        {
                            int partid = Convert.ToInt32(HttpContext.Current.Session["partnumber"].ToString());
                            if (partid != 0)
                            {
                                var partnumber = db.pr_getPartnumber(partid).FirstOrDefault();
                                var details = db.pr_getPartnumberDetail(partid).FirstOrDefault();
                                if (details != null)
                                    sValue = string.Format("<a href='#' data-toggle='popover' title='{1}' data-placement='top'>{0}</a>", partnumber.description, HttpUtility.HtmlEncode(details.description));
                                else sValue = partnumber.description;
                              
                            }
                            return "You have completed the questions for " + sValue + ", please select YES to COMPLETE this questionnaire by providing an eSignature, or NO to leave the QUESTIONNAIRE INCOMPLETE and return to change your answers for " + sValue + ".";
                        }
                        break;
                    case "[first name]":
                        sValue = partner.firstName;
                        break;
                    case "[last name]":
                        sValue = partner.lastName;
                        break;
                    case "[firstname]":
                        sValue = partner.firstName;
                        break;
                    case "[lastname]":
                        sValue = partner.lastName;
                        break;
                    case "[title]":
                        sValue = partner.title;
                        break;
                    case "[email address]":
                        sValue = partner.email;
                        break;
                    case "[phone number]":
                        sValue = partner.phone;
                        break;
                    case "[partner country]":
                        sValue =db.pr_getCountry(partner.country).FirstOrDefault().name;
                        break;
                    case "[partner address one]":
                        sValue = partner.address1;
                        break;
                    case "[partner_city]":
                        sValue = partner.city;
                        break;
                    case "[partner_state]":
                        sValue =db.pr_getStateByID(partner.state).FirstOrDefault().name;
                        break;
                    case "[partner zip code]":
                        sValue = partner.zipcode;
                        break;
                    case "[company url]":
                        sValue = partner.dunsNumber;
                        break;
                    //case "[partner_state]":
                    //    sValue = db.pr_getStateByID(partner.state).FirstOrDefault().name;
                    //    break;
                    default:
                        var lowered = sVariable.ToLower();
                        if (lowered.Contains("[registration link standard"))
                        {
                            var splitted = sVariable.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitted.Length > 1)
                            {

                                sValue = this.sGetRegistrationLink(this.sGetpartnerAccessCode(partner, touchpoint, ptq), this.sGetProjectUrl(enterprise), false, splitted[1]);
                            }
                            else sValue = this.sGetRegistrationLink(this.sGetpartnerAccessCode(partner, touchpoint, ptq), this.sGetProjectUrl(enterprise), false);
                        }
                        else if (lowered.Contains("[registration link advanced"))
                        {
                            var splitted = sVariable.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (splitted.Length > 1)
                            {
                                sValue = this.sGetRegistrationLink(this.sGetpartnerAccessCode(partner, touchpoint, ptq), this.sGetProjectUrl(enterprise), true, splitted[1]);
                            }
                            else sValue = this.sGetRegistrationLink(this.sGetpartnerAccessCode(partner, touchpoint, ptq), this.sGetProjectUrl(enterprise), true);
                        }
                        else if (lowered.Contains("[unsubscribe"))
                        {
                            var splitted = sVariable.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            sValue = "<a href='" + this.sGetProjectUrl(enterprise) + "/Registration/Home/Unsubscribe/" + partner.id + "'>{0}</a>";
                            sValue = splitted.Length > 1 ? string.Format(sValue, splitted[1].Replace("]", "")) : "click here";
                        }
                        else if (lowered.Contains("[forward to"))
                        {
                            var splitted = sVariable.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            sValue = "<a href='" + this.sGetProjectUrl(enterprise) + "/wrongContact?AccessCode="+this.sGetpartnerAccessCode(partner,touchpoint,ptq)+"'>{0}</a>";
                            //sValue = "<a href='" + this.sGetProjectUrl(enterprise) + "/Registration/Home/Unsubscribe/" + partner.id + "'>{0}</a>";
                            sValue = splitted.Length > 1 ? string.Format(sValue, splitted[1].Replace("]", "")) : "click here";
                        }
                        else
                            sValue = sVariable;
                        break;
                }
                //replace emailBody's variable with real data
                sEmailBody = sEmailBody.Replace(sVariable, sValue);
            }
            return sEmailBody;
        }

        public string sGetRegistrationLink(string accessCode, string projectUrl, bool advanced, string linkText = null)
        {
            if (string.IsNullOrEmpty(linkText)) linkText = "link";
            else
            {
                linkText = linkText.Replace("]", "");
            }
            if (advanced)
                return "<a href='" + projectUrl + "/Registration/?accessCode=" + accessCode + "&advanced=true' >" + linkText + "</a>";
            else return "<a href='" + projectUrl + "/Registration/?accessCode=" + accessCode + "'>" + linkText + "</a>";
        }

        private string sGetProjectUrl(enterprise enterprise)
        {
            if (enterprise == null)
            {
                return "https://www.intelleges.com/mvcmt/Generic";
            }
            else
            {
                if (enterprise.multiTenantProjectType == 1)
                {
                    return "https://www.intelleges.com/mvcmt/Generic";
                }
                else if (enterprise.multiTenantProjectType == 2)
                {
                    return "https://www.intelleges.com/mvcmt/BAA";
                }
                else
                {
                    return "https://www.intelleges.com/mvcmt/Generic";
                }
            }
        }
        private string sGetpersonFullName(person person)
        {
            return person.firstName + " " + person.lastName;
        }

        private string sGetpersonFirstName(person person)
        {
            return person.firstName;
        }

        private string sGetpersonLastName(person person)
        {
            return person.lastName;
        }

        private string sGetpersonTitle(person person)
        {
            return person.title;
        }

        private string sGetpersonRole(person person)
        {
            //   return person.role.getRoleDetail().description;
            return "";
        }

        private string sGetpersonPhone(person person)
        {
            return person.phone;
        }

        private string sGetpersonFax(person person)
        {
            return person.fax;
        }

        private string sGetpersonEmail(person person)
        {
            return person.email;
        }

        private string sGetpersonPassword(person person)
        {
            return person.passWord;
        }

        private string sGetEncryptedpartnerId(partner partner)
        {
            return partner.id.ToString("X");
        }

        private string sGetpartnerName(partner partner)
        {
            return partner.name;
        }

        private string sGetpartnerContactFullName(partner partner)
        {
            if (partner != null)
            {
                return partner.firstName + " " + partner.lastName;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerContactFirstName(partner partner)
        {
            if (partner != null)
            {
                return partner.firstName;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerContactLastName(partner partner)
        {
            if (partner != null)
            {
                return partner.lastName;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerContactEmail(partner partner)
        {
            if (partner != null)
            {
                return partner.email;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerContactTitle(partner partner)
        {
            if (partner != null)
            {
                return partner.title;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerContactPhone(partner partner)
        {
            if (partner != null)
            {
                return partner.phone;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerContactFax(partner partner)
        {
            if (partner != null)
            {
                return partner.fax;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerAddress(partner partner)
        {
            if (partner.country != null)
            {
                string address = partner.address1 + Environment.NewLine;
                if (!string.IsNullOrEmpty(partner.address2))
                {
                    address += partner.address2 + Environment.NewLine;
                }
                address += partner.city + ", ";
                if (partner.country == 1)
                {
                    // address += partner.state.getState().code + " " + partner.zipcode + Environment.NewLine;
                    address = "";
                }
                else
                {
                    address += partner.province + " " + partner.zipcode + Environment.NewLine;
                }
                //address += partner.country.getCountryById().name;
                address = "";
                return address;
            }
            else
            {
                return "";
            }
        }

        private string sGetpartnerHROFullName(partner partner)
        {
            //if (partner.relationshipOwner != null)
            //{
            //    return partner.relationshipOwner.firstName + " " + partner.relationshipOwner.lastName;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }

        private string sGetpartnerHROFirstName(partner partner)
        {
            //if (partner.relationshipOwner != null)
            //{
            //    return partner.relationshipOwner.firstName;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }

        private string sGetpartnerHROLastName(partner partner)
        {
            //if (partner.relationshipOwner != null)
            //{
            //    return partner.relationshipOwner.lastName;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }

        private string sGetpartnerHROEmail(partner partner)
        {
            //if (partner.relationshipOwner != null)
            //{
            //    return partner.relationshipOwner.email;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }

        private string sGetpartnerHROTitle(partner partner)
        {
            //if (partner.relationshipOwner != null)
            //{
            //    return partner.relationshipOwner.title;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }

        private string sGetpartnerHROPhone(partner partner)
        {
            //if (partner.relationshipOwner != null)
            //{
            //    return partner.relationshipOwner.phone;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }

        private string sGetpartnerHROFax(partner partner)
        {
            //if (partner.relationshipOwner != null)
            //{
            //    return partner.relationshipOwner.fax;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }

        private string sGetpartnerOwnerFullName(partner partner)
        {
            //if (partner.owner != null)
            //{
            //    return partner.owner.firstName + " " + partner.owner.lastName;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }
        private string sGetpartnerOwnerEmail(partner partner)
        {
            //if (partner.owner != null)
            //{
            //    return partner.owner.email;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }
        private string sGetpartnerEmailandPassword(partner partner)
        {
            string personnamePassword = null;


            if (partner.internalID != null)
            {


                if (partner.email != null || partner.email != null)
                {
                    personnamePassword = "Email address:" + partner.email + "Password:" + "";
                }
                else
                {
                    personnamePassword = "";
                }
                return personnamePassword;
            }
            else
            {
                return "";
            }
        }
        private string sGetpartnerAccessCode(partner partner, touchpoint touchpoint, int ptq)
        {
            EntitiesDBContext db = new EntitiesDBContext();
            var accesscode = db.pr_getpartnerPartnertypeTouchpointQuestionnaireByPartnerAndPTQ(partner.id, ptq).FirstOrDefault().accesscode;
            db.Dispose();
            //return partner.getAccessCodeBytouchpointpartner(touchpoint).accessCode;
            return accesscode;

        }

        private string sGetpartnerType(partner partner)
        {
            //if (partner.partnerType != null)
            //{
            //    return partner.partnerType.getpartnerType().description;
            //}
            //else
            //{
            //    return "";
            //}
            return "";
        }
    }

}
