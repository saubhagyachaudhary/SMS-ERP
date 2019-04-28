using Dapper;
using SMS.job_scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class dashboard
    {


        public int school_strength { get; set; }

        public int male_std { get; set; }

        public int female_std { get; set; }

        public decimal fees_received { get; set; }

        public decimal cash_received { get; set; }

        public decimal bank_received { get; set; }

        public int transport_std { get; set; }

        public int transport_female_std { get; set; }

        public int transport_male_std { get; set; }

        public int newAdmission { get; set; }

        public int newAdmission_male { get; set; }

        public int newAdmission_female { get; set; }

        public string[] name { get; set; }

        public string[] name_attendance { get; set; }

        public string[] date_list { get; set; }

        public string[] session { get; set; }

        public decimal[] session_dues { get; set; }

        public decimal[] dues { get; set; }

        public decimal[] recovered { get; set; }

        public decimal total_recovered { get; set; }

        public decimal total_dues { get; set; }

        public decimal total_cash_received { get; set; }

        public decimal total_bank_received { get; set; }

        public decimal total_cash_bank_received { get; set; }
        
        public string sms_credit_left { get; set; }

        public string today_consumption { get; set; }

        public int daily_present { get; set; }

        public int daily_absent { get; set; }

        public int[] present { get; set; }

        public int[] absent { get; set; }

        public int[] thirty_day_present { get; set; }

        public int[] thirty_day_absent { get; set; }

        public IEnumerable<attendance_register> finalize_list { get; set; }

        public IEnumerable<attendance_register> list_att_left_class { get; set; }

        public IEnumerable<dailyBirthdayWish> std_birthday_list { get; set; }

        public IEnumerable<dailyBirthdayWish> staff_birthday_list { get; set; }

    }


}