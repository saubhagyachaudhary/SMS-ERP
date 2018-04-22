using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_transportMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddTransport(mst_transport mst)
        {
            try
            {
                string query = "INSERT INTO sms.mst_transport (pickup_id,pickup_point,transport_fees,transport_number,bl_apr,bl_may,bl_jun,bl_jul,bl_aug,bl_sep,bl_oct,bl_nov,bl_dec,bl_jan,bl_feb,bl_mar) VALUES (@pickup_id,@pickup_point,@transport_fees,@transport_number,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar)";

                string maxid = "select ifnull(MAX(pickup_id),0)+1 from mst_transport";

                //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                int id = con.ExecuteScalar<int>(maxid);

                if (id == 1)
                {
                    id = 1000;
                }

                mst.pickup_id = id;
                mst.pickup_point = mst.pickup_point.Trim();

                con.Execute(query, new
                {
                    mst.pickup_id,
                    mst.pickup_point,
                    mst.transport_fees,
                    mst.transport_number,
                    mst.bl_apr,
                    mst.bl_may,
                    mst.bl_jun,
                    mst.bl_jul,
                    mst.bl_aug,
                    mst.bl_sep,
                    mst.bl_oct,
                    mst.bl_nov,
                    mst.bl_dec,
                    mst.bl_jan,
                    mst.bl_feb,
                    mst.bl_mar
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_transport> AllTransportList()
        {
            String query = "SELECT pickup_id,pickup_point,transport_fees,transport_number FROM sms.mst_transport";

            var result = con.Query<mst_transport>(query);

            return result;
        }

        public mst_transport FindTransport(int? id)
        {
            String Query = "SELECT pickup_id,pickup_point,transport_fees,transport_number,bl_apr,bl_may,bl_jun,bl_jul,bl_aug,bl_sep,bl_oct,bl_nov,bl_dec,bl_jan,bl_feb,bl_mar FROM sms.mst_transport where pickup_id = @pickup_id";

            return con.Query<mst_transport>(Query, new { pickup_id = id }).SingleOrDefault();
        }

        public mst_transport FindTransportBySr(int id)
        {
            String Query = @"SELECT pickup_id
                          ,pickup_point
                          ,transport_fees
                          ,transport_number
                          ,bl_apr
                          ,bl_may
                          ,bl_jun
                          ,bl_jul
                          ,bl_aug
                          ,bl_sep
                          ,bl_oct
                          ,bl_nov
                          ,bl_dec
                          ,bl_jan
                          ,bl_feb
                          ,bl_mar
                      FROM sms.mst_transport a,sms.sr_register b
                      where a.pickup_id = b.std_pickup_id
                      and b.sr_number = @sr_num";

            return con.Query<mst_transport>(Query, new { sr_num = id }).SingleOrDefault();
        }

        public void EditTransport(mst_transport mst)
        {

            try
            {
                string query = "UPDATE sms.mst_transport SET pickup_point = @pickup_point,transport_fees = @transport_fees,transport_number = @transport_number,bl_apr = @bl_apr,bl_may= @bl_may,bl_jun= @bl_jun,bl_jul= @bl_jul,bl_aug= @bl_aug,bl_sep= @bl_sep,bl_oct= @bl_oct,bl_nov= @bl_nov,bl_dec= @bl_dec,bl_jan= @bl_jan,bl_feb= @bl_feb,bl_mar= @bl_mar WHERE pickup_id = @pickup_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_transport DeleteTransport(int id)
        {
            try
            {
                String Query = "DELETE FROM sms.mst_transport WHERE pickup_id = @pickup_id";

                return con.Query<mst_transport>(Query, new { pickup_id = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}