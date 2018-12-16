using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class change_password
    {
        [Required(ErrorMessage = "Please Provide Old Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Old Password")]
        public string old_password { get; set; }

        [Required(ErrorMessage = "Please Provide New Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        public string new_password { get; set; }

        [Required(ErrorMessage = "Please Provide Confirm Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        public string confirm_password { get; set; }

        public int user_id { get; set; }

        public string username { get; set; }


    }
}