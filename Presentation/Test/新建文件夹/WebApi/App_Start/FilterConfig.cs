using System.Web;
using System.Web.Mvc;
using WebApi.Common.Files;

namespace WebApi
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new GlobalErrorFilter());
        }
    }
}
