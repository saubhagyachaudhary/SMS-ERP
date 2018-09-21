using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class enable_wedgetController : Controller
    {
        [HttpGet]
        public ActionResult AllUserList()
        {
            enable_wedgetMain main = new enable_wedgetMain();

            return View(main.AllUserList());
        }
        [HttpGet]
        public ActionResult EditWedget(int user_id)
        {
            enable_wedgetMain main = new enable_wedgetMain();

            return View(main.AllWedgetList(user_id));
        }

        [HttpPost]
        public ActionResult EditWedget(List<enable_wedget> wedget)
        {
            enable_wedgetMain mstMain = new enable_wedgetMain();

            foreach (var feat in wedget)
            {
                if (feat.active)
                    mstMain.AddWedget(feat);
                else
                    mstMain.DeleteWedget(feat);
            }



            return RedirectToAction("AllUserList");
        }
    }
}