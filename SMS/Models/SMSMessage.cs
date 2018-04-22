using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace SMS.Models
{
    public class SMSMessage
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void SendSMS(string smsText, string sendTo)
        {
            #region Variables
            string userId = ConfigurationManager.AppSettings["SMSGatewayUserID"];
            string pwd = ConfigurationManager.AppSettings["SMSGatewayPassword"];
            string senderId = ConfigurationManager.AppSettings["SMSGatewayGSMSenderID"];
            string postURL = ConfigurationManager.AppSettings["SMSGatewayPostURL"];

            StringBuilder postData = new StringBuilder();
            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            #endregion

            try
            {
                // Prepare POST data
                postData.Append("&user=" + userId);
                postData.Append("&pass=" + pwd);
                postData.Append("&sender=" + senderId);
                postData.Append("&phone=" + sendTo);
                postData.Append("&text=" + smsText);
                postData.Append("&priority=ndnd&stype=normal");

                // byte[] data = new System.Text.ASCIIEncoding().GetBytes(postData.ToString());
                byte[] data = new System.Text.UTF8Encoding().GetBytes(postData.ToString());

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

                    string msg = responseMessage.Substring(0,1);

                    string query = @"INSERT INTO `sms`.`sms_record`
                                    (`phoneNumber`,
                                    `message`,
                                    `sms_status`,
                                      date_time)
                                    VALUES
                                    (@phone,
                                    @message,
                                    @status,
                                   @date_time)";

                    DateTime dt = System.DateTime.Now.AddMinutes(750);

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
                string query = @"INSERT INTO `sms`.`sms_record`
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
                        date_time = System.DateTime.Now.AddMinutes(750)
            });
           

            throw objException;
            }
        }


        public void SendMultiSms(string smsText, string sendTo)
        {
            #region Variables
            string userId = ConfigurationManager.AppSettings["SMSGatewayUserID"];
            string pwd = ConfigurationManager.AppSettings["SMSGatewayPassword"];
            string senderId = ConfigurationManager.AppSettings["SMSGatewayGSMSenderID"];
            string postURL = ConfigurationManager.AppSettings["SMSGatewayPostURL"];

            StringBuilder postData = new StringBuilder();
            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            #endregion

            try
            {
                // Prepare POST data
                postData.Append("&user=" + userId);
                postData.Append("&pass=" + pwd);
                postData.Append("&sender=" + senderId);
                postData.Append("&phone=" + sendTo);
                postData.Append("&text=" + smsText);
                postData.Append("&priority=ndnd&stype=normal");

                // byte[] data = new System.Text.ASCIIEncoding().GetBytes(postData.ToString());
                byte[] data = new System.Text.UTF8Encoding().GetBytes(postData.ToString());

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

                    //string msg = responseMessage.Substring(0, 1);

                    //string query = @"INSERT INTO `sms`.`sms_record`
                    //                (`phoneNumber`,
                    //                `message`,
                    //                `sms_status`,
                    //                  date_time)
                    //                VALUES
                    //                (@phone,
                    //                @message,
                    //                @status,
                    //               @date_time)";

                    //DateTime dt = System.DateTime.Now.AddMinutes(750);

                    //if (msg == "S")
                    //{
                    //    con.Execute(query,
                    //   new
                    //   {
                    //       phone = sendTo,
                    //       message = smsText,
                    //       status = "Success",
                    //       date_time = dt

                    //   });
                    //}
                    //else
                    //{
                    //    con.Execute(query,
                    // new
                    // {
                    //     phone = sendTo,
                    //     message = smsText,
                    //     status = "Failed",
                    //     date_time = dt
                    // });
                    //}



                }
            }
            catch (Exception objException)
            {
                //string query = @"INSERT INTO `sms`.`sms_record`
                //                    (`phoneNumber`,
                //                    `message`,
                //                    `sms_status`,
                //                      date_time)
                //                    VALUES
                //                    (@phone,
                //                    @message,
                //                    @status,
                //                   @date_time)";
                //con.Execute(query,
                //   new
                //   {
                //       phone = sendTo,
                //       message = smsText,
                //       status = "Failed",
                //       date_time = System.DateTime.Now.AddMinutes(750)
                //   });


                throw objException;
            }
        }
    }
}