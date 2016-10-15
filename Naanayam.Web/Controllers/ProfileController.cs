using Naanayam.Web.Models;
using System.Web.Mvc;

namespace Naanayam.Web.Controllers
{
    public class ProfileController : Base
    {
        // GET: profile/changepassword
        public ActionResult ChangePassword()
        {
            return View();
        }

        // POST: profile/changepassword
        [HttpPost]
        public ActionResult ChangePassword(ChangePassword o)
        {
            ActionResult result = null;

            if (o.NewPassword.Equals(o.ConfirmPassword) && Service.ChangeUserPasword(o.CurrentPassword, o.NewPassword))
            {
                result = RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("Authentication", "Please check your username or password.");

                result = View(o);
            }

            return result;
        }
    }
}