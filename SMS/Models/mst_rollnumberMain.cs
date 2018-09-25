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



            string query = @" select b.class_id,b.section_id,a.sr_number sr_num,concat(ifnull(a.std_first_name, ''), ' ', ifnull(std_last_name, '')) std_name from sr_register a, mst_section b
                            where
                            a.std_section_id = b.section_id
                            and
                            b.section_id = @section_id
                            and
                            b.class_id = @class_id
                            and
                            b.session = @session
                            and 
                            a.sr_number not in (select sr_num from mst_rollnumber where session = @session and class_id = @class_id and section_id = @section_id)
                            and 
                            a.std_active = 'Y'
                            order by std_name";

          
            return con.Query<mst_rollnumber>(query, new { class_id = class_id, section_id = section_id,session =sess });
        }

        public int max_roll_number(int class_id,int section_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findActive_finalSession();

            string query = @"select ifnull(max(roll_number),0) from mst_rollnumber where session = @session and class_id = @class_id and section_id = @section_id";

            var result = con.Query<int>(query, new { class_id = class_id,session = sess,section_id=section_id }).SingleOrDefault();

            return result;
        }

        public IEnumerable<mst_attendance> assign_class_list(int user_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findActive_finalSession();

            string query = @"select b.class_id,b.class_name,a.section_id,c.section_name from mst_attendance a,mst_class b,mst_section c 
                                where a.user_id = @user_id
                                and a.class_id =b.class_id 
                                and c.session = @session
                                and a.section_id = c.section_id
                                and c.section_id in (select distinct b.section_id from sr_register a, mst_section b
														where
														a.std_section_id = b.section_id
														and
                                                        a.std_active = 'Y'
                                                        and
														b.session = @session
														and
														a.sr_number not in (select sr_num from mst_rollnumber))";

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
                            `roll_number`,
                            `class_id`,
                            `section_id`)
                            VALUES
                            (@session,
                            @sr_num,
                            @roll_number,
                            @class_id,
                            @section_id)";
               


                con.Query<mst_rollnumber>(query, new {session = sess,sr_num = li.sr_num,roll_number=li.roll_number,class_id = li.class_id,section_id=li.section_id });


                query = @"SELECT count(*) FROM attendance_register where sr_num = @sr_num and session = @session";

                int count = con.Query<int>(query, new {sr_num = li.sr_num, session = sess }).SingleOrDefault();

                if(count>0)
                {
                    query = @"UPDATE attendance_register
                            SET
                            `roll_no` = @roll_no
                            WHERE `session` = @session AND `sr_num` = @sr_num AND `roll_no` != 0 AND `class_id` = @class_id AND `section_id` = @section_id";
                    con.Execute(query, new { sr_num = li.sr_num, session = sess,roll_no = li.roll_number,class_id=li.class_id,section_id=li.section_id });
                }

            }



        }

    }
}