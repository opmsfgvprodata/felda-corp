using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_SYSTEM.Controllers
{
    public class BaseController : Controller
    {
        private string CurrentLanguageCode { get; set; }

        protected override void Initialize(RequestContext requestContext)
        {
            if (requestContext.RouteData.Values["lang"] != null && requestContext.RouteData.Values["lang"] as string != "null")
            {
                CurrentLanguageCode = (string)requestContext.RouteData.Values["lang"];
                if (CurrentLanguageCode != null)
                {
                    try
                    {
                        Thread.CurrentThread.CurrentCulture = Thread.CurrentThread.CurrentUICulture = new CultureInfo(CurrentLanguageCode);
                        //NumberFormatInfo nfi = new NumberFormatInfo { CurrencySymbol = "£" };
                        //Thread.CurrentThread.CurrentUICulture.NumberFormat = Thread.CurrentThread.CurrentCulture.NumberFormat = nfi;
                    }
                    catch (Exception)
                    {
                        throw new NotSupportedException($"Invalid language code '{CurrentLanguageCode}'.");
                    }
                }
            }
            base.Initialize(requestContext);
        }
    }
}