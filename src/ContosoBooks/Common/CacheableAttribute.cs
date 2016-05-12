using System;
using System.Globalization;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Net.Http.Headers;

namespace ContosoBooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : ActionFilterAttribute
    {
        private string CacheKey { get; set; }
        private IMemoryCache _cache { get; set; }

        private double _serverExpiration { get; set; }
        private double _clientExpiration { get; set; }

        public CacheableAttribute(double serverExpiration, double clientExpiration)
        {
            _serverExpiration = serverExpiration;
            _clientExpiration = clientExpiration;
            _cache = new MemoryCache(new MemoryCacheOptions());
        }


        public override void OnActionExecuting(ActionExecutingContext context)
        {
           
            IActionResult result;
            CacheKey = "ResultCache-" + context.HttpContext.Request.Method;

            if (_cache.TryGetValue(CacheKey, out result))
            {
                context.Result = (ActionResult)result;
            }
            var headers = context.HttpContext.Response.Headers;
            headers.Remove("Vary");
            headers.Remove("Cache-control");
            headers.Remove("Pragma");
            string cacheControlValue = string.Format(CultureInfo.InvariantCulture, "private,max-age={0}", _clientExpiration);
            headers.Add("Cache-control", cacheControlValue);
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            CacheKey = "ResultCache-" + context.HttpContext.Request.Method;
            if (_serverExpiration > 0)
            {
                _cache.Set(CacheKey, context.Result,
                new MemoryCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_serverExpiration) });
            }
            
            base.OnActionExecuted(context);
        }
    }

   
}
