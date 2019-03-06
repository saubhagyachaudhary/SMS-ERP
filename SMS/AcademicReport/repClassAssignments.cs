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

namespace SMS.AcademicReport
{
    public class class_subject
    {
        public int subject_id { get; set; }

        public string subject_name { get; set; }

        public string teacher_name { get; set; }
    }

    public class mst_class_notebook
    {
        
        public int subject_id { get; set; }
        public string work_type { get; set; }
        public int day { get; set; }
    }

    

    public class repClassAssignments
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public void pdfClassAssignment(int class_id, int section_id, int month_no, string session)
        {

            using (MemoryStream ms = new MemoryStream())
            {


                using (Document doc = new Document(PageSize.A4.Rotate()))
                {

                    string query = @"SELECT 
                                            b.subject_name,
                                            a.subject_id,
                                            CONCAT(IFNULL(c.FirstName, ''),
                                                    ' ',
                                                    IFNULL(c.LastName, '')) teacher_name
                                        FROM
                                            mst_class_subject_teacher a,
                                            mst_subject b,
                                            emp_profile c
                                        WHERE
                                            a.session = b.session
                                                AND a.subject_id = b.subject_id
                                                AND a.class_id = @class_id
                                                AND a.session = @session
                                                AND a.section_id = @section_id
                                                AND a.subject_teacher_id = c.user_id";

                    IEnumerable<class_subject> class_subject = con.Query<class_subject>(query, new { class_id = class_id, session = session, section_id = section_id });

                    query = @"SELECT 
                                    CONCAT('Class: ',
                                            class_name,
                                            '  ',
                                            'Section: ',
                                            section_name) class_name
                                FROM
                                    mst_section a,
                                    mst_class b
                                WHERE
                                    a.session = b.session
                                        AND a.class_id = b.class_id
                                        AND a.session = @session
                                        AND b.class_id = @class_id
                                        AND a.section_id = @section_id";

                    string class_name = con.Query<string>(query, new { class_id = class_id, session = session, section_id = section_id }).SingleOrDefault();

                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                    // writer.PageEvent = new PDFFooter();
                    doc.Open();
                    PdfPTable pt = new PdfPTable(10);
                    pt.WidthPercentage = 90;
                    // string imageURL = "E:\\HPS\\logo.jpg";
                    string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                    Image jpg = Image.GetInstance(imageURL);
                    jpg.ScaleAbsolute(60f, 60f);

                    imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/cbse.png");
                    Image cbse = Image.GetInstance(imageURL);
                    cbse.ScaleAbsolute(60f, 60f);





                    PdfPCell _cell;
                    Chunk text;
                    Phrase ph;
                    _cell = new PdfPCell(cbse);
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

                    ph = new Phrase();
                    _cell = new PdfPCell(jpg);
                    _cell.Border = 0;
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.PaddingBottom = 5;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 12));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
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

                    ph = new Phrase();
                    text = new Chunk("Date-wise Assignment Chart for the Month of "+ CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month_no) + " " + year.ToString() + " " + class_name, FontFactory.GetFont("Areal", 15));
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

                    var date_list = getAllDates(year, month_no);

                    pt = new PdfPTable(date_list.Count() + 12);
                    pt.WidthPercentage = 95f;
                    doc.Add(pt);

                    var startOfMonth = new DateTime(year, month_no, 1);
                    var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

                    query = @"SELECT 
                                    subject_id,work_type,DAY(work_date) day
                                FROM
                                    mst_class_notebook
                                WHERE
                                    class_id = @class_id AND session = @session
                                        AND work_date BETWEEN @startOfMonth AND @endOfMonth
                                        AND section_id = @section_id";

                    IEnumerable<mst_class_notebook> CW_HW = con.Query<mst_class_notebook>(query, new { session = session, class_id = class_id,section_id = section_id, startOfMonth = startOfMonth, endOfMonth = endOfMonth });

                   

                    foreach (class_subject dtt in class_subject)
                    {
                       

                        ph = new Phrase();
                        text = new Chunk("Teacher Name", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 5;
                        _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk("Subject Name", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 4;
                        _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk("W.Type", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.VerticalAlignment = Element.ALIGN_MIDDLE;

                        pt.AddCell(_cell);

                        for (int i = 0; i <= date_list.Count() - 1; i++)
                        {

                            ph = new Phrase();
                            text = new Chunk(date_list[i].Day.ToString(), FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            ph.Add("\n");
                            text = new Chunk(date_list[i].DayOfWeek.ToString().Substring(0, 1), FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                        }

                        ph = new Phrase();
                        text = new Chunk(dtt.teacher_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 5;
                        _cell.Rowspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(dtt.subject_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 4;
                        _cell.Rowspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk("C.W", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        for (int i = 0; i <= date_list.Count() - 1; i++)
                        {

                            var check = (from e in CW_HW where e.day == i + 1 where e.subject_id == dtt.subject_id where e.work_type == "CW" select e).ToList();



                            if (check.Count() > 0)
                            {


                                ph = new Phrase();

                                if (System.DateTime.Now >= startOfMonth && System.DateTime.Now <= endOfMonth)
                                {
                                    if (i <= System.DateTime.Now.Day - 1)
                                        text = new Chunk("Y", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                    else
                                        text = new Chunk("", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                }
                                else
                                {
                                    text = new Chunk("Y", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                }
                               

                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }
                            else
                            {
                                ph = new Phrase();
                                if (System.DateTime.Now >= startOfMonth && System.DateTime.Now <= endOfMonth)
                                {
                                    if (i <= System.DateTime.Now.Day - 1)
                                        text = new Chunk("N", FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                    else
                                        text = new Chunk("", FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                }
                                else
                                {
                                    text = new Chunk("N", FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                }
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }


                        }

                        ph = new Phrase();
                        text = new Chunk("H.W", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        for (int i = 0; i <= date_list.Count() - 1; i++)
                        {

                            var check = (from e in CW_HW where e.day == i + 1 where e.subject_id == dtt.subject_id where e.work_type == "HW" select e).ToList();



                            if (check.Count() > 0)
                            {


                                ph = new Phrase();

                                if (System.DateTime.Now >= startOfMonth && System.DateTime.Now <= endOfMonth)
                                {
                                    if (i <= System.DateTime.Now.Day - 1)
                                        text = new Chunk("Y", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                    else
                                        text = new Chunk("", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                }
                                else
                                {
                                    text = new Chunk("Y", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                }

                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }
                            else
                            {
                                ph = new Phrase();

                                if (System.DateTime.Now >= startOfMonth && System.DateTime.Now <= endOfMonth)
                                {
                                    if (i <= System.DateTime.Now.Day - 1)
                                        text = new Chunk("N", FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                    else
                                        text = new Chunk("", FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                }
                                else
                                {
                                    text = new Chunk("N", FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                }

                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }


                        }

                        ph = new Phrase();
                        text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = date_list.Count() + 12;
                        _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);
                    }
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

        public List<DateTime> getAllDates(int year, int month)
        {
            var ret = new List<DateTime>();
            for (int i = 1; i <= DateTime.DaysInMonth(year, month); i++)
            {
                ret.Add(new DateTime(year, month, i));
            }
            return ret;
        }


      
    }
}