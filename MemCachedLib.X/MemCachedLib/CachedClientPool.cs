using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace MemCachedLib
{
	internal class CachedClientPool : IDisposable
	{
		private IPEndPoint ip;

		private ConcurrentStack<CachedClient> clientStack = new ConcurrentStack<CachedClient>();

		private SpinWait spinWait = default(SpinWait);

		public int MaxClient
		{
			get;
			private set;
		}

		public CachedClientPool(IPEndPoint ip) : this(ip, 10)
		{
		}

		public CachedClientPool(IPEndPoint ip, int maxClient)
		{
			this.ip = ip;
			this.MaxClient = maxClient;
			for (int i = 0; i < maxClient; i++)
			{
				this.Push(new CachedClient(ip));
			}
		}

		public CachedClient Pop()
		{
			CachedClient client = null;
			while (!this.clientStack.TryPop(out client))
			{
				this.spinWait.SpinOnce();
			}
			return client;
		}

		public void Push(CachedClient client)
		{
			this.clientStack.Push(client);
		}

		public void Dispose()
		{
			CachedClient client = null;
			while (this.clientStack.TryPop(out client))
			{
				client.Dispose();
			}
		}
	}
}
