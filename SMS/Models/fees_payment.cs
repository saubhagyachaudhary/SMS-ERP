using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class fees_payment
    {
       

        public int sr_num { get; set; }

        public string Fees_type { get; set; }

        public int serial { get; set; }

        public DateTime dt_date { get; set; }

        public decimal due_amount { get; set; }

        public decimal amount_to_be_paid { get; set; }

        public decimal fine { get; set; }

        public decimal discount { get; set; }

        public int receipt_no { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime receipt_date { get; set; }

        public string Narration { get; set; }

        public string cheque_no { get; set; }

        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime cheque_date { get; set; }

        public string Bank_name { get; set; }

        public int acc_id { get; set; }

        public bool check { get; set; }

        public int month_no { get; set; }

        public string month_name { get; set; }

        public int reg_num { get; set; }

        public DateTime reg_date { get; set; }

        public string fin_id { get; set; }

        public bool clear_flag { get; set; }

        public string mode_flag { get; set; }

        public string chq_reject { get; set; }

    }

}