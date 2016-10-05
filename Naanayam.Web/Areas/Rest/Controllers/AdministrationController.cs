using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Naanayam.Web.Filters;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    [HandleException]
    public class AdministrationController : Base
    {
        // GET: api/admin/install
        [HttpGet]
        [Route("api/admin/install")]
        public bool Install()
        {
            bool result = false;

            result = Server.Install();

            return result;
        }

        // GET: api/admin/uninsall
        [HttpGet]
        [Route("api/admin/uninsall")]
        public bool Uninstall()
        {
            bool result = false;

            result = Server.Uninstall();

            return result;
        }
    }
}
