using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Super Admin,Admin 1,Admin 2,Admin 3,Super Power User,Super User")]
    public class GetProcessStatusController : Controller
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private GetIdentity getidentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();
        // GET: GetProcessStatus
        public JsonResult Index()
        {
            bool status = false;
            string percentstatus = "";
            int? NegaraID = 0;
            int? SyarikatID = 0;
            int? WilayahID = 0;
            int? LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            int? getclientid = getidentity.ClientID(User.Identity.Name);
            decimal? totalpercent = 0;
            decimal? totalrunningpercent = 100;

            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var getProcessStatus = db.tbl_SevicesProcess.Where(x => x.fld_ClientID == getclientid && x.fld_Flag == 1).FirstOrDefault();

            List<vw_ServicesProcess> ServicesProcessList = new List<vw_ServicesProcess>();

            if (WilayahID == 0 && LadangID == 0)
            {
                ServicesProcessList = db.vw_ServicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Flag == 1 && (x.fld_Category == "HQ" || x.fld_Category == "REGION")).ToList();
            }
            else if (WilayahID != 0 && LadangID == 0)
            {
                ServicesProcessList = db.vw_ServicesProcess.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && ((x.fld_ClientID == getclientid && x.fld_Flag == 1 && x.fld_Category == "REGION") || (x.fld_Flag == 1 && x.fld_Category == "HQ"))).ToList();
            }
            else if (WilayahID != 0 && LadangID != 0)
            {
                
            }
            
            if (ServicesProcessList.Count() != 0)
            {
                var datacount = ServicesProcessList.Count(); 
                foreach (var totalpercentcount in ServicesProcessList)
                {
                    totalpercent = totalpercent + totalpercentcount.fld_ProcessPercentage;
                    if (datacount == 1)
                    {
                        totalrunningpercent = 100;
                    }
                    else
                    {
                        totalrunningpercent = totalrunningpercent + 100;
                    }
                }
                totalpercent = (totalpercent / totalrunningpercent) * 100;
                status = true;
                percentstatus = Math.Round((double)totalpercent, 2).ToString();
            }
            return Json(new { status = status, percentstatus = percentstatus }, JsonRequestBehavior.AllowGet);
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