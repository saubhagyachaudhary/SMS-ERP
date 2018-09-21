﻿using Dapper;
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
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        public void AddSession(mst_session mst)
        {
            try
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
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_session> AllSesssionList()
        {
            String query = "SELECT session,session_start_date,session_end_date,session_active,session_finalize FROM mst_session";

            var result = con.Query<mst_session>(query);

            return result;
        }

        public mst_session FindSession(string id)
        {
            String Query = "SELECT session,session_start_date,session_end_date,session_active,session_finalize FROM mst_session where session = @session";

            return con.Query<mst_session>(Query, new { session = id }).SingleOrDefault();
        }

        public string findActive_finalSession()
        {
            string Query = "select session from mst_session where session_active = 'Y' and session_finalize = 'Y'";

            return con.Query<string>(Query).SingleOrDefault();
        }

        public string findActive_Session()
        {
            string Query = "select session from mst_session where session_active = 'Y'";

            return con.Query<string>(Query).SingleOrDefault();
        }

        public mst_session getStartEndDate(string session)
        {
             String Query = @"SELECT session_start_date,session_end_date FROM mst_session where session = @session";

             return con.Query<mst_session>(Query,new {session = session }).SingleOrDefault();
        }

        public bool checkSessionNotExpired()
        {
            String Query = @"SELECT session_start_date,session_end_date FROM mst_session where session_active = 'Y' and session_finalize = 'Y'";

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

        public void EditSession(mst_session mst)
        {

            try
            {
                string query = "UPDATE mst_session SET session_active = @session_active,session_finalize = @session_finalize WHERE session = @session";

                con.Execute(query, mst);
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
                String Query = "DELETE FROM mst_session WHERE session = @session";

                return con.Query<mst_session>(Query, new { session = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}