using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
    public class ContractModel
    {
        public string customerName { get; set;}
        public string contractNumber { get; set; }
        public string primeContractNo { get; set; }
        public string contractManagerName { get; set; }
        public string programName { get; set; }
        public DateTime contractAvardDate { get; set; }
        public DateTime entryDate { get; set; }
        public string sequenceNumber { get; set; }
        public string masterContractNo { get; set; }
        public int customerAccountAdministrator { get; set; }
        public int enterprise { get; set; }
        public int contractType { get; set; }
        public DateTime contractEndDate { get; set;}
        public int touchpoint { get; set; }
        public string SAPOrderNo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string GovContractOfficerPOC { get; set; }
        public string GCPPOCEmail { get; set; }
        public string Phone { get; set; }
        public Nullable<int> ContractId { get; set; }
    }
}