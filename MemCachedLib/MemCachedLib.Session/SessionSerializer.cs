using System;
using System.IO;
using System.Web.SessionState;

namespace MemCachedLib.Session
{
	internal static class SessionSerializer
	{
		public static byte[] Serialize(SessionStateItemCollection items)
		{
			if (items == null)
			{
				return null;
			}
			byte[] result;
			using (MemoryStream ms = new MemoryStream())
			{
				using (BinaryWriter writer = new BinaryWriter(ms))
				{
					items.Serialize(writer);
					writer.Close();
					result = ms.ToArray();
				}
			}
			return result;
		}

		public static SessionStateItemCollection Deserialize(byte[] binary)
		{
			if (binary == null || binary.Length == 0)
			{
				return new SessionStateItemCollection();
			}
			SessionStateItemCollection result;
			using (MemoryStream ms = new MemoryStream(binary))
			{
				using (BinaryReader reader = new BinaryReader(ms))
				{
					result = SessionStateItemCollection.Deserialize(reader);
				}
			}
			return result;
		}
	}
}
