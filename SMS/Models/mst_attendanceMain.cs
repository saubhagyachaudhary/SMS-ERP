using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_attendanceMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void Assign_faculty(mst_attendance mst)
        {
            try
            {
               
                string query = @"INSERT INTO `mst_attendance`
                                (`user_id`,
                                `class_id`,
                                `section_id`,
                                `finalizer`)
                                VALUES
                                (@user_id,
                                @class_id,
                                @section_id,
                                @finalizer_user_id)";

                con.Execute(query, new
                {
                   mst.user_id,mst.class_id,mst.finalizer_user_id,mst.section_id
                });

            }
            catch
            {
                throw;
            }
        }

        public IEnumerable<mst_attendance> assignList()
        {
            String query = @"select a.user_id,a.class_id,a.section_id,e.section_name,concat(c.FirstName,' ',c.LastName) faculty_name,b.class_name,a.finalizer finalizer_user_id,concat(d.FirstName,' ',d.LastName) finalizer_name from mst_attendance a,mst_class b, emp_profile c,emp_profile d,mst_section e
                                where
                                a.class_id = b.class_id
                                and
                                a.user_id = c.user_id
                                and
                                a.finalizer = d.user_id
                                and
                                a.section_id = e.section_id";

            var result = con.Query<mst_attendance>(query);

            return result;
        }

        public IEnumerable<mst_attendance> Attendance_class_list(int user_id)
        {
            String query = @"select b.class_id,b.class_name,a.section_id,c.section_name from mst_attendance a,mst_class b,mst_section c 
                                where a.user_id = @user_id  
                                and a.class_id =b.class_id 
                                and a.section_id not in (SELECT distinct section_id FROM attendance_register where att_date = date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE )))
                                and a.section_id = c.section_id";

            var result = con.Query<mst_attendance>(query,new {user_id = user_id});

            return result;
        }

        public void deleteFaculty(int user_id,int class_id,int finalizer_user_id,int section_id)
        {
            String query = @"DELETE FROM `mst_attendance`
                            WHERE user_id = @user_id and class_id = @class_id and finalizer = @finalizer_user_id and section_id = @section_id";

           con.Query(query,new {user_id=user_id,class_id=class_id, finalizer_user_id= finalizer_user_id,section_id=section_id });

            
        }

        public mst_attendance FindFaculty(int user_id,int class_id,int finalizer_user_id,int section_id)
        {
            String query = @"select a.user_id,a.class_id, e.section_name ,concat(c.FirstName,' ',c.LastName) faculty_name,b.class_name,a.finalizer finalizer_user_id,concat(d.FirstName,' ',d.LastName) finalizer_name from mst_attendance a,mst_class b, emp_profile c,emp_profile d,mst_section e
                                where
                                a.class_id = b.class_id
                                and
                                a.user_id = c.user_id
                                and
                                a.finalizer = d.user_id
                                and
								a.class_id = @class_id
								and
								a.user_id = @user_id
								and
								a.finalizer = @finalizer_user_id
                                and 
                                a.section_id = e.section_id
                                and
                                a.section_id = @section_id";



            return con.Query<mst_attendance>(query, new {user_id = user_id, class_id = class_id, finalizer_user_id= finalizer_user_id,section_id = section_id }).SingleOrDefault();

        }
    }
}