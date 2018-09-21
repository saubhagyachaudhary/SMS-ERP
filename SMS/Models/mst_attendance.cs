using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_attendance
    {
        
        public int user_id { get; set; }

        [Display(Name = "Faculty")]
        public string faculty_name { get; set; }

        [Display(Name = "Finalizer")]
        public string finalizer_name { get; set; }

        [Required(ErrorMessage = "Error: Must select class")]
        public int class_id { get; set; }
        [Display(Name ="Class")]

        public string class_name { get; set; }

        [Required(ErrorMessage = "Error: Must select section")]
        public int section_id { get; set; }

        [Display(Name = "Section")]
        public string section_name { get; set; }

        public int finalizer_user_id { get; set; }
    }
}