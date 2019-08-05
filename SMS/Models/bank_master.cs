using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class bank_master
    {
        [Key]
        [Display(Name = "Bank Name")]
        public int bank_id { get; set; }

        [Required]
        [Display(Name = "Bank Name")]
        public string bank_name { get; set; }
    }
}