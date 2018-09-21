using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_coscholastic_gradesMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddCoscholasticGrades(List<mst_coscholastic_grades> mst)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session = sess.findActive_finalSession();

                string query = @"INSERT INTO `mst_coscholastic_grades`
                                (`session`,
                                `sr_num`,
                                `term_id`,
                                `co_scholastic_id`,
                                `class_id`,
                                `section_id`,
                                `grade`,
                                `user_id`)
                                VALUES
                                (@session,
                                @sr_num,
                                @term_id,
                                @co_scholastic_id,
                                @class_id,
                                @section_id,
                                @grade,
                                @user_id)";

                string update = @"UPDATE `mst_coscholastic_grades` 
                                    SET 
                                        `grade` = @grade,
                                        `user_id` = @user_id
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `term_id` = @term_id
                                            AND `co_scholastic_id` = @co_scholastic_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id";

                string query1 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        `mst_coscholastic_grades`
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `term_id` = @term_id
                                            AND `co_scholastic_id` = @co_scholastic_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id";

                foreach (var marks in mst)
                {

                    marks.session = session;

                    int count = con.Query<int>(query1, new {session = marks.session,sr_num = marks.sr_num,term_id = marks.term_id, co_scholastic_id = marks.co_scholastic_id,class_id = marks.class_id,section_id = marks.section_id }).SingleOrDefault();

                    if (count > 0)
                    {
                        con.Execute(update, new
                        {
                            marks.session,
                            marks.sr_num,
                            marks.term_id,
                            marks.co_scholastic_id,
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
                            marks.co_scholastic_id,
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

        public IEnumerable<mst_coscholastic_grades> student_list_for_Coscholastic_Grades(int class_id, int section_id)
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

            return con.Query<mst_coscholastic_grades>(query, new { class_id = class_id, section_id = section_id, session = session_name });
        }

        public IEnumerable<mst_coscholastic_grades> AllCoscholasticGradesList()
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT a.session,a.class_id,a.exam_id,c.class_name,b.exam_name FROM mst_coscholastic_grades a, mst_co_scholastic b, mst_class c
                                where
                                a.class_id = c.class_id
                                and
                                a.co_scholastic_id = b.co_scholastic_id
                                and
                                a.session = @session";

            var result = con.Query<mst_coscholastic_grades>(query, new { session = sess.findActive_finalSession() });

            return result;
        }


        public IEnumerable<mst_coscholastic_grades> FindCoscholasticGrades(int class_id,int section_id ,int co_scholastic_id)
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
                                mst_coscholastic_grades a,
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
                                    AND a.co_scholastic_id = @co_scholastic_id
                            ORDER BY roll_number";

            return con.Query<mst_coscholastic_grades>(Query, new { class_id = class_id, co_scholastic_id = co_scholastic_id, session = session.findActive_finalSession(), section_id = section_id });
        }

        public mst_coscholastic_grades DeleteCoscholasticGrades(int class_id, int co_scholastic_id, string session)
        {
            String Query = @"DELETE FROM `mst_coscholastic_grades`
                            WHERE class_id = @class_id
                            and
                            co_scholastic_id = @co_scholastic_id
                            and
                            session = @session";

            return con.Query<mst_coscholastic_grades>(Query, new { class_id = class_id, co_scholastic_id = co_scholastic_id, session = session }).SingleOrDefault();
        }
    }
}