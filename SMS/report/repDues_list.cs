using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class repDues_list
    {
        public int sr_number { get; set; }

        public string name { get; set; }

        public string std_father_name { get; set; }

        public string pickup_point { get; set; }

        public string class_name { get; set; }

        public string contact { get; set; }

        public decimal amount { get; set; }

        public bool check { get; set; }

        public bool flag_sms { get; set; }
        
        public string month_name { get; set; }

        public string address { get; set; }

        public DateTime payment_by { get; set; }

        public string message { get; set; }
    }
}