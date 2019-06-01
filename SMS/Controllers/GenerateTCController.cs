using Dapper;
using MySql.Data.MySqlClient;
using SMS.ExcelReport;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class GenerateTCController : Controller
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult all_TC_List()
        {
            mst_sessionMain mstSession = new mst_sessionMain();

            IEnumerable<mst_session> session_list = mstSession.AllSesssionList();

            IEnumerable<SelectListItem> list1 = new SelectList(session_list, "session", "session");

            ViewData["session"] = list1;

            IEnumerable<TC> tc_list = TCMain.AllTCList(session_list.First().session);

            return View(tc_list);
        }


        [HttpPost]
        public ActionResult all_TC_List(string session)
        {
            mst_sessionMain mstSession = new mst_sessionMain();

            IEnumerable<mst_session> session_list = mstSession.AllSesssionList();

            IEnumerable<SelectListItem> list1 = new SelectList(session_list, "session", "session");

            ViewData["session"] = list1;

            IEnumerable<TC> tc_list = TCMain.AllTCList(session);

            return View(tc_list);
        }

        [HttpGet]
        public ActionResult DownloadTC(int sr_number,string username,string session, int tc_number, DateTime tc_date)
        {

            ExcelTc_form tc = new ExcelTc_form();            
            
            return File(tc.Download_TC(sr_number,username,session,tc_number,tc_date), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet","TC_"+sr_number + ".xlsx");
        }

        [HttpGet]
        public ActionResult all_nso_students()
        {
            mst_sessionMain mstSession = new mst_sessionMain();

            IEnumerable<mst_session> session_list = mstSession.AllSesssionList();

            IEnumerable<SelectListItem> list1 = new SelectList(session_list, "session", "session");

            ViewData["session"] = list1;

            IEnumerable<nso_students> tc_list = TCMain.AllNSOList(session_list.First().session);

            return View(tc_list);
        }


        [HttpPost]
        public ActionResult all_nso_students(string session)
        {
            mst_sessionMain mstSession = new mst_sessionMain();

            IEnumerable<mst_session> session_list = mstSession.AllSesssionList();

            IEnumerable<SelectListItem> list1 = new SelectList(session_list, "session", "session");

            ViewData["session"] = list1;

            IEnumerable<nso_students> tc_list = TCMain.AllNSOList(session);

            return View(tc_list);
        }

        [HttpGet]
        public ActionResult GenerateTC(int sr_number, string session)
        {
            string query = @"select count(*) from tc_register where sr_num = @sr_number";

            int check = con.Query<int>(query, new {sr_number = sr_number }).SingleOrDefault();

            if (check != 0)
            {
               

                ViewData["message"] = "TC already generated.";

                return View("message");
            }

            if (System.DateTime.Now.Month >= 4 && System.DateTime.Now.Month <= 12)
                {
                    query = @"SELECT 
                                        ifnull(SUM(dues),0)
                                    FROM
                                        (SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND month_no <= MONTH(CURDATE())
                                                AND session = (SELECT 
                                                                    session
                                                                FROM
                                                                    mst_session
                                                                WHERE
                                                                    session_active = 'Y'
                                                                        AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND month_no BETWEEN 4 AND 12
                                                AND b.sr_number = @sr_number UNION ALL SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND a.session != (SELECT 
                                                                        session
                                                                    FROM
                                                                        mst_session
                                                                    WHERE
                                                                        session_active = 'Y'
                                                                            AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND b.sr_number = @sr_number) a";
                }
                else if (System.DateTime.Now.Month == 1)
                {

                    query = @"SELECT 
                                        ifnull(SUM(dues),0)
                                    FROM
                                        (SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND month_no NOT IN (2 , 3)
                                                AND session = (SELECT 
                                                                    session
                                                                FROM
                                                                    mst_session
                                                                WHERE
                                                                    session_active = 'Y'
                                                                        AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND b.sr_number = @sr_number UNION ALL SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND a.session != (SELECT 
                                                                        session
                                                                    FROM
                                                                        mst_session
                                                                    WHERE
                                                                        session_active = 'Y'
                                                                            AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND b.sr_number = @sr_number) a";
                }
                else if (System.DateTime.Now.Month == 2)
                {
                    query = @"SELECT 
                                        ifnull(SUM(dues),0)
                                    FROM
                                        (SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND month_no != 3
                                                AND session = (SELECT 
                                                                    session
                                                                FROM
                                                                    mst_session
                                                                WHERE
                                                                    session_active = 'Y'
                                                                        AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND b.sr_number = @sr_number UNION ALL SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND a.session != (SELECT 
                                                                        session
                                                                    FROM
                                                                        mst_session
                                                                    WHERE
                                                                        session_active = 'Y'
                                                                            AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND b.sr_number = @sr_number) a";
                }
                else
                {
                    query = @"SELECT 
                                        ifnull(SUM(dues),0)
                                    FROM
                                        (SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND session = (SELECT 
                                                                    session
                                                                FROM
                                                                    mst_session
                                                                WHERE
                                                                    session_active = 'Y'
                                                                        AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND b.sr_number = @sr_number UNION ALL SELECT 
                                            SUM(IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0)) dues
                                        FROM
                                            out_standing a, sr_register b
                                        WHERE
                                            IFNULL(outstd_amount, 0) - IFNULL(rmt_amount, 0) != 0
                                                AND a.session != (SELECT 
                                                                        session
                                                                    FROM
                                                                        mst_session
                                                                    WHERE
                                                                        session_active = 'Y'
                                                                            AND session_finalize = 'Y')
                                                AND a.sr_number = b.sr_number
                                                AND b.sr_number = @sr_number) a";
                }

                decimal dues = con.Query<decimal>(query, new { sr_number = sr_number }).SingleOrDefault();

                if (dues != 0)
                {
                   
                     ViewData["message"] = "Sorry task cannot be completed. First clear all the dues upto current month.";
    
                     return View("message");
                
                }
                
            ExcelTc_form tc = new ExcelTc_form();

            return File(tc.Generate_TC(sr_number, Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()), Request.Cookies["loginUserFullName"].Value.ToString()), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "TC_" + sr_number+ ".xlsx");
        }
    }

    public class TC
    {
        [Display(Name = "Session")]
        public string session { get; set; }

        [Display(Name = "TC Number")]
        public int tc_no { get; set; }

        [Display(Name = "TC Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime tc_date { get; set; }

        [Display(Name = "Admission Number")]
        public int sr_num { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }

        [Display(Name = "Prepared By")]
        public string prepared_by { get; set; }

    }

    public class nso_students
    {
        [Display(Name = "Session")]
        public string session { get; set; }

        [Display(Name = "Admission Number")]
        public int sr_number { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }

        [Display(Name = "Father Name")]
        public string std_father_name { get; set; }

        [Display(Name = "Mother Name")]
        public string std_mother_name { get; set; }
    }

    public class TCMain
    {
        

        public static IEnumerable<TC> AllTCList(string session)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT 
                                session,
                                tc_no,
                                tc_date,
                                sr_num,
                                CONCAT(IFNULL(std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name,
                                (SELECT 
                                        CONCAT(IFNULL(FirstName, ''),
                                                    ' ',
                                                    IFNULL(LastName, ''))
                                    FROM
                                        emp_profile
                                    WHERE
                                        user_id = a.user_id) prepared_by
                            FROM
                                tc_register a,
                                sr_register b
                            WHERE
                                a.sr_num = b.sr_number
                                    AND b.std_active = 'N'
                                    AND a.session = @session";

            var result = con.Query<TC>(query, new { session = session });

            return result;
        }

        public static IEnumerable<nso_students> AllNSOList(string session)
        {
            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT 
                                    @session session,
                                    sr_number,
                                    CONCAT(IFNULL(std_first_name, ''),
                                            ' ',
                                            IFNULL(std_last_name, '')) std_name,
                                    std_father_name,
                                    std_mother_name
                                FROM
                                    sr_register
                                WHERE
                                    nso_date BETWEEN (SELECT 
                                            session_start_date
                                        FROM
                                            mst_session
                                        WHERE
                                            session = @session) AND (SELECT 
                                            session_end_date
                                        FROM
                                            mst_session
                                        WHERE
                                            session = @session)
                                        AND std_active = 'N'
                                        and ifnull(tc_generated,0) = 0";

            var result = con.Query<nso_students>(query, new { session = session });

            return result;
        }
    }
}