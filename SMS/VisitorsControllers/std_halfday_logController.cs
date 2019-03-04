using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.VisitorsControllers
{
    public class std_halfday_logController : Controller
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult std_half_day()
        {
            return View();
        }

        [HttpPost]
        public ActionResult std_half_day(std_halfday_log std)
        {
           
                std_halfday_logMain main = new std_halfday_logMain();

                main.AddHalfDayLog(std);

                return View("success");
           
        }

        public JsonResult SendOTP(int id)
        {
            string query = @"SELECT
                                coalesce(std_contact,std_contact1,std_contact2)
                            FROM
                                sr_register
                            WHERE
                                std_active = 'Y' AND sr_number = @sr_number";
            
            string std_number = con.Query<string>(query, new { sr_number = id }).SingleOrDefault();

            SMSMessage sm = new SMSMessage();

            Random rdm = new Random();

            string password = rdm.Next(1000, 9999).ToString();

            string txt = @"Your ward take away OTP is " + password;

            sm.SendOTP(txt, std_number, false);

            return Json(password);

        }

        public JsonResult GetClass(int id)
        {
            string query = @"SELECT 
                                    CONCAT('Class: ',
                                            c.class_name,
                                            '     Section: ',
                                            d.section_name) std_class
                                FROM
                                    mst_std_class a,
                                    mst_std_section b,
                                    mst_class c,
                                    mst_section d,
                                    sr_register e
                                WHERE
                                    a.session = b.session
                                        AND b.session = c.session
                                        AND c.session = d.session
                                        AND d.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')
                                        AND a.sr_num = b.sr_num
                                        AND b.sr_num = e.sr_number
                                        AND e.std_active = 'Y'
                                        AND e.sr_number = @sr_number
                                        AND a.class_id = c.class_id
                                        AND b.section_id = d.section_id";



            var std_class = con.Query<string>(query, new { sr_number = id });
            
            return Json(std_class);

        }

        public JsonResult GetStd_name(int id)
        {
            string query = @"SELECT 
                               std_father_name
                            FROM
                                sr_register
                            WHERE
                                std_active = 'Y' AND sr_number = @sr_number";



            var std_class = con.Query<string>(query, new { sr_number = id });

            return Json(std_class);

        }

        public JsonResult GetFathers_name(int id)
        {
            string query = @"SELECT
                                CONCAT(IFNULL(std_first_name, ''),
                                        ' ',
                                        IFNULL(std_last_name, '')) std_name
                            FROM
                                sr_register
                            WHERE
                                std_active = 'Y' AND sr_number = @sr_number";



            var std_class = con.Query<string>(query, new { sr_number = id });

            return Json(std_class);

        }

        public JsonResult GetNumber(int id)
        {
            string query = @"SELECT
                                coalesce(std_contact,std_contact1,std_contact2)
                            FROM
                                sr_register
                            WHERE
                                std_active = 'Y' AND sr_number = @sr_number";



            var std_class = con.Query<string>(query, new { sr_number = id });

            return Json(std_class);

        }
    }

    public class std_halfday_log
    {
        [Key]
        [Required]
        [Display(Name ="Session")]
        public string session { get; set; }

        [Key]
        [Required]
        [Display(Name = "Admission Number")]
        public int sr_number { get; set; }

        [Key]
        public DateTime date_time { get; set; }
                
        [Required]
        [Display(Name = "Reason")]
        public string reason { get; set; }

    }

    public class std_halfday_logMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public void AddHalfDayLog(std_halfday_log std)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();

                std.session = sess.findFinal_Session();

                std.date_time = DateTime.Now;

                string query = @"INSERT INTO `std_halfday_log`
                                (`session`,
                                `sr_number`,
                                `date_time`,
                                `reason`)
                                VALUES
                                (@session,
                                @sr_number,
                                @date_time,
                                @reason);";

                con.Execute(query,std);
            }
            catch(Exception ex)
            {
                //do for error handling
            }
        }
    }
}