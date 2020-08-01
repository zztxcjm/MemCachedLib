using System;

namespace MemCachedLib.Request
{
	internal class FlushRequest : RequestHeader
	{
		protected override OpCodes OpCode
		{
			get
			{
				return OpCodes.Flush;
			}
		}

		public FlushRequest(TimeSpan expiry)
		{
			this.Expiry = new int?((int)expiry.TotalSeconds);
		}
	}
}
