using Naanayam.Server;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    public class Base : ApiController
    {
        protected IServer Server { get; set; }

        public Base()
        {
            Server = DependencyResolver.Current.GetService<IServer>();

            if (User != null && User.Identity.IsAuthenticated)
            {
                Server.Context.Impersonate(User.Identity.Name);

                var cookies = HttpContext.Current.Request.Cookies.Get("account");

                if (cookies != null && !string.IsNullOrEmpty(cookies.Value))
                    Server.Context.ChangeAccount(uint.Parse(cookies.Value));
            }
        }
    }
}