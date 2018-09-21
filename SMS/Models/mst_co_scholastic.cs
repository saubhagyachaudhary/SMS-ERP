using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class mst_co_scholastic
    {
        [Key]
        public int co_scholastic_id { get; set; }

        [Key]
        public string session { get; set; }

        [Display(Name ="CO-Scholastic Area")]
        public string co_scholastic_name { get; set; }
    }
}