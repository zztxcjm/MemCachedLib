using MemCachedLib.Cached;
using System;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.SessionState;

namespace MemCachedLib.Session
{
	public class MemSessionProvider : SessionStateStoreProviderBase
	{
		private MemCachedEx cachedEx;

		public override string Description
		{
			get
			{
				return base.Description;
			}
		}

		public override string Name
		{
			get
			{
				return base.GetType().Name;
			}
		}

		public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
		{
			HttpStaticObjectsCollection staticObjects = SessionStateUtility.GetSessionStaticObjects(context);
			return new SessionStateStoreData(new SessionStateItemCollection(), staticObjects, timeout);
		}

		public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
		{
			SessionItem session = new SessionItem
			{
				ActionFlag = SessionStateActions.InitializeItem,
				TimeOut = timeout
			};
			this.cachedEx.Set(id, session, TimeSpan.FromMinutes((double)session.TimeOut), 0L);
		}

		public override void EndRequest(HttpContext context)
		{
		}

		private SessionStateStoreData GetItem(bool isExclusive, HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			locked = false;
			lockAge = TimeSpan.Zero;
			lockId = null;
			actions = SessionStateActions.None;
			SessionItem session = this.cachedEx.Get<SessionItem>(id).Value;
			if (session == null)
			{
				return null;
			}
			actions = session.ActionFlag;
			if (session.Locked)
			{
				locked = true;
				lockId = session.LockId;
				lockAge = DateTime.UtcNow - session.LockTime;
				return null;
			}
			if (isExclusive)
			{
				locked = (session.Locked = true);
				session.LockTime = DateTime.UtcNow;
				lockAge = TimeSpan.Zero;
				lockId = ++session.LockId;
			}
			session.ActionFlag = SessionStateActions.None;
			this.cachedEx.Set(id, session, TimeSpan.FromMinutes((double)session.TimeOut), 0L);
			HttpStaticObjectsCollection staticObjects = SessionStateUtility.GetSessionStaticObjects(context);
			SessionStateItemCollection sessionCollection = (actions == SessionStateActions.InitializeItem) ? new SessionStateItemCollection() : SessionSerializer.Deserialize(session.Binary);
			return new SessionStateStoreData(sessionCollection, staticObjects, session.TimeOut);
		}

		public override SessionStateStoreData GetItem(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			return this.GetItem(false, context, id, out locked, out lockAge, out lockId, out actions);
		}

		public override SessionStateStoreData GetItemExclusive(HttpContext context, string id, out bool locked, out TimeSpan lockAge, out object lockId, out SessionStateActions actions)
		{
			return this.GetItem(false, context, id, out locked, out lockAge, out lockId, out actions);
		}

		public override void InitializeRequest(HttpContext context)
		{
		}

		public override void Initialize(string name, NameValueCollection config)
		{
			IPEndPoint[] ips = (from item in config["server"].Split(new char[]
			{
				';'
			})
			select item.Trim() into item
			where item.Contains(':')
			select item.Split(new char[]
			{
				':'
			}) into item
			select new IPEndPoint(IPAddress.Parse(item[0]), int.Parse(item[1]))).ToArray<IPEndPoint>();
			this.cachedEx = MemCachedEx.Create(ips);
			base.Initialize(name, config);
		}

		public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
		{
			SessionItem session = this.cachedEx.Get<SessionItem>(id).Value;
			if (session != null && session.Locked)
			{
				session.Locked = false;
				this.cachedEx.Set(id, session, TimeSpan.FromMinutes((double)session.TimeOut), 0L);
			}
		}

		public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
		{
			this.cachedEx.Delete(id);
		}

		public override void ResetItemTimeout(HttpContext context, string id)
		{
			SessionItem session = this.cachedEx.Get<SessionItem>(id).Value;
			this.cachedEx.Set(id, session, TimeSpan.FromMinutes((double)session.TimeOut), 0L);
		}

		public override void SetAndReleaseItemExclusive(HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
		{
			byte[] binary = SessionSerializer.Serialize(item.Items as SessionStateItemCollection);
			SessionItem session = new SessionItem
			{
				LockId = (lockId == null) ? 0 : ((int)lockId),
				Binary = binary,
				TimeOut = item.Timeout,
				ActionFlag = SessionStateActions.None
			};
			this.cachedEx.Set(id, session, TimeSpan.FromMinutes((double)item.Timeout), 0L);
		}

		public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
		{
			return false;
		}

		public override void Dispose()
		{
			this.cachedEx.Dispose();
		}
	}
}
