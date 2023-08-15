using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;
using MVC_SYSTEM.log;

namespace MVC_SYSTEM
{
    public class MvcApplication : System.Web.HttpApplication
    {
        ChangeTimeZone timezone = new ChangeTimeZone();
        errorlog geterror = new errorlog();
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorViewEngine());
        }

        void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            HttpCookie authoCookies = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authoCookies != null)
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authoCookies.Value);
                JavaScriptSerializer js = new JavaScriptSerializer();
                tblUser user = js.Deserialize<tblUser>(ticket.UserData);
                MyIdentity myIdentity = new MyIdentity(user);
                MyPrincipal myPrincipal = new MyPrincipal(myIdentity);
                HttpContext.Current.User = myPrincipal;
            }
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            if (!exception.ToString().Contains("GetPDF"))
            {
                geterror.catcherro(exception.Message, exception.StackTrace, exception.Source, exception.TargetSite.ToString());
                Server.ClearError();
                string appname = Request.ApplicationPath;
                string domain = Request.Url.GetLeftPart(UriPartial.Authority);
                var lang = Request.RequestContext.RouteData.Values["lang"];
                
                if (appname != "/")
                {
                    domain = domain + appname + "/" + lang.ToString();
                }
                else
                {
                    domain = domain + "/" + lang.ToString();
                }
                Response.Redirect(domain + "/ErrorHandler");
            }

        }
    }
}
