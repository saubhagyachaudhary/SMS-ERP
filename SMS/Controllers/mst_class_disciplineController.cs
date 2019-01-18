using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_class_disciplineController : BaseController
    {
        [HttpGet]
        public ActionResult AddClassDiscipline()
        {
            mst_classMain mstClass = new mst_classMain();
            mst_disciplineMain mstdiscipline = new mst_disciplineMain();

            mst_sessionMain sess = new mst_sessionMain();

            var class_list = mstClass.AllClassList(sess.findFinal_Session());

            var discipline_list = mstdiscipline.AllDisciplineList();

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            IEnumerable<SelectListItem> list1 = new SelectList(discipline_list, "discipline_id", "discipline_name");


            ViewData["class_id"] = list;
            ViewData["discipline_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddClassDiscipline(mst_class_discipline mst)
        {

            try
            {

                mst_class_disciplineMain main = new mst_class_disciplineMain();

                main.AddClassDiscipline(mst);

                return RedirectToAction("AllClassDisciplineList");
            }
            catch
            {
                mst_classMain mstClass = new mst_classMain();
                mst_disciplineMain mstdescipline = new mst_disciplineMain();

                mst_sessionMain sess = new mst_sessionMain();

                var class_list = mstClass.AllClassList(sess.findFinal_Session());

                var discipline_list = mstdescipline.AllDisciplineList();

                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
                IEnumerable<SelectListItem> list1 = new SelectList(discipline_list, "discipline_id", "discipline_name");


                ViewData["class_id"] = list;
                ViewData["discipline_id"] = list1;

                ModelState.AddModelError(String.Empty, "Discipline Area Already Assigned");

                return View(mst);
            }
        }



        [HttpGet]
        public ActionResult AllClassDisciplineList()
        {
            mst_class_disciplineMain stdMain = new mst_class_disciplineMain();

            return View(stdMain.AllClassDisciplineList());
        }



        [HttpGet]
        public ActionResult DeleteClassDiscipline(int class_id, int discipline_id, string session)
        {
            mst_class_disciplineMain stdMain = new mst_class_disciplineMain();

            return View(stdMain.FindDisciplineClass(class_id, discipline_id, session));
        }

        [HttpPost]
        public ActionResult DeleteClassDiscipline(int class_id, int discipline_id, string session, FormCollection collection)
        {
            mst_class_disciplineMain stdMain = new mst_class_disciplineMain();


            stdMain.DeleteDisciplineClass(class_id, discipline_id, session);
            return RedirectToAction("AllClassDisciplineList");

        }
    }
}