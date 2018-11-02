using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_subjectMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddSubject(mst_subject mst)
        {
            try
            {
                mst_sessionMain session = new mst_sessionMain();

                string query = "INSERT INTO mst_subject (session,subject_id,subject_name) VALUES (@session,@subject_id,@subject_name)";

                string maxid = @"SELECT 
                                        IFNULL(MAX(subject_id), 0) + 1
                                    FROM
                                        mst_subject
                                    WHERE
                                        session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y'
                                                    AND session_active = 'Y')";

                int id = con.ExecuteScalar<int>(maxid);


                mst.session = session.findActive_finalSession();
                mst.subject_id = id;
                mst.subject_name = mst.subject_name.Trim();

                con.Execute(query, new
                {
                    mst.session,
                    mst.subject_id,
                    mst.subject_name
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_subject> AllSubjectList()
        {
            mst_sessionMain session = new mst_sessionMain();

            string query = @"SELECT 
                                subject_id, subject_name
                            FROM
                                mst_subject
                            WHERE
                                session = @session";

            var result = con.Query<mst_subject>(query, new {session = session.findActive_finalSession() });

            return result;
        }


        public mst_subject FindSubject(int? id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string Query = @"SELECT 
                                    subject_id, subject_name
                                FROM
                                    mst_subject
                                WHERE
                                    subject_id = @subject_id
                                        AND session = @session";

            return con.Query<mst_subject>(Query, new { subject_id = id,session = session.findActive_finalSession() }).SingleOrDefault();
        }

        public void EditSubject(mst_subject mst)
        {

            try
            {
                mst_sessionMain session = new mst_sessionMain();

                mst.session = session.findActive_finalSession();

                string query = @"UPDATE mst_subject 
                                    SET
                                        subject_name = @subject_name
                                    WHERE
                                        subject_id = @subject_id
                                            AND session = @session";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_subject DeleteSubject(int id)
        {
            try
            {
                mst_sessionMain session = new mst_sessionMain();

                string Query = @"DELETE FROM mst_subject 
                                    WHERE
                                        subject_id = @subject_id
                                        AND session = @session";

                return con.Query<mst_subject>(Query, new { subject_id = id,session=session.findActive_finalSession() }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}