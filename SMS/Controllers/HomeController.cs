using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class HomeController : BaseController
    {
        // GET: Home
       //[Authorize(Roles = "superadmin,admin,principal,faculty")]
        public ActionResult Dashboard()
        {
            string wedget = Request.Cookies["wedget"].Value.ToString();
            var list = wedget.Split(',');

            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            //Task t1, t2 ,t3,t4,t5,t6,t7,t8,t9,t10,t11,t12,t13,t14,t15,t16,t17,t18,t19,t20,t21,t22,t23,t24;

            Task t1 = Task.FromResult<object>(null);
            Task t2 = Task.FromResult<object>(null);
            Task t3 = Task.FromResult<object>(null);
            Task t4 = Task.FromResult<object>(null);
            Task t5 = Task.FromResult<object>(null);
            Task t6 = Task.FromResult<object>(null);
            Task t7 = Task.FromResult<object>(null);
            Task t8 = Task.FromResult<object>(null);
            Task t9 = Task.FromResult<object>(null);
            Task t10 = Task.FromResult<object>(null);
            Task t11 = Task.FromResult<object>(null);
            Task t12 = Task.FromResult<object>(null);
            Task t13 = Task.FromResult<object>(null);
            Task t14 = Task.FromResult<object>(null);
            Task t15 = Task.FromResult<object>(null);
            Task t16 = Task.FromResult<object>(null);
            Task t17 = Task.FromResult<object>(null);
            Task t18 = Task.FromResult<object>(null);
            Task t19 = Task.FromResult<object>(null);
            Task t20 = Task.FromResult<object>(null);
            Task t21 = Task.FromResult<object>(null);
            Task t22 = Task.FromResult<object>(null);
            Task t23 = Task.FromResult<object>(null);
            Task t24 = Task.FromResult<object>(null);
            Task t25 = Task.FromResult<object>(null);
            Task t26 = Task.FromResult<object>(null);
            Task t27 = Task.FromResult<object>(null);
            Task t28 = Task.FromResult<object>(null);
            Task t29 = Task.FromResult<object>(null);

            if (list.Contains("fees_received"))
            {
                t1 = Task.Factory.StartNew(() => db.bank_received = dmain.bank_received());

                t2 = Task.Factory.StartNew(() => db.cash_received = dmain.cash_received());

                
            }
            if (list.Contains("school_strength"))
            {

                t3 = Task.Factory.StartNew(() => db.school_strength = dmain.school_strength());

                t4 = Task.Factory.StartNew(() => db.male_std = dmain.school_Male_std());

                t5 = Task.Factory.StartNew(() => db.female_std = dmain.school_Female_std());

                

            }
            if (list.Contains("transport_std"))
            {
                t5 = Task.Factory.StartNew(() => db.transport_male_std = dmain.transport_Male_std());

                t6 = Task.Factory.StartNew(() => db.transport_female_std = dmain.transport_Female_std());
               
            }
            
            if (list.Contains("newAdmission"))
            {
                t7 = Task.Factory.StartNew(() => db.newAdmission = dmain.new_admission());


                t8 = Task.Factory.StartNew(() => db.newAdmission_male = dmain.new_admission_male_std());


                t9 = Task.Factory.StartNew(() => db.newAdmission_female = dmain.new_admission_female_std());

                
            }
            if (list.Contains("class_wise_dues_chart"))
            {
                t10 = Task.Factory.StartNew(() => db.name = dmain.school_class());
                
            }
            else
            {
                db.name = new string[] { "" };
            }

            if (list.Contains("class_wise_dues_chart")|| list.Contains("total_recovery"))
            {

                t11 = Task.Factory.StartNew(() => db.dues = dmain.dues());

                t12 = Task.Factory.StartNew(() => db.recovered = dmain.recovered());

            }
            else
            {
                db.dues = new decimal[] { 0 };

                db.recovered = new decimal[] { 0 };

                db.total_dues = db.dues.Sum();

                db.total_recovered = db.recovered.Sum();
            }


            if (list.Contains("sms_credit_left"))
            {
                t13 = Task.Factory.StartNew(() => db.sms_credit_left = dmain.SMSCredit());

                t14 = Task.Factory.StartNew(() => db.today_consumption = dmain.today_consumption().ToString());
                
            }
          
            if (list.Contains("class_wise_attendance_chart")|| list.Contains("attendance_summary"))
            {
                if (User.IsInRole("superadmin") || User.IsInRole("principal"))
                {

                    t15 = Task.Factory.StartNew(() => db.name_attendance = dmain.school_class_for_attendance(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));

                    t16 = Task.Factory.StartNew(() => db.present = dmain.present(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));

                    t17 = Task.Factory.StartNew(() => db.absent = dmain.absent(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));
                   
                }
                else
                {
                    t15 = Task.Factory.StartNew(() => db.name_attendance = dmain.school_class_for_attendance(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));

                    t16 = Task.Factory.StartNew(() => db.present = dmain.present(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));

                    t17 = Task.Factory.StartNew(() => db.absent = dmain.absent(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));

                }
               
            }
            else
            {
                db.name_attendance = new string[] { "" };

                db.present = new int[] { 0 };

                db.absent = new int[] { 0 };
            }

            if (list.Contains("date_wise_attendance_chart"))
            {
                if (User.IsInRole("superadmin") || User.IsInRole("principal"))
                {
                    t18 = Task.Factory.StartNew(() => db.date_list = dmain.date_list_for_attendance(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));

                    t19 = Task.Factory.StartNew(() => db.thirty_day_present = dmain.thirty_day_present(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));

                    t20 = Task.Factory.StartNew(() => db.thirty_day_absent = dmain.thirty_day_absent(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));

                    
                }
                else
                {

                    t18 = Task.Factory.StartNew(() => db.date_list = dmain.date_list_for_attendance(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));

                    t19 = Task.Factory.StartNew(() => db.thirty_day_present = dmain.thirty_day_present(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));

                    t20 = Task.Factory.StartNew(() => db.thirty_day_absent = dmain.thirty_day_absent(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));
                }

            }
            else
            {
                db.date_list = new string[] { "" };

                db.thirty_day_present = new int[] { 0 };

                db.thirty_day_absent = new int[] { 0 };
            }



            if (list.Contains("finalize_list"))
            {
                if(User.IsInRole("superadmin")|| User.IsInRole("principal"))
                    t21 = Task.Factory.StartNew(() => db.finalize_list = dmain.finalizer_list(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));
                else
                    t21 = Task.Factory.StartNew(() => db.finalize_list = dmain.finalizer_list(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));
            }

            if (list.Contains("list_att_left_class"))
            {
                if (User.IsInRole("superadmin") || User.IsInRole("principal"))
                    t22 = Task.Factory.StartNew(() => db.list_att_left_class = dmain.att_left_classess(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));
                else
                    t22 = Task.Factory.StartNew(() => db.list_att_left_class = dmain.att_left_classess(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));
                

            }
            if (list.Contains("total_cash_bank_received"))
            {

                t23 = Task.Factory.StartNew(() => db.total_bank_received = dmain.total_bank_received());

                t24 = Task.Factory.StartNew(() => db.total_cash_received = dmain.total_cash_received());
                
                
            }


            if (list.Contains("std_birthday_list"))
            {
                if (User.IsInRole("superadmin") || User.IsInRole("principal"))
                    t25 = Task.Factory.StartNew(() => db.std_birthday_list = dmain.std_birthday_list(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), true));
                else
                    t26 = Task.Factory.StartNew(() => db.std_birthday_list = dmain.std_birthday_list(int.Parse(Request.Cookies["loginUserId"].Value.ToString()), false));
            }

            if (list.Contains("staff_birthday_list"))
            {
                
                    t27 = Task.Factory.StartNew(() => db.staff_birthday_list = dmain.staff_birthday());
            }

            if (list.Contains("session_wise_dues_chart"))
            {

                t28 = Task.Factory.StartNew(() => db.session = dmain.session());

                t29 = Task.Factory.StartNew(() => db.session_dues = dmain.session_dues());
            }
            else
            {
                db.session = new string[] { "" };

                db.session_dues = new decimal[] { 0 };
            }


            var tasklist = new List<Task> { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10, t11, t12, t13, t14, t15, t16, t17, t18, t19, t20, t21, t22, t23, t24, t25, t26, t27, t28, t29 };


            Task.WaitAll(tasklist.ToArray());



            if (list.Contains("fees_received"))
            {
                db.fees_received = db.bank_received + db.cash_received;
            }
            if (list.Contains("transport_std"))
            {

                db.transport_std = db.transport_male_std + db.transport_female_std;
            }

            if (list.Contains("class_wise_dues_chart") || list.Contains("total_recovery"))
            {

                db.total_dues = db.dues.Sum();

                db.total_recovered = db.recovered.Sum();
            }
            else
            {
                db.total_dues = db.dues.Sum();

                db.total_recovered = db.recovered.Sum();
            }

            if (list.Contains("class_wise_attendance_chart") || list.Contains("attendance_summary"))
            {
               
                db.daily_absent = db.absent.Sum();

                db.daily_present = db.present.Sum();
            }
            else
            {
                db.name_attendance = new string[] { "" };

                db.present = new int[] { 0 };

                db.absent = new int[] { 0 };
            }

            if (list.Contains("finalize_list"))
            {   
                foreach (var item in db.finalize_list)
                {
                    item.absent = dmain.absent_finalizer(item.session, item.att_date.ToString("yyyy-MM-dd"), item.section_id);
                    item.present = dmain.present_finalizer(item.session, item.att_date.ToString("yyyy-MM-dd"), item.section_id);
                }
            }

            if (list.Contains("list_att_left_class"))
            {
                
                foreach (var item in db.list_att_left_class)
                {
                    item.class_teacher = dmain.class_teacher(item.class_id, item.section_id);
                    item.finalizer = dmain.finalizer(item.class_id, item.section_id);
                }

            }

            if (list.Contains("total_cash_bank_received"))
            {
                
                db.total_cash_bank_received = db.total_bank_received + db.total_cash_received;
            }


            return View("Dashboard", db);
            
        
        }
    }
}