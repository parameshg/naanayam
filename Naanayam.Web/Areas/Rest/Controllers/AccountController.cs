﻿using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http.Results;
using System.Web.Http;
using Naanayam.Web.Filters;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    [HandleException]
    public class AccountController : Base
    {
        // GET: api/accounts
        [Route("api/accounts")]
        public async Task<JsonResult<List<Account>>> Get()
        {
            List<Account> result = new List<Account>();

            result.AddRange(await Server.GetAccountsAsync());

            return Json(result);
        }

        // POST: api/accounts
        [Route("api/accounts")]
        public async Task<JsonResult<List<Account>>> Post(Account o)
        {
            List<Account> result = new List<Account>();

            await Server.CreateAccountAsync(o.Name, o.Description, o.Currency);

            result.AddRange(await Server.GetAccountsAsync());

            return Json(result);
        }

        // PUT: api/accounts
        [Route("api/accounts")]
        public async Task<JsonResult<List<Account>>> Put(Account o)
        {
            List<Account> result = new List<Account>();

            await Server.UpdateAccountAsync(o.ID, o.Name, o.Description, o.Currency);

            result.AddRange(await Server.GetAccountsAsync());

            return Json(result);
        }

        // DELETE: api/accounts/5
        [Route("api/accounts/{id}")]
        public async Task<JsonResult<List<Account>>> Delete(string id)
        {
            List<Account> result = new List<Account>();

            await Server.DeleteAccountAsync(uint.Parse(id));

            result.AddRange(await Server.GetAccountsAsync());

            return Json(result);
        }
    }
}