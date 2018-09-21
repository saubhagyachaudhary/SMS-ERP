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
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddExamMarks(List<mst_exam_marks> mst)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session = sess.findActive_finalSession();

                string query = @"INSERT INTO `mst_exam_marks`
                                        (`session`,
                                        `sr_num`,
                                        `exam_id`,
                                        `subject_id`,
                                        `user_id`,
                                        `marks_assigned_user_id`,
                                        `marks`,
                                        `present`,
                                        `class_id`,
                                        `section_id`)
                                        VALUES
                                        (@session,
                                        @sr_num,
                                        @exam_id,
                                        @subject_id,
                                        @user_id,
                                        @marks_assigned_user_id,
                                        @marks,
                                        @present,
                                        @class_id,
                                        @section_id)";

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
                                            AND `subject_id` = @subject_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id";

                string query2 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        mst_exam_marks
                                    WHERE
                                        `session` = @session
                                            AND `sr_num` = @sr_num
                                            AND `exam_id` = @exam_id
                                            AND `subject_id` = @subject_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id";

                foreach (var marks in mst)
                {

                    marks.session = session;

                    int count = con.Query<int>(query2,new { session = marks.session,sr_num = marks.sr_num,exam_id = marks.exam_id, subject_id = marks.exam_id, class_id = marks.class_id, section_id = marks.section_id }).SingleOrDefault();

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
                            marks.present,
                            marks.class_id,
                            marks.section_id
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
                            marks.present,
                            marks.class_id,
                            marks.section_id
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<mst_exam_marks> student_list_for_marks_update(int subject_id,int class_id, int section_id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string session_name = sess.findActive_finalSession();

            string query = @"SELECT 
                            a.class_id,
                            a.section_id,
                            c.roll_number roll_no,
                            a.sr_num,
                            CONCAT(IFNULL(b.std_first_name, ''),
                                    ' ',
                                    IFNULL(b.std_last_name, '')) std_name,
	                        a.marks,
                            a.present
                        FROM
                            mst_exam_marks a,
                            sr_register b,
                            mst_rollnumber c
                        WHERE
                            a.exam_id = 1 AND a.session = @session
                                AND a.subject_id = @subject_id
                                AND a.class_id = @class_id
                                AND a.section_id = @section_id
                                AND a.sr_num = b.sr_number
                                AND b.sr_number = c.sr_num
                                AND a.session = c.session
                                order by c.roll_number";

            return con.Query<mst_exam_marks>(query, new { subject_id = subject_id, class_id = class_id, section_id = section_id, session = session_name });
        }

        public IEnumerable<mst_exam_marks> student_list_for_marks(int class_id, int section_id)
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

            return con.Query<mst_exam_marks>(query, new { class_id = class_id, section_id = section_id, session = session_name });
        }

        public IEnumerable<mst_exam_marks> find_marks(int exam_id,int subject_id,int class_id,int section_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string Query = @"SELECT 
                                *
                            FROM
                                mst_exam_marks
                            WHERE
                                exam_id = 1 AND session = @session
                                    AND subject_id = @subject_id
                                    AND class_id = @class_id
                                    AND section_id = @section_id";

            return con.Query<mst_exam_marks>(Query, new { class_id = class_id,section_id = section_id, exam_id = exam_id,subject_id = subject_id ,session = session.findActive_finalSession() });
        }

        public IEnumerable<mst_exam_class> AllExamClassList()
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT a.session,a.class_id,a.exam_id,c.class_name,b.exam_name FROM mst_exam_class a, mst_exam b, mst_class c
                                where
                                a.class_id = c.class_id
                                and
                                a.exam_id = b.exam_id
                                and
                                a.session = @session";

            var result = con.Query<mst_exam_class>(query, new { session = sess.findActive_finalSession() });

            return result;
        }


        public mst_exam_class FindExamClass(int class_id, int exam_id, string session)
        {
            String Query = @"SELECT a.session,a.class_id,a.exam_id,c.class_name,b.exam_name FROM mst_exam_class a, mst_exam b, mst_class c
                                where
                                a.class_id = c.class_id
                                and
                                a.exam_id = b.exam_id
                                and
                                a.session = @session
                                and
                                a.class_id = @class_id
                                and
                                a.exam_id = @exam_id";

            return con.Query<mst_exam_class>(Query, new { class_id = class_id, exam_id = exam_id, session = session }).SingleOrDefault();
        }

        public mst_exam_class DeleteExamClass(int class_id, int exam_id, string session)
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