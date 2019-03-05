using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class repStd_half_day
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public class std_halfday_log
        {
            public int sr_number { get; set; }

            public string std_name { get; set; }

            public string std_father_name { get; set; }

            public string class_name { get; set; }

            public string section_name { get; set; }

            public string std_contact { get; set; }

            public string reason { get; set; }

            public DateTime date_time { get; set; }
        }

        public void pdfstd_half_day(DateTime fromDate, DateTime toDate, string session)
        {


            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "Class Student List.pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(0f, 0f, 10f, 70f);
            try
            {

                string query = @"SELECT 
                                    a.sr_number,
                                    CONCAT(IFNULL(b.std_first_name, ''),
                                            ' ',
                                            IFNULL(b.std_last_name, '')) std_name,
                                    b.std_father_name,
                                    CONCAT('class: ',
                                            e.class_name,
                                            '  Section: ',
                                            f.section_name) class_name,
                                    COALESCE(b.std_contact,
                                            b.std_contact1,
                                            b.std_contact2) std_contact,
                                    a.reason,
                                    a.date_time
                                FROM
                                    std_halfday_log a,
                                    sr_register b,
                                    mst_std_class c,
                                    mst_std_section d,
                                    mst_class e,
                                    mst_section f
                                WHERE
                                    DATE(date_time) BETWEEN @fromDate AND @toDate
                                        AND a.session = c.session
                                        AND c.session = d.session
                                        AND d.session = e.session
                                        AND e.session = f.session
                                        AND f.session = @session
                                        AND a.sr_number = b.sr_number
                                        AND b.sr_number = c.sr_num
                                        AND c.sr_num = d.sr_num
                                        AND c.class_id = e.class_id
                                        AND d.section_id = f.section_id
                                        AND c.class_id = f.class_id";

                var result = con.Query<std_halfday_log>(query, new { fromDate = fromDate, toDate  = toDate, session = session});


                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();



                doc.Open();
                // string imageURL = "E:\\HPS\\logo.jpg";
                string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                Image jpg = Image.GetInstance(imageURL);
                jpg.ScaleAbsolute(50f, 50f);


                PdfPTable pt = new PdfPTable(6);


                PdfPCell _cell;
                Chunk text;
                Phrase ph;

                _cell = new PdfPCell(jpg);
                _cell.Border = 0;
                _cell.Border = Rectangle.BOTTOM_BORDER;
                _cell.PaddingBottom = 5;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);


                text = new Chunk(SchoolName, FontFactory.GetFont("Areal", 24));
                ph = new Phrase();
                ph.Add(text);
                ph.Add("\n");


                text = new Chunk("(" + Affiliation + ")", FontFactory.GetFont("Areal", 12));
                ph.Add(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 5;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = Rectangle.BOTTOM_BORDER;
                _cell.PaddingBottom = 5;
                //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pt.AddCell(_cell);

                doc.Add(pt);

                int colnumber = 20;

                pt = new PdfPTable(colnumber);
                pt.HeaderRows = 3;
                pt.WidthPercentage = 90f;

                text = new Chunk("\n");
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = Rectangle.NO_BORDER;
                _cell.Colspan = colnumber;
                pt.AddCell(_cell);


                text = new Chunk("Half Day Student list From " + fromDate.ToString("dd/MM/yyyy")+" to "+toDate.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 12));
                ph = new Phrase(text);
                ph.Add("\n");
                ph.Add("\n");
                text.SetUnderline(0.1f, -2f);
                _cell = new PdfPCell(ph);
                _cell.Colspan = colnumber;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = Rectangle.NO_BORDER;
                pt.AddCell(_cell);

                

                text = new Chunk("Serial No.", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Adm No.", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Student Name", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 3;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Father's Name", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 3;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Class Name", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 3;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Contact", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Date & Time", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("reason", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 5;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                int sr_no = 0;

                int line = 1; ;

                foreach (var list in result)
                {
                    sr_no++;
                    ph = new Phrase();
                    text = new Chunk(sr_no.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.sr_number.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.std_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 3;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.std_father_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.Colspan = 3;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.class_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.Colspan = 3;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.std_contact, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.date_time.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk(list.reason, FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 5;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    if (line == 2)
                        line = 1;
                    else
                        line++;

                }


                doc.Add(pt);



                doc.Close();

                HttpContext.Current.Response.OutputStream.Write(ms.ToArray(), 0, ms.ToArray().Length);
                HttpContext.Current.Response.OutputStream.Flush();
                HttpContext.Current.Response.OutputStream.Close();





            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                doc.Close();
                ms.Flush();
            }
        }

        public class PDFFooter : PdfPageEventHelper
        {
            // write on top of document
            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                base.OnOpenDocument(writer, document);

            }

            // write on start of each page
            public override void OnStartPage(PdfWriter writer, Document document)
            {
                base.OnStartPage(writer, document);
            }

            // write on end of each page
            public override void OnEndPage(PdfWriter writer, Document document)
            {
                base.OnEndPage(writer, document);
            }

            //write on close of document
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
            }
        }
    }
}