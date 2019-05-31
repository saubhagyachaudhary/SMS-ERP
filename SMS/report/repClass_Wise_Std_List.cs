using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class repClass_Wise_Std_List
    {
        public int sr_number { get; set; }

        public int roll_number { get; set; }

        public string std_name { get; set; }

        public string std_father_name { get; set; }

        public string std_mother_name { get; set; }

        public DateTime std_dob { get; set; }

        public string std_contact { get; set; }

        public string std_contact1 { get; set; }

        public string std_contact2 { get; set; }

        public string pickup_point { get; set; }

        public string address { get; set; }

        public string class_name { get; set; }

        public string section_name { get; set; }

        public DateTime std_admission_date { get; set; }
    }
}