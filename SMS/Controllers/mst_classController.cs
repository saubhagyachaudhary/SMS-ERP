using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_classController : BaseController
    {
        [HttpGet]
        public ActionResult AddClass()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddClass(mst_class mst)
        {
            mst_classMain mstMain = new mst_classMain();

            mstMain.AddClass(mst);

            return RedirectToAction("AllClassList");
        }

        [HttpGet]
        public ActionResult AllClassList()
        {
            mst_classMain stdMain = new mst_classMain();

            return View(stdMain.AllClassList());
        }

        [HttpGet]
        public ActionResult EditClass(int? id)
        {
            mst_classMain stdMain = new mst_classMain();

            return View(stdMain.FindClass(id));
        }

        [HttpPost]
        public ActionResult EditClass(mst_class mst)
        {
            mst_classMain stdMain = new mst_classMain();

            stdMain.EditClass(mst);

            return RedirectToAction("AllClassList");
        }

        [HttpGet]
        public ActionResult DeleteClass(int id)
        {
            mst_classMain stdMain = new mst_classMain();

            return View(stdMain.FindClass(id));
        }

        [HttpPost]
        public ActionResult DeleteClass(int id, FormCollection collection)
        {
            try
            {
                mst_classMain stdMain = new mst_classMain();

                stdMain.DeleteClass(id);

                return RedirectToAction("AllClassList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Sections are created cannot delete");

                return RedirectToAction("AllClassList");
            }
        }
    }
}