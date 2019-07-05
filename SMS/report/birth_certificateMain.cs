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
    public class birth_certificateMain
    {
        
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();


        static readonly string[] ones = new string[] { "", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine" };
        static readonly string[] teens = new string[] { "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
        static readonly string[] tens = new string[] { "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };
        static readonly string[] thousandsGroups = { "", " Thousand", " Million", " Billion" };

        public void pdfStudent_BirthCertificate(int sr_num)
        {

            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "Birth_Certificate_" + sr_num + ".pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(40f, 40f,180f, 30f);
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();

                    string query = @"SELECT 
                                    CONCAT(IFNULL(a.std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, '')) std_name,
                                    std_father_name,
                                    std_dob,
                                    c.class_name
                                FROM
                                    sr_register a,
                                    mst_std_class b,
                                    mst_class c
                                WHERE
                                    b.class_id = c.class_id
                                        AND a.sr_number = b.sr_num
                                        AND a.sr_number = @sr_num
                                        AND c.session = b.session
                                        AND b.session = @session
                                        AND a.std_active = 'Y'";

                    student_ledger std = con.Query<student_ledger>(query, new { sr_num = sr_num, session = session.findFinal_Session() }).SingleOrDefault();

                    PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();

                    doc.Open();

                    Chunk text = new Chunk();

                    text = new Chunk("Date: " + System.DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Times New Roman", 12));
                    Paragraph para = new Paragraph(text);
                    para.SpacingBefore = 10;
                    para.SpacingAfter = 10;
                    para.Alignment = 0;
                    doc.Add(para);


                    text = new Chunk("To Whom It May Concern", FontFactory.GetFont("Times New Roman", 15, Font.BOLD));
                    text.SetUnderline(2, -3);
                    para = new Paragraph(text);
                    para.SpacingBefore = 20;
                    para.SpacingAfter = 20;
                    para.Alignment = 1;
                    doc.Add(para);



                    string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

                    Chunk school_name = new Chunk(" (" + SchoolName + ")", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    school_name.SetUnderline(2, -3);

                    Chunk std_name = new Chunk(std.std_name + ", Admission number " + sr_num.ToString(), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    std_name.SetUnderline(2, -3);

                    Chunk std_father = new Chunk(std.std_father_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    std_father.SetUnderline(2, -3);

                    Chunk std_class = new Chunk("Class " + std.class_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    std_class.SetUnderline(2, -3);

                    Chunk std_dob = new Chunk(std.std_dob.ToString("dd/MM/yyyy"), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    std_dob.SetUnderline(2, -3);

                    Chunk std_dob_word = new Chunk(DateToWritten(std.std_dob) + ".", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    std_dob_word.SetUnderline(2, -3);

                    text = new Chunk("This is certify that ", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = Element.ALIGN_JUSTIFIED;
                    para.Add(std_name);

                    text = new Chunk(" is son/daughter of ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_father);

                    text = new Chunk(" studying in ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_class);


                    para.Add(school_name);

                    text = new Chunk(". As per school record, His/her date of birth is ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_dob);

                    text = new Chunk(". In words ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_dob_word);
                    para.SetLeading(15, 1);
                    para.SpacingBefore = 40;
                    para.SpacingAfter = 40;
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








                    doc.Close();

                    HttpContext.Current.Response.OutputStream.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    HttpContext.Current.Response.OutputStream.Flush();
                    HttpContext.Current.Response.OutputStream.Close();





                }
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

        public void pdfReimbursementCertificate(int sr_num, string session)
        {

            MemoryStream ms = new MemoryStream();

            HttpContext.Current.Response.ContentType = "application/pdf";
            string name = "Birth_Certificate_" + sr_num + ".pdf";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A4);

            // MemoryStream stream = new MemoryStream();
            doc.SetMargins(40f, 40f, 100f, 30f);
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {

                    string query = @"SELECT 
                                        CONCAT(IFNULL(a.std_first_name, ''),
                                                ' ',
                                                IFNULL(std_last_name, '')) std_name,
                                        std_father_name,
                                        std_dob,
                                        c.class_name,
                                        (SELECT 
                                                section_name
                                            FROM
                                                mst_section
                                            WHERE
                                                section_id = f.section_id
                                                    AND session = f.session) section_name,
                                        d.roll_number,
                                        e.session_start_date,
                                        e.session_end_date
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_class c,
                                        mst_rollnumber d,
                                        mst_session e,
                                        mst_std_section f
                                    WHERE
                                        b.class_id = c.class_id
                                            AND f.sr_num = d.sr_num
                                            AND d.sr_num = a.sr_number
                                            AND a.sr_number = b.sr_num
                                            AND a.sr_number = @sr_num
                                            AND f.session = e.session
                                            AND e.session = d.session
                                            AND d.session = c.session
                                            AND c.session = b.session
                                            AND b.session = @session
                                            AND a.std_active = 'Y'";

                    reimbursement std = con.Query<reimbursement>(query, new { sr_num = sr_num, session = session }).SingleOrDefault();

                    PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();

                    doc.Open();

                    Chunk text = new Chunk();

                    text = new Chunk("CERTIFICATE FROM THE HEAD OF INSTITUTION/SCHOOL", FontFactory.GetFont("Times New Roman", 13, Font.BOLD));
                    Paragraph para = new Paragraph(text);
                    text.SetUnderline(2, -3);
                    para.Alignment = 1;
                    doc.Add(para);

                    text = new Chunk("(FOR REIMBURSEMENT OF CEA/HOSTEL SUBSIDY)", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                    para = new Paragraph(text);
                    para.SpacingBefore = 20;
                    para.SpacingAfter = 20;
                    para.Alignment = 1;
                    doc.Add(para);


                    text = new Chunk("Date: " + System.DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Add("\n\n\n");
                    para.SpacingBefore = 10;
                    para.SpacingAfter = 10;
                    para.Alignment = 0;
                    doc.Add(para);





                    string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

                    Chunk school_name = new Chunk(SchoolName + " " + Address, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    Chunk std_name = new Chunk(std.std_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    Chunk adm_no = new Chunk(sr_num.ToString(), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    Chunk std_father = new Chunk(std.std_father_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    Chunk std_class = new Chunk(std.class_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));

                    Chunk std_section = new Chunk(std.section_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));

                    Chunk std_roll_number = new Chunk(std.roll_number.ToString(), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    Chunk session_start_date = new Chunk(std.session_start_date.ToString("dd MMMM yyyy"), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    Chunk session_end_date = new Chunk(std.session_end_date.ToString("dd MMMM yyyy"), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    Chunk std_dob = new Chunk(std.std_dob.ToString("dd MMMM yyyy"), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    text = new Chunk("It is certified that Master/Kumari ", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph(text);
                    para.Alignment = Element.ALIGN_JUSTIFIED;
                    para.Add(std_name);

                    text = new Chunk(" having, admission No. ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(adm_no);

                    text = new Chunk(" D.O.B ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_dob);

                    text = new Chunk(" Son/Daughter of Mr/Mrs. ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_father);

                    text = new Chunk(" was studying in class ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_class);

                    text = new Chunk(" section ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_section);

                    text = new Chunk(" Roll No.  ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(std_roll_number);

                    text = new Chunk(" during the previous academic year from  ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(session_start_date);

                    text = new Chunk(" to  ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(session_end_date);

                    text = new Chunk(" School/Institution, namely  ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(school_name);

                    Chunk Affiliati = new Chunk(Affiliation, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));

                    text = new Chunk(" vide affiliation ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(Affiliati);

                    Chunk pattern = new Chunk("Day School CBSE", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    text = new Chunk(" and Pattern ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(pattern);

                    text = new Chunk(" Curriculum.", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);


                    doc.Add(para);

                    text = new Chunk("Signature of Principal with rubber stamp", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph("\n\n\n\n");
                    para.Add(text);
                    para.SpacingBefore = 40;
                    para.Alignment = 2;

                    doc.Add(para);

                    Chunk previous = new Chunk("Previous Academic Year.", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));

                    Chunk aff = new Chunk("Affiliation:", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));

                    Chunk pat = new Chunk("Pattern.", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));

                    Chunk curr = new Chunk("Curriculum.", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));


                    text = new Chunk("Note:", FontFactory.GetFont("Times New Roman", 12));
                    para = new Paragraph("\n");
                    para.Add(text);
                    para.Add("\n");

                    text = new Chunk(" 1) ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(previous);
                    text = new Chunk(" Refers to the period for which the claim is being made.", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add("\n");

                    text = new Chunk(" 2) ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(aff);
                    text = new Chunk(" Fill in the Regd/Affiliation No. allotted to the school.", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add("\n");

                    text = new Chunk(" 3) ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(pat);
                    text = new Chunk(" Day school/ Boarding", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add("\n");

                    text = new Chunk(" 4) ", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add(curr);
                    text = new Chunk(" CBSE/ ICSE etc", FontFactory.GetFont("Times New Roman", 12));
                    para.Add(text);
                    para.Add("\n");


                    doc.Add(para);

                    doc.Close();

                    HttpContext.Current.Response.OutputStream.Write(ms.ToArray(), 0, ms.ToArray().Length);
                    HttpContext.Current.Response.OutputStream.Flush();
                    HttpContext.Current.Response.OutputStream.Close();


                }
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


        private static string FriendlyInteger(int n, string leftDigits, int thousands)
        {
            if (n == 0)
                return leftDigits;

            string friendlyInt = leftDigits;
            if (friendlyInt.Length > 0)
                friendlyInt += " ";

            if (n < 10)
                friendlyInt += ones[n];
            else if (n < 20)
                friendlyInt += teens[n - 10];
            else if (n < 100)
                friendlyInt += FriendlyInteger(n % 10, tens[n / 10 - 2], 0);
            else if (n < 1000)
                friendlyInt += FriendlyInteger(n % 100, (ones[n / 100] + " Hundred"), 0);
            else
                friendlyInt += FriendlyInteger(n % 1000, FriendlyInteger(n / 1000, "", thousands + 1), 0);

            return friendlyInt + thousandsGroups[thousands];
        }

        public static string DateToWritten(DateTime date)
        {
            return string.Format("{0} {1} {2}", IntegerToWritten(date.Day), date.ToString("MMMM"), IntegerToWritten(date.Year));
        }

        public static string IntegerToWritten(int n)
        {
            if (n == 0)
                return "Zero";
            else if (n < 0)
                return "Negative " + IntegerToWritten(-n);

            return FriendlyInteger(n, "", 0);
        }

        public class PDFFooter : PdfPageEventHelper
        {
           
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
                
            }

            //write on close of document
            public override void OnCloseDocument(PdfWriter writer, Document document)
            {
                base.OnCloseDocument(writer, document);


            }
        }
    }

    public class reimbursement
    {
        public string std_name { get; set; }

        public string std_father_name { get; set; }

        public DateTime std_dob { get; set; }

        public string class_name { get; set; }

        public int roll_number { get; set; }

        public DateTime session_start_date { get; set; }

        public DateTime session_end_date { get; set; }

        public string section_name { get; set; }
    }
}