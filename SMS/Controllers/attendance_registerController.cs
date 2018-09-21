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
            
            return View(att.Attendance_class_list(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString())));
        }

        [HttpGet]
        public ActionResult attendance_class_student_list(int class_id,int section_id)
        {
            mst_sessionMain session = new mst_sessionMain();

            string sess = session.findActive_finalSession();


            string query = @"select count(*) from sr_register a, mst_section b
														where
														a.std_section_id = b.section_id
														and
                                                        a.std_active = 'Y'
                                                        and
														b.session = @session
														and
                                                        b.class_id = @class_id
                                                        and 
                                                        b.section_id = @section_id
                                                        and
														a.sr_number not in (select sr_num from mst_rollnumber where session = @session)";

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
        public async Task<ActionResult> finalize_class_attendance_sheet(List<attendance_register> attendance)
        {
            attendance_registerMain attendanceMain = new attendance_registerMain();

            await attendanceMain.finalize_attendance(attendance);

            return View("success");
        }
    }
}