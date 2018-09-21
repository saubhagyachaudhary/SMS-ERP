using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class users
    {
        [Display(Name = "User Id")]
        public int user_id { get; set; }

        [Required(ErrorMessage ="Please Provide Username",AllowEmptyStrings =false)]
        [Display(Name = "Username")]
        public string username { get; set; }

        [Required(ErrorMessage = "Please Provide Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string password { get; set; }

        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string lastname { get; set; }

        public string roles { get; set; }

        public string features { get; set; }

        public HttpPostedFileBase profilePicture { get; set; }

        public string ReturnUrl { get; set; }

    }

    public class ddemp_list
    {
        public string id { get; set; }

        public int user_id { get; set; }
    }
}