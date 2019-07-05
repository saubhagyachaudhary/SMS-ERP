using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using Dapper;

namespace SMS.Models
{
    public class mst_sectionMain
    {
        

        public void AddSection(mst_section mst)
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
                                            mst_section
                                        WHERE
                                            class_id = @class_id
                                                AND section_name = @section_name
                                                AND session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_active = 'Y')";

                    int dup = con.ExecuteScalar<int>(duplicate, new { mst.class_id, mst.Section_name });

                    if (dup > 0)
                    {
                        throw new DuplicateWaitObjectException();
                    }
                    else
                    {
                        string query = "INSERT INTO mst_section(section_id,class_id,section_name,session) VALUES (@section_id,@class_id,@section_name,@session)";

                        string maxid = @"SELECT 
                                            IFNULL(MAX(section_id), 0) + 1
                                        FROM
                                            mst_section
                                        WHERE
                                            session = @session";

                        //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                        int id = con.ExecuteScalar<int>(maxid, new { session = session });

                        if (id == 1)
                        {
                            id = 100;
                        }

                        mst.section_id = id;
                        mst.Section_name = mst.Section_name.Trim();

                        con.Execute(query, new
                        {
                            mst.section_id,
                            mst.class_id,
                            mst.Section_name,
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

        public IEnumerable<mst_section> AllSectionList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                section.section_id, class.class_name, section_name
                            FROM
                                mst_section section,
                                mst_class class
                            WHERE
                                class.class_id = section.class_id
                                    AND section.session = class.session
                                    AND class.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_active = 'Y')
                            ORDER BY class.class_id";

                var result = con.Query<mst_section>(query);

                return result;
            }
        }

        public mst_section FindSection(int? id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    section.section_id, class.class_name, section_name
                                FROM
                                    mst_section section,
                                    mst_class class
                                WHERE
                                    class.class_id = section.class_id
                                    and
                                    section.session = class.session
                                    and class.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y'
                                                AND session_active = 'Y')
                                        AND section.section_id = @section_id";

                return con.Query<mst_section>(Query, new { section_id = id }).SingleOrDefault();
            }
        }

        public void EditSection(mst_section mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"UPDATE mst_section 
                                    SET 
                                        section_id = @section_id,
                                        class_id = @class_id,
                                        section_name = @section_name
                                    WHERE
                                        section_id = @section_id
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

        public mst_section DeleteSection(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"DELETE FROM mst_section 
                                WHERE
                                    section_id = @section_id
                                    AND session = (SELECT
                                        session
                                    FROM
                                        mst_session

                                    WHERE
                                        session_finalize = 'Y'
                                        AND session_active = 'Y')";

                return con.Query<mst_section>(Query, new { section_id = id }).SingleOrDefault();
            }
        }
    }
}