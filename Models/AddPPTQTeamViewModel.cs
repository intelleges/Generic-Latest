using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class AddPPTQTeamViewModel
    {
        public int Id { get; set; }

        public int partnerType { get; set; }

        public int partnerSpreadsheetDataLoadId { get; set; }

        public List<int> Ids { get; set; }
    }
}