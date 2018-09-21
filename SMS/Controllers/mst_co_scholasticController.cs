using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_co_scholasticController : BaseController
    {
        [HttpGet]
        public ActionResult AddCoScholastic()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCoScholastic(mst_co_scholastic mst)
        {
            mst_co_scholasticMain mstMain = new mst_co_scholasticMain();

            mstMain.AddCoScholastic(mst);

            return RedirectToAction("AllCoScholasticList");
        }

        [HttpGet]
        public ActionResult AllCoScholasticList()
        {
            mst_co_scholasticMain stdMain = new mst_co_scholasticMain();

            return View(stdMain.AllCoScholasticList());
        }

        [HttpGet]
        public ActionResult EditCoScholastic(int? id)
        {
            mst_co_scholasticMain stdMain = new mst_co_scholasticMain();

            return View(stdMain.FindCoScholastic(id));
        }

        [HttpPost]
        public ActionResult EditCoScholastic(mst_co_scholastic mst)
        {
            mst_co_scholasticMain stdMain = new mst_co_scholasticMain();

            stdMain.EditCoScholastic(mst);

            return RedirectToAction("AllCoScholasticList");
        }

        [HttpGet]
        public ActionResult DeleteCoScholastic(int id)
        {
            mst_co_scholasticMain stdMain = new mst_co_scholasticMain();

            return View(stdMain.FindCoScholastic(id));
        }

        [HttpPost]
        public ActionResult DeleteCoScholastic(int id, FormCollection collection)
        {
            try
            {
                mst_co_scholasticMain stdMain = new mst_co_scholasticMain();

                stdMain.DeleteCoScholastic(id);

                return RedirectToAction("AllCoScholasticList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Co Scholastic area assigned cannot delete");

                return RedirectToAction("AllCoScholasticList");
            }
        }
    }
}