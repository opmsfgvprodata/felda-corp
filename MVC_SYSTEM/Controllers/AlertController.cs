using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    public class AlertController : Controller
    {
        // GET: Users
        public ActionResult Denied()
        {
            ViewBag.Message = "You are not authorized to access this page ."; // 13 Jan 2021 sepul tukar ayat dari You cannot access this page. 
            return View();
        }

        [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3")]
        public ActionResult AlreadySignIn()
        {
            GetIdentity name = new GetIdentity();
            ViewBag.Message = "System has been login by " + name.MyName(User.Identity.Name) + ". Please logout this user to login another user.";
            return View();
        }

        public ActionResult error404()
        {
            ViewBag.Message = "Sorry page is not exist. TQ";
            return View();
        }

        public ActionResult error400()
        {
            ViewBag.Message = "Sorry you cannot go this page. TQ";
            return View();
        }
    }
}