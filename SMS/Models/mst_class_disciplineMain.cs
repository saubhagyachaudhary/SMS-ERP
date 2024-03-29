﻿using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_class_disciplineMain
    {
        

        public void AddClassDiscipline(mst_class_discipline mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();



                    string query = @"INSERT INTO mst_class_discipline
                                   (session
                                   ,class_id
                                   ,discipline_id)
                                     VALUES
                                   (@session,
                                   @class_id,
                                   @discipline_id)";

                    mst.session = sess.findFinal_Session();

                    con.Execute(query, new
                    {
                        mst.session,
                        mst.class_id,
                        mst.discipline_id
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_class_discipline> AllClassDisciplineList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

                string query = @"SELECT 
                                a.session,
                                a.class_id,
                                a.discipline_id,
                                c.class_name,
                                b.discipline_name
                            FROM
                                mst_class_discipline a,
                                mst_discipline b,
                                mst_class c
                            WHERE
                                a.class_id = c.class_id
                                    AND a.discipline_id = b.discipline_id
                                    AND a.session = @session
                                    AND b.session = a.session
                                    AND c.session = b.session";

                var result = con.Query<mst_class_discipline>(query, new { session = sess.findFinal_Session() });

                return result;
            }
        }


        public mst_class_discipline FindDisciplineClass(int class_id, int discipline_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"SELECT a.session,a.class_id,a.discipline_id,c.class_name,b.discipline_name FROM mst_class_discipline a, mst_discipline b, mst_class c
                                where
                                a.class_id = c.class_id
                                and
                                a.discipline_id = b.discipline_id
                                and
                                a.session = @session
                                and
                                a.class_id = @class_id
                                and
                                a.discipline_id = @discipline_id";

                return con.Query<mst_class_discipline>(Query, new { class_id = class_id, discipline_id = discipline_id, session = session }).SingleOrDefault();
            }
        }

        public mst_class_discipline DeleteDisciplineClass(int class_id, int discipline_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"DELETE FROM `mst_class_discipline`
                            WHERE class_id = @class_id
                            and
                            discipline_id = @discipline_id
                            and
                            session = @session";

                return con.Query<mst_class_discipline>(Query, new { class_id = class_id, discipline_id = discipline_id, session = session }).SingleOrDefault();
            }
        }
    }
}