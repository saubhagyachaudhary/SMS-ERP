using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("mst_acc_head")]
    public class mst_acc_head
    {
       
       
            [Key]
            public int acc_id { get; set; }

            [Required]
            [Display(Name = "Account Name")]
            public String acc_name { get; set; }

            [Required]
            [Display(Name = "Nature")]
            public String nature { get; set; }


    }
}