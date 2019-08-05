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
using SMS.report;
using System.Web.Routing;
using System.Web.Mvc;
using System.Threading.Tasks;
using SMS.Hubs;
using System.Net.Mime;
using System.Net.Mail;

namespace SMS.Models
{
    public class fees_receiptMain
    {
        
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        string donotreplyMail = ConfigurationManager.AppSettings["donotreplyMail"].ToString();
        string donotreplyMailPassword = ConfigurationManager.AppSettings["donotreplyMailPassword"].ToString();

        public int AddReceipt(List<fees_receipt> fees)
        {
            string phone;
            int sr_num =0;
            decimal amount = 0;
            string flag = "";

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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

                        con.Open();

                        MySqlCommand myCommand = con.CreateCommand();

                        MySqlTransaction myTrans;

                        myTrans = con.BeginTransaction();

                        myCommand.Connection = con;

                        myCommand.Transaction = myTrans;

                        string sess="";

                        decimal total_amount = 0;

                        try
                        {
                            foreach (fees_receipt fee in fees)
                            {
                                total_amount = total_amount + fee.amount;
                            }

                             foreach (fees_receipt fee in fees)
                            {

                                query1 = @"SELECT 
                                        IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)
                                    FROM
                                        out_standing
                                    WHERE
                                        serial = @serial AND session = @session";

                                decimal dues = con.Query<decimal>(query1, new { serial = fee.serial, session = fee.session }).SingleOrDefault();

                                if (dues != fee.due_amount)
                                {
                                    rect_no = 0;
                                    myTrans.Rollback();
                                    throw new System.InvalidOperationException("trying to update double entry in a single out_standing serial");
                                }

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
                                        AND session = @session";

                                    int id = con.Query<int>(query1, new { sr_number = fee.sr_number, session = fee.session }).SingleOrDefault();

                                    fee.section_id = id;

                                    query1 = @"SELECT 
                                            class_id
                                        FROM
                                            mst_std_class
                                        WHERE
                                            sr_num = @sr_number
                                        AND session = @session";

                                    id = con.Query<int>(query1, new { sr_number = fee.sr_number, session = fee.session }).SingleOrDefault();


                                    fee.class_id = id;

                                }


                                fee.receipt_no = rect_no;

                                fee.receipt_date = rect_date;


                                fee.dt_date = rect_date;

                                string reg_date;
                                string chq_date;

                                if (fee.reg_date == DateTime.MinValue)
                                {
                                    reg_date = "null";
                                }
                                else
                                {
                                    reg_date = String.Format("'{0}'", fee.reg_date.Value.ToString("yyyy-MM-dd"));
                                }

                                if (fee.chq_date == DateTime.MinValue)
                                {
                                    chq_date = "null";
                                }
                                else
                                {
                                    chq_date = String.Format("'{0}'", fee.chq_date.Value.ToString("yyyy-MM-dd"));
                                }



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

                                out_main.updateOutstandingReceipt(ot_std, myCommand);

                                string query = String.Format(@"INSERT INTO fees_receipt
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
                                ,user_id
                                ,secret_code)
                         VALUES
                               ('{0}'
                                ,'{1}'
                               ,{2}
                               ,'{3}'
                               ,{4}
                               ,'{5}'
                               ,{6}
                               ,{7}
                               ,{8}
                               ,{9}
                               ,{10}
                               ,{11}
                               ,{12}
                               ,{13}
                               ,'{14}'
                               ,{15}
                               ,'{16}'
                               ,'{17}'
                               ,'{18}'
                               ,{19}
                               ,'{20}'
                               ,{21}
                                ,{22},reverse(concat(unix_timestamp(),'-',base64_encode({2}),'-',base64_encode({6}),'-',base64_encode({23}))));", fee.fin_id,
                                            fee.session,
                                            fee.receipt_no,
                                            fee.receipt_date.ToString("yyyy-MM-dd"),
                                            fee.acc_id,
                                            fee.fees_name,
                                            fee.sr_number,
                                            fee.class_id,
                                            fee.section_id,
                                            fee.amount,
                                            fee.reg_no,
                                            reg_date,
                                            fee.dc_fine,
                                            fee.dc_discount,
                                            fee.narration,
                                            fee.serial,
                                            fee.dt_date.ToString("yyyy-MM-dd"),
                                            fee.bnk_name,
                                            fee.chq_no,
                                            chq_date,
                                            fee.mode_flag,
                                            fee.clear_flag,
                                            fee.user_id,
                                            total_amount);


                                myCommand.CommandText = query;

                                myCommand.ExecuteNonQuery();



                                sr_num = fee.sr_number;
                                sess = fee.session;

                                amount = amount + fee.amount;
                                amount = amount + fee.dc_fine;
                                amount = amount - fee.dc_discount;

                                flag = fee.mode_flag;


                            }
                            myTrans.Commit();

                            query1 = @"SELECT 
                                            std_email
                                        FROM
                                            sr_register
                                        WHERE
                                            sr_number = @sr_number";

                            string email_id = con.Query<string>(query1, new { sr_number = sr_num }).SingleOrDefault();

                            if (email_id != null && email_id.Trim() !="")
                            {
                                query1 = @"SELECT 
                                    sr_number num,
                                    CONCAT(ifnull(std_first_name,''), ' ',ifnull(std_last_name,'')) name,
                                    std_father_name father_name,
                                    CONCAT(c.class_name, ' Sec. ', b.section_name) class_name
                                FROM
                                    sr_register a,
                                    mst_section b,
                                    mst_class c,
                                    mst_std_section d,
                                    mst_std_class e
                                WHERE
                                    d.section_id = b.section_id
                                        AND b.class_id = c.class_id
                                        AND e.class_id = b.class_id
                                        AND a.sr_number = d.sr_num
                                        AND d.sr_num = e.sr_num
                                        AND a.sr_number = @sr_number
                                        AND b.session = c.session
                                        AND c.session = d.session
                                        AND d.session = e.session
                                        AND e.session = @session";

                                var rep = con.Query<rep_fees>(query1, new { sr_number = sr_num, session = sess }).SingleOrDefault();

                                query1 = @"SELECT 
                                        mode_flag,
                                        chq_no,
                                        chq_date,
                                        receipt_date,
                                        fees_name,
                                        sr_number,
                                        class_id,
                                        amount fees,
                                        dc_fine fine,
                                        dc_discount discount,
                                        amount + dc_fine - dc_discount amount,
                                        reg_no,
                                        reg_date,
                                        session
                                    FROM
                                        fees_receipt
                                    WHERE
                                        fin_id = (SELECT 
                                                fin_id
                                            FROM
                                                mst_fin
                                            WHERE
                                                fin_close = 'N')
                                            AND receipt_no = @receipt_no";


                                IEnumerable<fees_receipt> result = con.Query<fees_receipt>(query1, new { receipt_no = rect_no });

                                string fees_Table = "";

                                decimal total = 0;

                                foreach (var fee in result)
                                {
                                    fees_Table = fees_Table + @" <tr>
                                <td>
                                <div style='border-radius:2px;font-size:12px;color:#999;float:left;width:100%;border:solid 1px #f4f4f4'>
                                <ul style='list-style:none;width:96%;float:left;border-bottom:1px solid #e2e2e2;padding:2%;margin:0'>
                                <li style='float:left;width:40%;text-align:center;margin-left: 0px;'>
                                    " + fee.fees_name + @"
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    " + fee.fees + @"
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    " + fee.fine + @"
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    " + fee.discount + @"
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    " + fee.amount + @"
                                </li>

                            </ul>
                        </div>
                    </td>
                </tr>";

                                    total = total + fee.amount;
                                }

                                string Affiliation = ConfigurationManager.AppSettings["Affiliation"].ToString();
                                string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();


                                string body = @"<div style='width:584px;margin:0 auto;border:#ececec solid 1px'>
    <div style='padding:22px 34px 15px 34px'>
        <div style='float:left'><img title='Institute Logo' src='" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + @"/images/logo.jpg' alt='Institute Logo' width='100px' class='CToWUd'></div>
        <div style='float:right; font:bold 30px Arial,Helvetica,sans-serif;margin-top:20px'>" + SchoolName + @"</div>
        <div style='float:right; font:12px Arial,Helvetica,sans-serif;margin-top:5px'>(" + Affiliation + @")</div>
    </div>
    <div style='clear:both'></div>
    <div style='float:left;color:#333333;font:normal 14px Arial,Helvetica,sans-serif;width:250px'>
        
        <!--<div>Order no: #8846417200 <br>  2019-07-27T07:13:42.000Z </div>-->
        <div style='padding-top:10px'></div>
    </div>
    
    <div style='clear:both'> </div>
</div>
<div style='width:584px;background-color:#ffffff;border:#e8e7e7 solid 1px;padding:27px 0;margin:0 auto;border-bottom:0'>
    <div style='border-bottom:#717171 dotted 1px;font:normal 14px Arial,Helvetica,sans-serif;color:#666666;padding:0px 33px 10px'>
        
        <table style='width:100%' border='0' cellspacing='0' cellpadding='2'>
            <tbody>
                <tr>
                    <td width='450px'>Receipt No:</td>
                    <td width='450px'>" + rect_no + @"</td>
                    <td width='450px'>Receipt Date:</td>
                    <td width='450px'>" + result.First().receipt_date.ToString("dd-MMM-yyyy") + @"</td>
                </tr>
                <tr>
                    <td width='450px'>Admission No:</td>
                    <td width='450px'>" + rep.num + @"</td>
                    <td width='450px'>Class:</td>
                    <td width='450px'>" + rep.class_name + @"</td>
                </tr>
                <tr>
                    <td width='450px'>Name:</td>
                    <td width='450px'>" + rep.name + @"</td>
                    
                </tr>
                <tr>
                    <td width='450px'>Father's Name: </td>
                    <td width='450px'>" + rep.father_name + @"</td>
                   
                </tr>
            </tbody>
        </table>
    </div>
    <div style='border-bottom:#717171 dotted 1px;font:normal 14px Arial,Helvetica,sans-serif;color:#666666;padding:10px 33px 10px'>
        <br>
        <table style='width:100%' border='0' cellspacing='0' cellpadding='2'>
            <tbody>

                <tr>
                    <td>
                        <div style='border-radius:2px;font-size:12px;color:#999;float:left;width:100%;border:solid 1px #f4f4f4'>

                            <ul style='list-style:none;width:96%;float:left;border-bottom:1px solid #e2e2e2;padding:2%;margin:0'>
                                <li style='float:left;width:40%;text-align:center;margin-left: 0px;'>
                                    Particulars
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    Fees
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    Fine
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    Discount
                                </li>
                                <li style='float:left;width:15%;text-align:center;margin-left: 0px;'>
                                    Paid
                                </li>
                            </ul>
                        </div>
                    </td>
                </tr>
                            " + fees_Table + @"
            </tbody>
        </table>

        <div style='border-bottom:#717171 dotted 1px;font:600 14px Arial,Helvetica,sans-serif;color:#333333;padding:17px 33px 17px'>
            <table style='width:100%' border='0' cellspacing='0' cellpadding='2'>
                <tbody>
                    <tr>
                        <td width='450px'>Amount Paid</td>
                        <td width='313px' style='text-align:right'>Rs." + total + @"</td>
                    </tr>
                </tbody>
            </table>
        </div>
    </div>
    <div style='border-bottom:#717171 dotted 1px;font:normal 14px Arial,Helvetica,sans-serif;color:#666666;padding:10px 33px 10px'>
        <br>
        <table style='width:100%' border='0' cellspacing='0' cellpadding='2'>
            <tbody>
                <tr>
                    <td colspan='2'>
                        Received with thanks:
                    </td>
                    <td colspan='2'>
                        " + repFees_receipt.NumbersToWords(Decimal.ToInt32(total)) + @"
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</div>
<div style='margin:0 auto;width:594px'><img title='' src='" + HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + @"/images/fee_receipt.png' alt='' class='CToWUd'></div>";

                                if(total != 0)
                                    _sendMail(email_id,rect_no.ToString(), result.First().receipt_date.ToString("dd-MMM-yyyy"), body);

                            }
                        }
                        catch (Exception e)
                        {
                            try
                            {
                                rect_no = 0;
                                myTrans.Rollback();

                            }
                            catch (MySqlException ex)
                            {
                                if (myTrans.Connection != null)
                                {
                                    Console.WriteLine("An exception of type " + ex.GetType() +
                                    " was encountered while attempting to roll back the transaction.");
                                }
                            }

                            Console.WriteLine("An exception of type " + e.GetType() +
                            " was encountered while inserting the data.");
                            Console.WriteLine("Neither record was written to database.");
                        }
                        finally
                        {
                            con.Close();
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

                                sms.SendSMS(body, phone,true);
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

                                sms.SendSMS(body, phone,true);
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void _sendMail(string mail_id, string receipt_no, string receipt_date, string body)
        {
            string FromMail = donotreplyMail;
            string ToMail = mail_id;
            string Subject = "Thanks For The Payment. Your Receipt No: " + receipt_no; //"Attendance Sheet of class " + class_name + " date " + DateTime.Now.Date.ToString("dd/MM/yyyy");
            string Body = body;

            if (ToMail != null)
            {
               
                    using (MailMessage mm = new MailMessage(
                      FromMail, ToMail, Subject, Body))
                    {
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        NetworkCredential networkCredential = new NetworkCredential(FromMail, donotreplyMailPassword);
                        smtp.Credentials = networkCredential;
                        smtp.EnableSsl = true;
                        smtp.Host = "smtp.gmail.com";
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                
            }
        }


        public void updateReceipt(fees_receipt fees)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"UPDATE fees_receipt
                               SET sr_number = @sr_number
                                  ,class_id = @class_id
                                  ,section_id = @section_id
                             WHERE 
		                            reg_no = @reg_no
		                            and
		                            reg_date = @reg_date";

                    con.Execute(query, new
                    {

                        sr_number = fees.sr_number,
                        class_id = fees.class_id,
                        section_id = fees.section_id,
                        reg_no = fees.reg_no,
                        reg_date = fees.reg_date

                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        

        public IEnumerable<fees_receipt> AllPaidFees(int sr_num,string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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

                var result = con.Query<fees_receipt>(query, new { sr_number = sr_num, session = session });

                return result;
            }
        }

        public IEnumerable<fees_receipt> AllPaidFeesReg(int reg_no,string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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



                var result = con.Query<fees_receipt>(query, new { reg_no = reg_no, fin_id = fin_id, session = session });

                return result;
            }
        }

    }
}