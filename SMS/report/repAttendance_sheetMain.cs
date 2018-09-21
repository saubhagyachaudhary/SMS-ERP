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
        

        public void pdfAttendanceSheet(int section_id,int month_no, string session)
        {

            string query1 = @"SELECT concat(ifnull(b.class_name,''),' Section ',ifnull(a.section_name,'')) class_name FROM mst_section a,mst_class b 
                                where a.class_id = b.class_id
                                and a.section_id = @section_id";

            string class_name = con.Query<string>(query1, new { section_id = section_id}).SingleOrDefault();

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





                //         string query = @"select a.sr_number sr_num,a.roll_no,concat(ifnull(a.std_first_name,''),' ',ifnull(std_last_name,'')) std_name from sr_register a,mst_batch b
                //                     where 
                //                     a.std_batch_id = b.batch_id
                //                     and 
                //                     b.class_id = @class_id
                //order by a.std_first_name";

                string query = @"select a.sr_number sr_num, c.roll_number roll_no,concat(ifnull(a.std_first_name, ''), ' ', ifnull(std_last_name, '')) std_name from sr_register a, mst_section b,mst_rollnumber c
                            where
                            a.std_section_id = b.section_id
                            and
                            b.section_id = @section_id
                            and
                            c.sr_num = a.sr_number
                            order by c.roll_number";


                IEnumerable<repAttendance_sheet> sr_list = con.Query<repAttendance_sheet>(query, new { section_id = section_id});


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

                query = @"select case when a.attendance = 1 then 'P' else 'A' end attendance,sr_num,a.roll_no,day(att_date) day  from attendance_register a,sr_register b 
                                where a.section_id = @section_id 
                                and att_date between @startOfMonth and @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number";

                check1 = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth});

                query = @"select count(*) P_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date between @startOfMonth and @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 1
                                group by sr_num";

                P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                query = @"select count(*) A_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date between @startOfMonth and @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 0
                                group by sr_num";

                A_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                query = @"select count(*) P_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date <= @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 1
                                group by sr_num";

                T_P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                query = @"select count(*) A_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date <= @endOfMonth
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 0
                                group by sr_num";

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
                            _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
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
                string query1 = @"SELECT concat(ifnull(b.class_name,''),' Section ',ifnull(a.section_name,'')) class_name FROM mst_section a,mst_class b 
                                where a.class_id = b.class_id
                                and a.section_id = @section_id";

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


                    string query = @"select a.sr_number sr_num, c.roll_number roll_no,concat(ifnull(a.std_first_name, ''), ' ', ifnull(std_last_name, '')) std_name from sr_register a, mst_section b,mst_rollnumber c
                            where
                            a.std_section_id = b.section_id
                            and
                            b.section_id = @section_id
                            and
                            c.sr_num = a.sr_number
                            order by c.roll_number";


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

                    query = @"select case when a.attendance = 1 then 'P' else 'A' end attendance,sr_num,a.roll_no,day(att_date) day  from attendance_register a,sr_register b 
                                where a.section_id = @section_id 
                                and att_date between @startOfMonth and @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number";

                    check1 = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"select count(*) P_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date between @startOfMonth and @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 1
                                group by sr_num";

                    P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"select count(*) A_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date between @startOfMonth and @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 0
                                group by sr_num";

                    A_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"select count(*) P_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date <= @endOfMonth 
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 1
                                group by sr_num";

                    T_P_count = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                    query = @"select count(*) A_count, sr_num  from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date <= @endOfMonth
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 0
                                group by sr_num";

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

                    query = @"select count(*) A_count from attendance_register a, sr_register b
                                where a.section_id = @section_id
                                and att_date = date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 1";

                    int T_P_count_day = con.Query<int>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth }).SingleOrDefault();


                    query = @"select count(*) A_count from attendance_register a, sr_register b
                                where a.section_id = @section_id 
                                and att_date = date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 0";

                    int T_A_count_day = con.Query<int>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth }).SingleOrDefault();

                    query = @"select sr_num,concat(ifnull(std_first_name,''),' ',ifnull(std_last_name,'')) std_name,coalesce(std_contact,std_contact1,std_contact2) contact from attendance_register a, sr_register b
                                where a.section_id = @section_id
                                and att_date = date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))
                                and session = @session
                                and a.sr_num = b.sr_number
                                and a.attendance = 0";

                    IEnumerable<repAttendance_sheet> absent_details = con.Query<repAttendance_sheet>(query, new { section_id = section_id, session = session, startOfMonth = startOfMonth, endOfMonth = endOfMonth });


                    string body = "Total number of students Present " + T_P_count_day +"<br>" + "Total number of students Absent "+T_A_count_day ;


                    if (absent_details.Count() > 0)
                    {
                        body = body + "<br><br>" + "Following are the details of Absent students:" + "<br><br>";
                        string body1 = @"";
                        int serial = 0;
                        foreach (var details in absent_details)
                        {
                            query = @"select a.attendance from attendance_register a
                                where a.section_id = @section_id
                                and session = @session
                                and a.sr_num = @sr_num
                                order by att_date desc";

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

                            body1 = body1 + @" <tr style='border: 1px solid black'>
                                    <td style='border: 1px solid black' align='center'>" + serial + @"</td>
                                    <td style='border: 1px solid black' align='center'>" + details.sr_num + @"</td> 
                                    <td style='border: 1px solid black' align='center'>" + details.std_name + @"</td>
                                    <td style='border: 1px solid black' align='center'>" + details.contact + @"</td>
                                    <td style='border: 1px solid black' align='center'>" + serial_count + @"</td>
                                  </tr>";

                            // body1 = body1 + details.sr_num + " " + details.std_name + " " + details.contact + Environment.NewLine;
                        }

                        body1 = @"<table style='border: 1px solid black'>
                                  <tr style='border: 1px solid black'>
                                    <th style='border: 1px solid black' align='center'>Serial No.</th>
                                    <th style='border: 1px solid black' align='center'>Admission No.</th> 
                                    <th style='border: 1px solid black' align='center'>Student Name</th>
                                    <th style='border: 1px solid black' align='center'>Contact No</th> 
                                    <th style='border: 1px solid black' align='center'>Cont Absent Days</th> 
                                  </tr>" + body1 +
                                   "</table>";

                       
                             body = body + body1;
                    }
                   
                  
                    body = body + @"<br><br><a href=""" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + @"/attendance_register/finalize_class_attendance_sheet?section_id="+section_id+"&session="+ session +"&att_date="+att_date.ToString("yyyy-MM-dd")+@""">Click Here </a>to finalize attendance sheet";
                    _sendMail(ms, mail_id, name, class_name, body);

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