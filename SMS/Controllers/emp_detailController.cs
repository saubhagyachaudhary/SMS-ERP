using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class emp_detailController : BaseController
    {
        [HttpGet]
        public ActionResult EmpList()
        {
            emp_detailMain emp = new emp_detailMain();
            return View(emp.AllEmpList());
        }

        [HttpGet]
        public ActionResult AddEmp()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddEmp(emp_detail emp)
        {
            emp_detailMain empMain = new emp_detailMain();

            empMain.AddEmployee(emp);

            return RedirectToAction("EmpList");
        }

        [HttpGet]
        public ActionResult EditEmp(int user_id)
        {
            emp_detailMain empMain = new emp_detailMain();

            return View(empMain.FindEmployee(user_id));
        }

        [HttpPost]
        public ActionResult EditEmp(emp_detail emp)
        {
            emp_detailMain empMain = new emp_detailMain();

            empMain.EditEmp(emp);

            return RedirectToAction("EmpList");
        }

        [HttpGet]
        public ActionResult DetailEmp(int user_id)
        {
            emp_detailMain empMain = new emp_detailMain();

            return View(empMain.FindEmployee(user_id));
        }
    }
}