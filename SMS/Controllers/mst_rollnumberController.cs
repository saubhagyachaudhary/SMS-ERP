using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_rollnumberController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult roll_class_list()
        {
            mst_rollnumberMain att = new mst_rollnumberMain();

            return View(att.assign_class_list(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString())));
        }

        [HttpGet]
        public ActionResult rollno_class_student_list(int class_id, int section_id)
        {
            mst_rollnumberMain attendanceMain = new mst_rollnumberMain();

            IEnumerable<mst_rollnumber> roll = attendanceMain.student_list_for_rollnumber(class_id, section_id);



            int j = attendanceMain.max_roll_number(class_id,section_id);

            foreach(var i in roll)
            {
                j = j + 1;

                i.roll_number = j;


            }

            return View(roll);
        }

        [HttpPost]
        public ActionResult rollno_class_student_list(List<mst_rollnumber> list)
        {
            string query = @"select count(*) from mst_rollnumber where session = @session and roll_number = @roll_number and class_id = @class_id and section_id = @section_id";

            mst_sessionMain sess = new mst_sessionMain();

            string session = sess.findActive_finalSession();
            int count = 0;
            foreach(var roll in list)
            {
                count = con.Query<int>(query, new { session = session, roll_number = roll.roll_number, class_id = roll.class_id,section_id = roll.section_id }).SingleOrDefault();
                if(count > 0)
                {
                    ModelState.AddModelError(String.Empty, "Roll number already assigned to other student.");

                    mst_rollnumberMain attendanceMain = new mst_rollnumberMain();

                    IEnumerable<mst_rollnumber> roll_no = attendanceMain.student_list_for_rollnumber(roll.class_id, roll.section_id);



                    int j = attendanceMain.max_roll_number(roll.class_id, roll.section_id);

                    foreach (var i in roll_no)
                    {
                        j = j + 1;

                        i.roll_number = j;


                    }

                    return View(roll_no);
                }
            }

            

            mst_rollnumberMain rollMain = new mst_rollnumberMain();

            rollMain.update_roll_no(list);

            return RedirectToAction("roll_class_list");
        }
    }
}