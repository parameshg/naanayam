using Naanayam.Server;
using NLog;
using System.Web.Mvc;

namespace Naanayam.Web.Controllers
{
    public class Base : Controller
    {
        protected ILogger Log { get; private set; }

        protected IServer Service { get; set; }

        public Base()
        {
            Log = DependencyResolver.Current.GetService<ILogger>();

            Service = DependencyResolver.Current.GetService<IServer>();

            if (HttpContext != null && HttpContext.User.Identity.IsAuthenticated)
                Service.Context.Impersonate(HttpContext.User.Identity.Name);
        }
    }
}