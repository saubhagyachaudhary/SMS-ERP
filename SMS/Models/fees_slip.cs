using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class fees_slip
    {
        public string fin_id { get; set; }

        public string payment_mode { get; set; }

        public string Narration { get; set; }

        public string receipt_no { get; set; }

        public DateTime receipt_date { get; set; }

        public string cheque_no { get; set; }

        public DateTime cheque_date { get; set; }

        public string bank_name { get; set; }

        public string bank_branch { get; set; }
    }
}