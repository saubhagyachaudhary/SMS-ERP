using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("mst_class")]
    public class mst_class
    {
            [Key]
            public int class_id { get; set; }

            [Required]
            [Display(Name = "Class Name")]
            public string class_name { get; set; }
        
    }
}