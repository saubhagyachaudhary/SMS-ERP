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
    public class ExcelDaily_reportMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void ExcelCansolidated(DateTime fromdt, DateTime todt, string mode)
        {


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



                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Fees Statement");

                ws.Cells["A1"].Value = "Rcpt No.";
                ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["B1"].Value = "Rcpt Dt.";
                ws.Cells["B1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["C1"].Value = "Adm No.";
                ws.Cells["C1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D1"].Value = "Std Name.";
                ws.Cells["D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                ws.Cells["E1"].Value = "Class.";
                ws.Cells["E1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["F1"].Value = "Amt";
                ws.Cells["F1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["G1"].Value = "Mode.";
                ws.Cells["G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                if (mode == "Bank")
                {
                    ws.Cells["H1"].Value = "Bank Name";
                    ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["I1"].Value = "Inst No.";
                    ws.Cells["I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["J1"].Value = "Inst Date";
                    ws.Cells["J1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["J1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["K1"].Value = "Status";
                    ws.Cells["K1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["K1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells["A1:K1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A1:K1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#acacac"));

                }
                else
                {
                    ws.Cells["H1"].Value = "Status";
                    ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    
                    ws.Cells["A1:H1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    ws.Cells["A1:H1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#acacac"));
                }
                    

                int rowStart = 2;
                int line = 1;

                foreach (var item in result)
                {
                    if(line == 2)
                    {
                        if(mode == "Bank")
                        {
                            ws.Cells[string.Format("A{0}:K{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:K{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e3e2e1"));
                        }
                        else
                        {
                            ws.Cells[string.Format("A{0}:H{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:H{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e3e2e1"));
                        }
                       
                    }
                    else
                    {
                        if (mode == "Bank")
                        {
                            ws.Cells[string.Format("A{0}:K{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:K{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffffff"));
                        }
                        else
                        {
                            ws.Cells[string.Format("A{0}:H{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:H{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffffff"));
                        }

                      
                    }

                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.receipt_no;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.receipt_date.ToString("dd/MM/yyyy");
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if (item.sr_number == 0)
                    {
                        ws.Cells[string.Format("C{0}", rowStart)].Value = "";
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    else
                    {
                        ws.Cells[string.Format("C{0}", rowStart)].Value = item.sr_number;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                        
                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.std_name;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.class_name;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.amount;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("G{0}", rowStart)].Value = item.mode_flag;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if (mode == "Bank")
                    {
                        ws.Cells[string.Format("H{0}", rowStart)].Value = item.bnk_name;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("I{0}", rowStart)].Value = item.chq_no;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("J{0}", rowStart)].Value = item.chq_date;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("J{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        ws.Cells[string.Format("K{0}", rowStart)].Value = item.chq_reject;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("K{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    else
                    {
                        ws.Cells[string.Format("H{0}", rowStart)].Value = item.chq_reject;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    rowStart++;


                    if (line == 2)
                        line = 1;
                    else
                        line++;
                }

                ws.Cells[string.Format("F{0}", rowStart)].Formula = String.Format("=SUM(F2:F{0})",rowStart-1);

                ws.Cells["A:K"].AutoFitColumns();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=Fees Statement.xlsx");
                HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.OutputStream.Flush();
                HttpContext.Current.Response.OutputStream.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
               
            }
        }

        public void ExcelDetailed(DateTime fromdt, DateTime todt, string mode)
        {
            
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


                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Fees Statement");

                ws.Cells["A1"].Value = "Rcpt No.";
                ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["B1"].Value = "Rcpt Dt.";
                ws.Cells["B1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["C1"].Value = "Adm No.";
                ws.Cells["C1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D1"].Value = "Std Name.";
                ws.Cells["D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["E1"].Value = "Fees.";
                ws.Cells["E1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["F1"].Value = "Class.";
                ws.Cells["F1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["G1"].Value = "Amt";
                ws.Cells["G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H1"].Value = "Mode.";
                ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

               
                ws.Cells["I1"].Value = "Status";
                ws.Cells["I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["A1:I1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#acacac"));
                


                int rowStart = 2;
                int line = 1;

                foreach (var item in result)
                {
                    if (line == 2)
                    {
                       
                            ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e3e2e1"));
                       
                    }
                    else
                    {
                       
                            ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffffff"));
                       
                    }

                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.receipt_no;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.receipt_date.ToString("dd/MM/yyyy");
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if (item.sr_number == 0)
                    {
                        ws.Cells[string.Format("C{0}", rowStart)].Value = "";
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    else
                    {
                        ws.Cells[string.Format("C{0}", rowStart)].Value = item.sr_number;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.std_name;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.fees_name;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.class_name;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("G{0}", rowStart)].Value = item.amount;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("H{0}", rowStart)].Value = item.mode_flag;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                
                   
                    ws.Cells[string.Format("I{0}", rowStart)].Value = item.chq_reject;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                   
                    rowStart++;


                    if (line == 2)
                        line = 1;
                    else
                        line++;
                }

                ws.Cells[string.Format("G{0}", rowStart)].Formula = String.Format("=SUM(G2:G{0})", rowStart - 1);

                ws.Cells["A:K"].AutoFitColumns();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=Fees Statement.xlsx");
                HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.OutputStream.Flush();
                HttpContext.Current.Response.OutputStream.Close();

            }

            catch (Exception ex)
            {
                throw ex;
            }

            
            finally
            {
                
            }
        }

        public void ExcelHeadWiseStatement(DateTime fromdt, DateTime todt, string mode, string session, int acc_id)
        {
           
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

                ExcelPackage pck = new ExcelPackage();
                ExcelWorksheet ws = pck.Workbook.Worksheets.Add("Fees Statement");

                ws.Cells["A1"].Value = "Rcpt No.";
                ws.Cells["A1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["A1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["B1"].Value = "Rcpt Dt.";
                ws.Cells["B1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["B1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["C1"].Value = "Adm No.";
                ws.Cells["C1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["C1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["D1"].Value = "Std Name.";
                ws.Cells["D1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["D1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["E1"].Value = "Fees.";
                ws.Cells["E1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["E1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["F1"].Value = "Class.";
                ws.Cells["F1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["F1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["G1"].Value = "Amt";
                ws.Cells["G1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["G1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["H1"].Value = "Mode.";
                ws.Cells["H1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["H1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["H1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["H1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                ws.Cells["I1"].Value = "Status";
                ws.Cells["I1"].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                ws.Cells["I1"].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                ws.Cells["I1"].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                ws.Cells["I1"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                ws.Cells["A1:I1"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                ws.Cells["A1:I1"].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#acacac"));



                int rowStart = 2;
                int line = 1;

                foreach (var item in result)
                {
                    if (line == 2)
                    {

                        ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#e3e2e1"));

                    }
                    else
                    {

                        ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        ws.Cells[string.Format("A{0}:I{0}", rowStart)].Style.Fill.BackgroundColor.SetColor(ColorTranslator.FromHtml("#ffffff"));

                    }

                    ws.Cells[string.Format("A{0}", rowStart)].Value = item.receipt_no;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("A{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("B{0}", rowStart)].Value = item.receipt_date.ToString("dd/MM/yyyy");
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("B{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    if (item.sr_number == 0)
                    {
                        ws.Cells[string.Format("C{0}", rowStart)].Value = "";
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }
                    else
                    {
                        ws.Cells[string.Format("C{0}", rowStart)].Value = item.sr_number;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        ws.Cells[string.Format("C{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    }

                    ws.Cells[string.Format("D{0}", rowStart)].Value = item.std_name;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("D{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("E{0}", rowStart)].Value = item.fees_name;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("E{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("F{0}", rowStart)].Value = item.class_name;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("F{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("G{0}", rowStart)].Value = item.amount;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("G{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    ws.Cells[string.Format("H{0}", rowStart)].Value = item.mode_flag;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("H{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;


                    ws.Cells[string.Format("I{0}", rowStart)].Value = item.chq_reject;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    ws.Cells[string.Format("I{0}", rowStart)].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    rowStart++;


                    if (line == 2)
                        line = 1;
                    else
                        line++;
                }

                ws.Cells[string.Format("G{0}", rowStart)].Formula = String.Format("=SUM(G2:G{0})", rowStart - 1);

                ws.Cells["A:K"].AutoFitColumns();
                HttpContext.Current.Response.Clear();
                HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment: filename=Fees Statement.xlsx");
                HttpContext.Current.Response.BinaryWrite(pck.GetAsByteArray());
                HttpContext.Current.Response.End();
                HttpContext.Current.Response.OutputStream.Flush();
                HttpContext.Current.Response.OutputStream.Close();

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