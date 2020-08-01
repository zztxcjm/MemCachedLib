using System;
using System.Web.SessionState;

namespace MemCachedLib.Session
{
	[Serializable]
	internal class SessionItem
	{
		public SessionStateActions ActionFlag
		{
			get;
			set;
		}

		public int LockId
		{
			get;
			set;
		}

		public DateTime LockTime
		{
			get;
			set;
		}

		public int TimeOut
		{
			get;
			set;
		}

		public bool Locked
		{
			get;
			set;
		}

		public byte[] Binary
		{
			get;
			set;
		}
	}
}
