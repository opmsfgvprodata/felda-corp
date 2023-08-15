using System.Web.Mvc;
using System.Web.Routing;

namespace MVC_SYSTEM.Helper
{
    public static class LanguageHelper
    {
        public static MvcHtmlString LangSwitcher(this UrlHelper url, string Name, RouteData routeData, string lang)
        {
            var liTagBuilder = new TagBuilder("li");
            var aTagBuilder = new TagBuilder("a");
            var spanTagBuilder = new TagBuilder("span");
            var routeValueDictionary = new RouteValueDictionary(routeData.Values);
            if (routeValueDictionary.ContainsKey("lang"))
            {
                if (routeData.Values["lang"] as string == lang)
                {
                    liTagBuilder.AddCssClass("active");
                }
                else
                {
                    routeValueDictionary["lang"] = lang;
                }
            }



            aTagBuilder.MergeAttribute("onclick", "return DisplayProgressMessage3(this);");
            aTagBuilder.MergeAttribute("href", url.RouteUrl(routeValueDictionary));
            aTagBuilder.SetInnerText(Name);
            switch (Name)
            {
                case "Bahasa Malaysia":
                    spanTagBuilder.AddCssClass("flag-icon flag-icon-my");

                    break;
                case "English":
                    spanTagBuilder.AddCssClass("flag-icon flag-icon-gb");

                    break;
                case "Indonesia":
                    spanTagBuilder.AddCssClass("flag-icon flag-icon-id");

                    break;
            }
            aTagBuilder.InnerHtml = spanTagBuilder + " " + Name;
            liTagBuilder.InnerHtml = aTagBuilder.ToString();
            return new MvcHtmlString(liTagBuilder.ToString());
        }
    }
}