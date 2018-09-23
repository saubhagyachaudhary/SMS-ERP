using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SMS.job_scheduler
{
    public class dailyBirthdayWishMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

        public async Task SendBirthdayWish()
        {
            IEnumerable<dailyBirthdayWish> std;

            string query = @"SELECT 
                                CONCAT(IFNULL(std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name,
                                COALESCE(std_contact, std_contact1, std_contact2) std_contact
                            FROM
                                sr_register
                            WHERE
                                MONTH(std_dob) = MONTH(CURDATE())
                                    AND DAY(std_dob) = DAY(CURDATE())
                                    AND std_active = 'Y' 
                            UNION SELECT 
                                CONCAT(IFNULL(FirstName, ''),
                                        ' ',
                                        IFNULL(FirstName, '')) std_name,
                                COALESCE(contact, contact1, contact2) std_contact
                            FROM
                                emp_profile a
                            WHERE
                                MONTH(dob) = MONTH(CURDATE())
                                    AND DAY(dob) = DAY(CURDATE())
                                    AND emp_active = 1";

            std = con.Query<dailyBirthdayWish>(query);

            SMSMessage sms = new SMSMessage();

            foreach (var item in std)
            {
                foreach (var bdy in sms.smsbody("birthday"))
                {
                    await sms.SendSMS(bdy.Replace("#name#", item.std_name), item.std_contact);
                }
            }
        }
    }
}