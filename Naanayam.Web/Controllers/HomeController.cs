using Naanayam.Web.Filters;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Naanayam.Web.Controllers
{
    [HandleException]
    public class HomeController : Base
    {
        public async Task<ActionResult> Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                var account = (await Service.GetAccountsAsync()).FirstOrDefault();

                if (account != null)
                    Response.Cookies.Add(new System.Web.HttpCookie("account", account.ID.ToString()));
            }

            return View();
        }
    }
}