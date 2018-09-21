using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_subject
    {
        [Key]
        public int subject_id { get; set; }

        [Key]
        public string session { get; set; }


        [Required]
        [Display(Name = "Subject Name")]
        public string subject_name { get; set; }
    }
}