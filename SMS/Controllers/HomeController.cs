using SMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMS.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
       [Authorize(Roles = "superadmin,admin")]
        public ActionResult Dashboard()
        {
            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            db.bank_received = dmain.bank_received();

            db.cash_received = dmain.cash_received();

            db.fees_received = db.bank_received + db.cash_received;

            db.school_strength = dmain.school_strength();

            db.transport_std = dmain.transport_std();

            db.newAdmission = dmain.new_admission();

            db.name = dmain.school_class();

            db.dues = dmain.dues();

            db.total_dues = db.dues.Sum();

            db.recovered = dmain.recovered();

            db.total_recovered = db.recovered.Sum();

            return View(db);
        }
    }
}