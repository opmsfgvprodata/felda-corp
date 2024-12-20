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
        public FileStreamResult PocketCheckrollPdf(int? WilayahList, int? LadangList, int? RadioGroup, int? MonthList, int? YearList, string SelectionList, string StatusList, string WorkCategoryList)
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
                    var gajiPkj = gajiBulanan.Where(x => x.fld_Nopkj == pkj).FirstOrDefault();
                    if (gajiPkj != null)
                    {
                        var checkrollPkj = pkjmast.Where(x => x.fld_Nopkj == pkj).FirstOrDefault();
                        var kerjaHdrPkj = kerjahdr.Where(x => x.fld_Nopkj == pkj).ToList();
                        var kerjaPkj = kerja.Where(x => x.fld_Nopkj == pkj).ToList();
                        var kerjaOTPkj = kerjaOT.Where(x => x.fld_Nopkj == pkj).ToList();
                        var kerjahdrCutiPkj = kerjahdrCuti.Where(x => x.fld_Nopkj == pkj).ToList();
                        var incentivePkj = insentif.Where(x => x.fld_Nopkj == pkj).ToList();
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

                                    chunk = new Chunk("RM", FontFactory.GetFont("Arial", 7, Font.NORMAL, BaseColor.BLACK));
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

                    }
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