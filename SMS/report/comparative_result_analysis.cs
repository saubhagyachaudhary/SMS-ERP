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
    public class exam_list
    {
        
        public int exam_id { get; set; }

        public string exam_name { get; set; }
    }

    public class comparative_result_analysis
    {
       

        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public void pdfComparative_result_analysis(int subject_id,int class_id, int section_id,string session)
        {
           
            using (MemoryStream ms = new MemoryStream())
            {
               

                using (Document doc = new Document(PageSize.A4.Rotate()))
                {
                    using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                    {
                        string query = @"SELECT DISTINCT
                                a.term_id, b.term_name
                            FROM
                                mst_term_rules a,
                                mst_term b
                            WHERE
                                a.class_id = @class_id
                                    AND a.session = @session
                                    AND a.term_id = b.term_id
                                    AND a.session = b.session";

                        IEnumerable<mst_term> term_list = con.Query<mst_term>(query, new { class_id = class_id, session = session });



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

                        ph = new Phrase();
                        text = new Chunk("Pro-forma for Comparative Study/Result Analysis for Academic Session " + session, FontFactory.GetFont("Areal", 15));
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

                        query = @"SELECT 
                                subject_name
                            FROM
                                mst_subject
                            WHERE
                                session = @session AND subject_id = @subject_id";

                        string subject = con.Query<string>(query, new { session = session, subject_id = subject_id }).SingleOrDefault();



                        query = @"SELECT 
                                CONCAT('Class: ',
                                        class_name,
                                        ' Section: ',
                                        section_name)
                            FROM
                                mst_class a,
                                mst_section b
                            WHERE
                                a.session = b.session
                                    AND a.class_id = b.class_id
                                    AND a.class_id = @class_id
                                    AND b.section_id = @section_id
                                    AND b.session = @session";

                        string class_name = con.Query<string>(query, new { session = session, section_id = section_id, class_id = class_id }).SingleOrDefault();

                        string exam_li = @"SELECT 
                                    exam_id,exam_name
                                FROM
                                    (SELECT 
                                        term_id, c.exam_id, exam_name
                                    FROM
                                        mst_term_rules a, mst_exam c
                                    WHERE
                                        a.session = c.session
                                            AND a.exam_id1 = c.exam_id
                                            AND a.class_id = @class_id
                                            AND a.session = @session
                                            AND a.term_id = @term_id UNION ALL SELECT 
                                        term_id, c.exam_id, exam_name
                                    FROM
                                        mst_term_rules a, mst_exam c
                                    WHERE
                                        a.session = c.session
                                            AND a.exam_id2 = c.exam_id
                                            AND a.class_id = @class_id
                                            AND a.session = @session
                                            AND a.term_id = @term_id) a
                                ORDER BY a.term_id , a.exam_id";


                        string std_query = @"SELECT 
                                                count(*)
                                            FROM
                                                mst_std_class a,
                                                mst_std_section b,
                                                sr_register c
                                            WHERE
                                                a.session = b.session
                                                    AND b.session = @session
                                                    AND a.sr_num = b.sr_num
                                                    AND b.sr_num = c.sr_number
                                                    AND c.std_active = 'Y'
                                                    AND a.class_id = @class_id
                                                    AND b.section_id = @section_id";

                        int total_std = con.Query<int>(std_query, new { session = session, section_id = section_id, class_id = class_id }).SingleOrDefault();

                        doc.Add(pt);


                        pt = new PdfPTable(2);

                        ph = new Phrase();
                        text = new Chunk("Subject: " + subject, FontFactory.GetFont("Areal", 12));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 1;
                        _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(class_name, FontFactory.GetFont("Areal", 12));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 1;
                        _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pt.AddCell(_cell);

                        doc.Add(pt);
                        IEnumerable<exam_list> exam_list;

                        foreach (var term in term_list)
                        {


                            exam_list = con.Query<exam_list>(exam_li, new { class_id = class_id, session = session, term_id = term.term_id });

                            pt = new PdfPTable((exam_list.Count() * 2) + 6);

                            ph = new Phrase();
                            text = new Chunk(term.term_name, FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = (exam_list.Count() * 2) + 6;
                            _cell.BackgroundColor = BaseColor.GRAY;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("S.No", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Rowspan = 2;
                            _cell.BackgroundColor = BaseColor.GRAY;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("Particulars", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.Rowspan = 2;
                            _cell.BackgroundColor = BaseColor.GRAY;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            foreach (var part in exam_list)
                            {
                                ph = new Phrase();
                                text = new Chunk(part.exam_name, FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.Colspan = 2;
                                _cell.BackgroundColor = BaseColor.GRAY;
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);


                            }

                            foreach (var part in exam_list)
                            {

                                ph = new Phrase();
                                text = new Chunk("No.", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.BackgroundColor = BaseColor.GRAY;
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                ph = new Phrase();
                                text = new Chunk("%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.BackgroundColor = BaseColor.GRAY;
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("1", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students Appeared", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);


                            foreach (var part in exam_list)
                            {
                                query = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        exam_id = @exam_id AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND present = 1";

                                int appeared = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();

                                ph = new Phrase();
                                text = new Chunk(appeared.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)appeared / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                            }

                            ph = new Phrase();
                            text = new Chunk("2", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students Passed", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {
                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            and round((a.marks / d.max_no) * 100,0) >= 33";

                                int passed = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();

                                ph = new Phrase();
                                text = new Chunk(passed.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)passed / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }


                            ph = new Phrase();
                            text = new Chunk("3", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students Failed", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            and round((a.marks / d.max_no) * 100,0) < 33";

                                int failed = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(failed.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)failed / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8)); ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("4", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students Placed in 1st Division", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            and round((a.marks / d.max_no) * 100,0) >= 60";

                                int Ist_division = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(Ist_division.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)Ist_division / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("5", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students Placed in 2nd Division", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            AND round((a.marks / d.max_no) * 100,0) >= 50 AND round((a.marks / d.max_no) * 100,0) < 60";

                                int IIst_division = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(IIst_division.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)IIst_division / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("6", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students who scored 90% and above", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            AND round((a.marks / d.max_no) * 100,0) >= 90";

                                int marks80_above = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(marks80_above.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)marks80_above / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("7", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students who scored between 80% to 89%", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            AND round((a.marks / d.max_no) * 100,0) >= 80 AND round((a.marks / d.max_no) * 100,0) <= 89";

                                int marks80_above = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(marks80_above.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)marks80_above / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("8", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students who scored between 70% to 79%", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            AND round((a.marks / d.max_no) * 100,0) >= 70 AND round((a.marks / d.max_no) * 100,0) <= 79";

                                int marks80_above = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(marks80_above.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)marks80_above / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("9", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students who scored between 50% to 69%", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            AND round((a.marks / d.max_no) * 100,0) >= 50 AND round((a.marks / d.max_no) * 100,0) <= 69";

                                int marks80_above = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(marks80_above.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)marks80_above / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);
                            }

                            ph = new Phrase();
                            text = new Chunk("10", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            ph = new Phrase();
                            text = new Chunk("No. of Students who scored between 40% to 49%", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = 5;
                            _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);



                            foreach (var part in exam_list)
                            {

                                query = @"SELECT 
                                        count(*)
                                    FROM
                                        mst_exam_marks a,
                                        mst_std_class b,
                                        mst_std_section c,
                                        mst_exam d
                                    WHERE
                                        a.exam_id = @exam_id 
                                        and
                                        a.exam_id = d.exam_id
                                        AND a.subject_id = @subject_id
                                            AND b.class_id = @class_id
                                            AND c.section_id = @section_id
                                            AND a.session = b.session
                                            AND b.session = c.session
                                            AND a.session = @session
                                            AND a.sr_num = b.sr_num
                                            AND b.sr_num = c.sr_num
                                            AND c.session = d.session
                                            AND present = 1
                                            AND round((a.marks / d.max_no) * 100,0) >= 40 AND round((a.marks / d.max_no) * 100,0) <= 49";

                                int marks80_above = con.Query<int>(query, new { class_id = class_id, session = session, exam_id = part.exam_id, section_id = section_id, subject_id = subject_id }).SingleOrDefault();


                                ph = new Phrase();
                                text = new Chunk(marks80_above.ToString(), FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);

                                decimal result = Math.Round(((decimal)marks80_above / (decimal)total_std) * 100, 2);

                                ph = new Phrase();
                                text = new Chunk(result.ToString() + "%", FontFactory.GetFont("Areal", 8));
                                ph.Add(text);
                                _cell = new PdfPCell(ph);
                                _cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pt.AddCell(_cell);


                            }

                            ph = new Phrase();
                            text = new Chunk("\n\n", FontFactory.GetFont("Areal", 8));
                            ph.Add(text);
                            _cell = new PdfPCell(ph);
                            _cell.Colspan = exam_list.Count() * 2 + 6;
                            _cell.Border = iTextSharp.text.Rectangle.NO_BORDER;
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);


                            doc.Add(pt);
                        }

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
}