using System;
using System.Text;

namespace MemCachedLib.Request
{
	internal class StatRequest : RequestHeader
	{
		protected override OpCodes OpCode
		{
			get
			{
				return OpCodes.Stat;
			}
		}

		public StatRequest(StatItems key = StatItems.nothing)
		{
			if (key != StatItems.nothing)
			{
				this.Key = Encoding.ASCII.GetBytes(key.ToString());
			}
		}
	}
}
