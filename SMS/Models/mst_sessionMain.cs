using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_sessionMain
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddSession(mst_session mst)
        {
            try
            {
                string query = @"INSERT INTO [dbo].[mst_session]
                               ([session]
                               ,[session_start_date]
                                ,[session_end_date]
                                ,[session_active])
                                VALUES
                               (@session,
                               @session_start_date,
                                @session_end_date,
                                @session_active)";

               
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
            String query = "SELECT [session],[session_start_date],[session_end_date],[session_active]FROM [SMS].[dbo].[mst_session]";

            var result = con.Query<mst_session>(query);

            return result;
        }

        public mst_session FindSession(string id)
        {
            String Query = "SELECT [session],[session_start_date],[session_end_date],[session_active]FROM [SMS].[dbo].[mst_session] where session = @session";

            return con.Query<mst_session>(Query, new { session = id }).SingleOrDefault();
        }

        public void EditSession(mst_session mst)
        {

            try
            {
                string query = "UPDATE [dbo].[mst_session] SET [session_active] = @session_active WHERE session = @session";

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
                String Query = "DELETE FROM [dbo].[mst_session] WHERE session = @session";

                return con.Query<mst_session>(Query, new { session = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}