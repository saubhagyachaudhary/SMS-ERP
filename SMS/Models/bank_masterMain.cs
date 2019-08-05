using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class bank_masterMain
    {
        public static void AddBank(bank_master mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    
                    string query = "INSERT INTO bank_master (bank_id,bank_name) VALUES (@bank_id,@bank_name)";

                    string maxid = @"SELECT 
                                        IFNULL(MAX(bank_id), 0) + 1
                                    FROM
                                        bank_master";

                    int id = con.ExecuteScalar<int>(maxid);


                    mst.bank_id = id;
                    mst.bank_name = mst.bank_name.Trim();

                    con.Execute(query, new
                    {
                        
                        mst.bank_id,
                        mst.bank_name
                    });
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static IEnumerable<bank_master> AllBankList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {

                string query = @"SELECT 
                                bank_id, bank_name
                            FROM
                                bank_master";

                var result = con.Query<bank_master>(query);

                return result;
            }
        }


        public static bank_master FindBank(int? id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                
                string Query = @"SELECT 
                                    bank_id, bank_name
                                FROM
                                    bank_master
                                WHERE
                                    bank_id = @bank_id";

                return con.Query<bank_master>(Query, new { bank_id = id }).SingleOrDefault();
            }
        }

        public static void EditBank(bank_master mst)
        {

            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                   
                    string query = @"UPDATE bank_master
                                    SET
                                        bank_name = @bank_name
                                    WHERE
                                        bank_id = @bank_id";

                    con.Execute(query, mst);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static bank_master DeleteBank(int id)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string Query = @"DELETE FROM bank_master 
                                    WHERE
                                        bank_id = @bank_id";

                    return con.Query<bank_master>(Query, new { bank_id = id}).SingleOrDefault();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}