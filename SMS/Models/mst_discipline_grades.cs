using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_discipline_grades
    {
        [Key]
        [Display(Name = "Session")]
        public string session { get; set; }

        [Key]
        [Display(Name = "Admission No")]
        public int sr_num { get; set; }

        [Key]
        [Display(Name = "Term Name")]
        public int term_id { get; set; }

        [Key]
        [Display(Name = "Discipline Area")]
        public int discipline_id { get; set; }

        public int class_id { get; set; }

        public int section_id { get; set; }

        public int user_id { get; set; }

        [Display(Name = "Grade")]
        public string grade { get; set; }

        [Display(Name = "Roll Number")]
        public int roll_no { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }

        [Display(Name = "Discipline Area")]
        public string discipline_name { get; set; }
    }
}