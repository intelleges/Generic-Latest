using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace Generic.Helpers.Utility
{
    /// <summary>
    /// Summary description for SendGridEvents
    /// </summary>
    public class SendGridEvents
    {

        public string email { get; set; }
        public string timestamp { get; set; }
      //  public string category { get; set; }
        public string @event { get; set; }
        public string reason { get; set; }
        public string url { get; set; }
        public string status { get; set; }
        public string created { get; set; }
        public string accesscode { get; set; }

        public string ApplicationName { get; set; }
        public string enterprise { get; set; }
        public string protocolCampaign { get; set; }
        public string loadgroup { get; set; }
        public unique_args unique_args { get; set; }
    }

    public class unique_args     
    {
        public string accesscode { get; set; }
        public string ApplicationName { get; set; }
        public string enterprise { get; set; }
    }
}