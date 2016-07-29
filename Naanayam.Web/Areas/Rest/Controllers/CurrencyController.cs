using System.Collections.Generic;

namespace Naanayam.Web.Areas.Rest.Controllers
{
    public class CurrencyController : Base
    {
        // GET: api/currency
        public List<string> Get()
        {
            List<string> result = new List<string>();

            result.AddRange(Server.GetCurrencies());

            return result;
        }
    }
}