using BPX.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BPX.Website.Controllers
{
	public class HomeController : BaseController<HomeController>
	{

		private UserService _userService;

		public HomeController(ILogger<HomeController> logger, ICoreService coreService, IUserService userService) : base(logger, coreService)
		{
			_userService = (UserService)userService;
		}

		public IActionResult Index()
		{
			//var nums = new List<int> { 1, 2, 3, 54,21,678, 4,8,3,9};
			//var result = string.Join(string.Empty, nums);

			//logger.LogError("test error from BPX " + this.bpxUseCache + "|" + this.bpxPageSize);

			//var abc = _userService.GetRecordByID(2);

			//ShowAlert(AlertType.Error, "test alert message");


			//var abc = MemoryCacheExtensions.GetKeys((IMemoryDistributedCache)bpxCache);
			//var abc = new MemoryCacheManager((IMemoryCache) bpxCache).GetKeys();

			//var abc = MemoryCacheExtensions.GetKeys((LazyCache.IAppCache)bpxCache);


			//var field = typeof(MemoryDistributedCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
			//var collection = field.GetValue(bpxCache) as ICollection;
			//var items = new List<string>();
			//if (collection != null)
			//	foreach (var item in collection)
			//	{
			//		var methodInfo = item.GetType().GetProperty("Key");
			//		var val = methodInfo.GetValue(item);
			//		items.Add(val.ToString());
			//	}



			//var field = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);
			//var collection = field.GetValue(bpxCache) as ICollection;
			//var items = new List<string>();
			//if (collection != null)
			//{
			//	foreach (var item in collection)
			//	{
			//		var methodInfo = item.GetType().GetProperty("Key");
			//		var val = methodInfo.GetValue(item);
			//		//items.Add(val.ToString());

			//		int i = 0;
			//	}
			//}



			//// Get the empty definition for the EntriesCollection
			//var cacheEntriesCollectionDefinition = typeof(MemoryCache).GetProperty("EntriesCollection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

			//// Populate the definition with your IMemoryCache instance.  
			//// It needs to be cast as a dynamic, otherwise you can't
			//// loop through it due to it being a collection of objects.
			//var cacheEntriesCollection = cacheEntriesCollectionDefinition.GetValue(bpxCache) as dynamic;

			//// Define a new list we'll be adding the cache entries too
			//List<Microsoft.Extensions.Caching.Memory.ICacheEntry> cacheCollectionValues = new List<Microsoft.Extensions.Caching.Memory.ICacheEntry>();

			//foreach (var cacheItem in cacheEntriesCollection)
			//{
			//	//// Get the "Value" from the key/value pair which contains the cache entry   
			//	//Microsoft.Extensions.Caching.Memory.ICacheEntry cacheItemValue = cacheItem.GetType().GetProperty("Value").GetValue(cacheItem, null);

			//	//// Add the cache entry to the list
			//	//cacheCollectionValues.Add(cacheItemValue);

			//	int i = 0;
			//}



			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}
	}
}
