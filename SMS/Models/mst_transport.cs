using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("mst_section")]
    public class mst_transport
    {
        [Key]
        public int pickup_id { get; set; }

        [Required]
        [Display(Name = "Pickup Point")]
        public String pickup_point { get; set; }

        [Required]
        [Display(Name = "Transport Fees")]
        public decimal transport_fees { get; set; }

        [Display(Name = "Transport Number")]
        public String transport_number { get; set; }

        [Display(Name = "April")]
        public bool bl_apr { get; set; }

        [Display(Name = "May")]
        public bool bl_may { get; set; }

        [Display(Name = "Jun")]
        public bool bl_jun { get; set; }

        [Display(Name = "July")]
        public bool bl_jul { get; set; }

        [Display(Name = "August")]
        public bool bl_aug { get; set; }

        [Display(Name = "September")]
        public bool bl_sep { get; set; }

        [Display(Name = "October")]
        public bool bl_oct { get; set; }

        [Display(Name = "November")]
        public bool bl_nov { get; set; }

        [Display(Name = "December")]
        public bool bl_dec { get; set; }

        [Display(Name = "January")]
        public bool bl_jan { get; set; }

        [Display(Name = "February")]
        public bool bl_feb { get; set; }

        [Display(Name = "March")]
        public bool bl_mar { get; set; }
    }
}