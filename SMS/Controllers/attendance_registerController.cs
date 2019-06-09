using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class attendance_registerController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult attendance_class_list()
        {
            mst_attendanceMain att = new mst_attendanceMain();

            bool flag;
            string role = "";

            if (User.IsInRole("superadmin") || User.IsInRole("principal") || User.IsInRole("admin"))
            {
                flag = true;
                if (User.IsInRole("superadmin"))
                {
                    role = "Super Admin";
                }
                else if (User.IsInRole("principal"))
                {
                    role = "Principal";
                }
                else
                {
                    role = "Admin";
                }

            }
            else
            {
                flag = false;
                
            }

            return View(att.Attendance_class_list(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()),flag,role));

            //test
        }

        [HttpGet]
        public ActionResult attendance_class_student_list(int class_id,int section_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findFinal_Session();


            string query = @"SELECT 
                                COUNT(*)
                            FROM
                                sr_register a,
                                mst_section b,
                                mst_std_section c,
                                mst_std_class d
                            WHERE
                                c.section_id = b.section_id
                                    AND d.class_id = b.class_id
                                    AND a.std_active = 'Y'
                                    AND b.session = @session
                                    AND b.session = c.session
                                    AND c.session = d.session
                                    AND a.sr_number = c.sr_num
                                    AND c.sr_num = d.sr_num
                                    AND d.class_id = @class_id
                                    AND b.section_id = @section_id
                                    AND a.sr_number NOT IN (SELECT 
                                        sr_num
                                    FROM
                                        mst_rollnumber
                                    WHERE
                                        session = @session)";

            int count = con.Query<int>(query, new { class_id = class_id,section_id = section_id ,session = sess }).SingleOrDefault();

            if(count==0)
            {

                attendance_registerMain attendanceMain = new attendance_registerMain();

                return View(attendanceMain.student_list_for_attendance(class_id, section_id));
            }
            else
            {
                return View("updateRollNo");
            }

        }

        [HttpPost]
        public async Task<ActionResult> attendance_class_student_list(List<attendance_register> attendance)
        {
            attendance_registerMain attendanceMain = new attendance_registerMain();

            await attendanceMain.mark_attendance(attendance, Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()));

            return RedirectToAction("attendance_class_list");
        }

        [HttpGet]
        public ActionResult finalize_class_attendance_sheet(int section_id,DateTime att_date,string session)
        {
            attendance_registerMain attendanceMain = new attendance_registerMain();

            IEnumerable<attendance_register> att = attendanceMain.find_attendance_sheet_for_finalize(section_id, att_date, session);

            if(att.Count()>0)
            {
                return View(att);
            }
            else
            {
                return View("nodatafound");
            }

        }

        [HttpPost]
        public async Task<ActionResult> finalize_class_attendance_sheet(List<attendance_register> attendance,bool send_sms)
        {
            attendance_registerMain attendanceMain = new attendance_registerMain();

            await attendanceMain.finalize_attendance(attendance,send_sms);

            return View("success");
        }
    }
}