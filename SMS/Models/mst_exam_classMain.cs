using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace SMS.Models
{
    public class mst_exam_classMain
    {
        
        public void AddExamClass(mst_exam_class mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();



                    string query = @"INSERT INTO mst_exam_class
                                   (session
                                   ,class_id
                                   ,exam_id)
                                     VALUES
                                   (@session,
                                   @class_id,
                                   @exam_id)";

                    mst.session = sess.findFinal_Session();

                    con.Execute(query, new
                    {
                        mst.session,
                        mst.class_id,
                        mst.exam_id
                    });

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_exam_class> AllExamClassList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

                string query = @"SELECT 
                                    a.session, a.class_id, a.exam_id, c.class_name, b.exam_name
                                FROM
                                    mst_exam_class a,
                                    mst_exam b,
                                    mst_class c
                                WHERE
                                    a.class_id = c.class_id
                                        AND a.exam_id = b.exam_id
                                        AND a.session = @session
                                        AND a.session = b.session
                                        AND b.session = c.session";

                var result = con.Query<mst_exam_class>(query, new { session = sess.findFinal_Session() });

                return result;
            }
        }


        public mst_exam_class FindExamClass(int class_id, int exam_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    a.session, a.class_id, a.exam_id, c.class_name, b.exam_name
                                FROM
                                    mst_exam_class a,
                                    mst_exam b,
                                    mst_class c
                                WHERE
                                    a.class_id = c.class_id
                                        AND a.exam_id = b.exam_id
                                        AND a.session = b.session
                                        AND b.session = c.session
                                        AND c.session = @session
                                        AND a.class_id = @class_id
                                        AND a.exam_id = @exam_id";

                return con.Query<mst_exam_class>(Query, new { class_id = class_id, exam_id = exam_id, session = session }).SingleOrDefault();
            }
        }

        public mst_exam_class DeleteExamClass(int class_id, int exam_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"DELETE FROM `mst_exam_class`
                            WHERE class_id = @class_id
                            and
                            exam_id = @exam_id
                            and
                            session = @session";

                return con.Query<mst_exam_class>(Query, new { class_id = class_id, exam_id = exam_id, session = session }).SingleOrDefault();
            }
        }
    }
}