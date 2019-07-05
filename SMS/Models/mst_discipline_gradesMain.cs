using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_discipline_gradesMain
    {
        

        public void AddDisciplineGrades(List<mst_discipline_grades> mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();

                    string session = sess.findFinal_Session();

                    string query = @"INSERT INTO `mst_discipline_grades`
                                (`session`,
                                `sr_num`,
                                `term_id`,
                                `discipline_id`,
                                `grade`,
                                `user_id`)
                                VALUES
                                (@session,
                                @sr_num,
                                @term_id,
                                @discipline_id,
                                @grade,
                                @user_id)";

                    string update = @"UPDATE `mst_discipline_grades` 
                                    SET 
                                        `grade` = @grade,
                                        `user_id` = @user_id
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `term_id` = @term_id
                                            AND `discipline_id` = @discipline_id";

                    string query1 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        `mst_discipline_grades`
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `term_id` = @term_id
                                            AND `discipline_id` = @discipline_id";

                    foreach (var marks in mst)
                    {

                        marks.session = session;

                        int count = con.Query<int>(query1, new { session = marks.session, sr_num = marks.sr_num, term_id = marks.term_id, discipline_id = marks.discipline_id, class_id = marks.class_id, section_id = marks.section_id }).SingleOrDefault();

                        if (count > 0)
                        {
                            con.Execute(update, new
                            {
                                marks.session,
                                marks.sr_num,
                                marks.term_id,
                                marks.discipline_id,
                                marks.grade,
                                marks.user_id
                            });
                        }
                        else
                        {

                            con.Execute(query, new
                            {
                                marks.session,
                                marks.sr_num,
                                marks.term_id,
                                marks.discipline_id,
                                marks.grade,
                                marks.user_id
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_discipline_grades> student_list_for_Discipline_Grades(int class_id, int section_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session_name = sess.findFinal_Session();

                string query = @"SELECT 
                                b.class_id,
                                b.section_id,
                                c.roll_number roll_no,
                                a.sr_number sr_num,
                                CONCAT(IFNULL(a.std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_rollnumber c,
                                mst_std_section e,
                                mst_std_class f
                            WHERE
                                a.sr_number = e.sr_num
                                    AND e.sr_num = f.sr_num
                                    AND f.sr_num = c.sr_num
                                    AND e.section_id = b.section_id
                                    AND b.section_id = @section_id
                                    AND b.class_id = @class_id
                                    AND f.class_id = b.class_id
                                    AND e.section_id = b.section_id
                                    AND b.session = c.session
                                    AND c.session = e.session
                                    AND e.session = f.session
                                    AND f.session = @session
                            ORDER BY roll_no";

                return con.Query<mst_discipline_grades>(query, new { class_id = class_id, section_id = section_id, session = session_name });

            }
        }

        public IEnumerable<mst_discipline_grades> AllDisciplineList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

                string query = @"SELECT 
                                a.session,
                                d.class_id,
                                a.discipline_id,
                                c.class_name,
                                b.discipline_name
                            FROM
                                mst_discipline_grades a,
                                mst_discipline b,
                                mst_class c,
                                mst_std_class d
                            WHERE
                                a.sr_num = d.sr_num
                                    AND d.class_id = c.class_id
                                    AND a.discipline_id = b.discipline_id
                                    AND b.session = a.session
                                    AND a.session = c.session
                                    AND c.session = d.session
                                    AND d.session = @session";

                var result = con.Query<mst_discipline_grades>(query, new { session = sess.findFinal_Session() });

                return result;
            }
        }


        public IEnumerable<mst_discipline_grades> FindDisciplineGrades(int class_id, int section_id, int discipline_id, int term_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain session = new mst_sessionMain();

                string Query = @"SELECT 
                                d.class_id,
                                e.section_id,
                                b.roll_number roll_no,
                                a.grade,
                                a.sr_num,
                                CONCAT(IFNULL(c.std_first_name, ''),
                                        ' ',
                                        IFNULL(c.std_last_name, '')) std_name
                            FROM
                                mst_discipline_grades a,
                                mst_rollnumber b,
                                sr_register c,
                                mst_std_class d,
                                mst_std_section e
                            WHERE
                                a.sr_num = b.sr_num
                                    AND b.sr_num = c.sr_number
                                    AND c.sr_number = d.sr_num
                                    AND d.sr_num = e.sr_num
                                    AND a.session = b.session
                                    AND b.session = d.session
                                    AND d.session = e.session
                                    AND e.session = @session
                                    AND d.class_id = @class_id
                                    AND e.section_id = @section_id
                                    AND a.discipline_id = @discipline_id
                                     AND a.term_id = @term_id
                            ORDER BY roll_number";

                return con.Query<mst_discipline_grades>(Query, new { class_id = class_id, discipline_id = discipline_id, section_id = section_id, session = session.findFinal_Session(), term_id = term_id });
            }
        }

        public mst_discipline_grades DeleteDisciplineGrades(int class_id, int discipline_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"DELETE FROM `mst_discipline_grades`
                            WHERE
                            discipline_id = @discipline_id
                            and
                            session = @session";

                return con.Query<mst_discipline_grades>(Query, new { class_id = class_id, discipline_id = discipline_id, session = session }).SingleOrDefault();
            }
        }
    }
}