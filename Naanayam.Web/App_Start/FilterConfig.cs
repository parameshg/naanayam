using System.Web;
using System.Web.Mvc;
using Naanayam.Web.Filters;

namespace Naanayam.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleException());
            filters.Add(new RequestLogger());
        }
    }
}