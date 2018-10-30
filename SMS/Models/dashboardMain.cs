using Dapper;
using MySql.Data.MySqlClient;
using SMS.job_scheduler;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace SMS.Models
{
    public class dashboardMain
    {
       
       

        public int school_strength()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            String query = @"select ifnull(count(sr_number),0) from sr_register where std_active = 'Y'";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

        public int school_Male_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            String query = @"select ifnull(count(sr_number),0) from sr_register where std_active = 'Y' and std_sex = 'M';";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

        public int school_Female_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            String query = @"select ifnull(count(sr_number),0) from sr_register where std_active = 'Y' and std_sex = 'F';";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

        public int pending_mentor()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query1 = @"SELECT fin_id
                             FROM mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            String query = @"select ifnull(count(mentor_no),0) from mentor_header where dead_line >= date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE )) and fin_id = @fin_id";

            var result = con.Query<int>(query, new { fin_id = fin }).SingleOrDefault();

            return result;
        }



        public decimal cash_received()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);


            string query1 = @"SELECT fin_id
                             FROM mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            string rect_date = System.DateTime.Now.AddMinutes(dateTimeOffSet).ToString("yyyy-MM-dd");

            String query = @"select ifnull(sum(amount),0)+ifnull(sum(dc_fine),0)-ifnull(sum(dc_discount),0) from fees_receipt where receipt_date = @dt and fin_id = @fin and mode_flag = 'Cash'";

            var result = con.Query<decimal>(query, new { dt = rect_date, fin = fin }).SingleOrDefault();

            return result;
        }

        public decimal bank_received()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);


            string query1 = @"SELECT fin_id
                             FROM mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            string rect_date = System.DateTime.Now.AddMinutes(dateTimeOffSet).ToString("yyyy-MM-dd");

            string query = @"select ifnull(sum(amount),0)+ifnull(sum(dc_fine),0)-ifnull(sum(dc_discount),0) from fees_receipt where receipt_date = @dt and fin_id = @fin and mode_flag = 'Cheque'";

            var result = con.Query<decimal>(query, new { dt = rect_date, fin = fin }).SingleOrDefault();

            return result;
        }

        public decimal total_cash_received()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            string query1 = @"SELECT fin_id
                             FROM mst_fin
                          where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();

            string query = @"SELECT 
                                    IFNULL(SUM(IFNULL(amount, 0)) + SUM(IFNULL(dc_fine, 0)) - SUM(IFNULL(dc_discount, 0)),
                                            0)
                                FROM
                                    fees_receipt
                                WHERE
                                    fin_id = fin_id
                                        AND mode_flag = 'Cash'
                                        AND IFNULL(chq_reject, 'Cleared') = 'Cleared'";

            var result = con.Query<decimal>(query, new { fin_id = fin_id }).SingleOrDefault();

            return result;
        }

        public decimal total_bank_received()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            string query1 = @"SELECT fin_id
                             FROM mst_fin
                          where fin_close = 'N'";

            string fin_id = con.Query<string>(query1).SingleOrDefault();

            string query = @"SELECT 
                                    IFNULL(SUM(IFNULL(amount, 0)) + SUM(IFNULL(dc_fine, 0)) - SUM(IFNULL(dc_discount, 0)),
                                            0)
                                FROM
                                    fees_receipt
                                WHERE
                                    fin_id = fin_id
                                        AND mode_flag = 'Cheque'
                                        AND IFNULL(chq_reject, 'Cleared') = 'Cleared'";

            var result = con.Query<decimal>(query, new { fin_id = fin_id }).SingleOrDefault();

            return result;
        }

        public int transport_Male_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            string query = @"select ifnull(count(sr_number),0) from sr_register where std_active = 'Y' and std_pickup_id != 1000 and std_sex = 'M'";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

        public int transport_Female_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            string query = @"select ifnull(count(sr_number),0) from sr_register where std_active = 'Y' and std_pickup_id != 1000 and std_sex = 'F'";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

        public int new_admission()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            mst_sessionMain sess = new mst_sessionMain();

            string session = sess.findActive_finalSession();

            String query = @"select ifnull(count(sr_number),0)	 from sr_register where std_active = 'Y' and adm_session = @session";

            var result = con.Query<int>(query, new { session = session }).SingleOrDefault();

            return result;
        }

        public int new_admission_male_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            mst_sessionMain sess = new mst_sessionMain();

            string session = sess.findActive_finalSession();

            String query = @"select ifnull(count(sr_number),0)	 from sr_register where std_active = 'Y' and adm_session = @session and std_sex = 'M'";

            var result = con.Query<int>(query, new { session = session }).SingleOrDefault();

            return result;
        }

        public int new_admission_female_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            mst_sessionMain sess = new mst_sessionMain();

            string session = sess.findActive_finalSession();

            string query = @"select ifnull(count(sr_number),0)	 from sr_register where std_active = 'Y' and adm_session = @session and std_sex = 'F'";

            var result = con.Query<int>(query, new { session = session }).SingleOrDefault();

            return result;
        }

        //public int daily_std_present()
        //{
            
        //    string query = @"SELECT count(*) present FROM attendance_register where attendance = 1 and att_date = curdate() and finalize = 1";

        //    var result = con.Query<int>(query).SingleOrDefault();

        //    return result;
        //}

        //public int daily_std_absent()
        //{

        //    string query = @"SELECT count(*) absent FROM attendance_register where attendance = 0 and att_date = curdate() and finalize = 1";

        //    var result = con.Query<int>(query).SingleOrDefault();

        //    return result;
        //}

        public string[] school_class()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            String query = @"select class_name from mst_class";

            var result = con.Query<string>(query);

            string[] s = result.ToArray<string>();

            return s;
        }
       
        public string[] school_class_for_attendance(int user_id,bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";

            IEnumerable<string> result;

            if (flag)
            {
                query = @"SELECT 
                                    b.class_name
                                FROM
                                    attendance_register a,
                                    mst_class b
                                WHERE
                                    a.class_id = b.class_id
                                        AND att_date = CURDATE()
                                GROUP BY a.class_id
                                ORDER BY a.class_id";

                result = con.Query<string>(query);
            }
            else
            {
                query = @"SELECT 
                                    b.class_name
                                FROM
                                    attendance_register a,
                                    mst_class b
                                WHERE
                                    a.class_id = b.class_id
                                        AND att_date = CURDATE()
                                         AND section_id IN(SELECT
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                                GROUP BY a.class_id
                                ORDER BY a.class_id";

                result = con.Query<string>(query,new { user_id = user_id});
            }
             

            string[] s = result.ToArray<string>();

            return s;
        }

        public string[] date_list_for_attendance(int user_id, bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";

            IEnumerable<string> result;

            if (flag)
            {
                query = @"SELECT 
                                concat(day(att_date),' ',DATE_FORMAT(att_date, '%b'))
                            FROM
                                attendance_register
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                            GROUP BY att_date
                            ORDER BY att_date";

                result = con.Query<string>(query);
            }else
            {
                query = @"SELECT 
                                concat(day(att_date),' ',DATE_FORMAT(att_date, '%b'))
                            FROM
                                attendance_register
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                            AND section_id IN (SELECT 
                                                    section_id
                                                FROM
                                                    mst_attendance
                                                WHERE
                                                    finalizer = @user_id)
                            GROUP BY att_date
                            ORDER BY att_date";

                result = con.Query<string>(query,new {user_id = user_id });
            }

            string[] s = result.ToArray<string>();

            return s;
        }

        public IEnumerable<mentor_header> pending_Observation_list()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query1 = @"SELECT fin_id
                             FROM mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            String query = @"select mentor_no,mentor_date,concat(b.std_first_name,' ',b.std_last_name) std_name,c.class_name,concat(d.staff_first_name,' ',d.staff_last_name) mentor_name,problem,dead_line from mentor_header a,sr_register b, mst_class c, mst_staff d
                                where mentor_no not in 
                                (select distinct mentor_no 
                                from mentor_detail 
                                where fin_id = @fin
                                and observation_date =  date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE )))
                                and fin_id = @fin
                                and dead_line >= date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE )) 
                                and a.sr_number = b.sr_number
                                and a.class_id = c.class_id
                                and a.mentor_id = d.staff_id";

            var result = con.Query<mentor_header>(query, new { fin = fin });



            return result;
        }

        public IEnumerable<attendance_register> finalizer_list(int user_id,bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            IEnumerable<attendance_register> result;
            if (flag)
            {
                 query = @"SELECT DISTINCT
                                DATE_FORMAT(a.att_date, '%d') date_num,
                                DATE_FORMAT(a.att_date, '%b') month_name,
                                CONCAT('Class ',
                                        b.class_name,
                                        ' Section ',
                                        c.section_name) class_name,
                                a.section_id,
                                a.session,
                                a.att_date
                            FROM
                                attendance_register a,
                                mst_class b,
                                mst_section c
                            WHERE
                                IFNULL(a.finalize, 0) = 0
                                    AND a.class_id = b.class_id
                                    AND a.section_id = c.section_id
                            GROUP BY a.class_id
                            ORDER BY date_num , a.class_id";

                 result = con.Query<attendance_register>(query);
            }
            else
            {
                query = @"SELECT DISTINCT
                                DATE_FORMAT(a.att_date, '%d') date_num,
                                DATE_FORMAT(a.att_date, '%b') month_name,
                                CONCAT('Class ',
                                        b.class_name,
                                        ' Section ',
                                        c.section_name) class_name,
                                a.section_id,
                                a.session,
                                a.att_date
                            FROM
                                attendance_register a,
                                mst_class b,
                                mst_section c
                            WHERE
                                IFNULL(a.finalize, 0) = 0
                                    AND a.class_id = b.class_id
                                    AND a.section_id = c.section_id
                                    AND a.section_id in (select section_id from mst_attendance where finalizer = @user_id)
                            GROUP BY a.class_id
                            ORDER BY date_num , a.class_id";

                result = con.Query<attendance_register>(query,new { user_id = user_id });
            }


            return result;
        }

        public int present_finalizer(string session,string date,int section_id)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT 
                                COUNT(*)
                            FROM
                                attendance_register
                            WHERE
                               section_id = @section_id
                                    AND session = @session
                                    AND att_date = @date
                                    AND attendance = 1
                                    AND IFNULL(finalize, 0) = 0";

            var result = con.Query<int>(query,new { section_id = section_id, session = session, date = date }).SingleOrDefault();

            return result;
        }

        public int absent_finalizer(string session, string date, int section_id)
        {

            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT 
                                    COUNT(*)
                                FROM
                                    attendance_register
                                WHERE
                                    section_id = @section_id
                                        AND session = @session
                                        AND att_date = @date
                                        AND attendance = 0
                                        AND IFNULL(finalize, 0) = 0";

            var result = con.Query<int>(query, new { section_id = section_id, session = session, date = date }).SingleOrDefault();

            return result;
        }

        public string class_teacher (int class_id, int section_id)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT 
                                    CONCAT(b.FirstName, ' ', b.LastName) class_teacher
                                FROM
                                    mst_attendance a,
                                    emp_profile b
                                WHERE
                                    a.user_id = b.user_id
                                        AND a.class_id = @class_id
                                        AND a.section_id = @section_id";

            var result = con.Query<string>(query, new { section_id = section_id, class_id = class_id}).SingleOrDefault();

            return result;
        }

        public string finalizer(int class_id, int section_id)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT 
                                    CONCAT(b.FirstName, ' ', b.LastName) class_teacher
                                FROM
                                    mst_attendance a,
                                    emp_profile b
                                WHERE
                                    a.finalizer = b.user_id
                                        AND a.class_id = @class_id
                                        AND a.section_id = @section_id";

            var result = con.Query<string>(query, new { section_id = section_id, class_id = class_id }).SingleOrDefault();

            return result;
        }

        public IEnumerable<attendance_register> att_left_classess(int user_id,bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            IEnumerable<attendance_register> result;
            if (flag)
            {
                query = @"SELECT 
                            CONCAT('Class ',
                                    b.class_name,
                                    ' Section ',
                                    a.section_name) class_name,a.class_id,a.section_id
                        FROM
                            mst_section a,
                            mst_class b
                        WHERE
                            a.class_id = b.class_id
                                AND a.section_id NOT IN (SELECT DISTINCT
                                    section_id
                                FROM
                                    attendance_register
                                WHERE
                                    att_date = CURDATE())
                        ORDER BY a.class_id";

                result = con.Query<attendance_register>(query);
            }
            else
            {
                query = @"SELECT 
                            CONCAT('Class ',
                                    b.class_name,
                                    ' Section ',
                                    a.section_name) class_name,a.class_id,a.section_id
                        FROM
                            mst_section a,
                            mst_class b
                        WHERE
                            a.class_id = b.class_id
                                AND a.section_id IN (SELECT 
                                    section_id
                                FROM
                                    mst_attendance
                                WHERE
                                    finalizer = @user_id)
                                AND a.section_id NOT IN (SELECT DISTINCT
                                    section_id
                                FROM
                                    attendance_register
                                WHERE
                                    att_date = CURDATE())
                        ORDER BY a.class_id";

                result = con.Query<attendance_register>(query, new { user_id = user_id });
            }


            return result;
        }

        public IEnumerable<dailyBirthdayWish> std_birthday_list(int user_id, bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            IEnumerable<dailyBirthdayWish> result;
            if (flag)
            {
                query = @"SELECT 
                            DATE_FORMAT(a.std_dob, '%d') date_num,
                            DATE_FORMAT(a.std_dob, '%b') month_name,
                            CONCAT(IFNULL(std_first_name, ''),
                                    ' ',
                                    IFNULL(std_last_name, '')) std_name,
                            d.class_name,
                            e.section_name
                        FROM
                            ( SELECT 
                            *
                        FROM
                            sr_register
                        WHERE
                            DATE_FORMAT(std_dob, '%c-%d') BETWEEN DATE_FORMAT(CURDATE(), '%c-%d') AND DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 7 DAY),
                                    '%c-%d')
                                OR (MONTH(CURDATE()) > MONTH(DATE_ADD(CURDATE(), INTERVAL 7 DAY))
                                AND (MONTH(std_dob) >= MONTH(CURDATE())
                                OR MONTH(std_dob) <= MONTH(DATE_ADD(CURDATE(), INTERVAL 7 DAY))))) a,
                            mst_batch b,
                            mst_attendance c,
                            mst_class d,
                            mst_section e
                        WHERE
		                        a.std_Active = 'Y'
                                And a.std_batch_id = b.batch_id
                                AND b.class_id = c.class_id
                                AND a.std_section_id = c.section_id
                                AND b.class_id = d.class_id
                                AND a.std_section_id = e.section_id
                        ORDER BY YEAR(CURDATE()) , MONTH(std_dob) , DAY(std_dob)";

                result = con.Query<dailyBirthdayWish>(query);
            }
            else
            {
                query = @"SELECT 
                            DATE_FORMAT(a.std_dob, '%d') date_num,
                            DATE_FORMAT(a.std_dob, '%b') month_name,
                            CONCAT(IFNULL(std_first_name, ''),
                                    ' ',
                                    IFNULL(std_last_name, '')) std_name,
                            d.class_name,
                            e.section_name
                        FROM
                            ( SELECT 
                            *
                        FROM
                            sr_register
                        WHERE
                            DATE_FORMAT(std_dob, '%c-%d') BETWEEN DATE_FORMAT(CURDATE(), '%c-%d') AND DATE_FORMAT(DATE_ADD(CURDATE(), INTERVAL 7 DAY),
                                    '%c-%d')
                                OR (MONTH(CURDATE()) > MONTH(DATE_ADD(CURDATE(), INTERVAL 7 DAY))
                                AND (MONTH(std_dob) >= MONTH(CURDATE())
                                OR MONTH(std_dob) <= MONTH(DATE_ADD(CURDATE(), INTERVAL 7 DAY))))) a,
                            mst_batch b,
                            mst_attendance c,
                            mst_class d,
                            mst_section e
                        WHERE
		                        a.std_Active = 'Y'
                                AND a.std_batch_id = b.batch_id
                                AND b.class_id = c.class_id
                                AND a.std_section_id = c.section_id
                                AND b.class_id = d.class_id
                                and c.user_id = @user_id 
                                and a.std_section_id = e.section_id
                        ORDER BY YEAR(CURDATE()) , MONTH(std_dob) , DAY(std_dob)";

                result = con.Query<dailyBirthdayWish>(query, new { user_id = user_id });
            }


            return result;
        }


        public IEnumerable<dailyBirthdayWish> staff_birthday()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            IEnumerable<dailyBirthdayWish> result;
           
                query = @"SELECT 
                            DATE_FORMAT(a.dob, '%d') date_num,
                            DATE_FORMAT(a.dob, '%b') month_name,
                            CONCAT(IFNULL(FirstName, ''),
                                    ' ',
                                    IFNULL(FirstName, '')) std_name
                        FROM
                            emp_profile a
                        WHERE
                            CONCAT(DAY(dob), MONTH(dob)) IN (CONCAT(DAY(DATE_ADD(CURDATE(), INTERVAL 0 DAY)),
                                    MONTH(DATE_ADD(CURDATE(), INTERVAL 0 DAY))) , CONCAT(DAY(DATE_ADD(CURDATE(), INTERVAL 1 DAY)),
                                        MONTH(DATE_ADD(CURDATE(), INTERVAL 1 DAY))),
                                CONCAT(DAY(DATE_ADD(CURDATE(), INTERVAL 2 DAY)),
                                        MONTH(DATE_ADD(CURDATE(), INTERVAL 2 DAY))),
                                CONCAT(DAY(DATE_ADD(CURDATE(), INTERVAL 3 DAY)),
                                        MONTH(DATE_ADD(CURDATE(), INTERVAL 3 DAY))),
                                CONCAT(DAY(DATE_ADD(CURDATE(), INTERVAL 4 DAY)),
                                        MONTH(DATE_ADD(CURDATE(), INTERVAL 4 DAY))),
                                CONCAT(DAY(DATE_ADD(CURDATE(), INTERVAL 5 DAY)),
                                        MONTH(DATE_ADD(CURDATE(), INTERVAL 5 DAY))),
                                CONCAT(DAY(DATE_ADD(CURDATE(), INTERVAL 6 DAY)),
                                        MONTH(DATE_ADD(CURDATE(), INTERVAL 6 DAY))))
                                AND emp_active = 1
                        ORDER BY YEAR(CURDATE()) , MONTH(dob) , DAY(dob)";

                result = con.Query<dailyBirthdayWish>(query);
            
          


            return result;
        }

        public IEnumerable<mentor_header> pending_Observation_list_for_faculty(int mentor_id)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query1 = @"SELECT fin_id
                             FROM mst_fin
                          where fin_close = 'N'";

            string fin = con.Query<string>(query1).SingleOrDefault();

            string query = @"SELECT a.mentor_id,a.fin_id,a.mentor_no, a.mentor_date,a.sr_number sr_num,concat(b.std_first_name,' ',b.std_last_name) std_name,e.class_name,a.problem,concat(d.staff_first_name,' ',d.staff_last_name) mentor_name,a.dead_line 
                                FROM mentor_header a, sr_register b, mst_batch c,mst_staff d,mst_class e
                                where a.sr_number = b.sr_number
                                and b.std_batch_id = c.batch_id
                                and a.mentor_id = d.staff_id 
                                and e.class_id = c.class_id
                                and b.std_active = 'Y' 
                                and a.fin_id = @fin
                                and a.mentor_id = @mentor_id
                                and a.mentor_no not in (select mentor_no from mentor_detail where mentor_id = @mentor_id and observation_date = date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE )))
                                and a.dead_line >= date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))";

            return con.Query<mentor_header>(query, new { fin = fin, mentor_id = mentor_id });


        }


        public decimal[] dues()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);


            string query1 = @"SELECT session FROM mst_session where session_active = 'Y'";

            string session = con.Query<string>(query1).SingleOrDefault();

            String query = "";

            if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
            {
                query = @"SELECT 
                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                class_id
                            FROM
                                out_standing a,
                                sr_register b
                            WHERE
                                a.sr_number = b.sr_number
                                    AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                    AND month_no <= MONTH(DATE(DATE_ADD(NOW(),
                                            INTERVAL '00:00' HOUR_MINUTE)))
                                    AND month_no BETWEEN 4 AND 12
                                    AND a.session =  @session
                                    AND b.std_active = 'Y'
                            GROUP BY class_id ASC 
                            UNION ALL SELECT 
                                0, class_id
                            FROM
                                mst_class
                            WHERE
                                class_id NOT IN (SELECT 
                                        class_id
                                    FROM
                                        out_standing a,
                                        sr_register b
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                            AND month_no <= MONTH(DATE(DATE_ADD(NOW(),
                                                    INTERVAL '00:00' HOUR_MINUTE)))
                                            AND month_no BETWEEN 4 AND 12
                                            AND a.session =  @session
                                            AND b.std_active = 'Y')";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 1)
            {
                query = @"select sum(ifnull(outstd_amount,0)-ifnull(rmt_amount,0)) amount from out_standing a,sr_register b
                            where
                            a.sr_number = b.sr_number 
                            and (ifnull(outstd_amount,0)-ifnull(rmt_amount,0)) != 0
                            and month_no not in (2,3)
                            and a.session = @session
                            and b.std_active = 'Y'
                            group by class_id asc
                             UNION ALL SELECT 
                                0, class_id
                            FROM
                                mst_class
                            WHERE
                                class_id NOT IN (SELECT 
                                        class_id
                                    FROM
                                        out_standing a,
                                        sr_register b
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                            and month_no not in (2,3)
                                            AND a.session =  @session
                                            AND b.std_active = 'Y')";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 2)
            {
                query = @"select sum(ifnull(outstd_amount,0)-ifnull(rmt_amount,0)) amount from out_standing a,sr_register b
                            where
                            a.sr_number = b.sr_number 
                            and (ifnull(outstd_amount,0)-ifnull(rmt_amount,0)) != 0
                            and month_no != 3
                            and a.session = @session
                            and b.std_active = 'Y'
                            group by class_id asc
                             UNION ALL SELECT 
                                0, class_id
                            FROM
                                mst_class
                            WHERE
                                class_id NOT IN (SELECT 
                                        class_id
                                    FROM
                                        out_standing a,
                                        sr_register b
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                            and month_no != 3
                                            AND a.session = @session
                                            AND b.std_active = 'Y')";
            }
            else
            {
                query = @"select sum(ifnull(outstd_amount,0)-ifnull(rmt_amount,0)) amount from out_standing a,sr_register b
                            where
                            a.sr_number = b.sr_number 
                            and (ifnull(outstd_amount,0)-ifnull(rmt_amount,0)) != 0
                            and a.session = @session
                            and b.std_active = 'Y'
                            group by class_id asc
                             UNION ALL SELECT 
                                0, class_id
                            FROM
                                mst_class
                            WHERE
                                class_id NOT IN (SELECT 
                                        class_id
                                    FROM
                                        out_standing a,
                                        sr_register b
                                    WHERE
                                        a.sr_number = b.sr_number
                                            AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                            AND a.session = @session
                                            AND b.std_active = 'Y')";
            }

            var result = con.Query<decimal>(query, new { session = session });


            decimal[] s = result.ToArray<decimal>();


            return s;
        }

        public decimal[] recovered()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

            string query1 = @"SELECT session FROM mst_session where session_active = 'Y'";

            string session = con.Query<string>(query1).SingleOrDefault();

            String query = "";

            if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
            {
                query = @"SELECT 
                                IFNULL(SUM(d.amount), 0) - IFNULL(SUM(d.dc_discount), 0)
                            FROM
                                out_standing a,
                                sr_register b,
                                mst_batch c,
                                fees_receipt d
                            WHERE
                                a.month_no <= MONTH(DATE(DATE_ADD(NOW(),
                                            INTERVAL '00:00' HOUR_MINUTE)))
                                    AND a.month_no BETWEEN 4 AND 12
                                    AND a.sr_number = b.sr_number
                                    AND b.std_batch_id = c.batch_id
                                    and b.std_active = 'Y'
                                    AND a.session = @session
                                    AND a.serial = d.serial
                            GROUP BY c.class_id ASC 
                            UNION ALL SELECT 
                                0
                            FROM
                                mst_class
                            WHERE
                                class_id NOT IN (SELECT DISTINCT
                                        class_id
                                    FROM
                                        fees_receipt)";
            }
            else
            {
                query = @"select sum(sum)from(select ifnull(sum(d.amount), 0) - ifnull(sum(d.dc_discount), 0) from out_standing a, sr_register b, mst_batch c, fees_receipt d where a.month_no between 4 and 12
                            and a.sr_number = b.sr_number
                            and b.std_batch_id = c.batch_id
                            and a.session = @session
                            and a.serial = d.serial
                            and b.std_active = 'Y'
                             group by c.class_id asc
                             union all
                             select ifnull(sum(d.amount), 0) - ifnull(sum(d.dc_discount), 0) from out_standing a, sr_register b, mst_batch c, fees_receipt d where a.month_no <= month(date(DATE_ADD( now( ) , INTERVAL  '00:00' HOUR_MINUTE ))) and a.month_no between 1 and 3
                            and a.sr_number = b.sr_number
                            and b.std_batch_id = c.batch_id
                            and a.session = @session
                            and a.serial = d.serial
                            and b.std_active = 'Y'
                             group by c.class_id asc) a
                             group by a.class_id";
            }
            var result = con.Query<decimal>(query, new { session = session });

            decimal[] s = result.ToArray<decimal>();

            return s;
        }

        public int[] thirty_day_present(int user_id,bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            int[] s;
            if (flag)
            {
                query = @"SELECT 
                            COUNT(*)
                        FROM
                            attendance_register
                        WHERE
                            att_date BETWEEN DATE_SUB(CURDATE(),
                                INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                AND attendance = 1
                        GROUP BY att_date
                        ORDER BY att_date";

                var result = con.Query<int>(query);
                s = result.ToArray<int>();
            }
            else
            {
                query = @"SELECT 
                                COUNT(*)
                            FROM
                                attendance_register
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                    AND attendance = 1
                                    AND section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                            GROUP BY att_date
                            ORDER BY att_date";

                var result = con.Query<int>(query,new {user_id = user_id });
                s = result.ToArray<int>();
            }
            

            return s;
        }

        public int[] thirty_day_absent(int user_id, bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            int[] s;
            if (flag)
            {
                query = @"SELECT 
                            COUNT(*)
                        FROM
                            attendance_register
                        WHERE
                            att_date BETWEEN DATE_SUB(CURDATE(),
                                INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                AND attendance = 0
                        GROUP BY att_date
                        ORDER BY att_date";

                var result = con.Query<int>(query);
                s = result.ToArray<int>();
            }
            else
            {
                query = @"SELECT 
                                COUNT(*)
                            FROM
                                attendance_register
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                    AND attendance = 0
                                    AND section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                            GROUP BY att_date
                            ORDER BY att_date";

                var result = con.Query<int>(query, new { user_id = user_id });
                s = result.ToArray<int>();
            }


            return s;
        }

        public int[] absent(int user_id, bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            int[] s;
            if (flag)
            {
                query = @"SELECT 
                            COUNT(IF(attendance=0,0, NULL))
                        FROM
                            attendance_register
                        WHERE
                            att_date = CURDATE()
                        GROUP BY class_id
                        ORDER BY class_id";

                var result = con.Query<int>(query);
                s = result.ToArray<int>();
            }
            else
            {
                query = @"SELECT 
                            COUNT(IF(attendance=0,0, NULL))
                        FROM
                            attendance_register
                        WHERE
                            att_date = CURDATE()
                                    AND section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                            GROUP BY class_id
                            ORDER BY class_id";

                var result = con.Query<int>(query, new { user_id = user_id });
                s = result.ToArray<int>();
            }


            return s;
        }

        public int[] present(int user_id, bool flag)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = "";
            int[] s;
            if (flag)
            {
                query = @"SELECT 
                            COUNT(IF(attendance=1,1, NULL))
                        FROM
                            attendance_register
                        WHERE
                            att_date = CURDATE()
                        GROUP BY class_id
                        ORDER BY class_id";

                var result = con.Query<int>(query);
                s = result.ToArray<int>();
            }
            else
            {
                query = @"SELECT 
                            COUNT(IF(attendance=1,1, NULL))
                        FROM
                            attendance_register
                        WHERE
                            att_date = CURDATE()
                                    AND section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                            GROUP BY class_id
                            ORDER BY class_id";

                var result = con.Query<int>(query, new { user_id = user_id });
                s = result.ToArray<int>();
            }


            return s;
        }

        public int today_consumption()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT count(*) FROM sms_record where date(date_time) = curdate() and sms_status = 'Success'";

            var result = con.Query<int>(query).SingleOrDefault();

            return result;
        }

        public string SMSCredit()
        {
            #region Variables
            string postURL = ConfigurationManager.AppSettings["SMSGatewayCreditURL"];

            string responseMessage = string.Empty;
            HttpWebRequest request = null;
            #endregion
            
            try
            {
             
                // Prepare web request
                request = (HttpWebRequest)WebRequest.Create(postURL);
                request.Method = "GET";

                // Send the request and get a response
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    // Read the response
                    using (StreamReader srResponse = new StreamReader(response.GetResponseStream()))
                    {
                        responseMessage = srResponse.ReadToEnd();
                    }


                    // Logic to interpret response from your gateway goes here
                    //Response.Write(String.Format("Response from gateway: {0}", responseMessage));
                }

                return responseMessage;
            }
            catch 
            {
               return "0";
            }
        }
    }
}