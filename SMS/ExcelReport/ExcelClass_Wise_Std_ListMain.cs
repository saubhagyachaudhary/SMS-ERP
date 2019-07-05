using Dapper;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Linq;
using System.Web;

namespace SMS.ExcelReport
{
    public class ExcelClass_Wise_Std_ListMain
    {
        

        public void ExcelClass_Wise_Std_List(int class_id, int section_id, string session)
        {


            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    IEnumerable<repClass_Wise_Std_List> result = Enumerable.Empty<repClass_Wise_Std_List>();

                    string query = @"SELECT 
                                    sr_number,
                                    (SELECT 
                                            roll_number
                                        FROM
                                            mst_rollnumber
                                        WHERE
                                            session = c.session
                                                AND sr_num = a.sr_number) roll_number,
                                    CONCAT(IFNULL(std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, '')) std_name,
                                    std_father_name,
                                    std_mother_name,
                                    std_dob,
                                    CASE
                                        WHEN std_sex = 'M' THEN 'Male'
                                        ELSE 'Female'
                                    END std_sex,
                                    std_contact,
                                    std_contact1,
                                    std_contact2,
                                    d.pickup_point,
                                    CONCAT(IFNULL(a.std_address, ''),
                                            ' ',
                                            IFNULL(a.std_address1, ''),
                                            ' ',
                                            IFNULL(a.std_address2, ''),
                                            ' ',
                                            IFNULL(a.std_district, ''),
                                            ' ',
                                            IFNULL(a.std_state, ''),
                                            ' ',
                                            IFNULL(a.std_pincode, '')) address
                            FROM
                                sr_register a,
                                mst_std_class b,
                                mst_std_section c,
                                mst_transport d
                            WHERE
                                a.sr_number = b.sr_num
                                    AND b.sr_num = c.sr_num
                                    AND a.std_pickup_id = d.pickup_id
                                    AND d.session = b.session
                                    AND b.session = c.session
                                    AND c.session = @session
                                    AND a.std_active = 'Y'
                                    AND b.class_id = @class_id
                                    AND c.section_id = @section_id
                            ORDER BY roll_number";

                    result = con.Query<repClass_Wise_Std_List>(query, new { session = session, class_id = class_id, section_id = section_id });

                    query = @"SELECT 
                                CONCAT(a.class_name,' ',b.section_name)
                            FROM
                                mst_class a,
                                mst_section b
                            WHERE
                                a.class_id = b.class_id
                                    AND a.class_id = @class_id
                                    AND b.section_id = @section_id
                                    AND a.session = b.session
                                    AND a.session = @session";


                    string worksheet = con.Query<string>(query, new { session = session, class_id = class_id, section_id = section_id }).SingleOrDefault();



                    ExcelPackage pck = new ExcelPackage();
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add(worksheet);

                    ws.Cells["A1"].Value = "Serial No";
                    ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["B1"].Value = "Admission No";
                    ws.Cells["B1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C1"].Value = "Roll Number";
                    ws.Cells["C1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["D1"].Value = "Student Name";
                    ws.Cells["D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["E1"].Value = "Father's Name";
                    ws.Cells["E1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["F1"].Value = "Mother's Name";
                    ws.Cells["F1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["G1"].Value = "Date of Birth";
                    ws.Cells["G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["H1"].Value = "Student Gender";
                    ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["I1"].Value = "Student Contact";
                    ws.Cells["I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["J1"].Value = "Father Contact";
                    ws.Cells["J1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["K1"].Value = "Mother Contact";
                    ws.Cells["K1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["L1"].Value = "Pickup Point";
                    ws.Cells["L1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["L1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["L1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["L1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["M1"].Value = "Address";
                    ws.Cells["M1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["M1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["M1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["M1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["A1:M1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A1:M1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#63A8E1"));
                    ws.Cells["A1:M1"].AutoFilter = true;



                    int rowStart = 2;
                    int line = 1;
                    int serial = 1;
                    foreach (var item in result)
                    {
                        if (line == 2)
                        {

                            ws.Cells[string.Format("A{0}:M{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:M{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C5D9F1"));

                        }
                        else
                        {


                            ws.Cells[string.Format("A{0}:M{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:M{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffffff"));


                        }

                        ws.Cells[string.Format("A{0}", rowStart)].Value = serial;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("B{0}", rowStart)].Value = item.sr_number;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("C{0}", rowStart)].Value = item.roll_number;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        ws.Cells[string.Format("D{0}", rowStart)].Value = item.std_name;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        ws.Cells[string.Format("E{0}", rowStart)].Value = item.std_father_name;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("F{0}", rowStart)].Value = item.std_mother_name;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("G{0}", rowStart)].Value = item.std_dob.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("H{0}", rowStart)].Value = item.std_sex;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("I{0}", rowStart)].Value = item.std_contact;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("J{0}", rowStart)].Value = item.std_contact1;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("K{0}", rowStart)].Value = item.std_contact2;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("L{0}", rowStart)].Value = item.pickup_point;
                        ws.Cells[string.Format("L{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("L{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("L{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("L{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("M{0}", rowStart)].Value = item.address;
                        ws.Cells[string.Format("M{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("M{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("M{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("M{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        rowStart++;
                        serial++;

                        if (line == 2)
                            line = 1;
                        else
                            line++;
                    }

                    ws.View.FreezePanes(2, 1);
                    ws.Cells["A:L"].AutoFitColumns();
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=class student list.xlsx");
                    HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                    HttpContext.Current.Response.End();
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

            }
        }

        public void Excelsession_new_admission(string session)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    IEnumerable<repClass_Wise_Std_List> result = Enumerable.Empty<repClass_Wise_Std_List>();

                    string query = @"SELECT 
                                    sr_number,
                                    CONCAT(IFNULL(std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, '')) std_name,
                                    std_father_name,
                                    std_mother_name,
                                    COALESCE(std_contact, std_contact1, std_contact2) std_contact,
                                    d.pickup_point,
                                    e.class_name,
                                    f.section_name,
                                    a.std_admission_date
                                FROM
                                    sr_register a,
                                    mst_std_class b,
                                    mst_std_section c,
                                    mst_transport d,
                                    mst_class e,
                                    mst_section f
                                WHERE
                                    a.sr_number = b.sr_num
                                        AND b.sr_num = c.sr_num
                                        AND a.std_pickup_id = d.pickup_id
                                        AND d.session = b.session
                                        AND b.session = c.session
                                        AND c.session = e.session
                                        AND e.session = f.session
                                        AND b.class_id = e.class_id
                                        AND e.class_id = f.class_id
                                        AND c.section_id = f.section_id
                                        AND a.adm_session = @session
                                        AND a.std_active = 'Y'
                                ORDER BY a.sr_number DESC";

                    result = con.Query<repClass_Wise_Std_List>(query, new { session = session });


                    ExcelPackage pck = new ExcelPackage();
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("New Admission");

                    ws.Cells["A1"].Value = "Serial No";
                    ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["B1"].Value = "Admission No";
                    ws.Cells["B1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C1"].Value = "Student Name";
                    ws.Cells["C1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["D1"].Value = "Father's Name";
                    ws.Cells["D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["E1"].Value = "Mother's Name";
                    ws.Cells["E1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["F1"].Value = "Contact";
                    ws.Cells["F1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["G1"].Value = "Pickup Point";
                    ws.Cells["G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["H1"].Value = "Class Name";
                    ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["I1"].Value = "Section Name";
                    ws.Cells["I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["J1"].Value = "Admission Date";
                    ws.Cells["J1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["A1:J1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#63A8E1"));
                    ws.Cells["A1:J1"].AutoFilter = true;



                    int rowStart = 2;
                    int line = 1;
                    int serial = 1;
                    foreach (var item in result)
                    {
                        if (line == 2)
                        {

                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C5D9F1"));

                        }
                        else
                        {


                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffffff"));


                        }

                        ws.Cells[string.Format("A{0}", rowStart)].Value = serial;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("B{0}", rowStart)].Value = item.sr_number;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        ws.Cells[string.Format("C{0}", rowStart)].Value = item.std_name;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        ws.Cells[string.Format("D{0}", rowStart)].Value = item.std_father_name;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("E{0}", rowStart)].Value = item.std_mother_name;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("F{0}", rowStart)].Value = item.std_contact;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("G{0}", rowStart)].Value = item.pickup_point;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("H{0}", rowStart)].Value = item.class_name;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("I{0}", rowStart)].Value = item.section_name;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("J{0}", rowStart)].Value = item.std_admission_date.ToString("dd/MM/yyyy");
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        rowStart++;
                        serial++;

                        if (line == 2)
                            line = 1;
                        else
                            line++;
                    }

                    ws.View.FreezePanes(2, 1);
                    ws.Cells["A:j"].AutoFitColumns();
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=New student list.xlsx");
                    HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                    HttpContext.Current.Response.End();
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

            }
        }

        public void ExcelSchool_Strength(string session)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    IEnumerable<repClass_Wise_Std_List> result = Enumerable.Empty<repClass_Wise_Std_List>();

                    string query = @"SELECT 
                                    sr_number,
                                    CONCAT(IFNULL(std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, '')) std_name,
                                    std_father_name,
                                    std_mother_name,
                                    COALESCE(std_contact, std_contact1, std_contact2) std_contact,
                                    d.pickup_point,
                                    e.class_name,
                                    f.section_name,
                                    CONCAT(IFNULL(a.std_address, ''),
                                            ' ',
                                            IFNULL(a.std_address1, ''),
                                            ' ',
                                            IFNULL(a.std_address2, ''),
                                            ' ',
                                            IFNULL(a.std_district, ''),
                                            ' ',
                                            IFNULL(a.std_state, ''),
                                            ' ',
                                            IFNULL(a.std_pincode, '')) address
                                FROM
                                    sr_register a,
                                    mst_std_class b,
                                    mst_std_section c,
                                    mst_transport d,
                                    mst_class e,
                                    mst_section f
                                WHERE
                                    a.sr_number = b.sr_num
                                        AND b.sr_num = c.sr_num
                                        AND a.std_pickup_id = d.pickup_id
                                        AND d.session = b.session
                                        AND b.session = c.session
                                        AND c.session = e.session
                                        AND e.session = f.session
                                        AND b.class_id = e.class_id
                                        AND e.class_id = f.class_id
                                        AND c.section_id = f.section_id
                                        AND f.session = @session
                                        AND a.std_active = 'Y'
                                ORDER BY e.order_by";

                    result = con.Query<repClass_Wise_Std_List>(query, new { session = session });


                    ExcelPackage pck = new ExcelPackage();
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("All Student List");

                    ws.Cells["A1"].Value = "Serial No";
                    ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["B1"].Value = "Admission No";
                    ws.Cells["B1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C1"].Value = "Student Name";
                    ws.Cells["C1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["D1"].Value = "Father's Name";
                    ws.Cells["D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["E1"].Value = "Mother's Name";
                    ws.Cells["E1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["F1"].Value = "Contact";
                    ws.Cells["F1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["G1"].Value = "Pickup Point";
                    ws.Cells["G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["H1"].Value = "Class Name";
                    ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["I1"].Value = "Section Name";
                    ws.Cells["I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["J1"].Value = "Address";
                    ws.Cells["J1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells["A1:J1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A1:J1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#63A8E1"));
                    ws.Cells["A1:J1"].AutoFilter = true;



                    int rowStart = 2;
                    int line = 1;
                    int serial = 1;
                    foreach (var item in result)
                    {
                        if (line == 2)
                        {

                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#C5D9F1"));

                        }
                        else
                        {


                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:J{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffffff"));


                        }

                        ws.Cells[string.Format("A{0}", rowStart)].Value = serial;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("B{0}", rowStart)].Value = item.sr_number;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        ws.Cells[string.Format("C{0}", rowStart)].Value = item.std_name;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        ws.Cells[string.Format("D{0}", rowStart)].Value = item.std_father_name;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("E{0}", rowStart)].Value = item.std_mother_name;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("F{0}", rowStart)].Value = item.std_contact;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("G{0}", rowStart)].Value = item.pickup_point;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("H{0}", rowStart)].Value = item.class_name;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("I{0}", rowStart)].Value = item.section_name;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("J{0}", rowStart)].Value = item.address;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                        rowStart++;
                        serial++;

                        if (line == 2)
                            line = 1;
                        else
                            line++;
                    }

                    ws.View.FreezePanes(2, 1);
                    ws.Cells["A:J"].AutoFitColumns();
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=School Strength.xlsx");
                    HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                    HttpContext.Current.Response.End();
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

            }
        }
    }
}