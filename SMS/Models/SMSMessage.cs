using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SMS.Models
{
    public class SMSMessage
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

        public IEnumerable<string> smsbody(string sms_code)
        {
            string query = @"SELECT 
                            sms_body
                        FROM
                            sms_format
                        WHERE
                            sms_code = @sms_code
                            AND enable = 1";

            return con.Query<string>(query, new { sms_code = sms_code });
        }

        public async Task SendSMS(string smsText, string sendTo)
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
                using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    // Read the response
                    using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                    {
                        responseMessage = srResponse.ReadToEnd();
                        
                    }

                    // Logic to interpret response from your gateway goes here
                    //Response.Write(String.Format("Response from gateway: {0}", responseMessage));

                    string msg = responseMessage.Substring(0,1);

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

        public IEnumerable<string> getNumberByClass(int class_id)
        {
            string query1 = @"select distinct coalesce(std_contact,std_contact1,std_contact2) from sr_register a, mst_batch b
                                where a.std_batch_id = b.batch_id
                                and b.class_id = @class_id
                                and a.std_active = 'Y';";

            var numberlist = con.Query<string>(query1,new { class_id = class_id });
            //string phone = String.Join(",", numberlist);

            return numberlist;
            //SendMultiSms("hello", phone);
        }

        public IEnumerable<string> getNumberByTransport(int pickup_id)
        {
            string query1 = @"SELECT distinct coalesce(std_contact,std_contact1,std_contact2) FROM sr_register where std_pickup_id = @pickup_id and std_pickup_id != 1000 and std_active = 'Y'";

            var numberlist = con.Query<string>(query1, new { pickup_id = pickup_id });
            //string phone = String.Join(",", numberlist);

            return numberlist;
            //SendMultiSms("hello", phone);
        }

        public IEnumerable<class_list> Class_Name()
        {
            string query1 = @"select concat( 'Class',' ' ,class_name) class_name ,class_id from mst_class";

            var class_list = con.Query<class_list>(query1 );

            return class_list;
        }

        public IEnumerable<pickup_list> pickup_Name()
        {
            string query1 = @"SELECT pickup_id,pickup_point FROM mst_transport where pickup_id != 1000";

            var class_list = con.Query<pickup_list>(query1);

            return class_list;
        }

        

        public IEnumerable<string> getNumberWholeClass()
        {
            string query1 = @"SELECT distinct coalesce(std_contact,std_contact1,std_contact2) FROM sr_register where std_active = 'Y'";

            var numberlist = con.Query<string>(query1);
            //string phone = String.Join(",", numberlist);

            return numberlist;
            //SendMultiSms("hello", phone);
        }

        public IEnumerable<string> getNumberWholeTransport()
        {
            string query1 = @"SELECT distinct coalesce(std_contact,std_contact1,std_contact2) FROM sr_register where std_pickup_id != 1000 and std_active = 'Y'";

            var numberlist = con.Query<string>(query1);
            //string phone = String.Join(",", numberlist);

            return numberlist;
            //SendMultiSms("hello", phone);
        }

        public IEnumerable<string> getNumberWholeStaff()
        {
            string query1 = @"SELECT distinct coalesce(contact,contact1,contact2) FROM emp_profile where emp_active = 1";

            var numberlist = con.Query<string>(query1);
          

            return numberlist;
            
        }

        public async Task SendMultiSms(string smsText, string sendTo,IEnumerable<string> phone)
        {
          
            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            string postURL = ConfigurationManager.AppSettings["SMSGatewayPostURL"];

            try
            {
                postURL = String.Format(postURL,sendTo,smsText);
                // byte[] data = new System.Text.ASCIIEncoding().GetBytes(postData.ToString());
                //byte[] data = new System.Text.UTF8Encoding().GetBytes(postURL.ToString());

                // Prepare web request
                request = (HttpWebRequest)WebRequest.Create(postURL);
                request.Method = "GET";
                //request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = data.Length;

               // Send the request and get a response
                using (HttpWebResponse response = (HttpWebResponse) await request.GetResponseAsync())
                {
                    
                    // Read the response
                    using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                    {
                        responseMessage = srResponse.ReadToEnd();
                    }

                    // Logic to interpret response from your gateway goes here
                    //Response.Write(String.Format("Response from gateway: {0}", responseMessage));

                    string msg = responseMessage.Substring(0, 1);

                    foreach (var i in phone)
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

                        DateTime dt = System.DateTime.Now.AddMinutes(dateTimeOffSet);

                        if (msg == "S")
                        {
                            con.Execute(query,
                           new
                           {
                               phone = i,
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
                             phone = i,
                             message = smsText,
                             status = "Failed",
                             date_time = dt
                         });

                           
                        }

                    }

                }
            }
            catch (Exception objException)
            {
                foreach (var i in phone)
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
                           phone = i,
                           message = smsText,
                           status = "Failed",
                           date_time = System.DateTime.Now.AddMinutes(dateTimeOffSet)
                       });

                }
                throw objException;
            }
        }
    }
}