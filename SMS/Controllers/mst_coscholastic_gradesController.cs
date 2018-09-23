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
    public class mst_coscholastic_gradesController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddCoscholasticGrades()
        {
            mst_classMain mstClass = new mst_classMain();

            bool flag;

            if (User.IsInRole("superadmin") || User.IsInRole("principal"))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

                var class_list = mstClass.AllClassListByTeacher(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()),flag);


            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            ViewData["class_id"] = list;


            mst_termMain mst_term = new mst_termMain();

            var term_list = mst_term.AllTermList();

            IEnumerable<SelectListItem> list1 = new SelectList(term_list, "term_id", "term_name");
            ViewData["term_id"] = list1;

            return View();
        }

        [HttpGet]
        public ActionResult studentList(mst_coscholastic_grades mst)
        {
            mst_coscholastic_gradesMain mstMain = new mst_coscholastic_gradesMain();
            IEnumerable<mst_coscholastic_grades> grades ;

            mst_co_scholasticMain subjectMain = new mst_co_scholasticMain();
            mst_co_scholastic subject = new mst_co_scholastic();

            List<mst_coscholastic_grades> list = new List<mst_coscholastic_grades>();

            grades = mstMain.FindCoscholasticGrades(mst.class_id, mst.section_id, mst.co_scholastic_id);

            subject = subjectMain.FindCoScholastic(mst.co_scholastic_id);

            if (grades.Count() > 0)
            {
                foreach (var i in grades)
                {
                    list.Add(new mst_coscholastic_grades { grade = i.grade, class_id = i.class_id, term_id = mst.term_id, section_id = i.section_id, roll_no = i.roll_no, sr_num = i.sr_num, std_name = i.std_name, co_scholastic_id = mst.co_scholastic_id, co_scholastic_name = subject.co_scholastic_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()) });
                }
            }
            else
            {
               

                var student_list = mstMain.student_list_for_Coscholastic_Grades(mst.class_id, mst.section_id);

               

                foreach (var i in student_list)
                {
                    list.Add(new mst_coscholastic_grades { class_id = i.class_id, term_id = mst.term_id, section_id = i.section_id, roll_no = i.roll_no, sr_num = i.sr_num, std_name = i.std_name, co_scholastic_id = mst.co_scholastic_id, co_scholastic_name = subject.co_scholastic_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()) });
                }
            }

            

            return View(list);
        }

        [HttpPost]
        public ActionResult studentListSubmit(List<mst_coscholastic_grades> mst)
        {
            mst_coscholastic_gradesMain mstMain = new mst_coscholastic_gradesMain();

            mstMain.AddCoscholasticGrades(mst);

            return RedirectToAction("AddCoscholasticGrades");
        }



        public JsonResult GetCoScholastic(int id,int term_id)
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                    b.co_scholastic_id, b.co_scholastic_name
                                FROM
                                    mst_class_coscholastic a,
                                    mst_co_scholastic b
                                WHERE
                                    a.co_scholastic_id = b.co_scholastic_id
                                        AND a.class_id = @class_id
                                        AND a.session = @session";

            var exam_list = con.Query<mst_co_scholastic>(query, new { class_id = id, session = sess.findActive_finalSession(), term_id= term_id });

            IEnumerable<SelectListItem> list = new SelectList(exam_list, "co_scholastic_id", "co_scholastic_name");

            return Json(list);

        }

        public JsonResult GetSection(int id)
        {


            string query = @"SELECT a.section_id,b.section_name FROM mst_attendance a,mst_section b
                            where
                            a.section_id = b.section_id
                            and
                            a.class_id = @class_id";


            var exam_list = con.Query<mst_section>(query, new { class_id = id });

            IEnumerable<SelectListItem> list = new SelectList(exam_list, "section_id", "section_name");

            return Json(list);

        }

    }
}