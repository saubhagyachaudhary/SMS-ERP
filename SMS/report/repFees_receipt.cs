using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Configuration;
using SMS.Models;
using Dapper;
using System.Diagnostics;
using System.Web.Mvc;
using MySql.Data.MySqlClient;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;

namespace SMS.report
{
    public class repFees_receipt
    {
        
        string shortfirstname = ConfigurationManager.AppSettings["ShortFirstName"].ToString();
        string shortlastname = ConfigurationManager.AppSettings["ShortLastName"].ToString();
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
        string rcptHead = ConfigurationManager.AppSettings["RcptHead"].ToString();

        public void pdf(int receipt_no)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {

                MemoryStream ms = new MemoryStream();

                HttpContext.Current.Response.ContentType = "application/pdf";
                string name = "receiptno_" + receipt_no + ".pdf";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
                var doc = new Document(PageSize.A6);

                // MemoryStream stream = new MemoryStream();
                doc.SetMargins(0f, 0f, 10f, 70f);
                try
                {


                    string query = @"SELECT 
                                        mode_flag,
                                        chq_no,
                                        chq_date,
                                        receipt_date,
                                        fees_name,
                                        sr_number,
                                        class_id,
                                        amount fees,
                                        dc_fine fine,
                                        dc_discount discount,
                                        amount + dc_fine - dc_discount amount,
                                        reg_no,
                                        reg_date,
                                        session
                                    FROM
                                        fees_receipt
                                    WHERE
                                        fin_id = (SELECT 
                                                fin_id
                                            FROM
                                                mst_fin
                                            WHERE
                                                fin_close = 'N')
                                            AND receipt_no = @receipt_no";


                    IEnumerable<fees_receipt> result = con.Query<fees_receipt>(query, new { receipt_no = receipt_no });

                    rep_fees rep = new rep_fees();

                    if (result.First<fees_receipt>().sr_number == 0)
                    {
                        query = @"SELECT 
                                reg_no num,
                                CONCAT(IFNULL(std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) name,
                                std_father_name father_name,
                                b.class_name
                            FROM
                                std_registration a,
                                mst_class b
                            WHERE
                                a.std_class_id = b.class_id
                                    AND reg_no = @reg_no
                                    AND reg_date = @reg_date
                                    AND b.session = a.session
                                    AND a.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_active = 'Y')";

                        rep = con.Query<rep_fees>(query, new { reg_no = result.First<fees_receipt>().reg_no, reg_date = result.First<fees_receipt>().reg_date }).SingleOrDefault();
                    }
                    else
                    {
                        query = @"SELECT 
                                    sr_number num,
                                    CONCAT(ifnull(std_first_name,''), ' ',ifnull(std_last_name,'')) name,
                                    std_father_name father_name,
                                    CONCAT(c.class_name, ' Sec. ', b.section_name) class_name
                                FROM
                                    sr_register a,
                                    mst_section b,
                                    mst_class c,
                                    mst_std_section d,
                                    mst_std_class e
                                WHERE
                                    d.section_id = b.section_id
                                        AND b.class_id = c.class_id
                                        AND e.class_id = b.class_id
                                        AND a.sr_number = d.sr_num
                                        AND d.sr_num = e.sr_num
                                        AND a.sr_number = @sr_number
                                        AND b.session = c.session
                                        AND c.session = d.session
                                        AND d.session = e.session
                                        AND e.session = @session";

                        rep = con.Query<rep_fees>(query, new { sr_number = result.First<fees_receipt>().sr_number, session = result.First<fees_receipt>().session }).SingleOrDefault();
                    }

                    rep.receipt_no = receipt_no;
                    rep.receipt_date = result.First<fees_receipt>().receipt_date.ToString("yyyy-MM-dd");
                    //rep.fees_name = result.fees_name;
                    //rep.amount = result.amount;



                    PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();



                    doc.Open();
                    // string imageURL = "E:\\HPS\\logo.jpg";

                    PdfPTable pt = new PdfPTable(6);
                    PdfPCell _cell;
                    Chunk text;
                    Phrase ph;

                    if (int.Parse(rcptHead) == 0)
                    {

                        string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                        Image jpg = Image.GetInstance(imageURL);
                        jpg.ScaleAbsolute(50f, 50f);



                        _cell = new PdfPCell(jpg);
                        _cell.Border = 0;
                        _cell.Colspan = 2;
                        _cell.Border = Rectangle.BOTTOM_BORDER;
                        _cell.PaddingBottom = 5;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);


                        text = new Chunk(shortfirstname, FontFactory.GetFont("Areal", 18));
                        ph = new Phrase();
                        ph.Add(text);
                        ph.Add("\n");
                        text = new Chunk(shortlastname, FontFactory.GetFont("Areal", 12));
                        //ph = new Phrase();
                        ph.Add(text);
                        /* _cell = new PdfPCell(ph);
                         _cell.Colspan = 3;
                         _cell.HorizontalAlignment = Element.ALIGN_CENTER;*/

                        //  _cell.Border = 0;
                        // pt.AddCell(_cell);
                        //ph.Add("\n");
                        //text = new Chunk("Nh-24 Village Ballia, Dhaneta, Meerganj Bareilly-243504", FontFactory.GetFont("Areal", 08));
                        //ph.Add(text);

                        ph.Add("\n");
                        text = new Chunk("(" + Affiliation + ")", FontFactory.GetFont("Areal", 08));
                        ph.Add(text);


                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 4;
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = Rectangle.BOTTOM_BORDER;
                        _cell.PaddingBottom = 5;
                        //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;


                        pt.AddCell(_cell);

                        doc.Add(pt);
                    }
                    else
                    {
                        string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                        Image jpg = Image.GetInstance(imageURL);
                        jpg.ScaleAbsolute(30f, 30f);


                        _cell = new PdfPCell(jpg);
                        _cell.Border = 0;
                        _cell.Border = Rectangle.BOTTOM_BORDER;
                        _cell.PaddingBottom = 5;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);


                        text = new Chunk(SchoolName, FontFactory.GetFont("Areal", 16));
                        ph = new Phrase();
                        ph.Add(text);
                        ph.Add("\n");



                        text = new Chunk("(" + Affiliation + ")", FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 5;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        _cell.Border = Rectangle.BOTTOM_BORDER;
                        _cell.PaddingBottom = 5;
                        //_cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        pt.AddCell(_cell);

                        doc.Add(pt);
                    }
                    pt = new PdfPTable(4);

                    text = new Chunk("Fees Receipt", FontFactory.GetFont("Areal", 10));
                    text.SetUnderline(0.1f, -1f);
                    ph = new Phrase(text);

                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    text = new Chunk("\n");
                    ph = new Phrase(text);

                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    _cell.Border = 0;
                    pt.AddCell(_cell);


                    text = new Chunk("Receipt No:", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    text = new Chunk(rep.receipt_no.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    text = new Chunk("Receipt Date:", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    text = new Chunk(DateTime.ParseExact(rep.receipt_date, "yyyy-MM-dd", null).ToString("dd-MMM-yyyy"), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    if (result.First<fees_receipt>().sr_number == 0)
                    {
                        text = new Chunk("Registration No:", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);
                    }
                    else
                    {
                        text = new Chunk("Admission No:", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);
                    }
                    text = new Chunk(rep.num.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    if (result.First<fees_receipt>().sr_number == 0)
                    {
                        text = new Chunk("Reg Class: ", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);
                    }
                    else
                    {
                        text = new Chunk("Class: ", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);
                    }

                    text = new Chunk(rep.class_name, FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    if (result.First<fees_receipt>().mode_flag != "Cash")
                    {
                        text = new Chunk("Inst No:", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);

                        text = new Chunk(result.First<fees_receipt>().chq_no, FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);

                        text = new Chunk("Inst Date:", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);

                        text = new Chunk(result.First<fees_receipt>().chq_date.Value.ToString("dd-MMM-yyyy"), FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Border = 0;
                        pt.AddCell(_cell);

                    }


                    text = new Chunk("Name:", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    text = new Chunk(rep.name, FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 3;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);



                    text = new Chunk("Father's Name:", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    text = new Chunk(rep.father_name, FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 3;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    text = new Chunk("\n");
                    ph = new Phrase(text);

                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    _cell.Border = 0;
                    pt.AddCell(_cell);

                    doc.Add(pt);

                    pt = new PdfPTable(7);

                    pt.WidthPercentage = 90f;

                    text = new Chunk("Particulars", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 3;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Fees", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Fine", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Dis.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Paid", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    decimal total = 0;

                    foreach (var fee in result)
                    {

                        ph = new Phrase();
                        text = new Chunk(fee.fees_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        _cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.fees.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.fine.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.discount.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.amount.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Border = Rectangle.LEFT_BORDER | Rectangle.RIGHT_BORDER;
                        _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pt.AddCell(_cell);

                        total = total + fee.amount;
                    }
                    

                    text = new Chunk("Total amount paid", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 6;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk(total.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 1;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    doc.Add(pt);

                    //_cell.FixedHeight = 150;
                    pt = new PdfPTable(4);

                    text = new Chunk("Received with thanks: ", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = 0;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    text = new Chunk(NumbersToWords(Decimal.ToInt32(total)) + " Only", FontFactory.GetFont("Areal", 8));
                    text.SetUnderline(0.1f, -1f);

                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 3;
                    _cell.Border = 0;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
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

                //catch (DocumentException dex)
                //{
                //    throw (dex);
                //}
                //catch (IOException io)
                //{
                //    throw (io);
                //}
                finally
                {
                    doc.Close();
                    ms.Flush();
                }
            }
        }


        public static string NumbersToWords(int inputNumber)
        {
            int inputNo = inputNumber;

            if (inputNo == 0)
                return "Zero";

            int[] numbers = new int[4];
            int first = 0;
            int u, h, t;
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            if (inputNo < 0)
            {
                sb.Append("Minus ");
                inputNo = -inputNo;
            }

            string[] words0 = {"" ,"One ", "Two ", "Three ", "Four ",
            "Five " ,"Six ", "Seven ", "Eight ", "Nine "};
            string[] words1 = {"Ten ", "Eleven ", "Twelve ", "Thirteen ", "Fourteen ",
            "Fifteen ","Sixteen ","Seventeen ","Eighteen ", "Nineteen "};
            string[] words2 = {"Twenty ", "Thirty ", "Forty ", "Fifty ", "Sixty ",
            "Seventy ","Eighty ", "Ninety "};
            string[] words3 = { "Thousand ", "Lakh ", "Crore " };

            numbers[0] = inputNo % 1000; // units
            numbers[1] = inputNo / 1000;
            numbers[2] = inputNo / 100000;
            numbers[1] = numbers[1] - 100 * numbers[2]; // thousands
            numbers[3] = inputNo / 10000000; // crores
            numbers[2] = numbers[2] - 100 * numbers[3]; // lakhs

            for (int i = 3; i > 0; i--)
            {
                if (numbers[i] != 0)
                {
                    first = i;
                    break;
                }
            }
            for (int i = first; i >= 0; i--)
            {
                if (numbers[i] == 0) continue;
                u = numbers[i] % 10; // ones
                t = numbers[i] / 10;
                h = numbers[i] / 100; // hundreds
                t = t - 10 * h; // tens
                if (h > 0) sb.Append(words0[h] + "Hundred ");
                if (u > 0 || t > 0)
                {
                    if (h > 0 || i == 0) sb.Append("and ");
                    if (t == 0)
                        sb.Append(words0[u]);
                    else if (t == 1)
                        sb.Append(words1[u]);
                    else
                        sb.Append(words2[t - 2] + words0[u]);
                }
                if (i != 0) sb.Append(words3[i - 1]);
            }
            return sb.ToString().TrimEnd();
        }


    }

    public class PDFFooter : PdfPageEventHelper
    {
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
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
            tabFot.TotalWidth = 300F;
            Chunk text;
            Phrase ph = new Phrase();

            text = new Chunk("This is a computer generated receipt.", FontFactory.GetFont("Areal", 8));
            ph.Add(text);
            ph.Add("\n");
            ph.Add("\n");

            cell = new PdfPCell(ph);
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            cell.Border = 0;
            tabFot.AddCell(cell);

            text = new Chunk(Address, FontFactory.GetFont("Areal", 8));
            ph = new Phrase();
            ph.Add(text);

            ph.Add("\n");
            //text = new Chunk("(Affiliated to CBSE New Delhi. Affiliation Number 2132182)", FontFactory.GetFont("Areal", 08));
            text = new Chunk(MaiLSite, FontFactory.GetFont("Areal", 8));
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