using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Hangfire.MySql;
using Hangfire.Dashboard;
using Microsoft.Owin;
using Owin;
using Hangfire;
using System.Web.Mvc;
using Hangfire.Annotations;

namespace SMS.Models
{
    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {

        public bool Authorize([NotNull] DashboardContext context)
        {


            return HttpContext.Current.User.IsInRole("superadmin");


        }
    }
}