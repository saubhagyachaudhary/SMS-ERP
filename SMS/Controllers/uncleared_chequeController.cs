using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class uncleared_chequeController : BaseController
    {
        // GET: uncleared_cheque
        [HttpGet]
        public ActionResult AllUnclearedChequeList()
        {
            uncleared_chequeMain uncleared_cheque = new uncleared_chequeMain();

            return View(uncleared_cheque.AllUnclearedChequeList());

            
        }

        [HttpPost]
        public async Task<ActionResult> AllUnclearedChequeList(List<uncleared_cheque> unclear)
        {
            uncleared_chequeMain uncleared_cheque = new uncleared_chequeMain();

            foreach (uncleared_cheque obj in unclear)
            {
                if (obj.check)
                {
                    if (!obj.clear_flag)
                    {
                        obj.chq_reject = "Bounce";
                        obj.chq_date = Convert.ToDateTime(obj.str_date);
                        await uncleared_cheque.Updatefees_Bounce(obj);
                    }
                    else
                    {
                        obj.chq_reject = "Cleared";
                        obj.chq_date = Convert.ToDateTime(obj.str_date);
                        uncleared_cheque.Updatefees_cleared(obj);
                    }
                    

                }
            }

          

            return View(uncleared_cheque.AllUnclearedChequeList());
            


        }
    }
}