﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    public class TransactionController : Base
    {
        // GET: api/account/5/transaction
        [Route("api/account/{account}/transaction/{year}/{month}")]
        public async Task<JsonResult<List<Transaction>>> Get(uint account, int? year, int? month)
        {
            List<Transaction> result = new List<Transaction>();

            DateTime? searchFrom = null;
            DateTime? searchTo = null;

            if (year.HasValue && month.HasValue)
            {
                searchFrom = DateTime.Parse(string.Format("01/{0}/{1}", month.Value, year.Value));
                searchTo = searchFrom.Value.AddMonths(1);
            }

            result.AddRange(await Server.GetTransactionsAsync(account, searchFrom, searchTo));

            return Json(result);
        }

        // POST: api/account/transaction
        [Route("api/transaction")]
        public async Task Post([FromBody]Transaction o)
        {
            await Server.CreateTransactionAsync(o.Account, o.Timestamp, o.Type, o.Category, o.Description, o.Amount);
        }

        // PUT: api/account/transaction
        [Route("api/transaction")]
        public async Task Put([FromBody]Transaction o)
        {
            await Server.UpdateTransactionAsync(o.ID, o.Timestamp, o.Type, o.Category, o.Description, o.Amount);
        }

        // DELETE: api/account/5/transactions/5
        [Route("api/account/{account}/transaction/{id}")]
        public async Task Delete(uint account, uint id)
        {
            await Server.DeleteTransactionAsync(id);
        }
    }
}