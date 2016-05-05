using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Filters;

namespace ContosoBooks.Common
{
    [AttributeUsage(AttributeTargets.Method)]
    public class CacheableAttribute : ActionFilterAttribute
    {
        

        public CacheableAttribute()
        {
            
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            
            base.OnActionExecuting(context);
        }

        public override void OnActionExecuted(ActionExecutedContext context)
        {
            base.OnActionExecuted(context);
        }
    }
}
