using SMS.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class std_discountController : BaseController
    {
        [HttpGet]
        public ActionResult AddDiscount()
        {
            mst_feesMain mstfess = new mst_feesMain();
            
            var acc_head = mstfess.account_head();

            IEnumerable<SelectListItem> list1 = new SelectList(acc_head, "acc_id", "acc_name");

            ViewData["acc_id"] = list1;

            return View();
        }

        [HttpPost]
        public ActionResult AddDiscount(std_discount std)
        {
            try
            {
                std_discountMain main = new std_discountMain();
                main.AddFees(std);
                return RedirectToAction("AllStdDiscountList");
            }
            catch 
            {
                mst_feesMain mstfess = new mst_feesMain();

                var acc_head = mstfess.account_head();

                IEnumerable<SelectListItem> list1 = new SelectList(acc_head, "acc_id", "acc_name");

                ViewData["acc_id"] = list1;

               
                ModelState.AddModelError(String.Empty, "Discount on Admission number already applied for the particular account head or Admission number not exist");  

                return View(std);
            }
        }

        [HttpGet]
        public ActionResult AllStdDiscountList()
        {
            std_discountMain stdMain = new std_discountMain();

            return View(stdMain.AllStdDiscountList());
        }

        [HttpGet]
        public ActionResult EditDiscount(int sr_num, int acc_id,string session)
        {
            std_discountMain stdMain = new std_discountMain();

            return View(stdMain.FindDiscount(sr_num, acc_id));
        }

        [HttpPost]
        public ActionResult EditDiscount(std_discount mst)
        {

            std_discountMain stdMain = new std_discountMain();

            stdMain.EditDiscount(mst);

            return RedirectToAction("AllStdDiscountList");
        }


        [HttpGet]
        public ActionResult DeleteDiscount(int sr_num, int acc_id, string session)
        {
            std_discountMain stdMain = new std_discountMain();

            return View(stdMain.FindDiscount(sr_num, acc_id));
        }

        [HttpPost]
        public ActionResult DeleteDiscount(int sr_num, int acc_id, string session, FormCollection collection)
        {
            std_discountMain stdMain = new std_discountMain();

            stdMain.DeleteDiscount(sr_num, acc_id,session);

            return RedirectToAction("AllStdDiscountList");
        }

    }
}