using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_class_coscholastic
    {
         [Key]
        public string session { get; set; }

        [Key]
        public int class_id { get; set; }

        [Key]
        public int co_scholastic_id { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Display(Name = "Co-Scholastic Name")]
        public string co_scholastic_name { get; set; }
    }
}