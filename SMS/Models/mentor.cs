using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mentor_header
    {
        [Required]
        public string fin_id{ get; set; }

        [Required]
        [Display(Name = "Mentoring No")]
        public int mentor_no { get; set; }

        [Required]
        [Display(Name = "Mentoring Assign Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime mentor_date { get; set; }

        [Required]
        [Display(Name = "Admission No")]
        public int sr_num{ get; set; }

        [Required]
        public int class_id{ get; set; }

       
        [Display(Name = "Student Name")]
        public string std_name { get; set; }

        [Display(Name = "Mentor Name")]
        public string mentor_name { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Required]
        [Display(Name = "Working area")]
        public string problem { get; set; }

        [Required]
        public int mentor_id{ get; set; }


        
        [Display(Name = "Deal Line")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime dead_line { get; set; }
    }

    public class mentor_detail
    {
        [Required]
        public string fin_id { get; set; }

        [Required]
        public int serial_no { get; set; }

        [Required]
        [Display(Name = "Mentoring No")]
        public int mentor_no { get; set; }

        [Required]
        [Display(Name = "Mentoring Assign Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime mentor_date { get; set; }

        [Required]
        public int mentor_id { get; set; }

        [Required]
        [Display(Name = "Mentor Observation")]
        public string mentor_observation{ get; set; }

        [Required]
        [Display(Name = "Parents Observation")]
        public string parents_observation { get; set; }

        [Required]
        [Display(Name = "Observation Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime observation_date { get; set; }
    }
}