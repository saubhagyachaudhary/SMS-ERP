using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class uncleared_chequeMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<uncleared_cheque> AllUnclearedChequeList()
        {


            String query = @"select
                               bnk_name,
                               chq_no,
                               chq_date,
                               (SUM(amount) + SUM(dc_fine)) - SUM(dc_discount) Amount
                               from sms.fees_receipt
                               where
                               clear_flag = 0
							   and
							   chq_reject is null
                               group by bnk_name,chq_date,chq_no";

            var result = con.Query<uncleared_cheque>(query);

            return result;
        }

        public void Updatefees_Bounce(uncleared_cheque unclear)
        {

            try
            {
                String query = @"UPDATE sms.fees_receipt
                               SET chq_reject = @chq_reject,
                                    nt_clear_reason = @narration,
                                    clear_flag = 1
                             WHERE bnk_name = @bnk_name
                             and chq_date = date_format(@chq_date,'%Y-%m-%d')
                             and chq_no = @chq_no
                             and clear_flag = 0";

                con.Execute(query, unclear);

                // fees_receipt fee = new fees_receipt();

                query = @"select DISTINCT
                               serial,amount
							   from sms.fees_receipt
                               WHERE bnk_name = @bnk_name
                                and chq_date = date_format(@chq_date,'%Y-%m-%d')
                                and chq_no = @chq_no";

                var result = con.Query<uncleared_cheque>(query, new { bnk_name = unclear.bnk_name, chq_date = unclear.chq_date, chq_no = unclear.chq_no });



                foreach (uncleared_cheque val in result)
                {
                    query = @"update sms.out_standing
                            set rmt_amount = rmt_amount - @rmt_amount
                            where serial = @serial ";

                    con.Execute(query, new { rmt_amount = val.amount, serial = val.serial });
                }

                if (unclear.bnk_charges != 0)
                {
                    query = @"select
                               DISTINCT sr_number,reg_no
							   from sms.fees_receipt
                               WHERE bnk_name = @bnk_name
                                and chq_date = date_format(@chq_date,'%Y-%m-%d')
                                and chq_no = @chq_no";

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
                        outstd.AddOutStanding(std);
                    }

                    if (unclear.chq_reject == "Bounce")
                    {
                        string text = @"Your Cheque No "+ unclear.chq_no + " is unfortunately Bounce INR "+ unclear.bnk_charges + " will be charged against it. kindly make the payment as soon as possible. Thank You. Hariti Public School ";
                        SMSMessage sms = new SMSMessage();

                        query = @"select coalesce(std_contact, std_contact1, std_contact2) from sms.sr_register where sr_number = @sr_number";


                        string phone = con.Query<string>(query, new { sr_number = std.sr_number }).SingleOrDefault();


                       sms.SendSMS(text,phone);

                        text = @"आपका चेक नंबर " + unclear.chq_no + " बाउंस हो गया है। जिसका बाउंस शुल्क " + unclear.bnk_charges + " देय होगा। कृपया जितनी जल्दी हो सके भुगतान करें।  धन्यवाद। Hariti Public School.";

                        sms.SendSMS(text, phone);

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
                String query = @"UPDATE sms.fees_receipt
                               SET chq_reject = @chq_reject,clear_flag = 1, nt_clear_reason = @narration
                             WHERE bnk_name = @bnk_name
                             and chq_date = date_format(@chq_date,'%Y-%m-%d')
                             and chq_no = @chq_no
                             and clear_flag = 0";

                con.Execute(query, unclear);


            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}