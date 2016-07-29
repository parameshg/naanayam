using Naanayam.Server;
using NLog;
using System.Web.Mvc;

namespace Naanayam.Web.Controllers
{
    public class Base : Controller
    {
        protected ILogger Log { get; private set; }

        protected string Username { get { return User.Identity.Name; } }

        protected IServer Server { get; set; }

        public Base()
        {
            Log = DependencyResolver.Current.GetService<ILogger>();

            Server = DependencyResolver.Current.GetService<IServer>();
        }
    }
}