using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;

namespace SMS.Models
{
    public class fees_receiptMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddReceipt(List<fees_receipt> fees)
        {
            string phone;
            int sr_num =0;
            decimal amount = 0;
            string flag = "";

            try
            {
                out_standingMain out_main = new out_standingMain();
                out_standing ot_std = new out_standing();

                       string query1 = @"SELECT fin_id
                             FROM sms.mst_fin
                          where fin_close = 'N'";

                    string fin = con.Query<string>(query1).SingleOrDefault();

                   

                string maxid = "select ifnull(MAX(receipt_no),0)+1 from sms.fees_receipt where fin_id = @fin_id";

                int rect_no = con.Query<int>(maxid, new { fin_id = fin }).SingleOrDefault();

                DateTime rect_date = System.DateTime.Now.AddMinutes(750);


                foreach (fees_receipt fee in fees)
                { 

                if (fee.fin_id == null)
                {
                   
                   fee.fin_id = fin;
                }

                if (fee.class_id == 0)
                {
                     query1 = @"SELECT std_section_id
                                        FROM sms.sr_register
                                        where sr_number = @sr_number";

                    int id = con.Query<int>(query1,new {sr_number = fee.sr_number }).SingleOrDefault();

                    fee.section_id = id;

                     query1 = @"SELECT std_batch_id
                                        FROM sms.sr_register
                                        where sr_number = @sr_number";

                    id = con.Query<int>(query1, new { sr_number = fee.sr_number }).SingleOrDefault();

                    fee.batch_id = id;

                    query1 = @"SELECT b.class_id
                                FROM sms.sr_register a,sms.mst_batch b
                                where sr_number = @sr_number
                                and
                                a.std_batch_id = b.batch_id";

                    id = con.Query<int>(query1, new { sr_number = fee.sr_number }).SingleOrDefault();

                       
                        fee.class_id = id;

                }


                    fee.receipt_no = rect_no;

                    fee.receipt_date = rect_date;
                    

                    fee.dt_date = rect_date;

                    if (fee.reg_date == DateTime.MinValue)
                    {
                        fee.reg_date = null;
                    }

                    if (fee.chq_date == DateTime.MinValue)
                    {
                        fee.chq_date = null;
                    }

                    string query = @"INSERT INTO sms.fees_receipt
                               (fin_id
                               ,receipt_no
                               ,receipt_date
                               ,acc_id
                               ,fees_name
                               ,sr_number
                               ,class_id
                               ,section_id
                               ,batch_id
                               ,amount
                               ,reg_no
                               ,reg_date
                               ,dc_fine
                               ,dc_discount
                               ,narration
                               ,serial
                               ,dt_date
                               ,bnk_name
                               ,chq_no
                               ,chq_date
                               ,mode_flag
                               ,clear_flag
                                ,user_id)
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
                               ,@clear_flag
                                ,@user_id)";
                
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
                            fee.clear_flag,
                            fee.user_id
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

                    sr_num = fee.sr_number;

                    amount = amount + fee.amount;
                    amount = amount + fee.dc_fine;
                    amount = amount - fee.dc_discount;

                    flag = fee.mode_flag;

                }

                query1 = @"select coalesce(std_contact, std_contact1, std_contact2) from sms.sr_register where sr_number = @sr_number";

                phone = con.Query<string>(query1, new { sr_number = sr_num }).SingleOrDefault();

                query1 = @"SELECT concat(std_first_name,' ',std_last_name)
                                FROM sms.sr_register
                                where sr_number = @sr_number";

                string name = con.Query<string>(query1, new { sr_number = sr_num }).SingleOrDefault();

                SMSMessage sms = new SMSMessage();

                //string text = @"Hariti Public School Thank you for you cooperation. Amount INR "+amount+ " Successfully Submitted on "+ DateTime.Now.AddMinutes(750).ToString();
                if (flag == "Cash")
                {
                    string dt = DateTime.Now.AddMinutes(750).ToString();
                    string text = @"School fees of " + name + " INR " + amount + " is successfully submitted on " + dt + ". Thank you for your cooperation. Hariti Public School";
                    sms.SendSMS(text, phone);
                    text = name + @" की स्कूल फीस INR " + amount + " दिनांक "+dt+" को सफलतापूर्वक जमा हो चुकी है। आपके सहयोग के लिए धन्यवाद। Hariti Public School";
                    sms.SendSMS(text, phone);
                }
                else
                {
                    string dt = DateTime.Now.AddMinutes(750).ToString();
                    string text = @"School fees of " + name + " INR " + amount + " is successfully submitted on " + dt + ". Subject to Bank Clearance. Thank you for your cooperation. Hariti Public School";
                    sms.SendSMS(text, phone);

                    text = name + @" की स्कूल फीस INR " + amount + " दिनांक " + dt + " को सफलतापूर्वक जमा हो चुकी है। राशि बैंक निकासी के अधीन है। आपके सहयोग के लिए धन्यवाद। Hariti Public School";
                    sms.SendSMS(text, phone);

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
                string query = @"UPDATE sms.fees_receipt
                               SET sr_number = @sr_number
                                  ,class_id = @class_id
                                  ,section_id = @section_id
                                  ,batch_id = @batch_id
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
            String query = @"SELECT fin_id
                              ,receipt_no
                              ,receipt_date
                              ,acc_id
                              ,fees_name
                              ,sr_number
                              ,class_id
                              ,section_id
                              ,batch_id
                              ,amount
                              ,reg_no
                              ,reg_date
                              ,dc_fine
                              ,dc_discount
                              ,narration
                              ,mode_flag
                              ,case mode_flag when 'Cash' then 'Cleared' else case  when chq_reject is NULL then 'Not Cleared' else chq_reject end end as chq_reject
                          FROM sms.fees_receipt
                                where sr_number = @sr_number
                                order by receipt_date desc";

            var result = con.Query<fees_receipt>(query, new { sr_number = sr_num});

            return result;
        }

        public IEnumerable<fees_receipt> AllPaidFeesReg(int reg_no)
        {

            string query1 = @"SELECT fin_id FROM sms.mst_fin where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();


            String query = @"SELECT fin_id
                              ,receipt_no
                              ,receipt_date
                              ,acc_id
                              ,fees_name
                              ,sr_number
                              ,class_id
                              ,section_id
                              ,batch_id
                              ,amount
                              ,reg_no
                              ,reg_date
                              ,dc_fine
                              ,dc_discount
                              ,narration
                              ,mode_flag
                              ,case mode_flag when 'Cash' then 'Cleared' else case  when chq_reject is NULL then 'Not Cleared' else chq_reject end end as chq_reject
                          FROM sms.fees_receipt
                                where reg_no = @reg_no
                                and fin_id = @fin_id";

           

            var result = con.Query<fees_receipt>(query, new { reg_no = reg_no, fin_id = fin_id});

            return result;
        }

    }
}