﻿using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_attendanceMain
    {
        

        public void Assign_faculty(mst_attendance mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"INSERT INTO `mst_attendance`
                                (`user_id`,
                                `class_id`,
                                `section_id`,
                                `finalizer`)
                                VALUES
                                (@user_id,
                                @class_id,
                                @section_id,
                                @finalizer_user_id)";

                    con.Execute(query, new
                    {
                        mst.user_id,
                        mst.class_id,
                        mst.finalizer_user_id,
                        mst.section_id
                    });
                }
            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<mst_attendance> assignList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String query = @"SELECT 
                                    a.user_id,
                                    a.class_id,
                                    a.section_id,
                                    e.section_name,
                                    CONCAT(IFNULL(c.FirstName, ''),
                                            ' ',
                                            IFNULL(c.LastName, '')) faculty_name,
                                    b.class_name,
                                    a.finalizer finalizer_user_id,
                                    CONCAT(IFNULL(d.FirstName, ''),
                                            ' ',
                                            IFNULL(d.LastName, '')) finalizer_name
                                FROM
                                    mst_attendance a,
                                    mst_class b,
                                    emp_profile c,
                                    emp_profile d,
                                    mst_section e
                                WHERE
                                    a.class_id = b.class_id
                                        AND a.user_id = c.user_id
                                        AND a.finalizer = d.user_id
                                        AND a.section_id = e.section_id
                                        AND b.session = e.session
                                        AND e.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

                var result = con.Query<mst_attendance>(query);

                return result;
            }
        }

        public IEnumerable<mst_attendance> Attendance_class_list(int user_id, bool flag, string role)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = "";
                if (flag)
                {
                    query = @"SELECT 
                                    b.class_id, b.class_name, a.section_id, c.section_name
                                FROM
                                    mst_attendance a,
                                    mst_class b,
                                    mst_section c
                                WHERE
                                         a.class_id = b.class_id
                                        AND a.section_id NOT IN (SELECT DISTINCT
                                            section_id
                                        FROM
                                            attendance_register d,
                                            mst_std_section e
                                        WHERE
                                            d.sr_num = e.sr_num
                                                AND att_date = DATE(DATE_ADD(NOW(),
                                                    INTERVAL '00:00' HOUR_MINUTE))
                                                AND d.session = e.session
                                                AND e.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_finalize = 'Y'))
                                        AND a.section_id = c.section_id
                                        AND b.session = c.session
                                        AND c.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";
                    var result = con.Query<mst_attendance>(query, new { user_id = user_id });

                    foreach (var i in result)
                    {
                        i.role = role;
                    }

                    return result;
                }
                else
                {
                    query = @"SELECT DISTINCT
                                    b.class_id, b.class_name, a.section_id, c.section_name, 'Class Teacher' role
                                FROM
                                    mst_attendance a,
                                    mst_class b,
                                    mst_section c
                                WHERE
                                    a.user_id = @user_id
                                        AND a.class_id = b.class_id
                                        AND a.section_id NOT IN (SELECT DISTINCT
                                            section_id
                                        FROM
                                            attendance_register d,
                                            mst_std_section e
                                        WHERE
                                            d.sr_num = e.sr_num
                                                AND att_date = DATE(DATE_ADD(NOW(),
                                                    INTERVAL '00:00' HOUR_MINUTE))
                                                AND d.session = e.session
                                                AND e.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_finalize = 'Y'))
                                        AND a.section_id = c.section_id
                                        AND b.session = c.session
                                        AND c.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y') 
                                UNION ALL SELECT 
                                    b.class_id, b.class_name, a.section_id, c.section_name, 'Finalizer' role
                                FROM
                                    mst_attendance a,
                                    mst_class b,
                                    mst_section c
                                WHERE
                                    a.finalizer = @user_id
                                        AND a.class_id = b.class_id
                                        AND a.section_id NOT IN (SELECT DISTINCT
                                            section_id
                                        FROM
                                            attendance_register d,
                                            mst_std_section e
                                        WHERE
                                            d.sr_num = e.sr_num
                                                AND att_date = DATE(DATE_ADD(NOW(),
                                                    INTERVAL '00:00' HOUR_MINUTE))
                                                AND d.session = e.session
                                                AND e.session = (SELECT 
                                                    session
                                                FROM
                                                    mst_session
                                                WHERE
                                                    session_finalize = 'Y'))
                                        AND a.section_id = c.section_id
                                        AND b.session = c.session
                                        AND c.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";
                    var result = con.Query<mst_attendance>(query, new { user_id = user_id });

                    return result;
                }

            }
            
        }

        public void deleteFaculty(int user_id,int class_id,int finalizer_user_id,int section_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String query = @"DELETE FROM `mst_attendance`
                            WHERE user_id = @user_id and class_id = @class_id and finalizer = @finalizer_user_id and section_id = @section_id";

                con.Query(query, new { user_id = user_id, class_id = class_id, finalizer_user_id = finalizer_user_id, section_id = section_id });

            }
        }

        public mst_attendance FindFaculty(int user_id,int class_id,int finalizer_user_id,int section_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String query = @"SELECT 
                                a.user_id,
                                a.class_id,
                                e.section_name,
                                CONCAT(ifnull(c.FirstName,''), ' ', ifnull(c.LastName,'')) faculty_name,
                                b.class_name,
                                a.finalizer finalizer_user_id,
                                CONCAT(ifnull(d.FirstName,''), ' ', ifnull(d.LastName,'')) finalizer_name
                            FROM
                                mst_attendance a,
                                mst_class b,
                                emp_profile c,
                                emp_profile d,
                                mst_section e
                            WHERE
                                a.class_id = b.class_id
                                    AND a.user_id = c.user_id
                                    AND a.finalizer = d.user_id
                                    AND a.class_id = @class_id
                                    AND a.user_id = @user_id
                                    AND a.finalizer = @finalizer_user_id
                                    AND a.section_id = e.section_id
                                    AND a.section_id = @section_id
                                    AND b.session = e.session
                                    AND e.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')";



                return con.Query<mst_attendance>(query, new { user_id = user_id, class_id = class_id, finalizer_user_id = finalizer_user_id, section_id = section_id }).SingleOrDefault();
            }
        }
    }
}