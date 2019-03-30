using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SMS.AcademicReport
{
    public class repTC_form
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public void pdfTransferCertificate()
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (Document doc = new Document(PageSize.A4.Rotate()))
                {
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    PdfPTable pt = new PdfPTable(10);
                    pt.WidthPercentage = 90;
                    // string imageURL = "E:\\HPS\\logo.jpg";
                    

                    PdfPCell _cell;
                    Chunk text;
                    Phrase ph;
                   

                    text = new Chunk(SchoolName, FontFactory.GetFont("Areal", 30));
                    ph = new Phrase();
                    ph.Add(text);
                    ph.Add("\n");
                    ph.Add("\n");
                    text = new Chunk("(" + Affiliation + ")", FontFactory.GetFont("Areal", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan =10;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;
                    //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    pt.AddCell(_cell);
                    

                    ph = new Phrase();
                    text = new Chunk("Scholar’s Register & Transfer Certificate Form", FontFactory.GetFont("Gabriola", 24));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;
                    //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    pt.AddCell(_cell);


                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;
                    //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    pt.AddCell(_cell);

                    doc.Add(pt);


                    pt = new PdfPTable(4);
                    pt.WidthPercentage = 95;
                    ph = new Phrase();
                    text = new Chunk("Admission File No.", FontFactory.GetFont("Arial", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.PaddingTop = 10;
                    _cell.PaddingBottom = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Withdrawal File No.", FontFactory.GetFont("Arial", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.PaddingTop = 10;
                    _cell.PaddingBottom = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Transfer Certificate No.", FontFactory.GetFont("Arial", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.PaddingTop = 10;
                    _cell.PaddingBottom = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Register No.", FontFactory.GetFont("Arial", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.PaddingTop = 10;
                    _cell.PaddingBottom = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT ;
                    _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                    pt.AddCell(_cell);

                    doc.Add(pt);

                  
                    doc.Close();
                    byte[] bytes = ms.ToArray();
                    ms.Close();
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/pdf";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=comparative_result.pdf");
                    HttpContext.Current.Response.ContentType = "application/pdf";
                    HttpContext.Current.Response.Buffer = true;
                    HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    HttpContext.Current.Response.BinaryWrite(bytes);
                    HttpContext.Current.Response.End();
                    HttpContext.Current.Response.Close();
                }
            }
        }
    }
}