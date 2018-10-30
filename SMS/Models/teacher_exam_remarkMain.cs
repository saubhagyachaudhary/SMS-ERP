using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class teacher_exam_remarkMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddRemark(List<teacher_exam_remark> mst)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session = sess.findActive_finalSession();

                string query = @"INSERT INTO `teacher_exam_remark`
                                (`session`,
                                `term_id`,
                                `class_id`,
                                `section_id`,
                                `sr_number`,
                                `remark`,
                                `roll_no`,
                                `user_id`)
                                VALUES
                                (@session,
                                @term_id,
                                @class_id,
                                @section_id,
                                @sr_number,
                                @remark,
                                @roll_no,
                                @user_id)";

                string update = @"UPDATE `teacher_exam_remark` 
                                    SET 
                                        `user_id` = @user_id,
                                        `remark` = @remark
                                    WHERE
                                        `session` = @session
                                            AND `term_id` = @term_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id
                                            AND `sr_number` = @sr_number";

                string query1 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        `teacher_exam_remark`
                                    WHERE
                                        `session` = @session
                                            AND `term_id` = @term_id
                                            AND `class_id` = @class_id
                                            AND `section_id` = @section_id
                                            AND `sr_number` = @sr_number";

                foreach (var remark in mst)
                {

                    remark.session = session;

                    int count = con.Query<int>(query1, new { session = remark.session, sr_number = remark.sr_number, term_id = remark.term_id, class_id = remark.class_id, section_id = remark.section_id }).SingleOrDefault();

                    if (count > 0)
                    {
                        con.Execute(update, new
                        {
                            remark.session,
                            remark.sr_number,
                            remark.term_id,
                            remark.class_id,
                            remark.section_id,
                            remark.remark,
                            remark.user_id
                        });
                    }
                    else
                    {

                        con.Execute(query, new
                        {
                            remark.session,
                            remark.term_id,
                            remark.class_id,
                            remark.section_id,
                            remark.sr_number,
                            remark.remark,
                            remark.roll_no,
                            remark.user_id
                        });
                    }
                }
               
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public IEnumerable<teacher_exam_remark> student_list_for_remark(int class_id, int section_id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string session_name = sess.findActive_finalSession();

            string query = @"select b.class_id,b.section_id,c.roll_number roll_no,a.sr_number,concat(ifnull(a.std_first_name, ''), ' ', ifnull(std_last_name, '')) std_name from sr_register a, mst_section b,mst_rollnumber c
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
                            and
                            a.std_active = 'Y'
                            order by roll_no";

            return con.Query<teacher_exam_remark>(query, new { class_id = class_id, section_id = section_id, session = session_name });
        }

        public IEnumerable<teacher_exam_remark> FindRemarks(int class_id, int section_id, int term_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string Query = @"select * from (SELECT 
                                a.class_id,
                                a.section_id,
                                b.roll_number roll_no,
                                a.remark,
                                a.sr_number,
                                CONCAT(IFNULL(c.std_first_name, ''),
                                        ' ',
                                        IFNULL(c.std_last_name, '')) std_name
                            FROM
                                teacher_exam_remark a,
                                mst_rollnumber b,
                                sr_register c
                            WHERE
                                a.sr_number = b.sr_num
                                    AND b.sr_num = c.sr_number
                                    AND a.sr_number = c.sr_number
                                    AND a.session = b.session
                                    AND a.session = @session
                                    AND a.class_id = @class_id
                                    AND a.section_id = @section_id
                                    AND a.term_id = @term_id
                            UNION ALL 
                            SELECT 
                                b.class_id,
                                b.section_id,
                                c.roll_number roll_no,
                                '',
                                a.sr_number,
                                CONCAT(IFNULL(a.std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_rollnumber c
                            WHERE
                                a.std_section_id = b.section_id
                                    AND b.section_id = @section_id
                                    AND b.class_id = @class_id
                                    AND c.class_id = b.class_id
                                    AND c.section_id = b.section_id
                                    AND b.session = c.session
                                    AND c.session = @session
                                    AND a.sr_number = c.sr_num
                                    AND a.std_active = 'Y'
                                    AND sr_number NOT IN (SELECT 
                                        sr_number
                                    FROM
                                        teacher_exam_remark
                                    WHERE
                                        session = @session AND term_id = @term_id
                                            AND class_id = @class_id
                                            AND section_id = @section_id)) a
                                            order by a.roll_no";

            return con.Query<teacher_exam_remark>(Query, new { class_id = class_id, term_id = term_id, session = session.findActive_finalSession(), section_id = section_id });
        }
    }
}