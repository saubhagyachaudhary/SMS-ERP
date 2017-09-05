using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_feesController : Controller
    {

        [HttpGet]
        public ActionResult AddFees()
        {
            mst_classMain mstClass = new mst_classMain();
            mst_feesMain mstfess = new mst_feesMain();


            var class_list = mstClass.AllClassList();

            var acc_head = mstfess.account_head();

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");
            IEnumerable<SelectListItem> list1 = new SelectList(acc_head, "acc_id", "acc_name");


            ViewData["class_id"] = list;
            ViewData["acc_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddFees(mst_fees mst)
        {

            try
            {
                mst_feesMain mstMain = new mst_feesMain();
                mstMain.AddFees(mst);
                return RedirectToAction("AllFeesList");
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
        public ActionResult AllFeesList()
        {
            mst_feesMain stdMain = new mst_feesMain();

            return View(stdMain.AllFeesList());
        }

        [HttpGet]
        public ActionResult EditFees(int class_id, int acc_id)
        {
            mst_feesMain stdMain = new mst_feesMain();

            return View(stdMain.Findfees(class_id, acc_id));
        }

        [HttpPost]
        public ActionResult EditFees(mst_fees mst)
        {
            mst_feesMain stdMain = new mst_feesMain();

            stdMain.EditFees(mst);

            return RedirectToAction("AllFeesList");
        }

        [HttpGet]
        public ActionResult DeleteFees(int class_id, int acc_id)
        {
            mst_feesMain stdMain = new mst_feesMain();

            return View(stdMain.Findfees(class_id, acc_id));
        }

        [HttpPost]
        public ActionResult DeleteFees(int class_id, int acc_id, FormCollection collection)
        {
            mst_feesMain stdMain = new mst_feesMain();

            stdMain.DeleteFees(class_id, acc_id);

            return RedirectToAction("AllFeesList");
        }
    }
}