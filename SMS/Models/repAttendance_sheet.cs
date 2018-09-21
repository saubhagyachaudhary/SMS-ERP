using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class repAttendance_sheet
    {
        [Display(Name = "Select Class")]
        public int section_id { get; set; }

        [Display(Name = "Select Month")]
        public int month_no { get; set; }

        [Display(Name = "Select Session")]
        public string session { get; set; }
    }
}