using iTextSharp.text;
using iTextSharp.text.pdf;
using MVC_SYSTEM.Class;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using MVC_SYSTEM.ModelsCorporate;

namespace MVC_SYSTEM.Class
{
    public class GeneralClass
    {
        private MVC_SYSTEM_ModelsCorporate db = new MVC_SYSTEM_ModelsCorporate();
        private GetIdentity GetIdentity = new GetIdentity();


        //public class FooterEwallet : iTextSharp.text.pdf.PdfPageEventHelper
        //{
        //    public string fld_LdgName { get; set; }
        //    public string fld_LdgCode { get; set; }
        //    public string tarikh { get; set; }
        //    public int Year { get; set; }
        //    public int Month { get; set; }
        //    public string headername { get; set; }
        //    public string headername2 { get; set; }
        //    public int totalpages { get; set; }

        //    public string Header
        //    {
        //        get { return _header; }
        //        set { _header = value; }
        //    }
        //    private string _header;
        //    PdfContentByte cb;
        //    PdfTemplate headerTemplate, footerTemplate;
        //    BaseFont bf = null;
        //    PdfTemplate template;

        //    // This keeps track of the creation time
        //    DateTime PrintTime = DateTime.Now;

        //    public override void OnOpenDocument(PdfWriter writer, Document doc)
        //    {
        //        try
        //        {
        //            // PrintTime = DateTime.Now;
        //            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //            cb = writer.DirectContent;
        //            headerTemplate = cb.CreateTemplate(100, 100);
        //            footerTemplate = cb.CreateTemplate(50, 50);
        //        }
        //        catch (DocumentException de)
        //        {
        //            //handle exception here
        //        }
        //        catch (System.IO.IOException ioe)
        //        {
        //            //handle exception here
        //        }
        //    }

        //    public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        //    {


        //        base.OnEndPage(writer, doc);
        //        Paragraph footer = new Paragraph("THANK YOU", FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.NORMAL));
        //        footer.Alignment = Element.ALIGN_RIGHT;
        //        PdfPTable footerTbl = new PdfPTable(1);
        //        footerTbl.TotalWidth = 800;
        //        footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
        //        Chunk chunk = new Chunk();
        //        Paragraph para = new Paragraph();
        //        int pageN = writer.PageNumber;
        //        String text = "Page " + writer.PageNumber + " of ";

        //        PdfPTable table = new PdfPTable(1);
        //        table.TotalWidth = 850;

        //        Image image = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Asset/Images/logo.jpg"));
        //        PdfPCell cell = new PdfPCell(image);
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        image.ScaleAbsolute(50, 50);
        //        table.AddCell(cell);


        //        chunk = new Chunk(headername, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
        //        cell = new PdfPCell(new Phrase(chunk));
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        table.AddCell(cell);
        //        chunk = new Chunk(headername2, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
        //        cell = new PdfPCell(new Phrase(chunk));
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        table.AddCell(cell);

        //        PdfPTable tableH = new PdfPTable(4);
        //        tableH.TotalWidth = 850;
        //        tableH.WidthPercentage = 100;
        //        tableH.SpacingBefore = 4f;
        //        float[] widthsH = new float[] { 2.5f, 1, 0.5f, 0.5f };
        //        tableH.SetWidths(widthsH);
        //        PdfPCell cellH = new PdfPCell();

        //        chunk = new Chunk("             " + fld_LdgCode + " - " + fld_LdgName, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);


        //        chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Reference ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": 21", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);


        //        chunk = new Chunk("             " + "EWALLET", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Date ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": " + tarikh, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Year/Period ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": " + Year + " / " + Month, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(" ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Page ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": " + text + totalpages.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        //PdfPTable tableH2 = new PdfPTable(4);
        //        //tableH2.TotalWidth = 850;
        //        //tableH2.WidthPercentage = 100;
        //        //tableH2.SpacingBefore = 4f;
        //        //float[] widthsH2 = new float[] { 1, 1, 1, 1 };
        //        //tableH2.SetWidths(widthsH2);
        //        //PdfPCell cellH2 = new PdfPCell();

        //        //footerTbl.WriteSelectedRows(0, -1, 415, 30, writer.DirectContent);
        //        table.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 20), writer.DirectContent);
        //        tableH.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 100), writer.DirectContent);
        //        //tableH2.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 200), writer.DirectContent);

        //    }
        //}

        //public class FooterPostingToSAP : iTextSharp.text.pdf.PdfPageEventHelper
        //{
        //    public string fld_LdgName { get; set; }
        //    public string fld_LdgCode { get; set; }
        //    public string tarikh { get; set; }
        //    public int Year { get; set; }
        //    public int Month { get; set; }
        //    public string headername { get; set; }
        //    public string headername2 { get; set; }
        //    public int totalpages { get; set; }

        //    public string Header
        //    {
        //        get { return _header; }
        //        set { _header = value; }
        //    }
        //    private string _header;
        //    PdfContentByte cb;
        //    PdfTemplate headerTemplate, footerTemplate;
        //    BaseFont bf = null;
        //    PdfTemplate template;

        //    // This keeps track of the creation time
        //    DateTime PrintTime = DateTime.Now;

        //    public override void OnOpenDocument(PdfWriter writer, Document doc)
        //    {
        //        try
        //        {
        //            // PrintTime = DateTime.Now;
        //            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        //            cb = writer.DirectContent;
        //            headerTemplate = cb.CreateTemplate(100, 100);
        //            footerTemplate = cb.CreateTemplate(50, 50);
        //        }
        //        catch (DocumentException de)
        //        {
        //            //handle exception here
        //        }
        //        catch (System.IO.IOException ioe)
        //        {
        //            //handle exception here
        //        }
        //    }

        //    public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        //    {


        //        base.OnEndPage(writer, doc);
        //        Paragraph footer = new Paragraph("THANK YOU", FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.NORMAL));
        //        footer.Alignment = Element.ALIGN_RIGHT;
        //        PdfPTable footerTbl = new PdfPTable(1);
        //        footerTbl.TotalWidth = 800;
        //        footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
        //        Chunk chunk = new Chunk();
        //        Paragraph para = new Paragraph();
        //        int pageN = writer.PageNumber;
        //        String text = "Page " + writer.PageNumber + " of ";


        //        PdfPTable table = new PdfPTable(1);
        //        table.TotalWidth = 850;

        //        Image image = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Asset/Images/logo.jpg"));
        //        PdfPCell cell = new PdfPCell(image);
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        image.ScaleAbsolute(50, 50);
        //        table.AddCell(cell);


        //        chunk = new Chunk(headername, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
        //        cell = new PdfPCell(new Phrase(chunk));
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        table.AddCell(cell);

        //        chunk = new Chunk(headername2, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
        //        cell = new PdfPCell(new Phrase(chunk));
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        table.AddCell(cell);

        //        PdfPTable tableH = new PdfPTable(4);
        //        tableH.TotalWidth = 850;
        //        tableH.WidthPercentage = 100;
        //        tableH.SpacingBefore = 4f;
        //        float[] widthsH = new float[] { 1.5f, 1, 0.3f, 0.5f };
        //        tableH.SetWidths(widthsH);
        //        PdfPCell cellH = new PdfPCell();

        //        chunk = new Chunk("", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(fld_LdgCode + " - " + fld_LdgName, FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Page ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);            

        //        chunk = new Chunk(": " + text + totalpages.ToString(), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("SAP Posting Document", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Date ", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": " + DateTime.Now.ToString("dd.MM.yyyy"), FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);


        //        //PdfPTable tableH2 = new PdfPTable(4);
        //        //tableH2.TotalWidth = 850;
        //        //tableH2.WidthPercentage = 100;
        //        //tableH2.SpacingBefore = 4f;
        //        //float[] widthsH2 = new float[] { 1, 1, 1, 1 };
        //        //tableH2.SetWidths(widthsH2);
        //        //PdfPCell cellH2 = new PdfPCell();

        //        table.WriteSelectedRows(0, -1, -122 ,(doc.PageSize.Width + 210 ), writer.DirectContent);
        //        tableH.WriteSelectedRows(0, -1, -300, (doc.PageSize.Width + 130 ), writer.DirectContent);
        //        //table.WriteSelectedRows(0, -1, 100, (doc.PageSize.Height - 20), writer.DirectContent);
        //        //tableH.WriteSelectedRows(0, -1, 50, (doc.PageSize.Height - 20), writer.DirectContent);
        //        //tableH2.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 200), writer.DirectContent);

        //    }
        //}

        //public class FooterMaybank : iTextSharp.text.pdf.PdfPageEventHelper
        //{
        //    public string fld_LdgName { get; set; }
        //    public string fld_LdgCode { get; set; }
        //    public string tarikh { get; set; }
        //    public int Year { get; set; }
        //    public int Month { get; set; }
        //    public string coeb_bank_account_no { get; set; }
        //    public string coeb_origin_name { get; set; }
        //    public string coeb_origin_id { get; set; }
        //    public string headername { get; set; }
        //    public string headername2 { get; set; }
        //    public int totalpages { get; set; }
        //    public string Header
        //    {
        //        get { return _header; }
        //        set { _header = value; }
        //    }
        //    private string _header;
        //    PdfContentByte cb;
        //    PdfTemplate headerTemplate, footerTemplate;
        //    BaseFont bf = null;
        //    PdfTemplate template;

        //    // This keeps track of the creation time
        //    DateTime PrintTime = DateTime.Now;

        //    public override void OnEndPage(iTextSharp.text.pdf.PdfWriter writer, iTextSharp.text.Document doc)
        //    {


        //        base.OnEndPage(writer, doc);
        //        Paragraph footer = new Paragraph("THANK YOU", FontFactory.GetFont(FontFactory.TIMES, 10, iTextSharp.text.Font.NORMAL));
        //        footer.Alignment = Element.ALIGN_RIGHT;
        //        PdfPTable footerTbl = new PdfPTable(1);
        //        footerTbl.TotalWidth = 300;
        //        footerTbl.HorizontalAlignment = Element.ALIGN_CENTER;
        //        Chunk chunk = new Chunk();
        //        Paragraph para = new Paragraph();
        //        int pageN = writer.PageNumber;
        //        String text = "Page " + writer.PageNumber + " of ";

        //        PdfPTable table = new PdfPTable(1);
        //        table.TotalWidth = 600;
        //        table.WidthPercentage = 100;

        //        Image image = Image.GetInstance(HttpContext.Current.Server.MapPath("~/Asset/Images/logo.jpg"));
        //        PdfPCell cell = new PdfPCell(image);
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        image.ScaleAbsolute(50, 50);
        //        table.AddCell(cell);


        //        chunk = new Chunk(headername, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
        //        cell = new PdfPCell(new Phrase(chunk));
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        table.AddCell(cell);
        //        chunk = new Chunk(headername2, FontFactory.GetFont("Arial", 9, Font.BOLD, BaseColor.BLACK));
        //        cell = new PdfPCell(new Phrase(chunk));
        //        cell.HorizontalAlignment = Element.ALIGN_CENTER;
        //        cell.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cell.Border = 0;
        //        table.AddCell(cell);

        //        PdfPTable tableH = new PdfPTable(3);
        //        tableH.TotalWidth = 600;
        //        tableH.WidthPercentage = 100;
        //        tableH.SpacingBefore = 4f;
        //        float[] widthsH = new float[] { 1, 0.5f, 0.5f };
        //        tableH.SetWidths(widthsH);
        //        PdfPCell cellH = new PdfPCell();

        //        //PdfPTable outerTable = new PdfPTable(columnWidths);
        //        //table.setTotalWidth(770F);

        //        //cb.MoveTo(40, doc.PageSize.Height - 100);
        //        //cb.LineTo(doc.PageSize.Width - 40, doc.PageSize.Height - 100);
        //        //cb.Stroke();

        //        chunk = new Chunk("             " + fld_LdgCode + " - " + fld_LdgName, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Reference ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": 21", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);


        //        chunk = new Chunk("             " + "ONLINE PAYMENT SUMMARY", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Date ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": " + tarikh, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Year/Period ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": " + Year + "/" + Month, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk("Page ", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        chunk = new Chunk(": " + text + " " + totalpages.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH = new PdfPCell(new Phrase(chunk));
        //        cellH.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH.BorderColor = BaseColor.BLACK;
        //        tableH.AddCell(cellH);

        //        PdfPTable tableH2 = new PdfPTable(4);
        //        tableH2.TotalWidth = 600;
        //        tableH2.WidthPercentage = 100;
        //        tableH2.SpacingBefore = 4f;
        //        float[] widthsH2 = new float[] { 1, 1, 1, 1 };
        //        tableH2.SetWidths(widthsH2);
        //        PdfPCell cellH2 = new PdfPCell();

        //        chunk = new Chunk("             " + "Co ID", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk(": " + fld_LdgCode.Substring(1, 3), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk("Originator ID", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk(": " + coeb_origin_id.ToString(), FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk("             " + "Originator Name", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk(": " + coeb_origin_name, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk("Originator Account", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk(": " + coeb_bank_account_no, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk("             " + "Payment Reference", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk(": " + "WORKERS SALARY", FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk("Payment Desc", FontFactory.GetFont("Arial", 8, Font.BOLD, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        chunk = new Chunk(": " + Month + "/" + Year, FontFactory.GetFont("Arial", 8, Font.NORMAL, BaseColor.BLACK));
        //        cellH2 = new PdfPCell(new Phrase(chunk));
        //        cellH2.HorizontalAlignment = Element.ALIGN_LEFT;
        //        cellH2.VerticalAlignment = Element.ALIGN_MIDDLE;
        //        cellH2.Border = Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER | Rectangle.NO_BORDER;
        //        cellH2.BorderColor = BaseColor.BLACK;
        //        tableH2.AddCell(cellH2);

        //        //footerTbl.WriteSelectedRows(0, -1, 415, 30, writer.DirectContent);
        //        table.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 20), writer.DirectContent);
        //        tableH.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 100), writer.DirectContent);
        //        tableH2.WriteSelectedRows(0, -1, 0, (doc.PageSize.Height - 150), writer.DirectContent);

        //    }
        //}
     
        public DateTime GetDateTime()
        {
            DateTime serverTime = DateTime.Now;
            DateTime _localTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(serverTime, TimeZoneInfo.Local.Id, "Singapore Standard Time");
            return _localTime;
        }
 
        public string getconfig(string name)
        {
            IniParser parser = new IniParser(AppDomain.CurrentDomain.BaseDirectory + "config.ini");
            return parser.GetSetting("config", name);
        }
        public void GetError(string data, string data2, string data3, string data4)
        {
            string message = string.Format("Time: {0}", GetDateTime().ToString("dd/MM/yyyy hh:mm:ss tt"));
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            message += string.Format("Message: {0}", data);
            message += Environment.NewLine;
            message += string.Format("StackTrace: {0}", data2);
            message += Environment.NewLine;
            message += string.Format("Source: {0}", data3);
            message += Environment.NewLine;
            message += string.Format("TargetSite: {0}", data4);
            message += Environment.NewLine;
            message += "-----------------------------------------------------------";
            message += Environment.NewLine;
            string path = HttpContext.Current.Server.MapPath("~/ErrorLog/ErrorLog-" + GetDateTime().ToString("dd-MM-yyyy") + ".txt");
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(message);
                writer.Close();
            }
        }

        public Document Header(Document pdfDoc, string headername, string headername2, string headername3)
        {
            Paragraph date = new Paragraph(new Chunk("Date : " + GetDateTime().ToString("dd/MM/yyyy"), FontFactory.GetFont("Arial", 10, Font.NORMAL, BaseColor.BLACK)));
            date.Alignment = Element.ALIGN_RIGHT;
            pdfDoc.Add(date);
            PdfPTable table = new PdfPTable(1);
            table.WidthPercentage = 100;
            PdfPCell cell = new PdfPCell();
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
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

        public PdfPCell CellWithRedFullBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
            cell.BorderColor = BaseColor.RED;
            return cell;
        }

        public PdfPCell CellWithBlackFullBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.LEFT_BORDER;
            cell.BorderColor = BaseColor.BLACK;
            return cell;
        }

        public PdfPCell CellWithGrayBottomBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = Rectangle.BOTTOM_BORDER;
            cell.BorderColor = BaseColor.GRAY;
            return cell;
        }

        public PdfPCell CellWithGrayTopBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = Rectangle.TOP_BORDER;
            cell.BorderColor = BaseColor.GRAY;
            return cell;
        }

        public PdfPCell CellWithGrayTopBottomBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
            cell.BorderColor = BaseColor.GRAY;
            return cell;
        }

        public PdfPCell CellWithGrayBottomBorderSmallFont(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 5, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_TOP;
            cell.Border = Rectangle.BOTTOM_BORDER;
            cell.BorderColor = BaseColor.GRAY;
            return cell;
        }

        public PdfPCell CellNoBorder(string Param, int HorizontalAlignment, int FontType)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 9, FontType, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = 0;
            return cell;
        }

        public PdfPCell CellWithRedTopBottomBorder(string Param, int HorizontalAlignment)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER;
            cell.BorderColor = BaseColor.RED;
            return cell;
        }

        public PdfPCell CellWithRedTopBorder(string Param, int HorizontalAlignment)
        {
            var chunk = new Chunk(Param, FontFactory.GetFont("Arial", 7, Font.BOLD, BaseColor.BLACK));
            var cell = new PdfPCell(new Phrase(chunk));
            cell.HorizontalAlignment = HorizontalAlignment;
            cell.VerticalAlignment = Element.ALIGN_MIDDLE;
            cell.Border = Rectangle.TOP_BORDER;
            cell.BorderColor = BaseColor.RED;
            return cell;
        }
  
        public string Insentif(int? kod, int? NegaraID, int? SyarikatID)
        {
            string insentif = "";
            string code = kod.ToString();
            insentif = db.tblOptionConfigsWebs.Where(x => x.fldOptConfFlag1 == "insentif" && x.fldOptConfValue == code && x.fldDeleted == false && x.fld_NegaraID == NegaraID && x.fld_SyarikatID == SyarikatID).Select(s => s.fldOptConfDesc).FirstOrDefault();
            return insentif;
        }
  
    }
}