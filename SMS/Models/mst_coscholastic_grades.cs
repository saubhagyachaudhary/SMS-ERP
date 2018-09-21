using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_coscholastic_grades
    {
        [Key]
        [Display(Name ="Session")]
        public string session { get; set; }

        [Key]
        [Display(Name = "Admission No")]
        public int sr_num { get; set; }

        [Key]
        [Display(Name = "Term Name")]
        public int term_id { get; set; }

        [Key]
        [Display(Name = "Co-Scholastic Area")]
        public int co_scholastic_id { get; set; }

        [Display(Name = "Class Name")]
        public int class_id { get; set; }

        [Display(Name = "Section Name")]
        public int section_id { get; set; }

        public int user_id { get; set; }

        [Display(Name ="Grade")]
        public string grade { get; set; }

        [Display(Name = "Roll Number")]
        public int roll_no { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }

        [Display(Name = "Co-Scholastic Area")]
        public string co_scholastic_name { get; set; }

    }
}