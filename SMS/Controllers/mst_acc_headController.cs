using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_acc_headController : BaseController
    {
        [HttpGet]
        public ActionResult AddAccount()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddAccount(mst_acc_head mst)
        {
            mst_acc_headMain mstMain = new mst_acc_headMain();

            mstMain.AddHead(mst);
            
            return RedirectToAction("AllAccountList");
        }

        [HttpGet]
        public ActionResult AllAccountList()
        {
            mst_acc_headMain stdMain = new mst_acc_headMain();

            return View(stdMain.AllAccountList());
        }

        [HttpGet]
        public ActionResult EditAccount(int id)
        {
            mst_acc_headMain stdMain = new mst_acc_headMain();

            return View(stdMain.FindAccount(id));
        }

        [HttpPost]
        public ActionResult EditAccount(mst_acc_head mst)
        {
            mst_acc_headMain stdMain = new mst_acc_headMain();

            stdMain.EditAccount(mst);

            return RedirectToAction("AllAccountList");
        }

        [HttpGet]
        public ActionResult DeleteAccount(int id)
        {
            mst_acc_headMain stdMain = new mst_acc_headMain();

            return View(stdMain.FindAccount(id));
        }

        [HttpPost]
        public ActionResult DeleteAccount(int id, FormCollection collection)
        {
            try
            {
                mst_acc_headMain stdMain = new mst_acc_headMain();

                stdMain.DeleteAccount(id);

                return RedirectToAction("AllAccountList");
            }
            catch 
            {
                // ModelState.AddModelError(String.Empty, "Sections are created cannot delete");

                return RedirectToAction("AllAccountList");
            }
        }
    }
}