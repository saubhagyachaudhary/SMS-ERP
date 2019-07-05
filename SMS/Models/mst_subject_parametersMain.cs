using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_subject_parametersMain
    {
        

        public IEnumerable<mst_subject_parameters> AllParametersList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                    a.parameter_id, b.subject_name, a.parameter_name
                                FROM
                                    mst_subject_parameters a,
                                    mst_subject b
                                WHERE
                                    a.session = b.session
                                        AND a.subject_id = b.subject_id
                                        AND b.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_active = 'Y')";

                var result = con.Query<mst_subject_parameters>(query);

                return result;
            }
        }

        public void AddParameters(mst_subject_parameters mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();

                    string session = sess.findActive_Session();

                    string duplicate = @"SELECT 
                                            COUNT(*)
                                        FROM
                                            mst_subject_parameters
                                        WHERE
                                            subject_id = @subject_id
                                                AND parameter_name = @parameter_name
                                                AND session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_active = 'Y')";

                    int dup = con.ExecuteScalar<int>(duplicate, new { mst.subject_id, mst.parameter_name });

                    if (dup > 0)
                    {
                        throw new DuplicateWaitObjectException();
                    }
                    else
                    {
                        string query = @"INSERT INTO `mst_subject_parameters`
                                    (`session`,
                                    `parameter_id`,
                                    `subject_id`,
                                    `parameter_name`)
                                    VALUES
                                    (@session,
                                    @parameter_id,
                                    @subject_id,
                                    @parameter_name);";

                        string maxid = @"SELECT 
                                            IFNULL(MAX(parameter_id), 0) + 1
                                        FROM
                                            mst_subject_parameters
                                        WHERE
                                            session = @session";

                        //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                        int id = con.ExecuteScalar<int>(maxid, new { session = session });



                        mst.parameter_id = id;
                        mst.parameter_name = mst.parameter_name.Trim();

                        con.Execute(query, new
                        {
                            mst.parameter_id,
                            mst.parameter_name,
                            mst.subject_id,
                            session = session
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_subject_parameters FindParameters(int? id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    a.parameter_id, b.subject_name, a.parameter_name
                                FROM
                                    mst_subject_parameters a,
                                    mst_subject b
                                WHERE
                                    a.session = b.session
                                        AND a.subject_id = b.subject_id
                                        AND b.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y'
                                                AND session_active = 'Y')
                                        AND a.parameter_id = @parameter_id";

                return con.Query<mst_subject_parameters>(Query, new { parameter_id = id }).SingleOrDefault();
            }
        }

        public void DeleteParameter(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"DELETE FROM mst_subject_parameters 
                                WHERE
                                    parameter_id = @parameter_id
                                    AND session = (SELECT
                                        session
                                    FROM
                                        mst_session

                                    WHERE
                                        session_finalize = 'Y'
                                        AND session_active = 'Y')";

                con.Execute(Query, new { parameter_id = id });
            }
        }

        public void EditParameter(mst_subject_parameters mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"UPDATE mst_subject_parameters 
                                    SET 
                                        subject_id = @subject_id,
                                        parameter_id = @parameter_id,
                                        parameter_name = @parameter_name
                                    WHERE
                                        parameter_id = @parameter_id
                                            AND session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y'
                                                    AND session_active = 'Y')";

                    con.Execute(query, mst);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}