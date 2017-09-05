using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using iTextSharp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Data.SqlClient;
using System.Configuration;
using SMS.Models;
using Dapper;
using System.Diagnostics;
using System.Web.Mvc;

namespace SMS.report
{
    public class repFees_receipt
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void pdf(String session,int receipt_no, string receipt_date)
        {
            HttpContext.Current.Response.ContentType = "application/pdf";
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=GridViewExport.pdf");
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            //string path = "E:\\HPS" + "\\" + receipt_no.ToString()+"("+receipt_date.ToString("dd-MM-yyyy")+")"+ ".pdf";
            var doc = new Document(PageSize.A6);

           // MemoryStream stream = new MemoryStream();
            doc.SetMargins(0f,0f,0f,0f);
            try
            {
                String query = @"SELECT [fees_name]
                              ,[sr_number]
                              ,[class_id]
                              ,[nature]
                              ,[amount]
                              ,[reg_no]
                              ,[reg_date]
                          FROM [SMS].[dbo].[fees_receipt]
                          where fin_id = @fin_id
                          and convert(varchar,receipt_date,103) = @receipt_date
                          and receipt_no = @receipt_no";

                fees_receipt result = con.Query<fees_receipt>(query,new { fin_id = session,receipt_date = receipt_date ,receipt_no = receipt_no}).SingleOrDefault();

                rep_fees rep = new rep_fees();

                if (result.sr_number == 0)
                {
                    query = @"SELECT [reg_no] num  
                               ,[std_first_name] name
                              ,[std_father_name] father_name
	                          ,b.class_name
	                           FROM [SMS].[dbo].[std_registration] a, [SMS].[dbo].[mst_class] b
	                           where a.std_class_id = b.class_id
	                           and reg_no = @reg_no
	                           and reg_date = @reg_date";

                    rep = con.Query<rep_fees>(query, new { reg_no = result.reg_no, reg_date = result.reg_date }).SingleOrDefault();
                }
                else
                {
                    query = @"SELECT [sr_number] num   
                                   ,[std_first_name] name
                                  ,[std_father_name] father_name
	                              ,CONCAT(c.class_name,' ',b.section_name) class_name
                                  FROM [SMS].[dbo].[sr_register] a,[SMS].[dbo].[mst_section] b,[SMS].[dbo].[mst_class] c
	                              where a.std_section_id = b.section_id
	                              and b.class_id = c.class_id
                                  and sr_number = @sr_number";

                    rep = con.Query<rep_fees>(query, new { sr_number = result.sr_number}).SingleOrDefault();
                }

                rep.receipt_no = receipt_no;
                rep.receipt_date = receipt_date;
                rep.fees_name = result.fees_name;
                rep.amount = result.amount;


                PdfWriter.GetInstance(doc, HttpContext.Current.Response.OutputStream);//stream).CloseStream = false;//new FileStream(path, FileMode.Create));


                doc.Open();
                string imageURL = "E:\\HPS\\logo.jpg";
                Image jpg = Image.GetInstance(imageURL);
                jpg.ScaleAbsolute(30f,30f);
             

                PdfPTable pt = new PdfPTable(4);
                PdfPCell _cell;
                Chunk text;
                Phrase ph;

                _cell = new PdfPCell(jpg);
                _cell.Border = 0;
                pt.AddCell(_cell);
                

                text = new Chunk("HARITI PUBLIC SCHOOL", FontFactory.GetFont("Areal", 12));
                ph = new Phrase();
                ph.Add(text);
                /* _cell = new PdfPCell(ph);
                 _cell.Colspan = 3;
                 _cell.HorizontalAlignment = Element.ALIGN_CENTER;*/

                //  _cell.Border = 0;
                // pt.AddCell(_cell);
                ph.Add("\n");
                text = new Chunk("Nh-24 Village Ballia, Dhaneta, Meerganj Bareilly-243504", FontFactory.GetFont("Areal", 6));
                ph.Add(text);

                _cell = new PdfPCell(ph);
                  _cell.Colspan = 3;
                  _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                  _cell.Border = 0;

                  pt.AddCell(_cell);
               
              

               

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

                text = new Chunk(rep.receipt_no.ToString(), FontFactory.GetFont("Areal", 10));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                pt.AddCell(_cell);

                text = new Chunk("Receipt Date:" , FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                pt.AddCell(_cell);

                text = new Chunk(rep.receipt_date, FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                pt.AddCell(_cell);

                if (result.sr_number == 0)
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

                if (result.sr_number == 0)
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

                text = new Chunk("Particulars", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 3;
                _cell.BackgroundColor = BaseColor.GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Amount", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.BackgroundColor = BaseColor.GRAY;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk(rep.fees_name, FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                ph.Add("\n");
               _cell = new PdfPCell(ph);
                _cell.FixedHeight = 150;
                _cell.Colspan = 3;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                ph = new Phrase();
                text = new Chunk(rep.amount.ToString(), FontFactory.GetFont("Areal", 8));
                ph.Add(text);
                ph.Add("\n");
                ph.Add("\n");
                _cell = new PdfPCell(ph);

                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Total", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 3;
                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk(rep.amount.ToString(), FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);

                _cell.HorizontalAlignment = Element.ALIGN_CENTER;
                pt.AddCell(_cell);

                text = new Chunk("Received with thanks: ", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Border = 0;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);

                text = new Chunk(NumbersToWords(Decimal.ToInt32(rep.amount))+" Only", FontFactory.GetFont("Areal", 8));
                text.SetUnderline(0.1f, -1f);
                
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 3;
                _cell.Border = 0;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                pt.AddCell(_cell);


                text = new Chunk("\n");
                ph = new Phrase(text);

                _cell = new PdfPCell(ph);
                _cell.Colspan = 4;
                _cell.Border = 0;
                pt.AddCell(_cell);

                text = new Chunk("Paid By: "+"Cash", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                pt.AddCell(_cell);
                
                text = new Chunk("Cashier", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _cell.Border = 0;
                pt.AddCell(_cell);

                text = new Chunk("\n");
                ph = new Phrase(text);

                _cell = new PdfPCell(ph);
                _cell.Colspan = 4;
                _cell.Border = 0;
                pt.AddCell(_cell);

                text = new Chunk("\n");
                ph = new Phrase(text);

                _cell = new PdfPCell(ph);
                _cell.Colspan = 4;
                _cell.Border = 0;
                pt.AddCell(_cell);

                /*text = new Chunk("Total Dues: " + "7000", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_LEFT;
                _cell.Border = 0;
                pt.AddCell(_cell);*/

                text = new Chunk("Seal & Sign", FontFactory.GetFont("Areal", 8));
                ph = new Phrase(text);
                _cell = new PdfPCell(ph);
                _cell.Colspan = 2;
                _cell.HorizontalAlignment = Element.ALIGN_RIGHT;
                _cell.Border = 0;
                pt.AddCell(_cell);



                doc.Add(pt);



            }
            catch (DocumentException dex)
            {
                throw (dex);
            }
            catch (IOException io)
            {
                throw (io);
            }
            finally
            {
                doc.Close();

                //stream.Flush();

                //   byte[] byteInfo = stream.ToArray();
                //  stream.Write(byteInfo,0,byteInfo.Length);
                // stream.Position = 0;

                HttpContext.Current.Response.Write(doc);
                HttpContext.Current.Response.End();

            
                /*Process process = new Process();
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = path;
                process.Start();*/
            }

           // return new FileStreamResult(stream, "application/pdf");
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
}