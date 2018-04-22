using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("[out_standing]")]
    public class out_standing
    {
      

        [Key]
        public int serial { get; set; }

        [Key]
        public string fin_id { get; set; }

        [Key]
        public DateTime dt_date { get; set; }

        [Display(Name = "Frequency")]
        public String frequency { get; set; }

        //public int Rank { get; set; }

        [Required]
        public int acc_id { get; set; }

        [Required]
        public string acc_name { get; set; }

        [Required]
        public int sr_number { get; set; }

        [Required]
        public decimal outstd_amount { get; set; }

        public decimal rmt_amount { get; set; }

        public string narration { get; set; }

        public int reg_num { get; set; }

        public int receipt_no { get; set; }

        public DateTime receipt_date { get; set; }

        public int month_no { get; set; }

        public string month_name { get; set; }

        public bool clear_flag { get; set; }

    }
}