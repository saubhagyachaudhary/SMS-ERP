﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using SMS.report;
using System.Web.Routing;
using System.Web.Mvc;
using System.Threading.Tasks;
using SMS.Hubs;

namespace SMS.Models
{
    public class fees_receiptMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        public async Task<int> AddReceipt(List<fees_receipt> fees)
        {
            string phone;
            int sr_num =0;
            decimal amount = 0;
            string flag = "";

            try
            {
                out_standingMain out_main = new out_standingMain();
                out_standing ot_std = new out_standing();


                mst_finMain fin = new mst_finMain();

                if (fin.checkFYnotExpired())
                {

                    string fin_id = fin.FindActiveFinId();
                    string query1 = "";
                    string maxid = "select ifnull(MAX(receipt_no),0)+1 from fees_receipt where fin_id = @fin_id";

                    int rect_no = con.Query<int>(maxid, new { fin_id = fin_id }).SingleOrDefault();

                    DateTime rect_date = System.DateTime.Now.AddMinutes(dateTimeOffSet);


                    foreach (fees_receipt fee in fees)
                    {

                        if (fee.fin_id == null)
                        {

                            fee.fin_id = fin_id;
                        }

                        if (fee.class_id == 0)
                        {
                            query1 = @"SELECT 
                                            section_id
                                        FROM
                                            mst_std_section 
                                        WHERE
                                            sr_num = @sr_number 
                                        AND session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_active = 'Y'
                                                        AND session_finalize = 'Y')";

                            int id = con.Query<int>(query1, new { sr_number = fee.sr_number }).SingleOrDefault();

                            fee.section_id = id;
                            
                            query1 = @"SELECT 
                                            class_id
                                        FROM
                                            mst_std_class
                                        WHERE
                                            sr_num = @sr_number
                                        AND session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_active = 'Y'
                                                        AND session_finalize = 'Y')";

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

                        string query = @"INSERT INTO fees_receipt
                               (fin_id
                                ,session
                               ,receipt_no
                               ,receipt_date
                               ,acc_id
                               ,fees_name
                               ,sr_number
                               ,class_id
                               ,section_id
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
                                ,@session
                               ,@receipt_no
                               ,@receipt_date
                               ,@acc_id
                               ,@fees_name
                               ,@sr_number
                               ,@class_id
                               ,@section_id
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

                        await con.ExecuteAsync(query,
                                new
                                {
                                    fee.fin_id,
                                    fee.session,
                                    fee.receipt_no,
                                    fee.receipt_date,
                                    fee.acc_id,
                                    fee.fees_name,
                                    fee.sr_number,
                                    fee.class_id,
                                    fee.section_id,
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
                        ot_std.session = fee.session;
                        ot_std.dt_date = fee.dt_date;
                        ot_std.clear_flag = fee.clear_flag;
                        ot_std.month_no = fee.month_no;
                        ot_std.dc_fine = fee.dc_fine;
                        ot_std.dc_discount = fee.dc_discount;

                        out_main.updateOutstandingReceipt(ot_std);

                        sr_num = fee.sr_number;

                        amount = amount + fee.amount;
                        amount = amount + fee.dc_fine;
                        amount = amount - fee.dc_discount;

                        flag = fee.mode_flag;

                    }

#if !DEBUG
                    query1 = @"select coalesce(std_contact, std_contact1, std_contact2) from sr_register where sr_number = @sr_number";

                    phone = con.Query<string>(query1, new { sr_number = sr_num }).SingleOrDefault();

                    if (phone != null && amount != 0)
                    {
                        query1 = @"SELECT concat(std_first_name,' ',std_last_name)
                                FROM sr_register
                                where sr_number = @sr_number";

                        string name = con.Query<string>(query1, new { sr_number = sr_num }).SingleOrDefault();

                        

                        if (flag == "Cash")
                        {
                            string dt = DateTime.Now.AddMinutes(dateTimeOffSet).ToString();

                            SMSMessage sms = new SMSMessage();

                            foreach (var item in sms.smsbody("cash_fees_submit"))
                            {
                                string body = item.Replace("#student_name#", name);

                                body = body.Replace("#current_date#", dt);

                                body = body.Replace("#fees_amount#", amount.ToString());

                                await sms.SendSMS(body, phone,true);
                            }


                            //string dt = DateTime.Now.AddMinutes(dateTimeOffSet).ToString();

                            //SMSMessage sms = new SMSMessage();

                            //string text = @"School fees of " + name + " INR " + amount + " is successfully submitted on " + dt + ". Thank you for your cooperation. Hariti Public School";
                            //sms.SendSMS(text, phone);

                            //SMSMessage sms1 = new SMSMessage();
                            //string text1 = name + @" की स्कूल फीस INR " + amount + " दिनांक " + dt + " को सफलतापूर्वक जमा हो चुकी है। आपके सहयोग के लिए धन्यवाद। Hariti Public School";
                            //sms1.SendSMS(text1, phone);
                        }
                        else
                        {
                            string dt = DateTime.Now.AddMinutes(dateTimeOffSet).ToString();

                            SMSMessage sms = new SMSMessage();

                            foreach (var item in sms.smsbody("bank_fees_submit"))
                            {
                                string body = item.Replace("#student_name#", name);

                                body = body.Replace("#current_date#", dt);

                                body = body.Replace("#fees_amount#", amount.ToString());

                                await sms.SendSMS(body, phone,true);
                            }


                            //string dt = DateTime.Now.AddMinutes(dateTimeOffSet).ToString();

                            //SMSMessage sms = new SMSMessage();

                            //string text = @"School fees of " + name + " INR " + amount + " is successfully submitted on " + dt + ". Subject to Bank Clearance. Thank you for your cooperation. Hariti Public School";
                            //sms.SendSMS(text, phone);

                            //SMSMessage sms1 = new SMSMessage();
                            //string text1 = name + @" की स्कूल फीस INR " + amount + " दिनांक " + dt + " को सफलतापूर्वक जमा हो चुकी है। राशि बैंक निकासी के अधीन है। आपके सहयोग के लिए धन्यवाद। Hariti Public School";
                            //sms1.SendSMS(text1, phone);

                        }



                    }

                    DashboardHub dash = new DashboardHub();

                    dash.DailyFeesUpdate();
#endif
                    return rect_no;
                }
                else
                {
                    throw new System.InvalidOperationException("Financial Year Expired");
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
                string query = @"UPDATE fees_receipt
                               SET sr_number = @sr_number
                                  ,class_id = @class_id
                                  ,section_id = @section_id
                             WHERE 
		                            reg_no = @reg_no
		                            and
		                            reg_date = @reg_date";

                con.Execute(query, new {

                    sr_number = fees.sr_number,
                    class_id = fees.class_id,
                    section_id = fees.section_id,
                    reg_no = fees.reg_no,
                    reg_date = fees.reg_date

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public IEnumerable<fees_receipt> AllPaidFees(int sr_num,string session)
        {
            mst_finMain fin = new mst_finMain();

            String query = @"SELECT session
                              ,receipt_no
                              ,receipt_date
                              ,acc_id
                              ,fees_name
                              ,sr_number
                              ,class_id
                              ,section_id
                              ,amount
                              ,reg_no
                              ,reg_date
                              ,dc_fine
                              ,dc_discount
                              ,narration
                              ,mode_flag
                              ,case mode_flag when 'Cash' then 'Cleared' else case  when chq_reject is NULL then 'Not Cleared' else chq_reject end end as chq_reject
                          FROM fees_receipt
                                where sr_number = @sr_number
                                and session = @session
                                order by receipt_date desc";

            var result = con.Query<fees_receipt>(query, new { sr_number = sr_num, session = session});

            return result;
        }

        public IEnumerable<fees_receipt> AllPaidFeesReg(int reg_no,string session)
        {

            string query1 = @"SELECT fin_id FROM mst_fin where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();


            String query = @"SELECT session
                              ,receipt_no
                              ,receipt_date
                              ,acc_id
                              ,fees_name
                              ,sr_number
                              ,class_id
                              ,section_id
                              ,amount
                              ,reg_no
                              ,reg_date
                              ,dc_fine
                              ,dc_discount
                              ,narration
                              ,mode_flag
                              ,case mode_flag when 'Cash' then 'Cleared' else case  when chq_reject is NULL then 'Not Cleared' else chq_reject end end as chq_reject
                          FROM fees_receipt
                                where reg_no = @reg_no
                                and fin_id = @fin_id
                                 AND session = @session";

           

            var result = con.Query<fees_receipt>(query, new { reg_no = reg_no, fin_id = fin_id, session = session});

            return result;
        }

    }
}