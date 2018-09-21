using Dapper;
using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace SMS.Controllers
{
    public class std_registrationController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

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
        public async Task<ActionResult> AddRegistration(std_registration mst)
        {
            
            std_registrationMain mstMain = new std_registrationMain();
           

            mst.reg_date = System.DateTime.Now.AddMinutes(dateTimeOffSet);

            if (mst.std_contact == null)
            {
                ModelState.AddModelError(String.Empty, "Primary contact is mandatory.");
                mst_classMain mstClass = new mst_classMain();

                var class_list = mstClass.AllClassList();

                IEnumerable<SelectListItem> list1 = new SelectList(class_list, "class_id", "class_name");

                ViewData["class_id"] = list1;

                return View(mst);
            }



            await mstMain.AddRegistration(mst);


           


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
                string query = @"select count(*) from out_standing where reg_num = @reg and rmt_amount = 0 and session = @session";

                int count = con.Query<int>(query, new { reg = reg,session = sess }).SingleOrDefault();

                if(count > 0)
                {
                    std_registrationMain stdMain = new std_registrationMain();

                    stdMain.DeleteRegistration(sess, reg, dt);

                    return RedirectToAction("AllRegistrationList");

                }
                else
                {
                    //ModelState.AddModelError(String.Empty, "Payment made cannot delete the registration.");

                    //std_registrationMain stdMain = new std_registrationMain();

                    //ViewBag.message = "Payment made cannot delete the registration.";

                    return View("cannotdelete");
                }


               
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