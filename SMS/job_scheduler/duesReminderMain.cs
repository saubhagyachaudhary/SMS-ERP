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
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

        public async Task SendDuesReminder()
        {
            IEnumerable<duesReminder> std;

            string query;

            mst_sessionMain session = new mst_sessionMain();

            if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
            {
                query = @"SELECT 
                                CONCAT(IFNULL(b.std_first_name, ''),
                                        ' ',
                                        IFNULL(b.std_last_name, '')) std_name,
                                a.sr_number,
                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                COALESCE(b.std_contact,
                                        std_contact1,
                                        std_contact2) std_contact
                            FROM
                                out_standing a,
                                sr_register b
                            WHERE
                                IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                    AND month_no <= MONTH(CURDATE())
                                    AND session = @session
                                    AND a.sr_number = b.sr_number
                                    AND b.std_active = 'Y'
                                    AND month_no BETWEEN 4 AND 12
                            GROUP BY sr_number";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 1)
            {
                query = @"SELECT 
                                CONCAT(IFNULL(b.std_first_name, ''),
                                        ' ',
                                        IFNULL(b.std_last_name, '')) std_name,
                                a.sr_number,
                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                COALESCE(b.std_contact,
                                        std_contact1,
                                        std_contact2) std_contact
                            FROM
                                out_standing a,
                                sr_register b
                            WHERE
                                IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                    and month_no not in (2,3)
                                    AND a.session = @session
                                    AND a.sr_number = b.sr_number
                                    AND b.std_active = 'Y'
                            GROUP BY sr_number";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 2)
            {
                query = @"SELECT 
                                CONCAT(IFNULL(b.std_first_name, ''),
                                        ' ',
                                        IFNULL(b.std_last_name, '')) std_name,
                                a.sr_number,
                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                COALESCE(b.std_contact,
                                        std_contact1,
                                        std_contact2) std_contact
                            FROM
                                out_standing a,
                                sr_register b
                            WHERE
                                IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                    AND month_no != 3
                                    AND a.session = @session
                                    AND a.sr_number = b.sr_number
                                    AND b.std_active = 'Y'
                            GROUP BY sr_number";


            }
            else
            {
                query = @"SELECT 
                                CONCAT(IFNULL(b.std_first_name, ''),
                                        ' ',
                                        IFNULL(b.std_last_name, '')) std_name,
                                a.sr_number,
                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues,
                                COALESCE(b.std_contact,
                                        std_contact1,
                                        std_contact2) std_contact
                            FROM
                                out_standing a,
                                sr_register b
                            WHERE
                                IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                    AND a.session = @session
                                    AND a.sr_number = b.sr_number
                                    AND b.std_active = 'Y'
                            GROUP BY sr_number";
            }
            std = con.Query<duesReminder>(query, new { session = session.findActive_finalSession() });

            SMSMessage sms = new SMSMessage();

            foreach (var item in std)
            {
                foreach (var bdy in sms.smsbody("dues_reminder"))
                {
                    string body = bdy.Replace("#student_name#", item.std_name);

                    body = body.Replace("#sr_number#", item.sr_number.ToString());

                    body = body.Replace("#dues#", item.dues.ToString());

                    body = body.Replace("#current_date#", DateTime.Now.ToString("dddd, dd MMMM yyyy"));

                    await sms.SendSMS(body, item.std_contact,false);
                }
            }

            DashboardHub hub = new DashboardHub();

            hub.SMSCreditLeft();

        }
    }
}