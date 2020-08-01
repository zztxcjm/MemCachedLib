using System;
using System.Text;

namespace MemCachedLib.Request
{
	internal class GATRequest : RequestHeader
	{
		protected override OpCodes OpCode
		{
			get
			{
				return OpCodes.GAT;
			}
		}

		public GATRequest(string key, TimeSpan expiry)
		{
			this.Key = Encoding.ASCII.GetBytes(key);
			this.Expiry = new int?((int)expiry.TotalSeconds);
		}
	}
}
