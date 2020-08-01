using System;
using System.Collections.Generic;
using System.Linq;

namespace MemCachedLib.Cached
{
	public class ConsistentHash<T>
	{
		private int[] hashKeys;

		private int defaultReplicate = 100;

		private SortedDictionary<int, T> keyHashNodeDic;

		public ConsistentHash() : this(null)
		{
		}

		public ConsistentHash(IEnumerable<T> nodes)
		{
			this.keyHashNodeDic = new SortedDictionary<int, T>();
			if (nodes != null)
			{
				foreach (T node in nodes)
				{
					this.Add(node, false);
				}
				this.hashKeys = this.keyHashNodeDic.Keys.ToArray<int>();
			}
		}

		public void Add(T node)
		{
			this.Add(node, true);
		}

		private void Add(T node, bool updateKeyArray)
		{
			for (int i = 0; i < this.defaultReplicate; i++)
			{
				int hash = HashAlgorithm.GetHashCode(node.GetHashCode().ToString() + i);
				this.keyHashNodeDic[hash] = node;
			}
			if (updateKeyArray)
			{
				this.hashKeys = this.keyHashNodeDic.Keys.ToArray<int>();
			}
		}

		public bool Remove(T node)
		{
			for (int i = 0; i < this.defaultReplicate; i++)
			{
				int hash = HashAlgorithm.GetHashCode(node.GetHashCode().ToString() + i);
				if (!this.keyHashNodeDic.Remove(hash))
				{
					return false;
				}
			}
			this.hashKeys = this.keyHashNodeDic.Keys.ToArray<int>();
			return true;
		}

		public T GetNode(string key)
		{
			int firstNode = this.GetHaskKeyIndex(key);
			return this.keyHashNodeDic[this.hashKeys[firstNode]];
		}

		private int GetHaskKeyIndex(string key)
		{
			int hashCode = HashAlgorithm.GetHashCode(key);
			int begin = 0;
			int end = this.hashKeys.Length - 1;
			if (this.hashKeys[end] < hashCode || this.hashKeys[0] > hashCode)
			{
				return 0;
			}
			while (end - begin > 1)
			{
				int mid = (end + begin) / 2;
				if (this.hashKeys[mid] >= hashCode)
				{
					end = mid;
				}
				else
				{
					begin = mid;
				}
			}
			return end;
		}
	}
}
