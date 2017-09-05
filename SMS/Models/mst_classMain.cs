﻿using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_classMain
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddClass(mst_class mst)
        {
            try
            {
                string query = "INSERT INTO [dbo].[mst_class] ([class_id],[class_name]) VALUES (@class_id,@class_name)";

                string maxid = "select isnull(MAX(class_id),0)+1 from mst_class";

                //                var id = con.Query<mst_section>(maxid).ToString().Trim();

                int id = con.ExecuteScalar<int>(maxid);
                               
                mst.class_id = id;
                mst.class_name = mst.class_name.Trim();

                con.Execute(query, new
                {
                    mst.class_id,
                    mst.class_name
                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_class> AllClassList()
        {
            String query = "SELECT [class_id],[class_name]FROM [SMS].[dbo].[mst_class]";

            var result = con.Query<mst_class>(query);

            return result;
        }

        public mst_class FindClass(int? id)
        {
            String Query = "SELECT [class_id],[class_name]FROM [SMS].[dbo].[mst_class] where class_id = @class_id";

            return con.Query<mst_class>(Query, new { class_id = id }).SingleOrDefault();
        }

        public void EditClass(mst_class mst)
        {

            try
            {
                string query = "UPDATE [dbo].[mst_class] SET [class_name] = @class_name WHERE class_id = @class_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_class DeleteClass(int id)
        {
            try
            {
                String Query = "DELETE FROM [dbo].[mst_class] WHERE class_id = @class_id";

                return con.Query<mst_class>(Query, new { class_id = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}