using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_exam_classController : BaseController
    {
        [HttpGet]
        public ActionResult AddExamClass()
        {
            mst_classMain mstClass = new mst_classMain();
            mst_examMain mstsubject = new mst_examMain();


            var class_list = mstClass.AllClassList();

            var exam_list = mstsubject.AllExamList();

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            IEnumerable<SelectListItem> list1 = new SelectList(exam_list, "exam_id", "exam_name");


            ViewData["class_id"] = list;
            ViewData["exam_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddExamClass(mst_exam_class mst)
        {

            try
            {

                mst_exam_classMain main = new mst_exam_classMain();

                main.AddExamClass(mst);

                return RedirectToAction("AllExamClassList");
            }
            catch
            {
                mst_classMain mstClass = new mst_classMain();
                mst_examMain mstsubject = new mst_examMain();


                var class_list = mstClass.AllClassList();

                var exam_list = mstsubject.AllExamList();

                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
                IEnumerable<SelectListItem> list1 = new SelectList(exam_list, "exam_id", "exam_name");


                ViewData["class_id"] = list;
                ViewData["exam_id"] = list1;

                ModelState.AddModelError(String.Empty, "Class Already Assigned");

                return View(mst);
            }
        }



        [HttpGet]
        public ActionResult AllExamClassList()
        {
            mst_exam_classMain stdMain = new mst_exam_classMain();

            return View(stdMain.AllExamClassList());
        }



        [HttpGet]
        public ActionResult DeleteExamClass(int class_id, int exam_id, string session)
        {
            mst_exam_classMain stdMain = new mst_exam_classMain();

            return View(stdMain.FindExamClass(class_id, exam_id, session));
        }

        [HttpPost]
        public ActionResult DeleteExamClass(int class_id, int exam_id, string session, FormCollection collection)
        {
            mst_exam_classMain stdMain = new mst_exam_classMain();


            stdMain.DeleteExamClass(class_id, exam_id, session);
            return RedirectToAction("AllExamClassList");

        }

    }
}