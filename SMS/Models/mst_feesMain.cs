using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_feesMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddFees(mst_fees mst)
        {
            try
            {
                string query = @"SELECT session FROM mst_session where session_active = 'Y'";

                string session = con.Query<string>(query).SingleOrDefault();

                 query = @"INSERT INTO mst_fees
                                   (session
                                   ,class_id
                                   ,acc_id
                                   ,fees_amount                                 
                                   ,bl_onetime
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
                                   ,bl_mar)
                                     VALUES
                                   (@session,
                                   @class_id,
                                   @acc_id,
                                   @fees_amount,
                                   @bl_onetime,
                                   @bl_apr,
                                   @bl_may,
                                   @bl_jun,
                                   @bl_jul,
                                   @bl_aug,
                                   @bl_sep,
                                   @bl_oct,
                                   @bl_nov,
                                   @bl_dec,
                                   @bl_jan,
                                   @bl_feb,
                                   @bl_mar)";

                mst.session = session;

                con.Execute(query, new
                {
                    mst.session,
                    mst.class_id,
                    mst.acc_id,
                    mst.fees_amount,
                    mst.bl_onetime,
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

        public IEnumerable<mst_fees> AllFeesList()
        {

            String query = @"SELECT
                           a.session 
                           ,a.class_id
                           ,a.acc_id 
                           ,b.class_name
                          ,c.acc_name
                          ,fees_amount
                           FROM mst_fees a,mst_class b, mst_acc_head c
                           where 
                           a.class_id = b.class_id
                           and
                           a.acc_id = c.acc_id";

            var result = con.Query<mst_fees>(query);

            return result;
        }

        public IEnumerable<mst_fees> FindFeesDetails(int sr_num)
        {
            String query = @"SELECT a.class_id
	                       ,a.acc_id
	                       ,c.acc_name
                          ,fees_amount
                          ,bl_onetime
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
                          FROM mst_fees a, mst_acc_head c,sr_register d,mst_batch e
                          where 
                          a.acc_id = c.acc_id
						  and
						  e.class_id=a.class_id
						  and
						  d.std_batch_id=e.batch_id
						  and
						  d.sr_number = @sr_num
						  and bl_onetime = 0";

            var result = con.Query<mst_fees>(query, new {sr_num = sr_num });

            return result;
        }

        public IEnumerable<mst_acc_head> account_head()
        {
            String query = "SELECT acc_id,acc_name,nature FROM mst_acc_head where nature = 'A'";

            var result = con.Query<mst_acc_head>(query);

            return result;
        }

        public mst_fees Findfees(int cls_id,int ac_id,string session)
        {
            String Query = @"SELECT a.class_id
	                       ,a.acc_id
	                       ,b.class_name
                          ,c.acc_name
                          ,fees_amount
                          ,bl_onetime
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
                          FROM mst_fees a,mst_class b, mst_acc_head c
                          where 
                          a.class_id = b.class_id
                          and
                          a.acc_id = c.acc_id
                          and
                          a.class_id = @class_id
                          and
                          a.acc_id = @acc_id
                          and 
                          a.session = @session";

            return con.Query<mst_fees>(Query, new { class_id =  cls_id, acc_id = ac_id,session = session}).SingleOrDefault();
        }

        

        public void EditFees(mst_fees mst)
        {

            try
            {
                string query = @"UPDATE mst_fees
                               SET fees_amount = @fees_amount
                                  ,bl_onetime = @bl_onetime
                                  ,bl_apr = @bl_apr
                                  ,bl_may = @bl_may
                                  ,bl_jun = @bl_jun
                                  ,bl_jul = @bl_jul
                                  ,bl_aug = @bl_aug
                                  ,bl_sep = @bl_sep
                                  ,bl_oct = @bl_oct
                                  ,bl_nov = @bl_nov
                                  ,bl_dec = @bl_dec
                                  ,bl_jan = @bl_jan
                                  ,bl_feb = @bl_feb
                                  ,bl_mar = @bl_mar
                            WHERE class_id = @class_id and acc_id = @acc_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_fees DeleteFees(int cls_id, int ac_id,string session)
        {
            String Query = "DELETE FROM mst_fees WHERE class_id = @class_id and acc_id = @acc_id and session = @session";

            return con.Query<mst_fees>(Query, new { class_id = cls_id, acc_id = ac_id,session = session }).SingleOrDefault();
        }
    }
}