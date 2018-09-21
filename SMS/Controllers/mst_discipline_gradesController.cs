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
    public class mst_discipline_gradesController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddDisciplineGrades()
        {
            mst_classMain mstClass = new mst_classMain();

            var class_list = mstClass.AllClassListByTeacher(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()));


            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            ViewData["class_id"] = list;


            mst_termMain mst_term = new mst_termMain();

            var term_list = mst_term.AllTermList();

            IEnumerable<SelectListItem> list1 = new SelectList(term_list, "term_id", "term_name");
            ViewData["term_id"] = list1;

            return View();
        }

        [HttpGet]
        public ActionResult studentList(mst_discipline_grades mst)
        {
            mst_discipline_gradesMain mstMain = new mst_discipline_gradesMain();
            IEnumerable<mst_discipline_grades> grades;

            mst_disciplineMain subjectMain = new mst_disciplineMain();
            mst_discipline subject = new mst_discipline();

            List<mst_discipline_grades> list = new List<mst_discipline_grades>();

            grades = mstMain.FindDisciplineGrades(mst.class_id, mst.section_id, mst.discipline_id);

            subject = subjectMain.FindDiscipline(mst.discipline_id);

            if (grades.Count() > 0)
            {
                foreach (var i in grades)
                {
                    list.Add(new mst_discipline_grades { grade = i.grade, class_id = i.class_id, term_id = mst.term_id, section_id = i.section_id, roll_no = i.roll_no, sr_num = i.sr_num, std_name = i.std_name, discipline_id = mst.discipline_id, discipline_name= subject.discipline_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()) });
                }
            }
            else
            {


                var student_list = mstMain.student_list_for_Discipline_Grades(mst.class_id, mst.section_id);



                foreach (var i in student_list)
                {
                    list.Add(new mst_discipline_grades { class_id = i.class_id, term_id = mst.term_id, section_id = i.section_id, roll_no = i.roll_no, sr_num = i.sr_num, std_name = i.std_name, discipline_id = mst.discipline_id, discipline_name= subject.discipline_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()) });
                }
            }

            return View(list);

        }

        [HttpPost]
        public ActionResult studentListSubmit(List<mst_discipline_grades> mst)
        {
            mst_discipline_gradesMain mstMain = new mst_discipline_gradesMain();

            mstMain.AddDisciplineGrades(mst);

            return RedirectToAction("AddDisciplineGrades");
        }



        public JsonResult GetDiscipline(int id, int term_id)
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT b.discipline_id,b.discipline_name FROM mst_class_discipline a, mst_discipline b
                                where
                                a.discipline_id = b.discipline_id
                                and
                                a.class_id = @class_id
                                and
                                a.session = @session";


            var exam_list = con.Query<mst_discipline>(query, new { class_id = id, session = sess.findActive_finalSession(), term_id = term_id });

            IEnumerable<SelectListItem> list = new SelectList(exam_list, "discipline_id", "discipline_name");

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