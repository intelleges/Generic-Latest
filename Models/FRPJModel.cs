using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Generic.Models
{
	public class FRPJModel
	{
		[Required]
		[DisplayName("Suplier name")]
		public string SuplierName { get; set; }
		[Required]
		[DisplayName("Supplier Number")]
		public string DunsNumber { get; set; }
		[Required]
		[DisplayName("Ariba Contract #")]
		public string ContractNumber { get; set; }
		[Required]
		[DisplayName("Contract Summary Date")]
		public DateTime? ContractStartDate { get; set; }
		//[Required]
		//[DisplayName("Contract Expiration Date")]
		//public DateTime? ContractExpirationDate { get; set; }
		[Required]
		[DisplayName("Amendment #")]
		public string AmendmentNumber { get; set; }
		//[Required]
		//[DisplayName("Estimated USD Annual Forcast")]
		//public string EstimatedUSDAnnualForcast { get; set; }
		//[Required]
		//[DisplayName("Estimated USD Total Forcast")]
		//public string EstimatedUSDTotalForcast { get; set; }
		[Required]
		[DisplayName("First Name")]
		public string CMFirstName { get; set; }
		[Required]
		[DisplayName("Last Name")]
		public string CMLastName { get; set; }
		[Required]
		[DisplayName("Email")]
		public string CMEmail { get; set; }
		[Required]
		[DisplayName("Protocol")]
		public int? Protocol { get; set; }
		[Required]
		[DisplayName("Touchpoint")]
		public int? Touchpoint { get; set; }
		[Required]
		[DisplayName("Direct/Indirect/Project: (HBT and PMT ONLY if Project and over $250K, complete Attachment B)")]
		public int? PartnerType { get; set; }
		[Required]
		[DisplayName("ContractType")]
		public string ContractType { get; set; }
		[Required]
		[DisplayName("Type of Agreement")]
		public int? Group { get; set; }
		[Required]
		[DisplayName("Owner")]
		public int? Owner { get; set; }
		[Required]
		[DisplayName("Date")]
		public DateTime? Date { get; set; }
	}
}