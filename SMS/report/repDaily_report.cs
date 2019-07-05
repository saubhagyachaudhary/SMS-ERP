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

namespace SMS.report
{
    public class repDaily_report
    {
       
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();


        public void pdfdetailed(DateTime fromdt, DateTime todt, string mode)
        {

            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                MemoryStream ms = new MemoryStream();

                HttpContext.Current.Response.ContentType = "application/pdf";
                string name = "FR_" + fromdt.ToString("dd/MM/yyyy") + "_" + todt.ToString("dd/MM/yyyy") + ".pdf";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
                var doc = new Document(PageSize.A4);

                // MemoryStream stream = new MemoryStream();
                doc.SetMargins(0f, 0f, 10f, 70f);
                try
                {
                    IEnumerable<repDaily_reportMain> result;

                    if (mode == "Both")
                    {
                        string query = @"SELECT 
                                        *
                                    FROM
                                        (SELECT 
                                            receipt_no,
                                                receipt_date,
                                                a.sr_number,
                                                CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                fees_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND c.class_id = d.class_id
                                                AND b.sr_number = d.sr_num
                                                AND receipt_date BETWEEN @fromdt AND @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0
                                                AND a.session = c.session
                                                AND c.session = d.session
                                                UNION ALL SELECT 
                                            receipt_no,
                                                receipt_date,
                                                0 sr_number,
                                                CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                fees_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            std_registration a, fees_receipt b, mst_class c
                                        WHERE
                                            a.reg_no = b.reg_no
                                                AND a.reg_date = b.reg_date
                                                AND a.session = b.session
                                                AND b.session = c.session
                                                AND c.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_active = 'Y')
                                                AND a.std_class_id = c.class_id
                                                AND b.receipt_date BETWEEN @fromdt AND @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                    ORDER BY a.receipt_date";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt });

                    }
                    else if (mode == "Bank")
                    {
                        string query = @"SELECT 
                                        *
                                    FROM
                                        (SELECT 
                                            receipt_no,
                                                receipt_date,
                                                a.sr_number,
                                                CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                fees_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND c.class_id = d.class_id
                                                AND b.sr_number = d.sr_num
                                                AND receipt_date BETWEEN @fromdt AND @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND a.mode_flag = 'Cheque'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0 
                                                AND a.session = c.session
                                                AND c.session = d.session
                                                UNION ALL SELECT 
                                            receipt_no,
                                                receipt_date,
                                                0 sr_number,
                                                CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                fees_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            std_registration a, fees_receipt b, mst_class c
                                        WHERE
                                            a.reg_no = b.reg_no
                                                AND a.reg_date = b.reg_date
                                                AND a.session = b.session
                                                 AND b.session = c.session
                                                            AND c.session = (SELECT 
                                                                session
                                                            FROM
                                                                mst_session
                                                            WHERE
                                                                session_active = 'Y')
                                                AND a.std_class_id = c.class_id
                                                AND b.receipt_date BETWEEN @fromdt AND @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND b.mode_flag = 'Cheque'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                    ORDER BY a.receipt_date";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt });

                    }
                    else
                    {
                        string query = @"SELECT 
                                            *
                                        FROM
                                            (SELECT 
                                                receipt_no,
                                                    receipt_date,
                                                    a.sr_number,
                                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                    fees_name,
                                                    c.class_name,
                                                    IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                    CASE mode_flag
                                                        WHEN 'Cheque' THEN 'Bank'
                                                        ELSE mode_flag
                                                    END mode_flag,
                                                    CASE mode_flag
                                                        WHEN 'Cash' THEN 'Cleared'
                                                        ELSE CASE
                                                            WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                            ELSE chq_reject
                                                        END
                                                    END AS chq_reject
                                            FROM
                                                fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                            WHERE
                                                a.sr_number = b.sr_number
                                                    AND c.class_id = d.class_id
                                                    AND b.sr_number = d.sr_num
                                                    AND receipt_date BETWEEN @fromdt AND @todt
                                                    AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                    AND a.mode_flag = 'Cash'
                                                    AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0 
                                                    AND a.session = c.session
                                                    AND c.session = d.session                                
                                                    UNION ALL SELECT 
                                                receipt_no,
                                                    receipt_date,
                                                    0 sr_number,
                                                    CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                    fees_name,
                                                    c.class_name,
                                                    IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                    CASE mode_flag
                                                        WHEN 'Cheque' THEN 'Bank'
                                                        ELSE mode_flag
                                                    END mode_flag,
                                                    CASE mode_flag
                                                        WHEN 'Cash' THEN 'Cleared'
                                                        ELSE CASE
                                                            WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                            ELSE chq_reject
                                                        END
                                                    END AS chq_reject
                                            FROM
                                                std_registration a, fees_receipt b, mst_class c
                                            WHERE
                                                a.reg_no = b.reg_no
                                                    AND a.reg_date = b.reg_date
                                                    AND a.session = b.session
                                                     AND b.session = c.session
                                                    AND c.session = (SELECT 
                                                        session
                                                    FROM
                                                        mst_session
                                                    WHERE
                                                        session_active = 'Y')
                                                    AND a.std_class_id = c.class_id
                                                    AND b.receipt_date BETWEEN @fromdt AND @todt
                                                    AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                    AND b.mode_flag = 'Cash'
                                                    AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                        ORDER BY a.receipt_date";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt });

                    }

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



                    pt = new PdfPTable(17);

                    pt.WidthPercentage = 95f;

                    text = new Chunk("\n");
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.TOP_BORDER;
                    _cell.Colspan = 18;
                    pt.AddCell(_cell);


                    text = new Chunk("Fees statement for the period of " + fromdt.ToString("dd/MM/yyyy") + " to " + todt.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 12));
                    ph = new Phrase(text);
                    ph.Add("\n");
                    ph.Add("\n");
                    text.SetUnderline(0.1f, -2f);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 18;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = Rectangle.NO_BORDER;
                    pt.AddCell(_cell);



                    text = new Chunk("Rcpt No.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Rcpt Dt.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Adm No.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);

                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Std Name.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 3;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Fees.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 4;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Class.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Amt.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Mode.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Status.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    decimal total = 0;

                    int line = 1;

                    foreach (var fee in result)
                    {

                        ph = new Phrase();
                        text = new Chunk(fee.receipt_no.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.receipt_date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        if (fee.sr_number == 0)
                            text = new Chunk("N/A", FontFactory.GetFont("Areal", 8));
                        else
                            text = new Chunk(fee.sr_number.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.std_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.fees_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 4;
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
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.amount.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.mode_flag, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.chq_reject, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
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
                    _cell.Colspan = 12;
                    pt.AddCell(_cell);

                    text = new Chunk(total.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                    _cell.Colspan = 2;
                    pt.AddCell(_cell);

                    text = new Chunk("", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                    _cell.Colspan = 3;
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


        public void pdfCansolidated(DateTime fromdt, DateTime todt, string mode)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {

                MemoryStream ms = new MemoryStream();

                HttpContext.Current.Response.ContentType = "application/pdf";
                string name = "FR_" + fromdt.ToString("dd/MM/yyyy") + "_" + todt.ToString("dd/MM/yyyy") + ".pdf";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
                var doc = new Document(PageSize.A4);

                // MemoryStream stream = new MemoryStream();
                doc.SetMargins(0f, 0f, 10f, 70f);
                try
                {
                    IEnumerable<repDaily_reportMain> result;

                    if (mode == "Both")
                    {
                        string query = @"SELECT 
                                        receipt_no,
                                        receipt_date,
                                        sr_number,
                                        std_name,
                                        class_name,
                                        SUM(amount) amount,
                                        mode_flag,
                                        chq_reject
                                    FROM
                                        (SELECT 
                                            receipt_no,
                                                receipt_date,
                                                a.sr_number,
                                                CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND c.class_id = d.class_id
                                                AND b.sr_number = d.sr_num
                                                AND receipt_date BETWEEN @fromdt and @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0
                                                AND a.session = c.session
                                                AND c.session = d.session                                                
                                                UNION ALL SELECT 
                                            receipt_no,
                                                receipt_date,
                                                0 sr_number,
                                                CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            std_registration a, fees_receipt b, mst_class c
                                        WHERE
                                            a.reg_no = b.reg_no
                                                AND a.reg_date = b.reg_date
                                                AND a.session = b.session
                                                 AND b.session = c.session
                                                            AND c.session = (SELECT 
                                                                session
                                                            FROM
                                                                mst_session
                                                            WHERE
                                                                session_active = 'Y')
                                                AND a.std_class_id = c.class_id
                                                AND b.receipt_date BETWEEN @fromdt and @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                    GROUP BY receipt_no , receipt_date , sr_number , std_name , class_name , mode_flag , chq_reject
                                    ORDER BY a.receipt_date , receipt_no";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt });

                    }
                    else if (mode == "Bank")
                    {
                        string query = @"SELECT 
                                        receipt_no,
                                        receipt_date,
                                        sr_number,
                                        std_name,
                                        class_name,
                                        SUM(amount) amount,
                                        mode_flag,
                                        chq_reject,
                                        a.bnk_name,
                                        a.chq_no,
                                        a.chq_date
                                    FROM
                                        (SELECT 
                                            receipt_no,
                                                receipt_date,
                                                a.sr_number,
                                                a.bnk_name,
                                                a.chq_no,
                                                a.chq_date,
                                                CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND c.class_id = d.class_id
                                                AND b.sr_number = d.sr_num
                                                AND receipt_date BETWEEN @fromdt AND @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND a.mode_flag = 'Cheque'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0 
                                                AND a.session = c.session
                                                AND c.session = d.session
                                                UNION ALL SELECT 
                                            receipt_no,
                                                receipt_date,
                                                0 sr_number,
                                                b.bnk_name,
                                                b.chq_no,
                                                b.chq_date,
                                                CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            std_registration a, fees_receipt b, mst_class c
                                        WHERE
                                            a.reg_no = b.reg_no
                                                AND a.reg_date = b.reg_date
                                                AND a.session = b.session
                                                 AND b.session = c.session
                                                AND c.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_active = 'Y')
                                                AND a.std_class_id = c.class_id
                                                AND b.receipt_date BETWEEN @fromdt AND @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND b.mode_flag = 'Cheque'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                    GROUP BY receipt_no , receipt_date , sr_number , std_name , class_name , mode_flag , chq_reject
                                    ORDER BY a.receipt_date , receipt_no";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt });

                    }
                    else
                    {
                        string query = @"SELECT 
                                        receipt_no,
                                        receipt_date,
                                        sr_number,
                                        std_name,
                                        class_name,
                                        SUM(amount) amount,
                                        mode_flag,
                                        chq_reject
                                    FROM
                                        (SELECT 
                                            receipt_no,
                                                receipt_date,
                                                a.sr_number,
                                                CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND c.class_id = d.class_id
                                                AND b.sr_number = d.sr_num
                                                AND receipt_date BETWEEN @fromdt and @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND a.mode_flag = 'Cash'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0 
                                                AND a.session = c.session
                                                AND c.session = d.session
                                                UNION ALL SELECT 
                                            receipt_no,
                                                receipt_date,
                                                0 sr_number,
                                                CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                c.class_name,
                                                IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                CASE mode_flag
                                                    WHEN 'Cheque' THEN 'Bank'
                                                    ELSE mode_flag
                                                END mode_flag,
                                                CASE mode_flag
                                                    WHEN 'Cash' THEN 'Cleared'
                                                    ELSE CASE
                                                        WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                        ELSE chq_reject
                                                    END
                                                END AS chq_reject
                                        FROM
                                            std_registration a, fees_receipt b, mst_class c
                                        WHERE
                                            a.reg_no = b.reg_no
                                                AND a.reg_date = b.reg_date
                                                AND a.session = b.session
                                                 AND b.session = c.session
                                                AND c.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_active = 'Y')
                                                AND a.std_class_id = c.class_id
                                                AND b.receipt_date BETWEEN @fromdt and @todt
                                                AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                AND b.mode_flag = 'Cash'
                                                AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                    GROUP BY receipt_no , receipt_date , sr_number , std_name , class_name , mode_flag , chq_reject
                                    ORDER BY a.receipt_date , receipt_no";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt });

                    }

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

                    if (mode == "Bank")
                    {
                        pt = new PdfPTable(12);
                    }
                    else
                    {
                        pt = new PdfPTable(9);
                    }

                    pt.WidthPercentage = 95f;

                    text = new Chunk("\n");
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.TOP_BORDER;
                    _cell.Colspan = 18;
                    pt.AddCell(_cell);


                    text = new Chunk("Fees statement for the period of " + fromdt.ToString("dd/MM/yyyy") + " to " + todt.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 12));
                    ph = new Phrase(text);
                    ph.Add("\n");
                    ph.Add("\n");
                    text.SetUnderline(0.1f, -2f);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 18;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = Rectangle.NO_BORDER;
                    pt.AddCell(_cell);




                    text = new Chunk("Rcpt No.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Rcpt Dt.", FontFactory.GetFont("Areal", 8));
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

                    text = new Chunk("Std Name.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 2;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Class.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Amt.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Mode.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    if (mode == "Bank")
                    {
                        text = new Chunk("Bank Name", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        text = new Chunk("Inst No.", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        text = new Chunk("Inst Date", FontFactory.GetFont("Areal", 8));
                        ph = new Phrase(text);
                        _cell = new PdfPCell(ph);
                        _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);
                    }

                    text = new Chunk("Status.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);

                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    decimal total = 0;

                    int line = 1;

                    foreach (var fee in result)
                    {

                        ph = new Phrase();
                        text = new Chunk(fee.receipt_no.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.receipt_date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        if (fee.sr_number == 0)
                            text = new Chunk("N/A", FontFactory.GetFont("Areal", 8));
                        else
                            text = new Chunk(fee.sr_number.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.std_name, FontFactory.GetFont("Areal", 8));
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

                        ph = new Phrase();
                        text = new Chunk(fee.mode_flag, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        if (mode == "Bank")
                        {
                            text = new Chunk(fee.bnk_name, FontFactory.GetFont("Areal", 8));
                            ph = new Phrase(text);
                            _cell = new PdfPCell(ph);
                            if (line == 2)
                                _cell.BackgroundColor = new BaseColor(224, 224, 224);
                            _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                            pt.AddCell(_cell);

                            text = new Chunk(fee.chq_no, FontFactory.GetFont("Areal", 8));
                            ph = new Phrase(text);
                            _cell = new PdfPCell(ph);
                            if (line == 2)
                                _cell.BackgroundColor = new BaseColor(224, 224, 224);
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);

                            text = new Chunk(fee.chq_date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 8));
                            ph = new Phrase(text);
                            _cell = new PdfPCell(ph);
                            if (line == 2)
                                _cell.BackgroundColor = new BaseColor(224, 224, 224);
                            _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pt.AddCell(_cell);
                        }


                        ph = new Phrase();
                        text = new Chunk(fee.chq_reject, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
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
                    _cell.Colspan = 6;
                    pt.AddCell(_cell);

                    text = new Chunk(total.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;

                    _cell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                    if (mode == "Bank")
                    {
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        _cell.Colspan = 2;
                    }
                    else
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;


                    pt.AddCell(_cell);

                    text = new Chunk("", FontFactory.GetFont("Areal", 10));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Border = Rectangle.BOTTOM_BORDER | Rectangle.TOP_BORDER | Rectangle.RIGHT_BORDER;
                    if (mode == "Bank")
                        _cell.Colspan = 4;
                    else
                        _cell.Colspan = 2;

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
        }


        public void pdfHeadWiseStatement(DateTime fromdt, DateTime todt, string mode, string session, int acc_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                MemoryStream ms = new MemoryStream();

                HttpContext.Current.Response.ContentType = "application/pdf";
                string name = "FR_" + fromdt.ToString("dd/MM/yyyy") + "_" + todt.ToString("dd/MM/yyyy") + ".pdf";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
                var doc = new Document(PageSize.A4);

                // MemoryStream stream = new MemoryStream();
                doc.SetMargins(0f, 0f, 10f, 70f);
                try
                {
                    IEnumerable<repDaily_reportMain> result;

                    if (mode == "Both")
                    {
                        string query = @"SELECT 
                                                *
                                            FROM
                                                (SELECT 
                                                    receipt_no,
                                                        receipt_date,
                                                        a.sr_number,
                                                        CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                        fees_name,
                                                        c.class_name,
                                                        IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                        CASE mode_flag
                                                            WHEN 'Cheque' THEN 'Bank'
                                                            ELSE mode_flag
                                                        END mode_flag,
                                                        CASE mode_flag
                                                            WHEN 'Cash' THEN 'Cleared'
                                                            ELSE CASE
                                                                WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                                ELSE chq_reject
                                                            END
                                                        END AS chq_reject
                                                FROM
                                                    fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                                WHERE
                                                    a.sr_number = b.sr_number
                                                        AND c.class_id = d.class_id
                                                        AND b.sr_number = d.sr_num
                                                        AND receipt_date BETWEEN @fromdt AND @todt
                                                        AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                        AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0
                                                        AND a.session = @session
                                                        AND a.session = c.session
                                                        AND c.session = d.session
                                                        AND a.acc_id = @acc_id UNION ALL SELECT 
                                                    receipt_no,
                                                        receipt_date,
                                                        0 sr_number,
                                                        CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                        fees_name,
                                                        c.class_name,
                                                        IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                        CASE mode_flag
                                                            WHEN 'Cheque' THEN 'Bank'
                                                            ELSE mode_flag
                                                        END mode_flag,
                                                        CASE mode_flag
                                                            WHEN 'Cash' THEN 'Cleared'
                                                            ELSE CASE
                                                                WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                                ELSE chq_reject
                                                            END
                                                        END AS chq_reject
                                                FROM
                                                    std_registration a, fees_receipt b, mst_class c
                                                WHERE
                                                    a.reg_no = b.reg_no
                                                        AND a.reg_date = b.reg_date
                                                        AND a.session = b.session
                                                        AND b.session = c.session
                                                        AND c.session = @session
                                                        AND a.std_class_id = c.class_id
                                                        AND b.acc_id = @acc_id
                                                        AND b.receipt_date BETWEEN @fromdt AND @todt
                                                        AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                        AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                            ORDER BY a.receipt_date";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt, acc_id = acc_id, session = session });

                    }
                    else if (mode == "Bank")
                    {
                        string query = @"SELECT 
                                            *
                                        FROM
                                            (SELECT 
                                                receipt_no,
                                                    receipt_date,
                                                    a.sr_number,
                                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                    fees_name,
                                                    c.class_name,
                                                    IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                    CASE mode_flag
                                                        WHEN 'Cheque' THEN 'Bank'
                                                        ELSE mode_flag
                                                    END mode_flag,
                                                    CASE mode_flag
                                                        WHEN 'Cash' THEN 'Cleared'
                                                        ELSE CASE
                                                            WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                            ELSE chq_reject
                                                        END
                                                    END AS chq_reject
                                            FROM
                                                fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                            WHERE
                                                a.sr_number = b.sr_number
                                                    AND c.class_id = d.class_id
                                                    AND b.sr_number = d.sr_num
                                                    AND receipt_date BETWEEN @fromdt AND @todt
                                                    AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                    AND a.mode_flag = 'Cheque'
                                                    AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0
                                                    AND a.session = @session
                                                    AND a.session = c.session
                                                    AND c.session = d.session
                                                    AND a.acc_id = @acc_id UNION ALL SELECT 
                                                receipt_no,
                                                    receipt_date,
                                                    0 sr_number,
                                                    CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                    fees_name,
                                                    c.class_name,
                                                    IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                    CASE mode_flag
                                                        WHEN 'Cheque' THEN 'Bank'
                                                        ELSE mode_flag
                                                    END mode_flag,
                                                    CASE mode_flag
                                                        WHEN 'Cash' THEN 'Cleared'
                                                        ELSE CASE
                                                            WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                            ELSE chq_reject
                                                        END
                                                    END AS chq_reject
                                            FROM
                                                std_registration a, fees_receipt b, mst_class c
                                            WHERE
                                                a.reg_no = b.reg_no
                                                    AND a.reg_date = b.reg_date
                                                    AND a.session = b.session
                                                    AND b.session = c.session
                                                    AND c.session = @session
                                                    AND a.std_class_id = c.class_id
                                                    AND b.acc_id = @acc_id
                                                    AND b.receipt_date BETWEEN @fromdt AND @todt
                                                    AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                    AND b.mode_flag = 'Cheque'
                                                    AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                        ORDER BY a.receipt_date";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt, acc_id = acc_id, session = session });

                    }
                    else
                    {
                        string query = @"SELECT 
                                            *
                                        FROM
                                            (SELECT 
                                                receipt_no,
                                                    receipt_date,
                                                    a.sr_number,
                                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                                    fees_name,
                                                    c.class_name,
                                                    IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                    CASE mode_flag
                                                        WHEN 'Cheque' THEN 'Bank'
                                                        ELSE mode_flag
                                                    END mode_flag,
                                                    CASE mode_flag
                                                        WHEN 'Cash' THEN 'Cleared'
                                                        ELSE CASE
                                                            WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                            ELSE chq_reject
                                                        END
                                                    END AS chq_reject
                                            FROM
                                                fees_receipt a, sr_register b, mst_class c, mst_std_class d
                                            WHERE
                                                a.sr_number = b.sr_number
                                                    AND c.class_id = d.class_id
                                                    AND b.sr_number = d.sr_num
                                                    AND receipt_date BETWEEN @fromdt AND @todt
                                                    AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                    AND a.mode_flag = 'Cash'
                                                    AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0
                                                    AND a.session = @session
                                                    AND a.session = c.session
                                                    AND c.session = d.session
                                                    AND a.acc_id = @acc_id UNION ALL SELECT 
                                                receipt_no,
                                                    receipt_date,
                                                    0 sr_number,
                                                    CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(a.std_last_name, '')) std_name,
                                                    fees_name,
                                                    c.class_name,
                                                    IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) amount,
                                                    CASE mode_flag
                                                        WHEN 'Cheque' THEN 'Bank'
                                                        ELSE mode_flag
                                                    END mode_flag,
                                                    CASE mode_flag
                                                        WHEN 'Cash' THEN 'Cleared'
                                                        ELSE CASE
                                                            WHEN chq_reject IS NULL THEN 'Not Cleared'
                                                            ELSE chq_reject
                                                        END
                                                    END AS chq_reject
                                            FROM
                                                std_registration a, fees_receipt b, mst_class c
                                            WHERE
                                                a.reg_no = b.reg_no
                                                    AND a.reg_date = b.reg_date
                                                    AND a.session = b.session
                                                    AND b.session = c.session
                                                    AND c.session = @session
                                                    AND a.std_class_id = c.class_id
                                                    AND b.acc_id = @acc_id
                                                    AND b.receipt_date BETWEEN @fromdt AND @todt
                                                    AND IFNULL(chq_reject, 'Cleared') = 'Cleared'
                                                    AND b.mode_flag = 'Cash'
                                                    AND IFNULL(amount, 0) + IFNULL(dc_fine, 0) - IFNULL(dc_discount, 0) != 0) a
                                        ORDER BY a.receipt_date";


                        result = con.Query<repDaily_reportMain>(query, new { fromdt = fromdt, todt = todt, acc_id = acc_id, session = session });

                    }

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



                    pt = new PdfPTable(17);

                    pt.WidthPercentage = 95f;

                    text = new Chunk("\n");
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Border = Rectangle.TOP_BORDER;
                    _cell.Colspan = 18;
                    pt.AddCell(_cell);


                    text = new Chunk("Fees statement for the period of " + fromdt.ToString("dd/MM/yyyy") + " to " + todt.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 12));
                    ph = new Phrase(text);
                    ph.Add("\n");
                    ph.Add("\n");
                    text.SetUnderline(0.1f, -2f);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 18;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = Rectangle.NO_BORDER;
                    pt.AddCell(_cell);



                    text = new Chunk("Rcpt No.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Rcpt Dt.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Adm No.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);

                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Std Name.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 3;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Fees.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.Colspan = 4;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Class.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Amt.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Mode.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk("Status.", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    decimal total = 0;

                    int line = 1;

                    foreach (var fee in result)
                    {

                        ph = new Phrase();
                        text = new Chunk(fee.receipt_no.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.receipt_date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        if (fee.sr_number == 0)
                            text = new Chunk("N/A", FontFactory.GetFont("Areal", 8));
                        else
                            text = new Chunk(fee.sr_number.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.std_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 3;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.fees_name, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 4;
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
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.amount.ToString(), FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.mode_flag, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                        pt.AddCell(_cell);

                        ph = new Phrase();
                        text = new Chunk(fee.chq_reject, FontFactory.GetFont("Areal", 8));
                        ph.Add(text);
                        ph.Add("\n");
                        _cell = new PdfPCell(ph);
                        _cell.Colspan = 2;
                        if (line == 2)
                            _cell.BackgroundColor = new BaseColor(224, 224, 224);
                        _cell.HorizontalAlignment = Element.ALIGN_CENTER;
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
                    _cell.Colspan = 12;
                    pt.AddCell(_cell);

                    text = new Chunk(total.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    _cell.Border = Rectangle.LEFT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                    _cell.Colspan = 2;
                    pt.AddCell(_cell);

                    text = new Chunk("", FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    _cell.Border = Rectangle.RIGHT_BORDER | Rectangle.TOP_BORDER | Rectangle.BOTTOM_BORDER;
                    _cell.Colspan = 3;
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