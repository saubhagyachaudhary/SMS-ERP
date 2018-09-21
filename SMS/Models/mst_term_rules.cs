using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_term_rules
    {

        [Key]
        [Required]
        [Display(Name = "Session")]
        public string session { get; set; }

        [Key]
        public int evaluation_id { get; set; }

        [Key]
        [Required]
        [Display(Name = "Term Name")]
        public int term_id { get; set; }

        
        [Display(Name = "Term Name")]
        [Required]
        public string term_name { get; set; }

        [Key]
        [Required]
        [Display(Name = "Class Name")]
        public int class_id { get; set; }
        
        [Display(Name = "Class Name")]
        [Required]
        public string class_name { get; set; }

        [Display(Name = "Evaluation Name")]
        [Required]
        public string evaluation_name { get; set; }

        [Display(Name = "Exam Name 1")]
        [Required]
        public int exam_id1 { get; set; }

        
        [Display(Name = "Exam Name 1")]
        [Required]
        public string exam_name1 { get; set; }

        
        [Display(Name = "Exam Name 2")]
        public int exam_id2 { get; set; }

        
        [Display(Name = "Exam Name 2")]
        public string exam_name2 { get; set; }

        
        [Display(Name = "Rule")]
        [Required]
        public string rule { get; set; }

    }
}