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
    public class repReport_cardMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();


        public void pdfReportCard( int class_id, int section_id, string session)
        {

            string query1 = @"SELECT concat(ifnull(b.class_name,''),' Section ',ifnull(a.section_name,'')) class_name FROM mst_section a,mst_class b 
                                where a.class_id = b.class_id
                                and a.section_id = @section_id";

            string class_name = con.Query<string>(query1, new { section_id = section_id }).SingleOrDefault();

             query1 = @"SELECT class_name FROM mst_class b 
                                where b.class_id = @class_id";

            string class_na = con.Query<string>(query1, new { class_id = class_id }).SingleOrDefault();

            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "Rep_Card_" + class_name + ".pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document();

            // MemoryStream stream = new MemoryStream();
           // doc.SetMargins(0f, 0f, 10f, 30f);
            try
            {
                string query = @"SELECT * FROM mst_scholastic_grades where session = @session order by from_marks desc";

                IEnumerable<mst_scholastic_grades> grades = con.Query<mst_scholastic_grades>(query, new { session = session });


                query = @"select a.sr_number sr_num, c.roll_number roll_no,concat(ifnull(a.std_first_name, ''), ' ', ifnull(std_last_name, '')) std_name,std_father_name,std_mother_name,std_dob,b.section_name,concat(ifnull(std_address,''),' ',ifnull(std_address1,''),' ',ifnull(std_address2,''),' ',ifnull(std_district,'')) address from sr_register a, mst_section b,mst_rollnumber c
                            where
                            a.std_section_id = b.section_id
                            and
                            b.section_id = @section_id
                            and
                            c.sr_num = a.sr_number
                            order by c.roll_number";


                IEnumerable<repReport_card> sr_list = con.Query<repReport_card>(query, new { section_id = section_id });




                IEnumerable<mst_term_rules> term;

                IEnumerable<mst_term> term_list;

                string query_term = @"SELECT * FROM mst_term_rules where class_id = @class_id and session = @session and term_id = @term_id";

                
                query = @"select distinct a.term_id,b.term_name from mst_term_rules a,mst_term b 
                            where 
                            a.class_id = @class_id
                            and
                            a.session = @session
                            and
                            a.term_id = b.term_id
                            and
                            a.session = b.session";

                term_list = con.Query<mst_term>(query, new { class_id = class_id, session = session });

               


                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();

                IEnumerable<mst_coscholastic_grades> coscholastic;

                IEnumerable<mst_discipline_grades> discipline;

                doc.Open();

                PdfPTable pt;

                foreach (var std in sr_list)
                {
                    pt = new PdfPTable(10);
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
                    text = new Chunk("("+Affiliation+")", FontFactory.GetFont("Areal", 12));
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
                    text = new Chunk("Academic Session: "+session, FontFactory.GetFont("Areal", 18));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 10;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = iTextSharp.text.Rectangle.TOP_BORDER;
                    _cell.PaddingBottom = 5;
                    //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Report Card for class " + class_na, FontFactory.GetFont("Areal", 12));
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
                    pt = new PdfPTable(5);

                    ph = new Phrase();
                    text = new Chunk("Name of Student", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(std.std_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Admission No.", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(std.sr_num.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Father's Name", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(std.std_father_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Roll No.", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(std.roll_no.ToString(), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Mother's Name", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(std.std_mother_name, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Date of Birth", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(std.std_dob.ToString("dd-MM-yyyy"), FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Address", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(std.address, FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 5;
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    doc.Add(pt);

                    string count = @"select count(*) from mst_term_rules a
                            where 
                            a.class_id = @class_id
                            and
                            a.session = @session";

                    int exam_count = con.Query<int>(count, new { class_id = class_id, session = session }).SingleOrDefault();

                   
                    pt = new PdfPTable(4 + (exam_count));

                    ph = new Phrase();
                    text = new Chunk("Scholastic Area", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    foreach (var list in term_list)
                    {

                        term = con.Query<mst_term_rules>(query_term, new { class_id = class_id, session = session, term_id = list.term_id });

                            ph = new Phrase();
                            text = new Chunk(list.term_name, FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            ph.Add("\n");
                            _cell = new PdfPCell(ph);
                             _cell.Colspan = exam_count/term_list.Count();
                            _cell.BackgroundColor = BaseColor.YELLOW;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);
                     
                    }

                    ph = new Phrase();
                    text = new Chunk("Summary", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    ph = new Phrase();
                    text = new Chunk("Subject Name", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    foreach (var list in term_list)
                    {
                        
                        term = con.Query<mst_term_rules>(query_term, new { class_id = class_id, session = session, term_id = list.term_id });

                        foreach (var scolastic in term)
                        {
                            ph = new Phrase();
                            text = new Chunk(scolastic.evaluation_name, FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            ph.Add("\n");
                            _cell = new PdfPCell(ph);
                            _cell.BackgroundColor = BaseColor.YELLOW;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);
                        }


                        
                       
                    }

                    string query_Convert_marks = @"SELECT sum(convert_to) FROM mst_term_rules a, mst_exam b where a.class_id = @class_id and a.session = @session and b.exam_id = a.exam_id1";

                    int sum_convert_marks = con.Query<int>(query_Convert_marks, new { class_id = class_id, session = session }).SingleOrDefault();

                    ph = new Phrase();
                    text = new Chunk("Total Marks ("+ sum_convert_marks + ")", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Grade", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    IEnumerable<mst_subject> subject;

                    query = @"SELECT a.subject_id,b.subject_name FROM mst_class_subject a, mst_subject b
                                    where
                                    a.subject_id = b.subject_id
                                    and
                                    a.session = b.session
                                    and
                                    a.session = @session
                                    and
                                    class_id = @class_id";

                    subject = con.Query<mst_subject>(query, new { class_id = class_id, session = session });

                    query = @"select round((marks/(select max_no from mst_exam where exam_id = a.exam_id )*(select convert_to from mst_exam where exam_id = a.exam_id )),1) from mst_exam_marks a
                            where 
                            session = @session
                            and 
                            subject_id = @subject_id
                            and
                            class_id = @class_id
                            and
                            section_id = @section_id
                            and
                            exam_id	 = @exam_id
                            and
                            sr_num = @sr_num";

                    string query_max_no = @"select convert_to from mst_exam where exam_id = @exam_id";

                    foreach (var sub_list in subject)
                    {
                        decimal total = 0;
                        decimal max = 0;

                        ph = new Phrase();
                        text = new Chunk(sub_list.subject_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        foreach(var list in term_list)
                        {
                            term = con.Query<mst_term_rules>(query_term, new { class_id = class_id, session = session, term_id = list.term_id });

                            foreach (var scolastic in term)
                            {


                                if (scolastic.rule == "Only")
                                {
                                    decimal marks = con.Query<decimal>(query, new { class_id = class_id, session = session, subject_id = sub_list.subject_id, section_id = section_id, exam_id = scolastic.exam_id1, sr_num = std.sr_num }).SingleOrDefault();
                                     max = max + con.Query<decimal>(query_max_no, new { exam_id = scolastic.exam_id1 }).SingleOrDefault();

                                    ph = new Phrase();
                                    text = new Chunk(marks.ToString(), FontFactory.GetFont("Areal", 8));
                                    ph.Add(text);
                                    ph.Add("\n");
                                    _cell = new PdfPCell(ph);
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    pt.AddCell(_cell);

                                    total = total + marks;

                                }
                                else if (scolastic.rule == "Maximum")
                                {
                                    decimal marks1 = con.Query<decimal>(query, new { class_id = class_id, session = session, subject_id = sub_list.subject_id, section_id = section_id, exam_id = scolastic.exam_id1, sr_num = std.sr_num }).SingleOrDefault();
                                    decimal marks2 = con.Query<decimal>(query, new { class_id = class_id, session = session, subject_id = sub_list.subject_id, section_id = section_id, exam_id = scolastic.exam_id2, sr_num = std.sr_num }).SingleOrDefault();

                                    max = max + con.Query<decimal>(query_max_no, new { exam_id = scolastic.exam_id1 }).SingleOrDefault();


                                    if (marks1 > marks2)
                                    {
                                        ph = new Phrase();
                                        text = new Chunk(marks1.ToString(), FontFactory.GetFont("Areal", 8));
                                        ph.Add(text);
                                        ph.Add("\n");
                                        _cell = new PdfPCell(ph);
                                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        pt.AddCell(_cell);

                                        total = total + marks1;

                                    }
                                    else
                                    {
                                        ph = new Phrase();
                                        text = new Chunk(marks2.ToString(), FontFactory.GetFont("Areal", 8));
                                        ph.Add(text);
                                        ph.Add("\n");
                                        _cell = new PdfPCell(ph);
                                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                        pt.AddCell(_cell);

                                        total = total + marks2;

                                    }

                                }

                            }

                        }

                        

                        ph = new Phrase();
                        decimal value = (Math.Round(total) / max) * 100;
                        text = new Chunk(Math.Round(total, 0).ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        foreach (var gr in grades)
                        {
                            if (gr.from_marks <= Math.Round(value, 0) && gr.to_marks >= Math.Round(value, 0))
                            {
                                
                                ph = new Phrase();
                                if (gr.grade == "E")
                                    text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8,BaseColor.RED));
                                else
                                    text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8,BaseColor.BLACK));
                                ph.Add(text);
                                ph.Add("\n");
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                                break;
                            }
                        }





                    }

                    doc.Add(pt);



                    string term_co = @"SELECT distinct a.term_id, b.term_name FROM mst_coscholastic_grades a, mst_term b
                                    where
                                    a.term_id = b.term_id
                                    and
                                    a.session = b.session
                                    and
                                    a.session = @session
                                    and
                                    a.term_id = b.term_id
                                    and
                                    class_id = @class_id";

                    IEnumerable<mst_term> mst_term = con.Query<mst_term>(term_co, new { class_id = class_id, session = session });


                    pt = new PdfPTable(3 + mst_term.Count());

                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 3 + mst_term.Count();
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Co-Scholastic Areas: [on a 3-point (A-C) grading scale]", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.Colspan = 3;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);


                    foreach(var dt in mst_term)
                    {
                        ph = new Phrase();
                        text = new Chunk( dt.term_name+" Grades", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.BackgroundColor = BaseColor.YELLOW;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                    }


                    string coschol = @"SELECT a.co_scholastic_id,b.co_scholastic_name FROM mst_class_coscholastic a,mst_co_scholastic b 
                                        where
                                        a.session = @session
                                        and 
                                        a.class_id = @class_id
                                        and
                                        a.co_scholastic_id = b.co_scholastic_id
                                        order by b.co_scholastic_name";

                    IEnumerable<mst_co_scholastic> scho = con.Query<mst_co_scholastic>(coschol, new { class_id = class_id, session = session});

                    query = @"select b.co_scholastic_name,a.grade from mst_coscholastic_grades a, mst_co_scholastic b 
                                where
                                sr_num = @sr_num
                                and term_id = @term_id
                                 and a.session = @session
                                and a.co_scholastic_id = b.co_scholastic_id
                                and a.co_scholastic_id  = @co_scholastic_id 
                                and a.session = b.session
                                order by b.co_scholastic_name";

                    

                    foreach (var dt in scho)
                    {

                        ph = new Phrase();
                        text = new Chunk(dt.co_scholastic_name, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        foreach(var dtt in mst_term)
                        {
                            coscholastic = con.Query<mst_coscholastic_grades>(query, new { sr_num = std.sr_num, session = session, co_scholastic_id = dt.co_scholastic_id,term_id = dtt.term_id });

                            if (coscholastic.Count() > 0)
                            {
                                foreach (var gr in coscholastic)
                                {

                                    ph = new Phrase();
                                    text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                    ph.Add(text);
                                    _cell = new PdfPCell(ph);
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    pt.AddCell(_cell);


                                }
                            }
                            else
                            {
                                ph = new Phrase();
                                text = new Chunk("", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                        }
                        

                    }

                    

                    doc.Add(pt);


                    term_co = @"SELECT distinct a.term_id, b.term_name FROM mst_discipline_grades a, mst_term b
                                    where
                                    a.term_id = b.term_id
                                    and
                                    a.session = b.session
                                    and
                                    a.session = @session
                                    and
                                    a.term_id = b.term_id
                                    and
                                    class_id = @class_id";

                    mst_term = con.Query<mst_term>(term_co, new { class_id = class_id, session = session });

                    pt = new PdfPTable(3 + mst_term.Count());

                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 3 + mst_term.Count();
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(" Discipline : [on a 3-point (A-C) grading scale]", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 3;
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                   

                    foreach (var dt in mst_term)
                    {
                        ph = new Phrase();
                        text = new Chunk(dt.term_name + " Grades", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.BackgroundColor = BaseColor.YELLOW;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                    }


                 


                    string desci = @"SELECT a.discipline_id,b.discipline_name FROM mst_class_discipline a,mst_discipline b 
                                        where
                                        a.session = @session
                                        and 
                                        a.class_id = @class_id
                                        and
                                        a.discipline_id = b.discipline_id
                                        order by b.discipline_name";

                    IEnumerable<mst_discipline> disci = con.Query<mst_discipline>(desci, new { class_id = class_id, session = session });

                    query = @"select b.discipline_name,a.grade from mst_discipline_grades a, mst_discipline b 
                            where 
                            sr_num = @sr_num 
                            and term_id = @term_id
                            and a.session = @session
                            and a.discipline_id = b.discipline_id
                            and a.discipline_id = @discipline_id
                            and a.session = b.session
                            order by b.discipline_name";



                    foreach (var dt in disci)
                    {

                        ph = new Phrase();
                        text = new Chunk(dt.discipline_name, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        foreach (var dtt in mst_term)
                        {
                            discipline = con.Query<mst_discipline_grades>(query, new { sr_num = std.sr_num, session = session, discipline_id = dt.discipline_id,term_id = dtt.term_id });

                            if (discipline.Count() > 0)
                            {
                                foreach (var gr in discipline)
                                {

                                    ph = new Phrase();
                                    text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                    ph.Add(text);
                                    _cell = new PdfPCell(ph);
                                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    pt.AddCell(_cell);


                                }
                            }
                            else
                            {
                                ph = new Phrase();
                                text = new Chunk("", FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                        }

                        

                    }


                    doc.Add(pt);

                    string remark_query = @"SELECT 
                                                remark
                                            FROM
                                                teacher_exam_remark
                                            WHERE
                                                term_id = @term_id AND class_id = @class_id
                                                    AND section_id = @section_id
                                                    AND sr_number = @sr_number
                                                    AND session = @session";

                    pt = new PdfPTable(8);

                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 8;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                   

                    foreach (var list in term_list)
                    {
                        string remark = con.Query<string>(remark_query, new { sr_number = std.sr_num, session = session, section_id = section_id, term_id = list.term_id, class_id = class_id }).SingleOrDefault();

                        ph = new Phrase();
                        text = new Chunk("Class Teacher Remark: ", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Border = Rectangle.NO_BORDER;
                        _cell.Colspan = 2;
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(remark, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Border = Rectangle.BOTTOM_BORDER;
                        _cell.Colspan = 6;
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        pt.AddCell(_cell);

                    }

                    doc.Add(pt);

                    pt = new PdfPTable(4);

                    ph = new Phrase();
                    text = new Chunk("\n\n\n\n", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.Colspan = 4;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Date: "+System.DateTime.Now.Date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 10));
                    ph.Add(text);
                    
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.BOTTOM_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Signature of Class Teacher", FontFactory.GetFont("Areal", 10));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.BOTTOM_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.BOTTOM_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Signature of Principal", FontFactory.GetFont("Areal", 10));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.BOTTOM_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Instructions", FontFactory.GetFont("Areal", 10));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("Grading scale for scholastic areas : Grades are awarded on a 8- point grading scale as follows –", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    _cell.Border = Rectangle.NO_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    doc.Add(pt);


                    pt = new PdfPTable(2);

                    pt.WidthPercentage = 40;

                    ph = new Phrase();
                    text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.Border = Rectangle.BOTTOM_BORDER;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("MARKS RANGE", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    ph = new Phrase();
                    text = new Chunk("GRADES", FontFactory.GetFont("Areal", 8));
                    ph.Add(text);
                    ph.Add("\n");
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.YELLOW;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    foreach (var gr in grades)
                    {

                        ph = new Phrase();
                        text = new Chunk(gr.from_marks + " - " + gr.to_marks, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        if (gr.grade == "E")
                        {
                            ph = new Phrase();
                            text = new Chunk(gr.grade + " (Needs improvement)", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            ph.Add("\n");
                            _cell = new PdfPCell(ph);
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);
                        }
                        else
                        {
                            ph = new Phrase();
                            text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            ph.Add("\n");
                            _cell = new PdfPCell(ph);
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);
                        }

                    }

                    doc.Add(pt);

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