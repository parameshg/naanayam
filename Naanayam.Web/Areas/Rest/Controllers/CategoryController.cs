using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    public class CategoryController : Base
    {
        // GET: api/transaction/category
        [Route("api/transaction/category")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> Get()
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }

        // GET: api/transaction/category/5
        [Route("api/transaction/category/{category}")]
        public async Task<JsonResult<List<string>>> Get(string category)
        {
            List<string> result = new List<string>();

            result.AddRange(await Server.GetTransactionCategoriesAsync(category));

            return Json(result);
        }

        // POST: api/transaction/category
        [Route("api/transaction/category")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> Post([FromBody]string o)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            await Server.AddTransactionCategoryAsync(o);

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }

        // DELETE: api/transaction/category/5
        [Route("api/transaction/category/{category}")]
        public async Task<JsonResult<Dictionary<string, List<string>>>> Delete(string category)
        {
            Dictionary<string, List<string>> result = new Dictionary<string, List<string>>();

            await Server.RemoveTransactionCategoryAsync(category);

            foreach (var i in await Server.GetTransactionCategoriesAsync())
                result.Add(i.Key, new List<string>(i.Value));

            return Json(result);
        }
    }
}