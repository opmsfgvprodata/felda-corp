using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_SYSTEM.Class
{
    public class GetCalculateTotal
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        
        public int GetTotalAuditTrail(int year, int month, int wilid)
        {
            int gettotal = 0;
            switch (month)
            {
                case 1:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln1 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 2:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln2 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 3:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln3 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 4:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln4 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 5:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln5 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 6:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln6 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 7:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln7 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 8:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln8 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 9:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln9 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 10:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln10 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 11:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln11 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;
                case 12:
                    gettotal = db.vw_AuditTrail.Where(x => x.fld_Bln12 == 1 && x.fld_Thn == year && x.fld_WilayahID == wilid).Count();
                    break;

            }

            return gettotal;
        }
    }
}