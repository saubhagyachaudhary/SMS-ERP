using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("mst_section")]
    public class mst_section
    {
        [Key]
        public int section_id { get; set; }

        [Required]
        public int class_id { get; set; }

        
        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Required]
        [Display(Name = "Section Name")]
        public string Section_name { get; set; }

    }
}