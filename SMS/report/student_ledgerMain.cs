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
    public class student_ledgerMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public void pdfStudent_ledger(int sr_num,string session)
        {
            var doc = new Document(PageSize.A4);
            MemoryStream ms = new MemoryStream();

            try
            {
              

                HttpContext.Current.Response.ContentType = "application/pdf";
                string name = "Ledger_" + sr_num + ".pdf";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline;filename=" + name);
                HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

                //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
               

                // MemoryStream stream = new MemoryStream();
                doc.SetMargins(0f, 0f, 10f, 70f);

                IEnumerable<student_ledger> result;

                string query = @"SELECT 
                                    a.sr_number sr_num,
                                    CONCAT(IFNULL(a.std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, '')) std_name,
                                    std_father_name,
                                    std_dob,
                                    CONCAT(IFNULL(std_address, ''),
                                            ' ',
                                            IFNULL(std_address1, ''),
                                            ' ',
                                            IFNULL(std_address2, ''),
                                            ' ',
                                            IFNULL(std_district, '')) address,
                                    std_email,
                                    COALESCE(std_contact, std_contact1, std_contact2) contact,
                                    c.class_name,
                                    d.pickup_point
                                FROM
                                    sr_register a,
                                    mst_std_class b,
                                    mst_class c,
                                    mst_transport d
                                WHERE
                                    a.sr_number = b.sr_num
                                        AND b.class_id = c.class_id
                                        AND a.sr_number = @sr_num
                                        AND b.session = @session
                                        AND b.session = d.session
                                        AND d.session = c.session
                                        AND a.std_pickup_id = d.pickup_id";

                student_ledger std_ledger = con.Query<student_ledger>(query, new { sr_num = sr_num, session = session }).SingleOrDefault();

                query = @"SELECT 
                                *
                            FROM
                                (SELECT 
                                    CONCAT(b.acc_name, ' ', a.month_name) fees_name,
                                        a.acc_id,
                                        NULL receipt_date,
                                        NULL receipt_no,
                                        a.outstd_amount out_standing,
                                        NULL paid,
                                        serial,
                                        month_no,
                                        dc_fine,
                                        NULL dc_discount
                                FROM
                                    out_standing a, mst_acc_head b
                                WHERE
                                    a.sr_number = @sr_num
                                        AND a.acc_id = b.acc_id
                                        AND a.session = @session
                                        AND a.month_no BETWEEN 4 AND 12) one 
                            UNION ALL SELECT 
                                *
                            FROM
                                (SELECT 
                                    a.fees_name,
                                        a.acc_id,
                                        a.receipt_date,
                                        a.receipt_no,
                                        NULL paid,
                                        IFNULL(a.amount, 0) + IFNULL(a.dc_fine, 0) amount,
                                        a.serial,
                                        b.month_no,
                                        NULL,
                                        a.dc_discount
                                FROM
                                    fees_receipt a, out_standing b
                                WHERE
                                    a.sr_number = @sr_num
                                        AND a.serial = b.serial
                                        AND a.session = b.session
                                        AND a.session = @session
                                        AND ifnull(a.chq_reject,'Cleared') != 'Bounce'
                                        AND b.month_no BETWEEN 4 AND 12) two
                            ORDER BY month_no , acc_id , receipt_date , serial";

                result = con.Query<student_ledger>(query, new { sr_num = sr_num, session = session });


                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream).PageEvent = new PDFFooter();



                doc.Open();
                // string imageURL = "E:\\HPS\\logo.jpg";
                string imageURL = System.Web.Hosting.HostingEnvironment.MapPath("/images/logo.jpg");
                Image jpg = Image.GetInstance(imageURL);
                jpg.ScaleAbsolute(50f, 50f);


                PdfPTable pt = new PdfPTable(6);
                pt.WidthPercentage = 90f;

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

                pt = new PdfPTable(4);
                pt.WidthPercentage = 90f;

                text = new Chunk("\n");
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = Rectangle.NO_BORDER;
                _cell.Colspan = 4;
                pt.AddCell(_cell);


                text = new Chunk("Account statement as on Date: " + System.DateTime.Now.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 12));
                ph = new Phrase(text);
                ph.Add("\n");
                ph.Add("\n");
                text.SetUnderline(0.1f, -2f);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 4;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                _cell.Border = Rectangle.NO_BORDER;
                pt.AddCell(_cell);

                text = new Chunk("Student Name", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(std_ledger.std_name, FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk("Admission No", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(sr_num.ToString(), FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk("Father's Name", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(std_ledger.std_father_name, FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);
            
                text = new Chunk("Date of birth", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(std_ledger.std_dob.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk("Contact Number", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(std_ledger.contact.ToString(), FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk("Class", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(std_ledger.class_name, FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk("Pickup Point", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(std_ledger.pickup_point, FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk("Session", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(session, FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk("Address", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(std_ledger.address, FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 3;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

               


                doc.Add(pt);

                pt = new PdfPTable(13);
                pt.HeaderRows = 2;
                pt.WidthPercentage = 90f;

                text = new Chunk("\n");
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = Rectangle.NO_BORDER;
                _cell.Colspan = 13;
                pt.AddCell(_cell);


                text = new Chunk("Sr. No", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Fees Head", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 4;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Receipt Date", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Rcpt No.", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);



                text = new Chunk("Fees Amt", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Paid Amt", FontFactory.GetFont("Areal", 8));
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

                text = new Chunk("Discount", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Balance", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.LIGHT_GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                int sr_no = 0;

                decimal balance = 0;

                int line = 1;

                foreach (var list in result)
                {
                    sr_no++;
                  
                    text = new Chunk(sr_no.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if(line == 2)
                        _cell.BackgroundColor = new BaseColor(224,224,224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk(list.fees_name, FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    if (list.receipt_date == System.DateTime.MinValue)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.receipt_date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 8));
                    }
                    
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    if (list.receipt_no == 0)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.receipt_no.ToString(), FontFactory.GetFont("Areal", 8));
                    }   
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    if (list.out_standing == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.out_standing.ToString(), FontFactory.GetFont("Areal", 8));
                    }
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    if (list.paid == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.paid.ToString(), FontFactory.GetFont("Areal", 8));
                    }
                    
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    if (list.dc_fine == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.dc_fine.ToString(), FontFactory.GetFont("Areal", 8));
                    }
                    
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    if (list.dc_discount == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.dc_discount.ToString(), FontFactory.GetFont("Areal", 8));
                    }
                   
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    text = new Chunk((balance + (list.out_standing + list.dc_fine - list.paid)).ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    balance = balance + list.out_standing + list.dc_fine - list.paid;

                    if (line == 2)
                        line = 1;
                    else
                        line++;
                }

                query = @"SELECT 
                                *
                            FROM
                                (SELECT 
                                    CONCAT(b.acc_name, ' ', a.month_name) fees_name,
                                        a.acc_id,
                                        NULL receipt_date,
                                        NULL receipt_no,
                                        a.outstd_amount out_standing,
                                        NULL paid,
                                        serial,
                                        month_no,
                                        dc_fine,
                                        NULL dc_discount
                                FROM
                                    out_standing a, mst_acc_head b
                                WHERE
                                    a.sr_number = @sr_num
                                        AND a.acc_id = b.acc_id
                                        AND a.session = @session
                                        AND a.month_no BETWEEN 1 AND 3) one 
                            UNION ALL SELECT 
                                *
                            FROM
                                (SELECT 
                                    a.fees_name,
                                        a.acc_id,
                                        a.receipt_date,
                                        a.receipt_no,
                                        NULL paid,
                                        IFNULL(a.amount, 0) + IFNULL(a.dc_fine, 0) amount,
                                        a.serial,
                                        b.month_no,
                                        NULL,
                                        a.dc_discount
                                FROM
                                    fees_receipt a, out_standing b
                                WHERE
                                    a.sr_number = @sr_num
                                        AND a.serial = b.serial
                                        AND a.session = b.session
                                        AND a.session = @session
                                        AND ifnull(a.chq_reject,'Cleared') != 'Bounce'
                                        AND b.month_no BETWEEN 1 AND 3) two
                            ORDER BY month_no , acc_id , receipt_date , serial";

                result = con.Query<student_ledger>(query, new { sr_num = sr_num,session = session });

                foreach (var list in result)
                {
                    sr_no++;

                    text = new Chunk(sr_no.ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    text = new Chunk(list.fees_name, FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 4;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                    pt.AddCell(_cell);

                    if (list.receipt_date == System.DateTime.MinValue)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.receipt_date.ToString("dd/MM/yyyy"), FontFactory.GetFont("Areal", 8));
                    }

                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    _cell.Colspan = 2;
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    if (list.receipt_no == 0)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.receipt_no.ToString(), FontFactory.GetFont("Areal", 8));
                    }
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                    pt.AddCell(_cell);

                    if (list.out_standing == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.out_standing.ToString(), FontFactory.GetFont("Areal", 8));
                    }
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    if (list.paid == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.paid.ToString(), FontFactory.GetFont("Areal", 8));
                    }

                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    if (list.dc_fine == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.dc_fine.ToString(), FontFactory.GetFont("Areal", 8));
                    }

                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    if (list.dc_discount == 0m)
                    {
                        text = new Chunk(" ", FontFactory.GetFont("Areal", 8));
                    }
                    else
                    {
                        text = new Chunk(list.dc_discount.ToString(), FontFactory.GetFont("Areal", 8));
                    }

                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    text = new Chunk((balance + (list.out_standing + list.dc_fine - list.paid)).ToString(), FontFactory.GetFont("Areal", 8));
                    ph = new Phrase(text);
                    _cell = new PdfPCell(ph);
                    if (line == 2)
                        _cell.BackgroundColor = new BaseColor(224, 224, 224);
                    _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                    pt.AddCell(_cell);

                    balance = balance + list.out_standing + list.dc_fine - list.paid;

                    if (line == 2)
                        line = 1;
                    else
                        line++;
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

        public class PDFFooter : PdfPageEventHelper
        {
            string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
            string Address = ConfigurationManager.AppSettings["Address"].ToString();
            string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
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