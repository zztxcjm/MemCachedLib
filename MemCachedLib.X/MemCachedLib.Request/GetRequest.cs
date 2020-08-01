using System;
using System.Text;

namespace MemCachedLib.Request
{
	internal class GetRequest : RequestHeader
	{
		protected override OpCodes OpCode
		{
			get
			{
				return OpCodes.Get;
			}
		}

		public GetRequest(string key)
		{
			this.Key = Encoding.ASCII.GetBytes(key);
		}
	}
}
