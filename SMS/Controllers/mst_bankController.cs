using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class mst_bankController : Controller
    {
        [HttpGet]
        public ActionResult AddBank()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddBank(bank_master mst)
        {
           
            bank_masterMain.AddBank(mst);

            return RedirectToAction("AllBankList");
        }

        [HttpGet]
        public ActionResult AllBankList()
        {
            
            return View(bank_masterMain.AllBankList());
        }

        [HttpGet]
        public ActionResult EditBank(int? id)
        {
           
            return View(bank_masterMain.FindBank(id));
        }

        [HttpPost]
        public ActionResult EditBank(bank_master mst)
        {
           
            bank_masterMain.EditBank(mst);

            return RedirectToAction("AllBankList");
        }

        [HttpGet]
        public ActionResult DeleteBank(int id)
        {
           
            return View(bank_masterMain.FindBank(id));
        }

        [HttpPost]
        public ActionResult DeleteBank(int id, FormCollection collection)
        {
           
                
                bank_masterMain.DeleteBank(id);

                return RedirectToAction("AllBankList");
          
        }
    }
}