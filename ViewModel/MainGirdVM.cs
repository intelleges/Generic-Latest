using System;
using System.Collections.Generic;
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
        
    }
    public class PartnerTypeData
    {
        public int ID{get;set;}
        public string Description{get;set;}
    }
}