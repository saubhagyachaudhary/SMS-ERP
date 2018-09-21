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
            String query = @"SELECT serial
                              ,session
                              ,dt_date
                              ,acc_id
                              ,sr_number
                              ,outstd_amount
                              ,rmt_amount
                              ,narration
                              ,reg_num
                          FROM out_standing
                          where reg_num = @reg_num";

            var result = con.Query<out_standing>(query, new { reg_num = reg });

            return result;
        }

        public IEnumerable<out_standing> AllOutStanding(int sr_num)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string query = @"select * from (SELECT 1,
                             serial
                            ,session
                            , dt_date
                            , a.acc_id
                            , concat(b.acc_name, ' ', ifnull(a.month_name, ' ')) acc_name
                            , a.month_no
                            , sr_number
                            , ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) outstd_amount
                            , narration
                             FROM out_standing a, mst_acc_head b
                             where sr_number = @sr_number
                             and ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) <> 0
                            and session = @session
                             and a.acc_id = b.acc_id

                             and month_no between 4 and 12
							  ) one
                               union all

                              select * from (SELECT 2,
                             serial
                            , session
                            , dt_date
                            , a.acc_id
                            , concat(b.acc_name, ' ', ifnull(a.month_name, ' ')) acc_name
                            , a.month_no
                            , sr_number
                            , ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) outstd_amount
                            , narration
                             FROM out_standing a, mst_acc_head b
                             where sr_number = @sr_number
                             and ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) <> 0
                            and session = @session
                             and a.acc_id = b.acc_id

                             and month_no between 1 and 3
							 ) two order by 1, month_no, acc_id";

            var result = con.Query<out_standing>(query, new { sr_number = sr_num, session = sess.findActive_finalSession() });
            

            return result;
        }

      
        public IEnumerable<out_standing> AllOutStandingByReg(int reg)
        {
            String query = @"SELECT serial
                            ,session
                            ,dt_date
                            ,a.acc_id
							,b.acc_name
                            ,sr_number
                            ,reg_num
                            ,ifnull(outstd_amount,0) - ifnull(rmt_amount, 0) outstd_amount
                            ,narration
                             FROM out_standing a, mst_acc_head b 
                             where reg_num = @reg_num
                             and ifnull(outstd_amount,0) - ifnull(rmt_amount,0) <> 0
							 and a.acc_id = b.acc_id";

            var result = con.Query<out_standing>(query, new { reg_num = reg });


            return result;
        }

        public void updateOutstanding(out_standing std)
        {

            try
            {
                string query = @"UPDATE out_standing
                                SET sr_number = @sr_number,
                                    class_id = @class_id                                        
                                WHERE reg_num = @reg_num
                                and dt_date = @dt_date";

                con.Execute(query, std);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     

        public void updateOutstandingReceipt(out_standing std)
        {

            try
            {
                
                string query = @"UPDATE out_standing
                                SET receipt_no = @receipt_no,
                                   receipt_date = @receipt_date,
                                   rmt_amount = ifnull(rmt_amount,0) + @rmt_amount,
                                   dc_fine = ifnull(dc_fine,0) + @dc_fine,
                                   dc_discount = ifnull(dc_discount,0) + @dc_discount,
                                   clear_flag = @clear_flag
                                   WHERE session = @session
                                and serial = @serial";

                con.Execute(query, std);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void markStdNSO(int sr_num)
        {

            try
            {
                string query1 = @"SELECT session FROM mst_session where session_active = 'Y'";

                string session = con.Query<string>(query1).SingleOrDefault();

                if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
                {
                    string query = @"delete from out_standing where sr_number = @sr_num and month_no > month(date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))) and month_no between 4 and 12 and ifnull(rmt_amount,0) = 0 and session = @session";

                    con.Execute(query, new { sr_num = sr_num, session = session });

                    query = @"delete from out_standing where sr_number = @sr_num and month_no between 1 and 3 and ifnull(rmt_amount,0) = 0 and session = @session";

                    con.Execute(query, new { sr_num = sr_num, session = session });
                }
                else
                {
                    string query = @"delete from out_standing where sr_number = @sr_num and month_no > month(date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))) and month_no between 1 and 3 and ifnull(rmt_amount,0) = 0 and session = @session";

                    con.Execute(query, new { sr_num = sr_num, session = session });
                }

                 string query2 = @"UPDATE sr_register
                               SET nso_date = @nso_date
                               WHERE sr_number = @sr_num";

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
               

                string query1 = @"SELECT session FROM mst_session where session_active = 'Y'";

                string session = con.Query<string>(query1).SingleOrDefault();




                string maxid = "select ifnull(MAX(serial),0)+1 from out_standing where session = @session";

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