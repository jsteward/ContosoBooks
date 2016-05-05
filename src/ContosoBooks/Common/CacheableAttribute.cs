using System;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ContosoBooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : ActionFilterAttribute
    {
        private string CacheKey { get; set; }
        private IMemoryCache _cache { get; set; }

        private double _serverExpiration { get; set; }

        public CacheableAttribute(double ServerExpiration)
        {
            _serverExpiration = ServerExpiration;
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

            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            CacheKey = "ResultCache-" + context.HttpContext.Request.Method;
            _cache.Set(CacheKey, context.Result,
                new MemoryCacheEntryOptions {AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_serverExpiration) });
            base.OnActionExecuted(context);
        }
    }

   
}
