using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class reptransportList
    {
        [Display(Name ="Pickup Point")]
        public List<pickup_list> pickup_list { get; set; }

        public int[] pickup { get; set; }

    }

    public class reptransportList_DuesList
    {
        [Display(Name = "Pickup Point")]
        public List<pickup_list> pickup_list { get; set; }

        public int[] pickup { get; set; }

        public decimal amount { get; set; }

        public string operation { get; set; }
    }

}