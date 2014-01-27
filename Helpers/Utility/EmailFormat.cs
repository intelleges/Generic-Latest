using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

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

        public string sGetEmailBody(string sEmailBody, person sender, partner partner, touchpoint touchpoint)
        {
            return this.sGetResult(sEmailBody, sender, null, partner, null, touchpoint);
        }
        private string sGetResult(string sEmailBody, person sender, person receiver, partner partner, enterprise enterprise, touchpoint touchpoint)
        {
            Regex regex = new Regex(@"\[(.*?)\]");
            //comment add sebody
            if (sEmailBody == "" || sEmailBody == null)
            {
                sEmailBody = "Survey has been completed";
            }
            MatchCollection collection = regex.Matches(sEmailBody);
            string sVariable = "";
            string sValue = "";

            //iterate each variale found
            foreach (Match match in collection)
            {
                sVariable = match.ToString();

                switch (sVariable)
                {
                    case "[Receiver Full Name]":
                        sValue = this.sGetpersonFullName(receiver);
                        break;
                    case "[Receiver Title]":
                        sValue = this.sGetpersonTitle(receiver);
                        break;
                    case "[Receiver First Name]":
                        sValue = this.sGetpersonFirstName(receiver);
                        break;
                    case "[Receiver Last Name]":
                        sValue = this.sGetpersonLastName(receiver);
                        break;
                    case "[Receiver Phone]":
                        sValue = this.sGetpersonPhone(receiver);
                        break;
                    case "[Receiver Email]":
                        sValue = this.sGetpersonEmail(receiver);
                        break;
                    case "[Receiver Role]":
                        sValue = this.sGetpersonRole(receiver);
                        break;
                    case "[Receiver Password]":
                        sValue = this.sGetpersonPassword(receiver);
                        break;
                    case "[Sender Full Name]":
                        sValue = this.sGetpersonFullName(sender);
                        break;
                    case "[Sender Title]":
                        sValue = this.sGetpersonTitle(sender);
                        break;
                    case "[Sender First Name]":
                        sValue = this.sGetpersonFirstName(sender);
                        break;
                    case "[Sender Last Name]":
                        sValue = this.sGetpersonLastName(sender);
                        break;
                    case "[Sender Phone]":
                        sValue = this.sGetpersonPhone(sender);
                        break;
                    case "[Sender Email]":
                        sValue = this.sGetpersonEmail(sender);
                        break;
                    case "[Sender Role]":
                        sValue = this.sGetpersonRole(sender);
                        break;
                    case "[partner Encrypted Id]":
                        sValue = this.sGetEncryptedpartnerId(partner);
                        break;
                    case "[partner Name]":
                        sValue = this.sGetpartnerName(partner);
                        break;
                    case "[partner Address]":
                        sValue = this.sGetpartnerAddress(partner);
                        break;
                    case "[partner Contact Full Name]":
                        sValue = this.sGetpartnerContactFullName(partner);
                        break;
                    case "[partner Contact First Name]":
                        sValue = this.sGetpartnerContactFirstName(partner);
                        break;
                    case "[partner Contact Last Name]":
                        sValue = this.sGetpartnerContactLastName(partner);
                        break;
                    case "[partner Contact Email]":
                        sValue = this.sGetpartnerContactEmail(partner);
                        break;
                    case "[partner Contact Title]":
                        sValue = this.sGetpartnerContactTitle(partner);
                        break;
                    case "[partner Contact Phone]":
                        sValue = this.sGetpartnerContactPhone(partner);
                        break;
                    case "[partner Contact Fax]":
                        sValue = this.sGetpartnerContactFax(partner);
                        break;
                    case "[partner HRO Full Name]":
                        sValue = this.sGetpartnerHROFullName(partner);
                        break;
                    case "[partner HRO First Name]":
                        sValue = this.sGetpartnerHROFirstName(partner);
                        break;
                    case "[partner HRO Last Name]":
                        sValue = this.sGetpartnerHROLastName(partner);
                        break;
                    case "[partner HRO Email]":
                        sValue = this.sGetpartnerHROEmail(partner);
                        break;
                    case "[partner HRO Phone]":
                        sValue = this.sGetpartnerContactPhone(partner);
                        break;
                    case "[partner HRO Title]":
                        sValue = this.sGetpartnerHROTitle(partner);
                        break;
                    case "[partner HRO Fax]":
                        sValue = this.sGetpartnerHROFax(partner);
                        break;
                    case "[partner Access Code]":
                        sValue = this.sGetpartnerAccessCode(partner, touchpoint);
                        break;
                    case "[partner Code]":
                        sValue = this.sGetpartnerAccessCode(partner, touchpoint);
                        sValue = sValue.Substring(1, 4);
                        break;
                    case "[partner Type]":
                        sValue = this.sGetpartnerType(partner);
                        break;
                    case "[touchpoint]":
                        sValue = touchpoint.title;
                        break;
                    case "[enterprise]":
                        sValue = enterprise.description;
                        break;
                    case "[Protocol]":
                        //sValue = touchpoint.protocol.getProtocol().description;
                        sValue = "";
                        break;
                    case "[Group]":
                        break;
                    case "[Due Date]":
                       // sValue = partner.getDueDateByInitialInvitation(partner, touchpoint).ToShortDateString();
                        sValue = "";
                        break;
                    case "[Start Date]":
                        sValue = DateTime.Now.ToShortDateString();
                        break;
                    case "[partner Owner Full Name]":
                        sValue = this.sGetpartnerOwnerFullName(partner);
                        break;
                    case "[partner Owner Email]":
                        sValue = this.sGetpartnerOwnerEmail(partner);
                        break;
                    case "[partner Email and Password]":
                        sValue = this.sGetpartnerEmailandPassword(partner);
                        break;
                    default:
                        sValue = "";
                        break;
                }
                //replace emailBody's variable with real data
                sEmailBody = sEmailBody.Replace(sVariable, sValue);
            }
            return sEmailBody;
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
            return person.lastName; ;
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
        private string sGetpartnerAccessCode(partner partner, touchpoint touchpoint)
        {
            EntitiesDBContext db = new EntitiesDBContext();
            var accesscode = db.pr_getPartnerPartnertypeTouchpointQuestionnaireByPartner(partner.id).FirstOrDefault().accesscode;
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
