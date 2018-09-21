using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;

namespace SMS.report
{
    public class Notification
    {
        string donotreplyMail = ConfigurationManager.AppSettings["donotreplyMail"].ToString();
        string donotreplyMailPassword = ConfigurationManager.AppSettings["donotreplyMailPassword"].ToString();

        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

        public void _sendMail(string mail_id,string subject ,string body)
        {
            string FromMail = donotreplyMail;
            string ToMail = mail_id;
            
            string Body = body;
            
            
           
                using (MailMessage mm = new MailMessage(
                  FromMail, ToMail, subject, Body))
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

        public void SendSMS(string smsText, string sendTo)
        {

            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            string postURL = ConfigurationManager.AppSettings["SMSGatewayPostURL"];

            try
            {


                postURL = String.Format(postURL, sendTo, smsText);
                // byte[] data = new System.Text.ASCIIEncoding().GetBytes(postData.ToString());
                byte[] data = new System.Text.UTF8Encoding().GetBytes(postURL.ToString());


                // Prepare web request
                request = (HttpWebRequest)WebRequest.Create(postURL);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = data.Length;

                // Write data to stream
                using (Stream newStream = request.GetRequestStream())
                {
                    newStream.Write(data, 0, data.Length);
                }



                // Send the request and get a response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Read the response
                    using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                    {
                        responseMessage = srResponse.ReadToEnd();

                    }

                    // Logic to interpret response from your gateway goes here
                    //Response.Write(String.Format("Response from gateway: {0}", responseMessage));

                    string msg = responseMessage.Substring(0, 1);

                    string query = @"INSERT INTO sms_record
                                    (`phoneNumber`,
                                    `message`,
                                    `sms_status`,
                                      date_time)
                                    VALUES
                                    (@phone,
                                    @message,
                                    @status,
                                   @date_time)";

                    DateTime dt = System.DateTime.Now.AddMinutes(dateTimeOffSet);

                    if (msg == "S")
                    {
                        con.Execute(query,
                       new
                       {
                           phone = sendTo,
                           message = smsText,
                           status = "Success",
                           date_time = dt

                       });
                    }
                    else
                    {
                        con.Execute(query,
                     new
                     {
                         phone = sendTo,
                         message = smsText,
                         status = "Failed",
                         date_time = dt
                     });
                    }



                }
            }
            catch (Exception objException)
            {
                string query = @"INSERT INTO sms_record
                                    (`phoneNumber`,
                                    `message`,
                                    `sms_status`,
                                      date_time)
                                    VALUES
                                    (@phone,
                                    @message,
                                    @status,
                                   @date_time)";
                con.Execute(query,
                   new
                   {
                       phone = sendTo,
                       message = smsText,
                       status = "Failed",
                       date_time = System.DateTime.Now.AddMinutes(dateTimeOffSet)
                   });


                throw objException;
            }
        }
    }
}