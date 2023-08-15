using MVC_SYSTEM.Attributes;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LocalizationAttribute("ms"), 0);
        }
    }
}
