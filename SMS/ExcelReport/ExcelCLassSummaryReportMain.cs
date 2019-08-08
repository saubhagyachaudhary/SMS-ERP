using Dapper;
using MySql.Data.MySqlClient;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.ExcelReport
{
    public class ExcelClassSummaryReportMain
    {

        public static void ExcelClassSummary(string session)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {

                    ExcelPackage pck = new ExcelPackage();
                    ExcelWorksheet ws = pck.Workbook.Worksheets.Add("School Student Summary Report");

                   
                    ws.Cells["A2"].Value = "Class Name";
                    ws.Cells["A2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["A2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["A2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["B1:C1"].Merge = true;
                    ws.Cells["B1"].Value = "General";
                    ws.Cells["B1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["B2"].Value = "Male";
                    ws.Cells["B2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["B2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["B2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["C2"].Value = "Female";
                    ws.Cells["C2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["C2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["C2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["D1:E1"].Merge = true;
                    ws.Cells["D1"].Value = "SC";
                    ws.Cells["D1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["D2"].Value = "Male";
                    ws.Cells["D2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["D2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["D2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["E2"].Value = "Female";
                    ws.Cells["E2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["E2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["E2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["F1:G1"].Merge = true;
                    ws.Cells["F1"].Value = "ST";
                    ws.Cells["F1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["F2"].Value = "Male";
                    ws.Cells["F2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["F2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["F2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["G2"].Value = "Female";
                    ws.Cells["G2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["G2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["G2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["H1:I1"].Merge = true;
                    ws.Cells["H1"].Value = "OBC";
                    ws.Cells["H1"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["H2"].Value = "Male";
                    ws.Cells["H2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["H2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["I2"].Value = "Female";
                    ws.Cells["I2"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    ws.Cells["I2"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I2"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I2"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I2"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    string query = @"SELECT 
                                        class_id, class_name
                                    FROM
                                        mst_Class
                                    WHERE
                                        session = @session
                                    ORDER BY order_by";

                    IEnumerable<mst_class> class_name = con.Query<mst_class>(query, new { session = session });
                    int i = 3;
                    foreach (var result in class_name)
                    {
                        query = @"SELECT 
                                        section_id, section_name
                                    FROM
                                        mst_section
                                    WHERE
                                        session = @session AND class_id = @class_id";

                        IEnumerable<mst_section> section_name = con.Query<mst_section>(query, new { session = session, class_id = result.class_id });

                        foreach(var section in section_name)
                        {
                           

                            

                            ws.Cells["A" + i].Value = result.class_name +" "+ section.Section_name;
                            ws.Cells["A" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["A" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'M'
                                            and a.std_category = 'General';";

                            int qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["B" + i].Value = qty;
                            ws.Cells["B" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["B" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["B" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["B" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'F'
                                            and a.std_category = 'General';";

                            qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["C" + i].Value = qty;
                            ws.Cells["C" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["C" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'M'
                                            and a.std_category = 'SC';";

                            qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["D" + i].Value = qty;
                            ws.Cells["D" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["D" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["D" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["D" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'F'
                                            and a.std_category = 'SC';";

                            qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["E" + i].Value = qty;
                            ws.Cells["E" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["E" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'M'
                                            and a.std_category = 'ST';";

                            qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["F" + i].Value = qty;
                            ws.Cells["F" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["F" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["F" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["F" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'F'
                                            and a.std_category = 'ST';";

                            qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["G" + i].Value = qty;
                            ws.Cells["G" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["G" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["G" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["G" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'M'
                                            and a.std_category = 'OBC';";

                            qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["H" + i].Value = qty;
                            ws.Cells["H" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["H" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            query = @"SELECT
                                        count(*) category_qty
                                    FROM
                                        sr_register a,
                                        mst_std_class b,
                                        mst_std_section c
                                    WHERE
                                        b.session = @session
                                            AND b.session = c.session
                                            and a.std_active = 'Y'
                                            and b.class_id = @class_id
                                            and c.section_id = @section_id
                                            and a.sr_number = b.sr_num
                                            and b.sr_num = c.sr_num
                                            and a.std_sex = 'F'
                                            and a.std_category = 'OBC';";

                            qty = con.Query<int>(query, new { session = session, class_id = result.class_id, section_id = section.section_id }).SingleOrDefault();

                            ws.Cells["I" + i].Value = qty;
                            ws.Cells["I" + i].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            ws.Cells["I" + i].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            ws.Cells["I" + i].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            ws.Cells["I" + i].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            i++;
                        }

                    }

                    ws.Cells["A:I"].AutoFitColumns();
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=School Student Summary Report.xlsx");
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

        public class Excel_summary
        {
            public int male { get; set; }

            public int female { get; set; }

            public int category_qty { get; set; }
        }
    }
}