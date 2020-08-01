using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;

namespace MemCachedLib.Cached
{
	public sealed class MemCachedEx : IDisposable
	{
		private IEnumerable<MemCached> memCacheds;

		private ConsistentHash<MemCached> searcher;

		private MemCached this[string key]
		{
			get
			{
				return this.searcher.GetNode(key);
			}
		}

		public static MemCachedEx Create(params IPEndPoint[] ips)
		{
			return new MemCachedEx(ips);
		}

		private MemCachedEx(IPEndPoint[] ips)
		{
			this.memCacheds = from item in ips
			select MemCached.Create(item);
			this.searcher = new ConsistentHash<MemCached>(this.memCacheds);
		}

		public CachedReault<T> Get<T>(string key)
		{
			return this[key].Get<T>(key);
		}

		public CachedReault<dynamic> Get(string key)
		{
			return this.Get<object>(key);
		}

		public OprationStatus Set(string key, object value, TimeSpan expiry, long cas = 0L)
		{
			return this[key].Set(key, value, expiry, cas);
		}

		public OprationStatus Add(string key, object value, TimeSpan expiry, long cas = 0L)
		{
			return this[key].Add(key, value, expiry, cas);
		}

		public OprationStatus Replace(string key, object value, TimeSpan expiry, long cas = 0L)
		{
			return this[key].Replace(key, value, expiry, cas);
		}

		public OprationStatus Delete(string key)
		{
			return this[key].Delete(key);
		}

		public void Flush(TimeSpan expiry)
		{
			foreach (MemCached i in this.memCacheds)
			{
				i.Flush(expiry);
			}
		}

		public OprationStatus Touch(string key, TimeSpan expiry)
		{
			return this[key].Touch(key, expiry);
		}

		public CachedReault<T> GAT<T>(string key, TimeSpan expiry)
		{
			return this[key].GAT<T>(key, expiry);
		}

		public CachedReault<string> Version(IPEndPoint ip)
		{
			return this.memCacheds.FirstOrDefault((MemCached item) => item.IPEndPoint == ip).Version();
		}

		public List<KeyValuePair<string, string>> Stat(IPEndPoint ip, StatItems item = StatItems.nothing)
		{
			return this.memCacheds.FirstOrDefault((MemCached m) => m.IPEndPoint == ip).Stat(item);
		}

		public void Dispose()
		{
			foreach (MemCached i in this.memCacheds)
			{
				i.Dispose();
			}
		}
	}
}
