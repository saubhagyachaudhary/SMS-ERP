using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("std_discount")]
    public class std_discount
    {
        [Key]
        [Display(Name = "Session")]
        public string session { get; set; }

        [Key]
        [Display(Name = "Admission Number")]
        public int sr_num { get; set; }

        [Key]
        public int acc_id { get; set; }

        [Display(Name = "Account head")]
        public string account_name { get; set; }

        [Display(Name = "Student Name")]
        public string stdName { get; set; }

        [Display(Name = "Student Class")]
        public string stdclass { get; set; }

        [Display(Name = "Discount Percent")]
        public int percent { get; set; }

        [Display(Name = "Discount Percent")]
        public String per { get; set; }

        [Display(Name = "Exempt")]
        public bool bl_exempt { get; set; }

        [Display(Name = "April")]
        public bool bl_apr { get; set; }

        [Display(Name = "May")]
        public bool bl_may { get; set; }

        [Display(Name = "June")]
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

        [Display(Name = "Remarks")]
        public string std_remarks { get; set; }


    }
}