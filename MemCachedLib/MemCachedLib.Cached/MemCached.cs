using MemCachedLib.Request;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;

namespace MemCachedLib.Cached
{
	public sealed class MemCached : IDisposable
	{
		private int hashCode;

		private CachedClientPool clientPool;

		public IPEndPoint IPEndPoint
		{
			get;
			private set;
		}

		public static MemCached Create(IPEndPoint ip)
		{
			return new MemCached(ip);
		}

		private MemCached(IPEndPoint ip)
		{
			this.IPEndPoint = ip;
			this.clientPool = new CachedClientPool(ip);
			this.hashCode = HashAlgorithm.GetHashCode(ip.ToString());
		}

		private T Request<T>(Func<CachedClient, T> func)
		{
			CachedClient client = this.clientPool.Pop();
			T result = func(client);
			this.clientPool.Push(client);
			return result;
		}

		private void Request(Action<CachedClient> action)
		{
			CachedClient client = this.clientPool.Pop();
			action(client);
			this.clientPool.Push(client);
		}

		public CachedReault<object> Get(string key)
		{
			return this.Get<object>(key);
		}

		public CachedReault<T> Get<T>(string key)
		{
			return this.Request<CachedReault<T>>(delegate (CachedClient client)
			{
				ResponseHeader response = client.Send(new GetRequest(key));
				T value = client.ToEntity<T>(response.Value);
				return new CachedReault<T>(response.Status, response.CAS, value);
			});
		}

		public bool TryGet(string key, out object val)
		{
			return this.TryGet<object>(key, out val);
		}

		public bool TryGet<T>(string key, out T val)
		{
			val = default(T);
			var result = this.Get<object>(key);
			if (result.Status == OprationStatus.Key_Exists
				|| result.Status == OprationStatus.No_Error)
			{
				val = (T)result.Value;
				return true;
			}
			return false;
		}

		public bool Exist(string key)
		{
			return this.Get<object>(key).Status != OprationStatus.Key_Not_Found;
		}

		private OprationStatus Store(OpCodes code, string key, object value, TimeSpan expiry, long cas = 0L)
		{
			return this.Request<OprationStatus>(delegate(CachedClient client)
			{

				byte[] valueBytes = client.ToBinary(value);

				//防止数据超过限制的大小
				if (valueBytes != null && valueBytes.LongLength > Setting.DataMaxSize)
				{
					throw new DataTooLargeException();
				}

				StoreRequest request = new StoreRequest(code, key, valueBytes, expiry, cas);
				return client.Send(request).Status;

			});
		}

		public OprationStatus Set(string key, object value, long cas = 0L)
		{
			return this.Store(OpCodes.Set, key, value, TimeSpan.FromDays(7), cas);
		}

		public OprationStatus Set(string key, object value, TimeSpan expiry, long cas = 0L)
		{
			return this.Store(OpCodes.Set, key, value, expiry, cas);
		}

		public OprationStatus Add(string key, object value, long cas = 0L)
		{
			return this.Store(OpCodes.Add, key, value, TimeSpan.FromDays(7), cas);
		}

		public OprationStatus Add(string key, object value, TimeSpan expiry, long cas = 0L)
		{
			return this.Store(OpCodes.Add, key, value, expiry, cas);
		}

		public OprationStatus Replace(string key, object value, TimeSpan expiry, long cas = 0L)
		{
			return this.Store(OpCodes.Replace, key, value, expiry, cas);
		}

		public OprationStatus Delete(string key)
		{
			return this.Request<OprationStatus>((CachedClient client) => client.Send(new DeleteRequest(key)).Status);
		}

		public void Flush(TimeSpan expiry)
		{
			this.Request<ResponseHeader>((CachedClient client) => client.Send(new FlushRequest(expiry)));
		}

		public OprationStatus Touch(string key, TimeSpan expiry)
		{
			return this.Request<OprationStatus>(delegate(CachedClient client)
			{
				TouchReqeuest request = new TouchReqeuest(key, expiry);
				return client.Send(request).Status;
			});
		}

		public CachedReault<T> GAT<T>(string key, TimeSpan expiry)
		{
			return this.Request<CachedReault<T>>(delegate(CachedClient client)
			{
				ResponseHeader res = client.Send(new GATRequest(key, expiry));
				T value = client.ToEntity<T>(res.Value);
				return new CachedReault<T>(res.Status, res.CAS, value);
			});
		}

		public CachedReault<string> Version()
		{
			return this.Request<CachedReault<string>>(delegate(CachedClient client)
			{
				ResponseHeader response = client.Send(new VersionRequest());
				string version = Encoding.ASCII.GetString(response.Value);
				return new CachedReault<string>(response.Status, response.CAS, version);
			});
		}

		public List<KeyValuePair<string, string>> Stat(StatItems item = StatItems.nothing)
		{
			return this.Request<List<KeyValuePair<string, string>>>(delegate(CachedClient client)
			{
				List<KeyValuePair<string, string>> dic = new List<KeyValuePair<string, string>>();
				IEnumerable<ResponseHeader> responses = client.Sends(new StatRequest(item));
				foreach (ResponseHeader res in responses)
				{
					if (res.TotalBody > 0)
					{
						string i = Encoding.ASCII.GetString(res.Key);
						string v = Encoding.ASCII.GetString(res.Value);
						dic.Add(new KeyValuePair<string, string>(i, v));
					}
				}
				return dic;
			});
		}

		public override int GetHashCode()
		{
			return this.hashCode;
		}

		public void Dispose()
		{
			this.clientPool.Dispose();
		}
	}
}
