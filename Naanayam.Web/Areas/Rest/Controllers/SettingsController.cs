using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;
using Naanayam.Web.Filters;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    [HandleException]
    public class SettingsController : Base
    {
        #region Currency

        // GET: api/settings/currency
        [HttpGet]
        [Route("api/settings/currency")]
        public List<string> GetCurrency()
        {
            List<string> result = new List<string>();

            result.AddRange(Server.GetCurrencies());

            return result;
        }

        #endregion

        #region Types

        // GET: api/settings/types
        [HttpGet]
        [Route("api/settings/types")]
        public async Task<JsonResult<List<string>>> GetTypes()
        {
            List<string> result = new List<string>();

            result.AddRange(await Server.GetTransactionTypesAsync());

            return Json(result);
        }

        #endregion

        #region Category

        // GET: api/settings/types/income/category
        [Route("api/settings/types/{type}/category")]
        public async Task<JsonResult<List<string>>> GetCategory(string type)
        {
            List<string> result = new List<string>();

            result.AddRange(await Server.GetTransactionCategoriesAsync(type));

            return Json(result);
        }

        // GET: api/settings/types/income/category/travel
        [Route("api/settings/types/{type}/category/{category}")]
        public async Task<JsonResult<List<string>>> GetCategory(string type, string category)
        {
            List<string> result = new List<string>();

            result.AddRange(await Server.GetTransactionCategoriesAsync(type, category));

            return Json(result);
        }

        // POST: api/settings/types/income/category
        [HttpPost]
        [Route("api/settings/types/{type}/category")]
        public async Task<JsonResult<List<string>>> AddCategory(string type, [FromBody] KeyValuePair<string, string> category)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(category.Value))
                await Server.AddTransactionCategoryAsync(type, category.Value);

            result.AddRange(await Server.GetTransactionCategoriesAsync(type));

            return Json(result);
        }

        // DELETE: api/settings/types/expense/category
        [HttpDelete]
        [Route("api/settings/types/{type}/category/{category}")]
        public async Task<JsonResult<List<string>>> RemoveCategory(string type, string category)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(category))
                await Server.RemoveTransactionCategoryAsync(type, category);

            result.AddRange(await Server.GetTransactionCategoriesAsync(type));

            return Json(result);
        }

        // POST: api/settings/types/expense/category/tax
        [HttpPost]
        [Route("api/settings/types/{type}/category/{category}")]
        public async Task<JsonResult<List<string>>> AddSubCategory(string type, string category, [FromBody] KeyValuePair<string, string> subCategory)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(subCategory.Value))
                await Server.AddTransactionCategoryAsync(type, category, subCategory.Value);

            result.AddRange(await Server.GetTransactionCategoriesAsync(type, category));

            return Json(result);
        }

        // DELETE: api/settings/types/expense/category/tax/sales
        [HttpDelete]
        [Route("api/settings/types/{type}/category/{category}/{subCategory}")]
        public async Task<JsonResult<List<string>>> RemoveCategory(string type, string category, string subCategory)
        {
            List<string> result = new List<string>();

            if (!string.IsNullOrEmpty(type) && !string.IsNullOrEmpty(category) && !string.IsNullOrEmpty(subCategory))
                await Server.RemoveTransactionCategoryAsync(type, category, subCategory);

            result.AddRange(await Server.GetTransactionCategoriesAsync(type, category));

            return Json(result);
        }

        #endregion
    }
}