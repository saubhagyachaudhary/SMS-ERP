﻿using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_class_subjectMain
    {
        

        public void AddClassSubject(mst_class_subject mst)
        {
            try
            {

                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();



                    string query = @"INSERT INTO mst_class_subject
                                   (session
                                   ,class_id
                                   ,subject_id)
                                     VALUES
                                   (@session,
                                   @class_id,
                                   @subject_id)";

                    mst.session = sess.findFinal_Session();

                    con.Execute(query, new
                    {
                        mst.session,
                        mst.class_id,
                        mst.subject_id
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_class_subject> AllClassSubjectList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

                string query = @"SELECT 
                                a.session,
                                a.class_id,
                                a.subject_id,
                                c.class_name,
                                b.subject_name
                            FROM
                                mst_class_subject a,
                                mst_subject b,
                                mst_class c
                            WHERE
                                a.class_id = c.class_id
                                    AND a.subject_id = b.subject_id
                                    AND a.session = @session
                                    AND b.session = a.session
                                    AND a.session = c.session";

                var result = con.Query<mst_class_subject>(query, new { session = sess.findFinal_Session() });

                return result;
            }
        }

       
        public mst_class_subject FindSubjectClass(int class_id, int subject_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"SELECT 
                                a.session,
                                a.class_id,
                                a.subject_id,
                                c.class_name,
                                b.subject_name
                            FROM
                                mst_class_subject a,
                                mst_subject b,
                                mst_class c
                            WHERE
                                a.class_id = c.class_id
                                    AND a.subject_id = b.subject_id
                                    AND a.session = @session
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND a.class_id = @class_id
                                    AND a.subject_id = @subject_id";

                return con.Query<mst_class_subject>(Query, new { class_id = class_id, subject_id = subject_id, session = session }).SingleOrDefault();
            }
        }

       

        public mst_class_subject DeleteSubjectClass(int class_id, int subject_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"DELETE FROM `mst_class_subject`
                            WHERE class_id = @class_id
                            and
                            subject_id = @subject_id
                            and
                            session = @session";

                return con.Query<mst_class_subject>(Query, new { class_id = class_id, subject_id = subject_id, session = session }).SingleOrDefault();
            }
        }
    }
}