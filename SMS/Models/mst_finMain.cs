using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_finMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        public void AddFin(mst_fin mst)
        {
            try
            {
                 string query = @"INSERT INTO mst_fin
                               (fin_id
		                       ,fin_start_date
                               ,fin_end_date
                               ,fin_close)
                                VALUES
                               (@fin_id
		                       ,@fin_start_date
                               ,@fin_end_date
                               ,@fin_close)";

                mst.fin_close = "N";
              
                con.Execute(query, new
                {
                    mst.fin_id,
                    mst.fin_start_date,
                    mst.fin_end_date,
                    mst.fin_close
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_fin> AllFinList()
        {
            String query = @"SELECT fin_id
                          ,fin_start_date
                          ,fin_end_date
                          ,fin_close
                          FROM mst_fin";

            var result = con.Query<mst_fin>(query);

            return result;
        }

        public mst_fin FindFin(String id)
        {
            String Query = @"SELECT fin_id
                          ,fin_start_date
                          ,fin_end_date
                          ,fin_close
                           FROM mst_fin
                           where fin_id = @fin_id";

            return con.Query<mst_fin>(Query, new { fin_id = id }).SingleOrDefault();
        }


        public string FindActiveFinId()
        {
            String Query = @"SELECT fin_id
                           FROM mst_fin
                           where fin_close = 'N'";

            return con.Query<string>(Query).SingleOrDefault();
        }


        public bool checkFYnotExpired()
        {
            String Query = @"SELECT fin_id
                          ,fin_start_date
                          ,fin_end_date
                          ,fin_close
                           FROM mst_fin
                           where fin_close = 'N'";

            mst_fin mst = con.Query<mst_fin>(Query).SingleOrDefault();

            if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Date >= mst.fin_start_date && System.DateTime.Now.AddMinutes(dateTimeOffSet).Date <= mst.fin_end_date.Date)
            {
                return true;
            }
            else
            {
                return false;
            }

             
        }

        public void EditFin(mst_fin mst)
        {

            try
            {
                string query = "UPDATE mst_fin SET fin_close = @fin_close WHERE fin_id = @fin_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_fin DeleteFin(String id)
        {
            try
            {
                String Query = "DELETE FROM mst_fin WHERE fin_id = @fin_id";

                return con.Query<mst_fin>(Query, new { fin_id = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}