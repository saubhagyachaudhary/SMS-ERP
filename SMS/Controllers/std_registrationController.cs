using Dapper;
using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class std_registrationController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddRegistration()
        {
            mst_classMain mstClass = new mst_classMain();

            

            var class_list = mstClass.AllClassList();

            IEnumerable<SelectListItem> list1 = new SelectList(class_list, "class_id", "class_name");

            ViewData["class_id"] = list1;
            
            return View();
        }

        [HttpPost]
        public ActionResult AddRegistration(std_registration mst)
        {
            
            std_registrationMain mstMain = new std_registrationMain();
           

            mst.reg_date = System.DateTime.Now;

           
                mstMain.AddRegistration(mst);

               
           
            return RedirectToAction("AllRegistrationList");
        }

        

      

        public JsonResult GetFees(int id)
        {
            string query = "select fees_amount from mst_fees where class_id = @class_id and acc_id = 1";


            decimal fees = con.Query<decimal>(query, new { class_id = id }).SingleOrDefault();

            

            return Json(fees);

        }

        [HttpGet]
        public ActionResult AllRegistrationList()
        {
            std_registrationMain stdMain = new std_registrationMain();

            return View(stdMain.AllRegistrationList());
        }

        [HttpGet]
        public ActionResult EditRegistration(string sess,int reg, DateTime dt)
        {
            std_registrationMain stdMain = new std_registrationMain();
            var obj = stdMain.FindRegistration(sess, reg, dt);
            DDclass_name(obj);
            return View(obj);
        }

        [HttpPost]
        public ActionResult EditRegistration(std_registration mst)
        {
            std_registrationMain stdMain = new std_registrationMain();

            stdMain.EditRegistration(mst);

            return RedirectToAction("AllRegistrationList");
        }

        [HttpGet]
        public ActionResult DeleteRegistration(string sess, int reg, DateTime dt)
        {
            std_registrationMain stdMain = new std_registrationMain();

            return View(stdMain.FindRegistration(sess,reg,dt));
        }

        [HttpPost]
        public ActionResult DeleteRegistration(string sess, int reg, DateTime dt, FormCollection collection)
        {
            try
            {
                std_registrationMain stdMain = new std_registrationMain();

                stdMain.DeleteRegistration(sess, reg, dt);

                return RedirectToAction("AllRegistrationList");
            }
            catch (Exception ex)
            {
                // ModelState.AddModelError(String.Empty, "Sections are created cannot delete");

                return RedirectToAction("AllRegistrationList");
            }
        }

        public void DDclass_name(std_registration obj)
        {
            mst_classMain mstClass = new mst_classMain();

            var class_list = mstClass.AllClassList();

            IEnumerable<SelectListItem> list1 = new SelectList(class_list, "class_id", "class_name",obj.std_class_id);
            
            ViewData["class_id"] = list1;


            
        }

    }
}