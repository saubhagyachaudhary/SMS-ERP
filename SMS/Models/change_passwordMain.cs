using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class change_passwordMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void changePassword(change_password password)
        {
            try
            {
                string query = @"UPDATE users 
                                SET 
                                    password = @new_password
                                WHERE
                                    user_id = @user_id
                                        AND username = @username";

                con.Execute(query, password);
            }
            catch
            {
                throw null;
            }
        }

        public int checkPassword(change_password password)
        {
            try
            {
                string query = @"SELECT 
                                    COUNT(*)
                                FROM
                                    users
                                WHERE
                                    password = @old_password AND user_id = user_id
                                        AND username = username";

                return con.Query<int>(query, password).SingleOrDefault();
            }
            catch
            {
                throw null;
            }
        }
    }
}