using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class rep_Class_Wise_Std_List
    {
        [Display(Name = "Select Class")]
        [Required]
        public int class_id { get; set; }

        [Display(Name = "Select Section")]
        [Required]
        public int section_id { get; set; }

        [Display(Name = "Select Session")]
        [Required]
        public string session { get; set; }

        [Display(Name = "Format")]
        public string format { get; set; }
    }
}