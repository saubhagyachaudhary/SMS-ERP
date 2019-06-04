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
                if (ModelState.IsValid)
                {
                    if (std.std_contact == null)
                    {
                        ModelState.AddModelError(String.Empty, "Primary contact is mandatory.");


                        DDclass_name(std);

                        DDtransport_id(std);

                        return View(std);
                    }
                    sr_registerMain stdMain = new sr_registerMain();

                    string query = @"SELECT 
                                    class_id
                                FROM
                                    mst_class
                                WHERE
                                    class_name = @std_admission_class
                                        AND session = (SELECT
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_active = 'Y')";

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

                DDclass_name(std);

                DDtransport_id(std);

                return View(std);
            }
            catch
            {
                return RedirectToAction("AllRegistrationList","std_registration");
            }
        }

        public JsonResult GetFees(String id)
        {
            

            string query = @"SELECT 
                                    fees_amount
                                FROM
                                    mst_fees a,
                                    mst_class b
                                WHERE
                                    a.class_id = b.class_id
                                        AND b.class_name = @class_name
                                        AND a.acc_id = 2
                                        AND a.session = b.session
                                        AND b.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_active = 'Y')";

            decimal fees = con.Query<decimal>(query, new { class_name = id }).SingleOrDefault();



            return Json(fees);

        }

        public void DDclass_name(sr_register obj)
        {
            mst_classMain mstClass = new mst_classMain();

            mst_sessionMain sess = new mst_sessionMain();

            var class_list = mstClass.AllClassList(sess.findFinal_Session());

            var adm_class_list = mstClass.AllClassList(obj.adm_session);

            IEnumerable<SelectListItem> list1 = new SelectList(class_list, "class_id", "class_name",obj.class_id);
            IEnumerable<SelectListItem> list = new SelectList(adm_class_list, "class_name", "class_name");

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
            string query = @"SELECT 
                                section_id, section_name
                            FROM
                                mst_section
                            WHERE
                                class_id = @class_id
                                    AND session = (SELECT
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_active = 'Y')";



           var section_list = con.Query<mst_section>(query,new { class_id = id});

            IEnumerable<SelectListItem> list = new SelectList(section_list,"section_id","section_name");

            return Json(list);

        }

        public void DDSections(sr_register obj)
        {
            string query = @"SELECT 
                                    section_id, section_name
                                FROM
                                    mst_section
                                WHERE
                                    class_id = @class_id
                                        AND session = (SELECT
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_active = 'Y')";


            var section_list = con.Query<mst_section>(query, new { class_id = obj.class_id});

            IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name",obj.std_section_id);

            ViewData["section_id"] = list;

        }

        public void DDClassWiseSection()
        {

            mst_sessionMain sess = new mst_sessionMain();

           string query = @"SELECT 
                                a.section_id,
                                CONCAT(IFNULL(b.class_name, ''),
                                        ' Section ',
                                        IFNULL(a.section_name, '')) Section_name
                            FROM
                                mst_section a,
                                mst_class b
                            WHERE
                                a.class_id = b.class_id
                                    AND a.session = @session
                                    AND a.session = b.session
                            ORDER BY b.order_by";


            var section_list = con.Query<mst_section>(query,new {session =  sess.findFinal_Session() });

            IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name");

            ViewData["section_id"] = list;

        }


        [HttpGet]
        public ActionResult AllStudentList()
        {
            sr_registerMain stdMain = new sr_registerMain();
            sr_register sr = new sr_register();

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"SELECT 
                                a.section_id
                            FROM
                                mst_section a,
                                mst_class b
                            WHERE
                                a.class_id = b.class_id
                                    AND a.session = @session
                                    AND a.session = b.session
                            ORDER BY b.order_by 
                            LIMIT 1";

            int section = con.Query<int>(query, new { session = sess.findFinal_Session() }).SingleOrDefault();

            sr.sr_regi = stdMain.AllStudentList(section);
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

        [HttpGet]
        public ActionResult FormViewer(int id)
        {
            string query = @"SELECT 
                                    adm_form_link
                                FROM
                                    hariti.sr_register
                                WHERE
                                    sr_number = @id AND std_active = 'Y'";


            ViewData["link"] = con.Query<string>(query, new { id = id }).SingleOrDefault();
            ViewData["sr_number"] = id;
            return View();
        }

        [HttpGet]
        public ActionResult StudentDetails(int id, string calling_view)
        {
            sr_registerMain stdMain = new sr_registerMain();

            mst_sessionMain sess = new mst_sessionMain();

            ViewData["calling_view"] = calling_view;

            return View(stdMain.FindStudent(id,sess.findFinal_Session()));
        }

        [HttpGet]
        public ActionResult EditDetails(int id, string calling_view)
        {
            sr_registerMain stdMain = new sr_registerMain();
            mst_sessionMain sess = new mst_sessionMain();
            var obj = stdMain.FindStudent(id,sess.findFinal_Session());

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

            ViewData["calling_view"] = calling_view;

            return View(obj);
        }

        [HttpPost]
        public ActionResult EditDetails(sr_register std,string calling_view)
        {
            sr_registerMain stdMain = new sr_registerMain();
            decimal dues = 0m;

            string query = @"SELECT 
                                class_id
                            FROM
                                mst_class
                            WHERE
                                class_name = @std_admission_class
                                    AND session = (SELECT
                                        session
                                    FROM
                                        mst_session
                                    WHERE
                                        session_finalize = 'Y'
                                            AND session_active = 'Y')";

            int id = con.ExecuteScalar<int>(query, new { std.std_admission_class });

            //if (std.class_id < id)
            //{
            //    ModelState.AddModelError(String.Empty, "Class cannot be lower than admission class");


            //    DDclass_name(std);

            //    DDtransport_id(std);

            //    DDSections(std);

            //    return View(std);
            //}

            query = @"SELECT 
                            class_id
                        FROM
                            sr_register a,
                            mst_std_class b
                        WHERE
                            a.sr_number = b.sr_num
                                AND b.session = (SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_active = 'Y'
                                        AND session_finalize = 'Y')
                                AND sr_number = @sr_num";

            int changedclassid = con.Query<int>(query, new { sr_num = std.sr_number }).SingleOrDefault();

             query = @"SELECT 
                            IFNULL(COUNT(CASE
                                        WHEN rmt_amount = 0.00 THEN NULL
                                        ELSE rmt_amount
                                    END),
                                    0)
                        FROM
                            out_standing
                        WHERE
                            sr_number = @sr_num AND serial != 0
                                AND session = (SELECT 
                                    session
                                FROM
                                    mst_session
                                WHERE
                                    session_active = 'Y')
                                AND acc_id NOT IN (1 , 2, 6)";

            int error = con.Query<int>(query, new { sr_num = std.sr_number }).SingleOrDefault();



            if (error != 0 && changedclassid != std.class_id && calling_view == "AllStudentList")
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

            if(calling_view == "AllStudentList")
            {
                return RedirectToAction(calling_view);
            }
            else
            {
                return RedirectToAction(calling_view,"GenerateTC");
            }

            
        }

        [HttpGet]
        public ActionResult DeleteStudent(int id)
        {
            sr_registerMain stdMain = new sr_registerMain();

            mst_sessionMain sess = new mst_sessionMain();

            return View(stdMain.FindStudent(id,sess.findFinal_Session()));
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