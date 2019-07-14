using Dapper;
using MySql.Data.MySqlClient;
using SMS.ExcelReport;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.VisitorsControllers
{
    public class std_halfday_logController : Controller
    {
        

        [HttpGet]
        public ActionResult std_half_day()
        {

            std_halfday_log log = new std_halfday_log();

            if (Convert.ToString(Session["CapturedImage"]) != string.Empty)
            {

                ViewBag.pic = Session["CapturedImage"].ToString();
                ViewBag.disable = "";
                log.Userpic = Session["CapturedImage"].ToString();
                Session["CapturedImage"] = "";
            }
            else
            {
                ViewBag.pic = "../../images/person.png";
                Session["CapturedImage"] = "";
                ViewBag.disable = "disabled";
                log.Userpic = "../../images/person.png";
            }
            return View(log);
        }


        [HttpPost]
        public ActionResult std_half_day(std_halfday_log std)
        {

            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                gate_pass_no, session
                            FROM
                                std_halfday_log
                            WHERE
                                sr_number = @sr_number
                                    AND DATE(date_time) = CURDATE()";

                std_halfday_log duplicategatepass = con.Query<std_halfday_log>(query, new { sr_number = std.sr_number }).SingleOrDefault();

                ExcelGatePassMain gatepass = new ExcelGatePassMain();

                if (duplicategatepass != null)
                {
                    return File(gatepass.gate_pass(duplicategatepass.session, duplicategatepass.gate_pass_no, std.Userpic), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Gate Pas.xlsx");
                }
                else
                {

                    std_halfday_logMain main = new std_halfday_logMain();

                    mst_sessionMain sess = new mst_sessionMain();

                    std.session = sess.findFinal_Session();
                    
                    return File(gatepass.gate_pass(std.session, main.AddHalfDayLog(std), std.Userpic), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Gate Pas.xlsx");
                }
            }
        }

        
        public JsonResult GetGatePass(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                count(*)
                            FROM
                                std_halfday_log
                            WHERE
                                sr_number = @sr_number
                                    AND DATE(date_time) = CURDATE()";

                int count = con.Query<int>(query, new { sr_number = id }).SingleOrDefault();

                return Json(count);

            }
        }

        public JsonResult SendOTP(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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

        }

        public JsonResult GetClass(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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
        }

      

        public JsonResult GetStd_name(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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

        }

        public JsonResult GetFathers_name(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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

        }

        public JsonResult GetNumber(int id)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
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




        #region webcam

        [HttpGet]
        public ActionResult Index()
        {
            Session["CapturedImage"] = "";
            return View();
        }

        [HttpPost]
        public ActionResult Index(string Imagename)
        {
           
            return View();
        }



        public JsonResult Rebind()
        {
            
            string path = Session["CapturedImage"].ToString();

            return Json(path, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Capture()
        {
            var stream = Request.InputStream;
            //string dump;

            using (var reader = new StreamReader(stream))
            {
               
                string hexString = Server.UrlEncode(reader.ReadToEnd());
                byte[] bytes = String_To_Bytes2(hexString);
                string base64String = Convert.ToBase64String(bytes, 0, bytes.Length);
                Session["CapturedImage"] = "data:image/png;base64," + base64String;

            }

            return View("Index");
        }

        private byte[] String_To_Bytes2(string strInput)
        {
            int numBytes = (strInput.Length) / 2;

            byte[] bytes = new byte[numBytes];

            for (int x = 0; x < numBytes; ++x)
            {
                bytes[x] = Convert.ToByte(strInput.Substring(x * 2, 2), 16);
            }

            return bytes;
        }

        #endregion
    }

    public class std_halfday_log
    {
        [Key]
        [Required]
        [Display(Name ="Session")]
        public string session { get; set; }

        [Key]
        [Required]
        [Display(Name = "Gate Pass No")]
        public int gate_pass_no { get; set; }

        [Key]
        [Required]
        [Display(Name = "Admission Number")]
        public int sr_number { get; set; }

        [Key]
        public DateTime date_time { get; set; }

        [Required]
        [Display(Name = "Relation")]
        public string std_relation { get; set; }

        [Required]
        [Display(Name = "Person Name")]
        public string escorter_name { get; set; }

        [Required]
        [Display(Name = "Person address")]
        public string escorter_address { get; set; }

        [Required]
        [Display(Name = "Reason")]
        public string reason { get; set; }

        public string Userpic { get; set; }

    }

    public class std_halfday_logMain
    {
       
        public int AddHalfDayLog(std_halfday_log std)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"SELECT 
                                            MAX(IFNULL(gate_pass_no, 0)) + 1
                                        FROM
                                            std_halfday_log
                                        WHERE
                                            session = @session";

                    std.gate_pass_no = con.Query<int>(query, new { session = std.session }).SingleOrDefault();
                    
                    std.date_time = DateTime.Now;

                     query = @"INSERT INTO `std_halfday_log`
                                (`gate_pass_no`,
                                `session`,
                                `sr_number`,
                                `date_time`,
                                `std_relation`,
                                `escorter_name`,
                                `escorter_address`,
                                `reason`)
                                VALUES
                                (@gate_pass_no,
                                @session,
                                @sr_number,
                                @date_time,
                                @std_relation,
                                @escorter_name,
                                @escorter_address,
                                @reason);";

                    con.Execute(query, std);

                    return std.gate_pass_no;
                }
            }
            catch (Exception ex)
            {
                //do for error handling
            }
            return 0;
        }
    }
}