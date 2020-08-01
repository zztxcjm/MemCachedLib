using System;

namespace MemCachedLib
{
	internal class ResponseHeader
	{
		private ByteBuilder builder;
		private OprationStatus status;

		public OpCodes OpCode
		{
			get
			{
				return (OpCodes)this.builder.ToByte(1);
			}
		}

		public short KeyLength
		{
			get
			{
				return this.builder.ToInt16(2);
			}
		}

		public byte ExtraLength
		{
			get
			{
				return this.builder.ToByte(4);
			}
		}

		public OprationStatus Status
		{
			get
			{
				if (builder != null)
				{
					status = (OprationStatus)this.builder.ToInt16(6);
				}
				return status;
			}
			set
			{
				this.status = value;
			}
		}

		public int TotalBody
		{
			get
			{
				return this.builder.ToInt32(8);
			}
		}

		public long CAS
		{
			get
			{
				return this.builder.ToInt64(16);
			}
		}

		public byte[] Key
		{
			get
			{
				int index = (int)(24 + this.ExtraLength);
				return this.builder.ToArray(index, (int)this.KeyLength);
			}
		}

		public byte[] Value
		{
			get
			{
				int index = (int)((short)(24 + this.ExtraLength) + this.KeyLength);
				return this.builder.ToArray(index);
			}
		}

		public ResponseHeader(byte[] binary)
		{
			this.builder = new ByteBuilder(binary);
		}

		public ResponseHeader()
		{
		}

	}
}
