using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class std_discountMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddFees(std_discount std)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string query = @"INSERT INTO std_discount
                                   (session
                                   ,sr_num
                                   ,acc_id
                                   ,percent
                                   ,bl_exempt
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
                                   ,remark)
                                     VALUES
                                    (@session
                                    ,@sr_num
                                   ,@acc_id
                                   ,@percent
                                   ,@bl_exempt
                                   ,@bl_apr
                                   ,@bl_may
                                   ,@bl_jun
                                   ,@bl_jul
                                   ,@bl_aug
                                   ,@bl_sep
                                   ,@bl_oct
                                   ,@bl_nov
                                   ,@bl_dec
                                   ,@bl_jan
                                   ,@bl_feb
                                   ,@bl_mar
                                   ,@std_remarks)";

                std.session = sess.findActive_finalSession();

                con.Execute(query, new
                {
                    std.session,
                    std.sr_num,
                    std.acc_id,
                    std.percent,
                    std.bl_exempt,
                    std.bl_apr,
                    std.bl_may,
                    std.bl_jun,
                    std.bl_jul,
                    std.bl_aug,
                    std.bl_sep,
                    std.bl_oct,
                    std.bl_nov,
                    std.bl_dec,
                    std.bl_jan,
                    std.bl_feb,
                    std.bl_mar,
                    std.std_remarks
                });

                var p = new DynamicParameters();
                p.Add("@sr_num", std.sr_num);
                p.Add("@ac_id", std.acc_id);
                con.Execute("stdMiddiscount", p, commandType: System.Data.CommandType.StoredProcedure);

            }
            catch 
            {
                throw;
            }
        }

        public IEnumerable<std_discount> AllStdDiscountList()
        {
            string query = @"SELECT 
                                a.session,
                                a.sr_num,
                                CONCAT(ifnull(b.std_first_name,''), ' ', ifnull(b.std_last_name,'')) stdName,
                                d.class_name stdclass,
                                e.acc_name account_name,
                                CONCAT(percent, '%') per,
                                a.acc_id,
                                a.sr_num,
                                a.remark std_remarks
                            FROM
                                std_discount a,
                                sr_register b,
                                mst_std_class c,
                                mst_class d,
                                mst_acc_head e
                            WHERE
                                a.sr_num = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND c.class_id = d.class_id
                                    AND a.acc_id = e.acc_id
                                    AND a.session = c.session
                                    AND c.session = d.session
                                    AND d.session = e.session
                                    AND d.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                            AND session_active = 'Y')";

            var result = con.Query<std_discount>(query);

            return result;
        }

        public std_discount FindDiscount(int sr_num, int ac_id)
        {
            string Query = @"SELECT 
                                sr_num,
                                acc_id,
                                percent,
                                bl_exempt,
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
                                bl_mar,
                                remark std_remarks
                            FROM
                                std_discount
                            WHERE
                                sr_num = @sr_num AND acc_id = @acc_id
                                    AND session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                            AND session_active = 'Y')";

            return con.Query<std_discount>(Query, new { sr_num = sr_num, acc_id = ac_id }).SingleOrDefault();
        }



        public void EditDiscount(std_discount mst)
        {

            try
            {
                string query = @"UPDATE std_discount 
                                    SET 
                                        acc_id = @acc_id,
                                        percent = @percent,
                                        bl_exempt = @bl_exempt,
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
                                        bl_mar = @bl_mar,
                                        remark = @std_remarks
                                    WHERE
                                        sr_num = @sr_num AND acc_id = @acc_id
                                            AND session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y'
                                                    AND session_active = 'Y')";

                con.Execute(query, mst);

                var p = new DynamicParameters();
                p.Add("@sr_num", mst.sr_num);
                p.Add("@ac_id", mst.acc_id);
                con.Execute("stdMiddiscount", p, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteDiscount(int sr_num, int acc_id, string session)
        {
            try
            {
                string Query = @"DELETE FROM std_discount 
                                WHERE
                                    sr_num = @sr_num AND acc_id = @acc_id
                                    AND session = @session";




                con.Query<mst_fees>(Query, new { sr_num = sr_num, acc_id = acc_id, session = session }).SingleOrDefault();

                var p = new DynamicParameters();
                p.Add("@sr_num", sr_num);
                p.Add("@ac_id", acc_id);
                con.Execute("DeleteDiscount", p, commandType: System.Data.CommandType.StoredProcedure);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}