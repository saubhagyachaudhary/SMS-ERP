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
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
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
                                        AND b.session = @session
                                        AND a.std_active = 'Y'";

                student_ledger std = con.Query<student_ledger>(query, new { sr_num = sr_num, session = session.findActive_finalSession() }).SingleOrDefault();

                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();

                doc.Open();

                Chunk text = new Chunk();

                text = new Chunk("Date: " + System.DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Times New Roman", 12));
                Paragraph para = new Paragraph(text);
                para.SpacingBefore = 10;
                para.SpacingAfter = 10;
                para.Alignment = 0;
                doc.Add(para);


                text = new Chunk("To Whom It May Concern", FontFactory.GetFont("Times New Roman", 15,Font.BOLD));
                text.SetUnderline(2, -3);
                para = new Paragraph(text);
                para.SpacingBefore = 20;
                para.SpacingAfter = 20;
                para.Alignment = 1;
                doc.Add(para);

                
                
                string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

                Chunk school_name = new Chunk(" ("+SchoolName+")", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                school_name.SetUnderline(2, -3);

                Chunk std_name = new Chunk(std.std_name+ ", Admission number " + sr_num.ToString(), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                std_name.SetUnderline(2, -3);

                Chunk std_father = new Chunk(std.std_father_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                std_father.SetUnderline(2, -3);

                Chunk std_class = new Chunk("Class "+std.class_name, FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                std_class.SetUnderline(2, -3);

                Chunk std_dob = new Chunk(std.std_dob.ToString("dd/MM/yyyy"), FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
                std_dob.SetUnderline(2, -3);

                Chunk std_dob_word = new Chunk(DateToWritten(std.std_dob)+".", FontFactory.GetFont("Times New Roman", 12, Font.BOLD));
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
                para.SetLeading(15,1);
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
}