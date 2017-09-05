using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("mst_fin")]
    public class mst_fin
    {
        [Key]
        [Display(Name = "FY Name")]
        public String fin_id { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "FY Start Date")]
        public DateTime fin_start_date { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "FY End Date")]
        public DateTime fin_end_date { get; set; }

        [Required]
        [Display(Name = "FY Status")]
        public String fin_close { get; set; }


    }
}