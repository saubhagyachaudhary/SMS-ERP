using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using Dapper;

namespace SMS.Models
{
    public class mst_rollnumberMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<mst_rollnumber> student_list_for_rollnumber(int class_id, int section_id)
        {

            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findActive_finalSession();



            string query = @"SELECT 
                                    c.class_id,
                                    b.section_id,
                                    a.sr_number sr_num,
                                    CONCAT(IFNULL(a.std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, '')) std_name
                                FROM
                                    sr_register a,
                                    mst_std_section b,
                                    mst_std_class c
                                WHERE
                                    a.sr_number = b.sr_num
                                        AND b.sr_num = c.sr_num
                                        AND b.section_id = @section_id
                                        AND c.class_id = @class_id
                                        AND b.session = @session
                                        AND b.session = c.session
                                        AND a.sr_number NOT IN (SELECT 
                                            sr_num
                                        FROM
                                            mst_rollnumber
                                        WHERE
                                            session = @session
                                                AND class_id = @class_id
                                                AND section_id = @section_id)
                                        AND a.std_active = 'Y'
                                ORDER BY std_name";

          
            return con.Query<mst_rollnumber>(query, new { class_id = class_id, section_id = section_id,session =sess });
        }

        public int max_roll_number(int class_id,int section_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findActive_finalSession();

            string query = @"SELECT 
                                    IFNULL(MAX(roll_number), 0)
                                FROM
                                    mst_rollnumber a,
                                    mst_std_class b,
                                    mst_std_section c
                                WHERE
                                    a.session = b.session
                                        AND b.session = c.session
                                        AND c.session = @session
                                        AND a.sr_num = b.sr_num
                                        AND b.sr_num = c.sr_num
                                        AND b.class_id = @class_id
                                        AND c.section_id = @section_id";

            var result = con.Query<int>(query, new { class_id = class_id,session = sess,section_id=section_id }).SingleOrDefault();

            return result;
        }

        public IEnumerable<mst_attendance> assign_class_list(int user_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findActive_finalSession();

            string query = @"SELECT 
                                    b.class_id, b.class_name, a.section_id, c.section_name
                                FROM
                                    mst_attendance a,
                                    mst_class b,
                                    mst_section c
                                WHERE
                                    a.user_id = @user_id
                                        AND a.class_id = b.class_id
                                        AND c.session = @session
                                        AND a.section_id = c.section_id
                                        AND c.section_id IN (SELECT DISTINCT
                                            b.section_id
                                        FROM
                                            sr_register a,
                                            mst_std_section b
                                        WHERE
                                            a.sr_number = b.sr_num
                                                AND a.std_active = 'Y'
                                                AND b.session = @session
                                                AND a.sr_number NOT IN (SELECT 
                                                    sr_num
                                                FROM
                                                    mst_rollnumber))";

            var result = con.Query<mst_attendance>(query, new { user_id = user_id,session = sess });

            return result;
        }

        public void update_roll_no(List<mst_rollnumber> list)
        {
            string query = "";

            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findActive_finalSession();

           

            foreach (var li in list)
            {
                    query = @"INSERT INTO `mst_rollnumber`
                            (`session`,
                            `sr_num`,
                            `roll_number`)
                            VALUES
                            (@session,
                            @sr_num,
                            @roll_number)";
               


                con.Query<mst_rollnumber>(query, new {session = sess,sr_num = li.sr_num,roll_number=li.roll_number,class_id = li.class_id,section_id=li.section_id });

            }



        }

    }
}