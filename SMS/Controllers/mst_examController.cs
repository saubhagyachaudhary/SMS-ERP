using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_examController : BaseController
    {
            [HttpGet]
            public ActionResult AddExam()
            {
                return View();
            }

            [HttpPost]
            public ActionResult AddExam(mst_exam mst)
            {
                mst_examMain mstMain = new mst_examMain();

                mstMain.AddExam(mst);

                return RedirectToAction("AllExamList");
            }

            [HttpGet]
            public ActionResult AllExamList()
            {
                mst_examMain stdMain = new mst_examMain();

                return View(stdMain.AllExamList());
            }

            [HttpGet]
            public ActionResult EditExam(int? id)
            {
                mst_examMain stdMain = new mst_examMain();

                return View(stdMain.FindExam(id));
            }

            [HttpPost]
            public ActionResult EditExam(mst_exam mst)
            {
                mst_examMain stdMain = new mst_examMain();

                stdMain.EditExam(mst);

                return RedirectToAction("AllExamList");
            }

            [HttpGet]
            public ActionResult DeleteExam(int id)
            {
                mst_examMain stdMain = new mst_examMain();

                return View(stdMain.FindExam(id));
            }

            [HttpPost]
            public ActionResult DeleteExam(int id, FormCollection collection)
            {
                try
                {
                    mst_examMain stdMain = new mst_examMain();

                    stdMain.DeleteExam(id);

                    return RedirectToAction("AllExamList");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "Exam is assigned cannot delete");

                    return RedirectToAction("AllExamList");
                }
            }
        }
    
}