using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_termController : BaseController
    {
        [HttpGet]
        public ActionResult AddTerm()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddTerm(mst_term mst)
        {
            mst_termMain mstMain = new mst_termMain();

            mstMain.AddTerm(mst);

            return RedirectToAction("AllTermList");
        }

        [HttpGet]
        public ActionResult AllTermList()
        {
            mst_termMain stdMain = new mst_termMain();

            return View(stdMain.AllTermList());
        }

        [HttpGet]
        public ActionResult EditTerm(int? id)
        {
            mst_termMain stdMain = new mst_termMain();

            return View(stdMain.FindTerm(id));
        }

        [HttpPost]
        public ActionResult EditTerm(mst_term mst)
        {
            mst_termMain stdMain = new mst_termMain();

            stdMain.EditTerm(mst);

            return RedirectToAction("AllTermList");
        }

        [HttpGet]
        public ActionResult DeleteTerm(int id)
        {
            mst_termMain stdMain = new mst_termMain();

            return View(stdMain.FindTerm(id));
        }

        [HttpPost]
        public ActionResult DeleteTerm(int id, FormCollection collection)
        {
            mst_termMain stdMain = new mst_termMain();

            try
            {

                stdMain.DeleteTerm(id);

                return RedirectToAction("AllTermList");
            }
            catch 
            {
                ModelState.AddModelError(String.Empty, "Term is assigned cannot delete");

                return View(stdMain.FindTerm(id));
            }
        }
    }
}