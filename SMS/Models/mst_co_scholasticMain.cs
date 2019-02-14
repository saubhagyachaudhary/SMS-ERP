using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_co_scholasticMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddCoScholastic(mst_co_scholastic mst)
        {
            try
            {
                string query = "INSERT INTO mst_co_scholastic (session,co_scholastic_id,co_scholastic_name) VALUES (@session,@co_scholastic_id,@co_scholastic_name)";

                string maxid = @"SELECT 
                                        IFNULL(MAX(co_scholastic_id), 0) + 1
                                    FROM
                                        mst_co_scholastic
                                    WHERE
                                        session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')";

                int id = con.ExecuteScalar<int>(maxid);

                mst_sessionMain session = new mst_sessionMain();

                mst.session = session.findActive_finalSession();
                mst.co_scholastic_id = id;
                mst.co_scholastic_name = mst.co_scholastic_name.Trim();

                con.Execute(query, new
                {
                    mst.co_scholastic_id,
                    mst.co_scholastic_name,
                    mst.session
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_co_scholastic> AllCoScholasticList()
        {
            mst_sessionMain session = new mst_sessionMain();

            string query = @"SELECT 
                                    co_scholastic_id, co_scholastic_name
                                FROM
                                    mst_co_scholastic
                                WHERE
                                    session = @session";

            var result = con.Query<mst_co_scholastic>(query,new {session = session.findFinal_Session() });

            return result;
        }


        public mst_co_scholastic FindCoScholastic(int? id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string Query = @"SELECT 
                                co_scholastic_id, co_scholastic_name
                            FROM
                                mst_co_scholastic
                            WHERE
                                co_scholastic_id = @co_scholastic_id
                                    AND session = @session";

            return con.Query<mst_co_scholastic>(Query, new { co_scholastic_id = id ,session = session.findFinal_Session()}).SingleOrDefault();
        }

        public void EditCoScholastic(mst_co_scholastic mst)
        {

            try
            {
                mst_sessionMain session = new mst_sessionMain();

                mst.session = session.findFinal_Session();

                string query = @"UPDATE mst_co_scholastic 
                                    SET
                                        co_scholastic_name = @co_scholastic_name
                                    WHERE
                                        co_scholastic_id = @co_scholastic_id
                                            AND session = @session";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteCoScholastic(int id)
        {
            try
            {
                mst_sessionMain session = new mst_sessionMain(); 

                string Query = @"DELETE FROM mst_co_scholastic 
                                    WHERE
                                        co_scholastic_id = @co_scholastic_id
                                        AND session = @session";

                con.Query<mst_co_scholastic>(Query, new { co_scholastic_id = id,session = session.findFinal_Session()}).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}