using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    [Table("sr_register")]
    public class sr_register
    {
        [Key]
        [Required]
        [Display(Name = "Sr Number")]
        public virtual int sr_number { get; set; }
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
        [Display(Name = "Contact Father")]
        public virtual string std_contact1 { get; set; }
        [Display(Name = "Contact Mother")]
        public virtual string std_contact2 { get; set; }
        [Display(Name = "Email")]
        public virtual string std_email { get; set; }
        [Display(Name = "Father Occupation")]
        public virtual string std_father_occupation { get; set; }
        [Display(Name = "Mother Occupation")]
        public virtual string std_mother_occupation { get; set; }
        [Display(Name = "Blood Group")]
        public virtual string std_blood_gp { get; set; }
        [Display(Name = "House Hold Income")]
        public virtual string std_house_income { get; set; }
        
        [Required]
        [Display(Name = "Nationality")]
        public virtual string std_nationality { get; set; }

        [Required]
        [Display(Name = "Category")]
        public virtual string std_category { get; set; }

        [Required]
        [Display(Name = "Cast")]
        public virtual string std_cast { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime std_dob { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual String std_dob_str { get; set; }


        [Required]
        [Display(Name = "Sex")]
        public virtual string std_sex { get; set; }

        [Display(Name = "Last School")]
        public virtual string std_last_school { get; set; }

        [Display(Name = "Admission Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public virtual DateTime std_admission_date { get; set; }

        [Display(Name = "Admission Date")]
        public virtual string std_admission_date_str { get; set; }

        [Display(Name = "Aadhar Number")]
        public virtual string std_aadhar { get; set; }

        [Required]
        [Display(Name = "Admission Class")]
        public virtual string std_admission_class { get; set; }
        
        [Display(Name = "Section Name")]
        public string section_name { get; set; }

        [Display(Name = "Class Name")]
        public string class_name { get; set; }

        [Required]
        [Display(Name = "Class Name")]
        public int class_id { get; set; }

        [Display(Name = "Active")]
        public bool active { get; set; }

        [Display(Name = "House")]
        public String std_house { get; set; }


        [Required]
        [ForeignKey("mst_section")]
        [Display(Name = "Avail Transport")]
        public virtual int std_section_id { get; set; }
        
        [Display(Name = "Remark")]
        public virtual string std_remark { get; set; }
        [Display(Name = "Avail Transport")]
        public String pickup_point { get; set; }

        [ForeignKey("mst_transport")]
        [Required]
        [Display(Name = "Avail Transport")]
        public virtual int? std_pickup_id { get; set; }

        [Display(Name = "Transport from")]
        public int from_month_no { get; set; }

        [Display(Name = "Active")]
        public virtual string std_active { get; set; }
        [Required]
        [ForeignKey("std_registration")]
        public virtual string adm_session { get; set; }
        [Required]
        [ForeignKey("std_registration")]
        public virtual int reg_no { get; set; }
        [Required]
        public virtual DateTime reg_date { get; set; }

        [Display(Name = "Admission Fees")]
        public virtual decimal fees_amount { get; set; }

        [Display(Name = "NSO Date")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime nso_date { get; set; }

        public IEnumerable<sr_register> sr_regi { get; set; }

        [Display(Name = "Admission form pdf link")]
        public string adm_form_link { get; set; }

    }
}