using Dapper;
using SMS.Models;
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
    public class sr_registerController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult AddStudent(string sess,int reg,DateTime dt)
        {
            std_registrationMain std_reg = new std_registrationMain();

            std_registration obj =  std_reg.FindRegistration(sess, reg, dt);

            sr_register std = new sr_register();

            std.std_first_name = obj.std_first_name;
            std.std_last_name = obj.std_last_name;
            std.adm_session = obj.session;
            std.class_id = obj.std_class_id;
            std.reg_no = obj.reg_no;
            std.reg_date = obj.reg_date;
            std.std_active = "Y";
            std.std_address = obj.std_address;
            std.std_address1 = obj.std_address1;
            std.std_address2 = obj.std_address2;
           
            std.std_admission_date_str = DateTime.Now.ToShortDateString();
            std.std_contact = obj.std_contact;
            std.std_contact1 = obj.std_contact1;
            std.std_contact2 = obj.std_contact2;
            std.std_country = obj.std_country;
            std.std_district = obj.std_district;
            std.std_email = obj.std_email;
            std.std_father_name = obj.std_father_name;
            std.std_mother_name = obj.std_mother_name;
            std.std_pincode = obj.std_pincode;
            std.std_state = obj.std_state;
            
            DDclass_name(std);
         
            DDtransport_id(std);

            return View(std);
        }

        [HttpPost]
        public async Task<ActionResult> AddStudent(sr_register std)
        {
            try
            {
                if (std.std_contact == null)
                {
                    ModelState.AddModelError(String.Empty, "Primary contact is mandatory.");


                    DDclass_name(std);

                    DDtransport_id(std);

                    return View(std);
                }
                sr_registerMain stdMain = new sr_registerMain();

                string query = "select class_id from mst_class where class_name = @std_admission_class";

                int id = con.ExecuteScalar<int>(query, new { std.std_admission_class });

                if (std.class_id < id)
                {
                    ModelState.AddModelError(String.Empty, "Class cannot be lower than admission class");


                    DDclass_name(std);

                    DDtransport_id(std);

                    return View(std);
                }

                if (std.std_pickup_id == null)
                {
                    ModelState.AddModelError(String.Empty, "Avail Transport cannot be blank.");


                    DDclass_name(std);

                    DDtransport_id(std);

                    return View(std);
                }

                await stdMain.AddStudent(std);

                

                return RedirectToAction("AllRegistrationList", "std_registration");
            }
            catch
            {
                return RedirectToAction("AllRegistrationList","std_registration");
            }
        }

        public JsonResult GetFees(String id)
        {
            

            string query = @" SELECT fees_amount
                          FROM mst_fees a, mst_class b
                          where
                          a.class_id = b.class_id
                          and
                          b.class_name = @class_name 
                          and acc_id = 2";

            decimal fees = con.Query<decimal>(query, new { class_name = id }).SingleOrDefault();



            return Json(fees);

        }

        public void DDclass_name(sr_register obj)
        {
            mst_classMain mstClass = new mst_classMain();

            var class_list = mstClass.AllClassList();

            IEnumerable<SelectListItem> list1 = new SelectList(class_list, "class_id", "class_name",obj.class_id);
            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_name", "class_name");

            ViewData["class_id"] = list1;


            ViewData["class_name"] = list;
        }

        
        public void DDtransport_id(sr_register obj)
        {
            mst_transportMain mstTransport = new mst_transportMain();

            var transport_list = mstTransport.AllTransportList();

            IEnumerable<SelectListItem> list2 = new SelectList(transport_list, "pickup_id", "pickup_point");

            ViewData["pickup_id"] = list2;
        }

      

        public JsonResult GetSections(int id)
        {
            string query = "select section_id,section_name from mst_section where class_id = @class_id";

          
           var section_list = con.Query<mst_section>(query,new { class_id = id});

            IEnumerable<SelectListItem> list = new SelectList(section_list,"section_id","section_name");

            return Json(list);

        }

        public void DDSections(sr_register obj)
        {
            string query = "select section_id,section_name from mst_section where class_id = @class_id";


            var section_list = con.Query<mst_section>(query, new { class_id = obj.class_id});

            IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name",obj.std_section_id);

            ViewData["section_id"] = list;

        }

        public void DDClassWiseSection()
        {

            mst_sessionMain sess = new mst_sessionMain();

           string query = @"select a.section_id,concat(ifnull(b.class_name,''),' Section ',ifnull(a.section_name,'')) Section_name from mst_section a,mst_class b
                            where
                            a.class_id = b.class_id
                            and 
                            session = @session
                            order by b.class_name";


            var section_list = con.Query<mst_section>(query,new {session =  sess.findActive_finalSession() });

            IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name");

            ViewData["section_id"] = list;

        }


        [HttpGet]
        public ActionResult AllStudentList()
        {
            sr_registerMain stdMain = new sr_registerMain();
            sr_register sr = new sr_register();

            sr.sr_regi = stdMain.AllStudentList(103);
            DDClassWiseSection();
            return View(sr);
        }

        [HttpPost]
        public ActionResult AllStudentList(int section_id)
        {
            sr_registerMain stdMain = new sr_registerMain();
            sr_register sr = new sr_register();

            sr.sr_regi = stdMain.AllStudentList(section_id);
            DDClassWiseSection();
            return View(sr);
        }

        //[HttpGet]
        //public ActionResult AllNSOStudentList()
        //{
        //    sr_registerMain stdMain = new sr_registerMain();
        //    sr_register sr = new sr_register();

        //    sr.sr_regi = stdMain.AllNSOStudentList(103);
        //    DDClassWiseSection();

           
        //    return View("AllStudentList", sr);
        //}


        [HttpGet]
        public ActionResult StudentDetails(int id)
        {
            sr_registerMain stdMain = new sr_registerMain();

            return View(stdMain.FindStudent(id));
        }

        [HttpGet]
        public ActionResult EditDetails(int id)
        {
            sr_registerMain stdMain = new sr_registerMain();
            var obj = stdMain.FindStudent(id);

            DDclass_name(obj);
            DDtransport_id(obj);
            DDSections(obj);
          

            if (obj.std_active == "Y")
            {
                obj.active = true;
            }
            else
            {
                obj.active = false;
            }


            return View(obj);
        }

        [HttpPost]
        public ActionResult EditDetails(sr_register std)
        {
            sr_registerMain stdMain = new sr_registerMain();

            string query = "select class_id from mst_class where class_name = @std_admission_class";

            int id = con.ExecuteScalar<int>(query, new { std.std_admission_class });

            if (std.class_id < id)
            {
                ModelState.AddModelError(String.Empty, "Class cannot be lower than admission class");


                DDclass_name(std);

                DDtransport_id(std);

                DDSections(std);

                return View(std);
            }

            query = @"select class_id from sr_register a, mst_batch b
where
a.std_batch_id = b.batch_id
and sr_number = @sr_num";

            int changedclassid = con.Query<int>(query, new { sr_num = std.sr_number }).SingleOrDefault();

             query = @"select ifnull(count(CASE 
WHEN rmt_amount = 0.00  THEN null 
else rmt_amount end),0) from out_standing where sr_number = @sr_num  and serial != 0  and session = (SELECT session FROM mst_session where session_active = 'Y') and acc_id not in (1,2,6)";

            int error = con.Query<int>(query, new { sr_num = std.sr_number }).SingleOrDefault();



            if (error != 0 && changedclassid != std.class_id)
            {
                ModelState.AddModelError(String.Empty, "Cannot change, class fees already paid");

                DDclass_name(std);

                DDtransport_id(std);

                DDSections(std);

                return View(std);

            }

            if (std.active)
            {
                std.std_active = "Y";
            }
            else
            {
                std.std_active = "N";
            }

            stdMain.EditStudent(std);



            return RedirectToAction("AllStudentList");
        }

        [HttpGet]
        public ActionResult DeleteStudent(int id)
        {
            sr_registerMain stdMain = new sr_registerMain();

            return View(stdMain.FindStudent(id));
        }

        [HttpPost]
        public ActionResult DeleteStudent(int id, FormCollection collection)
        {
            sr_registerMain stdMain = new sr_registerMain();

            stdMain.DeleteStudent(id);

            return RedirectToAction("AllStudentList");
        }
    }
}