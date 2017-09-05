using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class out_standingMain
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<out_standing> OutStandingByReg(int reg)
        {
            String query = @"SELECT [serial]
                              ,[fin_id]
                              ,[dt_date]
                              ,[acc_id]
                              ,[sr_number]
                              ,[outstd_amount]
                              ,[rmt_amount]
                              ,[narration]
                              ,[reg_num]
                          FROM [SMS].[dbo].[out_standing]
                          where reg_num = @reg_num";

            var result = con.Query<out_standing>(query, new { reg_num = reg });

            return result;
        }

        public IEnumerable<out_standing> AllOutStanding(int sr_num)
        {
            String query = @"		select * from (		SELECT top 1000
                             [serial]
                            ,[fin_id]
                            ,[dt_date]
                            ,a.[acc_id]
							,b.acc_name +' ' +isnull(a.month_name,' ') acc_name
                            ,[sr_number]
                            ,ISNULL(outstd_amount,0) - ISNULL(rmt_amount, 0) outstd_amount
                            ,[narration]
                             FROM[SMS].[dbo].[out_standing] a, [SMS].[dbo].[mst_acc_head] b 
                             where sr_number =@sr_number
                             and ISNULL(outstd_amount,0) - ISNULL(rmt_amount,0) <> 0
							 and a.acc_id = b.acc_id
							 and month_no between 4 and 12
							  order by month_no,acc_id) one
							   union all
							  select * from (		SELECT top 1000
                             [serial]
                            ,[fin_id]
                            ,[dt_date]
                            ,a.[acc_id]
							,b.acc_name +' ' +isnull(a.month_name,' ') acc_name
                            ,[sr_number]
                            ,ISNULL(outstd_amount,0) - ISNULL(rmt_amount, 0) outstd_amount
                            ,[narration]
                             FROM[SMS].[dbo].[out_standing] a, [SMS].[dbo].[mst_acc_head] b 
                             where sr_number =@sr_number
                             and ISNULL(outstd_amount,0) - ISNULL(rmt_amount,0) <> 0
							 and a.acc_id = b.acc_id
							 and month_no between 1 and 3
							  order by month_no,acc_id) two";

            var result = con.Query<out_standing>(query, new { sr_number = sr_num });
            

            return result;
        }

        //public IEnumerable<out_standing> MonthlyFees(int sr_num)
        //{
        //    String query = @"SELECT fees_amount outstd_amount,acc_name,a.acc_id
        //                      FROM [SMS].[dbo].[sr_register] b,[SMS].[dbo].[mst_fees] d, [SMS].[dbo].[mst_batch] e,[SMS].[dbo].[mst_acc_head] a 
        //                      where
        //                      b.std_batch_id = e.batch_id
        //                      and
        //                      e.class_id = d.class_id
        //                      and 
        //                      d.period = 'M'
        //                      and
        //                      a.acc_id = d.acc_id
        //                      and 
        //                      b.sr_number = @sr_number";

        //    var result = con.Query<out_standing>(query, new { sr_number = sr_num });

        //    query = @"SELECT DATEDIFF(Month,fin_start_date,GETDATE())+1
        //              FROM [SMS].[dbo].[mst_fin]
	       //           where fin_close = 'N'";

        //   int months = con.Query<int>(query).SingleOrDefault();

        //    List<out_standing> payment = new List<out_standing>();
        //    /*
        //    for (int i = 1; i <= months; i++)
        //    {
        //        if (i + 3 <= 12)
        //        {
        //            payment.Add(new out_standing { acc_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i+3) +" " + result.acc_name, outstd_amount = result.outstd_amount });
        //        }
        //        else
        //        {
        //            int j = (i + 3) - 12;
        //            payment.Add(new out_standing { acc_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(j) +" "+ result.acc_name, outstd_amount = result.outstd_amount });
        //        }
        //    }
        //    */
        //    foreach (out_standing da in result)
        //    {
        //        payment.Add(new out_standing {acc_id=da.acc_id, acc_name = da.acc_name, outstd_amount = da.outstd_amount * months });
        //    }
        //    return payment;
        //}

        public IEnumerable<out_standing> AllOutStandingByReg(int reg)
        {
            String query = @"SELECT [serial]
                            ,[fin_id]
                            ,[dt_date]
                            ,a.[acc_id]
							,b.acc_name
                            ,[sr_number]
                            ,[reg_num]
                            ,ISNULL(outstd_amount,0) - ISNULL(rmt_amount, 0) outstd_amount
                            ,[narration]
                             FROM[SMS].[dbo].[out_standing] a, [SMS].[dbo].[mst_acc_head] b 
                             where reg_num = @reg_num
                             and ISNULL(outstd_amount,0) - ISNULL(rmt_amount,0) <> 0
							 and a.acc_id = b.acc_id";

            var result = con.Query<out_standing>(query, new { reg_num = reg });


            return result;
        }

        public void updateOutstanding(out_standing std)
        {

            try
            {
                string query = @"UPDATE [dbo].[out_standing]
                                SET[sr_number] = @sr_number
                                WHERE [reg_num] = @reg_num
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
                string query = @"SELECT DATEPART(m, max([dt_date]))      
                              FROM [SMS].[dbo].[out_standing]
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
                string query1 = @"SELECT [fin_id]
                               FROM [SMS].[dbo].[mst_fin]
                          where fin_close = 'N'";

                string fin = con.Query<string>(query1).SingleOrDefault();
                
                string query = @"SELECT [acc_id],
                                isnull([month_no],0) [month_no]
                                  FROM [SMS].[dbo].[out_standing]
                                  where sr_number = @sr_num
                                  and fin_id = @fin
                                  and acc_id = @acc_id
                                  and isnull([month_no],0) <> 0";

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
                string query1 = @"SELECT [fin_id]
                               FROM [SMS].[dbo].[mst_fin]
                          where fin_close = 'N'";

                string fin = con.Query<string>(query1).SingleOrDefault();

                string query = @"SELECT [acc_id],
                                isnull([month_no],0) [month_no]
                                  FROM [SMS].[dbo].[out_standing]
                                  where sr_number = @sr_num
                                  and fin_id = @fin
                                  and acc_id = @acc_id
                                  and isnull([month_no],0) <> 0";

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
                string query = @"SELECT DATEPART(YYYY, max([dt_date]))      
                              FROM [SMS].[dbo].[out_standing]
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
                //string query = @"UPDATE [dbo].[out_standing]
                //                SET[receipt_no] = @receipt_no,
                //                   [receipt_date] = @receipt_date,
                //                   [rmt_amount] = isnull([rmt_amount],0) + @rmt_amount,
                //                   [clear_flag] = @clear_flag,
                //                   [month_no] = @month_no
                //                WHERE fin_id = @fin_id
                //                and serial = @serial";

                string query = @"UPDATE [dbo].[out_standing]
                                SET[receipt_no] = @receipt_no,
                                   [receipt_date] = @receipt_date,
                                   [rmt_amount] = isnull([rmt_amount],0) + @rmt_amount,
                                   [clear_flag] = @clear_flag
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

                string query1 = @"SELECT [fin_id]
                              ,[fin_start_date]
                              ,[fin_end_date]
                              ,[fin_close]
                          FROM [SMS].[dbo].[mst_fin]
                          where fin_close = 'N'";

                fin = con.Query<mst_fin>(query1).SingleOrDefault();




                string maxid = "select isnull(MAX(serial),0)+1 from out_standing";

                int id = con.Query<int>(maxid).SingleOrDefault();



                string query = @"INSERT INTO [dbo].[out_standing]
           ([serial]
           ,[fin_id]
           ,[dt_date]
           ,[acc_id]
           ,[sr_number]
           ,[outstd_amount]
           ,[rmt_amount]
           ,[narration]
           ,[reg_num]
            ,[clear_flag]
           ,[month_name]
           ,[month_no]
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
           ,DATENAME(month, GETDATE()) + ' ' +convert(varchar,DATEPART(YYYY,GETDATE()))
           ,DATEPART(M,getdate()))";


                std.serial = id;

                std.fin_id = fin.fin_id;

                std.dt_date = System.DateTime.Now;

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
                        });




            }

            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}