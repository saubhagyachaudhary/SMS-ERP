using Dapper;
using MySql.Data.MySqlClient;
using SMS.Hubs;
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
        
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

        public IEnumerable<string> smsbody(string sms_code)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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
        }

        public string getRecentSMS(string sms_code,int sms_serial)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                sms_body
                            FROM
                                sms_format
                            WHERE
                                sms_serial = @sms_serial
                                    AND sms_code = @sms_code
                                    AND enable = 1";

                return con.Query<string>(query, new { sms_code = sms_code, sms_serial = sms_serial }).SingleOrDefault();
            }
        }

        public void setRecentSMS(string sms_body,int serial,string sms_code)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"UPDATE `sms_format` 
                                SET 
                                    `sms_body` = @sms_body
                                WHERE
                                    (`sms_serial` = @serial)
                                        AND (`sms_code` = @sms_code) AND enable = 1;";

                con.Execute(query, new { sms_body = sms_body, serial = serial, sms_code = sms_code });
            }

        }

        public async Task SendSMS(string smsText, string sendTo,bool flag)
        {
           
            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            string postURL = ConfigurationManager.AppSettings["SMSGatewayPostURL"];

            try
            {

                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    postURL = String.Format(postURL, sendTo, smsText);

                    request = (HttpWebRequest)WebRequest.Create(postURL);



                    // Send the request and get a response
                    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                    {
                        // Read the response
                        using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                        {
                            responseMessage = srResponse.ReadToEnd();

                        }

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
                               status = "Success",
                               date_time = System.DateTime.Now.AddMinutes(dateTimeOffSet)
                           });



                    }

                    if (flag)
                    {
                        DashboardHub hub = new DashboardHub();

                        hub.SMSCreditLeft();
                    }
                }
            }
            catch (Exception objException)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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

        public void SendOTP(string smsText, string sendTo, bool flag)
        {

            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            string postURL = ConfigurationManager.AppSettings["SMSGatewayPostURL"];

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {

                    postURL = String.Format(postURL, sendTo, smsText);

                    request = (HttpWebRequest)WebRequest.Create(postURL);

                    // Send the request and get a response
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        // Read the response
                        using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                        {
                            responseMessage = srResponse.ReadToEnd();

                        }

                    }

                    if (flag)
                    {
                        DashboardHub hub = new DashboardHub();

                        hub.SMSCreditLeft();
                    }
                }
            }
            catch (Exception objException)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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
                }

                throw objException;
            }
        }

        public IEnumerable<string> getNumberByClass(int class_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query1 = @"SELECT DISTINCT
                                    COALESCE(std_contact, std_contact1, std_contact2)
                                FROM
                                    sr_register a,
                                    mst_std_class b
                                WHERE
                                    a.sr_number = b.sr_num
                                        AND b.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')
                                        AND b.class_id = @class_id
                                        AND a.std_active = 'Y'";

                var numberlist = con.Query<string>(query1, new { class_id = class_id });
                //string phone = String.Join(",", numberlist);

                return numberlist;
                //SendMultiSms("hello", phone);
            }
        }

        public IEnumerable<string> getNumberByTransport(int pickup_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query1 = @"SELECT DISTINCT
                                COALESCE(std_contact, std_contact1, std_contact2)
                            FROM
                                sr_register
                            WHERE
                                std_pickup_id = @pickup_id
                                    AND std_pickup_id != 1000
                                    AND std_active = 'Y'";

                var numberlist = con.Query<string>(query1, new { pickup_id = pickup_id });
                //string phone = String.Join(",", numberlist);

                return numberlist;
                //SendMultiSms("hello", phone);
            }
        }

        public IEnumerable<class_list> Class_Name()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query1 = @"SELECT 
                                    CONCAT('Class', ' ', class_name) class_name, class_id
                                FROM
                                    mst_class
                                WHERE
                                    session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

                var class_list = con.Query<class_list>(query1);

                return class_list;
            }
        }

        public IEnumerable<pickup_list> pickup_Name()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query1 = @"SELECT 
                                    pickup_id, pickup_point
                                FROM
                                    mst_transport
                                WHERE
                                    pickup_id != 1000
                                        AND session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

                var class_list = con.Query<pickup_list>(query1);

                return class_list;
            }
        }

        

        public IEnumerable<string> getNumberWholeClass()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query1 = @"SELECT DISTINCT
                                    COALESCE(std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    sr_register a,
                                    mst_std_class b,
                                    mst_std_section c
                                WHERE
                                    a.sr_number = b.sr_num
                                        AND b.sr_num = c.sr_num
                                        AND b.session = c.session
                                        AND c.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')
                                        AND a.std_active = 'Y'";

                var numberlist = con.Query<string>(query1);
                //string phone = String.Join(",", numberlist);

                return numberlist;
                //SendMultiSms("hello", phone);
            }
        }

        public IEnumerable<string> getNumberWholeTransport()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query1 = @"SELECT DISTINCT
                                    COALESCE(std_contact, std_contact1, std_contact2)
                                FROM
                                    sr_register
                                WHERE
                                    std_pickup_id != 1000
                                        AND std_active = 'Y'";

                var numberlist = con.Query<string>(query1);
                //string phone = String.Join(",", numberlist);

                return numberlist;
                //SendMultiSms("hello", phone);
            }
        }

        public IEnumerable<string> getNumberWholeStaff()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query1 = @"SELECT DISTINCT
                                    COALESCE(contact, contact1, contact2)
                                FROM
                                    emp_profile
                                WHERE
                                    emp_active = 1";

                var numberlist = con.Query<string>(query1);


                return numberlist;
            }
            
        }

        public async Task SendMultiSms(string smsText, string sendTo, IEnumerable<string> phone)
        {

            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            string postURL = ConfigurationManager.AppSettings["SMSGatewayPostURL"];

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    postURL = String.Format(postURL, sendTo, smsText);

                    request = (HttpWebRequest)WebRequest.Create(postURL);


                    // Send the request and get a response
                    using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
                    {

                        // Read the response
                        using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                        {
                            responseMessage = srResponse.ReadToEnd();
                        }

                        // Logic to interpret response from your gateway goes here
                        //Response.Write(String.Format("Response from gateway: {0}", responseMessage));



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
                                   status = "Success",
                                   date_time = System.DateTime.Now.AddMinutes(dateTimeOffSet)
                               });

                        }

                    }

                    DashboardHub hub = new DashboardHub();

                    hub.SMSCreditLeft();
                }
            }
            catch (Exception objException)
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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
}