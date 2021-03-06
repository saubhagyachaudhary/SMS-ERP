﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_classMain
    {
        

        public void AddClass(mst_class mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();
                    string sess = session.findActive_Session();

                    string query = "INSERT INTO mst_class (session,class_id,class_name) VALUES (@session,@class_id,@class_name)";

                    string maxid = @"SELECT 
                                        IFNULL(MAX(class_id), 0) + 1
                                    FROM
                                        mst_class
                                    WHERE
                                        session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_active = 'Y')";

                    //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                    int id = con.ExecuteScalar<int>(maxid);


                    mst.class_id = id;
                    mst.class_name = mst.class_name.Trim();

                    con.Execute(query, new
                    {
                        session = sess,
                        mst.class_id,
                        mst.class_name
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_class> AllClassList(string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                class_id, class_name
                            FROM
                                mst_class
                            WHERE
                                session = @session";

                var result = con.Query<mst_class>(query, new { session = session });

                return result;
            }
        }

        public IEnumerable<mst_class> AllClassListByTeacher(int user_id,bool flag)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                if (flag)
                {
                    string query = @"SELECT DISTINCT
                                        class_id, class_name
                                    FROM
                                        mst_class
                                    WHERE
                                        session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')
                                    ORDER BY class_id";

                    var result = con.Query<mst_class>(query);
                    return result;
                }
                else
                {
                    string query = @"SELECT DISTINCT
                                        *
                                    FROM
                                        (SELECT 
                                            a.class_id, b.class_name
                                        FROM
                                            mst_attendance a, mst_class b
                                        WHERE
                                            a.class_id = b.class_id
                                                AND a.user_id = @user_id
                                                AND b.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_finalize = 'Y') UNION ALL SELECT 
                                            a.class_id, b.class_name
                                        FROM
                                            mst_attendance a, mst_class b
                                        WHERE
                                            a.class_id = b.class_id
                                                AND a.finalizer = @user_id
                                                AND b.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_finalize = 'Y')) a
                                    ORDER BY a.class_id";

                    var result = con.Query<mst_class>(query, new { user_id = user_id });
                    return result;
                }
            }
        }

        public IEnumerable<mst_class> AllClassListWithSection(string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                b.section_id class_id,
                                CONCAT(IFNULL(a.class_name, ''),
                                        ' Section ',
                                        IFNULL(b.section_name, '')) class_name
                            FROM
                                mst_class a,
                                mst_section b
                            WHERE
                                a.session = b.session
                                    AND b.session = @session
                                    AND a.class_id = b.class_id
                            ORDER BY class_name";

                var result = con.Query<mst_class>(query, new { session = session });

                return result;
            }
        }
        
        public mst_class FindClass(int? id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    class_id, class_name
                                FROM
                                    mst_class
                                WHERE
                                    class_id = @class_id
                                        AND session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_active = 'Y')";

                return con.Query<mst_class>(Query, new { class_id = id }).SingleOrDefault();
            }
        }

        public void EditClass(mst_class mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"UPDATE mst_class 
                                    SET
                                        class_name = @class_name
                                    WHERE
                                        class_id = @class_id
                                        AND session = (SELECT
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_active = 'Y')";

                    con.Execute(query, mst);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_class DeleteClass(int id)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string Query = @"DELETE FROM mst_class 
                                WHERE
                                    class_id = @class_id
                                    AND session = (SELECT
                                        session
                                    FROM
                                        mst_session

                                    WHERE
                                        session_finalize = 'Y'
                                        AND session_active = 'Y')";

                    return con.Query<mst_class>(Query, new { class_id = id }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}