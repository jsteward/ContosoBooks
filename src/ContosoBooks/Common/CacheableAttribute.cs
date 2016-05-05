using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ContosoBooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : ActionFilterAttribute
    {
        public string CacheKey { get; set; }
        public IMemoryCache _cache { get; set; }

        public CacheableAttribute(IMemoryCache cache)
        {
            _cache = cache;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
           // context.HttpContext.
            var result = context.Result;
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

            _cache.Set(CacheKey, context.Result);

            //Add a value in order to know the last time it was cached.
            //context.Controller..ViewData["CachedStamp"] = DateTime.Now;

            base.OnActionExecuted(context);
        }
    }

    //public class BookCacher
    //{

    //}
}
