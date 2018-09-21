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
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddDisciplineGrades(List<mst_discipline_grades> mst)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session = sess.findActive_finalSession();

                string query = @"INSERT INTO `mst_discipline_grades`
                                (`session`,
                                `sr_num`,
                                `term_id`,
                                `discipline_id`,
                                `class_id`,
                                `section_id`,
                                `grade`,
                                `user_id`)
                                VALUES
                                (@session,
                                @sr_num,
                                @term_id,
                                @discipline_id,
                                @class_id,
                                @section_id,
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
                                            AND `discipline_id` = @discipline_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id";

                string query1 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        `mst_discipline_grades`
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `term_id` = @term_id
                                            AND `discipline_id` = @discipline_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id";

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
                            marks.class_id,
                            marks.section_id,
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
                            marks.class_id,
                            marks.section_id,
                            marks.grade,
                            marks.user_id
                        });
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
            mst_sessionMain sess = new mst_sessionMain();

            string session_name = sess.findActive_finalSession();

            string query = @"select b.class_id,b.section_id,c.roll_number roll_no,a.sr_number sr_num,concat(ifnull(a.std_first_name, ''), ' ', ifnull(std_last_name, '')) std_name from sr_register a, mst_section b,mst_rollnumber c
                            where
                            a.std_section_id = b.section_id
                            and
                            b.section_id = @section_id
                            and
                            b.class_id = @class_id
                            and
                            c.class_id = b.class_id
                            and
                            c.section_id = b.section_id
                            and
                            b.session = c.session
                            and
                            c.session = @session
                            and
                            a.sr_number = c.sr_num
                            order by roll_no";

            return con.Query<mst_discipline_grades>(query, new { class_id = class_id, section_id = section_id, session = session_name });
        }

        public IEnumerable<mst_discipline_grades> AllDisciplineList()
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT a.session,a.class_id,a.term_id,c.class_name,b.term_name FROM mst_discipline_grades a, mst_discipline b, mst_class c
                                where
                                a.class_id = c.class_id
                                and
                                a.discipline_id = b.discipline_id
                                and
                                a.session = @session";

            var result = con.Query<mst_discipline_grades>(query, new { session = sess.findActive_finalSession() });

            return result;
        }


        public IEnumerable<mst_discipline_grades> FindDisciplineGrades(int class_id, int section_id, int discipline_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string Query = @"SELECT 
                                a.class_id,
                                a.section_id,
                                b.roll_number roll_no,
                                a.grade,
                                a.sr_num,
                                CONCAT(IFNULL(c.std_first_name, ''),
                                        ' ',
                                        IFNULL(c.std_last_name, '')) std_name
                            FROM
                                mst_discipline_grades a,
                                mst_rollnumber b,
                                sr_register c
                            WHERE
                                a.sr_num = b.sr_num
                                    AND b.sr_num = c.sr_number
                                    AND a.sr_num = c.sr_number
                                    AND a.session = b.session
                                    AND a.session = @session
                                    AND a.class_id = @class_id
                                    AND a.section_id = @section_id
                                    AND a.discipline_id = @discipline_id
                            ORDER BY roll_number";

            return con.Query<mst_discipline_grades>(Query, new { class_id = class_id, discipline_id = discipline_id,section_id = section_id ,session = session.findActive_finalSession() });
        }

        public mst_discipline_grades DeleteDisciplineGrades(int class_id, int discipline_id, string session)
        {
            String Query = @"DELETE FROM `mst_discipline_grades`
                            WHERE class_id = @class_id
                            and
                            discipline_id = @discipline_id
                            and
                            session = @session";

            return con.Query<mst_discipline_grades>(Query, new { class_id = class_id, discipline_id = discipline_id, session = session }).SingleOrDefault();
        }
    }
}