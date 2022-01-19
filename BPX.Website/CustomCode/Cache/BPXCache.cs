using BPX.Domain.DbModels;
using BPX.Service;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Linq;
using System.Text.Json;

namespace BPX.Website.CustomCode.Cache
{
	public class BPXCache : IBPXCache
	{
		public IDistributedCache distributedCache;

		public BPXCache(IDistributedCache distributedCache)
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
		//		SlidingExpiration = TimeSpan.FromMinutes(30)
		//	};

		//	distributedCache.Set(key, JsonSerializer.SerializeToUtf8Bytes(values), cacheOptions);
		//}

		public void SetCache<T>(T values, string key, ICacheKeyService CacheKeyService)
		{
			DistributedCacheEntryOptions cacheOptions = new()
			{
				AbsoluteExpiration = DateTime.Now.AddDays(7),
				SlidingExpiration = TimeSpan.FromMinutes(30)
			};

			distributedCache.Set(key, JsonSerializer.SerializeToUtf8Bytes(values), cacheOptions);

			// handle cache :: add to cache, add key to the database	
			CacheKey CacheKey = CacheKeyService.GetRecordsByFilter(c => c.CacheKeyName.Equals(key)).SingleOrDefault();

			if (CacheKey != null)
			{
				CacheKey.ModifiedDate = DateTime.Now;

				CacheKeyService.UpdateRecord(CacheKey);
			}
			else
			{
				CacheKey = new CacheKey();
				CacheKey.CacheKeyName = key;
				CacheKey.ModifiedDate = DateTime.Now;

				CacheKeyService.InsertRecord(CacheKey);
			}

			CacheKeyService.SaveDBChanges();
		}

		public void RemoveCache(string key)
		{
			distributedCache.Remove(key);
		}
	}

	public interface IBPXCache
	{
		//void SetCache<T>(T values, string key);

		void SetCache<T>(T values, string key, ICacheKeyService CacheKeyService);

		T GetCache<T>(string key) where T : class;

		void RemoveCache(string key);
	}
}


/*

IMemoryCache (described in this article) is recommended over System.Runtime.Caching/MemoryCache because it's better integrated into ASP.NET Core.


-------------------------------------

A distributed cache is a cache shared by multiple app servers, typically maintained as an external service to the app servers that access it. 
A distributed cache can improve the performance and scalability of an ASP.NET Core app, especially when the app is hosted by a cloud service 
or a server farm.

A distributed cache has several advantages over other caching scenarios where cached data is stored on individual app servers.

Distributed cache configuration is implementation specific. This article describes how to configure SQL Server and Redis distributed caches. 
Third party implementations are also available, such as NCache.

The Distributed Memory Cache (AddDistributedMemoryCache) is a framework-provided implementation of IDistributedCache that stores items in memory. 
The Distributed Memory Cache isn't an actual distributed cache. Cached items are stored by the app instance on the server where the app is running.

The Distributed Memory Cache is a useful implementation:
1. In development and testing scenarios.
2. When a single server is used in production and memory consumption isn't an issue. 

The sample app makes use of the Distributed Memory Cache when the app is run in the Development environment in Program.cs:
builder.Services.AddDistributedMemoryCache();


The two cache systems are quite different:

IMemoryCache is for storing live object graphs, e.g. a List<Customer>, or a ComplexCrazyObjectGraph
IDistributedGraph is for storing serialized bytes, i.e. byte[]. If you want to store a List<Customer> in there, it must first be serialized. 
But, not all data types are serializable, so that is a limitation of this interface. But, this interface can be used with distributed cache stores, 
such as SQL Server, Redis, or other implementations.

Microsoft has 2 solutions 2 different NuGet packages for caching. Both are great. As per Microsoft’s recommendation, 
prefer using Microsoft.Extensions.Caching.Memory because it integrates better with Asp. NET Core. It can be easily injected into 
Asp .NET Core’s dependency injection mechanism.


 */