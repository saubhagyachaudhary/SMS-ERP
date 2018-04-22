using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class dashboardMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public int school_strength()
        {
            String query = @"select ifnull(count(sr_number),0) from sms.sr_register where std_active = 'Y'";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

       

        public decimal cash_received()
        {

            string query1 = @"SELECT fin_id
                             FROM sms.mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            string rect_date = System.DateTime.Now.AddMinutes(750).ToString("yyyy-MM-dd");

            String query = @"select ifnull(sum(amount),0)+ifnull(sum(dc_fine),0)-ifnull(sum(dc_discount),0) from sms.fees_receipt where receipt_date = @dt and fin_id = @fin and mode_flag = 'Cash'";

            var result = con.Query<decimal>(query, new { dt = rect_date,fin=fin }).SingleOrDefault();

            return result;
        }

        public decimal bank_received()
        {

            string query1 = @"SELECT fin_id
                             FROM sms.mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            string rect_date = System.DateTime.Now.AddMinutes(750).ToString("yyyy-MM-dd");

            string query = @"select ifnull(sum(amount),0)+ifnull(sum(dc_fine),0)-ifnull(sum(dc_discount),0) from sms.fees_receipt where receipt_date = @dt and fin_id = @fin and mode_flag = 'Cheque'";

            var result = con.Query<decimal>(query, new { dt = rect_date, fin = fin }).SingleOrDefault();

            return result;
        }

        public int transport_std()
        {

            string query = @"select ifnull(count(sr_number),0) from sms.sr_register where std_active = 'Y' and std_pickup_id != 1000";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

        public int new_admission()
        {
            string query1 = @"SELECT fin_id
                             FROM sms.mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            String query = @"select ifnull(count(sr_number),0)	 from sms.sr_register where std_active = 'Y' and adm_session = @fin";

            var result = con.Query<int>(query, new { fin = fin }).SingleOrDefault();

            return result;
        }

        public string[] school_class()
        {

            String query = @"select class_name from sms.mst_class";

            var result = con.Query<string>(query);

            string[] s = result.ToArray<string>();

            return s;
        }

        public decimal[] dues()
        {
            string query1 = @"SELECT fin_id
                             FROM sms.mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            String query = @"select ifnull(sum(outstd_amount),0)-ifnull(sum(rmt_amount),0) from sms.out_standing a, sms.sr_register b, sms.mst_batch c where a.month_no <= month(date(DATE_ADD( now( ) , INTERVAL  '12:30' HOUR_MINUTE )))
                            and a.sr_number = b.sr_number
                            and b.std_batch_id = c.batch_id
                            and a.fin_id = @fin
                            group by c.class_id asc";

            var result = con.Query<decimal>(query,new {fin = fin });

            
            decimal[] s = result.ToArray<decimal>();

            
            return s;
        }

        public decimal[] recovered()
        {
            string query1 = @"SELECT fin_id
                             FROM sms.mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            String query = @"select ifnull(sum(d.amount),0)-ifnull(sum(d.dc_discount),0) from sms.out_standing a, sms.sr_register b, sms.mst_batch c,sms.fees_receipt d where a.month_no <= month(date(DATE_ADD( now( ) , INTERVAL  '12:30' HOUR_MINUTE )))
                            and a.sr_number = b.sr_number
                            and b.std_batch_id = c.batch_id
                            and a.fin_id = @fin
                            and a.serial = d.serial
                             group by c.class_id asc";

            var result = con.Query<decimal>(query, new { fin = fin });

            decimal[] s = result.ToArray<decimal>();

            return s;
        }
    }
}