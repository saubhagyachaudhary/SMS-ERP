using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class usersMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());



        public users GetUserDetails(users us)
        {
         
            try
            {
                string query = @"SELECT a.user_id
                                  ,a.username
                                  ,a.password
                                  ,b.FirstName
                                  ,b.lastname
                                  ,a.roles
                                 FROM users a,emp_profile b
                                 where username = @username
                                 and password = @password
                                 and a.user_id = b.user_id;";

                return con.Query<users>(query, new { username = us.username , password = us.password}).SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }

          
        }

        public string GetUserFeatures(int user_id)
        {

            try
            {
                string query = @"SELECT features_id FROM enable_features where user_id = @user_id";

                var feature_list = con.Query<string>(query, new { user_id = user_id});

                return String.Join(",", feature_list);
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public string GetUserWedget(int user_id)
        {

            try
            {
                string query = @"SELECT wedget_id FROM enable_wedget where user_id = @user_id";

                var feature_list = con.Query<string>(query, new { user_id = user_id });

                return String.Join(",", feature_list);
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
                                 FROM users
                                 where username = @username";

                return con.Query<string>(query, new { username = username }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public IEnumerable<ddemp_list> employees()
        {
            string query = @"SELECT user_id,concat(FirstName,' ',LastName,' (',user_id,')') id FROM emp_profile where emp_active = 1 and user_id not in (select user_id from users)";

            var result = con.Query<ddemp_list>(query);

            return result;
        }

        public users GetUserProfileDetails(string username)
        {

            try
            {
                string query = @"SELECT a.user_id
                                  ,a.username
                                  ,a.password
                                  ,b.FirstName
                                  ,b.lastname
                                 FROM users a,emp_profile b
                                 where username = @username
                                 and a.user_id = b.user_id;";

                return con.Query<users>(query, new { username = username }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                return null;
            }


        }

        public IEnumerable<users> allUsersList()
        {

            try
            {
                string query = @"select a.user_id,a.username,b.FirstName,b.LastName,a.password,a.roles from users a,emp_profile b where a.user_id = b.user_id";

                return con.Query<users>(query);
            }
            catch
            {
                return null;
            }


        }

        public users FindUser(int user_id)
        {

                string query = @"select a.user_id,a.username,b.FirstName,b.LastName,a.password,a.roles from users a,emp_profile b where a.user_id = b.user_id and a.user_id =@user_id";

                 return con.Query<users>(query,new { user_id = user_id }).SingleOrDefault();
            
        }

        public void DeleteUser(int user_id)
        {

            string query = @"DELETE FROM enable_features WHERE user_id=@user_id";

            con.Query<users>(query, new { user_id = user_id }).SingleOrDefault();

             query = @"DELETE FROM users WHERE user_id=@user_id";

            con.Query<users>(query, new { user_id = user_id }).SingleOrDefault();

        }

        public void addUser(users user)
        {
            try
            {
                string query = @"INSERT INTO users
                            (user_id,
                            username,
                            password,
                            roles)
                            VALUES
                            (@user_id,
                            @username,
                            @password,
                            @roles)";

                con.Query<users>(query, user).SingleOrDefault();

            }
            catch
            {
                
            }
       }
    }
}