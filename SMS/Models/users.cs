using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class users
    {
        public int user_id { get; set; }
        [Required(ErrorMessage ="Please Provide Username",AllowEmptyStrings =false)]
        public string username { get; set; }
        [Required(ErrorMessage = "Please Provide Password", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string password { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        public HttpPostedFileBase profilePicture { get; set; }
    }
}