using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class fees_receiptMain
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddReceipt(List<fees_receipt> fees)
        {


            try
            {
                out_standingMain out_main = new out_standingMain();
                out_standing ot_std = new out_standing();

                       string query1 = @"SELECT [fin_id]
                             FROM [SMS].[dbo].[mst_fin]
                          where fin_close = 'N'";

                    string fin = con.Query<string>(query1).SingleOrDefault();

                   

                string maxid = "select isnull(MAX(receipt_no),0)+1 from fees_receipt where fin_id = @fin_id";

                int rect_no = con.Query<int>(maxid, new { fin_id = fin }).SingleOrDefault();

                DateTime rect_date = System.DateTime.Now;


                foreach (fees_receipt fee in fees)
                { 

                if (fee.fin_id == null)
                {
                   
                   fee.fin_id = fin;
                }

                if (fee.class_id == 0)
                {
                     query1 = @"SELECT std_section_id
                                        FROM [SMS].[dbo].[sr_register]
                                        where sr_number = @sr_number";

                    int id = con.Query<int>(query1,new {sr_number = fee.sr_number }).SingleOrDefault();

                    fee.section_id = id;

                     query1 = @"SELECT std_batch_id
                                        FROM [SMS].[dbo].[sr_register]
                                        where sr_number = @sr_number";

                    id = con.Query<int>(query1, new { sr_number = fee.sr_number }).SingleOrDefault();

                    fee.batch_id = id;

                    query1 = @"SELECT b.class_id
                                FROM [SMS].[dbo].[sr_register] a,[SMS].[dbo].[mst_batch] b
                                where sr_number = @sr_number
                                and
                                a.std_batch_id = b.batch_id";

                    id = con.Query<int>(query1, new { sr_number = fee.sr_number }).SingleOrDefault();

                    fee.class_id = id;

                }


                    fee.receipt_no = rect_no;

                    fee.receipt_date = rect_date;

                    /* if (fee.serial == 0)
                     {
                         string max = "select isnull(MAX(serial),0) from out_standing";

                         int id = con.Query<int>(max).SingleOrDefault();

                         fee.serial = id;

                         max = "select dt_date from out_standing where serial = (select isnull(MAX(serial),0) from out_standing)";

                         DateTime date = con.Query<DateTime>(max).SingleOrDefault();

                         fee.dt_date = date;
                     }
                     */

                    

                    fee.dt_date = rect_date;

                    if (fee.reg_date == DateTime.MinValue)
                    {
                        fee.reg_date = null;
                    }

                    if (fee.chq_date == DateTime.MinValue)
                    {
                        fee.chq_date = null;
                    }

                    string query = @"INSERT INTO [dbo].[fees_receipt]
                               ([fin_id]
                               ,[receipt_no]
                               ,[receipt_date]
                               ,[acc_id]
                               ,[fees_name]
                               ,[sr_number]
                               ,[class_id]
                               ,[section_id]
                               ,[batch_id]
                               ,[amount]
                               ,[reg_no]
                               ,[reg_date]
                               ,[dc_fine]
                               ,[dc_discount]
                               ,[narration]
                               ,[serial]
                               ,[dt_date]
                               ,[bnk_name]
                               ,[chq_no]
                               ,[chq_date]
                               ,[mode_flag]
                               ,[clear_flag])
                         VALUES
                               (@fin_id
                               ,@receipt_no
                               ,@receipt_date
                               ,@acc_id
                               ,@fees_name
                               ,@sr_number
                               ,@class_id
                               ,@section_id
                               ,@batch_id
                               ,@amount
                               ,@reg_no
                               ,@reg_date
                               ,@dc_fine
                               ,@dc_discount
                               ,@narration
                               ,@serial
                               ,@dt_date
                               ,@bnk_name
                               ,@chq_no
                               ,@chq_date
                               ,@mode_flag
                               ,@clear_flag)";
                
                con.Execute(query,
                        new
                        {
                            fee.fin_id,
                            fee.receipt_no,
                            fee.receipt_date,
                            fee.acc_id,
                            fee.fees_name,
                            fee.sr_number,
                            fee.class_id,
                            fee.section_id,
                            fee.batch_id,
                            fee.amount,
                            fee.reg_no,
                            fee.reg_date,
                            fee.dc_fine,
                            fee.dc_discount,
                            fee.narration,
                            fee.serial,
                            fee.dt_date,
                            fee.bnk_name,
                            fee.chq_no,
                            fee.chq_date,
                            fee.mode_flag,
                            fee.clear_flag
                        });

                    ot_std.serial = fee.serial;
                    ot_std.rmt_amount = fee.amount;
                    ot_std.receipt_no = fee.receipt_no;
                    ot_std.receipt_date = fee.receipt_date;
                    ot_std.fin_id = fee.fin_id;
                    ot_std.dt_date = fee.dt_date;
                    ot_std.clear_flag = fee.clear_flag;
                    ot_std.month_no = fee.month_no;

                    out_main.updateOutstandingReceipt(ot_std);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void updateReceipt(fees_receipt fees)
        {
            try
            {
                string query = @"UPDATE [dbo].[fees_receipt]
                               SET [sr_number] = @sr_number
                                  ,[class_id] = @class_id
                                  ,[section_id] = @section_id
                                  ,[batch_id] = @batch_id
                             WHERE 
		                            reg_no = @reg_no
		                            and
		                            reg_date = @reg_date";

                con.Execute(query, new {

                    sr_number = fees.sr_number,
                    class_id = fees.class_id,
                    section_id = fees.section_id,
                    batch_id = fees.batch_id,
                    reg_no = fees.reg_no,
                    reg_date = fees.reg_date

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public IEnumerable<fees_receipt> AllPaidFees(int sr_num)
        {
            String query = @"SELECT [fin_id]
                              ,[receipt_no]
                              ,[receipt_date]
                              ,[acc_id]
                              ,[fees_name]
                              ,[sr_number]
                              ,[class_id]
                              ,[section_id]
                              ,[batch_id]
                              ,[amount]
                              ,[reg_no]
                              ,[reg_date]
                              ,[dc_fine]
                              ,[dc_discount]
                              ,[narration]
                              ,[mode_flag]
                              ,case mode_flag when 'Cash' then 'Cleared' else case  when chq_reject is NULL then 'Not Cleared' else chq_reject end end as chq_reject
                          FROM [SMS].[dbo].[fees_receipt]
                                where sr_number = @sr_number
                                order by receipt_date desc";

            var result = con.Query<fees_receipt>(query, new { sr_number = sr_num});

            return result;
        }

    }
}