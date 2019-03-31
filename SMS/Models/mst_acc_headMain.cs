using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_acc_headMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddHead(mst_acc_head mst)
        {
            try
            {
                string query = @"INSERT INTO mst_acc_head
                               (session
                                ,acc_id
                                ,acc_name)
                                VALUES
                               (@session
                                ,@acc_id
                                ,@acc_name)";

                string maxid = "select ifnull(MAX(acc_id),0)+1 from mst_acc_head where session = @session";

               
                mst_sessionMain sess = new mst_sessionMain();

                string ses = sess.findActive_Session();

                int id = con.ExecuteScalar<int>(maxid, new { session = ses });

                

                mst.acc_id = id;
                mst.session = ses;

                con.Execute(query, new
                {
                    mst.acc_id,
                    mst.acc_name,
                    mst.session
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_acc_head> AllAccountList()
        {
            mst_sessionMain sess = new mst_sessionMain();

            string ses = sess.findActive_Session();

            String query = "SELECT acc_id,acc_name FROM mst_acc_head where session = @session";

            var result = con.Query<mst_acc_head>(query, new { session = ses });

            return result;
        }

        public mst_acc_head FindAccount(int? id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string ses = sess.findActive_Session();

            String Query = "SELECT acc_id,acc_name FROM mst_acc_head where acc_id = @acc_id and session = @session";

            return con.Query<mst_acc_head>(Query, new { acc_id = id, session = ses }).SingleOrDefault();
        }

        public void EditAccount(mst_acc_head mst)
        {

            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string ses = sess.findActive_Session();

                mst.session = ses;

                string query = "UPDATE mst_acc_head SET acc_name = @acc_name WHERE acc_id = @acc_id and session = @session";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_acc_head DeleteAccount(int id)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string ses = sess.findActive_Session();

                String Query = "DELETE FROM mst_acc_head WHERE acc_id = @acc_id  and session = @session";

                return con.Query<mst_acc_head>(Query, new { acc_id = id, session = ses }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}