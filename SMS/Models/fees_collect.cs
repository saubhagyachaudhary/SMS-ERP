using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class fees_collect
    {
        
        [Display(Name = "Student Name")]
        public String std_Name { get; set; }

        
        [Display(Name = "Admission Number")]
        public int sr_num { get; set; }

        [Display(Name = "Registration Number")]
        public int reg_num { get; set; }

        [Display(Name = "Father Name")]
        public String std_father_name { get; set; }

        [Display(Name = "Mother Name")]
        public String std_mother_name { get; set; }

        [Display(Name = "Primary Contact")]
        public String std_contact { get; set; }

        [Display(Name = "Email Id")]
        public String std_Email { get; set; }

        [Display(Name = "Class")]
        public String std_Class { get; set; }

        [Display(Name = "Section")]
        public String std_Section { get; set; }

        [Display(Name = "Pickup Point")]
        public String std_Pickup_point { get; set; }

        public IEnumerable<sr_register> list { get; set; }

        [Display(Name = "Select Class")]
        public int section_id { get; set; }

        [Display(Name = "Aadhar Number")]
        public string std_aadhar { get; set; }

        [Display(Name = "Session")]
        public string session { get; set; }


    }
}