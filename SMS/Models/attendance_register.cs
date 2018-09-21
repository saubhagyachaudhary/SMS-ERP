using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class attendance_register
    {
        [Display(Name = "Session")]
        public string session { get; set; }

        public int user_id { get; set; }
        
        [Display(Name = "Attendance Date")]
        public DateTime att_date { get; set; }

        public int class_id { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        public int section_id { get; set; }

        [Display(Name = "Class Name")]
        public string section_name { get; set; }

        [Display(Name = "Admission No")]
        public int sr_num { get; set; }

        [Display(Name = "Attendance")]
        public bool attendance { get; set; }

        [Display(Name = "Roll Number")]
        public int roll_no { get; set; }

        [Display(Name = "Student Name")]
        public string std_name { get; set; }

        public int date_num { get; set; }

        public string month_name { get; set; }

        public int absent{ get; set; }

        public int present { get; set; }

        public string class_teacher { get; set; }

        public string finalizer { get; set; }

    }
}