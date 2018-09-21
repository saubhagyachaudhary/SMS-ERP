using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_term
    {
        [Key]
        [Display(Name = "Session")]
        public string session { get; set; }

        [Key]
        public int term_id { get; set; }

        [Required]
        [Display(Name = "Term Name")]
        public string term_name { get; set; }
    }
}