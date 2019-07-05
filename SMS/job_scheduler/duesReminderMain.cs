using Dapper;
using MySql.Data.MySqlClient;
using SMS.Hubs;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SMS.job_scheduler
{
    public class duesReminderMain
    {
        
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

        public async Task SendDuesReminder()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                IEnumerable<duesReminder> std;

                string query;

                mst_sessionMain session = new mst_sessionMain();

                if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
                {
                    query = @"SELECT 
                                std_name, sr_number, SUM(dues) dues, std_contact
                            FROM
                                (SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND month_no <= MONTH(CURDATE())
                                        AND session = @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                        AND month_no BETWEEN 4 AND 12
                                GROUP BY sr_number UNION ALL SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND a.session != @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                GROUP BY sr_number) a
                            WHERE
                                a.sr_number IN (SELECT 
                                        sr_num
                                    FROM
                                        mst_std_class
                                    WHERE
                                        session = @session
                                            AND class_id IN (SELECT 
                                                class_id
                                            FROM
                                                enable_dues_reminder
                                            WHERE
                                                session = @session AND enable = 1))
                            GROUP BY a.sr_number";
                }
                else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 1)
                {
                    query = @"SELECT 
                                std_name, sr_number, SUM(dues) dues, std_contact
                            FROM
                                (SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND month_no NOT IN (2 , 3)
                                        AND session = @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                GROUP BY sr_number UNION ALL SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND a.session != @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                GROUP BY sr_number) a
                            WHERE
                                a.sr_number IN (SELECT 
                                        sr_num
                                    FROM
                                        mst_std_class
                                    WHERE
                                        session = @session
                                            AND class_id IN (SELECT 
                                                class_id
                                            FROM
                                                enable_dues_reminder
                                            WHERE
                                                session = @session AND enable = 1))
                            GROUP BY a.sr_number";
                }
                else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 2)
                {
                    query = @"SELECT 
                                std_name, sr_number, SUM(dues) dues, std_contact
                            FROM
                                (SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND month_no != 3
                                        AND session = @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                GROUP BY sr_number UNION ALL SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND a.session != @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                GROUP BY sr_number) a
                            WHERE
                                a.sr_number IN (SELECT 
                                        sr_num
                                    FROM
                                        mst_std_class
                                    WHERE
                                        session = @session
                                            AND class_id IN (SELECT 
                                                class_id
                                            FROM
                                                enable_dues_reminder
                                            WHERE
                                                session = @session AND enable = 1))
                            GROUP BY a.sr_number";


                }
                else
                {
                    query = @"SELECT 
                                std_name, sr_number, SUM(dues) dues, std_contact
                            FROM
                                (SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND session = @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                GROUP BY sr_number UNION ALL SELECT 
                                    CONCAT(IFNULL(b.std_first_name, ''), ' ', IFNULL(b.std_last_name, '')) std_name,
                                        a.sr_number,
                                        SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                        COALESCE(b.std_contact, std_contact1, std_contact2) std_contact
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                        AND a.session != @session
                                        AND a.sr_number = b.sr_number
                                        AND b.std_active = 'Y'
                                GROUP BY sr_number) a
                            WHERE
                                a.sr_number IN (SELECT 
                                        sr_num
                                    FROM
                                        mst_std_class
                                    WHERE
                                        session = @session
                                            AND class_id IN (SELECT 
                                                class_id
                                            FROM
                                                enable_dues_reminder
                                            WHERE
                                                session = @session AND enable = 1))
                            GROUP BY a.sr_number";
                }


                std = con.Query<duesReminder>(query, new { session = session.findFinal_Session() });

                SMSMessage sms = new SMSMessage();

                foreach (var item in std)
                {
                    foreach (var bdy in sms.smsbody("dues_reminder"))
                    {
                        string body = bdy.Replace("#student_name#", item.std_name);

                        body = body.Replace("#sr_number#", item.sr_number.ToString());

                        body = body.Replace("#dues#", item.dues.ToString());

                        body = body.Replace("#current_date#", DateTime.Now.ToString("dddd, dd MMMM yyyy"));
#if !DEBUG
                    await sms.SendSMS(body, item.std_contact,false);
#endif
                    }
                }

                DashboardHub hub = new DashboardHub();

                hub.SMSCreditLeft();

            }
        }
    }
}