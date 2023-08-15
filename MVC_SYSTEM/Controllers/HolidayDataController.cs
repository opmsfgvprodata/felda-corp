using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HtmlAgilityPack;
using MVC_SYSTEM.TrickModel;
using System.Globalization;
using Microsoft.Ajax.Utilities;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Attributes;
using MVC_SYSTEM.ModelsCorporate;
using tbl_CutiUmum = MVC_SYSTEM.Models.tbl_CutiUmum;
using System.Data.Entity.Validation;

namespace MVC_SYSTEM.Controllers
{
    [AccessDeniedAuthorizeAttribute(Roles = "Super Power Admin,Admin 1,Admin 2,Admin 3")]
    public class HolidayDataController : Controller
    {
        // GET: GetHolidayData
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private ChangeTimeZone timezone = new ChangeTimeZone();
        private errorlog geterror = new errorlog();
        private GetIdentity GetIdentity = new GetIdentity();
        private GetNSWL GetNSWL = new GetNSWL();

        public ActionResult Index()
        {
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drprangeyear; i <= drpyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }
            ViewBag.YearList = yearlist;
            return View();
        } 

        [HttpPost]
        public ActionResult Index(short YearList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            WebClient WebClient = new WebClient();
            HtmlDocument doc = new HtmlDocument();
            List<ModelsCorporate.tbl_CutiUmum> HolidayList = new List<ModelsCorporate.tbl_CutiUmum>();
            DateTime HolidayDate = new DateTime();
            List<List<string>> holidays = new List<List<string>>();
            //int i = 1;
            string page = "";
            int year = timezone.gettimezone().Year;
            int month = timezone.gettimezone().AddMonths(-1).Month;
            int drpyear = 0;
            int drprangeyear = 0;
            string keterangan = "";
            drpyear = timezone.gettimezone().Year + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();

            for (var i = drprangeyear; i <= drpyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            var getregions = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "negeri" && x.fldOptConfValue != "16" && x.fldDeleted == false).Select(s => new { s.fldOptConfFlag2, s.fldOptConfValue }).ToList();

            try
            {
                var checkgenerateyear = db.tbl_CutiUmum.Where(x => x.fld_Tahun == YearList).Count();

                if (checkgenerateyear == 0)
                {
                    foreach (var getregion in getregions)
                    {
                        page = WebClient.DownloadString("http://www.officeholidays.com/countries/malaysia/regional.php?list_year=" + YearList + "&list_region=" + getregion.fldOptConfFlag2.ToLower().Replace(' ', '_') + "");
                        doc.LoadHtml(page);
                        holidays = doc.DocumentNode.SelectSingleNode("//table[@class='list-table']")
                        .Descendants("tr")
                        .Skip(1)
                        .Where(tr => tr.Elements("td").Count() > 1)
                        .Select(tr => tr.Elements("td").Select(td => td.InnerText.Trim()).ToList())
                        .ToList();


                        string getdate = "";
                        foreach (var holiday in holidays)
                        {
                            if (holiday[2].ToUpper().Trim() == "MOTHER'S DAY" || holiday[2].ToUpper().Trim() == "FATHER'S DAY")
                            {

                            }
                            else
                            {
                                getdate = holiday[1].Replace("\r\n", "*");
                                getdate = getdate.Remove(getdate.IndexOf("*"));
                                HolidayDate = DateTime.ParseExact(getdate + ", " + YearList + "", "MMMM d, yyyy", CultureInfo.InvariantCulture);
                                keterangan = holiday[2].ToUpper().Trim();
                                HolidayList.Add(new ModelsCorporate.tbl_CutiUmum() { fld_TarikhCuti = HolidayDate, fld_KeteranganCuti = keterangan, fld_NegaraID = NegaraID, fld_SyarikatID = SyarikatID, fld_Deleted = false, fld_Tahun = YearList, fld_Negeri = getregion.fldOptConfValue });
                                //geterror.catcherro(holiday[0].ToUpper().Trim(), HolidayDate.ToString(), keterangan, holiday[3].ToUpper().Trim());
                            }
                        }
                    }

                    if (HolidayList != null)
                    {
                        try
                        {
                            db.tbl_CutiUmum.AddRange(HolidayList);
                            db.SaveChanges();
                        }

                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                    }

                    ModelState.AddModelError("", "Success to get the holiday");
                }
                else
                {
                    ModelState.AddModelError("", "Holiday already added for this year");
                }
                
            }
            catch(Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
                ModelState.AddModelError("", "Problem to get holiday from source");
            }

            ViewBag.YearList = yearlist;
            return View();
        }

        public ActionResult PopulatePublicHolidayMaster()
        {
            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drprangeyear; i <= drpyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            return View();
        }

        [HttpPost]
        public ActionResult PopulatePublicHolidayMaster(short YearList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            List<tbl_CutiUmumMaster> cutiUmumMaster = new List<tbl_CutiUmumMaster>();

            var holidayData = db.tbl_CutiUmum.Where(x =>
                    x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Tahun == YearList &&
                    x.fld_Deleted == false)
                .GroupBy(x => new {x.fld_KeteranganCuti, x.fld_TarikhCuti})
                .Select(s => new {s.Key.fld_KeteranganCuti, s.Key.fld_TarikhCuti});

            foreach (var holiday in holidayData)
            {
                cutiUmumMaster.Add(new ModelsCorporate.tbl_CutiUmumMaster
                {
                    fld_KeteranganCuti = holiday.fld_KeteranganCuti,
                    fld_TarikhCuti = holiday.fld_TarikhCuti,
                    fld_Tahun = YearList,
                    fld_NegaraID = NegaraID,
                    fld_SyarikatID = SyarikatID,
                    fld_Deleted = false,
                });
            }

            db.tbl_CutiUmumMaster.AddRange(cutiUmumMaster);
            db.SaveChanges();

            ViewBag.YearList = YearList;

            return View();
        }

        public ActionResult MapPublicHoliday()
        {
            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drprangeyear; i <= drpyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            return View();
        }

        [HttpPost]
        public ActionResult MapPublicHoliday(short YearList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var holidayData = db.tbl_CutiUmum.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Tahun == YearList &&
                x.fld_Deleted == false);

            var holidayMasterData = db.tbl_CutiUmumMaster.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Tahun == YearList &&
                x.fld_Deleted == false);

            foreach (var holidayMaster in holidayMasterData)
            {
                foreach (var holiday in holidayData)
                {
                    if (holidayMaster.fld_KeteranganCuti == holiday.fld_KeteranganCuti)
                    {
                        holiday.fld_CutiUmumMasterID = holidayMaster.fld_CutiUmumMasterID;
                    }
                }
            }

            db.SaveChanges();

            ViewBag.YearList = YearList;

            return View();
        }

        public ActionResult PopulateHolidayEligibility()
        {
            int drpyear = 0;
            int drprangeyear = 0;

            drpyear = timezone.gettimezone().Year + 1;
            drprangeyear = timezone.gettimezone().Year;

            var yearlist = new List<SelectListItem>();
            for (var i = drprangeyear; i <= drpyear; i++)
            {
                if (i == timezone.gettimezone().Year)
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
                }
                else
                {
                    yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            ViewBag.YearList = yearlist;

            return View();
        }

        [HttpPost]
        public ActionResult PopulateHolidayEligibility(short YearList)
        {
            int? getuserid = GetIdentity.ID(User.Identity.Name);
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

            var ladangData = db.tbl_Ladang.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Deleted == false);

            var holidayData = db.tbl_CutiUmum.Where(x =>
                x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_Tahun == YearList &&
                x.fld_Deleted == false);

            List<tbl_CutiUmumKelayakan> cutiUmumKelayakanList = new List<tbl_CutiUmumKelayakan>();

            foreach (var ladang in ladangData)
            {
                foreach (var holiday in holidayData)
                {

                    if (ladang.fld_KodNegeri == holiday.fld_Negeri)
                    {
                        cutiUmumKelayakanList.Add(new ModelsCorporate.tbl_CutiUmumKelayakan
                        {
                            fld_CutiMasterID = holiday.fld_CutiUmumMasterID,
                            fld_NegaraID = NegaraID,
                            fld_SyarikatID = SyarikatID,
                            fld_WilayahID = ladang.fld_WlyhID,
                            fld_LadangID = ladang.fld_ID,
                            fld_Deleted = false
                        });
                    }
                }
            }

            db.tbl_CutiUmumKelayakan.AddRange(cutiUmumKelayakanList);
            db.SaveChanges();

            ViewBag.YearList = YearList;

            return View();
        }

        //public ActionResult PopulatePublicHolidayEstate()
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    int drpyear = 0;
        //    int drprangeyear = 0;

        //    drpyear = timezone.gettimezone().Year + 1;
        //    drprangeyear = timezone.gettimezone().Year;

        //    var yearlist = new List<SelectListItem>();
        //    for (var i = drprangeyear; i <= drpyear; i++)
        //    {
        //        if (i == timezone.gettimezone().Year)
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });
        //        }
        //        else
        //        {
        //            yearlist.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
        //        }
        //    }

        //    ViewBag.YearList = yearlist;

        //    return View();
        //}

        //[HttpPost]
        //public ActionResult PopulatePublicHolidayMaster(short YearList)
        //{
        //    int? getuserid = GetIdentity.ID(User.Identity.Name);
        //    int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
        //    GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);

        //    List<tbl_CutiUmumMaster> cutiUmumMaster = new List<tbl_CutiUmumMaster>();

        //    var holidayData = db.tbl_CutiUmum.Where(x => x.fld_Tahun == YearList && x.fld_Deleted == false)
        //        .GroupBy(x => new { x.fld_KeteranganCuti, x.fld_TarikhCuti })
        //        .Select(s => new { s.Key.fld_KeteranganCuti, s.Key.fld_TarikhCuti });

        //    foreach (var holiday in holidayData)
        //    {
        //        cutiUmumMaster.Add(new ModelsCorporate.tbl_CutiUmumMaster
        //        {
        //            fld_KeteranganCuti = holiday.fld_KeteranganCuti,
        //            fld_TarikhCuti = holiday.fld_TarikhCuti,
        //            fld_Tahun = YearList,
        //            fld_NegaraID = NegaraID,
        //            fld_SyarikatID = SyarikatID,
        //            fld_Deleted = false,
        //        });
        //    }

        //    db.tbl_CutiUmumMaster.AddRange(cutiUmumMaster);
        //    db.SaveChanges();

        //    ViewBag.YearList = YearList;

        //    return View();
        //}
    }
}