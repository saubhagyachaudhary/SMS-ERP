using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_class_subject
    {
        [Key]
        public string session { get; set; }

        [Key]
        public int class_id { get; set; }

        [Key]
        public int subject_id { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Display(Name = "Subject Name")]
        public string subject_name { get; set; }
    }
}