using Microsoft.Reporting.WebForms;
using SMS.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace SMS.Controllers
{
    public class Fees_collectController : Controller
    {
        [HttpGet]
        public ActionResult fees_collect()
        {
            fees_collect fee = new fees_collect();

            sr_registerMain stdMain = new sr_registerMain();

            fee.list = stdMain.AllStudentList();

            return View(fee);
        }

        [HttpGet]
        public ActionResult FeesStatement(String sr_num, String rcpt_no, String rcpt_dt)
        {
            ReportViewer reportViewer = new ReportViewer();

            reportViewer.ProcessingMode = ProcessingMode.Remote;
            reportViewer.Width = Unit.Pixel(1200);
            reportViewer.Height = Unit.Pixel(600);

            string[] str = rcpt_no.Split(new[] { ',', ' ' },StringSplitOptions.RemoveEmptyEntries); 

            // reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"Reports\ddd.rdl";

            reportViewer.ServerReport.ReportPath = "/feesReceipt";
            reportViewer.ServerReport.ReportServerUrl = new Uri("http://saubhagya-pc/ReportServer_MSSQL");

            List<ReportParameter> paramList = new List<ReportParameter>();
            ReportParameter param = new ReportParameter("rcpt_no");

            paramList.Add(new ReportParameter("sr_num", sr_num, false));
            param.Values.AddRange(str);
            paramList.Add(param);

            reportViewer.ServerReport.SetParameters(paramList);
            //reportViewer.LocalReport.DataSources.Clear();
            reportViewer.ShowPrintButton = true;
            reportViewer.ShowParameterPrompts = false;
            // reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSource1", getData()));

            ViewBag.ReportViewer = reportViewer;

            return PartialView("FeesStatement");
        }

        [HttpPost]
        public ActionResult fees_collect(fees_collect col)
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


                    return View("submit_fees", col);
                }
                else
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
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Student record not found");
                fees_collect fee = new fees_collect();

                sr_registerMain stdMain = new sr_registerMain();

                fee.list = stdMain.AllStudentList();

                return View(fee);
            }  
          
        }
        [HttpPost]
        public ActionResult submit_fees(List<fees_payment> fees)
        {
            List<fees_receipt> rec = new List<fees_receipt>();
            fees_receiptMain main = new fees_receiptMain();
            foreach (fees_payment fee in fees)
            {


                if (fee.check)
                {
                    if(fee.serial == 0)
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
                            user_id = Int32.Parse(Session["loginUserId"].ToString())

                        });
                    }
                    else
                    { 

                    rec.Add(new fees_receipt {acc_id = fee.acc_id,
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
                        user_id = Int32.Parse(Session["loginUserId"].ToString())
                    } );

                    }

                }
                
            }

            
            
            main.AddReceipt(rec);

            return RedirectToAction("fees_collect");
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
                    discount = discountMain.FindDiscount(sr_num,val.acc_id);

                if (discount != null)
                {
                        if (discount.acc_id == val.acc_id && discount.bl_apr && val.month_no == 4)
                        {
                            if(discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_may && val.month_no == 5)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_jun && val.month_no == 6)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_jul && val.month_no == 7)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_aug && val.month_no == 8)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_sep && val.month_no == 9)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_oct && val.month_no == 10)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_nov && val.month_no == 11)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_dec && val.month_no == 12)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_jan && val.month_no == 1)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_feb && val.month_no == 2)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        if (discount.acc_id == val.acc_id && discount.bl_mar && val.month_no == 3)
                        {
                            if (discount.percent != 100)
                                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, discount = val.outstd_amount * (discount.percent) / 100, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                        else
                        {
                            payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                        }
                    }
                
                else
                {
                    payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, due_amount = val.outstd_amount, serial = val.serial, sr_num = val.sr_number });
                }

                
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
                payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, due_amount = val.outstd_amount, serial = val.serial, reg_num = val.reg_num, reg_date = val.dt_date });
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