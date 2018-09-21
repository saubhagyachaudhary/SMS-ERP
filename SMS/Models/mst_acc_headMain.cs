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
                               (acc_id
                                ,acc_name
                               ,nature)
                                VALUES
                               (@acc_id
                                ,@acc_name
                               ,@nature)";

                string maxid = "select ifnull(MAX(acc_id),0)+1 from mst_acc_head";

                //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                int id = con.ExecuteScalar<int>(maxid);

                

                mst.acc_id = id;
                

                con.Execute(query, new
                {
                    mst.acc_id,
                    mst.acc_name,
                    mst.nature
                    
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_acc_head> AllAccountList()
        {
            String query = "SELECT acc_id,acc_name,nature FROM mst_acc_head";

            var result = con.Query<mst_acc_head>(query);

            return result;
        }

        public mst_acc_head FindAccount(int? id)
        {
            String Query = "SELECT acc_id,acc_name,nature FROM mst_acc_head where acc_id = @acc_id";

            return con.Query<mst_acc_head>(Query, new { acc_id = id }).SingleOrDefault();
        }

        public void EditAccount(mst_acc_head mst)
        {

            try
            {
                string query = "UPDATE mst_acc_head SET acc_name = @acc_name,nature = @nature WHERE acc_id = @acc_id";

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
                String Query = "DELETE FROM mst_acc_head WHERE acc_id = @acc_id";

                return con.Query<mst_acc_head>(Query, new { acc_id = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}