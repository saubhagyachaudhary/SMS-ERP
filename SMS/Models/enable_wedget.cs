using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class enable_wedget
    {

        public int user_id { get; set; }

        public string wedget_id { get; set; }

        public bool active { get; set; }

        [Display(Name = "Wedget Name")]
        public string wedget_name { get; set; }

        [Display(Name = "User Name")]
        public string username { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "First Name")]
        public string LastName { get; set; }

        [Display(Name = "Group")]
        public string group { get; set; }

        [Display(Name = "Description")]
        public string description { get; set; }

    }
}