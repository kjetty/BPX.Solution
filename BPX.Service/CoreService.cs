using BPX.Domain.DbModels;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Text.Json;

namespace BPX.Service
{
	public class CoreService : ICoreService
	{
		private readonly IConfiguration configuration; 
		private readonly ICacheService cacheService; 
		private readonly ICacheKeyService cacheKeyService;
		
		public CoreService(IConfiguration configuration, ICacheService cacheService, ICacheKeyService cacheKeyService)
		{
			this.configuration = configuration;
			this.cacheService = cacheService;
			this.cacheKeyService = cacheKeyService;
		}

		ICacheKeyService ICoreService.GetCacheKeyService()
		{
			return cacheKeyService;
		}

		ICacheService ICoreService.GetCacheService()
		{
			return cacheService;
		}

		IConfiguration ICoreService.GetConfiguration()
		{
			return configuration;
		}
	}

	public interface ICoreService
	{
		IConfiguration GetConfiguration();
		ICacheService GetCacheService();
		ICacheKeyService GetCacheKeyService();

	}
}
