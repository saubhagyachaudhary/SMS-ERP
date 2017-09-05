﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_finMain
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddFin(mst_fin mst)
        {
            try
            {
                 string query = @"INSERT INTO [dbo].[mst_fin]
                               ([fin_id]
		                       ,[fin_start_date]
                               ,[fin_end_date]
                               ,[fin_close])
                                VALUES
                               (@fin_id
		                       ,@fin_start_date
                               ,@fin_end_date
                               ,@fin_close)";

                mst.fin_close = "N";
              
                con.Execute(query, new
                {
                    mst.fin_id,
                    mst.fin_start_date,
                    mst.fin_end_date,
                    mst.fin_close
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_fin> AllFinList()
        {
            String query = @"SELECT [fin_id]
                          ,[fin_start_date]
                          ,[fin_end_date]
                          ,[fin_close]
                          FROM [dbo].[mst_fin]";

            var result = con.Query<mst_fin>(query);

            return result;
        }

        public mst_fin FindFin(String id)
        {
            String Query = @"SELECT [fin_id]
                          ,[fin_start_date]
                          ,[fin_end_date]
                          ,[fin_close]
                           FROM[dbo].[mst_fin]
                           where fin_id = @fin_id";

            return con.Query<mst_fin>(Query, new { fin_id = id }).SingleOrDefault();
        }

        public void EditFin(mst_fin mst)
        {

            try
            {
                string query = "UPDATE [dbo].[mst_fin] SET [fin_close] = @fin_close WHERE fin_id = @fin_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_fin DeleteFin(String id)
        {
            try
            {
                String Query = "DELETE FROM [dbo].[mst_fin] WHERE fin_id = @fin_id";

                return con.Query<mst_fin>(Query, new { fin_id = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}