using System;

namespace MemCachedLib.Request
{
	internal class VersionRequest : RequestHeader
	{
		protected override OpCodes OpCode
		{
			get
			{
				return OpCodes.Version;
			}
		}
	}
}
