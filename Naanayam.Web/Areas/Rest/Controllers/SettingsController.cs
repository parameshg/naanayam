using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Naanayam.Web.Areas.Rest.Controllers
{
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

        #region Type

        // GET: api/settings/type
        [Route("api/settings/type")]
        public async Task<JsonResult<List<string>>> GetType()
        {
            List<string> result = new List<string>();

            result.AddRange(await Server.GetTransactionTypesAsync());

            return Json(result);
        }

        #endregion

        #region Category

        // GET: api/settings/category
        [HttpGet]
        [Route("api/settings/category")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> GetCategory()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }

        // POST: api/settings/category
        [HttpPost]
        [Route("api/settings/category")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> AddCategory([FromBody]string o)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            await Server.AddTransactionCategoryAsync(o);

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }

        // DELETE: api/settings/category/5
        [HttpDelete]
        [Route("api/settings/category/{category}")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> RemoveCategory(string category)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            await Server.RemoveTransactionCategoryAsync(category);

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }

        // POST: api/settings/sub-category/5
        [HttpPost]
        [Route("api/settings/sub-category/{category}")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> AddSubCategory(string category, [FromBody]string subCategory)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            await Server.AddTransactionCategoryAsync(category, subCategory);

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }

        // DELETE: api/settings/sub-category/5/1
        [HttpDelete]
        [Route("api/settings/sub-category/{category}/{subCategory}")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> RemoveCategory(string category, string subCategory)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            await Server.RemoveTransactionCategoryAsync(category, subCategory);

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }

        #endregion
    }
}