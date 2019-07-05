using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_subject_parameters
    {
        [Display(Name = "Session")]
        [Key]
        public string session { get; set; }

        [Display(Name = "Parameter Name")]
        [Key]
        public int parameter_id { get; set; }

        [Display(Name = "Parameter Name")]
        [Key]
        public string parameter_name { get; set; }

        [Display(Name = "Subject Name")]
        [Key]
        public int subject_id { get; set; }

        [Display(Name = "Subject Name")]
        [Key]
        public string subject_name { get; set; }
    }
}