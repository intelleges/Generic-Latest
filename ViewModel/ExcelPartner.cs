using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class ExcelPartner : partner
    {
        public string StateName { get; set; }
        public string CountryName { get; set; }

        public string PARTNER_SAP_ID { get; set; }
        public string RO_FIRST_NAME { get; set; }
        public string RO_LAST_NAME { get; set; }
        public string RO_EMAIL { get; set; }
        public DateTime? DUE_DATE { get; set; }
        //public int InternalId { get; set; }
        //public string  Name { get; set; }
        //public string  Address1 { get; set; }
        //public string  Address2 { get; set; }
        //public string  City { get; set; }
        //public string  State { get; set; }
        //public string  Province { get; set; }
        //public string  Zipcode { get; set; }
        // public string  Country { get; set; }
        //public string  POC { get; set; }
        //public string  Phone { get; set; }
        //public string  POC { get; set; }
        //public string  Fax { get; set; }
        //public string  POCEmail { get; set; }
        //public string  POCFirstName { get; set; }
        //	POC LastName	POC Title	POC DUNS	POC EID	RO FirstName	RO LastName	RO Phone	RO Email

    }
    public enum InteratePartnerStatus
    {
        NotSet=0,
        Busy=1,
        Do_Not_Call=4,
        HangUp=5,
        LeftMessage=6,
        MusicBox=13,
        NoAnswer=7,
        NoHelp=8,
        No_Message_Left_Call_Back=10,//No Message Left (Call Back)
        Successful_Call_Call_Back=3,//Successful Call - Call Back
       Successful_Call_Appointment=2, //Successful Call - Appointment
        Not_In_Service=9,//Not In Service
        Other=14,//Other
        Transferred=11,//Transferredme to you wrote this 
        Wrong_Number=12,//Wrong Number
        NoTouchpoint=15,
        None=16,
        EmailSent=17
    };

    public enum InteratePartnerNextStatus
    {
        //Call Back
        /*Set Appointment
         * 
         */
    };

    public class IteratePartnerView
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public int Status { get; set; }
        public string Title { get; set; }
        public DateTime? LastContact { get; set; }
        public DateTime? NewContact { get; set; }
        public string StatusDescription
        {
            get
            {
                return ExcelInteratePartner.GetStatusString((InteratePartnerStatus)Status);
            }
        }
        
    }
    

    public class ExcelInteratePartner
    {
        #region FiledSet
        public string PARTNER_INTERNAL_ID { get; set; }
        public string PARTNER_NAME { get; set; }
        public string PARTNER_DUNS { get; set; }
        public string PARTNER_SAP_ID { get; set; }

        public string PARTNER_POC_FIRST_NAME { get; set; }
        public string PARTNER_POC_LAST_NAME { get; set; }
        public string PARTNER_POC_TITLE { get; set; }
        public string PARTNER_POC_PHONE_NUMBER { get; set; }

        public string PARTNER_POC_EMAIL_ADDRESS { get; set; }
        public string PARTNER_ADDRESS_ONE { get; set; }
        public string PARTNER_ADDRESS_TWO { get; set; }
        public string PARTNER_CITY { get; set; }

        public string PARTNER_STATE { get; set; }
        public string PARTNER_ZIPCODE { get; set; }
        public string PARTNER_COUNTRY { get; set; }
        public string PARTNER_CONTACT_FAX { get; set; }

        public string PARTNER_PROVINCE { get; set; }
        public string RO_FIRST_NAME { get; set; }
        public string RO_LAST_NAME { get; set; }
        public string RO_EMAIL { get; set; }
        public string DUE_DATE { get; set; }
        public string CURRENT_STATUS { get; set; }
        public string NEXT_ACTION { get; set; }
        public string ANNUAL_REVENUE { get; set; }
        public string EMPLOYEE_COUNT { get; set; }

        public string LAST_CONTACT { get; set; }
        public string LAST_CONTACT_DATE { get; set; }
        public string PREVIOUS_CONTACT { get; set; }
        public string PREVIOUS_CONTACT_DATE { get; set; }
        public string NEXT_ACTION_DATE { get; set; }
        public string NOTES { get; set; }
        public string ACCESS_CODE { get; set; }
        #endregion

        public InteratePartnerStatus StatusValue
        {
            get
            {
                return ExcelInteratePartner.GetStatusValue(CURRENT_STATUS);
            }
        }

        public static int GetStatusId(List<pr_getIteratePersonStatus_Result> source, string status)
        {
            return source.Where(o => o.description == status).Select(o => o.id).FirstOrDefault();
        }

        public static int GetNextActionId(List<pr_getIteratePersonNextAction_Result> source, string nextAction)
        {
            return source.Where(o => o.nextAction == nextAction).Select(o => o.id).FirstOrDefault();
        }

        public static InteratePartnerStatus GetStatusValue(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                switch (value.ToLower())
                {
                    case "busy": return InteratePartnerStatus.Busy; break;
                    case "do not call": return InteratePartnerStatus.Do_Not_Call; break;
                    case "hang up": return InteratePartnerStatus.HangUp; break;
                    case "left message": return InteratePartnerStatus.LeftMessage; break;
                    case "music box": return InteratePartnerStatus.MusicBox; break;
                    case "no answer": return InteratePartnerStatus.NoAnswer; break;
                    case "no help": return InteratePartnerStatus.NoHelp; break;
                    case "no message left (call back)": return InteratePartnerStatus.No_Message_Left_Call_Back; break;
                    case "not in service": return InteratePartnerStatus.Not_In_Service; break;
                    case "other": return InteratePartnerStatus.Other; break;
                    case "successful call - appointment": return InteratePartnerStatus.Successful_Call_Appointment; break;
                    case "successful call - call back": return InteratePartnerStatus.Successful_Call_Call_Back; break;
                    case "transferred": return InteratePartnerStatus.Transferred; break;
                    case "wrong number": return InteratePartnerStatus.Wrong_Number; break;
                    default: return InteratePartnerStatus.NotSet; break;
                }
            }
            else
            {
                return InteratePartnerStatus.NotSet;
            }
        }

        public static string GetStatusString(InteratePartnerStatus status)
        {
            switch(status)
            {
                case InteratePartnerStatus.Wrong_Number: return "Wrong Number"; break;
                case InteratePartnerStatus.Transferred: return "Transferred"; break;
                case InteratePartnerStatus.Successful_Call_Call_Back: return "Successful Call - Call Back"; break;
                case InteratePartnerStatus.Successful_Call_Appointment: return "Successful Call - Appointment"; break;
                case InteratePartnerStatus.Other: return "Other"; break;
                case InteratePartnerStatus.NotSet: return "Not Set"; break;
                case InteratePartnerStatus.Not_In_Service: return "Not in Service"; break;
                case InteratePartnerStatus.NoHelp: return "No Help"; break;
                case InteratePartnerStatus.NoAnswer: return "No Answer"; break;
                case InteratePartnerStatus.No_Message_Left_Call_Back: return "No Message Left (Call Back)"; break;
                case InteratePartnerStatus.MusicBox: return "Music Box"; break;
                case InteratePartnerStatus.LeftMessage: return "Left Message"; break;
                case InteratePartnerStatus.HangUp: return "Hang Up"; break;
                case InteratePartnerStatus.Do_Not_Call: return "Do Not Call"; break;
                case InteratePartnerStatus.Busy: return "Busy"; break;
                default:
                    return "Not Set";
                    break;
            }
        }

    }

    public class ExcelPartnumber : ExcelPartner
    {
        public string INTERNAL_SITE_ID { get; set; }
        public string SAP_SITE { get; set; }
        public string SAP_PLANT_CODE { get; set; }
        public string SITE_NAME { get; set; }

        public string PART_NUMBER_SAP { get; set; }
        public string PART_NUMBER_INTERNAL { get; set; }
        public string SUB_COMMODITY_OWNER { get; set; }
        public string CENTER_OF_EXCELLENCE { get; set; }

        public string RO_FIRST_NAME { get; set; }
        public string RO_LAST_NAME { get; set; }
        public string RO_EMAIL { get; set; }

        public DateTime? DUE_DATE { get; set; }


    }
}