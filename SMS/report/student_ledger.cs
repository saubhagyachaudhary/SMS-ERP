using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class student_ledger
    {
        public string fees_name { get; set; }

        public DateTime receipt_date { get; set; }

        public int receipt_no { get; set; }

        public decimal out_standing { get; set; }

        public decimal paid { get; set; }

        public decimal dc_fine { get; set; }

        public decimal dc_discount { get; set; }

        [Display(Name = "Admission Number")]
        public int sr_num { get; set; }

        public string std_name { get; set; }

        public string std_father_name { get; set; }

        public string contact { get; set; }

        public string address { get; set; }

        public string class_name { get; set; }

        public string std_email { get; set; }

        public DateTime std_dob { get; set; }

        public string pickup_point { get; set; }

        [Display(Name = "Session")]
        public string session { get; set; }

    }
}