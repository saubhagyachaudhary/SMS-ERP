using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_exam_marksMain
    {
        

        public void AddExamMarks(List<mst_exam_marks> mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();

                string session = sess.findFinal_Session();

                string query = @"INSERT INTO `mst_exam_marks`
                                        (`session`,
                                        `sr_num`,
                                        `exam_id`,
                                        `subject_id`,
                                        `user_id`,
                                        `marks_assigned_user_id`,
                                        `marks`,
                                        `present`)
                                        VALUES
                                        (@session,
                                        @sr_num,
                                        @exam_id,
                                        @subject_id,
                                        @user_id,
                                        @marks_assigned_user_id,
                                        @marks,
                                        @present)";

                string query1 = @"UPDATE `mst_exam_marks` 
                                    SET 
                                        `marks` = @marks,
                                        `present` = @present,
                                        `user_id` = @user_id,
                                        `marks_assigned_user_id` = @marks_assigned_user_id
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `exam_id` = @exam_id
                                            AND `subject_id` = @subject_id";

                string query2 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        mst_exam_marks
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `exam_id` = @exam_id
                                            AND `subject_id` = @subject_id";
                
                    foreach (var marks in mst)
                    {

                        marks.session = session;
                    
                        int count = con.Query<int>(query2, new { session = marks.session, sr_num = marks.sr_num, exam_id = marks.exam_id, subject_id = marks.subject_id, class_id = marks.class_id, section_id = marks.section_id }).SingleOrDefault();

                        if (count > 0)
                        {
                            con.Execute(query1, new
                            {
                                marks.session,
                                marks.sr_num,
                                marks.exam_id,
                                marks.subject_id,
                                marks.marks,
                                marks.user_id,
                                marks.marks_assigned_user_id,
                                marks.present
                            });
                        }
                        else
                        {
                            con.Execute(query, new
                            {
                                marks.session,
                                marks.sr_num,
                                marks.exam_id,
                                marks.subject_id,
                                marks.user_id,
                                marks.marks_assigned_user_id,
                                marks.marks,
                                marks.present
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

        public IEnumerable<mst_exam_marks> student_list_for_marks_update(int subject_id,int class_id, int section_id,int exam_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

            string session_name = sess.findFinal_Session();

            //string query = @"SELECT 
            //                    d.class_id,
            //                    e.section_id,
            //                    c.roll_number roll_no,
            //                    a.sr_num,
            //                    CONCAT(IFNULL(b.std_first_name, ''),
            //                            ' ',
            //                            IFNULL(b.std_last_name, '')) std_name,
            //                    a.marks,
            //                    a.present
            //                FROM
            //                    mst_exam_marks a,
            //                    sr_register b,
            //                    mst_rollnumber c,
            //                    mst_std_class d,
            //                    mst_std_section e
            //                WHERE
            //                    a.exam_id = @exam_id
            //                        AND e.session = @session
            //                        AND a.subject_id = @subject_id
            //                        AND d.class_id = @class_id
            //                        AND e.section_id = @section_id
            //                        AND a.sr_num = b.sr_number
            //                        AND b.sr_number = c.sr_num
            //                        AND a.session = c.session
            //                        AND c.session = d.session
            //                        AND d.session = e.session
            //                        AND a.sr_num = b.sr_number
            //                        AND b.sr_number = c.sr_num
            //                        AND c.sr_num = d.sr_num
            //                        AND d.sr_num = e.sr_num
            //                        AND b.std_active = 'Y'
            //                ORDER BY c.roll_number";

            string query = @"SELECT 
                                e.class_id,
                                d.section_id,
                                c.roll_number roll_no,
                                a.sr_number sr_num,
                                CONCAT(IFNULL(a.std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name,
                                (SELECT 
                                        MARKS
                                    FROM
                                        mst_exam_marks
                                    WHERE
                                        sr_num = a.sr_number
                                            AND session = e.session
                                            AND exam_id = @exam_id
                                            AND subject_id = @subject_id) marks,
                                ifnull((SELECT 
                                        present
                                    FROM
                                        mst_exam_marks
                                    WHERE
                                        sr_num = a.sr_number
                                            AND session = e.session
                                            AND exam_id = @exam_id
                                            AND subject_id = @subject_id),1) present
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_rollnumber c,
                                mst_std_section d,
                                mst_std_class e
                            WHERE
                                d.section_id = b.section_id
                                    AND d.section_id = @section_id
                                    AND e.class_id = @class_id
                                    AND e.class_id = b.class_id
                                    AND d.section_id = b.section_id
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = e.session
                                    AND e.session = @session
                                    AND a.sr_number = c.sr_num
                                    AND c.sr_num = d.sr_num
                                    AND d.sr_num = e.sr_num
                                    AND a.std_active = 'Y'
                            ORDER BY roll_no";
            
                return con.Query<mst_exam_marks>(query, new { subject_id = subject_id, class_id = class_id, section_id = section_id, session = session_name, exam_id = exam_id });
            }
        }

        public IEnumerable<mst_exam_marks> student_list_for_marks(int class_id, int section_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

            string session_name = sess.findFinal_Session();

            string query = @"SELECT 
                                e.class_id,
                                d.section_id,
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
                                d.section_id = b.section_id
                                    AND d.section_id = @section_id
                                    AND e.class_id = @class_id
                                    AND e.class_id = b.class_id
                                    AND d.section_id = b.section_id
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = e.session
                                    AND e.session = @session
                                    AND a.sr_number = c.sr_num
                                    AND c.sr_num = d.sr_num
                                    AND d.sr_num = e.sr_num
                                    AND a.std_active = 'Y'
                            ORDER BY roll_no";
            
                return con.Query<mst_exam_marks>(query, new { class_id = class_id, section_id = section_id, session = session_name });
            }
        }

        public IEnumerable<mst_exam_marks> find_marks(int exam_id,int subject_id,int class_id,int section_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain session = new mst_sessionMain();

            string Query = @"SELECT 
                                *
                            FROM
                                mst_exam_marks a,
                                mst_std_class b,
                                mst_std_section c
                            WHERE
                                exam_id = @exam_id
                                    AND a.session = @session
                                    AND a.subject_id = @subject_id
                                    AND b.class_id = @class_id
                                    AND c.section_id = @section_id
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND c.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                                    AND a.sr_num = b.sr_num
                                    AND b.sr_num = c.sr_num";
            
                return con.Query<mst_exam_marks>(Query, new { class_id = class_id, section_id = section_id, exam_id = exam_id, subject_id = subject_id, session = session.findFinal_Session() });
            }
        }

        public IEnumerable<mst_exam_class> AllExamClassList()
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                    a.session, a.class_id, a.exam_id, c.class_name, b.exam_name
                                FROM
                                    mst_exam_class a,
                                    mst_exam b,
                                    mst_class c
                                WHERE
                                    a.class_id = c.class_id
                                        AND a.exam_id = b.exam_id
                                        AND a.session = @session
                                        AND a.session = b.session
                                        AND b.session = c.session";
            
                var result = con.Query<mst_exam_class>(query, new { session = sess.findFinal_Session() });

                return result;
            }
        }


        public mst_exam_class FindExamClass(int class_id, int exam_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string Query = @"SELECT 
                                    a.session, a.class_id, a.exam_id, c.class_name, b.exam_name
                                FROM
                                    mst_exam_class a,
                                    mst_exam b,
                                    mst_class c
                                WHERE
                                    a.class_id = c.class_id
                                        AND a.exam_id = b.exam_id
                                        AND a.session = @session
                                        AND a.class_id = @class_id
                                        AND a.exam_id = @exam_id
                                        and a.session = b.session
                                        and b.session = c.session";
            
                return con.Query<mst_exam_class>(Query, new { class_id = class_id, exam_id = exam_id, session = session }).SingleOrDefault();
            }
        }

        public mst_exam_class DeleteExamClass(int class_id, int exam_id, string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                String Query = @"DELETE FROM `mst_exam_class`
                            WHERE class_id = @class_id
                            and
                            exam_id = @exam_id
                            and
                            session = @session";
            
                return con.Query<mst_exam_class>(Query, new { class_id = class_id, exam_id = exam_id, session = session }).SingleOrDefault();
            }
        }
    }
}