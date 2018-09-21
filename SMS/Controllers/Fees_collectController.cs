using Dapper;
using MySql.Data.MySqlClient;
using SMS.Models;
using SMS.report;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SMS.Controllers
{
    public class Fees_collectController : BaseController
    {
        MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

        [HttpGet]
        public ActionResult fees_collect(int? rcpt_no)
        {
            mst_finMain fin = new mst_finMain();
            fees_collect fee = new fees_collect();

            if (fin.checkFYnotExpired())
            {
                
                sr_registerMain stdMain = new sr_registerMain();


                fee.list = stdMain.AllStudentList(GetDefaultSection());



                DDClassWiseSection();

                //fee.list = stdMain.AllStudentList(0);

                ViewBag.recpt_no = rcpt_no;
                return View(fee);

            }
            else
            {
                ModelState.AddModelError(String.Empty, "Financial Year Expired cannot submit fees create new Financial  year.");
                sr_registerMain stdMain = new sr_registerMain();

                fee.list = stdMain.AllStudentList(GetDefaultSection());
                DDClassWiseSection();

                ViewBag.recpt_no = rcpt_no;
                return View(fee);
            }
        }

        [HttpGet]
        public void FeesStatement(int rcpt_no)
        {

             repFees_receipt rept = new repFees_receipt();

             rept.pdf(rcpt_no);

            

        }

        [HttpPost]
        public ActionResult fees_collect(fees_collect col)
        {
            mst_finMain fin = new mst_finMain();
            if (fin.checkFYnotExpired())
            {
                try
                {
                    if (col.sr_num > 0)
                    {
                        sr_registerMain reg = new sr_registerMain();
                        sr_register register = new sr_register();

                        register = reg.FindStudent(col.sr_num);

                        col.std_Name = register.std_first_name + " " + register.std_last_name;

                        col.std_father_name = register.std_father_name;

                        col.std_mother_name = register.std_mother_name;

                        col.std_contact = register.std_contact;

                        col.std_Email = register.std_email;

                        col.std_Class = register.class_name;

                        col.std_Section = register.section_name;

                        col.std_Pickup_point = register.pickup_point;

                        col.std_aadhar = register.std_aadhar;

                        return View("submit_fees", col);
                    }
                    else if(col.reg_num>0)
                    {
                        std_registrationMain reg = new std_registrationMain();
                        std_registration registration = new std_registration();


                        registration = reg.FindRegistrationForFees(col.reg_num);


                        col.std_Name = registration.std_first_name + " " + registration.std_last_name;

                        col.std_father_name = registration.std_father_name;

                        col.std_mother_name = registration.std_mother_name;

                        col.std_contact = registration.std_contact;

                        col.std_Email = registration.std_email;

                        col.std_Class = registration.class_name;

                        col.std_Section = "N/A";

                        col.std_Pickup_point = "N/A";

                        return View("submit_fees", col);
                    }else
                    {
                        fees_collect fee = new fees_collect();

                        sr_registerMain stdMain = new sr_registerMain();

                        fee.list = stdMain.AllStudentList(col.section_id);



                        DDClassWiseSection();



                        return View(fee);
                    }
                }
                catch
                {
                   
                        ModelState.AddModelError(string.Empty, "Student record not found");
                        fees_collect fee = new fees_collect();

                        sr_registerMain stdMain = new sr_registerMain();

                        fee.list = stdMain.AllStudentList(GetDefaultSection());



                        DDClassWiseSection();



                        return View(fee);
                    
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Financial Year Expired cannot submit fees create new Financial  year.");
                fees_collect fee = new fees_collect();

                sr_registerMain stdMain = new sr_registerMain();

                fee.list = stdMain.AllStudentList(GetDefaultSection());



                DDClassWiseSection();



                return View(fee);
            }
          
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


            var section_list = con.Query<mst_section>(query, new { session = sess.findActive_finalSession() });

            IEnumerable<SelectListItem> list = new SelectList(section_list, "section_id", "section_name");

            ViewData["section_id"] = list;

        }

        public int GetDefaultSection()
        {

            mst_sessionMain sess = new mst_sessionMain();

            string query = @"select a.section_id from mst_section a
                            where
                            session = @session
                            and
                            a.class_id = 4";


            var dafault = con.Query<int>(query, new { session = sess.findActive_finalSession() }).SingleOrDefault();

            return dafault;

        }

        [HttpPost]
        public async Task<ActionResult> submit_fees(List<fees_payment> fees)
        {
           

                List<fees_receipt> rec = new List<fees_receipt>();
                fees_receiptMain main = new fees_receiptMain();
                foreach (fees_payment fee in fees)
                {


                    if (fee.check)
                    {
                        if (fee.serial == 0)
                        {

                            out_standing out_std = new out_standing();
                            out_standingMain out_stdMain = new out_standingMain();

                            out_std.outstd_amount = fee.due_amount;
                            out_std.rmt_amount = fee.due_amount;
                            out_std.sr_number = fee.sr_num;
                            out_std.acc_id = fee.acc_id;
                            out_std.month_no = fee.month_no;
                            out_std.month_name = fee.month_name;
                            out_std.clear_flag = fee.clear_flag;
                            out_std.session = fee.session;
                            out_stdMain.AddOutStanding(out_std);


                            rec.Add(new fees_receipt
                            {
                                acc_id = fee.acc_id,
                                amount = fee.amount_to_be_paid,
                                fees_name = fee.Fees_type,
                                sr_number = fee.sr_num,
                                dc_fine = fee.fine,
                                dc_discount = fee.discount,
                                narration = fee.Narration,
                                serial = out_std.serial,
                                clear_flag = fee.clear_flag,
                                bnk_name = fee.Bank_name,
                                chq_no = fee.cheque_no,
                                chq_date = fee.cheque_date,
                                mode_flag = fee.mode_flag,
                                month_no = fee.month_no,
                                session = fee.session,
                                user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString())

                            });
                        }
                        else
                        {

                            rec.Add(new fees_receipt
                            {
                                acc_id = fee.acc_id,
                                amount = fee.amount_to_be_paid,
                                fees_name = fee.Fees_type,
                                sr_number = fee.sr_num,
                                dc_fine = fee.fine,
                                dc_discount = fee.discount,
                                narration = fee.Narration,
                                serial = fee.serial,
                                dt_date = fee.dt_date,
                                reg_no = fee.reg_num,
                                reg_date = fee.reg_date,
                                clear_flag = fee.clear_flag,
                                bnk_name = fee.Bank_name,
                                chq_no = fee.cheque_no,
                                chq_date = fee.cheque_date,
                                mode_flag = fee.mode_flag,
                                session = fee.session,
                                user_id = Int32.Parse(Request.Cookies["loginUserId"].Value.ToString())
                            });

                        }

                    }

                }



                int rcpt_no = await main.AddReceipt(rec);
                return RedirectToAction("fees_collect", new { rcpt_no = rcpt_no });
          
            
        }

        private IEnumerable<fees_payment> GetFeesPayment(int sr_num)
        {
            List<fees_payment> payment = new List<fees_payment>();
            
          

            out_standingMain outstd = new out_standingMain();
            std_discount discount = new std_discount();
            std_discountMain discountMain = new std_discountMain();

            IEnumerable<out_standing> std = new List<out_standing>();

            std = outstd.AllOutStanding(sr_num);

            
            
                foreach (out_standing val in std)
                {
                    //discount = discountMain.FindDiscount(sr_num,val.acc_id);

                            payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number, session = val.session });
                        
                 }
                
               

                
                

            return payment;

        }

        

        private IEnumerable<fees_payment> GetFeesPaymentByReg(int reg_num)
        {
            List<fees_payment> payment = new List<fees_payment>();

            out_standingMain outstd = new out_standingMain();

            IEnumerable<out_standing> std = new List<out_standing>();

            std = outstd.AllOutStandingByReg(reg_num);

            foreach (out_standing val in std)
            {
                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, due_amount = val.outstd_amount, serial = val.serial, reg_num = val.reg_num, reg_date = val.dt_date, session = val.session });
            }


            return payment;

        }

        private IEnumerable<fees_payment> GetFeesPaid(int sr_num)
        {
            List<fees_payment> payment = new List<fees_payment>();

            

            fees_receiptMain outrect = new fees_receiptMain();

            IEnumerable<fees_receipt> rect = new List<fees_receipt>();

            rect = outrect.AllPaidFees(sr_num);

            foreach (fees_receipt val in rect)
            {

                payment.Add(new fees_payment { sr_num=sr_num,mode_flag = val.mode_flag, fin_id = val.fin_id, receipt_no = val.receipt_no, receipt_date = val.receipt_date ,Fees_type = val.fees_name, amount_to_be_paid = val.amount, fine = val.dc_fine,discount = val.dc_discount,Narration = val.narration,acc_id = val.acc_id , chq_reject = val.chq_reject });
            }

           

            return payment;

        }

        private IEnumerable<fees_payment> GetFeesPaidByReg(int reg)
        {
            List<fees_payment> payment = new List<fees_payment>();



            fees_receiptMain outrect = new fees_receiptMain();

            IEnumerable<fees_receipt> rect = new List<fees_receipt>();

            rect = outrect.AllPaidFeesReg(reg);

            foreach (fees_receipt val in rect)
            {

                payment.Add(new fees_payment { sr_num = reg, mode_flag = val.mode_flag, fin_id = val.fin_id, receipt_no = val.receipt_no, receipt_date = val.receipt_date, Fees_type = val.fees_name, amount_to_be_paid = val.amount, fine = val.dc_fine, discount = val.dc_discount, Narration = val.narration, acc_id = val.acc_id, chq_reject = val.chq_reject });
            }



            return payment;

        }



        public PartialViewResult RenderPayment(int sr_num, int reg_num)
        {
            if (sr_num > 0)
            {
                return PartialView(GetFeesPayment(sr_num));
            }
            else
            {
                return PartialView(GetFeesPaymentByReg(reg_num));
            }
        }

        public PartialViewResult RenderPaid(int sr_num, int reg_num)
        {
            if (sr_num > 0)
            {
                return PartialView(GetFeesPaid(sr_num));
            }
            else
            {
                return PartialView(GetFeesPaidByReg(reg_num));
            }


        }



    }
}