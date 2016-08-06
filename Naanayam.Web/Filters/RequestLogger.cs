using NLog;
using System;
using System.Web.Mvc;
using Naanayam.Tools;

namespace Naanayam.Web.Filters
{
    public class RequestLogger : FilterAttribute, IActionFilter
    {
        private ILogger Log { get { return DependencyResolver.Current.GetService<ILogger>(); } }

        private DateTime Timestamp { get; set; }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            Timestamp = DateTime.Now;

            Log.Info(context.HttpContext.Request.Url.ToString());
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            Log.Debug(Serializer<object>.Current.SerializeToJson(new { Url = context.HttpContext.Request.Url.ToString(), Status = context.HttpContext.Response.StatusCode, Duration = (int)Math.Ceiling(DateTime.Now.Subtract(Timestamp).TotalMilliseconds) }));
        }
    }
}