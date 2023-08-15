using MVC_SYSTEM.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Class
{
    public class GetDataCount
    {

        //private MVC_SYSTEM_SP_Models db3 = new MVC_SYSTEM_SP_Models();
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();

        //public int datacount(int negaraid, int syarikatid, int wilayahid, int ladangid, string monthstring, int year)
        //{
        //    int totalcount = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Count();
        //    return totalcount;
        //}

        //public decimal SumDbt(int negaraid, int syarikatid, int wilayahid, int ladangid, string monthstring, int year)
        //{
        //    decimal? sum = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Select(s => s.fldDebit).Sum();
        //    decimal getsum = sum.Value;
        //    return getsum;
        //}
        //public decimal SumKdt(int negaraid, int syarikatid, int wilayahid, int ladangid, string monthstring, int year)
        //{
        //    decimal? sum = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Select(s => s.fldKredit).Sum();
        //    decimal getsum = sum.Value;
        //    return getsum;
        //}

        //public decimal getgaji(int negaraid, int syarikatid, int wilayahid, int ladangid, string monthstring, int year)
        //{
        //    decimal? gajiBuruh = 0;
        //    gajiBuruh = db3.sp_RptTransPek(negaraid, syarikatid, wilayahid, ladangid, monthstring, year).Where(s => s.fldLejar == "452").Select(s => s.fldKredit).Sum();
        //    decimal getgaji = gajiBuruh.Value;
        //    return getgaji;
        //}

        public int detailpkrjaCount(int month, int wilayahid, int ladangid, string kerakyatan)
        {
            if (month >= 0)
            {
                if (wilayahid == 0 && ladangid == 0 && kerakyatan == "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid != 0 && ladangid == 0 && kerakyatan == "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid != 0 && ladangid != 0 && kerakyatan == "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid == 0 && ladangid == 0 && kerakyatan != "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid != 0 && ladangid == 0 && kerakyatan != "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt == month && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
            }
            else
            {
                if (wilayahid == 0 && ladangid == 0 && kerakyatan == "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid != 0 && ladangid == 0 && kerakyatan == "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid != 0 && ladangid != 0 && kerakyatan == "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid == 0 && ladangid == 0 && kerakyatan != "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else if (wilayahid != 0 && ladangid == 0 && kerakyatan != "ALL")
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
                else
                {
                    int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt <= -1 && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                    return totalcount;
                }
            }
        }

        public int detailpkrjaCount2(int wilayahid, int ladangid, string kerakyatan)
        {
            if (wilayahid == 0 && ladangid == 0 && kerakyatan == "ALL")
            {
                int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                return totalcount;
            }
            else if (wilayahid != 0 && ladangid == 0 && kerakyatan == "ALL")
            {
                int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                return totalcount;
            }
            else if (wilayahid != 0 && ladangid != 0 && kerakyatan == "ALL")
            {
                int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                return totalcount;
            }
            else if (wilayahid == 0 && ladangid == 0 && kerakyatan != "ALL")
            {
                int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                return totalcount;
            }
            else if (wilayahid != 0 && ladangid == 0 && kerakyatan != "ALL")
            {
                int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                return totalcount;
            }
            else
            {
                int totalcount = db.vw_DetailPekerja.Where(x => x.fld_BilBlnTmtPsprt != null && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_Kdrkyt == kerakyatan && x.fld_Kdaktf == "0").Select(s => s.fld_Nama1).Count();
                return totalcount;
            }

        }

        public int SkbCount(int month, int year, int negaraid, int syarikatid, int wlyhid, int ladangid)
        {
            string monthstring = month.ToString();
            if (monthstring.Length == 1)
            {
                monthstring = "0" + monthstring;
            }
            int skbcount = 0; //db3.sp_RptTransPek(negaraid, syarikatid, wlyhid, ladangid, monthstring, year).Where(x => x.fldLejar == "452").Count();//db.vw_skb.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wlyhid).Select(s => s.fld_skb).Count();
            return skbcount;
        }
        public int SkbCount2(int month, int year, int wlyhid, int ldgid)
        {
            string monthstring = month.ToString();
            if (monthstring.Length == 1)
            {
                monthstring = "0" + monthstring;
            }
            int skbcount = db.vw_skb.Where(x => x.fld_Bulan == monthstring && x.fld_Tahun == year && x.fld_WilayahID == wlyhid && x.fld_LadangID == ldgid).Select(s => s.fld_skb).Count();
            return skbcount;
        }

        public int getSameDateWorkerReport(int negaraid, int syarikatid, int wilayahid, int ladangid, int month, int year, string nopkj, DateTime? rowdate)
        {
            int result = 0;

            result = db.vw_KerjaHarian.Where(x => x.fld_Tarikh.Value.Month == month && x.fld_Tarikh.Value.Year == year && x.fld_Nopkj == nopkj && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_SyarikatID == syarikatid && x.fld_NegaraID == negaraid && x.fld_Tarikh == rowdate).Count();

            return result;
        }

        public int countGajiByLadang(int month, int year, int wilayahid, int ladangid)
        {
            if (ladangid == 0)
            {
                int resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_TerimaHQ_Status == 1).Count();
                return resultreport;
            }
            else
            {
                int resultreport = db.tbl_SokPermhnWang.Where(x => x.fld_Month == month && x.fld_Year == year && x.fld_WilayahID == wilayahid && x.fld_LadangID == ladangid && x.fld_TerimaHQ_Status == 1).Count();
                return resultreport;
            }

        }

        public int pkjAppcount(int fileid, int ldgid, int wlyhid, int ngra, int syrkt)
        {
            MVC_SYSTEM_ModelsCorporate dbhq = new MVC_SYSTEM_ModelsCorporate();
            int count = dbhq.tblPkjmastApp.Where(x => x.fldFileID == fileid && x.fldLadangID == ldgid && x.fldWilayahID == wlyhid && x.fldNegaraID == ngra && x.fldSyarikatID == syrkt && x.fldStatus == 2).Count();
            return count;
        }
    }
}