using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class phoneSMSController : Controller
    {
        // GET: phoneSMS
        [HttpGet]
        public ActionResult smsSend()
        {
            return View();
        }

        [HttpPost]
        public ActionResult smsSend(phoneSMS ph)
        {
            return View(ph);
        }
    }
}