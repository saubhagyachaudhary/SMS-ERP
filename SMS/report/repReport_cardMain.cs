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
using System.Text;
using System.Web;

namespace SMS.report
{
    public class repReport_cardMain
    {
        
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();


        public void pdfReportCard(int class_id, int section_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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
                                        AND a.session = @session";

                string class_name = con.Query<string>(query1, new { section_id = section_id, session = session }).SingleOrDefault();

                query1 = @"SELECT 
                            class_name
                        FROM
                            mst_class b
                        WHERE
                            b.class_id = @class_id
                            and b.session = @session";

                string class_na = con.Query<string>(query1, new { class_id = class_id, session = session }).SingleOrDefault();

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
                    string query = @"SELECT 
                                        *
                                    FROM
                                        mst_scholastic_grades
                                    WHERE
                                        session = @session
                                    ORDER BY from_marks DESC";

                    IEnumerable<mst_scholastic_grades> grades = con.Query<mst_scholastic_grades>(query, new { session = session });


                    query = @"SELECT 
                                a.sr_number sr_num,
                                c.roll_number roll_no,
                                CONCAT(IFNULL(a.std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name,
                                std_father_name,
                                std_mother_name,
                                std_dob,
                                b.section_name,
                                CONCAT(IFNULL(std_address, ''),
                                        ' ',
                                        IFNULL(std_address1, ''),
                                        ' ',
                                        IFNULL(std_address2, ''),
                                        ' ',
                                        IFNULL(std_district, '')) address
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_rollnumber c,
                                mst_std_section d
                            WHERE
                                d.section_id = b.section_id
                                    AND d.section_id = @section_id
                                    AND c.sr_num = a.sr_number
                                    AND a.sr_number = d.sr_num
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = @session
                            ORDER BY c.roll_number";


                    IEnumerable<repReport_card> sr_list = con.Query<repReport_card>(query, new { section_id = section_id, session = session });




                    IEnumerable<mst_term_rules> term;

                    IEnumerable<mst_term> term_list;

                    string query_term = @"SELECT 
                                            *
                                        FROM
                                            mst_term_rules
                                        WHERE
                                            class_id = @class_id
                                                AND session = @session
                                                AND term_id = @term_id";


                    query = @"SELECT DISTINCT
                                a.term_id, b.term_name
                            FROM
                                mst_term_rules a,
                                mst_term b
                            WHERE
                                a.class_id = @class_id
                                    AND a.session = @session
                                    AND a.term_id = b.term_id
                                    AND a.session = b.session";

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
                        text = new Chunk("Academic Session: " + session, FontFactory.GetFont("Areal", 18));
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

                        string count = @"SELECT 
                                            COUNT(*)
                                        FROM
                                            mst_term_rules a
                                        WHERE
                                            a.class_id = @class_id
                                                AND a.session = @session";

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
                            _cell.Colspan = term.Count();//exam_count/term_list.Count();
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

                        string query_Convert_marks = @"SELECT 
                                                        SUM(convert_to)
                                                    FROM
                                                        mst_term_rules a,
                                                        mst_exam b
                                                    WHERE
                                                        a.class_id = @class_id
                                                            AND a.session = @session
                                                            AND b.exam_id = a.exam_id1
                                                            and a.session = b.session";

                        int sum_convert_marks = con.Query<int>(query_Convert_marks, new { class_id = class_id, session = session }).SingleOrDefault();

                        ph = new Phrase();
                        text = new Chunk("Total Marks (" + sum_convert_marks + ")", FontFactory.GetFont("Areal", 8));
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

                        query = @"SELECT 
                                    a.subject_id, b.subject_name
                                FROM
                                    mst_class_subject a,
                                    mst_subject b
                                WHERE
                                    a.subject_id = b.subject_id
                                        AND a.session = b.session
                                        AND a.session = @session
                                        AND class_id = @class_id";

                        subject = con.Query<mst_subject>(query, new { class_id = class_id, session = session });

                        query = @"SELECT 
                                    ROUND((marks / (SELECT 
                                                    max_no
                                                FROM
                                                    mst_exam
                                                WHERE
                                                    exam_id = a.exam_id AND session = a.session) * (SELECT 
                                                    convert_to
                                                FROM
                                                    mst_exam
                                                WHERE
                                                    exam_id = a.exam_id AND session = a.session)),
                                            1)
                                FROM
                                    mst_exam_marks a,
                                    mst_std_class b,
                                    mst_std_section c
                                WHERE
                                    a.session = @session
                                        AND a.session = b.session
                                        AND b.session = c.session
                                        AND a.sr_num = b.sr_num
                                        AND b.sr_num = c.sr_num
                                        AND a.subject_id = @subject_id
                                        AND b.class_id = @class_id
                                        AND c.section_id = @section_id
                                        AND a.exam_id = @exam_id
                                        AND a.sr_num = @sr_num";

                        string query_max_no = @"SELECT 
                                                convert_to
                                            FROM
                                                mst_exam
                                            WHERE
                                                exam_id = @exam_id
                                                    AND session = @session";

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

                            foreach (var list in term_list)
                            {
                                term = con.Query<mst_term_rules>(query_term, new { class_id = class_id, session = session, term_id = list.term_id });

                                foreach (var scolastic in term)
                                {


                                    if (scolastic.rule == "Only")
                                    {
                                        decimal marks = con.Query<decimal>(query, new { class_id = class_id, session = session, subject_id = sub_list.subject_id, section_id = section_id, exam_id = scolastic.exam_id1, sr_num = std.sr_num }).SingleOrDefault();
                                        max = max + con.Query<decimal>(query_max_no, new { exam_id = scolastic.exam_id1, session = session }).SingleOrDefault();

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

                                        max = max + con.Query<decimal>(query_max_no, new { exam_id = scolastic.exam_id1, session = session }).SingleOrDefault();


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
                                        text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                    else
                                        text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
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



                        string term_co = @"SELECT DISTINCT
                                            a.term_id, b.term_name
                                        FROM
                                            mst_coscholastic_grades a,
                                            mst_term b,
                                            mst_std_class c,
                                            mst_std_section d
                                        WHERE
                                            a.term_id = b.term_id
                                                AND a.session = b.session
                                                AND b.session = c.session
                                                AND c.session = d.session
                                                AND a.session = @session
                                                AND a.sr_num = c.sr_num
                                                AND c.sr_num = d.sr_num
                                                AND a.term_id = b.term_id
                                                AND c.class_id = @class_id";

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


                        string coschol = @"SELECT 
                                            a.co_scholastic_id, b.co_scholastic_name
                                        FROM
                                            mst_class_coscholastic a,
                                            mst_co_scholastic b
                                        WHERE
                                            a.session = @session
                                                AND a.session = b.session
                                                AND a.class_id = @class_id
                                                AND a.co_scholastic_id = b.co_scholastic_id
                                        ORDER BY b.co_scholastic_name";

                        IEnumerable<mst_co_scholastic> scho = con.Query<mst_co_scholastic>(coschol, new { class_id = class_id, session = session });

                        query = @"SELECT 
                                    b.co_scholastic_name, a.grade
                                FROM
                                    mst_coscholastic_grades a,
                                    mst_co_scholastic b
                                WHERE
                                    sr_num = @sr_num AND term_id = @term_id
                                        AND a.session = @session
                                        AND a.co_scholastic_id = b.co_scholastic_id
                                        AND a.co_scholastic_id = @co_scholastic_id
                                        AND a.session = b.session
                                ORDER BY b.co_scholastic_name";



                        foreach (var dt in scho)
                        {

                            ph = new Phrase();
                            text = new Chunk(dt.co_scholastic_name, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 3;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            foreach (var dtt in mst_term)
                            {
                                coscholastic = con.Query<mst_coscholastic_grades>(query, new { sr_num = std.sr_num, session = session, co_scholastic_id = dt.co_scholastic_id, term_id = dtt.term_id });

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


                        term_co = @"SELECT DISTINCT
                                    a.term_id, b.term_name
                                FROM
                                    mst_discipline_grades a,
                                    mst_term b,
                                    mst_std_class c
                                WHERE
                                    a.term_id = b.term_id
                                        AND a.session = b.session
                                        AND b.session = c.session
                                        AND a.session = @session
                                        AND a.sr_num = c.sr_num
                                        AND a.term_id = b.term_id
                                        AND c.class_id = @class_id";

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





                        string desci = @"SELECT 
                                            a.discipline_id, b.discipline_name
                                        FROM
                                            mst_class_discipline a,
                                            mst_discipline b
                                        WHERE
                                            a.session = @session
                                                AND a.session = b.session
                                                AND a.class_id = @class_id
                                                AND a.discipline_id = b.discipline_id
                                        ORDER BY b.discipline_name";

                        IEnumerable<mst_discipline> disci = con.Query<mst_discipline>(desci, new { class_id = class_id, session = session });

                        query = @"SELECT 
                                    b.discipline_name, a.grade
                                FROM
                                    mst_discipline_grades a,
                                    mst_discipline b
                                WHERE
                                    sr_num = @sr_num AND term_id = @term_id
                                        AND a.session = @session
                                        AND a.discipline_id = b.discipline_id
                                        AND a.discipline_id = @discipline_id
                                        AND a.session = b.session
                                ORDER BY b.discipline_name";



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
                                discipline = con.Query<mst_discipline_grades>(query, new { sr_num = std.sr_num, session = session, discipline_id = dt.discipline_id, term_id = dtt.term_id });

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
                                                teacher_exam_remark a,
                                                mst_std_section b,
                                                mst_std_class c
                                            WHERE
                                                term_id = @term_id
                                                    AND c.class_id = @class_id
                                                    AND b.section_id = @section_id
                                                    AND a.sr_number = @sr_number
                                                    AND a.sr_number = b.sr_num
                                                    AND b.sr_num = c.sr_num
                                                    AND a.session = @session
                                                    AND a.session = b.session";

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
                            text = new Chunk(list.term_name + " Remark: ", FontFactory.GetFont("Areal", 8));
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

                            ph = new Phrase();
                            text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 8;
                            _cell.Border = Rectangle.NO_BORDER;
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
                        text = new Chunk("Date: " + System.DateTime.Now.Date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 10));
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
        }

        public byte[] WebsiteReportCard(int sr_number, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                class_id
                            FROM
                                mst_std_class
                            WHERE
                                sr_num = @sr_number
                                    AND session = @session";
                int class_id = con.Query<int>(query, new { sr_number = sr_number, session = session }).SingleOrDefault();

                query = @"SELECT 
                                section_id
                            FROM
                                mst_std_section
                            WHERE
                                sr_num = @sr_number
                                    AND session = @session";
                int section_id = con.Query<int>(query, new { sr_number = sr_number, session = session }).SingleOrDefault();


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
                                        AND a.session = @session";

                string class_name = con.Query<string>(query1, new { section_id = section_id, session = session }).SingleOrDefault();

                query1 = @"SELECT 
                            class_name
                        FROM
                            mst_class b
                        WHERE
                            b.class_id = @class_id
                            and b.session = @session";

                string class_na = con.Query<string>(query1, new { class_id = class_id, session = session }).SingleOrDefault();

                using (MemoryStream ms = new MemoryStream())
                {


                    using (Document doc = new Document())
                    {



                        // MemoryStream stream = new MemoryStream();
                        // doc.SetMargins(0f, 0f, 10f, 30f);
                        try
                        {
                            query = @"SELECT 
                                        *
                                    FROM
                                        mst_scholastic_grades
                                    WHERE
                                        session = @session
                                    ORDER BY from_marks DESC";

                            IEnumerable<mst_scholastic_grades> grades = con.Query<mst_scholastic_grades>(query, new { session = session });


                            query = @"SELECT 
                                a.sr_number sr_num,
                                c.roll_number roll_no,
                                CONCAT(IFNULL(a.std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name,
                                std_father_name,
                                std_mother_name,
                                std_dob,
                                b.section_name,
                                CONCAT(IFNULL(std_address, ''),
                                        ' ',
                                        IFNULL(std_address1, ''),
                                        ' ',
                                        IFNULL(std_address2, ''),
                                        ' ',
                                        IFNULL(std_district, '')) address
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_rollnumber c,
                                mst_std_section d
                            WHERE
                                d.section_id = b.section_id
                                    AND c.sr_num = a.sr_number
                                    AND a.sr_number = d.sr_num
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = @session
                                    and a.sr_number = @sr_number
                            ORDER BY c.roll_number";


                            repReport_card sr_list = con.Query<repReport_card>(query, new { sr_number = sr_number, session = session }).SingleOrDefault();




                            IEnumerable<mst_term_rules> term;

                            IEnumerable<mst_term> term_list;

                            string query_term = @"SELECT 
                                            *
                                        FROM
                                            mst_term_rules
                                        WHERE
                                            class_id = @class_id
                                                AND session = @session
                                                AND term_id = @term_id";


                            query = @"SELECT DISTINCT
                                a.term_id, b.term_name
                            FROM
                                mst_term_rules a,
                                mst_term b
                            WHERE
                                a.class_id = @class_id
                                    AND a.session = @session
                                    AND a.term_id = b.term_id
                                    AND a.session = b.session";

                            term_list = con.Query<mst_term>(query, new { class_id = class_id, session = session });



                            PdfWriter writer = PdfWriter.GetInstance(doc, ms);

                            writer.PageEvent = new PDFFooter();

                            string password = sr_list.std_dob.ToString("ddMMyyyy");

                            byte[] USER = Encoding.ASCII.GetBytes(password);

                            byte[] OWNER = Encoding.ASCII.GetBytes("@sau4651@");

                            writer.SetEncryption(USER, OWNER, PdfWriter.AllowPrinting, PdfWriter.ENCRYPTION_AES_128);

                            IEnumerable<mst_coscholastic_grades> coscholastic;

                            IEnumerable<mst_discipline_grades> discipline;

                            doc.Open();

                            PdfPTable pt;


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
                            text = new Chunk("Academic Session: " + session, FontFactory.GetFont("Areal", 18));
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
                            text = new Chunk(sr_list.std_name, FontFactory.GetFont("Areal", 8));
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
                            text = new Chunk(sr_list.sr_num.ToString(), FontFactory.GetFont("Areal", 8));
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
                            text = new Chunk(sr_list.std_father_name, FontFactory.GetFont("Areal", 8));
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
                            text = new Chunk(sr_list.roll_no.ToString(), FontFactory.GetFont("Areal", 8));
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
                            text = new Chunk(sr_list.std_mother_name, FontFactory.GetFont("Areal", 8));
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
                            text = new Chunk(sr_list.std_dob.ToString("dd-MM-yyyy"), FontFactory.GetFont("Areal", 8));
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
                            text = new Chunk(sr_list.address, FontFactory.GetFont("Areal", 8));
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

                            string count = @"SELECT 
                                            COUNT(*)
                                        FROM
                                            mst_term_rules a
                                        WHERE
                                            a.class_id = @class_id
                                                AND a.session = @session";

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
                                _cell.Colspan = term.Count();//exam_count/term_list.Count();
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

                            string query_Convert_marks = @"SELECT 
                                                        SUM(convert_to)
                                                    FROM
                                                        mst_term_rules a,
                                                        mst_exam b
                                                    WHERE
                                                        a.class_id = @class_id
                                                            AND a.session = @session
                                                            AND b.exam_id = a.exam_id1
                                                            and a.session = b.session";

                            int sum_convert_marks = con.Query<int>(query_Convert_marks, new { class_id = class_id, session = session }).SingleOrDefault();

                            ph = new Phrase();
                            text = new Chunk("Total Marks (" + sum_convert_marks + ")", FontFactory.GetFont("Areal", 8));
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

                            query = @"SELECT 
                                    a.subject_id, b.subject_name
                                FROM
                                    mst_class_subject a,
                                    mst_subject b
                                WHERE
                                    a.subject_id = b.subject_id
                                        AND a.session = b.session
                                        AND a.session = @session
                                        AND class_id = @class_id";

                            subject = con.Query<mst_subject>(query, new { class_id = class_id, session = session });

                            query = @"SELECT 
                                    ROUND((marks / (SELECT 
                                                    max_no
                                                FROM
                                                    mst_exam
                                                WHERE
                                                    exam_id = a.exam_id) * (SELECT 
                                                    convert_to
                                                FROM
                                                    mst_exam
                                                WHERE
                                                    exam_id = a.exam_id)),
                                            1)
                                FROM
                                    mst_exam_marks a,
                                    mst_std_class b,
                                    mst_std_section c
                                WHERE
                                    a.session = @session
                                        AND a.session = b.session
                                        AND b.session = c.session
                                        AND a.sr_num = b.sr_num
                                        AND b.sr_num = c.sr_num
                                        AND a.subject_id = @subject_id
                                        AND b.class_id = @class_id
                                        AND c.section_id = @section_id
                                        AND a.exam_id = @exam_id
                                        AND a.sr_num = @sr_num";

                            string query_max_no = @"SELECT 
                                                convert_to
                                            FROM
                                                mst_exam
                                            WHERE
                                                exam_id = @exam_id
                                                    AND session = @session";

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

                                foreach (var list in term_list)
                                {
                                    term = con.Query<mst_term_rules>(query_term, new { class_id = class_id, session = session, term_id = list.term_id });

                                    foreach (var scolastic in term)
                                    {


                                        if (scolastic.rule == "Only")
                                        {
                                            decimal marks = con.Query<decimal>(query, new { class_id = class_id, session = session, subject_id = sub_list.subject_id, section_id = section_id, exam_id = scolastic.exam_id1, sr_num = sr_list.sr_num }).SingleOrDefault();
                                            max = max + con.Query<decimal>(query_max_no, new { exam_id = scolastic.exam_id1, session = session }).SingleOrDefault();

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
                                            decimal marks1 = con.Query<decimal>(query, new { class_id = class_id, session = session, subject_id = sub_list.subject_id, section_id = section_id, exam_id = scolastic.exam_id1, sr_num = sr_list.sr_num }).SingleOrDefault();
                                            decimal marks2 = con.Query<decimal>(query, new { class_id = class_id, session = session, subject_id = sub_list.subject_id, section_id = section_id, exam_id = scolastic.exam_id2, sr_num = sr_list.sr_num }).SingleOrDefault();

                                            max = max + con.Query<decimal>(query_max_no, new { exam_id = scolastic.exam_id1, session = session }).SingleOrDefault();


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
                                            text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8, BaseColor.RED));
                                        else
                                            text = new Chunk(gr.grade, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
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



                            string term_co = @"SELECT DISTINCT
                                            a.term_id, b.term_name
                                        FROM
                                            mst_coscholastic_grades a,
                                            mst_term b,
                                            mst_std_class c,
                                            mst_std_section d
                                        WHERE
                                            a.term_id = b.term_id
                                                AND a.session = b.session
                                                AND b.session = c.session
                                                AND c.session = d.session
                                                AND a.session = @session
                                                AND a.sr_num = c.sr_num
                                                AND c.sr_num = d.sr_num
                                                AND a.term_id = b.term_id
                                                AND c.class_id = @class_id";

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


                            string coschol = @"SELECT 
                                            a.co_scholastic_id, b.co_scholastic_name
                                        FROM
                                            mst_class_coscholastic a,
                                            mst_co_scholastic b
                                        WHERE
                                            a.session = @session
                                                AND a.session = b.session
                                                AND a.class_id = @class_id
                                                AND a.co_scholastic_id = b.co_scholastic_id
                                        ORDER BY b.co_scholastic_name";

                            IEnumerable<mst_co_scholastic> scho = con.Query<mst_co_scholastic>(coschol, new { class_id = class_id, session = session });

                            query = @"SELECT 
                                    b.co_scholastic_name, a.grade
                                FROM
                                    mst_coscholastic_grades a,
                                    mst_co_scholastic b
                                WHERE
                                    sr_num = @sr_num AND term_id = @term_id
                                        AND a.session = @session
                                        AND a.co_scholastic_id = b.co_scholastic_id
                                        AND a.co_scholastic_id = @co_scholastic_id
                                        AND a.session = b.session
                                ORDER BY b.co_scholastic_name";



                            foreach (var dt in scho)
                            {

                                ph = new Phrase();
                                text = new Chunk(dt.co_scholastic_name, FontFactory.GetFont("Areal", 8, BaseColor.BLACK));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.Colspan = 3;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                foreach (var dtt in mst_term)
                                {
                                    coscholastic = con.Query<mst_coscholastic_grades>(query, new { sr_num = sr_list.sr_num, session = session, co_scholastic_id = dt.co_scholastic_id, term_id = dtt.term_id });

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


                            term_co = @"SELECT DISTINCT
                                    a.term_id, b.term_name
                                FROM
                                    mst_discipline_grades a,
                                    mst_term b,
                                    mst_std_class c
                                WHERE
                                    a.term_id = b.term_id
                                        AND a.session = b.session
                                        AND b.session = c.session
                                        AND a.session = @session
                                        AND a.sr_num = c.sr_num
                                        AND a.term_id = b.term_id
                                        AND c.class_id = @class_id";

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





                            string desci = @"SELECT 
                                            a.discipline_id, b.discipline_name
                                        FROM
                                            mst_class_discipline a,
                                            mst_discipline b
                                        WHERE
                                            a.session = @session
                                                AND a.session = b.session
                                                AND a.class_id = @class_id
                                                AND a.discipline_id = b.discipline_id
                                        ORDER BY b.discipline_name";

                            IEnumerable<mst_discipline> disci = con.Query<mst_discipline>(desci, new { class_id = class_id, session = session });

                            query = @"SELECT 
                                    b.discipline_name, a.grade
                                FROM
                                    mst_discipline_grades a,
                                    mst_discipline b
                                WHERE
                                    sr_num = @sr_num AND term_id = @term_id
                                        AND a.session = @session
                                        AND a.discipline_id = b.discipline_id
                                        AND a.discipline_id = @discipline_id
                                        AND a.session = b.session
                                ORDER BY b.discipline_name";



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
                                    discipline = con.Query<mst_discipline_grades>(query, new { sr_num = sr_list.sr_num, session = session, discipline_id = dt.discipline_id, term_id = dtt.term_id });

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
                                                teacher_exam_remark a,
                                                mst_std_section b,
                                                mst_std_class c
                                            WHERE
                                                term_id = @term_id
                                                    AND c.class_id = @class_id
                                                    AND b.section_id = @section_id
                                                    AND a.sr_number = @sr_number
                                                    AND a.sr_number = b.sr_num
                                                    AND b.sr_num = c.sr_num
                                                    AND a.session = @session
                                                    AND a.session = b.session";

                            pt = new PdfPTable(8);

                            ph = new Phrase();
                            text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Border = Rectangle.NO_BORDER;
                            _cell.Colspan = 8;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);


                            // Teacher remark is commented for website code is working fine.

                            //foreach (var list in term_list)
                            //{
                            //    string remark = con.Query<string>(remark_query, new { sr_number = sr_list.sr_num, session = session, section_id = section_id, term_id = list.term_id, class_id = class_id }).SingleOrDefault();

                            //    ph = new Phrase();
                            //    text = new Chunk(list.term_name + " Remark: ", FontFactory.GetFont("Areal", 8));
                            //    ph.Add(text);
                            //    _cell = new PdfPCell(ph);
                            //    _cell.Border = Rectangle.NO_BORDER;
                            //    _cell.Colspan = 2;
                            //    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            //    pt.AddCell(_cell);

                            //    ph = new Phrase();
                            //    text = new Chunk(remark, FontFactory.GetFont("Areal", 8));
                            //    ph.Add(text);
                            //    _cell = new PdfPCell(ph);
                            //    _cell.Border = Rectangle.BOTTOM_BORDER;
                            //    _cell.Colspan = 6;
                            //    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            //    pt.AddCell(_cell);

                            //    ph = new Phrase();
                            //    text = new Chunk("\n", FontFactory.GetFont("Areal", 8));
                            //    ph.Add(text);
                            //    _cell = new PdfPCell(ph);
                            //    _cell.Colspan = 8;
                            //    _cell.Border = Rectangle.NO_BORDER;
                            //    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            //    pt.AddCell(_cell);

                            //}

                            doc.Add(pt);

                            pt = new PdfPTable(4);

                            ph = new Phrase();
                            text = new Chunk("\n\n", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Border = Rectangle.NO_BORDER;
                            _cell.Colspan = 4;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("Note: This is a provisional Report Card", FontFactory.GetFont("Areal", 10, BaseColor.RED));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 4;
                            _cell.Border = Rectangle.BOTTOM_BORDER;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
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



                        }

                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                    return ms.ToArray();
                }
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