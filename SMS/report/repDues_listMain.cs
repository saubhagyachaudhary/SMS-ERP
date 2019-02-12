using Dapper;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class repDues_listMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public void pdfDues_list(int section_id,decimal amt,string operation)
        {

            string query1 = @"SELECT 
                                    CONCAT(IFNULL(b.class_name, ''),
                                            ' Section ',
                                            IFNULL(a.section_name, '')) class_name
                                FROM
                                    mst_section a,
                                    mst_class b
                                WHERE
                                    a.class_id = b.class_id
                                        AND a.section_id = @section_id
                                        AND a.session = b.session
                                        AND a.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

            string class_name = con.Query<string>(query1, new { section_id = section_id }).SingleOrDefault();

            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "DL_Class_"+class_name+".pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(0f, 0f, 10f, 70f);
            try
            {
                

                 query1 = @"SELECT session FROM mst_session where session_finalize = 'Y'";

                string session = con.Query<string>(query1).SingleOrDefault();

                IEnumerable<repDues_list> result;

                string query = "";

                if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
                {
                     query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            AND month_no <= MONTH(DATE(DATE_ADD(NOW(), INTERVAL '00:00' HOUR_MINUTE)))
                                            AND month_no BETWEEN 4 AND 12
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";
                }
                else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 1)
                {

                    query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            and month_no not in (2,3)
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";
                }
                else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 2)
                {

                    query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            and month_no != 3
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";

                    
                }
                else
                {

                    query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";

                }

                    result = con.Query<repDues_list>(query, new { section_id = section_id, session = session,amt = amt});

                

                rep_fees rep = new rep_fees();


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
                /* _cell = new PdfPCell(ph);
                 _cell.Colspan = 3;
                 _cell.HorizontalAlignment = Element.ALIGN_CENTER;*/

                //  _cell.Border = 0;
                // pt.AddCell(_cell);
                //ph.Add("\n");
                //text = new Chunk("Nh-24 Village Ballia, Dhaneta, Meerganj Bareilly-243504", FontFactory.GetFont("Areal", 08));
                //ph.Add(text);


                text = new Chunk("("+Affiliation+")", FontFactory.GetFont("Areal", 12));
                ph.Add(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 5;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = Rectangle.BOTTOM_BORDER;
                _cell.PaddingBottom = 5;
                //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pt.AddCell(_cell);





                doc.Add(pt);



                pt = new PdfPTable(9);

                //pt.WidthPercentage = 95f;

                text = new Chunk("\n");
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = Rectangle.TOP_BORDER;
                _cell.Colspan = 9;
                pt.AddCell(_cell);


                text = new Chunk("Dues list of class "+ class_name, FontFactory.GetFont("Areal", 12));
                ph = new Phrase(text);
                ph.Add("\n");
                ph.Add("\n");
                text.SetUnderline(0.1f, -2f);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 9;
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
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Father's Name", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Pickup Point", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Contact", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Dues", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                decimal total = 0;
                int line = 1;
                int serial = 0;

                foreach (var fee in result)
                {
                    serial++;

                    ph = new Phrase();
                    text = new Chunk(serial.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.sr_number.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.std_father_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.pickup_point, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.contact, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.amount.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);
                    
                    total = total + fee.amount;

                    if (line == 2)
                        line = 1;
                    else
                        line++;
                }

                //_cell.FixedHeight = 150;

                text = new Chunk("Total", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Colspan = 8;
                pt.AddCell(_cell);

                text = new Chunk(total.ToString(), FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                pt.AddCell(_cell);

              
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

        public IEnumerable<repDues_list> duesList_Notice(int section_id, decimal amt, string operation,string month_name)
        {
            IEnumerable<repDues_list> result;
            string query;

            string query1 = @"SELECT session FROM mst_session where session_finalize = 'Y'";

            string session = con.Query<string>(query1).SingleOrDefault();

            int month_no = 0;

            switch (month_name)
            {
                case "January":
                    month_no = 1;
                    break;
                case "February":
                    month_no = 2;
                    break;
                case "March":
                    month_no = 3;
                    break;
                case "April":
                    month_no = 4;
                    break;
                case "May":
                    month_no = 5;
                    break;
                case "June":
                    month_no = 6;
                    break;
                case "July":
                    month_no = 7;
                    break;
                case "August":
                    month_no = 8;
                    break;
                case "September":
                    month_no = 9;
                    break;
                case "October":
                    month_no = 10;
                    break;
                case "November":
                    month_no = 11;
                    break;
                case "December":
                    month_no = 12;
                    break;
               
            }

            if (month_no >= 4 && month_no  <= 12)
            {
                query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                            month_no
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            AND month_no <= @month_no
                                            AND month_no BETWEEN 4 AND 12
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";
            }
            else if (month_no  == 1)
            {

                query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                            month_no
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            and month_no not in (2,3)
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";
            }
            else if (month_no  == 2)
            {

                query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                            month_no
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            and month_no != 3
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";


            }
            else
            {

                query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.pickup_point,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                            month_no
                                    FROM
                                        out_standing a, sr_register b, mst_transport c, mst_std_section d
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.sr_number = d.sr_num
                                            AND d.section_id = @section_id
                                            AND a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND b.std_active = 'Y'
                                            AND b.std_pickup_id = c.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
	                                a.amount " + operation + " @amt ORDER BY pickup_point";

            }

             result = con.Query<repDues_list>(query, new { section_id = section_id, session = session, amt = amt,month_no = month_no });

            return result;
        }

        public void pdfDuesList_notice(IEnumerable<repDues_list> list)
        {

           

            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "DL_Notice.pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(40f, 40f, 30f, 70f);
            try
            {

              


                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();



                doc.Open();
                // string imageURL = "E:\\HPS\\logo.jpg";



                PdfPTable pt;


                PdfPCell _cell;
                Chunk text;
                Phrase ph;
                foreach (var li in list)
                {
                    pt = new PdfPTable(9);
                    pt.WidthPercentage = 90;
                    // string imageURL = "E:\\HPS\\logo.jpg";
                    string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                    Image jpg = Image.GetInstance(imageURL);
                    jpg.ScaleAbsolute(60f, 60f);

                    _cell = new PdfPCell(jpg);
                    _cell.Border = 0;

                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;

                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    text = new Chunk(SchoolName, FontFactory.GetFont("Areal", 24));
                    ph = new Phrase();
                    ph.Add(text);
                    ph.Add("\n");
                    ph.Add("\n");
                    text = new Chunk("(" + Affiliation + ")", FontFactory.GetFont("Areal", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 8;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;
                    //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    pt.AddCell(_cell);

                    doc.Add(pt);

                    text = new Chunk("\n", FontFactory.GetFont("Times New Roman", 12));
                    Paragraph para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("\n", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("To", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk(li.std_father_name, FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("\n", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("Date: " + System.DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.SpacingBefore = 10;
                    para.SpacingAfter = 10;
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("Subject: Reminder about pending fees", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("\n", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("Dear Parents,", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("\n", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);

                    text = new Chunk("Most respectfully, it is stated that an amount of " + li.amount + ", fee upto the month of "+ li.month_name +" is pending against school fee of your son/daughter " + li.name + ". We earnestly request you to kindly settle the payment as early as possible so that " + li.name + " can smoothly continue his studies at our prestigious institute. Also please be informed that we have a policy of accepting fees in instalments. Your prompt attention in this matter will be highly appreciated.", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;
                    doc.Add(para);





                    text = new Chunk("Signature", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.SpacingBefore = 40;
                    para.Alignment = 0;

                    doc.Add(para);

                    text = new Chunk("Head Master/Principal", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = 0;

                    doc.Add(para);
                    doc.NewPage();
                }

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

        public void pdfDues_listTransportWise(List<int> pickup_id, decimal amt, string operation)
        {

            mst_sessionMain session = new mst_sessionMain();

            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "DL_Transport.pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(0f, 0f, 10f, 70f);
            try
            {

                IEnumerable<repDues_list> result = Enumerable.Empty<repDues_list>();

                string query = "";
                foreach (var i in pickup_id)
                {
                    if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
                    {
                        query = @"SELECT 
                                        *
                                    FROM
                                        (SELECT 
                                            a.sr_number,
                                                CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                                b.std_father_name,
                                                COALESCE(std_contact, std_contact1, std_contact2) contact,
                                                c.class_name,
                                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                                e.pickup_point
                                        FROM
                                            out_standing a, sr_register b, mst_class c, mst_std_class d, mst_transport e
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND b.std_pickup_id = @pickup_id
                                                AND month_no <= MONTH(DATE(DATE_ADD(NOW(), INTERVAL '00:00' HOUR_MINUTE)))
                                                AND month_no BETWEEN 4 AND 12
                                                AND e.session = d.session
                                                AND d.session = c.session
                                                AND c.session = a.session
                                                AND a.session = @session
                                                AND b.std_active = 'Y'
                                                AND b.sr_number = d.sr_num
                                                AND d.class_id = c.class_id
                                                AND b.std_pickup_id = e.pickup_id
                                        GROUP BY sr_number) a
                                    WHERE
                                         a.amount " + operation + " @amt ORDER BY class_name";

                    }
                    else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 1)
                    {

                        query = @"SELECT 
                                        *
                                    FROM
                                        (SELECT 
                                            a.sr_number,
                                                CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                                b.std_father_name,
                                                COALESCE(std_contact, std_contact1, std_contact2) contact,
                                                c.class_name,
                                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                                e.pickup_point
                                        FROM
                                            out_standing a, sr_register b, mst_class c, mst_std_class d, mst_transport e
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND b.std_pickup_id = @pickup_id
                                                AND month_no NOT IN (2 , 3)
                                                AND e.session = d.session
                                                AND d.session = c.session
                                                AND c.session = a.session
                                                AND a.session = @session
                                                AND b.std_active = 'Y'
                                                AND b.sr_number = d.sr_num
                                                AND d.class_id = c.class_id
                                                AND b.std_pickup_id = e.pickup_id
                                        GROUP BY sr_number) a
                                    WHERE
                                       a.amount " + operation + " @amt ORDER BY class_name";



                    }
                    else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 2)
                    {
                        query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.class_name,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                            e.pickup_point
                                    FROM
                                        out_standing a, sr_register b, mst_class c, mst_std_class d,mst_transport e
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.std_pickup_id = @pickup_id
                                            AND month_no != 3
                                            AND e.session = d.session
                                            AND d.session = c.session
                                            AND c.session = a.session
                                            AND a.session = @session
                                            AND b.std_active = 'Y'
                                            AND b.sr_number = d.sr_num
                                            AND d.class_id = c.class_id
                                            AND b.std_pickup_id = e.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
                                   a.amount " + operation + " @amt ORDER BY class_name";
                    }
                    else
                    {
                        query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        a.sr_number,
                                            CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) name,
                                            b.std_father_name,
                                            COALESCE(std_contact, std_contact1, std_contact2) contact,
                                            c.class_name,
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                            e.pickup_point
                                    FROM
                                        out_standing a, sr_register b, mst_class c, mst_std_class d,mst_transport e
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND b.std_pickup_id = @pickup_id
                                            AND e.session = d.session
                                            AND d.session = c.session
                                            AND c.session = a.session
                                            AND a.session = @session
                                            AND b.std_active = 'Y'
                                            AND b.sr_number = d.sr_num
                                            AND d.class_id = c.class_id
                                            AND b.std_pickup_id = e.pickup_id
                                    GROUP BY sr_number) a
                                WHERE
                                   a.amount " + operation + " @amt ORDER BY class_name";

                    }

                    result = result.Concat(con.Query<repDues_list>(query, new { pickup_id = i, session = session.findFinal_Session(), amt = amt }));
                }


                rep_fees rep = new rep_fees();


                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();



                doc.Open();
                // string imageURL = "E:\\HPS\\logo.jpg";
                string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                Image jpg = Image.GetInstance(imageURL);
                jpg.ScaleAbsolute(50f, 50f);


                PdfPTable pt = new PdfPTable(6);
                pt.WidthPercentage = 85f;

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
                /* _cell = new PdfPCell(ph);
                 _cell.Colspan = 3;
                 _cell.HorizontalAlignment = Element.ALIGN_CENTER;*/

                //  _cell.Border = 0;
                // pt.AddCell(_cell);
                //ph.Add("\n");
                //text = new Chunk("Nh-24 Village Ballia, Dhaneta, Meerganj Bareilly-243504", FontFactory.GetFont("Areal", 08));
                //ph.Add(text);


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



                pt = new PdfPTable(10);

                pt.WidthPercentage = 85f;

                text = new Chunk("\n");
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = Rectangle.TOP_BORDER;
                _cell.Colspan = 10;
                pt.AddCell(_cell);


                text = new Chunk("Transport wise dues list as on date " + System.DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 12));
                ph = new Phrase(text);
                ph.Add("\n");
                ph.Add("\n");
                text.SetUnderline(0.1f, -2f);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 10;
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
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Father's Name", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Class", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Contact", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Pickup Point", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 1;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Dues", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                decimal total = 0;
                int line = 1;
                int serial = 0;

                foreach (var fee in result)
                {
                    serial++;

                    ph = new Phrase();
                    text = new Chunk(serial.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.sr_number.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.std_father_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.class_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.contact, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.pickup_point, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 1;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(fee.amount.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    total = total + fee.amount;

                    if (line == 2)
                        line = 1;
                    else
                        line++;
                }

                //_cell.FixedHeight = 150;

                text = new Chunk("Total", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Colspan = 9;
                pt.AddCell(_cell);

                text = new Chunk(total.ToString(), FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                pt.AddCell(_cell);


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