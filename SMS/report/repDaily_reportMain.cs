using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.report
{
    public class repDaily_reportMain
    {
       

        public int receipt_no { get; set; }

       
        public DateTime receipt_date { get; set; }

        public int sr_number { get; set; }

        public string std_name { get; set; }

        public string fees_name { get; set; }

        public string class_name { get; set; }

        public decimal fees { get; set; }

        public decimal fine { get; set; }

        public decimal discount { get; set; }

        public decimal amount { get; set; }

        public string mode_flag { get; set; }

        public string chq_reject { get; set; }

        public string bnk_name { get; set; }

        public string chq_no { get; set; }

        public DateTime chq_date { get; set; }

        public int acc_id { get; set; }
    }
}