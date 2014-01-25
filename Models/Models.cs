using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using Generic.Helpers.Utility;
using Generic;
namespace Generic.Models
{
    ///<summary>
    /// represent Id
    /// </summary>
    public class Id
    {
        public Id()
        {

        }

        public Id(int id)
        {
            this.id = id;
        }

        private int _id;

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }
    }


    ///<summary>
    ///represent email
    ///</summary>
    public class Email
    {
        public Email()
        {

        }
        public Email(autoMailMessage amm)
        {
            this.body = amm.text + "<br>" + amm.footer1 + "<br>" + amm.footer2;
            this.subject = amm.subject;

        }
       
        private int _id;
   //     private User _sender;
        private string _subject;
        private string _body;
        private DateTime _timeStamp;
        private string _action;
        private string _attachment;
     //   private Provider _provider;
   //     private User _user;
        private string _type;
        private int _emailType;
        private List<AttachmentFile> _attachmentCollection;
        private Boolean _isSendToProvider;
        private Boolean _isSendToUser;
        private string _emailTo;
    //    private Questionnaire _questionnaire;

        //public Questionnaire questionnaire
        //{
        //    get { return _questionnaire; }
        //    set { _questionnaire = value; }
        //}


        public Boolean isSentToUser
        {
            get { return _isSendToUser; }
            set { _isSendToUser = value; }
        }

        public Boolean isSendToProvider
        {
            get { return _isSendToProvider; }
            set { _isSendToProvider = value; }
        }


        public List<AttachmentFile> attachmentCollection
        {
            get { return _attachmentCollection; }
            set { _attachmentCollection = value; }
        }

        public int emailType
        {
            get { return _emailType; }
            set { _emailType = value; }
        }


        public string type
        {
            get { return _type; }
            set { _type = value; }
        }


        //public Provider provider
        //{
        //    get { return _provider; }
        //    set { _provider = value; }
        //}

        //public User user
        //{
        //    get { return _user; }
        //    set { _user = value; }
        //}


        public string attachment
        {
            get { return _attachment; }
            set { _attachment = value; }
        }

        public string action
        {
            get { return _action; }
            set { _action = value; }
        }

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }

        //public User sender
        //{
        //    get { return _sender; }
        //    set { _sender = value; }
        //}

        public string emailTo
        {
            get { return _emailTo; }
            set { _emailTo = value; }
        }

        public string subject
        {
            get { return _subject; }
            set { _subject = value; }
        }

        public string body
        {
            get { return _body; }
            set { _body = value; }
        }

        public DateTime timeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public string sendEmail(Email email)
        {
            SendEmail send = new SendEmail();
            return send.sendEmail(email);
        }

        /////<summary>
        /////get autoEmail by: 
        /////<param name="providerId">Provider Id</param>
        /////<param name="protocolId">Protocol Id</param>
        /////<param name="campaignId">Campaign Id</param>
        /////<param name="providerTypeId">ProviderType Id</param>
        /////<param name="mailType">Mail Type (Int)</param>
        /////</summary>
        //public Email getAutoEmail(Provider provider)
        //{
        //    DataAccess dataAccess = new DataAccess();
        //    return dataAccess.getAutoMailByProtocolCampaignProviderTypeMailType(provider, this);
        //}

        //public Email getAutoMailByProtocolCampaignProviderTypeMailType(Protocol protocol,
        //    Campaign campaign, ProviderType providerType, EmailType emailType)
        //{
        //    DataAccess dataAccess = new DataAccess();
        //    return dataAccess.getAutoMailByProtocolCampaignProviderTypeMailType(protocol, campaign,
        //        providerType, emailType);
        //}

        //public Email getAutoMailByCampaignProviderTypeScore(Campaign campaign, ProviderType providerType, Score score)
        //{
        //    DataAccess dataAccess = new DataAccess();
        //    return dataAccess.getAutoMailByCampaignProviderTypeScore(campaign, providerType, score);
        //}

        //public int addAutoMail(Protocol protocol, Campaign campaign, ProviderType providerType,
        //    EmailType emailType, Email email)
        //{
        //    DataAccess dataAccess = new DataAccess();
        //    return dataAccess.addAutoMail(protocol, campaign, providerType, emailType, email);
        //}

        //public int addAutoMailAttachment(AttachmentFile attachment, int autoMailId)
        //{
        //    DataAccess dataAccess = new DataAccess();
        //    return dataAccess.addAutoMailAttachment(autoMailId, attachment.attachment, attachment.note);
        //}

        //public int addCampaignProviderTypeScoreAutoMail(Campaign campaign, ProviderType providerType,
        //    Score score, string subject, string body, Boolean isSendToProvider, Boolean isSendToUser)
        //{
        //    DataAccess dataAccess = new DataAccess();
        //    return dataAccess.addCampaignProviderTypeScoreAutoMail(campaign, providerType, score, subject, body, isSendToProvider, isSendToUser);
        //}
    }

    public class AttachmentFile
    {
        public AttachmentFile()
        {

        }

        public AttachmentFile(string attachment, string note)
        {
            this.attachment = attachment;
            this.note = note;
        }

        public AttachmentFile(string attachment)
        {
            this.attachment = attachment;
        }

        private string _attachment;
        private string _note;

        public string note
        {
            get { return _note; }
            set { _note = value; }
        }

        public string attachment
        {
            get { return _attachment; }
            set { _attachment = value; }
        }
    }

    public class EmailType
    {
        public EmailType()
        {

        }

        public EmailType(Id id)
        {
            this.id = id;
        }

        public EmailType(string description)
        {
            this.description = description;
        }

        public EmailType(Id id, string description)
        {
            this.id = id;
            this.description = description;
        }

        private Id _id;
        private string _description;

        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Id id
        {
            get { return _id; }
            set { _id = value; }
        }
    }

    public class Address
    {
        public Address()
        {

        }
        public Address(Id id)
        {
            this.id = id;
        }
        public Address(string address1, string address2, string city, string province,
                state state, string zipCode, country country)
        {

            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.province = province;
            this.state = state;
            this.zipCode = zipCode;
            this.country = country;
        }

        public Address(Id id, string address1, string address2, string city, string province,
                state state, string zipCode, country country)
        {
            this.id = id;
            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.province = province;
            this.state = state;
            this.zipCode = zipCode;
            this.country = country;
        }
        public Address(Id id, string address1, string address2, string city, string province,
                       state state, string zipCode, country country, AddressType addressType)
        {
            this.id = id;
            this.address1 = address1;
            this.address2 = address2;
            this.city = city;
            this.province = province;
            this.state = state;
            this.zipCode = zipCode;
            this.country = country;
            this.addressType = addressType;
        }
        private Id _id;
        private string _address1;
        private string _address2;
        private string _city;
        private string _province;
        private state _state;
        private country _country;
        private string _zipCode;
        private AddressType _addressType;

        public Id id
        {
            get { return _id; }
            set { _id = value; }
        }
        public AddressType addressType
        {
            get { return _addressType; }
            set { _addressType = value; }
        }
        public string zipCode
        {
            get { return _zipCode; }
            set { _zipCode = value; }
        }

        public country country
        {
            get { return _country; }
            set { _country = value; }
        }

        public state state
        {
            get { return _state; }
            set { _state = value; }
        }

        public string province
        {
            get { return _province; }
            set { _province = value; }
        }

        public string city
        {
            get { return _city; }
            set { _city = value; }
        }

        public string address2
        {
            get { return _address2; }
            set { _address2 = value; }
        }

        public string address1
        {
            get { return _address1; }
            set { _address1 = value; }
        }
    }

    public class AddressType
    {
        public AddressType()
        {

        }

        public AddressType(Id id)
        {
            this.id = id;
        }

        public AddressType(string description)
        {
            this.description = description;
        }

        public AddressType(Id id, string description)
        {
            this.id = id;
            this.description = description;
        }

        private Id _id;
        private string _description;

        public string description
        {
            get { return _description; }
            set { _description = value; }
        }

        public Id id
        {
            get { return _id; }
            set { _id = value; }
        }

        //public AddressType getAddressType()
        //{
        //    DataAccess dataAccess = new DataAccess();
        //    return dataAccess.getAddressType(this);
        //}
    }

   
}