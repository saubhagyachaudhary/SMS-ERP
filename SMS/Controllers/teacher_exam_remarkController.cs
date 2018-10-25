﻿using Dapper;
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
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

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