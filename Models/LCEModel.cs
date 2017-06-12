using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
	public class LCEModel
	{
		[Required]
		[DisplayName("Program Name")]
		public string ProgramName { get; set; }
		[Required]
		[DisplayName("Designation")]
		public string Designation { get; set; }
		[Required]
		[DisplayName("Owner")]
		public int Owner { get; set; }
		[Required]
		[DisplayName("Due date")]
		public DateTime? Duedate { get; set; }


		[DisplayName("Program Leader/Manager")]
		public string ProgramLeader { get; set; }
		[DisplayName("Coordinator")]
		public string Coordinator { get; set; }
		[DisplayName("TAC Approver")]
		public string TACApprover { get; set; }
		[DisplayName("Sec. 2 Transitions to China")]
		public string ApproverSecTwoTransitionsToChina { get; set; }
		[DisplayName("Sec. 3 Export")]
		public string ApproverSecThreeExport { get; set; }
		[DisplayName("Sec. 4 IP Compliance")]
		public string ApproverSecFourIpCompliance { get; set; }
		[DisplayName("Sec. 5(a) Channel 2 Customer Constracts")]
		public string ApproverSecFiveAChannelCustomerConstracts { get; set; }
		[DisplayName("Sec. 5(b) Channel 1 Customer Constracts")]
		public string ApproverSecFiveBChannelCustomerConstracts { get; set; }
		[DisplayName("Sec. 6 Buy American Act")]
		public string ApproverSecFixBuyAmericanAct { get; set; }
		[DisplayName("Sec. 7 Berry Amedment")]
		public string ApproverSecSevenBerryAmedment { get; set; }
		[DisplayName("Sec. 8 Supplier Contracts")]
		public string ApproverSecEightSupplierContracts { get; set; }
		[DisplayName("Sec. 9 Offset")]
		public string ApproverSecNineOffset { get; set; }
		[DisplayName("Project Description")]
		public string ProjectDescription { get; set; } 
	}
}