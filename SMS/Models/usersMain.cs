using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class usersMain
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public users GetUserDetails(users us)
        {
           
            try
            {
                string query = @"SELECT [user_id]
                                  ,[username]
                                  ,[password]
                                  ,[first_name]
                                  ,[last_name]
                                 FROM [SMS].[dbo].[users]
                                 where username = @username
                                 and password = @password";

                return con.Query<users>(query, new { username = us.username , password = us.password}).SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }

          
        }

        public string GetRole(string username)
        {

            try
            {
                string query = @"SELECT roles
                                 FROM [SMS].[dbo].[users]
                                 where username = @username";

                return con.Query<string>(query, new { username = username }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public users GetUserProfileDetails(string username)
        {

            try
            {
                string query = @"SELECT [user_id]
                                  ,[username]
                                  ,[password]
                                  ,[first_name]
                                  ,[last_name]
                                 FROM [SMS].[dbo].[users]
                                 where username = @username";

                return con.Query<users>(query, new { username = username }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }


        }
    }
}