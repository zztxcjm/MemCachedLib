using System;
using System.Text;

namespace MemCachedLib.Request
{
	internal class TouchReqeuest : RequestHeader
	{
		protected override OpCodes OpCode
		{
			get
			{
				return OpCodes.Touch;
			}
		}

		public TouchReqeuest(string key, TimeSpan expiry)
		{
			this.Key = Encoding.ASCII.GetBytes(key);
			this.Expiry = new int?((int)expiry.TotalSeconds);
		}
	}
}
