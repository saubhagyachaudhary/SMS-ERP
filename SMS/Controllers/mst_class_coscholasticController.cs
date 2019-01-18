using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_class_coscholasticController : BaseController
    {
        [HttpGet]
        public ActionResult AddClassCoScholastic()
        {
            mst_classMain mstClass = new mst_classMain();
            mst_co_scholasticMain mstcoscholastic = new mst_co_scholasticMain();

            mst_sessionMain sess = new mst_sessionMain();

            var class_list = mstClass.AllClassList(sess.findFinal_Session());

            var coscholastic_list = mstcoscholastic.AllCoScholasticList();

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            IEnumerable<SelectListItem> list1 = new SelectList(coscholastic_list, "co_scholastic_id", "co_scholastic_name");


            ViewData["class_id"] = list;
            ViewData["coscholastic_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddClassCoScholastic(mst_class_coscholastic mst)
        {

            try
            {

                mst_class_coscholasticMain main = new mst_class_coscholasticMain();

                main.AddClassCoscholastic(mst);

                return RedirectToAction("AllClassCoScholasticList");
            }
            catch
            {
                mst_classMain mstClass = new mst_classMain();
                mst_co_scholasticMain mstcoscholastic = new mst_co_scholasticMain();

                mst_sessionMain sess = new mst_sessionMain();
                var class_list = mstClass.AllClassList(sess.findFinal_Session());

                var coscholastic_list = mstcoscholastic.AllCoScholasticList();

                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
                IEnumerable<SelectListItem> list1 = new SelectList(coscholastic_list, "co_scholastic_id", "co_scholastic_name");


                ViewData["class_id"] = list;
                ViewData["coscholastic_id"] = list1;

                ModelState.AddModelError(String.Empty, "Co-Scholastic area Already Assigned");

                return View(mst);
            }
        }



        [HttpGet]
        public ActionResult AllClassCoScholasticList()
        {
            mst_class_coscholasticMain stdMain = new mst_class_coscholasticMain();

            return View(stdMain.AllClassCoscholasticList());
        }



        [HttpGet]
        public ActionResult DeleteClassCoScholastic(int class_id, int co_scholastic_id, string session)
        {
            mst_class_coscholasticMain stdMain = new mst_class_coscholasticMain();

            return View(stdMain.FindCoscholasticClass(class_id, co_scholastic_id, session));
        }

        [HttpPost]
        public ActionResult DeleteClassCoScholastic(int class_id, int co_scholastic_id, string session, FormCollection collection)
        {
            mst_class_coscholasticMain stdMain = new mst_class_coscholasticMain();


            stdMain.DeleteCoscholasticClass(class_id, co_scholastic_id, session);
            return RedirectToAction("AllClassCoScholasticList");

        }
    }
}