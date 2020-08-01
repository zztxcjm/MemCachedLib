using System;
using System.Text;

namespace MemCachedLib.Request
{
	internal class DeleteRequest : RequestHeader
	{
		protected override OpCodes OpCode
		{
			get
			{
				return OpCodes.Delete;
			}
		}

		public DeleteRequest(string key)
		{
			this.Key = Encoding.ASCII.GetBytes(key);
		}
	}
}
