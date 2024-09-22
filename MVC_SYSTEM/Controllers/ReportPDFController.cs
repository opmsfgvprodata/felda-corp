using Dapper;
using iTextSharp.text.pdf;
using iTextSharp.text;
using MVC_SYSTEM.Class;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.ModelsCorporate;
using MVC_SYSTEM.ModelsDapper;
using MVC_SYSTEM.ModelsEstate;
using MVC_SYSTEM.ModelsSP;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVC_SYSTEM.Controllers
{
    public class ReportPdfController : Controller
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity getidentity = new GetIdentity();
        GetNSWL GetNSWL = new GetNSWL();
        Connection Connection = new Connection();
        ChangeTimeZone timezone = new ChangeTimeZone();
        GetConfig GetConfig = new GetConfig();
        errorlog geterror = new errorlog();
        GetTriager GetTriager = new GetTriager();
        GeneralClass generalClass = new GeneralClass();
        // GET: ReportPdf
        public FileStreamResult PocketCheckrollPdf(int? WilayahList, int? LadangList,int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string StatusList, string WorkCategoryList)
        {
            Connection Connection = new Connection();
            int? NegaraID, SyarikatID, WilayahID, LadangID = 0;
            int? getuserid = getidentity.ID(User.Identity.Name);
            string host, catalog, user, pass = "";
            GetNSWL.GetData(out NegaraID, out SyarikatID, out WilayahID, out LadangID, getuserid, User.Identity.Name);
            WilayahID = WilayahList;
            LadangID = LadangList;
            Connection.GetConnection(out host, out catalog, out user, out pass, WilayahList, SyarikatID.Value, NegaraID.Value);
            MVC_SYSTEM_ModelsEstate dbr = MVC_SYSTEM_ModelsEstate.ConnectToSqlServer(host, catalog, user, pass);

            Document pdfDoc = new Document(PageSize.A3.Rotate(), 10, 10, 5, 5);
            MemoryStream ms = new MemoryStream();
            MemoryStream output = new MemoryStream();
            PdfWriter pdfWriter = PdfWriter.GetInstance(pdfDoc, ms);
            Chunk chunk = new Chunk();
            Paragraph para = new Paragraph();
            pdfDoc.Open();
            var pkjList = new List<ModelsEstate.tbl_Pkjmast>();
            var nswl = db.vw_NSWL.Where(x => x.fld_LadangID == LadangID).FirstOrDefault();
            if (WorkCategoryList == "0" || WorkCategoryList == null)
            {
                if (RadioGroup == 0)
                {
                    //individu
                    if (StatusList == "0")
                    {
                        // aktif & xaktif
                        if (SelectionList == "0")
                        {
                            //semua individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();

                        }
                        else
                        {
                            //selected individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                        }

                    }
                    else
                    {
                        // aktif/xaktif
                        if (SelectionList == "0")
                        {
                            //semua individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                        }
                        else
                        {
                            //selected individu
                            pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Kdaktf == StatusList && x.fld_Nopkj == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                        }
                    }
                }
                else
                {
                    //group
                    if (SelectionList == "0")
                    {
                        //semua group
                        pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                    }
                    else
                    {
                        //selected group
                        var kumpID = dbr.tbl_KumpulanKerja.Where(x => x.fld_KodKumpulan == SelectionList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_KumpulanID).FirstOrDefault();
                        pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
                    }
                }
            }
            else
            {
                pkjList = dbr.tbl_Pkjmast.Where(x => x.fld_Ktgpkj == WorkCategoryList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1).ToList();
            }
            if (pkjList.Count() > 0)
            {
                string[] flag1 = new string[] { "KesukaranMembaja", "KesukaranMenuai", "KesukaranMemunggah", "designation", "jantina" };
                List<ModelsCorporate.tblOptionConfigsWeb> webConfigList = GetConfig.GetWebConfigList(flag1, NegaraID, SyarikatID);
                var pktHargaKesukaran = dbr.tbl_PktHargaKesukaran.Where(x => x.fld_LadangID == LadangID).ToList();
                var pkjNoList = pkjList.Select(s => s.fld_Nopkj).Distinct().ToList();
                var getpkjInfo2 = dbr.tbl_Pkjmast.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_StatusApproved == 1);
                var hardWorkDatas = dbr.tbl_Kerja.Where(x => pkjNoList.Contains(x.fld_Nopkj) && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_LadangID == LadangID && x.fld_HrgaKwsnSkar > 0.00m && !string.IsNullOrEmpty(x.fld_HrgaKwsnSkar.Value.ToString())).ToList();
                var attWorkDatas = dbr.tbl_Kerjahdr.Where(x => pkjNoList.Contains(x.fld_Nopkj) && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_LadangID == LadangID).ToList();
                var hardWorkDataIDs = hardWorkDatas.Select(s => s.fld_ID).ToList();
                var hardWorkDatasNew = dbr.tbl_KerjaKesukaran.Where(x => hardWorkDataIDs.Contains(x.fld_KerjaID.Value)).ToList();

                var tbl_KumpulanKerja = dbr.tbl_KumpulanKerja.Where(x => x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).ToList();
                var NamaSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NamaSyarikat).FirstOrDefault();
                var NoSyarikat = db.tbl_Syarikat.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).Select(s => s.fld_NoSyarikat).FirstOrDefault();
                var NamaLadang = db.tbl_Ladang.Where(x => x.fld_ID == LadangID && x.fld_Deleted == false).Select(s => s.fld_LdgCode + "-" + s.fld_LdgName).FirstOrDefault();

                var datelist = generalClass.GetDateListFunc(MonthList, YearList);
                var activiti = db.tbl_UpahAktiviti.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                var incentive = db.tbl_JenisInsentif.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();
                var incomeIncentiveCode = incentive.Where(x => x.fld_JenisInsentif == "P").Select(s => s.fld_KodInsentif).Distinct().ToList();
                var deductionIncentiveCode = incentive.Where(x => x.fld_JenisInsentif == "T").Select(s => s.fld_KodInsentif).Distinct().ToList();
                var jenisCarumanTambahan = db.tbl_CarumanTambahan.Where(x => x.fld_SyarikatID == SyarikatID && x.fld_NegaraID == NegaraID && x.fld_Deleted == false).ToList();

                var pkjmast = new List<ModelsEstate.tbl_Pkjmast>();
                var kerjahdr = new List<tbl_Kerjahdr>();
                var kerja = new List<tbl_Kerja>();
                var kerjaKesukaran = new List<tbl_KerjaKesukaran>();
                var kerjahdrCuti = new List<tbl_KerjahdrCuti>();
                var kerjaOT = new List<tbl_KerjaOT>();
                var insentif = new List<ModelsEstate.tbl_Insentif>();
                var kerjaBonus = new List<ModelsEstate.tbl_KerjaBonus>();
                var gajiBulanan = new List<ModelsEstate.tbl_GajiBulanan>();
                var carumanTambahan = new List<ModelsEstate.tbl_ByrCarumanTambahan>();

                if (MonthList != null && YearList != null)
                {
                    var workers = new List<Worker>();
                    foreach (var pkjNo in pkjNoList)
                    {
                        workers.Add(new Worker { WorkerID = pkjNo });
                    }
                    var workersDT = workers.ToDataTable();

                    string constr = Connection.GetConnectionString(WilayahID.Value, SyarikatID.Value, NegaraID.Value);
                    var con = new SqlConnection(constr);
                    try
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("NegaraID", NegaraID);
                        parameters.Add("SyarikatID", SyarikatID);
                        parameters.Add("WilayahID", WilayahID);
                        parameters.Add("LadangID", LadangID);
                        parameters.Add("Month", MonthList);
                        parameters.Add("Year", YearList);
                        parameters.Add("Workers", workersDT.AsTableValuedParameter("[dbo].[Workers]"));
                        con.Open();
                        SqlMapper.Settings.CommandTimeout = 300;
                        var pocketCheckroll = SqlMapper.QueryMultiple(con, "sp_PocketCheckroll", parameters);
                        pkjmast = pocketCheckroll.Read<ModelsEstate.tbl_Pkjmast>().ToList();
                        kerjahdr = pocketCheckroll.Read<tbl_Kerjahdr>().ToList();
                        kerja = pocketCheckroll.Read<tbl_Kerja>().ToList();
                        kerjaKesukaran = pocketCheckroll.Read<tbl_KerjaKesukaran>().ToList();
                        kerjahdrCuti = pocketCheckroll.Read<tbl_KerjahdrCuti>().ToList();
                        kerjaOT = pocketCheckroll.Read<tbl_KerjaOT>().ToList();
                        insentif = pocketCheckroll.Read<ModelsEstate.tbl_Insentif>().ToList();
                        kerjaBonus = pocketCheckroll.Read<ModelsEstate.tbl_KerjaBonus>().ToList();
                        gajiBulanan = pocketCheckroll.Read<ModelsEstate.tbl_GajiBulanan>().ToList();
                        carumanTambahan = pocketCheckroll.Read<ModelsEstate.tbl_ByrCarumanTambahan>().ToList();
                        con.Close();
                    }
                    catch (Exception)
                    {
                        throw;
                    }

                }

                foreach (var pkj in pkjNoList)
                {
                    var checkrollPkj = pkjmast.Where(x => x.fld_Nopkj == pkj).FirstOrDefault();
                    var kerjaHdrPkj = kerjahdr.Where(x => x.fld_Nopkj == pkj).ToList();
                    var kerjaPkj = kerja.Where(x => x.fld_Nopkj == pkj).ToList();
                    var kerjaOTPkj = kerjaOT.Where(x => x.fld_Nopkj == pkj).ToList();
                    var kerjahdrCutiPkj = kerjahdrCuti.Where(x => x.fld_Nopkj == pkj).ToList();
                    var incentivePkj = insentif.Where(x => x.fld_Nopkj == pkj).ToList();
                    var gajiPkj = gajiBulanan.Where(x => x.fld_Nopkj == pkj).FirstOrDefault();
                    var carumanTambahanPkj = new List<tbl_ByrCarumanTambahan>();
                    if (gajiPkj != null)
                    {
                        carumanTambahanPkj = carumanTambahan.Where(x => x.fld_GajiID == gajiPkj.fld_ID).ToList();
                    }
                    decimal? totalIncomePkj = 0m;
                    decimal? totalDeductionPkj = 0m;
                    decimal? totalNetIncomePkj = 0m;

                    if (checkrollPkj.fld_KumpulanID != null && kerjaHdrPkj.Count() > 0)
                    {
                        var kumpulan = tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == checkrollPkj.fld_KumpulanID).FirstOrDefault();
                        var kategori = webConfigList.Where(x => x.fldOptConfFlag1 == "designation" && x.fldOptConfValue == checkrollPkj.fld_Ktgpkj && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                        var jantina = webConfigList.Where(x => x.fldOptConfFlag1 == "jantina" && x.fldOptConfValue == checkrollPkj.fld_Kdjnt && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();

                        pdfDoc.NewPage();
                        //Header
                        pdfDoc = Header(pdfDoc, NamaSyarikat, "(" + NoSyarikat + ")\n" + nswl.fld_LdgCode + " - " + nswl.fld_NamaLadang, "Pocket Checkroll Pekerja bagi " + MonthList + "/" + YearList + "");
                        //Header

                        PdfPTable table = new PdfPTable(6);
                        table.WidthPercentage = 100;
                        table.SpacingBefore = 10f;
                        float[] widths = new float[] { 0.5f, 1, 0.5f, 1, 0.5f, 1 };

                        table.SetWidths(widths);
                        PdfPCell cell = new PdfPCell();
                        chunk = new Chunk("ID Pekerja: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(pkj, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Nama Pekerja: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(checkrollPkj.fld_Nama, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Kod Kumpulan: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(kumpulan.fld_Keterangan, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Jawatan: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(kategori, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("Jantina: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(jantina, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk("No KP / Passport: ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        chunk = new Chunk(checkrollPkj.fld_Nokp, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = 0;
                        table.AddCell(cell);

                        pdfDoc.Add(table);

                        table = new PdfPTable(37);
                        table.WidthPercentage = 100;
                        table.SpacingBefore = 5f;
                        widths = new float[] { 2, 2, 1.5f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 1.2f, 2, 2, 2 };
                        table.SetWidths(widths);

                        chunk = new Chunk("Aktiviti", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        cell.Rowspan = 2;
                        table.AddCell(cell);

                        chunk = new Chunk("Kwsn", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        cell.Rowspan = 2;
                        table.AddCell(cell);

                        chunk = new Chunk("UOM", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        cell.Rowspan = 2;
                        table.AddCell(cell);

                        chunk = new Chunk("Tarikh", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Colspan = 31;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Jumlah", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Rowspan = 2;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("Kadar", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Rowspan = 2;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        chunk = new Chunk("RM", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Rowspan = 2;
                        cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                        cell.BorderColor = BaseColor.RED;
                        table.AddCell(cell);

                        for (int x = 1; x <= 31; x++)
                        {
                            chunk = new Chunk(x.ToString(), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                            cell.BorderColor = BaseColor.RED;
                            table.AddCell(cell);
                        }

                        chunk = new Chunk("Kehadiran", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Colspan = 3;
                        table.AddCell(cell);

                        foreach (var date in datelist)
                        {
                            var attandance = kerjaHdrPkj.Where(x => x.fld_Tarikh.Value == date.Date).FirstOrDefault();
                            var attandancecode = attandance != null ? attandance.fld_Kdhdct : "-";

                            chunk = new Chunk(attandancecode, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);
                        }

                        if (datelist.Count() <= 31)
                        {
                            var remainingdatecount = 31 - datelist.Count();
                            for (int x = 1; remainingdatecount >= x; x++)
                            {
                                chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                            }
                        }

                        chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Colspan = 3;
                        table.AddCell(cell);

                        //Normal Work
                        foreach (var item in kerjaPkj.Select(s => new { s.fld_KodAktvt, s.fld_KodPkt }).Distinct().ToList())
                        {
                            var activitiDetail = activiti.Where(x => x.fld_KodAktvt == item.fld_KodAktvt).FirstOrDefault();

                            chunk = new Chunk(activitiDetail.fld_Desc, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk(item.fld_KodPkt, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk(activitiDetail.fld_Unit, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            foreach (var item2 in datelist)
                            {
                                var totalDailyOutcome = kerjaPkj.Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt && x.fld_Tarikh == item2.Date).Sum(s => s.fld_JumlahHasil);
                                if (totalDailyOutcome > 0)
                                {
                                    chunk = new Chunk(totalDailyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                    cell = new PdfPCell(new Phrase(chunk));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Border = Rectangle.BOTTOM_BORDER;
                                    cell.BorderColor = BaseColor.BLACK;
                                    table.AddCell(cell);
                                }
                                else
                                {
                                    chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                    cell = new PdfPCell(new Phrase(chunk));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Border = Rectangle.BOTTOM_BORDER;
                                    cell.BorderColor = BaseColor.BLACK;
                                    table.AddCell(cell);
                                }
                            }

                            if (datelist.Count() <= 31)
                            {
                                var remainingdatecount = 31 - datelist.Count();
                                for (int x = 1; remainingdatecount >= x; x++)
                                {
                                    chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                    cell = new PdfPCell(new Phrase(chunk));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Border = Rectangle.BOTTOM_BORDER;
                                    cell.BorderColor = BaseColor.BLACK;
                                    table.AddCell(cell);
                                }
                            }
                            var totalMonthlyOutcome = kerjaPkj.Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt).Sum(s => s.fld_JumlahHasil);
                            var totalAmt = kerjaPkj.Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt).Sum(s => s.fld_Amount);
                            totalIncomePkj += totalAmt;

                            chunk = new Chunk(totalMonthlyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk(activitiDetail.fld_Harga.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk(totalAmt.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);
                        }
                        //Normal Work

                        //Kesukaran
                        foreach (var item in kerjaPkj.Select(s => new { s.fld_KodAktvt, s.fld_KodPkt }).Distinct().ToList())
                        {
                            var activitiDetail = activiti.Where(x => x.fld_KodAktvt == item.fld_KodAktvt).FirstOrDefault();
                            var totalMonthlyOutcome = kerjaPkj.Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt).Sum(s => s.fld_HrgaKwsnSkar);
                            var totalAmt = kerjaPkj.Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt).Sum(s => s.fld_HrgaKwsnSkar);
                            if (totalAmt > 0)
                            {

                                chunk = new Chunk(activitiDetail.fld_Desc + " - Kesukaran", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(item.fld_KodPkt, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("RM", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                foreach (var item2 in datelist)
                                {
                                    var totalDailyOutcome = kerjaPkj.Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt && x.fld_Tarikh == item2.Date).Sum(s => s.fld_HrgaKwsnSkar);
                                    if (totalDailyOutcome > 0)
                                    {
                                        chunk = new Chunk(totalDailyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                    else
                                    {
                                        chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                }

                                if (datelist.Count() <= 31)
                                {
                                    var remainingdatecount = 31 - datelist.Count();
                                    for (int x = 1; remainingdatecount >= x; x++)
                                    {
                                        chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                }

                                totalIncomePkj += totalAmt;

                                chunk = new Chunk(totalMonthlyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(totalAmt.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                            }

                        }
                        //Kesukaran

                        //Bonus
                        foreach (var item in kerjaPkj.Select(s => new { s.fld_KodAktvt, s.fld_KodPkt }).Distinct().ToList())
                        {
                            var activitiDetail = activiti.Where(x => x.fld_KodAktvt == item.fld_KodAktvt).FirstOrDefault();
                            var totalMonthlyOutcome = kerjaPkj.Join(kerjaBonus, a => a.fld_ID, b => b.fld_KerjaID, (a, b) => new { a, b })
                            .Select(s => new
                            {
                                s.a.fld_KodPkt,
                                s.a.fld_KodAktvt,
                                s.a.fld_Tarikh,
                                s.b.fld_Bonus,
                                s.b.fld_Kadar,
                                s.b.fld_Jumlah
                            })
                            .Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt).Sum(s => s.fld_Jumlah);
                            if (totalMonthlyOutcome > 0)
                            {
                                chunk = new Chunk(activitiDetail.fld_Desc + " - Bonus", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(item.fld_KodPkt, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("JAM", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                                foreach (var item2 in datelist)
                                {
                                    var totalDailyOutcome = kerjaPkj.Join(kerjaBonus, a => a.fld_ID, b => b.fld_KerjaID, (a, b) => new { a, b })
                                        .Select(s => new
                                        {
                                            s.a.fld_KodPkt,
                                            s.a.fld_KodAktvt,
                                            s.a.fld_Tarikh,
                                            s.b.fld_Bonus,
                                            s.b.fld_Kadar,
                                            s.b.fld_Jumlah
                                        })
                                        .Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt && x.fld_Tarikh == item2.Date).Sum(s => s.fld_Jumlah);
                                    if (totalDailyOutcome > 0)
                                    {
                                        chunk = new Chunk(totalDailyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                    else
                                    {
                                        chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                }
                                if (datelist.Count() <= 31)
                                {
                                    var remainingdatecount = 31 - datelist.Count();
                                    for (int x = 1; remainingdatecount >= x; x++)
                                    {
                                        chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                }

                                totalIncomePkj += totalMonthlyOutcome;

                                chunk = new Chunk(totalMonthlyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(totalMonthlyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                            }
                        }
                        //Bonus

                        //OT
                        foreach (var item in kerjaPkj.Select(s => new { s.fld_KodAktvt, s.fld_KodPkt }).Distinct().ToList())
                        {
                            var activitiDetail = activiti.Where(x => x.fld_KodAktvt == item.fld_KodAktvt).FirstOrDefault();
                            var totalMonthlyOutcome = kerjaPkj.Join(kerjaOTPkj, a => a.fld_ID, b => b.fld_KerjaID, (a, b) => new { a, b })
                            .Select(s => new
                            {
                                s.a.fld_KodPkt,
                                s.a.fld_KodAktvt,
                                s.a.fld_Tarikh,
                                s.b.fld_JamOT,
                                s.b.fld_Kadar,
                                s.b.fld_Jumlah
                            })
                            .Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt).Sum(s => s.fld_Jumlah);
                            var totalJamOT = kerjaPkj.Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt).Sum(s => s.fld_JamOT);
                            if (totalJamOT > 0)
                            {
                                chunk = new Chunk(activitiDetail.fld_Desc + " - OT", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(item.fld_KodPkt, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("JAM", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                                foreach (var item2 in datelist)
                                {
                                    var totalDailyOutcome = kerjaPkj.Join(kerjaOTPkj, a => a.fld_ID, b => b.fld_KerjaID, (a, b) => new { a, b })
                                        .Select(s => new
                                        {
                                            s.a.fld_KodPkt,
                                            s.a.fld_KodAktvt,
                                            s.a.fld_Tarikh,
                                            s.b.fld_JamOT,
                                            s.b.fld_Kadar,
                                            s.b.fld_Jumlah
                                        })
                                        .Where(x => x.fld_KodAktvt == item.fld_KodAktvt && x.fld_KodPkt == item.fld_KodPkt && x.fld_Tarikh == item2.Date).Sum(s => s.fld_JamOT);
                                    if (totalDailyOutcome > 0)
                                    {
                                        chunk = new Chunk(totalDailyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                    else
                                    {
                                        chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                }
                                if (datelist.Count() <= 31)
                                {
                                    var remainingdatecount = 31 - datelist.Count();
                                    for (int x = 1; remainingdatecount >= x; x++)
                                    {
                                        chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                        cell = new PdfPCell(new Phrase(chunk));
                                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                        cell.Border = Rectangle.BOTTOM_BORDER;
                                        cell.BorderColor = BaseColor.BLACK;
                                        table.AddCell(cell);
                                    }
                                }

                                totalIncomePkj += totalMonthlyOutcome;

                                chunk = new Chunk(totalJamOT.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                chunk = new Chunk(totalMonthlyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);
                            }
                        }
                        //OT

                        //Cuti
                        if (kerjahdrCutiPkj.Count() > 0)
                        {
                            chunk = new Chunk("Cuti Berbayar", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk("RM", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            foreach (var item2 in datelist)
                            {
                                var totalDailyOutcome = kerjahdrCutiPkj.Where(x => x.fld_Tarikh == item2.Date).Sum(s => s.fld_Jumlah);
                                if (totalDailyOutcome > 0)
                                {
                                    chunk = new Chunk(totalDailyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                    cell = new PdfPCell(new Phrase(chunk));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Border = Rectangle.BOTTOM_BORDER;
                                    cell.BorderColor = BaseColor.BLACK;
                                    table.AddCell(cell);
                                }
                                else
                                {
                                    chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                    cell = new PdfPCell(new Phrase(chunk));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Border = Rectangle.BOTTOM_BORDER;
                                    cell.BorderColor = BaseColor.BLACK;
                                    table.AddCell(cell);
                                }
                            }
                            if (datelist.Count() <= 31)
                            {
                                var remainingdatecount = 31 - datelist.Count();
                                for (int x = 1; remainingdatecount >= x; x++)
                                {
                                    chunk = new Chunk("-".ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                    cell = new PdfPCell(new Phrase(chunk));
                                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.Border = Rectangle.BOTTOM_BORDER;
                                    cell.BorderColor = BaseColor.BLACK;
                                    table.AddCell(cell);
                                }
                            }

                            var totalMonthlyOutcome = kerjahdrCutiPkj.Sum(s => s.fld_Jumlah);
                            var totalAmt = kerjahdrCutiPkj.Sum(s => s.fld_Jumlah);
                            totalIncomePkj += totalAmt;

                            chunk = new Chunk(totalMonthlyOutcome.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            chunk = new Chunk(totalAmt.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);
                        }
                        //Cuti

                        //Insentif
                        foreach (var item in incentivePkj.Where(x => incomeIncentiveCode.Contains(x.fld_KodInsentif)).ToList())
                        {
                            var incentiveDetail = incentive.Where(x => x.fld_KodInsentif == item.fld_KodInsentif).FirstOrDefault();

                            chunk = new Chunk(incentiveDetail.fld_Keterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.Colspan = 36;
                            table.AddCell(cell);

                            chunk = new Chunk(item.fld_NilaiInsentif.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            totalIncomePkj += item.fld_NilaiInsentif;
                        }
                        //Insentif

                        chunk = new Chunk("Jumlah Pendapatan Kasar", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Colspan = 36;
                        table.AddCell(cell);

                        chunk = new Chunk(totalIncomePkj.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        if (incentivePkj.Where(x => deductionIncentiveCode.Contains(x.fld_KodInsentif)).Count() > 0 || gajiPkj.fld_KWSPPkj > 0 || gajiPkj.fld_SocsoPkj > 0)
                        {
                            chunk = new Chunk("Potongan", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.Colspan = 37;
                            table.AddCell(cell);
                        }

                        foreach (var item in incentivePkj.Where(x => deductionIncentiveCode.Contains(x.fld_KodInsentif)).ToList())
                        {
                            var incentiveDetail = incentive.Where(x => x.fld_KodInsentif == item.fld_KodInsentif).FirstOrDefault();

                            chunk = new Chunk(incentiveDetail.fld_Keterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.Colspan = 36;
                            table.AddCell(cell);

                            chunk = new Chunk(item.fld_NilaiInsentif.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);
                            totalDeductionPkj += item.fld_NilaiInsentif;
                        }

                        if (gajiPkj.fld_KWSPPkj > 0)
                        {
                            chunk = new Chunk("KWSP", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.Colspan = 36;
                            table.AddCell(cell);

                            chunk = new Chunk(gajiPkj.fld_KWSPPkj.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            totalDeductionPkj += gajiPkj.fld_KWSPPkj;
                        }

                        if (gajiPkj.fld_SocsoPkj > 0)
                        {
                            chunk = new Chunk("SOCSO", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.Colspan = 36;
                            table.AddCell(cell);

                            chunk = new Chunk(gajiPkj.fld_SocsoPkj.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);

                            totalDeductionPkj += gajiPkj.fld_SocsoPkj;
                        }

                        foreach (var item in carumanTambahanPkj)
                        {
                            if (item.fld_CarumanPekerja > 0)
                            {
                                var carumanName = jenisCarumanTambahan.Where(x => x.fld_KodCaruman == item.fld_KodCaruman).Select(s => s.fld_NamaCaruman).FirstOrDefault();

                                chunk = new Chunk(carumanName, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                cell.Colspan = 36;
                                table.AddCell(cell);

                                chunk = new Chunk(item.fld_CarumanPekerja.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                                cell = new PdfPCell(new Phrase(chunk));
                                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                cell.Border = Rectangle.BOTTOM_BORDER;
                                cell.BorderColor = BaseColor.BLACK;
                                table.AddCell(cell);

                                totalDeductionPkj += item.fld_CarumanPekerja;
                            }
                        }

                        if (incentivePkj.Where(x => deductionIncentiveCode.Contains(x.fld_KodInsentif)).Count() > 0 || gajiPkj.fld_KWSPPkj > 0 || gajiPkj.fld_SocsoPkj > 0)
                        {
                            chunk = new Chunk("Jumlah Potongan", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            cell.Colspan = 36;
                            table.AddCell(cell);

                            chunk = new Chunk(totalDeductionPkj.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                            cell = new PdfPCell(new Phrase(chunk));
                            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            cell.Border = Rectangle.BOTTOM_BORDER;
                            cell.BorderColor = BaseColor.BLACK;
                            table.AddCell(cell);
                        }

                        totalNetIncomePkj = totalIncomePkj - totalDeductionPkj;

                        chunk = new Chunk("Jumlah Pendapatan Bersih", FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        cell.Colspan = 36;
                        table.AddCell(cell);

                        chunk = new Chunk(totalNetIncomePkj.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                        cell = new PdfPCell(new Phrase(chunk));
                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        cell.Border = Rectangle.BOTTOM_BORDER;
                        cell.BorderColor = BaseColor.BLACK;
                        table.AddCell(cell);

                        pdfDoc.Add(table);
                    }



                    //var getpkjInfo = getpkjInfo2.Where(x => x.fld_Nopkj == pkj);
                    //if (result.Count() > 0)
                    //{
                    //    var NamaPkj = getpkjInfo.Select(s => s.fld_Nama).FirstOrDefault();
                    //    var NoKwsp = getpkjInfo.Select(s => s.fld_Nokwsp).FirstOrDefault();
                    //    var NoSocso = getpkjInfo.Select(s => s.fld_Noperkeso).FirstOrDefault();
                    //    var NoKp = getpkjInfo.Select(s => s.fld_Nokp).FirstOrDefault();

                    //    int? kumpID = getpkjInfo.Select(s => s.fld_KumpulanID).FirstOrDefault();//desc
                    //    string ktgrPkj = getpkjInfo.Select(s => s.fld_Ktgpkj).FirstOrDefault();//desc
                    //    string jntnaPkj = getpkjInfo.Select(s => s.fld_Kdjnt).FirstOrDefault();//desc

                    //    var Kump = tbl_KumpulanKerja.Where(x => x.fld_KumpulanID == kumpID && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID && x.fld_deleted == false).Select(s => s.fld_Keterangan).FirstOrDefault();
                    //    var Kategori = webConfigList.Where(x => x.fldOptConfFlag1 == "designation" && x.fldOptConfValue == ktgrPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();
                    //    var Jantina = webConfigList.Where(x => x.fldOptConfFlag1 == "jantina" && x.fldOptConfValue == jntnaPkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fldDeleted == false).Select(s => s.fldOptConfDesc).FirstOrDefault();


                    //    pdfDoc.NewPage();
                    //    //Header
                    //    pdfDoc = Header(pdfDoc, NamaSyarikat, "(" + NoSyarikat + ")\n" + nswl.fld_LdgCode + " - " + nswl.fld_NamaLadang, "Laporan Slip Gaji Pekerja Bagi Bulan " + MonthList + "/" + YearList + "");
                    //    //Header
                    //    PdfPTable table = new PdfPTable(6);
                    //    table.WidthPercentage = 100;
                    //    table.SpacingBefore = 10f;
                    //    float[] widths = new float[] { 0.5f, 1, 0.5f, 1, 0.5f, 1 };
                    //    table.SetWidths(widths);
                    //    PdfPCell cell = new PdfPCell();




                    //    //var deductiondata = new List<sp_Payslip_Result>();
                    //    //int i = 1;
                    //    //foreach (var item in result.Where(x => x.fldNopkj == pkj && x.fldFlag == 3))
                    //    //{
                    //    //    deductiondata.Add(new sp_Payslip_Result { fldID = i, fldKeterangan = item.fldKeterangan, fldJumlah = item.fldJumlah });
                    //    //    i++;
                    //    //}

                    //    //int f = 1;
                    //    //foreach (var item in result.Where(x => x.fldNopkj == pkj && x.fldFlag <= 2))
                    //    //{
                    //    //    if (item.fldKeterangan != "AIPS")
                    //    //    {
                    //    //        decimal? hardWorkPrice = 0;
                    //    //        decimal? totalAmount = 0;
                    //    //        decimal? quantity = 0;
                    //    //        List<tbl_Kerja> hardWorkData = null;
                    //    //        List<tbl_KerjaKesukaran> hardWorkDataNew = null;
                    //    //        if (item.fldKodPkt != null)
                    //    //        {
                    //    //            string codeAtt = "";
                    //    //            switch (item.fldGandaan)
                    //    //            {
                    //    //                case 1:
                    //    //                    codeAtt = "H01";
                    //    //                    break;
                    //    //                case 2:
                    //    //                    codeAtt = "H02";
                    //    //                    break;
                    //    //                case 3:
                    //    //                    codeAtt = "H03";
                    //    //                    break;
                    //    //            }
                    //    //            var attWorkDate = attWorkDatas.Where(x => x.fld_Kdhdct == codeAtt && x.fld_Nopkj == item.fldNopkj).Select(s => s.fld_Tarikh).ToArray();
                    //    //            hardWorkData = hardWorkDatas.Where(x => x.fld_KodPkt == item.fldKodPkt && x.fld_KodAktvt == item.fldKod && attWorkDate.Contains(x.fld_Tarikh) && x.fld_Nopkj == item.fldNopkj).ToList();
                    //    //            var hardWorkDataIDs2 = hardWorkData.Select(s => s.fld_ID).ToList();
                    //    //            var hardWorkDataNewFilter = hardWorkDatasNew.Where(x => hardWorkDataIDs2.Contains(x.fld_KerjaID.Value)).ToList();
                    //    //            hardWorkPrice = hardWorkData.Where(x => x.fld_KodPkt == item.fldKodPkt && x.fld_Nopkj == item.fldNopkj).Sum(s => s.fld_HrgaKwsnSkar);
                    //    //            var hardWorkPriceIDs = hardWorkData.Where(x => x.fld_KodPkt == item.fldKodPkt).Select(s => s.fld_ID).ToList();
                    //    //            hardWorkDataNew = hardWorkDataNewFilter.Where(x => hardWorkPriceIDs.Contains(x.fld_KerjaID.Value)).ToList();
                    //    //            quantity = hardWorkDatas.Where(x => x.fld_KodPkt == item.fldKodPkt).Sum(s => s.fld_JumlahHasil);
                    //    //            totalAmount = item.fldJumlah - hardWorkPrice;
                    //    //        }
                    //    //        else
                    //    //        {
                    //    //            totalAmount = item.fldJumlah;
                    //    //        }

                    //    //        if (item.fldKodPkt != null)
                    //    //        {
                    //    //            chunk = new Chunk(item.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //        }
                    //    //        else
                    //    //        {
                    //    //            chunk = new Chunk(item.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //        }






                    //    //        chunk = new Chunk(GetTriager.GetDashForNull(item.fldUnit), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //        cell = new PdfPCell(new Phrase(chunk));
                    //    //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //        cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //        cell.BorderColor = BaseColor.BLACK;
                    //    //        table.AddCell(cell);

                    //    //        chunk = new Chunk(GetTriager.GetDashForNull(item.fldKadar.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //        cell = new PdfPCell(new Phrase(chunk));
                    //    //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //        cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //        cell.BorderColor = BaseColor.BLACK;
                    //    //        table.AddCell(cell);

                    //    //        chunk = new Chunk(GetTriager.GetDashForNull(item.fldGandaan.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //        cell = new PdfPCell(new Phrase(chunk));
                    //    //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //        cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //        cell.BorderColor = BaseColor.BLACK;
                    //    //        table.AddCell(cell);

                    //    //        chunk = new Chunk(GetTriager.GetTotalForMoney(totalAmount), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //        cell = new PdfPCell(new Phrase(chunk));
                    //    //        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //        cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //        cell.BorderColor = BaseColor.BLACK;
                    //    //        table.AddCell(cell);

                    //    //        var getdeduction = deductiondata.Where(x => x.fldID == f).FirstOrDefault();
                    //    //        if (getdeduction != null)
                    //    //        {
                    //    //            //farahin - comment - 15/09/2021
                    //    //            //chunk = new Chunk(item.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //            //farahin modified - 15/09/2021
                    //    //            chunk = new Chunk(getdeduction.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //            cell = new PdfPCell(new Phrase(chunk));
                    //    //            cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //            cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //            cell.BorderColor = BaseColor.BLACK;
                    //    //            table.AddCell(cell);

                    //    //            chunk = new Chunk(GetTriager.GetTotalForMoney(getdeduction.fldJumlah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //            cell = new PdfPCell(new Phrase(chunk));
                    //    //            cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //            cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //            cell.BorderColor = BaseColor.BLACK;
                    //    //            table.AddCell(cell);
                    //    //        }
                    //    //        else
                    //    //        {
                    //    //            cell = new PdfPCell();
                    //    //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //    //            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //            cell.Border = 0;
                    //    //            table.AddCell(cell);

                    //    //            cell = new PdfPCell();
                    //    //            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //    //            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //            cell.Border = 0;
                    //    //            table.AddCell(cell);
                    //    //        }

                    //    //        f++;

                    //    //        if (hardWorkPrice > 0)
                    //    //        {
                    //    //            var hardWorkCode = hardWorkData.Select(s => s.fld_KodKwsnSkar).FirstOrDefault();
                    //    //            if (hardWorkCode == "**")
                    //    //            {
                    //    //                foreach (var item2 in hardWorkDataNew.GroupBy(g => new { g.fld_KodKesukaran, g.fld_Kadar }).Select(s => new { kod = s.Key.fld_KodKesukaran, kadar = s.Key.fld_Kadar, amount = s.Sum(am => am.fld_Jumlah) }).ToList())
                    //    //                {
                    //    //                    var hardWorkDesc = pktHargaKesukaran.Where(x => x.fld_KodHargaKesukaran == item2.kod).Select(s => s.fld_KeteranganHargaKesukaran).FirstOrDefault();
                    //    //                    var hardWorkRate = pktHargaKesukaran.Where(x => x.fld_KodHargaKesukaran == item2.kod).Select(s => s.fld_HargaKesukaran).FirstOrDefault();
                    //    //                    var desc = item.fldKeterangan + " (" + hardWorkDesc + ")";

                    //    //                    chunk = new Chunk(desc, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);

                    //    //                    chunk = new Chunk(GetTriager.GetDashForNull("-"), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);

                    //    //                    chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);

                    //    //                    chunk = new Chunk(GetTriager.GetDashForNull(item2.kadar.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);

                    //    //                    chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);

                    //    //                    chunk = new Chunk(GetTriager.GetTotalForMoney(item2.amount), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);

                    //    //                    getdeduction = deductiondata.Where(x => x.fldID == f).FirstOrDefault();
                    //    //                    if (getdeduction != null)
                    //    //                    {
                    //    //                        //farahin - comment - 15/09/2021
                    //    //                        //chunk = new Chunk(item.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                        //farahin modified - 15/09/2021
                    //    //                        chunk = new Chunk(getdeduction.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                        cell = new PdfPCell(new Phrase(chunk));
                    //    //                        cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                        cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                        cell.BorderColor = BaseColor.BLACK;
                    //    //                        table.AddCell(cell);

                    //    //                        chunk = new Chunk(GetTriager.GetTotalForMoney(getdeduction.fldJumlah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                        cell = new PdfPCell(new Phrase(chunk));
                    //    //                        cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                        cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                        cell.BorderColor = BaseColor.BLACK;
                    //    //                        table.AddCell(cell);
                    //    //                    }
                    //    //                    else
                    //    //                    {
                    //    //                        cell = new PdfPCell();
                    //    //                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //    //                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                        cell.Border = 0;
                    //    //                        table.AddCell(cell);

                    //    //                        cell = new PdfPCell();
                    //    //                        cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //    //                        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                        cell.Border = 0;
                    //    //                        table.AddCell(cell);
                    //    //                    }

                    //    //                    f++;
                    //    //                }
                    //    //            }
                    //    //            else
                    //    //            {
                    //    //                var hardWorkDesc = pktHargaKesukaran.Where(x => x.fld_KodHargaKesukaran == hardWorkCode).Select(s => s.fld_KeteranganHargaKesukaran).FirstOrDefault();
                    //    //                var hardWorkRate = pktHargaKesukaran.Where(x => x.fld_KodHargaKesukaran == hardWorkCode).Select(s => s.fld_HargaKesukaran).FirstOrDefault();
                    //    //                var desc = item.fldKeterangan + " (" + hardWorkDesc + ")";

                    //    //                chunk = new Chunk(desc, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                cell = new PdfPCell(new Phrase(chunk));
                    //    //                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                cell.BorderColor = BaseColor.BLACK;
                    //    //                table.AddCell(cell);

                    //    //                chunk = new Chunk(GetTriager.GetDashForNull("-"), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                cell = new PdfPCell(new Phrase(chunk));
                    //    //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                cell.BorderColor = BaseColor.BLACK;
                    //    //                table.AddCell(cell);

                    //    //                chunk = new Chunk("-", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                cell = new PdfPCell(new Phrase(chunk));
                    //    //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                cell.BorderColor = BaseColor.BLACK;
                    //    //                table.AddCell(cell);

                    //    //                chunk = new Chunk(GetTriager.GetDashForNull(hardWorkRate.ToString()), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                cell = new PdfPCell(new Phrase(chunk));
                    //    //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                cell.BorderColor = BaseColor.BLACK;
                    //    //                table.AddCell(cell);

                    //    //                chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                cell = new PdfPCell(new Phrase(chunk));
                    //    //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                cell.BorderColor = BaseColor.BLACK;
                    //    //                table.AddCell(cell);

                    //    //                chunk = new Chunk(GetTriager.GetTotalForMoney(hardWorkPrice), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                cell = new PdfPCell(new Phrase(chunk));
                    //    //                cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                cell.BorderColor = BaseColor.BLACK;
                    //    //                table.AddCell(cell);

                    //    //                getdeduction = deductiondata.Where(x => x.fldID == f).FirstOrDefault();
                    //    //                if (getdeduction != null)
                    //    //                {
                    //    //                    //farahin - comment - 15/09/2021
                    //    //                    //chunk = new Chunk(item.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    //farahin modified - 15/09/2021
                    //    //                    chunk = new Chunk(getdeduction.fldKeterangan, FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);

                    //    //                    chunk = new Chunk(GetTriager.GetTotalForMoney(getdeduction.fldJumlah), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //                    cell = new PdfPCell(new Phrase(chunk));
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //                    cell.BorderColor = BaseColor.BLACK;
                    //    //                    table.AddCell(cell);
                    //    //                }
                    //    //                else
                    //    //                {
                    //    //                    cell = new PdfPCell();
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = 0;
                    //    //                    table.AddCell(cell);

                    //    //                    cell = new PdfPCell();
                    //    //                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //    //                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //                    cell.Border = 0;
                    //    //                    table.AddCell(cell);
                    //    //                }

                    //    //                f++;
                    //    //            }
                    //    //        }
                    //    //    }
                    //    //}

                    //    //chunk = new Chunk("Jumlah Pendapatan", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Colspan = 5;
                    //    //cell.Border = Rectangle.TOP_BORDER;
                    //    //cell.BorderColor = BaseColor.RED;
                    //    //table.AddCell(cell);

                    //    //decimal? TotalPendapatan = result.Where(x => x.fldFlag == 2).Select(s => s.fldJumlah).Sum();

                    //    //chunk = new Chunk(GetTriager.GetTotalForMoney(TotalPendapatan), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Colspan = 1;
                    //    //cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.RED;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Potongan", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Colspan = 1;
                    //    //cell.Border = Rectangle.TOP_BORDER;
                    //    //cell.BorderColor = BaseColor.RED;
                    //    //table.AddCell(cell);

                    //    //decimal? TotalPotongan = deductiondata.Select(s => s.fldJumlah).Sum();

                    //    //chunk = new Chunk(GetTriager.GetTotalForMoney(TotalPotongan), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Colspan = 1;
                    //    //cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.RED;
                    //    //table.AddCell(cell);

                    //    //decimal GajiBersih = TotalPendapatan.Value - TotalPotongan.Value;

                    //    //chunk = new Chunk("Gaji Bersih", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Colspan = 7;
                    //    //cell.Border = 0;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(GetTriager.GetTotalForMoney(GajiBersih), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Colspan = 1;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.RED;
                    //    //table.AddCell(cell);

                    //    //pdfDoc.Add(table);

                    //    //Paragraph line = new Paragraph(new Chunk(new iTextSharp.text.pdf.draw.LineSeparator(0.0F, 100.0F, BaseColor.BLACK, Element.ALIGN_LEFT, 1)));
                    //    //pdfDoc.Add(line);

                    //    //PdfPTable maintable = new PdfPTable(1);
                    //    //maintable.WidthPercentage = 100;

                    //    //chunk = new Chunk("*Gandaan : 1 = Hari Bekerja, 2 = Hujung Minggu, 3 = Cuti Umum\n*Gandaan Bonus Harga : 0.5 = 50% Capaian, 1 = 100% Capaian", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //PdfPCell cell1 = new PdfPCell(new Phrase(chunk));
                    //    //cell1.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell1.VerticalAlignment = Element.ALIGN_TOP;
                    //    //cell1.Border = 0;
                    //    //maintable.AddCell(cell1);

                    //    //table = new PdfPTable(6);
                    //    //table.WidthPercentage = 100;
                    //    //table.HorizontalAlignment = 0;
                    //    //table.SpacingBefore = 5f;
                    //    //widths = new float[] { 1, 1, 1, 1, 1, 1 };
                    //    //table.SetWidths(widths);

                    //    //chunk = new Chunk("Perincian", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Colspan = 6;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
                    //    //cell.BorderColor = BaseColor.RED;
                    //    //table.AddCell(cell);

                    //    ////get Hadir and cuti Count
                    //    //var hdr = dbr.tbl_Kerjahdr.Where(x => x.fld_Nopkj == pkj && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                    //    //var hdrhrbs = hdr.Where(x => x.fld_Kdhdct == "H01").Count();

                    //    //var hdrhrmg = hdr.Where(x => x.fld_Kdhdct == "H02").Count();

                    //    //var hdrhrcu = hdr.Where(x => x.fld_Kdhdct == "H03").Count();

                    //    //var hdrhrpg = hdr.Where(x => x.fld_Kdhdct == "P01").Count();

                    //    //var hdrhrct = hdr.Where(x => x.fld_Kdhdct == "C02").Count();

                    //    //var hdrhrtg = hdr.Where(x => x.fld_Kdhdct == "C05").Count();

                    //    //var hdrhrcs = hdr.Where(x => x.fld_Kdhdct == "C03").Count();

                    //    //var hdrhrca = hdr.Where(x => x.fld_Kdhdct == "C01").Count();

                    //    //var hdrhrcm = hdr.Where(x => x.fld_Kdhdct == "C07").Count();

                    //    //var hdrhrcb = hdr.Where(x => x.fld_Kdhdct == "C04").Count();

                    //    //var hdrhrch = hdr.Where(x => x.fld_Kdhdct == "C10").Count();

                    //    ////get hdr OT
                    //    //var hdrot = dbr.vw_KerjaHdrOT.Where(x => x.fld_Nopkj == pkj && x.fld_Tarikh.Value.Month == MonthList && x.fld_Tarikh.Value.Year == YearList && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).ToList();
                    //    //var hdrothrbs = hdrot.Where(x => x.fld_Kdhdct == "H01").Sum(s => s.fld_JamOT);
                    //    //hdrothrbs = hdrothrbs == null ? 0m : hdrothrbs;

                    //    //var hdrothrcm = hdrot.Where(x => x.fld_Kdhdct == "H02").Sum(s => s.fld_JamOT);
                    //    //hdrothrcm = hdrothrcm == null ? 0m : hdrothrcm;

                    //    //var hdrothrcu = hdrot.Where(x => x.fld_Kdhdct == "H03").Sum(s => s.fld_JamOT);
                    //    //hdrothrcu = hdrothrcu == null ? 0m : hdrothrcu;

                    //    //var hdrhrhujan = hdr.Where(x => x.fld_Hujan == 1).Count();

                    //    ////get Jumlah Hari Kerja
                    //    //int? hrkrja = 0;
                    //    ////get jmlh hari hadir
                    //    //var cdct = new string[] { "H01", "H02", "H03" };
                    //    //var jmlhhdr = hdr.Where(x => cdct.Contains(x.fld_Kdhdct)).Count();


                    //    ////get avg slry
                    //    //DateTime cdate = new DateTime(YearList.Value, MonthList.Value, 15);
                    //    //DateTime ldate = cdate.AddMonths(-1);
                    //    //var cravgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == cdate.Month && x.fld_Year == cdate.Year && x.fld_Nopkj == pkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => new { s.fld_PurataGaji, s.fld_PurataGaji12Bln }).FirstOrDefault();
                    //    //var crmnthavgslry = cravgslry.fld_PurataGaji == null ? 0m : cravgslry.fld_PurataGaji;

                    //    //var lsmnthavgslry = dbr.tbl_GajiBulanan.Where(x => x.fld_Month == ldate.Month && x.fld_Year == ldate.Year && x.fld_Nopkj == pkj && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID && x.fld_WilayahID == WilayahID && x.fld_LadangID == LadangID).Select(s => s.fld_PurataGaji).FirstOrDefault();
                    //    //lsmnthavgslry = lsmnthavgslry == null ? 0m : lsmnthavgslry;

                    //    //var yearavgslry = cravgslry.fld_PurataGaji12Bln == null || cravgslry.fld_PurataGaji12Bln > 200 ? 0m : cravgslry.fld_PurataGaji12Bln;

                    //    //chunk = new Chunk("Jumlah Tawaran Hari Bekerja", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hrkrja.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Cuti Tahunan", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrct.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah OT - Hari Biasa (Jam)", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrothrbs.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Hadir Hari Biasa", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrbs.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Cuti Sakit", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrcs.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah OT - Hari Cuti Minggu (Jam)", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrothrcm.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Hadir Hari Minggu", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrmg.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Cuti Hospitalisasi", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrch.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah OT - Hari Cuti Umum (Jam)", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrothrcu.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Hadir Hari Cuti Umum", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrcu.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Cuti Umum", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrca.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Purata Gaji Bulan Ini", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(crmnthavgslry.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Hari Hadir", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(jmlhhdr.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Cuti Hari Minggu", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrcm.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Purata Gaji Bulan Lepas", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(lsmnthavgslry.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Tidak Hadir", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrpg.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Cuti Bersalin", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrcb.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Purata Gaji Setahun", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(yearavgslry.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = Rectangle.BOTTOM_BORDER;
                    //    //cell.BorderColor = BaseColor.BLACK;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Hari Terabai", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = 0;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrhujan.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = 0;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("Jumlah Cuti Tanpa Gaji", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = 0;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk(hdrhrtg.ToString(), FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = 0;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = 0;
                    //    //table.AddCell(cell);

                    //    //chunk = new Chunk("", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
                    //    //cell = new PdfPCell(new Phrase(chunk));
                    //    //cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    //    //cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    //    //cell.Border = 0;
                    //    //table.AddCell(cell);

                    //    //cell1 = new PdfPCell(table);
                    //    //cell1.HorizontalAlignment = Element.ALIGN_RIGHT;
                    //    //cell1.VerticalAlignment = Element.ALIGN_TOP;
                    //    //cell1.Border = 0;
                    //    //maintable.AddCell(cell1);

                    //    //pdfDoc.Add(maintable);
                    //}
                }
            }
            else
            {
                PdfPTable table = new PdfPTable(1);
                table.WidthPercentage = 100;
                PdfPCell cell = new PdfPCell();
                chunk = new Chunk("No Data Found", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
                cell = new PdfPCell(new Phrase(chunk));
                cell.HorizontalAlignment = Element.ALIGN_LEFT;
                cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                cell.Border = 0;
                table.AddCell(cell);
                pdfDoc.Add(table);
            }
            //pdfDoc = Footer(pdfDoc, chunk, para);
            pdfWriter.CloseStream = false;
            pdfDoc.Close();
            byte[] file = ms.ToArray();
            output.Write(file, 0, file.Length);
            output.Position = 0;
            return new FileStreamResult(output, "application/pdf");
        }

        public Document Header(Document pdfDoc, string headername, string headername2, string headername3)
        {
            Paragraph date = new Paragraph(new Chunk("Tarikh : " + timezone.gettimezone().ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
            date.Alignment = Element.ALIGN_RIGHT;
            pdfDoc.Add(date);
            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            Image image = Image.GetInstance(Server.MapPath("~/Asset/Images/logo_FTPSB.jpg"));
            PdfPCell cell = new PdfPCell(image);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            image.ScaleAbsolute(50, 50);
            table.AddCell(cell);

            Chunk chunk = new Chunk(headername, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);
            chunk = new Chunk(headername2, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);
            chunk = new Chunk(headername3, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
            cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            table.AddCell(cell);
            pdfDoc.Add(table);
            return pdfDoc;
        }

        public Document Footer(Document pdfDoc, Chunk chunk, Paragraph para)
        {

            return pdfDoc;
        }
    }
}