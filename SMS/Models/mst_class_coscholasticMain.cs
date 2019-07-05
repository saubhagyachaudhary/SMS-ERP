using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_class_coscholasticMain
    {
        

        public void AddClassCoscholastic(mst_class_coscholastic mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();



                    string query = @"INSERT INTO mst_class_coscholastic
                                   (session
                                   ,class_id
                                   ,co_scholastic_id)
                                     VALUES
                                   (@session,
                                   @class_id,
                                   @co_scholastic_id)";

                    mst.session = sess.findFinal_Session();

                    con.Execute(query, new
                    {
                        mst.session,
                        mst.class_id,
                        mst.co_scholastic_id
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_class_coscholastic> AllClassCoscholasticList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

                string query = @"SELECT 
                                    a.session,
                                    a.class_id,
                                    a.co_scholastic_id,
                                    c.class_name,
                                    b.co_scholastic_name
                                FROM
                                    mst_class_coscholastic a,
                                    mst_co_scholastic b,
                                    mst_class c
                                WHERE
                                    a.class_id = c.class_id
                                        AND a.co_scholastic_id = b.co_scholastic_id
                                        AND a.session = @session
                                        AND b.session = a.session
                                        AND c.session = b.session";

                var result = con.Query<mst_class_coscholastic>(query, new { session = sess.findFinal_Session() });

                return result;
            }
        }


        public mst_class_coscholastic FindCoscholasticClass(int class_id, int co_scholastic_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"SELECT a.session,a.class_id,a.co_scholastic_id,c.class_name,b.co_scholastic_name FROM mst_class_coscholastic a, mst_co_scholastic b, mst_class c
                                where
                                a.class_id = c.class_id
                                and
                                a.co_scholastic_id = b.co_scholastic_id
                                and
                                a.session = @session
                                and
                                a.class_id = @class_id
                                and
                                a.co_scholastic_id = @co_scholastic_id";

                return con.Query<mst_class_coscholastic>(Query, new { class_id = class_id, co_scholastic_id = co_scholastic_id, session = session }).SingleOrDefault();
            }
        }

        public mst_class_coscholastic DeleteCoscholasticClass(int class_id, int co_scholastic_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"DELETE FROM `mst_class_coscholastic`
                            WHERE class_id = @class_id
                            and
                            co_scholastic_id = @co_scholastic_id
                            and
                            session = @session";

                return con.Query<mst_class_coscholastic>(Query, new { class_id = class_id, co_scholastic_id = co_scholastic_id, session = session }).SingleOrDefault();
            }
        }

    }
}