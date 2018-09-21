using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class enable_wedgetMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<enable_wedget> AllUserList()
        {
            String query = @"SELECT b.user_id,c.FirstName,c.LastName,b.username FROM users b,emp_profile c where b.user_id = c.user_id";

            var result = con.Query<enable_wedget>(query);

            return result;
        }

        public IEnumerable<enable_wedget> AllWedgetList(int user_id)
        {
            String query = @"SELECT 
                                *
                            FROM
                                (SELECT 
                                    b.user_id,
                                        a.wedget_id,
                                        a.wedget_name,
                                        a.group,
                                        a.description,
                                        1 AS active
                                FROM
                                    erp_wedget a, enable_wedget b
                                WHERE
                                    a.wedget_id = b.wedget_id
                                        AND b.user_id = @user_id UNION SELECT 
                                    @user_id user_id,
                                        wedget_id,
                                        wedget_name,
                                        a.group,
                                        a.description,
                                        0 AS active
                                FROM
                                    erp_wedget a
                                WHERE
                                    a.wedget_id NOT IN (SELECT 
                                            wedget_id
                                        FROM
                                            enable_wedget
                                        WHERE
                                            user_id = @user_id)) v
                            ORDER BY wedget_name";

            var result = con.Query<enable_wedget>(query, new { user_id = user_id });

            return result;
        }

        public void AddWedget(enable_wedget mst)
        {
            try
            {
                string check = @"SELECT count(*) FROM enable_wedget where user_id = @user_id and wedget_id = @wedget_id";

                int cnt = con.Query<int>(check, new { user_id = mst.user_id, wedget_id = mst.wedget_id }).SingleOrDefault();

                if (cnt == 0)
                {
                    string query = @"INSERT INTO enable_wedget
                                (user_id,
                                wedget_id)
                                VALUES
                                (@user_id,
                                @wedget_id);";


                    con.Execute(query, new
                    {
                        mst.user_id,
                        mst.wedget_id
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteWedget(enable_wedget mst)
        {
            try
            {
                string query = @"DELETE FROM enable_wedget
                                    WHERE
                                        user_id = @user_id
                                        AND wedget_id = @wedget_id;";


                con.Execute(query, new
                {
                    mst.user_id,
                    mst.wedget_id
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}