using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class reportController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());


        [HttpGet]
        public ActionResult feesStatement()
        {
            repFeesStatement main = new repFeesStatement();

            main.fromDt = (System.DateTime.Now.AddMinutes(dateTimeOffSet));

            main.toDt = (System.DateTime.Now.AddMinutes(dateTimeOffSet));

            return View(main);
        }

        [HttpPost]
        public ActionResult feesStatement(DateTime fromDt, DateTime toDt,string mode,string detailed)
        {
            repDaily_report aa = new repDaily_report();

            if (detailed == "Detailed")
            {
                aa.pdfdetailed(fromDt, toDt, mode);
            }
            else
            {
                aa.pdfCansolidated(fromDt, toDt, mode);
            }

            return View();
        }

        [HttpGet]
        public ActionResult duesList()
        {

            mst_sessionMain session = new mst_sessionMain();

            DDclass_name(session.findFinal_Session());

            return View();
        }

        [HttpPost] 
        public ActionResult duesList(int section_id,decimal amount,string operation)
        {
          
            repDues_listMain dues = new repDues_listMain();

            dues.pdfDues_list(section_id, amount,operation);

            mst_sessionMain session = new mst_sessionMain();

            DDclass_name(session.findFinal_Session());

            return View();
        }

        [HttpGet]
        public ActionResult duesListNotice()
        {

            mst_sessionMain session = new mst_sessionMain();

            DDclass_name(session.findFinal_Session());

            return View();
        }

        [HttpGet]
        public ActionResult duesListNotice_students(int section_id, decimal amount, string operation, string month_name)
        {

            repDues_listMain dues = new repDues_listMain();
            IEnumerable<repDues_list> result;
            result = dues.duesList_Notice(section_id, amount, operation,month_name);
            foreach(var i in result)
            {
                i.month_name = month_name;
            }
            return View(result);
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> duesListNotice_students(IEnumerable<repDues_list> list)
        {

            repDues_listMain dues = new repDues_listMain();
            List<repDues_list> result = new List<repDues_list>();

            bool flag = false;

            foreach (repDues_list li in list)
            {
                if (li.check)
                {
                    result.Add(new repDues_list { sr_number = li.sr_number, amount = li.amount, month_name = li.month_name, std_father_name = li.std_father_name, name = li.name });
                    if (li.flag_sms)
                        flag = true;
                    else
                        flag = false;
                }
            }

#if !DEBUG
            if (flag)
            {

                SMSMessage sms = new SMSMessage();
                foreach (var std in result)
                {
                    string contact = @"SELECT 
                                        COALESCE(std_contact, std_contact1, std_contact2)
                                    FROM
                                        sr_register
                                    WHERE
                                        sr_number = @sr_number
                                            AND std_active = 'Y'";
                    string number = con.Query<string>(contact, new { sr_number = std.sr_number }).SingleOrDefault();

                    foreach (var item in sms.smsbody("fees_notice"))
                    {
                        string body = item.Replace("#father_name#", std.std_father_name);

                        body = body.Replace("#amount#", std.amount.ToString());

                        body = body.Replace("#month_name#", std.month_name);

                        body = body.Replace("#std_name#", std.name);

                        await sms.SendSMS(body, number, true);
                    }
                }
                return View("success");
            }
#endif

            repDues_listMain ll = new repDues_listMain();

            ll.pdfDuesList_notice(result);


            return View(list);
        }

        [HttpGet]
        public ActionResult duesListTransportWise()
        {

            reptransportList_DuesList list = new reptransportList_DuesList();

            DDPickup_point_withoutWholeTransport(list);

            return View(list);
        }

        [HttpPost]
        public ActionResult duesListTransportWise(reptransportList_DuesList list)
        {

            repDues_listMain dues = new repDues_listMain();

            List<int> pick_id = new List<int>();

            foreach (var i in list.pickup)
            {
                pick_id.Add(i);
            }

            dues.pdfDues_listTransportWise(pick_id, list.amount, list.operation);
         
            DDPickup_point_withoutWholeTransport(list);
            return View(list);
        }

        [HttpGet]
        public ActionResult transportList()
        {
            reptransportList list = new reptransportList();

            DDPickup_point(list);

            return View(list);
        }

        [HttpPost]
        public ActionResult transportList(reptransportList pickup_id)
        {
            reptransportList list = new reptransportList();
            reptransportListMain dues = new reptransportListMain();

            List<int> pick_id = new List<int>();

            foreach(var i in pickup_id.pickup)
            {
                pick_id.Add(i);
            }

            dues.pdfTransport_list(pick_id);
            DDPickup_point(list);
            return View(list);
        }

        [HttpGet]
        public ActionResult birth_certificate()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult birth_certificate(int sr_num)
        {
            try
            {
                string query = @"select count(*) from sr_register where std_active = 'Y' and sr_number = @sr_num;";

                int count = con.Query<int>(query, new { sr_num = sr_num}).SingleOrDefault();

                if(count == 1)
                {
                    birth_certificateMain certificate = new birth_certificateMain();

                    certificate.pdfStudent_BirthCertificate(sr_num);

                    return View();
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Student not found.");
                    return View();
                }

                
            }
            catch
            {

                return View();
            }
        }


        [HttpGet]
        public ActionResult student_ledger()
        {

            DDsession_name();
            return View();
        }

        [HttpPost]
        public ActionResult student_ledger(int sr_num, string session)
        {
            try
            {
                string query = @"select count(*) from sr_register where std_active = 'Y' and sr_number = @sr_num;";

                int count = con.Query<int>(query, new { sr_num = sr_num }).SingleOrDefault();

                if (count == 1)
                {

                    student_ledgerMain ledger = new student_ledgerMain();

                    ledger.pdfStudent_ledger(sr_num, session);
                    DDsession_name();
                    return View();
                }
                else
                {
                    ModelState.AddModelError(String.Empty, "Student not found.");
                    DDsession_name();
                    return View();
                }
            }
            catch
            {
                DDsession_name();
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public HttpResponseMessage apiStudentLedger(int sr_number)
        {

            student_ledgerMain ledger = new student_ledgerMain();

            mst_sessionMain sess = new mst_sessionMain();

            ledger.StreampdfStudent_ledger(sr_number, sess.findFinal_Session());

            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.OK);

            return httpResponseMessage;
        }

        [HttpGet]
        public ActionResult report_card()
        {

            mst_classMain mstClass = new mst_classMain();

            bool flag;

            if (User.IsInRole("superadmin") || User.IsInRole("principal"))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            var class_list = mstClass.AllClassListByTeacher(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()),flag);


            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");

            ViewData["class_id"] = list;
            DDsession_name();
            return View();
        }

        [HttpPost]
        public ActionResult report_card(int section_id, int class_id, string session)
        {

            repReport_cardMain report_card = new repReport_cardMain();

            report_card.pdfReportCard(class_id, section_id, session);

            mst_classMain mstClass = new mst_classMain();

            bool flag;

            if (User.IsInRole("superadmin") || User.IsInRole("principal"))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            var class_list = mstClass.AllClassListByTeacher(Int32.Parse(Request.Cookies["loginUserId"].Value.ToString()),flag);


            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");

            ViewData["class_id"] = list;
            DDsession_name();
            return View();
        }

        public JsonResult GetClass(string session)
        {
            string query = @"SELECT 
                                class_id, class_name
                            FROM
                                mst_class
                            WHERE
                                session = @session";



            var class_list = con.Query<mst_section>(query, new { session = session });

            IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");

            return Json(list);

        }

        public JsonResult GetSections(int id,string session)
        {
            string query = @"SELECT 
                                section_id, section_name
                            FROM
                                mst_section
                            WHERE
                                class_id = @id AND session = @session";



            var section_list = con.Query<mst_section>(query, new { id = id,session=session });

            IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name");

            return Json(list);

        }

        [HttpGet]
        public ActionResult attendance_sheet()
        {

            DDsession_name();
            //DDclass_name();
            return View();
        }

        [HttpPost]
        public ActionResult attendance_sheet(int class_id, int section_id, int month_no, string session)
        {

            repAttendance_sheetMain attendance = new repAttendance_sheetMain();

            attendance.pdfAttendanceSheet(class_id, section_id, month_no, session);
            DDsession_name();
            //DDclass_name();
            return View();
        }

        [HttpGet]
        public ActionResult class_wise_std_list()
        {

            DDsession_name();
            //DDclass_name();
            return View();
        }

        [HttpPost]
        public ActionResult class_wise_std_list(int class_id,int section_id, string session)
        {

            //repAttendance_sheetMain attendance = new repAttendance_sheetMain();

            // attendance.pdfAttendanceSheet(class_id,section_id,month_no,session);

            repClass_Wise_Std_ListMain std_list = new repClass_Wise_Std_ListMain();

            std_list.pdfClass_Wise_Std_List(class_id, section_id, session);

            DDsession_name();
            //DDclass_name();
            return View();
        }

        //trach tranport
        [HttpGet]
        public ActionResult track_vehicle()
        {
            mst_transportMain stdMain = new mst_transportMain();

            return View(stdMain.AllTransportNumber());
            
        }

        [HttpGet]
        public ActionResult track_live_location(string transport_number)
        {

            ViewData["tranport_name"] = transport_number;
            ViewData["database"] = String.Format("{0}/{1}/{2}/{3}",transport_number,DateTime.Now.ToString("yyyy"),DateTime.Now.ToString("MM"), DateTime.Now.ToString("dd"));
            return View();

        }

        public JsonResult GetSection(int id)
        {
            bool flag;

            if (User.IsInRole("superadmin") || User.IsInRole("principal"))
            {
                flag = true;
            }
            else
            {
                flag = false;
            }

            if(flag)
            {
                string query = @"SELECT 
                                    a.section_id, b.section_name
                                FROM
                                    mst_attendance a,
                                    mst_section b
                                WHERE
                                    a.section_id = b.section_id
                                        AND a.class_id = @class_id
                                        AND b.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";


                var exam_list = con.Query<mst_section>(query, new { class_id = id});

                IEnumerable<SelectListItem> list = new SelectList(exam_list, "section_id", "section_name");

                return Json(list);
            }
            else
            {
                string query = @"SELECT 
                                    a.section_id, b.section_name
                                FROM
                                    mst_attendance a,
                                    mst_section b
                                WHERE
                                    a.section_id = b.section_id
                                        AND a.class_id = @class_id
                                        AND a.user_id = @user_id
                                        AND b.session = (SELECT 
                                            session
                                        FROM
                                            mst_session
                                        WHERE
                                            session_finalize = 'Y')";


                var exam_list = con.Query<mst_section>(query, new { class_id = id, user_id = int.Parse(Request.Cookies["loginUserId"].Value.ToString()) });

                IEnumerable<SelectListItem> list = new SelectList(exam_list, "section_id", "section_name");

                return Json(list);
            }
           

            

        }

        public void DDsession_name()
        {
            mst_sessionMain mstSession= new mst_sessionMain();

            IEnumerable<mst_session> session_list = mstSession.AllSesssionList();

            IEnumerable<SelectListItem> list1 = new SelectList(session_list, "session","session");

            ViewData["session"] = list1;
        }

        public void DDclass_name(string session)
        {
            mst_classMain mstClass = new mst_classMain();

            var class_list = mstClass.AllClassListWithSection(session);

            IEnumerable<SelectListItem> list1 = new SelectList(class_list, "class_id", "class_name");
           
            ViewData["class_id"] = list1;
        }

        public void DDPickup_point_withoutWholeTransport(reptransportList_DuesList list)
        {
            mst_transportMain mstTransport = new mst_transportMain();

            var transport_list = mstTransport.AllTransportList();

            List<pickup_list> mst = new List<pickup_list>();

            //mst.Add(new pickup_list { pickup_id = 999, pickup_point = "Whole Transport" });

            foreach (var lst in transport_list)
            {

                mst.Add(new pickup_list { pickup_id = lst.pickup_id, pickup_point = lst.pickup_point });


            }

            list.pickup_list = mst;
        }

        public void DDPickup_point(reptransportList list)
        {
            mst_transportMain mstTransport = new mst_transportMain();

            var transport_list = mstTransport.AllTransportList();

            List<pickup_list> mst = new List<pickup_list>();

            //mst.Add(new pickup_list { pickup_id = 999, pickup_point = "Whole Transport" });

            foreach (var lst in transport_list)
            {
                
                    mst.Add(new pickup_list { pickup_id = lst.pickup_id, pickup_point = lst.pickup_point });
                
                
            }

            list.pickup_list = mst;

            //IEnumerable<SelectListItem> list1 = new SelectList(mst, "pickup_id", "pickup_point");

            //ViewData["pickup_id"] = list1;
        }

        
    }
}