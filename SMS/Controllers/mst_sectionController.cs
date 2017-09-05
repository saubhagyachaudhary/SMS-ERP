using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_sectionController : Controller
    {
        [HttpGet]
        public ActionResult AddSection()
        {
            mst_classMain mstClass = new mst_classMain();

            var class_list = mstClass.AllClassList();
            
            IEnumerable<SelectListItem> list = new SelectList(class_list,"class_id","class_name");

            ViewData["class_id"] = list;
            

            return View();
        }

        [HttpPost]
        public ActionResult AddSection(mst_section mst)
        {
            
                try
                {
                    mst_sectionMain mstMain = new mst_sectionMain();
                    mstMain.AddSection(mst);
                    return RedirectToAction("AllSectionList");
                }
                catch (Exception ex)
                {
                mst_classMain mstClass = new mst_classMain();

                var class_list = mstClass.AllClassList();
                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");

                ViewData["class_id"] = list;

                 ModelState.AddModelError(String.Empty, "Section Already Created");

                return View(mst);
                }
            }

        [HttpGet]
        public ActionResult AllSectionList()
        {
            mst_sectionMain stdMain = new mst_sectionMain();

            return View(stdMain.AllSectionList());
        }

       // edit code is perfectly written but commented because of no need : by saubhagya chaudhary

      /*  [HttpGet]
        public ActionResult EditSection(int? id)
        {
            mst_sectionMain stdMain = new mst_sectionMain();

            return View(stdMain.FindSection(id));
        }

        [HttpPost]
        public ActionResult EditSection(mst_section mst)
        {
            mst_sectionMain stdMain = new mst_sectionMain();

            stdMain.EditSection(mst);

            return RedirectToAction("AllSectionList");
        }*/

        [HttpGet]
        public ActionResult DeleteSection(int id)
        {
            mst_sectionMain stdMain = new mst_sectionMain();

            return View(stdMain.FindSection(id));
        }

        [HttpPost]
        public ActionResult DeleteSection(int id, FormCollection collection)
        {
            mst_sectionMain stdMain = new mst_sectionMain();

            stdMain.DeleteSection(id);

            return RedirectToAction("AllSectionList");
        }
    }
}