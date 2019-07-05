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
        

        public void AddCoscholasticGrades(List<mst_coscholastic_grades> mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();

                    string session = sess.findFinal_Session();

                    string query = @"INSERT INTO `mst_coscholastic_grades`
                                (`session`,
                                `sr_num`,
                                `term_id`,
                                `co_scholastic_id`,
                                `grade`,
                                `user_id`)
                                VALUES
                                (@session,
                                @sr_num,
                                @term_id,
                                @co_scholastic_id,
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
                                            AND `co_scholastic_id` = @co_scholastic_id";

                    string query1 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        `mst_coscholastic_grades`
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `term_id` = @term_id
                                            AND `co_scholastic_id` = @co_scholastic_id";

                    foreach (var marks in mst)
                    {

                        marks.session = session;

                        int count = con.Query<int>(query1, new { session = marks.session, sr_num = marks.sr_num, term_id = marks.term_id, co_scholastic_id = marks.co_scholastic_id, class_id = marks.class_id, section_id = marks.section_id }).SingleOrDefault();

                        if (count > 0)
                        {
                            con.Execute(update, new
                            {
                                marks.session,
                                marks.sr_num,
                                marks.term_id,
                                marks.co_scholastic_id,
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

        public IEnumerable<mst_coscholastic_grades> student_list_for_Coscholastic_Grades(int class_id, int section_id)
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
                                mst_std_section d,
                                mst_std_class e
                            WHERE
                                a.sr_number = c.sr_num
                                    AND c.sr_num = d.sr_num
                                    AND d.sr_num = e.sr_num
                                    AND d.section_id = b.section_id
                                    AND b.section_id = @section_id
                                    AND b.class_id = @class_id
                                    AND e.class_id = b.class_id
                                    AND d.section_id = b.section_id
                                    AND e.session = d.session
                                    AND d.session = b.session
                                    AND b.session = c.session
                                    AND c.session = @session
                                    AND a.sr_number = c.sr_num
                            ORDER BY roll_no";

                return con.Query<mst_coscholastic_grades>(query, new { class_id = class_id, section_id = section_id, session = session_name });
            }
        }

        //public IEnumerable<mst_coscholastic_grades> AllCoscholasticGradesList()
        //{

        //    mst_sessionMain sess = new mst_sessionMain();

        //    string query = @"SELECT a.session,a.class_id,a.exam_id,c.class_name,b.exam_name FROM mst_coscholastic_grades a, mst_co_scholastic b, mst_class c
        //                        where
        //                        a.class_id = c.class_id
        //                        and
        //                        a.co_scholastic_id = b.co_scholastic_id
        //                        and
        //                        a.session = @session";

        //    var result = con.Query<mst_coscholastic_grades>(query, new { session = sess.findActive_finalSession() });

        //    return result;
        //}


        public IEnumerable<mst_coscholastic_grades> FindCoscholasticGrades(int class_id, int section_id, int co_scholastic_id)
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
                                mst_coscholastic_grades a,
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
                                    AND a.session = @session
                                    AND d.class_id = @class_id
                                    AND e.section_id = @section_id
                                    AND a.co_scholastic_id = @co_scholastic_id
                            ORDER BY roll_number";

                return con.Query<mst_coscholastic_grades>(Query, new { class_id = class_id, co_scholastic_id = co_scholastic_id, session = session.findFinal_Session(), section_id = section_id });
            }
        }
        public mst_coscholastic_grades DeleteCoscholasticGrades(int class_id, int co_scholastic_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"DELETE FROM `mst_coscholastic_grades`
                            WHERE 
                            co_scholastic_id = @co_scholastic_id
                            and
                            session = @session";

                return con.Query<mst_coscholastic_grades>(Query, new { class_id = class_id, co_scholastic_id = co_scholastic_id, session = session }).SingleOrDefault();
            }
        }
    }
}