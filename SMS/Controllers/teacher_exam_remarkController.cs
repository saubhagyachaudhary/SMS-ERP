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
    public class teacher_exam_remarkController : BaseController
    {
        

        [HttpGet]
        public ActionResult AddRemarks()
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

            var class_list = mstClass.AllClassListByTeacher(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()), flag);


            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            ViewData["class_id"] = list;


            mst_termMain mst_term = new mst_termMain();

            var term_list = mst_term.AllTermList();

            IEnumerable<SelectListItem> list1 = new SelectList(term_list, "term_id", "term_name");
            ViewData["term_id"] = list1;

            return View();
        }

        [HttpGet]
        public ActionResult studentList(teacher_exam_remark mst)
        {
            if(mst.term_id == 0 || mst.section_id == 0)
            {
                
                ModelState.AddModelError(String.Empty, "Check for required field.");
                
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

                var class_list = mstClass.AllClassListByTeacher(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()), flag);


                IEnumerable<SelectListItem> list3 = new SelectList(class_list, "class_id", "class_name");
                ViewData["class_id"] = list3;


                mst_termMain mst_term = new mst_termMain();

                var term_list = mst_term.AllTermList();

                IEnumerable<SelectListItem> list1 = new SelectList(term_list, "term_id", "term_name");
                ViewData["term_id"] = list1;

                return View("AddRemarks");
            }

            teacher_exam_remarkMain main = new teacher_exam_remarkMain();
            List<teacher_exam_remark> list = new List<teacher_exam_remark>();
            IEnumerable<teacher_exam_remark> remark;

            remark = main.FindRemarks(mst.class_id, mst.section_id, mst.term_id);

            if (remark.Count() > 0)
            {
                foreach (var i in remark)
                {
                    list.Add(new teacher_exam_remark { remark = i.remark, class_id = i.class_id, term_id = mst.term_id, section_id = i.section_id, roll_no = i.roll_no, sr_number = i.sr_number, std_name = i.std_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()) });
                }
            }
            else
            {


                var std_list = main.student_list_for_remark(mst.class_id, mst.section_id);

                foreach (var i in std_list)
                {
                    list.Add(new teacher_exam_remark { class_id = i.class_id, term_id = mst.term_id, section_id = i.section_id, roll_no = i.roll_no, sr_number = i.sr_number, std_name = i.std_name, user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()) });
                }
            }
            return View(list);
        }

        [HttpPost]
        public ActionResult studentListSubmit(List<teacher_exam_remark> mst)
        {
            teacher_exam_remarkMain mstMain = new teacher_exam_remarkMain();

            mstMain.AddRemark(mst);

            return RedirectToAction("AddRemarks");
        }

        public JsonResult GetSection(int id)
        {

            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                a.section_id, b.section_name
                            FROM
                                mst_attendance a,
                                mst_section b
                            WHERE
                                a.section_id = b.section_id
                                    AND a.class_id = @class_id
                                    AND session = (SELECT 
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y')";


                var exam_list = con.Query<mst_section>(query, new { class_id = id });

                IEnumerable<SelectListItem> list = new SelectList(exam_list, "section_id", "section_name");

                return Json(list);
            }
        }

    }
}