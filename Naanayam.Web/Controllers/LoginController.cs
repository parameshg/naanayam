using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Naanayam.Web.Filters;
using Naanayam.Web.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Naanayam.Web.Controllers
{
    [Authorize]
    [HandleException]
    public class LoginController : Base
    {
        public IAuthenticationManager LoginManager { get { return HttpContext.GetOwinContext().Authentication; } }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(Login o)
        {
            ActionResult result = null;

            if (o != null)
            {
                if (await Service.AuthenticateAsync(o.Username, o.Password))
                {
                    result = RedirectToAction("Index", "Home");

                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()),
                        new Claim(ClaimTypes.Name, o.Username)
                    };

                    var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

                    LoginManager.SignIn(new AuthenticationProperties(), identity);
                }
                else
                {
                    ModelState.AddModelError("Authentication", "Login failed. Please check your username or password.");

                    result = View(o);
                }
            }

            return result;
        }

        public ActionResult Exit()
        {
            LoginManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

            return RedirectToAction("Index", "Home");
        }
    }
}