using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class reptransportListMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public void pdfTransport_list(List<int> pickup_id)
        {
           
           
            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "Transport List.pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(0f, 0f, 10f, 70f);
            try
            {

                IEnumerable<transportList> result = Enumerable.Empty<transportList>();



                foreach (var i in pickup_id)
                {
                    string query = @"SELECT 
                                        a.sr_number,
                                        CONCAT(IFNULL(a.std_first_name, ''),
                                                ' ',
                                                IFNULL(a.std_last_name, '')) std_name,
                                        a.std_father_name,
                                        b.pickup_point,
                                        c.class_name,
                                        e.section_name,
                                        COALESCE(a.std_contact,
                                                a.std_contact1,
                                                a.std_contact2) contact
                                    FROM
                                        sr_register a,
                                        mst_transport b,
                                        mst_class c,
                                        mst_std_class d,
                                        mst_section e,
                                        mst_std_section f
                                    WHERE
                                        a.std_pickup_id = b.pickup_id
                                            AND a.sr_number = d.sr_num
                                            AND d.class_id = c.class_id
                                            AND c.class_id = e.class_id
                                            AND d.class_id = e.class_id
                                            AND a.std_active = 'Y'
                                            AND a.sr_number = f.sr_num
                                            AND f.section_id = e.section_id
                                            AND b.pickup_id = @pickup_id
                                            AND b.session = c.session
                                            AND c.session = d.session
                                            AND d.session = e.session
                                            AND e.session = f.session
                                            AND f.session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y'
                                                    AND session_active = 'Y')
                                    ORDER BY c.class_name , e.section_name";

                   result = result.Concat(con.Query<transportList>(query, new { pickup_id = i }));
                }


                

                   
             

               


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

                int colnumber = 14;

                pt = new PdfPTable(colnumber);
                pt.HeaderRows = 3;
                // pt.WidthPercentage = 90f;

                text = new Chunk("\n");
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = Rectangle.NO_BORDER;
                _cell.Colspan = colnumber;
                pt.AddCell(_cell);

               
                text = new Chunk("Transport list as on Date: "+System.DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 12));
                ph = new Phrase(text);
                ph.Add("\n");
                ph.Add("\n");
                text.SetUnderline(0.1f, -2f);
                _cell = new PdfPCell(ph);
                _cell.Colspan = colnumber;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = Rectangle.NO_BORDER;
                pt.AddCell(_cell);



                text = new Chunk("Sr. No", FontFactory.GetFont("Areal", 8));
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

            

                text = new Chunk("Class", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Section", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Contact No.", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Pickup Point", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
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
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.std_father_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.Colspan = 3;
                   _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.class_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.section_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(list.contact, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk(list.pickup_point, FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
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
            string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
            string Address = ConfigurationManager.AppSettings["Address"].ToString();
            string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
            string MaiLSite = ConfigurationManager.AppSettings["MaiLSite"].ToString();

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
                PdfPTable tabFot = new PdfPTable(new float[] { 1F });
                PdfPCell cell;
                tabFot.TotalWidth = 600f;
                Chunk text;
                Phrase ph = new Phrase();


                text = new Chunk(Address, FontFactory.GetFont("Areal", 12));
                ph = new Phrase();
                ph.Add(text);

                ph.Add("\n");
                //text = new Chunk("(Affiliated to CBSE New Delhi. Affiliation Number 2132182)", FontFactory.GetFont("Areal", 08));
                text = new Chunk(MaiLSite, FontFactory.GetFont("Areal", 12));
                ph.Add(text);
                cell = new PdfPCell(ph);
                //cell = new PdfPCell(new Phrase("Nh-24 Village Ballia Post Dhaneta Teshil Meerganj Bareilly 243504. Ph.9058083211 /n Email: contact@hariti.in   Website: wwww.hariti.edu.in", FontFactory.GetFont("Areal", 8)));
                cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                cell.Padding = 5;
                tabFot.AddCell(cell);
                tabFot.WriteSelectedRows(0, -1, 0, document.Bottom, writer.DirectContent);
            }

            //write on close of document
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);


            }
        }
    }
}