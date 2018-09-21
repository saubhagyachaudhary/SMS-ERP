using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_disciplineController : BaseController
    {
        [HttpGet]
        public ActionResult AddDiscipline()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddDiscipline(mst_discipline mst)
        {
            mst_disciplineMain mstMain = new mst_disciplineMain();

            mstMain.AddDiscipline(mst);

            return RedirectToAction("AllDisciplineList");
        }

        [HttpGet]
        public ActionResult AllDisciplineList()
        {
            mst_disciplineMain stdMain = new mst_disciplineMain();

            return View(stdMain.AllDisciplineList());
        }

        [HttpGet]
        public ActionResult EditDiscipline(int? id)
        {
            mst_disciplineMain stdMain = new mst_disciplineMain();

            return View(stdMain.FindDiscipline(id));
        }

        [HttpPost]
        public ActionResult EditDiscipline(mst_discipline mst)
        {
            mst_disciplineMain stdMain = new mst_disciplineMain();

            stdMain.EditDiscipline(mst);

            return RedirectToAction("AllDisciplineList");
        }

        [HttpGet]
        public ActionResult DeleteDiscipline(int id)
        {
            mst_disciplineMain stdMain = new mst_disciplineMain();

            return View(stdMain.FindDiscipline(id));
        }

        [HttpPost]
        public ActionResult DeleteDiscipline(int id, FormCollection collection)
        {
            try
            {
                mst_disciplineMain stdMain = new mst_disciplineMain();

                stdMain.DeleteDiscipline(id);

                return RedirectToAction("AllDisciplineList");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Discipline area is assigned cannot delete");

                return RedirectToAction("AllDisciplineList");
            }
        }
    }
}