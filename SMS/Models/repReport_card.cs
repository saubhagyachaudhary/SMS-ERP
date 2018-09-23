using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class repReport_card
    {
        
        

        [Display(Name = "Select Class")]
        public int class_id { get; set; }

        [Display(Name = "Select Section")]
        public int section_id { get; set; }

        [Display(Name = "Select Session")]
        public string session { get; set; }
    }
}