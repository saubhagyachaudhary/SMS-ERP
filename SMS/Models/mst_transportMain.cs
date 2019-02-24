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
                string query = "INSERT INTO mst_transport (session,pickup_id,pickup_point,transport_fees,transport_number,bl_apr,bl_may,bl_jun,bl_jul,bl_aug,bl_sep,bl_oct,bl_nov,bl_dec,bl_jan,bl_feb,bl_mar) VALUES (@session,@pickup_id,@pickup_point,@transport_fees,@transport_number,@bl_apr,@bl_may,@bl_jun,@bl_jul,@bl_aug,@bl_sep,@bl_oct,@bl_nov,@bl_dec,@bl_jan,@bl_feb,@bl_mar)";

                string maxid = @"SELECT 
                                        IFNULL(MAX(pickup_id), 0) + 1
                                    FROM
                                        mst_transport
                                    WHERE
                                        session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y'
                                                    AND session_active = 'Y')";

                //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                int id = con.ExecuteScalar<int>(maxid);

                if (id == 1)
                {
                    id = 1000;
                }

                mst.pickup_id = id;
                mst.pickup_point = mst.pickup_point.Trim();

              

                string query1 = @"SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_active = 'Y'";

                mst.session = con.Query<string>(query1).SingleOrDefault();

                //mst.session = sess.findActive_finalSession();

                con.Execute(query, new
                {   
                    mst.session,
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
            string query = @"SELECT 
                                    session,
                                    pickup_id,
                                    pickup_point,
                                    transport_fees,
                                    transport_number
                                FROM
                                    mst_transport
                                WHERE
                                    session = (SELECT
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_active = 'Y')";

            var result = con.Query<mst_transport>(query);

            return result;
        }

        public IEnumerable<mst_transport> AllTransportNumber()
        {
            string query = @"SELECT DISTINCT
                                transport_number
                            FROM
                                mst_transport
                            WHERE
                                session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                                    AND transport_number IS NOT NULL
                                    AND transport_number != ''";

            var result = con.Query<mst_transport>(query);

            return result;
        }

        public mst_transport FindTransport(int? id,string session)
        {
            string Query = @"SELECT 
                                session,
                                pickup_id,
                                pickup_point,
                                transport_fees,
                                transport_number,
                                bl_apr,
                                bl_may,
                                bl_jun,
                                bl_jul,
                                bl_aug,
                                bl_sep,
                                bl_oct,
                                bl_nov,
                                bl_dec,
                                bl_jan,
                                bl_feb,
                                bl_mar
                            FROM
                                mst_transport
                            WHERE
                                pickup_id = @pickup_id
                                    AND session = (SELECT
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_active = 'Y')";

            return con.Query<mst_transport>(Query, new { pickup_id = id, session = session }).SingleOrDefault();
        }

        public mst_transport FindTransportBySr(int id)
        {
            string Query = @"SELECT 
                                a.session,
                                pickup_id,
                                pickup_point,
                                transport_fees,
                                transport_number,
                                bl_apr,
                                bl_may,
                                bl_jun,
                                bl_jul,
                                bl_aug,
                                bl_sep,
                                bl_oct,
                                bl_nov,
                                bl_dec,
                                bl_jan,
                                bl_feb,
                                bl_mar
                            FROM
                                mst_transport a,
                                sr_register b
                            WHERE
                                a.pickup_id = b.std_pickup_id
                                    AND b.sr_number = @sr_num
                                    AND a.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                            AND session_active = 'Y')";

            return con.Query<mst_transport>(Query, new { sr_num = id }).SingleOrDefault();
        }

        public void EditTransport(mst_transport mst)
        {

            try
            {
                string query = @"UPDATE mst_transport 
                                    SET
                                        pickup_point = @pickup_point,
                                        transport_fees = @transport_fees,
                                        transport_number = @transport_number,
                                        bl_apr = @bl_apr,
                                        bl_may = @bl_may,
                                        bl_jun = @bl_jun,
                                        bl_jul = @bl_jul,
                                        bl_aug = @bl_aug,
                                        bl_sep = @bl_sep,
                                        bl_oct = @bl_oct,
                                        bl_nov = @bl_nov,
                                        bl_dec = @bl_dec,
                                        bl_jan = @bl_jan,
                                        bl_feb = @bl_feb,
                                        bl_mar = @bl_mar
                                    WHERE
                                        pickup_id = @pickup_id
                                            AND session = @session";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_transport DeleteTransport(int id, string session)
        {
            try
            {
                string Query = @"DELETE FROM mst_transport 
                                    WHERE
                                        pickup_id = @pickup_id
                                        AND session = @session";

                return con.Query<mst_transport>(Query, new { pickup_id = id , session = session}).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}