using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_exam
    {
        [Key]
        public int exam_id { get; set; }

        [Key]
        public string session { get; set; }

        [Required]
        [Display(Name = "Exam Name")]
        public string exam_name { get; set; }

        [Required]
        [Display(Name = "Maximum Number")]
        public int max_no{ get; set; }

        [Required]
        [Display(Name = "Convert To")]
        public int convert_to { get; set; }
    }
}