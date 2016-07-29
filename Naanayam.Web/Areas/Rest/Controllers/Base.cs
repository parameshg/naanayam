using Naanayam.Server;
using System.Web.Http;
using System.Web.Mvc;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    public class Base : ApiController
    {
        protected string Username { get { return User.Identity.Name; } }

        protected IServer Server { get; set; }

        public Base()
        {
            Server = DependencyResolver.Current.GetService<IServer>();
        }
    }
}