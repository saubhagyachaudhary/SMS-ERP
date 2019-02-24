using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class out_standingMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        public IEnumerable<out_standing> OutStandingByReg(int reg)
        {
            string query = @"SELECT 
                                serial,
                                session,
                                dt_date,
                                acc_id,
                                sr_number,
                                outstd_amount,
                                rmt_amount,
                                narration,
                                reg_num
                            FROM
                                out_standing
                            WHERE
                                reg_num = @reg_num
                                    AND session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                            AND session_active = 'Y')";

            var result = con.Query<out_standing>(query, new { reg_num = reg });

            return result;
        }

        public IEnumerable<out_standing> AllOutStanding(int sr_num,string session)
        {
            

            string query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        1,
                                            serial,
                                            a.session,
                                            dt_date,
                                            a.acc_id,
                                            CONCAT(b.acc_name, ' ', IFNULL(a.month_name, ' ')) acc_name,
                                            a.month_no,
                                            sr_number,
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) outstd_amount,
                                            narration
                                    FROM
                                        out_standing a, mst_acc_head b
                                    WHERE
                                        sr_number = @sr_number
                                            AND IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) <> 0
                                            AND a.session = @session
                                            AND a.session = b.session
                                            AND a.acc_id IN (1 , 2)
                                            AND a.acc_id = b.acc_id) one 
                                UNION ALL SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        2,
                                            serial,
                                            a.session,
                                            dt_date,
                                            a.acc_id,
                                            CONCAT(b.acc_name, ' ', IFNULL(a.month_name, ' ')) acc_name,
                                            a.month_no,
                                            sr_number,
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) outstd_amount,
                                            narration
                                    FROM
                                        out_standing a, mst_acc_head b
                                    WHERE
                                        sr_number = @sr_number
                                            AND IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) <> 0
                                            AND a.session = @session
                                            AND a.session = b.session
                                            AND a.acc_id NOT IN (1 , 2)
                                            AND a.acc_id = b.acc_id
                                            AND month_no BETWEEN 4 AND 12) twp 
                                UNION ALL SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        3,
                                            serial,
                                            a.session,
                                            dt_date,
                                            a.acc_id,
                                            CONCAT(b.acc_name, ' ', IFNULL(a.month_name, ' ')) acc_name,
                                            a.month_no,
                                            sr_number,
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) outstd_amount,
                                            narration
                                    FROM
                                        out_standing a, mst_acc_head b
                                    WHERE
                                        sr_number = @sr_number
                                            AND IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) <> 0
                                            AND a.session = @session
                                            AND a.session = b.session
                                            AND a.acc_id NOT IN (1 , 2)
                                            AND a.acc_id = b.acc_id
                                            AND month_no BETWEEN 1 AND 3) three
                                ORDER BY 1 , month_no,acc_id";

            var result = con.Query<out_standing>(query, new { sr_number = sr_num, session = session });
            

            return result;
        }

      
        public IEnumerable<out_standing> AllOutStandingByReg(int reg)
        {
            string query = @"SELECT 
                                serial,
                                a.session,
                                dt_date,
                                a.acc_id,
                                b.acc_name,
                                sr_number,
                                reg_num,
                                IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) outstd_amount,
                                narration
                            FROM
                                out_standing a,
                                mst_acc_head b
                            WHERE
                                reg_num = @reg_num
                                    AND IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) <> 0
                                    AND a.acc_id = b.acc_id
                                    AND a.session = b.session
                                    AND a.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_active = 'Y')";

            var result = con.Query<out_standing>(query, new { reg_num = reg });


            return result;
        }

        public void updateOutstanding(out_standing std)
        {

            try
            {
                string query = @"UPDATE out_standing 
                                    SET 
                                        sr_number = @sr_number,
                                        class_id = @class_id
                                    WHERE
                                        reg_num = @reg_num
                                            AND dt_date = @dt_date
                                            AND session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_active = 'Y')";

                con.Execute(query, std);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     

        public void updateOutstandingReceipt(out_standing std,MySqlCommand myCommand)
        {
    
                    string query = @"UPDATE out_standing 
                                    SET 
                                        receipt_no = "+std.receipt_no+ @",
                                        receipt_date = '"+ std.receipt_date.ToString("yyyy-MM-dd") +@"',
                                        rmt_amount = IFNULL(rmt_amount, 0) + "+std.rmt_amount+@",
                                        dc_fine = IFNULL(dc_fine, 0) + "+std.dc_fine+@",
                                        dc_discount = IFNULL(dc_discount, 0) + "+std.dc_discount+@",
                                        clear_flag = "+std.clear_flag+@"
                                    WHERE
                                        session = '"+std.session+"' AND serial = "+std.serial;

                myCommand.CommandText = query;

                //con.Execute(query, std);
                myCommand.ExecuteNonQuery();

           
    }

        public void markStdNSO(int sr_num)
        {

            try
            {
                string query1 = @"SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_active = 'Y'";

                string session = con.Query<string>(query1).SingleOrDefault();

                if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
                {
                    string query = @"DELETE FROM out_standing 
                                    WHERE
                                        sr_number = @sr_num
                                        AND month_no > MONTH(DATE(DATE_ADD(NOW(),
                                                INTERVAL '00:00' HOUR_MINUTE)))
                                        AND month_no BETWEEN 4 AND 12
                                        AND IFNULL(rmt_amount, 0) = 0
                                        AND session = @session";

                    con.Execute(query, new { sr_num = sr_num, session = session });

                    query = @"DELETE FROM out_standing 
                                WHERE
                                    sr_number = @sr_num
                                    AND month_no BETWEEN 1 AND 3
                                    AND IFNULL(rmt_amount, 0) = 0
                                    AND session = @session";

                    con.Execute(query, new { sr_num = sr_num, session = session });
                }
                else
                {
                    string query = @"DELETE FROM out_standing 
                                        WHERE
                                            sr_number = @sr_num
                                            AND month_no > MONTH(DATE(DATE_ADD(NOW(),
                                                    INTERVAL '00:00' HOUR_MINUTE)))
                                            AND month_no BETWEEN 1 AND 3
                                            AND IFNULL(rmt_amount, 0) = 0
                                            AND session = @session";

                    con.Execute(query, new { sr_num = sr_num, session = session });
                }

                 string query2 = @"UPDATE sr_register 
                                    SET 
                                        nso_date = @nso_date
                                    WHERE
                                        sr_number = @sr_num";

                DateTime nso_date = System.DateTime.Now.AddMinutes(dateTimeOffSet);

                con.Execute(query2, new {sr_num = sr_num, nso_date = nso_date});

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void AddOutStanding(out_standing std)
        {
            try
            {
               

                string query1 = @"SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_active = 'Y'";

                string session = con.Query<string>(query1).SingleOrDefault();




                string maxid = @"SELECT 
                                    IFNULL(MAX(serial), 0) + 1
                                FROM
                                    out_standing
                                WHERE
                                    session = @session";

                int id = con.Query<int>(maxid,new {session = session }).SingleOrDefault();



                string query = @"INSERT INTO out_standing
           (serial
           ,session
           ,dt_date
           ,acc_id
           ,sr_number
           ,outstd_amount
           ,rmt_amount
           ,narration
           ,reg_num
            ,clear_flag
           ,month_name
           ,month_no
           ,class_id
           )
     VALUES
           (@serial
           ,@session
           ,@dt_date
           ,@acc_id
           ,@sr_number
           ,@outstd_amount
           ,@rmt_amount
           ,@narration
           ,@reg_num
            ,@clear_flag
            ,@MonthName
            ,@MonthNum
            ,@class_id)";

                std.serial = id;

                std.session = session;

                std.dt_date = System.DateTime.Now.AddMinutes(dateTimeOffSet);
                string MonthName = std.dt_date.ToString("MMMM") + " " + std.dt_date.Year.ToString();
                int MonthNum = std.dt_date.Month;

                std.rmt_amount = 0;

               

                con.Execute(query,
                        new
                        {
                            std.serial
                           ,
                            std.session
                           ,
                            std.dt_date
                           ,
                            std.acc_id
                           ,
                            std.sr_number
                           ,
                            std.outstd_amount
                           ,
                            std.rmt_amount
                           ,
                            std.narration
                            ,
                            std.reg_num
                            ,
                            std.clear_flag
                            ,
                            MonthName
                            ,
                            MonthNum
                            ,
                            std.class_id
                        });




            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}