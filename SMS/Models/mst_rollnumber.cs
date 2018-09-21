using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_rollnumber
    {
        [Display (Name="Session" )]
        public string session { get; set; }

        [Display(Name = "Admission No")]
        public int sr_num { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }

        [Display(Name = "Class")]
        public int class_id { get; set; }

        [Display(Name = "Section")]
        public int section_id { get; set; }

        [Display(Name = "Roll Number")]
        public int roll_number { get; set; }
    }
}