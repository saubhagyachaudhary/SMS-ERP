using Dapper;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.ExcelReport
{
    public class tc_details
    {
        public int sr_number { get; set; }

        public string std_name { get; set; }

        public string std_father { get; set; }

        public DateTime std_dob { get; set; }

        public string std_last_school { get; set; }


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

    public class ExcelTc_form
    {

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        
        string Address = ConfigurationManager.AppSettings["Address"].ToString();
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public void Generate_TC(int sr_number, int user_id, string username)
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

                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=class student list.xlsx");
                HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.OutputStream.Flush();
                HttpContext.Current.Response.OutputStream.Close();

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

            }
            catch(Exception ex)
            {

            }
            
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