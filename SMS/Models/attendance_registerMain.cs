using Dapper;
using MySql.Data.MySqlClient;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace SMS.Models
{
    public class attendance_registerMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
        int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);
        string SchoolName = ConfigurationManager.AppSettings["SchoolName"].ToString();

        public async Task mark_attendance(List<attendance_register> attendance,int user_id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string session_name = sess.findActive_finalSession();

            int class_id = 0;

            int section_id = 0;

            int month_no = 0;

            DateTime att_date = DateTime.Now.Date;

            if (sess.checkSessionNotExpired())
            {
                string query = @"INSERT INTO attendance_register
                                (`session`,
                                `user_id`,
                                `att_date`,
                                `class_id`,
                                `section_id`,
                                `sr_num`,
                                `roll_no`,
                                `attendance`)
                                VALUES
                                (@session,
                                @user_id,
                                @att_date,
                                @class_id,
                                @section_id,
                                @sr_num,
                                @roll_no,
                                @attendance)";
                foreach (attendance_register att in attendance)
                {
                    att.session = session_name;
                    att.user_id = user_id;
                    att.att_date = System.DateTime.Now.AddMinutes(dateTimeOffSet);
                    await con.ExecuteAsync(query,
                               new
                               {
                                   att.session,
                                   att.user_id,
                                   att.att_date,
                                   att.class_id,
                                   att.section_id,
                                   att.sr_num,
                                   att.roll_no,
                                   att.attendance
                               });
                    class_id = att.class_id;
                    section_id = att.section_id;
                    month_no = att.att_date.Month;
                    att_date = att.att_date.Date;
                }

                repAttendance_sheetMain sendAttSheet = new repAttendance_sheetMain();

                 query = @"select c.Email from mst_attendance a,emp_profile b,emp_profile c
                            where
                            a.user_id = b.user_id
                            and
                            a.class_id = @class_id
                            and 
                            a.user_id = @user_id
                            and
                            a.section_id = @section_id
                            and
                            a.finalizer = c.user_id";

                string email_id = con.Query<string>(query, new { class_id = class_id,user_id = user_id,section_id = section_id }).SingleOrDefault();

                sendAttSheet.MailAttendanceSheet(section_id, month_no, session_name, email_id, att_date);
            }

        }

        public IEnumerable<attendance_register> student_list_for_attendance(int class_id,int section_id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string session_name = sess.findActive_finalSession();

            string query = @"select b.class_id,b.section_id,c.roll_number roll_no,a.sr_number sr_num,concat(ifnull(a.std_first_name, ''), ' ', ifnull(std_last_name, '')) std_name,1 attendance from sr_register a, mst_section b,mst_rollnumber c
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

            return con.Query<attendance_register>(query, new { class_id = class_id, section_id= section_id,session = session_name });
        }

        public IEnumerable<attendance_register> find_attendance_sheet_for_finalize(int section_id, DateTime att_date, string session)
        {
            string query = @"select a.session,a.att_date,a.roll_no,a.class_id,a.section_id,a.sr_num,concat(ifnull(b.std_first_name,''),' ',ifnull(b.std_last_name,'')) std_name,attendance from attendance_register a, sr_register b
                                where 
                                a.sr_num = b.sr_number
                                and 
                                a.section_id = @section_id
                                and 
                                a.session = @session
                                and 
                                a.att_date = @att_date
                                and 
                                ifnull(a.finalize,0) = 0
                                order by roll_no";

            return con.Query<attendance_register>(query, new { section_id = section_id, session = session,att_date = att_date });

        }

        public async Task finalize_attendance(List<attendance_register> attendance)
        {
                string query = @"UPDATE attendance_register
                                SET
                                `attendance` = @attendance,
                                `finalize` = 1
                                WHERE `session` = @session AND `att_date` = @att_date AND `class_id` = @class_id and sr_num = @sr_num and section_id = @section_id and roll_no = @roll_no";

           

            
            

            foreach (attendance_register att in attendance)
            {

                await con.ExecuteAsync(query,
                           new
                           {
                               att.session,
                               att.att_date,
                               att.class_id,
                               att.attendance,
                               att.sr_num,
                               att.section_id,
                               att.roll_no
                           });

                if (!att.attendance)
                {
                    sr_register std = new sr_register();
                    string phone_query = @"select coalesce(std_contact,std_contact1,std_contact2) std_contact,concat(ifnull(std_first_name,''),' ',ifnull(std_last_name,' ')) std_first_name from sr_register where sr_number = @sr_number";
                    std = con.Query<sr_register>(phone_query, new { sr_number = att.sr_num }).SingleOrDefault();


                    SMSMessage sms = new SMSMessage();

                    foreach (var item in sms.smsbody("absent"))
                    {
                        string body = item.Replace("#student_name#", std.std_first_name);

                        body = body.Replace("#current_date#", att.att_date.ToString("dd/MM/yyyy"));

                        await sms.SendSMS(body, std.std_contact);
                    }

                    

                    //SMSMessage sms = new SMSMessage();

                    //string message = @"Dear Parents,This is to inform you that "+ std.std_first_name +" is absent on "+att.att_date.ToString("dd/MM/yyyy")+".Thank you. "+SchoolName;

                    //sms.SendSMS(message, std.std_contact);

                    //SMSMessage sms1 = new SMSMessage();

                    //string message1 = @"प्रिय अभिभावक, आपको सूचित किया जाता है कि "+ std.std_first_name+ " दिनांक "+ att.att_date.ToString("dd/MM/yyyy") + " को स्कूल में अनुपस्थित है। धन्यवाद। "+ SchoolName;

                    //sms1.SendSMS(message1, std.std_contact);
                }
            }
                

            

        }
    }
}