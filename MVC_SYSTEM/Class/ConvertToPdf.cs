using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

using System.Data;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using HTMLtoPDF;
using MVC_SYSTEM.log;
using MVC_SYSTEM.Models;
using MVC_SYSTEM.AuthModels;

namespace MVC_SYSTEM.Class
{
    public class ConvertToPdf
    {
        private MVC_SYSTEM_Models db = new MVC_SYSTEM_Models();
        private MVC_SYSTEM_Auth db2 = new MVC_SYSTEM_Auth();
        private GetIdentity getidentity = new GetIdentity();
        errorlog geterror = new errorlog();

        public string DownloadAsPDF(string htmlstring, string filename, string username, string reportname, string domain)
        {
            string result = "";
            try
            {
                string strHtml = htmlstring;
                string pdfFileName = HttpContext.Current.Server.MapPath("~/files/" + filename + ".pdf");
                int? getuserid = getidentity.ID(username);

                CreatePDFFromHTMLFile(strHtml, pdfFileName, reportname);

                tblReportExport tblReportExport = new tblReportExport();
                tblReportExport.fldFileName = filename + ".pdf";
                tblReportExport.fldPath = domain + "/files/";
                tblReportExport.fldReportName = reportname;
                tblReportExport.fldUserID = getuserid;

                db.tblReportExports.Add(tblReportExport);
                db.SaveChanges();

                result = domain + "/files/" + filename + ".pdf";
                
            }
            catch (Exception ex)
            {
                geterror.catcherro(ex.Message, ex.StackTrace, ex.Source, ex.TargetSite.ToString());
            }

            return result;
        }
        public void CreatePDFFromHTMLFile(string HtmlStream, string FileName, string reportname)
        {
            try
            {
                var getsize = db.tblReportLists.Where(x => x.fldReportListAction == "ActiveWorker").FirstOrDefault();
                object TargetFile = FileName;
                string ModifiedFileName = string.Empty;
                string FinalFileName = string.Empty;
                string width, height = "";

                width = getsize.fldWidthReport.ToString();
                height = getsize.fldHeightReport.ToString();

                GeneratePDF.HtmlToPdfBuilder builder = new GeneratePDF.HtmlToPdfBuilder(new Rectangle(int.Parse(width), int.Parse(height)));
                GeneratePDF.HtmlPdfPage first = builder.AddPage();
                first.AppendHtml(HtmlStream);
                byte[] file = builder.RenderPdf();
                File.WriteAllBytes(TargetFile.ToString(), file);

                PdfReader reader = new PdfReader(TargetFile.ToString());
                ModifiedFileName = TargetFile.ToString();
                ModifiedFileName = ModifiedFileName.Insert(ModifiedFileName.Length - 4, "1");

                PdfEncryptor.Encrypt(reader, new FileStream(ModifiedFileName, FileMode.Append), PdfWriter.STRENGTH128BITS, "", "", PdfWriter.AllowPrinting);
                reader.Close();
                if (File.Exists(TargetFile.ToString()))
                    File.Delete(TargetFile.ToString());
                FinalFileName = ModifiedFileName.Remove(ModifiedFileName.Length - 5, 1);
                File.Copy(ModifiedFileName, FinalFileName);
                if (File.Exists(ModifiedFileName))
                    File.Delete(ModifiedFileName);

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}