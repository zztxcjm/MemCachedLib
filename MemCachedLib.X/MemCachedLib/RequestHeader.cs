using System;
using System.Linq;

namespace MemCachedLib
{
	internal abstract class RequestHeader
	{
		protected byte[] Key;

		protected long CAS;

		protected int? Flags;

		protected int? Expiry;

		protected byte[] Value;

		protected abstract OpCodes OpCode
		{
			get;
		}

		public byte[] ToByteArray()
		{
			int extraLength = 0;
			if (this.Flags.HasValue)
			{
				extraLength += 4;
			}
			if (this.Expiry.HasValue)
			{
				extraLength += 4;
			}
			int keyLength = (this.Key == null) ? 0 : this.Key.Length;
			int valueLenth = (this.Value == null) ? 0 : this.Value.Length;
			int totalBodyLength = extraLength + keyLength + valueLenth;
			byte[] packet = new byte[24 + totalBodyLength];
			packet[0] = 128;
			packet[1] = (byte)this.OpCode;
			if (keyLength > 0)
			{
				byte[] keyLenBytes = BitConverter.GetBytes(keyLength);
				packet[2] = keyLenBytes[1];
				packet[3] = keyLenBytes[0];
			}
			packet[4] = (byte)extraLength;
			if (totalBodyLength > 0)
			{
				byte[] bodyLenBytes = BitConverter.GetBytes(totalBodyLength);
				packet[8] = bodyLenBytes[3];
				packet[9] = bodyLenBytes[2];
				packet[10] = bodyLenBytes[1];
				packet[11] = bodyLenBytes[0];
			}
			if (this.CAS > 0L)
			{
				byte[] casBytes = BitConverter.GetBytes(this.CAS).Reverse<byte>().ToArray<byte>();
				Array.Copy(casBytes, 0, packet, 16, 8);
			}
			if (extraLength == 4)
			{
				int extra = this.Expiry.HasValue ? this.Expiry.Value : this.Flags.Value;
				byte[] extraBytes = BitConverter.GetBytes(extra);
				packet[24] = extraBytes[3];
				packet[25] = extraBytes[2];
				packet[26] = extraBytes[1];
				packet[27] = extraBytes[0];
			}
			else if (extraLength == 8)
			{
				byte[] flagsBytes = BitConverter.GetBytes(this.Flags.Value);
				byte[] expiryBytes = BitConverter.GetBytes(this.Expiry.Value);
				packet[24] = flagsBytes[3];
				packet[25] = flagsBytes[2];
				packet[26] = flagsBytes[1];
				packet[27] = flagsBytes[0];
				packet[28] = expiryBytes[3];
				packet[29] = expiryBytes[2];
				packet[30] = expiryBytes[1];
				packet[31] = expiryBytes[0];
			}
			if (keyLength > 0)
			{
				Array.Copy(this.Key, 0, packet, 24 + extraLength, keyLength);
			}
			if (valueLenth > 0)
			{
				Array.Copy(this.Value, 0, packet, 24 + extraLength + keyLength, valueLenth);
			}
			return packet;
		}

		public override string ToString()
		{
			return this.OpCode.ToString();
		}
	}
}
