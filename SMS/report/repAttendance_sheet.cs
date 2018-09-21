using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class repAttendance_sheet
    {
        public DateTime att_date { get; set; }

        public int sr_num { get; set; }

        public string std_name { get; set; }

        public int roll_no { get; set; }

        public string attendance { get; set; }

        public int day { get; set; }

        public int P_count { get; set; }

        public int A_count { get; set; }

        public string contact { get; set; }
    }
}