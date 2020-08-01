using System;
using System.Text;

namespace MemCachedLib.Request
{
	internal class StoreRequest : RequestHeader
	{
		private OpCodes opCode;

		protected override OpCodes OpCode
		{
			get
			{
				return this.opCode;
			}
		}

		public StoreRequest(OpCodes code, string key, byte[] value, TimeSpan expiry, long cas = 0L)
		{
			this.opCode = code;
			this.Key = Encoding.ASCII.GetBytes(key);
			this.Value = value;
			this.Expiry = new int?((int)expiry.TotalSeconds);
			this.Flags = new int?(0);
			this.CAS = cas;
		}
	}
}
