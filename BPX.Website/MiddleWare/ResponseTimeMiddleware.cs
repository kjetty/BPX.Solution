using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Threading.Tasks;

namespace BPX.Website.MiddleWare
{
    public class ResponseTimeMiddleware
    {
        private readonly RequestDelegate _next;

        public ResponseTimeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext httpContext)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            httpContext.Response.OnStarting(() =>
            {
                watch.Stop();
                long elapsedTime = watch.ElapsedMilliseconds;

                // add the response time information to the response headers, custom headers starts with "x-"  
                httpContext.Response.Headers["x-response-time"] = elapsedTime.ToString();

                return Task.CompletedTask;
            });

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ResponseTimeMiddlewareExtensions
    {
        public static IApplicationBuilder UseResponseTimeMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResponseTimeMiddleware>();
        }
    }
}
