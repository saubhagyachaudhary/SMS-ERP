using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Hangfire;
using SMS.report;
using SMS.job_scheduler;
using System.Configuration;
using Hangfire.MySql;
using SMS.Models;
using System.Web;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using Dapper;

[assembly: OwinStartup(typeof(SMS.Startup))]

namespace SMS
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseStorage(new MySqlStorage(ConfigurationManager.ConnectionStrings["HangeFireConnection"].ToString()));

            app.UseHangfireDashboard("/job_scheduled", new DashboardOptions()
            {

                Authorization = new[] { new HangFireAuthorizationFilter() },

                AppPath = VirtualPathUtility.ToAbsolute("~/Home/Dashboard")

            });

            MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString());

            string query = @"SELECT 
                                    job_name, job_datetime
                                FROM
                                    recurring_jobs";

            IEnumerable<recurring_jobs> result = con.Query<recurring_jobs>(query);

            dailyBirthdayWishMain birthday = new dailyBirthdayWishMain();

            duesReminderMain dues = new duesReminderMain();
#if !DEBUG
            foreach(var job in result)
            {
               
               RecurringJob.AddOrUpdate(job.job_name, () => birthday.SendBirthdayWish(), job.job_datetime, TimeZoneInfo.Local);
               
            }
#endif
            //RecurringJob.AddOrUpdate("Birth Day Wish", () => birthday.SendBirthdayWish(), "0 6 * * *",TimeZoneInfo.Local);

            // RecurringJob.AddOrUpdate("Dues Reminder date 5", () => dues.SendDuesReminder(), "0 9 5 * *",TimeZoneInfo.Local);

            app.MapSignalR();

            app.UseHangfireServer();
        }
    }

    public class recurring_jobs
    {
       
        public string job_name { get; set; }

        public string job_datetime { get; set; }
    }
}
