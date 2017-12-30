using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class MainGirdVM
    {
        public  List<GroupsVM> Groups { get; set; }
       
    }

    public class PartnerTypeDataList
    {
        public List<PartnerTypeData> partnerType { get; set; }
    }
    public class Dashboard1
    {
        public List<PartnerTypeData> partnerType { get; set; }
        public List<Generic.group> groups { get; set; }
        public List<Generic.ptqGroup> ptqGroups { get; set; }
        public List<pr_getDashboardCountForReferenceByPTQ_Result> ptqDashboard { get; set; }
        public List<pr_getTouchpointAllByEnterprise_Result> ptqtouchPoints { get; set; }
        
    }

    public class Dashboard2
    {
        [DisplayName("Activity Type")]
        public int partnerType { get; set; }

        [DisplayName("Project Name")]
        public int projectName { get; set; }

        [DisplayName("Access Code")]
        public int accessCode { get; set; }
    }

    public class PartnerTypeData
    {
        public int ID{get;set;}
        public string Description{get;set;}
    }
    public class View4
    {
        public int groupID { get; set; }
        public string groupDescription { get; set; }
        public string pieChart { get; set; }
    }



    public class Dashboard21
    {
        public List<Dashboard21Group> Groups { get; set; }

        public List<pr_getDashboardCountForEventByPTQ2_Result> Objs { get; set; }

        public List<partnerType> PartnerTypes { get; set; }
    }

    public class Dashboard21Group {

        public int Id { get; set; }

        public string PtqId { get; set; }

        public string Description { get; set; }

        public List<Dashboard21PartnerType> PartnerTypes { get; set; }
    }

    public class Dashboard21PartnerType
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public List<pr_getDashboardCountForEventByPTQ2_Result> Data { get; set; }
    }
}