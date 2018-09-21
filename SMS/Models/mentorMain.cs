using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mentorMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        public IEnumerable<mentor_header> Allmentor_header(string role,string mentor_id)
        {
            try
            {
                string query1 = @"SELECT fin_id
                              FROM mst_fin
                          where fin_close = 'N'";

                string fin = con.Query<string>(query1).SingleOrDefault();

                string query = "";

                if (role == "superadmin" || role == "principal")
                {
                    query = @"SELECT a.mentor_id,a.fin_id,a.mentor_no, a.mentor_date,a.sr_number sr_num,concat(b.std_first_name,' ',b.std_last_name) std_name,e.class_name,a.problem,concat(d.staff_first_name,' ',d.staff_last_name) mentor_name,a.dead_line 
                                FROM mentor_header a, sr_register b, mst_batch c,mst_staff d,mst_class e
                                where a.sr_number = b.sr_number
                                and b.std_batch_id = c.batch_id
                                and a.mentor_id = d.staff_id 
                                and e.class_id = c.class_id
                                and b.std_active = 'Y' 
                                and a.fin_id = @fin_id
                                and a.dead_line >= date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))";
                    return con.Query<mentor_header>(query, new { fin_id = fin });

                }
                else
                { 
               
                     query = @"SELECT a.mentor_id,a.fin_id,a.mentor_no, a.mentor_date,a.sr_number sr_num,concat(b.std_first_name,' ',b.std_last_name) std_name,e.class_name,a.problem,concat(d.staff_first_name,' ',d.staff_last_name) mentor_name,a.dead_line 
                                FROM mentor_header a, sr_register b, mst_batch c,mst_staff d,mst_class e
                                where a.sr_number = b.sr_number
                                and b.std_batch_id = c.batch_id
                                and a.mentor_id = d.staff_id 
                                and e.class_id = c.class_id
                                and b.std_active = 'Y' 
                                and a.fin_id = @fin_id
                                and a.mentor_id = @mentor_id
                                and a.dead_line >= date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))";

                    return con.Query<mentor_header>(query, new { fin_id = fin,mentor_id = mentor_id });

                }



            }
            catch(Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mentor_detail> Allmentor_detail(int mentor_no, string mentor_date)
        {

            string query1 = @"SELECT fin_id
                              FROM mst_fin
                          where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();

            String query = @"SELECT fin_id,serial_no,mentor_no,mentor_date,mentor_observation,parents_observation,observation_date FROM mentor_detail where fin_id = @fin_id and mentor_no = @mentor_no and mentor_date = @mentor_date";

            var result = con.Query<mentor_detail>(query, new {fin_id = fin_id,mentor_no = mentor_no,mentor_date = mentor_date });

            return result;
        }

        public void deleteObservation(int serial,int mentor_no, DateTime mentor_date)
        {

            string query1 = @"SELECT fin_id
                              FROM mst_fin
                          where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();

            String query = @"delete from mentor_detail where fin_id = @fin_id and serial_no = @serial and mentor_no = @mentor_no and mentor_date = @mentor_date";

            var result = con.Query(query, new { fin_id = fin_id,serial = serial ,mentor_no = mentor_no, mentor_date = mentor_date.ToString("yyyy-MM-dd") });

            
        }

        public mentor_detail findObservation(int serial_no, int mentor_no, DateTime mentor_date)
        {

            string query1 = @"SELECT fin_id
                              FROM mst_fin
                          where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();

            String query = @"select * from mentor_detail where fin_id = @fin_id and serial_no = @serial and mentor_no = @mentor_no and mentor_date = @mentor_date";

            var result = con.Query<mentor_detail>(query, new { fin_id = fin_id, serial = serial_no, mentor_no = mentor_no, mentor_date = mentor_date.ToString("yyyy-MM-dd") }).SingleOrDefault();

            return result;
        }

        public int check_record(int mentor_no, DateTime mentor_date,int mentor_id)
        {

            string query1 = @"SELECT fin_id
                              FROM mst_fin
                          where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();

            string query = @"select ifnull(count(mentor_no),0) from mentor_detail where observation_date = date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE )) and mentor_no = @mentor_no and mentor_date = @mentor_date and fin_id = @fin_id and mentor_id = @mentor_id";

            var result = con.Query<int>(query, new { fin_id = fin_id, mentor_no = mentor_no, mentor_date = mentor_date, mentor_id = mentor_id }).SingleOrDefault();

            return result;
        }

        public void addObsevation(mentor_detail mentor)
        {
            try
            {
                mentor_detail detail = new mentor_detail();

                string query1 = @"SELECT fin_id
                              FROM mst_fin
                          where fin_close = 'N'";

                string fin_id = con.Query<string>(query1).SingleOrDefault();

                string maxid = "select ifnull(max(serial_no),0)+1 from mentor_detail where fin_id = @fin_id and mentor_no = @mentor_no and mentor_date = @mentor_date";

                int max_no = con.ExecuteScalar<int>(maxid, new { fin_id = fin_id, mentor_no = mentor.mentor_no, mentor_date = mentor.mentor_date });

                string query = @"INSERT INTO `mentor_detail`
                                    (`fin_id`,
                                    `serial_no`,
                                    `mentor_no`,
                                    `mentor_date`,
                                     mentor_id,
                                    `mentor_observation`,
                                    `parents_observation`,
                                    `observation_date`)
                                    VALUES
                                    (@fin_id,
                                    @serial_no,
                                    @mentor_no,
                                    @mentor_date,
                                    @mentor_id,
                                    @mentor_observation,
                                    @parents_observation,
                                    @observation_date)";



                mentor.fin_id = fin_id;
                mentor.serial_no = max_no;
                mentor.observation_date = System.DateTime.Now.AddMinutes(dateTimeOffSet);
               
                con.Execute(query, new
                {
                    mentor.fin_id,
                    mentor.serial_no,
                    mentor.mentor_no,
                    mentor.mentor_date,
                    mentor.mentor_id,
                    mentor.mentor_observation,
                    mentor.parents_observation,
                    mentor.observation_date

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void assignMentor(mentor_header mentor)
        {
            try
            {

                string query1 = @"SELECT fin_id
                              FROM mst_fin
                          where fin_close = 'N'";

                string fin_id = con.Query<string>(query1).SingleOrDefault();

                string maxid = "select ifnull(max(mentor_no),0)+1 from mentor_header where fin_id = @fin_id";

                int max_no = con.ExecuteScalar<int>(maxid, new { fin_id= fin_id });

                string query2 = "select b.class_id from sr_register a, mst_batch b where a.std_batch_id = b.batch_id and a.sr_number =  @sr_num";

                int class_id = con.ExecuteScalar<int>(query2, new { sr_num = mentor.sr_num });

                string query = @"INSERT INTO `mentor_header`
                                (`fin_id`,
                                `mentor_no`,
                                `mentor_date`,
                                `sr_number`,
                                `class_id`,
                                `problem`,
                                `mentor_id`,
                                `dead_line`)
                                VALUES
                                (@fin_id,
                                @mentor_no,
                                @mentor_date,
                                @sr_num,
                                @class_id,
                                @problem,
                                @mentor_id,
                                @dead_line)";

               



                mentor.mentor_no = max_no;

                mentor.mentor_date = System.DateTime.Now.AddMinutes(dateTimeOffSet);

                mentor.fin_id = fin_id;

                mentor.class_id = class_id;

                con.Execute(query, new
                {
                    mentor.fin_id,
                    mentor.mentor_no,
                    mentor.mentor_date,
                    mentor.sr_num,
                    mentor.class_id,
                    mentor.problem,
                    mentor.mentor_id,
                    mentor.dead_line

                });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}