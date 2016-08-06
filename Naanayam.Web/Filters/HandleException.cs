using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Naanayam.Web.Filters
{
    public class HandleException :  FilterAttribute, IExceptionFilter
    {
        private NLog.ILogger Log { get { return DependencyResolver.Current.GetService<NLog.ILogger>(); } }

        public void OnException(ExceptionContext context)
        {
            Log.Error(context.Exception);

            var model = new HandleErrorInfo(context.Exception, "Error", "Index");

            var result = new ViewResult
            {
                ViewName = "Error",
                MasterName = "_Layout",
                ViewData = new ViewDataDictionary<HandleErrorInfo>(model),
                TempData = context.Controller.TempData
            };

            result.ViewBag.Title = "Error";
            result.ViewBag.Error = context.Exception.Message;
            result.ViewBag.StackTrace = context.Exception.StackTrace;

            context.Result = result;
            context.ExceptionHandled = true;
            context.HttpContext.Response.Clear();
        }
    }
}