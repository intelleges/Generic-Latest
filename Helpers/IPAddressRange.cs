using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Web;

namespace Generic.Helpers
{
	public class IPAddressRange
	{
		readonly AddressFamily addressFamily;
		readonly byte[] lowerBytes;
		readonly byte[] upperBytes;

		public IPAddressRange(IPAddress lower, IPAddress upper)
		{
			// Assert that lower.AddressFamily == upper.AddressFamily

			this.addressFamily = lower.AddressFamily;
			this.lowerBytes = lower.GetAddressBytes();
			this.upperBytes = upper.GetAddressBytes();
		}

		public IPAddressRange(string lower, string upper)
		{
			
			this.addressFamily =IPAddress.Parse(lower).AddressFamily;
			this.lowerBytes = IPAddress.Parse(lower).GetAddressBytes();
			this.upperBytes = IPAddress.Parse(upper).GetAddressBytes();
		}

		public bool IsInRange(string address)
		{
			return IsInRange(IPAddress.Parse(address));
		}

		public bool IsInRange(IPAddress address)
		{
			if (address.AddressFamily != addressFamily)
			{
				return false;
			}

			byte[] addressBytes = address.GetAddressBytes();

			bool lowerBoundary = true, upperBoundary = true;

			for (int i = 0; i < this.lowerBytes.Length &&
				(lowerBoundary || upperBoundary); i++)
			{
				if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) ||
					(upperBoundary && addressBytes[i] > upperBytes[i]))
				{
					return false;
				}

				lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
				upperBoundary &= (addressBytes[i] == upperBytes[i]);
			}

			return true;
		}
	}
}