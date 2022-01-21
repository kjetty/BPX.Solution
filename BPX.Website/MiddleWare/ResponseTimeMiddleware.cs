using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BPX.Website.MiddleWare
{
	public class ResponseTimeMiddleware
	{
		// name of the Response Header, Custom Headers starts with "X-"  
		private const string RESPONSE_HEADER_RESPONSE_TIME = "x-response-time";
		
		// handle to the next Middleware in the pipeline  
		private readonly RequestDelegate _next;

		public ResponseTimeMiddleware(RequestDelegate next)
		{
			_next = next;
		}

		public Task InvokeAsync(HttpContext context)
		{
			var watch = new Stopwatch();
			watch.Start();

			context.Response.OnStarting(() => { 
				watch.Stop();

				var elapsedTime = watch.ElapsedMilliseconds;

				// add the Response time information in the Response headers.   
				context.Response.Headers[RESPONSE_HEADER_RESPONSE_TIME] = elapsedTime.ToString();

				return Task.CompletedTask;
			});

			// call the next delegate/middleware in the pipeline   
			return this._next(context);
		}
	}
}
