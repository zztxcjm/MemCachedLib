using System;

namespace MemCachedLib
{
	public class CachedReault<T>
	{
		public OprationStatus Status
		{
			get;
			private set;
		}

		public long CAS
		{
			get;
			private set;
		}

		public T Value
		{
			get;
			private set;
		}

		internal CachedReault(OprationStatus status, long cas, T value)
		{
			this.Status = status;
			this.Value = value;
			this.CAS = cas;
		}

		public override string ToString()
		{
			if (this.Value != null)
			{
				T value = this.Value;
				return value.ToString();
			}
			return null;
		}
	}
}
