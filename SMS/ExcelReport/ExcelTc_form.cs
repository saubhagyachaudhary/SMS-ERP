using Dapper;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SMS.ExcelReport
{
    public class tc_details
    {
        public int sr_number { get; set; }

        public string std_name { get; set; }

        public string std_father { get; set; }

        public string std_mother { get; set; }

        public DateTime std_dob { get; set; }

        public string std_last_school { get; set; }

        public string std_nationality { get; set; }

        public string std_category { get; set; }

        public DateTime std_admission_date { get; set; }

        public string std_admission_class { get; set; }

        public DateTime nso_date { get; set; }

        public string std_pass_class { get; set; }

        public int working_days { get; set; }

        public int present_days { get; set; }


    }

    public class tc_class_details
    {
        public DateTime session_start_date { get; set; }

        public DateTime session_end_date { get; set; }

        public string class_name { get; set; }

        public DateTime std_admission_date { get; set; }

        public DateTime nso_date { get; set; }

        public string session { get; set; }

    }

    public class ExcelCBSE_TC_form
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();
        string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
        string Affiliation_no = ConfigurationManager.AppSettings["Affiliation_no"].ToString();
        string School_code = ConfigurationManager.AppSettings["School_code"].ToString();

        public byte[] Generate_TC(int sr_number, int user_id)
        {
            try
            {
                string query = @"SELECT 
                                        sr_number,
                                        CONCAT(IFNULL(std_first_name, ''),
                                                ' ',
                                                IFNULL(std_last_name, '')) std_name,
                                        std_mother_name std_mother,
                                        std_father_name std_father,
                                        std_nationality,
                                        std_category,
                                        std_admission_date,
                                        std_admission_class,
                                        std_dob,
                                        (SELECT 
                                                class_name
                                            FROM
                                                mst_std_Class a,
                                                mst_class b
                                            WHERE
                                                sr_num = a.sr_number
                                                    AND a.session = b.session
                                                    AND a.class_id = b.class_id
                                                    AND a.session != (SELECT 
                                                        session
                                                    FROM
                                                        mst_session
                                                    WHERE
                                                        session_finalize = 'Y')
                                            ORDER BY order_by
                                            LIMIT 1) std_pass_class,
                                        (SELECT 
                                                COUNT(*)
                                            FROM
                                                attendance_register
                                            WHERE
                                                sr_num = a.sr_number
                                                    AND session = (SELECT 
                                                        session
                                                    FROM
                                                        mst_session
                                                    WHERE
                                                        session_finalize != 'Y'
                                                    ORDER BY session_start_date DESC
                                                    LIMIT 1)) working_days,
                                        (SELECT 
                                                COUNT(*)
                                            FROM
                                                attendance_register
                                            WHERE
                                                sr_num = a.sr_number
                                                    AND session = (SELECT 
                                                        session
                                                    FROM
                                                        mst_session
                                                    WHERE
                                                        session_finalize != 'Y'
                                                    ORDER BY session_start_date DESC
                                                    LIMIT 1)
                                                    AND attendance = 1) present_days,
                                        nso_date
                                    FROM
                                        sr_register a
                                    WHERE
                                        std_active = 'N' AND sr_number = @sr_number";

                tc_details result = con.Query<tc_details>(query, new { sr_number = sr_number }).SingleOrDefault();


                mst_sessionMain session = new mst_sessionMain();
                string sess = session.findFinal_Session();

                string max_id = @"select lpad(ifnull(max(tc_no),0)+1,4,0) from tc_register where session = @session";

                string tc_no = con.ExecuteScalar<string>(max_id, new { session = sess });

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TC");

                ws.PrinterSettings.TopMargin = 0.25m;
                ws.PrinterSettings.BottomMargin = 0.25m;
                ws.PrinterSettings.LeftMargin = 0.25m;
                ws.PrinterSettings.RightMargin = 0.25m;
                ws.PrinterSettings.HeaderMargin = 0.3m;
                ws.PrinterSettings.FooterMargin = 0.3m;
                ws.PrinterSettings.HorizontalCentered = true;

                ws.Column(1).Width = ExcelTc_form.GetTrueColumnWidth(8.14);
                ws.Column(2).Width = ExcelTc_form.GetTrueColumnWidth(9.86);
                ws.Column(3).Width = ExcelTc_form.GetTrueColumnWidth(8.43);
                ws.Column(4).Width = ExcelTc_form.GetTrueColumnWidth(8.43);
                ws.Column(5).Width = ExcelTc_form.GetTrueColumnWidth(6.71);
                ws.Column(6).Width = ExcelTc_form.GetTrueColumnWidth(10.86);
                ws.Column(7).Width = ExcelTc_form.GetTrueColumnWidth(10.29);
                ws.Column(8).Width = ExcelTc_form.GetTrueColumnWidth(8.43);
                ws.Column(9).Width = ExcelTc_form.GetTrueColumnWidth(11.71);
                ws.Column(10).Width = ExcelTc_form.GetTrueColumnWidth(10.29);

                for(int i = 7; i<= 31; i++)
                {
                    ws.Row(i).Height = 19.5;
                }
               

                ws.Row(1).Height = 108.75;

                using (ExcelRange Rng = ws.Cells["A1:J1"])
                {
                    Rng.Merge = true;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    Rng.Style.WrapText = true;
                     
                    ExcelRichTextCollection RichTxtCollection = Rng.RichText;
                    ExcelRichText RichText = RichTxtCollection.Add("TRANSFER CERTIFICATE\n");
                    RichText.Size = 14;

                    RichText = RichTxtCollection.Add(SchoolName+"\n");
                    RichText.Size = 28;
                    RichText.Bold = true;

                    RichText = RichTxtCollection.Add(Address+"\n");
                    RichText.Size = 11;
                    

                    RichText = RichTxtCollection.Add(Affiliation);
                    RichText.Size = 11;
                    RichText.Bold = false;

                    Rng.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("/images/logo.jpg")))
                {
                    var excelImage = ws.Drawings.AddPicture("My Logo", image);

                    //add the image to row 20, column E
                    excelImage.SetPosition(0, 30, 0,20);

                    excelImage.SetSize(80, 100);
                }

                ws.Cells["A3:C3"].Merge = true;
                ws.Cells["A3"].Value = "Affiliation No.: "+Affiliation_no;
                ws.Cells["A3"].Style.Font.Name = "Calibri";
                ws.Cells["A3"].Style.Font.Size = 11;
                ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I3:J3"].Merge = true;
                ws.Cells["I3"].Value = "School Code: "+School_code;
                ws.Cells["I3"].Style.Font.Name = "Calibri";
                ws.Cells["I3"].Style.Font.Size = 11;
                ws.Cells["I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A5"].Value = "Book No. ";
                ws.Cells["A5"].Style.Font.Name = "Calibri";
                ws.Cells["A5"].Style.Font.Size = 11;
                ws.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B5"].Style.Font.Name = "Calibri";
                ws.Cells["B5"].Style.Font.Size = 11;
                ws.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["B5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["E5"].Value = "TC No. ";
                ws.Cells["E5"].Style.Font.Name = "Calibri";
                ws.Cells["E5"].Style.Font.Size = 11;
                ws.Cells["E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F5"].Value = sess + "/" + tc_no.ToString();
                ws.Cells["F5"].Style.Font.Name = "Calibri";
                ws.Cells["F5"].Style.Font.Size = 11;
                ws.Cells["F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["F5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I5"].Value = "Admission No ";
                ws.Cells["I5"].Style.Font.Name = "Calibri";
                ws.Cells["I5"].Style.Font.Size = 11;
                ws.Cells["I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["J5"].Value = result.sr_number;
                ws.Cells["J5"].Style.Font.Name = "Calibri";
                ws.Cells["J5"].Style.Font.Size = 11;
                ws.Cells["J5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["J5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["A7"].Value = "1";
                ws.Cells["A7"].Style.Font.Name = "Calibri";
                ws.Cells["A7"].Style.Font.Size = 11;
                ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B7"].Value = "Name of Pupil ";
                ws.Cells["B7"].Style.Font.Name = "Calibri";
                ws.Cells["B7"].Style.Font.Size = 11;
                ws.Cells["B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D7"].Value = result.std_name;
                ws.Cells["D7"].Style.Font.Name = "Calibri";
                ws.Cells["D7"].Style.Font.Size = 11;
                ws.Cells["D7"].Style.Font.Bold = true;
                ws.Cells["D7"].Style.Font.UnderLine = true;
                ws.Cells["D7"].Style.Font.Italic = true;
                ws.Cells["D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A8"].Value = "2";
                ws.Cells["A8"].Style.Font.Name = "Calibri";
                ws.Cells["A8"].Style.Font.Size = 11;
                ws.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B8"].Value = "Mother's Name ";
                ws.Cells["B8"].Style.Font.Name = "Calibri";
                ws.Cells["B8"].Style.Font.Size = 11;
                ws.Cells["B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D8"].Value = result.std_mother;
                ws.Cells["D8"].Style.Font.Name = "Calibri";
                ws.Cells["D8"].Style.Font.Size = 11;
                ws.Cells["D8"].Style.Font.Bold = true;
                ws.Cells["D8"].Style.Font.UnderLine = true;
                ws.Cells["D8"].Style.Font.Italic = true;
                ws.Cells["D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A9"].Value = "3";
                ws.Cells["A9"].Style.Font.Name = "Calibri";
                ws.Cells["A9"].Style.Font.Size = 11;
                ws.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B9"].Value = "Father's/Guardian's Name ";
                ws.Cells["B9"].Style.Font.Name = "Calibri";
                ws.Cells["B9"].Style.Font.Size = 11;
                ws.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["E9"].Value = result.std_father;
                ws.Cells["E9"].Style.Font.Name = "Calibri";
                ws.Cells["E9"].Style.Font.Size = 11;
                ws.Cells["E9"].Style.Font.Bold = true;
                ws.Cells["E9"].Style.Font.UnderLine = true;
                ws.Cells["E9"].Style.Font.Italic = true;
                ws.Cells["E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["E9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A10"].Value = "4";
                ws.Cells["A10"].Style.Font.Name = "Calibri";
                ws.Cells["A10"].Style.Font.Size = 11;
                ws.Cells["A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B10"].Value = "Nationality  ";
                ws.Cells["B10"].Style.Font.Name = "Calibri";
                ws.Cells["B10"].Style.Font.Size = 11;
                ws.Cells["B10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["C10"].Value = result.std_nationality;
                ws.Cells["C10"].Style.Font.Name = "Calibri";
                ws.Cells["C10"].Style.Font.Size = 11;
                ws.Cells["C10"].Style.Font.Bold = true;
                ws.Cells["C10"].Style.Font.UnderLine = true;
                ws.Cells["C10"].Style.Font.Italic = true;
                ws.Cells["C10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["C10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A11"].Value = "5";
                ws.Cells["A11"].Style.Font.Name = "Calibri";
                ws.Cells["A11"].Style.Font.Size = 11;
                ws.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B11"].Value = "Whether the candidate belongs to Schedule caste or schedule Tribe or OBC";
                ws.Cells["B11"].Style.Font.Name = "Calibri";
                ws.Cells["B11"].Style.Font.Size = 11;
                ws.Cells["B11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I11"].Value = result.std_category;
                ws.Cells["I11"].Style.Font.Name = "Calibri";
                ws.Cells["I11"].Style.Font.Size = 11;
                ws.Cells["I11"].Style.Font.Bold = true;
                ws.Cells["I11"].Style.Font.UnderLine = true;
                ws.Cells["I11"].Style.Font.Italic = true;
                ws.Cells["I11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A12"].Value = "6";
                ws.Cells["A12"].Style.Font.Name = "Calibri";
                ws.Cells["A12"].Style.Font.Size = 11;
                ws.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B12"].Value = "Date of First admission in the School with class";
                ws.Cells["B12"].Style.Font.Name = "Calibri";
                ws.Cells["B12"].Style.Font.Size = 11;
                ws.Cells["B12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G12"].Value = result.std_admission_date.ToString("dd/MM/yyyy");
                ws.Cells["G12"].Style.Font.Name = "Calibri";
                ws.Cells["G12"].Style.Font.Size = 11;
                ws.Cells["G12"].Style.Font.Bold = true;
                ws.Cells["G12"].Style.Font.UnderLine = true;
                ws.Cells["G12"].Style.Font.Italic = true;
                ws.Cells["G12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["H12"].Value = "Class";
                ws.Cells["H12"].Style.Font.Name = "Calibri";
                ws.Cells["H12"].Style.Font.Size = 11;
                ws.Cells["H12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["H12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I12"].Value = result.std_admission_class;
                ws.Cells["I12"].Style.Font.Name = "Calibri";
                ws.Cells["I12"].Style.Font.Size = 11;
                ws.Cells["I12"].Style.Font.Bold = true;
                ws.Cells["I12"].Style.Font.UnderLine = true;
                ws.Cells["I12"].Style.Font.Italic = true;
                ws.Cells["I12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A13"].Value = "7";
                ws.Cells["A13"].Style.Font.Name = "Calibri";
                ws.Cells["A13"].Style.Font.Size = 11;
                ws.Cells["A13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B13"].Value = "Date of Birth (in Christian Era) according to Admission & withdrawal Register (in figures)";
                ws.Cells["B13"].Style.Font.Name = "Calibri";
                ws.Cells["B13"].Style.Font.Size = 11;
                ws.Cells["B13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["J13"].Value = result.std_dob.ToString("dd/MM/yyyy");
                ws.Cells["J13"].Style.Font.Name = "Calibri";
                ws.Cells["J13"].Style.Font.Size = 11;
                ws.Cells["J13"].Style.Font.Bold = true;
                ws.Cells["J13"].Style.Font.UnderLine = true;
                ws.Cells["J13"].Style.Font.Italic = true;
                ws.Cells["J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B14"].Value = "(In Words)";
                ws.Cells["B14"].Style.Font.Name = "Calibri";
                ws.Cells["B14"].Style.Font.Size = 11;
                ws.Cells["B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["C14"].Value = birth_certificateMain.DateToWritten(result.std_dob).ToString();
                ws.Cells["C14"].Style.Font.Name = "Calibri";
                ws.Cells["C14"].Style.Font.Size = 11;
                ws.Cells["C14"].Style.Font.Bold = true;
                ws.Cells["C14"].Style.Font.UnderLine = true;
                ws.Cells["C14"].Style.Font.Italic = true;
                ws.Cells["C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A15"].Value = "8";
                ws.Cells["A15"].Style.Font.Name = "Calibri";
                ws.Cells["A15"].Style.Font.Size = 11;
                ws.Cells["A15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B15"].Value = "Class in which the pupil last studied (in figures)";
                ws.Cells["B15"].Style.Font.Name = "Calibri";
                ws.Cells["B15"].Style.Font.Size = 11;
                ws.Cells["B15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G15"].Value = result.std_pass_class;
                ws.Cells["G15"].Style.Font.Name = "Calibri";
                ws.Cells["G15"].Style.Font.Size = 11;
                ws.Cells["G15"].Style.Font.Bold = true;
                ws.Cells["G15"].Style.Font.UnderLine = true;
                ws.Cells["G15"].Style.Font.Italic = true;
                ws.Cells["G15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //ws.Cells["H15"].Value = "(in words)";
                //ws.Cells["H15"].Style.Font.Name = "Calibri";
                //ws.Cells["H15"].Style.Font.Size = 11;
                //ws.Cells["H15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                //ws.Cells["H15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I15"].Value = "";
                ws.Cells["I15"].Style.Font.Name = "Calibri";
                ws.Cells["I15"].Style.Font.Size = 11;
                ws.Cells["I15"].Style.Font.Bold = true;
                ws.Cells["I15"].Style.Font.UnderLine = true;
                ws.Cells["I15"].Style.Font.Italic = true;
                ws.Cells["I15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["I15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A16"].Value = "9";
                ws.Cells["A16"].Style.Font.Name = "Calibri";
                ws.Cells["A16"].Style.Font.Size = 11;
                ws.Cells["A16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B16"].Value = "School/Board Annual Examination last taken with result";
                ws.Cells["B16"].Style.Font.Name = "Calibri";
                ws.Cells["B16"].Style.Font.Size = 11;
                ws.Cells["B16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A17"].Value = "10";
                ws.Cells["A17"].Style.Font.Name = "Calibri";
                ws.Cells["A17"].Style.Font.Size = 11;
                ws.Cells["A17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B17"].Value = "Whether failed, if so once/twice in the same class";
                ws.Cells["B17"].Style.Font.Name = "Calibri";
                ws.Cells["B17"].Style.Font.Size = 11;
                ws.Cells["B17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A18"].Value = "11";
                ws.Cells["A18"].Style.Font.Name = "Calibri";
                ws.Cells["A18"].Style.Font.Size = 11;
                ws.Cells["A18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B18"].Value = "Subjects Studied:";
                ws.Cells["B18"].Style.Font.Name = "Calibri";
                ws.Cells["B18"].Style.Font.Size = 11;
                ws.Cells["B18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                query = @"SELECT 
                                subject_name
                            FROM
                                mst_class_subject a,
                                mst_subject b
                            WHERE
                                a.subject_id = b.subject_id
                                    AND a.session = b.session
                                    AND a.class_id = (SELECT 
                                        b.class_id
                                    FROM
                                        mst_std_Class a,
                                        mst_class b
                                    WHERE
                                        sr_num = @sr_number AND a.session = b.session
                                            AND a.class_id = b.class_id
                                            AND a.session != (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')
                                    ORDER BY order_by
                                    LIMIT 1)
                                    AND a.session != (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                    ORDER BY session_start_date DESC
                                    LIMIT 1)";

                IEnumerable<string> subjects = con.Query<string>(query,new {sr_number = sr_number });

                ws.Cells["D18"].Value = String.Join(",",subjects);
                ws.Cells["D18"].Style.Font.Name = "Calibri";
                ws.Cells["D18"].Style.Font.Size = 11;
                ws.Cells["D18"].Style.Font.Bold = true;
                ws.Cells["D18"].Style.Font.UnderLine = true;
                ws.Cells["D18"].Style.Font.Italic = true;
                ws.Cells["D18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A19"].Value = "12";
                ws.Cells["A19"].Style.Font.Name = "Calibri";
                ws.Cells["A19"].Style.Font.Size = 11;
                ws.Cells["A19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B19"].Value = "Whether qualified for promotion to the higher class";
                ws.Cells["B19"].Style.Font.Name = "Calibri";
                ws.Cells["B19"].Style.Font.Size = 12;
                ws.Cells["B19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B20"].Value = "If so, to which class (in figures)";
                ws.Cells["B20"].Style.Font.Name = "Calibri";
                ws.Cells["B20"].Style.Font.Size = 11;
                ws.Cells["B20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G20"].Value = "(in words)";
                ws.Cells["G20"].Style.Font.Name = "Calibri";
                ws.Cells["G20"].Style.Font.Size = 11;
                ws.Cells["G20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A21"].Value = "13";
                ws.Cells["A21"].Style.Font.Name = "Calibri";
                ws.Cells["A21"].Style.Font.Size = 11;
                ws.Cells["A21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B21"].Value = "Month upto which the pupil has paid school dues";
                ws.Cells["B21"].Style.Font.Name = "Calibri";
                ws.Cells["B21"].Style.Font.Size = 11;
                ws.Cells["B21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                query = @"SELECT 
                                month_name
                            FROM
                                out_standing
                            WHERE
                                sr_number = @sr_number
                                    AND IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) = 0
                            GROUP BY month_name
                            ORDER BY dt_date DESC , serial DESC , receipt_no DESC
                            LIMIT 1";

                string month = con.Query<string>(query, new { sr_number = sr_number }).SingleOrDefault();

                ws.Cells["G21"].Value = month;
                ws.Cells["G21"].Style.Font.Name = "Calibri";
                ws.Cells["G21"].Style.Font.Size = 11;
                ws.Cells["G21"].Style.Font.Bold = true;
                ws.Cells["G21"].Style.Font.UnderLine = true;
                ws.Cells["G21"].Style.Font.Italic = true;
                ws.Cells["G21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A22"].Value = "14";
                ws.Cells["A22"].Style.Font.Name = "Calibri";
                ws.Cells["A22"].Style.Font.Size = 11;
                ws.Cells["A22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B22"].Value = "Any fee concession availed of: If so, the nature of such concession ";
                ws.Cells["B22"].Style.Font.Name = "Calibri";
                ws.Cells["B22"].Style.Font.Size = 11;
                ws.Cells["B22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A23"].Value = "15";
                ws.Cells["A23"].Style.Font.Name = "Calibri";
                ws.Cells["A23"].Style.Font.Size = 11;
                ws.Cells["A23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A23"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B23"].Value = "Total No. of Working days in the academic session";
                ws.Cells["B23"].Style.Font.Name = "Calibri";
                ws.Cells["B23"].Style.Font.Size = 11;
                ws.Cells["B23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B23"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G23"].Value = result.working_days;
                ws.Cells["G23"].Style.Font.Name = "Calibri";
                ws.Cells["G23"].Style.Font.Size = 11;
                ws.Cells["G23"].Style.Font.Bold = true;
                ws.Cells["G23"].Style.Font.UnderLine = true;
                ws.Cells["G23"].Style.Font.Italic = true;
                ws.Cells["G23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G23"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A24"].Value = "16";
                ws.Cells["A24"].Style.Font.Name = "Calibri";
                ws.Cells["A24"].Style.Font.Size = 11;
                ws.Cells["A24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A24"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B24"].Value = "Total No. of Working days pupil present in school";
                ws.Cells["B24"].Style.Font.Name = "Calibri";
                ws.Cells["B24"].Style.Font.Size = 11;
                ws.Cells["B24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B24"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G24"].Value = result.present_days;
                ws.Cells["G24"].Style.Font.Name = "Calibri";
                ws.Cells["G24"].Style.Font.Size = 11;
                ws.Cells["G24"].Style.Font.Bold = true;
                ws.Cells["G24"].Style.Font.UnderLine = true;
                ws.Cells["G24"].Style.Font.Italic = true;
                ws.Cells["G24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G24"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A25"].Value = "17";
                ws.Cells["A25"].Style.Font.Name = "Calibri";
                ws.Cells["A25"].Style.Font.Size = 11;
                ws.Cells["A25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A25"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B25"].Value = "Whether NCC Cadet/Boy scout/Girl Guide (details may be given)";
                ws.Cells["B25"].Style.Font.Name = "Calibri";
                ws.Cells["B25"].Style.Font.Size = 11;
                ws.Cells["B25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B25"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A26"].Value = "18";
                ws.Cells["A26"].Style.Font.Name = "Calibri";
                ws.Cells["A26"].Style.Font.Size = 11;
                ws.Cells["A26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A26"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B26"].Value = "Games played or extra curricular activities in which the pupil usually took part";
                ws.Cells["B26"].Style.Font.Name = "Calibri";
                ws.Cells["B26"].Style.Font.Size = 11;
                ws.Cells["B26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B26"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A27"].Value = "19";
                ws.Cells["A27"].Style.Font.Name = "Calibri";
                ws.Cells["A27"].Style.Font.Size = 11;
                ws.Cells["A27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A27"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B27"].Value = "General Conduct";
                ws.Cells["B27"].Style.Font.Name = "Calibri";
                ws.Cells["B27"].Style.Font.Size = 11;
                ws.Cells["B27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B27"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D27"].Value = "Good";
                ws.Cells["D27"].Style.Font.Name = "Calibri";
                ws.Cells["D27"].Style.Font.Size = 11;
                ws.Cells["D27"].Style.Font.Bold = true;
                ws.Cells["D27"].Style.Font.UnderLine = true;
                ws.Cells["D27"].Style.Font.Italic = true;
                ws.Cells["D27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D27"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A28"].Value = "20";
                ws.Cells["A28"].Style.Font.Name = "Calibri";
                ws.Cells["A28"].Style.Font.Size = 11;
                ws.Cells["A28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A28"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B28"].Value = "Date of Application for Certificate";
                ws.Cells["B28"].Style.Font.Name = "Calibri";
                ws.Cells["B28"].Style.Font.Size = 11;
                ws.Cells["B28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B28"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F28"].Value = result.nso_date.ToString("dd/MM/yyyy");
                ws.Cells["F28"].Style.Font.Name = "Calibri";
                ws.Cells["F28"].Style.Font.Size = 11;
                ws.Cells["F28"].Style.Font.Bold = true;
                ws.Cells["F28"].Style.Font.UnderLine = true;
                ws.Cells["F28"].Style.Font.Italic = true;
                ws.Cells["F28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["F28"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A29"].Value = "21";
                ws.Cells["A29"].Style.Font.Name = "Calibri";
                ws.Cells["A29"].Style.Font.Size = 11;
                ws.Cells["A29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A29"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B29"].Value = "Date of Issue of Certificate ";
                ws.Cells["B29"].Style.Font.Name = "Calibri";
                ws.Cells["B29"].Style.Font.Size = 11;
                ws.Cells["B29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B29"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F29"].Value = System.DateTime.Now.ToString("dd/MM/yyyy");
                ws.Cells["F29"].Style.Font.Name = "Calibri";
                ws.Cells["F29"].Style.Font.Size = 11;
                ws.Cells["F29"].Style.Font.Bold = true;
                ws.Cells["F29"].Style.Font.UnderLine = true;
                ws.Cells["F29"].Style.Font.Italic = true;
                ws.Cells["F29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["F29"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A30"].Value = "22";
                ws.Cells["A30"].Style.Font.Name = "Calibri";
                ws.Cells["A30"].Style.Font.Size = 11;
                ws.Cells["A30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B30"].Value = "Reasons for leaving the School ";
                ws.Cells["B30"].Style.Font.Name = "Calibri";
                ws.Cells["B30"].Style.Font.Size = 11;
                ws.Cells["B30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A30"].Value = "23";
                ws.Cells["A30"].Style.Font.Name = "Calibri";
                ws.Cells["A30"].Style.Font.Size = 11;
                ws.Cells["A30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B30"].Value = "Any other remarks ";
                ws.Cells["B30"].Style.Font.Name = "Calibri";
                ws.Cells["B30"].Style.Font.Size = 11;
                ws.Cells["B30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                ws.Cells["B36"].Value = "Signature of Class Teacher";
                ws.Cells["B36"].Style.Font.Name = "Calibri";
                ws.Cells["B36"].Style.Font.Size = 11;
                ws.Cells["B36"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B36"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F36"].Value = "Checked by";
                ws.Cells["F36"].Style.Font.Name = "Calibri";
                ws.Cells["F36"].Style.Font.Size = 11;
                ws.Cells["F36"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["F36"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I36"].Value = "Sign of Principal";
                ws.Cells["I36"].Style.Font.Name = "Calibri";
                ws.Cells["I36"].Style.Font.Size = 11;
                ws.Cells["I36"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["I36"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                query = @"INSERT INTO `tc_register`
                        (`session`,
                        `tc_no`,
                        `tc_date`,
                        `sr_num`,
                        `user_id`)
                        VALUES
                        (@session,
                        @tc_no,
                        @tc_date,
                        @sr_num,
                        @user_id)";



                con.Execute(query, new { session = sess, tc_no = tc_no, tc_date = System.DateTime.Now, sr_num = sr_number, user_id = user_id });

                query = @"UPDATE `sr_register`
                        SET
                        `tc_generated` = 1
                        WHERE `sr_number` = @sr_number";

                con.Execute(query, new { sr_number = sr_number });

                return pck.GetAsByteArray();
            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public byte[] Download_TC(int sr_number, string session, int tc_number, DateTime tc_date)
        {
            try
            {
                string query = @"SELECT 
                                        sr_number,
                                        CONCAT(IFNULL(std_first_name, ''),
                                                ' ',
                                                IFNULL(std_last_name, '')) std_name,
                                        std_mother_name std_mother,
                                        std_father_name std_father,
                                        std_nationality,
                                        std_category,
                                        std_admission_date,
                                        std_admission_class,
                                        std_dob,
                                        (SELECT 
                                                class_name
                                            FROM
                                                mst_std_Class a,
                                                mst_class b
                                            WHERE
                                                sr_num = a.sr_number
                                                    AND a.session = b.session
                                                    AND a.class_id = b.class_id
                                                    AND a.session != (SELECT 
                                                        session
                                                    FROM
                                                        mst_session
                                                    WHERE
                                                        session_finalize = 'Y')
                                            ORDER BY order_by
                                            LIMIT 1) std_pass_class,
                                        (SELECT 
                                                COUNT(*)
                                            FROM
                                                attendance_register
                                            WHERE
                                                sr_num = a.sr_number
                                                    AND session = (SELECT 
                                                        session
                                                    FROM
                                                        mst_session
                                                    WHERE
                                                        session_finalize != 'Y'
                                                    ORDER BY session_start_date DESC
                                                    LIMIT 1)) working_days,
                                        (SELECT 
                                                COUNT(*)
                                            FROM
                                                attendance_register
                                            WHERE
                                                sr_num = a.sr_number
                                                    AND session = (SELECT 
                                                        session
                                                    FROM
                                                        mst_session
                                                    WHERE
                                                        session_finalize != 'Y'
                                                    ORDER BY session_start_date DESC
                                                    LIMIT 1)
                                                    AND attendance = 1) present_days,
                                        nso_date
                                    FROM
                                        sr_register a
                                    WHERE
                                        std_active = 'N' AND sr_number = @sr_number";

                tc_details result = con.Query<tc_details>(query, new { sr_number = sr_number }).SingleOrDefault();



                string sess = session;



                string tc_no = tc_number.ToString().PadLeft(tc_number.ToString().Length + 3, '0');

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TC");

                ws.PrinterSettings.TopMargin = 0.25m;
                ws.PrinterSettings.BottomMargin = 0.25m;
                ws.PrinterSettings.LeftMargin = 0.25m;
                ws.PrinterSettings.RightMargin = 0.25m;
                ws.PrinterSettings.HeaderMargin = 0.3m;
                ws.PrinterSettings.FooterMargin = 0.3m;
                ws.PrinterSettings.HorizontalCentered = true;

                ws.Column(1).Width = ExcelTc_form.GetTrueColumnWidth(8.14);
                ws.Column(2).Width = ExcelTc_form.GetTrueColumnWidth(9.86);
                ws.Column(3).Width = ExcelTc_form.GetTrueColumnWidth(8.43);
                ws.Column(4).Width = ExcelTc_form.GetTrueColumnWidth(8.43);
                ws.Column(5).Width = ExcelTc_form.GetTrueColumnWidth(6.71);
                ws.Column(6).Width = ExcelTc_form.GetTrueColumnWidth(10.86);
                ws.Column(7).Width = ExcelTc_form.GetTrueColumnWidth(10.29);
                ws.Column(8).Width = ExcelTc_form.GetTrueColumnWidth(8.43);
                ws.Column(9).Width = ExcelTc_form.GetTrueColumnWidth(11.71);
                ws.Column(10).Width = ExcelTc_form.GetTrueColumnWidth(10.29);

                for (int i = 7; i <= 31; i++)
                {
                    ws.Row(i).Height = 19.5;
                }


                ws.Row(1).Height = 108.75;

                using (ExcelRange Rng = ws.Cells["A1:J1"])
                {
                    Rng.Merge = true;
                    Rng.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    Rng.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    Rng.Style.WrapText = true;

                    ExcelRichTextCollection RichTxtCollection = Rng.RichText;
                    ExcelRichText RichText = RichTxtCollection.Add("TRANSFER CERTIFICATE\n");
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


                using (System.Drawing.Image image = System.Drawing.Image.FromFile(HttpContext.Current.Server.MapPath("/images/logo.jpg")))
                {
                    var excelImage = ws.Drawings.AddPicture("My Logo", image);

                    //add the image to row 20, column E
                    excelImage.SetPosition(0, 30, 0, 20);

                    excelImage.SetSize(80, 100);
                }

                ws.Cells["A3:C3"].Merge = true;
                ws.Cells["A3"].Value = "Affiliation No.: " + Affiliation_no;
                ws.Cells["A3"].Style.Font.Name = "Calibri";
                ws.Cells["A3"].Style.Font.Size = 11;
                ws.Cells["A3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I3:J3"].Merge = true;
                ws.Cells["I3"].Value = "School Code: " + School_code;
                ws.Cells["I3"].Style.Font.Name = "Calibri";
                ws.Cells["I3"].Style.Font.Size = 11;
                ws.Cells["I3"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["I3"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A5"].Value = "Book No. ";
                ws.Cells["A5"].Style.Font.Name = "Calibri";
                ws.Cells["A5"].Style.Font.Size = 11;
                ws.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B5"].Style.Font.Name = "Calibri";
                ws.Cells["B5"].Style.Font.Size = 11;
                ws.Cells["B5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["B5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["B5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["E5"].Value = "TC No. ";
                ws.Cells["E5"].Style.Font.Name = "Calibri";
                ws.Cells["E5"].Style.Font.Size = 11;
                ws.Cells["E5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F5"].Value = sess + "/" + tc_no.ToString();
                ws.Cells["F5"].Style.Font.Name = "Calibri";
                ws.Cells["F5"].Style.Font.Size = 11;
                ws.Cells["F5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["F5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["F5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I5"].Value = "Admission No ";
                ws.Cells["I5"].Style.Font.Name = "Calibri";
                ws.Cells["I5"].Style.Font.Size = 11;
                ws.Cells["I5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["J5"].Value = result.sr_number;
                ws.Cells["J5"].Style.Font.Name = "Calibri";
                ws.Cells["J5"].Style.Font.Size = 11;
                ws.Cells["J5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["J5"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["J5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["A7"].Value = "1";
                ws.Cells["A7"].Style.Font.Name = "Calibri";
                ws.Cells["A7"].Style.Font.Size = 11;
                ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B7"].Value = "Name of Pupil ";
                ws.Cells["B7"].Style.Font.Name = "Calibri";
                ws.Cells["B7"].Style.Font.Size = 11;
                ws.Cells["B7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D7"].Value = result.std_name;
                ws.Cells["D7"].Style.Font.Name = "Calibri";
                ws.Cells["D7"].Style.Font.Size = 11;
                ws.Cells["D7"].Style.Font.Bold = true;
                ws.Cells["D7"].Style.Font.UnderLine = true;
                ws.Cells["D7"].Style.Font.Italic = true;
                ws.Cells["D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A8"].Value = "2";
                ws.Cells["A8"].Style.Font.Name = "Calibri";
                ws.Cells["A8"].Style.Font.Size = 11;
                ws.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B8"].Value = "Mother's Name ";
                ws.Cells["B8"].Style.Font.Name = "Calibri";
                ws.Cells["B8"].Style.Font.Size = 11;
                ws.Cells["B8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D8"].Value = result.std_mother;
                ws.Cells["D8"].Style.Font.Name = "Calibri";
                ws.Cells["D8"].Style.Font.Size = 11;
                ws.Cells["D8"].Style.Font.Bold = true;
                ws.Cells["D8"].Style.Font.UnderLine = true;
                ws.Cells["D8"].Style.Font.Italic = true;
                ws.Cells["D8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D8"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A9"].Value = "3";
                ws.Cells["A9"].Style.Font.Name = "Calibri";
                ws.Cells["A9"].Style.Font.Size = 11;
                ws.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B9"].Value = "Father's/Guardian's Name ";
                ws.Cells["B9"].Style.Font.Name = "Calibri";
                ws.Cells["B9"].Style.Font.Size = 11;
                ws.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["E9"].Value = result.std_father;
                ws.Cells["E9"].Style.Font.Name = "Calibri";
                ws.Cells["E9"].Style.Font.Size = 11;
                ws.Cells["E9"].Style.Font.Bold = true;
                ws.Cells["E9"].Style.Font.UnderLine = true;
                ws.Cells["E9"].Style.Font.Italic = true;
                ws.Cells["E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["E9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A10"].Value = "4";
                ws.Cells["A10"].Style.Font.Name = "Calibri";
                ws.Cells["A10"].Style.Font.Size = 11;
                ws.Cells["A10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B10"].Value = "Nationality  ";
                ws.Cells["B10"].Style.Font.Name = "Calibri";
                ws.Cells["B10"].Style.Font.Size = 11;
                ws.Cells["B10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["C10"].Value = result.std_nationality;
                ws.Cells["C10"].Style.Font.Name = "Calibri";
                ws.Cells["C10"].Style.Font.Size = 11;
                ws.Cells["C10"].Style.Font.Bold = true;
                ws.Cells["C10"].Style.Font.UnderLine = true;
                ws.Cells["C10"].Style.Font.Italic = true;
                ws.Cells["C10"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["C10"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A11"].Value = "5";
                ws.Cells["A11"].Style.Font.Name = "Calibri";
                ws.Cells["A11"].Style.Font.Size = 11;
                ws.Cells["A11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B11"].Value = "Whether the candidate belongs to Schedule caste or schedule Tribe or OBC";
                ws.Cells["B11"].Style.Font.Name = "Calibri";
                ws.Cells["B11"].Style.Font.Size = 11;
                ws.Cells["B11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I11"].Value = result.std_category;
                ws.Cells["I11"].Style.Font.Name = "Calibri";
                ws.Cells["I11"].Style.Font.Size = 11;
                ws.Cells["I11"].Style.Font.Bold = true;
                ws.Cells["I11"].Style.Font.UnderLine = true;
                ws.Cells["I11"].Style.Font.Italic = true;
                ws.Cells["I11"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I11"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A12"].Value = "6";
                ws.Cells["A12"].Style.Font.Name = "Calibri";
                ws.Cells["A12"].Style.Font.Size = 11;
                ws.Cells["A12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B12"].Value = "Date of First admission in the School with class";
                ws.Cells["B12"].Style.Font.Name = "Calibri";
                ws.Cells["B12"].Style.Font.Size = 11;
                ws.Cells["B12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G12"].Value = result.std_admission_date.ToString("dd/MM/yyyy");
                ws.Cells["G12"].Style.Font.Name = "Calibri";
                ws.Cells["G12"].Style.Font.Size = 11;
                ws.Cells["G12"].Style.Font.Bold = true;
                ws.Cells["G12"].Style.Font.UnderLine = true;
                ws.Cells["G12"].Style.Font.Italic = true;
                ws.Cells["G12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["H12"].Value = "Class";
                ws.Cells["H12"].Style.Font.Name = "Calibri";
                ws.Cells["H12"].Style.Font.Size = 11;
                ws.Cells["H12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["H12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I12"].Value = result.std_admission_class;
                ws.Cells["I12"].Style.Font.Name = "Calibri";
                ws.Cells["I12"].Style.Font.Size = 11;
                ws.Cells["I12"].Style.Font.Bold = true;
                ws.Cells["I12"].Style.Font.UnderLine = true;
                ws.Cells["I12"].Style.Font.Italic = true;
                ws.Cells["I12"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I12"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A13"].Value = "7";
                ws.Cells["A13"].Style.Font.Name = "Calibri";
                ws.Cells["A13"].Style.Font.Size = 11;
                ws.Cells["A13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B13"].Value = "Date of Birth (in Christian Era) according to Admission & withdrawal Register (in figures)";
                ws.Cells["B13"].Style.Font.Name = "Calibri";
                ws.Cells["B13"].Style.Font.Size = 11;
                ws.Cells["B13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["J13"].Value = result.std_dob.ToString("dd/MM/yyyy");
                ws.Cells["J13"].Style.Font.Name = "Calibri";
                ws.Cells["J13"].Style.Font.Size = 11;
                ws.Cells["J13"].Style.Font.Bold = true;
                ws.Cells["J13"].Style.Font.UnderLine = true;
                ws.Cells["J13"].Style.Font.Italic = true;
                ws.Cells["J13"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                ws.Cells["J13"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B14"].Value = "(In Words)";
                ws.Cells["B14"].Style.Font.Name = "Calibri";
                ws.Cells["B14"].Style.Font.Size = 11;
                ws.Cells["B14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["C14"].Value = birth_certificateMain.DateToWritten(result.std_dob).ToString();
                ws.Cells["C14"].Style.Font.Name = "Calibri";
                ws.Cells["C14"].Style.Font.Size = 11;
                ws.Cells["C14"].Style.Font.Bold = true;
                ws.Cells["C14"].Style.Font.UnderLine = true;
                ws.Cells["C14"].Style.Font.Italic = true;
                ws.Cells["C14"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["C14"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A15"].Value = "8";
                ws.Cells["A15"].Style.Font.Name = "Calibri";
                ws.Cells["A15"].Style.Font.Size = 11;
                ws.Cells["A15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B15"].Value = "Class in which the pupil last studied (in figures)";
                ws.Cells["B15"].Style.Font.Name = "Calibri";
                ws.Cells["B15"].Style.Font.Size = 11;
                ws.Cells["B15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G15"].Value = result.std_pass_class;
                ws.Cells["G15"].Style.Font.Name = "Calibri";
                ws.Cells["G15"].Style.Font.Size = 11;
                ws.Cells["G15"].Style.Font.Bold = true;
                ws.Cells["G15"].Style.Font.UnderLine = true;
                ws.Cells["G15"].Style.Font.Italic = true;
                ws.Cells["G15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                //ws.Cells["H15"].Value = "(in words)";
                //ws.Cells["H15"].Style.Font.Name = "Calibri";
                //ws.Cells["H15"].Style.Font.Size = 11;
                //ws.Cells["H15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                //ws.Cells["H15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I15"].Value = "";
                ws.Cells["I15"].Style.Font.Name = "Calibri";
                ws.Cells["I15"].Style.Font.Size = 11;
                ws.Cells["I15"].Style.Font.Bold = true;
                ws.Cells["I15"].Style.Font.UnderLine = true;
                ws.Cells["I15"].Style.Font.Italic = true;
                ws.Cells["I15"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["I15"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A16"].Value = "9";
                ws.Cells["A16"].Style.Font.Name = "Calibri";
                ws.Cells["A16"].Style.Font.Size = 11;
                ws.Cells["A16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B16"].Value = "School/Board Annual Examination last taken with result";
                ws.Cells["B16"].Style.Font.Name = "Calibri";
                ws.Cells["B16"].Style.Font.Size = 11;
                ws.Cells["B16"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B16"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A17"].Value = "10";
                ws.Cells["A17"].Style.Font.Name = "Calibri";
                ws.Cells["A17"].Style.Font.Size = 11;
                ws.Cells["A17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B17"].Value = "Whether failed, if so once/twice in the same class";
                ws.Cells["B17"].Style.Font.Name = "Calibri";
                ws.Cells["B17"].Style.Font.Size = 11;
                ws.Cells["B17"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B17"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A18"].Value = "11";
                ws.Cells["A18"].Style.Font.Name = "Calibri";
                ws.Cells["A18"].Style.Font.Size = 11;
                ws.Cells["A18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B18"].Value = "Subjects Studied:";
                ws.Cells["B18"].Style.Font.Name = "Calibri";
                ws.Cells["B18"].Style.Font.Size = 11;
                ws.Cells["B18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                query = @"SELECT 
                                subject_name
                            FROM
                                mst_class_subject a,
                                mst_subject b
                            WHERE
                                a.subject_id = b.subject_id
                                    AND a.session = b.session
                                    AND a.class_id = (SELECT 
                                        b.class_id
                                    FROM
                                        mst_std_Class a,
                                        mst_class b
                                    WHERE
                                        sr_num = @sr_number AND a.session = b.session
                                            AND a.class_id = b.class_id
                                            AND a.session != (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')
                                    ORDER BY order_by
                                    LIMIT 1)
                                    AND a.session != (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                    ORDER BY session_start_date DESC
                                    LIMIT 1)";

                IEnumerable<string> subjects = con.Query<string>(query, new { sr_number = sr_number });

                ws.Cells["D18"].Value = String.Join(",", subjects);
                ws.Cells["D18"].Style.Font.Name = "Calibri";
                ws.Cells["D18"].Style.Font.Size = 11;
                ws.Cells["D18"].Style.Font.Bold = true;
                ws.Cells["D18"].Style.Font.UnderLine = true;
                ws.Cells["D18"].Style.Font.Italic = true;
                ws.Cells["D18"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D18"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A19"].Value = "12";
                ws.Cells["A19"].Style.Font.Name = "Calibri";
                ws.Cells["A19"].Style.Font.Size = 11;
                ws.Cells["A19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B19"].Value = "Whether qualified for promotion to the higher class";
                ws.Cells["B19"].Style.Font.Name = "Calibri";
                ws.Cells["B19"].Style.Font.Size = 12;
                ws.Cells["B19"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B19"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B20"].Value = "If so, to which class (in figures)";
                ws.Cells["B20"].Style.Font.Name = "Calibri";
                ws.Cells["B20"].Style.Font.Size = 11;
                ws.Cells["B20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G20"].Value = "(in words)";
                ws.Cells["G20"].Style.Font.Name = "Calibri";
                ws.Cells["G20"].Style.Font.Size = 11;
                ws.Cells["G20"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G20"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A21"].Value = "13";
                ws.Cells["A21"].Style.Font.Name = "Calibri";
                ws.Cells["A21"].Style.Font.Size = 11;
                ws.Cells["A21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B21"].Value = "Month upto which the pupil has paid school dues";
                ws.Cells["B21"].Style.Font.Name = "Calibri";
                ws.Cells["B21"].Style.Font.Size = 11;
                ws.Cells["B21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                query = @"SELECT 
                                month_name
                            FROM
                                out_standing
                            WHERE
                                sr_number = @sr_number
                                    AND IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) = 0
                            GROUP BY month_name
                            ORDER BY dt_date DESC , serial DESC , receipt_no DESC
                            LIMIT 1";

                string month = con.Query<string>(query, new { sr_number = sr_number }).SingleOrDefault();

                ws.Cells["G21"].Value = month;
                ws.Cells["G21"].Style.Font.Name = "Calibri";
                ws.Cells["G21"].Style.Font.Size = 11;
                ws.Cells["G21"].Style.Font.Bold = true;
                ws.Cells["G21"].Style.Font.UnderLine = true;
                ws.Cells["G21"].Style.Font.Italic = true;
                ws.Cells["G21"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G21"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A22"].Value = "14";
                ws.Cells["A22"].Style.Font.Name = "Calibri";
                ws.Cells["A22"].Style.Font.Size = 11;
                ws.Cells["A22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B22"].Value = "Any fee concession availed of: If so, the nature of such concession ";
                ws.Cells["B22"].Style.Font.Name = "Calibri";
                ws.Cells["B22"].Style.Font.Size = 11;
                ws.Cells["B22"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B22"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A23"].Value = "15";
                ws.Cells["A23"].Style.Font.Name = "Calibri";
                ws.Cells["A23"].Style.Font.Size = 11;
                ws.Cells["A23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A23"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B23"].Value = "Total No. of Working days in the academic session";
                ws.Cells["B23"].Style.Font.Name = "Calibri";
                ws.Cells["B23"].Style.Font.Size = 11;
                ws.Cells["B23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B23"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G23"].Value = result.working_days;
                ws.Cells["G23"].Style.Font.Name = "Calibri";
                ws.Cells["G23"].Style.Font.Size = 11;
                ws.Cells["G23"].Style.Font.Bold = true;
                ws.Cells["G23"].Style.Font.UnderLine = true;
                ws.Cells["G23"].Style.Font.Italic = true;
                ws.Cells["G23"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G23"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A24"].Value = "16";
                ws.Cells["A24"].Style.Font.Name = "Calibri";
                ws.Cells["A24"].Style.Font.Size = 11;
                ws.Cells["A24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A24"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B24"].Value = "Total No. of Working days pupil present in school";
                ws.Cells["B24"].Style.Font.Name = "Calibri";
                ws.Cells["B24"].Style.Font.Size = 11;
                ws.Cells["B24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B24"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["G24"].Value = result.present_days;
                ws.Cells["G24"].Style.Font.Name = "Calibri";
                ws.Cells["G24"].Style.Font.Size = 11;
                ws.Cells["G24"].Style.Font.Bold = true;
                ws.Cells["G24"].Style.Font.UnderLine = true;
                ws.Cells["G24"].Style.Font.Italic = true;
                ws.Cells["G24"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G24"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A25"].Value = "17";
                ws.Cells["A25"].Style.Font.Name = "Calibri";
                ws.Cells["A25"].Style.Font.Size = 11;
                ws.Cells["A25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A25"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B25"].Value = "Whether NCC Cadet/Boy scout/Girl Guide (details may be given)";
                ws.Cells["B25"].Style.Font.Name = "Calibri";
                ws.Cells["B25"].Style.Font.Size = 11;
                ws.Cells["B25"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B25"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A26"].Value = "18";
                ws.Cells["A26"].Style.Font.Name = "Calibri";
                ws.Cells["A26"].Style.Font.Size = 11;
                ws.Cells["A26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A26"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B26"].Value = "Games played or extra curricular activities in which the pupil usually took part";
                ws.Cells["B26"].Style.Font.Name = "Calibri";
                ws.Cells["B26"].Style.Font.Size = 11;
                ws.Cells["B26"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B26"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A27"].Value = "19";
                ws.Cells["A27"].Style.Font.Name = "Calibri";
                ws.Cells["A27"].Style.Font.Size = 11;
                ws.Cells["A27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A27"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B27"].Value = "General Conduct";
                ws.Cells["B27"].Style.Font.Name = "Calibri";
                ws.Cells["B27"].Style.Font.Size = 11;
                ws.Cells["B27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B27"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["D27"].Value = "Good";
                ws.Cells["D27"].Style.Font.Name = "Calibri";
                ws.Cells["D27"].Style.Font.Size = 11;
                ws.Cells["D27"].Style.Font.Bold = true;
                ws.Cells["D27"].Style.Font.UnderLine = true;
                ws.Cells["D27"].Style.Font.Italic = true;
                ws.Cells["D27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D27"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A28"].Value = "20";
                ws.Cells["A28"].Style.Font.Name = "Calibri";
                ws.Cells["A28"].Style.Font.Size = 11;
                ws.Cells["A28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A28"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B28"].Value = "Date of Application for Certificate";
                ws.Cells["B28"].Style.Font.Name = "Calibri";
                ws.Cells["B28"].Style.Font.Size = 11;
                ws.Cells["B28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B28"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F28"].Value = result.nso_date.ToString("dd/MM/yyyy");
                ws.Cells["F28"].Style.Font.Name = "Calibri";
                ws.Cells["F28"].Style.Font.Size = 11;
                ws.Cells["F28"].Style.Font.Bold = true;
                ws.Cells["F28"].Style.Font.UnderLine = true;
                ws.Cells["F28"].Style.Font.Italic = true;
                ws.Cells["F28"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["F28"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A29"].Value = "21";
                ws.Cells["A29"].Style.Font.Name = "Calibri";
                ws.Cells["A29"].Style.Font.Size = 11;
                ws.Cells["A29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A29"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B29"].Value = "Date of Issue of Certificate ";
                ws.Cells["B29"].Style.Font.Name = "Calibri";
                ws.Cells["B29"].Style.Font.Size = 11;
                ws.Cells["B29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B29"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F29"].Value = tc_date.ToString("dd/MM/yyyy");
                ws.Cells["F29"].Style.Font.Name = "Calibri";
                ws.Cells["F29"].Style.Font.Size = 11;
                ws.Cells["F29"].Style.Font.Bold = true;
                ws.Cells["F29"].Style.Font.UnderLine = true;
                ws.Cells["F29"].Style.Font.Italic = true;
                ws.Cells["F29"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["F29"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A30"].Value = "22";
                ws.Cells["A30"].Style.Font.Name = "Calibri";
                ws.Cells["A30"].Style.Font.Size = 11;
                ws.Cells["A30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B30"].Value = "Reasons for leaving the School ";
                ws.Cells["B30"].Style.Font.Name = "Calibri";
                ws.Cells["B30"].Style.Font.Size = 11;
                ws.Cells["B30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A30"].Value = "23";
                ws.Cells["A30"].Style.Font.Name = "Calibri";
                ws.Cells["A30"].Style.Font.Size = 11;
                ws.Cells["A30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["B30"].Value = "Any other remarks ";
                ws.Cells["B30"].Style.Font.Name = "Calibri";
                ws.Cells["B30"].Style.Font.Size = 11;
                ws.Cells["B30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B30"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;


                ws.Cells["B36"].Value = "Signature of Class Teacher";
                ws.Cells["B36"].Style.Font.Name = "Calibri";
                ws.Cells["B36"].Style.Font.Size = 11;
                ws.Cells["B36"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["B36"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["F36"].Value = "Checked by";
                ws.Cells["F36"].Style.Font.Name = "Calibri";
                ws.Cells["F36"].Style.Font.Size = 11;
                ws.Cells["F36"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["F36"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["I36"].Value = "Sign of Principal";
                ws.Cells["I36"].Style.Font.Name = "Calibri";
                ws.Cells["I36"].Style.Font.Size = 11;
                ws.Cells["I36"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["I36"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                
               
                return pck.GetAsByteArray();
            }
            catch (Exception ex)
            {

            }

            return null;
        }
    }

    public class ExcelTc_form
    {

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public byte[] Download_TC(int sr_number, string username, string session, int tc_number, DateTime tc_date)
        {
            try
            {


                string query = @"SELECT 
                                    sr_number,
                                    CONCAT(IFNULL(std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, ''),
                                            ', ',
                                            IFNULL(std_nationality, ''),
                                            ', ',
                                            IFNULL(std_cast, '')) std_name,
                                    CONCAT(IFNULL(std_father_name, ''),
                                            ', ',
                                            IFNULL(std_father_occupation, ''),
                                            ', ',
                                            IFNULL(std_address, ''),
                                            ' ',
                                            IFNULL(std_address1, ''),
                                            ' ',
                                            IFNULL(std_address2, ''),
                                            ' ',
                                            IFNULL(std_district, ''),
                                            ' ',
                                            IFNULL(std_state, ''),
                                            ' ',
                                            IFNULL(std_pincode, '')) std_father,
                                    std_dob,
                                    ifnull(std_last_school,'') std_last_school
                                FROM
                                    sr_register
                                WHERE
                                    std_active = 'N' AND sr_number = @sr_number";

                var result = con.Query<tc_details>(query, new { sr_number = sr_number }).SingleOrDefault();

                query = @"SELECT 
                            session_start_date,
                            session_end_date,
                            (SELECT 
                                    class_name
                                FROM
                                    mst_class
                                WHERE
                                    class_id = a.class_id
                                        AND session = b.session) class_name,
                            c.std_admission_date,
                            nso_date,
                            b.session
                        FROM
                            mst_std_class a,
                            mst_session b,
                            sr_register c
                        WHERE
                            a.sr_num = @sr_number
                                AND a.sr_num = c.sr_number
                                AND a.session = b.session
                                AND c.std_active = 'N'";


            

                string tc_no = tc_number.ToString().PadLeft(tc_number.ToString().Length + 3, '0');

                IEnumerable<tc_class_details> class_details = con.Query<tc_class_details>(query, new { sr_number = sr_number });

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TC");

                ws.PrinterSettings.TopMargin = 0.5m;
                ws.PrinterSettings.BottomMargin = 0.5m;
                ws.PrinterSettings.LeftMargin = 0.19m;
                ws.PrinterSettings.RightMargin = 0.19m;
                ws.PrinterSettings.HeaderMargin = 0.05m;
                ws.PrinterSettings.FooterMargin = 0.05m;
                ws.PrinterSettings.HorizontalCentered = true;

                ws.Column(1).Width = GetTrueColumnWidth(4.71);
                ws.Column(2).Width = GetTrueColumnWidth(9.43);
                ws.Column(3).Width = GetTrueColumnWidth(9.57);
                ws.Column(4).Width = GetTrueColumnWidth(9.86);
                ws.Column(5).Width = GetTrueColumnWidth(7.71);
                ws.Column(6).Width = GetTrueColumnWidth(5.71);
                ws.Column(7).Width = GetTrueColumnWidth(8.43);
                ws.Column(8).Width = GetTrueColumnWidth(10);
                ws.Column(9).Width = GetTrueColumnWidth(12.86);
                ws.Column(10).Width = GetTrueColumnWidth(7.71);
                ws.Column(11).Width = GetTrueColumnWidth(8.43);


                ws.Cells["A1:K1"].Merge = true;
                ws.Row(1).Height = 36;
                ws.Cells["A1"].Value = SchoolName;
                ws.Cells["A1"].Style.Font.Name = "Calibri";
                ws.Cells["A1"].Style.Font.Size = 28;
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A2:K2"].Merge = true;
                ws.Cells["A2"].Value = Address;
                ws.Cells["A2"].Style.Font.Name = "Calibri";
                ws.Cells["A2"].Style.Font.Size = 11;
                ws.Cells["A2"].Style.Font.Bold = true;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A4:K4"].Merge = true;
                ws.Cells["A4"].Value = "Scholar's Register & Transfer Certificate Form";
                ws.Cells["A4"].Style.Font.Name = "Bookman Old Style";
                ws.Cells["A4"].Style.Font.Size = 18;
                ws.Cells["A4"].Style.Font.Bold = true;
                ws.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A5:C5"].Merge = true;
                ws.Cells["A5"].Value = "Admission File No. " + result.sr_number.ToString();
                ws.Cells["A5"].Style.Font.Name = "Calibri";
                ws.Cells["A5"].Style.Font.Size = 11;
                ws.Cells["A5"].Style.Font.Bold = true;
                ws.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A5:C5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:C5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:C5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:C5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D5:F5"].Merge = true;
                ws.Cells["D5"].Value = "Withdrawal File No.";
                ws.Cells["D5"].Style.Font.Name = "Calibri";
                ws.Cells["D5"].Style.Font.Size = 11;
                ws.Cells["D5"].Style.Font.Bold = true;
                ws.Cells["D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D5:F5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D5:F5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D5:F5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D5:F5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["G5:I5"].Merge = true;
                ws.Cells["G5"].Value = "Transfer Certificate No. " + session + "/" + tc_no.ToString();
                ws.Cells["G5"].Style.Font.Name = "Calibri";
                ws.Cells["G5"].Style.Font.Size = 11;
                ws.Cells["G5"].Style.Font.Bold = true;
                ws.Cells["G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G5:I5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["G5:I5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["G5:I5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["G5:I5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                ws.Cells["J5:K5"].Merge = true;
                ws.Cells["J5"].Value = "Register No.";
                ws.Cells["J5"].Style.Font.Name = "Calibri";
                ws.Cells["J5"].Style.Font.Size = 11;
                ws.Cells["J5"].Style.Font.Bold = true;
                ws.Cells["J5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["J5:K5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["J5:K5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["J5:K5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["J5:K5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Row(6).Height = 48;

                ws.Cells["A6:C6"].Merge = true;
                ws.Cells["A6"].Value = "Name of the Scholar with nationality and caste if Hindu, otherwise religion.";
                ws.Cells["A6"].Style.Font.Name = "Calibri";
                ws.Cells["A6"].Style.Font.Size = 11;
                ws.Cells["A6"].Style.Font.Bold = true;
                ws.Cells["A6"].Style.WrapText = true;
                ws.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A6:C6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:C6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:C6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:C6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D6:G6"].Merge = true;
                ws.Cells["D6"].Value = "Name, Occupation & Address of Parents, Guardian or Husband";
                ws.Cells["D6"].Style.Font.Name = "Calibri";
                ws.Cells["D6"].Style.Font.Size = 11;
                ws.Cells["D6"].Style.Font.Bold = true;
                ws.Cells["D6"].Style.WrapText = true;
                ws.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["D6:G6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D6:G6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D6:G6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D6:G6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H6"].Value = "Date of birth of the Scholar";
                ws.Cells["H6"].Style.Font.Name = "Calibri";
                ws.Cells["H6"].Style.Font.Size = 11;
                ws.Cells["H6"].Style.Font.Bold = true;
                ws.Cells["H6"].Style.WrapText = true;
                ws.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["H6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I6:K6"].Merge = true;
                ws.Cells["I6"].Value = "The last institution if any which the Scholar attended for joining this institution";
                ws.Cells["I6"].Style.Font.Name = "Calibri";
                ws.Cells["I6"].Style.Font.Size = 11;
                ws.Cells["I6"].Style.Font.Bold = true;
                ws.Cells["I6"].Style.WrapText = true;
                ws.Cells["I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["I6:K6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I6:K6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I6:K6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I6:K6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Row(7).Height = 48;

                ws.Cells["A7:C7"].Merge = true;
                ws.Cells["A7"].Value = result.std_name;
                ws.Cells["A7"].Style.Font.Name = "Calibri";
                ws.Cells["A7"].Style.Font.Size = 11;
                ws.Cells["A7"].Style.WrapText = true;
                ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A7:C7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:C7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:C7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:C7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D7:G7"].Merge = true;
                ws.Cells["D7"].Value = result.std_father;
                ws.Cells["D7"].Style.Font.Name = "Calibri";
                ws.Cells["D7"].Style.Font.Size = 11;
                ws.Cells["D7"].Style.WrapText = true;
                ws.Cells["D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["D7:G7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D7:G7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D7:G7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D7:G7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H7"].Value = result.std_dob.ToString("dd.MM.yyyy");
                ws.Cells["H7"].Style.Font.Name = "Calibri";
                ws.Cells["H7"].Style.Font.Size = 11;
                ws.Cells["H7"].Style.WrapText = true;
                ws.Cells["H7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["H7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I7:K7"].Merge = true;
                ws.Cells["I7"].Value = result.std_last_school;
                ws.Cells["I7"].Style.Font.Name = "Calibri";
                ws.Cells["I7"].Style.Font.Size = 11;
                ws.Cells["I7"].Style.WrapText = true;
                ws.Cells["I7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["I7:K7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I7:K7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I7:K7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I7:K7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                ws.Cells["A8:K8"].Merge = true;
                ws.Cells["A8"].Value = "Date of birth in words: " + birth_certificateMain.DateToWritten(result.std_dob).ToString();
                ws.Cells["A8"].Style.Font.Name = "Calibri";
                ws.Cells["A8"].Style.Font.Size = 11;
                ws.Cells["A8"].Style.WrapText = true;
                ws.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A8:K8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A8:K8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A8:K8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A8:K8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Row(9).Height = (double)48.75;

                ws.Cells["A9"].Value = "Class";
                ws.Cells["A9"].Style.Font.Name = "Calibri";
                ws.Cells["A9"].Style.Font.Size = 11;
                ws.Cells["A9"].Style.WrapText = true;
                ws.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                ws.Cells["B9"].Value = "Date Of Admission";
                ws.Cells["B9"].Style.Font.Name = "Calibri";
                ws.Cells["B9"].Style.Font.Size = 11;
                ws.Cells["B9"].Style.WrapText = true;
                ws.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["B9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["B9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["C9"].Value = "Date of Promotion";
                ws.Cells["C9"].Style.Font.Name = "Calibri";
                ws.Cells["C9"].Style.Font.Size = 11;
                ws.Cells["C9"].Style.WrapText = true;
                ws.Cells["C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["C9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["C9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["C9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["C9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D9"].Value = "Date of Removal";
                ws.Cells["D9"].Style.Font.Name = "Calibri";
                ws.Cells["D9"].Style.Font.Size = 11;
                ws.Cells["D9"].Style.WrapText = true;
                ws.Cells["D9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["D9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["E9:G9"].Merge = true;
                ws.Cells["E9"].Value = "Cause of removal e.g Non Payment of dues, transfer of Family, expulsion etc.";
                ws.Cells["E9"].Style.Font.Name = "Calibri";
                ws.Cells["E9"].Style.Font.Size = 11;
                ws.Cells["E9"].Style.WrapText = true;
                ws.Cells["E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["E9:G9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["E9:G9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["E9:G9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["E9:G9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H9"].Value = "Year";
                ws.Cells["H9"].Style.Font.Name = "Calibri";
                ws.Cells["H9"].Style.Font.Size = 11;
                ws.Cells["H9"].Style.WrapText = true;
                ws.Cells["H9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["H9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I9"].Value = "Conduct";
                ws.Cells["I9"].Style.Font.Name = "Calibri";
                ws.Cells["I9"].Style.Font.Size = 11;
                ws.Cells["I9"].Style.WrapText = true;
                ws.Cells["I9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["I9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["J9"].Value = "Work";
                ws.Cells["J9"].Style.Font.Name = "Calibri";
                ws.Cells["J9"].Style.Font.Size = 11;
                ws.Cells["J9"].Style.WrapText = true;
                ws.Cells["J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["J9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["J9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["J9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["J9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["K9"].Value = "Principal Signature";
                ws.Cells["K9"].Style.Font.Name = "Calibri";
                ws.Cells["K9"].Style.Font.Size = 11;
                ws.Cells["K9"].Style.WrapText = true;
                ws.Cells["K9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["K9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["K9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["K9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["K9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                int i = 10;
                int record = class_details.Count();
                foreach (var detail in class_details)
                {
                    ws.Row(i).Height = (double)19.05;

                    ws.Cells["A" + i].Value = detail.class_name;
                    ws.Cells["A" + i].Style.Font.Name = "Calibri";
                    ws.Cells["A" + i].Style.Font.Size = 11;
                    ws.Cells["A" + i].Style.WrapText = true;
                    ws.Cells["A" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if (10 == i)
                    {
                        ws.Cells["B" + i].Value = detail.std_admission_date.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        ws.Cells["B" + i].Value = detail.session_start_date.ToString("dd.MM.yyyy");
                    }
                    ws.Cells["B" + i].Style.Font.Name = "Calibri";
                    ws.Cells["B" + i].Style.Font.Size = 11;
                    ws.Cells["B" + i].Style.WrapText = true;
                    ws.Cells["B" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    if ((record + 9) != i)
                    {
                        ws.Cells["C" + i].Value = detail.session_end_date.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        ws.Cells["C" + i].Value = "-";
                    }
                    ws.Cells["C" + i].Style.Font.Name = "Calibri";
                    ws.Cells["C" + i].Style.Font.Size = 11;
                    ws.Cells["C" + i].Style.WrapText = true;
                    ws.Cells["C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if ((record + 9) == i)
                    {
                        ws.Cells["D" + i].Value = detail.nso_date.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        ws.Cells["D" + i].Value = "-";
                    }
                    ws.Cells["D" + i].Style.Font.Name = "Calibri";
                    ws.Cells["D" + i].Style.Font.Size = 11;
                    ws.Cells["D" + i].Style.WrapText = true;
                    ws.Cells["D" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["E" + i + ":G" + i].Merge = true;
                    ws.Cells["E" + i].Style.Font.Name = "Calibri";
                    ws.Cells["E" + i].Style.Font.Size = 11;
                    ws.Cells["E" + i].Style.WrapText = true;
                    ws.Cells["E" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["H" + i].Value = detail.session;
                    ws.Cells["H" + i].Style.Font.Name = "Calibri";
                    ws.Cells["H" + i].Style.Font.Size = 11;
                    ws.Cells["H" + i].Style.WrapText = true;
                    ws.Cells["H" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["H" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["I" + i].Style.Font.Name = "Calibri";
                    ws.Cells["I" + i].Style.Font.Size = 11;
                    ws.Cells["I" + i].Style.WrapText = true;
                    ws.Cells["I" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["I" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["J" + i].Style.Font.Name = "Calibri";
                    ws.Cells["J" + i].Style.Font.Size = 11;
                    ws.Cells["J" + i].Style.WrapText = true;
                    ws.Cells["J" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["J" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["J" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["K" + i].Style.Font.Name = "Calibri";
                    ws.Cells["K" + i].Style.Font.Size = 11;
                    ws.Cells["K" + i].Style.WrapText = true;
                    ws.Cells["K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["K" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    i++;
                }

                for (; i <= 24; i++)
                {
                    ws.Row(i).Height = (double)19.05;

                    ws.Cells["A" + i].Style.Font.Name = "Calibri";
                    ws.Cells["A" + i].Style.Font.Size = 11;
                    ws.Cells["A" + i].Style.WrapText = true;
                    ws.Cells["A" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["B" + i].Style.Font.Name = "Calibri";
                    ws.Cells["B" + i].Style.Font.Size = 11;
                    ws.Cells["B" + i].Style.WrapText = true;
                    ws.Cells["B" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["C" + i].Style.Font.Name = "Calibri";
                    ws.Cells["C" + i].Style.Font.Size = 11;
                    ws.Cells["C" + i].Style.WrapText = true;
                    ws.Cells["C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["D" + i].Style.Font.Name = "Calibri";
                    ws.Cells["D" + i].Style.Font.Size = 11;
                    ws.Cells["D" + i].Style.WrapText = true;
                    ws.Cells["D" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["E" + i + ":G" + i].Merge = true;
                    ws.Cells["E" + i].Style.Font.Name = "Calibri";
                    ws.Cells["E" + i].Style.Font.Size = 11;
                    ws.Cells["E" + i].Style.WrapText = true;
                    ws.Cells["E" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["H" + i].Style.Font.Name = "Calibri";
                    ws.Cells["H" + i].Style.Font.Size = 11;
                    ws.Cells["H" + i].Style.WrapText = true;
                    ws.Cells["H" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["H" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["I" + i].Style.Font.Name = "Calibri";
                    ws.Cells["I" + i].Style.Font.Size = 11;
                    ws.Cells["I" + i].Style.WrapText = true;
                    ws.Cells["I" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["I" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["J" + i].Style.Font.Name = "Calibri";
                    ws.Cells["J" + i].Style.Font.Size = 11;
                    ws.Cells["J" + i].Style.WrapText = true;
                    ws.Cells["J" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["J" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["J" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["K" + i].Style.Font.Name = "Calibri";
                    ws.Cells["K" + i].Style.Font.Size = 11;
                    ws.Cells["K" + i].Style.WrapText = true;
                    ws.Cells["K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["K" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                ws.Row(27).Height = (double)29.25;
                ws.Cells["A27:K27"].Merge = true;
                ws.Cells["A27"].Value = "Cetificate that the above Scholar's Register has been posted up to date of the scholar leaving as required by the Department Rules.";
                ws.Cells["A27"].Style.Font.Name = "Calibri";
                ws.Cells["A27"].Style.Font.Size = 11;
                ws.Cells["A27"].Style.WrapText = true;
                ws.Cells["A27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["A30:D30"].Merge = true;
                ws.Cells["A30"].Value = "Prepared by: " + username;
                ws.Cells["A30"].Style.Font.Name = "Calibri";
                ws.Cells["A30"].Style.Font.Size = 11;
                ws.Cells["A30"].Style.WrapText = true;
                ws.Cells["A30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["F30:H30"].Merge = true;
                ws.Cells["F30"].Value = "Date: " + tc_date.ToString("dd/MM/yyyy");
                ws.Cells["F30"].Style.Font.Name = "Calibri";
                ws.Cells["F30"].Style.Font.Size = 11;
                ws.Cells["F30"].Style.WrapText = true;
                ws.Cells["F30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["I30:K30"].Merge = true;
                ws.Cells["I30"].Value = "Head of the Institution";
                ws.Cells["I30"].Style.Font.Name = "Calibri";
                ws.Cells["I30"].Style.Font.Size = 11;
                ws.Cells["I30"].Style.WrapText = true;
                ws.Cells["I30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                ws.Row(32).Height = (double)29.25;
                ws.Cells["A32:K32"].Merge = true;
                ws.Cells["A32"].Value = "Note: If the scholar has been among the first five position in the class, this fact should be mentioned in the column of conduct and work.";
                ws.Cells["A32"].Style.Font.Name = "Calibri";
                ws.Cells["A32"].Style.Font.Size = 11;
                ws.Cells["A32"].Style.WrapText = true;
                ws.Cells["A32"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                return pck.GetAsByteArray();
                

            }
            catch (Exception ex)
            {

            }

            return null;
        }

        public byte[] Generate_TC(int sr_number, int user_id, string username)
        {
            try
            {
               

                string query = @"SELECT 
                                    sr_number,
                                    CONCAT(IFNULL(std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, ''),
                                            ', ',
                                            IFNULL(std_nationality, ''),
                                            ', ',
                                            IFNULL(std_cast, '')) std_name,
                                    CONCAT(IFNULL(std_father_name, ''),
                                            ', ',
                                            IFNULL(std_father_occupation, ''),
                                            ', ',
                                            IFNULL(std_address, ''),
                                            ' ',
                                            IFNULL(std_address1, ''),
                                            ' ',
                                            IFNULL(std_address2, ''),
                                            ' ',
                                            IFNULL(std_district, ''),
                                            ' ',
                                            IFNULL(std_state, ''),
                                            ' ',
                                            IFNULL(std_pincode, '')) std_father,
                                    std_dob,
                                    ifnull(std_last_school,'') std_last_school
                                FROM
                                    sr_register
                                WHERE
                                    std_active = 'N' AND sr_number = @sr_number";

                var result = con.Query<tc_details>(query, new { sr_number = sr_number}).SingleOrDefault();

                query = @"SELECT 
                            session_start_date,
                            session_end_date,
                            (SELECT 
                                    class_name
                                FROM
                                    mst_class
                                WHERE
                                    class_id = a.class_id
                                        AND session = b.session) class_name,
                            c.std_admission_date,
                            nso_date,
                            b.session
                        FROM
                            mst_std_class a,
                            mst_session b,
                            sr_register c
                        WHERE
                            a.sr_num = @sr_number
                                AND a.sr_num = c.sr_number
                                AND a.session = b.session
                                AND c.std_active = 'N'";

                mst_sessionMain session = new mst_sessionMain();
                string sess = session.findFinal_Session();

                string max_id = @"select lpad(ifnull(max(tc_no),0)+1,4,0) from tc_register where session = @session";

                string tc_no = con.ExecuteScalar<string>(max_id, new {session = sess });

                IEnumerable<tc_class_details> class_details = con.Query<tc_class_details>(query, new { sr_number = sr_number });

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("TC");

                ws.PrinterSettings.TopMargin = 0.5m;
                ws.PrinterSettings.BottomMargin = 0.5m;
                ws.PrinterSettings.LeftMargin = 0.19m;
                ws.PrinterSettings.RightMargin = 0.19m;
                ws.PrinterSettings.HeaderMargin = 0.05m;
                ws.PrinterSettings.FooterMargin = 0.05m;
                ws.PrinterSettings.HorizontalCentered = true;

                ws.Column(1).Width = GetTrueColumnWidth(4.71);
                ws.Column(2).Width = GetTrueColumnWidth(9.43);
                ws.Column(3).Width = GetTrueColumnWidth(9.57);
                ws.Column(4).Width = GetTrueColumnWidth(9.86);
                ws.Column(5).Width = GetTrueColumnWidth(7.71);
                ws.Column(6).Width = GetTrueColumnWidth(5.71);
                ws.Column(7).Width = GetTrueColumnWidth(8.43);
                ws.Column(8).Width = GetTrueColumnWidth(10);
                ws.Column(9).Width = GetTrueColumnWidth(12.86);
                ws.Column(10).Width = GetTrueColumnWidth(7.71);
                ws.Column(11).Width = GetTrueColumnWidth(8.43);


                ws.Cells["A1:K1"].Merge = true;
                ws.Row(1).Height = 36;
                ws.Cells["A1"].Value = SchoolName;
                ws.Cells["A1"].Style.Font.Name = "Calibri";
                ws.Cells["A1"].Style.Font.Size = 28;
                ws.Cells["A1"].Style.Font.Bold = true;
                ws.Cells["A1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A1"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A2:K2"].Merge = true;
                ws.Cells["A2"].Value = Address;
                ws.Cells["A2"].Style.Font.Name = "Calibri";
                ws.Cells["A2"].Style.Font.Size = 11;
                ws.Cells["A2"].Style.Font.Bold = true;
                ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A2"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A4:K4"].Merge = true;
                ws.Cells["A4"].Value = "Scholar's Register & Transfer Certificate Form";
                ws.Cells["A4"].Style.Font.Name = "Bookman Old Style";
                ws.Cells["A4"].Style.Font.Size = 18;
                ws.Cells["A4"].Style.Font.Bold = true;
                ws.Cells["A4"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A4"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                ws.Cells["A5:C5"].Merge = true;
                ws.Cells["A5"].Value = "Admission File No. "+result.sr_number.ToString();
                ws.Cells["A5"].Style.Font.Name = "Calibri";
                ws.Cells["A5"].Style.Font.Size = 11;
                ws.Cells["A5"].Style.Font.Bold = true;
                ws.Cells["A5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A5:C5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:C5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:C5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A5:C5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D5:F5"].Merge = true;
                ws.Cells["D5"].Value = "Withdrawal File No.";
                ws.Cells["D5"].Style.Font.Name = "Calibri";
                ws.Cells["D5"].Style.Font.Size = 11;
                ws.Cells["D5"].Style.Font.Bold = true;
                ws.Cells["D5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["D5:F5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D5:F5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D5:F5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D5:F5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["G5:I5"].Merge = true;
                ws.Cells["G5"].Value = "Transfer Certificate No. "+sess+"/"+tc_no.ToString();
                ws.Cells["G5"].Style.Font.Name = "Calibri";
                ws.Cells["G5"].Style.Font.Size = 11;
                ws.Cells["G5"].Style.Font.Bold = true;
                ws.Cells["G5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["G5:I5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["G5:I5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["G5:I5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["G5:I5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                ws.Cells["J5:K5"].Merge = true;
                ws.Cells["J5"].Value = "Register No.";
                ws.Cells["J5"].Style.Font.Name = "Calibri";
                ws.Cells["J5"].Style.Font.Size = 11;
                ws.Cells["J5"].Style.Font.Bold = true;
                ws.Cells["J5"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["J5:K5"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["J5:K5"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["J5:K5"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["J5:K5"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Row(6).Height = 48;

                ws.Cells["A6:C6"].Merge = true;
                ws.Cells["A6"].Value = "Name of the Scholar with nationality and caste if Hindu, otherwise religion.";
                ws.Cells["A6"].Style.Font.Name = "Calibri";
                ws.Cells["A6"].Style.Font.Size = 11;
                ws.Cells["A6"].Style.Font.Bold = true;
                ws.Cells["A6"].Style.WrapText = true;
                ws.Cells["A6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A6:C6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:C6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:C6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A6:C6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D6:G6"].Merge = true;
                ws.Cells["D6"].Value = "Name, Occupation & Address of Parents, Guardian or Husband";
                ws.Cells["D6"].Style.Font.Name = "Calibri";
                ws.Cells["D6"].Style.Font.Size = 11;
                ws.Cells["D6"].Style.Font.Bold = true;
                ws.Cells["D6"].Style.WrapText = true;
                ws.Cells["D6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["D6:G6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D6:G6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D6:G6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D6:G6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H6"].Value = "Date of birth of the Scholar";
                ws.Cells["H6"].Style.Font.Name = "Calibri";
                ws.Cells["H6"].Style.Font.Size = 11;
                ws.Cells["H6"].Style.Font.Bold = true;
                ws.Cells["H6"].Style.WrapText = true;
                ws.Cells["H6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["H6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I6:K6"].Merge = true;
                ws.Cells["I6"].Value = "The last institution if any which the Scholar attended for joining this institution";
                ws.Cells["I6"].Style.Font.Name = "Calibri";
                ws.Cells["I6"].Style.Font.Size = 11;
                ws.Cells["I6"].Style.Font.Bold = true;
                ws.Cells["I6"].Style.WrapText = true;
                ws.Cells["I6"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I6"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["I6:K6"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I6:K6"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I6:K6"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I6:K6"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Row(7).Height = 48;

                ws.Cells["A7:C7"].Merge = true;
                ws.Cells["A7"].Value = result.std_name;
                ws.Cells["A7"].Style.Font.Name = "Calibri";
                ws.Cells["A7"].Style.Font.Size = 11;
                ws.Cells["A7"].Style.WrapText = true;
                ws.Cells["A7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A7:C7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:C7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:C7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A7:C7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D7:G7"].Merge = true;
                ws.Cells["D7"].Value = result.std_father;
                ws.Cells["D7"].Style.Font.Name = "Calibri";
                ws.Cells["D7"].Style.Font.Size = 11;
                ws.Cells["D7"].Style.WrapText = true;
                ws.Cells["D7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["D7:G7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D7:G7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D7:G7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D7:G7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H7"].Value = result.std_dob.ToString("dd.MM.yyyy");
                ws.Cells["H7"].Style.Font.Name = "Calibri";
                ws.Cells["H7"].Style.Font.Size = 11;
                ws.Cells["H7"].Style.WrapText = true;
                ws.Cells["H7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["H7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I7:K7"].Merge = true;
                ws.Cells["I7"].Value = result.std_last_school;
                ws.Cells["I7"].Style.Font.Name = "Calibri";
                ws.Cells["I7"].Style.Font.Size = 11;
                ws.Cells["I7"].Style.WrapText = true;
                ws.Cells["I7"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I7"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["I7:K7"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I7:K7"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I7:K7"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I7:K7"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                

                ws.Cells["A8:K8"].Merge = true;
                ws.Cells["A8"].Value = "Date of birth in words: " + birth_certificateMain.DateToWritten(result.std_dob).ToString();
                ws.Cells["A8"].Style.Font.Name = "Calibri";
                ws.Cells["A8"].Style.Font.Size = 11;
                ws.Cells["A8"].Style.WrapText = true;
                ws.Cells["A8"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
                ws.Cells["A8:K8"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A8:K8"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A8:K8"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A8:K8"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Row(9).Height = (double)48.75;

                ws.Cells["A9"].Value = "Class";
                ws.Cells["A9"].Style.Font.Name = "Calibri";
                ws.Cells["A9"].Style.Font.Size = 11;
                ws.Cells["A9"].Style.WrapText = true;
                ws.Cells["A9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["A9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["A9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                
                ws.Cells["B9"].Value = "Date Of Admission";
                ws.Cells["B9"].Style.Font.Name = "Calibri";
                ws.Cells["B9"].Style.Font.Size = 11;
                ws.Cells["B9"].Style.WrapText = true;
                ws.Cells["B9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["B9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["B9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["C9"].Value = "Date of Promotion";
                ws.Cells["C9"].Style.Font.Name = "Calibri";
                ws.Cells["C9"].Style.Font.Size = 11;
                ws.Cells["C9"].Style.WrapText = true;
                ws.Cells["C9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["C9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["C9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["C9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["C9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["C9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D9"].Value = "Date of Removal";
                ws.Cells["D9"].Style.Font.Name = "Calibri";
                ws.Cells["D9"].Style.Font.Size = 11;
                ws.Cells["D9"].Style.WrapText = true;
                ws.Cells["D9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["D9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["D9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["E9:G9"].Merge = true;
                ws.Cells["E9"].Value = "Cause of removal e.g Non Payment of dues, transfer of Family, expulsion etc.";
                ws.Cells["E9"].Style.Font.Name = "Calibri";
                ws.Cells["E9"].Style.Font.Size = 11;
                ws.Cells["E9"].Style.WrapText = true;
                ws.Cells["E9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["E9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["E9:G9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["E9:G9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["E9:G9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["E9:G9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H9"].Value = "Year";
                ws.Cells["H9"].Style.Font.Name = "Calibri";
                ws.Cells["H9"].Style.Font.Size = 11;
                ws.Cells["H9"].Style.WrapText = true;
                ws.Cells["H9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["H9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["H9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["I9"].Value = "Conduct";
                ws.Cells["I9"].Style.Font.Name = "Calibri";
                ws.Cells["I9"].Style.Font.Size = 11;
                ws.Cells["I9"].Style.WrapText = true;
                ws.Cells["I9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["I9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["I9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["J9"].Value = "Work";
                ws.Cells["J9"].Style.Font.Name = "Calibri";
                ws.Cells["J9"].Style.Font.Size = 11;
                ws.Cells["J9"].Style.WrapText = true;
                ws.Cells["J9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["J9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["J9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["J9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["J9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["J9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["K9"].Value = "Principal Signature";
                ws.Cells["K9"].Style.Font.Name = "Calibri";
                ws.Cells["K9"].Style.Font.Size = 11;
                ws.Cells["K9"].Style.WrapText = true;
                ws.Cells["K9"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                ws.Cells["K9"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                ws.Cells["K9"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["K9"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["K9"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["K9"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                int i = 10;
                int record = class_details.Count();
                foreach (var detail in class_details)
                {
                    ws.Row(i).Height = (double)19.05;

                    ws.Cells["A" + i].Value = detail.class_name;
                    ws.Cells["A" + i].Style.Font.Name = "Calibri";
                    ws.Cells["A" + i].Style.Font.Size = 11;
                    ws.Cells["A" + i].Style.WrapText = true;
                    ws.Cells["A" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if (10 == i)
                    {
                        ws.Cells["B" + i].Value = detail.std_admission_date.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        ws.Cells["B" + i].Value = detail.session_start_date.ToString("dd.MM.yyyy");
                    }
                    ws.Cells["B" + i].Style.Font.Name = "Calibri";
                    ws.Cells["B" + i].Style.Font.Size = 11;
                    ws.Cells["B" + i].Style.WrapText = true;
                    ws.Cells["B" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                   
                    if ((record + 9) != i)
                    {
                        ws.Cells["C" + i].Value = detail.session_end_date.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        ws.Cells["C" + i].Value = "-";
                    }
                    ws.Cells["C" + i].Style.Font.Name = "Calibri";
                    ws.Cells["C" + i].Style.Font.Size = 11;
                    ws.Cells["C" + i].Style.WrapText = true;
                    ws.Cells["C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if ((record + 9) == i)
                    {
                        ws.Cells["D" + i].Value = detail.nso_date.ToString("dd.MM.yyyy");
                    }
                    else
                    {
                        ws.Cells["D" + i].Value = "-";
                    }
                    ws.Cells["D" + i].Style.Font.Name = "Calibri";
                    ws.Cells["D" + i].Style.Font.Size = 11;
                    ws.Cells["D" + i].Style.WrapText = true;
                    ws.Cells["D" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["E"+i+":G"+i].Merge = true;
                    ws.Cells["E" + i].Style.Font.Name = "Calibri";
                    ws.Cells["E" + i].Style.Font.Size = 11;
                    ws.Cells["E" + i].Style.WrapText = true;
                    ws.Cells["E" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["H" + i].Value = detail.session;
                    ws.Cells["H" + i].Style.Font.Name = "Calibri";
                    ws.Cells["H" + i].Style.Font.Size = 11;
                    ws.Cells["H" + i].Style.WrapText = true;
                    ws.Cells["H" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["H" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                
                    ws.Cells["I" + i].Style.Font.Name = "Calibri";
                    ws.Cells["I" + i].Style.Font.Size = 11;
                    ws.Cells["I" + i].Style.WrapText = true;
                    ws.Cells["I" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["I" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    
                    ws.Cells["J" + i].Style.Font.Name = "Calibri";
                    ws.Cells["J" + i].Style.Font.Size = 11;
                    ws.Cells["J" + i].Style.WrapText = true;
                    ws.Cells["J" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["J" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["J" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    
                    ws.Cells["K" + i].Style.Font.Name = "Calibri";
                    ws.Cells["K" + i].Style.Font.Size = 11;
                    ws.Cells["K" + i].Style.WrapText = true;
                    ws.Cells["K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["K" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    i++;
                }

                for(; i <= 24;i++)
                {
                    ws.Row(i).Height = (double)19.05;

                    ws.Cells["A" + i].Style.Font.Name = "Calibri";
                    ws.Cells["A" + i].Style.Font.Size = 11;
                    ws.Cells["A" + i].Style.WrapText = true;
                    ws.Cells["A" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["A" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                 
                    ws.Cells["B" + i].Style.Font.Name = "Calibri";
                    ws.Cells["B" + i].Style.Font.Size = 11;
                    ws.Cells["B" + i].Style.WrapText = true;
                    ws.Cells["B" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["B" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                   
                    ws.Cells["C" + i].Style.Font.Name = "Calibri";
                    ws.Cells["C" + i].Style.Font.Size = 11;
                    ws.Cells["C" + i].Style.WrapText = true;
                    ws.Cells["C" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["D" + i].Style.Font.Name = "Calibri";
                    ws.Cells["D" + i].Style.Font.Size = 11;
                    ws.Cells["D" + i].Style.WrapText = true;
                    ws.Cells["D" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["D" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["E" + i + ":G" + i].Merge = true;
                    ws.Cells["E" + i].Style.Font.Name = "Calibri";
                    ws.Cells["E" + i].Style.Font.Size = 11;
                    ws.Cells["E" + i].Style.WrapText = true;
                    ws.Cells["E" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E" + i + ":G" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                   
                    ws.Cells["H" + i].Style.Font.Name = "Calibri";
                    ws.Cells["H" + i].Style.Font.Size = 11;
                    ws.Cells["H" + i].Style.WrapText = true;
                    ws.Cells["H" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["H" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["I" + i].Style.Font.Name = "Calibri";
                    ws.Cells["I" + i].Style.Font.Size = 11;
                    ws.Cells["I" + i].Style.WrapText = true;
                    ws.Cells["I" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["I" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["J" + i].Style.Font.Name = "Calibri";
                    ws.Cells["J" + i].Style.Font.Size = 11;
                    ws.Cells["J" + i].Style.WrapText = true;
                    ws.Cells["J" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["J" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["J" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["K" + i].Style.Font.Name = "Calibri";
                    ws.Cells["K" + i].Style.Font.Size = 11;
                    ws.Cells["K" + i].Style.WrapText = true;
                    ws.Cells["K" + i].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["K" + i].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    ws.Cells["K" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                }

                ws.Row(27).Height = (double)29.25;
                ws.Cells["A27:K27"].Merge = true;
                ws.Cells["A27"].Value = "Cetificate that the above Scholar's Register has been posted up to date of the scholar leaving as required by the Department Rules.";
                ws.Cells["A27"].Style.Font.Name = "Calibri";
                ws.Cells["A27"].Style.Font.Size = 11;
                ws.Cells["A27"].Style.WrapText = true;
                ws.Cells["A27"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["A30:D30"].Merge = true;
                ws.Cells["A30"].Value = "Prepared by: "+ username;
                ws.Cells["A30"].Style.Font.Name = "Calibri";
                ws.Cells["A30"].Style.Font.Size = 11;
                ws.Cells["A30"].Style.WrapText = true;
                ws.Cells["A30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;
            
                ws.Cells["F30:H30"].Merge = true;
                ws.Cells["F30"].Value = "Date: " + System.DateTime.Now.ToString("dd/MM/yyyy");
                ws.Cells["F30"].Style.Font.Name = "Calibri";
                ws.Cells["F30"].Style.Font.Size = 11;
                ws.Cells["F30"].Style.WrapText = true;
                ws.Cells["F30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                ws.Cells["I30:K30"].Merge = true;
                ws.Cells["I30"].Value = "Head of the Institution";
                ws.Cells["I30"].Style.Font.Name = "Calibri";
                ws.Cells["I30"].Style.Font.Size = 11;
                ws.Cells["I30"].Style.WrapText = true;
                ws.Cells["I30"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;

                ws.Row(32).Height = (double)29.25;
                ws.Cells["A32:K32"].Merge = true;
                ws.Cells["A32"].Value = "Note: If the scholar has been among the first five position in the class, this fact should be mentioned in the column of conduct and work.";
                ws.Cells["A32"].Style.Font.Name = "Calibri";
                ws.Cells["A32"].Style.Font.Size = 11;
                ws.Cells["A32"].Style.WrapText = true;
                ws.Cells["A32"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;

                

                query = @"INSERT INTO `tc_register`
                        (`session`,
                        `tc_no`,
                        `tc_date`,
                        `sr_num`,
                        `user_id`)
                        VALUES
                        (@session,
                        @tc_no,
                        @tc_date,
                        @sr_num,
                        @user_id)";

               

                con.Execute(query, new {session = sess,tc_no = tc_no,tc_date = System.DateTime.Now,sr_num = sr_number, user_id = user_id });

                query = @"UPDATE `sr_register`
                        SET
                        `tc_generated` = 1
                        WHERE `sr_number` = @sr_number";

                con.Execute(query, new { sr_number = sr_number});

                return pck.GetAsByteArray();

            }
            catch(Exception ex)
            {

            }
            return null;
        }

        public static double GetTrueColumnWidth(double width)
        {
            //DEDUCE WHAT THE COLUMN WIDTH WOULD REALLY GET SET TO
            double z = 1d;
            if (width >= (1 + 2 / 3))
            {
                z = Math.Round((Math.Round(7 * (width - 1 / 256), 0) - 5) / 7, 2);
            }
            else
            {
                z = Math.Round((Math.Round(12 * (width - 1 / 256), 0) - Math.Round(5 * width, 0)) / 12, 2);
            }

            //HOW FAR OFF? (WILL BE LESS THAN 1)
            double errorAmt = width - z;

            //CALCULATE WHAT AMOUNT TO TACK ONTO THE ORIGINAL AMOUNT TO RESULT IN THE CLOSEST POSSIBLE SETTING 
            double adj = 0d;
            if (width >= (1 + 2 / 3))
            {
                adj = (Math.Round(7 * errorAmt - 7 / 256, 0)) / 7;
            }
            else
            {
                adj = ((Math.Round(12 * errorAmt - 12 / 256, 0)) / 12) + (2 / 12);
            }

            //RETURN A SCALED-VALUE THAT SHOULD RESULT IN THE NEAREST POSSIBLE VALUE TO THE TRUE DESIRED SETTING
            if (z > 0)
            {
                return width + adj;
            }

            return 0d;
        }
    }
}