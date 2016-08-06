using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Naanayam.Web.Filters;

namespace Naanayam.Web.Controllers
{
    [HandleException]
    public class HomeController : Base
    {
        public async Task<ActionResult> Index()
        {
            return View();
        }
    }
}