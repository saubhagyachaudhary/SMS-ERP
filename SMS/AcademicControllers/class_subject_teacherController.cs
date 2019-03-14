using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.AcademicControllers
{
    public class class_subject_teacherController : Controller
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AllSubjectTeacher()
        {
            mst_class_subject_teacherMain main = new mst_class_subject_teacherMain();
            
            return View(main.AllSubjectTeacher());
        }

        [HttpGet]
        public ActionResult AddSubjectTeacher()
        {
            mst_classMain mstClass = new mst_classMain();
            
            emp_detailMain mstFaculty = new emp_detailMain();

            mst_sessionMain sess = new mst_sessionMain();

            var class_list = mstClass.AllClassList(sess.findFinal_Session());

            var emp_list = mstFaculty.DDFacultyList();

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
           
            IEnumerable<SelectListItem> list2 = new SelectList(emp_list, "user_id", "user_name");


            ViewData["class_id"] = list;
          
            ViewData["subject_teacher_id"] = list2;

            return View();
        }

        [HttpPost]
        public ActionResult AddSubjectTeacher(mst_class_subject_teacher mst)
        {

            try
            {

                mst_class_subject_teacherMain main = new mst_class_subject_teacherMain();

                main.AddSubjectTeacher(mst);

                return RedirectToAction("AllSubjectTeacher");
            }
            catch
            {
                mst_classMain mstClass = new mst_classMain();
                
                emp_detailMain mstFaculty = new emp_detailMain();

                mst_sessionMain sess = new mst_sessionMain();

                var class_list = mstClass.AllClassList(sess.findFinal_Session());

               

                var emp_list = mstFaculty.DDFacultyList();

                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
                
                IEnumerable<SelectListItem> list2 = new SelectList(emp_list, "user_id", "user_name");


                ViewData["class_id"] = list;
                
                ViewData["subject_teacher_id"] = list2;

                ModelState.AddModelError(String.Empty, "Subject Already Assigned");

                return View(mst);
            }
        }

        [HttpGet]
        public ActionResult DeleteSubjectTeacher(int class_id ,int subject_id ,string session ,int section_id ,int subject_teacher_id )
        {
            mst_class_subject_teacherMain main = new mst_class_subject_teacherMain();

            return View(main.Find_subject_teacher(class_id, subject_id, session, section_id, subject_teacher_id));
        }

        [HttpPost]
        public ActionResult DeleteSubjectTeacher(mst_class_subject_teacher instance)
        {
            mst_class_subject_teacherMain main = new mst_class_subject_teacherMain();

            main.DeleteSubjectTeacher(instance);
       
            return RedirectToAction("AllSubjectTeacher");
        }

        public JsonResult GetSubject(int id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                    a.subject_id,a.subject_name
                                FROM
                                    mst_subject a,
                                    mst_class_subject b
                                WHERE
                                    a.session = b.session
                                        AND a.subject_id = b.subject_id
                                        AND a.session = @session
                                        AND b.class_id = @class_id";



            var subject_list = con.Query<mst_subject>(query, new { class_id = id, session = sess.findFinal_Session() });

            IEnumerable<SelectListItem> list = new SelectList(subject_list, "subject_id", "subject_name");

            return Json(list);



        }

        public JsonResult GetSections(int id)
        {
            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                section_id, section_name
                            FROM
                                mst_section
                            WHERE
                                class_id = @id AND session = @session";



            var section_list = con.Query<mst_section>(query, new { id = id, session = sess.findFinal_Session() });

            IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name");

            return Json(list);

        }
    }

    public class mst_class_subject_teacher
    {
        [Key]
        [Required]
        [Display(Name ="Session")]
        public string session { get; set; }

        [Key]
        [Required]
        [Display(Name = "Class Name")]
        public int class_id { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Key]
        [Required]
        [Display(Name = "Section Name")]
        public int section_id { get; set; }

        [Display(Name = "Section Name")]
        public string section_name { get; set; }

        [Key]
        [Required]
        [Display(Name = "Subject Name")]
        public int subject_id { get; set; }

        [Display(Name = "Subject Name")]
        public string subject_name { get; set; }

        [Key]
        [Required]
        [Display(Name = "Subject Teacher")]
        public int subject_teacher_id { get; set; }

        [Display(Name = "Subject Teacher")]
        public string subject_teacher_name { get; set; }
    }

    public class mst_class_subject_teacherMain
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        public IEnumerable<mst_class_subject_teacher> AllSubjectTeacher()
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                a.session,
                                a.class_id,
                                a.subject_id,
                                c.class_name,
                                a.section_id,
                                d.section_name,
                                b.subject_name,
                                a.subject_teacher_id,
                                (SELECT 
                                        FirstName
                                    FROM
                                        emp_profile
                                    WHERE
                                        user_id = a.subject_teacher_id) subject_teacher_name
                            FROM
                                mst_class_subject_teacher a,
                                mst_subject b,
                                mst_class c,
                                mst_section d
                            WHERE
                                a.class_id = c.class_id
                                    AND a.subject_id = b.subject_id
                                    AND a.section_id = d.section_id
                                    AND a.session = @session
                                    AND b.session = a.session
                                    AND a.session = c.session
                                    AND c.session = d.session";

            var result = con.Query<mst_class_subject_teacher>(query, new { session = sess.findFinal_Session() });

            return result;
        }

        public void AddSubjectTeacher(mst_class_subject_teacher mst)
        {
            try
            {
                mst_sessionMain sess = new mst_sessionMain();



                string query = @"INSERT INTO `mst_class_subject_teacher`
                                    (`session`,
                                    `class_id`,
                                    `section_id`,
                                    `subject_id`,
                                    `subject_teacher_id`)
                                    VALUES
                                    (@session,
                                    @class_id,
                                    @section_id,
                                    @subject_id,
                                    @subject_teacher_id)";

                mst.session = sess.findFinal_Session();

                con.Execute(query, new
                {
                    mst.session,
                    mst.class_id,
                    mst.section_id,
                    mst.subject_id,
                    mst.subject_teacher_id
                    
                });

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public mst_class_subject_teacher Find_subject_teacher(int class_id, int subject_id, string session, int section_id, int subject_teacher_id)
        {
            string query = @"SELECT 
                            a.session,
                            a.class_id,
                            a.subject_id,
                            c.class_name,
                            a.section_id,
                            d.section_name,
                            b.subject_name,
                            a.subject_teacher_id,
                            (SELECT 
                                    FirstName
                                FROM
                                    emp_profile
                                WHERE
                                    user_id = a.subject_teacher_id) subject_teacher_name
                        FROM
                            mst_class_subject_teacher a,
                            mst_subject b,
                            mst_class c,
                            mst_section d
                        WHERE
                            a.class_id = c.class_id
                                AND a.subject_id = b.subject_id
                                AND a.section_id = d.section_id
                                AND a.session = @session
                                AND b.session = a.session
                                AND a.session = c.session
                                AND c.session = d.session
                                AND a.class_id = @class_id
                                AND a.subject_id = @subject_id
                                AND a.section_id = @section_id
                                AND a.subject_teacher_id = @subject_teacher_id";

            var result = con.Query<mst_class_subject_teacher>(query,new {session = session,class_id = class_id,subject_id = subject_id,section_id = section_id,subject_teacher_id = subject_teacher_id }).SingleOrDefault();

            return result;

        }

        public void DeleteSubjectTeacher(mst_class_subject_teacher mst)
        {
            try
            {

                string query = @"DELETE FROM `mst_class_subject_teacher` 
                                WHERE
                                    session = @session
                                    AND class_id = @class_id
                                    AND section_id = @section_id
                                    AND subject_id = @subject_id
                                    AND subject_teacher_id = @subject_teacher_id";

                con.Execute(query, mst);
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
            }
        }
    }
}