using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_subjectController : BaseController
    {
        [HttpGet]
        public ActionResult AddSubject()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddSubject(mst_subject mst)
        {
            mst_subjectMain mstMain = new mst_subjectMain();

            mstMain.AddSubject(mst);

            return RedirectToAction("AllSubjectList");
        }

        [HttpGet]
        public ActionResult AllSubjectList()
        {
            mst_subjectMain stdMain = new mst_subjectMain();

            return View(stdMain.AllSubjectList());
        }

        [HttpGet]
        public ActionResult EditSubject(int? id)
        {
            mst_subjectMain stdMain = new mst_subjectMain();

            return View(stdMain.FindSubject(id));
        }

        [HttpPost]
        public ActionResult EditSubject(mst_subject mst)
        {
            mst_subjectMain stdMain = new mst_subjectMain();

            stdMain.EditSubject(mst);

            return RedirectToAction("AllSubjectList");
        }

        [HttpGet]
        public ActionResult DeleteSubject(int id)
        {
            mst_subjectMain stdMain = new mst_subjectMain();

            return View(stdMain.FindSubject(id));
        }

        [HttpPost]
        public ActionResult DeleteSubject(int id, FormCollection collection)
        {
            try
            {
                mst_subjectMain stdMain = new mst_subjectMain();

                stdMain.DeleteSubject(id);

                return RedirectToAction("AllSubjectList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Subject is assigned cannot delete");

                return RedirectToAction("AllSubjectList");
            }
        }
    }
}