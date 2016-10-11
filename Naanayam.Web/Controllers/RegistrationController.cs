using Naanayam.Web.Filters;
using Naanayam.Web.Models;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Naanayam.Web.Controllers
{
    [HandleException]
    public class RegistrationController : Base
    {
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(Registration o)
        {
            ActionResult result = null;

            if (o != null)
            {
                if (await Service.CreateUserAsync(o.Username, o.Password, o.FirstName, o.LastName, o.Email, string.Empty))
                {
                    result = RedirectToAction("Index", "Login");
                }
                else
                {
                    ModelState.AddModelError("Registration", "Registration cannot be completed");

                    result = View();
                }
            }

            return result;
        }
    }
}