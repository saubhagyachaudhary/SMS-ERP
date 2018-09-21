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
    public class mst_class_subjectController : BaseController
    {
        [HttpGet]
        public ActionResult AddClassSubject()
        {
            mst_classMain mstClass = new mst_classMain();
            mst_subjectMain mstsubject = new mst_subjectMain();


            var class_list = mstClass.AllClassList();

            var subject_list = mstsubject.AllSubjectList();

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            IEnumerable<SelectListItem> list1 = new SelectList(subject_list, "subject_id", "subject_name");


            ViewData["class_id"] = list;
            ViewData["subject_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddClassSubject(mst_class_subject mst)
        {

            try
            {

                mst_class_subjectMain main = new mst_class_subjectMain();

                main.AddClassSubject(mst);

                return RedirectToAction("AllClassSubjectList");
            }
            catch
            {
                mst_classMain mstClass = new mst_classMain();
                mst_subjectMain mstsubject = new mst_subjectMain();


                var class_list = mstClass.AllClassList();

                var subject_list = mstsubject.AllSubjectList();

                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
                IEnumerable<SelectListItem> list1 = new SelectList(subject_list, "subject_id", "subject_name");


                ViewData["class_id"] = list;
                ViewData["subject_id"] = list1;

                ModelState.AddModelError(String.Empty, "Subject Already Assigned");

                return View(mst);
            }
        }



        [HttpGet]
        public ActionResult AllClassSubjectList()
        {
            mst_class_subjectMain stdMain = new mst_class_subjectMain();

            return View(stdMain.AllClassSubjectList());
        }

      

        [HttpGet]
        public ActionResult DeleteClassSubject(int class_id, int subject_id, string session)
        {
            mst_class_subjectMain stdMain = new mst_class_subjectMain();

            return View(stdMain.FindSubjectClass(class_id, subject_id, session));
        }

        [HttpPost]
        public ActionResult DeleteClassSubject(int class_id, int subject_id, string session, FormCollection collection)
        {
            mst_class_subjectMain stdMain = new mst_class_subjectMain();

           
           stdMain.DeleteSubjectClass(class_id, subject_id, session);
           return RedirectToAction("AllClassSubjectList");
          
        }
    }
}