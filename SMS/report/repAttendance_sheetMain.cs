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
using SMS.Models;
using System.Globalization;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace SMS.report
{
    public class repAttendance_sheetMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
        string donotreplyMail = ConfigurationManager.AppSettings["donotreplyMail"].ToString();
        string donotreplyMailPassword = ConfigurationManager.AppSettings["donotreplyMailPassword"].ToString();
        

        public void pdfAttendanceSheet(int class_id,int section_id,int month_no, string session)
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
                                        AND b.class_id = @class_id
                                        AND a.section_id = @section_id
                                        AND a.session = b.session
                                        AND a.session = @session";

            string class_name = con.Query<string>(query1, new { section_id = section_id, session = session , class_id = class_id }).SingleOrDefault();

            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "Att_Sheet_" + class_name + ".pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4.Rotate(),0,0,0,0);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(0f, 0f, 10f, 30f);
            try
            {




                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();



                doc.Open();
                // string imageURL = "E:\\HPS\\logo.jpg";
                string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                Image jpg = Image.GetInstance(imageURL);
                jpg.ScaleAbsolute(80f, 80f);


                PdfPTable pt = new PdfPTable(10);


                PdfPCell _cell;
                Chunk text;
                Phrase ph;
                _cell = new PdfPCell(jpg);
                _cell.Border = 0;

                _cell.Border = Rectangle.NO_BORDER;
                _cell.PaddingBottom = 5;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);


                text = new Chunk(SchoolName, FontFactory.GetFont("Areal", 40));
                ph = new Phrase();
                ph.Add(text);
                ph.Add("\n");

                text = new Chunk("("+Address+")", FontFactory.GetFont("Areal", 20));
                ph.Add(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 9;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                _cell.PaddingBottom = 5;
                //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                pt.AddCell(_cell);



                mst_sessionMain sess = new mst_sessionMain();

                mst_session mst = sess.getStartEndDate(session);

                int year = 0;
                if (month_no == 1 || month_no == 2 || month_no == 3)
                {
                    year = mst.session_end_date.Year;
                }
                else
                {
                    year = mst.session_start_date.Year;
                }


                doc.Add(pt);

                var date_list = getAllDates(year, month_no);

                pt = new PdfPTable(date_list.Count()+24);

                pt.WidthPercentage = 95f;

                pt.HeaderRows = 3;


                text = new Chunk("\n");
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = Rectangle.NO_BORDER;
                _cell.Colspan = 5;
                pt.AddCell(_cell);

                

               
               


                var startOfMonth = new DateTime(year, month_no, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                text = new Chunk("Attendance Sheet for the month of "+ CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month_no)+" "+year.ToString() +" Class "+ class_name, FontFactory.GetFont("Areal", 12));
                ph = new Phrase(text);
                ph.Add("\n");
                ph.Add("\n");
                text.SetUnderline(0.1f, -2f);
                _cell = new PdfPCell(ph);
                _cell.Colspan = date_list.Count() + 24;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = Rectangle.NO_BORDER;
                pt.AddCell(_cell);





          

                string query = @"SELECT 
                                        a.sr_number sr_num,
                                        c.roll_number roll_no,
                                        CONCAT(IFNULL(a.std_first_name, ''),
                                                ' ',
                                                IFNULL(std_last_name, '')) std_name
                                    FROM
                                        sr_register a,
                                        mst_section b,
                                        mst_rollnumber c,
                                        mst_std_section d
                                    WHERE
                                        d.section_id = b.section_id
                                            AND b.section_id = @section_id
                                            AND c.sr_num = a.sr_number
                                            AND a.sr_number = d.sr_num
                                            AND b.session = c.session
                                            AND c.session = d.session
                                            AND d.session = @session
                                    ORDER BY c.roll_number";


                IEnumerable<repAttendance_sheet> sr_list = con.Query<repAttendance_sheet>(query, new { section_id = section_id,session=session});


                ph = new Phrase();
                text = new Chunk("Student Particulars", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = 12;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Dates in the month of "+ CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month_no) + " " + year.ToString(), FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = date_list.Count();
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Student attendance summary", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = 12;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Adm No.", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Roll No.", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Student Name", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = 8;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                for (int i = 0; i<= date_list.Count()-1;i++)
                {

                    ph = new Phrase();
                    text = new Chunk(date_list[i].Day.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    text = new Chunk(date_list[i].DayOfWeek.ToString().Substring(0, 1), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                }

                ph = new Phrase();
                text = new Chunk("Month W/D", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Month Prsnt", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Month Absnt", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Total W/D", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);


                ph = new Phrase();
                text = new Chunk("Total Prsnt", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk("Total Absnt", FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                //IEnumerable<repAttendance_sheet> check;

                IEnumerable<repAttendance_sheet> check1;

                IEnumerable<repAttendance_sheet> P_count;

                IEnumerable<repAttendance_sheet> A_count;

                IEnumerable<repAttendance_sheet> T_P_count;

                IEnumerable<repAttendance_sheet> T_A_count;

                query = @"SELECT 
                                CASE
                                    WHEN a.attendance = 1 THEN 'P'
                                    ELSE 'A'
                                END attendance,
                                a.sr_num,
                                c.roll_number roll_no,
                                DAY(att_date) day
                            FROM
                                attendance_register a,
                                mst_std_section b,
                                mst_rollnumber c
                            WHERE
                                b.section_id = @section_id
                                    AND att_date BETWEEN @startOfMonth AND @endOfMonth
                                    AND a.session = @session
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND a.sr_num = b.sr_num
                                    AND b.sr_num = c.sr_num";

                check1 = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth});

                query = @"SELECT 
                                COUNT(*) P_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date BETWEEN @startOfMonth AND @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 1
                            GROUP BY sr_num";

                P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                query = @"SELECT 
                                COUNT(*) A_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date BETWEEN @startOfMonth AND @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 0
                            GROUP BY sr_num";

                A_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                query = @"SELECT 
                                COUNT(*) P_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date <= @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 1
                            GROUP BY sr_num";

                T_P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                query = @"SELECT 
                                COUNT(*) A_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date <= @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 0
                            GROUP BY sr_num";

                T_A_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });


                foreach (repAttendance_sheet dtt in sr_list)
                {
                    ph = new Phrase();
                    text = new Chunk(dtt.sr_num.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(dtt.roll_no.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(dtt.std_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 8;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);


                    for (int i = 0; i <= date_list.Count() - 1; i++)
                    {

                        var check = (from e in check1 where e.day == i+1 where e.sr_num == dtt.sr_num where e.roll_no == dtt.roll_no select e).ToList();

                    

                        if (check.Count() > 0)
                        {
                           

                            ph = new Phrase();
                            if(check[0].attendance == "P")
                            {
                               
                                text = new Chunk(check[0].attendance, FontFactory.GetFont("Areal", 8,BaseColor.BLACK));
                            }
                          
                            else
                            {
                               
                               text = new Chunk(check[0].attendance, FontFactory.GetFont("Areal", 8,BaseColor.RED));
                            }
                                
                            ph.Add(text);
                            ph.Add("\n");
                            _cell = new PdfPCell(ph);
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);
                        }
                        else
                        {
                            ph = new Phrase();
                            text = new Chunk("", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            ph.Add("\n");
                            _cell = new PdfPCell(ph);
                            if (i >= int.Parse(System.DateTime.Now.ToString("dd"))-1 && month_no == int.Parse(DateTime.Now.ToString("MM")))
                            {
                                _cell.BackgroundColor = BaseColor.WHITE;
                            }
                            else
                            {
                                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            }
                            
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);
                        }
                       

                    }

                
                    var total_present = (from e in P_count where e.sr_num == dtt.sr_num select e).ToList();

                    var total_absent = (from e in A_count where e.sr_num == dtt.sr_num select e).ToList();

                    if(total_present.Count() == 0)
                    {
                       
                        total_present.Add(new repAttendance_sheet { P_count = 0 });
                    }

                    if (total_absent.Count() == 0)
                    {
                        total_absent.Add(new repAttendance_sheet { A_count = 0 });
                    }


                    ph = new Phrase();
                    text = new Chunk((total_present[0].P_count+total_absent[0].A_count).ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(total_present[0].P_count.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(total_absent[0].A_count.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    total_present = (from e in T_P_count where e.sr_num == dtt.sr_num select e).ToList();

                    total_absent = (from e in T_A_count where e.sr_num == dtt.sr_num select e).ToList();

                    if (total_present.Count() == 0)
                    {

                        total_present.Add(new repAttendance_sheet { P_count = 0 });
                    }

                    if (total_absent.Count() == 0)
                    {
                        total_absent.Add(new repAttendance_sheet { A_count = 0 });
                    }

             
                    ph = new Phrase();
                    text = new Chunk((total_present[0].P_count+total_absent[0].A_count).ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(total_present[0].P_count.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(total_absent[0].A_count.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

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

        public void MailAttendanceSheet(int section_id, int month_no, string session, string mail_id,DateTime att_date)
        {
            using (MemoryStream ms = new MemoryStream())
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

                //MemoryStream ms = new MemoryStream();

                HttpContext.Current.Response.ContentType = "application/pdf";
                string name = "Att_Sheet_" + class_name + ".pdf";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment;filename=" + name);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
                var doc = new Document(PageSize.A4.Rotate(), 0, 0, 0, 0);

                // MemoryStream stream = new MemoryStream();
                doc.SetMargins(0f, 0f, 10f, 30f);
                try
                {




                    PdfWriter writer = PdfWriter.GetInstance(doc,ms);

                    writer.CloseStream = false;


                    doc.Open();
                    // string imageURL = "E:\\HPS\\logo.jpg";
                    string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                    Image jpg = Image.GetInstance(imageURL);
                    jpg.ScaleAbsolute(80f, 80f);


                    PdfPTable pt = new PdfPTable(10);


                    PdfPCell _cell;
                    Chunk text;
                    Phrase ph;
                    _cell = new PdfPCell(jpg);
                    _cell.Border = 0;

                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    text = new Chunk(SchoolName, FontFactory.GetFont("Areal", 40));
                    ph = new Phrase();
                    ph.Add(text);
                    ph.Add("\n");

                    text = new Chunk("("+Affiliation+")", FontFactory.GetFont("Areal", 20));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 9;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;
                    //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    pt.AddCell(_cell);



                    mst_sessionMain sess = new mst_sessionMain();

                    mst_session mst = sess.getStartEndDate(session);

                    int year = 0;
                    if (month_no == 1 || month_no == 2 || month_no == 3)
                    {
                        year = mst.session_end_date.Year;
                    }
                    else
                    {
                        year = mst.session_start_date.Year;
                    }


                    doc.Add(pt);

                    var date_list = getAllDates(year, month_no);

                    pt = new PdfPTable(date_list.Count() + 24);

                    pt.WidthPercentage = 95f;

                    pt.HeaderRows = 3;


                    text = new Chunk("\n");
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 5;
                    pt.AddCell(_cell);







                    var startOfMonth = new DateTime(year, month_no, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                    text = new Chunk("Attendance Sheet for the month of " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month_no) + " " + year.ToString() + " Class " + class_name, FontFactory.GetFont("Areal", 12));
                    ph = new Phrase(text);
                    ph.Add("\n");
                    ph.Add("\n");
                    text.SetUnderline(0.1f, -2f);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = date_list.Count() + 24;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = Rectangle.NO_BORDER;
                    pt.AddCell(_cell);


                    string query = @"SELECT 
                                        a.sr_number sr_num,
                                        c.roll_number roll_no,
                                        CONCAT(IFNULL(a.std_first_name, ''),
                                                ' ',
                                                IFNULL(std_last_name, '')) std_name
                                    FROM
                                        sr_register a,
                                        mst_section b,
                                        mst_rollnumber c,
                                        mst_std_section d
                                    WHERE
                                        d.section_id = b.section_id
                                            AND b.section_id = @section_id
                                            AND c.sr_num = a.sr_number
                                            AND a.sr_number = d.sr_num
                                            AND b.session = c.session
                                            AND c.session = d.session
                                            AND d.session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')
                                    ORDER BY c.roll_number";


                    IEnumerable<repAttendance_sheet> sr_list = con.Query<repAttendance_sheet>(query, new { section_id = section_id });


                    ph = new Phrase();
                    text = new Chunk("Student Particulars", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 12;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Dates in the month of " + CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month_no) + " " + year.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = date_list.Count();
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Student attendance summary", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 12;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Adm No.", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;

                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Roll No.", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Student Name", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 8;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    for (int i = 0; i <= date_list.Count() - 1; i++)
                    {

                        ph = new Phrase();
                        text = new Chunk(date_list[i].Day.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        text = new Chunk(date_list[i].DayOfWeek.ToString().Substring(0, 1), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                    }

                    ph = new Phrase();
                    text = new Chunk("Month W/D", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Month Prsnt", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Month Absnt", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Total W/D", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    ph = new Phrase();
                    text = new Chunk("Total Prsnt", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Total Absnt", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    //IEnumerable<repAttendance_sheet> check;

                    IEnumerable<repAttendance_sheet> check1;

                    IEnumerable<repAttendance_sheet> P_count;

                    IEnumerable<repAttendance_sheet> A_count;

                    IEnumerable<repAttendance_sheet> T_P_count;

                    IEnumerable<repAttendance_sheet> T_A_count;

                    query = @"SELECT 
                                CASE
                                    WHEN a.attendance = 1 THEN 'P'
                                    ELSE 'A'
                                END attendance,
                                a.sr_num,
                                c.roll_number roll_no,
                                DAY(att_date) day
                            FROM
                                attendance_register a,
                                mst_std_section b,
                                mst_rollnumber c
                            WHERE
                                b.section_id = @section_id
                                    AND att_date BETWEEN @startOfMonth AND @endOfMonth
                                    AND a.session = @session
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND a.sr_num = b.sr_num
                                    AND b.sr_num = c.sr_num";

                    check1 = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"SELECT 
                                COUNT(*) P_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date BETWEEN @startOfMonth AND @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 1
                            GROUP BY sr_num";

                    P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"SELECT 
                                COUNT(*) A_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date BETWEEN @startOfMonth AND @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 0
                            GROUP BY sr_num";

                    A_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"SELECT 
                                COUNT(*) P_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date <= @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 1
                            GROUP BY sr_num";

                    T_P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"SELECT 
                                COUNT(*) A_count, a.sr_num
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date <= @endOfMonth
                                    AND a.session = @session
                                    AND a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND a.attendance = 0
                            GROUP BY sr_num";

                    T_A_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });



                    foreach (repAttendance_sheet dtt in sr_list)
                    {
                        ph = new Phrase();
                        text = new Chunk(dtt.sr_num.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(dtt.roll_no.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(dtt.std_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 8;
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        pt.AddCell(_cell);


                        for (int i = 0; i <= date_list.Count() - 1; i++)
                        {

                            var check = (from e in check1 where e.day == i + 1 where e.sr_num == dtt.sr_num where e.roll_no == dtt.roll_no select e).ToList();



                            if (check.Count() > 0)
                            {


                                ph = new Phrase();
                                if (check[0].attendance == "P")
                                {

                                    text = new Chunk(check[0].attendance, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));

                                  
                                }

                                else
                                {

                                    text = new Chunk(check[0].attendance, FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                }

                                ph.Add(text);
                                ph.Add("\n");
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }
                            else
                            {
                                ph = new Phrase();
                                text = new Chunk("", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                ph.Add("\n");
                                _cell = new PdfPCell(ph);
                                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }


                        }


                        var total_present = (from e in P_count where e.sr_num == dtt.sr_num select e).ToList();

                        var total_absent = (from e in A_count where e.sr_num == dtt.sr_num select e).ToList();

                        if (total_present.Count() == 0)
                        {

                            total_present.Add(new repAttendance_sheet { P_count = 0 });
                        }

                        if (total_absent.Count() == 0)
                        {
                            total_absent.Add(new repAttendance_sheet { A_count = 0 });
                        }


                        ph = new Phrase();
                        text = new Chunk((total_present[0].P_count + total_absent[0].A_count).ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(total_present[0].P_count.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(total_absent[0].A_count.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        total_present = (from e in T_P_count where e.sr_num == dtt.sr_num select e).ToList();

                        total_absent = (from e in T_A_count where e.sr_num == dtt.sr_num select e).ToList();

                        if (total_present.Count() == 0)
                        {

                            total_present.Add(new repAttendance_sheet { P_count = 0 });
                        }

                        if (total_absent.Count() == 0)
                        {
                            total_absent.Add(new repAttendance_sheet { A_count = 0 });
                        }


                        ph = new Phrase();
                        text = new Chunk((total_present[0].P_count + total_absent[0].A_count).ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(total_present[0].P_count.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(total_absent[0].A_count.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                    }



                    doc.Add(pt);



                    doc.Close();

                    ms.Position = 0;

                    query = @"SELECT 
                                    COUNT(*) A_count
                                FROM
                                    attendance_register a,
                                    sr_register b,
                                    mst_std_section c
                                WHERE
                                    c.section_id = @section_id
                                        AND att_date = DATE(DATE_ADD(NOW(),
                                            INTERVAL '00:00' HOUR_MINUTE))
                                        AND a.session = @session
                                        AND a.session = c.session
                                        AND a.sr_num = b.sr_number
                                        AND b.sr_number = c.sr_num
                                        AND a.attendance = 1";

                    int T_P_count_day = con.Query<int>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth }).SingleOrDefault();


                    query = @"SELECT 
                                    COUNT(*) A_count
                                FROM
                                    attendance_register a,
                                    sr_register b,
                                    mst_std_section c
                                WHERE
                                    c.section_id = @section_id
                                        AND att_date = DATE(DATE_ADD(NOW(),
                                            INTERVAL '00:00' HOUR_MINUTE))
                                        AND a.session = @session
                                        AND a.session = c.session
                                        AND a.sr_num = b.sr_number
                                        AND b.sr_number = c.sr_num
                                        AND a.attendance = 0";

                    int T_A_count_day = con.Query<int>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth }).SingleOrDefault();

                    query = @"SELECT 
                                a.sr_num,
                                CONCAT(IFNULL(std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name,
                                COALESCE(std_contact, std_contact1, std_contact2) contact
                            FROM
                                attendance_register a,
                                sr_register b,
                                mst_std_section c
                            WHERE
                                c.section_id = @section_id
                                    AND att_date = DATE(DATE_ADD(NOW(),
                                        INTERVAL '00:00' HOUR_MINUTE))
                                    AND a.session = @session
                                    and a.session = c.session
                                    AND a.sr_num = b.sr_number
                                    and b.sr_number = c.sr_num
                                    AND a.attendance = 0";

                    IEnumerable<repAttendance_sheet> absent_details = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });


                    //string body = "Total number of students Present " + T_P_count_day +"<br>" + "Total number of students Absent "+T_A_count_day ;



                    string body1 = @"";


                    if (absent_details.Count() > 0)
                    {
                       
                        
                        int serial = 0;
                        foreach (var details in absent_details)
                        {
                            query = @"SELECT 
                                            a.attendance
                                        FROM
                                            attendance_register a,
                                            mst_std_section b
                                        WHERE
                                            a.sr_num = b.sr_num
                                                AND a.session = b.session
                                                AND b.section_id = @section_id
                                                AND a.session =  @session
                                                AND a.sr_num = @sr_num
                                        ORDER BY att_date DESC";

                            IEnumerable<int> absent_count = con.Query<int>(query, new { section_id = section_id, session = session, sr_num = details.sr_num });

                            int serial_count = 0;

                            foreach(int cnt in absent_count)
                            {
                                if (cnt == 0)
                                {
                                    serial_count = serial_count + 1;
                                }
                                else
                                {
                                    break;
                                }
                            }

                            serial = serial + 1;

                            body1 = body1 + @" <tr>
                                    <td style='border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;'>" + serial + @"</td>
                                    <td style='border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;'>" + details.sr_num + @"</td>
                                    <td style='border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;'>" + details.std_name + @"</td>
                                    <td style='border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;'>" + details.contact + @"</td>
                                    <td style='border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;' align='center'>" + serial_count + @"</td>
                                  </tr>";

                            // body1 = body1 + details.sr_num + " " + details.std_name + " " + details.contact + Environment.NewLine;
                        }

                        body1 = @"<tr>
                                   <th style='border-width:1px;border-style:solid;border-color:#ddd;padding-right:8px;padding-left:8px;padding-top:12px;padding-bottom:12px;text-align:left;background-color:#2d1846;color:white;'>Serial No.</th>
                                   <th style='border-width:1px;border-style:solid;border-color:#ddd;padding-right:8px;padding-left:8px;padding-top:12px;padding-bottom:12px;text-align:left;background-color:#2d1846;color:white;'>Admission No.</th>
                                   <th style='border-width:1px;border-style:solid;border-color:#ddd;padding-right:8px;padding-left:8px;padding-top:12px;padding-bottom:12px;text-align:left;background-color:#2d1846;color:white;'>Student Name</th>
                                   <th style='border-width:1px;border-style:solid;border-color:#ddd;padding-right:8px;padding-left:8px;padding-top:12px;padding-bottom:12px;text-align:left;background-color:#2d1846;color:white;'>Contact No</th>
                                   <th style='border-width:1px;border-style:solid;border-color:#ddd;padding-right:8px;padding-left:8px;padding-top:12px;padding-bottom:12px;text-align:left;background-color:#2d1846;color:white;'>Cont Absent Days</th>
                                   </tr>" + body1;

                       
                            
                    }

                    string Subject = "Attendance Sheet of class " + class_name + " date " + DateTime.Now.Date.ToString("dd/MM/yyyy");

                    string body2 = @"<div style='margin:0;background-color:#f7f7f7'>

<table width='100%' bgcolor='#f7f7f7' cellpadding='0' cellspacing='0' border='0' style='border-collapse:collapse;border-spacing:0'>
  <tbody><tr>
    <td>
      
      <table bgcolor='#f7f7f7' width='600' cellpadding='0' cellspacing='0' border='0' align='center'  style='border-collapse:collapse;border-spacing:0;background-color:#f7f7f7'>
        <tbody><tr>
          <td style='padding:0px;margin:0px'><table bgcolor='#f7f7f7' cellpadding='0' cellspacing='0' border='0' align='center'  style='border-collapse:collapse;border-spacing:0'>
              <tbody><tr>
                <td><table bgcolor='#f7f7f7' width='551' cellpadding='0' cellspacing='0' border='0' align='center'  style='border-collapse:collapse;border-spacing:0'>
                    <tbody><tr>
                      <td style='padding:0px'><table width='100%' border='0' style='border-collapse:collapse;border-spacing:0'>
                          <tbody><tr>
                            <td style='padding:0px'><table align='left' width='100%' border='0' style='border-collapse:collapse;border-spacing:0'>
                                <tbody><tr>
                                  <td style='line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height='16'>&nbsp;</td>
                                </tr>
                                <tr>
                                  <td style='padding:0px;text-align:center'><img src='" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + @"/images/logo.jpg' width='50' height='50' alt='speaker'></td>
                                </tr>
                                <tr>
                                  <td style='line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height='16'>&nbsp;</td>
                                </tr>
                              </tbody></table></td>
                          </tr>
                        </tbody></table></td>
                    </tr>
                  </tbody></table></td>
              </tr>
            </tbody></table></td>
        </tr>
      </tbody></table>
      <table bgcolor='#2d1846' width='600' cellpadding='0' cellspacing='0' border='0' align='center' style='border-collapse:collapse;border-spacing:0;background-color:#2d1846'>
        <tbody><tr>
          <td style='padding:0px;margin:0px;border-bottom:4px solid #2d1846'><table bgcolor='#2d1846' cellpadding='0' cellspacing='0' border='0' align='center' style='border-collapse:collapse;border-spacing:0'>
              <tbody><tr>
                <td><table bgcolor='#2d1846' width='551' cellpadding='0' cellspacing='0' border='0' align='center' style='border-collapse:collapse;border-spacing:0'>
                    <tbody><tr>
                      <td style='padding:0px;vertical-align:top;text-align:center'><img style='max-width:200px;vertical-align:bottom' <img src='https://www.techgig.com/files/nicUploads/824168872795083.png' alt='logo'></td>
                    </tr>
                    <tr>
                      <td style = 'padding:0px' ><table width ='100%' border = '0' style = 'border-collapse:collapse;border-spacing:0' >
         
                                   <tbody><tr>
         
                                     <td style = 'padding:0px' ><table align = 'left' width = '100%' border = '0' style = 'border-collapse:collapse;border-spacing:0' >
                    
                                                    <tbody><tr>
                    
                                                      <td style = 'padding:0px;padding-left:10px' ><table width = '100%' border = '0' style = 'border-collapse:collapse;border-spacing:0'>
                             
                                                                   <tbody><tr>
                             
                                                                     <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height = '16' > &nbsp;</td>
                                    
                                                                          </tr>
                                    
                                                                          <tr>
                                    
                                                                            <td style = 'margin:0;padding:0;font-size:22px;text-align:center;color:#ffffff;line-height:28px;font-family:Helvetica,Arial,sans-serif;font-weight:bold;text-transform:uppercase'> " + Subject + @"</td>
                                      </tr>
                                      <tr>
                                        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height='4'>&nbsp;</td>
                                      </tr>
                                  
                                    </tbody></table></td>
                                </tr>
                                <tr>
                                  <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height='16'>&nbsp;</td>
                                </tr>
                              </tbody></table></td>
                          </tr>
                        </tbody></table></td>
                    </tr>
                  </tbody></table></td>
              </tr>
            </tbody></table></td>
        </tr>
      </tbody></table></td>
  </tr>
</tbody></table>

<table bgcolor = '#ffffff' width='600' cellpadding='0' cellspacing='0' border='0' align='center' style='border-collapse:collapse;border-spacing:0;background-color:#ffffff'>
    <tbody><tr>
  
  <td style = 'padding:0px;margin:0px' ><table bgcolor='#ffffff' cellpadding='0' cellspacing='0' border='0' align='center' style='border-collapse:collapse;border-spacing:0'>
      <tbody><tr>
        <td><table bgcolor = '#ffffff' width='551' cellpadding='0' cellspacing='0' border='0' align='center' style='border-collapse:collapse;border-spacing:0'>
                <tbody>
                    <tr>
                        <td style = 'padding:0px' >
                            <table width='100%' border='0' style='border-collapse:collapse;border-spacing:0'>
                                <tbody>
                                    <tr>
                                        <td style = 'padding:0px' >
                                            <table width='100%' border='0' style='border-collapse:collapse;border-spacing:0'>
                                                <tbody>
                                                    <tr>
                                                        <td style = 'padding:0px' ></td>
                                                    </tr>
                                                </tbody >
                                            </table >
                                            <table width='580' align='left' border='0' style='border-collapse:collapse;border-spacing:0'>

                                                <tbody>
                                                    <tr>
                                                        <td style = 'padding:0px' >
                                                            <table width='100%' border='0' style='border-collapse:collapse;border-spacing:0'>
                                                                <tbody>
                                                                    <tr>
                                                                        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height='20'>&nbsp;</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td colspan='5' style = 'margin:0;padding:0;font-size:16px;text-align:left;color:#4b4649;line-height:22px;font-family:Helvetica,Arial,sans-serif;font-weight:bold' > Following are the details of Absent students:</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height='12'>&nbsp;</td>
                                                                    </tr>
                                                                 "+body1+ @"
                                                                 </tbody>
 
                                                             </table>
 
                                                         </td>
 
                                                     </tr>
 


                                                     <tr>
 
                                                         <td style= 'padding:0px'>
 
                                                             <table width= '100%' border= '0' style= 'border-collapse:collapse;border-spacing:0' >
 
                                                                 <tbody>
 
                                                                     <tr>
 
                                                                         <td style= 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height= '24' > &nbsp;</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style = 'margin:0;padding:0;font-size:16px;text-align:left;color:#4b4649;line-height:21px;font-family:Helvetica,Arial,sans-serif;font-weight:bold' > Attendance Summary</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height='7'>&nbsp;</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style = 'border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;' > Total number of students Present</td>
                                                                        <td style = 'border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;' > "+ T_P_count_day + @" </ td >
   
                                                                       </tr>
   
                                                                       <tr>
   
                                                                           <td style= 'border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;' > Total number of students Absent</td>
                                                                        <td style = 'border-width:1px;border-style:solid;border-color:#ddd;padding-top:8px;padding-bottom:8px;padding-right:8px;padding-left:8px;' > " + T_A_count_day + @" </ td >
   
                                                                       </tr>
   
                                                                   <tbody>
   
                                                                       <tr>
   
                                                                           <td style= 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height= '24' > &nbsp;</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style = 'margin:0;padding:0;font-size:16px;text-align:left;color:#4b4649;line-height:21px;font-family:Helvetica,Arial,sans-serif;font-weight:bold' > Note: Please find the attached attendance sheet with this mail.</td>
                                                                    </tr>
                                                                    <tr>
                                                                        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height= '7' > &nbsp;</td>
                                                                    </tr>
                                                                </tbody>
                                                            </table>
                                                        </td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                            <table width = '165' align= 'right' border= '0' style= 'border-collapse:collapse;border-spacing:0' >
    
                                                    <tbody>
    
                                                        <tr>
    
                                                            <td style= 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height= '16' > &nbsp;</td>
                                                    </tr>



                                                    <tr>
                                                        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height= '16' > &nbsp;</td>
                                                    </tr>
                                                </tbody>
                                            </table>

                                        </td>

                                    </tr>
                                </tbody>
                            </table>
                    <tr>
                        <td align = 'center' >
    
                                <table style= 'border-collapse:collapse;border-spacing:0' width= '300' >
    
                                    <tbody>
    
                                        <tr>
    
                                            <td style= 'background-color:#d7263d;text-align:center;color:#ffffff;border-radius:3px;font-size:16px;text-decoration:none;font-weight:bold' ><a href='" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/attendance_register/finalize_class_attendance_sheet?section_id=" + section_id + "&session=" + session + "&att_date=" + att_date.ToString("yyyy-MM-dd") + @"' style= 'margin:0;padding:0px 3px 0px 3px;display:block;color:#ffffff;font-size:16px;line-height:18px;font-family:Arial,Helvetica,sans-serif;text-align:center;font-weight:bold;text-align:center;text-decoration:none;border:10px solid #d7263d;border-radius:3px' target= '_blank' > Yes! Finalize Attendance Sheet</a></td>
                                    </tr>
                                </tbody>
                            </table>
                        </td>
                    </tr>
</td>
            </tr>
          </tbody></table></td>

      </tr><tr>
        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height= '40' > &nbsp;</td>
      </tr>
          
      <tr>
        <td style = 'margin:0;color:#4b4649;font-size:14px;line-height:21px;font-family:Arial,Helvetica,sans-serif;font-style:normal;font-weight:normal;text-align:left' > Warm Regards,<br>

              Team ERP </td>
      </tr>
      <tr>
        <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:left' height= '16' > &nbsp;</td>
      </tr>
        
      
    </tbody>
</table>


<table bgcolor = '#f7f7f7' width= '600' cellpadding= '0' cellspacing= '0' border= '0' align= 'center' style= 'border-collapse:collapse;border-spacing:0;background-color:#f7f7f7' >
    
      <tbody><tr>
    
        <td style= 'padding:0px;margin:0px' ><table bgcolor= '#f7f7f7' cellpadding= '0' cellspacing= '0' border= '0' align= 'center' style= 'border-collapse:collapse;border-spacing:0' >
    
            <tbody><tr>
    
              <td><table bgcolor= '#f7f7f7' width= '100%' cellpadding= '0' cellspacing= '0' border= '0' align= 'center' style= 'border-collapse:collapse;border-spacing:0' >
    
                  <tbody><tr>
    
                    <td style= 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:center' height= '24' > &nbsp;</td>
              </tr>
              <tr>
                <td style = 'padding:0px' ><table align= 'left' width= '290' border= '0' style= 'border-collapse:collapse;border-spacing:0' >
    
                        <tbody><tr>
    
                          <td style= 'margin:0;padding:0;font-size:14px;text-align:left;color:#c2c2c2;line-height:18px;font-family:Arial,Helvetica,sans-serif;font-weight:normal' > " + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + @" </td>
                    </tr>
                    <tr>
                      <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:center' height= '4' > &nbsp;</td>
                    </tr>
                    <tr>
                      <td style = 'margin:0;padding:0;font-size:12px;text-align:left;color:#c2c2c2;line-height:18px;font-family:Arial,Helvetica,sans-serif;font-weight:normal' > "+Address+@" </td>
                        </tr>
                  </tbody></table>
                  <table align = 'right' width= '290' border= '0' style= 'border-collapse:collapse;border-spacing:0' >
    
                        <tbody><tr>
    
                          <td style= 'padding:0px' ><table width= '100%' border= '0' style= 'border-collapse:collapse;border-spacing:0' >
    
                              <tbody><tr>
    
                                <td width= '185' style= 'padding:0px' ><table width= '100%' border= '0' style= 'border-collapse:collapse;border-spacing:0' >
    
                                    <tbody>
    
                                    <tr>
    
                                      <td style= 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:center' height= '4' > &nbsp;</td>
                                </tr>
                                
                              </tbody></table></td>
							
                          </tr>
                        </tbody></table></td>
                    </tr>
                  </tbody></table></td>
              </tr>
              <tr>
                <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:center;border-bottom:1px solid #dadada' height= '30' > &nbsp;</td>
              </tr>
              <tr>
                <td style = 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:center' height= '7' > &nbsp;</td>
              </tr>
              <tr>
                <td style = 'margin:0;padding:0;font-size:12px;text-align:left;color:#c2c2c2;line-height:18px;font-family:Arial,Helvetica,sans-serif;font-weight:normal' > Note: For your privacy and protection, please do not forward this mail to anyone.To make sure this email is not sent to your 'junk/spam' folder, select the email and add the sender to your Address Book. <div style = 'display:none' >< img src='" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + @"/images/logo.jpg' style= 'display:none' ></ div ></ td >
    
                  </tr>
    
                  <tr>
    
                    <td style= 'line-height:0;font-size:0;vertical-align:top;padding:0px;text-align:center' height= '24' > &nbsp;</td>
              </tr>
            </tbody></table></td>
        </tr>
      </tbody></table></td>
  </tr>
</tbody></table><div></div></div>";


                    //body = body + @"<br><br><a href=""" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + @"/attendance_register/finalize_class_attendance_sheet?section_id="+section_id+"&session="+ session +"&att_date="+att_date.ToString("yyyy-MM-dd")+@""">Click Here </a>to finalize attendance sheet";

                    //body = body + "<br><br><a href='" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/attendance_register/finalize_class_attendance_sheet?section_id=" + section_id + "&session=" + session + "&att_date=" + att_date.ToString("yyyy-MM-dd") + "'><button class='button' style='background-color:#4CAF50;border-style:none;color:white;padding-top:15px;padding-bottom:15px;padding-right:32px;padding-left:32px;text-align:center;text-decoration:none;display:inline-block;font-size:16px;margin-top:4px;margin-bottom:4px;margin-right:2px;margin-left:2px;cursor:pointer;-webkit-transition-duration:0.4s;transition-duration:0.4s;box-shadow:0 8px 16px 0 rgba(0,0,0,0.2), 0 6px 20px 0 rgba(0,0,0,0.19);'>Finalize attendance sheet</button></a>";


                    _sendMail(ms, mail_id, name, class_name, body2);

                    //HttpContext.Current.Response.OutputStream.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    //HttpContext.Current.Response.OutputStream.Flush();
                    //HttpContext.Current.Response.OutputStream.Close();





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
        }


        private void _sendMail(Stream ms, string mail_id,string pdf_name,string class_name,string body)
        {
            string FromMail = donotreplyMail;
            string ToMail = mail_id;
            string Subject = "Attendance Sheet of class "+ class_name + " date "+DateTime.Now.Date.ToString("dd/MM/yyyy");
            string Body = body;
            string bcc;
            int bccNumber = Int16.Parse(ConfigurationManager.AppSettings["bccNumber"]);

            using (Attachment att =
              new Attachment(ms, pdf_name, MediaTypeNames.Application.Pdf))
            {
                using (MailMessage mm = new MailMessage(
                  FromMail, ToMail, Subject, Body))
                {
                    mm.Attachments.Add(att);
                    mm.IsBodyHtml = true;
                    for(int i=1;i<= bccNumber; i++)
                    {
                        bcc = ConfigurationManager.AppSettings["bcc"+i].ToString();
                        mm.Bcc.Add(bcc);
                    }
                    
                    SmtpClient smtp = new SmtpClient();
                    NetworkCredential networkCredential = new NetworkCredential(FromMail, donotreplyMailPassword);
                    smtp.Credentials = networkCredential;
                    smtp.EnableSsl = true;
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.Send(mm);
                }
            }
        }


        public List<DateTime> getAllDates(int year, int month)
        {
            var ret = new List<DateTime>();
            for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            {
                ret.Add(new DateTime(year, month, i));
            }
            return ret;
        }

        public class PDFFooter : PdfPageEventHelper
        {

            private PdfContentByte cb;
            private List<PdfTemplate> templates;

            public PDFFooter()
            {
                this.templates = new List<PdfTemplate>();
            }

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
                //PdfPTable tabFot = new PdfPTable(new float[] { 1F });
                //PdfPCell cell;
                //tabFot.TotalWidth = 850f;
                //Chunk text;
                //Phrase ph = new Phrase();


                //text = new Chunk("Nh-24 Village Ballia Post Dhaneta Teshil Meerganj Bareilly 243504. Ph.9058083211", FontFactory.GetFont("Areal", 12));
                //ph = new Phrase();
                //ph.Add(text);

                //ph.Add("\n");
                ////text = new Chunk("(Affiliated to CBSE New Delhi. Affiliation Number 2132182)", FontFactory.GetFont("Areal", 08));
                //text = new Chunk("Email: contact@hariti.in   Website: www.hariti.edu.in", FontFactory.GetFont("Areal", 12));
                //ph.Add(text);
                //cell = new PdfPCell(ph);
                ////cell = new PdfPCell(new Phrase("Nh-24 Village Ballia Post Dhaneta Teshil Meerganj Bareilly 243504. Ph.9058083211 /n Email: contact@hariti.in   Website: wwww.hariti.edu.in", FontFactory.GetFont("Areal", 8)));
                //cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                //cell.Border = Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                //cell.HorizontalAlignment = Element.ALIGN_CENTER;
                //cell.Padding = 5;
                //tabFot.AddCell(cell);
                //tabFot.WriteSelectedRows(0, -1, 0, document.Bottom, writer.DirectContent);



               

                cb = writer.DirectContentUnder;
                PdfTemplate templateM = cb.CreateTemplate(50, 50);
                templates.Add(templateM);

                int pageN = writer.CurrentPageNumber;
                String pageText = "Page " + pageN.ToString() + " of ";
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                float len = bf.GetWidthPoint(pageText, 10);
                cb.BeginText();
                cb.SetFontAndSize(bf, 10);
                cb.SetTextMatrix(750f, document.PageSize.GetBottom(document.BottomMargin)-10);
                cb.ShowText(pageText);
                cb.EndText();
                cb.AddTemplate(templateM, 750f + len, document.PageSize.GetBottom(document.BottomMargin)-10);

                //document.SetMargins(0f, 0f, 10f, 80f);

            }

            //write on close of document
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
                foreach (PdfTemplate item in templates)
                {
                    item.BeginText();
                    item.SetFontAndSize(bf, 10);
                    item.SetTextMatrix(0, 0);
                    item.ShowText("" + (writer.PageNumber));
                    item.EndText();
                }


            }
        }

    }
}