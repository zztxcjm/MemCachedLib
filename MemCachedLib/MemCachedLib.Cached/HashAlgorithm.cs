using System;
using System.Runtime.InteropServices;
using System.Text;

namespace MemCachedLib.Cached
{
	internal class HashAlgorithm
	{
		[StructLayout(LayoutKind.Explicit)]
		private struct Byte2Uint
		{
			[FieldOffset(0)]
			public byte[] Bytes;

			[FieldOffset(0)]
			public uint[] UInts;
		}

		private const uint m = 1540483477u;

		private const int r = 24;

		public static int GetHashCode(string key)
		{
			return (int)HashAlgorithm.GetHashCode(Encoding.ASCII.GetBytes(key));
		}

		public static uint GetHashCode(byte[] bytes)
		{
			return HashAlgorithm.GetHashCode(bytes, 3314489979u);
		}

		public static uint GetHashCode(byte[] bytes, uint seed)
		{
			int length = bytes.Length;
			if (length == 0)
			{
				return 0u;
			}
			uint h = seed ^ (uint)length;
			int currentIndex = 0;
			HashAlgorithm.Byte2Uint byte2Uint = default(HashAlgorithm.Byte2Uint);
			byte2Uint.Bytes = bytes;
			uint[] hackArray = byte2Uint.UInts;
			while (length >= 4)
			{
				uint i = hackArray[currentIndex++];
				i *= 1540483477u;
				i ^= i >> 24;
				i *= 1540483477u;
				h *= 1540483477u;
				h ^= i;
				length -= 4;
			}
			currentIndex *= 4;
			switch (length)
			{
			case 1:
				h ^= (uint)bytes[currentIndex];
				h *= 1540483477u;
				break;
			case 2:
				h ^= (uint)((ushort)((int)bytes[currentIndex++] | (int)bytes[currentIndex] << 8));
				h *= 1540483477u;
				break;
			case 3:
				h ^= (uint)((ushort)((int)bytes[currentIndex++] | (int)bytes[currentIndex++] << 8));
				h ^= (uint)((uint)bytes[currentIndex] << 16);
				h *= 1540483477u;
				break;
			}
			h ^= h >> 13;
			h *= 1540483477u;
			return h ^ h >> 15;
		}
	}
}
