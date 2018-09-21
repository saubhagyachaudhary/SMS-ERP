using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_discipline
    {
        [Key]
        public int discipline_id { get; set; }

        [Key]
        public string session { get; set; }


        [Display(Name ="Discipline Area")]
        public string discipline_name { get; set; }
    }
}