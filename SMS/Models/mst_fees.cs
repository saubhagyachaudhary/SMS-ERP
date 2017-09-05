using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("mst_fees")]
    public class mst_fees
    {
        [Key]
        public int class_id { get; set; }

        [Key]
        public int acc_id { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Display(Name = "Account Head")]
        public string acc_name { get; set; }

        [Required]
        [Display(Name = "Fees Amount")]
        public decimal fees_amount { get; set; }

        [Display(Name = "One Time")]
        public bool bl_onetime { get; set; }

        [Display(Name = "April")]
        public bool bl_apr { get; set; }

        [Display(Name = "May")]
        public bool bl_may { get; set; }

        [Display(Name = "Jun")]
        public bool bl_jun { get; set; }

        [Display(Name = "July")]
        public bool bl_jul { get; set; }

        [Display(Name = "August")]
        public bool bl_aug { get; set; }

        [Display(Name = "September")]
        public bool bl_sep { get; set; }

        [Display(Name = "October")]
        public bool bl_oct { get; set; }

        [Display(Name = "November")]
        public bool bl_nov { get; set; }

        [Display(Name = "December")]
        public bool bl_dec { get; set; }

        [Display(Name = "January")]
        public bool bl_jan { get; set; }

        [Display(Name = "February")]
        public bool bl_feb { get; set; }

        [Display(Name = "March")]
        public bool bl_mar { get; set; }

    }
}