using MVC_SYSTEM.AuthModels;
using MVC_SYSTEM.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    public class MobileRequestController : Controller
    {
        private MVC_SYSTEM_Auth db = new MVC_SYSTEM_Auth();
        private EncryptDecrypt Encrypt = new EncryptDecrypt();
        
        public JsonResult UsersRequest(int fld_KmplnSyrktID, int fldNegaraID, int fldSyarikatID, int fldWilayahID, int fldLadangID)
        {
            var users = db.tblUsers.Where(x => x.fld_KmplnSyrktID == fld_KmplnSyrktID && x.fldNegaraID == fldNegaraID && x.fldSyarikatID == fldSyarikatID && x.fldWilayahID == fldWilayahID && x.fldLadangID == fldLadangID).Select(s => new { s.fldUserName, s.fldUserPassword }).ToArray();
            //string password;
            //password = Encrypt.Decrypt(users.)
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}