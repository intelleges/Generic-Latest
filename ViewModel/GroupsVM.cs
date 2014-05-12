using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.ViewModel
{
    public class GroupsVM
    {
        public String GroupName { get; set; }
        public int GroupId { get; set; }
        public List<string> Types { get; set; }
        public List<GridDataVM> Data { get; set; }
        //public List<int> G { get; set; }
        //public List<int> U { get; set; }
        //public List<int> R { get; set; }
        //public List<int> C { get; set; }
        //public List<int> N_R { get; set; }
        //public List<int> R_I { get; set; }
        //public List<int> RC { get; set; }
        //public List<int> T { get; set; }   
       // public List<GridDataVM> ListGridData { get; set; }        
    }
}