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

        public IEnumerable<out_standing> OutStandingByReg(int reg)
        {
            String query = @"SELECT serial
                              ,fin_id
                              ,dt_date
                              ,acc_id
                              ,sr_number
                              ,outstd_amount
                              ,rmt_amount
                              ,narration
                              ,reg_num
                          FROM sms.out_standing
                          where reg_num = @reg_num";

            var result = con.Query<out_standing>(query, new { reg_num = reg });

            return result;
        }

        public IEnumerable<out_standing> AllOutStanding(int sr_num)
        {
            /*String query = @"		select * from (		SELECT 
                             serial
                            ,fin_id
                            ,dt_date
                            ,a.acc_id
							,concat(b.acc_name ,' ' ,ifnull(a.month_name,' ')) acc_name
                            ,a.month_no
                            ,sr_number
                            ,ifnull(outstd_amount,0) - ifnull(rmt_amount, 0) outstd_amount
                            ,narration
                             FROM sms.out_standing a, sms.mst_acc_head b 
                             where sr_number =@sr_number
                             and ifnull(outstd_amount,0) - ifnull(rmt_amount,0) <> 0
							 and a.acc_id = b.acc_id
							 and month_no between 4 and 12
							  order by month_no,acc_id) one
							   union all
							  select * from (		SELECT 
                             serial
                            ,fin_id
                            ,dt_date
                            ,a.acc_id
							,concat(b.acc_name ,' ' ,ifnull(a.month_name,' ')) acc_name
                            ,a.month_no
                            ,sr_number
                            ,ifnull(outstd_amount,0) - ifnull(rmt_amount, 0) outstd_amount
                            ,narration
                             FROM sms.out_standing a, sms.mst_acc_head b 
                             where sr_number =@sr_number
                             and ifnull(outstd_amount,0) - ifnull(rmt_amount,0) <> 0
							 and a.acc_id = b.acc_id
							 and month_no between 1 and 3
							  order by month_no,acc_id) two";*/

            String query = @"select * from (SELECT 1 as Rank,
                             serial
                            , fin_id
                            , dt_date
                            , a.acc_id
                            , concat(b.acc_name, ' ', ifnull(a.month_name, ' ')) acc_name
                            , a.month_no
                            , sr_number
                            , ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) outstd_amount
                            , narration
                             FROM sms.out_standing a, sms.mst_acc_head b
                             where sr_number = @sr_number
                             and ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) <> 0

                             and a.acc_id = b.acc_id

                             and month_no between 4 and 12
							  ) one
                               union all

                              select* from (SELECT 2 as Rank,
                             serial
                            , fin_id
                            , dt_date
                            , a.acc_id
                            , concat(b.acc_name, ' ', ifnull(a.month_name, ' ')) acc_name
                            , a.month_no
                            , sr_number
                            , ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) outstd_amount
                            , narration
                             FROM sms.out_standing a, sms.mst_acc_head b
                             where sr_number = @sr_number
                             and ifnull(outstd_amount, 0) - ifnull(rmt_amount, 0) <> 0

                             and a.acc_id = b.acc_id

                             and month_no between 1 and 3
							 ) two order by Rank, month_no, acc_id";

            var result = con.Query<out_standing>(query, new { sr_number = sr_num });
            

            return result;
        }

      
        public IEnumerable<out_standing> AllOutStandingByReg(int reg)
        {
            String query = @"SELECT serial
                            ,fin_id
                            ,dt_date
                            ,a.acc_id
							,b.acc_name
                            ,sr_number
                            ,reg_num
                            ,ifnull(outstd_amount,0) - ifnull(rmt_amount, 0) outstd_amount
                            ,narration
                             FROM sms.out_standing a, sms.mst_acc_head b 
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
                string query = @"UPDATE sms.out_standing
                                SET sr_number = @sr_number
                                WHERE reg_num = @reg_num
                                and dt_date = @dt_date";

                con.Execute(query, std);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

       public int GetlatestFeesMonth(int sr_num)
        {

            try
            {
                string query = @"SELECT month(max(dt_date))      
                              FROM sms.out_standing
                              where sr_number = @sr_num";

                int mon = con.Query<int>(query, new {sr_num = sr_num }).SingleOrDefault();
                return mon;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public IEnumerable<out_standing> GetAdvanceMonth(int sr_num,int acc_id)
        {

            try
            {
                string query1 = @"SELECT fin_id
                               FROM sms.mst_fin
                          where fin_close = 'N'";

                string fin = con.Query<string>(query1).SingleOrDefault();
                
                string query = @"SELECT acc_id,
                                ifnull(month_no,0) month_no
                                  FROM sms.out_standing
                                  where sr_number = @sr_num
                                  and fin_id = @fin
                                  and acc_id = @acc_id
                                  and ifnull(month_no,0) <> 0";

                var mon = con.Query<out_standing>(query, new { sr_num = sr_num , fin = fin, acc_id = acc_id});
                return mon;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<out_standing> GetAdvanceTransportMonth(int sr_num, int acc_id)
        {

            try
            {
                string query1 = @"SELECT fin_id
                               FROM sms.mst_fin
                          where fin_close = 'N'";

                string fin = con.Query<string>(query1).SingleOrDefault();

                string query = @"SELECT acc_id,
                                ifnull(month_no,0) month_no
                                  FROM sms.out_standing
                                  where sr_number = @sr_num
                                  and fin_id = @fin
                                  and acc_id = @acc_id
                                  and ifnull(month_no,0) <> 0";

                var mon = con.Query<out_standing>(query, new { sr_num = sr_num, fin = fin, acc_id = acc_id });
                return mon;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public int GetlatestFeesYear(int sr_num)
        {

            try
            {
                string query = @"SELECT year(max(dt_date))      
                              FROM sms.out_standing
                              where sr_number = @sr_num";

                int mon = con.Query<int>(query, new { sr_num = @sr_num }).SingleOrDefault();
                return mon;
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
                //string query = @"UPDATE sms.out_standing
                //                SETreceipt_no = @receipt_no,
                //                   receipt_date = @receipt_date,
                //                   rmt_amount = ifnull(rmt_amount,0) + @rmt_amount,
                //                   clear_flag = @clear_flag,
                //                   month_no = @month_no
                //                WHERE fin_id = @fin_id
                //                and serial = @serial";

                string query = @"UPDATE sms.out_standing
                                SET receipt_no = @receipt_no,
                                   receipt_date = @receipt_date,
                                   rmt_amount = ifnull(rmt_amount,0) + @rmt_amount,
                                   clear_flag = @clear_flag
                                   WHERE fin_id = @fin_id
                                and serial = @serial";

                con.Execute(query, std);
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
                mst_fin fin = new mst_fin();

                string query1 = @"SELECT fin_id
                              ,fin_start_date
                              ,fin_end_date
                              ,fin_close
                          FROM sms.mst_fin
                          where fin_close = 'N'";

                fin = con.Query<mst_fin>(query1).SingleOrDefault();




                string maxid = "select ifnull(MAX(serial),0)+1 from sms.out_standing";

                int id = con.Query<int>(maxid).SingleOrDefault();



                string query = @"INSERT INTO sms.out_standing
           (serial
           ,fin_id
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
           )
     VALUES
           (@serial
           ,@fin_id
           ,@dt_date
           ,@acc_id
           ,@sr_number
           ,@outstd_amount
           ,@rmt_amount
           ,@narration
           ,@reg_num
            ,@clear_flag
            ,@MonthName
            ,@MonthNum)";

                std.serial = id;

                std.fin_id = fin.fin_id;

                std.dt_date = System.DateTime.Now.AddMinutes(750);
                string MonthName = std.dt_date.ToString("MMMM") + " " + std.dt_date.Year.ToString();
                int MonthNum = std.dt_date.Month;

                std.rmt_amount = 0;

               

                con.Execute(query,
                        new
                        {
                            std.serial
                           ,
                            std.fin_id
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
                        });




            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}