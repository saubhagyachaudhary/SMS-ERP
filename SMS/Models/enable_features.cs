using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class enable_features
    {

        public int user_id { get; set; }

        public string feature_id { get; set; }

        public bool active { get; set; }

        [Display(Name = "Feature Name")]
        public string feature_name { get; set; }

        [Display(Name = "User Name")]
        public string username { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "First Name")]
        public string LastName { get; set; }

    }
}