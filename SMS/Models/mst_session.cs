using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("mst_session")]
    public class mst_session
    {
        [Key]
        [Display(Name = "Session Name")]
        public String session { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Session Start Date")]
        public DateTime session_start_date { get; set; }

        [Required]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        [Display(Name = "Sesssion End Date")]
        public DateTime session_end_date { get; set; }

        [Required]
        [Display(Name = "Admission Open")]
        public String session_active { get; set; }

    }
}