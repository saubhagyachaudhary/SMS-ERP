using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class repFeesStatement
    {
        [Display(Name = "From Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime fromDt { get; set; }

        [Display(Name = "To Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime toDt { get; set; }

        [Display(Name = "Mode")]
        public string mode { get; set; }

        [Display(Name = "Type")]
        public string detailed { get; set; }

        [Display(Name = "Session")]
        public string session { get; set; }

        [Display(Name = "Account Name")]
        public string acc_id { get; set; }


    }
}