using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_termMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddTerm(mst_term mst)
        {
            try
            {

                mst_sessionMain sess = new mst_sessionMain();

                string query = "INSERT INTO mst_term (session,term_id,term_name) VALUES (@session,@term_id,@term_name)";

                string maxid = "select ifnull(MAX(term_id),0)+1 from mst_term";

                //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                int id = con.ExecuteScalar<int>(maxid);


                mst.term_id = id;
                mst.term_name = mst.term_name.Trim();
                mst.session = sess.findActive_finalSession();
                con.Execute(query, new
                {
                    mst.session,
                    mst.term_id,
                    mst.term_name
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_term> AllTermList()
        {
            String query = "SELECT term_id,term_name FROM mst_term";

            var result = con.Query<mst_term>(query);

            return result;
        }


        public mst_term FindTerm(int? id)
        {
            String Query = "SELECT term_id,term_name FROM mst_term where term_id = @term_id";

            return con.Query<mst_term>(Query, new { term_id = id }).SingleOrDefault();
        }

        public void EditTerm(mst_term mst)
        {

            try
            {
                string query = "UPDATE mst_term SET term_name = @term_name WHERE term_id = @term_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_term DeleteTerm(int id)
        {
            try
            {
                String Query = "DELETE FROM mst_term WHERE term_id = @term_id";

                return con.Query<mst_term>(Query, new { term_id = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}