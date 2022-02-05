using BPX.Domain.DbModels;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Text.Json;

namespace BPX.Service
{
	public class CacheService : ICacheService
	{
		public IDistributedCache distributedCache;

		public CacheService(IDistributedCache distributedCache)
		{
			this.distributedCache = distributedCache;
		}

		public T GetCache<T>(string key) where T : class
		{
			byte[] values = distributedCache.Get(key);

			if (values != null)
				return JsonSerializer.Deserialize<T>(values);
			else
				return null;
		}

		//public void SetCache<T>(T values, string key)
		//{
		//	DistributedCacheEntryOptions cacheOptions = new()
		//	{
		//		AbsoluteExpiration = DateTime.Now.AddDays(7),
		//		SlidingExpiration = TimeSpan.FromMinutes(240)
		//	};

		//	distributedCache.Set(key, JsonSerializer.SerializeToUtf8Bytes(values), cacheOptions);
		//}

		public void SetCache<T>(T values, string key, ICacheKeyService cacheKeyService)
		{
			DistributedCacheEntryOptions cacheOptions = new()
			{
				AbsoluteExpiration = DateTime.Now.AddDays(7),
				SlidingExpiration = TimeSpan.FromMinutes(240)
			};

			distributedCache.Set(key, JsonSerializer.SerializeToUtf8Bytes(values), cacheOptions);

			// handle cache :: add to cache, add key to the database	
			CacheKey CacheKey = cacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.ToUpper().Equals(key.ToUpper())).SingleOrDefault();

			if (CacheKey != null)
			{
				CacheKey.ModifiedDate = DateTime.Now;

				cacheKeyService.UpdateRecord(CacheKey);
			}
			else
			{
				CacheKey = new CacheKey();
				CacheKey.CacheKeyName = key;
				CacheKey.ModifiedDate = DateTime.Now;

				cacheKeyService.InsertRecord(CacheKey);
			}

			cacheKeyService.SaveDBChanges();
		}

		public void RemoveCache(string key)
		{
			distributedCache.Remove(key);
		}
	}

	public interface ICacheService
	{
		//void SetCache<T>(T values, string key);
		void SetCache<T>(T values, string key, ICacheKeyService CacheKeyService);
		T GetCache<T>(string key) where T : class;
		void RemoveCache(string key);
	}
}
