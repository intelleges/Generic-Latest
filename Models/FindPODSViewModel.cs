using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Generic.Models
{
	public class FindPODSViewModel
	{
		public string SupplierNumber { get; set; }

		public string SupplierName { get; set; }

		public string PoNumber { get; set; }

		public decimal? PoValue { get; set; }

		public decimal? ChangeAmt { get; set; }

		public string PoVersion { get; set; }

		public string PartNumber { get; set; }

		public string BuyerEmail { get; set; }

		public string AccessCode { get; set; }


		public string touchpoint { get; set; }

		public string partnertype { get; set; }

		public string partnerStatus { get; set; }
	}
}