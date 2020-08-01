using System;
using System.Net;

namespace MemCachedLib
{
	internal class ByteBuilder
	{
		private byte[] binary;

		private int capacity;

		private int Capacity;

		public int Length
		{
			get;
			private set;
		}

		public int Position
		{
			get;
			private set;
		}

		public ByteBuilder(int capacity)
		{
			this.capacity = capacity;
			this.Capacity = capacity;
			this.binary = new byte[capacity];
		}

		public ByteBuilder(byte[] binary)
		{
			this.capacity = binary.Length;
			this.Capacity = binary.Length;
			this.Length = binary.Length;
			this.binary = binary;
		}

		public void Add(byte[] srcArray)
		{
			this.Add(srcArray, 0, srcArray.Length);
		}

		public void Add(byte[] srcArray, int index, int length)
		{
			if (srcArray == null)
			{
				return;
			}
			int newCapacity = this.Position + this.Length + length;
			if (newCapacity > this.Capacity)
			{
				while (newCapacity > this.Capacity)
				{
					this.Capacity *= 2;
				}
				byte[] newBuffer = new byte[this.Capacity];
				this.binary.CopyTo(newBuffer, 0);
				this.binary = newBuffer;
			}
			Array.Copy(srcArray, index, this.binary, this.Position + this.Length, length);
			this.Length += length;
		}

		public void RemoveRange(int length)
		{
			this.Position += length;
			this.Length -= length;
		}

		public void RemoveRange(int length, int offset)
		{
			if (offset == 0)
			{
				this.RemoveRange(length);
				return;
			}
			int destIndex = this.Position + offset;
			int srcIndex = destIndex + length;
			length = this.Length - length - offset;
			Array.Copy(this.binary, srcIndex, this.binary, destIndex, length);
			this.Length -= length;
		}

		public void CopyTo(byte[] destArray, int index, int length)
		{
			this.CopyTo(destArray, index, length, 0);
		}

		public void CopyTo(byte[] destArray, int index, int length, int offset)
		{
			int srcIndex = this.Position + offset;
			Array.Copy(this.binary, srcIndex, destArray, index, length);
		}

		public void CutTo(byte[] destArray, int index, int length)
		{
			this.CopyTo(destArray, index, length);
			this.RemoveRange(length);
		}

		public void CutTo(byte[] destArray, int index, int length, int offset)
		{
			this.CopyTo(destArray, index, length, offset);
			this.RemoveRange(length, offset);
		}

		public byte ToByte(int offset)
		{
			int index = offset + this.Position;
			return this.binary[index];
		}

		public short ToInt16(int offset)
		{
			int index = offset + this.Position;
			short value = BitConverter.ToInt16(this.binary, index);
			return IPAddress.HostToNetworkOrder(value);
		}

		public int ToInt32(int offset)
		{
			int index = offset + this.Position;
			int value = BitConverter.ToInt32(this.binary, index);
			return IPAddress.HostToNetworkOrder(value);
		}

		public long ToInt64(int offset)
		{
			int index = offset + this.Position;
			long value = BitConverter.ToInt64(this.binary, index);
			return IPAddress.HostToNetworkOrder(value);
		}

		public byte[] ToArray()
		{
			return this.ToArray(0);
		}

		public byte[] ToArray(int offset)
		{
			int length = this.Length - offset;
			return this.ToArray(offset, length);
		}

		public byte[] ToArray(int offset, int length)
		{
			int index = offset + this.Position;
			byte[] buffer = new byte[length];
			Array.Copy(this.binary, index, buffer, 0, length);
			return buffer;
		}

		public byte[] ReadRange(int length)
		{
			byte[] buffer = new byte[length];
			this.CutTo(buffer, 0, length);
			return buffer;
		}

		public byte[] ReadRange(int length, int offset)
		{
			byte[] buffer = new byte[length];
			this.CutTo(buffer, 0, length, offset);
			return buffer;
		}

		public void Clear()
		{
			this.Position = 0;
			this.Length = 0;
		}

		public void ReSet()
		{
			this.Position = 0;
			this.Length = 0;
			this.Capacity = this.capacity;
			this.binary = new byte[this.capacity];
		}

		public override string ToString()
		{
			return this.Length.ToString();
		}
	}
}
