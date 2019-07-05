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
        

        public void AddRemark(List<teacher_exam_remark> mst)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    mst_sessionMain sess = new mst_sessionMain();

                    string session = sess.findFinal_Session();

                    string query = @"INSERT INTO `teacher_exam_remark`
                                (`session`,
                                `term_id`,
                                `sr_number`,
                                `remark`,
                                `user_id`)
                                VALUES
                                (@session,
                                @term_id,
                                @sr_number,
                                @remark,
                                @user_id)";

                    string update = @"UPDATE `teacher_exam_remark` 
                                    SET 
                                        `user_id` = @user_id,
                                        `remark` = @remark
                                    WHERE
                                        `session` = @session
                                            AND `term_id` = @term_id
                                            AND `sr_number` = @sr_number";

                    string query1 = @"SELECT 
                                        COUNT(*)
                                    FROM
                                        `teacher_exam_remark`
                                    WHERE
                                        `session` = @session
                                            AND `term_id` = @term_id
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
                                remark.sr_number,
                                remark.remark,
                                remark.user_id
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

        public IEnumerable<teacher_exam_remark> student_list_for_remark(int class_id, int section_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain sess = new mst_sessionMain();

                string session_name = sess.findActive_finalSession();

                string query = @"SELECT 
                                b.class_id,
                                b.section_id,
                                c.roll_number roll_no,
                                a.sr_number,
                                CONCAT(IFNULL(a.std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_rollnumber c,
                                mst_std_class d,
                                mst_std_section e
                            WHERE
                                a.sr_number = d.sr_num
                                    AND d.sr_num = e.sr_num
                                    AND e.sr_num = c.sr_num
                                    AND e.section_id = b.section_id
                                    AND d.class_id = b.class_id
                                    AND b.section_id = @section_id
                                    AND b.class_id = @class_id
                                    AND d.class_id = b.class_id
                                    AND e.section_id = b.section_id
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = e.session
                                    AND e.session = @session
                                    AND a.std_active = 'Y'
                            ORDER BY roll_no";

                return con.Query<teacher_exam_remark>(query, new { class_id = class_id, section_id = section_id, session = session_name });
            }
        }

        public IEnumerable<teacher_exam_remark> FindRemarks(int class_id, int section_id, int term_id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                mst_sessionMain session = new mst_sessionMain();

                string Query = @"SELECT 
                                    *
                                FROM
                                    (SELECT 
                                        d.class_id,
                                            e.section_id,
                                            b.roll_number roll_no,
                                            a.remark,
                                            a.sr_number,
                                            CONCAT(IFNULL(c.std_first_name, ''), ' ', IFNULL(c.std_last_name, '')) std_name
                                    FROM
                                        teacher_exam_remark a, mst_rollnumber b, sr_register c, mst_std_class d, mst_std_section e
                                    WHERE
                                        a.sr_number = b.sr_num
                                            AND b.sr_num = c.sr_number
                                            AND c.sr_number = d.sr_num
                                            AND d.sr_num = e.sr_num
                                            AND a.session = b.session
                                            AND b.session = d.session
                                            AND d.session = e.session
                                            AND e.session = @session
                                            AND d.class_id = @class_id
                                            AND e.section_id = @section_id
                                            AND a.term_id = @term_id UNION ALL SELECT 
                                        b.class_id,
                                            b.section_id,
                                            c.roll_number roll_no,
                                            '',
                                            a.sr_number,
                                            CONCAT(IFNULL(a.std_first_name, ''), ' ', IFNULL(std_last_name, '')) std_name
                                    FROM
                                        sr_register a, mst_section b, mst_rollnumber c, mst_std_class d, mst_std_section e
                                    WHERE
                                        e.section_id = b.section_id
                                            AND d.class_id = b.class_id
                                            AND a.sr_number = c.sr_num
                                            AND c.sr_num = d.sr_num
                                            AND d.sr_num = e.sr_num
                                            AND b.session = c.session
                                            AND c.session = d.session
                                            AND d.session = e.session
                                            AND e.session = @session
                                            AND b.section_id = @section_id
                                            AND b.class_id = @class_id
                                            AND a.sr_number = c.sr_num
                                            AND a.std_active = 'Y'
                                            AND sr_number NOT IN (SELECT 
                                                sr_number
                                            FROM
                                                teacher_exam_remark a, mst_std_class b, mst_std_section c
                                            WHERE
                                                a.session = b.session
                                                    AND b.session = c.session
                                                    AND c.session = @session
                                                    AND a.sr_number = b.sr_num
                                                    AND b.sr_num = c.sr_num
                                                    AND a.term_id = @term_id
                                                    AND b.class_id = @class_id
                                                    AND c.section_id = @section_id)) a
                                ORDER BY a.roll_no";

                return con.Query<teacher_exam_remark>(Query, new { class_id = class_id, term_id = term_id, session = session.findFinal_Session(), section_id = section_id });
            }
        }
    }
}