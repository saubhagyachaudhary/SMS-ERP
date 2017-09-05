using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("sr_register")]
    public class std_registration
    {
        [Key]
        [Required]
        [Display(Name = "Session")]
        public virtual string session { get; set; }

        [Key]
        [Required]
        [Display(Name = "Reg No")]
        public virtual int reg_no { get; set; }

        [Required]
        [Display(Name = "Reg Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime reg_date { get; set; }


        [Required]
        [Display(Name = "First Name")]
        public virtual string std_first_name { get; set; }
        [Display(Name = "Last Name")]
        public virtual string std_last_name { get; set; }
        [Required]
        [Display(Name = "Father Name")]
        public virtual string std_father_name { get; set; }
        [Required]
        [Display(Name = "Mother Name")]
        public virtual string std_mother_name { get; set; }
        [Required]
        [Display(Name = "Address")]
        public virtual string std_address { get; set; }
        [Display(Name = "Address Line 1")]
        public virtual string std_address1 { get; set; }
        [Display(Name = "Address Line 2")]
        public virtual string std_address2 { get; set; }
        [Required]
        [Display(Name = "District")]
        public virtual string std_district { get; set; }
        [Required]
        [Display(Name = "State")]
        public virtual string std_state { get; set; }
        [Required]
        [Display(Name = "Country")]
        public virtual string std_country { get; set; }
        [Required]
        [Display(Name = "Pincode")]
        public virtual string std_pincode { get; set; }


        [Required]
        [Display(Name = "Contact")]
        public virtual string std_contact { get; set; }
        [Display(Name = "Contact 1")]
        public virtual string std_contact1 { get; set; }
        [Display(Name = "Contact 2")]
        public virtual string std_contact2 { get; set; }
        [Display(Name = "Email")]
        public virtual string std_email { get; set; }

        [Required]
        [Display(Name = "Reg Class")]
        public virtual int std_class_id { get; set; }

        [Required]
        [Display(Name = "Reg Class")]
        public virtual string class_name { get; set; }

        [Required]
        [Display(Name = "Registration Fees")]
        public virtual decimal fees_amount { get; set; }

        

    }
}