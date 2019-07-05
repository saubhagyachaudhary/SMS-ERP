using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_sessionMain
    {
        
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        public void AddSession(mst_session mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"INSERT INTO mst_session
                               (session
                               ,session_start_date
                                ,session_end_date
                                ,session_active
                                ,session_finalize)
                                VALUES
                               (@session,
                               @session_start_date,
                                @session_end_date,
                                @session_active,
                                'N')";

                
                    con.Execute(query, new
                    {
                        mst.session,
                        mst.session_start_date,
                        mst.session_end_date,
                        mst.session_active
                    });
                }
                   
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_session> AllSesssionList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                session,
                                session_start_date,
                                session_end_date,
                                session_active,
                                session_finalize
                            FROM
                                mst_session ORDER BY session_finalize desc";

            
                var result = con.Query<mst_session>(query);

                return result;
            }
        }

        public IEnumerable<string> GetSesssionList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                    session
                                FROM
                                    mst_session
                                ORDER BY session DESC";
           
                var result = con.Query<string>(query);

                return result;
            }
        }

        public mst_session FindSession(string id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    session,
                                    session_start_date,
                                    session_end_date,
                                    session_active,
                                    session_finalize
                                FROM
                                    mst_session
                                WHERE
                                    session = @session";
            
                return con.Query<mst_session>(Query, new { session = id }).SingleOrDefault();
            }
        }

        public string findActive_finalSession()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_active = 'Y'
                                        AND session_finalize = 'Y'";
            
                return con.Query<string>(Query).SingleOrDefault();
            }
        }

        public string findActive_Session()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_active = 'Y'";
            
                return con.Query<string>(Query).SingleOrDefault();
            }
        }

        public string findFinal_Session()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_finalize = 'Y'";
            
                return con.Query<string>(Query).SingleOrDefault();
            }
        }

        public mst_session getStartEndDate(string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    session_start_date, session_end_date
                                FROM
                                    mst_session
                                WHERE
                                    session = @session";
            
                return con.Query<mst_session>(Query, new { session = session }).SingleOrDefault();
            }
        }

        public bool checkSessionNotExpired()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    session_start_date, session_end_date
                                FROM
                                    mst_session
                                WHERE
                                    session_finalize = 'Y'";
            
                mst_session mst = con.Query<mst_session>(Query).SingleOrDefault();

                if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Date >= mst.session_start_date && System.DateTime.Now.AddMinutes(dateTimeOffSet).Date <= mst.session_end_date.Date)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        public void EditSession(mst_session mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"UPDATE mst_session 
                                    SET
                                        session_active = @session_active,
                                        session_finalize = @session_finalize
                                    WHERE
                                        session = @session";
               
                    con.Execute(query, mst);

                    if (mst.session_finalize == "Y")
                    {
                        query = @"UPDATE mst_session 
                                    SET
                                        session_finalize = 'C'
                                    WHERE
                                        session != @session";

                        con.Execute(query, mst);
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_session DeleteSession(string id)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    String Query = @"DELETE FROM mst_session 
                                WHERE
                                    session = @session";
                
                    return con.Query<mst_session>(Query, new { session = id }).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}