using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Generic.ViewModel
{
    public class PersonViewModel:view_PersonData
    {
        
        public bool IsSelected { get; set; }

        
    }
    public class AddRoleModel
    {
        public List<RolesListModel> AvailableRoleList { get; set; }
        public List<RolesListModel> SelectedRoleList { get; set; }
        public int[] SelectedRoleIds { get; set; }
        public int PersonId { get; set; }
    }
    public class RolesListModel
    {
        public int RoleId { get; set; }
        public string Description { get; set; }
    }
    public class TransferModel
    {
        public List<OwnerModel> CurrentOwnerList { get; set; }
        public List<OwnerModel> NewOwnerList { get; set; }
        public int[] SelectedOwnerIds { get; set; }
        public int PersonId { get; set; }
        public List<SelectListItem> TouchPointList { get; set; }
        public List<SelectListItem> NewOwnerDrptList { get; set; }
        public string SelectedPersonName { get; set; }
    }

    public class OwnerModel
    {
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
    }
}