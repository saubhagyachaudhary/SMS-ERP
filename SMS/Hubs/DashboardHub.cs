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

            decimal[] fees = { db.fees_received, db.cash_received, db.bank_received, db.total_cash_bank_received, db.total_cash_received, db.total_bank_received };

            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            context.Clients.All.DashBoadDailyFeesUpdate(fees);
        }


        public void SMSCreditLeft()
        {

            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

             db.sms_credit_left = dmain.SMSCredit();

            db.today_consumption = dmain.today_consumption().ToString();

            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            string[] sms = { db.sms_credit_left, db.today_consumption };

            context.Clients.All.DashBoadSMSCreditLeft(sms);
        }

        public void DashboardSchoolStrength()
        {

            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            db.school_strength = dmain.school_strength();

            db.male_std = dmain.school_Male_std();

            db.female_std = dmain.school_Female_std();

            db.newAdmission = dmain.new_admission();


            db.newAdmission_male = dmain.new_admission_male_std();


            db.newAdmission_female = dmain.new_admission_female_std();


            db.transport_male_std = dmain.transport_Male_std();

            db.transport_female_std = dmain.transport_Female_std();

            db.transport_std = db.transport_male_std + db.transport_female_std;


            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            int[] strength = { db.school_strength, db.male_std, db.female_std, db.newAdmission, db.newAdmission_male, db.newAdmission_female, db.transport_std, db.transport_male_std, db.transport_female_std };

            context.Clients.All.DashboardSchoolStrength(strength);
        }

    }
}