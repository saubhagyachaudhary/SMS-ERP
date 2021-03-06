﻿using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_disciplineMain
    {
        

        public void AddDiscipline(mst_discipline mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();

                    string query = "INSERT INTO mst_discipline (session,discipline_id,discipline_name) VALUES (@session,@discipline_id,@discipline_name)";

                    string maxid = @"SELECT 
                                    IFNULL(MAX(discipline_id), 0) + 1
                                FROM
                                    mst_discipline
                                    where
                                    session = (SELECT
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

                    int id = con.ExecuteScalar<int>(maxid);

                    mst.session = session.findFinal_Session();
                    mst.discipline_id = id;
                    mst.discipline_name = mst.discipline_name.Trim();

                    con.Execute(query, new
                    {
                        mst.session,
                        mst.discipline_id,
                        mst.discipline_name
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_discipline> AllDisciplineList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain session = new mst_sessionMain();

                string query = "SELECT discipline_id,discipline_name FROM mst_discipline where session = @session";

                var result = con.Query<mst_discipline>(query, new { session = session.findFinal_Session() });

                return result;
            }
        }


        public mst_discipline FindDiscipline(int? id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain session = new mst_sessionMain();

                string Query = "SELECT discipline_id,discipline_name FROM mst_discipline where discipline_id = @discipline_id and session = @session";

                return con.Query<mst_discipline>(Query, new { discipline_id = id, session = session.findFinal_Session() }).SingleOrDefault();
            }
        }

        public void EditDiscipline(mst_discipline mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();

                    string query = "UPDATE mst_discipline SET discipline_name = @discipline_name WHERE discipline_id = @discipline_id and session = @session";

                    mst.session = session.findFinal_Session();

                    con.Execute(query, mst);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_discipline DeleteDiscipline(int id)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain session = new mst_sessionMain();

                    string Query = "DELETE FROM mst_discipline WHERE discipline_id = @discipline_id and session = @session";

                    return con.Query<mst_discipline>(Query, new { discipline_id = id, session = session.findFinal_Session() }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}