﻿using Dapper;
using MySql.Data.MySqlClient;
using SMS.AcademicReport;
using SMS.ExcelReport;
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
        


        [HttpGet]
        public ActionResult feesStatement()
        {
            repFeesStatement main = new repFeesStatement();

            main.fromDt = (System.DateTime.Now.AddMinutes(dateTimeOffSet));

            main.toDt = (System.DateTime.Now.AddMinutes(dateTimeOffSet));

            return View(main);
        }

        [HttpPost]
        public ActionResult feesStatement(DateTime fromDt, DateTime toDt,string mode,string detailed, string format)
        {

            if (format == "PDF")
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

            }
            else
            {
                ExcelDaily_reportMain excel = new ExcelDaily_reportMain();

                if(detailed == "Detailed")
                {
                    excel.ExcelDetailed(fromDt, toDt, mode);
                }
                else
                {
                    excel.ExcelCansolidated(fromDt, toDt, mode);
                }

                
            }

           

            repFeesStatement main = new repFeesStatement();
            main.fromDt = fromDt;
            main.toDt = toDt;

            return View(main);
        }

        [HttpGet]
        public ActionResult head_wise_fees_statement()
        {
            repFeesStatement main = new repFeesStatement();

            main.fromDt = (System.DateTime.Now.AddMinutes(dateTimeOffSet));

            main.toDt = (System.DateTime.Now.AddMinutes(dateTimeOffSet));

            DDsession_name();

            return View(main);
        }

       

        [HttpPost]
        public ActionResult head_wise_fees_statement(DateTime fromDt, DateTime toDt, string mode, string session,int acc_id, string format)
        {
            

            if (format == "PDF")
            {
                repDaily_report aa = new repDaily_report();
                aa.pdfHeadWiseStatement(fromDt, toDt, mode, session, acc_id);
            }
            else
            {
                ExcelDaily_reportMain excel = new ExcelDaily_reportMain();
                excel.ExcelHeadWiseStatement(fromDt, toDt, mode, session, acc_id);
            }


            repFeesStatement main = new repFeesStatement();
            
            main.fromDt = fromDt;

            main.toDt = toDt;
            
            DDsession_name();

            return View(main);
        }

        public JsonResult GetAccountHead(string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                    acc_id, acc_name
                                FROM
                                    mst_acc_head
                                WHERE
                                    session = @session";



                var acc_list = con.Query<mst_acc_head>(query, new { session = session });

                IEnumerable<SelectListItem> list = new SelectList(acc_list, "acc_id", "acc_name");

                return Json(list);
            }
        }

        [HttpGet]
        public ActionResult duesList()
        {

           
            DDsession_name();

            return View();
        }

        [HttpPost] 
        public ActionResult duesList(repDues_Statement rep)
        {
          
            repDues_listMain dues = new repDues_listMain();

            dues.pdfDues_list(rep.section_id, rep.amount, rep.operation,rep.class_id,rep.session,rep.month_name);

           
            DDsession_name();

            return View();
        }

        [HttpGet]
        public ActionResult duesListNotice()
        {

            //mst_sessionMain session = new mst_sessionMain();

            //DDclass_name(session.findFinal_Session());

            DDsession_name();

            repDues_Statement model = new repDues_Statement();

            SMSMessage sms = new SMSMessage();

            model.message = sms.getRecentSMS("fees_notice", 15);

            model.payment_by = DateTime.Now;

            model.font_size = 8;

            return View(model);
        }

        [HttpGet]
        public ActionResult duesListNotice_students(int class_id,int section_id, decimal amount, string operation, string month_name, string  payment_by, string message,string session, int font_size)
        {
           
                if (payment_by == null)
                {
                    ModelState.AddModelError(String.Empty, "Due Date is mandatory.");
                }

                repDues_listMain dues = new repDues_listMain();
                IEnumerable<repDues_list> result;
                result = dues.duesList_Notice(class_id, section_id, amount, operation, month_name, session);
                foreach (var i in result)
                {
                    i.month_name = month_name;
                    i.payment_by = DateTime.Parse(payment_by);
                    i.message = message;
                    i.font_size = font_size;
                }
                return View(result);
           
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> duesListNotice_students(IEnumerable<repDues_list> list)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {

                repDues_listMain dues = new repDues_listMain();
                List<repDues_list> result = new List<repDues_list>();

                bool flag = false;

                int font_size = 8;

                foreach (repDues_list li in list)
                {
                    if (li.check)
                    {
                        result.Add(new repDues_list { sr_number = li.sr_number, amount = li.amount, month_name = li.month_name, std_father_name = li.std_father_name, name = li.name, payment_by = li.payment_by, message = li.message });
                        if (li.flag_sms)
                            flag = true;
                        else
                            flag = false;
                    }

                    if (li.font_size != 0)
                    {
                        font_size = li.font_size;
                    }
                }
                SMSMessage sms = new SMSMessage();
#if !DEBUG
            
            sms.setRecentSMS(result.FirstOrDefault().message, 15, "fees_notice");
            if (flag)
            {

                
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

                   
                        string body = std.message.Replace("#father_name#", std.std_father_name);

                        body = body.Replace("#amount#", std.amount.ToString());

                        body = body.Replace("#month_name#", std.month_name);

                        body = body.Replace("#std_name#", std.name);

                        body = body.Replace("#date#", std.payment_by.ToString("dd/MM/yyyy"));

                        await sms.SendSMS(body, number, true);
                    
                }
                return View("success");
            }
#endif

                repDues_listMain ll = new repDues_listMain();

                ll.pdfDuesList_notice(result, font_size);


                string msg = "";

                foreach (var i in result)
                {
                    msg = i.message;

                    break;
                }

                sms.setRecentSMS(msg, 15, "fees_notice");

                return View(list);
            }
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
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"select count(*) from sr_register where std_active = 'Y' and sr_number = @sr_num;";

                    int count = con.Query<int>(query, new { sr_num = sr_num }).SingleOrDefault();

                    if (count == 1)
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
            }
            catch
            {

                return View();
            }
        }

        [HttpGet]
        public ActionResult Reimbursement_certificate()
        {
            DDsession_name();
            return View();
        }

        [HttpPost]
        public ActionResult Reimbursement_certificate(int sr_num, string session)
        {
            try
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"select count(*) from sr_register where std_active = 'Y' and sr_number = @sr_num;";

                    int count = con.Query<int>(query, new { sr_num = sr_num }).SingleOrDefault();

                    if (count == 1)
                    {

                        birth_certificateMain certificate = new birth_certificateMain();

                        certificate.pdfReimbursementCertificate(sr_num, session);
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
            }
            catch
            {
                DDsession_name();
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
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
                {
                    string query = @"select count(*) from sr_register where sr_number = @sr_num;";

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
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                class_id, class_name
                            FROM
                                mst_class
                            WHERE
                                session = @session
                                order by order_by";



                var class_list = con.Query<mst_section>(query, new { session = session });

                IEnumerable<SelectListItem> list = new SelectList(class_list, "class_id", "class_name");

                return Json(list);
            }

        }

        public JsonResult GetSections(int id,string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                section_id, section_name
                            FROM
                                mst_section
                            WHERE
                                class_id = @id AND session = @session";



                var section_list = con.Query<mst_section>(query, new { id = id, session = session });

                IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name");

                return Json(list);
            }

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
           
            return View();
        }

        [HttpPost]
        public ActionResult class_wise_std_list(int class_id,int section_id, string session, string format)
        {

            if (format == "Excel")
            {
                ExcelClass_Wise_Std_ListMain std_list = new ExcelClass_Wise_Std_ListMain();

                std_list.ExcelClass_Wise_Std_List(class_id, section_id, session);
            }
            else
            {
                repClass_Wise_Std_ListMain std_list = new repClass_Wise_Std_ListMain();

                std_list.pdfClass_Wise_Std_List(class_id, section_id, session);
            }
            DDsession_name();
          
            return View();
        }

        [HttpGet]
        public ActionResult session_new_admission()
        {

            DDsession_name();

            return View();
        }

        [HttpPost]
        public ActionResult session_new_admission(string session,string format)
        {

            if (format == "Excel")
            {
                ExcelClass_Wise_Std_ListMain std_list = new ExcelClass_Wise_Std_ListMain();

                std_list.Excelsession_new_admission(session);
            }
            else
            {

                repClass_Wise_Std_ListMain std_list = new repClass_Wise_Std_ListMain();

                std_list.session_new_admission(session);
            }
            DDsession_name();

            return View();
        }

        [HttpGet]
        public ActionResult school_strength()
        {

            DDsession_name();

            return View();
        }

        [HttpPost]
        public ActionResult school_strength(string session, string format)
        {

           if(format == "Excel")
            {
                ExcelClass_Wise_Std_ListMain std_list = new ExcelClass_Wise_Std_ListMain();

                std_list.ExcelSchool_Strength(session);
            }
           else
            {
                repClass_Wise_Std_ListMain std_list = new repClass_Wise_Std_ListMain();

                std_list.school_strength(session);
            }
            DDsession_name();
            return View();
        }

        [HttpGet]
        public ActionResult StudentSummary()
        {

            DDsession_name();

            return View();
        }

        [HttpPost]
        public ActionResult StudentSummary(string session)
        { 

            ExcelClassSummaryReportMain.ExcelClassSummary(session);
            DDsession_name();
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

       

        public JsonResult GetSubject(int id,string session)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString()))
            {
                string query = @"SELECT 
                                    a.subject_id,a.subject_name
                                FROM
                                    mst_subject a,
                                    mst_class_subject b
                                WHERE
                                    a.session = b.session
                                        AND a.subject_id = b.subject_id
                                        AND a.session = @session
                                        AND b.class_id = @class_id";



                var subject_list = con.Query<mst_subject>(query, new { class_id = id, session = session });

                IEnumerable<SelectListItem> list = new SelectList(subject_list, "subject_id", "subject_name");

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

        #region Academic Report
        [HttpGet]
        public ActionResult subject_comparative_study()
        {
           
            DDsession_name();
            return View();

            
        }

        [HttpPost]
        public ActionResult subject_comparative_study(mst_exam_marks marks)
        {
            comparative_result_analysis study = new comparative_result_analysis();

            study.pdfComparative_result_analysis(marks.subject_id, marks.class_id, marks.section_id, marks.session);

            DDsession_name();
            return View();


        }

        [HttpGet]
        public ActionResult assignment_chart()
        {

            DDsession_name();
            //DDclass_name();
            return View();
        }

        [HttpPost]
        public ActionResult assignment_chart(int class_id, int section_id, int month_no, string session)
        {

            repClassAssignments chart = new repClassAssignments();

            chart.pdfClassAssignment(class_id, section_id, month_no, session);
            DDsession_name();
            //DDclass_name();
            return View();
        }

        #endregion

        #region visitors

        [HttpGet]
        public ActionResult pdfstd_half_day()
        {

            DDsession_name();
            return View();


        }

        [HttpPost]
        public ActionResult pdfstd_half_day(DateTime fromDate,DateTime toDate,string session)
        {
            repStd_half_day study = new repStd_half_day();

            study.pdfstd_half_day(fromDate,toDate,session);

            DDsession_name();
            return View();


        }


        #endregion
    }
}