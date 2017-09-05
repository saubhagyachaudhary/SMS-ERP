using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class uncleared_cheque
    {

        /*[Display(Name = "FY")]
        public String fin_id { get; set; }

        [Display(Name = "Rcpt No")]
        public int receipt_no { get; set; }

        [Display(Name = "Rcp Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime receipt_date { get; set; }

        public int acc_id { get; set; }

        [Display(Name = "Fee Desc")]
        public String fees_name { get; set; }


       

        [Display(Name = "Std Name")]
        public String std_name { get; set; }

        [Display(Name = "Class")]
        public String class_name { get; set; }*/

        [Display(Name = "Amount")]
        public decimal amount { get; set; }

        [Display(Name = "Bank Name")]
        public String bnk_name { get; set; }

        [Display(Name = "Inst No")]
        public String chq_no { get; set; }

        [Display(Name = "Inst Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime chq_date { get; set; }

        
        public string str_date { get; set; }

        [Display(Name = "Inst Status")]
        public bool clear_flag { get; set; }

        [Display(Name = "Bank Charges")]
        public decimal bnk_charges { get; set; }

        public bool check { get; set; }

        [Display(Name = "Narration")]
        public String narration { get; set; }

       
        public String chq_reject { get; set; }

        public int serial { get; set; }

        public int sr_number { get; set; }

        public int reg_no { get; set; }
    }
}