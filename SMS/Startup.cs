﻿using System;
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

            dailyBirthdayWishMain birthday = new dailyBirthdayWishMain();

            duesReminderMain dues = new duesReminderMain();

            RecurringJob.AddOrUpdate("Birth Day Wish", () => birthday.SendBirthdayWish(), "0 6 * * *",TimeZoneInfo.Local);

            RecurringJob.AddOrUpdate("Dues Reminder date 5", () => dues.SendDuesReminder(), "0 9 5 * *",TimeZoneInfo.Local);

            app.MapSignalR();

            app.UseHangfireServer();
        }
    }
}
