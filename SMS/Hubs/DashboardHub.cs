using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using SMS.Models;
using SMS.Controllers;
using System.Threading.Tasks;

namespace SMS.Hubs
{
    public class DashboardHub : Hub
    {
        public void DailyFeesUpdate()
        {
            
            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            Task t1, t2, t3, t4;
            
            t1 = Task.Factory.StartNew(() => db.bank_received = dmain.bank_received());

            t2 = Task.Factory.StartNew(() => db.cash_received = dmain.cash_received());

            t3 = Task.Factory.StartNew(() => db.total_bank_received = dmain.total_bank_received());

            t4 = Task.Factory.StartNew(() => db.total_cash_received = dmain.total_cash_received());

            var tasklist = new List<Task> { t1, t2, t3, t4};

            Task.WaitAll(tasklist.ToArray());


            db.fees_received = db.bank_received + db.cash_received;

            db.total_cash_bank_received = db.total_bank_received + db.total_cash_received;

            decimal[] fees = { db.fees_received, db.cash_received, db.bank_received, db.total_cash_bank_received, db.total_cash_received, db.total_bank_received };

            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            Task.Factory.StartNew(() => context.Clients.All.DashBoadDailyFeesUpdate(fees));

            DashboardClassWiseDuesChart();
        }

        public void DashboardClassWiseDuesChart()
        {
            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            Task t1, t2, t3;

            // Dashboard dues chart live update
            t1 = Task.Factory.StartNew(() => db.name = dmain.school_class());

            t2 = Task.Factory.StartNew(() => db.dues = dmain.dues());

            t3 = Task.Factory.StartNew(() => db.recovered = dmain.recovered());

            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            Task.Factory.StartNew(() => context.Clients.All.DashboardClassWiseDuesChart( db.name, db.dues, db.recovered));
        }


        public void SMSCreditLeft()
        {

            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            Task t1, t2;

            t1 = Task.Factory.StartNew(() => db.sms_credit_left = dmain.SMSCredit());

            t2 = Task.Factory.StartNew(() => db.today_consumption = dmain.today_consumption().ToString());

            var tasklist = new List<Task> { t1, t2};

            Task.WaitAll(tasklist.ToArray());
            
            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            string[] sms = { db.sms_credit_left, db.today_consumption };

            Task.Factory.StartNew(() => context.Clients.All.DashBoadSMSCreditLeft(sms));
        }

        public void DashboardSchoolStrength()
        {

            dashboard db = new dashboard();
            dashboardMain dmain = new dashboardMain();

            Task t1, t2, t3, t4,t5,t6,t7,t8;

            t1 = Task.Factory.StartNew(() => db.school_strength = dmain.school_strength());

            t2 = Task.Factory.StartNew(() => db.male_std = dmain.school_Male_std());

            t3 = Task.Factory.StartNew(() => db.female_std = dmain.school_Female_std());

            t4 = Task.Factory.StartNew(() => db.newAdmission = dmain.new_admission());

            t5 = Task.Factory.StartNew(() => db.newAdmission_male = dmain.new_admission_male_std());

            t6 = Task.Factory.StartNew(() => db.newAdmission_female = dmain.new_admission_female_std());

            t7 = Task.Factory.StartNew(() => db.transport_male_std = dmain.transport_Male_std());

            t8 = Task.Factory.StartNew(() => db.transport_female_std = dmain.transport_Female_std());

            var tasklist = new List<Task> { t1, t2, t3, t4,t5,t6,t7,t8 };

            Task.WaitAll(tasklist.ToArray());


            db.transport_std = db.transport_male_std + db.transport_female_std;


            var context = GlobalHost.ConnectionManager.GetHubContext<DashboardHub>();

            int[] strength = { db.school_strength, db.male_std, db.female_std, db.newAdmission, db.newAdmission_male, db.newAdmission_female, db.transport_std, db.transport_male_std, db.transport_female_std };

            Task.Factory.StartNew(() => context.Clients.All.DashboardSchoolStrength(strength));
           
        }

    }
}