using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_classMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddClass(mst_class mst)
        {
            try
            {
                string query = "INSERT INTO mst_class (class_id,class_name) VALUES (@class_id,@class_name)";

                string maxid = "select ifnull(MAX(class_id),0)+1 from mst_class";

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
            String query = "SELECT class_id,class_name FROM mst_class";

            var result = con.Query<mst_class>(query);

            return result;
        }

        public IEnumerable<mst_class> AllClassListByTeacher(int user_id,bool flag)
        {

            if (flag)
            {
                String query = @"SELECT distinct a.class_id,b.class_name FROM mst_attendance a,mst_class b 
                            where 
                            a.class_id = b.class_id";

                var result = con.Query<mst_class>(query);
                return result;
            }
            else
            {
                String query = @"SELECT distinct a.class_id,b.class_name FROM mst_attendance a,mst_class b 
                            where 
                            a.class_id = b.class_id
                            and
                            a.user_id = @user_id";

                var result = con.Query<mst_class>(query, new { user_id = user_id });
                return result;
            }
            
        }

        public IEnumerable<mst_class> AllClassListWithSection()
        {
            String query = @"SELECT b.section_id class_id,concat(ifnull(a.class_name,''),' Section ',ifnull(b.section_name,'')) class_name FROM mst_class a,mst_section b
                            where
                            a.class_id = b.class_id
                            order by class_name";

            var result = con.Query<mst_class>(query);

            return result;
        }


        public mst_class FindClass(int? id)
        {
            String Query = "SELECT class_id,class_name FROM mst_class where class_id = @class_id";

            return con.Query<mst_class>(Query, new { class_id = id }).SingleOrDefault();
        }

        public void EditClass(mst_class mst)
        {

            try
            {
                string query = "UPDATE mst_class SET class_name = @class_name WHERE class_id = @class_id";

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
                String Query = "DELETE FROM mst_class WHERE class_id = @class_id";

                return con.Query<mst_class>(Query, new { class_id = id }).SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}