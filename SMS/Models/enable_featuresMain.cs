using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class enable_featuresMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<enable_features> AllUserList()
        {
            String query = @"SELECT b.user_id,c.FirstName,c.LastName,b.username FROM users b,emp_profile c where b.user_id = c.user_id";

            var result = con.Query<enable_features>(query);

            return result;
        }

        public IEnumerable<enable_features> AllFeatureList(int user_id)
        {
            String query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        user_id, feature_id, feature_name, 1 as active
                                    FROM
                                        mst_erp_features a, enable_features b
                                    WHERE
                                        a.feature_id = b.features_id
                                            AND b.user_id = @user_id 
                                    UNION 
                                    SELECT 
                                       @user_id user_id, feature_id, feature_name, 0 as active
                                    FROM
                                        mst_erp_features a
                                    WHERE
                                        a.feature_id NOT IN (SELECT 
                                                features_id
                                            FROM
                                                enable_features
                                            WHERE
                                                user_id = @user_id)) v
                                ORDER BY feature_name";

            var result = con.Query<enable_features>(query,new {user_id = user_id });

            return result;
        }

        public void AddFeature(enable_features mst)
        {
            try
            {
                string check = @"SELECT count(*) FROM enable_features where user_id = @user_id and features_id = @feature_id";

                int cnt = con.Query<int>(check, new { user_id = mst.user_id, feature_id  = mst.feature_id}).SingleOrDefault();

                if (cnt == 0)
                {
                    string query = @"INSERT INTO enable_features
                                (user_id,
                                features_id)
                                VALUES
                                (@user_id,
                                @feature_id);";


                    con.Execute(query, new
                    {
                        mst.user_id,
                        mst.feature_id
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void DeleteFeature(enable_features mst)
        {
            try
            {
                string query = @"DELETE FROM enable_features 
                                    WHERE
                                        user_id = @user_id
                                        AND features_id = @feature_id;";


                con.Execute(query, new
                {
                    mst.user_id,
                    mst.feature_id
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}