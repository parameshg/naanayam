using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Naanayam.Web.Startup))]
namespace Naanayam.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
