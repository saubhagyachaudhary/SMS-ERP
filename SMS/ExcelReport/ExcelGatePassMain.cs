using Dapper;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;

namespace SMS.ExcelReport
{
    public class ExcelGatePassMain
    {
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Affiliation_no = ConfigurationManager.AppSettings["Affiliation_no"].ToString();
        string School_code = ConfigurationManager.AppSettings["School_code"].ToString();

        private Bitmap Base64StringToBitmap(string base64String)
        {
            var bitmapData = Convert.FromBase64String(FixBase64ForImage(base64String));
            var streamBitmap = new System.IO.MemoryStream(bitmapData);
            var bitmap = new Bitmap((Bitmap)Image.FromStream(streamBitmap));
            return bitmap;
        }

        private string FixBase64ForImage(string Image)
        {
            var sbText = new System.Text.StringBuilder(Image, Image.Length);
            sbText.Replace("\r\n", String.Empty);
            sbText.Replace(" ", String.Empty);
            return sbText.ToString();
        }

        public byte[] gate_pass(string session ,int gate_pass_no,string image)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    
                    string query = @"SELECT 
                                        a.session,
                                        gate_pass_no,
                                        b.sr_number,
                                        date_time,
                                        std_relation,
                                        escorter_name,
                                        escorter_address,
                                        reason,
                                        CONCAT(IFNULL(std_first_name, ''),
                                                ' ',
                                                IFNULL(std_last_name, '')) std_name,
                                        CONCAT(IFNULL(class_name, ''),
                                                ' ',
                                                IFNULL(section_name, '')) std_class,
                                        std_father_name,
                                        COALESCE(std_contact, std_contact1, std_contact2) contact_no
                                    FROM
                                        std_halfday_log a,
                                        sr_register b,
                                        mst_std_class c,
                                        mst_std_section d,
                                        mst_class e,
                                        mst_section f
                                    WHERE
                                        a.session = @session
                                            AND a.session = c.session
                                            AND c.session = d.session
                                            AND d.session = e.session
                                            AND e.session = f.session
                                            AND a.sr_number = b.sr_number
                                            AND b.sr_number = c.sr_num
                                            AND c.sr_num = d.sr_num
                                            AND c.class_id = e.class_id
                                            AND d.section_id = f.section_id
                                            AND a.gate_pass_no = @gate_pass_no";

                    var result = con.Query<ExcelGatePass>(query, new { session = session, gate_pass_no = gate_pass_no }).SingleOrDefault();

                    string gatePassNo = gate_pass_no.ToString().PadLeft(4, '0');

                    ExcelPackage pck = new ExcelPackage();
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Gate Pass");

                    ws.PrinterSettings.TopMargin = 1m / 2.54m;
                    ws.PrinterSettings.BottomMargin = 1m / 2.54m;
                    ws.PrinterSettings.LeftMargin = 1.8m / 2.54m;
                    ws.PrinterSettings.RightMargin = 1.8m / 2.54m;
                    ws.PrinterSettings.HeaderMargin = 0.0m / 2.54m;
                    ws.PrinterSettings.FooterMargin = 0.0m / 2.54m;
                    ws.PrinterSettings.HorizontalCentered = false;

                    ws.Column(1).Width = ExcelTc_form.GetTrueColumnWidth(13.18);
                    ws.Column(2).Width = ExcelTc_form.GetTrueColumnWidth(11.09);
                    ws.Column(3).Width = ExcelTc_form.GetTrueColumnWidth(8.09);
                    ws.Column(4).Width = ExcelTc_form.GetTrueColumnWidth(8.09);
                    ws.Column(5).Width = ExcelTc_form.GetTrueColumnWidth(9.18);
                    ws.Column(6).Width = ExcelTc_form.GetTrueColumnWidth(8.09);
                    ws.Column(7).Width = ExcelTc_form.GetTrueColumnWidth(8.09);
                    ws.Column(8).Width = ExcelTc_form.GetTrueColumnWidth(14.55);

                    for (int i = 2; i <= 15; i++)
                    {
                        ws.Row(i).Height = 17;
                    }

                    ws.Row(1).Height = 108.8;

                    using (ExcelRange Rng = ws.Cells["A1:H1"])
                    {
                        Rng.Merge = true;
                        Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        Rng.Style.WrapText = true;

                        ExcelRichTextCollection RichTxtCollection = Rng.RichText;
                        ExcelRichText RichText = RichTxtCollection.Add("Gate Pass\n");
                        RichText.Size = 14;

                        RichText = RichTxtCollection.Add(SchoolName + "\n");
                        RichText.Size = 28;
                        RichText.Bold = true;

                        RichText = RichTxtCollection.Add(Address + "\n");
                        RichText.Size = 11;


                        RichText = RichTxtCollection.Add(Affiliation);
                        RichText.Size = 11;
                        RichText.Bold = false;

                        Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    if(image == "../../images/person.png")
                    {
                        using (System.Drawing.Image img = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("/images/person.png")))
                        {
                            var excelImage = ws.Drawings.AddPicture("Picture", img);

                            //add the image to row 20, column E
                            excelImage.SetPosition(3, 0, 5, 0);

                            excelImage.SetSize(233, 134);
                        }
                    }
                    else
                    {
                        var img = Base64StringToBitmap(image.Remove(0, 22));

                        var excelImage = ws.Drawings.AddPicture("Picture", img);

                        //add the image to row 20, column E
                        excelImage.SetPosition(3, 0, 5, 0);

                        excelImage.SetSize(233, 134);
                    }

                   
                    

                    ws.Cells["A2"].Value = "Gate Pass No: ";
                    ws.Cells["A2"].Style.Font.Name = "Calibri";
                    ws.Cells["A2"].Style.Font.Size = 11;
                    ws.Cells["A2"].Style.Font.Bold = true;
                    ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B2"].Value = session + "/" + gatePassNo.ToString(); ;
                    ws.Cells["B2"].Style.Font.Name = "Calibri";
                    ws.Cells["B2"].Style.Font.Size = 11;
                    ws.Cells["B2"].Style.Font.UnderLine = true;
                    ws.Cells["B2"].Style.Font.Italic = true;
                    ws.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["D2"].Value = "Adm No: ";
                    ws.Cells["D2"].Style.Font.Name = "Calibri";
                    ws.Cells["D2"].Style.Font.Size = 11;
                    ws.Cells["D2"].Style.Font.Bold = true;
                    ws.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["D2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["E2"].Value = result.sr_number.ToString();
                    ws.Cells["E2"].Style.Font.Name = "Calibri";
                    ws.Cells["E2"].Style.Font.Size = 11;
                    ws.Cells["E2"].Style.Font.UnderLine = true;
                    ws.Cells["E2"].Style.Font.Italic = true;
                    ws.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["E2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["G2"].Value = "Date: ";
                    ws.Cells["G2"].Style.Font.Name = "Calibri";
                    ws.Cells["G2"].Style.Font.Size = 11;
                    ws.Cells["G2"].Style.Font.Bold = true;
                    ws.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["G2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["H2"].Value = result.date_time.ToString("dd/MM/yyyy hh:mm");
                    ws.Cells["H2"].Style.Font.Name = "Calibri";
                    ws.Cells["H2"].Style.Font.Size = 11;
                    ws.Cells["H2"].Style.Font.UnderLine = true;
                    ws.Cells["H2"].Style.Font.Italic = true;
                    ws.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["H2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A4"].Value = "Name ";
                    ws.Cells["A4"].Style.Font.Name = "Calibri";
                    ws.Cells["A4"].Style.Font.Size = 11;
                    ws.Cells["A4"].Style.Font.Bold = true;
                    ws.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B4"].Value = result.std_name;
                    ws.Cells["B4"].Style.Font.Name = "Calibri";
                    ws.Cells["B4"].Style.Font.Size = 11;
                    ws.Cells["B4"].Style.Font.UnderLine = true;
                    ws.Cells["B4"].Style.Font.Italic = true;
                    ws.Cells["B4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A5"].Value = "Class ";
                    ws.Cells["A5"].Style.Font.Name = "Calibri";
                    ws.Cells["A5"].Style.Font.Size = 11;
                    ws.Cells["A5"].Style.Font.Bold = true;
                    ws.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B5"].Value = result.std_class;
                    ws.Cells["B5"].Style.Font.Name = "Calibri";
                    ws.Cells["B5"].Style.Font.Size = 11;
                    ws.Cells["B5"].Style.Font.UnderLine = true;
                    ws.Cells["B5"].Style.Font.Italic = true;
                    ws.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A6"].Value = "Father Name ";
                    ws.Cells["A6"].Style.Font.Name = "Calibri";
                    ws.Cells["A6"].Style.Font.Size = 11;
                    ws.Cells["A6"].Style.Font.Bold = true;
                    ws.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B6"].Value = result.std_father_name;
                    ws.Cells["B6"].Style.Font.Name = "Calibri";
                    ws.Cells["B6"].Style.Font.Size = 11;
                    ws.Cells["B6"].Style.Font.UnderLine = true;
                    ws.Cells["B6"].Style.Font.Italic = true;
                    ws.Cells["B6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A7"].Value = "Contact No ";
                    ws.Cells["A7"].Style.Font.Name = "Calibri";
                    ws.Cells["A7"].Style.Font.Size = 11;
                    ws.Cells["A7"].Style.Font.Bold = true;
                    ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B7"].Value = result.contact_no;
                    ws.Cells["B7"].Style.Font.Name = "Calibri";
                    ws.Cells["B7"].Style.Font.Size = 11;
                    ws.Cells["B7"].Style.Font.UnderLine = true;
                    ws.Cells["B7"].Style.Font.Italic = true;
                    ws.Cells["B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A8"].Value = "Person Name ";
                    ws.Cells["A8"].Style.Font.Name = "Calibri";
                    ws.Cells["A8"].Style.Font.Size = 11;
                    ws.Cells["A8"].Style.Font.Bold = true;
                    ws.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B8"].Value = result.escorter_name;
                    ws.Cells["B8"].Style.Font.Name = "Calibri";
                    ws.Cells["B8"].Style.Font.Size = 11;
                    ws.Cells["B8"].Style.Font.UnderLine = true;
                    ws.Cells["B8"].Style.Font.Italic = true;
                    ws.Cells["B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A9"].Value = "Relationship ";
                    ws.Cells["A9"].Style.Font.Name = "Calibri";
                    ws.Cells["A9"].Style.Font.Size = 11;
                    ws.Cells["A9"].Style.Font.Bold = true;
                    ws.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B9"].Value = result.std_relation;
                    ws.Cells["B9"].Style.Font.Name = "Calibri";
                    ws.Cells["B9"].Style.Font.Size = 11;
                    ws.Cells["B9"].Style.Font.UnderLine = true;
                    ws.Cells["B9"].Style.Font.Italic = true;
                    ws.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A10"].Value = "Person Address ";
                    ws.Cells["A10"].Style.Font.Name = "Calibri";
                    ws.Cells["A10"].Style.Font.Size = 11;
                    ws.Cells["A10"].Style.Font.Bold = true;
                    ws.Cells["A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B10"].Value = result.escorter_address;
                    ws.Cells["B10"].Style.Font.Name = "Calibri";
                    ws.Cells["B10"].Style.Font.Size = 11;
                    ws.Cells["B10"].Style.Font.UnderLine = true;
                    ws.Cells["B10"].Style.Font.Italic = true;
                    ws.Cells["B10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A11"].Value = "Reason ";
                    ws.Cells["A11"].Style.Font.Name = "Calibri";
                    ws.Cells["A11"].Style.Font.Size = 11;
                    ws.Cells["A11"].Style.Font.Bold = true;
                    ws.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["B11"].Value = result.reason;
                    ws.Cells["B11"].Style.Font.Name = "Calibri";
                    ws.Cells["B11"].Style.Font.Size = 11;
                    ws.Cells["B11"].Style.Font.UnderLine = true;
                    ws.Cells["B11"].Style.Font.Italic = true;
                    ws.Cells["B11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["B11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["A15"].Value = "Parents Sign ";
                    ws.Cells["A15"].Style.Font.Name = "Calibri";
                    ws.Cells["A15"].Style.Font.Size = 11;
                    ws.Cells["A15"].Style.Font.Bold = true;
                    ws.Cells["A15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["A15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["D15"].Value = "Auth Sign. ";
                    ws.Cells["D15"].Style.Font.Name = "Calibri";
                    ws.Cells["D15"].Style.Font.Size = 11;
                    ws.Cells["D15"].Style.Font.Bold = true;
                    ws.Cells["D15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["D15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                    ws.Cells["H15"].Value = "Incharge Sign. ";
                    ws.Cells["H15"].Style.Font.Name = "Calibri";
                    ws.Cells["H15"].Style.Font.Size = 11;
                    ws.Cells["H15"].Style.Font.Bold = true;
                    ws.Cells["H15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                    ws.Cells["H15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                    return pck.GetAsByteArray();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

          
        }
    }

    public class ExcelGatePass
    {
        public int gate_pass_no { get; set; }

        public string session { get; set; }

        public int sr_number { get; set; }

        public DateTime date_time { get; set; }

        public string std_relation { get; set; }

        public string escorter_name { get; set; }

        public string escorter_address { get; set; }

        public string reason { get; set; }

        public string std_name { get; set; }

        public string std_class { get; set; }

        public string std_father_name { get; set; }

        public string contact_no { get; set; }

    }
}