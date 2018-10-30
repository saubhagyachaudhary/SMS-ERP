using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class teacher_exam_remark
    {
        [Display(Name = "Session")]
        public string session { get; set; }

        [Display(Name = "Term")]
        public int term_id { get; set; }

        [Display(Name = "Term Name")]
        public string term_name { get; set; }

        [Display(Name = "Class")]
        public int class_id { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Display(Name = "Section")]
        public int section_id { get; set; }

        [Display(Name = "Section Name")]
        public string section_name { get; set; }

        [Display(Name = "Admission No")]
        public int sr_number { get; set; }

        [Display(Name = "Remark")]
        public string remark { get; set; }

        [Display(Name = "Roll No")]
        public int roll_no { get; set; }

        [Display(Name = "User ID")]
        public int user_id { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }

    }
}