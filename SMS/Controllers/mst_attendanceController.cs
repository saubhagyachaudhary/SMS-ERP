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
    public class mst_attendanceController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult list()
        {
            mst_attendanceMain mst = new mst_attendanceMain();

            return View(mst.assignList());
        }

        [HttpGet]
        public ActionResult Assign_faculty()
        {


            DDFacultyList();

            DDclass_name();

            return View();
        }

        public JsonResult GetSections(int id)
        {
            string query = @"SELECT 
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

            var section_list = con.Query<mst_section>(query,new { class_id = id});

            IEnumerable<SelectListItem> list = new SelectList(section_list,"section_id","section_name");

            return Json(list);

        }

        [HttpPost]
        public ActionResult Assign_faculty(mst_attendance mst)
        {
            string query = @"SELECT 
                                COUNT(*)
                            FROM
                                mst_attendance
                            WHERE
                                class_id = @class_id
                                    AND section_id = @section_id";

            int count = con.Query<int>(query, new { class_id = mst.class_id,section_id = mst.section_id }).SingleOrDefault();

            if(mst.class_id == 0 || mst.section_id == 0)
            {
                ModelState.AddModelError(String.Empty, "Class and section cannot be blank.");
                DDFacultyList();

                DDclass_name();
                return View(mst);
            }

            if(count==0)
            {
                mst_attendanceMain att = new mst_attendanceMain();

                att.Assign_faculty(mst);

                return RedirectToAction("list");
            }
            else
            {
                ModelState.AddModelError(String.Empty, "Class already assigned to other faculty.");
                DDFacultyList();

                DDclass_name();
                return View(mst);
            }

           

            
        }

        public void DDFacultyList()
        {
            emp_detailMain emp = new emp_detailMain();

            var user_id = emp.DDFacultyList();

            IEnumerable<SelectListItem> list1 = new SelectList(user_id, "user_id", "user_name");

            ViewData["user_id"] = list1;

        }


        public void DDclass_name()
        {
            mst_classMain mstClass = new mst_classMain();

            mst_sessionMain sess = new mst_sessionMain();

            var class_list = mstClass.AllClassList(sess.findFinal_Session());

            IEnumerable<SelectListItem> list1 = new SelectList(class_list, "class_id", "class_name");
           

            ViewData["class_id"] = list1;
        }

        [HttpGet]
        public ActionResult deleteFaculty(int user_id, int class_id,int finalizer_user_id,int section_id)
        {

            mst_attendanceMain mst = new mst_attendanceMain();



            return View(mst.FindFaculty(user_id,class_id,finalizer_user_id,section_id)); ;
        }

        [HttpPost]
        public ActionResult deleteFaculty(int user_id, int class_id,int finalizer_user_id,int section_id, FormCollection ff)
        {
            mst_attendanceMain att = new mst_attendanceMain();

            att.deleteFaculty(user_id,class_id,finalizer_user_id,section_id);

            return RedirectToAction("list"); ;
        }

    }
}