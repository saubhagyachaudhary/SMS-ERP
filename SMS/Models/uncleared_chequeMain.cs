using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace SMS.Models
{
    public class uncleared_chequeMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<uncleared_cheque> AllUnclearedChequeList()
        {


            string query = @"SELECT 
                                    bnk_name,
                                    chq_no,
                                    chq_date,
                                    (SUM(amount) + SUM(dc_fine)) - SUM(dc_discount) Amount
                                FROM
                                    fees_receipt
                                WHERE
                                    clear_flag = 0 AND chq_reject IS NULL
                                GROUP BY bnk_name , chq_date , chq_no";

            var result = con.Query<uncleared_cheque>(query);

            return result;
        }

        public async Task Updatefees_Bounce(uncleared_cheque unclear)
        {

            try
            {
                string query = @"UPDATE fees_receipt 
                                    SET 
                                        chq_reject = @chq_reject,
                                        nt_clear_reason = @narration,
                                        clear_flag = 1
                                    WHERE
                                        bnk_name = @bnk_name
                                            AND chq_date = DATE_FORMAT(@chq_date, '%Y-%m-%d')
                                            AND chq_no = @chq_no
                                            AND clear_flag = 0";

                con.Execute(query, unclear);

                // fees_receipt fee = new fees_receipt();

                query = @"SELECT DISTINCT
                                serial, session, amount, dc_fine, dc_discount
                            FROM
                                fees_receipt
                            WHERE
                                bnk_name = @bnk_name
                                    AND chq_date = DATE_FORMAT(@chq_date, '%Y-%m-%d')
                                    AND chq_no = @chq_no";

                var result = con.Query<uncleared_cheque>(query, new { bnk_name = unclear.bnk_name, chq_date = unclear.chq_date, chq_no = unclear.chq_no });



                foreach (uncleared_cheque val in result)
                {
                    query = @"UPDATE out_standing 
                                SET 
                                    rmt_amount = rmt_amount - @rmt_amount,
                                    dc_fine = dc_fine - @dc_fine,
                                    dc_discount = dc_discount - @dc_discount
                                WHERE
                                    serial = @serial AND session = @session";

                    con.Execute(query, new { rmt_amount = val.amount,dc_fine = val.dc_fine,dc_discount = val.dc_discount ,serial = val.serial,session = val.session });
                }

                if (unclear.bnk_charges != 0)
                {
                    query = @"SELECT DISTINCT
                                    sr_number, session, reg_no, class_id
                                FROM
                                    fees_receipt
                                WHERE
                                    bnk_name = @bnk_name
                                        AND chq_date = DATE_FORMAT(@chq_date, '%Y-%m-%d')
                                        AND chq_no = @chq_no";

                    result = con.Query<uncleared_cheque>(query, new { bnk_name = unclear.bnk_name, chq_date = unclear.chq_date, chq_no = unclear.chq_no });



                    out_standing std = new out_standing();
                    out_standingMain outstd = new out_standingMain();

                    std.acc_id = 3;
                    std.clear_flag = false;
                    std.outstd_amount = (unclear.bnk_charges) / result.Count();

                    foreach (uncleared_cheque val in result)
                    {

                        std.sr_number = val.sr_number;
                        std.reg_num = val.reg_no;
                        std.class_id = val.class_id;
                        std.session = val.session;
                        outstd.AddOutStanding(std);
                    }

                    if (unclear.chq_reject == "Bounce")
                    {
                        query = @"select coalesce(std_contact, std_contact1, std_contact2) from sr_register where sr_number = @sr_number";
                        string phone = con.Query<string>(query, new { sr_number = std.sr_number }).SingleOrDefault();


                        SMSMessage sms = new SMSMessage();

                        foreach (var item in sms.smsbody("cheque_bounce"))
                        {
                            string body = item.Replace("#cheque_number#", unclear.chq_no);

                            body = body.Replace("#bounce_charge#", unclear.bnk_charges.ToString());

                            await sms.SendSMS(body, phone,true);
                        }

                        // SMSMessage sms = new SMSMessage();

                        // string text = @"Your Cheque No "+ unclear.chq_no + " is unfortunately Bounce INR "+ unclear.bnk_charges + " will be charged against it. kindly make the payment as soon as possible. Thank You. Hariti Public School ";

                        //sms.SendSMS(text,phone);

                        // SMSMessage sms1 = new SMSMessage();

                        // string text1 = @"आपका चेक नंबर " + unclear.chq_no + " बाउंस हो गया है। जिसका बाउंस शुल्क " + unclear.bnk_charges + " देय होगा। कृपया जितनी जल्दी हो सके भुगतान करें।  धन्यवाद। Hariti Public School.";

                        // sms1.SendSMS(text1, phone);

                    }
                }

               
                
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Updatefees_cleared(uncleared_cheque unclear)
        {

            try
            {
                string query = @"UPDATE fees_receipt 
                                    SET 
                                        chq_reject = @chq_reject,
                                        clear_flag = 1,
                                        nt_clear_reason = @narration
                                    WHERE
                                        bnk_name = @bnk_name
                                            AND chq_date = DATE_FORMAT(@chq_date, '%Y-%m-%d')
                                            AND chq_no = @chq_no
                                            AND clear_flag = 0";

                con.Execute(query, unclear);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}