﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Naanayam.Web.Filters;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    [HandleException]
    public class ReportController : Base
    {
        // GET: api/account/5/report/monthly-expenses-by-category/2016/8
        [Route("api/account/{account}/report/monthly-expenses-by-category/{year}/{month}")]
        public async Task<JsonResult<List<Report.CategoryValue>>> Get(uint account, int? year, int? month)
        {
            List<Report.CategoryValue> result = new List<Report.CategoryValue>();

            DateTime? searchFrom = null;
            DateTime? searchTo = null;

            if (year.HasValue && month.HasValue)
            {
                searchFrom = DateTime.ParseExact(string.Format("01/{0}/{1}", month.Value, year.Value), "dd/M/yyyy", CultureInfo.InvariantCulture);
                searchTo = searchFrom.Value.AddMonths(1);
            }

            result = await Server.GetMonthlyExpensesByCategoryReportAsync(account, searchFrom, searchTo);

            return Json(result);
        }
    }
}