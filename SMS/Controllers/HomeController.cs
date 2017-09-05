using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
       [Authorize(Roles = "superadmin,admin")]
        public ActionResult Dashboard()
        {
            return View();
        }
    }
}