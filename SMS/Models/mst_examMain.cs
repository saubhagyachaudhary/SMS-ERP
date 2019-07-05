using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_examMain
    {
        

        public void AddExam(mst_exam mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();

                    string query = @"INSERT INTO `mst_exam`
                                (session,
                                `exam_id`,
                                `exam_name`,
                                `max_no`,
                                `convert_to`)
                                VALUES
                                (@session,
                                @exam_id,
                                @exam_name,
                                @max_no,
                                @convert_to)";

                    string maxid = @"SELECT 
                                        IFNULL(MAX(exam_id), 0) + 1
                                    FROM
                                        mst_exam
                                    WHERE
                                        session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')";



                    int id = con.ExecuteScalar<int>(maxid);

                    mst.session = session.findFinal_Session();
                    mst.exam_id = id;
                    mst.exam_name = mst.exam_name.Trim();

                    con.Execute(query, new
                    {
                        mst.session,
                        mst.exam_id,
                        mst.exam_name,
                        mst.max_no,
                        mst.convert_to
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_exam> AllExamList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain session = new mst_sessionMain();

                string query = "SELECT * FROM mst_exam where session = @session;";

                var result = con.Query<mst_exam>(query, new { session = session.findFinal_Session() });

                return result;
            }
        }


        public mst_exam FindExam(int? id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain session = new mst_sessionMain();

                string Query = "SELECT * FROM mst_exam where exam_id = @exam_id and session = @session ";

                return con.Query<mst_exam>(Query, new { exam_id = id, session = session.findFinal_Session() }).SingleOrDefault();
            }
        }

        public void EditExam(mst_exam mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();

                    string query = "UPDATE mst_exam SET exam_name = @exam_name,max_no = @max_no,convert_to = @convert_to WHERE exam_id = @exam_id and session = @session ";

                    mst.session = session.findFinal_Session();

                    con.Execute(query, mst);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_exam DeleteExam(int id)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();

                    string Query = "DELETE FROM mst_exam WHERE exam_id = @exam_id and session = @session";

                    return con.Query<mst_exam>(Query, new { exam_id = id, session = session.findFinal_Session() }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}