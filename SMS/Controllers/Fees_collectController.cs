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
            return View();
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

                    //out_standing std = new out_standing();

                    //out_standingMain stdMain = new out_standingMain();

                    registration = reg.FindRegistrationForFees(col.reg_num);

                   // registration = reg.FindRegistration(std.fin_id, std.reg_num, std.dt_date);

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
                            month_no = fee.month_no
                            
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
                    mode_flag = fee.mode_flag
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

            IEnumerable<out_standing> std = new List<out_standing>();

            std = outstd.AllOutStanding(sr_num);


            //if (std.Count() == 0)
            //{



            //    mst_feesMain fees_main = new mst_feesMain();

            //    IEnumerable<mst_fees> fees_details = new List<mst_fees>();

            //    fees_details = fees_main.FindFeesDetails(sr_num);

            //    int year = outstd.GetlatestFeesYear(sr_num);
            //    int mon = outstd.GetlatestFeesMonth(sr_num);
            //    int mon1;

            //    if (mon <= 3)
            //        mon1 = mon + 12;
            //    else
            //    {
            //        mon1 = mon + 1;
            //        mon = 0;
            //    }

            //    mst_transport transport = new mst_transport();

            //    mst_transportMain transportMain = new mst_transportMain();

            //    transport = transportMain.FindTransportBySr(sr_num);

            //    //  out_standing outstd = new out_standing();

            //    out_standingMain outstdMain = new out_standingMain();

            //    IEnumerable<out_standing> advoutstd = new List<out_standing>();

            //    IEnumerable<out_standing> advTransOutStd = new List<out_standing>();

            //    int flag = 0;

            //    int transFlag = 0;

            //    for (int i = mon1; i <= 12; i++)
            //    {
            //        foreach (mst_fees val in fees_details)
            //        {

            //            advoutstd = outstdMain.GetAdvanceMonth(sr_num, val.acc_id);
            //            advTransOutStd = outstdMain.GetAdvanceTransportMonth(sr_num, 3);

            //            if (val.bl_apr && i == 4)
            //            {
            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_apr && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_apr = false;
            //                }

            //            }




            //            if (val.bl_may && i == 5)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_may && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_may = false;
            //                }
            //            }


            //            if (val.bl_jun && i == 6)
            //            {

            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_jun && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_jun = false;
            //                }

            //            }


            //            if (val.bl_jul && i == 7)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_jul && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_jul = false;
            //                }
            //            }


            //            if (val.bl_aug && i == 8)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_aug && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_aug = false;
            //                }
            //            }


            //            if (val.bl_sep && i == 9)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_sep && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_sep = false;
            //                }
            //            }


            //            if (val.bl_oct && i == 10)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_oct && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_oct = false;
            //                }

            //            }



            //            if (val.bl_nov && i == 11)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    //flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_nov && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_nov = false;
            //                }

            //            }


            //            if (val.bl_dec && i == 12)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    // flag = 0;
            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_dec && transFlag == 0)
            //                {
            //                    payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });

            //                    transport.bl_dec = false;
            //                }

            //            }





            //        }
            //    }

            //    flag = 0;

            //    for (int i = mon + 1; i <= 3; i++)
            //    {
            //        foreach (mst_fees val in fees_details)
            //        {
            //            advoutstd = outstdMain.GetAdvanceMonth(sr_num, val.acc_id);
            //            advTransOutStd = outstdMain.GetAdvanceTransportMonth(sr_num, 3);

            //            if (val.bl_jan && i == 1)
            //            {

            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    if (System.DateTime.Now.Month == 1 || System.DateTime.Now.Month == 2 || System.DateTime.Now.Month == 3)
            //                        payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year).ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    else
            //                        payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString() });

            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_jan && transFlag == 0)
            //                {

            //                    if (System.DateTime.Now.Month == 1 || System.DateTime.Now.Month == 2 || System.DateTime.Now.Month == 3)
            //                        payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year).ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    else
            //                        payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString() });


            //                    transport.bl_jan = false;
            //                }



            //            }



            //            if (val.bl_feb && i == 2)
            //            {


            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    if (System.DateTime.Now.Month == 1 || System.DateTime.Now.Month == 2 || System.DateTime.Now.Month == 3)
            //                        payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year).ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    else
            //                        payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString() });

            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_feb && transFlag == 0)
            //                {

            //                    if (System.DateTime.Now.Month == 1 || System.DateTime.Now.Month == 2 || System.DateTime.Now.Month == 3)
            //                        payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year).ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    else
            //                        payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString() });


            //                    transport.bl_feb = false;
            //                }
            //            }



            //            if (val.bl_mar && i == 3)
            //            {
            //                foreach (out_standing o in advoutstd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        flag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        flag = 0;
            //                    }

            //                }

            //                if (flag == 0)
            //                {
            //                    if (System.DateTime.Now.Month == 1 || System.DateTime.Now.Month == 2 || System.DateTime.Now.Month == 3)
            //                        payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year).ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    else
            //                        payment.Add(new fees_payment { acc_id = val.acc_id, sr_num = sr_num, Fees_type = val.acc_name + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString(), amount_to_be_paid = val.fees_amount, due_amount = val.fees_amount, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString() });

            //                }

            //                foreach (out_standing o in advTransOutStd)
            //                {
            //                    if (o.month_no == i)
            //                    {
            //                        transFlag = 1;
            //                        break;
            //                    }
            //                    else
            //                    {
            //                        transFlag = 0;
            //                    }

            //                }

            //                if (transport.bl_mar && transFlag == 0)
            //                {

            //                    if (System.DateTime.Now.Month == 1 || System.DateTime.Now.Month == 2 || System.DateTime.Now.Month == 3)
            //                        payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year).ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + year.ToString() });
            //                    else
            //                        payment.Add(new fees_payment { acc_id = 3, sr_num = sr_num, Fees_type = "Transport Fees" + ' ' + CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString(), amount_to_be_paid = transport.transport_fees, due_amount = transport.transport_fees, month_no = i, month_name = CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(i) + ' ' + (year + 1).ToString() });


            //                    transport.bl_mar = false;
            //                }

            //            }


            //        }
            //    }
            //}
            //else
            //{
            //    foreach (out_standing val in std)
            //    {
            //        payment.Add(new fees_payment { acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, due_amount = val.outstd_amount, serial = val.serial,sr_num = val.sr_number });
            //    }
            //}

                foreach (out_standing val in std)
                {
                    payment.Add(new fees_payment {acc_id = val.acc_id, Fees_type = val.acc_name, amount_to_be_paid = val.outstd_amount, due_amount = val.outstd_amount, serial = val.serial,sr_num = val.sr_number });
                }

            return payment;

        }

        

        private IEnumerable<fees_payment> GetFeesPaymentByReg(int reg_num)
        {
            List<fees_payment> payment = new List<fees_payment>();

            /* payment.Add(new fees_payment { Fees_type = "Tution Fees", amount_to_be_paid = 4000, due_amount = 4000 });
             payment.Add(new fees_payment { Fees_type = "Annual", amount_to_be_paid = 3000, due_amount = 3000 });
             payment.Add(new fees_payment { Fees_type = "IT", amount_to_be_paid = 2000, due_amount = 2000 });
             payment.Add(new fees_payment { Fees_type = "Development", amount_to_be_paid = 1000, due_amount = 1000 });

              */

            //fees_paymentMain feesMain = new fees_paymentMain();

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

        public PartialViewResult RenderPaid(int sr_num)
        {
          
                return PartialView(GetFeesPaid(sr_num));
          
        }



    }
}