using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SMS.Models;
using SMS.Controllers;

namespace SMS.Hubs
{
    public class DashboardHub : Hub 
    {
        public void DailyFeesUpdate()
        {
            
            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            db.bank_received = dmain.bank_received();

            db.cash_received = dmain.cash_received();

            db.fees_received = db.bank_received + db.cash_received;

            db.total_bank_received = dmain.total_bank_received();

            db.total_cash_received = dmain.total_cash_received();

            db.total_cash_bank_received = db.total_bank_received + db.total_cash_received;

            decimal[] fees = { db.fees_received, db.cash_received, db.bank_received, db.total_cash_received, db.total_cash_received, db.total_cash_bank_received };

            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            context.Clients.All.DashBoadDailyFeesUpdate(fees);
        }

    }
}