using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_exam_marks
    {
        [Display(Name="Session")]
        [Required]
        public string session { get; set; }

        [Display(Name = "Admission No")]
        [Required]
        public int sr_num { get; set; }

        [Display(Name = "Exam Name")]
        [Required]
        public int exam_id { get; set; }

        [Display(Name = "Exam Name")]
        [Required]
        public string exam_name { get; set; }

        [Display(Name = "Class Name")]
        [Required]
        public int class_id { get; set; }

        [Display(Name = "Class Name")]
        [Required]
        public string class_name { get; set; }

        [Display(Name = "Section Name")]
        [Required]
        public int section_id { get; set; }

        [Display(Name = "Section Name")]
        [Required]
        public string section_name { get; set; }

        [Display(Name = "Subject Name")]
        [Required]
        public int subject_id { get; set; }

        [Display(Name = "Subject Name")]
        [Required]
        public string subject_name { get; set; }

        [Required]
        public int user_id { get; set; }

        [Display(Name = "Subject Faculty")]
        [Required]
        public int marks_assigned_user_id { get; set; }

        [Display(Name = "Subject Faculty")]
        [Required]
        public string subject_faculty { get; set; }

        [Display(Name = "Marks")]
        [Required]
        public decimal marks { get; set; }

        [Display(Name = "Present")]
        public bool present { get; set; }


        [Display(Name = "Roll Number")]
        public int roll_no { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }
    }
}