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

            string session = sess.findActive_Session();

            String query = @"select ifnull(count(sr_number),0)	 from sr_register where std_active = 'Y' and adm_session = @session";

            var result = con.Query<int>(query, new { session = session }).SingleOrDefault();

            return result;
        }

        public int new_admission_male_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            mst_sessionMain sess = new mst_sessionMain();

            string session = sess.findActive_Session();

            String query = @"select ifnull(count(sr_number),0)	 from sr_register where std_active = 'Y' and adm_session = @session and std_sex = 'M'";

            var result = con.Query<int>(query, new { session = session }).SingleOrDefault();

            return result;
        }

        public int new_admission_female_std()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            mst_sessionMain sess = new mst_sessionMain();

            string session = sess.findActive_Session();

            string query = @"select ifnull(count(sr_number),0)	 from sr_register where std_active = 'Y' and adm_session = @session and std_sex = 'F'";

            var result = con.Query<int>(query, new { session = session }).SingleOrDefault();

            return result;
        }

        public string[] school_class()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            String query = @"SELECT 
                                    class_name
                                FROM
                                    mst_class
                                WHERE
                                    session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";

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
                                c.class_name
                            FROM
                                attendance_register a,
                                mst_std_class b,
                                mst_class c
                            WHERE
                                a.sr_num = b.sr_num
                                    AND b.class_id = c.class_id
                                    AND att_date = CURDATE()
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND c.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                            GROUP BY b.class_id
                            ORDER BY b.class_id";

                result = con.Query<string>(query);
            }
            else
            {
                query = @"SELECT 
                                c.class_name
                            FROM
                                attendance_register a,
                                mst_std_class b,
                                mst_class c,
                                mst_std_section d
                            WHERE
                                a.sr_num = b.sr_num
                                    AND a.sr_num = d.sr_num
                                    AND b.class_id = c.class_id
                                    AND att_date = CURDATE()
                                    AND d.section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                            GROUP BY b.class_id
                            ORDER BY b.class_id";

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
                                CONCAT(DAY(att_date),
                                        ' ',
                                        DATE_FORMAT(att_date, '%b'))
                            FROM
                                attendance_register
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                    AND session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                            GROUP BY att_date
                            ORDER BY att_date";

                result = con.Query<string>(query);
            }else
            {
                query = @"SELECT 
                                CONCAT(DAY(att_date),
                                        ' ',
                                        DATE_FORMAT(att_date, '%b'))
                            FROM
                                attendance_register a,
                                mst_std_section b
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                    AND a.sr_num = b.sr_num
                                    AND b.section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                                    AND a.session = b.session
                                    AND b.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                            GROUP BY att_date
                            ORDER BY att_date";

                result = con.Query<string>(query,new {user_id = user_id });
            }

            string[] s = result.ToArray<string>();

            return s;
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
                            e.section_id,
                            a.session,
                            a.att_date
                        FROM
                            attendance_register a,
                            mst_class b,
                            mst_section c,
                            mst_std_class d,
                            mst_std_section e
                        WHERE
                            IFNULL(a.finalize, 0) = 0
                                AND a.sr_num = d.sr_num
                                AND d.class_id = b.class_id
                                AND a.sr_num = e.sr_num
                                AND e.section_id = c.section_id
                                AND b.session = c.session
                                AND c.session = d.session
                                AND d.session = e.session
                                AND e.session = (SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_finalize = 'Y')
                        GROUP BY d.class_id
                        ORDER BY date_num , d.class_id";

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
                                e.section_id,
                                a.session,
                                a.att_date
                            FROM
                                attendance_register a,
                                mst_class b,
                                mst_section c,
                                mst_std_class d,
                                mst_std_section e
                            WHERE
                                IFNULL(a.finalize, 0) = 0
                                    AND a.sr_num = d.sr_num
                                    AND d.class_id = b.class_id
                                    AND a.sr_num = e.sr_num
                                    AND e.section_id = c.section_id
                                    AND e.section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND d.session = e.session
                                    AND e.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                            GROUP BY d.class_id
                            ORDER BY date_num , d.class_id";

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
                                    attendance_register a,
                                    mst_std_section b
                                WHERE
                                    a.sr_num = b.sr_num
                                        AND b.section_id = @section_id
                                        AND a.session = b.session
                                        AND b.session = @session
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
                                    attendance_register a,
                                    mst_std_section b
                                WHERE
                                    a.sr_num = b.sr_num
                                        AND b.section_id = @section_id
                                        AND a.session = b.session
                                        AND b.session = @session
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
                                        a.section_name) class_name,
                                a.class_id,
                                a.section_id
                            FROM
                                mst_section a,
                                mst_class b
                            WHERE
                                a.class_id = b.class_id
                                    AND a.session = b.session
                                    AND b.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                                    AND a.section_id NOT IN (SELECT DISTINCT
                                        b.section_id
                                    FROM
                                        attendance_register a,
                                        mst_std_section b
                                    WHERE
                                        b.sr_num = a.sr_num
                                            AND att_date = CURDATE())
                            ORDER BY a.class_id";

                result = con.Query<attendance_register>(query);
            }
            else
            {
                query = @"SELECT 
                                CONCAT('Class ',
                                        b.class_name,
                                        ' Section ',
                                        a.section_name) class_name,
                                a.class_id,
                                a.section_id
                            FROM
                                mst_section a,
                                mst_class b
                            WHERE
                                a.class_id = b.class_id
                                    AND a.session = b.session
                                    AND b.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
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
                                f.section_name
                            FROM
                                (SELECT 
                                    *
                                FROM
                                    sr_register
                                WHERE
                                    DATE_FORMAT(std_dob, '%m-%d') >= DATE_FORMAT(NOW(), '%m-%d')
                                        AND DATE_FORMAT(std_dob, '%m-%d') <= DATE_FORMAT((NOW() + INTERVAL + 7 DAY), '%m-%d')) a,
                                mst_std_class b,
                                mst_attendance c,
                                mst_class d,
                                mst_std_section e,
                                mst_section f
                            WHERE
                                a.std_Active = 'Y'
                                    AND a.sr_number = b.sr_num
                                    AND b.class_id = c.class_id
                                    AND e.section_id = c.section_id
                                    AND b.class_id = d.class_id
                                    AND a.sr_number = e.sr_num
                                    AND e.section_id = f.section_id
                                    AND b.session = d.session
                                    AND d.session = e.session
                                    AND e.session = f.session
                                    AND f.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
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
                                f.section_name
                            FROM
                                (SELECT 
                                    *
                                FROM
                                    sr_register
                                WHERE
                                    DATE_FORMAT(std_dob, '%m-%d') >= DATE_FORMAT(NOW(), '%m-%d')
                                        AND DATE_FORMAT(std_dob, '%m-%d') <= DATE_FORMAT((NOW() + INTERVAL + 7 DAY), '%m-%d')) a,
                                mst_std_class b,
                                mst_attendance c,
                                mst_class d,
                                mst_std_section e,
                                mst_section f
                            WHERE
                                a.std_Active = 'Y'
                                    AND a.sr_number = b.sr_num
                                    AND b.class_id = c.class_id
                                    AND e.section_id = c.section_id
                                    AND b.class_id = d.class_id
                                    AND c.user_id = @user_id
                                    AND a.sr_number = e.sr_num
                                    AND e.section_id = f.section_id
                                    AND b.session = d.session
                                    AND d.session = e.session
                                    AND e.session = f.session
                                    AND f.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
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
                                        IFNULL(LastName, '')) std_name
                            FROM
                                emp_profile a
                            WHERE
                                DATE_FORMAT(dob, '%m-%d') >= DATE_FORMAT(NOW(), '%m-%d')
                                    AND DATE_FORMAT(dob, '%m-%d') <= DATE_FORMAT((NOW() + INTERVAL + 7 DAY), '%m-%d')
                                    AND emp_active = 1
                            ORDER BY YEAR(CURDATE()) , MONTH(dob) , DAY(dob)";

                result = con.Query<dailyBirthdayWish>(query);
            
          


            return result;
        }

        public decimal[] dues()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);


            string query1 = @"SELECT session FROM mst_session where session_finalize = 'Y'";

            string session = con.Query<string>(query1).SingleOrDefault();

            String query = "";

            if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
            {
                query = @"SELECT 
                                *
                            FROM
                                (SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                        class_id
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    a.sr_number = b.sr_number
                                        AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                        AND month_no <= MONTH(DATE(DATE_ADD(NOW(), INTERVAL '00:00' HOUR_MINUTE)))
                                        AND month_no BETWEEN 4 AND 12
                                        AND a.session = @session
                                        AND b.std_active = 'Y'
                                GROUP BY class_id UNION ALL SELECT 
                                    0, class_id
                                FROM
                                    mst_class
                                WHERE
                                    class_id NOT IN (SELECT 
                                            class_id
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                                AND month_no <= MONTH(DATE(DATE_ADD(NOW(), INTERVAL '00:00' HOUR_MINUTE)))
                                                AND month_no BETWEEN 4 AND 12
                                                AND a.session = @session
                                                AND b.std_active = 'Y')
                                        AND session = @session) a
                            ORDER BY a.class_id ASC";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 1)
            {
                query = @"SELECT 
                                *
                            FROM
                                (SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                        class_id
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    a.sr_number = b.sr_number
                                        AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                        AND month_no IN (4 , 5, 6, 7, 8, 9, 10, 11, 12, 1)
                                        AND a.session = @session
                                        AND b.std_active = 'Y'
                                GROUP BY class_id UNION ALL SELECT 
                                    0, class_id
                                FROM
                                    mst_class
                                WHERE
                                    class_id NOT IN (SELECT 
                                            class_id
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                                AND month_no IN (4 , 5, 6, 7, 8, 9, 10, 11, 12, 1)
                                                AND a.session = @session
                                                AND b.std_active = 'Y')
                                        AND session = @session) a
                            ORDER BY a.class_id ASC";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 2)
            {
                query = @"select * from (SELECT 
                                SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                class_id
                            FROM
                                out_standing a,
                                sr_register b
                            WHERE
                                a.sr_number = b.sr_number
                                    AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                    AND month_no != 3
                                    AND a.session = @session
                                    AND b.std_active = 'Y'
                            GROUP BY class_id
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
                                            AND month_no != 3
                                            AND a.session = @session
                                            AND b.std_active = 'Y')
                                    AND session = @session) a
                                    order by a.class_id asc";
            }
            else
            {
                query = @"SELECT 
                                *
                            FROM
                                (SELECT 
                                    SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) amount,
                                        class_id
                                FROM
                                    out_standing a, sr_register b
                                WHERE
                                    a.sr_number = b.sr_number
                                        AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                        AND a.session = @session
                                        AND b.std_active = 'Y'
                                GROUP BY class_id UNION ALL SELECT 
                                    0, class_id
                                FROM
                                    mst_class
                                WHERE
                                    class_id NOT IN (SELECT 
                                            class_id
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            a.sr_number = b.sr_number
                                                AND (IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) != 0
                                                AND a.session = @session
                                                AND b.std_active = 'Y')
                                        AND session = @session) a
                            ORDER BY a.class_id ASC";
            }

            var result = con.Query<decimal>(query, new { session = session });


            decimal[] s = result.ToArray<decimal>();


            return s;
        }

        public decimal[] recovered()
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());
            int dateTimeOffSet = Convert.ToInt32(ConfigurationManager.AppSettings["DateTimeOffSet"]);

            string query1 = @"SELECT session FROM mst_session where session_finalize = 'Y'";

            string session = con.Query<string>(query1).SingleOrDefault();

            String query = "";

            if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month >= 4 && System.DateTime.Now.AddMinutes(dateTimeOffSet).Month <= 12)
            {
                query = @"SELECT 
                                IFNULL(SUM(d.amount), 0) - IFNULL(SUM(d.dc_discount), 0)
                            FROM
                                out_standing a,
                                sr_register b,
                                fees_receipt d,
                                mst_std_class c
                            WHERE
                                a.month_no <= MONTH(DATE(DATE_ADD(NOW(),
                                            INTERVAL '00:00' HOUR_MINUTE)))
                                    AND a.month_no BETWEEN 4 AND 12
                                    AND a.sr_number = b.sr_number
                                    AND b.sr_number = c.sr_num
                                    AND c.session = a.session
                                    AND b.std_active = 'Y'
                                    AND a.session = @session
                                    AND a.serial = d.serial
                            GROUP BY c.class_id 
                            UNION ALL SELECT 
                                0
                            FROM
                                mst_class
                            WHERE
                                class_id NOT IN (SELECT DISTINCT
                                        class_id
                                    FROM
                                        fees_receipt)
                                        AND session = @session";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 1)
            {
                query = @"SELECT 
                                amt
                            FROM
                                (SELECT 
                                    IFNULL(SUM(d.amount), 0) - IFNULL(SUM(d.dc_discount), 0) amt,
                                        c.class_id
                                FROM
                                    out_standing a, sr_register b, fees_receipt d, mst_std_class c
                                WHERE
                                    a.month_no IN (4 , 5, 6, 7, 8, 9, 10, 11, 12, 1)
                                        AND a.sr_number = b.sr_number
                                        AND b.sr_number = c.sr_num
                                        AND c.session = a.session
                                        AND b.std_active = 'Y'
                                        AND a.session =  @session
                                        AND a.serial = d.serial
                                GROUP BY c.class_id UNION ALL SELECT 
                                    0 amt, class_id
                                FROM
                                    mst_class
                                WHERE
                                    class_id NOT IN (SELECT DISTINCT
                                            class_id
                                        FROM
                                            fees_receipt
                                        WHERE
                                            session =  @session) AND session = @session) ab
                            ORDER BY ab.class_id asc";
            }
            else if (System.DateTime.Now.AddMinutes(dateTimeOffSet).Month == 2)
            {
                query = @"SELECT 
                                amt
                            FROM
                                (SELECT 
                                    IFNULL(SUM(d.amount), 0) - IFNULL(SUM(d.dc_discount), 0) amt,
                                        c.class_id
                                FROM
                                    out_standing a, sr_register b, fees_receipt d, mst_std_class c
                                WHERE
                                    a.month_no != 3
                                        AND a.sr_number = b.sr_number
                                        AND b.sr_number = c.sr_num
                                        AND c.session = a.session
                                        AND b.std_active = 'Y'
                                        AND a.session =  @session
                                        AND a.serial = d.serial
                                GROUP BY c.class_id UNION ALL SELECT 
                                    0 amt, class_id
                                FROM
                                    mst_class
                                WHERE
                                    class_id NOT IN (SELECT DISTINCT
                                            class_id
                                        FROM
                                            fees_receipt
                                        WHERE
                                            session =  @session) AND session = @session) ab
                            ORDER BY ab.class_id asc";
            }
            else
            {
                query = @"SELECT 
                                amt
                            FROM
                                (SELECT 
                                    IFNULL(SUM(d.amount), 0) - IFNULL(SUM(d.dc_discount), 0) amt,
                                        c.class_id
                                FROM
                                    out_standing a, sr_register b, fees_receipt d, mst_std_class c
                                WHERE
                                    a.sr_number = b.sr_number
                                        AND b.sr_number = c.sr_num
                                        AND c.session = a.session
                                        AND b.std_active = 'Y'
                                        AND a.session = @session
                                        AND a.serial = d.serial
                                GROUP BY c.class_id UNION ALL SELECT 
                                    0 amt, class_id
                                FROM
                                    mst_class
                                WHERE
                                    class_id NOT IN (SELECT DISTINCT
                                            class_id
                                        FROM
                                            fees_receipt
                                        WHERE
                                            session = @session) AND session = @session) ab
                            ORDER BY ab.class_id asc";
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
                                    AND session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
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
                                attendance_register a,
                                mst_std_section b
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                    AND attendance = 1
                                    AND a.sr_num = b.sr_num
                                    AND b.section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                                    AND a.session = b.session
                                    AND b.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
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
                                AND session = (SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_finalize = 'Y')
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
                                attendance_register a,
                                mst_std_section b
                            WHERE
                                att_date BETWEEN DATE_SUB(CURDATE(),
                                    INTERVAL DAY(LAST_DAY(CURDATE())) DAY) AND CURDATE()
                                    AND attendance = 0
                                    AND a.sr_num = b.sr_num
                                    AND b.section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                                    AND a.session = b.session
                                    AND b.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
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
                                COUNT(IF(attendance = 0, 0, NULL))
                            FROM
                                attendance_register a,
                                mst_std_class b
                            WHERE
                                att_date = CURDATE()
                                    AND a.session = b.session
                                    AND b.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                                    AND a.sr_num = b.sr_num
                            GROUP BY b.class_id
                            ORDER BY b.class_id";

                var result = con.Query<int>(query);
                s = result.ToArray<int>();
            }
            else
            {
                query = @"SELECT 
                                COUNT(IF(attendance = 0, 0, NULL))
                            FROM
                                attendance_register a,
                                mst_std_class b,
                                mst_std_section c
                            WHERE
                                att_date = CURDATE()
                                    AND a.session = b.session
                                    AND b.session = c.session
                                    AND c.session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')
                                    AND a.sr_num = b.sr_num
                                    AND a.sr_num = c.sr_num
                                    AND c.section_id IN (SELECT 
                                        section_id
                                    FROM
                                        mst_attendance
                                    WHERE
                                        finalizer = @user_id)
                            GROUP BY b.class_id
                            ORDER BY b.class_id";

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
                            COUNT(IF(attendance = 1, 1, NULL))
                        FROM
                            attendance_register a,
                            mst_std_class b
                        WHERE
                            att_date = CURDATE()
                                AND a.sr_num = b.sr_num
                                AND a.session = b.session
                                AND b.session = (SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_finalize = 'Y')
                        GROUP BY b.class_id
                        ORDER BY b.class_id";

                var result = con.Query<int>(query);
                s = result.ToArray<int>();
            }
            else
            {
                query = @"SELECT 
                            COUNT(IF(attendance = 1, 1, NULL))
                        FROM
                            attendance_register a,
                            mst_std_class b,
                            mst_std_section c
                        WHERE
                            att_date = CURDATE()
                                AND a.sr_num = b.sr_num
                                AND a.sr_num = c.sr_num
                                AND a.session = b.session
                                AND b.session = c.session
                                AND b.session = (SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_finalize = 'Y')
                                AND c.section_id IN (SELECT 
                                    section_id
                                FROM
                                    mst_attendance
                                WHERE
                                    finalizer = @user_id)
                        GROUP BY b.class_id
                        ORDER BY b.class_id";

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