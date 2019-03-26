using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMS.Models;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper;

namespace SMS.Controllers
{
    public class mst_exam_marksController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult addExamMarks()
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

            emp_detailMain empMain = new emp_detailMain();

            var emp_list = empMain.DDFacultyList();

            IEnumerable<SelectListItem> list1 = new SelectList(emp_list, "user_id", "user_name");

            ViewData["class_id"] = list;
            ViewData["marks_assigned_user_id"] = list1;

            return View();
        }

        [HttpGet]
        public ActionResult studentList(mst_exam_marks mst)
        {


            mst_subjectMain subjectMain = new mst_subjectMain();
            mst_subject subject = new mst_subject();
            List<mst_exam_marks> list = new List<mst_exam_marks>();

            mst_examMain examMain = new mst_examMain();
            mst_exam exam = new mst_exam();


            mst_exam_marksMain mstMain = new mst_exam_marksMain();

            IEnumerable<mst_exam_marks> marks = mstMain.find_marks(mst.exam_id, mst.subject_id, mst.class_id, mst.section_id);

            if (marks.Count() > 0)
            {
                var student_list = mstMain.student_list_for_marks_update(mst.subject_id,mst.class_id,mst.section_id,mst.exam_id);
                exam = examMain.FindExam(mst.exam_id);

                ViewData["MaxMarks"] = exam.max_no;

                subject = subjectMain.FindSubject(mst.subject_id);

                foreach (var i in student_list)
                {
                    list.Add(new mst_exam_marks { present = i.present, marks = i.marks,class_id = i.class_id, exam_id = mst.exam_id, exam_name = exam.exam_name, marks_assigned_user_id = mst.marks_assigned_user_id, section_id = i.section_id, roll_no = i.roll_no, sr_num = i.sr_num, std_name = i.std_name, subject_id = mst.subject_id, subject_name = subject.subject_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()) });
                }
            }
            else
            {
                var student_list = mstMain.student_list_for_marks(mst.class_id, mst.section_id);
                exam = examMain.FindExam(mst.exam_id);

                ViewData["MaxMarks"] = exam.max_no;

                subject = subjectMain.FindSubject(mst.subject_id);

                foreach (var i in student_list)
                {
                    list.Add(new mst_exam_marks { class_id = i.class_id, exam_id = mst.exam_id, exam_name = exam.exam_name, marks_assigned_user_id = mst.marks_assigned_user_id, section_id = i.section_id, roll_no = i.roll_no, sr_num = i.sr_num, std_name = i.std_name, subject_id = mst.subject_id, subject_name = subject.subject_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()), present = true });
                }
            }


          

           

            return View(list);
        }

        [HttpPost]
        public ActionResult studentListSubmit(List<mst_exam_marks> mst)
        {
            mst_exam_marksMain mstMain = new mst_exam_marksMain();

            mstMain.AddExamMarks(mst);

            return RedirectToAction("addExamMarks");
        }



        public JsonResult GetExam(int id)
        {
           
            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                    b.exam_id, b.exam_name
                                FROM
                                    mst_exam_class a,
                                    mst_exam b
                                WHERE
                                    a.exam_id = b.exam_id
                                        AND a.class_id = @class_id
                                        AND a.session = @session
                                        AND a.session = b.session";


            var exam_list = con.Query<mst_exam>(query, new { class_id = id,session = sess.findFinal_Session() });

            IEnumerable<SelectListItem> list = new SelectList(exam_list, "exam_id", "exam_name");

            return Json(list);

        }

        public JsonResult GetSubject(int exam_id, int class_id, int section_id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                a.subject_id, b.subject_name
                            FROM
                                mst_class_subject a,
                                mst_subject b
                            WHERE
                                a.subject_id = b.subject_id
                                    AND a.session = @session
                                    AND a.class_id = @class_id
                                    AND a.session = b.session";



            var exam_list = con.Query<mst_subject>(query, new {class_id = class_id,session = sess.findFinal_Session() });

            IEnumerable<SelectListItem> list = new SelectList(exam_list, "subject_id", "subject_name");

            return Json(list);

        }

        public JsonResult GetSection(int id)
        {
            bool flag;
            string query;

            if (User.IsInRole("superadmin") || User.IsInRole("principal"))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            if(flag)
            {
                query = @"SELECT 
                                section_id, section_name
                            FROM
                                mst_section
                            WHERE
                                class_id = @class_id
                                    AND session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')";
            }
            else
            {
                query = @"SELECT DISTINCT
                                    *
                                FROM
                                    (SELECT 
                                        a.section_id, b.section_name
                                    FROM
                                        mst_attendance a, mst_section b
                                    WHERE
                                        a.section_id = b.section_id
                                            AND a.class_id = @class_id
                                            AND a.user_id = @user_id
                                            AND b.session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y') UNION ALL SELECT 
                                        a.section_id, b.section_name
                                    FROM
                                        mst_attendance a, mst_section b
                                    WHERE
                                        a.section_id = b.section_id
                                            AND a.class_id = @class_id
                                            AND a.finalizer = @user_id
                                            AND b.session = (SELECT 
                                                session
                                            FROM
                                                mst_session
                                            WHERE
                                                session_finalize = 'Y')) a";
            }

          


            var exam_list = con.Query<mst_section>(query, new { class_id = id, user_id = int.Parse(Request.Cookies["loginUserId"].Value.ToString()) });

            IEnumerable<SelectListItem> list = new SelectList(exam_list, "section_id", "section_name");

            return Json(list);

        }
    }
}