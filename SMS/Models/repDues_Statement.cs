using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class repDues_Statement
    {
        [Display(Name = "Class Name")]
        public int section_id { get; set; }

        [Display(Name = "Pickup Point")]
        public int pickup_id { get; set; }

        [Display(Name = "Amount")]
        [Required]
        public decimal amount { get; set; }

        public string operation { get; set; }

    }
}