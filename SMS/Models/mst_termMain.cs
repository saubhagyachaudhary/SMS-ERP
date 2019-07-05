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
        

        public void AddTerm(mst_term mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();

                    string query = "INSERT INTO mst_term (session,term_id,term_name) VALUES (@session,@term_id,@term_name)";

                    string maxid = @"SELECT 
                                        IFNULL(MAX(term_id), 0) + 1
                                    FROM
                                        mst_term
                                    WHERE
                                        session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')";

                    //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                    int id = con.ExecuteScalar<int>(maxid);


                    mst.term_id = id;
                    mst.term_name = mst.term_name.Trim();
                    mst.session = sess.findFinal_Session();
                    con.Execute(query, new
                    {
                        mst.session,
                        mst.term_id,
                        mst.term_name
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_term> AllTermList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                    term_id, term_name
                                FROM
                                    mst_term
                                WHERE
                                    session = (SELECT
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

                var result = con.Query<mst_term>(query);

                return result;
            }
        }


        public mst_term FindTerm(int? id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    term_id, term_name
                                FROM
                                    mst_term
                                WHERE
                                    term_id = @term_id
                                        AND session = (SELECT
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

                return con.Query<mst_term>(Query, new { term_id = id }).SingleOrDefault();
            }
        }

        public void EditTerm(mst_term mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"UPDATE mst_term 
                                    SET
                                        term_name = @term_name
                                    WHERE
                                        term_id = @term_id
                                            AND session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')";

                    con.Execute(query, mst);
                }
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
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string Query = @"DELETE FROM mst_term 
                                    WHERE
                                        term_id = @term_id
                                        AND session = (SELECT
                                            session
                                        FROM
                                            mst_session

                                        WHERE
                                            session_finalize = 'Y')";

                    return con.Query<mst_term>(Query, new { term_id = id }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}