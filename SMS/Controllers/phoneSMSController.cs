using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class phoneSMSController : BaseController
    {
        // GET: phoneSMS
        [HttpGet]
        public ActionResult smsSend()
        {
            SMSMessage sms = new SMSMessage();

            phoneSMS ph = new phoneSMS();

            ph.class_l = sms.Class_Name();

            ph.class_p = sms.pickup_Name();
            
            return View(ph);
        }

        [HttpPost]
        public async Task<ActionResult> smsSend(phoneSMS ph)
        {
            try
            {
                SMSMessage sms = new SMSMessage();

                List<string> phone = new List<string>();

                foreach (int i in ph.selected_list)
                {
                    if (i == 998)
                    {
                        foreach (var j in sms.getNumberWholeClass())
                        {
                            phone.Add(j);
                        }
                    }
                    else if (i == 999)
                    {
                        foreach (var j in sms.getNumberWholeTransport())
                        {
                            phone.Add(j);
                        }
                    }
                    else if (i == 997)
                    {
                        foreach (var j in sms.getNumberWholeStaff())
                        {
                            phone.Add(j);
                        }
                    }
                    else
                    {
                        if (i < 1000)
                        {
                            foreach (var j in sms.getNumberByClass(i))
                            {
                                phone.Add(j);
                            }
                        }
                        else
                        {
                            foreach (var j in sms.getNumberByTransport(i))
                            {
                                phone.Add(j);
                            }

                        }
                    }
                }


                //ph.class_l = sms.Class_Name();

                //ph.class_p = sms.pickup_Name();

                List<string> tmp = new List<string>();

                int jk = 0;

                //sms.SendMultiSms(ph.toText, String.Join(",", phone), phone);

                for (int i=0; i< phone.Count();i++)
                {

                    tmp.Add(phone[i]);

                    if(jk == 50)
                    {
                        jk = 0;
                        await sms.SendMultiSms(ph.toText, String.Join(",", tmp), tmp);

                        tmp = new List<string>();
                    }
                    jk++;
                }

                if(tmp.Count() != 0)
                {
                         await sms.SendMultiSms(ph.toText, String.Join(",", tmp), tmp);
                }


                return View("success");
            }
            catch (Exception ex)
            {
                return View("danger");
            }
        }
    }
}