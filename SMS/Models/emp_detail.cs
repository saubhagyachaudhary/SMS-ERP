using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SMS.Models
{
    public class emp_detail
    {
        [Display(Name = "Emp Id")]
        public int user_id { get; set; }

        [Display(Name = "First Name")]
        public string first_name{ get; set; }

        [Display(Name = "Last Name")]
        public string last_name { get; set; }

        [Display(Name = "First Name")]
        public string user_name { get; set; }

        [Display(Name = "Email Id")]
        public string email { get; set; }

        [Display(Name = "Contact")]
        public string contact { get; set; }

        [Display(Name = "Contact 2")]
        public string contact1 { get; set; }

        [Display(Name = "Contact 3")]
        public string contact2 { get; set; }

        [Display(Name = "Address")]
        public string address { get; set; }

        [Display(Name = "Address Line 1")]
        public string address1 { get; set; }

        [Display(Name = "Address Line 2")]
        public string address2 { get; set; }

        [Display(Name = "District")]
        public string district { get; set; }

        [Display(Name = "State")]
        public string state { get; set; }

        [Display(Name = "Country")]
        public string country { get; set; }

        [Display(Name = "Pincode")]
        public string pincode { get; set; }

        [Display(Name = "Father's Name")]
        public string FatherName { get; set; }

        [Display(Name = "Mother's Name")]
        public string MotherName { get; set; }

        [Display(Name = "Date of Birth")]
        public DateTime dob { get; set; }

        [Display(Name = "Sex")]
        public string sex { get; set; }

        [Display(Name = "Qualification")]
        public string education{ get; set; }

        [Display(Name = "Date of Joining")]
        public DateTime doj { get; set; }

        [Display(Name = "EPF UAN No")]
        public string epf_no { get; set; }

        [Display(Name = "Aadhar No")]
        public string aadhaar_no { get; set; }

        [Display(Name = "Pan No")]
        public string pan_no { get; set; }

        [Display(Name = "Bank Name")]
        public string bank_name { get; set; }

        [Display(Name = "Bank A/C No")]
        public string acc_no { get; set; }

        [Display(Name = "IFSC Code")]
        public string ifsc_no { get; set; }

        [Display(Name = "Bank Branch")]
        public string bank_branch { get; set; }

        [Display(Name = "Designation")]
        public string designation { get; set; }

        [Display(Name = "Bio Matric No")]
        public string bioMatricNo { get; set; }

        [Display(Name = "Active Employee")]
        public bool emp_active { get; set; }
    }
}